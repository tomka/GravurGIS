using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MapTools;
using System.Runtime.InteropServices;
using System.Drawing;
using GravurGIS.Topology;

namespace GravurGIS.Shapes
{
    public abstract class IShape
    {
        private SizeF commentSize = new SizeF();
        protected string comment = String.Empty;
        protected string category = String.Empty;
        protected bool visible = true;
        protected GravurGIS.Topology.QuadTree.QuadTreePositionItem<IShape> qtItem;
        protected double minX, minY, width, height;
        protected double tempMinX, tempMinY;
        private Point drawCommentOffset;
        private int maxSelectDistance = 4;

        public delegate void PositionChangedDelegate(IShape sender, bool combinedMove);
        public event PositionChangedDelegate Changed;

        protected virtual void OnChanged(bool combinedMove)
        {
            if (Changed != null)
                Changed(this, combinedMove);
        }

        public abstract void AddPoint(double x, double y, double scale);
        public abstract void InsertPointAt(int index, double x, double y, double scale);

        public abstract void RemovePoint(int index);
        public abstract int RemovePoint(ShpPoint point);

        /// <summary>
        /// Gets the bouding box of the IShape (without the margins) given a specific
        /// display area.
        /// </summary>
        /// <param name="dX">The x offset of the screen</param>
        /// <param name="dY">The y offset of the screen</param>
        /// <param name="pointSize">The demanded pointsize</param>
        /// <param name="scale">The current scale</param>
        /// <param name="extend">the amount of pixels to be added to each sid</param>
        /// <returns>A Rectangle which is the bouinding box of the IShape</returns>
        public abstract Rectangle getDisplayBoundingBox(double dX, double dY, int pointSize, double scale, int extend);

        /// <summary>
        /// Gets a list of points of the IShape given a specific
        /// display area.
        /// </summary>
        /// <param name="dX">The x offset of the screen</param>
        /// <param name="dY">The y offset of the screen</param>
        /// <param name="absoluteZoom">The current zoom level</param>
        /// <returns>An array of points which represent the IShape</returns>
        public abstract Point[] getPointList(int dX, int dY, double scale);

        //public abstract PointL[] getPointList(long dX, long dY, double scale);

        public abstract int getPointListSize();

        public IShape getLastElement()
        {
            return getElement(getPointListSize() - 1);
        }
        public abstract IShape getElement(int index);

        /// <summary>
        /// Moves the shape to the given destination by changing its
        /// display (e.g. int) coorinates
        /// "updateCoorinates()" should be called after the use of this!
        /// </summary>
        /// <param name="toX">Destination x coordinate</param>
        /// <param name="toY">Destination y coordinate</param>
        public abstract void moveTo(double toX, double toY, bool temp, bool combinedMove);

        /// <summary>
        /// Moves the shape to the given destination by changing its
        /// display (e.g. int) coorinates by the given difference
        /// "updateCoorinates()" should be called after the use of this!
        /// </summary>
        /// <param name="difX">Destination x coordinate</param>
        /// <param name="difY">Destination y coordinate</param>
        /// <param name="temp">Is this move a temporary move?</param>
        public abstract void moveToByDifference(double difX, double difY, bool temp);

        internal abstract void moveToByDifference(double difX, double difY, bool temp, bool combinedMove);

        /// <summary>
        /// Return the ShapeType of the interface' implementation
        /// </summary>
        public abstract ShapeLib.ShapeType Type { get; }

        /// <summary>
        /// A Function to get a List of all x components of a shape
        /// </summary>
        /// <returns></returns>
        public abstract double[] getXList();

        /// <summary>
        /// A Function to get a List of all y components of a shape
        /// It mirrors the y values horizontally - thus the height of
        /// the context (e.g. ESRI shape file), may with left offset, is needed!
        /// </summary>
        /// <returns></returns>
        public abstract double[] getYList();

        public abstract void Delete();

        public GravurGIS.Topology.QuadTree.QuadTreePositionItem<IShape> Reference
        {
            get { return qtItem; }
            set
            {
                if (qtItem != null) qtItem.Delete();
                qtItem = value;
            }
        }

        protected abstract void checkBoundingBox();

        //protected void checkBoundingBox(double x, double y)
        //{
        //    if (x < minX)
        //    {
        //        width += (minX - x);
        //        minX = tempMinX = x;
        //    }
        //    if (y < minY)
        //    {
        //        height += (minY - y);
        //        minY = tempMinY = y;
        //    }
        //    if (x > (minX + width)) width = x - minX;
        //    if (y > (minY + height)) height = y - minY;
        //}

        public Point DrawCommentOffset
        {
            get { return drawCommentOffset; }
            set { drawCommentOffset = value; }
        }

        public abstract int PointCount { get; }
        public abstract double CenterX { get; }
        public abstract double CenterY { get; }
        public abstract double Width { get; }
        public abstract double Height { get; }
        public abstract double RootX { get; }
        public abstract double RootY { get; }
        public abstract double MinX { get; }
        public abstract double MinY { get; }
        public string Commment {
			get { return comment; }
			set
			{
				if (value == null)
					comment = String.Empty;
				else
					comment = value;
			}
		 }
        public abstract bool Visible { get; set; }
        public SizeF StringSize
        {
            get { return commentSize; }
            set { this.commentSize = value; }
        }

        /// <summary>
        /// Gets or sets the category field used as a column "Category" in the dbf file
        /// </summary>
        public String Category
        {
            get { return category; }
            set {
				if (value == null)
					category = String.Empty;
				else
					category = value;
			}
        }

        public int MaxSelectDistance
        {
            get { return maxSelectDistance; }
        }

        public bool IsHighlighted
        {
            get;
            set;
        }

        public abstract IShape NearestPointTo(PointD position, double maxDistance);
    }
}
