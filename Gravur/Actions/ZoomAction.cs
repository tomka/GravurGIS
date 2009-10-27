using System;
using System.Collections.Generic;
using System.Text;
using GravurGIS.Layers;
using GravurGIS.Topology;

namespace GravurGIS.Actions
{
    class ZoomAction: IAction
    {
        private double oldAbsoluteZoom;
        private PointD oldD;
        private double firstScale;
        private double newScale;
        /// <summary>
        /// testAbsolutZoom is calculated in Execute Method of ZoomAction ans saves the highest absoluteZoom possible
        /// </summary>
        private double maxAbsZoom;
        private MainControler mainControler;
        private double absoluteZoom;
        private PointD d;
        private Layer layer;
        private PointD unscaledP;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldAbsoluteZoom"></param>
        /// <param name="oldD">old dx and dy</param>
        /// <param name="d">new dx and dy</param>
        /// <param name="dpiX">number of display pixels horizontically</param>
        /// <param name="dpiY">number of display pixels vertically</param>
        /// <param name="absoluteZoom"></param>
        /// <param name="unscaledP">actual click point in GK coordinates</param>
        /// <param name="mainControler"></param>
        public ZoomAction(double oldAbsoluteZoom, PointD oldD, PointD d,
            double absoluteZoom,
            PointD unscaledP, double maxAbsoluteZoom, MainControler mainControler)
        {
            this.oldAbsoluteZoom = oldAbsoluteZoom;
            this.oldD = oldD;
            this.mainControler = mainControler;
            this.absoluteZoom = absoluteZoom;
            this.d = d;
            this.firstScale = mainControler.LayerManager.FirstScale;
            this.unscaledP = unscaledP;
            this.maxAbsZoom = maxAbsoluteZoom;
        }

        #region IAction Members

        public bool Execute()
        {
            newScale = firstScale * absoluteZoom;

            // test for the maximum possible zoom level
            //PointD tempMax = new PointD(-1.0, -1.0);
            //for(int i = mainControler.LayerManager.LayerCount-1; i>=0;i--)
            //{
            //    tempMax.x = Math.Max(mainControler.LayerManager.LayerArray[i].BoundingBox.Right, tempMax.x);
            //    tempMax.y = Math.Max(mainControler.LayerManager.LayerArray[i].BoundingBox.Top,tempMax.y);    
            //}
            //testAbsolutZoom = Int32.MaxValue/(mainControler.LayerManager.FirstScale * Math.Max(tempMax.x,tempMax.y));

           if (absoluteZoom < this.maxAbsZoom)
            {
                double xOff = (d.x / newScale);
                double yOff = (d.y / newScale);
                DoCalculation(xOff, yOff, absoluteZoom, newScale);
                mainControler.LayerManager.Scale = newScale;
                mainControler.MapPanel.ViewHasChanged(d);
                mainControler.MapPanel.SetPositionStatus(unscaledP.x, unscaledP.y);
                return true;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Die Maximale Zoomtiefe wurde erreicht.", "Hinweis", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Asterisk, System.Windows.Forms.MessageBoxDefaultButton.Button1);
                mainControler.MapPanel.ViewHasChanged(oldD);
                mainControler.MapPanel.SetPositionStatus(unscaledP.x,unscaledP.y); //TODO: das ist nicht der letzte Klickpunkt--> Probleme
                return false;
            }
        }

        public void UnExecute()
        {
            if (this.absoluteZoom < maxAbsZoom)
            {
                double old_scale = firstScale * oldAbsoluteZoom;
                double xOff = (oldD.x / old_scale);
                double yOff = (oldD.y / old_scale);
                DoCalculation(xOff, yOff, oldAbsoluteZoom, old_scale);
                mainControler.LayerManager.Scale = old_scale;
                mainControler.MapPanel.ViewHasChanged(oldD);
                mainControler.MapPanel.SetPositionStatus(unscaledP.x,unscaledP.y);
            }
        }

        public void DoCalculation(double xOff,double yOff,double absoluteZoom,double new_scale)
        {    
            for (int i = mainControler.LayerManager.LayerCount - 1; i >= 0; i--)
            {
                layer = mainControler.LayerManager.LayerArray[i];
                layer.recalculateData(absoluteZoom, new_scale, xOff, yOff);
                layer.Changed = true;
                layer.reset();
                mainControler.MapPanel.ScreenChanged = true;
            }    
        }

        public void Dispose()
        {

        }
       
        #endregion

        #region Getters/Setters
      


        #endregion
    }
}
