using ASCE7_10Library;
using System.Windows.Controls;
using System.Windows.Media;

namespace WindCalculator.ViewModel
{
    public class WindViewModel
    {
        Canvas DrawingCanvas { get; set; }
        WindOrientations WindOrientation { get; set; }
        WindCasesDesignation WindCase { get; set; }
        public WindProvisions Wind_Prov { get; set; }
        public BuildingViewModel BuildingVM { get; set; }
        public PressureViewModel PressureVM { get; set; }
        public WindViewModel(Canvas canvas, BuildingViewModel bldg_vm, PressureViewModel pressure_vm, WindProvisions wind_prov, WindOrientations orient, WindCasesDesignation wind_case)
        {
            DrawingCanvas = canvas;
            BuildingVM = bldg_vm;
            Wind_Prov = wind_prov;
            PressureVM = pressure_vm;

            WindOrientation = orient;
            WindCase = wind_case;
        }

        public void Update()
        {
            BuildingVM.Update();
            PressureVM.Update();
        }

        public void Draw()
        {
            double text_ht = 12;


            // Draw camera information
            int i = 8; // number of items to display
            DrawingHelpersLibrary.DrawingHelpers.DrawText(DrawingCanvas, 0, DrawingCanvas.Height - text_ht * (i - 0), 0, "CAMERA: ", Brushes.Black, text_ht);
            DrawingHelpersLibrary.DrawingHelpers.DrawText(DrawingCanvas, 0, DrawingCanvas.Height - text_ht * (i - 1), 0, "POS <X,Y,Z>: " + BuildingVM.CameraObj.CameraPosition.X.ToString() + ", " + BuildingVM.CameraObj.CameraPosition.Y.ToString() + ", " + BuildingVM.CameraObj.CameraPosition.Z.ToString(), Brushes.Black, text_ht);
            DrawingHelpersLibrary.DrawingHelpers.DrawText(DrawingCanvas, 0, DrawingCanvas.Height - text_ht * (i - 2), 0, "TARGET <X,Y,Z>: " + BuildingVM.CameraObj.CameraTarget.X.ToString() + ", " + BuildingVM.CameraObj.CameraTarget.Y.ToString() + ", " + BuildingVM.CameraObj.CameraTarget.Z.ToString(), Brushes.Black, text_ht);
            DrawingHelpersLibrary.DrawingHelpers.DrawText(DrawingCanvas, 0, DrawingCanvas.Height - text_ht * (i - 3), 0, "YAW: " + BuildingVM.CameraObj.Yaw.ToString(), Brushes.Black, text_ht);
            DrawingHelpersLibrary.DrawingHelpers.DrawText(DrawingCanvas, 0, DrawingCanvas.Height - text_ht * (i - 4), 0, "PITCH: " + BuildingVM.CameraObj.Pitch.ToString(), Brushes.Black, text_ht);
            DrawingHelpersLibrary.DrawingHelpers.DrawText(DrawingCanvas, 0, DrawingCanvas.Height - text_ht * (i - 5), 0, "FRONT <X,Y,Z>: " + BuildingVM.CameraObj.CameraFront.X.ToString() + ", " + BuildingVM.CameraObj.CameraFront.Y.ToString() + ", " + BuildingVM.CameraObj.CameraFront.Z.ToString(), Brushes.Black, text_ht);
            DrawingHelpersLibrary.DrawingHelpers.DrawText(DrawingCanvas, 0, DrawingCanvas.Height - text_ht * (i - 6), 0, "RIGHT <X,Y,Z>: " + BuildingVM.CameraObj.CameraRight.X.ToString() + ", " + BuildingVM.CameraObj.CameraRight.Y.ToString() + ", " + BuildingVM.CameraObj.CameraRight.Z.ToString(), Brushes.Black, text_ht);
            DrawingHelpersLibrary.DrawingHelpers.DrawText(DrawingCanvas, 0, DrawingCanvas.Height - text_ht * (i - 7), 0, "UP <X,Y,Z>: " + BuildingVM.CameraObj.CameraUp.X.ToString() + ", " + BuildingVM.CameraObj.CameraUp.Y.ToString() + ", " + BuildingVM.CameraObj.CameraUp.Z.ToString(), Brushes.Black, text_ht);


            // Draw general information
            DrawingHelpersLibrary.DrawingHelpers.DrawText(DrawingCanvas, 0, 0, 0, "SCALE: " + BuildingVM.SCALE_FACTOR.ToString(), Brushes.Black, text_ht);
            DrawingHelpersLibrary.DrawingHelpers.DrawText(DrawingCanvas, 0, text_ht, 0, "L: " + BuildingVM.Model.L.ToString(), Brushes.Black, text_ht);
            DrawingHelpersLibrary.DrawingHelpers.DrawText(DrawingCanvas, 0, 2.0 * text_ht, 0, "B: " + BuildingVM.Model.B.ToString(), Brushes.Black, text_ht);
            DrawingHelpersLibrary.DrawingHelpers.DrawText(DrawingCanvas, 0, 3.0 * text_ht, 0, "H: " + BuildingVM.Model.H.ToString(), Brushes.Black, text_ht);



            BuildingVM.Draw(DrawingCanvas, 10);
            PressureVM.Draw(DrawingCanvas, 10);
            //PressureVM.Draw(DrawingCanvas);
        }
  
    }


}
