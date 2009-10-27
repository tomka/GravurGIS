using System.Drawing;

namespace GravurGIS.Topology
{
    public struct PointD
    {
        public double x;
        public double y;

        public PointD(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public PointD(int x, int y)
        {
            this.x = (double) x;
            this.y = (double) y;
        }

        public Point getIntPoint() {
            return new System.Drawing.Point((int) x, (int) y);
        }
        public static PointD operator *(PointD p1, double factor)
        {
            return new PointD(p1.x * factor, p1.y * factor);
        }
    }
}
