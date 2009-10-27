using System;
using System.Collections.Generic;
using System.Text;
using GravurGIS.Topology.QuadTree;
using GravurGIS.Shapes;
using MapTools;

namespace GravurGIS.Actions
{
    class RemoveShapeAction : IAction
    {
        // To temporary delete the shape we need to disable the shape and check for this
        // in quadtree lookup

        private QuadTreePositionItem<IShape> selectedTransportQuadtreeItem;
        
        private LayerManager layerManager;
        public RemoveShapeAction(QuadTreePositionItem<IShape> item,LayerManager layerManager)
        {
            this.selectedTransportQuadtreeItem = item;
            this.layerManager = layerManager;
        }

        #region IAction Members

        public bool Execute()
        {
            if (selectedTransportQuadtreeItem != null)
                selectedTransportQuadtreeItem.Parent.Visible = false;
            layerManager.GetMainControler().MapPanel.Invalidate(); // das ist nicht sauber!
            return true;
        }

        public void UnExecute()
        {
            if (selectedTransportQuadtreeItem != null)
                selectedTransportQuadtreeItem.Parent.Visible = true;
            layerManager.GetMainControler().MapPanel.Invalidate(); // das ist nicht sauber!
        }

        public void Dispose()
        {
            if (selectedTransportQuadtreeItem != null)
            {
                selectedTransportQuadtreeItem.Delete();
                ShapeLib.ShapeType shapeType = selectedTransportQuadtreeItem.Parent.Type;

                if (shapeType == ShapeLib.ShapeType.MultiPoint)
                    layerManager.TransportPointLayer.removePoint(selectedTransportQuadtreeItem.Parent);
                else if (shapeType == ShapeLib.ShapeType.PolyLine)
                    layerManager.TransportPolylineLayer.removePolyline(selectedTransportQuadtreeItem.Parent);
                else if (shapeType == ShapeLib.ShapeType.PolyLine)
                    layerManager.TransportPolygonLayer.removePolygon(selectedTransportQuadtreeItem.Parent);
                
                selectedTransportQuadtreeItem = null;
            }
        }

        #endregion
    }
}
