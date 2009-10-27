using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace GravurGIS.Topology
{
    public struct PointL
    {
        public long x;
        public long y;

        public PointL(long x, long y)
        {
            this.x = x;
            this.y = y;
        }
        public PointL(int x, int y)
        {
            this.x = (long)x;
            this.y = (long)y;
        }
    }
}
