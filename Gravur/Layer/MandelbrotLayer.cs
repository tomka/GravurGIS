using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using GravurGIS.Rendering;

namespace GravurGIS.Layers
{
    class MandelbrotLayer : Layer
    {
        int maxIterations;
        double xPos, yPos, size;
        private IntPtr cMandelbrot;

        public MandelbrotLayer(IntPtr cMandelbrot, double originX, double originY, int width, int height,
            int maxIterations, double xPos, double yPos, double size)
        {
            this._layerType = LayerType.Image;
            this._boundingBox = new GravurGIS.Topology.WorldBoundingBoxD(originX, originY + height, originX + width, originY);
            this.LayerName = "Mandelbrot";
            this.Description = "Fraktal-Beispiel";
            this.Visible = true;
            this.Changed = true;

            this.maxIterations = maxIterations;
            this.xPos = xPos;
            this.yPos = yPos;
            this.size = size;
            this.cMandelbrot = cMandelbrot;
        }

        #region ILayer Members

        public override bool Render(RenderProperties rp)
        {
            double newsize = size / rp.AbsoluteZoom;

            double CurrentxPos = (xPos + rp.DX / rp.AbsoluteZoom) * (newsize / Height);
            double CurrentyPos = (rp.DY / rp.AbsoluteZoom - yPos) * (newsize / Height);

            IntPtr hDC = rp.G.GetHdc();
            MapPanelBindings.DrawMandelbrot(
                cMandelbrot, hDC,
                rp.DX, rp.DY,
                maxIterations,
                CurrentxPos,
                CurrentyPos,
                newsize);
            rp.G.ReleaseHdc(hDC);

            return true;



            //Color[] cs = new Color[256];
            //// Fills cs with the colors from the current ColorMap file
            //cs = GetColors(ColMaps[CurColMap]);
            //// Creates the Bitmap we draw to
            //Bitmap b = new Bitmap(this.Width, this.Height);
            //// From here on out is just converted from the c++ version.
            //double x, y, x1, y1, xx, xmin, xmax, ymin, ymax = 0.0;

            //int looper, s, z = 0;
            //double intigralX, intigralY = 0.0;
            //xmin = Sx; // Start x value, normally -2.1
            //ymin = Sy; // Start y value, normally -1.3
            //xmax = Fx; // Finish x value, normally 1
            //ymax = Fy; // Finish y value, normally 1.3
            //intigralX = (xmax - xmin) / this.Width; // Make it fill the whole window
            //intigralY = (ymax - ymin) / this.Height;
            //x = xmin;

            //for (s = 1; s < this.Width; s++)
            //{
            //    y = ymin;
            //    for (z = 1; z < this.Height; z++)
            //    {
            //        x1 = 0;
            //        y1 = 0;
            //        looper = 0;
            //        while (looper < 100 && Math.Sqrt((x1 * x1) + (y1 * y1)) < 2)
            //        {
            //            looper++;
            //            xx = (x1 * x1) - (y1 * y1) + x;
            //            y1 = 2 * x1 * y1 + y;
            //            x1 = xx;
            //        }

            //        // Get the percent of where the looper stopped
            //        double perc = looper / (100.0);
            //        // Get that part of a 255 scale
            //        int val = ((int)(perc * 255));
            //        // Use that number to set the color
            //        b.SetPixel(s, z, cs[val]);
            //        y += intigralY;
            //    }
            //    x += intigralX;
            //}
            //bq = b; // bq is a globally defined bitmap
            //this.BackgroundImage = (Image)bq; // Draw it to the form   
        }

        public override void reset()
        {
            // do nothing yet
        }

        public override void recalculateData(double absoluteZoom, double scale, double xOff, double yOff)
        {
            // do nothing yet
        }

        #endregion
    }
}
