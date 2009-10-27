using System;
using System.Collections.Generic;
using System.Text;
using GravurGIS.Shapes;
using System.Drawing;
using GravurGIS.Topology;

namespace GravurGIS.Actions
{
    class MoveCommentAction: IAction
    {
        private ShapeInformation selShpInfo;
        private int[] mapPanelDiff = new int[2];
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

        public MoveCommentAction(Point m, Point dragStartPoint,
            int pointSize, double scale, LayerManager layerManager)
        {
            this.mapPanelDiff[0] = (m.X - dragStartPoint.X);
            this.mapPanelDiff[1] = (m.Y - dragStartPoint.Y);
            this.layerManager = layerManager;
            this.d.x = this.layerManager.GetMainControler().MapPanel.DX;
            this.d.y = this.layerManager.GetMainControler().MapPanel.DY;
            this.pointSize = pointSize;
            this.scale = scale;
            this.m = m;
            this.selShpInfo.iShapeInf
                = layerManager.GetMainControler().MapPanel.SelectedTransportShape;
            this.selShpInfo.quadTreePosItemInf
                = layerManager.SelectedTransportQuadtreeItem;
           
            this.dragStartPoint = dragStartPoint;
        }

        #region IAction Members

        public bool Execute()
        {
            IShape shape = this.selShpInfo.iShapeInf;
            SizeF stringSize = shape.StringSize;
            int commentWidth = (int)stringSize.Width + 3;
            int commentHeight = (int)stringSize.Height + 3;

            layerManager.GetMainControler().MapPanel.SelectedTransportShape = shape;
            layerManager.SelectedTransportQuadtreeItem = this.selShpInfo.quadTreePosItemInf;
            rectangleList.Clear();

            //old InvalidateRectangle
            Rectangle invalidateRect = shape.getDisplayBoundingBox(
                d.x, d.y, pointSize, scale, 1);

            //rectangleList.Add(invalidateRect);
            rectangleList.Add(new Rectangle(
                invalidateRect.Right + shape.DrawCommentOffset.X + 1,
                invalidateRect.Bottom + shape.DrawCommentOffset.Y + 1,
                commentWidth,
                commentHeight));

            shape.DrawCommentOffset = new Point(mapPanelDiff[0], mapPanelDiff[1]);

            rectangleList.Add(new Rectangle(
                invalidateRect.Right + shape.DrawCommentOffset.X + 1,
                invalidateRect.Bottom + shape.DrawCommentOffset.Y + 1,
                commentWidth,
                commentHeight));


            layerManager.GetMainControler().MapPanel.movePointHasChanged(m);
            layerManager.GetMainControler().MapPanel.InvalidateRegion(rectangleList);
            layerManager.GetMainControler().MainForm.changeTool(Tool.Pointer);
            return true;
        }

        public void UnExecute()
        {
            IShape shape = this.selShpInfo.iShapeInf;
            SizeF stringSize = shape.StringSize;
            int commentWidth = (int)stringSize.Width + 3;
            int commentHeight = (int)stringSize.Height + 3;

            //arbeite am richtigen selektierten TransportShape:
            layerManager.GetMainControler().MapPanel.SelectedTransportShape = shape;
            layerManager.SelectedTransportQuadtreeItem = this.selShpInfo.quadTreePosItemInf;
            rectangleList.Clear();
            Rectangle invalidateRect = this.selShpInfo.iShapeInf.getDisplayBoundingBox(
                d.x, d.y, pointSize, scale, 2);
           
            rectangleList.Add(invalidateRect); //old InvalidateRectangle

            rectangleList.Add(new Rectangle(
                invalidateRect.Right + shape.DrawCommentOffset.X + 1,
                invalidateRect.Bottom + shape.DrawCommentOffset.Y + 1,
                commentWidth,
                commentHeight));

            this.selShpInfo.iShapeInf.moveToByDifference(
                -1 * mapPanelDiff[0] / scale,
                -1 * mapPanelDiff[1] / scale, false);

            // new InvalidateRectangle
            invalidateRect = shape.getDisplayBoundingBox(
                d.x, d.y, pointSize, scale, 1);
            rectangleList.Add(invalidateRect);

            rectangleList.Add(new Rectangle(
               invalidateRect.Right + shape.DrawCommentOffset.X + 1,
               invalidateRect.Bottom + shape.DrawCommentOffset.Y + 1,
               commentWidth,
               commentHeight));
         
            layerManager.GetMainControler().MapPanel.movePointHasChanged(this.dragStartPoint);
            layerManager.GetMainControler().MapPanel.InvalidateRegion(rectangleList);
        }

        public void Dispose()
        {
           
        }

        #endregion
    }
}
