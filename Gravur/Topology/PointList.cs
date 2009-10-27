using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace GravurGIS.Topology
{
    public class PointList
    {
        public Point[] displayPointList;
        public PointD[] worldPointList;
        public int Length;

        /// <summary>
        /// Creates a new point list of a given size
        /// </summary>
        /// <param name="size">The desired length of the point list</param>
        public PointList(int size)
        {
            displayPointList = new Point[size];
            worldPointList = new PointD[size];
            Length = size;
        }

        /// <summary>
        /// Adds a pair of doubles to the pointList
        /// </summary>
        public void add(double x, double y, int xDisp, int yDisp, int pos)
        {
            this.worldPointList[pos] = new PointD(x, y);
            this.displayPointList[pos] = new Point(xDisp, yDisp);
        }

        /// <summary>
        /// Returns a specific point of the display pointList
        /// </summary>
        /// <param name="pos">the index of the point</param>
        /// <returns></returns>
        public Point getDispPoint(int pos)
        {
            return this.displayPointList[pos];
        }

        /// <summary>
        /// This function updates all the points to the new scale
        /// </summary>
        public void recalculatePoints(double scale, int dispHeight, double bbMinX, double bbMinY)
        {
            for (int i = worldPointList.Length - 1; i >= 0; i--)
            {
                displayPointList[i].X = Convert.ToInt32((worldPointList[i].x - bbMinX) * scale);

                displayPointList[i].Y = dispHeight - Convert.ToInt32((worldPointList[i].y - bbMinY) * scale);
            }
        }
    }
}
