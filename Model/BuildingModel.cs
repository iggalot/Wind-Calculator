using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindCalculator.Model
{
    public class BuildingModel
    {
        public float L { get; set; } = 10.0f;
        public float B { get; set; } = 10.0f;
        public float H { get; set; } = 10.0f;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public BuildingModel(float length, float width, float height)
        {
            if (length <= 0)
            {
                throw new InvalidOperationException("Invalid length for building received: length=" + length.ToString());
            } else
            {
                L = height;
            }

            if (width <= 0)
            {
                throw new InvalidOperationException("Invalid width for building received: width=" + width.ToString());
            } else
            {
                B = width;
            }

            if (height <= 0)
            {
                throw new InvalidOperationException("Invalid height for building received: height=" + height.ToString());
            } else
            {
                H = height;
            }

        }
    }
}
