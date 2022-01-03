
using DrawingPipeline;
using DrawingPipeline.DirectX;
using DrawingPipelineLibrary;
using DrawingPipelineLibrary.DirectX;
using SharpDX;
using SharpDX.Direct3D11;
using System;

namespace WindCalculator.Model
{
    public class WallModel
    {

        // contain the Model object
        public DModel Model { get; set; } = new DModel();

        // local coordinates for the wall vector
        SharpDX.Vector4[] PointsList = new SharpDX.Vector4[4];

        public Vector4 P0 { get => PointsList[0]; } // ref point lower left
        public Vector4 P1 { get => PointsList[1]; } // lower right
        public Vector4 P2 { get => PointsList[2]; } // upper right
        public Vector4 P3 { get => PointsList[3]; } // upper left

        public Vector4 NormalVector { get; set; }
        public float WallLength { get; set; }
        public float WallHeight { get; set; }

        public Vector4 InsertPoint { get; set; }

        /// <summary>
        /// Constructor for a rectangular wall panel
        /// </summary>
        /// <param name="l">length of the wall</param>
        /// <param name="h">height of the wall</param>
        /// <param name="ref_pt">insert point (lower left)</param>
        /// <param name="outward_normal">unit vector describing the outward direction of the wall panel</param>
        public WallModel(float l, float h, Vector4 ref_pt, Vector4 outward_normal)
        {
            WallHeight = h;
            WallLength = l;

            PointsList[0] = ref_pt;
            PointsList[1] = new Vector4(ref_pt.X + WallLength, ref_pt.Y, ref_pt.Z, ref_pt.W);
            PointsList[2] = new Vector4(ref_pt.X + WallLength, ref_pt.Y + WallHeight, ref_pt.Z, ref_pt.W);
            PointsList[3] = new Vector4(ref_pt.X, ref_pt.Y + WallHeight, ref_pt.Z, ref_pt.W);

            SharpDX.Vector4 v01 = DXMathFunctions.Vec_Sub(PointsList[1], PointsList[0]); // vector from 0 to 1
            SharpDX.Vector4 v03 = DXMathFunctions.Vec_Sub(PointsList[3], PointsList[0]); // vector from 0 to 4

            NormalVector = DXMathFunctions.Vec_CrossProduct(v01, v03);
        }

        public DModel CreateModel(BaseDrawingPipeline pipeline)
        {
            Model = new DModel();

            try
            {
                Model.ModelElementType = ModelElementTypes.MODEL_ELEMENT_TRIANGLE;

                // Set number of vertices in the vertex array.
                Model.VertexCount = 4; // for a rectangle
                // Set number of vertices in the index array.
                Model.IndexCount = 6;  // for two triangles

                if (pipeline.GetType() == typeof(DirectXDrawingPipeline))
                {
                    CreateDirectXModel((DirectXDrawingPipeline)pipeline);
                }
                else if (pipeline.GetType() == typeof(CanvasDrawingPipeline))
                {
                    CreateWPFModel((CanvasDrawingPipeline)pipeline);
                }
                else
                {
                    throw new NotImplementedException("In WallModel.CreateModel(): invalid pipeline type received");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new NotImplementedException("In WallModel.CreateModel(): error creating Model");
            }

            return Model;
        }

        private void CreateWPFModel(CanvasDrawingPipeline pipeline)
        {
            throw new NotImplementedException();
        }

        private void CreateDirectXModel(DirectXDrawingPipeline pipeline)
        {

            Device device = pipeline.GetDSystem.Graphics.D3D.Device;

            // model.InitializeBufferTestTriangle(device, ModelElementTypes.MODEL_ELEMENT_TRIANGLE);

            SharpDX.Vector4 line_color = new SharpDX.Vector4(1, 0, 0, 1);

            // Create the vertex array and load it with data
            DColorShader.DVertex[] vertices = new DColorShader.DVertex[6];

            for (int i = 0; i < Model.VertexCount; i++)
            {
                vertices[i] = new DColorShader.DVertex()
                {
                    position = new SharpDX.Vector3(PointsList[i].X, PointsList[i].Y, PointsList[i].Z),
                    color = line_color
                };
            }

            // Create Indicies for the IndexBuffer.  0-3-2 amd 0-2-1
            int[] indicies = new int[Model.IndexCount];
            indicies[0] = 0;
            indicies[1] = 3;
            indicies[2] = 2;
            indicies[3] = 0;
            indicies[4] = 2;
            indicies[5] = 1;

            // Create the vertex buffer.
            Model.VertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);

            // Create the index buffer.
            Model.IndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indicies);

            // Delete arrays now that they are in their respective vertex and index buffers.
            vertices = null;
            indicies = null;
        }
    }
}
