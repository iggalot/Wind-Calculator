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
    }


}
