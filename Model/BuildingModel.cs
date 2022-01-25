using ASCE7_10Library;
using System;
using System.Collections.Generic;

namespace WindCalculator.Model
{
    public enum BuildingWalls
    {
        BLDG_WALL_WW = 0,
        BLDG_WALL_LW = 1,
        BLDG_WALL_SW_1 = 2,  // left side wall when facing WW wall
        BLDG_WALL_SW_2 = 3    // right side wall when acing WW wall
    }
    /// <summary>
    /// Wrapper class for the Building Info basic data model of the ASCE7_10Linrary
    /// </summary>
    public class BuildingModel : BuildingInfo
    {
        public List<WallModel> WallsList { get; set; }

        public WallModel WW_Wall { get => WallsList[(int)(BuildingWalls.BLDG_WALL_WW)]; }
        public WallModel LW_Wall { get => WallsList[(int)(BuildingWalls.BLDG_WALL_LW)]; }
        public WallModel SW_Wall_1 { get => WallsList[(int)(BuildingWalls.BLDG_WALL_SW_1)]; }
        public WallModel SW_Wall_2 { get => WallsList[(int)(BuildingWalls.BLDG_WALL_SW_2)]; }

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
            WallModel new_wall = new WallModel((float)B, (float)H, BuildingWalls.BLDG_WALL_WW, new SharpDX.Vector4(0, 0, (float)(B), 1), new SharpDX.Vector4(-1, 0, 0, 0));
            WallsList.Add(new_wall);

            // LW
            new_wall = new WallModel((float)B, (float)H, BuildingWalls.BLDG_WALL_LW, new SharpDX.Vector4((float)L, 0, 0, 1), new SharpDX.Vector4(1, 0, 0, 0));
            WallsList.Add(new_wall);

            // SW-left (looking at windward)
            new_wall = new WallModel((float)L, (float)H, BuildingWalls.BLDG_WALL_SW_1, new SharpDX.Vector4((float)L, 0, (float)(B), 1), new SharpDX.Vector4(0, 0, 1, 0));
            WallsList.Add(new_wall);

            // SW-right (look at windward)
            new_wall = new WallModel((float)L, (float)H, BuildingWalls.BLDG_WALL_SW_2, new SharpDX.Vector4(0, 0, 0, 1), new SharpDX.Vector4(0, 0, -1, 0));
            WallsList.Add(new_wall);

            //// Roof surface
            //new_wall = new WallModel((float)L, (float)B, new SharpDX.Vector4(400, 0, 0, 1), new SharpDX.Vector4(0, 1, 0, 1));
            //WallsList.Add(new_wall);
        }
    }
}
