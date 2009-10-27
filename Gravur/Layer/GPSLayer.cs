using System;
using System.Collections.Generic;
using System.Text;
using GravurGIS.GPS;
using System.Windows.Forms;
using System.Drawing;
using GravurGIS.Topology;
using GravurGIS.Rendering;

namespace GravurGIS.Layers
{
    public class GPSLayer : Layer
    {
        #region ILayer Members

        private MainControler mainControler;
        private Config config;
        // GPS
        private EventHandler updateDataHandler;
        private EventHandler updateStatusHandler;
        GpsDeviceState device = null;
        GpsPosition position = null;

        private GKCoord gk;
        private GKCoord oldGk;
        private GKCoord gkToRender;
        private PointD p; 

        private List<Rectangle> recList = new List<Rectangle>();
        private Rectangle tempRec;
        
        private System.Threading.Timer gpsTimer;
        Boolean _updateAsDataComes;
        
        #region Events
        
		public delegate void PositionChangedDelegate(GKCoord position);
		public event PositionChangedDelegate PositionChanged;
		
        #endregion
        
        public GPSLayer(MainControler mainControler)
        {
            this._layerType = LayerType.GPSPosition;
            

            // GPS init 
            this.mainControler = mainControler;
            this.config = mainControler.Config;
            this.gpsTimer = new System.Threading.Timer(updateSceen, null, 0, config.GPSUpdateInterval);
            
            updateDataHandler = new EventHandler(UpdateData);
            updateStatusHandler = new EventHandler(UpdateStatus);
            mainControler.GpsControler.DeviceStateChanged += new DeviceStateChangedEventHandler(gps_DeviceStateChanged);
            mainControler.GpsControler.LocationChanged += new LocationChangedEventHandler(gps_LocationChanged);
			mainControler.SettingsLoaded += new MainControler.SettingsLoadedDelegate(mainControler_SettingsLoaded);
			
            this.mainControler.createNewCanvas(0, 0, 1.0);
            		
            // make sure there are initialized values in
            gk.h_value = 0.0;
            gk.r_value = 0.0;

            oldGk = gk;
        }
        
        ~GPSLayer() {
			mainControler.SettingsLoaded -= mainControler_SettingsLoaded;
			mainControler.GpsControler.DeviceStateChanged -= gps_DeviceStateChanged;
			mainControler.GpsControler.LocationChanged -= gps_LocationChanged;
        }
        
        /// <summary>
        /// Timer callback for GPS screen update
        /// </summary>
        /// <param name="state"></param>
        private void updateSceen(object state) {
        
			// have we moved?
			if ( config.UseGPS
				&& (Math.Abs(oldGk.h_value - gk.h_value) > 0.01)
				&& (Math.Abs(oldGk.r_value - gk.r_value) > 0.01) )
			{ // yes
			
				// copy the position we want to draw
				gkToRender = gk;

				OnPositionChanged();
			
				// call the UpdateData method via the updateDataHandler so that we 
				// update the UI on the UI thread 
				if (mainControler.MainForm.InvokeRequired)
				{
					//IAsyncResult result = 
					this.mainControler.MainForm.BeginInvoke(updateDataHandler);
					//result.AsyncWaitHandle.WaitOne();
					//this.mainControler.MainForm.EndInvoke(result);
				}
				else SetInvalidateRegions();
			}
			// if we have not moved, we ignore it
        }
        

		void mainControler_SettingsLoaded(Config config)
		{
			this.config = config;
			
			if (config.GPSUpdateInterval == 0) {
				_updateAsDataComes = true;
				gpsTimer.Dispose();
				gpsTimer = null;
			} else {
				
				_updateAsDataComes = false;
				if (gpsTimer != null) gpsTimer.Change(0, config.GPSUpdateInterval);
				else gpsTimer = new System.Threading.Timer(updateSceen, null, 0, config.GPSUpdateInterval);
			}
		}
		
        public GKCoord CurrentPositon
        {
            get { return this.gk; }
        }
        
        private void OnPositionChanged() {
			if (PositionChanged != null) PositionChanged(gkToRender);
        }

        protected void gps_LocationChanged(object sender, LocationChangedEventArgs args)
        {
            try
            {
                position = args.Position;
                if (position != null)
                {
                    if (position.LatitudeValid && position.LongitudeValid)
                    {
                        this.gk = mainControler.CoordCalculator.GeoGkGDAL(position, config.CoordGauﬂKruegerStripe);
                        
                        if (_updateAsDataComes) {
							OnPositionChanged(); 
							updateSceen(null);
						}
                    }
                }
            }
            catch
            {
               // if something is going wrong here, we have a serious problem
            }
        }

        protected void gps_DeviceStateChanged(object sender, DeviceStateChangedEventArgs args)
        {
            device = args.DeviceState;

            this.mainControler.MainForm.BeginInvoke(updateStatusHandler);
            // call the UpdateData method via the updateDataHandler so that we 
            // update the UI on the UI thread 
            //if (this.mainControler.MainForm.InvokeRequired)
            //{
            //    this.mainControler.MainForm.BeginInvoke(updateDataHandler);

            //}
            //else SetInvalidateRegions();    
        }

        void UpdateStatus(object sender, System.EventArgs args)
        {
            mainControler.setStatusTimed(String.Format("GPS-Status: {0}", device.DeviceState), 2500);
        }

        void UpdateData(object sender, System.EventArgs args)
        {
			// invalidate only iff we want to show GPS on the map
			if (config.UseGPS)
				SetInvalidateRegions();
        }

        private void SetInvalidateRegions()
        {
            if (mainControler.GpsControler.Opened && mainControler.HasActiveDisplay)
            {
                if (config.GpsViewMode == GpsView.StaticMap)
                {
                    recList.Clear();

                    p = CoordCalculator.calculateDisplayCoords(oldGk, mainControler.MapPanel.DX,
                        mainControler.MapPanel.DY, mainControler.LayerManager.Scale);

                    // Clipping: We do not need to invalide if the point is out of our view
                    if (isInDrawingArea(p, mainControler.MapPanel.DrawingArea))
                    {
                        tempRec.X = Convert.ToInt32(Math.Min(p.x - 2, Int32.MaxValue));
                        tempRec.Y = Convert.ToInt32(Math.Min(p.y - 2, Int32.MaxValue));
                        tempRec.Width = config.GpsLayerPointWidth + 4;
                        tempRec.Height = config.GpsLayerPointWidth + 4;
                        recList.Add(tempRec);
                    }
                    p = CoordCalculator.calculateDisplayCoords(gkToRender, mainControler.MapPanel.DX,
                        mainControler.MapPanel.DY, mainControler.LayerManager.Scale);

                    // Clipping: We do not need to invalide if the point is out of our view
                    if (isInDrawingArea(p, mainControler.MapPanel.DrawingArea))
                    {
                        tempRec.X = System.Convert.ToInt32(Math.Min(p.x - 2, Int32.MaxValue));
                        tempRec.Y = System.Convert.ToInt32(Math.Min(p.y - 2, Int32.MaxValue));
                        recList.Add(tempRec);
						oldGk = gkToRender;
                    }
                    this.mainControler.MapPanel.InvalidateRegion(recList);
                }
                else //config.GpsViewMode == GpsView.staticPoint
                {
                    double scale = mainControler.LayerManager.Scale;
                    mainControler.MapPanel.DX =
						mainControler.MapPanel.DrawingArea.Width / -2 + gkToRender.r_value * scale;
                    mainControler.MapPanel.DY =
						this.mainControler.MapPanel.DrawingArea.Height / 2 + gkToRender.h_value * scale;
                    this.mainControler.MapPanel.RequesLayerChange();
                    this.mainControler.MapPanel.Invalidate();
                }
            }
        }

        private bool isInDrawingArea(PointD pos, Rectangle canvas)
        {
            if ((   p.x < canvas.X
                ||  p.y < canvas.Y
                ||  p.x > (canvas.X + canvas.Width)
                ||  p.y > (canvas.Y + canvas.Height)))
                return false;
            else
                return true;
        }

        public override bool Render(RenderProperties rp)
        {
            // clip everything outside drawingArea
            // in this case this means, draw nothing
            if (isInDrawingArea(p, rp.DrawingArea))
            {
                SolidBrush brush = new SolidBrush(config.GpsLayerPointColor);
                p = CoordCalculator.calculateDisplayCoords(
                    gkToRender, rp.DX, rp.DY, this.mainControler.LayerManager.Scale);
                rp.G.FillEllipse(brush, new Rectangle(
                    Convert.ToInt32(p.x),
                    Convert.ToInt32(p.y),
                    config.GpsLayerPointWidth,
                    config.GpsLayerPointWidth));
            }
        
            return true;
        }

        public override void reset()
        {
           
        }

        public override void recalculateData(double absoluteZoom, double scale, double xOff, double yOff)
        {
            // we have nothing to do here   
        }

        public new string Description
        {
            get { return config.GpsLayerDescription; }
        }

        public new string LayerName
        {
            get { return config.GpsLayerName; }
            set { config.GpsLayerName = value; }
        }

        public new bool Changed
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        public new void Refresh()
        { 
        }

        #endregion
    }
}
