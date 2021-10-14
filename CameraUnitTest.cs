using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Numerics;
using WindCalculator.Model;

namespace WindCalcUnitTest
{
    [TestClass]
    public class CameraUnitTest
    {
        Camera TestCamera { get; set; } = new Camera(0, 0, 3, 0, 1, 0, 90, 0);
        float Width { get; set; } = 900;
        float Height { get; set; } = 900;
        float NearPlane { get; set; } = 0.1f;
        float FarPlane { get; set; } = 100.0f;

        float ErrorTolerance { get; set; } = 0.001f;
        public CameraUnitTest()
        {
            TestCamera.ProjectionMatrix = Camera.Perspective(45.0f, Width / Height, NearPlane, FarPlane);
        }

        [TestMethod]
        public void ProjectionMatrix()
        {
            Assert.IsTrue(Math.Abs(TestCamera.ProjectionMatrix.M11 - 2.414) < ErrorTolerance );
            Assert.IsTrue(Math.Abs(TestCamera.ProjectionMatrix.M12 - 0) < ErrorTolerance);
            Assert.IsTrue(Math.Abs(TestCamera.ProjectionMatrix.M13 - 0) < ErrorTolerance);
            Assert.IsTrue(Math.Abs(TestCamera.ProjectionMatrix.M14 - 0) < ErrorTolerance);
            Assert.IsTrue(Math.Abs(TestCamera.ProjectionMatrix.M21 - 0) < ErrorTolerance);
            Assert.IsTrue(Math.Abs(TestCamera.ProjectionMatrix.M22 - 2.414) < ErrorTolerance);
            Assert.IsTrue(Math.Abs(TestCamera.ProjectionMatrix.M23 - 0) < ErrorTolerance);
            Assert.IsTrue(Math.Abs(TestCamera.ProjectionMatrix.M24 - 0) < ErrorTolerance);
            Assert.IsTrue(Math.Abs(TestCamera.ProjectionMatrix.M31 - 0) < ErrorTolerance);
            Assert.IsTrue(Math.Abs(TestCamera.ProjectionMatrix.M32 - 0) < ErrorTolerance);
            Assert.IsTrue(Math.Abs(TestCamera.ProjectionMatrix.M33 - (-1.002002)) < ErrorTolerance);
            Assert.IsTrue(Math.Abs(TestCamera.ProjectionMatrix.M34 - (-1)) < ErrorTolerance);
            Assert.IsTrue(Math.Abs(TestCamera.ProjectionMatrix.M41 - 0) < ErrorTolerance);
            Assert.IsTrue(Math.Abs(TestCamera.ProjectionMatrix.M42 - 0) < ErrorTolerance);
            Assert.IsTrue(Math.Abs(TestCamera.ProjectionMatrix.M43 - (-0.200200)) < ErrorTolerance);
            Assert.IsTrue(Math.Abs(TestCamera.ProjectionMatrix.M44 - 0) < ErrorTolerance);
        }         

        [TestMethod]
        public void WorldToScreen()
        {
            Vector4 world_pt = new Vector4(10.0f, 5.0f, 1.0f, 1.0f);
            Vector4 screen_pt = Camera.WorldToScreen(world_pt, TestCamera.ViewMatrix, TestCamera.ProjectionMatrix, Width, Height);

            Assert.IsTrue(Math.Abs(screen_pt.X - 24.14) < ErrorTolerance);
            Assert.IsTrue(Math.Abs(screen_pt.Y - 12.07) < ErrorTolerance);
            Assert.IsTrue(Math.Abs(screen_pt.Z - (-2.002)) < ErrorTolerance);
            Assert.IsTrue(Math.Abs(screen_pt.W - 0.100100) < ErrorTolerance);

        }
    }
}
