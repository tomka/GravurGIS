using System;
using System.Collections.Generic;
using System.Text;
using GravurGIS.Shapes;
using System.Drawing;
using MapTools;
using GravurGIS.Topology;

namespace GravurGIS.Shapes
{
    public class ShpPoint : IShape
    {
        private double x, y, tempX, tempY;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">The unscaled and unzoomed x display coordinate</param>
        /// <param name="y">The unscaled and unzoomed y display coordinate</param>
        /// <param name="scale">The, here not needed, scale</param>
        public ShpPoint(double x, double y, double currentScale)
        {
            this.x = tempX = x;
            this.y = tempY = y;
        }

        #region IShape Members

        public override void Delete()
        {
            qtItem.Delete();
        }

        public override Rectangle getDisplayBoundingBox(double dX, double dY, int pointSize, double scale, int extend)
        {
            return new Rectangle(
                (int)(x * scale - dX) - (pointSize / 2) - extend,
                (int)(y * scale + dY) - (pointSize / 2) - extend,
                pointSize + 2*extend, pointSize + 2*extend);
        }


        public override MapTools.ShapeLib.ShapeType Type
        {
            get { return ShapeLib.ShapeType.MultiPoint; }
        }

        /// <summary>
        /// Updates the base coorinates (the unscaled coorinates withouth the origin
        /// of the shape)
        /// </summary>
        /// <param name="toX"></param>
        /// <param name="toY"></param>
        public override void moveTo(double toX, double toY, bool temp, bool combinedMove)
        {
            this.x = toX;
            this.y = toY;

            if (!temp)
            {
                tempX = x;
                tempY = y;
            }

            base.OnChanged(combinedMove);
        }

        internal override void moveToByDifference(double difX, double difY, bool temp, bool combinedMove)
        {
            this.x = tempX + difX;
            this.y = tempY + difY;

            if (!temp)
            {
                tempX = x;
                tempY = y;
            }

            base.OnChanged(combinedMove);
        }

        public override void moveToByDifference(double difX, double difY, bool temp)
        {
            moveToByDifference(difX, difY, temp, false);
        }

        public override Point[] getPointList(int dX, int dY, double scale)
        {
            Point[] returnList = new Point[1];
            returnList[0] = getPoint(dX, dY, scale);

            return returnList;
        }

        public override int getPointListSize()
        {
            return 1;
        }

        public Point getPoint(int dX, int dY, double scale)
        {
            return new Point(Convert.ToInt32(x * scale) - dX,
                Convert.ToInt32(y * scale) + dY);
        }

        public override void AddPoint(double x, double y, double scale)
        {
            throw new Exception("AddPoint can not be applied to a point!");
        }

        public override double[] getXList()
        {
            double[] returnArray = new double[1];
            returnArray[0] = x;
            return returnArray;
        }

        public override double[] getYList()
        {
            double[] returnArray = new double[1];
            returnArray[0] = y;
            return returnArray;
        }

        public override double CenterX
        {
            get { return x; }
        }

        public override double CenterY
        {
            get { return y; }
        }

        public override double Width
        {
            get { return 1.0d; }
        }

        public override double Height
        {
            get { return 1.0d; }
        }

        public override double RootX
        {
            get { return x; }
        }

        public override double RootY
        {
            get { return y; }
        }

        public override int PointCount
        {
            get { return 1; }
        }

        public override double MinX
        {
            get { return x; }
        }
        public override double MinY
        {
            get { return y; }
        }

        public override void RemovePoint(int index)
        {
            throw new Exception("A point can not delete itself.");
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

        public override IShape getElement(int index)
        {
            if ((index >= 1)
                || (index < 0))
                throw new ArgumentOutOfRangeException("index", "index was: " + index.ToString());

            return this;

        }

        /// <summary>
        /// Compares two points on coordinate basis. if they have the same coodinates they are considered equal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            ShpPoint pnt = obj as ShpPoint;

            if (pnt != null)
            {
                if (this.CenterX == pnt.CenterX && pnt.CenterY == this.CenterY)
                    return true;
            }
            
            return false;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="GetHashCode"/> is suitable for use 
        /// in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>A hash code for the current <see cref="GetHashCode"/>.</returns>
        public override Int32 GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode(); // ^ IsEmpty.GetHashCode();
        }

        public override IShape NearestPointTo(PointD position, double maxDistance)
        {
            double xDist = x - position.x;
            double yDist = y - position.y;

            if (xDist == 0 && yDist == 0) 
                return this;

            if (Math.Sqrt(xDist * xDist + yDist * yDist) <= maxDistance)
                return this;

            return null;
        }

        protected override void checkBoundingBox()
        {
            // nothing to do here
        }

        public override int RemovePoint(ShpPoint point)
        {
            throw new Exception("A point can not delete itself.");
        }

        public override void InsertPointAt(int index, double x, double y, double scale)
        {
            throw new NotImplementedException("You can insert nothing in a point.");
        }
    }
}
