
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
        // Default Wall Colors
        private SharpDX.Vector4 DEFAULT_COLOR_WW_WALL = new SharpDX.Vector4(1, 0, 0, 0);
        private SharpDX.Vector4 DEFAULT_COLOR_LW_WALL = new SharpDX.Vector4(0, 1, 0, 0);
        private SharpDX.Vector4 DEFAULT_COLOR_SW_1_WALL = new SharpDX.Vector4(0, 0, 1, 0);
        private SharpDX.Vector4 DEFAULT_COLOR_SW_2_WALL = new SharpDX.Vector4(0, 0, 0, 0);

        // contain the Model object
        public DModel Model { get; set; } = new DModel();

        // local coordinates for the wall vector
        SharpDX.Vector4[] PointsList = new SharpDX.Vector4[4];

        public Vector4 P0 { get => PointsList[0]; } // ref point lower left
        public Vector4 P1 { get => PointsList[1]; } // lower right
        public Vector4 P2 { get => PointsList[2]; } // upper right
        public Vector4 P3 { get => PointsList[3]; } // upper left

        public Vector4 NormalVector { get; set; }
        public SharpDX.Vector4 UpVector = new SharpDX.Vector4(0, 0, 0, 0);
        public SharpDX.Vector4 RightVector = new SharpDX.Vector4(0, 0, 0, 0);

        public float WallLength { get; set; }
        public float WallHeight { get; set; }
        public BuildingWalls WallDesignation { get; set; }

        public Vector4 InsertPoint { get; set; }

        /// <summary>
        /// Constructor for a rectangular wall panel with points oriented as
        /// 3 ------ 2
        /// |        |
        /// 0--------1
        /// </summary>
        /// <param name="l">length of the wall</param>
        /// <param name="h">height of the wall</param>
        /// <param name="ref_pt">insert point (lower left)</param>
        /// <param name="outward_normal">unit vector describing the outward direction of the wall panel</param>
        public WallModel(float l, float h, BuildingWalls desig, Vector4 ref_pt, Vector4 outward_normal)
        {
            WallHeight = h;
            WallLength = l;
            WallDesignation = desig;

            // Normalize the normal vector (just in case)
            NormalVector = DXMathFunctions.Vec_Normalize(outward_normal);
            SharpDX.Vector4 up =  DXMathFunctions.Vec_Normalize(new SharpDX.Vector4(0, 1, 0, 0));
            SharpDX.Vector4 right =  DXMathFunctions.Vec_Normalize(DXMathFunctions.Vec_CrossProduct(outward_normal,up));

            // Create the Up Vector for the wall (from bottom point to point directly above).
            UpVector = DXMathFunctions.Vec_Mul(up, WallHeight);

            // Determine the direction vector from ref. point to end point on bottom of well.
            RightVector = DXMathFunctions.Vec_Mul(right, WallLength); 
            
            // Compute the coordinates of the corners of a rectangular wall.
            // P0
            PointsList[0] = ref_pt;

            // P1
            PointsList[1] = DXMathFunctions.Vec_Add(ref_pt, RightVector);

            // P2
            PointsList[2] = DXMathFunctions.Vec_Add(PointsList[1], UpVector);

            // P3 
            PointsList[3] = DXMathFunctions.Vec_Add(ref_pt, UpVector);


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
                Model.IndexCount = 12;  // for four triangles -- two per face

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

        /// <summary>
        /// Create the SharpDX vertex and index data for this wall.
        /// 3 ------ 2
        /// |        |
        /// 0--------1
        /// </summary>
        /// <param name="pipeline"></param>
        private void CreateDirectXModel(DirectXDrawingPipeline pipeline)
        {
            Device device = pipeline.GetDSystem.Graphics.D3D.Device;

            // model.InitializeBufferTestTriangle(device, ModelElementTypes.MODEL_ELEMENT_TRIANGLE);

            SharpDX.Vector4 line_color = new SharpDX.Vector4(1, 0, 0, 1);

            // Create the vertex array and load it with data
            DColorShader.DVertex[] vertices = new DColorShader.DVertex[6];

            for (int i = 0; i < Model.VertexCount; i++)
            {
                // Determine the wall colors....
                switch (WallDesignation)
                {
                    case BuildingWalls.BLDG_WALL_WW:
                        line_color = DEFAULT_COLOR_WW_WALL;
                        break;
                    case BuildingWalls.BLDG_WALL_LW:
                        line_color = DEFAULT_COLOR_LW_WALL;
                        break;
                    case BuildingWalls.BLDG_WALL_SW_1:
                        line_color = DEFAULT_COLOR_SW_1_WALL;
                        break;
                    case BuildingWalls.BLDG_WALL_SW_2:
                        line_color = DEFAULT_COLOR_SW_2_WALL;
                        break;
                    default:
                        break;
                }
                 
                vertices[i] = new DColorShader.DVertex()
                {
                    position = new SharpDX.Vector3(PointsList[i].X, PointsList[i].Y, PointsList[i].Z),
                    color = line_color
                };
            }

            // Create Indicies for the IndexBuffer.  0-3-2 amd 0-2-1
            int[] indicies = new int[Model.IndexCount];

            // Front face when looking at object
            indicies[0] = 0;
            indicies[1] = 3;
            indicies[2] = 2;
            indicies[3] = 0;
            indicies[4] = 2;
            indicies[5] = 1;

            // Backface (when looking at object)
            indicies[6] = 0;
            indicies[7] = 2;
            indicies[8] = 3;
            indicies[9] = 0;
            indicies[10] = 1;
            indicies[11] = 2;

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
