using ASCE7_10Library;
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
        // Our camera object for this view model
        public Camera CameraObj { get; set; }
        public BuildingInfo Model { get; set; }
        public Canvas DrawingCanvas { get; set; }

        public Vector4 WW_GRD_1_SC { get; set; }
        public Vector4 WW_15_1_SC { get; set; }
        public Vector4 WW_H_1_SC { get; set; }
        public Vector4 ORIGIN_1_SC { get; set; } = new Vector4();
        public Vector4 ORIGIN_1_Test { get; set; } = new Vector4();

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

        public BuildingViewModel(Canvas canvas, Camera camera, BuildingInfo bldg, double drawing_scale_factor, WindOrientations orient)
        {
            Model = bldg;
            SCALE_FACTOR = drawing_scale_factor;
            WindOrient = orient;
            CameraObj = camera;
            DrawingCanvas = canvas;

            //CameraObj = new Camera(0, 0, 150.0f, 0, 1, 0, 90, 0);
            // Update our camera stuff
            //            CameraObj.ModelMatrix = Matrix4x4.Identity;
            ////CameraObj.ModelMatrix = CameraObj.ModelMatrix.Translate(new Vector3((float)(-canvas.Width / 2.0), (float)(-canvas.Height / 2.0f), 0));
            //CameraObj.ModelMatrix = CameraObj.ModelMatrix.ScaleBy(CameraObj.Zoom);

            // Set a basic view matrix
            //            CameraObj.ViewMatrix = Matrix4x4.Identity;

            // Translate the scene in the reverse direction of where we want to move.
            //            CameraObj.ViewMatrix = CameraObj.ViewMatrix.Translate(0.0f, 0.0f, -3.0f);

            //           Matrix4x4 res = new Matrix4x4();


            //// TODO::  Figure out how to remove 0,0 on the M11 element of this matrix.
            //if(!Matrix4x4.Invert(Camera.LookAt(CameraObj.Position, CameraObj.Target, CameraObj.WorldUp), out res))
            //{
            //    throw new InvalidOperationException("Unable to invert the LookAt matrix in view construction");
            //}
            //CameraObj.ViewMatrix = res;

            // Set the projection mode...
            //CameraObj.ProjectionMatrix = Camera.Perspective(CameraObj.FOV, (float)(canvas.Width / canvas.Height), 0.1f, 100.0f);

            // Vector4 world = new Vector4(10, 10, 0, 1.0f);
            // Vector4 screen = Camera.WorldToScreen(world, CameraObj.ModelMatrix, CameraObj.ViewMatrix, CameraObj.ProjectionMatrix, canvas.Width, canvas.Height);
            // Vector4 world_retrieve = Camera.ScreenToWorld(screen, CameraObj.ViewMatrix, CameraObj.ProjectionMatrix, canvas.Width, canvas.Height);

            UpdateScreenCoords();

        }

        public void UpdateScreenCoords()
        {
            BuildingInfo bldg = Model;
            Canvas canvas = DrawingCanvas;

            ORIGIN_1_SC = Camera.WorldToScreen(bldg.ORIGIN_1, CameraObj.ModelMatrix, CameraObj.ProjectionMatrix, CameraObj.ViewMatrix, canvas.Width, canvas.Height);
            WW_GRD_1_SC = Camera.WorldToScreen(bldg.WW_GRD_1, CameraObj.ModelMatrix, CameraObj.ProjectionMatrix, CameraObj.ViewMatrix, canvas.Width, canvas.Height);
            WW_15_1_SC = Camera.WorldToScreen(bldg.WW_15_1, CameraObj.ModelMatrix, CameraObj.ProjectionMatrix, CameraObj.ViewMatrix, canvas.Width, canvas.Height);
            WW_H_1_SC = Camera.WorldToScreen(bldg.WW_H_1, CameraObj.ModelMatrix, CameraObj.ProjectionMatrix, CameraObj.ViewMatrix, canvas.Width, canvas.Height);
            LW_GRD_1_SC = Camera.WorldToScreen(bldg.LW_GRD_1, CameraObj.ModelMatrix, CameraObj.ProjectionMatrix, CameraObj.ViewMatrix, canvas.Width, canvas.Height);
            LW_15_1_SC = Camera.WorldToScreen(bldg.LW_15_1, CameraObj.ModelMatrix, CameraObj.ProjectionMatrix, CameraObj.ViewMatrix, canvas.Width, canvas.Height);
            LW_H_1_SC = Camera.WorldToScreen(bldg.LW_H_1, CameraObj.ModelMatrix, CameraObj.ProjectionMatrix, CameraObj.ViewMatrix, canvas.Width, canvas.Height);
            RIDGE_1_SC = Camera.WorldToScreen(bldg.RIDGE_1, CameraObj.ModelMatrix, CameraObj.ProjectionMatrix, CameraObj.ViewMatrix, canvas.Width, canvas.Height);

            //CameraObj.ModelMatrix = CameraObj.ModelMatrix.Translate(new Vector3(30, 30, 0));

            ORIGIN_2_SC = Camera.WorldToScreen(bldg.ORIGIN_2, CameraObj.ModelMatrix, CameraObj.ProjectionMatrix, CameraObj.ViewMatrix, canvas.Width, canvas.Height);
            WW_GRD_2_SC = Camera.WorldToScreen(bldg.WW_GRD_2, CameraObj.ModelMatrix, CameraObj.ProjectionMatrix, CameraObj.ViewMatrix, canvas.Width, canvas.Height);
            WW_15_2_SC = Camera.WorldToScreen(bldg.WW_15_2, CameraObj.ModelMatrix, CameraObj.ProjectionMatrix, CameraObj.ViewMatrix, canvas.Width, canvas.Height);
            WW_H_2_SC = Camera.WorldToScreen(bldg.WW_H_2, CameraObj.ModelMatrix, CameraObj.ProjectionMatrix, CameraObj.ViewMatrix, canvas.Width, canvas.Height);
            LW_GRD_2_SC = Camera.WorldToScreen(bldg.LW_GRD_2, CameraObj.ModelMatrix, CameraObj.ProjectionMatrix, CameraObj.ViewMatrix, canvas.Width, canvas.Height);
            LW_15_2_SC = Camera.WorldToScreen(bldg.LW_15_2, CameraObj.ModelMatrix, CameraObj.ProjectionMatrix, CameraObj.ViewMatrix, canvas.Width, canvas.Height);
            LW_H_2_SC = Camera.WorldToScreen(bldg.LW_H_2, CameraObj.ModelMatrix, CameraObj.ProjectionMatrix, CameraObj.ViewMatrix, canvas.Width, canvas.Height);
            RIDGE_2_SC = Camera.WorldToScreen(bldg.RIDGE_2, CameraObj.ModelMatrix, CameraObj.ProjectionMatrix, CameraObj.ViewMatrix, canvas.Width, canvas.Height);

            RoofProfile_1_SC = new Vector4[bldg.RoofProfile_1.Length];
            RoofProfile_2_SC = new Vector4[bldg.RoofProfile_2.Length];

            // Get the roof profile screen coordsDraw the Roof object line
            for (int i = 0; i < bldg.RoofProfile_1.Length; i++)
            {
                RoofProfile_1_SC[i] = Camera.WorldToScreen(bldg.RoofProfile_1[i], CameraObj.ModelMatrix, CameraObj.ProjectionMatrix, CameraObj.ViewMatrix, canvas.Width, canvas.Height);
                RoofProfile_2_SC[i] = Camera.WorldToScreen(bldg.RoofProfile_2[i], CameraObj.ModelMatrix, CameraObj.ProjectionMatrix, CameraObj.ViewMatrix, canvas.Width, canvas.Height);
            }

            //// Create the screen coordinates for the pressure zone points
            //int count = 0;
            //RoofZonePoints_1_SC = new Vector4[bldg.RoofZonePts_1.Length];
            //RoofZonePoints_2_SC = new Vector4[bldg.RoofZonePts_2.Length];

            //for (int i = 0; i < bldg.RoofZonePts_1.Length; i++)
            //{
            //    RoofZonePoints_1_SC[i] = Camera.WorldToScreen(bldg.RoofZonePts_1[i], CameraObj.ModelMatrix, CameraObj.ProjectionMatrix, CameraObj.ViewMatrix, canvas.Width, canvas.Height);
            //    RoofZonePoints_2_SC[i] = Camera.WorldToScreen(bldg.RoofZonePts_2[i], CameraObj.ModelMatrix, CameraObj.ProjectionMatrix, CameraObj.ViewMatrix, canvas.Width, canvas.Height);
            //}
        }

        public void Update()
        {
            CameraObj.Update();
            UpdateScreenCoords();
            
        }

        public void Draw(Canvas canvas, double dim_text_ht)
        {
            // Draw coord info
            double text_ht = 12;
            DrawingHelpersLibrary.DrawingHelpers.DrawText(DrawingCanvas, 0, 0, 0, "SCREEN COORDS: " + SCALE_FACTOR.ToString(), Brushes.Black, text_ht);
            DrawingHelpersLibrary.DrawingHelpers.DrawText(DrawingCanvas, 500, text_ht * 1.0f, 0, "WW_GRD_1_SC <X,Y,Z>: " + WW_GRD_1_SC.X.ToString() + ", " + WW_GRD_1_SC.Y.ToString() + ", " + WW_GRD_1_SC.Z.ToString(), Brushes.Black, text_ht);
            DrawingHelpersLibrary.DrawingHelpers.DrawText(DrawingCanvas, 500, text_ht * 2.0f, 0, "WW_H_1_SC <X,Y,Z>: " + WW_H_1_SC.X.ToString() + ", " + WW_H_1_SC.Y.ToString() + ", " + WW_H_1_SC.Z.ToString(), Brushes.Black, text_ht);
            DrawingHelpersLibrary.DrawingHelpers.DrawText(DrawingCanvas, 500, text_ht * 3.0f, 0, "LW_GRD_1_SC <X,Y,Z>: " + LW_GRD_1_SC.X.ToString() + ", " + LW_GRD_1_SC.Y.ToString() + ", " + LW_GRD_1_SC.Z.ToString(), Brushes.Black, text_ht);
            DrawingHelpersLibrary.DrawingHelpers.DrawText(DrawingCanvas, 500, text_ht * 4.0f, 0, "LW_H_1_SC <X,Y,Z>: " + LW_H_1_SC.X.ToString() + ", " + LW_H_1_SC.Y.ToString() + ", " + LW_H_1_SC.Z.ToString(), Brushes.Black, text_ht);


            // WW points frame1
            double pt1_x, pt1_y, pt2_x, pt2_y, pt3_x, pt3_y;

            // WW points frame2
            double pt4_x, pt4_y, pt5_x, pt5_y, pt6_x, pt6_y;

            // LW points frame 1
            double pt7_x, pt7_y, pt8_x, pt8_y, pt9_x, pt9_y;

            // LW points frame 2
            double pt10_x, pt10_y, pt11_x, pt11_y, pt12_x, pt12_y;

            // Ridge points
            double pt13_x, pt13_y, pt14_x, pt14_y;

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

            pt13_x = RIDGE_1_SC.X;
            pt13_y = RIDGE_1_SC.Y;
            pt14_x = RIDGE_2_SC.X;
            pt14_y = RIDGE_2_SC.Y;

            // Draw the ground
            //DrawingHelpers.DrawLine(canvas, WW_GRD_1_SC.X, WW_GRD_1_SC.Y, LW_GRD_1_SC.X, LW_GRD_1_SC.Y, Brushes.Black, 3, Linetypes.LINETYPE_DASHED); ;
            DrawingHelpers.DrawLine(canvas, pt1_x, pt1_y, pt7_x, pt7_y, Brushes.Black, 3, Linetypes.LINETYPE_DASHED);
            DrawingHelpers.DrawLine(canvas, pt4_x, pt4_y, pt10_x, pt10_y, Brushes.Red, 3, Linetypes.LINETYPE_DASHED);

            //// Draw the WW wall object for frame #1
            DrawingHelpers.DrawLine(canvas, pt1_x, pt1_y, pt2_x, pt2_y, Brushes.Black, 3, Linetypes.LINETYPE_SOLID);   // 0-15 WW
            DrawingHelpers.DrawLine(canvas, pt2_x, pt2_y, pt3_x, pt3_y, Brushes.Black, 3, Linetypes.LINETYPE_SOLID); // 15-H WW

            //// Draw the WW wall object for frame #2
            DrawingHelpers.DrawLine(canvas, pt4_x, pt4_y, pt5_x, pt5_y, Brushes.Red, 3, Linetypes.LINETYPE_SOLID);   // 0-15 WW
            DrawingHelpers.DrawLine(canvas, pt5_x, pt5_y, pt6_x, pt6_y, Brushes.Red, 3, Linetypes.LINETYPE_SOLID); // 15-H WW

            //////Draw the LW object line for frame #1
            DrawingHelpers.DrawLine(canvas, pt7_x, pt7_y, pt8_x, pt8_y, Brushes.Black, 3, Linetypes.LINETYPE_SOLID);
            DrawingHelpers.DrawLine(canvas, pt8_x, pt8_y, pt9_x, pt9_y, Brushes.Black, 3, Linetypes.LINETYPE_SOLID);

            // Draw the LW wall object for frame #2
            DrawingHelpers.DrawLine(canvas, pt10_x, pt10_y, pt11_x, pt11_y, Brushes.Red, 3, Linetypes.LINETYPE_SOLID);
            DrawingHelpers.DrawLine(canvas, pt11_x, pt11_y, pt12_x, pt12_y, Brushes.Red, 3, Linetypes.LINETYPE_SOLID);

            // Draw WW_H_1 to WW_H_2 and LW_H_1 to LW_H_2
            DrawingHelpers.DrawLine(canvas, pt3_x, pt3_y, pt6_x, pt6_y, Brushes.Purple, 3, Linetypes.LINETYPE_SOLID);
            DrawingHelpers.DrawLine(canvas, pt9_x, pt9_y, pt12_x, pt12_y, Brushes.Purple, 3, Linetypes.LINETYPE_SOLID);

            // Draw RIDGE_1 to RIDGE_2
            if((Double.IsNaN(RIDGE_1_SC.X) || Double.IsNaN(RIDGE_1_SC.Y) || Double.IsNaN(RIDGE_1_SC.Z) || Double.IsNaN(RIDGE_2_SC.X) || Double.IsNaN(RIDGE_2_SC.Y) || Double.IsNaN(RIDGE_2_SC.Z)) != true)
            {
                DrawingHelpers.DrawLine(canvas, pt13_x, pt13_y, pt14_x, pt14_y, Brushes.Purple, 3, Linetypes.LINETYPE_SOLID);
            }


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

                DrawingHelpers.DrawLine(canvas, r_pt1.X, r_pt1.Y, r_pt2.X, r_pt2.Y, Brushes.Red, 3, Linetypes.LINETYPE_SOLID);
            }

            //if (WindOrient == WindOrientations.WIND_ORIENTATION_NORMALTORIDGE)
            //{

            //} else
            //{

            //}






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
