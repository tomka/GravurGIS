using GravurGIS.Rendering.Rendering2D;
using System.Drawing;
using GravurGIS.Styles;
using System.Runtime.InteropServices;
using System;

namespace GravurGIS.Rendering.Gdi
{
    class PInvokeVectorRenderer : VectorRenderer2D
    {
        // constants
        private const int BOX_WIDTH = 240;
        private const int BOX_HEIGHT = 240;
        private const int GDI_ITERATIONS_PER_REPORT = 10000;
        private const uint SRCCOPY = 0x00CC0020;
        private const int PS_NULL = 5;

#if PocketPC
        private const string KERNEL_DLL_IMPORT = "coredll.dll";
        private const string GDI_DLL_IMPORT = "coredll.dll";
        private const string USER_DLL_IMPORT = "coredll.dll";
#else
        private const string KERNEL_DLL_IMPORT = "kernel32.dll";
        private const string GDI_DLL_IMPORT = "gdi32.dll";
        private const string USER_DLL_IMPORT = "user32.dll";
#endif
        [DllImport(GDI_DLL_IMPORT, SetLastError = true)]
        public static extern int BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);
        [DllImport(GDI_DLL_IMPORT, SetLastError = true)]
        public static extern int Ellipse(IntPtr hdc, int left, int top, int right, int bottom);
        [DllImport(GDI_DLL_IMPORT, SetLastError = true)]
        public static extern IntPtr CreateSolidBrush(uint colorref);
        [DllImport(GDI_DLL_IMPORT, SetLastError = true)]
        public static extern IntPtr CreatePen(int style, int width, uint color);
        [DllImport(USER_DLL_IMPORT, SetLastError = true)]
        public static extern int FillRect(IntPtr hdc, ref Rectangle rect, IntPtr hBrush);
        [DllImport(GDI_DLL_IMPORT, SetLastError = true)]
        public static extern int DeleteObject(IntPtr hgdiobj);
        [DllImport(GDI_DLL_IMPORT, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport(KERNEL_DLL_IMPORT, SetLastError = true)]
        public static extern int QueryPerformanceCounter(out long counter);
        [DllImport(KERNEL_DLL_IMPORT, SetLastError = true)]
        public static extern int QueryPerformanceFrequency(out long counter);


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
