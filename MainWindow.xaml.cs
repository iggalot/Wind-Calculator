using ASCE7_10Library;
using DrawingHelpersLibrary;
using DrawingPipeline;
using DrawingPipeline.DirectX;
using DrawingPipelineLibrary.DirectX;
using SharpDX.Direct3D11;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Threading;
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
        private static bool bAppNeedsUpdate = false;
        //public const double PRESSURE_TEXT_HT = 10;
        //public const double STRUCTURE_DIM_TEXT_HT = 20;

        // Stores the last mouse point
        private Vector4 lastMousePoint { get; set; } = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);

        // Upper left corner of the UI window in pixels.
        public Point UIInsertPoint { get; set; } = new Point(0, 0);
        public static float UIWindowWidth { get; set; } = 800.0f;
        public static float UIWindowHeight { get; set; } = 800.0f;

        public static float DisplayWindowWidth { get; set; } = 600.0f;
        public static float DisplayWindowHeight { get; set; } = 600.0f;
        // Gridline model object
        public Gridlines GridlineModel { get; set; } = new Gridlines();

        /// <summary>
        /// The WindProvision view models for this graphic for a wind that is acting on the east face of the building.
        /// </summary>
        public WindViewModel WindVM_East_A { get; set; }
        public WindViewModel WindVM_East_B { get; set; }
        public WindViewModel WindVM_North_A { get; set; }
        public WindViewModel WindVM_North_B { get; set; }

        // Threads
        private Thread ThreadModel { get; set; }
        private Thread ThreadUI { get; set; }
        private Thread ThreadGraphics { get; set; }

        // Signals the application to quit
        public bool AppShouldShutdown { get; private set; } = false;
        public bool AppNeedsUpdate
        {
            get
            {
               // WriteToConsole(Thread.CurrentThread.Name + " has locked the mutex");
                return bAppNeedsUpdate;
            }
            set
            {
                // make changes
                ChangeAppNeedsUpdateStatus(value);
            }
        }

        internal void ChangeAppNeedsUpdateStatus(bool value)
        {
            // if nothing changed, no need to do anything.
            if (bAppNeedsUpdate == value)
            {
                //WriteToConsole("-- value not changed because it is already the same");
                return;
            }

            lock (this)
            {
                //WriteToConsole(Thread.CurrentThread.Name + " has locked app update to change for " + value.ToString());
                bAppNeedsUpdate = value;
                //WriteToConsole(Thread.CurrentThread.Name + " has released app update to new value " + bAppNeedsUpdate.ToString());
            }
            //// Otherwise lock the mutex to prevent a race condition
            //SemaphoreLock.WaitOne();
            //WriteToConsole(Thread.CurrentThread.Name + " has locked the semaphore");

            //// Toggle the flag
            //bAppNeedsUpdate = !bAppNeedsUpdate;
            //WriteToConsole(Thread.CurrentThread.Name + " has changed the flag condition");

            //// and unlock the mutex
            //SemaphoreLock.Release(1);
            //WriteToConsole(Thread.CurrentThread.Name + " has released the semaphore");

        }

        public int GaphicsRefreshTimer { get; set; } = 100;  // milliseconds between refresh checks
        public int ModelRefreshTimer { get; set; } = 100;  // milliseconds between model update checks
        public int UIRefreshTimer { get; set; } = 100;  // milliseconds between UI update checks
        public int AppUpdateTimer { get; set; } = 200; // milliseconds between application update

        // Flag to desginate whether directXX drawing should be used or more simple WPF canvas drawing
        public bool bIsDirectXEnabled { get; set; } = true;

        // The pipeline to use for rendering graphics.
        public BaseDrawingPipeline Pipeline { get; set; }
        public BuildingViewModel BuildingVM { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            // Add keyevent here
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);

            this.Left = 0.0f;
            this.Top = 0.0f;


            // Create threads
            // thread 1 -- Model and calculations
            // thread 2 -- UI handling
            // thread 3 -- Graphics / DirectX
            if(bIsDirectXEnabled)
            {
                ThreadGraphics = new Thread(InitializeDirectXGraphicsThread);
                ThreadGraphics.Name = "DirectX Graphics thread";
            } else
            {
                ThreadGraphics = new Thread(InitializeWPFGraphicsThread);
                ThreadGraphics.Name = "WPF Graphics thread";
            }

            ThreadUI = new Thread(InitializeUIThread);
            ThreadUI.Name = "Application UI Thread";

            ThreadModel = new Thread(InitializeModelThread);
            ThreadModel.Name = "Model thread";


            // Start the threads
            ThreadGraphics.Start();
            ThreadUI.Start();
            ThreadModel.Start();


            // Thread TESTING for updates and locking.
            //while(true)
            //{
            //    Thread.Sleep(AppUpdateTimer);
            //    WriteToConsole("Application needs updating....");
            //    WriteToConsole(Thread.CurrentThread.Name + " wants to change current value " + AppNeedsUpdate + " to TRUE.");
            //    AppNeedsUpdate = true;
            //    WriteToConsole(Thread.CurrentThread.Name + " has completed change to " + bAppNeedsUpdate.ToString());
            //}

            // Wait for the threads to finish
            //ThreadGraphics.Join();
            //ThreadUI.Join();
            //ThreadModel.Join();
        }

        /// <summary>
        /// Thread that controls creation of the model and calculations
        /// </summary>
        private void InitializeModelThread()
        {
            WriteToConsole("Initializing Model...");

            // Create the model info
            OnUserCreate();

            // Primary render loop
            while (!AppShouldShutdown)
            {
                // if the App needs Updating, lock the mutex and perform updating.
                while (AppNeedsUpdate)
                {
                    //                    Pipeline.Update();
                   // WriteToConsole(Thread.CurrentThread.Name + " wants to change current value " + AppNeedsUpdate + " to FALSE.");
                    AppNeedsUpdate = false;
                    //WriteToConsole(Thread.CurrentThread.Name + " has completed change to " + bAppNeedsUpdate.ToString());
                    continue;
                }

                Thread.Sleep(ModelRefreshTimer);
            }
        }

        /// <summary>
        /// Helper function to control thread safe writing to the console.
        /// </summary>
        /// <param name="v">string to be printed</param>
        private void WriteToConsole(string v)
        {
            lock (this)
            {
                Console.WriteLine(v);
            }
        }

        /// <summary>
        /// Thread that controls UI items
        /// </summary>
        private void InitializeUIThread()
        {
            WriteToConsole("Initializing UI...");

            // Primary render loop
            while (!AppShouldShutdown)
            {
                // if the App needs Updating, lock the mutex and perform updating.
                while (AppNeedsUpdate)
                {
                    //                    Pipeline.Update();
                    
                    //WriteToConsole(Thread.CurrentThread.Name + " wants to change current value " + AppNeedsUpdate + " to FALSE.");
                    AppNeedsUpdate = false;
                    //WriteToConsole(Thread.CurrentThread.Name + " has completed change to " + bAppNeedsUpdate.ToString());

                    continue;
                }

                Thread.Sleep(UIRefreshTimer);
            }
            //            Draw(MainCanvas);
            //            Draw(Canvas2);
            //            Draw(Canvas3);
            //            Draw(Canvas4);
        }

        /// <summary>
        /// Thread that controls graphics rendering
        /// </summary>
        private void InitializeDirectXGraphicsThread()
        {
            WriteToConsole("Initializing DirectX...");

            // Create the drawing pipeline to be used
            Pipeline = new DirectXDrawingPipeline((int)DisplayWindowWidth, (int)DisplayWindowHeight);

            

            Pipeline.RunPipeline();
            // Primary render loop
            while (!AppShouldShutdown)
            {
                // if the App needs Updating, lock the mutex and perform updating.
                while(AppNeedsUpdate)
                {
                    //                    Pipeline.Update();
                    //WriteToConsole(Thread.CurrentThread.Name + " wants to change current value " + AppNeedsUpdate + " to FALSE.");
                    AppNeedsUpdate = false;
                    //WriteToConsole(Thread.CurrentThread.Name + " has completed change to " + bAppNeedsUpdate.ToString());
                    continue;
                }

                Thread.Sleep(GaphicsRefreshTimer);
            }
        }

        private void InitializeWPFGraphicsThread()
        {
            WriteToConsole("Initializing WPF Graphics...");

            Pipeline = new CanvasDrawingPipeline(MainCanvas, (int)(DisplayWindowWidth + UIWindowWidth), (int)(DisplayWindowHeight + UIWindowHeight));
            MainCanvas.Width = DisplayWindowWidth;
            MainCanvas.Height = DisplayWindowHeight;

            while (!AppShouldShutdown)
            {
                while (AppNeedsUpdate)
                {
                    //Pipeline.Update();
                    //WriteToConsole(Thread.CurrentThread.Name + " wants to change current value " + AppNeedsUpdate + " to FALSE.");
                    AppNeedsUpdate = false;
                    //WriteToConsole(Thread.CurrentThread.Name + " has completed change to " + bAppNeedsUpdate.ToString());
                    continue;
                }

                Thread.Sleep(GaphicsRefreshTimer);
            }
        }

        /// <summary>
        /// Routine that runs everytime it is called (once per frame?  after user input?)
        /// </summary>
        private void OnUserUpdate()
        {
            // Update the view models
            //WindVM_East_A.Update();
            //WindVM_East_B.Update();
            //WindVM_North_A.Update();
            //WindVM_North_B.Update();

            //// Clear the canvas
            //MainCanvas.Children.Clear();
            //Canvas2.Children.Clear();
            //Canvas3.Children.Clear();
            //Canvas4.Children.Clear();


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
            float wall_ht = 100;  // wall height
            float b = 100; // length perpendicular to wind
            float l = 100; // length parallel to wind

            // Create a building object
            BuildingModel bldg1 = new BuildingModel(l, b, wall_ht);
            BuildingVM = new BuildingViewModel(bldg1);

            // Frame 1
            Vector4 ww_wall_1 = new Vector4(0.0f, (float)wall_ht, 0, 1.0f);
            Vector4 lw_wall_1 = new Vector4(100.0f, (float)wall_ht, 0, 1.0f);
            Vector4 ridge_1 = new Vector4(50.0f, (float)(wall_ht + 50.0f), 0, 1.0f);

            // Frame 2
            Vector4 ww_wall_2 = new Vector4(0.0f, (float)wall_ht, (float)b, 1.0f);
            Vector4 lw_wall_2 = new Vector4(100.0f, (float)wall_ht, (float)b, 1.0f);
            Vector4 ridge_2 = new Vector4(50.0f, (float)(wall_ht + 50.0f), (float)b, 1.0f);

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
 //           WindVM_East_A = CreateWindViewModels(MainCanvas, bldg_East, wind_prov_east, WindOrientations.WIND_ORIENTATION_NORMALTORIDGE, WindCasesDesignation.WIND_CASE_A);
 //           WindVM_East_B = CreateWindViewModels(Canvas2, bldg_East, wind_prov_east, WindOrientations.WIND_ORIENTATION_NORMALTORIDGE, WindCasesDesignation.WIND_CASE_B);
 //           WindVM_North_A = CreateWindViewModels(Canvas3, bldg_North, wind_prov_north, WindOrientations.WIND_ORIENTATION_PARALLELTORIDGE, WindCasesDesignation.WIND_CASE_A);
 //           WindVM_North_B = CreateWindViewModels(Canvas4, bldg_North, wind_prov_north, WindOrientations.WIND_ORIENTATION_PARALLELTORIDGE, WindCasesDesignation.WIND_CASE_B);
        }

        private WindViewModel CreateWindViewModels(Canvas canvas, BuildingInfo bldg, WindProvisions wind_prov, WindOrientations orient, WindCasesDesignation wind_case)
        {
            string title = (orient == WindOrientations.WIND_ORIENTATION_NORMALTORIDGE ? "Normal " : " Parallel ") + "to Ridge - Case " + (wind_case == WindCasesDesignation.WIND_CASE_A ? "A" : "B");

            // Set initial scale factors
            double SCALE_FACTOR_HORIZ = 0.6 * canvas.Width / bldg.L;
            double SCALE_FACTOR_VERT = 0.6 * canvas.Height / bldg.H;
            double SCALE_FACTOR = Math.Min(SCALE_FACTOR_HORIZ, SCALE_FACTOR_VERT);

            // create our camera object
            Camera camera = new Camera(canvas, (float)0, 0, -200, 0, 1, 0, 90, 0);
    
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

            //// Center point of current canvas
            //double x_center = canvas.Width * 0.5;
            //double y_center = canvas.Height * 0.5;

            //// Draw the centerlines of the canvas
            //DrawingHelpers.DrawLine(canvas, x_center, 0, x_center, 2.0 * y_center, Brushes.BlueViolet, 1, Linetypes.LINETYPE_PHANTOM);
            //DrawingHelpers.DrawLine(canvas, 0, y_center, 2.0 * x_center, y_center, Brushes.BlueViolet, 1, Linetypes.LINETYPE_PHANTOM);

            //// Draw the camera state message
            //string cam_str = (WindVM_East_A.BuildingVM.CameraObj.IsActive ? "ON" : "OFF");
            //DrawingHelpers.DrawText(canvas, canvas.Width - 100, canvas.Height - 12, 0.0, "CAMERA: " + cam_str, Brushes.Black, 12);

            //// Draw the drawing objects
            //WindVM_East_A.Draw();
            ////WindVM_East_B.Draw();
            ////WindVM_North_A.Draw();
            ////WindVM_North_B.Draw();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Send the keystroke to the pipeline for processing
            Pipeline.SetKeyState(e.Key, true);
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            //// if our  camera state isn't active do nothing
            //if (WindVM_East_A.BuildingVM.CameraObj.IsActive == false)
            //{
            //    e.Handled = true;
            //    return;
            //}

            //Point pt = Mouse.GetPosition(MainCanvas);
            //Vector4 currentPoint = new Vector4((float)(pt.X), (float)(pt.Y), 0.0f, 0.0f);

            //Vector4 distMoved = Vector4.Subtract(currentPoint, lastMousePoint);

            //WindVM_East_A.BuildingVM.CameraObj.ProcessMouseMovement(distMoved.X, distMoved.Y, true);
            //e.Handled = true;

            //lastMousePoint = currentPoint;

            //OnUserUpdate();
        }

        private void MainCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //CameraMovementDirections dir;
            //float deltaTime = 5.0f;
            //if (WindVM_East_A.BuildingVM.CameraObj.IsActive)
            //{
            //    dir = (e.Delta > 0) ? CameraMovementDirections.FORWARD : CameraMovementDirections.BACKWARD;
            //    WindVM_East_A.BuildingVM.CameraObj.ProcessKeyboard(dir, deltaTime);
            //    WindVM_East_A.BuildingVM.CameraObj.Update();
            //}

            //OnUserUpdate();
        }

        private void Model1_Click(object sender, RoutedEventArgs e)
        {
            CreateModel1();
        }

        private void CreateModel1()
        {
            // Clear the models
            ((DirectXDrawingPipeline)Pipeline).GetDSystem.Graphics.ModelList.Clear();

            // Create the gridlines
            GridlineModel = new Gridlines();
            GridlineModel.CreateModel((DirectXDrawingPipeline)Pipeline);
            ((DirectXDrawingPipeline)Pipeline).GetDSystem.Graphics.AddModel(GridlineModel.Model);

            DModel model = new DModel();
            model.InitializeBufferTestTriangle(((DirectXDrawingPipeline)Pipeline).GetDSystem.Graphics.D3D.Device, ModelElementTypes.MODEL_ELEMENT_TRIANGLE);
            ((DirectXDrawingPipeline)Pipeline).GetDSystem.Graphics.AddModel(model);
        }

        private void CreateModel2()
        {
            // Clear the models
            ((DirectXDrawingPipeline)Pipeline).GetDSystem.Graphics.ModelList.Clear();

            // Create the gridlines
            GridlineModel = new Gridlines();
            GridlineModel.CreateModel((DirectXDrawingPipeline)Pipeline);
            ((DirectXDrawingPipeline)Pipeline).GetDSystem.Graphics.AddModel(GridlineModel.Model);

            DModel model = new DModel();
            model.InitializeBufferTestTriangle(((DirectXDrawingPipeline)Pipeline).GetDSystem.Graphics.D3D.Device, ModelElementTypes.MODEL_ELEMENT_LINE);
            ((DirectXDrawingPipeline)Pipeline).GetDSystem.Graphics.AddModel(model);

            model = new DModel();
            model.InitializeBuffer(((DirectXDrawingPipeline)Pipeline).GetDSystem.Graphics.D3D.Device, ModelElementTypes.MODEL_ELEMENT_LINE);
            ((DirectXDrawingPipeline)Pipeline).GetDSystem.Graphics.AddModel(model);

        }

        private void CreateModel3()
        {
            // Clear the models
            ((DirectXDrawingPipeline)Pipeline).GetDSystem.Graphics.ModelList.Clear();

            // Create the gridlines
            GridlineModel = new Gridlines();
            GridlineModel.CreateModel((DirectXDrawingPipeline)Pipeline);
            ((DirectXDrawingPipeline)Pipeline).GetDSystem.Graphics.AddModel(GridlineModel.Model);

            DModel model = new DModel();
            model = BuildingVM.CreateModel((DirectXDrawingPipeline)Pipeline, ModelElementTypes.MODEL_ELEMENT_TRIANGLE);
            ((DirectXDrawingPipeline)Pipeline).GetDSystem.Graphics.AddModel(model);

            model = new DModel();
            model.InitializeBufferTestTriangle(((DirectXDrawingPipeline)Pipeline).GetDSystem.Graphics.D3D.Device, ModelElementTypes.MODEL_ELEMENT_LINE);
            ((DirectXDrawingPipeline)Pipeline).GetDSystem.Graphics.AddModel(model);

            model = new DModel();
            model.InitializeBuffer(((DirectXDrawingPipeline)Pipeline).GetDSystem.Graphics.D3D.Device, ModelElementTypes.MODEL_ELEMENT_LINE);
            ((DirectXDrawingPipeline)Pipeline).GetDSystem.Graphics.AddModel(model);

            //BuildingVM.Render(true, Pipeline);
        }

        private void Model2_Click(object sender, RoutedEventArgs e)
        {
            CreateModel2();
        }

        private void Model3_Click(object sender, RoutedEventArgs e)
        {
            CreateModel3();
        }
    }
}
