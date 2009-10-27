using System;
using System.Collections.Generic;
using System.Text;
using GravurGIS.Shapes;
using System.Drawing;
using GravurGIS.Topology;

namespace GravurGIS.Actions
{
    class MoveGeometryAction : IAction
    {
        private IShape shapeToMove;
        private IShape container;

        private PointD oldPosition = new PointD();
        private List<Rectangle> rectangleList = new List<Rectangle>();
        private PointD d = new PointD();
        private int pointSize;
        private double scale;
        private Point m;
        private LayerManager layerManager;
        private Point dragStartPoint;
        /// <summary>
        /// Moves a certain Transport to a certain position
        /// </summary>
        /// <param name="selectedTransportShape"></param>
        /// <param name="unscaledDifference"></param>
        /// <param name="pointSize"></param>
        /// <param name="scale"></param>
        /// <param name="d"></param>
        /// <param name="mapPanel"></param>
        //public MoveTransportShapeAction(Point newM, Point oldM,Point dragStartPoint,int pointSize,double scale,MapPanel mapPanel)
        //{
        //    this.mapPanelDiff[0] = (newM.X - dragStartPoint.X);
        //    this.mapPanelDiff[1] = (newM.Y - dragStartPoint.Y);
        //    this.mapPanel = mapPanel;
        //    this.d.x = mapPanel.DX;
        //    this.d.y = mapPanel.DY;
        //    this.pointSize = pointSize;
        //    this.scale = scale;
        //    this.newM = newM;
        //    this.oldM = oldM;
        //    this.this.selshpInfo.iShapeInf = mapPanel.SelectedTransportShape;
        //}

        public MoveGeometryAction(IShape shapeToMove, IShape container , Point m, Point dragStartPoint, int pointSize, double scale, LayerManager layerManager)
        {
            this.oldPosition.x = shapeToMove.RootX;
            this.oldPosition.y = shapeToMove.RootY;
            this.layerManager = layerManager;
            this.d.x = this.layerManager.GetMainControler().MapPanel.DX;
            this.d.y = this.layerManager.GetMainControler().MapPanel.DY;
            this.pointSize = pointSize;
            this.scale = scale;
            this.m = m;
            this.shapeToMove = shapeToMove;
            this.container = container;
            this.dragStartPoint = dragStartPoint;
        }

        #region IAction Members

        public bool Execute()
        {
            SizeF stringSize = shapeToMove.StringSize;
            int commentWidth  = (int)stringSize.Width + 3;
            int commentHeight = (int)stringSize.Height + 3;

            layerManager.GetMainControler().MapPanel.SelectedTransportShape = container;
            layerManager.SelectedTransportQuadtreeItem = container.Reference;
            rectangleList.Clear();

            //old InvalidateRectangle
            rectangleList.Add(container.getDisplayBoundingBox(
                d.x, d.y, pointSize, scale, 2));

            layerManager.GetMainControler().MapPanel.InvalidateRegion(rectangleList);
            layerManager.GetMainControler().MapPanel.Update();

            shapeToMove.moveTo(
                (d.x + m.X) / scale,
                (m.Y - d.y) / scale,
                false, false);


             // new InvalidateRectangle
             rectangleList.Add(container.getDisplayBoundingBox(
                  d.x,
                  d.y,
                  pointSize,
                  scale, 2));

             layerManager.GetMainControler().MapPanel.movePointHasChanged(m);
             layerManager.GetMainControler().MapPanel.InvalidateRegion(rectangleList);
             layerManager.GetMainControler().MainForm.changeTool(Tool.Pointer);
             return true;
        }

        public void UnExecute()
        {
            SizeF stringSize = shapeToMove.StringSize;
            int commentWidth = (int)stringSize.Width + 3;
            int commentHeight = (int)stringSize.Height + 3;

            //arbeite am richtigen selektierten TransportShape:
            layerManager.GetMainControler().MapPanel.SelectedTransportShape = container;
            layerManager.SelectedTransportQuadtreeItem = container.Reference;
            rectangleList.Clear();
            Rectangle invalidateRect = container.getDisplayBoundingBox(
                d.x, d.y, pointSize, scale, 2);
           
            rectangleList.Add(invalidateRect); //old InvalidateRectangle

            shapeToMove.moveTo(
                oldPosition.x,
                oldPosition.y,
                false, false);

            // new InvalidateRectangle
            invalidateRect = container.getDisplayBoundingBox(
                d.x, d.y, pointSize, scale, 2);
            rectangleList.Add(invalidateRect);
         
            layerManager.GetMainControler().MapPanel.movePointHasChanged(this.dragStartPoint);
            layerManager.GetMainControler().MapPanel.InvalidateRegion(rectangleList);
        }

        public void Dispose()
        {
           
        }

        #endregion
    }
}
