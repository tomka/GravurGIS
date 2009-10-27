using System;
using System.Collections.Generic;
using System.Text;
using MapTools;
using GravurGIS.Layers;

namespace GravurGIS.Actions
{
    
    class DrawAction: IAction
    {
		public delegate void ShapeAddedDelegate(ShapeInformation info, DrawAction sender);
		public event ShapeAddedDelegate ShapeAdded;
    
        private LayerType layerType;
        private MainControler mainControler;
        private double absoluteZoom = 1;
        private double x;
        private double y;
        private GravurGIS.ShapeInformation shpInfo;
        private bool bEditingStarted = false;
        private int indexAddedPoint = -1;
        private String Comment;
        private String Category;

        /// <summary>
        /// 
        /// Constructor for Drawing Points
        /// </summary>
        /// <param name="absoluteZoom"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mainControler"></param>

        public DrawAction(double absoluteZoom, double x, double y, MainControler mainControler)
        {
            this.layerType = LayerType.PointCanvas;
            this.mainControler = mainControler;
            this.absoluteZoom = absoluteZoom;
            this.x = x;
            this.y = y;
            this.shpInfo.iShapeInf = null;
            this.shpInfo.quadTreePosItemInf = null;
            this.Comment = String.Empty;
            this.Category = String.Empty;
        }
        /// <summary>
        /// Constructor for Drawing PolyLines and Polygons
        /// </summary>
        /// <param name="layerType"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="bEditingStarted"></param>
        /// <param name="mainControler"></param>
        public DrawAction(LayerType layerType, double x, double y, bool bEditingStarted, MainControler mainControler)
        {
            this.layerType = layerType;
            this.mainControler = mainControler;
            this.x = x;
            this.y = y;
            this.bEditingStarted = bEditingStarted;
        }

		public DrawAction(LayerType layerType, double x, double y, bool bEditingStarted, MainControler mainControler, String Comment, String Category)
			: this(layerType, x, y, bEditingStarted, mainControler)
		{
			this.Comment = Comment;
			this.Category = Category;
		}

        #region IAction Members
        //Draw Action muss auch ein Event im LayerManager ansprechen können
        
        void OnShapeAdded() {
			if (ShapeAdded != null) ShapeAdded(this.shpInfo, this);
        }

        public bool  Execute()
        {
            if (this.layerType == LayerType.PointCanvas)
            {
                this.shpInfo = mainControler.LayerManager.addTransportPointPoint(
                                         x, y, absoluteZoom);
                                         
                this.shpInfo.iShapeInf.Category = Category;
				this.shpInfo.iShapeInf.Commment = Comment;
                
                mainControler.MapPanel.Invalidate();
            }
            else
            {
                if (this.layerType == LayerType.PolylineCanvas)
                {
                    if (bEditingStarted)
                    {
                        this.indexAddedPoint = mainControler.LayerManager.addTransportPolylinePoint(x, y);
                        this.shpInfo = mainControler.LayerManager.getDrawShapeInformation();
                    }
                    else
                    {
                        this.shpInfo = mainControler.LayerManager.addTransportPolyline(x, y);
                        this.indexAddedPoint = 0;
						this.shpInfo.iShapeInf.Category = Category;
						this.shpInfo.iShapeInf.Commment = Comment;
                    }
                }

                if (this.layerType == LayerType.PolygonCanvas)
                {
                    if (bEditingStarted)
                    {
                       // if (disposable) disposable = false;
                        this.indexAddedPoint = mainControler.LayerManager.addTransportPolygonPoint(x, y);
                        this.shpInfo = mainControler.LayerManager.getDrawShapeInformation();
                    }
                    else
                    {
                        this.shpInfo = mainControler.LayerManager.addTransportPolygon(x, y);
                        this.indexAddedPoint = 0;
						this.shpInfo.iShapeInf.Category = Category;
						this.shpInfo.iShapeInf.Commment = Comment;
                    }

                }
                if (bEditingStarted) mainControler.LayerManager.setDrawShapeInformation(DrawShapeInformation.EditStoppedUndone, shpInfo, layerType);
                else mainControler.LayerManager.setDrawShapeInformation(DrawShapeInformation.EditStarted, shpInfo, layerType);
            }
            
            OnShapeAdded();
            
            return true;
        }

        public void  UnExecute()
        {
           
                if (this.shpInfo.quadTreePosItemInf != null)
                {
                    if (this.shpInfo.quadTreePosItemInf.Parent.Type == MapTools.ShapeLib.ShapeType.MultiPoint)
                    {
                        shpInfo.quadTreePosItemInf.Delete();
                        mainControler.LayerManager.TransportPointLayer.removePoint(this.shpInfo.quadTreePosItemInf.Parent);
                    }
                    else
                    {
                        if ((this.indexAddedPoint != -1) && (shpInfo.iShapeInf != null))
                            mainControler.LayerManager.removeTransPortComplexShapePoint(this.indexAddedPoint, this.shpInfo, this.layerType);
                     //   if (this.indexAddedPoint == 0) disposable = true;
                    }
                }

                if ((layerType == LayerType.PolygonCanvas) || (layerType == LayerType.PolylineCanvas))
                {
                    if (indexAddedPoint == 0) mainControler.LayerManager.setDrawShapeInformation(DrawShapeInformation.EditStopped, shpInfo, layerType);
                    else mainControler.LayerManager.setDrawShapeInformation(DrawShapeInformation.EditStarted, shpInfo, layerType);
                }
                mainControler.MapPanel.Invalidate();
            
        }
        public void Dispose()
        {
            /*
            if (disposable) //ShapeObject already deleted through points removing
                 mainControler.LayerManager.RemoveTransportComplexShape(shpInfo,this.layerType);
             */
        }

#endregion
}
}
