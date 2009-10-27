using System;
using System.Collections.Generic;
using System.Text;
using GravurGIS.Shapes;
using GravurGIS.Topology;
using System.Drawing;
using GravurGIS.Rendering;

namespace GravurGIS.Layers
{
    class TransportMultiPointLayer : Layer, ITransportLayer
    {
        private List<IShape> points;
        private int bbWidthMargin  = 3;
        private int bbHeightMargin = 3;
        private LayerManager layerManager;
        private Font CommentFont = new Font("Arial", 10, FontStyle.Regular);
        private Config config;

        public delegate void ElementAddedDelagate(IShape newElement);
        public event ElementAddedDelagate ElementAdded;

        public TransportMultiPointLayer(LayerManager lm, Config config)
        {
            this.Changed = this.Visible = true;

            this._layerType = LayerType.PointCanvas;
            
            points = new List<IShape>();
            this.layerManager = lm;
            this.config = config;
            lm.GetMainControler().SettingsLoaded += new MainControler.SettingsLoadedDelegate(TransportMultiPointLayer_SettingsLoaded);
        }

        void TransportMultiPointLayer_SettingsLoaded(Config config)
        {
            this.config = config;
        }

        public ShpPoint addPoint(double x, double y, double scale)
        {
            ShpPoint pnt = new ShpPoint(x, y, scale);

            if (points.Count == 0)
            {
                _boundingBox.Left = pnt.RootX;
                _boundingBox.Bottom = pnt.RootY;
                _boundingBox.TopRight = _boundingBox.BottomLeft;
            }
            else
            {
                if (pnt.RootX < _boundingBox.Left)
                    _boundingBox.Left = pnt.RootX;
                if (pnt.RootY < _boundingBox.Bottom)
                    _boundingBox.Bottom = pnt.RootY;
                if (pnt.RootX > _boundingBox.Right)
                    _boundingBox.Right = pnt.RootX;
                if (pnt.RootY > _boundingBox.Top)
                    _boundingBox.Top = pnt.RootY;
            }
            
            points.Add(pnt);

            pnt.Changed += new IShape.PositionChangedDelegate(pnt_Changed);
            
            ElementAdded(pnt);
            return pnt;
        }

        void pnt_Changed(IShape sender, bool combinedMove)
        {
            // Update the spatial index of the point
            GravurGIS.Topology.QuadTree.QuadTreePositionItem<IShape> qtItem = sender.Reference;

            if (qtItem != null)
            {
                double firstScale = layerManager.FirstScale;
                qtItem.changePosAndSize(
                    new GravurGIS.Topology.Vector2(sender.CenterX * firstScale, sender.CenterY * firstScale),
                    new GravurGIS.Topology.Vector2(sender.Width * firstScale, sender.Height * firstScale));
                qtItem.reactOnZoom((4.0 * firstScale) / layerManager.Scale);
            }
        }

        public void removePoint(IShape point)
        {
            points.Remove(point);
        }

        public string getComment(int index)
        {
            if (index >= 0 && index < this.Count)
                return points[index].Commment;
            return "";
        }

        public void clear()
        {
            for (int i = 0; i < points.Count; i++)
                points[i].Delete();

            points.Clear();
            Changed = true;
            this._boundingBox = new WorldBoundingBoxD();
        }

        #region Getters/Setters

        public int BbHeightMargin
        {
            get { return bbHeightMargin; }
            set { bbHeightMargin = value; }
        }
        public int BbWidthMargin
        {
            get { return bbWidthMargin; }
            set { bbWidthMargin = value; }
        }
        public int Count
        {
            get { return points.Count; }
        }

        public PointD getPoint(int index)
        {
            return new PointD(points[index].RootX, points[index].RootY);
        }

        #endregion

        #region ILayer Members

        public override bool Render(RenderProperties rp)
        {
            List<ShapeBBInformation> transportRectangleList = new List<ShapeBBInformation>();
            StringBuilder dispString = new StringBuilder();
            SolidBrush brush = new SolidBrush(config.ExPntLayerPointColor);
            SolidBrush highlightBrush = new SolidBrush(Color.Red);
            layerManager.generatePointList(ref transportRectangleList);
            int displayCharacterCount = config.ExPntLayerDisplayCommentMaxLength;

            if (transportRectangleList.Count != 0)
            {
                IShape shape;

                for (int i = 0; i < transportRectangleList.Count; i++)
                {
                    shape = transportRectangleList[i].Shape;

                    if (shape.Visible)
                    {
                        // draw the outline
                        if (shape.IsHighlighted)
                            rp.G.FillEllipse(highlightBrush, transportRectangleList[i].BoundingBox);
                        else
                            rp.G.FillEllipse(brush, transportRectangleList[i].BoundingBox);

                        if (config.ExPntLayerDisplayComments)
                        {
                            dispString.Remove(0, dispString.Length);
                            dispString.Append(shape.Commment);
                            if (dispString.Length > 0)
                            {
                                if (dispString.Length > displayCharacterCount)
                                {
                                    dispString.Remove(displayCharacterCount,
                                        dispString.Length - displayCharacterCount);
                                    dispString.Append("...");
                                }

                                SizeF stringSize = rp.G.MeasureString(dispString.ToString(), CommentFont);
                                shape.StringSize = stringSize;

                                rp.G.DrawString(dispString.ToString(),
                                    CommentFont, brush,
                                    new Rectangle(
                                        transportRectangleList[i].BoundingBox.Right + 2,
                                        transportRectangleList[i].BoundingBox.Bottom + 2,
                                        (int)stringSize.Width,
                                        (int)stringSize.Height));
                            }
                        }
                    }
                }
            }
            return true;
        }

        public override void reset()
        {
            // do nothing special
        }
 
        public override void recalculateData(double absoluteZoom, double scale, double xOff, double yOff)
        {
            // do nothing special
        }

        public new string Comment
        {

            get { return config.ExPntLayerComment; }
            set { config.ExPntLayerComment = value; }
        }

        public new string Description
        {
            get { return config.ExPntLayerDescription; }
            set { config.ExPntLayerDescription = value; }
        }

        public new string LayerName
        {
            get { return config.ExPntLayerName; }
            set { config.ExPntLayerName = value; }
        }

        public IShape getShape(int i)
        {
            if (i < points.Count) return points[i];
            else return null;
        }

        #endregion
    }
}
