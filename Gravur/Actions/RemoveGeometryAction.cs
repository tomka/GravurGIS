using System;
using System.Collections.Generic;
using System.Text;
using GravurGIS.Shapes;
using System.Drawing;
using GravurGIS.Topology;

namespace GravurGIS.Actions
{
    class RemoveGeometryAction : IAction
    {
        private ShpPoint shapeToDelete;
        private PointD deletedPoint;
        private IShape container;
        private int indexOfDeletedVertex = -1;
        private List<Rectangle> rectangleList = new List<Rectangle>();
        private PointD d = new PointD();
        private int pointSize;
        private double scale;
        private LayerManager layerManager;
 
        public RemoveGeometryAction(ShpPoint shapeToRemove, IShape container,
            LayerManager layerManager)
        {
            this.layerManager = layerManager;
            this.d.x = this.layerManager.GetMainControler().MapPanel.DX;
            this.d.y = this.layerManager.GetMainControler().MapPanel.DY;
            this.scale = layerManager.Scale;
            this.shapeToDelete = shapeToRemove;
            this.container = container;
            this.pointSize = 2;
            deletedPoint.x = shapeToDelete.RootX;
            deletedPoint.y = shapeToDelete.RootY;
        }

        #region IAction Members

        public bool Execute()
        {
            layerManager.GetMainControler().MapPanel.SelectedTransportShape = container;
            layerManager.SelectedTransportQuadtreeItem = container.Reference;
            rectangleList.Clear();

            //old InvalidateRectangle
            rectangleList.Add(container.getDisplayBoundingBox(
                d.x, d.y, pointSize, scale, 1));
            
            indexOfDeletedVertex = container.RemovePoint(shapeToDelete);

            if (indexOfDeletedVertex > -1)
            {
                layerManager.GetMainControler().MapPanel.InvalidateRegion(rectangleList);
                layerManager.GetMainControler().MainForm.changeTool(Tool.Pointer);
            }
            return (indexOfDeletedVertex > -1);
        }

        public void UnExecute()
        {
            layerManager.GetMainControler().MapPanel.SelectedTransportShape = container;
            layerManager.SelectedTransportQuadtreeItem = container.Reference;
            rectangleList.Clear();

            //old InvalidateRectangle
            rectangleList.Add(container.getDisplayBoundingBox(
                d.x, d.y, pointSize, scale, 1));

            container.InsertPointAt(indexOfDeletedVertex, deletedPoint.x, deletedPoint.y, scale);

            //new InvalidateRectangle
            rectangleList.Add(container.getDisplayBoundingBox(
                d.x, d.y, pointSize, scale, 1));

            layerManager.GetMainControler().MapPanel.InvalidateRegion(rectangleList);

        }

        public void Dispose()
        {
           
        }

        #endregion
    }
}
