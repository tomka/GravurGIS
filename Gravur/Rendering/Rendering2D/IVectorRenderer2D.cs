using System;
using System.Collections.Generic;
using System.Text;
using GravurGIS.Styles;
using System.Drawing;

namespace GravurGIS.Rendering.Rendering2D
{
    public interface IVectorRenderer2D
    {
        void DrawLine(StylePen pen, int x1, int y1, int x2, int y2);
        void DrawLines(StylePen pen, Point[] points);
        void DrawRectangle(StylePen pen, Rectangle rectangle);

        void FillRectangle(StyleBrush brush, Rectangle rectangle);
        void FillPolygon(StyleBrush brush, Point[] points);

        void DrawString(String text, Font font, SolidStyleBrush brush, int x, int y, StringFormat format);

        Graphics Graphics { get; set; }

        void FillRectangle(SolidStyleBrush brush, int x, int y, int width, int height);
    }
}
