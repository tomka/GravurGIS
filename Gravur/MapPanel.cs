//#define DRAW_BOUNDINGBOXES
//#define DRAW_QUADTREE
//#define DRAW_GRID
//#define DRAW_INVALIDATEREC

using System;
//using System.Drawing; 
using System.Collections.Generic;
using System.Windows.Forms;
using GravurGIS.Shapes;
using System.Threading;
using GravurGIS.Topology;
using GravurGIS.Topology.QuadTree;
using GravurGIS.GUI.Menu;
using GravurGIS.Layers;
using GravurGIS.Actions;
using GravurGIS.GUI;
using GravurGIS.Styles;
using GravurGIS.Rendering.Rendering2D;
using System.Drawing;
using GravurGIS.Rendering.Gdi;
using GravurGIS.Rendering;

namespace GravurGIS
{
    public enum Tool : byte { Pointer, ZoomIn, ZoomOut, Move,
        DrawPoint, DrawPolyline, DrawPolygon, TransportShapeMove,
        MoveComment, MoveGeometry, Identify, None
    }

    public class MapPanel : Panel
    {

        private MainControler mainControler;
        private MainForm mainForm;
        private int margin = 2;
        private bool mDragging;

        private List<QuadTreePositionItem<IShape>> itemList;
        private IShape selectedTransportShape;
        private bool bEditingStarted = false;
        private int     zoomWidth = 0;
        private int     zoomHeigth = 0;
        private System.Drawing.Rectangle zoomRec = new System.Drawing.Rectangle(0, 0, 0, 0);
        private System.Drawing.Rectangle IShapeCommentRec = new System.Drawing.Rectangle(0, 0, 0, 0);
        private System.Drawing.Point oldMovePoint = new System.Drawing.Point(0, 0);
        private double  absoluteZoom = 1.0d;
        private Tool    selectedTool;
        private bool    mZoom = false;
        private System.Drawing.Point dragStartPoint;
        private PointD d = new PointD();
        private int     dragWidth = 0;
        private int     dragHeight = 0;
        private bool    screenChanged = true;
        private int     shownLayer = 0;
        private GravurGIS.GUI.Controls.ToolTip toolTip;
        private IVectorRenderer2D vectorRenderer;
        private NPack.Interfaces.IAffineTransformMatrix<NPack.DoubleComponent> northPointingMatrix;
        private FastBitmap backBuffer = null;
        private float dpiX = 192;
        private float dpiY = 192;
        private int mousePressedTime = 0;
        private IShape subSelection = null;
        private bool contextMenuisActive = false;
        private Font font = new Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
        private int m_TextHeight = 10;
        private bool mHighlight = false;
        private bool isWellDefined = true; // true (=1) if there is a one to one relation between parts and shapes
		private bool displayTrackingStatus = false;
		private string _trackingStatus = String.Empty;
		/// <summary>
		/// A stopwatch for diagnostics likea meassuring render time
		/// </summary>
		private System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
        private Config config;

        #if DRAW_INVALIDATEREC
            private List<Rectangle> rectangleList = new List<Rectangle>();
        #endif

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        Random random = new Random();
        System.Drawing.Point stressTestMovementDelta;

        private System.Drawing.Rectangle drawingArea;
  
        // Menus
        private ShapeEditMenu shapeEditMenu;

        public delegate void SelectToolDelegate(Tool tool);
        public event SelectToolDelegate SelectedToolChanged;

        public delegate void EditModeInterruptedDelegate(bool interrupted, Tool interruptedTool);
        public event EditModeInterruptedDelegate EditModeInterrupted;

        public delegate void RequestChangeDelegate();
        public event RequestChangeDelegate RequestChange;



        public MapPanel(MainControler mc, Config config, MainForm mainForm, int width, int height, int margin)
        {
            this.config = config;
            this.mainControler = mc;
            this.Visible = false;
            this.mainForm = mainForm;
            this.Width   = width - 2 * margin;
            this.Height  = height - 2 * margin;
            this.Top = margin;
            this.Left = margin;
            this.margin = margin;
            
            this.MouseDown += new MouseEventHandler(PanelMouseDown);
            this.MouseUp += new MouseEventHandler(PanelMouseUp);
            this.MouseMove += new MouseEventHandler(PanelMouseMove);

            dragStartPoint = new System.Drawing.Point(0, 0);
            itemList = new List<QuadTreePositionItem<IShape>>();

            drawingArea = new System.Drawing.Rectangle(1, 1, this.Width - 2, this.Height - 2);
            vectorRenderer = new GdiVectorRenderer();

            //backBuffer = new FastBitmap(this.ClientRectangle.Width,
             //   this.ClientRectangle.Height);

            // Menus
            shapeEditMenu = new ShapeEditMenu(mainForm);
            //geometryEditMenu = new EditGeometryMenu(mainForm);

            LayerManager lm = mainControler.LayerManager;

            lm.FirstLayerAdded += new LayerManager.LayerAddedDelegate(LayerManager_FirstLayerAdded);
            lm.LayerAdded += new LayerManager.LayerAddedDelegate(layerAdded);
            lm.ScaleChanged += new LayerManager.ScaleChangedDelegate(LayerManager_ScaleChanged);
            mainControler.SettingsLoaded += new MainControler.SettingsLoadedDelegate(mainControler_SettingsLoaded);
            lm.TransportPointLayer.ElementAdded += new TransportMultiPointLayer.ElementAddedDelagate(TransportLayer_ElementAdded);
            lm.TransportPolygonLayer.ElementAdded += new TransportPolygonLayer.ElementAddedDelegate(TransportLayer_ElementAdded);
            lm.TransportPolylineLayer.ElementAdded += new TransportPolylineLayer.ElementAddedDelagate(TransportLayer_ElementAdded);
			lm.ShapeRemoved += new LayerManager.ChangedDelegate(Model_ShapeRemoved);


            this.BackColor = System.Drawing.Color.White;

            toolTip = new GravurGIS.GUI.Controls.ToolTip();
            toolTip.Visible = false;

            this.Controls.Add(toolTip);

            northPointingMatrix = new Matrix2D();

			mainControler.WaypointAdded += new MainControler.WaypointAddedDelegate(mainControler_WaypointAdded);
			mainControler.TrackingStarted += new MainControler.WaypointAddedDelegate(mainControler_TrackingStarted);
			mainControler.TrackingStopped += new MainControler.NoParamDelegate(mainControler_TrackingStopped);
        }

		void Model_ShapeRemoved()
		{
			selectedTransportShape = null;
			this.Refresh();
		}

		void mainControler_TrackingStopped()
		{
			displayTrackingStatus = false;
		}

		void mainControler_TrackingStarted()
		{
			if (config.ShowTrackingStatus)
			{
				displayTrackingStatus = true;
			}
		}

		/// <summary>
		/// Changes the way point status to the current point
		/// </summary>
		void mainControler_WaypointAdded()
		{
			// change way point status
			_trackingStatus = String.Format("Tracking: an, Bisherige Punkte: {0}", mainControler.CurrentlyCollectedWaypoints);
		}

        void TransportLayer_ElementAdded(IShape newElement)
        {
            if (selectedTool == Tool.DrawPoint
                || selectedTool == Tool.DrawPolygon
                || selectedTool == Tool.DrawPolyline)
            {
                OnSelectShape(newElement);
            }
        }

        void LayerManager_FirstLayerAdded(Layer newShapeObject)
        {
            Visible = true;
            mainControler.HasActiveDisplay = true;
        }

        void mainControler_SettingsLoaded(Config config)
        {
            this.config = config;
        }

        void LayerManager_ScaleChanged(double scale, double absoluteZoom)
        {
            this.absoluteZoom = absoluteZoom;
        }

        private static int MakeCOLORREF(byte r, byte g, byte b)
        {
            return (int)(((uint)r) | (((uint)g) << 8) | (((uint)b) << 16));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
			_stopwatch.Reset();
			_stopwatch.Start();
			
            SizeF textSize;
            String text = String.Empty;

            System.Drawing.Graphics gx
                = vectorRenderer.Graphics
                = e.Graphics; // Graphics.FromImage(backBuffer); //e.Graphics;

            gx.Clear(Color.White);

            this.dpiX = gx.DpiX;
            this.dpiY = gx.DpiY;

            double scale = mainControler.LayerManager.Scale;
            int layerCount = mainControler.LayerManager.LayerCount;
            LayerManager lm = mainControler.LayerManager;

            // TransportLayer
            // TODO: [9] Ist TransportShapes malen mit QuadTree schneller oder wenn man alles malt? (ab wann ist das wichtig?)
            lm.generateCurrentVisibleIShapes(d, drawingArea.Width, drawingArea.Height, absoluteZoom);

            // now draw the image previously created in the given order
            // ! We assume that there are no gaps (e.g. "1, 2, 4") in the mapping-list
            Layer layer;
            RenderProperties rp = new RenderProperties(gx, e.ClipRectangle, d.x, d.y,
                            absoluteZoom, screenChanged, this.drawingArea, scale, mHighlight);

            for (int i = layerCount - 1; i >= 0; i--)
            {
                layer = lm.getLayerFromMapping(i);
                if (layer.Visible)
                {
                    if (!(layer.Render(rp)))
                    {
                        this.Invalidate();
                        return;
                    }

                }
            }

            //IntPtr hBackBuffert = MapPanelBindings.GetRotatedBitmapNT(backBuffer.HBitmap, 0.0f, MakeCOLORREF(255, 255, 255));

            //Image newBackImage = Image.FromHbitmap(hBackBuffert);
            //gx = Graphics.FromImage(newBackImage);

            //rotateImage(backBuffer, 0);

            if (screenChanged)
            {
                mZoom = false;
                screenChanged = false;
            }

            StringFormat format = new StringFormat(StringFormatFlags.NoWrap);
            StylePen blackPen = new StylePen(StyleColor.Black, 1.0f);
            SolidStyleBrush blackBrush = new SolidStyleBrush(StyleColor.Black);

            #region Transportlayer

            lm.TransportPolygonLayer.Render(rp);
            lm.TransportPolylineLayer.Render(rp);
            lm.TransportPointLayer.Render(rp);

            #endregion

            #region GPS-Layer
            
            if (config.UseGPS)
                lm.GPSLayer.Render(rp);

            #endregion
            
            #region Längenanzeige
            if (config.ShowDistanceLine)
            {
                StylePen borderPen = new StylePen(StyleColor.White, 1.0f);

                // get the scaling factor for other dimensions
                int multiplikator = (int)(dpiX / 96.0);

                // get the amount of pixels representing one centimeter
                int width = Convert.ToInt32(dpiX / 2.54); ;

                blackPen.Width *= multiplikator;

                int left = drawingArea.X + 7*multiplikator;
                int right = left + width;
                int top = drawingArea.Bottom - 15 * multiplikator;
                int bottom = top + 7 * multiplikator;

                double dM = width / mainControler.LayerManager.Scale;
                
                if (dM > 1000) // display as kilometers
                    text = String.Format("{0}km", (dM / 1000).ToString("#0.0"));
                else if (dM > 1) // display as meters
                    text = String.Format("{0}m", Convert.ToInt32(dM).ToString());
                else // display as centimeters
                    text = String.Format("{0}cm", Convert.ToInt32(dM * 10).ToString());

                textSize = gx.MeasureString(text, font);

                int x = left + (int)((right - left - textSize.Width) / 2F);
                int y = top - 4 * multiplikator;

                // a white background for the text
                vectorRenderer.FillRectangle(new SolidStyleBrush(StyleColor.White),
                    new Rectangle(x - 1, y - 1, (int)textSize.Width + 2, (int)textSize.Height + 2));

                vectorRenderer.DrawLine(blackPen, left, top, left, bottom);
                vectorRenderer.DrawLine(blackPen, left, bottom, right, bottom);
                vectorRenderer.DrawLine(blackPen, right, bottom, right, top);
                
                // the white outline
                vectorRenderer.DrawLine(borderPen, left - 1, top, left - 1, bottom);
                vectorRenderer.DrawLine(borderPen, left + 1, top, left + 1, bottom - 1);
                vectorRenderer.DrawLine(borderPen, left - 1, top - 1, left + 1, top - 1);

                vectorRenderer.DrawLine(borderPen, left - 1, bottom + 1, right + 1, bottom + 1);
                vectorRenderer.DrawLine(borderPen, left + 1, bottom - 1, right - 1, bottom - 1);

                vectorRenderer.DrawLine(borderPen, right - 1, top, right - 1, bottom - 1);
                vectorRenderer.DrawLine(borderPen, right + 1, top, right + 1, bottom);
                vectorRenderer.DrawLine(borderPen, right - 1, top - 1, right + 1, top - 1);
                
                // the text
                vectorRenderer.DrawString(text,
                    font,
                    blackBrush, x, y, format);
            }
            #endregion

            #region Nordpfeil

            if (config.ShowNorthArrow)
            {
                // linke Seite (weiß)
                vectorRenderer.FillPolygon(new SolidStyleBrush(StyleColor.White), new Point[] {
                new Point(drawingArea.Right - 20, drawingArea.Bottom -27),
                new Point(drawingArea.Right - 25, drawingArea.Bottom -5),
                new Point(drawingArea.Right - 20, drawingArea.Bottom -8)});

                vectorRenderer.DrawLines(new StylePen(StyleColor.Black, 1.0f), new Point[] {
                new Point(drawingArea.Right - 20, drawingArea.Bottom -27),
                new Point(drawingArea.Right - 25, drawingArea.Bottom -5),
                new Point(drawingArea.Right - 20, drawingArea.Bottom -8)});

                // rechte Seite (schwarz)
                vectorRenderer.FillPolygon(new SolidStyleBrush(StyleColor.Black), new Point[] {
                new Point(drawingArea.Right - 20, drawingArea.Bottom -27),
                new Point(drawingArea.Right - 20, drawingArea.Bottom -8),
                new Point(drawingArea.Right - 14, drawingArea.Bottom -4),
                new Point(drawingArea.Right - 20, drawingArea.Bottom -27)});
            }

            #endregion

            #region Draw backbuffer (Backbuffer is currently disabled!)
                //e.Graphics.DrawImage(backBuffer, 0, 0);
            #endregion

            #region Zoom rectangle
            if (mZoom)
                e.Graphics.DrawRectangle(new Pen(Color.Red, 1.0f), zoomRec);
            #endregion

            #region SelectedItem / Tracking Info

			text = String.Empty;
			if (displayTrackingStatus)
			{
				text = _trackingStatus;
			} else if (selectedTransportShape != null)
            {
                text = String.Empty;

                if (selectedTransportShape as ShpPolyline != null)
                {
                    text = String.Format("Länge: {0}m", ((ShpPolyline)selectedTransportShape).Length.ToString("#0.00"));
                }
                else if (selectedTransportShape as ShpPolygon != null)
                {
                    ShpPolygon pg = (ShpPolygon)selectedTransportShape;

                    text = String.Format("Umfang: {0}m Fläche: {1} m²", pg.Perimeter.ToString("#0.00"), pg.Area.ToString("#0.00"));
                }
			}

			if (!String.IsNullOrEmpty(text))
			{
				textSize = gx.MeasureString(text, font);

				m_TextHeight = (int)(textSize.Height + 4);

				vectorRenderer.FillRectangle(new SolidStyleBrush(StyleColor.White), drawingArea.X, drawingArea.Y,
					drawingArea.Width, m_TextHeight);


				vectorRenderer.DrawLine(new StylePen(StyleColor.Blue, 1.0f), drawingArea.X, drawingArea.Y + m_TextHeight,
					drawingArea.Width, drawingArea.Y + m_TextHeight);

				vectorRenderer.DrawString(text, font, blackBrush, drawingArea.X + 1, drawingArea.Y + 2, format);
			}

            #endregion

            #region Meta-Define Drawings
#if DRAW_INVALIDATEREC
            foreach (Rectangle r in this.rectangleList)
            {   GKCoord gk = new GKCoord();
                gk.r_value = r.X;
                gk.h_value = r.Y;
                PointD p = CoordCalculator.calculateDisplayCoords(gk, this.DX, this.DY, mainControler.LayerManager.Scale);
                gx.DrawRectangle(new Pen(Color.Pink), new Rectangle((int)(p.x),(int)(p.y),r.Width,r.Height));
            }
#endif


#if DRAW_GRID
                // Grid drawing
                for (int i = (int)(mainControler.LayerManager.ShapeArray[0].Width * mainControler.LayerManager.Scale); i >= 0;
                    i -= mainControler.LayerManager.ShapeArray[0].PartGrid.Cell_width)
                {

                    gx.DrawLine(new Pen(Color.Red), (int)d[0] + i, (int)d[1] + margin, (int)d[0] + i,
                         (int)(d[1] + mainControler.LayerManager.ShapeArray[0].Height * mainControler.LayerManager.Scale) - margin);
                }
                for (int i = (int)(mainControler.LayerManager.ShapeArray[0].Height * mainControler.LayerManager.Scale); i >= 0;
                    i -= mainControler.LayerManager.ShapeArray[0].PartGrid.Cell_height)
                {

                    gx.DrawLine(new Pen(Color.Red), (int)d[0] + margin, (int)d[1]  + i,
                        (int)(d[0] + mainControler.LayerManager.ShapeArray[0].Width * mainControler.LayerManager.Scale) - margin,
                        (int)d[1] + i);
                }
#endif

#if DRAW_QUADTREE
            mainControler.LayerManager.draw_QuadTree(gx, d);
#endif

#if DRAW_BOUNDINGBOXES
                            // Draw Boundingboxes
                           List<Rectangle> transportRectangleList = new List<Rectangle>();
                            // :: Points
                            mainControler.LayerManager.generateBBList(ref transportRectangleList, absoluteZoom);

                            if (transportRectangleList.Count != 0)
                            {
                                Pen temporaryPen = new Pen(Color.Green);
                                for (int i = transportRectangleList.Count - 1; i >= 0; i--)
                                    gx.DrawRectangle(temporaryPen, transportRectangleList[i]);
                            }
#endif

            #endregion

            //Bounding Box of selected IShape
            //if (selectedTransportShape != null)
            //    vectorRenderer.DrawRectangle(new Pen(Color.Green), selectedTransportShape.getDisplayBoundingBox(
            //        d.x, d.y, config.ExPntLayerPointWidth, scale, 1));

            //Boundary rectangle 
            Rectangle rc = this.ClientRectangle;
            rc.Width--;
            rc.Height--;
            //Draw boundary 
            e.Graphics.DrawRectangle(new Pen(Color.Blue), rc);
            gx.Dispose();
            
            _stopwatch.Stop();
            
            // if the render time is higher than the gps update interval
            // we have to set the update interval higher
            if (_stopwatch.ElapsedMilliseconds >= config.GPSUpdateInterval) {
				// base update interval is render time * 1.2
				double newInterval = _stopwatch.ElapsedMilliseconds * 1.2;
				// since we want full 1000th as interval we round up to the next 1000
				try {
					config.GPSUpdateInterval = Convert.ToUInt32( newInterval + ( 1000 - newInterval % 1000 ) );
				} catch { config.GPSUpdateInterval = 5000; }
				finally { mainControler.OnSettingsChanged(); }
			}
        }

        private void rotateImage(FastBitmap bitmap, int angleInDegrees)
        {
            if (angleInDegrees != 0)
            {
                double theta = angleInDegrees * (Math.PI / 180);

                int width = bitmap.Width;
                int height = bitmap.Height;
                int newY, newX;

                //determine centre/pivot point ofsource
                //int centreX = Convert.ToInt32(bitmap.Width / 2.0);
                //int centreY = Convert.ToInt32(bitmap.Height / 2.0);

                //since image is rotated we need to allocate a rectangle
                //which can hold the image in any orientation
                //if (width > height) height = width;
                //else
                //    width = height;

                bitmap.LockPixels();

                byte[] pixels = bitmap.GetAllPixels();
                byte[] newpixels = new byte[bitmap.Width * bitmap.Height * 3];

                // make the baground white
                for (int i = 0; i < newpixels.Length; i++)
                {
                    newpixels[i] = 255;
                }

                float cosine = (float)Math.Cos(theta);
                float sine = (float)Math.Sin(theta);

                for (int x = 0; x < width; ++x)
                    for (int y = 0; y < height; ++y)
                    {
                        newY = (int)((x) * sine + (y) * cosine) * 3;
                        newX = (int)((x) * cosine - (y) * sine) * 3;

                        newX = newX + (newY) * width;
                        newY = (x + y * width) * 3;

                        if (newX >= 0 && newX < newpixels.Length)
                        {
                            newpixels[newX] = pixels[newY];
                            newpixels[newX + 1] = pixels[newY + 1];
                            newpixels[newX + 2] = pixels[newY + 2];
                        }
                    }


                bitmap.SetAllPixels(newpixels);
                bitmap.UnlockPixels();
            }
        }

        public void layerAdded(Layer newLayer)
        {
            this.Invalidate();
            this.Update();
            ScreenChanged = true;
            
            RequestChangeDelegate aDelegate = new RequestChangeDelegate(newLayer.Refresh);
            this.RequestChange += aDelegate;

           
        }

        #region ZoomFunctions
        public void reset()
        {
            LayerManager layerManager = mainControler.LayerManager;

            if (layerManager.LayerCount > 0)
            {
                PointD newD = new PointD();
                DRect worldBox =layerManager.getWorldBBox(); 
                double zoomFactor = calculateZoomFactor(worldBox.Width * layerManager.Scale, worldBox.Height * layerManager.Scale);
                double newAbsoluteZoom = absoluteZoom * zoomFactor;

                newD.x = worldBox.Left * layerManager.FirstScale * newAbsoluteZoom;
                newD.y= worldBox.Bottom * layerManager.FirstScale * newAbsoluteZoom;

                ZoomAction reset = 
                    new ZoomAction(absoluteZoom, d, newD, newAbsoluteZoom,
                        new PointD(drawingArea.Width * 0.5, drawingArea.Height * 0.5), 
                        layerManager.getMaxAbsoluteZoom(),
                        mainControler);
               
                if (mainControler.PerformAction(reset))
                {
                    mainControler.LayerManager.resetLayers();
                    mainForm.setStatusBarText("");
                    screenChanged = true;
                    this.Invalidate();

                }
                else
                {
                    mainForm.setStatus_Timed("Konnte keinen Reset durchführen!", 1000);
                }
            }
        }


        public void onLayerZoom(int shownLayer)
        {
            this.shownLayer = shownLayer;
            Layer layer = mainControler.LayerManager.getLayerFromMapping(shownLayer);
            double scale = mainControler.LayerManager.Scale;
            double zoomFactor = calculateZoomFactor(layer.Width * scale, layer.Height * scale);
            PointD newD = new PointD();
            newD.x = layer.BoundingBox.Left * mainControler.LayerManager.FirstScale * absoluteZoom * zoomFactor;
            newD.y = layer.BoundingBox.Top * mainControler.LayerManager.FirstScale * absoluteZoom * zoomFactor;

            ZoomAction layerZoomAction = new ZoomAction(
                absoluteZoom, d, newD, 
                absoluteZoom * zoomFactor,
                new PointD(drawingArea.Width / 2, drawingArea.Height / 2),
                mainControler.LayerManager.getMaxAbsoluteZoom(),
                mainControler);
            if (mainControler.PerformAction(layerZoomAction))
            {
                mainControler.LayerManager.resetLayers(); // -> prepareBitmap = true
                mainForm.setStatusBarText("");
                screenChanged = true;
                this.Invalidate();
            }
            else
            {
                mainForm.setStatus_Timed("Konnte keinen Reset durchführen!", 1000);
            }
        }


        /// <summary>
        /// calculates zoom factor 
        /// </summary>
        /// <param name="width">width of zoom rectangle</param>
        /// <param name="height">height of zoom rectangle</param>
        /// <returns></returns>
        public double calculateZoomFactor(double width, double height)
        {
            if (width > height) return ((double)drawingArea.Width) / width;
            else return ((double)drawingArea.Height) / height;
        }
        
        
        #endregion

        #region Drag-n-Drop events for Panel

        private void PanelMouseDown(object sender, MouseEventArgs m)
        {
            contextMenuisActive = false;

            if (selectedTool == Tool.Move) mDragging = true;
            else if (selectedTool == Tool.ZoomIn) mZoom = true;
            else if (selectedTool == Tool.TransportShapeMove)
            {
                oldMovePoint.X = Convert.ToInt32(d.x + selectedTransportShape.CenterX * mainControler.LayerManager.Scale);
                oldMovePoint.Y = Convert.ToInt32(d.y + selectedTransportShape.CenterY * mainControler.LayerManager.Scale);
            }
            else if (selectedTool == Tool.MoveGeometry && subSelection != null)
            {
                oldMovePoint.X = Convert.ToInt32(d.x + subSelection.CenterX * mainControler.LayerManager.Scale);
                oldMovePoint.Y = Convert.ToInt32(d.y + subSelection.CenterY * mainControler.LayerManager.Scale);
            }
            else if (selectedTool == Tool.MoveComment)
            {
                oldMovePoint.X = selectedTransportShape.DrawCommentOffset.X;
                oldMovePoint.Y = selectedTransportShape.DrawCommentOffset.Y;
            }
            else if (selectedTool == Tool.Pointer)
            {
                double invScale = 1 / mainControler.LayerManager.Scale;
                PointD currentPosition = new PointD((m.X + d.x) / absoluteZoom, (m.Y - d.y) / absoluteZoom);

                if (subSelection != null && selectedTransportShape != null)
                {
                    IShape tempSelection = selectedTransportShape.NearestPointTo(
                        new PointD((m.X + d.x) * invScale, (m.Y - d.y) * invScale),
                        config.PxMaxSelectDistance * invScale);

                    itemList.Add(selectedTransportShape.Reference);

                    if (tempSelection != null && tempSelection.Equals(subSelection))
                    {
                        mousePressedTime = Environment.TickCount;
                        return;
                    }

                    subSelection.IsHighlighted = false;
                    subSelection = null;

                    // TODO: invalidate old point
                }

                bool selectNewItem = true;

                if (itemList.Count > 0 && !(itemList[0].Parent is ShpPoint) ) // e. g. we have something selected
                {
                    subSelection = itemList[0].Parent.NearestPointTo(
                        new PointD((m.X + d.x) * invScale, (m.Y - d.y) * invScale),
                        config.PxMaxSelectDistance * invScale);

                    if (subSelection != null)
                    {
                        selectNewItem = false;
                        // highlight and save the selected point
                        subSelection.IsHighlighted = true;
                        this.Invalidate();

                        //TODO: Invalidate only changing region
                        //this.Invalidate(subSelection.getDisplayBoundingBox(d.x, d.y, config.ExPntLayerPointWidth,
                        //    mainControler.LayerManager.Scale, 2));

                    }
                    else
                        selectNewItem = true;
                }
                
                if (selectNewItem)
				{

                    mainControler.LayerManager.getItemsAtPoint(currentPosition, ref itemList);

                    // remove invisible items
                    for (int i = 0; i < itemList.Count; i++)
                        if (!(itemList[i].Parent.Visible))
                            itemList.RemoveAt(i);
                }

                int count = itemList.Count;

                if (count > 0)
                {
                    mousePressedTime = Environment.TickCount;

                    if (count > 1) mainControler.sortItemList(ref itemList);
                    IShape shape = itemList[0].Parent;

                    // Show Tooltip with Comment
                    if (!String.IsNullOrEmpty(shape.Commment))
                    {
                        toolTip.Location = new Point(m.X, m.Y);
                        toolTip.Text = shape.Commment;
                        toolTip.Visible = true;
                    }
                }

            }
            dragStartPoint.X = m.X;
            dragStartPoint.Y = m.Y;

        }

        private void PanelMouseMove(object sender, MouseEventArgs m)
        {
            if (this.Visible)
            {
                double scale = mainControler.LayerManager.Scale;
                double x = (m.X + d.x) / scale;
                double y = (m.Y - d.y) / scale;

                if ((SelectedTool == Tool.DrawPoint
                    || SelectedTool == Tool.DrawPolygon
                    || SelectedTool == Tool.DrawPolyline) && selectedTransportShape != null)
                {
                    SetDistanceStatus(
                        selectedTransportShape.getLastElement().RootX,
                        selectedTransportShape.getLastElement().RootY,
                        x, y);
                }
                else
                {
                    SetPositionStatus(x, y);
                }

                if (selectedTool == Tool.ZoomIn
                    || selectedTool == Tool.TransportShapeMove
                    || selectedTool == Tool.MoveComment)
                {
                    Rectangle InvalidateRec = new Rectangle(0, 0, 1, 1);

                    Point minCorner = new Point(dragStartPoint.X, dragStartPoint.Y);
                    Point maxCorner = new Point(m.X, m.Y);

                    #region ZoomRec Calculation

                    if (m.X < dragStartPoint.X)
                    {
                        minCorner.X = m.X;
                        maxCorner.X = dragStartPoint.X;
                    }
                    if (m.Y < dragStartPoint.Y)
                    {
                        minCorner.Y = m.Y;
                        maxCorner.Y = dragStartPoint.Y;
                    }

                    int recX = dragStartPoint.X;
                    int recY = dragStartPoint.Y;

                    zoomRec.X = minCorner.X;
                    zoomRec.Y = minCorner.Y;
                    zoomWidth = maxCorner.X - minCorner.X;
                    zoomHeigth = maxCorner.Y - minCorner.Y;

                    #endregion

                    if (selectedTool == Tool.TransportShapeMove || selectedTool == Tool.MoveComment)
                    {
                        int pointSize;
                        if (selectedTransportShape.Type == MapTools.ShapeLib.ShapeType.MultiPoint)
                            pointSize = config.ExPntLayerPointWidth;
                        else if (selectedTransportShape.Type == MapTools.ShapeLib.ShapeType.PolyLine)
                            pointSize = config.ExPLineLayerPointWidth;
                        else
                            pointSize = config.ExPGonLayerPointWidth;


                        Rectangle invalidateRect =
                            selectedTransportShape.getDisplayBoundingBox(
                                d.x, d.y, pointSize, scale, 1);
                        SizeF stringSize = selectedTransportShape.StringSize;
                        int commentWidth = (int)stringSize.Width;
                        int commentHeight = (int)stringSize.Height;

                        selectedTransportShape.moveToByDifference(
                            (m.X - dragStartPoint.X) / scale,
                            (m.Y - dragStartPoint.Y) / scale, true);

                        if (selectedTool == Tool.MoveComment)
                        {
                            selectedTransportShape.DrawCommentOffset
                                = new Point((m.X - dragStartPoint.X), (m.Y - dragStartPoint.Y));
                        }

                        this.Invalidate(new Rectangle(
                            invalidateRect.Right + selectedTransportShape.DrawCommentOffset.X + 1,
                            invalidateRect.Bottom + selectedTransportShape.DrawCommentOffset.Y + 1,
                            commentWidth,
                            commentHeight));

                        this.Invalidate(invalidateRect);

                        invalidateRect = selectedTransportShape.getDisplayBoundingBox(
                            d.x, d.y, pointSize, scale, 2);

                        this.Invalidate(invalidateRect);

                        this.Invalidate(new Rectangle(
                            invalidateRect.Right + selectedTransportShape.DrawCommentOffset.X + 1,
                            invalidateRect.Bottom + selectedTransportShape.DrawCommentOffset.Y + 1,
                            commentWidth,
                            commentHeight));

                    }
                    else
                    {
                        #region ZoomRec Invalidation

                        //Seite a
                        InvalidateRec.X = minCorner.X;
                        InvalidateRec.Y = minCorner.Y;
                        InvalidateRec.Width = zoomWidth + 5;
                        InvalidateRec.Height = 1;
                        this.Invalidate(InvalidateRec);

                        //Seite c
                        InvalidateRec.Y = maxCorner.Y;
                        this.Invalidate(InvalidateRec);

                        //Seite b
                        InvalidateRec.Y = minCorner.Y;
                        InvalidateRec.Width = 1;
                        InvalidateRec.Height = zoomHeigth + 5;
                        this.Invalidate(InvalidateRec);

                        //Seite d
                        InvalidateRec.X = maxCorner.X;
                        this.Invalidate(InvalidateRec);

                        //oldrec

                        minCorner.X = dragStartPoint.X;
                        minCorner.Y = dragStartPoint.Y;

                        maxCorner.X = oldMovePoint.X;
                        maxCorner.Y = oldMovePoint.Y;

                        if (oldMovePoint.X < dragStartPoint.X)
                        {
                            minCorner.X = oldMovePoint.X;
                            maxCorner.X = dragStartPoint.X;
                        }
                        if (oldMovePoint.Y < dragStartPoint.Y)
                        {
                            minCorner.Y = oldMovePoint.Y;
                            maxCorner.Y = dragStartPoint.Y;
                        }

                        //now width & heigth from rectangle always positive


                        //Seite a
                        InvalidateRec.X = minCorner.X;
                        InvalidateRec.Y = minCorner.Y;
                        InvalidateRec.Width = maxCorner.X - minCorner.X + 5;
                        InvalidateRec.Height = 1;
                        this.Invalidate(InvalidateRec);

                        //Seite c
                        InvalidateRec.Y = maxCorner.Y;
                        this.Invalidate(InvalidateRec);

                        //Seite b
                        InvalidateRec.Y = minCorner.Y;
                        InvalidateRec.Width = 1;
                        InvalidateRec.Height = maxCorner.Y - minCorner.Y + 5;
                        this.Invalidate(InvalidateRec);

                        //Seite d
                        InvalidateRec.X = maxCorner.X;
                        this.Invalidate(InvalidateRec);

                        #endregion

                        zoomRec.Width = zoomWidth;
                        zoomRec.Height = zoomHeigth;
                    }

                    oldMovePoint.X = m.X; //keep coordinates for Invalidation of Zoom Rectangle
                    oldMovePoint.Y = m.Y;

                    // Since we have invalidated some regions we want
                    // to get the OnPaint() invoked now
                    this.Update();
                }
                else if (selectedTool == Tool.MoveGeometry && subSelection != null)
                {
                    int pointSize = config.ExPntLayerPointWidth;

                    this.Invalidate(selectedTransportShape.getDisplayBoundingBox(d.x,
                     d.y, pointSize, scale, 2));

                    subSelection.moveToByDifference(
                        (m.X - dragStartPoint.X) / scale,
                        (m.Y - dragStartPoint.Y) / scale,
                        true);

                    this.Invalidate(selectedTransportShape.getDisplayBoundingBox(d.x,
                         d.y, pointSize, scale, 2));

                    invalidateInfoPanel();

                    this.Update();
                }
            }
        }

        private void PanelMouseUp(object sender, MouseEventArgs m)
        {
            if (this.Visible)
            {
                double scale = mainControler.LayerManager.Scale;
                //unscaled x and y:
                double x = (m.X + d.x) / scale;
                double y = (m.Y - d.y) / scale;


                if (SelectedTool != Tool.Pointer)
                {
                    // right is positive
                    dragWidth = m.X - dragStartPoint.X;
                    // down is positive
                    dragHeight = m.Y - dragStartPoint.Y;
                    screenChanged = true;

                    if (selectedTool == Tool.Move)
                    {
                        if (dragWidth == 0 && dragHeight == 0)
                        {  // since we do not really drag
                            this.mDragging = false;
                            return;
                        }
                        PointD oldD = new PointD();

                        oldD = d;
                        d.x -= dragWidth;
                        d.y += dragHeight;

                        //keep actual panning values for zoom calculation
                        mainControler.PerformAction(new PanAction(oldD, d, scale,
                            this, GravurGIS.CoordinateSystems.CoordinateType.Display));

                    }
                    else if (selectedTool == Tool.ZoomIn || selectedTool == Tool.ZoomOut) //Zoom Tools:
                    {

                        double zoomFactor;
                        PointD newD = new PointD();
                        double maxAbsZoom = mainControler.LayerManager.getMaxAbsoluteZoom();

                        if (selectedTool == Tool.ZoomIn) // ZoomIn
                        {
                            
                            // Wenn aufgezogenes Rechteck größer als 1x1 ist
							if ( Math.Abs(dragWidth) > 1 && Math.Abs(dragHeight) > 1 )
                            {
                                zoomFactor = calculateZoomFactor(zoomRec.Width, zoomRec.Height);

                                newD.x = (d.x + zoomRec.X) * zoomFactor;
                                newD.y = (d.y - zoomRec.Y) * zoomFactor;
                            }
                            //no zoom rectangle created
                            else
                            {
                                zoomFactor = config.ZoomInFactor;
                                newD.x = (int)((d.x + m.X) * zoomFactor) - drawingArea.Width * 0.5;
                                newD.y = (int)(-1 * (m.Y - d.y) * zoomFactor) + drawingArea.Height * 0.5;
                            }
                        }
                        else // ZoomOut
                        {
                            zoomFactor = config.ZoomOutFactor;
                            newD.x = (int)((d.x + m.X) * zoomFactor) - drawingArea.Width * 0.5;
                            newD.y = (int)(-1 * (m.Y - d.y) * zoomFactor) + drawingArea.Height * 0.5;
                        }

                        double newAbsoluteZoom = absoluteZoom * zoomFactor;
                        mainControler.PerformAction(
                            new ZoomAction(absoluteZoom, d, newD, newAbsoluteZoom,
                                new PointD(x, y), maxAbsZoom, mainControler));
                    }
                    else if (selectedTool == Tool.TransportShapeMove)
                    {
                        #region Tool.TransportShapeMove

                        int pointSize;
                        if (selectedTransportShape.Type == MapTools.ShapeLib.ShapeType.MultiPoint)
                            pointSize = config.ExPntLayerPointWidth;
                        else if (selectedTransportShape.Type == MapTools.ShapeLib.ShapeType.PolyLine)
                            pointSize = config.ExPLineLayerPointWidth;
                        else
                            pointSize = config.ExPGonLayerPointWidth;

                        if (m.X - dragStartPoint.X == 0 && m.Y - dragStartPoint.Y == 0) // a click, instead of dragging
                        {
                            this.Invalidate(selectedTransportShape.getDisplayBoundingBox(d.x,
                             d.y, pointSize, scale, 1));

                            mainControler.PerformAction(
                                new MoveIShapeAction(selectedTransportShape,
                                new Point(
                                    Convert.ToInt32(m.X + d.x),
                                    Convert.ToInt32(m.Y - d.y)),
                                new Point(
                                    Convert.ToInt32(selectedTransportShape.MinX * scale),
                                    Convert.ToInt32(selectedTransportShape.MinY * scale)),
                                pointSize,
                                scale,
                                this.mainControler.LayerManager));
                        }
                        else
                        {
                            mainControler.PerformAction(
                                new MoveIShapeAction(selectedTransportShape,
                                    new Point(m.X, m.Y),
                                dragStartPoint, pointSize,
                                scale, this.mainControler.LayerManager));
                        }

                        itemList.Clear();

                        #endregion
                    }
                    else if (selectedTool == Tool.MoveGeometry && subSelection != null)
                    {
                        #region Tool.MoveGeometry

                        int pointSize = config.ExPntLayerPointWidth;

                        this.Invalidate(selectedTransportShape.getDisplayBoundingBox(d.x,
                             d.y, pointSize, scale, 2));

                            mainControler.PerformAction(
                                new MoveGeometryAction(subSelection, selectedTransportShape,
                                    new Point(m.X, m.Y),
                                dragStartPoint, pointSize,
                                scale, this.mainControler.LayerManager));

                        this.Invalidate(selectedTransportShape.getDisplayBoundingBox(d.x,
                             d.y, pointSize, scale, 2));

                        invalidateInfoPanel();

                        #endregion
                    }
                    else if (selectedTool == Tool.MoveComment)
                    {
                        #region Tool.MoveComment

                        //mainControler.PerformAction(
                        //        new MoveCommentAction(new Point(m.X, m.Y),
                        //        dragStartPoint, pointSize,
                        //        scale, this.mainControler.LayerManager));

                        #endregion
                    }
                    else if (selectedTool == Tool.DrawPolyline)
                    {
                        #region Tool.DrawPolyline

                        mainControler.PerformAction(new DrawAction(LayerType.PolylineCanvas, x, y, bEditingStarted, mainControler));
                        bEditingStarted = true;

                        #endregion
                    }
                    else if (selectedTool == Tool.DrawPolygon)
                    {
                        #region Tool.DrawPolygon

                        mainControler.PerformAction(new DrawAction(LayerType.PolygonCanvas, x, y, bEditingStarted, mainControler));
                        bEditingStarted = true;

                        #endregion
                    }
                    else if (selectedTool == Tool.DrawPoint)
                    {
                        #region Tool.DrawPoint

                        mainControler.PerformAction(new DrawAction(absoluteZoom, x, y, mainControler));

                        #endregion
                    }
                    else if (selectedTool == Tool.Identify)
                    {
                        #region Tool.Identify

                        mHighlight = mainControler.identify(x, -y);
                        this.Invalidate();

                        #endregion
                    }

                    SetPositionStatus(x, y);
                }
                else // selectedTool == Tool.Pointer
                {
                    bool invalidate = false;

                    if (itemList.Count > 0 && !contextMenuisActive)
                    {
                        toolTip.Visible = false;
                        toolTip.Invalidate();
                        mainControler.LayerManager.SelectedTransportQuadtreeItem = itemList[0];

                        // if we change the selected item, invalidate!
                        if (selectedTransportShape != itemList[0].Parent)
                        {
							OnSelectShape(itemList[0].Parent);
							this.Invalidate();
                        }                       

                        if (Environment.TickCount - mousePressedTime > 800)
                        {
                            contextMenuisActive = true;
                            if (subSelection == null)
                                shapeEditMenu.Show(this, new Point(m.X + 5, m.Y + 5));
                            else
                            {
                                ShpPoint pointToDelete = subSelection as ShpPoint;
                                if (pointToDelete != null)
                                {
                                    using (EditGeometryMenu geometryMenu = new EditGeometryMenu(mainForm, selectedTransportShape,
                                        pointToDelete))
                                    {
                                        geometryMenu.Show(this, new Point(m.X + 5, m.Y + 5));
                                    }
                                }
                            }
                        }

                        // if SelectedTool == Tool.TransportShapeMove, then itemList.Clear() is done
                        // in the appropriate MouseUp-Part as the QuadTreeItem is still needed!
                        if (selectedTool != Tool.TransportShapeMove
                            && selectedTool != Tool.Pointer) itemList.Clear();
                    }
                    else
                    {
                        if (selectedTransportShape != null)
                        {
                            selectedTransportShape.IsHighlighted = false;
                            invalidate = true;
                        }

                        selectedTransportShape = null;

                        if (invalidate) this.Invalidate();
                    }
                }
            }
        }

		private void OnSelectShape(IShape shapeToSelect)
		{
			if (selectedTransportShape != null)
				selectedTransportShape.IsHighlighted = false;

			selectedTransportShape = shapeToSelect;
			selectedTransportShape.IsHighlighted = true;
		}

        public void SetPositionStatus(double x, double y) //wird übers MoveIShapeAction noch nicht aufgerufen!
        {
            mainForm.setStatusBarText(String.Format("{0} : {1} | 1:{2}",
                Convert.ToString(Math.Round(x, 2)),
                Convert.ToString(Math.Round(-y, 2)),
                ScaleDivisor.ToString()));
        }
        private void SetDistanceStatus(double oldX, double oldY, double x, double y)
        {
            double dist = Math.Sqrt((x - oldX) * (x - oldX) + (y - oldY) * (y - oldY));
            string text = string.Empty;

            if (dist >= 1000) // display as kilometers
                text = String.Format("{0}km", (dist / 1000).ToString("#0.0"));
            else if (dist >= 1) // display as meters
                text = String.Format("{0}m", Convert.ToInt32(dist).ToString());
            else // display as centimeters
                text = String.Format("{0}cm", Convert.ToInt32(dist * 10).ToString());

            mainForm.setStatusBarText(String.Format("{0} : {1} | {2}",
                Convert.ToString(Math.Round(x, 2)),
                Convert.ToString(Math.Round(-y, 2)),
                text));
        }

        /// <summary>
        /// Liefert oder setzt den Dividenten des Maßstabs ( das "x" bei 1:x).
        /// 
        /// Die Formel für die Kartenstrecke x (1 : x) leitet sich folgermaßen her:
        /// Ausgangspunkte:
        /// new_scale = firstScale * absoluteZoom;
        /// newAbsoluteZoom = absoluteZoom * zoomFactor;
        /// (mainControler.LayerManager.FirstMapMeassure * 100 * dpiX) / (absoluteZoom * 2.54)
        ///  
        /// sollte also zu schreiben sein als:
        /// ((absoluteZoom / new_scale) * 100 * dpiX) / (absoluteZoom * 2.54)
        ///  
        /// bzw.
        /// ((1/ new_scale * 2.54) * 100 * dpiX)
        ///  
        /// bzw.
        /// ((100 * dpiX) / (mainControler.LayerManager.Scale * 2.54))
        /// </summary>
        public int ScaleDivisor
        {
            get
            {
                return (int)Math.Round(((100 * dpiX) / (mainControler.LayerManager.Scale * 2.54)), 0);
            }
            set
            {
                // do only sth. if sth. changed
                if (value != ScaleDivisor)
                {
                    //double The scale would be: (100 * dpiX) / (value * 2.54);
                    double newAbsoluteZoom = (100 * dpiX) / (value * 2.54 * mainControler.LayerManager.FirstScale);
                    double zoomFactor = newAbsoluteZoom / absoluteZoom;
                    double mathAbsZoom = mainControler.LayerManager.getMaxAbsoluteZoom();
                    mainControler.PerformAction(new ZoomAction(
                        absoluteZoom,
                        d, d * zoomFactor,
                        newAbsoluteZoom,
                        d, mathAbsZoom, mainControler));
                }
            }
        }

        public void OnDrawInfChanged(DrawShapeInformation inf, LayerType lType)
        {
            if ((inf == DrawShapeInformation.EditStarted)
                || (inf == DrawShapeInformation.EditStoppedUndone)
              )
            {
                this.bEditingStarted = true;
                switch (lType)
                {
                    case LayerType.Shape:
                        break;
                    case LayerType.Image:
                        break;
                    case LayerType.PointCanvas:
                        this.SelectedTool = Tool.DrawPoint;
                        break;
                    case LayerType.PolylineCanvas:
                        this.SelectedTool = Tool.DrawPolyline;
                        break;
                    case LayerType.PolygonCanvas:
                        this.SelectedTool = Tool.DrawPolygon;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                this.bEditingStarted = false;
                this.SelectedTool = Tool.Pointer;
            }

            this.Invalidate();
        }

        #endregion

        public void panByValue(double newUnscaledX, double newUnscaledY)
        {
            double scale = mainControler.LayerManager.Scale;
            d.x = (newUnscaledX) * scale;
            d.y = (newUnscaledY) * scale;

            MapPanelBindings.RecalculateImages(scale,
                (d.x / scale), (d.y / scale));

            this.Invalidate();
        }

        public void SetPosition(PointD newPosition, GravurGIS.Styles.HorizontalAlignment hv, VerticalAlignment vv) {
            double scale = mainControler.LayerManager.Scale;
            double invScale = 1 / scale;
            newPosition.x -= (((int)hv * 0.5 * drawingArea.Width) * invScale);
            newPosition.y += (((int)vv * 0.5 * drawingArea.Height) * invScale);

            mainControler.PerformAction(new PanAction(d * invScale, newPosition,
                scale, this, GravurGIS.CoordinateSystems.CoordinateType.World));
        }

        public PointD GetPosition(GravurGIS.Styles.HorizontalAlignment hv, VerticalAlignment vv)
        {
            double scale = mainControler.LayerManager.Scale;
            return new PointD(
                (d.x + (int)hv * 0.5 * drawingArea.Width) / scale,
                (d.y - (int)vv * 0.5 * drawingArea.Height) / scale);
        }
     
      
        /// <summary>
        /// The Viewport of our view in world coordinates.
        /// (x,y) is the min x and min y corner (so in our view the bottom left corner)
        /// </summary>
        public System.Drawing.Rectangle Viewport
        {
            get
            {
                double scale = mainControler.LayerManager.Scale;
                return new Rectangle(
                      Convert.ToInt32( d.x / scale),
                      Convert.ToInt32( d.y / scale),
                      Convert.ToInt32(drawingArea.Width / scale),
                      Convert.ToInt32(drawingArea.Height / scale));
            }
        }
        
        public PointD D {
			get { return d; }
		}

        #region Getters/Setters

        public bool MDragging
        {
            get { return mDragging; }
            set { mDragging = value; }
        }
        
        public float DpiX {
			get { return this.dpiX; }
		}

		public float DpiY
		{
			get { return this.dpiY; }
		}
		       
        public double DX
        {
            get { return d.x; }
            set { d.x = value; }
        }
        public double DY
        {
            get { return d.y; }
            set { d.y = value; }
        }
        public double AbsolutZoom
        {
            get { return absoluteZoom; }
            set { absoluteZoom = value; }
        }

        public System.Drawing.Rectangle DrawingArea
        {
            get { return drawingArea; }
        }
        public Tool SelectedTool
        {
            get { return selectedTool; }
            set
            {
                if (value == Tool.DrawPolyline || value == Tool.DrawPolygon)
                {
                    if (bEditingStarted) EditModeInterrupted(false, value);
                    ClearSelections();
                }
                else if (bEditingStarted) EditModeInterrupted(true, selectedTool);

                if (value == Tool.DrawPoint) ClearSelections();
                selectedTool = value;
                SelectedToolChanged(selectedTool);
            }
        }

        private void ClearSelections()
        {
            bool invalidate = false;
            if (subSelection != null)
            {
                invalidate = true;
                subSelection.IsHighlighted = false;
                subSelection = null;
            }
            if (selectedTransportShape != null)
            {
                invalidate = true;
                selectedTransportShape.IsHighlighted = false;
                selectedTransportShape = null;
            }
            if (itemList.Count > 0)
            {
                invalidate = true;
                itemList.Clear();
            }
            if (invalidate) this.Invalidate();
            
        }
        public int Margin
        {
            get { return margin; }
            set { margin = value; }
        }
        public int DragWidth
        {
            get { return dragWidth; }
            set { dragWidth = value; }
        }
        public int DragHeight
        {
            get { return dragHeight; }
            set { dragHeight = value; }
        }
        public bool ScreenChanged
        {
            get { return screenChanged; }
            set { screenChanged = value; }
        }
        public List<QuadTreePositionItem<IShape>> ItemList
        {
            get { return itemList; }
        }
        
        public IShape SelectedTransportShape
        {
            get { return selectedTransportShape;}
			set { OnSelectShape(value); }
        }

        public void ViewHasChanged(PointD d)
        {
            this.d.x = d.x;
            this.d.y = d.y;
            this.ScreenChanged = true;
            this.Invalidate();
            this.Update();
        }

        public void movePointHasChanged(System.Drawing.Point m)
        {
            this.oldMovePoint.X = m.X;
            this.oldMovePoint.Y = m.Y;
        }

        private void invalidateInfoPanel()
        {
            this.Invalidate(new Rectangle(drawingArea.X, drawingArea.Y,
                        drawingArea.Width, m_TextHeight));
        }

        public void InvalidateRegion(List<System.Drawing.Rectangle> rectangleList)
        {
            #if DRAW_INVALIDATEREC
                this.rectangleList = rectangleList;
            #endif
            for (int i = 0; i < rectangleList.Count; i++)
            {
                this.Invalidate(rectangleList[i]);
            }
            this.Update();
        }
        #endregion

#if DEVELOP

		double __backupZomIn;
		double __backupZoomOut;
        
        internal void DoStressTest()
        {
            if (!timer.Enabled)
		    {
				__backupZomIn = config.ZoomInFactor;
				__backupZoomOut = config.ZoomOutFactor;

				config.ZoomInFactor = 2.0;
				config.ZoomOutFactor = 0.5;
                SelectedTool = Tool.Move;
                timer.Interval = 25000;
                timer.Tick += new EventHandler(timer_Tick_Zoom);
                timer.Enabled = true;
            }
            else {
                timer.Enabled = false;
                config.ZoomInFactor = __backupZomIn;
                config.ZoomOutFactor = __backupZoomOut;
            }
        }
        
        void timer_Tick_Zoom(object sender, EventArgs e)
        {
			int x;
			int y;
			
			reset();
			Thread.Sleep(2500);
			
			SelectedTool = Tool.ZoomIn;
			
			// Zoom 3x in to a random position, and wait everytime 2sec.
			for (int i = 0; i < 5; ++i) {
				x = (int)((drawingArea.Left + drawingArea.Right) * 0.5);
				y = (int)((drawingArea.Top + drawingArea.Bottom) * 0.3);
				
				this.PanelMouseDown(null,
					new MouseEventArgs(MouseButtons.Left, 1,
					x,
					y,
					0)
				);

				Thread.Sleep(200);

				this.PanelMouseUp(null, new MouseEventArgs(MouseButtons.Left, 1,
					x,
					y,
					0)
				);
				
				Thread.Sleep(1800);		
			}

			SelectedTool = Tool.ZoomOut;
			
			// Zoom 3x out to a random position, and wait everytime 2sec.
			for (int i = 0; i < 5; ++i) {
				x = (int)((drawingArea.Left + drawingArea.Right) * 0.6);
				y = (int)((drawingArea.Top + drawingArea.Bottom) * 0.8);
				
				this.PanelMouseDown(null,
					new MouseEventArgs(MouseButtons.Left, 1,
					x,
					y,
					0)
				);

				Thread.Sleep(200);
				
				this.PanelMouseUp(null, new MouseEventArgs(MouseButtons.Left, 1,
					x,
					y,
					0)
				);

				Thread.Sleep(1800);
			}
        }

        void timer_Tick_Pan(object sender, EventArgs e)
        {
            int x = random.Next(drawingArea.Left, drawingArea.Right);
            int y = random.Next(drawingArea.Top, drawingArea.Bottom);
            int dX = random.Next(drawingArea.Left, drawingArea.Right) - x;
            int dY = random.Next(drawingArea.Top, drawingArea.Bottom) - y;
            

            this.PanelMouseDown(null,
                new MouseEventArgs(MouseButtons.Left, 1,
                x,
                y,
                0)
            );

            Thread.Sleep(1000);

            while (stressTestMovementDelta.X + dX < -drawingArea.Right
                || stressTestMovementDelta.X + dX > 2 * drawingArea.Width
                || stressTestMovementDelta.Y + dY < -drawingArea.Bottom
                || stressTestMovementDelta.Y + dY > 2 * drawingArea.Height)
            {
                dX = random.Next(drawingArea.Left, drawingArea.Right) - x;
                dY = random.Next(drawingArea.Top, drawingArea.Bottom) - y;
            }

            stressTestMovementDelta.X += dX;
            stressTestMovementDelta.Y += dY;
            

            this.PanelMouseUp(null, new MouseEventArgs(MouseButtons.Left, 1,
                x + dX,
                y + dY,
                0)
            );
        }
#endif

        public void RequesLayerChange()
        {
            if(RequestChange != null)
                RequestChange();
        }
       
    }
}
