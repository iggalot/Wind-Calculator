using ASCE7_10Library;
using DrawingHelpersLibrary;
using DrawingPipeline;
using DrawingPipeline.DirectX;
using DrawingPipelineLibrary.DirectX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WindCalculator.Model;

namespace WindCalculator.ViewModel
{
    public class BuildingViewModel
    {
        public BuildingModel BldgModel { get; set; }

        public List<DModel> WallModels { get; set; } = new List<DModel>();
        public WindOrientations WindOrient { get; set; } = WindOrientations.WIND_ORIENTATION_NORMALTORIDGE;

        public BuildingViewModel(BuildingModel model)
        {
            BldgModel = model;
        }

        public BuildingViewModel(Canvas canvas, BuildingInfo bldg, double drawing_scale_factor, WindOrientations orient)
        {

            UpdateScreenCoords();

        }

        public void UpdateScreenCoords()
        {

        }

        public void Update()
        {
            
        }

        public void Draw(double dim_text_ht)
        {
            
        }

        public List<DModel> CreateModel(BaseDrawingPipeline pipeline)
        {
             WallModels = new List<DModel>();

            try
            {
                foreach(var item in BldgModel.WallsList)
                {
                    DModel model = new DModel();
                    model.ModelElementType = ModelElementTypes.MODEL_ELEMENT_TRIANGLE;

                    if (pipeline.GetType() == typeof(DirectXDrawingPipeline))
                    {
                        model = item.CreateModel(pipeline);
                    } else if (pipeline.GetType() == typeof(CanvasDrawingPipeline))
                    {
                        MessageBox.Show("WPF wall model drawings not supported at this time");
                    } else
                    {
                        throw new NotImplementedException("In BuildingViewModel.CreateModel(): invalid pipeline type received");
                    }

                    WallModels.Add(model);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new NotImplementedException("In UCSIcon.CreateModel(): error creating Model");
            }

            return WallModels;
        }

        /// <summary>
        /// The routine to render the building as a DirectX object.
        /// </summary>
        /// <param name="pipeline"></param>
        private DModel InitializeDirectXBuffers(DirectXDrawingPipeline pipeline, DModel model, ModelElementTypes element_type)
        {
            try
            {
                SharpDX.Vector4 wall1_color = new SharpDX.Vector4(0.5f, 0.3f, 0.2f, 1f);
                SharpDX.Vector4 wall2_color = new SharpDX.Vector4(0.3f, 0.2f, 0.5f, 1f);
                SharpDX.Vector4 wall3_color = new SharpDX.Vector4(0.3f, 0.8f, 0.5f, 1f);


                // Create the vertex array and load it with data.
                var vertices = new[]
                {
                    // Wall #1
					// Bottom left.
					new DColorShader.DVertex()
                    {
                        position = new SharpDX.Vector3(0, 0, 0),
                        color = wall1_color
                    },
					// Bottom right.
					new DColorShader.DVertex()
                    {
                        position = new SharpDX.Vector3((float)BldgModel.L, 0, 0),
                        color = wall1_color
                    },
					// top Right.
					new DColorShader.DVertex()
                    {
                        position = new SharpDX.Vector3((float)BldgModel.L, (float)BldgModel.H, 0),
                        color = wall1_color
                    },
                    					
                    // Top left.
					new DColorShader.DVertex()
                    {
                        position = new SharpDX.Vector3(0, (float)BldgModel.H, 0),
                        color = wall1_color
                    },

                                        
                    // Wall #2
					// Bottom left.
                    new DColorShader.DVertex()
                    {
                        position = new SharpDX.Vector3((float)BldgModel.L, 0, (float)BldgModel.B),
                        color = wall2_color
                    },

					// Bottom right.
					new DColorShader.DVertex()
                    {
                        position = new SharpDX.Vector3(0, 0, (float)BldgModel.B),
                        color = wall2_color
                    },
					// top Right.
					new DColorShader.DVertex()
                    {
                        position = new SharpDX.Vector3(0, (float)BldgModel.H, (float)BldgModel.B),
                        color = wall2_color
                    },
                    					
                    // Top left.
					new DColorShader.DVertex()
                    {
                        position = new SharpDX.Vector3((float)BldgModel.L, (float)BldgModel.H, (float)BldgModel.B),
                        color = wall2_color
                    },
                };

                // Create Indicies for the IndexBuffer.
                int[] indicies = new int[]
                {
                    // Wall1
                    0, // Bottom left.
					3, // Top middle.
					2,  // Bottom right.
                    0,
                    2,
                    1,

                    // Wall2
                    4,
                    7,
                    6,
                    4,
                    6,
                    5,

                    // Windward Wall
                    5,
                    3,
                    0,
                    5,
                    6,
                    3,

                    //Leeward Wall
                    1,
                    2,
                    7,
                    1,
                    7,
                    4
                };

                Device device = ((DirectXDrawingPipeline)pipeline).GetDSystem.Graphics.D3D.Device;

                // Set number of vertices in the vertex array.
                model.VertexCount = vertices.Length;

                // Set number of vertices in the index array.
                model.IndexCount = indicies.Length;

                // Create the vertex buffer.
                model.VertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);

                // Create the index buffer.
                model.IndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indicies, model.IndexCount * sizeof(int));

                model.ModelElementType = element_type;

                // Delete arrays now that they are in their respective vertex and index buffers.
                vertices = null;
                indicies = null;
            }
            catch (System.Exception)
            {
                throw new InvalidOperationException("Error in Rendering DirectX of the BuildingViewModel");
            }

            return model;
        }
    }


}
