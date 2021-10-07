using ASCE7_10Library;
using DrawingHelpersLibrary;
using System;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WindCalculator.ViewModel;

namespace WindCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const double PRESSURE_TEXT_HT = 10;
        public const double STRUCTURE_DIM_TEXT_HT = 20;


        double SCALE_FACTOR_HORIZ;
        double SCALE_FACTOR_VERT;
        double SCALE_FACTOR;
        //double SCALE_FACTOR = 1.0;

        // Independent scale factor for the pressure diagram
        double PRESSURE_SCALE_FACTOR;
        /// <summary>
        /// The WindProvision view model for this graphic.
        /// </summary>
        public WindViewModel WindVM { get; set; }

        // BuildingVM for each Canvas
        public BuildingViewModel BuildingVM_1 { get; set; }
        public BuildingViewModel BuildingVM_2 { get; set; }
        public BuildingViewModel BuildingVM_3 { get; set; }
        public BuildingViewModel BuildingVM_4 { get; set; }


        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            OnUserCreate(MainCanvas);
            OnUserCreate(Canvas2);
            OnUserCreate(Canvas3);
            OnUserCreate(Canvas4);

        }

        /// <summary>
        /// Routine that runs everytime it is called (once per frame?  after user input?)
        /// </summary>
        private void OnUserUpdate()
        {
            Draw(MainCanvas, BuildingVM_1);
            Draw(Canvas2, BuildingVM_2);
            Draw(Canvas3, BuildingVM_3);
            Draw(Canvas4, BuildingVM_4);
        }

        /// <summary>
        /// Routine that runs only once when the program is first executed
        /// </summary>
        private void OnUserCreate(Canvas canvas)
        {
            ExposureCategories exp = ExposureCategories.B;
            double V = 115;   // mph

            // building dimension constants
            double wall_ht = 35;  // wall height
            double b = 100; // length perpendicular to wind
            double ww_wall_x = 0;
            double ww_wall_y = wall_ht;
            double ridge_x = 50;
            double ridge_y = wall_ht + 50;
            double lw_wall_x = 100;
            double lw_wall_y = wall_ht;

            // Create a building object
            // Profile of the roof line
            // TODO:: Need to sort the order of these points or provide some sort of logic (left-to-right) progression of points

            ////TESTING: Sloped roof profile
            double[] profile = new double[] { ww_wall_x, ww_wall_y, ww_wall_x + 25, wall_ht + 10, ridge_x, ridge_y, ww_wall_x + 75, wall_ht + 10, lw_wall_x, lw_wall_y };
            BuildingInfo bldg = new SlopedRoofBuildingInfo(b, (lw_wall_x - ww_wall_x), 0.5 * (ridge_y + ww_wall_y), profile, RiskCategories.II);


            ////TESTING: flat roof profile
            //double[] profile = new double[] { ww_wall_x, ww_wall_y, lw_wall_x, lw_wall_y };
            //BuildingInfo bldg = new BuildingInfo(b, (lw_wall_x - ww_wall_x), 0.5 * (ridge_y + ww_wall_y), profile, RiskCategories.II);

            // Create the wind provision model
            WindProvisions wind_prov = new WindProvisions(V, bldg, exp);

            // Create the view model for the wind provisions
            WindVM = new WindViewModel(bldg, wind_prov);

            // Set initial scale factors
            SCALE_FACTOR_HORIZ = 0.6 * canvas.Width / WindVM.Bldg.L;
            SCALE_FACTOR_VERT = 0.6 * canvas.Height / WindVM.Bldg.H;
            SCALE_FACTOR = Math.Min(SCALE_FACTOR_HORIZ, SCALE_FACTOR_VERT);

            // Independent scale factor for the pressure diagram
            PRESSURE_SCALE_FACTOR = 0.6 * SCALE_FACTOR;

            // Create the view models for the building object on its canvas
            switch (canvas.Name)
            {
                case "MainCanvas":
                    BuildingVM_1 = new BuildingViewModel(canvas, bldg, SCALE_FACTOR);
                    break;
                case "Canvas2":
                    BuildingVM_2 = new BuildingViewModel(canvas, bldg, SCALE_FACTOR);
                    break;
                case "Canvas3":
                    BuildingVM_3 = new BuildingViewModel(canvas, bldg, SCALE_FACTOR);
                    break;
                case "Canvas4":
                    BuildingVM_4 = new BuildingViewModel(canvas, bldg, SCALE_FACTOR);
                    break;
                default:
                    break;
            }
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

        /// <summary>
        /// Draws the building object to the sspecified canvas
        /// </summary>
        /// <param name="canvas">the canvas object to draw to</param>
        private void DrawStructure_Elevation(Canvas canvas, BuildingViewModel building_vm)
        {
            building_vm.Draw(canvas, STRUCTURE_DIM_TEXT_HT);
        }

        /// <summary>
        /// Function to draw the wall pressures
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="x_ww_grd"></param>
        /// <param name="y_ww_grd"></param>
        /// <param name="x_ww_15"></param>
        /// <param name="y_ww_15"></param>
        /// <param name="x_ww_h"></param>
        /// <param name="y_ww_h"></param>
        /// <param name="x_lw_grd"></param>
        /// <param name="y_lw_grd"></param>
        /// <param name="x_lw_h"></param>
        /// <param name="y_lw_h"></param>
        private void DrawWallPressure_Elevation(Canvas canvas, double x_ww_grd, double y_ww_grd, double x_ww_15, double y_ww_15, double x_ww_h, double y_ww_h, double x_lw_grd, double y_lw_grd, double x_lw_h, double y_lw_h)
        {
            // Dynamic pressure q coordinates for WW
            double x_q0_ww = x_ww_grd - WindVM.Wind_Prov.Q_0 * PRESSURE_SCALE_FACTOR;
            double y_q0_ww = y_ww_grd;
            double x_q15_ww = x_ww_15 - WindVM.Wind_Prov.Q_15 * PRESSURE_SCALE_FACTOR;
            double y_q15_ww = y_ww_15;
            double x_qh_ww = x_ww_h - WindVM.Wind_Prov.Q_H * PRESSURE_SCALE_FACTOR;
            double y_qh_ww = y_ww_h;

            // for LW
            double x_qh_lw = x_lw_h + WindVM.Wind_Prov.Q_H * PRESSURE_SCALE_FACTOR;
            double y_qh_lw = y_lw_h;
            double x_q0_lw = x_lw_grd + WindVM.Wind_Prov.Q_H * PRESSURE_SCALE_FACTOR;
            double y_q0_lw = y_lw_grd;

            // Pressure p = G Cp q values coordinates for WW
            double x_p0_ww = x_ww_grd - WindVM.Wind_Prov.P_0_WW * PRESSURE_SCALE_FACTOR;
            double y_p0_ww = y_ww_grd;
            double x_p15_ww = x_ww_15 - WindVM.Wind_Prov.P_15_WW * PRESSURE_SCALE_FACTOR;
            double y_p15_ww = y_ww_15;
            double x_ph_ww = x_ww_h - WindVM.Wind_Prov.P_H_WW * PRESSURE_SCALE_FACTOR;
            double y_ph_ww = y_ww_h;

            // LW pressure points.  Subtraction here because LW wall pressures are suction (negative);
            double x_ph_lw = x_lw_h - WindVM.Wind_Prov.P_H_LW * PRESSURE_SCALE_FACTOR;
            double y_ph_lw = y_lw_h;
            double x_p0_lw = x_lw_grd - WindVM.Wind_Prov.P_H_LW * PRESSURE_SCALE_FACTOR;
            double y_p0_lw = y_lw_grd;

            // create our pressure label
            string pressure_str;

            // Draw the WW Pressure Profile.
            // p0 pressure line
            DrawingHelpers.DrawLine(canvas, x_p0_ww, y_p0_ww, x_ww_grd, y_ww_grd, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
            pressure_str = (Math.Round(WindVM.Wind_Prov.P_0_WW * 100.0) / 100.0).ToString();
            DrawingHelpers.DrawText(canvas, x_p0_ww, y_p0_ww, 0, pressure_str, Brushes.Blue, PRESSURE_TEXT_HT);

            // Ph pressure line
            DrawingHelpers.DrawLine(canvas, x_ph_ww, y_ph_ww, x_ww_h, y_ww_h, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
            pressure_str = (Math.Round(WindVM.Wind_Prov.P_H_WW * 100.0) / 100.0).ToString();
            DrawingHelpers.DrawText(canvas, x_ph_ww, y_ph_ww, 0, pressure_str, Brushes.Blue, PRESSURE_TEXT_HT);

            // show the 15' WW wall pressure location if necessary.
            if (WindVM.Bldg.H > 15)
            {
                // Negative here pulls the dimension to the left
                DrawingHelpers.DrawDimensionAligned(canvas, x_p15_ww, y_p15_ww, x_p0_ww, y_p0_ww, "15'", PRESSURE_TEXT_HT,
                    -30, -0.3, -5, -10, DrawingHelpers.DEFAULT_DIM_LEADER_COLOR, DrawingHelpers.DEFAULT_DIM_LINETYPE);

                // Q15 pressure line
                DrawingHelpers.DrawLine(canvas, x_p15_ww, y_p15_ww, x_ww_15, y_ww_15, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
                pressure_str = (Math.Round(WindVM.Wind_Prov.P_15_WW * 100.0) / 100.0).ToString();
                DrawingHelpers.DrawText(canvas, x_p15_ww, y_p15_ww, 0, pressure_str.ToString(), Brushes.Blue, PRESSURE_TEXT_HT);
            }

            // Draw lines between pressure points
            DrawingHelpers.DrawLine(canvas, x_p0_ww, y_p0_ww, x_p15_ww, y_p15_ww, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
            DrawingHelpers.DrawLine(canvas, x_p15_ww, y_p15_ww, x_ph_ww, y_ph_ww, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);

            // Draw the LW Pressure Proile.
            // p0 pressure line
            DrawingHelpers.DrawLine(canvas, x_p0_lw, y_p0_lw, x_lw_grd, y_lw_grd, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
            pressure_str = (Math.Round(WindVM.Wind_Prov.P_H_LW * 100.0) / 100.0).ToString();
            DrawingHelpers.DrawText(canvas, x_p0_lw, y_p0_ww, 0, pressure_str, Brushes.Blue, PRESSURE_TEXT_HT);

            // Ph pressure line
            DrawingHelpers.DrawLine(canvas, x_ph_lw, y_ph_lw, x_lw_h, y_lw_h, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
            pressure_str = (Math.Round(WindVM.Wind_Prov.P_H_LW * 100.0) / 100.0).ToString();
            DrawingHelpers.DrawText(canvas, x_ph_lw, y_ph_lw, 0, pressure_str, Brushes.Blue, PRESSURE_TEXT_HT);

            // Draw lines between LW pressure points
            DrawingHelpers.DrawLine(canvas, x_p0_lw, y_p0_lw, x_ph_lw, y_ph_lw, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
        }

        /// <summary>
        /// Function to draw the roof pressures
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="x_ww_grd"></param>
        /// <param name="y_ww_grd"></param>
        /// <param name="x_center"></param>
        /// <param name="y_center"></param>
        /// <param name="x_ww_h"></param>
        /// <param name="y_ww_h"></param>
        /// <param name="x_lw_grd"></param>
        /// <param name="y_lw_grd"></param>
        /// <param name="x_lw_h"></param>
        /// <param name="y_lw_h"></param>
        private void DrawRoofPressure_Elevation(Canvas canvas, double x_ww_grd, double y_ww_grd, double x_center, double y_center, double x_ww_h, double y_ww_h, double x_lw_grd, double y_lw_grd, double x_lw_h, double y_lw_h)
        {
            string title_str = "";
            // TODO:: NEED TO FIX SLOPED ROOF TOO
            // Draw the flat roof pressure cases
            if (canvas == MainCanvas)
            {
                title_str = "Normal to Ridge - Case A";
                DrawPressureRoof(canvas, x_ww_h, y_ww_h, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA);
            }
            else if (canvas == Canvas2)
            {
                title_str = "Normal to Ridge - Case B";
                DrawPressureRoof(canvas, x_ww_h, y_ww_h, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB);
            }
            else if (canvas == Canvas3)
            {
                title_str = "Parallel to Ridge - Case A";
                DrawPressureRoof(canvas, x_ww_h, y_ww_h, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA);
            }
            else if (canvas == Canvas4)
            {
                title_str = "Parallel to Ridge - Case B";
                DrawPressureRoof(canvas, x_ww_h, y_ww_h, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB);
            }

            // Draws the title for each picture based on the roof pressure diagram being generated.
            DrawingHelpers.DrawText(canvas, x_center - 130, y_ww_grd + 50, 0, title_str, Brushes.Black, 25);

        }

        /// <summary>
        /// the main drawing function for the canvas object
        /// </summary>
        /// <param name="canvas"></param>
        protected void Draw(Canvas canvas, BuildingViewModel view_model)
        {
            // Center point of current canvas
            double x_center = canvas.Width * 0.5;
            double y_center = canvas.Height * 0.5;

            // Draw the centerlines of the canvas
            DrawingHelpers.DrawLine(canvas, x_center, 0, x_center, 2.0 * y_center, Brushes.BlueViolet, 1, Linetypes.LINETYPE_PHANTOM);
            DrawingHelpers.DrawLine(canvas, 0, y_center, 2.0 * x_center, y_center, Brushes.BlueViolet, 1, Linetypes.LINETYPE_PHANTOM);

            // Ground location on canvas
            double y_ground = y_center + WindVM.Bldg.H * 0.5 * SCALE_FACTOR;

            double x_ww_grd = x_center - (WindVM.Bldg.L * 0.5) * SCALE_FACTOR;
            double y_ww_grd = y_ground;
            double x_ww_h = x_ww_grd;
            double y_ww_h = y_ground - WindVM.Bldg.H * SCALE_FACTOR;

            double x_lw_grd = x_center + (WindVM.Bldg.L * 0.5) * SCALE_FACTOR;
            double y_lw_grd = y_ground;
            double x_lw_h = x_lw_grd;
            double y_lw_h = y_ground - WindVM.Bldg.H * SCALE_FACTOR;

            double x_ww_15 = 0.5 * (x_ww_grd + x_ww_h);
            double y_ww_15 = y_ww_grd - (15.0 / WindVM.Bldg.H) * (y_ww_grd - y_ww_h);
            // Find coordinate of 15' mark
            if (WindVM.Bldg.H <= 15)
            {
                x_ww_15 = x_ww_h;
                y_ww_15 = y_ww_h;
            }

            // Draw the building object
            DrawStructure_Elevation(canvas, view_model);

            // Draw the wall pressures
            DrawWallPressure_Elevation(canvas, x_ww_grd, y_ww_grd, x_ww_15, y_ww_15, x_ww_h, y_ww_h, x_lw_grd, y_lw_grd, x_lw_h, y_lw_h);

            // Draw the roof pressures
            DrawRoofPressure_Elevation(canvas, x_ww_grd, y_ww_grd, x_center, y_center, x_ww_h, y_ww_h, x_lw_grd, y_lw_grd, x_lw_h, y_lw_h);
        }

        /// <summary>
        // Draws the wind pressure diagram for a roof
        /// </summary>
        /// <param name="x_ww_h">x position of top of windward wall</param>
        /// <param name="y_ww_h">y position of top of windward wall</param>
        /// <param name="x_center">x position of center of the canvas</param>
        /// <param name="y_center">y position of center of the canvas</param>
        /// <param name="x_ww_grd">x position of ground elev at windward wall</param>
        /// <param name="y_ww_grd">y position of ground elev at windward wall</param>
        /// <param name="o_scale">scale factor for object lines (building)</param>
        /// <param name="p_scale">scale factor for the pressure items</param>
        private void DrawPressureRoof(Canvas canvas, double x_ww_h, double y_ww_h, double[] arr)
        {
            for (int i = 0; i < WindVM.Bldg.RoofZonePts.Length / 2.0; i++)
            {
                double x_p1 = x_ww_h +  WindVM.Bldg.RoofZonePts[2*i] * SCALE_FACTOR;
                double x_p2 = x_ww_h +  WindVM.Bldg.RoofZonePts[2*i+1] * SCALE_FACTOR;
                double y_p = y_ww_h - arr[i] * PRESSURE_SCALE_FACTOR;

                // If the region has zero length, skip to the next region and don't illustrate it
                if(x_p1 == x_p2)
                {
                    continue;
                }

                // create our pressure label
                string pressure_str = (Math.Round(arr[i] * 100.0) / 100.0).ToString();

                DrawingHelpers.DrawRectangle(canvas, x_p1, y_ww_h, (x_p2 - x_p1), y_p - y_ww_h, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                DrawingHelpers.DrawText(canvas, x_p1, y_p, 0, pressure_str, Brushes.Red, PRESSURE_TEXT_HT);
                DrawingHelpers.DrawDimensionAligned(canvas, x_p1, y_ww_h, x_p2, y_ww_h, "Z" + (i+1).ToString(), 10);
            }
        }
    }
}
