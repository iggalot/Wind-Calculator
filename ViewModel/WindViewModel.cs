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
        BuildingViewModel BuildingVM { get; set; }
        PressureViewModel PressureVM { get; set; }
        public WindViewModel(Canvas canvas, BuildingViewModel bldg_vm, PressureViewModel pressure_vm, WindProvisions wind_prov, WindOrientations orient, WindCasesDesignation wind_case)
        {
            DrawingCanvas = canvas;
            BuildingVM = bldg_vm;
            Wind_Prov = wind_prov;
            PressureVM = pressure_vm;

            WindOrientation = orient;
            WindCase = wind_case;
        }

        public void Draw()
        {
            double text_ht = 12;

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
