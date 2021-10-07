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


        public Vector4 WW_P0_1_SC { get; set; }
        public Vector4 WW_P15_1_SC { get; set; }
        public Vector4 WW_PH_1_SC { get; set; }

        public Vector4 LW_P0_1_SC { get; set; }
        public Vector4 LW_P15_1_SC { get; set; }
        public Vector4 LW_PH_1_SC { get; set; }


        public PressureViewModel(Canvas canvas, WindProvisions wind_prov, BuildingViewModel bldg_vm, string title)
        {
            PRESSURE_SCALE_FACTOR = 0.6 * bldg_vm.SCALE_FACTOR;

            // Dynamic pressure q coordinates for WW
            WW_P0_1_SC = new Vector4(bldg_vm.WW_GRD_1_SC.X - (float)(wind_prov.P_0_WW * PRESSURE_SCALE_FACTOR), bldg_vm.WW_GRD_1_SC.Y, 0.0f, 1.0f);
            WW_P15_1_SC = new Vector4(bldg_vm.WW_15_1_SC.X - (float)(wind_prov.P_15_WW * PRESSURE_SCALE_FACTOR), bldg_vm.WW_15_1_SC.Y, 0.0f, 1.0f);
            WW_PH_1_SC = new Vector4(bldg_vm.WW_H_1_SC.X - (float)(wind_prov.P_H_WW * PRESSURE_SCALE_FACTOR), bldg_vm.WW_H_1_SC.Y, 0.0f, 1.0f);

            LW_P0_1_SC = new Vector4(bldg_vm.LW_GRD_1_SC.X + (float)(wind_prov.P_H_LW * PRESSURE_SCALE_FACTOR), bldg_vm.LW_GRD_1_SC.Y, 0.0f, 1.0f);
            LW_P15_1_SC = new Vector4(bldg_vm.LW_15_1_SC.X + (float)(wind_prov.P_H_LW * PRESSURE_SCALE_FACTOR), bldg_vm.LW_15_1_SC.Y, 0.0f, 1.0f);
            LW_PH_1_SC = new Vector4(bldg_vm.LW_H_1_SC.X + (float)(wind_prov.P_H_LW * PRESSURE_SCALE_FACTOR), bldg_vm.LW_H_1_SC.Y, 0.0f, 1.0f);

            WindProv = wind_prov;
            BuildingVM = bldg_vm;
            Title = title;
        }

        public void Draw(Canvas canvas, double pressure_text_ht)
        {
            string pressure_str;

            // Draws the title for each picture based on the roof pressure diagram being generated.
            DrawingHelpers.DrawText(canvas, BuildingVM.ORIGIN_SC.X - 130, BuildingVM.ORIGIN_SC.Y + 50, 0, Title, Brushes.Black, DEFAULT_TITLE_HT);


            /////////////////////
            // DRAW WALL PRESSURES
            /////////////////////
            // Windward
            // p0 pressure line
            DrawingHelpers.DrawLine(canvas, WW_P0_1_SC.X, WW_P0_1_SC.Y, BuildingVM.WW_GRD_1_SC.X,BuildingVM.WW_GRD_1_SC.Y, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
            pressure_str = (Math.Round(WindProv.P_0_WW * 100.0) / 100.0).ToString();
            DrawingHelpers.DrawText(canvas, WW_P0_1_SC.X, WW_P0_1_SC.Y, 0, pressure_str, Brushes.Blue, pressure_text_ht);

            // Ph pressure line
            DrawingHelpers.DrawLine(canvas, WW_PH_1_SC.X, WW_PH_1_SC.Y, BuildingVM.WW_H_1_SC.X, BuildingVM.WW_H_1_SC.Y, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
            pressure_str = (Math.Round(WindProv.P_H_WW * 100.0) / 100.0).ToString();
            DrawingHelpers.DrawText(canvas, WW_PH_1_SC.X, WW_PH_1_SC.Y, 0, pressure_str, Brushes.Blue, pressure_text_ht);

            // show the 15' WW wall pressure location if necessary.
            if (WindProv.Building.H > 15)
            {
                // Negative here pulls the dimension to the left
                DrawingHelpers.DrawDimensionAligned(canvas, BuildingVM.WW_15_1_SC.X, BuildingVM.WW_15_1_SC.Y, BuildingVM.WW_GRD_1_SC.X, BuildingVM.WW_GRD_1_SC.Y, "15'", pressure_text_ht,
                    -30, -0.3, -5, -10, DrawingHelpers.DEFAULT_DIM_LEADER_COLOR, DrawingHelpers.DEFAULT_DIM_LINETYPE);

                // P15 pressure line
                DrawingHelpers.DrawLine(canvas, WW_P15_1_SC.X, WW_P15_1_SC.Y, BuildingVM.WW_15_1_SC.X, BuildingVM.WW_15_1_SC.Y, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
                pressure_str = (Math.Round(WindProv.P_15_WW * 100.0) / 100.0).ToString();
                DrawingHelpers.DrawText(canvas, WW_P15_1_SC.X, WW_P15_1_SC.Y, 0, pressure_str.ToString(), Brushes.Blue, pressure_text_ht);

                // Draw the two pressure lines
                DrawingHelpers.DrawLine(canvas, WW_P0_1_SC.X, WW_P0_1_SC.Y, WW_P15_1_SC.X, WW_P15_1_SC.Y, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
                DrawingHelpers.DrawLine(canvas, WW_P15_1_SC.X, WW_P15_1_SC.Y, WW_PH_1_SC.X, WW_PH_1_SC.Y, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
            } else
            {
                // Draw the 2D pressure line for this elevation view
                DrawingHelpers.DrawLine(canvas, WW_P0_1_SC.X, WW_P0_1_SC.Y, WW_PH_1_SC.X, WW_PH_1_SC.Y, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
            }

            // Leeward
            // p0 pressure line
            DrawingHelpers.DrawLine(canvas, LW_P0_1_SC.X, LW_P0_1_SC.Y, BuildingVM.LW_GRD_1_SC.X, BuildingVM.LW_GRD_1_SC.Y, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);

            // Ph pressure line
            DrawingHelpers.DrawLine(canvas, LW_PH_1_SC.X, LW_PH_1_SC.Y, BuildingVM.LW_H_1_SC.X, BuildingVM.LW_H_1_SC.Y, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);
            pressure_str = (Math.Round(WindProv.P_H_LW * 100.0) / 100.0).ToString();
            DrawingHelpers.DrawText(canvas, LW_PH_1_SC.X, LW_PH_1_SC.Y, 0, pressure_str, Brushes.Blue, pressure_text_ht);

            //Draw the pressure lines
            DrawingHelpers.DrawLine(canvas, LW_P0_1_SC.X, LW_P0_1_SC.Y, LW_PH_1_SC.X, LW_PH_1_SC.Y, Brushes.Blue, 1, Linetypes.LINETYPE_DASHED);


            /////////////////////
            // DRAW ROOF PRESSURES
            /////////////////////
            // Is it a flat roof scenario?
            if(BuildingVM.Model.HasSingleRidge == false)
            {

            } else
            {
                DrawingHelpers.DrawText(canvas, BuildingVM.ORIGIN_SC.X - 130, BuildingVM.ORIGIN_SC.Y - 50, 0, "No Calcs Available for roof", Brushes.Red, DEFAULT_TITLE_HT);

            }
        }
    }
}
