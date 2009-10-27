using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace GravurGIS.Rendering.Rendering2D
{
    public abstract class VectorRenderer2D : IVectorRenderer2D
    {
        #region IVectorRenderer2D Members

        public abstract void DrawLine(GravurGIS.Styles.StylePen pen, int x1, int y1, int x2, int y2);

        public abstract void DrawString(string text, System.Drawing.Font font, GravurGIS.Styles.SolidStyleBrush brush, int x, int y, System.Drawing.StringFormat format);

        public abstract void FillRectangle(GravurGIS.Styles.StyleBrush brush, System.Drawing.Rectangle rectangle);

        public abstract void DrawLines(GravurGIS.Styles.StylePen pen, System.Drawing.Point[] points);

        public abstract void FillPolygon(GravurGIS.Styles.StyleBrush brush, System.Drawing.Point[] points);

        public abstract void DrawRectangle(GravurGIS.Styles.StylePen pen, System.Drawing.Rectangle rectangle);

        protected Graphics _graphics;
        public Graphics Graphics
        {
            get { return _graphics; }
            set { _graphics = value; }
        }

        public abstract void FillRectangle(GravurGIS.Styles.SolidStyleBrush brush, int x, int y, int width, int height);

        #endregion
    }
}
