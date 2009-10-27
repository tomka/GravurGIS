using System;
using System.Collections.Generic;
using System.Text;

namespace GravurGIS.Layers
{
    public class OGRLayer : Layer
    {
        private IntPtr container;
        private int layerID = 0;

        public OGRLayer(GravurGIS.MapPanelBindings.VectorLayerInfo info, IntPtr container)
        {
            this.Description = "OGR Layer";
            this._layerType = LayerType.OGRLayer;
            this.Visible = true;
            this.Changed = true;

            this.layerID = info.id;
            this.container = container;

            this._boundingBox = new GravurGIS.Topology.WorldBoundingBoxD(info.minBX,
                info.maxBY, info.maxBX, info.minBY);
        }

        ~OGRLayer()
        {
            MapPanelBindings.CloseGDAL(container);
        }



        public override bool Render(GravurGIS.Rendering.RenderProperties rp)
        {
            IntPtr hDC = rp.G.GetHdc();
            MapPanelBindings.OGRDrawImage(container, hDC, rp.Scale, rp.DX / rp.Scale, rp.DY / rp.Scale, layerID);
            rp.G.ReleaseHdc(hDC);

            return true;
        }

        public override void reset()
        {
            //throw new NotImplementedException();
        }

        public override void recalculateData(double absoluteZoom, double scale, double xOff, double yOff)
        {
            //throw new NotImplementedException();
        }
    }
}
