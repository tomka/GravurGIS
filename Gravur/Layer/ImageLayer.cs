using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using GravurGIS.Rendering;

namespace GravurGIS.Layers
{
    public class ImageLayer : Layer
    {
        private LayerManager layermanager;
        private int layerID = 0;
        private double[] resolution;
        private bool whiteIsTransparent = true;
        private LayerInfo layerInfo;

        public ImageLayer(LayerManager lm, GravurGIS.MapPanelBindings.ImageLayerInfo info, string name, string path)
        {
            this.Description = "Rasterbild";
            this._layerType = LayerType.Image;
            this.Visible = true;
            this.Changed = true;

            this.layerID = info.id;

            this._boundingBox = new GravurGIS.Topology.WorldBoundingBoxD(info.minBX,
                info.maxBY, info.maxBX, info.minBY);

            this.LayerName = name;
            this.layerInfo = new LayerInfo(path + "\\" + name);
            this.layermanager = lm;
            this.resolution = new double[2];
            this.resolution[0] = info.resolutionX;
            this.resolution[1] = info.resolutionY;
        }

        public double ResolutionX
        {
            get { return resolution[0]; }
        }
        public double ResolutionY
        {
            get { return resolution[1]; }
        }

        #region ILayer Members

        public override bool Render(RenderProperties rp)
        {
            double xOff = rp.DX / rp.Scale;
            double yOff = rp.DY / rp.Scale;

            if (Changed)
                recalculateData(rp.AbsoluteZoom, rp.Scale, xOff, yOff);

            IntPtr hDC = rp.G.GetHdc();
                MapPanelBindings.GDALDrawImage(layermanager.CGDALContainer, hDC,
                        xOff, yOff, this.layerID);
            rp.G.ReleaseHdc(hDC);
            return true;
        }

        public override void reset()
        {
            // do nothing special
        }

        public override void recalculateData(double absoluteZoom, double scale, double xOff, double yOff)
        {
            MapPanelBindings.RecalculateImage(scale, xOff, yOff, this.layerID);
            this.Changed = false;
        }

        public string FileName
        {
            get { return layerInfo.FileName;  }
        }
        public string FilePath
        {
            get { return layerInfo.FilePath; }
        }
        public bool WhiteIsTransparent
        {
            get { return whiteIsTransparent; }
            set
            {
                whiteIsTransparent = value;
                MapPanelBindings.SetLayerTransparency(this.layerID, value);
                this.Changed = true;
            }
        }

        public LayerInfo LayerInfo
        {
            get { return this.layerInfo; }
            set { layerInfo = value; }
        }

        #endregion
    }
}
