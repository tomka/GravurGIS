using GravurGIS.CoordinateSystems;
using GravurGIS.Topology;

namespace GravurGIS.Actions
{
    class PanAction: IAction
    {
        private PointD oldD;
        private PointD d;
        private double scale;
        private MapPanel mapPanel;
        private CoordinateType cType;
        /// <summary>
        /// Generates a pan action. If one sets the coordinate type to world both, new and old d, have to be that type
        /// </summary>
        /// <param name="oldD"></param>
        /// <param name="d"></param>
        /// <param name="scale"></param>
        /// <param name="mapPanel"></param>
        /// <param name="coordinatType"></param>
        public PanAction(PointD oldD, PointD newD, double scale, MapPanel mapPanel, CoordinateType coordinatType)
        {
            this.cType = coordinatType;
            this.oldD = oldD;
            this.d = newD;
            this.scale = scale;
            this.mapPanel = mapPanel;
        }

        #region IAction Members

        public bool Execute()
        {
            if (cType == CoordinateType.Display)
            {
                MapPanelBindings.RecalculateImages(scale, (d.x / scale), (d.y / scale));

                mapPanel.ViewHasChanged(d);
            }
            else
            {
                MapPanelBindings.RecalculateImages(scale, (d.x), (d.y));

                mapPanel.ViewHasChanged(d * scale);
            }

            return true;
        }

        public void UnExecute()
        {
            if (cType == CoordinateType.Display)
            {
                MapPanelBindings.RecalculateImages(scale, (oldD.x / scale), (oldD.y / scale));

                mapPanel.ViewHasChanged(oldD);
            }
            else
            {
                MapPanelBindings.RecalculateImages(scale, (oldD.x), (oldD.y));

                mapPanel.ViewHasChanged(oldD * scale);
            }
        }

        public void Dispose()
        {
          
        }

        #endregion
    }

}
