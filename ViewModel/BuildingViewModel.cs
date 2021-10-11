using ASCE7_10Library;
using DrawingHelpersLibrary;
using System;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Media;

namespace WindCalculator.ViewModel
{
    public class BuildingViewModel
    {
        public BuildingInfo Model { get; set; }

        // To reflect about vertical axis
        Matrix4x4 x_reflect = new Matrix4x4(
            -1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f,
            0.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);

        // To reflect about horizontal axis
        Matrix4x4 y_reflect = new Matrix4x4(
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, -1.0f, 0.0f, 0.0f,
            0.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);

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

        Matrix4x4 TRANS_Matrix { get; set; }
        Matrix4x4 SCALE_Matrix { get; set; }
        Matrix4x4 ROT_Matrix { get; set; }

        /// <summary>
        /// Our combined transformation matrix from world to pixel coords
        /// </summary>
        Matrix4x4 TRS_Matrix { get; set; }

        public BuildingViewModel(Canvas canvas, BuildingInfo bldg, double drawing_scale_factor, WindOrientations orient)
        {
            Model = bldg;
            SCALE_FACTOR = drawing_scale_factor;

            // Parameters for translating, scaling, and rotating.
            double x_trans = 0.5 * canvas.Width;
            double y_trans = 0.5 * canvas.Height;
            double z_trans = bldg.ORIGIN_1.Z;
            double x_rot = 0.0;  // rotation about the horizonal x-axis

            double y_rot = 0.0;  // rotation about the vertical y-axis

            //If this is a parallel wind view model, need to rotate the graphic by 90 degrees about the y-axis so we can see it
            if (orient == WindOrientations.WIND_ORIENTATION_PARALLELTORIDGE)
            {
                y_rot = 90;
            }

            double z_rot = 0.0;  // rotation about the z axis (out of the screen)

            double x_scale = drawing_scale_factor; // x-dir scale
            double y_scale = drawing_scale_factor; // y-dir scale
            double z_scale = drawing_scale_factor; // z-dir scale

            // Shift the origin of the building so that the mid height of the building is at 0,0 on the screen
            Vector3 position = new Vector3((float)-bldg.ORIGIN_1.X, (float)(-bldg.ORIGIN_1.Y - 0.5 * bldg.H), (float)-bldg.ORIGIN_1.Z);
            Vector3 rotation = new Vector3((float)x_rot, (float)y_rot, (float)z_rot);
            Vector3 scale = new Vector3((float)x_scale, (float)y_scale, (float)z_scale);

            // Apply the transform to move the center of the building building to the origin of the screen.
            // 1. and 2. Applies scaling and rotation at this step via the TRS_Matrix
            Matrix4x4 TRS_Matrix = Get_TRS_Matrix(position, rotation, scale);
            Matrix4x4 transMatrix = GetTranslationMatrix(new Vector3((float)x_trans, (float)y_trans, (float)z_trans));
            Matrix4x4 rotMatrix = GetRotationMatrix(new Vector3((float)x_rot, (float)y_rot, (float)z_rot));
            Matrix4x4 scaleMatrix = GetScaleMatrix(new Vector3((float)x_scale, (float)y_scale, (float)z_scale));

            // Reflect the y coords for the graphical display of Y+ downwards, by mirroring the struction about the horizontal x-axis
            TRS_Matrix = TRS_Matrix * y_reflect;

            // 3.Move image to center or canvas from 0,0
            TRS_Matrix = TRS_Matrix * transMatrix;

            TRANS_Matrix = transMatrix;
            SCALE_Matrix = scaleMatrix;
            ROT_Matrix = rotMatrix;

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
            // Draw the WW wall object for frame #1
            DrawingHelpers.DrawLine(canvas, WW_GRD_1_SC.X, WW_GRD_1_SC.Y, LW_GRD_1_SC.X, LW_GRD_1_SC.Y, Brushes.Black, 3, Linetypes.LINETYPE_DASHED); ;

            DrawingHelpers.DrawLine(canvas, WW_GRD_1_SC.X, WW_GRD_1_SC.Y, WW_15_1_SC.X, WW_15_1_SC.Y, Brushes.Red, 3, Linetypes.LINETYPE_SOLID); ;
            DrawingHelpers.DrawLine(canvas, WW_15_1_SC.X, WW_15_1_SC.Y, WW_H_1_SC.X,WW_H_1_SC.Y, Brushes.Black, 3, Linetypes.LINETYPE_SOLID); ;

            // Draw the Roof object line for frame #1
            for (int i = 0; i < RoofProfile_1_SC.Length - 1; i++)
            {
                Vector4 pt1 = RoofProfile_1_SC[i];
                Vector4 pt2 = RoofProfile_1_SC[i + 1];

                DrawingHelpers.DrawLine(canvas, pt1.X, pt1.Y, pt2.X, pt2.Y, Brushes.Black, 3, Linetypes.LINETYPE_SOLID);
            }

            // Draw the LW object line for frame #1
            DrawingHelpers.DrawLine(canvas, LW_GRD_1_SC.X, LW_GRD_1_SC.Y, LW_15_1_SC.X, LW_15_1_SC.Y, Brushes.Black, 3, Linetypes.LINETYPE_SOLID); ;
            DrawingHelpers.DrawLine(canvas, LW_15_1_SC.X, LW_15_1_SC.Y, LW_H_1_SC.X, LW_H_1_SC.Y, Brushes.Black, 3, Linetypes.LINETYPE_SOLID); ;



            // Draw the WW wall object for frame #2
            DrawingHelpers.DrawLine(canvas, WW_GRD_2_SC.X, WW_GRD_2_SC.Y, LW_GRD_2_SC.X, LW_GRD_2_SC.Y, Brushes.Black, 3, Linetypes.LINETYPE_DASHED); ;

            DrawingHelpers.DrawLine(canvas, WW_GRD_2_SC.X, WW_GRD_2_SC.Y, WW_15_2_SC.X, WW_15_2_SC.Y, Brushes.Red, 3, Linetypes.LINETYPE_SOLID); ;
            DrawingHelpers.DrawLine(canvas, WW_15_2_SC.X, WW_15_2_SC.Y, WW_H_2_SC.X, WW_H_2_SC.Y, Brushes.Black, 3, Linetypes.LINETYPE_SOLID); ;

            // Draw the Roof object line for frame #2
            for (int i = 0; i < RoofProfile_2_SC.Length - 1; i++)
            {
                Vector4 pt1 = RoofProfile_2_SC[i];
                Vector4 pt2 = RoofProfile_2_SC[i + 1];

                DrawingHelpers.DrawLine(canvas, pt1.X, pt1.Y, pt2.X, pt2.Y, Brushes.Black, 3, Linetypes.LINETYPE_SOLID);
            }

            // Draw the LW object line for frame #2
            DrawingHelpers.DrawLine(canvas, LW_GRD_2_SC.X, LW_GRD_2_SC.Y, LW_15_2_SC.X, LW_15_2_SC.Y, Brushes.Black, 3, Linetypes.LINETYPE_SOLID); ;
            DrawingHelpers.DrawLine(canvas, LW_15_2_SC.X, LW_15_2_SC.Y, LW_H_2_SC.X, LW_H_2_SC.Y, Brushes.Black, 3, Linetypes.LINETYPE_SOLID); ;

            // Draw building dimensions
            string dim_str;
            // Horizontal Building Dimension
            // TODO:: Plot this dimension below the grade line
            dim_str = (Math.Round(Model.L * 100.0) / 100.0).ToString() + "'";
            DrawingHelpers.DrawDimensionAligned(canvas, WW_GRD_1_SC.X, WW_GRD_1_SC.Y, LW_GRD_1_SC.X, LW_GRD_1_SC.Y, dim_str, dim_text_ht);

            // LW wall
            dim_str = (Math.Abs(Math.Round(((Model.LW_GRD_1.Y - Model.LW_H_1.Y)) * 100.0) / 100.0)).ToString() + "'";
            DrawingHelpers.DrawDimensionAligned(canvas, LW_H_1_SC.X, LW_H_1_SC.Y, LW_GRD_1_SC.X, LW_GRD_1_SC.Y, dim_str, dim_text_ht);

            // WW wall
            dim_str = (Math.Abs(Math.Round(((Model.WW_GRD_1.Y - Model.WW_H_1.Y)) * 100.0) / 100.0)).ToString() + "'";
            DrawingHelpers.DrawDimensionAligned(canvas, WW_GRD_1_SC.X, WW_GRD_1_SC.Y, WW_H_1_SC.X, WW_H_1_SC.Y, dim_str, dim_text_ht);

            // TODO::  Draw connector lines between Frame 1 and Frame 2



            // Dimension the Ridge Point if it isn't a flat roof
            if (Model.RoofSlopeType != RoofSlopeTypes.ROOF_SLOPE_FLAT)
            {
                // Vertical to Ridge
                dim_str = (Math.Abs(Math.Round(((Model.ORIGIN_1.Y - Model.RIDGE_1.Y)) * 100.0) / 100.0)).ToString() + "'";
                DrawingHelpers.DrawDimensionAligned(canvas, RIDGE_1_SC.X, ORIGIN_1_SC.Y, RIDGE_1_SC.X, RIDGE_1_SC.Y, dim_str, dim_text_ht);
                // Horizontal to Ridge from WW
                dim_str = (Math.Abs(Math.Round((Model.RIDGE_1.X - Model.WW_H_1.X) * 100.0) / 100.0)).ToString() + "'";
                DrawingHelpers.DrawDimensionAligned(canvas, WW_H_1_SC.X, RIDGE_1_SC.Y, RIDGE_1_SC.X, RIDGE_1_SC.Y, dim_str, dim_text_ht);

                // Horizontal to Ridge from LW
                dim_str = (Math.Abs(Math.Round((Model.LW_H_1.X - Model.RIDGE_1.X) * 100.0) / 100.0)).ToString() + "'";
                DrawingHelpers.DrawDimensionAligned(canvas, LW_H_1_SC.X, RIDGE_1_SC.Y, RIDGE_1_SC.X, RIDGE_1_SC.Y, dim_str, dim_text_ht); ;
            }
        }

        public static float ConvertDegToRad(float degrees)
        {
            return ((float)Math.PI / (float)180) * degrees;
        }

        public static Matrix4x4 GetTranslationMatrix(Vector3 position)
        {
            return new Matrix4x4(1, 0, 0, 0,
                                0, 1, 0, 0,
                                0, 0, 1, 0,
                                position.X, position.Y, position.Z, 1);
        }

        public static Matrix4x4 GetRotationMatrix(Vector3 anglesDeg)
        {
            anglesDeg = new Vector3(ConvertDegToRad(anglesDeg.X), ConvertDegToRad(anglesDeg.Y), ConvertDegToRad(anglesDeg.Z));

            Matrix4x4 rotationX = new Matrix4x4(1, 0, 0, 0,
                                                0, (float)Math.Cos(anglesDeg.X), (float)Math.Sin(anglesDeg.X), 0,
                                                0, (float)-Math.Sin(anglesDeg.X), (float)Math.Cos(anglesDeg.X), 0,
                                                0, 0, 0, 1);

            Matrix4x4 rotationY = new Matrix4x4((float)Math.Cos(anglesDeg.Y), 0, (float)-Math.Sin(anglesDeg.Y), 0,
                                                0, 1, 0, 0,
                                                (float)Math.Sin(anglesDeg.Y), 0, (float)Math.Cos(anglesDeg.Y), 0,
                                                0, 0, 0, 1);

            Matrix4x4 rotationZ = new Matrix4x4((float)Math.Cos(anglesDeg.Z), (float)Math.Sin(anglesDeg.Z), 0, 0,
                                                (float)-Math.Sin(anglesDeg.Z), (float)Math.Cos(anglesDeg.Z), 0, 0,
                                                0, 0, 1, 0,
                                                0, 0, 0, 1);

            return rotationX * rotationY * rotationZ;
        }

        public static Matrix4x4 GetScaleMatrix(Vector3 scale)
        {
            return new Matrix4x4(scale.X, 0, 0, 0,
                                 0, scale.Y, 0, 0,
                                 0, 0, scale.Z, 0,
                                 0, 0, 0, 1);
        }

        public static Matrix4x4 Get_TRS_Matrix(Vector3 position, Vector3 rotationAngles, Vector3 scale)
        {
            return GetTranslationMatrix(position) * GetRotationMatrix(rotationAngles) * GetScaleMatrix(scale);
        }
    }


}
