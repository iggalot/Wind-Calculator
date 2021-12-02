using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WindCalculator.Model
{
    public class WallModel
    {
        // local coordinates for the wall vector
        private Vector4 p1;
        private Vector4 p2;
        private Vector4 p3;
        private Vector4 p4;

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
            p1 = ref_pt;
            p2 = new Vector4(ref_pt.X + WallLength, ref_pt.Y, ref_pt.Z, ref_pt.W);
            p3 = new Vector4(ref_pt.X + WallLength, ref_pt.Y + WallHeight, ref_pt.Z, ref_pt.W);
            p4 = new Vector4(ref_pt.X, ref_pt.Y + WallHeight, ref_pt.Z, ref_pt.W);

            // TODO: Need to transform these points based on the outward normal vector to get the X, Y, Z coordinates in world space


        }
    }
}
