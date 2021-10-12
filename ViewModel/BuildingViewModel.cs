﻿using ASCE7_10Library;
using DrawingHelpersLibrary;
using System;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Media;
using WindCalculator.Model;

namespace WindCalculator.ViewModel
{
    public class BuildingViewModel
    {
        public BuildingInfo Model { get; set; }

        public Vector4 WW_GRD_1_SC { get; set; }
        public Vector4 WW_15_1_SC { get; set; }
        public Vector4 WW_H_1_SC { get; set; }
        public Vector4 ORIGIN_1_SC { get; set; } = new Vector4();
        public Vector4 RIDGE_1_SC { get; set; } = new Vector4();

        public Vector4 LW_H_1_SC { get; set; }
        public Vector4 LW_15_1_SC { get; set; }
        public Vector4 LW_GRD_1_SC { get; set; }


        public Vector4 WW_GRD_2_SC { get; set; }
        public Vector4 WW_15_2_SC { get; set; }
        public Vector4 WW_H_2_SC { get; set; }
        public Vector4 ORIGIN_2_SC { get; set; } = new Vector4();
        public Vector4 RIDGE_2_SC { get; set; } = new Vector4();

        public Vector4 LW_H_2_SC { get; set; }
        public Vector4 LW_15_2_SC { get; set; }
        public Vector4 LW_GRD_2_SC { get; set; }


        public Vector4[] RoofProfile_1_SC { get; set; }
        public Vector4[] RoofProfile_2_SC { get; set; }

        /// <summary>
        /// Pressure zone points in screen coordinates
        /// </summary>
        public Vector4[] RoofZonePoints_1_SC { get; set; }
        public Vector4[] RoofZonePoints_2_SC { get; set; }

        public double SCALE_FACTOR {get; set;}

        public WindOrientations WindOrient { get; set; } = WindOrientations.WIND_ORIENTATION_NORMALTORIDGE;



        /// <summary>
        /// Our combined transformation matrix from world to pixel coords
        /// </summary>
        Matrix4x4 TRS_Matrix { get; set; }

        public BuildingViewModel(Canvas canvas, BuildingInfo bldg, double drawing_scale_factor, WindOrientations orient)
        {
            Model = bldg;
            SCALE_FACTOR = drawing_scale_factor;
            WindOrient = orient;

            // Create our TRS transform for the window
            TRS_Matrix_Transform new_transform = new TRS_Matrix_Transform(bldg, 0.5 * canvas.Width, 0.5 * canvas.Height, bldg.ORIGIN_1.Z, 0.0, 0.0, 0.0, SCALE_FACTOR, orient);
            TRS_Matrix = new_transform.GetTRSMatrix;

            // Apply the transforms to get screen coords
            ORIGIN_1_SC = Vector4.Transform(bldg.ORIGIN_1, TRS_Matrix);
            WW_GRD_1_SC = Vector4.Transform(bldg.WW_GRD_1, TRS_Matrix);
            WW_15_1_SC = Vector4.Transform(bldg.WW_15_1, TRS_Matrix);
            WW_H_1_SC = Vector4.Transform(bldg.WW_H_1, TRS_Matrix);
            LW_GRD_1_SC = Vector4.Transform(bldg.LW_GRD_1, TRS_Matrix);
            LW_15_1_SC = Vector4.Transform(bldg.LW_15_1, TRS_Matrix);
            LW_H_1_SC = Vector4.Transform(bldg.LW_H_1, TRS_Matrix);
            RIDGE_1_SC = Vector4.Transform(bldg.RIDGE_1, TRS_Matrix);

            ORIGIN_2_SC = Vector4.Transform(bldg.ORIGIN_2, TRS_Matrix);
            WW_GRD_2_SC = Vector4.Transform(bldg.WW_GRD_2, TRS_Matrix);
            WW_15_2_SC = Vector4.Transform(bldg.WW_15_2, TRS_Matrix);
            WW_H_2_SC = Vector4.Transform(bldg.WW_H_2, TRS_Matrix);
            LW_GRD_2_SC = Vector4.Transform(bldg.LW_GRD_2, TRS_Matrix);
            LW_15_2_SC = Vector4.Transform(bldg.LW_15_2, TRS_Matrix);
            LW_H_2_SC = Vector4.Transform(bldg.LW_H_2, TRS_Matrix);
            RIDGE_2_SC = Vector4.Transform(bldg.RIDGE_2, TRS_Matrix);
            
            RoofProfile_1_SC = new Vector4[bldg.RoofProfile_1.Length];
            RoofProfile_2_SC = new Vector4[bldg.RoofProfile_2.Length];

            // Get the roof profile screen coordsDraw the Roof object line
            for (int i = 0; i < bldg.RoofProfile_1.Length; i++)
            {
                RoofProfile_1_SC[i] = Vector4.Transform(new Vector4(bldg.RoofProfile_1[i].X, bldg.RoofProfile_1[i].Y, bldg.RoofProfile_1[i].Z, 1.0f), TRS_Matrix);
                RoofProfile_2_SC[i] = Vector4.Transform(new Vector4(bldg.RoofProfile_2[i].X, bldg.RoofProfile_2[i].Y, bldg.RoofProfile_2[i].Z, 1.0f), TRS_Matrix);
            }

            // Create the screen coordinates for the pressure zone points
            int count = 0;
            RoofZonePoints_1_SC = new Vector4[bldg.RoofZonePts_1.Length];
            RoofZonePoints_2_SC = new Vector4[bldg.RoofZonePts_2.Length];

            foreach (var item in bldg.RoofZonePts_1)
            {
                RoofZonePoints_1_SC[count] = Vector4.Transform(new Vector4(item.X, item.Y, item.Z, 1.0f), TRS_Matrix);
                RoofZonePoints_2_SC[count] = Vector4.Transform(new Vector4(item.X, item.Y, item.Z, 1.0f), TRS_Matrix);
                count++;
            }
        }

        public void Draw(Canvas canvas, double dim_text_ht)
        {
            // WW points frame1
            double pt1_x, pt1_y, pt2_x, pt2_y, pt3_x, pt3_y;

            // WW points frame2
            double pt4_x, pt4_y, pt5_x, pt5_y, pt6_x, pt6_y;

            // LW points frame 1
            double pt7_x, pt7_y, pt8_x, pt8_y, pt9_x, pt9_y;

            // LW points frame 2
            double pt10_x, pt10_y, pt11_x, pt11_y, pt12_x, pt12_y;

            pt1_x = WW_GRD_1_SC.X;
            pt1_y = WW_GRD_1_SC.Y;
            pt2_x = WW_15_1_SC.X;
            pt2_y = WW_15_1_SC.Y;
            pt3_x = WW_H_1_SC.X;
            pt3_y = WW_H_1_SC.Y;

            pt4_x = WW_GRD_2_SC.X;
            pt4_y = WW_GRD_2_SC.Y;
            pt5_x = WW_15_2_SC.X;
            pt5_y = WW_15_2_SC.Y;
            pt6_x = WW_H_2_SC.X;
            pt6_y = WW_H_2_SC.Y;

            pt7_x = LW_GRD_1_SC.X;
            pt7_y = LW_GRD_1_SC.Y;
            pt8_x = LW_15_1_SC.X;
            pt8_y = LW_15_1_SC.Y;
            pt9_x = LW_H_1_SC.X;
            pt9_y = LW_H_1_SC.Y;

            pt10_x = LW_GRD_2_SC.X;
            pt10_y = LW_GRD_2_SC.Y;
            pt11_x = LW_15_2_SC.X;
            pt11_y = LW_15_2_SC.Y;
            pt12_x = LW_H_2_SC.X;
            pt12_y = LW_H_2_SC.Y;

            // Draw the ground
            //DrawingHelpers.DrawLine(canvas, WW_GRD_1_SC.X, WW_GRD_1_SC.Y, LW_GRD_1_SC.X, LW_GRD_1_SC.Y, Brushes.Black, 3, Linetypes.LINETYPE_DASHED); ;
            DrawingHelpers.DrawLine(canvas, pt1_x, pt1_y, pt7_x, pt7_y, Brushes.Black, 3, Linetypes.LINETYPE_DASHED); ;

            // Draw the WW wall object for frame #1
            DrawingHelpers.DrawLine(canvas, pt1_x, pt1_y, pt2_x, pt2_y, Brushes.Red, 3, Linetypes.LINETYPE_SOLID);   // 0-15 WW
            DrawingHelpers.DrawLine(canvas, pt2_x, pt2_y, pt3_x, pt3_y, Brushes.Black, 3, Linetypes.LINETYPE_SOLID); // 15-H WW

            // Draw the WW wall object for frame #2
            DrawingHelpers.DrawLine(canvas, pt4_x, pt4_y, pt5_x, pt5_y, Brushes.Red, 3, Linetypes.LINETYPE_SOLID);   // 0-15 WW
            DrawingHelpers.DrawLine(canvas, pt5_x, pt5_y, pt6_x, pt6_y, Brushes.Black, 3, Linetypes.LINETYPE_SOLID); // 15-H WW

            //Draw the LW object line for frame #1
            DrawingHelpers.DrawLine(canvas, pt7_x, pt7_y, pt8_x, pt8_y, Brushes.Black, 3, Linetypes.LINETYPE_SOLID); ;
            DrawingHelpers.DrawLine(canvas, pt8_x, pt8_y, pt9_x, pt9_y, Brushes.Black, 3, Linetypes.LINETYPE_SOLID); ;

            // Draw the LW wall object for frame #2
            DrawingHelpers.DrawLine(canvas, pt10_x, pt10_y, pt11_x, pt11_y, Brushes.Black, 3, Linetypes.LINETYPE_SOLID);
            DrawingHelpers.DrawLine(canvas, pt11_x, pt11_y, pt12_x, pt12_y, Brushes.Green, 3, Linetypes.LINETYPE_SOLID);

            // Draw the Roof object line for frame #1
            for (int i = 0; i < RoofProfile_1_SC.Length - 1; i++)
            {
                Vector4 r_pt1 = RoofProfile_1_SC[i];
                Vector4 r_pt2 = RoofProfile_1_SC[i + 1];

                DrawingHelpers.DrawLine(canvas, r_pt1.X, r_pt1.Y, r_pt2.X, r_pt2.Y, Brushes.Black, 3, Linetypes.LINETYPE_SOLID);
            }

            // Draw the Roof object line for frame #2
            for (int i = 0; i < RoofProfile_2_SC.Length - 1; i++)
            {
                Vector4 r_pt1 = RoofProfile_2_SC[i];
                Vector4 r_pt2 = RoofProfile_2_SC[i + 1];

                DrawingHelpers.DrawLine(canvas, r_pt1.X, r_pt1.Y, r_pt2.X, r_pt2.Y, Brushes.Black, 3, Linetypes.LINETYPE_SOLID);
            }

            if (WindOrient == WindOrientations.WIND_ORIENTATION_NORMALTORIDGE)
            {

            } else
            {

            }






            string dim_str;

            //// Draw building dimensions
            //// Horizontal Building Dimension
            //// TODO:: Plot this dimension below the grade line
            //dim_str = (Math.Round(Model.L * 100.0) / 100.0).ToString() + "'";
            //DrawingHelpers.DrawDimensionAligned(canvas, WW_GRD_1_SC.X, WW_GRD_1_SC.Y, LW_GRD_1_SC.X, LW_GRD_1_SC.Y, dim_str, dim_text_ht);

            //// LW wall
            //dim_str = (Math.Abs(Math.Round(((Model.LW_GRD_1.Y - Model.LW_H_1.Y)) * 100.0) / 100.0)).ToString() + "'";
            //DrawingHelpers.DrawDimensionAligned(canvas, LW_H_1_SC.X, LW_H_1_SC.Y, LW_GRD_1_SC.X, LW_GRD_1_SC.Y, dim_str, dim_text_ht);

            //// WW wall
            //dim_str = (Math.Abs(Math.Round(((Model.WW_GRD_1.Y - Model.WW_H_1.Y)) * 100.0) / 100.0)).ToString() + "'";
            //DrawingHelpers.DrawDimensionAligned(canvas, WW_GRD_1_SC.X, WW_GRD_1_SC.Y, WW_H_1_SC.X, WW_H_1_SC.Y, dim_str, dim_text_ht);

            // TODO::  Draw connector lines between Frame 1 and Frame 2


            // Dimension the Ridge Point if it isn't a flat roof or parallel to ridge
            //if (Model.RoofSlopeType != RoofSlopeTypes.ROOF_SLOPE_FLAT && WindOrient == WindOrientations.WIND_ORIENTATION_NORMALTORIDGE)
            //{
            //    // Vertical to Ridge
            //    dim_str = (Math.Abs(Math.Round(((Model.ORIGIN_1.Y - Model.RIDGE_1.Y)) * 100.0) / 100.0)).ToString() + "'";
            //    DrawingHelpers.DrawDimensionAligned(canvas, RIDGE_1_SC.X, ORIGIN_1_SC.Y, RIDGE_1_SC.X, RIDGE_1_SC.Y, dim_str, dim_text_ht);
            //    // Horizontal to Ridge from WW
            //    dim_str = (Math.Abs(Math.Round((Model.RIDGE_1.X - Model.WW_H_1.X) * 100.0) / 100.0)).ToString() + "'";
            //    DrawingHelpers.DrawDimensionAligned(canvas, WW_H_1_SC.X, RIDGE_1_SC.Y, RIDGE_1_SC.X, RIDGE_1_SC.Y, dim_str, dim_text_ht);

            //    // Horizontal to Ridge from LW
            //    dim_str = (Math.Abs(Math.Round((Model.LW_H_1.X - Model.RIDGE_1.X) * 100.0) / 100.0)).ToString() + "'";
            //    DrawingHelpers.DrawDimensionAligned(canvas, LW_H_1_SC.X, RIDGE_1_SC.Y, RIDGE_1_SC.X, RIDGE_1_SC.Y, dim_str, dim_text_ht); ;
            //}

        }
    }


}
