using DrawingPipeline.DirectX;
using DrawingPipelineLibrary.DirectX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindCalculator.Model
{
    public class Gridlines
    {
        private const int numLines = 20;
        
        private float X_SPA { get; set; } = 10;
        private float Y_SPA { get; set; } = 10;
        private float Z_SPA { get; set; } = 10;

        // contain the Model object
        public DModel Model { get; set; } = new DModel();

        // contains X,Y,Z coordinates for the end of the lines of the grid in this gridline object
        private SharpDX.Vector4[] GridlineList { get; set; }

        public Gridlines()
        {
            GridlineList = new SharpDX.Vector4[24];

            // gridlines in Z direction
            float x_off = -X_SPA * numLines * 0.5f;
            float y_off = 0;
            float z_off = -Z_SPA * numLines * 0.5f;

            // line in Z direction
            GridlineList[0] = new SharpDX.Vector4(x_off, y_off, z_off, 1);
            GridlineList[1] = new SharpDX.Vector4(x_off, y_off, -z_off, 1);

            GridlineList[2] = new SharpDX.Vector4(-x_off, y_off, z_off, 1);
            GridlineList[3] = new SharpDX.Vector4(x_off, y_off, z_off, 1);

            GridlineList[4] = new SharpDX.Vector4(-x_off, y_off, -z_off, 1);
            GridlineList[5] = new SharpDX.Vector4(-x_off, y_off, z_off, 1);

            GridlineList[6] = new SharpDX.Vector4( x_off, y_off, -z_off, 1);
            GridlineList[7] = new SharpDX.Vector4(-x_off, y_off, -z_off, 1);

            GridlineList[8] = new SharpDX.Vector4(x_off * 0.5f, y_off, z_off, 1);
            GridlineList[9] = new SharpDX.Vector4(x_off * 0.5f, y_off, -z_off, 1);

            GridlineList[10] = new SharpDX.Vector4(-x_off, y_off, z_off * 0.5f, 1);
            GridlineList[11] = new SharpDX.Vector4(x_off, y_off, z_off * 0.5f, 1);

            GridlineList[12] = new SharpDX.Vector4(-x_off * 0.5f, y_off, -z_off, 1);
            GridlineList[13] = new SharpDX.Vector4(-x_off * 0.5f, y_off, z_off, 1);

            GridlineList[14] = new SharpDX.Vector4(x_off, y_off, -z_off * 0.5f, 1);
            GridlineList[15] = new SharpDX.Vector4(-x_off, y_off, -z_off * 0.5f, 1);

            GridlineList[16] = new SharpDX.Vector4(0, y_off, z_off, 1);
            GridlineList[17] = new SharpDX.Vector4(0, y_off, -z_off, 1);

            GridlineList[18] = new SharpDX.Vector4(-x_off, y_off, 0, 1);
            GridlineList[19] = new SharpDX.Vector4(x_off, y_off, 0, 1);

            GridlineList[20] = new SharpDX.Vector4(0, y_off, -z_off, 1);
            GridlineList[21] = new SharpDX.Vector4(0, y_off, z_off, 1);

            GridlineList[22] = new SharpDX.Vector4(x_off, y_off, 0, 1);
            GridlineList[23] = new SharpDX.Vector4(-x_off, y_off, 0, 1);
        }

        public DModel CreateModel(DirectXDrawingPipeline pipeline )
        {
            Model = new DModel();
            Device device = pipeline.GetDSystem.Graphics.D3D.Device;

            // model.InitializeBufferTestTriangle(device, ModelElementTypes.MODEL_ELEMENT_TRIANGLE);

            SharpDX.Vector4 line_color = new SharpDX.Vector4(1, 1, 1, 1);
            try
            {
                // Set number of vertices in the vertex array.
                Model.VertexCount = GridlineList.Length;
                // Set number of vertices in the index array.
                Model.IndexCount = GridlineList.Length;

                // Create the vertex array and load it with data.
                DColorShader.DVertex[] vertices = new DColorShader.DVertex[numLines * 2];

                for (int i = 0; i < GridlineList.Length; i++)
                {
                    vertices[i] = new DColorShader.DVertex()
                    {
                        position = new SharpDX.Vector3(GridlineList[i].X, GridlineList[i].Y, GridlineList[i].Z),
                        color = line_color
                    };
                }

                // Create Indicies for the IndexBuffer.
                int[] indicies = new int[numLines * 2];

                for (int i = 0; i < GridlineList.Length; i++)
                {
                    indicies[i] = i;
                }

                // Create the vertex buffer.
                Model.VertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);

                // Create the index buffer.
                Model.IndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indicies);

                Model.ModelElementType = ModelElementTypes.MODEL_ELEMENT_LINE;

                // Delete arrays now that they are in their respective vertex and index buffers.
                vertices = null;
                indicies = null;

            }
            catch
            {
            }
            return Model;

        }
    }
}
