using ASCE7_10Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindCalculator.Model
{
    public class BuildingModel
    {
        // Building dimensions
        public float Length { get; set; } = 10.0f;
        public float Width { get; set; } = 10.0f;
        public float Height { get; set; } = 10.0f;

        // Building site parameters
        public RiskCategories RiskCategory { get; set; } = RiskCategories.II;
        public ExposureCategories ExposureCategory { get; set; } = ExposureCategories.B;
        public float WindSpeed { get; set; } = 115.0f;  // wind speed in MPH

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="length">length of building parallel to wind</param>
        /// <param name="width">width of building parallel to wind</param>
        /// <param name="height">wall height of building</param>
        public BuildingModel(float length, float width, float height, RiskCategories risk = RiskCategories.II, ExposureCategories exposure = ExposureCategories.B, float wind_speed = 115.0f)
        {

            if (length <= 0)
            {
                throw new InvalidOperationException("Invalid length for building received: length=" + length.ToString());
            }
            Length = length;

            if (width <= 0)
            {
                throw new InvalidOperationException("Invalid width for building received: width=" + width.ToString());
            }
            Width = width;

            if (height <= 0)
            {
                throw new InvalidOperationException("Invalid height for building received: height=" + height.ToString());
            }
            Height = height;

            RiskCategory = risk;
            ExposureCategory = exposure;
            WindSpeed = wind_speed;
        }
    }
}
