using ASCE7_10Library;
using DrawingHelpersLibrary;
using System;
using System.Windows;
using System.Windows.Media;
using WindCalculator.ViewModel;

namespace WindCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The WindProvision view model for this graphic.
        /// </summary>
        public WindViewModel WindVM { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            OnUserCreate();
        }

        /// <summary>
        /// Routine that runs everytime it is called (once per frame?  after user input?)
        /// </summary>
        private void OnUserUpdate()
        {
            Draw();
        }

        /// <summary>
        /// Routine that runs only once when the program is first executed
        /// </summary>
        private void OnUserCreate()
        {
            ExposureCategories exp = ExposureCategories.B;
            double V = 115;   // mph

            double theta = 25; // roof slope

            // Create a building object
            BuildingInfo bldg = new BuildingInfo(85, 100, 100, 0, RiskCategories.II);
            WindProvisions wind_prov = new WindProvisions(V, bldg, exp);

            WindVM = new WindViewModel(bldg, wind_prov);
        }

        /// <summary>
        /// Event that runs after the window is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            OnUserUpdate();
        }

        protected void Draw()
        {
            double SCALE_FACTOR_HORIZ = 0.6 * MainCanvas.Width / WindVM.Bldg.L;
            double SCALE_FACTOR_VERT = 0.6 * MainCanvas.Height / WindVM.Bldg.H;
            double SCALE_FACTOR = Math.Min(SCALE_FACTOR_HORIZ, SCALE_FACTOR_VERT);
            //double SCALE_FACTOR = 1.0;

            // Independent scale factor for the pressure diagram
            double PRESSURE_SCALE_FACTOR = 0.4 * SCALE_FACTOR;

            // Center point of current canvas
            double x_center = MainCanvas.Width * 0.5;
            double y_center = MainCanvas.Height * 0.5;

            // Ground location on canvas
            double y_ground = y_center + WindVM.Bldg.H * 0.5 * SCALE_FACTOR;
            //double y_ground = MainCanvas.Height * 0.8;

            double x_ww_grd = x_center - (WindVM.Bldg.L * 0.5) * SCALE_FACTOR;
            double y_ww_grd = y_ground;
            double x_ww_h = x_ww_grd;
            double y_ww_h = y_ground - WindVM.Bldg.H * SCALE_FACTOR;

            double x_lw_grd = x_center + (WindVM.Bldg.L * 0.5) * SCALE_FACTOR;
            double y_lw_grd = y_ground;
            double x_lw_h =  x_lw_grd;
            double y_lw_h = y_ground - WindVM.Bldg.H * SCALE_FACTOR;

            double x_ww_15 = 0.5 * (x_ww_grd + x_ww_h);
            double y_ww_15 = y_ww_grd - (15.0 / WindVM.Bldg.H) * (y_ww_grd - y_ww_h);
            // Find coordinate of 15' mark
            if (WindVM.Bldg.H <= 15)
            {
                x_ww_15 = x_ww_h;
                y_ww_15 = y_ww_h;
            }

            double x_q0_ww = x_ww_grd - WindVM.Wind_Prov.Q_0 *PRESSURE_SCALE_FACTOR;
            double y_q0_ww = y_ww_grd;
            double x_q15_ww = x_ww_15 - WindVM.Wind_Prov.Q_15 * PRESSURE_SCALE_FACTOR;
            double y_q15_ww = y_ww_15;
            double x_qh_ww = x_ww_h - WindVM.Wind_Prov.Q_H *PRESSURE_SCALE_FACTOR;
            double y_qh_ww = y_ww_h;

            // Draw the centerlines of the canvas
            DrawingHelpers.DrawLine(MainCanvas, x_center, 0, x_center, 2.0*y_center, Brushes.BlueViolet, 1, Linetypes.LINETYPE_PHANTOM);
            DrawingHelpers.DrawLine(MainCanvas, 0, y_center, 2.0 * x_center, y_center, Brushes.BlueViolet, 1, Linetypes.LINETYPE_PHANTOM);

            // Draw the WW wall object
            DrawingHelpers.DrawLine(MainCanvas, x_ww_grd, y_ww_grd, x_ww_h, y_ww_h, Brushes.Black, 3, Linetypes.LINETYPE_SOLID);

            // Draw the Roof object line
            DrawingHelpers.DrawLine(MainCanvas, x_ww_h, y_ww_h, x_lw_h, y_lw_h, Brushes.Black, 3, Linetypes.LINETYPE_SOLID);

            // Draw the LW object line
            DrawingHelpers.DrawLine(MainCanvas, x_lw_h, y_lw_h, x_lw_grd, y_lw_grd, Brushes.Black, 3, Linetypes.LINETYPE_SOLID);

            // Draw the WW Pressure Prodile.
            // q0 pressure line
            DrawingHelpers.DrawLine(MainCanvas, x_q0_ww, y_q0_ww, x_ww_grd, y_ww_grd, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
            DrawingHelpers.DrawText(MainCanvas, x_q0_ww, y_q0_ww, 0, WindVM.Wind_Prov.Q_0.ToString(), Brushes.Blue, 8);

            // Qh pressure line
            DrawingHelpers.DrawLine(MainCanvas, x_qh_ww, y_qh_ww, x_ww_h, y_ww_h, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
            DrawingHelpers.DrawText(MainCanvas, x_qh_ww, y_qh_ww, 0, WindVM.Wind_Prov.Q_H.ToString(), Brushes.Blue, 8);

            // Draw lines between pressure points
            DrawingHelpers.DrawLine(MainCanvas, x_q0_ww, y_q0_ww, x_q15_ww, y_q15_ww, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
            DrawingHelpers.DrawLine(MainCanvas, x_q15_ww, y_q15_ww, x_qh_ww, y_qh_ww, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);

            // show the dimensions
            if(WindVM.Bldg.H > 15)
            {
                DrawingHelpers.DrawVerticalDimension_Left(MainCanvas, 30, 0.2, 5, x_q15_ww, y_q15_ww, x_q0_ww, y_q0_ww, "15'");

                // Q15 pressure line
                DrawingHelpers.DrawLine(MainCanvas, x_q15_ww, y_q15_ww, x_ww_15, y_ww_15, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
                DrawingHelpers.DrawText(MainCanvas, x_q15_ww, y_q15_ww, 0, WindVM.Wind_Prov.Q_15.ToString(), Brushes.Blue, 8);
            }

            DrawingHelpers.DrawHorizontalDimension_Below(MainCanvas, 30, 0.2, 5, x_ww_grd, y_ww_grd, x_lw_grd, y_lw_grd, WindVM.Bldg.L + "'");
            DrawingHelpers.DrawVerticalDimension_Right(MainCanvas, 0.5 * (x_lw_grd - x_ww_grd), 0.2, 5, x_ww_h, y_ww_h, x_ww_grd, y_ww_grd, WindVM.Bldg.H + "'");
        }
    }
}
