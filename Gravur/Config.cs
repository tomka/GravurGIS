using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using GravurGIS.CoordinateSystems;
using System.Text.RegularExpressions;
using System.Globalization;
using GravurGIS.Styles.NewLayerStyle;

namespace GravurGIS
{
    public enum ScaleDisplay { Map, Zoom }
    public enum GpsView {StaticPoint, StaticMap}
    public enum GpsTrackingStartingMode { AfterKeyPress, AfterDialogClose }
    public enum GpsTrackingTriggerMode { KeyPress, TimeInterval }
	public enum HardwareKeys { Up, Down, Left, Right, Enter }
	public enum HardwareKeyMappings { TempMove, GPSTrackingTrigger, GPSTrackingStop, None }

    [Serializable]
    public class Config
    {
        public Config() {
			this.exPGonLayerLineWidth = 1;
			this.exPLineLayerLineWidth = 1;
			ShowTrackingStatus = true;
		}

        public void SetDefaults()
        {
            string personalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            this.searchDirList.Add(personalFolder);
            this.searchDirList.Add(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + programName);
            this.exPntLayerFile = personalFolder + "\\timberNet\\transferPunkt.shp";
            this.exPLineLayerFile = personalFolder + "\\timberNet\\transferPolyline.shp";
            this.exPGonLayerFile = personalFolder + "\\timberNet\\transferPolygon.shp";

            this.categoryList.Add("Kommentar");
            this.categoryList.Add("Baum");
            this.categoryList.Add("Fl‰che");

            this.horizontalDatum = HorizontalDatum.Bessel1841;
            
            this.GPSTrackingTimeInterval = 5000;
            this.GPSTrackingComment = "GPS";
            this.GPSTrackingDiscardSamePositions = true;
            this.UseGPS = false;

            this.KeyMapping_Up = HardwareKeyMappings.None;
            this.KeyMapping_Down = HardwareKeyMappings.None;
            this.KeyMapping_Left = HardwareKeyMappings.GPSTrackingTrigger;
            this.KeyMapping_Right = HardwareKeyMappings.GPSTrackingStop;
            this.KeyMapping_Enter = HardwareKeyMappings.TempMove;
            
            InitStaticData();
        }

        public void InitStaticData()
        {
            this.appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            this.datumFileList.Add("data/gt_datum.csv");
            this.ellipsoidFilList.Add("data/gt_ellips.csv");
        }

        // Program
        private static string programName = "GravurGIS";
        private static string configFileName = "config.xml";
        private string appDirectory;

        // Network
        private int httpTimeout = 3000; //Timeout in ms

        // Programstart
        private string defaultProject = "(keins)";
        private bool useDefaultProject = false;
        private bool showDistanceLine = true;

        // Display
        private bool showSpecialLayers = false;
        private ScaleDisplay scaleDisplayMode = ScaleDisplay.Map;
        private double zoomInFactor = 2.0d;
        private double zoomOutFactor = 0.75d;
        private bool showNorthArrow = true;
        private int pxMaxSelectDistance = 6;
		public bool ShowTrackingStatus { get; set; }
       

        // Layers
        private GravurGIS.Styles.NewLayerStyle.NewLayerStyles newLayerStyle = NewLayerStyles.RandomColor;

        // Exchange-Layers
        private List<String> categoryList = new List<string>();

        private string exPntLayerFile = "";
        private string exPntLayerName = "Punkt-Austauschebene";
        private string exPntLayerComment = "";
        private string exPntLayerDescription = "Austauschlayer (Punkte)";
        private Color exPntLayerPointColor = Color.Black;
        private int exPntLayerPointWidth = 4;
        private bool exPntLayerDisplayComments = false;
        private string exPntLayerDBFCommentFieldName = "Comment";
        private int exPntLayerDisplayCommentMaxLength = 10;

        private string exPLineLayerFile = "";
        private string exPLineLayerName = "Linienzug-Austauscheben";
        private string exPLineLayerDescription = "Austauschlayer (Linienz¸ge)";
        private string exPLineLayerComment = "";
        private string exPLineLayerDBFCommentFieldName = "Comment";
        private Color exPLineLayerPointColor = Color.LightGray;
        private Color exPLineLayerLineColor = Color.Black;
        private int exPLineLayerPointWidth = 1;
		public int exPLineLayerLineWidth { get; set; }
        private bool exPLineLayerDisplayComments = false;
        private int exPLineLayerDisplayCommentMaxLength = 10;

        private string exPGonLayerFile = "";
        private string exPGonLayerName = "Polygon-Austauschebene";
        private string exPGonLayerDescription = "Austauschlayer (Polygone)";
        private string exPGonLayerComment = "";
        private string exPGonLayerDBFCommentFieldName = "Comment";
        private Color exPGonLayerPointColor = Color.LightGray;
        private Color exPGonLayerLineColor = Color.Black;
        private Color exPGonLayerFillColor = Color.LightGray;
        private bool exPGonLayerFill = true;
        private int exPGonLayerPointWidth = 1;
		public int exPGonLayerLineWidth { get; set; }
        private bool exPGonLayerDisplayComments = false;
        private int exPGonLayerDisplayCommentMaxLength = 10;

        private Color selectedExShapeLineColor = Color.Orange;

        //GPS-Layer
        private string gpsLayerFile = "";
        private string gpsLayerName = "GPS Ebene";
        private string gpsLayerDescription = "Anzeigeebene von GPS Koordinaten";
        private Color gpsLayerPointColor = Color.Red;
        private int gpsLayerPointWidth = 4;
        private GpsView gpsViewMode = GpsView.StaticMap;
		private UInt32 gpsUpdateInterval = 3000;
    
        // Project
        private List<String> searchDirList = new List<string>();
        private List<String> datumFileList = new List<string>();
        private List<String> ellipsoidFilList = new List<string>();

        // GPS
        private int coordGauﬂKruegerStripe = 2;
        private bool useGPS = false;

        private HorizontalDatum horizontalDatum;
        private HorizontalDatum userDatum = null;
        
        #region Getters/Setters

		// Hardware Key Mappins

		public HardwareKeyMappings KeyMapping_Up { get; set; }
		public HardwareKeyMappings KeyMapping_Down { get; set; }
		public HardwareKeyMappings KeyMapping_Left { get; set; }
		public HardwareKeyMappings KeyMapping_Right { get; set; }
		public HardwareKeyMappings KeyMapping_Enter { get; set; }
        
        public bool GPSTracking { get; set; }
        
        public GpsTrackingStartingMode GPSTrackingStartupMode { get; set; }
        
        public String GPSTrackingComment { get; set; }
        
        public GpsTrackingTriggerMode GPSTrackingTriggerMode { get; set; }
        
        public int GPSTrackingTimeInterval { get; set; }

		public UInt32 GPSUpdateInterval { get { return gpsUpdateInterval; } set { gpsUpdateInterval = value; } }
        
        
        /// <summary>
        /// If set to true the GPS tracking system should use
		/// same positions only once
        /// </summary>
        public Boolean GPSTrackingDiscardSamePositions { get; set; }

        public int PxMaxSelectDistance
        {
            get { return pxMaxSelectDistance; }
            set { pxMaxSelectDistance = value; }
        }

        public bool ShowNorthArrow
        {
            get { return showNorthArrow; }
            set { showNorthArrow = value; }
        }

        [XmlIgnore()]
        public Color NewLayerStaticColor
        {
            get;
            set;
        }
        public int NewLayerStaticColorARGB
        {
            get { return NewLayerStaticColor.ToArgb(); }
            set { NewLayerStaticColor = Color.FromArgb(value); }
        }

        public GravurGIS.Styles.NewLayerStyle.NewLayerStyles NewLayerStyle
        {
            get { return newLayerStyle; }
            set { newLayerStyle = value; }
        }
        
        public bool ShowDistanceLine
        {
            get { return showDistanceLine; }
            set { showDistanceLine = value; }
        }

        [XmlIgnore]
        public HorizontalDatum Datum
        {
            get { return horizontalDatum; }
            set { this.horizontalDatum = value; }
        }

        [XmlIgnore]
        public HorizontalDatum UserDatum
        {
            get { return userDatum; }
            set { this.userDatum = value; }
        }

        public String DatumExtendedWKT
        {
            get
            {
                return WKTFromDatum(this.horizontalDatum);
            }
            set
            {
                this.Datum = DatumFromWKT(value);
            }
        }

        public String UserDatumExtendedWKT
        {
            get
            {
                if (this.userDatum == null)
                    return "NOTSET";
                else
                    return WKTFromDatum(this.userDatum);
            }
            set
            {
                if (value == "NOTSET")
                    this.userDatum = null;
                else
                    this.userDatum = DatumFromWKT(value);
            }
        }

        private string WKTFromDatum(HorizontalDatum datum)
        {
            StringBuilder sb = new StringBuilder();
            //CF needs a CultureInfo overload.This can likely be changed in the full framework version with no ill effect.
            System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("");
            sb.AppendFormat(CI, "DATUM[\"{0}\", {1}", datum.Name, datum.Ellipsoid.WKT);

            if (datum.Wgs84Parameters != null)
                sb.AppendFormat(CI, ", {0}", datum.Wgs84Parameters.WKT);
            if (!String.IsNullOrEmpty(datum.Authority) && datum.AuthorityCode > 0)
                sb.AppendFormat(CI, ", AUTHORITY[\"{0}\", \"{1}\"]", datum.Authority, datum.AuthorityCode);
            if (!String.IsNullOrEmpty(datum.Abbreviation))
                sb.AppendFormat(CI, ", ABBREVIATION[\"{0}\"]", datum.Abbreviation);
            sb.Append("]");

            return sb.ToString();
        }

        private HorizontalDatum DatumFromWKT(string wkt)
        {
            // DATUM["Bessel 1841",
            //  SPHEROID["Bessel 1841", 6377397.15508, 299.1528128, AUTHORITY["EPSG", "7004"]],
            //  TOWGS84[582, 105, 414, 1.04, 0.35, -3.08, 8.3],
            //  ABBREVIATION["BES"]]

            try
            {
                System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("");
                NumberFormatInfo style =
                (NumberFormatInfo)System.Globalization.CultureInfo.CurrentCulture.NumberFormat.Clone();
                style.NumberDecimalSeparator = ".";


                string name;
                Match match = Regex.Match(wkt, "DATUM\\[\"(.+?)\"");
                if (match.Success) name = match.Value.Split('"')[1];
                else throw new Exception();

                StringBuilder spheroid = new StringBuilder(); ;
                match = Regex.Match(wkt, "SPHEROID\\[\"(.+?)\\]\\],");
                if (match.Success) spheroid.Append(match.Value);
                else throw new Exception();

                StringBuilder toWGS84 = new StringBuilder();
                match = Regex.Match(wkt, "TOWGS84\\[(.+?)\\],");
                if (match.Success) toWGS84.Append(match.Value);
                else throw new Exception();

                string abrev;
                match = Regex.Match(wkt, "ABBREVIATION\\[\"(.+?)\\]\\]");
                if (match.Success) abrev = match.Value.Split('"')[1];
                else throw new Exception();

                // Build the WGS-Info
                toWGS84.Remove(0, 8);
                toWGS84.Remove(toWGS84.Length - 2, 2);
                toWGS84.Replace(" ", "");
                string[] wgs = toWGS84.ToString().Split(',');

                Wgs84ConversionInfo wgsInfo = new Wgs84ConversionInfo(double.Parse(wgs[0], style),
                    double.Parse(wgs[1], style), double.Parse(wgs[2], style), double.Parse(wgs[3], style),
                    double.Parse(wgs[4], style), double.Parse(wgs[5], style), double.Parse(wgs[6], style));

                // Build the Ellipsoid
                spheroid.Remove(0, 10);
                spheroid.Remove(spheroid.Length - 3, 3);
                spheroid.Replace("AUTHORITY[", "");
                spheroid.Replace(" ", "");
                spheroid.Replace("\"", "");

                string[] elData = spheroid.ToString().Split(',');
                string auth = String.Empty;
                long authcode = 0;

                if (elData.Length > 4)
                {
                    auth = elData[3];
                    authcode = long.Parse(elData[4]);
                }


                Ellipsoid el = new Ellipsoid(double.Parse(elData[1], CI), 0, double.Parse(elData[2], style),
                    true, LinearUnit.Metre, elData[0], auth, authcode, String.Empty, abrev, String.Empty);

                return new HorizontalDatum(
                    el,
                    wgsInfo,
                    DatumType.HD_Geocentric,
                    name,
                    String.Empty,
                    0,
                    String.Empty,
                    abrev,
                    String.Empty);
            }
            catch (Exception)
            {
                return HorizontalDatum.Bessel1841;
            }
        }

        [XmlIgnore]
        public string ApplicationDirectory
        {
            get { return appDirectory; }
        }

        [XmlArray("ElementCategoryList"), XmlArrayItem("Category", typeof(string))]
        public List<String> CategoryList
        {
            get { return categoryList; }
            set { categoryList = value; }
        }

        [XmlIgnore]
        public List<String> DatumFileList { get { return datumFileList; } }

        [XmlIgnore]
        public List<String> EllipsoidFilList { get { return ellipsoidFilList; } }


        [XmlArray("SearchDirectories"), XmlArrayItem("Directory", typeof(string))]
        public List<String> SearchDirList
        {
            get { return searchDirList; }
            set { searchDirList = value; }
        }
        public double ZoomOutFactor
        {
            get { return zoomOutFactor; }
            set { zoomOutFactor = value; }
        }
        public double ZoomInFactor
        {
            get { return zoomInFactor; }
            set { zoomInFactor = value; }
        }
        public string ConfigFileName
        {
            get { return configFileName; }
        }
        public static string ProgramName
        {
            get { return programName; }
        }
        public string ExPGonLayerDescription
        {
            get { return exPGonLayerDescription; }
            set { exPGonLayerDescription = value; }
        }
        public string ExPLineLayerDescription
        {
            get { return exPLineLayerDescription; }
            set { exPLineLayerDescription = value; }

        }
        public string ExPGonLayerDBFCommentFieldName
        {
            get { return exPGonLayerDBFCommentFieldName; }
            set { exPGonLayerDBFCommentFieldName = value; }
        }
        public string ExPLineLayerDBFCommentFieldName
        {
            get { return exPLineLayerDBFCommentFieldName; }
            set { exPLineLayerDBFCommentFieldName = value; }
        }
        public string ExPntLayerDBFCommentFieldName
        {
            get { return exPntLayerDBFCommentFieldName; }
            set { exPntLayerDBFCommentFieldName = value; }
        }
        public string ExPntLayerDescription
        {
            get { return exPntLayerDescription; }
            set { exPntLayerDescription = value; }
        }

        public string DefaultProject
        {
            get { return defaultProject; }
            set { defaultProject = value; }
        }
        public bool ShowSpecialLayers
        {
            get { return showSpecialLayers; }
            set { showSpecialLayers = value; }
        }
        public ScaleDisplay ScaleDisplayMode
        {
            get { return scaleDisplayMode; }
            set { scaleDisplayMode = value; }
        }
        public string ExPntLayerFile
        {
            get { return exPntLayerFile; }
            set { exPntLayerFile = value; }
        }
        public string ExPntLayerName
        {
            get { return exPntLayerName; }
            set { exPntLayerName = value; }
        }
        public string ExPntLayerComment
        {
            get { return exPntLayerComment; }
            set { exPntLayerComment = value; }
        }
        [XmlIgnore()]
        public Color ExPntLayerPointColor
        {
            get { return exPntLayerPointColor; }
            set { exPntLayerPointColor = value; }
        }
        public int ExPntLayerPointColorARGB
        {
            get { return exPntLayerPointColor.ToArgb(); }
            set { exPntLayerPointColor = Color.FromArgb(value); }
        }
        public int ExPntLayerPointWidth
        {
            get { return exPntLayerPointWidth; }
            set { exPntLayerPointWidth = value; }
        }
        public bool ExPntLayerDisplayComments
        {
            get { return exPntLayerDisplayComments; }
            set { exPntLayerDisplayComments = value; }
        }
        public string ExPLineLayerName
        {
            get { return exPLineLayerName; }
            set { exPLineLayerName = value; }
        }
        public string ExPLineLayerFile
        {
            get { return exPLineLayerFile; }
            set { exPLineLayerFile = value; }
        }
        public string ExPLineLayerComment
        {
            get { return exPLineLayerComment; }
            set { exPLineLayerComment = value; }
        }
        [XmlIgnore()]
        public Color ExPLineLayerPointColor
        {
            get { return exPLineLayerPointColor; }
            set { exPLineLayerPointColor = value; }
        }
        public int ExPLineLayerPointColorARGB
        {
            get { return exPLineLayerPointColor.ToArgb(); }
            set { exPLineLayerPointColor = Color.FromArgb(value); }
        }
        [XmlIgnore()]
        public Color ExPLineLayerLineColor
        {
            get { return exPLineLayerLineColor; }
            set { exPLineLayerLineColor = value; }
        }
        public int ExPLineLayerLineColorARGB
        {
            get { return exPLineLayerLineColor.ToArgb(); }
            set { exPLineLayerLineColor = Color.FromArgb(value); }
        }
        public int ExPLineLayerPointWidth
        {
            get { return exPLineLayerPointWidth; }
            set { exPLineLayerPointWidth = value; }
        }
        public bool ExPLineLayerDisplayComments
        {
            get { return exPLineLayerDisplayComments; }
            set { exPLineLayerDisplayComments = value; }
        }
        public bool ExPGonLayerDisplayComments
        {
            get { return exPGonLayerDisplayComments; }
            set { exPGonLayerDisplayComments = value; }
        }
        public int ExPGonLayerPointWidth
        {
            get { return exPGonLayerPointWidth; }
            set { exPGonLayerPointWidth = value; }
        }
        public bool ExPGonLayerFill
        {
            get { return exPGonLayerFill; }
            set { exPGonLayerFill = value; }
        }
        [XmlIgnore()]
        public Color ExPGonLayerFillColor
        {
            get { return exPGonLayerFillColor; }
            set { exPGonLayerFillColor = value; }
        }
        public int ExPGonLayerFillColorARGB
        {
            get { return exPGonLayerFillColor.ToArgb(); }
            set { exPGonLayerFillColor = Color.FromArgb(value); }
        }
        [XmlIgnore()]
        public Color ExPGonLayerLineColor
        {
            get { return exPGonLayerLineColor; }
            set { exPGonLayerLineColor = value; }
        }
        public int ExPGonLayerLineColorARGB
        {
            get { return exPGonLayerLineColor.ToArgb(); }
            set { exPGonLayerLineColor = Color.FromArgb(value); }
        }
        [XmlIgnore()]
        public Color ExPGonLayerPointColor
        {
            get { return exPGonLayerPointColor; }
            set { exPGonLayerPointColor = value; }
        }
        public int ExPGonLayerPointColorARGB
        {
            get { return exPGonLayerPointColor.ToArgb(); }
            set { exPGonLayerPointColor = Color.FromArgb(value); }
        }
        public string ExPGonLayerComment
        {
            get { return exPGonLayerComment; }
            set { exPGonLayerComment = value; }
        }
        public string ExPGonLayerName
        {
            get { return exPGonLayerName; }
            set { exPGonLayerName = value; }
        }
        public string ExPGonLayerFile
        {
            get { return exPGonLayerFile; }
            set { exPGonLayerFile = value; }
        }
        public bool UseDefaultProject
        {
            get { return useDefaultProject; }
            set { useDefaultProject = value; }
        }
        public int CoordGauﬂKruegerStripe
        {
            get { return coordGauﬂKruegerStripe; }
            set { coordGauﬂKruegerStripe = value; }
        }
        public bool UseGPS
        {
            get { return useGPS; }
            set { useGPS = value; }
        }
        public string GpsLayerName
        {
            get { return gpsLayerName; }
            set { gpsLayerName = value; }
        }
        public string GpsLayerDescription
        {
            get { return gpsLayerDescription; }
            set { gpsLayerDescription = value; }
        }
        public string GpsLayerFile
        {
            get { return gpsLayerFile; }
            set { gpsLayerFile = value; }
        }
        public int GpsLayerPointWidth
        {
            get { return gpsLayerPointWidth; }
            set { gpsLayerPointWidth = value; }
        }
        [XmlIgnore()]
        public Color GpsLayerPointColor
        {
            get { return gpsLayerPointColor; }
            set { gpsLayerPointColor = value; }
        }
        public int GpsLayerPointColorARGB
        {
            get { return gpsLayerPointColor.ToArgb(); }
            set { gpsLayerPointColor = Color.FromArgb(value); }
        }

        public GpsView GpsViewMode
        {
            get { return gpsViewMode; }
            set { gpsViewMode = value; }
        }

        public int ExPGonLayerDisplayCommentMaxLength
        {
            get { return exPGonLayerDisplayCommentMaxLength; }
            set { exPGonLayerDisplayCommentMaxLength = value; }
        }
        public int ExPntLayerDisplayCommentMaxLength
        {
            get { return exPntLayerDisplayCommentMaxLength; }
            set { exPntLayerDisplayCommentMaxLength = value; }
        }
        public int ExPLineLayerDisplayCommentMaxLength
        {
            get { return exPLineLayerDisplayCommentMaxLength; }
            set { exPLineLayerDisplayCommentMaxLength = value; }
        }
        public int HttpTimeout
        {
            get { return httpTimeout; }
            set { httpTimeout = value; }
        }
        #endregion
    }
}
