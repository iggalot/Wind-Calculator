using ASCE7_10Library;
using DrawingHelpersLibrary;
using System;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Media;

namespace WindCalculator.ViewModel
{
    public class PressureViewModel
    {
        const double DEFAULT_TITLE_HT = 20;

        WindProvisions WindProv { get; set; }
        BuildingViewModel BuildingVM { get; set; }

        public double PRESSURE_SCALE_FACTOR = 1.0;

        string Title { get; set; }


        /// <summary>
        /// Wall pressure screen coords
        /// </summary>
        public Vector4 WW_P0_1_SC { get; set; }
        public Vector4 WW_P15_1_SC { get; set; }
        public Vector4 WW_PH_1_SC { get; set; }

        public Vector4 LW_P0_1_SC { get; set; }
        public Vector4 LW_P15_1_SC { get; set; }
        public Vector4 LW_PH_1_SC { get; set; }

        public double[] RoofCaseA { get; set; }
        public double[] RoofCaseB { get; set; }

        //public Vector4 ROOF_WW_Z1_START_SC { get; set; }
        //public Vector4 ROOF_WW_Z1_END_SC { get; set; }

        //public Vector4 ROOF_LW_Z1_START_SC { get; set; }
        //public Vector4 ROOF_LW_Z1_END_SC { get; set; }

        public PressureViewModel(Canvas canvas, WindProvisions wind_prov, BuildingViewModel bldg_vm, string title, WindOrientations orient, WindCasesDesignation wind_case)
        {
            //PRESSURE_SCALE_FACTOR = 0.6 * bldg_vm.SCALE_FACTOR;

            //WindProv = wind_prov;
            //BuildingVM = bldg_vm;
            //Title = title;

            //UpdatePressureScreenCoords();
        }

        public void UpdatePressureScreenCoords()
        {
            BuildingViewModel bldg_vm = BuildingVM;
            WindProvisions wind_prov = WindProv;

            //// Dynamic pressure q coordinates for WW
            //WW_P0_1_SC = new Vector4(bldg_vm.WW_GRD_1_SC.X - (float)(wind_prov.P_0_WW * PRESSURE_SCALE_FACTOR), bldg_vm.WW_GRD_1_SC.Y, 0.0f, 1.0f);
            //WW_P15_1_SC = new Vector4(bldg_vm.WW_15_1_SC.X - (float)(wind_prov.P_15_WW * PRESSURE_SCALE_FACTOR), bldg_vm.WW_15_1_SC.Y, 0.0f, 1.0f);
            //WW_PH_1_SC = new Vector4(bldg_vm.WW_H_1_SC.X - (float)(wind_prov.P_H_WW * PRESSURE_SCALE_FACTOR), bldg_vm.WW_H_1_SC.Y, 0.0f, 1.0f);

            //LW_P0_1_SC = new Vector4(bldg_vm.LW_GRD_1_SC.X + (float)(wind_prov.P_H_LW * PRESSURE_SCALE_FACTOR), bldg_vm.LW_GRD_1_SC.Y, 0.0f, 1.0f);
            //LW_P15_1_SC = new Vector4(bldg_vm.LW_15_1_SC.X + (float)(wind_prov.P_H_LW * PRESSURE_SCALE_FACTOR), bldg_vm.LW_15_1_SC.Y, 0.0f, 1.0f);
            //LW_PH_1_SC = new Vector4(bldg_vm.LW_H_1_SC.X + (float)(wind_prov.P_H_LW * PRESSURE_SCALE_FACTOR), bldg_vm.LW_H_1_SC.Y, 0.0f, 1.0f);

        }

        public void Update()
        {
            UpdatePressureScreenCoords();
        }

        public void Draw(Canvas canvas, double pressure_text_ht)
        {
//            string pressure_str;

//            // Draws the title for each picture based on the roof pressure diagram being generated.
//            DrawingHelpers.DrawText(canvas, BuildingVM.ORIGIN_1_SC.X - 130, BuildingVM.ORIGIN_1_SC.Y + 50, 0, Title, Brushes.Black, DEFAULT_TITLE_HT);


//            /////////////////////
//            // DRAW WALL PRESSURES
//            /////////////////////
//            // Windward
//            // p0 pressure line
//            DrawingHelpers.DrawLine(canvas, WW_P0_1_SC.X, WW_P0_1_SC.Y, BuildingVM.WW_GRD_1_SC.X,BuildingVM.WW_GRD_1_SC.Y, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
//            pressure_str = (Math.Round(WindProv.P_0_WW * 100.0) / 100.0).ToString();
//            DrawingHelpers.DrawText(canvas, WW_P0_1_SC.X, WW_P0_1_SC.Y, 0, pressure_str, Brushes.Blue, pressure_text_ht);

//            // Ph pressure line
//            DrawingHelpers.DrawLine(canvas, WW_PH_1_SC.X, WW_PH_1_SC.Y, BuildingVM.WW_H_1_SC.X, BuildingVM.WW_H_1_SC.Y, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
//            pressure_str = (Math.Round(WindProv.P_H_WW * 100.0) / 100.0).ToString();
//            DrawingHelpers.DrawText(canvas, WW_PH_1_SC.X, WW_PH_1_SC.Y, 0, pressure_str, Brushes.Blue, pressure_text_ht);

//            // show the 15' WW wall pressure location if necessary.
//            if (WindProv.Building.H > 15)
//            {
//                // Negative here pulls the dimension to the left
//                DrawingHelpers.DrawDimensionAligned(canvas, BuildingVM.WW_15_1_SC.X, BuildingVM.WW_15_1_SC.Y, BuildingVM.WW_GRD_1_SC.X, BuildingVM.WW_GRD_1_SC.Y, "15'", pressure_text_ht,
//                    -30, -0.3, -5, -10, DrawingHelpers.DEFAULT_DIM_LEADER_COLOR, DrawingHelpers.DEFAULT_DIM_LINETYPE);

//                // P15 pressure line
//                DrawingHelpers.DrawLine(canvas, WW_P15_1_SC.X, WW_P15_1_SC.Y, BuildingVM.WW_15_1_SC.X, BuildingVM.WW_15_1_SC.Y, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
//                pressure_str = (Math.Round(WindProv.P_15_WW * 100.0) / 100.0).ToString();
//                DrawingHelpers.DrawText(canvas, WW_P15_1_SC.X, WW_P15_1_SC.Y, 0, pressure_str.ToString(), Brushes.Blue, pressure_text_ht);

//                // Draw the two pressure lines
//                DrawingHelpers.DrawLine(canvas, WW_P0_1_SC.X, WW_P0_1_SC.Y, WW_P15_1_SC.X, WW_P15_1_SC.Y, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
//                DrawingHelpers.DrawLine(canvas, WW_P15_1_SC.X, WW_P15_1_SC.Y, WW_PH_1_SC.X, WW_PH_1_SC.Y, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
//            } else
//            {
//                // Draw the 2D pressure line for this elevation view
//                DrawingHelpers.DrawLine(canvas, WW_P0_1_SC.X, WW_P0_1_SC.Y, WW_PH_1_SC.X, WW_PH_1_SC.Y, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
//            }

//            // Leeward
//            // p0 pressure line
//            DrawingHelpers.DrawLine(canvas, LW_P0_1_SC.X, LW_P0_1_SC.Y, BuildingVM.LW_GRD_1_SC.X, BuildingVM.LW_GRD_1_SC.Y, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);

//            // Ph pressure line
//            DrawingHelpers.DrawLine(canvas, LW_PH_1_SC.X, LW_PH_1_SC.Y, BuildingVM.LW_H_1_SC.X, BuildingVM.LW_H_1_SC.Y, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
//            pressure_str = (Math.Round(WindProv.P_H_LW * 100.0) / 100.0).ToString();
//            DrawingHelpers.DrawText(canvas, LW_PH_1_SC.X, LW_PH_1_SC.Y, 0, pressure_str, Brushes.Blue, pressure_text_ht);

//            //Draw the pressure lines
//            DrawingHelpers.DrawLine(canvas, LW_P0_1_SC.X, LW_P0_1_SC.Y, LW_PH_1_SC.X, LW_PH_1_SC.Y, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);


//            /////////////////////
//            // DRAW ROOF PRESSURES
//            /////////////////////
//            // Is it a flat roof scenario?
////            if(BuildingVM.Model.HasSingleRidge == false)
// //           {
//                //DrawPressureRoof(Canvas canvas);

////            } else
////            {
////                DrawingHelpers.DrawText(canvas, BuildingVM.ORIGIN_1_SC.X - 130, BuildingVM.ORIGIN_1_SC.Y - 50, 0, "No Calcs Available for roof", Brushes.Red, DEFAULT_TITLE_HT);

////            }
        }
        //public override void DrawPressureRoof(Canvas canvas, double[] arr, BuildingViewModel bldg_vm, PressureViewModel press_vm)
        //{

        //}
        /// <summary>
        /// Function to draw the roof pressures
        /// </summary>
        /// <param name="canvas"></param>
        private void DrawRoofPressure_Elevation(Canvas canvas)
        {
 //           DrawPressureRoof(canvas, Wind_Prov, BuildingVM, PressureVM);
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
        public virtual void DrawPressureRoof(Canvas canvas, double[] arr, BuildingViewModel bldg_vm, PressureViewModel press_vm)
        {

            //for (int i = 0; i < WindVM_East.Bldg.RoofZonePts_1.Length / 2.0; i++)
            //{
            //    double y_p = arr[i] * press_vm.PRESSURE_SCALE_FACTOR;

            //    //// If the region has zero length, skip to the next region and don't illustrate it
            //    //if (x_p1 == x_p2)
            //    //{
            //    //    continue;
            //    //}

            //    // create our pressure label
            //    string pressure_str = (Math.Round(arr[i] * 100.0) / 100.0).ToString();

            //    DrawingHelpers.DrawRectangleAligned_Base(canvas, bldg_vm.RoofZonePoints_1_SC[2 * i].X,
            //        bldg_vm.RoofZonePoints_1_SC[2 * i].Y,
            //        bldg_vm.RoofZonePoints_1_SC[2 * i + 1].X,
            //        bldg_vm.RoofZonePoints_1_SC[2 * i + 1].Y,
            //        y_p, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
            //    DrawingHelpers.DrawText(canvas, bldg_vm.RoofZonePoints_1_SC[2 * i].X, bldg_vm.RoofZonePoints_1_SC[2 * i].Y - y_p, 0, pressure_str, Brushes.Red, PRESSURE_TEXT_HT);

            //    DrawingHelpers.DrawDimensionAligned(canvas,
            //        bldg_vm.RoofZonePoints_1_SC[2 * i].X,
            //        bldg_vm.RoofZonePoints_1_SC[2 * i].Y,
            //        bldg_vm.RoofZonePoints_1_SC[2 * i + 1].X,
            //        bldg_vm.RoofZonePoints_1_SC[2 * i + 1].Y,
            //        "Z" + (i + 1).ToString(), 10);
            //}
        }
    }
}
