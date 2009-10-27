using GravurGIS.Rendering.Rendering2D;
using System.Drawing;
using GravurGIS.Styles;

namespace GravurGIS.Rendering.Gdi
{
    class GdiVectorRenderer : VectorRenderer2D
    {
        public override void DrawLine(GravurGIS.Styles.StylePen pen, int x1, int y1, int x2, int y2)
        {
            StyleColor color = pen.BackgroundBrush.Color;

            _graphics.DrawLine(new Pen(Color.FromArgb(color.R, color.G, color.B), pen.Width),
                x1, y1, x2, y2);
        }

        public override void DrawString(string text, System.Drawing.Font font, GravurGIS.Styles.SolidStyleBrush brush, int x, int y, System.Drawing.StringFormat format)
        {
            StyleColor color = brush.Color;
            _graphics.DrawString(text, font, new SolidBrush(Color.FromArgb(color.R, color.G, color.B)), x, y);
        }

        public override void FillRectangle(GravurGIS.Styles.StyleBrush brush, System.Drawing.Rectangle rectangle)
        {
            StyleColor color = brush.Color;
            _graphics.FillRectangle(new SolidBrush(Color.FromArgb(color.R, color.G, color.B)), rectangle);
        }

        public override void DrawLines(GravurGIS.Styles.StylePen pen, System.Drawing.Point[] points)
        {
            StyleColor color = pen.BackgroundBrush.Color;
            _graphics.DrawLines(new Pen(Color.FromArgb(color.R, color.G, color.B), pen.Width), points);
        }

        public override void FillPolygon(GravurGIS.Styles.StyleBrush brush, System.Drawing.Point[] points)
        {
            StyleColor color = brush.Color;
            _graphics.FillPolygon(new SolidBrush(Color.FromArgb(color.R, color.G, color.B)), points);
        }

        public override void FillRectangle(SolidStyleBrush brush, int x, int y, int width, int height)
        {
            StyleColor color = brush.Color;
            _graphics.FillRectangle(new SolidBrush(Color.FromArgb(color.R, color.G, color.B)), x, y, width, height);
        }

        public override void DrawRectangle(GravurGIS.Styles.StylePen pen, System.Drawing.Rectangle rectangle)
        {
            StyleColor color = pen.BackgroundBrush.Color;
            _graphics.DrawRectangle(new Pen(Color.FromArgb(color.R, color.G, color.B), pen.Width), rectangle);
        }
    }
}
