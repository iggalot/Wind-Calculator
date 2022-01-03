using DrawingPipeline;
using DrawingPipeline.DirectX;
using DrawingPipelineLibrary.DirectX;
using SharpDX.Direct3D11;
using System;

namespace WindCalculator.Model
{
    public class UCSIcon
    {
        private const int DEFAULT_AXIS_LENGTH = 300;
        private SharpDX.Vector4 Origin { get; set; } = new SharpDX.Vector4(0, 0, 0, 1);

        // contain the Model object
        public DModel Model { get; set; } = new DModel();

        // contains X,Y,Z coordinates for the end of the lines of the grid in this gridline object
        private SharpDX.Vector4[] UCSlineList { get; set; } = new SharpDX.Vector4[6];

        public UCSIcon(SharpDX.Vector4 origin)
        {
            // line in X direction
            UCSlineList[0] = new SharpDX.Vector4(origin.X, origin.Y, origin.Z, 1);
            UCSlineList[1] = new SharpDX.Vector4(origin.X + DEFAULT_AXIS_LENGTH, origin.Y, origin.Z, 1);

            // line in Y direction
            UCSlineList[2] = new SharpDX.Vector4(origin.X, origin.Y, origin.Z, 1);
            UCSlineList[3] = new SharpDX.Vector4(origin.X, origin.Y + DEFAULT_AXIS_LENGTH, origin.Z, 1);

            // line in Z direction
            UCSlineList[4] = new SharpDX.Vector4(origin.X, origin.Y, origin.Z, 1);
            UCSlineList[5] = new SharpDX.Vector4(origin.X, origin.Y, origin.Z + DEFAULT_AXIS_LENGTH, 1);
        }

        public DModel CreateModel(BaseDrawingPipeline pipeline)
        {
            Model = new DModel();

            try
            {
                Model.ModelElementType = ModelElementTypes.MODEL_ELEMENT_LINE;

                // Set number of vertices in the vertex array.
                Model.VertexCount = UCSlineList.Length;
                // Set number of vertices in the index array.
                Model.IndexCount = UCSlineList.Length;

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
                    throw new NotImplementedException("In UCSIcon.CreateModel(): invalid pipeline type received");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new NotImplementedException("In UCSIcon.CreateModel(): error creating Model");
            }

            return Model;
        }

        private void CreateWPFModel(CanvasDrawingPipeline pipeline)
        {
            // Create WPF specific stuff here.
            throw new NotImplementedException("WPF drawing not supported in UCSIcon for now");

        }

        /// <summary>
        /// Creates the DirectX Pipeline object for grid lines
        /// </summary>
        /// <param name="pipeline"></param>
        /// <returns></returns>
        public void CreateDirectXModel(DirectXDrawingPipeline pipeline )
        {
            Device device = pipeline.GetDSystem.Graphics.D3D.Device;

            // model.InitializeBufferTestTriangle(device, ModelElementTypes.MODEL_ELEMENT_TRIANGLE);

            SharpDX.Vector4 line_color_red = new SharpDX.Vector4(1, 0, 0, 1);
            SharpDX.Vector4 line_color_green = new SharpDX.Vector4(0, 1, 0, 1);
            SharpDX.Vector4 line_color_blue = new SharpDX.Vector4(0, 0, 1, 1);

            // Create the vertex array and load it with data -- 3 lines with two vertices each
            DColorShader.DVertex[] vertices = new DColorShader.DVertex[UCSlineList.Length];

            for (int i = 0; i < UCSlineList.Length; i++)
            {
                SharpDX.Vector4 line_color;
                if(i / 2 == 0)
                {
                    line_color = line_color_red;
                } else if (i / 2 == 1)
                {
                    line_color = line_color_green;
                } else
                {
                    line_color = line_color_blue;
                }

                vertices[i] = new DColorShader.DVertex()
                {
                    position = new SharpDX.Vector3(UCSlineList[i].X, UCSlineList[i].Y, UCSlineList[i].Z),
                    color = line_color
                };
            }

            // Create Indicies for the IndexBuffer.
            int[] indicies = new int[UCSlineList.Length];

            for (int i = 0; i < UCSlineList.Length; i++)
            {
                indicies[i] = i;
            }

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
