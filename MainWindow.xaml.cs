using ASCE7_10Library;
using DrawingHelpersLibrary;
using System;
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
            Draw(MainCanvas);
            Draw(Canvas2);
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
            BuildingInfo bldg = new BuildingInfo(100, 100, 20, 0, RiskCategories.II);
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

        protected void Draw(Canvas canvas)
        {

            double SCALE_FACTOR_HORIZ = 0.6 * MainCanvas.Width / WindVM.Bldg.L;
            double SCALE_FACTOR_VERT = 0.6 * MainCanvas.Height / WindVM.Bldg.H;
            double SCALE_FACTOR = Math.Min(SCALE_FACTOR_HORIZ, SCALE_FACTOR_VERT);
            //double SCALE_FACTOR = 1.0;

            // Independent scale factor for the pressure diagram
            double PRESSURE_SCALE_FACTOR = 0.6 * SCALE_FACTOR;

            // Center point of current canvas
            double x_center = canvas.Width * 0.5;
            double y_center = canvas.Height * 0.5;

            // Ground location on canvas
            double y_ground = y_center + WindVM.Bldg.H * 0.5 * SCALE_FACTOR;
            //double y_ground = MainCanvas.Height * 0.8;

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

            // Draw the centerlines of the canvas
            DrawingHelpers.DrawLine(canvas, x_center, 0, x_center, 2.0 * y_center, Brushes.BlueViolet, 1, Linetypes.LINETYPE_PHANTOM);
            DrawingHelpers.DrawLine(canvas, 0, y_center, 2.0 * x_center, y_center, Brushes.BlueViolet, 1, Linetypes.LINETYPE_PHANTOM);

            // Draw the WW wall object
            DrawingHelpers.DrawLine(canvas, x_ww_grd, y_ww_grd, x_ww_h, y_ww_h, Brushes.Black, 3, Linetypes.LINETYPE_SOLID);

            // Draw the Roof object line
            DrawingHelpers.DrawLine(canvas, x_ww_h, y_ww_h, x_lw_h, y_lw_h, Brushes.Black, 3, Linetypes.LINETYPE_SOLID);

            // Draw the LW object line
            DrawingHelpers.DrawLine(canvas, x_lw_h, y_lw_h, x_lw_grd, y_lw_grd, Brushes.Black, 3, Linetypes.LINETYPE_SOLID);

            // Draw building dimensions
            DrawingHelpers.DrawDimensionAligned(canvas, x_ww_grd, y_ww_grd, x_lw_grd, y_lw_grd, WindVM.Bldg.L + "'", STRUCTURE_DIM_TEXT_HT);
            DrawingHelpers.DrawDimensionAligned(canvas, x_ww_h, y_ww_h, x_ww_grd, y_ww_grd, WindVM.Bldg.H + "'", STRUCTURE_DIM_TEXT_HT);


            // Draw the WW Pressure Profile.
            // p0 pressure line
            DrawingHelpers.DrawLine(canvas, x_p0_ww, y_p0_ww, x_ww_grd, y_ww_grd, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
            DrawingHelpers.DrawText(canvas, x_p0_ww, y_p0_ww, 0, WindVM.Wind_Prov.P_0_WW.ToString(), Brushes.Blue, PRESSURE_TEXT_HT);

            // Ph pressure line
            DrawingHelpers.DrawLine(canvas, x_ph_ww, y_ph_ww, x_ww_h, y_ww_h, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
            DrawingHelpers.DrawText(canvas, x_ph_ww, y_ph_ww, 0, WindVM.Wind_Prov.P_H_WW.ToString(), Brushes.Blue, PRESSURE_TEXT_HT);

            // show the 15' WW wall pressure location if necessary.
            if (WindVM.Bldg.H > 15)
            {
                // Negative here pulls the dimension to the left
                DrawingHelpers.DrawDimensionAligned(canvas, x_p15_ww, y_p15_ww, x_p0_ww, y_p0_ww, "15'", PRESSURE_TEXT_HT,
                    -30, -0.3, -5, -10, DrawingHelpers.DEFAULT_DIM_LEADER_COLOR, DrawingHelpers.DEFAULT_DIM_LINETYPE);

                // Q15 pressure line
                DrawingHelpers.DrawLine(canvas, x_p15_ww, y_p15_ww, x_ww_15, y_ww_15, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
                DrawingHelpers.DrawText(canvas, x_p15_ww, y_p15_ww, 0, WindVM.Wind_Prov.P_15_WW.ToString(), Brushes.Blue, PRESSURE_TEXT_HT);
            }

            // Draw lines between pressure points
            DrawingHelpers.DrawLine(canvas, x_p0_ww, y_p0_ww, x_p15_ww, y_p15_ww, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
            DrawingHelpers.DrawLine(canvas, x_p15_ww, y_p15_ww, x_ph_ww, y_ph_ww, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);

            // Draw the LW Pressure Proile.
            // p0 pressure line
            DrawingHelpers.DrawLine(canvas, x_p0_lw, y_p0_lw, x_lw_grd, y_lw_grd, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
            DrawingHelpers.DrawText(canvas, x_p0_lw, y_p0_ww, 0, WindVM.Wind_Prov.P_H_LW.ToString(), Brushes.Blue, PRESSURE_TEXT_HT);

            // Ph pressure line
            DrawingHelpers.DrawLine(canvas, x_ph_lw, y_ph_lw, x_lw_h, y_lw_h, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
            DrawingHelpers.DrawText(canvas, x_ph_lw, y_ph_lw, 0, WindVM.Wind_Prov.P_H_LW.ToString(), Brushes.Blue, PRESSURE_TEXT_HT);

            // Draw lines between LW pressure points
            DrawingHelpers.DrawLine(canvas, x_p0_lw, y_p0_lw, x_ph_lw, y_ph_lw, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);


            // TODO::  This needs to be moved.
            // Draw the flat roof pressure cases
            if (canvas == MainCanvas)
                DrawRoofPressure_ParallelToRidge_CaseA(canvas, x_ww_h, y_ww_h, x_center, y_center, x_ww_grd, y_ww_grd, SCALE_FACTOR, PRESSURE_SCALE_FACTOR);
            else
                DrawRoofPressure_ParallelToRidge_CaseB(canvas, x_ww_h, y_ww_h, x_center, y_center, x_ww_grd, y_ww_grd, SCALE_FACTOR, PRESSURE_SCALE_FACTOR);

            //DrawingHelpers.AlignedDimensionTests(canvas);
        }

        /// <summary>
        // Draws the wind pressure diagram for wind normal to a ridge for Case A (uplift on WW roof)
        /// </summary>
        /// <param name="x_ww_h">x position of top of windward wall</param>
        /// <param name="y_ww_h">y position of top of windward wall</param>
        /// <param name="x_center">x position of center of the canvas</param>
        /// <param name="y_center">y position of center of the canvas</param>
        /// <param name="x_ww_grd">x position of ground elev at windward wall</param>
        /// <param name="y_ww_grd">y position of ground elev at windward wall</param>
        /// <param name="o_scale">scale factor for object lines (building)</param>
        /// <param name="p_scale">scale factor for the pressure items</param>
        protected void DrawRoofPressure_NormalToRidge_CaseA(Canvas canvas, double x_ww_h, double y_ww_h, double x_center, double y_center, double x_ww_grd, double y_ww_grd, double o_scale, double p_scale)
        {
            double PRESSURE_SCALE_FACTOR = p_scale;
            double SCALE_FACTOR = o_scale;

            // Z1
            double x_ph_ww_roof_z1_start = x_ww_h;
            double y_ph_ww_roof_z1_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA[0] * PRESSURE_SCALE_FACTOR;
            double x_ph_ww_roof_z1_end;
            double y_ph_ww_roof_z1_end;

            //Z2
            double x_ph_ww_roof_z2_start;
            double y_ph_ww_roof_z2_start;
            double x_ph_ww_roof_z2_end;
            double y_ph_ww_roof_z2_end;

            //Z3
            double x_ph_ww_roof_z3_start;
            double y_ph_ww_roof_z3_start;
            double x_ph_ww_roof_z3_end;
            double y_ph_ww_roof_z3_end;

            //Z4
            double x_ph_ww_roof_z4_start;
            double y_ph_ww_roof_z4_start;
            double x_ph_ww_roof_z4_end;
            double y_ph_ww_roof_z4_end;

            // Draw the graphic title
            DrawingHelpers.DrawText(canvas, x_center - 75, y_ww_grd + 50, 0, "Case A - Normal to Ridge", Brushes.Black, 15);

            if (WindVM.Bldg.RoofSlope < 10)
            {
                if (WindVM.Wind_Prov.Building.L < WindVM.Wind_Prov.Building.H / 2.0)
                {
                    x_ph_ww_roof_z1_end = x_ww_h + WindVM.Wind_Prov.Building.L * SCALE_FACTOR;
                    y_ph_ww_roof_z1_end = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA[0] * PRESSURE_SCALE_FACTOR;

                    //Zone 1
                    DrawingHelpers.DrawLine(canvas, x_ww_h, y_ww_h, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_end, y_ww_h, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ww_h, 0.5 * (y_ph_ww_roof_z1_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA[0].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Draw Pressure Dimensions
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z1_start, y_ww_h, x_ph_ww_roof_z1_end, y_ww_h, "Z1", STRUCTURE_DIM_TEXT_HT);
                }
                else if (WindVM.Wind_Prov.Building.L < WindVM.Wind_Prov.Building.H)
                {
                    x_ph_ww_roof_z1_end = x_ww_h + (0.5 * WindVM.Wind_Prov.Building.H) * SCALE_FACTOR;
                    y_ph_ww_roof_z1_end = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA[0] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z2_start = x_ph_ww_roof_z1_end;
                    y_ph_ww_roof_z2_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA[1] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z2_end = x_ww_h + WindVM.Wind_Prov.Building.L * SCALE_FACTOR;
                    y_ph_ww_roof_z2_end = y_ph_ww_roof_z2_start;

                    // Zone 1
                    DrawingHelpers.DrawLine(canvas, x_ww_h, y_ww_h, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_end, y_ww_h, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ww_h, 0.5 * (y_ph_ww_roof_z1_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA[0].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 2
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_end, y_ww_h, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z2_start, 0.5 * (y_ph_ww_roof_z2_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA[1].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Draw Pressure Dimensions
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z1_start, y_ww_h, x_ph_ww_roof_z1_end, y_ww_h, "Z1", STRUCTURE_DIM_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_end, y_ww_h, "Z2", STRUCTURE_DIM_TEXT_HT);
                }
                else if (WindVM.Wind_Prov.Building.L < 2.0 * WindVM.Wind_Prov.Building.H)
                {
                    x_ph_ww_roof_z1_end = x_ww_h + (0.5 * WindVM.Wind_Prov.Building.H) * SCALE_FACTOR;
                    y_ph_ww_roof_z1_end = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA[0] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z2_start = x_ph_ww_roof_z1_end;
                    y_ph_ww_roof_z2_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA[1] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z2_end = x_ww_h + WindVM.Wind_Prov.Building.H * SCALE_FACTOR;
                    y_ph_ww_roof_z2_end = y_ph_ww_roof_z2_start;

                    x_ph_ww_roof_z3_start = x_ph_ww_roof_z2_end;
                    y_ph_ww_roof_z3_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA[2] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z3_end = x_ww_h + WindVM.Wind_Prov.Building.L * SCALE_FACTOR;
                    y_ph_ww_roof_z3_end = y_ph_ww_roof_z3_start;

                    // Zone 1
                    DrawingHelpers.DrawLine(canvas, x_ww_h, y_ww_h, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_end, y_ww_h, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ww_h, 0.5 * (y_ph_ww_roof_z1_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA[0].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 2
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_end, y_ww_h, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z2_start, 0.5 * (y_ph_ww_roof_z2_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA[1].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 3
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_start, y_ww_h, x_ph_ww_roof_z3_start, y_ph_ww_roof_z3_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_end, y_ww_h, x_ph_ww_roof_z3_end, y_ph_ww_roof_z3_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_start, y_ph_ww_roof_z3_start, x_ph_ww_roof_z3_end, y_ph_ww_roof_z3_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z3_start, 0.5 * (y_ph_ww_roof_z3_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA[2].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Draw Pressure Dimensions
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z1_start, y_ww_h, x_ph_ww_roof_z1_end, y_ww_h, "Z1", STRUCTURE_DIM_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_end, y_ww_h, "Z2", STRUCTURE_DIM_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z3_start, y_ww_h, x_ph_ww_roof_z3_end, y_ww_h, "Z3", STRUCTURE_DIM_TEXT_HT);
                }
                else
                {
                    x_ph_ww_roof_z1_end = x_ww_h + (0.5 * WindVM.Wind_Prov.Building.H) * SCALE_FACTOR;
                    y_ph_ww_roof_z1_end = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA[0] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z2_start = x_ph_ww_roof_z1_end;
                    y_ph_ww_roof_z2_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA[1] * PRESSURE_SCALE_FACTOR;
                    x_ph_ww_roof_z2_end = x_ww_h + WindVM.Wind_Prov.Building.H * SCALE_FACTOR;
                    y_ph_ww_roof_z2_end = y_ph_ww_roof_z2_start;

                    x_ph_ww_roof_z3_start = x_ph_ww_roof_z2_end;
                    y_ph_ww_roof_z3_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA[2] * PRESSURE_SCALE_FACTOR;
                    x_ph_ww_roof_z3_end = x_ww_h + 2.0 * WindVM.Wind_Prov.Building.H * SCALE_FACTOR;
                    y_ph_ww_roof_z3_end = y_ph_ww_roof_z3_start;

                    x_ph_ww_roof_z4_start = x_ph_ww_roof_z3_end;
                    y_ph_ww_roof_z4_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA[3] * PRESSURE_SCALE_FACTOR;
                    x_ph_ww_roof_z4_end = x_ww_h + WindVM.Wind_Prov.Building.L * SCALE_FACTOR;
                    y_ph_ww_roof_z4_end = y_ph_ww_roof_z4_start;

                    // Zone 1
                    DrawingHelpers.DrawLine(canvas, x_ww_h, y_ww_h, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_end, y_ww_h, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ww_h, 0.5 * (y_ph_ww_roof_z1_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA[0].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 2
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_end, y_ww_h, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z2_start, 0.5 * (y_ph_ww_roof_z2_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA[1].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 3
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_start, y_ww_h, x_ph_ww_roof_z3_start, y_ph_ww_roof_z3_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_end, y_ww_h, x_ph_ww_roof_z3_end, y_ph_ww_roof_z3_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_start, y_ph_ww_roof_z3_start, x_ph_ww_roof_z3_end, y_ph_ww_roof_z3_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z3_start, 0.5 * (y_ph_ww_roof_z3_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA[2].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 4
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z4_start, y_ww_h, x_ph_ww_roof_z4_start, y_ph_ww_roof_z4_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z4_end, y_ww_h, x_ph_ww_roof_z4_end, y_ph_ww_roof_z4_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z4_start, y_ph_ww_roof_z4_start, x_ph_ww_roof_z4_end, y_ph_ww_roof_z4_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z4_start, 0.5 * (y_ph_ww_roof_z4_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEA[2].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Draw Pressure Dimensions
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z1_start, y_ww_h, x_ph_ww_roof_z1_end, y_ww_h, "Z1", STRUCTURE_DIM_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_end, y_ww_h, "Z2", STRUCTURE_DIM_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z3_start, y_ww_h, x_ph_ww_roof_z3_end, y_ww_h, "Z3", STRUCTURE_DIM_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z4_start, y_ww_h, x_ph_ww_roof_z4_end, y_ww_h, "Z4", STRUCTURE_DIM_TEXT_HT);
                }
            }
        }

        /// <summary>
        // Draws the wind pressure diagram for wind normal to a ridge for Case B (direct pressure on WW roof)
        /// </summary>
        /// <param name="x_ww_h">x position of top of windward wall</param>
        /// <param name="y_ww_h">y position of top of windward wall</param>
        /// <param name="x_center">x position of center of the canvas</param>
        /// <param name="y_center">y position of center of the canvas</param>
        /// <param name="x_ww_grd">x position of ground elev at windward wall</param>
        /// <param name="y_ww_grd">y position of ground elev at windward wall</param>
        /// <param name="o_scale">scale factor for object lines (building)</param>
        /// <param name="p_scale">scale factor for the pressure items</param>
        protected void DrawRoofPressure_NormalToRidge_CaseB(Canvas canvas, double x_ww_h, double y_ww_h, double x_center, double y_center, double x_ww_grd, double y_ww_grd, double o_scale, double p_scale)
        {
            double PRESSURE_SCALE_FACTOR = p_scale;
            double SCALE_FACTOR = o_scale;

            // Z1
            double x_ph_ww_roof_z1_start = x_ww_h;
            double y_ph_ww_roof_z1_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB[0] * PRESSURE_SCALE_FACTOR;
            double x_ph_ww_roof_z1_end;
            double y_ph_ww_roof_z1_end;

            //Z2
            double x_ph_ww_roof_z2_start;
            double y_ph_ww_roof_z2_start;
            double x_ph_ww_roof_z2_end;
            double y_ph_ww_roof_z2_end;

            //Z3
            double x_ph_ww_roof_z3_start;
            double y_ph_ww_roof_z3_start;
            double x_ph_ww_roof_z3_end;
            double y_ph_ww_roof_z3_end;

            //Z4
            double x_ph_ww_roof_z4_start;
            double y_ph_ww_roof_z4_start;
            double x_ph_ww_roof_z4_end;
            double y_ph_ww_roof_z4_end;

            // Draw the graphic title
            DrawingHelpers.DrawText(canvas, x_center - 75, y_ww_grd + 50, 0, "Case B - Normal to Ridge", Brushes.Black, 15);

            if (WindVM.Bldg.RoofSlope < 10)
            {
                if (WindVM.Wind_Prov.Building.L < WindVM.Wind_Prov.Building.H / 2.0)
                {
                    x_ph_ww_roof_z1_end = x_ww_h + WindVM.Wind_Prov.Building.L * SCALE_FACTOR;
                    y_ph_ww_roof_z1_end = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB[0] * PRESSURE_SCALE_FACTOR;

                    //Zone 1
                    DrawingHelpers.DrawLine(canvas, x_ww_h, y_ww_h, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_end, y_ww_h, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ww_h, 0.5 * (y_ph_ww_roof_z1_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB[0].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Draw Pressure Dimensions
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z1_start, y_ww_h, x_ph_ww_roof_z1_end, y_ww_h, "Z1", 1.5*PRESSURE_TEXT_HT);
                }
                else if (WindVM.Wind_Prov.Building.L < WindVM.Wind_Prov.Building.H)
                {
                    x_ph_ww_roof_z1_end = x_ww_h + (0.5 * WindVM.Wind_Prov.Building.H) * SCALE_FACTOR;
                    y_ph_ww_roof_z1_end = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB[0] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z2_start = x_ph_ww_roof_z1_end;
                    y_ph_ww_roof_z2_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB[1] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z2_end = x_ww_h + WindVM.Wind_Prov.Building.L * SCALE_FACTOR;
                    y_ph_ww_roof_z2_end = y_ph_ww_roof_z2_start;

                    // Zone 1
                    DrawingHelpers.DrawLine(canvas, x_ww_h, y_ww_h, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_end, y_ww_h, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ww_h, 0.5 * (y_ph_ww_roof_z1_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB[0].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 2
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_end, y_ww_h, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z2_start, 0.5 * (y_ph_ww_roof_z2_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB[1].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Draw Pressure Dimensions
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z1_start, y_ww_h, x_ph_ww_roof_z1_end, y_ww_h, "Z1", 1.5 * PRESSURE_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_end, y_ww_h, "Z2", 1.5 * PRESSURE_TEXT_HT);
                }
                else if (WindVM.Wind_Prov.Building.L < 2.0 * WindVM.Wind_Prov.Building.H)
                {
                    x_ph_ww_roof_z1_end = x_ww_h + (0.5 * WindVM.Wind_Prov.Building.H) * SCALE_FACTOR;
                    y_ph_ww_roof_z1_end = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB[0] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z2_start = x_ph_ww_roof_z1_end;
                    y_ph_ww_roof_z2_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB[1] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z2_end = x_ww_h + WindVM.Wind_Prov.Building.H * SCALE_FACTOR;
                    y_ph_ww_roof_z2_end = y_ph_ww_roof_z2_start;

                    x_ph_ww_roof_z3_start = x_ph_ww_roof_z2_end;
                    y_ph_ww_roof_z3_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB[2] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z3_end = x_ww_h + WindVM.Wind_Prov.Building.L * SCALE_FACTOR;
                    y_ph_ww_roof_z3_end = y_ph_ww_roof_z3_start;

                    // Zone 1
                    DrawingHelpers.DrawLine(canvas, x_ww_h, y_ww_h, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_end, y_ww_h, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ww_h, 0.5 * (y_ph_ww_roof_z1_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB[0].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 2
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_end, y_ww_h, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z2_start, 0.5 * (y_ph_ww_roof_z2_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB[1].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 3
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_start, y_ww_h, x_ph_ww_roof_z3_start, y_ph_ww_roof_z3_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_end, y_ww_h, x_ph_ww_roof_z3_end, y_ph_ww_roof_z3_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_start, y_ph_ww_roof_z3_start, x_ph_ww_roof_z3_end, y_ph_ww_roof_z3_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z3_start, 0.5 * (y_ph_ww_roof_z3_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB[2].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Draw Pressure Dimensions
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z1_start, y_ww_h, x_ph_ww_roof_z1_end, y_ww_h, "Z1", 1.5 * PRESSURE_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_end, y_ww_h, "Z2", 1.5 * PRESSURE_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z3_start, y_ww_h, x_ph_ww_roof_z3_end, y_ww_h, "Z3", 1.5 * PRESSURE_TEXT_HT);
                }
                else
                {
                    x_ph_ww_roof_z1_end = x_ww_h + (0.5 * WindVM.Wind_Prov.Building.H) * SCALE_FACTOR;
                    y_ph_ww_roof_z1_end = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB[0] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z2_start = x_ph_ww_roof_z1_end;
                    y_ph_ww_roof_z2_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB[1] * PRESSURE_SCALE_FACTOR;
                    x_ph_ww_roof_z2_end = x_ww_h + WindVM.Wind_Prov.Building.H * SCALE_FACTOR;
                    y_ph_ww_roof_z2_end = y_ph_ww_roof_z2_start;

                    x_ph_ww_roof_z3_start = x_ph_ww_roof_z2_end;
                    y_ph_ww_roof_z3_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB[2] * PRESSURE_SCALE_FACTOR;
                    x_ph_ww_roof_z3_end = x_ww_h + 2.0 * WindVM.Wind_Prov.Building.H * SCALE_FACTOR;
                    y_ph_ww_roof_z3_end = y_ph_ww_roof_z3_start;

                    x_ph_ww_roof_z4_start = x_ph_ww_roof_z3_end;
                    y_ph_ww_roof_z4_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB[3] * PRESSURE_SCALE_FACTOR;
                    x_ph_ww_roof_z4_end = x_ww_h + WindVM.Wind_Prov.Building.L * SCALE_FACTOR;
                    y_ph_ww_roof_z4_end = y_ph_ww_roof_z4_start;

                    // Zone 1
                    DrawingHelpers.DrawLine(canvas, x_ww_h, y_ww_h, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_end, y_ww_h, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ww_h, 0.5 * (y_ph_ww_roof_z1_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB[0].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 2
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_end, y_ww_h, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z2_start, 0.5 * (y_ph_ww_roof_z2_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB[1].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 3
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_start, y_ww_h, x_ph_ww_roof_z3_start, y_ph_ww_roof_z3_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_end, y_ww_h, x_ph_ww_roof_z3_end, y_ph_ww_roof_z3_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_start, y_ph_ww_roof_z3_start, x_ph_ww_roof_z3_end, y_ph_ww_roof_z3_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z3_start, 0.5 * (y_ph_ww_roof_z3_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB[2].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 4
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z4_start, y_ww_h, x_ph_ww_roof_z4_start, y_ph_ww_roof_z4_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z4_end, y_ww_h, x_ph_ww_roof_z4_end, y_ph_ww_roof_z4_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z4_start, y_ph_ww_roof_z4_start, x_ph_ww_roof_z4_end, y_ph_ww_roof_z4_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z4_start, 0.5 * (y_ph_ww_roof_z4_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_NORMAL_CASEB[2].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Draw Pressure Dimensions
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z1_start, y_ww_h, x_ph_ww_roof_z1_end, y_ww_h, "Z1", 1.5 * PRESSURE_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_end, y_ww_h, "Z2", 1.5 * PRESSURE_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z3_start, y_ww_h, x_ph_ww_roof_z3_end, y_ww_h, "Z3", 1.5 * PRESSURE_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z4_start, y_ww_h, x_ph_ww_roof_z4_end, y_ww_h, "Z4", 1.5 * PRESSURE_TEXT_HT);
                }
            }
        }

        /// <summary>
        // Draws the wind pressure diagram for wind normal to a ridge for Case A (uplift on WW roof)
        /// </summary>
        /// <param name="x_ww_h">x position of top of windward wall</param>
        /// <param name="y_ww_h">y position of top of windward wall</param>
        /// <param name="x_center">x position of center of the canvas</param>
        /// <param name="y_center">y position of center of the canvas</param>
        /// <param name="x_ww_grd">x position of ground elev at windward wall</param>
        /// <param name="y_ww_grd">y position of ground elev at windward wall</param>
        /// <param name="o_scale">scale factor for object lines (building)</param>
        /// <param name="p_scale">scale factor for the pressure items</param>
        protected void DrawRoofPressure_ParallelToRidge_CaseA(Canvas canvas, double x_ww_h, double y_ww_h, double x_center, double y_center, double x_ww_grd, double y_ww_grd, double o_scale, double p_scale)
        {
            double PRESSURE_SCALE_FACTOR = p_scale;
            double SCALE_FACTOR = o_scale;

            // Z1
            double x_ph_ww_roof_z1_start = x_ww_h;
            double y_ph_ww_roof_z1_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA[0] * PRESSURE_SCALE_FACTOR;
            double x_ph_ww_roof_z1_end;
            double y_ph_ww_roof_z1_end;

            //Z2
            double x_ph_ww_roof_z2_start;
            double y_ph_ww_roof_z2_start;
            double x_ph_ww_roof_z2_end;
            double y_ph_ww_roof_z2_end;

            //Z3
            double x_ph_ww_roof_z3_start;
            double y_ph_ww_roof_z3_start;
            double x_ph_ww_roof_z3_end;
            double y_ph_ww_roof_z3_end;

            //Z4
            double x_ph_ww_roof_z4_start;
            double y_ph_ww_roof_z4_start;
            double x_ph_ww_roof_z4_end;
            double y_ph_ww_roof_z4_end;

            // Draw the graphic title
            DrawingHelpers.DrawText(canvas, x_center - 75, y_ww_grd + 50, 0, "Case A - Parallel to Ridge", Brushes.Black, 15);

            if (WindVM.Bldg.RoofSlope < 10)
            {
                if (WindVM.Wind_Prov.Building.L < WindVM.Wind_Prov.Building.H / 2.0)
                {
                    x_ph_ww_roof_z1_end = x_ww_h + WindVM.Wind_Prov.Building.L * SCALE_FACTOR;
                    y_ph_ww_roof_z1_end = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA[0] * PRESSURE_SCALE_FACTOR;

                    //Zone 1
                    DrawingHelpers.DrawLine(canvas, x_ww_h, y_ww_h, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_end, y_ww_h, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ww_h, 0.5 * (y_ph_ww_roof_z1_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA[0].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Draw Pressure Dimensions
                    DrawingHelpers.DrawHorizontalDimension_Above(MainCanvas, 30, 0.2, 5, x_ph_ww_roof_z1_start, y_ww_h, x_ph_ww_roof_z1_end, y_ww_h, "Z1");
                }
                else if (WindVM.Wind_Prov.Building.L < WindVM.Wind_Prov.Building.H)
                {
                    x_ph_ww_roof_z1_end = x_ww_h + (0.5 * WindVM.Wind_Prov.Building.H) * SCALE_FACTOR;
                    y_ph_ww_roof_z1_end = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA[0] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z2_start = x_ph_ww_roof_z1_end;
                    y_ph_ww_roof_z2_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA[1] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z2_end = x_ww_h + WindVM.Wind_Prov.Building.L * SCALE_FACTOR;
                    y_ph_ww_roof_z2_end = y_ph_ww_roof_z2_start;

                    // Zone 1
                    DrawingHelpers.DrawLine(canvas, x_ww_h, y_ww_h, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_end, y_ww_h, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ww_h, 0.5 * (y_ph_ww_roof_z1_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA[0].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 2
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_end, y_ww_h, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z2_start, 0.5 * (y_ph_ww_roof_z2_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA[1].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Draw Pressure Dimensions
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z1_start, y_ww_h, x_ph_ww_roof_z1_end, y_ww_h, "Z1", 1.5  *PRESSURE_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_end, y_ww_h, "Z2", 1.5 * PRESSURE_TEXT_HT);
                }
                else if (WindVM.Wind_Prov.Building.L < 2.0 * WindVM.Wind_Prov.Building.H)
                {
                    x_ph_ww_roof_z1_end = x_ww_h + (0.5 * WindVM.Wind_Prov.Building.H) * SCALE_FACTOR;
                    y_ph_ww_roof_z1_end = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA[0] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z2_start = x_ph_ww_roof_z1_end;
                    y_ph_ww_roof_z2_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA[1] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z2_end = x_ww_h + WindVM.Wind_Prov.Building.H * SCALE_FACTOR;
                    y_ph_ww_roof_z2_end = y_ph_ww_roof_z2_start;

                    x_ph_ww_roof_z3_start = x_ph_ww_roof_z2_end;
                    y_ph_ww_roof_z3_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA[2] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z3_end = x_ww_h + WindVM.Wind_Prov.Building.L * SCALE_FACTOR;
                    y_ph_ww_roof_z3_end = y_ph_ww_roof_z3_start;

                    // Zone 1
                    DrawingHelpers.DrawLine(canvas, x_ww_h, y_ww_h, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_end, y_ww_h, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ww_h, 0.5 * (y_ph_ww_roof_z1_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA[0].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 2
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_end, y_ww_h, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z2_start, 0.5 * (y_ph_ww_roof_z2_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA[1].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 3
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_start, y_ww_h, x_ph_ww_roof_z3_start, y_ph_ww_roof_z3_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_end, y_ww_h, x_ph_ww_roof_z3_end, y_ph_ww_roof_z3_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_start, y_ph_ww_roof_z3_start, x_ph_ww_roof_z3_end, y_ph_ww_roof_z3_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z3_start, 0.5 * (y_ph_ww_roof_z3_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA[2].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Draw Pressure Dimensions
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z1_start, y_ww_h, x_ph_ww_roof_z1_end, y_ww_h, "Z1", 1.5 * PRESSURE_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_end, y_ww_h, "Z2", 1.5 * PRESSURE_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z3_start, y_ww_h, x_ph_ww_roof_z3_end, y_ww_h, "Z3", 1.5 * PRESSURE_TEXT_HT);
                }
                else
                {
                    x_ph_ww_roof_z1_end = x_ww_h + (0.5 * WindVM.Wind_Prov.Building.H) * SCALE_FACTOR;
                    y_ph_ww_roof_z1_end = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA[0] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z2_start = x_ph_ww_roof_z1_end;
                    y_ph_ww_roof_z2_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA[1] * PRESSURE_SCALE_FACTOR;
                    x_ph_ww_roof_z2_end = x_ww_h + WindVM.Wind_Prov.Building.H * SCALE_FACTOR;
                    y_ph_ww_roof_z2_end = y_ph_ww_roof_z2_start;

                    x_ph_ww_roof_z3_start = x_ph_ww_roof_z2_end;
                    y_ph_ww_roof_z3_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA[2] * PRESSURE_SCALE_FACTOR;
                    x_ph_ww_roof_z3_end = x_ww_h + 2.0 * WindVM.Wind_Prov.Building.H * SCALE_FACTOR;
                    y_ph_ww_roof_z3_end = y_ph_ww_roof_z3_start;

                    x_ph_ww_roof_z4_start = x_ph_ww_roof_z3_end;
                    y_ph_ww_roof_z4_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA[3] * PRESSURE_SCALE_FACTOR;
                    x_ph_ww_roof_z4_end = x_ww_h + WindVM.Wind_Prov.Building.L * SCALE_FACTOR;
                    y_ph_ww_roof_z4_end = y_ph_ww_roof_z4_start;

                    // Zone 1
                    DrawingHelpers.DrawLine(canvas, x_ww_h, y_ww_h, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_end, y_ww_h, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ww_h, 0.5 * (y_ph_ww_roof_z1_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA[0].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 2
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_end, y_ww_h, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z2_start, 0.5 * (y_ph_ww_roof_z2_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA[1].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 3
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_start, y_ww_h, x_ph_ww_roof_z3_start, y_ph_ww_roof_z3_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_end, y_ww_h, x_ph_ww_roof_z3_end, y_ph_ww_roof_z3_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_start, y_ph_ww_roof_z3_start, x_ph_ww_roof_z3_end, y_ph_ww_roof_z3_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z3_start, 0.5 * (y_ph_ww_roof_z3_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA[2].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 4
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z4_start, y_ww_h, x_ph_ww_roof_z4_start, y_ph_ww_roof_z4_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z4_end, y_ww_h, x_ph_ww_roof_z4_end, y_ph_ww_roof_z4_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z4_start, y_ph_ww_roof_z4_start, x_ph_ww_roof_z4_end, y_ph_ww_roof_z4_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z4_start, 0.5 * (y_ph_ww_roof_z4_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEA[2].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Draw Pressure Dimensions
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z1_start, y_ww_h, x_ph_ww_roof_z1_end, y_ww_h, "Z1", 1.5 * PRESSURE_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_end, y_ww_h, "Z2", 1.5 * PRESSURE_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z3_start, y_ww_h, x_ph_ww_roof_z3_end, y_ww_h, "Z3", 1.5 * PRESSURE_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z4_start, y_ww_h, x_ph_ww_roof_z4_end, y_ww_h, "Z4", 1.5 * PRESSURE_TEXT_HT);
                }
            }
        }

        /// <summary>
        // Draws the wind pressure diagram for wind normal to a ridge for Case A (uplift on WW roof)
        /// </summary>
        /// <param name="x_ww_h">x position of top of windward wall</param>
        /// <param name="y_ww_h">y position of top of windward wall</param>
        /// <param name="x_center">x position of center of the canvas</param>
        /// <param name="y_center">y position of center of the canvas</param>
        /// <param name="x_ww_grd">x position of ground elev at windward wall</param>
        /// <param name="y_ww_grd">y position of ground elev at windward wall</param>
        /// <param name="o_scale">scale factor for object lines (building)</param>
        /// <param name="p_scale">scale factor for the pressure items</param>
        protected void DrawRoofPressure_ParallelToRidge_CaseB(Canvas canvas, double x_ww_h, double y_ww_h, double x_center, double y_center, double x_ww_grd, double y_ww_grd, double o_scale, double p_scale)
        {
            double PRESSURE_SCALE_FACTOR = p_scale;
            double SCALE_FACTOR = o_scale;

            // Z1
            double x_ph_ww_roof_z1_start = x_ww_h;
            double y_ph_ww_roof_z1_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB[0] * PRESSURE_SCALE_FACTOR;
            double x_ph_ww_roof_z1_end;
            double y_ph_ww_roof_z1_end;

            //Z2
            double x_ph_ww_roof_z2_start;
            double y_ph_ww_roof_z2_start;
            double x_ph_ww_roof_z2_end;
            double y_ph_ww_roof_z2_end;

            //Z3
            double x_ph_ww_roof_z3_start;
            double y_ph_ww_roof_z3_start;
            double x_ph_ww_roof_z3_end;
            double y_ph_ww_roof_z3_end;

            //Z4
            double x_ph_ww_roof_z4_start;
            double y_ph_ww_roof_z4_start;
            double x_ph_ww_roof_z4_end;
            double y_ph_ww_roof_z4_end;

            // Draw the graphic title
            DrawingHelpers.DrawText(canvas, x_center - 75, y_ww_grd + 50, 0, "Case B - Parallel to Ridge", Brushes.Black, 15);

            if (WindVM.Bldg.RoofSlope < 10)
            {
                if (WindVM.Wind_Prov.Building.L < WindVM.Wind_Prov.Building.H / 2.0)
                {
                    x_ph_ww_roof_z1_end = x_ww_h + WindVM.Wind_Prov.Building.L * SCALE_FACTOR;
                    y_ph_ww_roof_z1_end = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB[0] * PRESSURE_SCALE_FACTOR;

                    //Zone 1
                    DrawingHelpers.DrawLine(canvas, x_ww_h, y_ww_h, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_end, y_ww_h, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ww_h, 0.5 * (y_ph_ww_roof_z1_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB[0].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Draw Pressure Dimensions
                    DrawingHelpers.DrawHorizontalDimension_Above(MainCanvas, 30, 0.2, 5, x_ph_ww_roof_z1_start, y_ww_h, x_ph_ww_roof_z1_end, y_ww_h, "Z1");
                }
                else if (WindVM.Wind_Prov.Building.L < WindVM.Wind_Prov.Building.H)
                {
                    x_ph_ww_roof_z1_end = x_ww_h + (0.5 * WindVM.Wind_Prov.Building.H) * SCALE_FACTOR;
                    y_ph_ww_roof_z1_end = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB[0] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z2_start = x_ph_ww_roof_z1_end;
                    y_ph_ww_roof_z2_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB[1] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z2_end = x_ww_h + WindVM.Wind_Prov.Building.L * SCALE_FACTOR;
                    y_ph_ww_roof_z2_end = y_ph_ww_roof_z2_start;

                    // Zone 1
                    DrawingHelpers.DrawLine(canvas, x_ww_h, y_ww_h, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_end, y_ww_h, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ww_h, 0.5 * (y_ph_ww_roof_z1_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB[0].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 2
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_end, y_ww_h, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z2_start, 0.5 * (y_ph_ww_roof_z2_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB[1].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Draw Pressure Dimensions
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z1_start, y_ww_h, x_ph_ww_roof_z1_end, y_ww_h, "Z1", 1.5 * PRESSURE_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_end, y_ww_h, "Z2", 1.5 * PRESSURE_TEXT_HT);
                }
                else if (WindVM.Wind_Prov.Building.L < 2.0 * WindVM.Wind_Prov.Building.H)
                {
                    x_ph_ww_roof_z1_end = x_ww_h + (0.5 * WindVM.Wind_Prov.Building.H) * SCALE_FACTOR;
                    y_ph_ww_roof_z1_end = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB[0] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z2_start = x_ph_ww_roof_z1_end;
                    y_ph_ww_roof_z2_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB[1] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z2_end = x_ww_h + WindVM.Wind_Prov.Building.H * SCALE_FACTOR;
                    y_ph_ww_roof_z2_end = y_ph_ww_roof_z2_start;

                    x_ph_ww_roof_z3_start = x_ph_ww_roof_z2_end;
                    y_ph_ww_roof_z3_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB[2] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z3_end = x_ww_h + WindVM.Wind_Prov.Building.L * SCALE_FACTOR;
                    y_ph_ww_roof_z3_end = y_ph_ww_roof_z3_start;

                    // Zone 1
                    DrawingHelpers.DrawLine(canvas, x_ww_h, y_ww_h, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_end, y_ww_h, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ww_h, 0.5 * (y_ph_ww_roof_z1_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB[0].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 2
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_end, y_ww_h, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z2_start, 0.5 * (y_ph_ww_roof_z2_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB[1].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 3
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_start, y_ww_h, x_ph_ww_roof_z3_start, y_ph_ww_roof_z3_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_end, y_ww_h, x_ph_ww_roof_z3_end, y_ph_ww_roof_z3_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_start, y_ph_ww_roof_z3_start, x_ph_ww_roof_z3_end, y_ph_ww_roof_z3_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z3_start, 0.5 * (y_ph_ww_roof_z3_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB[2].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Draw Pressure Dimensions
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z1_start, y_ww_h, x_ph_ww_roof_z1_end, y_ww_h, "Z1", 1.5 * PRESSURE_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_end, y_ww_h, "Z2", 1.5 * PRESSURE_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z3_start, y_ww_h, x_ph_ww_roof_z3_end, y_ww_h, "Z3", 1.5 * PRESSURE_TEXT_HT);
                }
                else
                {
                    x_ph_ww_roof_z1_end = x_ww_h + (0.5 * WindVM.Wind_Prov.Building.H) * SCALE_FACTOR;
                    y_ph_ww_roof_z1_end = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB[0] * PRESSURE_SCALE_FACTOR;

                    x_ph_ww_roof_z2_start = x_ph_ww_roof_z1_end;
                    y_ph_ww_roof_z2_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB[1] * PRESSURE_SCALE_FACTOR;
                    x_ph_ww_roof_z2_end = x_ww_h + WindVM.Wind_Prov.Building.H * SCALE_FACTOR;
                    y_ph_ww_roof_z2_end = y_ph_ww_roof_z2_start;

                    x_ph_ww_roof_z3_start = x_ph_ww_roof_z2_end;
                    y_ph_ww_roof_z3_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB[2] * PRESSURE_SCALE_FACTOR;
                    x_ph_ww_roof_z3_end = x_ww_h + 2.0 * WindVM.Wind_Prov.Building.H * SCALE_FACTOR;
                    y_ph_ww_roof_z3_end = y_ph_ww_roof_z3_start;

                    x_ph_ww_roof_z4_start = x_ph_ww_roof_z3_end;
                    y_ph_ww_roof_z4_start = y_ww_h - WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB[3] * PRESSURE_SCALE_FACTOR;
                    x_ph_ww_roof_z4_end = x_ww_h + WindVM.Wind_Prov.Building.L * SCALE_FACTOR;
                    y_ph_ww_roof_z4_end = y_ph_ww_roof_z4_start;

                    // Zone 1
                    DrawingHelpers.DrawLine(canvas, x_ww_h, y_ww_h, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_end, y_ww_h, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z1_start, y_ph_ww_roof_z1_start, x_ph_ww_roof_z1_end, y_ph_ww_roof_z1_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ww_h, 0.5 * (y_ph_ww_roof_z1_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB[0].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 2
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_end, y_ww_h, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z2_start, y_ph_ww_roof_z2_start, x_ph_ww_roof_z2_end, y_ph_ww_roof_z2_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z2_start, 0.5 * (y_ph_ww_roof_z2_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB[1].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 3
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_start, y_ww_h, x_ph_ww_roof_z3_start, y_ph_ww_roof_z3_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_end, y_ww_h, x_ph_ww_roof_z3_end, y_ph_ww_roof_z3_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z3_start, y_ph_ww_roof_z3_start, x_ph_ww_roof_z3_end, y_ph_ww_roof_z3_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z3_start, 0.5 * (y_ph_ww_roof_z3_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB[2].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Zone 4
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z4_start, y_ww_h, x_ph_ww_roof_z4_start, y_ph_ww_roof_z4_start, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z4_end, y_ww_h, x_ph_ww_roof_z4_end, y_ph_ww_roof_z4_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawLine(canvas, x_ph_ww_roof_z4_start, y_ph_ww_roof_z4_start, x_ph_ww_roof_z4_end, y_ph_ww_roof_z4_end, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                    DrawingHelpers.DrawText(canvas, x_ph_ww_roof_z4_start, 0.5 * (y_ph_ww_roof_z4_start + y_ww_h), 0, WindVM.Wind_Prov.P_H_ROOF_WW_PARALLEL_CASEB[3].ToString(), Brushes.Red, PRESSURE_TEXT_HT);

                    // Draw Pressure Dimensions
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z1_start, y_ww_h, x_ph_ww_roof_z1_end, y_ww_h, "Z1", 1.5 * PRESSURE_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z2_start, y_ww_h, x_ph_ww_roof_z2_end, y_ww_h, "Z2", 1.5 * PRESSURE_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z3_start, y_ww_h, x_ph_ww_roof_z3_end, y_ww_h, "Z3", 1.5 * PRESSURE_TEXT_HT);
                    DrawingHelpers.DrawDimensionAligned(canvas, x_ph_ww_roof_z4_start, y_ww_h, x_ph_ww_roof_z4_end, y_ww_h, "Z4", 1.5 * PRESSURE_TEXT_HT);
                }
            }
        }

    }
}
