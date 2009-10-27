
using System.Drawing;
using System.Windows.Forms;
namespace GravurGIS.Rendering
{
    public class RenderProperties
    {
        public RenderProperties(Graphics g, Rectangle clipRectagle, double dX, double dY, double absoluteZoom,
            bool screenChanged, Rectangle drawingArea, double scale, bool highlightSelectedFeatures)
        {
            this.g = g;
            this.dX = dX;
            this.dY = dY;
            this.absoluteZoom = absoluteZoom;
            this.screenChanged = screenChanged;
            this.drawingArea = drawingArea;
            this.scale = scale;
            this.m_highlight = highlightSelectedFeatures;
            this.m_cliprectangle = clipRectagle;
        }

        private bool m_highlight;

        private Graphics g;

        public Graphics G
        {
            get { return g; }
        }
        private double dX;

        public double DX
        {
            get { return dX; }
        }
        private double dY;

        public double DY
        {
            get { return dY; }
        }
        private double absoluteZoom;

        public double AbsoluteZoom
        {
            get { return absoluteZoom; }
        }
        private bool screenChanged;

        public bool ScreenChanged
        {
            get { return screenChanged; }
        }
        private Rectangle drawingArea;

        public Rectangle DrawingArea
        {
            get { return drawingArea; }
        }
        private double scale;

        public double Scale
        {
            get { return scale; }
        }

        public bool Highlight
        {
            get { return m_highlight; }
        }
        
        private Rectangle m_cliprectangle;
        
        public Rectangle ClipRectangle
        {
			get { return m_cliprectangle; }
		}
    }
}
