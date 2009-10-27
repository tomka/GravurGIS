using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using GravurGIS.Layers.MapServer;
using GravurGIS.Topology;
using System.Drawing;
using System.Globalization;
using GravurGIS.Rendering;

namespace GravurGIS.Layers
{
    public class MapServerLayer : Layer
    {
        private string mapserverURL = "http://localhost/cgi-bin/mapserv.exe";
        private string map = "/ms4w/apps/pda/htdocs/anweiler_basic.map";
        private MapServerService service = MapServerService.WMS;
        private MapServerVersion version = MapServerVersion.v1_1_1;
        private List<ConcreteMSLayer> layers = new List<ConcreteMSLayer>();
        private ConcreteMSLayer concreteLayer = new ConcreteMSLayer();
        private Bitmap bitmap;
        private bool autohidden = false;
        private bool completeURL = true;
        private bool isUptodate = false;

        public bool IsUptodate
        {
            get { return isUptodate; }
        }

        internal ConcreteMSLayer ConcreteLayer
        {
            get { return concreteLayer; }
        }

        // Events
        public delegate void MapServerInformationUpdateDelegate();
        public event MapServerInformationUpdateDelegate MapServerInfoUpdated;

        private MapServerLayer(MapServerVersion version, MapServerService service)
        {
            this.LayerName = "WMS Layer";
            this.Description = "WMS-Server Layer";
            Visible = true;
            Changed = false;
            this.service = service;
            this.version = version;
            this._boundingBox.Right = 100;
            this._boundingBox.Top = 100;
        }

        public MapServerLayer(string mapserverURL, string map, MapServerService service,
            MapServerVersion version) : this(version, service)
        {
            completeURL = false;
            this.mapserverURL = mapserverURL;
            this.map = map;
            
            GetData();
        }
        public MapServerLayer(string url, MapServerService service, MapServerVersion version)
            : this(version, service)
        {
            this.mapserverURL = url;
            GetData();
        }

        public void GetData()
        {
            string URL;

            if (!completeURL)
                URL = mapserverURL + "?map=" + map + "&SERVICE=WMS&VERSION=1.1.1&REQUEST=GetCapabilities";
            else
                URL = this.mapserverURL;
            //string URL = "http://localhost/cgi-bin/mapserv.exe?map=/ms4w/apps/pda/htdocs/anweiler_basic.map&SERVICE=WMS&VERSION=1.1.1&REQUEST=GetCapabilities";
            HttpWebRequest httpReq;
            HttpWebResponse httpResp;
            Stream httpStream;

            try
            {
                #region Fetch and parse GetCapabilities XML
                httpReq = (HttpWebRequest)WebRequest.Create(URL);
                httpResp = (HttpWebResponse)httpReq.GetResponse();
                httpStream = httpResp.GetResponseStream();

                //byte[] data = new byte[128000];
                //int count = httpStream.Read(data, 0, data.Length);

                //ASCIIEncoding ASCII = new ASCIIEncoding();
                //string tempstr = ASCII.GetString(data, 0, count);

                int bufferSize = 2048;
                byte[] data = new byte[bufferSize];
                ASCIIEncoding ASCII = new ASCIIEncoding();
                StringBuilder tempstrB = new StringBuilder();

                while (true)
                {
                    bufferSize = httpStream.Read(data, 0, data.Length);
                    if (bufferSize > 0)
                        tempstrB.Append(ASCII.GetString(data, 0, bufferSize));
                    else
                        break;
                }
                String tempstr = tempstrB.ToString().Substring(tempstrB.ToString().IndexOf("<WMT_MS_Capabilities version=\"1.1.1\""));
                tempstrB = null; // we don't need it anymore


                double minx = 0.0, miny = 0.0, maxx = 0.0, maxy = 0.0;

                XmlParserContext xpc = new XmlParserContext(
                    null, null, "", XmlSpace.Default, System.Text.Encoding.UTF8);

                using (StreamReader sr = new StreamReader(httpStream,
                    Encoding.GetEncoding(1252)))
                {
                    XmlTextReader xtr = new XmlTextReader(tempstr, XmlNodeType.Document, xpc);
                    xtr.WhitespaceHandling = WhitespaceHandling.None;

                    while (xtr.Read())
                    {
                        if (xtr.NodeType == XmlNodeType.Element)
                        {
                            switch (xtr.Name)
                            {
                                case "WMT_MS_Capabilities":
                                    if (xtr.HasAttributes)
                                        while (xtr.MoveToNextAttribute())
                                            if (xtr.Name == "version" && xtr.Value == "1.1.1") this.version = MapServerVersion.v1_1_1;
                                    break;
                                case "Service":
                                    #region Service subtree handling
                                    while (xtr.Read()) // go through the service nodes
                                    {
                                        if (xtr.NodeType == XmlNodeType.Element)
                                        {
                                            switch (xtr.Name)
                                            {
                                                case "Name":
                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Text)
                                                        concreteLayer.Name = xtr.Value;
                                                    break;
                                                case "Title":
                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Text)
                                                        concreteLayer.Title = xtr.Value;
                                                    break;
                                                case "Abstract":
                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Text)
                                                        concreteLayer.AbstractDesc = xtr.Value;
                                                    break;
                                                case "OnlineResource":
                                                    break;
                                                case "ContactInformation":
                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Text)
                                                        concreteLayer.ContactInfo = xtr.Value;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        else
                                            // if we reach the end element of service - break this loop
                                            if (xtr.Name == "Service" && xtr.NodeType == XmlNodeType.EndElement) break;
                                    }
                                    break;
                                    #endregion
                                case "Capability":
                                    while (xtr.Read()) // go through the capability nodes
                                        if (xtr.NodeType == XmlNodeType.Element)
                                            switch (xtr.Name)
                                            {
                                                case "Request":
                                                    #region Request
                                                    while (xtr.Read()) // go through the request nodes
                                                        if (xtr.NodeType == XmlNodeType.Element)
                                                            switch (xtr.Name)
                                                            {
                                                                case "GetCapabilities":
                                                                    #region GetCapabilities
                                                                    while (xtr.Read()) // go through the GetCapabilities nodes
                                                                        if (xtr.NodeType == XmlNodeType.Element)
                                                                            switch (xtr.Name)
                                                                            {
                                                                                case "Format":
                                                                                    #region Format
                                                                                    this.concreteLayer.Capabilities.GetCapabilitiesEnabled = true;
                                                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Text)
                                                                                        if (xtr.Value == "application/vnd.ogc.wms_xml")
                                                                                            this.concreteLayer.Capabilities.GetCapabilitiesFormats
                                                                                                = this.concreteLayer.Capabilities.GetCapabilitiesFormats | MapServerHttpFormats.application__vnd_ogc_wms_xml;
                                                                                    break;
                                                                                    #endregion
                                                                                case "DCPType":
                                                                                    #region DCPType
                                                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Element && xtr.Name == "HTTP")
                                                                                        while (xtr.Read())
                                                                                            if (xtr.NodeType == XmlNodeType.Element)
                                                                                                switch (xtr.Name)
                                                                                                {
                                                                                                    case "Get":
                                                                                                        if (xtr.Read() && xtr.NodeType == XmlNodeType.Element
                                                                                                            && xtr.Name == "OnlineResource" && xtr.HasAttributes)
                                                                                                            while (xtr.MoveToNextAttribute())
                                                                                                                if (xtr.Name == "xlink:href")
                                                                                                                    concreteLayer.Capabilities.GetCapabilitiesDCType.HttpGet = xtr.Value;

                                                                                                        break;
                                                                                                    case "Post":
                                                                                                        if (xtr.Read() && xtr.NodeType == XmlNodeType.Element
                                                                                                            && xtr.Name == "OnlineResource" && xtr.HasAttributes)
                                                                                                            while (xtr.MoveToNextAttribute())
                                                                                                                if (xtr.Name == "xlink:href")
                                                                                                                    concreteLayer.Capabilities.GetCapabilitiesDCType.HttpPost = xtr.Value;
                                                                                                        break;
                                                                                                    default:
                                                                                                        break;
                                                                                                }
                                                                                            else if (xtr.NodeType == XmlNodeType.EndElement && xtr.Name == "DCPType") break;
                                                                                    break;
                                                                                    #endregion
                                                                                default:
                                                                                    break;
                                                                            }
                                                                        // if we reach the end element of GetCapabilities - break this loop
                                                                        else if (xtr.Name == "GetCapabilities"
                                                                                && xtr.NodeType == XmlNodeType.EndElement) break;
                                                                    break;
                                                                    #endregion
                                                                case "GetMap":
                                                                    #region GetMap
                                                                    while (xtr.Read()) // go through the GetCapabilities nodes
                                                                        if (xtr.NodeType == XmlNodeType.Element)
                                                                            switch (xtr.Name)
                                                                            {
                                                                                case "Format":
                                                                                    #region Format
                                                                                    this.concreteLayer.Capabilities.GetMapEnabled = true;
                                                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Text)
                                                                                        if (xtr.Value == "image/gif")
                                                                                            this.concreteLayer.Capabilities.GetMapFormats
                                                                                                = this.concreteLayer.Capabilities.GetMapFormats | MapServerHttpFormats.image__gif;
                                                                                        else if (xtr.Value == "image/png")
                                                                                            this.concreteLayer.Capabilities.GetMapFormats
                                                                                                        = this.concreteLayer.Capabilities.GetMapFormats | MapServerHttpFormats.image__png;
                                                                                        else if (xtr.Value == "image/png; mode=24bit")
                                                                                            this.concreteLayer.Capabilities.GetMapFormats
                                                                                                = this.concreteLayer.Capabilities.GetMapFormats | MapServerHttpFormats.image__png_mode24bit;
                                                                                        else if (xtr.Value == "image/jpeg")
                                                                                            this.concreteLayer.Capabilities.GetMapFormats
                                                                                                = this.concreteLayer.Capabilities.GetMapFormats | MapServerHttpFormats.image__jpeg;
                                                                                        else if (xtr.Value == "image/wbmp")
                                                                                            this.concreteLayer.Capabilities.GetMapFormats
                                                                                                = this.concreteLayer.Capabilities.GetMapFormats | MapServerHttpFormats.image__wbmp;
                                                                                        else if (xtr.Value == "image/tiff")
                                                                                            this.concreteLayer.Capabilities.GetMapFormats
                                                                                                = this.concreteLayer.Capabilities.GetMapFormats | MapServerHttpFormats.image__tiff;
                                                                                        else if (xtr.Value == "image/svg+xml")
                                                                                            this.concreteLayer.Capabilities.GetMapFormats
                                                                                                = this.concreteLayer.Capabilities.GetMapFormats | MapServerHttpFormats.image__svg_xml;
                                                                                    break;
                                                                                    #endregion
                                                                                case "DCPType":
                                                                                    #region DCPType
                                                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Element && xtr.Name == "HTTP")
                                                                                        while (xtr.Read())
                                                                                            if (xtr.NodeType == XmlNodeType.Element)
                                                                                                switch (xtr.Name)
                                                                                                {
                                                                                                    case "Get":
                                                                                                        if (xtr.Read() && xtr.NodeType == XmlNodeType.Element
                                                                                                            && xtr.Name == "OnlineResource" && xtr.HasAttributes)
                                                                                                            while (xtr.MoveToNextAttribute())
                                                                                                                if (xtr.Name == "xlink:href")
                                                                                                                    concreteLayer.Capabilities.GetMapDCType.HttpGet = xtr.Value;

                                                                                                        break;
                                                                                                    case "Post":
                                                                                                        if (xtr.Read() && xtr.NodeType == XmlNodeType.Element
                                                                                                            && xtr.Name == "OnlineResource" && xtr.HasAttributes)
                                                                                                            while (xtr.MoveToNextAttribute())
                                                                                                                if (xtr.Name == "xlink:href")
                                                                                                                    concreteLayer.Capabilities.GetMapDCType.HttpPost = xtr.Value;
                                                                                                        break;
                                                                                                    default:
                                                                                                        break;
                                                                                                }
                                                                                            else if (xtr.NodeType == XmlNodeType.EndElement && xtr.Name == "DCPType") break;
                                                                                    break;
                                                                                    #endregion
                                                                                default:
                                                                                    break;
                                                                            }
                                                                        // if we reach the end element of GetCapabilities - break this loop
                                                                        else if (xtr.Name == "GetMap"
                                                                            && xtr.NodeType == XmlNodeType.EndElement) break;
                                                                    break;
                                                                    #endregion
                                                                case "GetFeatureInfo":
                                                                    #region GetFeatureInfo
                                                                    while (xtr.Read()) // go through the GetFeatureInfo nodes
                                                                        if (xtr.NodeType == XmlNodeType.Element)
                                                                            switch (xtr.Name)
                                                                            {
                                                                                case "Format":
                                                                                    #region Format
                                                                                    this.concreteLayer.Capabilities.GetFeatureInfoEnabled = true;
                                                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Text)
                                                                                        if (xtr.Value == "text/plain")
                                                                                            this.concreteLayer.Capabilities.GetFeatureInfoFormats
                                                                                                = this.concreteLayer.Capabilities.GetFeatureInfoFormats | MapServerHttpFormats.text__plain;
                                                                                        else if (xtr.Value == "text/html")
                                                                                            this.concreteLayer.Capabilities.GetFeatureInfoFormats
                                                                                                    = this.concreteLayer.Capabilities.GetFeatureInfoFormats | MapServerHttpFormats.text__html;
                                                                                        else if (xtr.Value == "application/vnd.ogc.gml")
                                                                                            this.concreteLayer.Capabilities.GetFeatureInfoFormats
                                                                                                        = this.concreteLayer.Capabilities.GetFeatureInfoFormats | MapServerHttpFormats.application__vnd_ogc_gml;
                                                                                    break;
                                                                                    #endregion
                                                                                case "DCPType":
                                                                                    #region DCPType
                                                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Element && xtr.Name == "HTTP")
                                                                                        while (xtr.Read())
                                                                                            if (xtr.NodeType == XmlNodeType.Element)
                                                                                                switch (xtr.Name)
                                                                                                {
                                                                                                    case "Get":
                                                                                                        if (xtr.Read() && xtr.NodeType == XmlNodeType.Element
                                                                                                            && xtr.Name == "OnlineResource" && xtr.HasAttributes)
                                                                                                            while (xtr.MoveToNextAttribute())
                                                                                                                if (xtr.Name == "xlink:href")
                                                                                                                    concreteLayer.Capabilities.GetFeatureInfoDCType.HttpGet = xtr.Value;

                                                                                                        break;
                                                                                                    case "Post":
                                                                                                        if (xtr.Read() && xtr.NodeType == XmlNodeType.Element
                                                                                                            && xtr.Name == "OnlineResource" && xtr.HasAttributes)
                                                                                                            while (xtr.MoveToNextAttribute())
                                                                                                                if (xtr.Name == "xlink:href")
                                                                                                                    concreteLayer.Capabilities.GetFeatureInfoDCType.HttpPost = xtr.Value;
                                                                                                        break;
                                                                                                    default:
                                                                                                        break;
                                                                                                }
                                                                                            else if (xtr.NodeType == XmlNodeType.EndElement && xtr.Name == "DCPType") break;
                                                                                    break;
                                                                                    #endregion
                                                                                default:
                                                                                    break;
                                                                            }
                                                                        // if we reach the end element of GetCapabilities - break this loop
                                                                        else if (xtr.Name == "GetFeatureInfo"
                                                                            && xtr.NodeType == XmlNodeType.EndElement) break;
                                                                    break;
                                                                    #endregion
                                                                case "DescribeLayer":
                                                                    #region DescribeLayer
                                                                    while (xtr.Read()) // go through the DescribeLayer nodes
                                                                        if (xtr.NodeType == XmlNodeType.Element)
                                                                            switch (xtr.Name)
                                                                            {
                                                                                case "Format":
                                                                                    #region Format
                                                                                    this.concreteLayer.Capabilities.DescribeLayerEnabled = true;
                                                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Text)
                                                                                        if (xtr.Value == "text/xml")
                                                                                            this.concreteLayer.Capabilities.DescribeLayerFormats
                                                                                                = this.concreteLayer.Capabilities.DescribeLayerFormats | MapServerHttpFormats.text__xml;
                                                                                    break;
                                                                                    #endregion
                                                                                case "DCPType":
                                                                                    #region DCPType
                                                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Element && xtr.Name == "HTTP")
                                                                                        while (xtr.Read())
                                                                                            if (xtr.NodeType == XmlNodeType.Element)
                                                                                                switch (xtr.Name)
                                                                                                {
                                                                                                    case "Get":
                                                                                                        if (xtr.Read() && xtr.NodeType == XmlNodeType.Element
                                                                                                            && xtr.Name == "OnlineResource" && xtr.HasAttributes)
                                                                                                            while (xtr.MoveToNextAttribute())
                                                                                                                if (xtr.Name == "xlink:href")
                                                                                                                    concreteLayer.Capabilities.DescribeLayerDCType.HttpGet = xtr.Value;

                                                                                                        break;
                                                                                                    case "Post":
                                                                                                        if (xtr.Read() && xtr.NodeType == XmlNodeType.Element
                                                                                                            && xtr.Name == "OnlineResource" && xtr.HasAttributes)
                                                                                                            while (xtr.MoveToNextAttribute())
                                                                                                                if (xtr.Name == "xlink:href")
                                                                                                                    concreteLayer.Capabilities.DescribeLayerDCType.HttpPost = xtr.Value;
                                                                                                        break;
                                                                                                    default:
                                                                                                        break;
                                                                                                }
                                                                                            else if (xtr.NodeType == XmlNodeType.EndElement && xtr.Name == "DCPType") break;
                                                                                    break;
                                                                                    #endregion
                                                                                default:
                                                                                    break;
                                                                            }
                                                                        // if we reach the end element of GetCapabilities - break this loop
                                                                        else if (xtr.Name == "DescribeLayer"
                                                                            && xtr.NodeType == XmlNodeType.EndElement) break;
                                                                    break;
                                                                    #endregion
                                                                case "GetLegendGraphic":
                                                                    #region GetLegendGraphic
                                                                    while (xtr.Read()) // go through the GetLegendGraphic nodes
                                                                        if (xtr.NodeType == XmlNodeType.Element)
                                                                            switch (xtr.Name)
                                                                            {
                                                                                case "Format":
                                                                                    #region Format
                                                                                    this.concreteLayer.Capabilities.GetLegendGraphicEnabled = true;
                                                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Text)
                                                                                        if (xtr.Value == "image/gif")
                                                                                            this.concreteLayer.Capabilities.GetLegendGraphicFormats
                                                                                                = this.concreteLayer.Capabilities.GetLegendGraphicFormats | MapServerHttpFormats.image__gif;
                                                                                        else if (xtr.Value == "image/png")
                                                                                            this.concreteLayer.Capabilities.GetLegendGraphicFormats
                                                                                                        = this.concreteLayer.Capabilities.GetLegendGraphicFormats | MapServerHttpFormats.image__png;
                                                                                        else if (xtr.Value == "image/png; mode=24bit")
                                                                                            this.concreteLayer.Capabilities.GetLegendGraphicFormats
                                                                                                = this.concreteLayer.Capabilities.GetLegendGraphicFormats | MapServerHttpFormats.image__png_mode24bit;
                                                                                        else if (xtr.Value == "image/jpeg")
                                                                                            this.concreteLayer.Capabilities.GetLegendGraphicFormats
                                                                                                = this.concreteLayer.Capabilities.GetLegendGraphicFormats | MapServerHttpFormats.image__jpeg;
                                                                                        else if (xtr.Value == "image/wbmp")
                                                                                            this.concreteLayer.Capabilities.GetLegendGraphicFormats
                                                                                                = this.concreteLayer.Capabilities.GetLegendGraphicFormats | MapServerHttpFormats.image__wbmp;
                                                                                    break;
                                                                                    #endregion
                                                                                case "DCPType":
                                                                                    #region DCPType
                                                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Element && xtr.Name == "HTTP")
                                                                                        while (xtr.Read())
                                                                                            if (xtr.NodeType == XmlNodeType.Element)
                                                                                                switch (xtr.Name)
                                                                                                {
                                                                                                    case "Get":
                                                                                                        if (xtr.Read() && xtr.NodeType == XmlNodeType.Element
                                                                                                            && xtr.Name == "OnlineResource" && xtr.HasAttributes)
                                                                                                            while (xtr.MoveToNextAttribute())
                                                                                                                if (xtr.Name == "xlink:href")
                                                                                                                    concreteLayer.Capabilities.GetLegendGraphicDCType.HttpGet = xtr.Value;

                                                                                                        break;
                                                                                                    case "Post":
                                                                                                        if (xtr.Read() && xtr.NodeType == XmlNodeType.Element
                                                                                                            && xtr.Name == "OnlineResource" && xtr.HasAttributes)
                                                                                                            while (xtr.MoveToNextAttribute())
                                                                                                                if (xtr.Name == "xlink:href")
                                                                                                                    concreteLayer.Capabilities.GetLegendGraphicDCType.HttpPost = xtr.Value;
                                                                                                        break;
                                                                                                    default:
                                                                                                        break;
                                                                                                }
                                                                                            else if (xtr.NodeType == XmlNodeType.EndElement && xtr.Name == "DCPType") break;
                                                                                    break;
                                                                                    #endregion
                                                                                default:
                                                                                    break;
                                                                            }
                                                                        // if we reach the end element of GetLegendGraphic - break this loop
                                                                        else if (xtr.Name == "GetLegendGraphic"
                                                                            && xtr.NodeType == XmlNodeType.EndElement) break;
                                                                    break;
                                                                    #endregion
                                                                case "GetStyles":
                                                                    #region GetStyles
                                                                    while (xtr.Read()) // go through the GetStyles nodes
                                                                        if (xtr.NodeType == XmlNodeType.Element)
                                                                            switch (xtr.Name)
                                                                            {
                                                                                case "Format":
                                                                                    #region Format
                                                                                    this.concreteLayer.Capabilities.GetStylesEnabled = true;
                                                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Text)
                                                                                        if (xtr.Value == "text/xml")
                                                                                            this.concreteLayer.Capabilities.GetStylesFormats
                                                                                                = this.concreteLayer.Capabilities.GetStylesFormats | MapServerHttpFormats.text__xml;
                                                                                    break;
                                                                                    #endregion
                                                                                case "DCPType":
                                                                                    #region DCPType
                                                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Element && xtr.Name == "HTTP")
                                                                                        while (xtr.Read())
                                                                                            if (xtr.NodeType == XmlNodeType.Element)
                                                                                                switch (xtr.Name)
                                                                                                {
                                                                                                    case "Get":
                                                                                                        if (xtr.Read() && xtr.NodeType == XmlNodeType.Element
                                                                                                            && xtr.Name == "OnlineResource" && xtr.HasAttributes)
                                                                                                            while (xtr.MoveToNextAttribute())
                                                                                                                if (xtr.Name == "xlink:href")
                                                                                                                    concreteLayer.Capabilities.GetStylesDCType.HttpGet = xtr.Value;

                                                                                                        break;
                                                                                                    case "Post":
                                                                                                        if (xtr.Read() && xtr.NodeType == XmlNodeType.Element
                                                                                                            && xtr.Name == "OnlineResource" && xtr.HasAttributes)
                                                                                                            while (xtr.MoveToNextAttribute())
                                                                                                                if (xtr.Name == "xlink:href")
                                                                                                                    concreteLayer.Capabilities.GetStylesDCType.HttpPost = xtr.Value;
                                                                                                        break;
                                                                                                    default:
                                                                                                        break;
                                                                                                }
                                                                                            else if (xtr.NodeType == XmlNodeType.EndElement && xtr.Name == "DCPType") break;
                                                                                    break;
                                                                                    #endregion
                                                                                default:
                                                                                    break;
                                                                            }
                                                                        // if we reach the end element of GetStyles - break this loop
                                                                        else if (xtr.Name == "GetStyles"
                                                                            && xtr.NodeType == XmlNodeType.EndElement) break;
                                                                    break;
                                                                    #endregion
                                                                default:
                                                                    break;
                                                            }
                                                        // if we reach the end element of Request - break this loop
                                                        else if (xtr.Name == "Request" && xtr.NodeType == XmlNodeType.EndElement) break;
                                                    break;
                                                    #endregion
                                                case "Exception":
                                                    #region Exception
                                                    while (xtr.Read()) // go through the Exception nodes
                                                        if (xtr.NodeType == XmlNodeType.Element)
                                                            switch (xtr.Name)
                                                            {
                                                                case "Format":
                                                                    #region Format
                                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Text)
                                                                        if (xtr.Value == "application/vnd.ogc.se_xml")
                                                                            this.concreteLayer.ExceptionFormats
                                                                                = this.concreteLayer.ExceptionFormats | MapServerHttpFormats.application__vnd_ogc_se_xml;
                                                                        else if (xtr.Value == "application/vnd.ogc.se_inimage")
                                                                            this.concreteLayer.ExceptionFormats
                                                                                        = this.concreteLayer.ExceptionFormats | MapServerHttpFormats.application__vnd_ogc_se_inimage;
                                                                        else if (xtr.Value == "application/vnd.ogc.se_blank")
                                                                            this.concreteLayer.ExceptionFormats
                                                                                = this.concreteLayer.ExceptionFormats | MapServerHttpFormats.application__vnd_ogc_se_blank;
                                                                    break;
                                                                    #endregion
                                                                default:
                                                                    break;
                                                            }
                                                        // if we reach the end element of Exception - break this loop
                                                        else if (xtr.Name == "Exception"
                                                            && xtr.NodeType == XmlNodeType.EndElement) break;
                                                    break;
                                                    #endregion
                                                case "UserDefinedSymbolization":
                                                    #region UserDefinedSymbolization
                                                    if (xtr.HasAttributes)
                                                        while (xtr.MoveToNextAttribute())
                                                            if (xtr.Name == "SupportSLD" && xtr.Value == "1")
                                                                this.concreteLayer.UserDefinedSymbolization = this.concreteLayer.UserDefinedSymbolization | MapServerUserDefinedSymbolization.SupportSLD;
                                                            else if (xtr.Name == "UserLayer" && xtr.Value == "1")
                                                                this.concreteLayer.UserDefinedSymbolization = this.concreteLayer.UserDefinedSymbolization | MapServerUserDefinedSymbolization.UserLayer;
                                                            else if (xtr.Name == "UserStyle" && xtr.Value == "1")
                                                                this.concreteLayer.UserDefinedSymbolization = this.concreteLayer.UserDefinedSymbolization | MapServerUserDefinedSymbolization.UserStyle;
                                                            else if (xtr.Name == "RemoteWFS" && xtr.Value == "1")
                                                                this.concreteLayer.UserDefinedSymbolization = this.concreteLayer.UserDefinedSymbolization | MapServerUserDefinedSymbolization.RemoteWFS;
                                                    break;
                                                    #endregion
                                                case "Layer":
                                                    #region Layer
                                                    while (xtr.Read()) // go through the Layer nodes
                                                    {
                                                        if (xtr.NodeType == XmlNodeType.Element)
                                                        {
                                                            switch (xtr.Name)
                                                            {
                                                                case "Name":
                                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Text)
                                                                        concreteLayer.LayerManager.Name = xtr.Value;
                                                                    break;
                                                                case "Title":
                                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Text)
                                                                        concreteLayer.LayerManager.Title = xtr.Value;
                                                                    break;
                                                                case "SRS":
                                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Text)
                                                                        concreteLayer.LayerManager.Srs = xtr.Value;
                                                                    break;
                                                                case "LatLonBoundingBox":
                                                                    #region LatLonBoundingBox
                                                                    if (xtr.HasAttributes)
                                                                    {
                                                                        while (xtr.MoveToNextAttribute())
                                                                        {
                                                                            if (xtr.Name == "minx")
                                                                            {
                                                                                try { minx = Convert.ToDouble(xtr.Value, CultureInfo.InvariantCulture); }
                                                                                catch { minx = 0.0; }
                                                                            }
                                                                            else if (xtr.Name == "miny")
                                                                            {
                                                                                try { miny = Convert.ToDouble(xtr.Value, CultureInfo.InvariantCulture); }
                                                                                catch { miny = 0.0; }
                                                                            }
                                                                            else if (xtr.Name == "maxx")
                                                                            {
                                                                                try { maxx = Convert.ToDouble(xtr.Value, CultureInfo.InvariantCulture); }
                                                                                catch { maxx = 0.0; }
                                                                            }
                                                                            else if (xtr.Name == "maxy")
                                                                            {
                                                                                try { maxy = Convert.ToDouble(xtr.Value, CultureInfo.InvariantCulture); }
                                                                                catch { maxy = 0.0; }
                                                                            }
                                                                        }
                                                                    }

                                                                    this.concreteLayer.LayerManager.LatLonBoundingBox = new GravurGIS.Topology.DRect(minx, maxy, maxx, miny);
                                                                    // copy bounding box values to our ILayer implementation
                                                                    if (this.concreteLayer.LayerManager.BoundingBoxSRS == "Missing")
                                                                    {
                                                                        this._boundingBox.Left = this.concreteLayer.LayerManager.LatLonBoundingBox.Left;
                                                                        this._boundingBox.Bottom = this.concreteLayer.LayerManager.LatLonBoundingBox.Bottom;
                                                                        this._boundingBox.Right = this.concreteLayer.LayerManager.LatLonBoundingBox.Right;
                                                                        this._boundingBox.Top = this.concreteLayer.LayerManager.LatLonBoundingBox.Top;
                                                                    }

                                                                    break;
                                                                    #endregion
                                                                case "BoundingBox":
                                                                    #region BoudingBox
                                                                    if (xtr.HasAttributes)
                                                                    {
                                                                        while (xtr.MoveToNextAttribute())
                                                                        {
                                                                            if (xtr.Name == "SRS")
                                                                            {
                                                                                try { this.concreteLayer.LayerManager.BoundingBoxSRS = xtr.Value; }
                                                                                catch { this.concreteLayer.LayerManager.BoundingBoxSRS = "Missing"; }
                                                                            }
                                                                            else if (xtr.Name == "minx")
                                                                            {
                                                                                try { minx = Convert.ToDouble(xtr.Value, CultureInfo.InvariantCulture); } // e.g. 2.63293e+006
                                                                                catch { minx = 0.0; }
                                                                            }
                                                                            else if (xtr.Name == "miny")
                                                                            {
                                                                                try { miny = Convert.ToDouble(xtr.Value, CultureInfo.InvariantCulture); }
                                                                                catch { miny = 0.0; }
                                                                            }
                                                                            else if (xtr.Name == "maxx")
                                                                            {
                                                                                try { maxx = Convert.ToDouble(xtr.Value, CultureInfo.InvariantCulture); }
                                                                                catch { maxx = 0.0; }
                                                                            }
                                                                            else if (xtr.Name == "maxy")
                                                                            {
                                                                                try { maxy = Convert.ToDouble(xtr.Value, CultureInfo.InvariantCulture); }
                                                                                catch { maxy = 0.0; }
                                                                            }
                                                                        }
                                                                    }

                                                                    this.concreteLayer.LayerManager.BoundingBox = new DRect(minx, maxy, maxx, miny);
                                                                    // copy bounding box values to our ILayer implementation
                                                                    this._boundingBox.Left = this.concreteLayer.LayerManager.BoundingBox.Left;
                                                                    this._boundingBox.Bottom = this.concreteLayer.LayerManager.BoundingBox.Bottom;
                                                                    this._boundingBox.Right = this.concreteLayer.LayerManager.BoundingBox.Right;
                                                                    this._boundingBox.Top = this.concreteLayer.LayerManager.BoundingBox.Top;

                                                                    break;
                                                                    #endregion
                                                                case "Layer":
                                                                    #region Layer
                                                                    MSLayer newLayer = new MSLayer();

                                                                    // parse the attributes
                                                                    if (xtr.HasAttributes)
                                                                        while (xtr.MoveToNextAttribute())
                                                                        {
                                                                            if (xtr.Name == "queryable")
                                                                            {
                                                                                try { newLayer.Queryable = Convert.ToInt32(xtr.Value); }
                                                                                catch { newLayer.Queryable = 0; }
                                                                            }
                                                                            else if (xtr.Name == "opaque")
                                                                            {
                                                                                try { newLayer.Opaque = Convert.ToInt32(xtr.Value); }
                                                                                catch { newLayer.Opaque = 0; }
                                                                            }
                                                                            else if (xtr.Name == "cascaded")
                                                                            {
                                                                                try { newLayer.Cascaded = Convert.ToInt32(xtr.Value); }
                                                                                catch { newLayer.Cascaded = 0; }
                                                                            }
                                                                        }

                                                                    // parse the subtree
                                                                    while (xtr.Read())
                                                                    {
                                                                        if (xtr.NodeType == XmlNodeType.Element)
                                                                        {
                                                                            switch (xtr.Name)
                                                                            {
                                                                                case "Name":
                                                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Text)
                                                                                        newLayer.Name = xtr.Value;
                                                                                    break;
                                                                                case "Title":
                                                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Text)
                                                                                        newLayer.Title = xtr.Value;
                                                                                    break;
                                                                                case "SRS":
                                                                                    if (xtr.Read() && xtr.NodeType == XmlNodeType.Text)
                                                                                        newLayer.Srs = xtr.Value;
                                                                                    break;
                                                                                case "LatLonBoundingBox":
                                                                                    #region LatLonBoundingBox
                                                                                    if (xtr.HasAttributes)
                                                                                    {
                                                                                        while (xtr.MoveToNextAttribute())
                                                                                        {
                                                                                            if (xtr.Name == "minx")
                                                                                            {
                                                                                                try { minx = Convert.ToDouble(xtr.Value, CultureInfo.InvariantCulture); }
                                                                                                catch { minx = 0.0; }
                                                                                            }
                                                                                            else if (xtr.Name == "miny")
                                                                                            {
                                                                                                try { miny = Convert.ToDouble(xtr.Value, CultureInfo.InvariantCulture); }
                                                                                                catch { miny = 0.0; }
                                                                                            }
                                                                                            else if (xtr.Name == "maxx")
                                                                                            {
                                                                                                try { maxx = Convert.ToDouble(xtr.Value, CultureInfo.InvariantCulture); }
                                                                                                catch { maxx = 0.0; }
                                                                                            }
                                                                                            else if (xtr.Name == "maxy")
                                                                                            {
                                                                                                try { maxy = Convert.ToDouble(xtr.Value, CultureInfo.InvariantCulture); }
                                                                                                catch { maxy = 0.0; }
                                                                                            }
                                                                                        }
                                                                                    }

                                                                                    newLayer.LatLonBoundingBox = new GravurGIS.Topology.DRect(minx, maxy, maxx, miny);
                                                                                    break;
                                                                                    #endregion
                                                                                case "BoundingBox":
                                                                                    #region BoudingBox
                                                                                    if (xtr.HasAttributes)
                                                                                    {
                                                                                        while (xtr.MoveToNextAttribute())
                                                                                        {
                                                                                            if (xtr.Name == "SRS")
                                                                                            {
                                                                                                try { newLayer.BoundingBoxSRS = xtr.Value; }
                                                                                                catch { newLayer.BoundingBoxSRS = "Missing"; }
                                                                                            }
                                                                                            else if (xtr.Name == "minx")
                                                                                            {
                                                                                                try { minx = Convert.ToDouble(xtr.Value, CultureInfo.InvariantCulture); }
                                                                                                catch { minx = 0.0; }
                                                                                            }
                                                                                            else if (xtr.Name == "miny")
                                                                                            {
                                                                                                try { miny = Convert.ToDouble(xtr.Value, CultureInfo.InvariantCulture); }
                                                                                                catch { miny = 0.0; }
                                                                                            }
                                                                                            else if (xtr.Name == "maxx")
                                                                                            {
                                                                                                try { maxx = Convert.ToDouble(xtr.Value, CultureInfo.InvariantCulture); }
                                                                                                catch { maxx = 0.0; }
                                                                                            }
                                                                                            else if (xtr.Name == "maxy")
                                                                                            {
                                                                                                try { maxy = Convert.ToDouble(xtr.Value, CultureInfo.InvariantCulture); }
                                                                                                catch { maxy = 0.0; }
                                                                                            }
                                                                                        }
                                                                                    }

                                                                                    newLayer.BoundingBox = new DRect(minx, maxy, maxx, miny);
                                                                                    break;
                                                                                    #endregion
                                                                                case "Style":
                                                                                    xtr.Skip();
                                                                                    break;
                                                                                default:
                                                                                    break;
                                                                            }
                                                                        }
                                                                        if (xtr.NodeType == XmlNodeType.EndElement && xtr.Name == "Layer") break;
                                                                    }

                                                                    if (!(this.concreteLayer.LayerManager.addLayer(newLayer)))
                                                                        MessageBox.Show("Fehler 0x9100: Konnte keinen neuen MapServer-Layer hinzufügen");

                                                                    break;
                                                                    #endregion
                                                                default:
                                                                    break;
                                                            }
                                                        }
                                                        else
                                                            // if we reach the end element of service - break this loop
                                                            if (xtr.Name == "Layer" && xtr.NodeType == XmlNodeType.EndElement) break;
                                                    }
                                                    break;
                                                    #endregion
                                                default:
                                                    break;
                                            }
                                        // if we reach the end element of Request - break this loop
                                        else if (xtr.Name == "Capability" && xtr.NodeType == XmlNodeType.EndElement) break;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    if (MapServerInfoUpdated != null) MapServerInfoUpdated();
                }

                isUptodate = true;
                #endregion
            }
            catch (WebException we)
            {
                MessageBox.Show(String.Format("Der Vorgang kann nicht durchgeführt werden da es ein Problem mit der Verbindung gibt:{0}{1}",
                    Environment.NewLine, we.Message), "Fehler", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format("Fehler beim Bearbeiten der XML-Struktur:{0}{1}",
                    Environment.NewLine, e.Message), "Fehler", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }

            isUptodate = false;
        }

        #region ILayer Members

        public override bool Render(RenderProperties rp)
        {
            if (rp.ScreenChanged || autohidden)
                updateImageFromUri(rp.DX, rp.DY, rp.AbsoluteZoom, rp.ScreenChanged, rp.DrawingArea, rp.Scale);

            if (!autohidden)
            {
                rp.G.DrawImage(
                    bitmap,
                    rp.DrawingArea.X,
                    rp.DrawingArea.Y);
            }
            return true;
        }

        private void updateImageFromUri(double dX, double dY,
            double absoluteZoom, bool screenChanged, Rectangle drawingArea, double scale)
        {
            /*
             * example uri:
             * http://localhost/cgi-bin/mapserv.exe?
             * map=/ms4w/apps/pda/htdocs/anweiler_basic.map
             * &SERVICE=WMS
             * &VERSION=1.1.1
             * &REQUEST=GetMap
             * &SRS=EPSG:31466
             * &BBOX=2632930,5451203,2647012,5462469
             * &layers=flpo_export
             * &Format=image/gif
             * &width=300
             * &height=300
             * &Styles=
            */

            double xmin = dX / scale;
            double ymin = (dY - drawingArea.Height) / scale;
            double xmax = (dX + drawingArea.Width) / scale;
            double ymax = dY / scale;


            StringBuilder URL = new StringBuilder(mapserverURL);

            if (!completeURL)
            {
                URL.Append("?map=");
                URL.Append(map);
            }


            URL.Append("&SERVICE=WMS&VERSION=");

            HttpWebRequest httpReq;
            HttpWebResponse httpResp;
            Stream httpStream = null;
            MemoryStream ms = null;

            //httpReq.Credentials = new NetworkCredential("UserName", "P@ssW0rd");
            //httpReq.PreAuthenticate = true;
            try
            {

                if (this.version == MapServerVersion.v1_1_1) URL.Append("1.1.1");
                else throw new Exception("Unsupported WMS-Server Version chosen");

                URL.Append("&REQUEST=GetMap&SRS=");
                URL.Append(this.concreteLayer.LayerManager.Srs);
                URL.Append("&BBOX=");
                URL.Append(xmin.ToString(CultureInfo.InvariantCulture)); // CultureInfo.InvariantCulture is needed since we want a "." and not a "," as decimal point
                URL.Append(",");
                URL.Append(ymin.ToString(CultureInfo.InvariantCulture));
                URL.Append(",");
                URL.Append(xmax.ToString(CultureInfo.InvariantCulture));
                URL.Append(",");
                URL.Append(ymax.ToString(CultureInfo.InvariantCulture));
                URL.Append("&layers=");

                bool layersFound = false;
                for (int i = this.concreteLayer.LayerManager.LayerCount - 1; i >= 0; i--)
                {
                    MSLayer currentLayer = this.concreteLayer.LayerManager.getLayer(i);
                    if (currentLayer.Visible)
                    {
                        if (!layersFound) layersFound = true;
                        URL.Append(currentLayer.Name);
                        URL.Append(",");
                    }
                }
                if (layersFound) URL.Remove(URL.Length - 1, 1);

                URL.Append("&Format=image/gif&width=");
                URL.Append(drawingArea.Width.ToString());
                URL.Append("&height=");
                URL.Append(drawingArea.Height.ToString());
                URL.Append("&Styles=");


                httpReq = (HttpWebRequest)WebRequest.Create(URL.ToString());
                httpReq.Timeout = 3000;

                httpResp = (HttpWebResponse)httpReq.GetResponse();
                httpStream = httpResp.GetResponseStream();
                int bufferSize = 2048;
                byte[] data = new byte[bufferSize];

                ms = new MemoryStream();
                while (true)
                {
                    bufferSize = httpStream.Read(data, 0, data.Length);
                    if (bufferSize > 0)
                        ms.Write(data, 0, bufferSize);
                    else
                        break;
                }
                ms.Position = 0; // Reset Position

                if (bitmap != null) bitmap.Dispose();

                if (httpResp.ContentType == "image/gif" || httpResp.ContentType == "image/png")
                {
                    bitmap = new Bitmap(ms);
                    if (autohidden) autohidden = false;
                }
                else
                {
                    using (StreamReader reader = new StreamReader(ms))
                    {
                        autohidden = true;
                        Visible = false;
                        (new GravurGIS.GUI.Dialogs.ErrorDialog()).ShowDialog(
                            String.Format("Fehler 0x9342: MapServer antwortete unerwarted:{0}{0}Anfrage-URL: {1}{0}{0}Antwort ({2}):{0}{3}",
                            Environment.NewLine, URL, httpResp.ContentType, reader.ReadToEnd())
                            );
                    }
                }
            }
            catch (WebException we)
            {
                if (!autohidden)
                {
                    if (we.Status == WebExceptionStatus.Timeout)
                        MessageBox.Show(
                            "Der MapServer antwortet leider nicht (mehr).\nSobald wieder eine Verbindung besteht wird wieder ein Bild gezeigt.");
                }
                if (bitmap != null) bitmap.Dispose();
                autohidden = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format("Es trat ein Fehler im WMS-Dienst Layer auf:{0}{1}", Environment
                    .NewLine, e.Message), "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                if (bitmap != null) bitmap.Dispose();
                autohidden = true;
            }
            finally
            {
                if (ms != null) ms.Close();
                if (httpStream != null) httpStream.Close();
            }
        }

        public override void reset()
        {
            // nothing to do here
        }

        public override void recalculateData(double absoluteZoom, double scale, double xOff, double yOff)
        {
            // nothing to do here
        }

        public string Map
        {
            get { return map; }
            set { map = value; }
        }
        public string MapserverURL
        {
            get { return mapserverURL; }
            set { mapserverURL = value; }
        }

        #endregion
    }
}

namespace GravurGIS.Layers.MapServer
{
    public enum MapServerService { WMS }
    public enum MapServerVersion { v1_1_1 }


    [FlagsAttribute]
    public enum MapServerHttpFormats
    {
        none = 0,
        image__gif = 1,
        image__png = 2,
        image__png_mode24bit = 4,
        image__jpeg = 8,
        image__wbmp = 16,
        image__tiff = 32,
        image__svg_xml = 64,
        application__vnd_ogc_wms_xml = 128,
        application__vnd_ogc_gml = 256,
        application__vnd_ogc_se_xml = 512,
        application__vnd_ogc_se_inimage = 1024,
        application__vnd_ogc_se_blank = 2048,
        text__plain = 4096,
        text__html = 8192,
        text__xml = 16384,
    }

    [FlagsAttribute]
    public enum MapServerUserDefinedSymbolization
    {
        None = 0,
        SupportSLD = 1,
        UserLayer = 2,
        UserStyle = 4,
        RemoteWFS = 8
    }

    class DCType
    {
        private string httpPost = "";
        public string HttpPost
        {
            get { return httpPost; }
            set { httpPost = value; }
        }
        private string httpGet = "";
        public string HttpGet
        {
            get { return httpGet; }
            set { httpGet = value; }
        }
    }

    class MSCapabilities
    {
        private bool getCapabilitiesEnabled = false;
        private DCType getCapabilitiesDCType = new DCType();
        private MapServerHttpFormats getCapabilitiesFormats = MapServerHttpFormats.none;

        private bool getMapEnabled = false;
        private DCType getMapDCType = new DCType();
        private MapServerHttpFormats getMapFormats = MapServerHttpFormats.none;

        private bool getFeatureInfoEnabled = false;
        private DCType getFeatureInfoDCType = new DCType();
        private MapServerHttpFormats getFeatureInfoFormats = MapServerHttpFormats.none;

        private bool describeLayerEnabled = false;
        private DCType describeLayerDCType = new DCType();
        private MapServerHttpFormats describeLayerFormats = MapServerHttpFormats.none;

        private bool getLegendGraphicEnabled = false;
        private DCType getLegendGraphicDCType = new DCType();
        private MapServerHttpFormats getLegendGraphicFormats = MapServerHttpFormats.none;

        private bool getStylesEnabled = false;
        private DCType getStylesDCType = new DCType();
        private MapServerHttpFormats getStylesFormats = MapServerHttpFormats.none;

        #region Getter/Setter

        public bool GetCapabilitiesEnabled { get { return getCapabilitiesEnabled; } set { getCapabilitiesEnabled = value; } }

        public DCType GetCapabilitiesDCType { get { return getCapabilitiesDCType; } set { getCapabilitiesDCType = value; } }

        public MapServerHttpFormats GetCapabilitiesFormats { get { return getCapabilitiesFormats; } set { getCapabilitiesFormats = value; } }

        public bool GetMapEnabled { get { return getMapEnabled; } set { getMapEnabled = value; } }

        public DCType GetMapDCType { get { return getMapDCType; } set { getMapDCType = value; } }

        public MapServerHttpFormats GetMapFormats { get { return getMapFormats; } set { getMapFormats = value; } }

        public bool GetFeatureInfoEnabled { get { return getFeatureInfoEnabled; } set { getFeatureInfoEnabled = value; } }

        public DCType GetFeatureInfoDCType { get { return getFeatureInfoDCType; } set { getFeatureInfoDCType = value; } }

        public MapServerHttpFormats GetFeatureInfoFormats { get { return getFeatureInfoFormats; } set { getFeatureInfoFormats = value; } }

        public bool DescribeLayerEnabled { get { return describeLayerEnabled; } set { describeLayerEnabled = value; } }

        public DCType DescribeLayerDCType { get { return describeLayerDCType; } set { describeLayerDCType = value; } }

        public MapServerHttpFormats DescribeLayerFormats { get { return describeLayerFormats; } set { describeLayerFormats = value; } }

        public bool GetLegendGraphicEnabled { get { return getLegendGraphicEnabled; } set { getLegendGraphicEnabled = value; } }

        public DCType GetLegendGraphicDCType { get { return getLegendGraphicDCType; } set { getLegendGraphicDCType = value; } }

        public MapServerHttpFormats GetLegendGraphicFormats { get { return getLegendGraphicFormats; } set { getLegendGraphicFormats = value; } }

        public bool GetStylesEnabled { get { return getStylesEnabled; } set { getStylesEnabled = value; } }

        public DCType GetStylesDCType { get { return getStylesDCType; } set { getStylesDCType = value; } }

        public MapServerHttpFormats GetStylesFormats { get { return getStylesFormats; } set { getStylesFormats = value; } }

        #endregion
    }

    class MSLayer
    {
        private int queryable = 0;
        public int Queryable
        {
            get { return queryable; }
            set { queryable = value; }
        }
        int opaque = 0;
        public int Opaque
        {
            get { return opaque; }
            set { opaque = value; }
        }
        int cascaded = 0;
        public int Cascaded
        {
            get { return cascaded; }
            set { cascaded = value; }
        }
        string title = "MapServer-Layer";
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        private string name = "MapServer-Layer";
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        string srs = "Missing";
        public string Srs
        {
            get { return srs; }
            set { srs = value; }
        }
        private DRect latLonBoundingBox = new DRect();
        public DRect LatLonBoundingBox
        {
            get { return latLonBoundingBox; }
            set { latLonBoundingBox = value; }
        }
        private string bbSRS = "";
        public string BoundingBoxSRS
        {
            get { return bbSRS; }
            set { bbSRS = value; }
        }
        
        private DRect boundingBox = new DRect();
        public DRect BoundingBox
        {
            get { return boundingBox; }
            set { boundingBox = value; }
        }

        private bool visible = true;

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }
    }

    class MSLayerManager
    {
        private string name;
        private string title;
        private string srs = "Missing";
        private DRect latLonBoundingBox = new DRect();
        private SortedList<int, MSLayer> layers = new SortedList<int,MSLayer>();
        private string bbSRS = "Missing";
        private DRect boundingBox = new DRect();
        
        public bool addLayer(MSLayer newLayer)
        {
            try
            {
                layers.Add(LayerCount, newLayer);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void switchLayerVisibility(int index)
        {
            Boolean state = true;
            if (layers[index].Visible)
                state = false;
            layers[index].Visible = state;
        }

        public MSLayer getLayer(int index)
        {
            if (index >= 0 && index < LayerCount)
                return layers[index];
            else
                throw new ArgumentOutOfRangeException("index");
        }

        public void moveLayerDownOneStep(int index)
        {
            MSLayer selectedLayer = layers[index]; // the associated index in the shapeFile-List
            layers[index] = layers[index + 1];
            layers[index + 1] = selectedLayer;
        }

        public void moveLayerUpOneStep(int index)
        {
            MSLayer selectedLayer = layers[index]; // the associated index in the shapeFile-List
            layers[index] = layers[index - 1];
            layers[index - 1] = selectedLayer;
        }

        public int LayerCount
        {
            get { return layers.Count; }
        }

        #region Getter/Setter
        public string Name { get { return name; } set { name = value; } }

        public string Title { get { return title; } set { title = value; } }

        public string Srs { get { return srs; } set { srs = value; } }

        public GravurGIS.Topology.DRect LatLonBoundingBox { get { return latLonBoundingBox; } set { latLonBoundingBox = value; } }

        public DRect BoundingBox
        {
            get { return boundingBox; }
            set { boundingBox = value; }
        }
        public string BoundingBoxSRS
        {
            get { return bbSRS; }
            set { bbSRS = value; }
        }
        #endregion
    }

    class ConcreteMSLayer
    {
        private MapServerUserDefinedSymbolization userDefinedSymbolization = MapServerUserDefinedSymbolization.None;
        public MapServerUserDefinedSymbolization UserDefinedSymbolization
        {
            get { return userDefinedSymbolization; }
            set { userDefinedSymbolization = value; }
        }

        private MapServerHttpFormats exceptionFormats = MapServerHttpFormats.none;
        public MapServerHttpFormats ExceptionFormats
        {
            get { return exceptionFormats; }
            set { exceptionFormats = value; }
        }

        private MSCapabilities capabilities = new MSCapabilities();
        public MSCapabilities Capabilities
        {
            get { return capabilities; }
            set { capabilities = value; }
        }

        private MSLayerManager layerManager = new MSLayerManager();
        public MSLayerManager LayerManager
        {
            get { return layerManager; }
            set { layerManager = value; }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string abstractDesc;
        public string AbstractDesc
        {
            get { return abstractDesc; }
            set { abstractDesc = value; }
        }

        private string contactInfo;
        public string ContactInfo
        {
            get { return contactInfo; }
            set { contactInfo = value; }
        }

        private bool visivle;
        public bool Visivle
        {
            get { return visivle; }
            set { visivle = value; }
        }
    }
}
