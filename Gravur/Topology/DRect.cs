// // // // // // // // // // // // //
// QuadTree and supporting code
// by Kyle Schouviller
// http://www.kyleschouviller.com
//
// December 2006: Original version
// May 06, 2007:  Updated for XNA Framework 1.0
//                and public release.
//
// You may use and modify this code however you
// wish, under the following condition:
// *) I must be credited
// A little line in the credits is all I ask -
// to show your appreciation.
// 
// If you have any questions, please use the
// contact form on my website.
//
// Now get back to making great games!
// // // // // // // // // // // // //
// Modified by Tom
// // // // // // // // // // // // //

#region Using declarations

using System;
using System.Collections.Generic;
using System.Text;
using GravurGIS.Topology;

#endregion

namespace GravurGIS.Topology
{
    /// <summary>
    /// A double bouding box with minimum y at top (like screen)
    /// </summary>
    public struct DBoudingBox
    {
        #region Properties

        /// <summary>
        /// The top left of this rectangle
        /// </summary>
        public GravurGIS.Topology.Vector2 topLeft;

        /// <summary>
        /// The bottom right of this rectangle
        /// </summary>
        public GravurGIS.Topology.Vector2 size;

        #endregion

        #region Initialization

        /// <summary>
        /// Floating-point rectangle constructor
        /// </summary>
        /// <param name="top">The top of the rectangle</param>
        /// <param name="left">The left of the rectangle</param>
        /// <param name="bottom">The bottom of the rectangle</param>
        /// <param name="right">The right of the rectangle</param>
        public DBoudingBox(double left, double top, double width, double height)
        {
            topLeft = new Vector2(left, top);
            size    = new Vector2(width, height);
        }

        #endregion
    }
}

namespace GravurGIS.Topology
{
    /// <summary>
    /// A double rectangle with the minimum y is top (often used on screens)
    /// </summary>
    public struct DRect
    {
        #region Properties

        /// <summary>
        /// The top left of this rectangle
        /// </summary>
        private Vector2 topLeft;

        /// <summary>
        /// The bottom right of this rectangle
        /// </summary>
        private Vector2 bottomRight;

        /// <summary>
        /// Gets the top left of this rectangle
        /// </summary>
        public Vector2 TopLeft
        {
            get { return topLeft; }
            set { topLeft = value; }
        }

        /// <summary>
        /// Gets the top right of this rectangle
        /// </summary>
        public Vector2 TopRight
        {
            get { return new Vector2(bottomRight.X, topLeft.Y); }
            set
            {
                bottomRight.X = value.X;
                topLeft.Y = value.Y;
            }
        }

        /// <summary>
        /// Gets the bottom right of this rectangle
        /// </summary>
        public Vector2 BottomRight
        {
            get { return bottomRight; }
            set { bottomRight = value; }
        }

        /// <summary>
        /// Gets the bottom left of this rectangle
        /// </summary>
        public Vector2 BottomLeft
        {
            get { return new Vector2(topLeft.X, bottomRight.Y); }
            set
            {
                topLeft.X = value.X;
                bottomRight.Y = value.Y;
            }
        }

        /// <summary>
        /// Gets the top of this rectangle
        /// </summary>
        public double Top
        {
            get { return TopLeft.Y; }
            set { topLeft.Y = value; }
        }

        /// <summary>
        /// Gets the left of this rectangle
        /// </summary>
        public double Left
        {
            get { return TopLeft.X; }
            set { topLeft.X = value; }
        }

        /// <summary>
        /// Gets the bottom of this rectangle
        /// </summary>
        public double Bottom
        {
            get { return BottomRight.Y; }
            set { bottomRight.Y = value; }
        }

        /// <summary>
        /// Gets the right of this rectangle
        /// </summary>
        public double Right
        {
            get { return BottomRight.X; }
            set { bottomRight.X = value; }
        }

        /// <summary>
        /// Gets the width of this rectangle
        /// </summary>
        public double Width
        {
            get { return bottomRight.X - topLeft.X; }
        }

        /// <summary>
        /// Gets the height of this rectangle
        /// </summary>
        public double Height
        {
            get { return bottomRight.Y - topLeft.Y; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Floating-point rectangle constructor
        /// </summary>
        /// <param name="topleft">The top left point of the rectangle</param>
        /// <param name="bottomright">The bottom right point of the rectangle</param>
        public DRect(Vector2 topleft, Vector2 bottomright)
        {
            topLeft = topleft;
            bottomRight = bottomright;
        }

        /// <summary>
        /// Floating-point rectangle constructor
        /// </summary>
        /// <param name="top">The top of the rectangle</param>
        /// <param name="left">The left of the rectangle</param>
        /// <param name="bottom">The bottom of the rectangle</param>
        /// <param name="right">The right of the rectangle</param>
        public DRect(double left, double top, double right, double bottom)
        {
            topLeft = new Vector2(left, top);
            bottomRight = new Vector2(right, bottom);
        }

        #endregion

        #region Intersection testing functions

        /// <summary>
        /// Checks if this rectangle contains a point
        /// </summary>
        /// <param name="Point">The point to test</param>
        /// <returns>Whether or not this rectangle contains the point</returns>
        public bool Contains(Vector2 Point)
        {
            return (topLeft.X <= Point.X && bottomRight.X >= Point.X &&
                    topLeft.Y <= Point.Y && bottomRight.Y >= Point.Y);
        }

        /// <summary>
        /// Checks if this rectangle intersects another rectangle
        /// </summary>
        /// <param name="Rect">The rectangle to check</param>
        /// <returns>Whether or not this rectangle intersects the other</returns>
        public bool Intersects(DRect Rect)
        {
            return (!( Bottom < Rect.Top ||
                       Top > Rect.Bottom ||
                       Right < Rect.Left ||
                       Left > Rect.Right ));
        }

        #endregion
    }

    
}
