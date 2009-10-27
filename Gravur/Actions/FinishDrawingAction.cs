using System;
using System.Collections.Generic;
using System.Text;
using GravurGIS.Layers;
//using Gravur.Shapes;

namespace GravurGIS.Actions
{
    class FinishDrawingAction: IAction
    {
        private LayerType layerType;
        private LayerManager layerManager;
        private ShapeInformation shpInfo;

       

        public FinishDrawingAction(LayerType layerType,LayerManager layerManager)
        {
            this.layerType = layerType;
            this.layerManager = layerManager;
            this.shpInfo = layerManager.getDrawShapeInformation();
        }
        #region IAction Members

        public bool Execute()
        {
            this.shpInfo = layerManager.getDrawShapeInformation();
            double scale = layerManager.Scale;
            if (layerType == LayerType.PolygonCanvas)
            {
                this.shpInfo.iShapeInf.AddPoint(
                    this.shpInfo.iShapeInf.RootX,
                    this.shpInfo.iShapeInf.RootY, scale);
            }

            ShapeInformation temp;
            temp.iShapeInf = null;
            temp.quadTreePosItemInf = null;
            layerManager.setDrawShapeInformation(DrawShapeInformation.EditStopped,temp,layerType);
            return true;
        }

        public void UnExecute()
        {
            if (layerType == LayerType.PolygonCanvas)
            { 
                this.shpInfo.iShapeInf.RemovePoint((this.shpInfo.iShapeInf.getPointListSize()-1));
            }
            layerManager.setDrawShapeInformation(DrawShapeInformation.EditStoppedUndone,this.shpInfo,layerType);
        }

        #endregion

        #region IAction Members


        public void Dispose()
        {
            
        }

        #endregion
    }
}
