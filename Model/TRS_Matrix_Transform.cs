using ASCE7_10Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WindCalculator.Model
{
    public class TRS_Matrix_Transform
    {
        private Matrix4x4 m_TRS_Matrix;

        public Matrix4x4 GetTRSMatrix { get=> m_TRS_Matrix; }

        Matrix4x4 TRANS_Matrix { get; set; }
        Matrix4x4 SCALE_Matrix { get; set; }
        Matrix4x4 ROT_Matrix { get; set; }

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

        public Matrix4x4 GetTransform { get; set; } = new Matrix4x4();
        public TRS_Matrix_Transform(BuildingInfo bldg, double x_trans, double y_trans, double z_trans, double x_rot, double y_rot, double z_rot, double drawing_scale_factor, WindOrientations orient)
        {
            // Parameters for translating, scaling, and rotating.
            // double x_rot = 0.0;  // rotation about the horizonal x-axis

            // double y_rot = 0.0;  // rotation about the vertical y-axis

            //If this is a parallel wind view model, need to rotate the graphic by 90 degrees about the y-axis so we can see it
            double temp;
            if (orient == WindOrientations.WIND_ORIENTATION_PARALLELTORIDGE)
            {
                // rotate the view about the 7-axis
                y_rot = 90;

                // swap the x and z translations
                temp = x_trans;
                x_trans = z_trans;
                z_trans = temp;
            }

           // double z_rot = 0.0;  // rotation about the z axis (out of the screen)

            double x_scale = drawing_scale_factor; // x-dir scale
            double y_scale = drawing_scale_factor; // y-dir scale
            double z_scale = drawing_scale_factor; // z-dir scale



            // Shift the origin of the building so that the mid height of the building is at 0,0 on the screen
            Vector3 position = new Vector3((float)-bldg.ORIGIN_1.X, (float)(-bldg.ORIGIN_1.Y - 0.5 * bldg.H), (float)-bldg.ORIGIN_1.Z);
            Vector3 rotation = new Vector3((float)x_rot, (float)y_rot, (float)z_rot);
            Vector3 scale = new Vector3((float)x_scale, (float)y_scale, (float)z_scale);

            // Apply the transform to move the center of the building building to the origin of the screen.
            // 1. and 2. Applies scaling and rotation at this step via the TRS_Matrix
            m_TRS_Matrix = Get_TRS_Matrix(position, rotation, scale);
            TRANS_Matrix = GetTranslationMatrix(new Vector3((float)x_trans, (float)y_trans, (float)z_trans));
            ROT_Matrix = GetRotationMatrix(new Vector3((float)x_rot, (float)y_rot, (float)z_rot));
            SCALE_Matrix = GetScaleMatrix(new Vector3((float)x_scale, (float)y_scale, (float)z_scale));

            // Reflect the y coords for the graphical display of Y+ downwards, by mirroring the struction about the horizontal x-axis
            m_TRS_Matrix = GetTRSMatrix * y_reflect;

            // 3.Move image to center or canvas from 0,0
            m_TRS_Matrix = GetTRSMatrix * TRANS_Matrix;
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

        public static float ConvertDegToRad(float degrees)
        {
            return ((float)Math.PI / (float)180) * degrees;
        }

    }
}
