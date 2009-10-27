using System;
using System.Collections.Generic;
using System.Text;
using GravurGIS.Shapes;
using System.Drawing;
using GravurGIS.Rendering;

namespace GravurGIS.Layers
{
    class TransportPolygonLayer : Layer, ITransportLayer
    {
        private List<IShape> polygons;
        private int bbWidthMargin = 3;
        private int bbHeightMargin = 3;
        private LayerManager layermanager;
        private int displayCharacterCount = 10;
        private Font commentFont = new Font("Arial", 10, FontStyle.Regular);
        private Config config;

        public delegate void ElementAddedDelegate(IShape newElement);
        public event ElementAddedDelegate ElementAdded;

        public TransportPolygonLayer(LayerManager lm, Config config)
        {
            this._layerType = LayerType.PolygonCanvas;
            this.Changed = this.Visible = true;

            polygons = new List<IShape>();
            this.layermanager = lm;
            this.config = config;

            lm.GetMainControler().SettingsLoaded +=new MainControler.SettingsLoadedDelegate(TransportPolygonLayer_SettingsLoaded);
        }

        void TransportPolygonLayer_SettingsLoaded(Config config)
        {
            this.config = config;
        }

        public ShpPolygon addPolygon(double x, double y, double scale)
        {
            ShpPolygon pl = new ShpPolygon(x, y, scale);
            polygons.Add(pl);

            pl.Changed += new IShape.PositionChangedDelegate(polygon_PositionChanged);

            ElementAdded(pl);

            return pl;
        }

        void polygon_PositionChanged(IShape sender, bool combinedMove)
        {
            // Update the spatial index of the polygon
            GravurGIS.Topology.QuadTree.QuadTreePositionItem<IShape> qtItem = sender.Reference;

            if (qtItem != null)
            {
                double firstScale = layermanager.FirstScale;
                qtItem.changePosAndSize(
                    new GravurGIS.Topology.Vector2(sender.CenterX * firstScale, sender.CenterY * firstScale),
                    new GravurGIS.Topology.Vector2(sender.Width * firstScale, sender.Height * firstScale));
                qtItem.reactOnZoom((4.0 * firstScale) / layermanager.Scale);
            }
        }

        public void removePolygon(IShape polyline)
        {
            polygons.Remove(polyline);
        }

        public double[] getXListOfPolygon(int index)
        {
            if (index < polygons.Count)
                return polygons[index].getXList();
            else
                return null;
        }
        public double[] getYListOfPolygon(int index)
        {
            if (index < polygons.Count)
                return polygons[index].getYList();
            else
                return null;
        }
        public int getVerticeCountOfPolygon(int index)
        {
            if (index < polygons.Count)
                return polygons[index].PointCount;
            else
                return -1;
        }

        public string getComment(int index)
        {
            if (index >= 0 && index < this.Count)
                return polygons[index].Commment;
            return "";
        }

        public void clear()
        {
            for (int i = 0; i < polygons.Count; i++)
                polygons[i].Delete();

            polygons.Clear();
            Changed = true;
            this._boundingBox = new GravurGIS.Topology.WorldBoundingBoxD();
        }

        #region Getters/Setters
        
        public new string LayerName
        {
            get { return config.ExPGonLayerName; }
            set { config.ExPGonLayerName = value; }
        }

        public int Count
        {
            get { return polygons.Count; }
        }

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
        #endregion

        #region ILayer Members

        public bool ShowComments
        {
            get { return config.ExPGonLayerDisplayComments; }
            set { config.ExPGonLayerDisplayComments = value; }
        }

        public override bool Render(RenderProperties rp)
        {
            List<PolyShapeBBInformation> transportPointList = new List<PolyShapeBBInformation>();
            StringBuilder dispString = new StringBuilder();
            Pen pen = new Pen(config.ExPGonLayerLineColor, config.exPGonLayerLineWidth);
			Pen hihglightPen = new Pen(Color.Red, config.exPGonLayerLineWidth);
            hihglightPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            Brush brush = new SolidBrush(config.ExPGonLayerLineColor);
            Brush fillBrush = new SolidBrush(config.ExPGonLayerFillColor);
            SolidBrush hlBrush = new SolidBrush(Color.Red);
			SolidBrush brushPoint = new SolidBrush(config.ExPGonLayerPointColor);

            Graphics g = rp.G;
            double dX = rp.DX;
            double dY = rp.DY;
            double scale = rp.Scale;
            int pntWidth = config.ExPGonLayerPointWidth;
            IShape vertex = null;

            layermanager.generatePolygonList(ref transportPointList);

            if (transportPointList.Count != 0)
            {
                IShape shape;

                for (int i = transportPointList.Count - 1; i >= 0; i--)
                {
                    shape = transportPointList[i].Shape;
                    if (shape.Visible)
                    {
                        // brush the fill
                        if (config.ExPGonLayerFill)
                            g.FillPolygon(fillBrush, transportPointList[i].Pointlist);


						// draw the outline
						if (shape.IsHighlighted)
							g.DrawLines(hihglightPen, transportPointList[i].Pointlist);
						else
							g.DrawLines(pen, transportPointList[i].Pointlist);

                        //draw highlighted vertices
                        for (int j = shape.PointCount - 1; j >= 0; j--)
                        {
                            vertex = shape.getElement(j);
							// draw highlighted vertices
							if (vertex.IsHighlighted)
								g.FillEllipse(hlBrush, vertex.getDisplayBoundingBox(dX, dY,
									pntWidth, scale, 1));
							else
							{
								// draw the vertices
								if (pntWidth > 0)
									g.FillEllipse(brushPoint, vertex.getDisplayBoundingBox(dX, dY,
										pntWidth, scale, 0));
							}
                        }

                        if (config.ExPGonLayerDisplayComments)
                        {
                            dispString.Remove(0, dispString.Length);
                            dispString.Append(shape.Commment);
                            if (dispString.Length > 0)
                            {
                                if (dispString.Length > displayCharacterCount)
                                {
                                    dispString.Remove(displayCharacterCount,
                                        dispString.Length - this.displayCharacterCount);
                                    dispString.Append("...");
                                }

                                SizeF stringSize = g.MeasureString(dispString.ToString(), commentFont);
                                shape.StringSize = stringSize;

                                g.DrawString(dispString.ToString(),
                                    this.commentFont, brush,
                                    new Rectangle(
                                        transportPointList[i].BoundingBox.Right + 2,
                                        transportPointList[i].BoundingBox.Bottom + 2,
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

        public new String Comment
        {
            get { return config.ExPGonLayerComment; }
            set { config.ExPGonLayerComment = value; }
        }

        public IShape getShape(int i)
        {
            if (i < polygons.Count) return polygons[i];
            else return null;
        }

        #endregion
    }
}
