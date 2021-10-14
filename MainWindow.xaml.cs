using ASCE7_10Library;
using DrawingHelpersLibrary;
using System;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WindCalculator.Model;
using WindCalculator.ViewModel;

namespace WindCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public const double PRESSURE_TEXT_HT = 10;
        //public const double STRUCTURE_DIM_TEXT_HT = 20;

        /// <summary>
        /// The WindProvision view models for this graphic for a wind that is acting on the east face of the building.
        /// </summary>
        public WindViewModel WindVM_East_A { get; set; }
        public WindViewModel WindVM_East_B { get; set; }
        public WindViewModel WindVM_North_A { get; set; }
        public WindViewModel WindVM_North_B { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            // Add keyevent here
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
            OnUserCreate();
        }



        /// <summary>
        /// Routine that runs everytime it is called (once per frame?  after user input?)
        /// </summary>
        private void OnUserUpdate()
        {
            // Update the view models
            WindVM_East_A.Update();
            WindVM_East_B.Update();
            WindVM_North_A.Update();
            WindVM_North_B.Update();

            // Clear the canvas
            MainCanvas.Children.Clear();
            Canvas2.Children.Clear();
            Canvas3.Children.Clear();
            Canvas4.Children.Clear();

            Draw(MainCanvas);
            Draw(Canvas2);
            Draw(Canvas3);
            Draw(Canvas4);
        }

        /// <summary>
        /// Routine that runs only once when the program is first executed
        /// </summary>
        private void OnUserCreate()
        {
            ExposureCategories exp = ExposureCategories.B;
            double V = 115;   // mph

            // building dimension constants
            RiskCategories risk_cat = RiskCategories.II;
            double wall_ht = 35;  // wall height
            double b = 50; // length perpendicular to wind

            // Frame 1
            Vector4 ww_wall_1 = new Vector4(0.0f, (float)wall_ht, 0, 1.0f);
            Vector4 lw_wall_1 = new Vector4(100.0f, (float)wall_ht, 0, 1.0f);
            Vector4 ridge_1 = new Vector4(50.0f, (float)(wall_ht + 50.0f), 0, 1.0f);

            // Frame 2
            Vector4 ww_wall_2 = new Vector4(0.0f, (float)wall_ht, (float)b, 1.0f);
            Vector4 lw_wall_2 = new Vector4(100.0f, (float)wall_ht, (float)b, 1.0f);
            Vector4 ridge_2 = new Vector4(50.0f, (float)(wall_ht + 50.0f), (float)b, 1.0f);

            // Create a building object
            // Profile of the roof line
            // TODO:: Need to sort the order of these points or provide some sort of logic (left-to-right) progression of points

            ////TESTING: Sloped roof profile
            //Vector4[] profile_east_1 = new Vector4[] { ww_wall_1, ridge_1, lw_wall_1 };
            //Vector4[] profile_east_2 = new Vector4[] { ww_wall_2, ridge_2, lw_wall_2 };

            // TESTING: Flat roof profile
            Vector4[] profile_east_1 = new Vector4[] { ww_wall_1, lw_wall_1 };
            Vector4[] profile_east_2 = new Vector4[] { ww_wall_2, lw_wall_2 };

            Vector4[] profile_north_1 = new Vector4[] { ww_wall_1, ww_wall_2 };
            Vector4[] profile_north_2 = new Vector4[] { lw_wall_1, lw_wall_2 };

            //BuildingInfo bldg_East = new SlopedRoofBuildingInfo(profile_east_1, profile_east_2, RoofSlopeTypes.ROOF_SLOPE_SINGLERIDGE, risk_cat);
            //BuildingInfo bldg_North = new SlopedRoofBuildingInfo(profile_north_1, profile_north_2, RoofSlopeTypes.ROOF_SLOPE_FLAT, risk_cat);

            ////TESTING: flat roof profile
            BuildingInfo bldg_East = new BuildingInfo(profile_east_1, profile_east_2, RoofSlopeTypes.ROOF_SLOPE_FLAT, risk_cat, WindOrientations.WIND_ORIENTATION_NORMALTORIDGE);
            BuildingInfo bldg_North = new BuildingInfo(profile_north_1, profile_north_2, RoofSlopeTypes.ROOF_SLOPE_FLAT, risk_cat, WindOrientations.WIND_ORIENTATION_PARALLELTORIDGE);

            // Create the wind provision models for wind in the east (perpendicular to ridge) and in the north (parallel to ridge)
            WindProvisions wind_prov_east = new WindProvisions(V, bldg_East, exp);
            WindProvisions wind_prov_north = new WindProvisions(V, bldg_North, exp);

            // Create our viewmodels
            WindVM_East_A = CreateWindViewModels(MainCanvas, bldg_East, wind_prov_east, WindOrientations.WIND_ORIENTATION_NORMALTORIDGE, WindCasesDesignation.WIND_CASE_A);
            WindVM_East_B = CreateWindViewModels(Canvas2, bldg_East, wind_prov_east, WindOrientations.WIND_ORIENTATION_NORMALTORIDGE, WindCasesDesignation.WIND_CASE_B);
            WindVM_North_A = CreateWindViewModels(Canvas3, bldg_North, wind_prov_north, WindOrientations.WIND_ORIENTATION_PARALLELTORIDGE, WindCasesDesignation.WIND_CASE_A);
            WindVM_North_B = CreateWindViewModels(Canvas4, bldg_North, wind_prov_north, WindOrientations.WIND_ORIENTATION_PARALLELTORIDGE, WindCasesDesignation.WIND_CASE_B);
        }

        private WindViewModel CreateWindViewModels(Canvas canvas, BuildingInfo bldg, WindProvisions wind_prov, WindOrientations orient, WindCasesDesignation wind_case)
        {
            string title = (orient == WindOrientations.WIND_ORIENTATION_NORMALTORIDGE ? "Normal " : " Parallel ") + "to Ridge - Case " + (wind_case == WindCasesDesignation.WIND_CASE_A ? "A" : "B");

            // Set initial scale factors
            double SCALE_FACTOR_HORIZ = 0.6 * canvas.Width / bldg.L;
            double SCALE_FACTOR_VERT = 0.6 * canvas.Height / bldg.H;
            double SCALE_FACTOR = Math.Min(SCALE_FACTOR_HORIZ, SCALE_FACTOR_VERT);

            // create our camera object
            Camera camera = new Camera(canvas, 5, 0, 500.0f, 0, 1, 0, 90, 0);
    
            BuildingViewModel bldg_vm = new BuildingViewModel(canvas, camera, bldg, SCALE_FACTOR, orient);
            PressureViewModel pressure_vm = new PressureViewModel(canvas, wind_prov, bldg_vm, title, orient, wind_case);

            return new WindViewModel(canvas, bldg_vm, pressure_vm, wind_prov, orient, wind_case);
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
        /// the main drawing function for the canvas object
        /// </summary>
        /// <param name="canvas"></param>
        protected void Draw(Canvas canvas)
        {

            canvas.Children.Clear();

            // Center point of current canvas
            double x_center = canvas.Width * 0.5;
            double y_center = canvas.Height * 0.5;

            // Draw the centerlines of the canvas
            DrawingHelpers.DrawLine(canvas, x_center, 0, x_center, 2.0 * y_center, Brushes.BlueViolet, 1, Linetypes.LINETYPE_PHANTOM);
            DrawingHelpers.DrawLine(canvas, 0, y_center, 2.0 * x_center, y_center, Brushes.BlueViolet, 1, Linetypes.LINETYPE_PHANTOM);

            // Draw the drawing objects
            WindVM_East_A.Draw();
            //WindVM_East_B.Draw();
            //WindVM_North_A.Draw();
            //WindVM_North_B.Draw();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.W)
            {
                WindVM_East_A.BuildingVM.CameraObj.CameraPosition += new Vector4(0, 0, -25, 1);
                WindVM_East_A.BuildingVM.CameraObj.Update();
            }

            if (e.Key == System.Windows.Input.Key.S)
            {
                WindVM_East_A.BuildingVM.CameraObj.CameraPosition += new Vector4(0, 0, 25, 1);
                WindVM_East_A.BuildingVM.CameraObj.Update();
            }

            if (e.Key == System.Windows.Input.Key.A)
            {
                WindVM_East_A.BuildingVM.CameraObj.CameraPosition += new Vector4(-25, 0, 0, 1);
                WindVM_East_A.BuildingVM.CameraObj.Update();
            }

            if (e.Key == System.Windows.Input.Key.D)
            {
                WindVM_East_A.BuildingVM.CameraObj.CameraPosition += new Vector4(25, 0, 0, 1);
                WindVM_East_A.BuildingVM.CameraObj.Update();
            }
            if (e.Key == System.Windows.Input.Key.Space)
            {
                WindVM_East_A.BuildingVM.CameraObj.CameraPosition += new Vector4(0, 25, 0, 1);
                WindVM_East_A.BuildingVM.CameraObj.Update();
            }

            if (e.Key == System.Windows.Input.Key.R)
            {
                WindVM_East_A.BuildingVM.CameraObj.CameraPosition = WindVM_East_A.BuildingVM.CameraObj.OriginalPosition;
                WindVM_East_A.BuildingVM.CameraObj.Update();
            }

            OnUserUpdate();
        }
    }
}
