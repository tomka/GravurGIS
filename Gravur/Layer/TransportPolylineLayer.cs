using System;
using System.Collections.Generic;
using System.Text;
using GravurGIS.Shapes;
using System.Drawing;
using GravurGIS.Topology;
using GravurGIS.Rendering;

namespace GravurGIS.Layers
{
    class TransportPolylineLayer : Layer, ITransportLayer
    {
        private List<IShape> polylines;
        private int lineWidth = 1;
        private int bbWidthMargin = 3;
        private int bbHeightMargin = 3;
        private LayerManager layermanager;
        private int displayCharacterCount = 10;
        private Font commentFont;
        private Config config;

        public delegate void ElementAddedDelagate(IShape newElement);
        public event ElementAddedDelagate ElementAdded;

        public TransportPolylineLayer(LayerManager lm, Config config)
        {
            this._layerType = LayerType.PolylineCanvas;
            this.Visible = this.Changed = true;
            polylines = new List<IShape>();
            this.layermanager = lm;
            this.commentFont = new Font("Arial", 10, FontStyle.Regular);
            this.config = config;
            lm.GetMainControler().SettingsLoaded += new MainControler.SettingsLoadedDelegate(TransportPolylineLayer_SettingsLoaded);
        }

        void TransportPolylineLayer_SettingsLoaded(Config config)
        {
            this.config = config;
        }

        public ShpPolyline addPolyline(double x, double y, double scale)
        {
            ShpPolyline pl = new ShpPolyline(x, y, scale);
            polylines.Add(pl);

            pl.Changed += new IShape.PositionChangedDelegate(polyline_PositionChanged);

            ElementAdded(pl);

            return pl;
        }

        void polyline_PositionChanged(IShape sender, bool combinedMove)
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

        public void removePolyline(IShape polyline)
        {
            polylines.Remove(polyline);
        }

        public string getComment(int index)
        {
            if (index >= 0 && index < this.Count)
                return polylines[index].Commment;
            return "";
        }
        public void clear()
        {
            for (int i = 0; i < polylines.Count; i++)
                polylines[i].Delete();

            polylines.Clear();
            Changed = true;
            this._boundingBox = new WorldBoundingBoxD();
        }


        // Funktioniert, wird aber im Moment nicht benötigt!
        //public double Height()
        //{
        //    int count = polylines.Count;

        //    if (count > 0)
        //    {
        //        if (count == 1) return polylines[0].Height;
        //        else
        //        {
        //            double min = polylines[0].MinY;
        //            double max = min + polylines[0].Height;

        //            for (int i = 1; i < polylines.Count; i++)
        //            {
        //                min = Math.Min(min, polylines[i].MinY);
        //                max = Math.Min(max, polylines[i].MinY + polylines[i].Height);
        //            }

        //            return max - min;
        //        }
        //    }
        //    else
        //        return -1;
        //}

        #region Getters/Setters

        public new string LayerName
        {
            get { return config.ExPLineLayerName; }
            set { config.ExPLineLayerName = value; }
        }

        public double[] getXListOfPolyLine(int index)
        {
            if (index < polylines.Count)
                return polylines[index].getXList();
            else
                return null;
        }
        public double[] getYListOfPolyLine(int index)
        {
            if (index < polylines.Count)
                return polylines[index].getYList();
            else
                return null;
        }
        public int getVerticeCountOfPolyLine(int index)
        {
            if (index < polylines.Count)
                return polylines[index].PointCount;
            else
                return -1;
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
        public int LineWidth
        {
            get { return lineWidth; }
            set { lineWidth = value; }
        }
        public int Count
        {
            get { return polylines.Count; }
        }
        #endregion

        #region ILayer Members

        public override bool Render(RenderProperties rp)
        {
            List<PolyShapeBBInformation> transportPointList = new List<PolyShapeBBInformation>();
            StringBuilder dispString = new StringBuilder();
            Pen pen = new Pen(config.ExPLineLayerLineColor, config.exPLineLayerLineWidth);
			Pen hihglightPen = new Pen(Color.Red, config.exPLineLayerLineWidth);
            hihglightPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            SolidBrush brush = new SolidBrush(config.ExPLineLayerLineColor);
			SolidBrush brushPoint = new SolidBrush(config.ExPLineLayerPointColor);
			SolidBrush hlBrush = new SolidBrush(Color.Red);

            Graphics g = rp.G;
            double scale = rp.Scale;
            double dX = rp.DX;
            double dY = rp.DY;
            int pntWidth = config.ExPLineLayerPointWidth;

            IShape vertex = null;

            layermanager.generatePolylineList(ref transportPointList);
            
            if (transportPointList.Count != 0)
            {
                IShape shape;

                for (int i = transportPointList.Count - 1; i >= 0; i--)
                {
                    shape = transportPointList[i].Shape;
                    if (shape.Visible)
                    {
                        // draw the outline
                        if (shape.IsHighlighted)
                            rp.G.DrawLines(hihglightPen, transportPointList[i].Pointlist);
                        else
                            rp.G.DrawLines(pen, transportPointList[i].Pointlist);

                        for (int j = shape.PointCount - 1; j>=0; j--) 
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

                        if (config.ExPLineLayerDisplayComments)
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

                                SizeF stringSize = rp.G.MeasureString(dispString.ToString(), commentFont);
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

        public new string Comment
        {
            get { return config.ExPLineLayerComment; }
            set { config.ExPLineLayerComment = value; }
        }

        public IShape getShape(int i)
        {
            if (i < polylines.Count) return polylines[i];
            else return null;
        }

        #endregion
    }
}
