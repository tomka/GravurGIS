using System;
using System.Collections.Generic;
using System.Text;
using GravurGIS.Shapes;
using System.Drawing;
using MapTools;
using GravurGIS.Topology;

namespace GravurGIS.Shapes
{
    public class ShpPolyline : IShape
    {
        private List<ShpPoint> points;

        public ShpPolyline(double x, double y, double scale)
        {
            points = new List<ShpPoint>();
            this.minX = tempMinX = x;
            this.minY = tempMinY = y;
            this.width = 1;
            this.height = 1;

            AddPoint(x, y, scale);
        }

        public override void AddPoint(double x, double y, double scale)
        {
            ShpPoint pointToAdd = new ShpPoint(x, y, scale);
            points.Add(pointToAdd);
            checkBoundingBox();
            
            pointToAdd.Changed += new PositionChangedDelegate(vertex_PositionChanged);
        }

        private void vertex_PositionChanged(IShape sender, bool combinedMove)
        {
            if (!combinedMove)
            {
                checkBoundingBox();
                base.OnChanged(true);
            }
        }

        #region IShape Members

        public override void Delete()
        {
            qtItem.Delete();
        }

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
            get { return ShapeLib.ShapeType.PolyLine; }
        }

        public override void moveTo(double toX, double toY, bool temp, bool combinedMove)
        {
            for (int i = points.Count - 1; i >= 0; i--)
                points[i].moveToByDifference(toX - minX, toY - minY, temp, combinedMove);


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
            get
            { return points.Count; }
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

        public override double MinX
        {
            get { return minX; }
        }
        public override double MinY
        {
            get { return minY; }
        }

        public override double Height
        {
            get { return height; }
        }

        public override double Width
        {
            get { return width; }
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
        /// Gets the length of the polyline
        /// </summary>
        public Double Length
        {
            get
            {
                double length = 0;
                double x, y;

                for (int i = 1; i < points.Count; ++i)
                {
                    x = points[i - 1].CenterX - points[i].CenterX;
                    y = points[i - 1].CenterY - points[i].CenterY;

                    length += Math.Sqrt(x * x + y * y);
                }

                return length;
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


            double x,y;

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
            int index = points.IndexOf(point);

            bool result = points.Remove(point);
            
            if (points.Count == 0)
            {
                minX = double.NaN;
                minY = double.NaN;
                width = double.NaN;
                height = double.NaN;
            }
            else
            {
                checkBoundingBox();
            }

            if (result) return index;
            else return -1;
        }

        public override void InsertPointAt(int index, double x, double y, double scale)
        {
            ShpPoint pointToAdd = new ShpPoint(x, y, scale);

            points.Insert(index, pointToAdd);
            checkBoundingBox();

            pointToAdd.Changed += new PositionChangedDelegate(vertex_PositionChanged);
        }
    }
}
