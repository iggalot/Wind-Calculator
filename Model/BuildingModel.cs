using ASCE7_10Library;
using System;
using System.Collections.Generic;

namespace WindCalculator.Model
{
    /// <summary>
    /// Wrapper class for the Building Info basic data model of the ASCE7_10Linrary
    /// </summary>
    public class BuildingModel : BuildingInfo
    {
        public List<WallModel> WallsList { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="length">length of building parallel to wind</param>
        /// <param name="width">width of building parallel to wind</param>
        /// <param name="height">wall height of building</param>
        public BuildingModel(float length, float width, float height, RoofSlopeTypes slope_type=RoofSlopeTypes.ROOF_SLOPE_FLAT, RiskCategories risk = RiskCategories.II, ExposureCategories exposure = ExposureCategories.B, float wind_speed= 115.0f)
            : base(length, width, height, slope_type, risk, exposure, wind_speed)
        {
            // error check dimensions
            if (length <= 0)
            {
                throw new InvalidOperationException("Invalid length for building received: length=" + length.ToString());
            }

            if (width <= 0)
            {
                throw new InvalidOperationException("Invalid width for building received: width=" + width.ToString());
            }

            if (height <= 0)
            {
                throw new InvalidOperationException("Invalid height for building received: height=" + height.ToString());
            }

            CreateWalls();
        }

        public BuildingModel(BuildingInfo info) : base((float)info.L, (float)info.B, (float)info.H, info.RoofSlopeType, info.RiskCategory, info.ExposureCategory, info.WindSpeed)
        {
            if(info == null)
            {
                throw new InvalidOperationException("In BuildingModel CTOR: Invalid building info received");
            }

            CreateWalls();
        }

        private void CreateWalls()
        {
            WallsList = new List<WallModel>();
            // WW
            WallModel new_wall = new WallModel((float)L, (float)H, new SharpDX.Vector4(0, 0, 0, 1), new SharpDX.Vector4(0, 0, -1, 1));
            WallsList.Add(new_wall);

            // LW
            new_wall = new WallModel((float)L, (float)H, new SharpDX.Vector4(100, 0, 0, 1), new SharpDX.Vector4(0, 0, 1, 1));
            WallsList.Add(new_wall);

            // SW-left (looking at windward)
            new_wall = new WallModel((float)B, (float)H, new SharpDX.Vector4(200, 0, 0, 1), new SharpDX.Vector4(-1, 0, 0, 1));
            WallsList.Add(new_wall);

            // SW-right (look at windward)
            new_wall = new WallModel((float)B, (float)H, new SharpDX.Vector4(300, 0, 0, 1), new SharpDX.Vector4(1, 0, 0, 1));
            WallsList.Add(new_wall);

            // Roof surface
            new_wall = new WallModel((float)B, (float)L, new SharpDX.Vector4(400, 0, 0, 1), new SharpDX.Vector4(0, 1, 0, 1));
            WallsList.Add(new_wall);
        }
    }
}
