using System;
using System.Collections.Generic;
using System.Text;
using GravurGIS.Shapes;
using System.Drawing;
using MapTools;
using GravurGIS.Topology;
using GravurGIS.Utilities;

namespace GravurGIS.Shapes
{
    public class ShpPolygon : IShape
    {
        private List<ShpPoint> points;
        private bool closingPolygon = false;
        private Tolerance tolerance = new Tolerance();

        public ShpPolygon(double x, double y, double scale)
        {
            points = new List<ShpPoint>();
            this.minX = tempMinX = x;
            this.minY = tempMinY = y;
            this.width = 1;
            this.height = 1;
            AddPoint(x, y, scale);
        }

        #region IShape Members

        public override void Delete()
        {
            qtItem.Delete();
        }

        public override void AddPoint(double x, double y, double scale)
        {
            ShpPoint pointToAdd = new ShpPoint(x, y, scale);
            points.Add(pointToAdd);
            checkBoundingBox();

            pointToAdd.Changed += new PositionChangedDelegate(vertex_PositionChanged);
        }

        private void vertex_PositionChanged(IShape sender, bool followUpMove)
        {
            if (!followUpMove)
            {
                if (closingPolygon)
                {
                    closingPolygon = false;
                    checkBoundingBox();
                    base.OnChanged(false);
                    return;
                }

                if (tolerance.Equal(sender.RootX, points[0].RootX) &&
                    tolerance.Equal(sender.RootY, points[0].RootY))
                { // the first point was changed
                    closingPolygon = true;
                    points[points.Count - 1].moveTo(sender.RootX, sender.RootY, false, false);
                }
                else if (tolerance.Equal(sender.RootX, points[points.Count - 1].RootX) &&
                    tolerance.Equal(sender.RootY, points[points.Count - 1].RootY)
                    )
                { // the last point was changed
                    closingPolygon = true;
                    points[0].moveTo(sender.RootX, sender.RootY, false, false);
                }
                else
                {
                    checkBoundingBox();
                    base.OnChanged(false);
                }

            }
            //else
            //    closingPolygon = false;
        }

        /// <summary>
        /// removes a point at a special position in a polygon and recalculates boundingBox
        /// </summary>
        /// <param name="index"></param>
        public override void RemovePoint(int index)
        {
            if (index == 0)
            {
                minX = double.NaN;
                minY = double.NaN;
                width = double.NaN;
                height = double.NaN;
                points.RemoveAt(index);
            }
            else
            {

                ShpPoint tempPoint = points[index];
                points.RemoveAt(index);
                if (tempPoint.CenterX == minX || tempPoint.CenterX == minX + width
                     || tempPoint.CenterY == minY || tempPoint.CenterY == minY + height
                   )
                {
                    minX = points[0].CenterX;
                    minY = points[0].CenterY;
                    width = 0;
                    height = 0;
                    checkBoundingBox();
                }
            }
        }


        public override Rectangle getDisplayBoundingBox(double dX, double dY,
            int pointSize, double scale, int extend)
        {
            return new Rectangle(
                Convert.ToInt32(minX * scale - dX - pointSize * 0.5) - extend,
                Convert.ToInt32(minY * scale + dY - pointSize * 0.5) - extend,
                Convert.ToInt32(width * scale + pointSize) + 2 * extend,
                Convert.ToInt32(height * scale + pointSize) + 2 * extend);
        }

        public override MapTools.ShapeLib.ShapeType Type
        {
            get { return ShapeLib.ShapeType.Polygon; }
        }

        public override void moveTo(double toX, double toY, bool temp, bool combinedMove)
        {
            for (int i = points.Count - 1; i >= 0; i--)
                points[i].moveToByDifference(toX - minX, toY - minY, combinedMove);

            // wenn wir eine kollektiv-Bewegung mehrerer Punkte haben kümmern wir uns an dieser Stelle
            // um den Rest (wie beim Pan) - sonst wird alles für jeden Punkt einzeln gemacht
            if (combinedMove)
            {
                this.minX = toX;
                this.minY = toY;

                if (!temp)
                {
                    tempMinX = minX;
                    tempMinY = minY;
                }

                base.OnChanged(true);
            }
        }

        internal override void moveToByDifference(double difX, double difY, bool temp, bool combinedMove)
        {
            for (int i = points.Count - 1; i >= 0; i--)
                points[i].moveToByDifference(difX, difY, temp, combinedMove);

            // wenn wir eine kollektiv-Bewegung mehrerer Punkte haben kümmern wir uns an dieser Stelle
            // um den Rest (wie beim Pan) - sonst wird alles für jeden Punkt einzeln gemacht
            if (combinedMove)
            {
                this.minX = tempMinX + difX;
                this.minY = tempMinY + difY;

                if (!temp)
                {
                    tempMinX = minX;
                    tempMinY = minY;
                }

                base.OnChanged(true);
            }
        }

        public override void moveToByDifference(double difX, double difY, bool temp)
        {
            moveToByDifference(difX, difY, temp, true);
        }

        public override Point[] getPointList(int dX, int dY, double absoluteZoom)
        {
            Point[] returnList = new Point[points.Count];

            for (int i = points.Count - 1; i >= 0; i--)
                returnList[i] = points[i].getPoint(dX, dY, absoluteZoom);

            return returnList;
        }

        public override int getPointListSize()
        {
            return points.Count;
        }

        public override double[] getXList()
        {
            double[] returnArray = new double[points.Count];

            for (int i = 0; i < points.Count; i++)
                returnArray[i] = points[i].RootX;
            return returnArray;
        }

        public override double[] getYList()
        {
            double[] returnArray = new double[points.Count];

            for (int i = 0; i < points.Count; i++)
                returnArray[i] = -points[i].RootY;
            return returnArray;
        }

        public override int PointCount
        {
            get { return points.Count; }
        }

        public override double MinX
        {
            get { return minX; }
        }
        public override double MinY
        {
            get { return minY; }
        }
        public override bool Visible
        {
            get
            {
                return this.visible;
            }
            set
            {
                this.visible = value;
            }
        }

        #endregion

        #region Getters/Setters

        public override double Height
        {
            get { return (double)height; }
        }

        public override double Width
        {
            get { return (double)width; }
        }

        public override double CenterX
        {
            get { return minX + (width * 0.5d); }
        }
        public override double CenterY
        {
            get { return minY + (height * 0.5d); }
        }

        public override double RootX
        {
            get { return points[0].RootX; }
        }

        public override double RootY
        {
            get { return points[0].RootY; }
        }
        #endregion

        public override IShape getElement(int index)
        {
            if ((index >= this.points.Count)
                || (index < 0))
                throw new ArgumentOutOfRangeException("index", "index was: " + index.ToString());
            return this.points[index];

        }

        /// <summary>
        /// Gets the area of the polygon. Interpretation of the number depends on coordinate system.
        /// </summary>
        public Double Area
        {
            get
            {
                // TODO: IsSimple-Test verbessern
                if (IsSimple())
                {
                    if (this.points.Count < 3)
                        return 0;
                    double sum = 0;
                    double ax = this.points[0].getXList()[0];
                    double ay = this.points[0].getYList()[0];
                    for (int i = 1; i < this.points.Count - 1; i++)
                    {
                        if (points[i].PointCount > 1) System.Windows.Forms.MessageBox.Show("> 1");
                        double bx = points[i].getXList()[0];
                        double by = points[i].getYList()[0];
                        double cx = points[i + 1].getXList()[0];
                        double cy = points[i + 1].getYList()[0];
                        sum += ax * by - ay * bx +
                            ay * cx - ax * cy +
                            bx * cy - cx * by;
                    }
                    return Math.Abs(-sum / 2);

                }
                else return Double.NaN;
            }
        }

        /// <summary>
        /// Gets the perimeter (ger: Umfang) of the polygon
        /// </summary>
        public Double Perimeter
        {
            get
            {
                double perimeter = 0;
                double x, y;

                for (int i = 1; i < points.Count; ++i)
                {
                    x = points[i - 1].CenterX - points[i].CenterX;
                    y = points[i - 1].CenterY - points[i].CenterY;

                    perimeter += Math.Sqrt(x * x + y * y);
                }

                return perimeter;
            }
        }

        //public double getarea()
        //{
        //    double area = 0;
        //    ///todo: testen, ob einfacher polygonzug
        //    ///     1. alle ecken verschieden
        //    ///     2. keine kreuzungen
        //    ///a0,1  =  (x0 – x1) (y0 + y1) / 2
        //    ///1.:
        //    double checkx, checky;
        //    bool equalpoints = false;
        //    for (int i = 0; i < points.count; i++)
        //    {
        //        int test = points[i].getxlist().length;
        //        checkx = points[i].getxlist()[0];
        //        checky = points[i].getylist()[0];
        //        for (int j = i+1; j < points.count; j++)
        //            if  ( (points[j].getxlist()[0] == checkx)
        //               && (points[j].getylist()[0] == checky))
        //            {
        //                equalpoints = true;
        //                break;
        //            }
        //        if (equalpoints) break;
        //    }
        //    ///2.:
        //    ///


        //    return area;
        //}

        public bool IsSimple()
        {
            List<ShpPoint> verts = new List<ShpPoint>(this.points.Count);
            for (int i = 0; i < points.Count; i++)
                if (!verts.Exists(delegate(ShpPoint p) { return p.Equals(points[i]); }))
                    verts.Add(points[i]);
            return (verts.Count == this.points.Count - (this.IsClosed ? 1 : 0));
        }

        public bool IsClosed
        {
            get
            {
                if (points.Count > 2 &&
                    tolerance.Equal(points[0].RootX, points[points.Count - 1].RootX) &&
                    tolerance.Equal(points[0].RootY, points[points.Count - 1].RootY))
                    return true;

                return false;
            }
        }

        public override IShape NearestPointTo(PointD position, double maxDistance)
        {
            IShape result = null;
            for (int i = points.Count - 1; i >= 0; i--)
            {
                result = points[i].NearestPointTo(position, maxDistance);
                if (result != null) return result;
            }

            return null;
        }

        protected override void checkBoundingBox()
        {
            double minX = Int32.MaxValue;
            double minY = minX;
            double maxX = Int32.MinValue;
            double maxY = maxX;


            double x, y;

            for (int i = 0; i < points.Count; i++)
            {
                x = points[i].CenterX;
                y = points[i].CenterY;

                if (x < minX) minX = x;
                if (x > maxX) maxX = x;
                if (y < minY) minY = y;
                if (y > maxY) maxY = y;
            }

            this.minX = tempMinX = minX;
            this.minY = tempMinY = minY;
            this.width = Math.Abs(maxX - minX);
            this.height = Math.Abs(maxY - minY);
        }

        public override int RemovePoint(ShpPoint point)
        {
            bool result = false;

            if (points.Count > 4) // since we save start and end points seperately
            {
                int index = points.IndexOf(point);

                result = points.Remove(point);

                if (result)
                {
                    if (index == 0) // did we delete the root point? Close the polygon again!
                        points[points.Count - 1].moveTo(
                            points[0].RootX,
                            points[0].RootY, false, true);

                    else if (index == points.Count) // did we delete the closing point? (since we removed one point we test against the now smaller list, hence Count can be used
                        points[0].moveTo(
                            points[points.Count - 1].RootX,
                            points[points.Count - 1].RootY, false, true);

                    checkBoundingBox();

                    return index;
                }
            }
            return -1;
        }

        public override void InsertPointAt(int index, double x, double y, double scale)
        {
            ShpPoint pointToAdd = new ShpPoint(x, y, scale);
            if (index == points.Count)
            {
                points.Add(pointToAdd);

                points[0].moveTo(
                        points[points.Count - 1].RootX,
                        points[points.Count - 1].RootY,
                        false,
                        true);
            }
            else
            {
                points.Insert(index, pointToAdd);


                if (index == 0) // did we delete the root point? Close the polygon again!
                {
                    points[points.Count - 1].moveTo(
                        points[0].RootX,
                        points[0].RootY,
                        false,
                        true);
                }
            }

            checkBoundingBox();

            pointToAdd.Changed += new PositionChangedDelegate(vertex_PositionChanged);
        }
    }
}
