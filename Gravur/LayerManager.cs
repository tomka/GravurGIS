//#define DRAW_BOUNDINGBOXES
//#define DRAW_QUADTREE

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using GravurGIS.Actions;
using GravurGIS.GUI.Dialogs;
using GravurGIS.Layers;
using GravurGIS.Shapes;
using GravurGIS.Styles.NewLayerStyle;
using GravurGIS.Topology;
using GravurGIS.Topology.QuadTree;
using MapTools;
using System.Collections.Generic;

namespace GravurGIS
{
    public struct ShapeInformation
    {
        public QuadTreePositionItem<IShape> quadTreePosItemInf;
        public IShape iShapeInf;
    }

    public enum DrawShapeInformation { EditStarted, EditStoppedUndone, EditStopped };

    public struct ShapeBBInformation
    {
        public Rectangle BoundingBox;
        public IShape Shape;
    }

    public struct PolyShapeBBInformation
    {
        public Point[] Pointlist;
        public Rectangle BoundingBox;
        public IShape Shape;
    }
    public class LayerManager
    {
        private List<Layer> layerArray;
        private QuadTree<IShape> transportLayerQuadtree;

        // < ListBoxIndex, ShapeArrayIndex>

        private SortedList<int, int> layerListViewMappingList;
        private SortedList<int, int> layerShapeMappingList; // <layerArray, shapeArray>
        private MainControler mainControler;
        private double scale = 1.0d;
        private double firstScale = 1.0d;
        private double bbMaxX = 0.0d;
        private double bbMaxY = 0.0d;
        private double bbDoubleTempX;
        private double bbDoubleTempY;
        private byte refLayer = 0;
        private TransportMultiPointLayer transportPointLayer;
        private TransportPolylineLayer   transportPolylineLayer;
        private TransportPolygonLayer transportPolygonLayer;
        private GPSLayer gpsLayer;
        private QuadTreePositionItem<IShape> selectedTransportQuadtreeItem = null;
        private List<QuadTreePositionItem<IShape>> currentlyVisibleIShapes;
        private IShape currentlyEditedIShape = null;
        private double dX;
        private double dY;
        private IntPtr cGDALContainer = IntPtr.Zero;
        private IntPtr cOGRContainer = IntPtr.Zero;

        //sizes if shp consists of only one point
        private int pointSize = 4;
        private int defaultShapeSize = 1000; //unscaled shpsize
        private double dFirstMapMeassure;
        private Config config;
        private GravurGIS.Styles.NewLayerStyle.INewLayerStyle newLayerStyle = null;
   
        ////////////////
        // Events
        ////////////////

        public delegate void LayerAddedDelegate(Layer newShapeObject);
        public event LayerAddedDelegate LayerAdded;
        public event LayerAddedDelegate FirstLayerAdded;

        public delegate void LastLayerRemovedDelegate();
        public event LastLayerRemovedDelegate LastLayerRemoved;

        public delegate void ScaleChangedDelegate(double scale,double absoluteZoom);
        public event ScaleChangedDelegate ScaleChanged;

        public delegate void DrawShapeInformationDelegate(DrawShapeInformation inf,LayerType lType);
        public event DrawShapeInformationDelegate DrawInfChanged;

		public delegate void LayerChangedDelegate(ILayer layer);
		public event LayerChangedDelegate LayerChanged;

		public delegate void ChangedDelegate();
		public event ChangedDelegate ShapeRemoved;


        ////////////////
        // Constructors
        ////////////////
    
        public LayerManager(MainControler mc, Config config)
        {
            this.config = config;
            this.mainControler = mc;
            layerArray = new List<Layer>();
            layerListViewMappingList = new SortedList<int, int>();
            layerShapeMappingList = new SortedList<int, int>();
            layerArray.Clear();

            transportPointLayer    = new TransportMultiPointLayer(this, config);
            transportPolylineLayer = new TransportPolylineLayer(this, config);
            transportPolygonLayer  = new TransportPolygonLayer(this, config);

            // Initialize the quadtree for the transportLayer data with assumed properties
            transportLayerQuadtree  = new QuadTree<IShape>(new DRect(0.0d, 0.0d, 100.0d, 100.0d), 4);
            this.ScaleChanged += new ScaleChangedDelegate(transportLayerQuadtree.ZoomReactor);
            currentlyVisibleIShapes = new List<QuadTreePositionItem<IShape>>();

            mainControler.SettingsLoaded += new MainControler.SettingsLoadedDelegate(mainControler_SettingsLoaded);

            this.NewLayerStyle = new RandomColorStyle();
        }

        void mainControler_SettingsLoaded(Config config)
        {
            this.config = config;

            try
            {
                switch (config.NewLayerStyle)
                {
                    case NewLayerStyles.SpecificColor:
                        this.newLayerStyle = new SpecificColorStyle(config.NewLayerStaticColor);
                        break;
                    default: // Random Color
                        this.newLayerStyle = new RandomColorStyle();
                        break;
                }
            }
            catch
            {
                MessageBox.Show("Fehler beim Laden der Einstellungen für das Erstellen neuer Layer - verwende Standard");
                this.newLayerStyle = new RandomColorStyle();
            }
        }

        /// <summary>
        /// Loads shapefile data for editing
        /// </summary>
        /// <param name="path">Path with filename and extension to the shapefile to open</param>
        /// <param name="clearBeforeImport">true: previous data on this layer will be deleted false: no deletion</param>
        public void ImportAsTransportLayer(string path, bool clearBeforeImport)
        {
            IntPtr cPointObjectPtr;

            // Load the file
            cPointObjectPtr = ShapeLib.SHPGetPolyLineList(path, "rb",
                    mainControler.MapPanel.Width, mainControler.MapPanel.Height,
                    mainControler.MapPanel.Margin, 1, 1.0d, 1, 0, 1);

            if (cPointObjectPtr.Equals(IntPtr.Zero))
                mainControler.addLogMessage("Fehler 0x0002: Shapereferenz ist ungültig\n" + path);
            else
            {
                int momentaryPointCount;
                double[] xList;
                double[] yList;
                int pointCount = 0;
                LayerType lType;

                ShapeLib.PointObject pntObj = new ShapeLib.PointObject();
                Marshal.PtrToStructure(cPointObjectPtr, pntObj);

                double[] MinB = new double[4] { pntObj.minBX, pntObj.minBY, 0.0, 0.0 };
                double[] MaxB = new double[4] { pntObj.maxBX, pntObj.maxBY, 0.0, 0.0 };

                // make sure we have shapes and parts
                if (pntObj.shapeCount > 0 && pntObj.partCount > 0)
                {
                    // Create the outer list which holds the pointers to the sublists
                    xList = new double[pntObj.nVertices];
                    Marshal.Copy(pntObj.worldX, xList, 0, pntObj.nVertices);
                    yList = new double[pntObj.nVertices];
                    Marshal.Copy(pntObj.worldY, yList, 0, pntObj.nVertices);

                    // potentially clear the underlying layers
                    if (clearBeforeImport)
                    {
                        switch (pntObj.shpType)
                        {
                            case ShapeLib.ShapeType.MultiPoint:
                                ClearTransportLayer(LayerType.PointCanvas);
                                break;
                            case ShapeLib.ShapeType.Polygon:
                                ClearTransportLayer(LayerType.PolygonCanvas);
                                break;
                            case ShapeLib.ShapeType.PolyLine:
                                ClearTransportLayer(LayerType.PolylineCanvas);
                                break;
                            default:
                                break;
                        }
                    }

                    // "draw" the data
                    if (pntObj.shpType == ShapeLib.ShapeType.MultiPoint)
                    {
                        for (int i = 0; i <= pntObj.shapeCount - 1; i++)
                            mainControler.PerformAction(
                                new DrawAction(1.0, xList[i], -yList[i], mainControler));
                    }
                    else if (pntObj.shpType == ShapeLib.ShapeType.PolyLine
                        || pntObj.shpType == ShapeLib.ShapeType.Polygon)
                    {
                        if (pntObj.shpType == ShapeLib.ShapeType.PolyLine)
                            lType = LayerType.PolylineCanvas;
                        else
                            lType = LayerType.PolygonCanvas;

                        // Get the list with the lengths of the sublists
                        int[] partListLengths = new int[pntObj.partCount];
                        Marshal.Copy(pntObj.partListLengths, partListLengths, 0, pntObj.partCount);

                        for (int i = 0; i < pntObj.partCount; i++)
                        {
                            momentaryPointCount = partListLengths[i];

                            // if we are loading a polygon we have to decrement
                            // the pointCount of the current part since the last point
                            // is the same as the first one and this is added with
                            // the FinishDrawingAction
                            if (lType == LayerType.PolygonCanvas)
                            {
                                momentaryPointCount--;
                                // add one more (+1) to compensate the decrement before
                                pointCount++;
                            }


                            // add the first polyline point
                            mainControler.PerformAction(
                                new DrawAction(lType,
                                                xList[pointCount], -yList[pointCount],
                                                false, mainControler));

                            // Copy the temporay lists into points and into the polyPointList
                            // we start at 1 since we added the first point seperately
                            for (int j = 1; j < momentaryPointCount; j++)
                            {
                                mainControler.PerformAction(
                                    new DrawAction(lType,
                                                   xList[pointCount + j],
                                                   -yList[pointCount + j],
                                                   true, mainControler));
                            }

                            mainControler.PerformAction(
                                new FinishDrawingAction(lType, this));

                            pointCount += momentaryPointCount;
                        }

                    }
                }
                ShapeLib.deletePointObject(cPointObjectPtr);
            }
        }

		public void OnShapeRemoved()
		{
			if (ShapeRemoved != null) ShapeRemoved();
		}

        /// <summary>
        /// The function removes a shape from the mapping and the shape-list
        /// </summary>
        /// <param name="index">The index in the listview</param>
        public void removeLayer(int listviewIndex)
        {
            try
            {
                int deletedShape = layerListViewMappingList[listviewIndex];
                int count = layerListViewMappingList.Count;
                Layer layer = layerArray[deletedShape];

                layerArray.Remove(layer);

                // Update the mapping-values since the shapeArray has changed!
                for (int i = count - 1; i >= 0; i--)
                    if (layerListViewMappingList[i] > deletedShape)
                        layerListViewMappingList[i]--;

                layerListViewMappingList.Remove(listviewIndex);

                // Update the mapping-keys since the shapeArray has changed!
                for (int i = listviewIndex + 1; i < count; i++)
                {
                    int content = layerListViewMappingList[i];
                    layerListViewMappingList.Remove(i);
                    layerListViewMappingList.Add(i - 1, content);
                }

                //VERSUCH: 
                mainControler.MapPanel.ScreenChanged = true;
                
                for (int i = 0; i < LayerCount; i++)
                    layerArray[i].Changed = true;
                
                mainControler.addLogMessage("[Layer] Layer " + listviewIndex.ToString() + " wurde entfernt");
                
                removeLayerPostProcessing();
            }
            catch (KeyNotFoundException e)
            {
                System.Windows.Forms.MessageBox.Show("Fehler: Inkonsistenz im Layermanagement entdeckt\nFehlernummer: 0x092\nBeschreibung: " + e.Message +"\n\nBitte speichern Sie alles und starten die Anwendung neu!");
            }
        }

        public ShapeObject addShapefileLayer(string FilePath)
        {
            ShapeObject shapeObject = new ShapeObject(this, FilePath);
            shapeObject.init();

            layerListViewMappingList.Add(LayerCount, LayerCount); // ListView-Index, Shape-Index
            layerArray.Add(shapeObject);

            addLayerPostProcessing(shapeObject);
            return shapeObject;
        }

        private void addLayerPostProcessing(Layer newLayer)
        {
            if (LayerCount == 1 && FirstLayerAdded != null) FirstLayerAdded(newLayer);
            if (LayerAdded != null) LayerAdded(newLayer);
        }
        private void removeLayerPostProcessing()
        {
            if (LayerCount == 0 && LastLayerRemoved != null) LastLayerRemoved();
        }

        public void addShapefileLayer(string filePath, LayerInfo layerInfo, VectorInfo vectorInfo)
        {
            //TODO: sollte man testen, ob alle Werte in layerInfo sinnvoll sind, bzw, von Standardkonstruktor abweichen
            ShapeObject shp = addShapefileLayer(filePath);
            if (layerInfo != null) shp.LayerInfo = layerInfo;
            if (vectorInfo != null) {
				shp.VectorInfo = vectorInfo;
				shp.Visible = vectorInfo.IsVisible;
			}
			if (LayerChanged != null) LayerChanged(shp);
        }



        public ImageLayer addImageLayer(string FilePath)
        {
            CreateNewGDALContainer(mainControler.MapPanel.ClientSize.Width,
                mainControler.MapPanel.ClientSize.Height);

            GravurGIS.MapPanelBindings.ImageLayerInfo ilinfo = GDALAddImageToContainer(FilePath);

            if (ilinfo.id >= 0)
            {
                ImageLayer imageLayer = new ImageLayer(this, ilinfo, Path.GetFileName(FilePath), Path.GetDirectoryName(FilePath));
                layerListViewMappingList.Add(LayerCount, LayerCount); // ListView-Index, Layer-Index
                layerArray.Add(imageLayer);

                if (LayerCount == 1)
                {              
                    this.firstScale = this.dFirstMapMeassure = 1;
                    mainControler.createNewCanvas(ilinfo.minBX * firstScale, ilinfo.maxBY * firstScale, scale);
                    mainControler.MapPanel.onLayerZoom(0);
                }

                addLayerPostProcessing(imageLayer);
                return imageLayer;
            }
            throw new ApplicationException("Konnte kein GeoBild Layer hinzufügen");
        }

        public void addOGRLayer(string FilePath)
        {
            addOGRLayer(FilePath, null);
        }

        public void addOGRLayer(string FilePath, string SpatialReference)
        {
            CreateNewOGRContainer(mainControler.MapPanel.ClientSize.Width,
                mainControler.MapPanel.ClientSize.Height);

            GravurGIS.MapPanelBindings.VectorLayerInfo vlinfo = OGRAddLayerToContainer(FilePath, SpatialReference);

            try
            {
                if (vlinfo.id >= 0)
                {
                    OGRLayer ogrLayer = new OGRLayer(vlinfo, cOGRContainer);

                    //ImageLayer imageLayer = new ImageLayer(this, ilinfo, Path.GetFileName(FilePath), Path.GetDirectoryName(FilePath));
                    layerListViewMappingList.Add(LayerCount, LayerCount); // ListView-Index, Layer-Index
                    layerArray.Add(ogrLayer);

                    if (LayerCount == 1)
                    {
                        this.firstScale = this.dFirstMapMeassure = 1;
                        mainControler.createNewCanvas(vlinfo.minBX * firstScale, vlinfo.maxBY * firstScale, scale);
                        mainControler.MapPanel.onLayerZoom(0);
                    }

                    addLayerPostProcessing(ogrLayer);
                    return;
                }
                else if (vlinfo.id == -2)
                {
                    // Either have a .prj file or specify a suitable alternative spatial reference!
                    MessageBox.Show("Es wurden keine Informationen über das verwendete geogr. Datum und eine möglicherweise verwendete Projektion gefunden. Im Moment wird Bessel 1841 und Gaußkrüger Zone 4 angenommen");

                    // write new .prj file
                    string path = String.Format("{0}\\{1}.prj", Path.GetDirectoryName(FilePath), Path.GetFileNameWithoutExtension(FilePath));
                    StreamWriter prjFile = new StreamWriter(path, false);
                    prjFile.WriteLine("PROJCS[\"DHDN_3_Degree_Gauss_Zone_4\",GEOGCS[\"GCS_Deutsches_Hauptdreiecksnetz\",DATUM[\"D_Deutsches_Hauptdreiecksnetz\",SPHEROID[\"Bessel_1841\",6377397.155,299.1528128]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Gauss_Kruger\"],PARAMETER[\"False_Easting\",4500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",12.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]");

                    prjFile.Close();

                    addOGRLayer(FilePath, null);

                }
                else
                    throw new ApplicationException("Info-Id < 0!");
            }
            catch (Exception e)
            {
                throw new ApplicationException("Konnte keinen neuen OGRLayer hinzufügen", e);
            }
        }

        public void addImageLayer(string filePath, LayerInfo layerInfo)
        {
            //TODO: sollte man testen, ob alle Werte in layerInfo sinnvoll sind, bzw, von Standardkonstruktor abweichen
            ImageLayer imgLayer = addImageLayer(filePath);
            if (layerInfo != null) imgLayer.LayerInfo = layerInfo;
            
        }

        public void switchLayerVisibility(int index, bool newState)
        {
			//Boolean state = true;
			//if (layerArray[layerListViewMappingList[index]].Visible)
			//    state = false;
			layerArray[layerListViewMappingList[index]].Visible = newState;
        }

        public void moveLayerDownOneStep(int index)
        {
            int selectedShp = layerListViewMappingList[index]; // the associated index in the shapeFile-List
            layerListViewMappingList[index] = layerListViewMappingList[index + 1];
            layerListViewMappingList[index + 1] = selectedShp;
        }

        public void moveLayerUpOneStep(int index)
        {
            int selectedShp = layerListViewMappingList[index]; // the associated index in the shapeFile-List
            layerListViewMappingList[index] = layerListViewMappingList[index - 1];
            layerListViewMappingList[index - 1] = selectedShp;
        }

        public void changeLayer(int selectedIndex, string FilePath)
        {
            Layer layer = getLayerFromMapping(selectedIndex);
            if (layer.Type == GravurGIS.Layers.LayerType.Shape)
            {
                ShapeObject shp = (ShapeObject) layer;

                System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(FilePath);
                reader.ReadStartElement();
                while (reader.Read())
                {
                    if (reader.NodeType == System.Xml.XmlNodeType.Element)
                        if (reader.Name == "z:row")
                            if (reader.HasAttributes)
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "PointColor") shp.VectorInfo.PointColor = Color.FromArgb(System.Convert.ToInt32(reader.Value));
                                    //if (reader.Name == "PointSize")  shp.pointSize = System.Convert.ToInt32(reader.Value);
                                    if (reader.Name == "LineColor") shp.VectorInfo.LineColor = Color.FromArgb(System.Convert.ToInt32(reader.Value));
                                    if (reader.Name == "FillColor") shp.VectorInfo.FillColor = Color.FromArgb(System.Convert.ToInt32(reader.Value));
                                    // if (reader.Name == "LineSize")   shp.lineSize   = System.Convert.ToInt32(reader.Value);
                                }
                }
            }
        }

        public void openMWD(string fileName)
        {
            //TODO: FileDialog, beim Öffnen eines Projektes werden eventuell vorhandene Layer gelöscht, sind sie sich wirklich sicher?
            mainControler.deleteAllLayers();
            importMWD(fileName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">gets openFileDialog.FileName as input, its path + fileName of an mwd File</param>
        public void importMWD(string fileName)
        {
            string[] pathnamearray = fileName.Split('\\');
            string pathName = ""; //its the path of the file + "\\"
            for (int i = 0; i <= pathnamearray.Length - 2; i++) pathName += pathnamearray[i] + '\\';
          
            bool pathFound = false;
            LayerInfo layerInfo = null;
            VectorInfo vectorInfo = null;
            bool isVisible = false;

            LayerType tempType = LayerType.Undefined;
            string tempName = "";
            string file = "";
            double[] Min = { 0, 0 };
            double[] Max = { 0, 0 };
            //check indicates wether all four bbox values are found
            int check = 0;
            bool oneLayerFound = false;
            string errFileNotFound = "";
            System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(fileName); ;
            
            while (reader.Read())
            {
                if (reader.NodeType == System.Xml.XmlNodeType.Element)
                    if (reader.Name == "z:row")
                    {
                            while (reader.MoveToNextAttribute())
                            {
                                #region im while

                                if (reader.Name == "Xmin") { Min[0] = Convert.ToDouble(reader.Value); check++; }
                                if (reader.Name == "Xmax") { Max[0] = Convert.ToDouble(reader.Value); check++; }
                                if (reader.Name == "Ymin") { Min[1] = Convert.ToDouble(reader.Value); check++; }
                                if (reader.Name == "Ymax") { Max[1] = Convert.ToDouble(reader.Value); check++; }
                                
                                if (reader.Name == "LayerPfad") //LayerPfad muss vor anderen Attributen stehen
                                {
                                    file = reader.Value;
                                    if (File.Exists(file)) pathFound = true;
                                    else if (File.Exists(pathName + Path.GetFileName(file)))
                                    {
                                        file = pathName + Path.GetFileName(file);
                                        pathFound = true;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < config.SearchDirList.Count; i++)
                                        {
                                            if (File.Exists(config.SearchDirList[i] + "\\" + Path.GetFileName(file)))
                                            {
                                                file = config.SearchDirList[i] + "\\" + Path.GetFileName(file);

                                                pathFound = true;
                                                break;
                                            }
                                        }

                                        if (!pathFound) //one of the layer in mwd file could not be found
                                        {
                                            errFileNotFound += file + "\r\n";
                                            //should look after other layerFiles
                                            break;
                                        }
                                    }
                                    if (pathFound)
                                    {
                                        oneLayerFound = true;
                                        layerInfo = new LayerInfo();
                                        vectorInfo = new VectorInfo();
                                        layerInfo.FilePath = file;
                                    }
                                }
                                if (pathFound)
                                {
                                    try
                                    {
                                        if (reader.Name == "Punktfarbe") vectorInfo.PointColor = Color.FromArgb(Int32.Parse(reader.Value));
                                        else if (reader.Name == "Linienfarbe") vectorInfo.LineColor = Color.FromArgb(Convert.ToInt32(reader.Value));
                                        else if (reader.Name == "Fuellfarbe") vectorInfo.FillColor = Color.FromArgb(Int32.Parse(reader.Value));
										else if (reader.Name == "Layergefuellt") vectorInfo.Fill = (reader.Value == "True");
                                        else if (reader.Name == "LayerBeschriftung") tempName = reader.Value;
                                        else if (reader.Name == "Visible") vectorInfo.IsVisible = Boolean.Parse(reader.Value);
                                        else if (reader.Name == "Linienstaerke") vectorInfo.LayerPen.Width = float.Parse(reader.Value);
                                        else if (reader.Name == "Punktgroesse") vectorInfo.LayerPen.Width = float.Parse(reader.Value);
                                        else if (reader.Name.ToLower() == "layertype")
                                                {
                                                    if (reader.Value.ToLower() == "image") tempType = LayerType.Image;
                                                    if (reader.Value.ToLower() == "shapefile") tempType = LayerType.Shape;
                                                }
                                    }
                                    catch (Exception)
                                    {
                                        MessageBox.Show("Fehler 0x6221: Die Projektdatei ist fehlerhaft - Layereinstellungen wird ignoriert\nGelesener Wert: " 
                                            + reader.Value + "\nDatei: " + layerInfo.FilePath);
                                    }
                                }
                                #endregion
                            }
                        if(pathFound)
                            switch (tempType)
                            {
                                case LayerType.Shape:
                                    addShapefileLayer(file, layerInfo, vectorInfo);
                                    break;
                                case LayerType.Image:
                                    addImageLayer(file,layerInfo);
                                    break;
                                
                                default:
                                    break;
                            }
                        pathFound = false;
                        tempType = LayerType.Undefined;
                        tempName = "";
                        isVisible = false;
                        layerInfo = null;
                        vectorInfo = null;
                    }
            }
            if ((check == 4) && (oneLayerFound)) // all bounding values are found + onelayer -> draw details of map
            {
                // scale und firstscale sind schon gesetzt durch das Laden der Layer

                MapPanel mp = mainControler.MapPanel;
                double zoom = mainControler.MapPanel.calculateZoomFactor(
                        (Max[0] - Min[0]) * scale, (Max[1] - Min[1]) * scale);

                double newAbsoluteZoom = mp.AbsolutZoom * zoom;


                PointD newD = new PointD(
                        Min[0] * firstScale * newAbsoluteZoom,
                        Max[1] * firstScale * newAbsoluteZoom);

                mainControler.PerformAction(
                            new ZoomAction(
                                mp.AbsolutZoom,
                                mainControler.MapPanel.D,
                                newD,
                                newAbsoluteZoom,
                                new PointD(Max[0], -Min[1]),
                                getMaxAbsoluteZoom(),
                                mainControler));
            }
            else { 
                if(!oneLayerFound) MessageBox.Show("Die Standardprojektdateien konnten nicht gefunden werden.");
                //wenn die vier werte nicht gefunden werden, bleiben die Ansicht bei der BBox des 1. Layers
            }
            reader.Close();
            //ensure that not two errorMessages appear if no layer was found
            if (!errFileNotFound.Equals("") && oneLayerFound){
                ErrorDialog fileNotFoundDialog = new ErrorDialog();
                fileNotFoundDialog.ShowDialog("Fehler 0x0500 - Folgende Dateien konnten nicht gefunden werden:\r\n" + errFileNotFound + "Projekt wurde ohne diese Dateien geladen.");
            }
            mainControler.MapPanel.ScreenChanged = true;
            mainControler.MapPanel.Invalidate();
            mainControler.MapPanel.Update();
        }

        public void saveAsMWD(string openedFileName)
        {
            List<int> saveErr = new List<int>();
            saveErr.Clear();

            string schemaFile = config.ApplicationDirectory + "\\Misc\\schema.mwd";
            //if there are no Layer in layerArray, then do not save anything
            if (LayerCount == 0) { MessageBox.Show("im zu speichernden Projekt sind keine Layer vorhanden. Speichervorgang wird abgebrochen"); }
            else
            {
                if (File.Exists(schemaFile))
                {
                    StringWriter tempStream = null;
                    XmlTextWriter xwriter = null;
                    StreamReader s = null;
                    StreamWriter myFile = null;

                    try
                    {
                        tempStream = new StringWriter();

                        xwriter = new XmlTextWriter(tempStream);
                        xwriter.Formatting = Formatting.Indented;
                        xwriter.Indentation = 4;
                        xwriter.WriteStartElement("rs:data");
                        xwriter.WriteStartElement("rs:insert");

                        // Write current viewport corners into mwd to save zoom and position
                        Rectangle viewport = mainControler.MapPanel.Viewport;
                        int min = viewport.Top - viewport.Height;

                        xwriter.WriteStartElement("z:row");
                        xwriter.WriteAttributeString("Xmin", viewport.Left.ToString());
                        xwriter.WriteAttributeString("Ymin", min.ToString());
                        xwriter.WriteAttributeString("Xmax", viewport.Right.ToString());
                        xwriter.WriteAttributeString("Ymax", viewport.Top.ToString());
                        xwriter.WriteEndElement();

                        // Write ImageFiles

                        // Write out the layer list
                        for (int i = 0; i < LayerCount; i++)
                        {
                            ILayer layer = getLayerFromMapping(i);
                   
                            if (layer != null)
                                switch (layer.Type)
                                {
                                    case LayerType.Shape:
                                        ShapeObject shp = layer as ShapeObject;
                                        xwriter.WriteStartElement("z:row");
                                        xwriter.WriteAttributeString("LayerName", shp.LayerInfo.FileName);
                                        xwriter.WriteAttributeString("LayerPfad", shp.LayerInfo.FilePath);
                                        xwriter.WriteAttributeString("LayerBeschriftung", shp.LayerName);
                                        xwriter.WriteAttributeString("Key", shp.LayerInfo.FilePath);
                                        xwriter.WriteAttributeString("Visible", shp.Visible.ToString());
                                        xwriter.WriteAttributeString("LayerType", "Shapefile"); // if we wanted to save also image layers - we have to use the LayerType property
                                        xwriter.WriteAttributeString("Fuellfarbe", (shp.VectorInfo.FillColor.ToArgb()).ToString());
                                        xwriter.WriteAttributeString("Linienfarbe", (shp.VectorInfo.LineColor.ToArgb()).ToString());
                                        xwriter.WriteAttributeString("Punktfarbe", (shp.VectorInfo.PointColor.ToArgb()).ToString());
                                        xwriter.WriteAttributeString("Stuetzpunkte", shp.BasePoints.ToString());
                                        xwriter.WriteAttributeString("Layergefuellt", shp.VectorInfo.Fill ? "True" : "False");
                                        xwriter.WriteAttributeString("Fuellmodus", "0");
                                        xwriter.WriteAttributeString("Linientyp", "0");
                                        xwriter.WriteAttributeString("Linienstaerke", shp.VectorInfo.LayerPen.Width.ToString());
                                        xwriter.WriteAttributeString("Punktgroesse", shp.VectorInfo.LayerPen.Width.ToString());
                                        xwriter.WriteAttributeString("Punkttyp", "0");
                                        xwriter.WriteAttributeString("Transparenz", "100");
                                        xwriter.WriteEndElement();
                                        break;
                                    case LayerType.Image:
                                        ImageLayer img = layer as ImageLayer;
                                        xwriter.WriteStartElement("z:row");
                                        xwriter.WriteAttributeString("LayerName", img.FileName);
                                        xwriter.WriteAttributeString("LayerPfad", img.FilePath);
                                        xwriter.WriteAttributeString("LayerBeschriftung", img.Description);
                                        xwriter.WriteAttributeString("Key", img.FilePath);

                                        xwriter.WriteAttributeString("Visible", img.Visible.ToString());
                                        xwriter.WriteAttributeString("LayerType", "Image");
                                        xwriter.WriteAttributeString("Transparenz", "0");
                                        xwriter.WriteEndElement();
                                        break;
                                    default:
                                        break;
                                }
                            else  saveErr.Add(getLayerIndexFromMapping(i));
                        }
                        xwriter.WriteEndElement();
                        xwriter.WriteEndElement();

                        s = new StreamReader(schemaFile);
                        myFile = new StreamWriter(openedFileName, false, Encoding.Unicode);

                        myFile.Write(s.ReadToEnd());
                        myFile.Write(tempStream.ToString());

                        myFile.Write("</xml>");
                        if (saveErr.Count > 0){
                            ErrorDialog errDialog = new ErrorDialog();
                            string err = "";
                            string n = ""; //grammatik von konnten
                            for (int j = 0; j < saveErr.Count - 2; j++)
                                err += saveErr[j] +1 + ", ";
                            if (saveErr.Count > 1)
                            {
                                n = "n";
                                err += saveErr[saveErr.Count - 2] +1 + " und ";
                            }
                            err += saveErr[saveErr.Count - 1]+1;
                                errDialog.ShowDialog("Fehler 0x3990: Layer " + err + " konnte" + n +" nicht gespeichert werden.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            String.Format("Fehler 0x3991: Beim Speichern des Projekts ist ein Fehler aufgetreten:{0}{1}",
                            Environment.NewLine, ex.Message));
                    }
                    finally
                    {
                        if (xwriter != null) xwriter.Close();
                        if (myFile != null) myFile.Close();
                        if (s != null) s.Close();
                        if (tempStream != null) tempStream.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Fehler 0x9012 - Es wurde kein XML-Schema gefunden.\nSchreiben der Datei abgebrochen!");
                }
            }
        }

        public void saveTransPortLayers()
        {
            GravurGIS.GUI.Dialogs.OverrideInformation result = new GravurGIS.GUI.Dialogs.OverrideInformation(
                false, false, false, DialogResult.Cancel);
            
            // check if directories exist and create them if neccecary
            if (!Directory.Exists(Path.GetDirectoryName(Path.ChangeExtension(config.ExPntLayerFile, "shp"))))
            {
                Directory.CreateDirectory(
                    Path.GetDirectoryName(Path.ChangeExtension(config.ExPntLayerFile, "shp")));
            }
            if (!Directory.Exists(Path.GetDirectoryName(Path.ChangeExtension(config.ExPLineLayerFile, "shp"))))
            {
                Directory.CreateDirectory(
                    Path.GetDirectoryName(Path.ChangeExtension(config.ExPLineLayerFile, "shp")));
            }
            if (!Directory.Exists(Path.GetDirectoryName(Path.ChangeExtension(config.ExPGonLayerFile, "shp"))))
            {
                Directory.CreateDirectory(
                    Path.GetDirectoryName(Path.ChangeExtension(config.ExPGonLayerFile, "shp")));
            }
            
            // check if the files do already exist
            if (System.IO.File.Exists(Path.ChangeExtension(config.ExPntLayerFile, "shp"))) result.PointChecked = true;
            if (System.IO.File.Exists(Path.ChangeExtension(config.ExPLineLayerFile, "shp"))) result.PolylineChecked = true;
            if (System.IO.File.Exists(Path.ChangeExtension(config.ExPGonLayerFile, "shp"))) result.PolygonChecked = true;
            
            if (result.PointChecked || result.PolylineChecked || result.PolygonChecked)
                result = mainControler.showOverrideDialog(result);
            else
                result = new GravurGIS.GUI.Dialogs.OverrideInformation(true, true, true, DialogResult.OK);

            if (result.dialogResult == DialogResult.OK)
            {
                // overwrite all and everything :)
                // start with Points
                ShapeLib.ShapeType shpType;
                IntPtr hShp, hDbf;
                int nVertices, nShapes;

                double[] xCoord;
                double[] yCoord;

                string fileName;
                IntPtr pshpObj;
                PointD pnt;
                int iRet, iCommentField, iCategoryField, iIDField;

                StringBuilder saveString = new StringBuilder("") ;

                int maxCommentLength = 1;

                // find the longest category name
                int maxCatNameLength = 1;
                for (int i = 0; i < config.CategoryList.Count; i++)
                    maxCatNameLength = Math.Max(maxCatNameLength, config.CategoryList[i].Length);

                if (result.PointChecked)
                {
                    ///////////////////
                    // Multipoint layer
                    ///////////////////

                    shpType = ShapeLib.ShapeType.MultiPoint;
                    nShapes = transportPointLayer.Count;

                    if (nShapes == 0) // actually we do not have any points
                        createNullShape(config.ExPntLayerFile);
                    else
                    {
                        //////////////////////////
                        // .shp & .shx & .dbf file
                        //////////////////////////

                        // create a new shapefile
                        hShp = ShapeLib.SHPCreateW(config.ExPntLayerFile, shpType);
                        hDbf = ShapeLib.DBFCreateW(config.ExPntLayerFile);

                        fileName = Path.GetFileNameWithoutExtension(config.ExPntLayerFile);

                        if (hShp.Equals(IntPtr.Zero))
                        {
                            MessageBox.Show("0x2001 - Fehler beim Schreiben der Punktlayer Datei!");
                            return;
                        }
                        if (hDbf.Equals(IntPtr.Zero))
                        {
                            MessageBox.Show("0x2011 - Fehler beim Schreiben der Punktlayer dbf-Datei!");
                            return;
                        }
                        

                        // find the maximum comment length
                        for (int i = 0; i <  nShapes; i++)
                            maxCommentLength = Math.Max(maxCommentLength, transportPointLayer.getShape(i).Commment.Length);

                        // create new ID Field
                        iIDField = ShapeLib.DBFAddFieldW(hDbf,
                            "ID",
                            ShapeLib.DBFFieldType.FTInteger, 11, 0);

                        // create new comment field - String, Max width: 30
                        iCommentField = ShapeLib.DBFAddFieldW(hDbf,
                            config.ExPntLayerDBFCommentFieldName,
                            ShapeLib.DBFFieldType.FTString, maxCommentLength, 0);

                        // create new category field - String, Max width: 30
                        iCategoryField = ShapeLib.DBFAddFieldW(hDbf,
                            "Category",
                            ShapeLib.DBFFieldType.FTString, maxCatNameLength, 0);

                        if (iCommentField == -1 || iCategoryField == -1 || iIDField == -1)
                        {
                            MessageBox.Show("0x2021 - Fehler beim Schreiben der DBF-Datei für Punktlayer!");
                            return;
                        }

                        // Generate point arrays
                        xCoord = new double[1];
                        yCoord = new double[1];
                        int count = 0;
                        for (int i = 0; i < nShapes; i++)
                        {
                            if (transportPointLayer.getShape(i).Visible)
                            {
                                count++;
                                pnt = transportPointLayer.getPoint(i);
                                xCoord[0] = pnt.x;
                                yCoord[0] = -pnt.y;
                                pshpObj = ShapeLib.SHPCreateSimpleObject(shpType, 1,
                                    xCoord, yCoord, new double[1]);
                                iRet = ShapeLib.SHPWriteObject(hShp, -1, pshpObj);
                                ShapeLib.SHPDestroyObject(pshpObj);

                                // write out ID
                                ShapeLib.DBFWriteIntegerAttribute(hDbf, iRet, iIDField,
                                    i);

                                // write out comments
                                if (String.IsNullOrEmpty(transportPointLayer.getComment(i)))
                                    ShapeLib.DBFWriteNULLAttribute(hDbf, iRet, iCommentField);
                                else
                                    ShapeLib.DBFWriteStringAttributeW(hDbf, iRet, iCommentField,
                                        transportPointLayer.getComment(i));

                                // write out categories
                                if (String.IsNullOrEmpty(transportPointLayer.getShape(i).Category))
                                    ShapeLib.DBFWriteNULLAttribute(hDbf, iRet, iCategoryField);
                                else
                                    ShapeLib.DBFWriteStringAttributeW(hDbf, iRet, iCategoryField,
                                        transportPointLayer.getShape(i).Category);
                            }
                        }

                        ShapeLib.SHPClose(hShp);
                        ShapeLib.DBFClose(hDbf);

                        if (count == 0) createNullShape(Path.ChangeExtension(config.ExPntLayerFile, "shp"));
                    }
                    saveString.Append("Punkt-, ");
                }

                if (result.PolylineChecked)
                {
                    /////////////////
                    // Polyline layer
                    /////////////////

                    shpType = ShapeLib.ShapeType.PolyLine;
                    nShapes = transportPolylineLayer.Count;

                    if (nShapes == 0) // actually we do not have any points
                        createNullShape(config.ExPLineLayerFile);
                    else
                    {

                        hShp = ShapeLib.SHPCreateW(config.ExPLineLayerFile, shpType);
                        hDbf = ShapeLib.DBFCreateW(config.ExPLineLayerFile);

                        fileName = Path.GetFileNameWithoutExtension(config.ExPLineLayerFile);

                        if (hShp.Equals(IntPtr.Zero))
                        {
                            MessageBox.Show("0x2002 - Fehler beim Schreiben der Polyline Datei!");
                            return;
                        }

                        // find the maximum comment length
                        for (int i = 0; i <  nShapes; i++)
                            maxCommentLength = Math.Max(maxCommentLength, transportPolylineLayer.getShape(i).Commment.Length);

                        // create new ID Field
                        iIDField = ShapeLib.DBFAddFieldW(hDbf,
                            "ID",
                            ShapeLib.DBFFieldType.FTInteger, 11, 0);

                        // create comment field in the DBF
                        iCommentField = ShapeLib.DBFAddFieldW(hDbf,
                            config.ExPLineLayerDBFCommentFieldName,
                            ShapeLib.DBFFieldType.FTString, maxCommentLength, 0);

                        // create new category field - String, Max width: 30
                        iCategoryField = ShapeLib.DBFAddFieldW(hDbf,
                            "Category",
                            ShapeLib.DBFFieldType.FTString, maxCatNameLength, 0);

                        if (iCommentField == -1 || iCategoryField == -1 || iIDField == -1)
                        {
                            MessageBox.Show("0x2021 - Fehler beim Schreiben der DBF-Datei für Punktlayer!");
                            return;
                        }
                        int count = 0;
                        for (int i = 0; i < nShapes; i++)
                        {
                            if (transportPolylineLayer.getShape(i).Visible)
                            {
                                count++;
                                xCoord = transportPolylineLayer.getXListOfPolyLine(i);
                                yCoord = transportPolylineLayer.getYListOfPolyLine(i);
                                nVertices = transportPolylineLayer.getVerticeCountOfPolyLine(i);

                                pshpObj = ShapeLib.SHPCreateSimpleObject(shpType, nVertices,
                                    xCoord, yCoord, new double[nVertices]);

                                iRet = ShapeLib.SHPWriteObject(hShp, -1, pshpObj);
                                ShapeLib.SHPDestroyObject(pshpObj);

                                // write out ID
                                ShapeLib.DBFWriteIntegerAttribute(hDbf, iRet, iIDField,
                                    i);

                                // write out comment
                                if (transportPolylineLayer.getComment(i).Equals(""))
                                    ShapeLib.DBFWriteNULLAttribute(hDbf, iRet, iCommentField);
                                else
                                    ShapeLib.DBFWriteStringAttributeW(hDbf, iRet, iCommentField,
                                        transportPolylineLayer.getComment(i));

                                // write out categories
                                if (String.IsNullOrEmpty(transportPolylineLayer.getShape(i).Category))
                                    ShapeLib.DBFWriteNULLAttribute(hDbf, iRet, iCategoryField);
                                else
                                    ShapeLib.DBFWriteStringAttributeW(hDbf, iRet, iCategoryField,
                                        transportPolylineLayer.getShape(i).Category);
                                
                            }
                        }

                        ShapeLib.SHPClose(hShp);
                        ShapeLib.DBFClose(hDbf);

                        if (count == 0) createNullShape(config.ExPLineLayerFile);
                    }
                    saveString.Append("Linienzug-, ");
                }

                if (result.PolygonChecked)
                {
                    /////////////////
                    // Polygon layer
                    /////////////////

                    shpType = ShapeLib.ShapeType.Polygon;
                    nShapes = transportPolygonLayer.Count;

                    if (nShapes == 0) // actually we do not have any points
                        createNullShape(config.ExPGonLayerFile);
                    else
                    {

                        hShp = ShapeLib.SHPCreateW(config.ExPGonLayerFile, shpType);
                        hDbf = ShapeLib.DBFCreateW(config.ExPGonLayerFile);

                        fileName = Path.GetFileNameWithoutExtension(config.ExPGonLayerFile);

                        if (hShp.Equals(IntPtr.Zero))
                        {
                            MessageBox.Show("0x2003 - Fehler beim Schreiben der Polygon Datei!");
                            return;
                        }
                        if (hDbf.Equals(IntPtr.Zero))
                        {
                            MessageBox.Show("0x2013 - Fehler beim Schreiben der Polygon dbf-Datei!");
                            return;
                        }

                        // find the maximum comment length
                        for (int i = 0; i < nShapes; i++)
                            maxCommentLength = Math.Max(maxCommentLength, transportPolygonLayer.getShape(i).Commment.Length);

                        // create new ID Field
                        iIDField = ShapeLib.DBFAddFieldW(hDbf,
                            "ID",
                            ShapeLib.DBFFieldType.FTInteger, 11, 0);

                        // create a new field in the DBF
                        iCommentField = ShapeLib.DBFAddFieldW(hDbf,
                            config.ExPGonLayerDBFCommentFieldName,
                            ShapeLib.DBFFieldType.FTString, maxCommentLength, 0);

                        // create new category field - String, Max width: 30
                        iCategoryField = ShapeLib.DBFAddFieldW(hDbf,
                            "Category",
                            ShapeLib.DBFFieldType.FTString, maxCatNameLength, 0);

                        if (iCommentField == -1 || iCategoryField == -1 || iIDField == -1)
                        {
                            MessageBox.Show("0x2021 - Fehler beim Schreiben der DBF-Datei für Punktlayer!");
                            return;
                        }
                        int count = 0;

                        for (int i = 0; i < nShapes; i++)
                        {
                            if (transportPolygonLayer.getShape(i).Visible)
                            {
                                count++;
                                xCoord = transportPolygonLayer.getXListOfPolygon(i);
                                yCoord = transportPolygonLayer.getYListOfPolygon(i);
                                nVertices = transportPolygonLayer.getVerticeCountOfPolygon(i);

                                pshpObj = ShapeLib.SHPCreateSimpleObject(shpType, nVertices,
                                    xCoord, yCoord, new double[nVertices]);

                                iRet = ShapeLib.SHPWriteObject(hShp, -1, pshpObj);

                                // write out ID
                                ShapeLib.DBFWriteIntegerAttribute(hDbf, iRet, iIDField,
                                    i);

                                ShapeLib.SHPDestroyObject(pshpObj);
                                if (transportPolygonLayer.getComment(i).Equals(""))
                                    ShapeLib.DBFWriteNULLAttribute(hDbf, iRet, iCommentField);
                                else
                                    ShapeLib.DBFWriteStringAttributeW(hDbf, iRet, iCommentField,
                                        transportPolygonLayer.getComment(i));

                                // write out categories
                                if (String.IsNullOrEmpty(transportPolygonLayer.getShape(i).Category))
                                    ShapeLib.DBFWriteNULLAttribute(hDbf, iRet, iCategoryField);
                                else
                                    ShapeLib.DBFWriteStringAttributeW(hDbf, iRet, iCategoryField,
                                        transportPolygonLayer.getShape(i).Category);
                            }
                        }

                        ShapeLib.SHPClose(hShp);
                        ShapeLib.DBFClose(hDbf);

                        if (count == 0) createNullShape(config.ExPGonLayerFile);

                        saveString.Append("Polygon-, ");
                    }
                    saveString.Append("Polygon-, ");
                }
                if (saveString.ToString().Equals(""))
                    mainControler.setStatusTimed("Achtung: Es wurden keine Daten gespeichert!", 3000);
                else
                {
                    saveString.Remove(saveString.Length - 2, 2);
                    mainControler.setStatusTimed(saveString.ToString() + "Layer gespeichert!", 3000);
                }
            } else
                mainControler.setStatusTimed("Achtung: Es wurden keine Daten gespeichert!", 3000);
        }

        private void createNullShape(string path)
        {
            IntPtr hShp = ShapeLib.SHPCreateW(path, ShapeLib.ShapeType.NullShape);
            if (hShp.Equals(IntPtr.Zero))
            {
                MessageBox.Show("0x2004 - Fehler beim Schreiben der NullShape Datei\n" + path);
                return;
            }

            IntPtr pshpObj = ShapeLib.SHPCreateSimpleObject(ShapeLib.ShapeType.NullShape, 0,
                    null, null, null);
            ShapeLib.SHPWriteObject(hShp, -1, pshpObj);
            ShapeLib.SHPDestroyObject(pshpObj);
            ShapeLib.SHPClose(hShp);
        }
        

        public void generateGeometryStructureInDLL(ShapeObject shpObject,
            ref GravurGIS.Topology.Grid.Grid partGrid)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            IntPtr cPointObjectPtr;

            if (LayerCount == 0)
                cPointObjectPtr = ShapeLib.SHPGetPolyLineList(shpObject.LayerInfo.FilePath, shpObject.Access,
                    mainControler.MapPanel.Width, mainControler.MapPanel.Height,
                    mainControler.MapPanel.Margin, 1, 1.0d, 0,pointSize, 0);
            else
                cPointObjectPtr = ShapeLib.SHPGetPolyLineList(shpObject.LayerInfo.FilePath, shpObject.Access,
                    mainControler.MapPanel.Width, mainControler.MapPanel.Height,
                    mainControler.MapPanel.Margin, 1, firstScale, 1,0, 0);

            if (cPointObjectPtr.Equals(IntPtr.Zero))
            {
                mainControler.showErrorMessage("Fehler 0x0002: Shapereferenz ist ungültig");
            }
            else
            {
                int momentaryPointCount;
                double[] xWorldList;
                double[] yWorldList;
                int[] xDispList;
                int[] yDispList;
                int pointCount = 0;

                ShapeLib.PointObject pntObj = new ShapeLib.PointObject();
                Marshal.PtrToStructure(cPointObjectPtr, pntObj);

                shpObject.BoundingBox = new WorldBoundingBoxD(pntObj.minBX,
                    pntObj.maxBY, pntObj.maxBX, pntObj.minBY);

                if (pntObj.isWellDefines == 0)
                    shpObject.IsWellDefined = false;
                else
                    shpObject.IsWellDefined = true;

                if (LayerCount == 0)
                {
                    mainControler.MapPanel.AbsolutZoom = 1;

                    if (pntObj.isPoint == 0) //normal shpObject
                    {
                        this.firstScale = this.scale = pntObj.scale;

                        mainControler.MapPanel.DX = firstScale * pntObj.minBX;
                        mainControler.MapPanel.DY = firstScale * pntObj.maxBY;
                    }
                    else //shpObject contains only 1 point
                    {
                        // 1:1 Maßstab oder lieber schon etwas rauszoomen?
                        this.scale = pntObj.scale; // ((mainControler.MapPanel.Width) - 2 * (mainControler.MapPanel.Margin) - 2) / (double)defaultShapeSize;
                        this.firstScale = this.scale;

                        shpObject.IsOnePoint = true;
                        mainControler.MapPanel.DX = firstScale * (pntObj.minBX - (int)(defaultShapeSize / 2));
                        mainControler.MapPanel.DY = firstScale * (pntObj.maxBY + (int)(defaultShapeSize / 2));
                    }
                }

                shpObject.NumberOfShapes = (uint)(pntObj.shapeCount > 0 ? pntObj.shapeCount : 0);

                shpObject.ShapeType = pntObj.shpType;
                shpObject.PartCount = (uint)(pntObj.partCount > 0 ? pntObj.partCount : 0);

                PointList[] polyLinePointList;

                // make sure we have shapes and parts
                if (pntObj.shapeCount > 0
                    && (pntObj.partCount > 0 || pntObj.shpType == ShapeLib.ShapeType.Point))
                {
                    // Create the outer list which holds the pointers to the sublists
                    xWorldList = new double[pntObj.nVertices];
                    Marshal.Copy(pntObj.worldX, xWorldList, 0, pntObj.nVertices);
                    yWorldList = new double[pntObj.nVertices];
                    Marshal.Copy(pntObj.worldY, yWorldList, 0, pntObj.nVertices);
                    xDispList = new int[pntObj.nVertices];
                    Marshal.Copy(pntObj.dispX, xDispList, 0, pntObj.nVertices);
                    yDispList = new int[pntObj.nVertices];
                    Marshal.Copy(pntObj.dispY, yDispList, 0, pntObj.nVertices);

                    // Create a new Grid
                    partGrid = new GravurGIS.Topology.Grid.Grid(25, 25,
                        Convert.ToInt32(shpObject.Width * firstScale),
                        Convert.ToInt32(shpObject.Height * firstScale), false);

                    if (pntObj.shpType != ShapeLib.ShapeType.MultiPoint
						&& pntObj.shpType != ShapeLib.ShapeType.Point)
                    {
                        // Get the list with the lengths of the sublists
                        int[] partListLengths = new int[pntObj.partCount];
                        Marshal.Copy(pntObj.partListLengths, partListLengths, 0, pntObj.partCount);

                        // Copy the sublists to a new pointList
                        polyLinePointList = new PointList[pntObj.partCount];

                        for (int i = 0; i < pntObj.partCount; i++)
                        {
                            momentaryPointCount = partListLengths[i];

                            polyLinePointList[i] = new PointList(momentaryPointCount);

                            // Copy the temporay lists into points and into the polyPointList
                            for (int j = 0; j < momentaryPointCount; j++)
                            {
                                int pos = pointCount + j;
                                polyLinePointList[i].add(xWorldList[pos],
                                    yWorldList[pos], xDispList[pos], yDispList[pos], j);
                            }
                            partGrid.insert(polyLinePointList[i], getBoundingBox(ref polyLinePointList[i]));

                            pointCount += momentaryPointCount;
                        }
                    }
                    else  //shpType == MultiPoint
                    {
                        polyLinePointList = new PointList[pntObj.shapeCount];
                        for (int i = 0; i < pntObj.shapeCount; i++)
                        {
                            polyLinePointList[i] = new PointList(1);
                            polyLinePointList[i].add(xWorldList[i], yWorldList[i], xDispList[i], yDispList[i], 0);
                            partGrid.insert(polyLinePointList[i], getBoundingBox(ref polyLinePointList[i]));
                        }

                    }

                    shpObject.PolyLinePointList = polyLinePointList;
                    //double tempAbsZoom = mainControler.MapPanel.AbsolutZoom;
                    
                    // TODO: Warum wird das hier gemacht? Wird das wirklich gebraucht?
                    // Tom: Ich habe es erstmal auskommentiert
                    //int dispHeight = (int)(shpObject.BoundingBox.Height * scale + 0.5);
                    //for (int pointList = 0; pointList < shpObject.PartCount; pointList++)
                    //    shpObject.PolyLinePointList[pointList].recalculatePoints(scale, dispHeight, pntObj.minBX, pntObj.minBY);
                }

                shpObject.Changed = true;

                stopWatch.Stop();

                mainControler.addLogMessage(String.Format("[Shape] geladen nach {0}ms", stopWatch.ElapsedMilliseconds.ToString()));

                // clean things up
                MapTools.ShapeLib.deletePointObject(cPointObjectPtr);
            }
        }

        public Rectangle getBoundingBox(ref PointList pointList)
        {
            int count = pointList.Length - 1;
            int xMin = pointList.displayPointList[count].X,
                yMin = pointList.displayPointList[count].Y,
                xMax = xMin,
                yMax = yMin;

            for (int i = count - 1; i >= 0; i--)
            {
                xMin = Math.Min(xMin, pointList.displayPointList[i].X);
                yMin = Math.Min(yMin, pointList.displayPointList[i].Y);
                xMax = Math.Max(xMax, pointList.displayPointList[i].X);
                yMax = Math.Max(yMax, pointList.displayPointList[i].Y);
            }
            int width = xMax - xMin;
            int height = yMax - yMin;
            bbMaxX = Math.Max(bbMaxX, width);
            bbMaxY = Math.Max(bbMaxY, height);

            return new Rectangle(xMin, yMax, width, height);
        }


        /// <summary>
        /// Adds a new point to the point layer.
        /// </summary>
        /// <param name="x">The scaled, but unzoomed x offset</param>
        /// <param name="y">The scaled, but unzoomed y offset</param>
        public ShapeInformation addTransportPointPoint(double x, double y, double absoluteZoom)
        {
            //int width  = 2* transportPointLayer.BbWidthMargin
            //    + config.ExPntLayerPointWidth;
            //int height = 2* transportPointLayer.BbHeightMargin
            //    + config.ExPntLayerPointWidth;

            ShpPoint point = transportPointLayer.addPoint(x, y, scale);

            ShapeInformation shpInfo;
            shpInfo.quadTreePosItemInf = transportLayerQuadtree.Insert(
                point,
                new Vector2(x * firstScale, y * firstScale), // the quadtree is organized with the first scale display coordinates
                new Vector2(0, 0)); // new Vector2(width, height));
            point.Reference = shpInfo.quadTreePosItemInf;
            byte pointBBoxFactor;
            if (mainControler.DisplayResolution == DisplayResolution.QVGA) pointBBoxFactor = 4;
            else pointBBoxFactor = 8;
            shpInfo.quadTreePosItemInf.reactOnZoom((pointBBoxFactor * firstScale) / scale);
            shpInfo.iShapeInf = point;
            return shpInfo;
        }

        /// <summary>
        /// Creates a new polyline object in the appr. layer, adds it to the quadtree
        /// (despite the fact that is has only one point at the moment) and set the
        /// currentlyEditedIShape to this shape.
        /// </summary>
        /// <param name="x">The x offset of the new shape</param>
        /// <param name="y">The y offset of the new shape</param>
        public ShapeInformation addTransportPolyline(double x, double y)
        {
            //int width = 2 * transportPolylineLayer.BbWidthMargin
            //    + config.ExPLineLayerPointWidth;
            //int height = 2 * transportPolylineLayer.BbHeightMargin
            //    + config.ExPLineLayerPointWidth;

            currentlyEditedIShape = transportPolylineLayer.addPolyline(x, y, scale);
            selectedTransportQuadtreeItem =
                transportLayerQuadtree.Insert(currentlyEditedIShape,
                new Vector2(x * firstScale, y * firstScale),
                new Vector2(1, 1)); //new Vector2(width, height));
            currentlyEditedIShape.Reference = selectedTransportQuadtreeItem;
            selectedTransportQuadtreeItem.reactOnZoom((4.0 * firstScale) / scale);
            ShapeInformation shpInfo;
            shpInfo.quadTreePosItemInf = selectedTransportQuadtreeItem;
            shpInfo.iShapeInf = currentlyEditedIShape;
            return shpInfo;
        }

        /// <summary>
        /// Adds a point to the "currentlyEditedIShape" - object assuming
        /// that this object is a polyline.
        /// Requires addTransportPolyline() called before and
        /// closeTransportShape() called afterwards.
        /// </summary>
        /// <param name="x">The x offset of the new point</param>
        /// <param name="y">The y offset of the new point</param>
        /// <return>index of added pLinePoint, test against -1!</return>
        public int addTransportPolylinePoint(double x, double y) //TODO: Undo functionality for pLinePoints requires return value here
        {
            if (currentlyEditedIShape != null
                && currentlyEditedIShape.Type == ShapeLib.ShapeType.PolyLine)
            {
                currentlyEditedIShape.AddPoint(x, y, scale);
                selectedTransportQuadtreeItem.changePosAndSize(
                    new Vector2(currentlyEditedIShape.CenterX * firstScale,
                        currentlyEditedIShape.CenterY * firstScale),
                    new Vector2(currentlyEditedIShape.Width * firstScale,
                        currentlyEditedIShape.Height * firstScale));
                selectedTransportQuadtreeItem.reactOnZoom((4.0 * firstScale) / scale);
                    //new Vector2(currentlyEditedIShape.Width * firstScale +       // Should we consider the pointsize here?
                    //    2 * transportPolylineLayer.BbWidthMargin,
                    //currentlyEditedIShape.Height * firstScale +
                    //    2 * transportPolylineLayer.BbHeightMargin));

                return ((ShpPolyline)currentlyEditedIShape).getPointListSize() - 1;
            }
            else return -1;
        }
        public void closeTransportPolyline()
        {
            currentlyEditedIShape = null;
            selectedTransportQuadtreeItem = null;
        }

        /// <summary>
        /// Creates a new polygon object in the appr. layer, adds it to the quadtree
        /// (despite the fact that is has only one point at the moment) and set the
        /// currentlyEditedIShape to this shape.
        /// </summary>
        /// <param name="x">The x offset of the new shape</param>
        /// <param name="y">The y offset of the new shape</param>
        public ShapeInformation addTransportPolygon(double x, double y)
        {
            //int width = 2 * transportPolygonLayer.BbWidthMargin
            //    + config.ExPGonLayerPointWidth;
            //int height = 2 * transportPolygonLayer.BbHeightMargin
            //    + config.ExPGonLayerPointWidth;

            currentlyEditedIShape = transportPolygonLayer.addPolygon(x, y, scale);
            selectedTransportQuadtreeItem =
                transportLayerQuadtree.Insert(currentlyEditedIShape,
                new Vector2(x, y),
                new Vector2(1, 1)); //new Vector2(width, height));
            currentlyEditedIShape.Reference = selectedTransportQuadtreeItem;
            selectedTransportQuadtreeItem.reactOnZoom((4.0 * firstScale) / scale);
            ShapeInformation shpInfo;
            shpInfo.iShapeInf = currentlyEditedIShape;
            shpInfo.quadTreePosItemInf = selectedTransportQuadtreeItem;

            return shpInfo;
            
        }
        /// <summary>
        /// Adds a point to the "currentlyEditedIShape" - object assuming
        /// that this object is a polyline.
        /// Requires addTransportPolygon() called before and
        /// closeTransportShape() called afterwards.
        /// </summary>
        /// <param name="x">The x offset of the new point</param>
        /// <param name="y">The y offset of the new point</param>
        /// <returns>index of added item</returns>
        public int addTransportPolygonPoint(double x, double y)
        {
            if (currentlyEditedIShape != null
                && currentlyEditedIShape.Type == ShapeLib.ShapeType.Polygon)
            {
                currentlyEditedIShape.AddPoint(x, y, scale);
                selectedTransportQuadtreeItem.changePosAndSize(
                    new Vector2(currentlyEditedIShape.CenterX * firstScale,
                        currentlyEditedIShape.CenterY * firstScale),
                    new Vector2(currentlyEditedIShape.Width * firstScale,
                        currentlyEditedIShape.Height * firstScale));
                selectedTransportQuadtreeItem.reactOnZoom((4.0 * firstScale) / scale);
                    //new Vector2(currentlyEditedIShape.Width * firstScale +       // Should we consider the pointsize here?
                    //    2 * transportPolygonLayer.BbWidthMargin,
                    //currentlyEditedIShape.Height * firstScale +
                    //    2 * transportPolygonLayer.BbHeightMargin));

                return currentlyEditedIShape.getPointListSize() - 1;
            }
            else return -1;
        }
       

        public void removeTransPortComplexShapePoint(int index,ShapeInformation shpInfo,LayerType lType)
        {
            if (shpInfo.iShapeInf != null)
            {
                if ((shpInfo.iShapeInf).getPointListSize() > 0)
                {
                    shpInfo.iShapeInf.RemovePoint(index);
                    if (lType == LayerType.PolygonCanvas)
                        if (shpInfo.iShapeInf.getPointListSize() > 0)
                        {
                            shpInfo.quadTreePosItemInf.changePosAndSize(
                                new Vector2(shpInfo.iShapeInf.CenterX * firstScale,
                                    shpInfo.iShapeInf.CenterY * firstScale),
                                new Vector2(shpInfo.iShapeInf.Width * firstScale,
                                    shpInfo.iShapeInf.Height * firstScale));

                                //new Vector2(shpInfo.iShapeInf.Width * firstScale +       // Should we consider the pointsize here?
                                //    2 * transportPolygonLayer.BbWidthMargin,
                                //shpInfo.iShapeInf.Height * firstScale +
                                //    2 * transportPolygonLayer.BbHeightMargin));
                        }
                        else
                        {
                            shpInfo.quadTreePosItemInf.Delete();
                            /*
                            shpInfo.quadTreePosItemInf.changePosAndSize(
                               new Vector2(-1,-1),
                               new Vector2(1,1));
                             */
                        }
                        else if (lType == LayerType.PolylineCanvas)
                        if (shpInfo.iShapeInf.getPointListSize() > 0)
                        {
                            shpInfo.quadTreePosItemInf.changePosAndSize(
                                new Vector2(shpInfo.iShapeInf.CenterX * firstScale,
                                    shpInfo.iShapeInf.CenterY * firstScale),
                                new Vector2(shpInfo.iShapeInf.Width * firstScale,
                                    shpInfo.iShapeInf.Height * firstScale));
                                //new Vector2(shpInfo.iShapeInf.Width * firstScale +       // Should we consider the pointsize here?
                                //    2 * transportPolylineLayer.BbWidthMargin,
                                //shpInfo.iShapeInf.Height * firstScale +
                                //    2 * transportPolylineLayer.BbHeightMargin));
                        }
                        else shpInfo.quadTreePosItemInf.Delete();
                }
            }
        }

        public void RemoveTransportComplexShape(ShapeInformation shpInfo,LayerType lType)
        { 
            if (shpInfo.quadTreePosItemInf.Parent.Type == ShapeLib.ShapeType.PolyLine)
                transportPolylineLayer.removePolyline(shpInfo.quadTreePosItemInf.Parent);
            else if (shpInfo.quadTreePosItemInf.Parent.Type == ShapeLib.ShapeType.PolyLine)
                transportPolygonLayer.removePolygon(shpInfo.quadTreePosItemInf.Parent);

            shpInfo.quadTreePosItemInf.Delete();

            this.setDrawShapeInformation(DrawShapeInformation.EditStopped, shpInfo, lType);
            selectedTransportQuadtreeItem = null;
        }


        public void getItemsAtPoint(PointD currentPosition, ref List<QuadTreePositionItem<IShape>> itemList)
        {
            itemList.Clear();
            transportLayerQuadtree.GetItems(new Vector2(currentPosition.x, currentPosition.y), ref itemList);
        }

        public void getItemsInRectangle(DRect rect, ref List<QuadTreePositionItem<IShape>> itemList)
        {
            transportLayerQuadtree.GetItems(rect, ref itemList);
        }

        public void removeSelectedTransportShape()
        {
            if (selectedTransportQuadtreeItem != null)
            {
                mainControler.PerformAction(new RemoveShapeAction(selectedTransportQuadtreeItem,this));
				OnShapeRemoved();
            }
        }

        /// <summary>
        /// Moves the currently selected transportShape quadtree item to another position
        /// in the tree.
        /// </summary>
        /// <param name="x">Destination x</param>
        /// <param name="y">Destination y</param>
        /// <param name="differenceMove">Should be true if x and y are differences to the old position, otherwise false.</param>
        //public void moveSelectedTransportQTItem(double x, double y, bool differenceMove)
        //{
        //    selectedTransportQuadtreeItem.move(x, y, differenceMove);
        //    selectedTransportQuadtreeItem.reactOnZoom((4.0 * firstScale) / scale);
        //}

        /// <summary>
        /// Generates and update a reference to a list with the currently visible TransportShapes
        /// </summary>
        /// <param name="dX">dX of the mapPanel - x offset</param>
        /// <param name="dY">dY of the mapPanel - y offset</param>
        /// <param name="width">the width of the drawingArea</param>
        /// <param name="height">the height of the drawingArea</param>
        public void generateCurrentVisibleIShapes(PointD d, double width,
            double height, double absoluteZoom) {

            currentlyVisibleIShapes.Clear();
            this.dX = d.x;
            this.dY = d.y;
            getItemsInRectangle(new DRect(dX / absoluteZoom, -dY / absoluteZoom,
                (dX + width) / absoluteZoom, (-dY + height) / absoluteZoom),
                ref currentlyVisibleIShapes);
        }

        #if DRAW_QUADTREE
                public void draw_QuadTree(Graphics gx, PointD d)
                {
                    double absoluteZoom = scale / firstScale;
                    Rectangle rect = new Rectangle(
                        Convert.ToInt32((transportLayerQuadtree.WorldRect.Left * absoluteZoom) + d.x),
                        Convert.ToInt32((transportLayerQuadtree.WorldRect.Top * absoluteZoom) + d.y),
                        Convert.ToInt32((transportLayerQuadtree.WorldRect.Width) * absoluteZoom),
                        Convert.ToInt32((transportLayerQuadtree.WorldRect.Height) * absoluteZoom));
                    gx.DrawRectangle(new Pen(Color.OrangeRed), rect);

                    List<QuadTreePositionItem<IShape>> list = new List<QuadTreePositionItem<IShape>>();
                    transportLayerQuadtree.GetAllItems(ref list);

                    foreach (QuadTreePositionItem<IShape> item in list)
                    {
                        rect = new Rectangle(
                            Convert.ToInt32((item.Rect.Left * absoluteZoom) + d.x),
                            Convert.ToInt32((item.Rect.Top * absoluteZoom) + d.y),
                            Convert.ToInt32((item.Rect.Width) * absoluteZoom),
                            Convert.ToInt32((item.Rect.Height) * absoluteZoom));

                        gx.DrawRectangle(new Pen(Color.Orange), rect);
                    }

                }
        #endif

        #if DRAW_BOUNDINGBOXES
        public void generateBBList(ref List<Rectangle> returnList, double absoluteZoom)
        {
            int count = currentlyVisibleIShapes.Count;
            returnList.Clear();
            for (int i = count - 1; i >= 0; i--) {
                if (currentlyVisibleIShapes[i].Parent.Type == ShapeLib.ShapeType.MultiPoint) {
                    returnList.Add(currentlyVisibleIShapes[i].Parent.getDisplayBoundingBox(
                        dX, dY, config.ExPLineLayerPointWidth, scale,1));
                } else if (currentlyVisibleIShapes[i].Parent.Type == ShapeLib.ShapeType.PolyLine) {
                    returnList.Add(currentlyVisibleIShapes[i].Parent.getDisplayBoundingBox(
                        dX, dY, config.ExPLineLayerPointWidth, scale,1));
                } else if (currentlyVisibleIShapes[i].Parent.Type == ShapeLib.ShapeType.Polygon) {
                    returnList.Add(currentlyVisibleIShapes[i].Parent.getDisplayBoundingBox(
                        dX, dY, config.ExPGonLayerPointWidth, scale, 1));
                }
            }

        }
        #endif

        /// <summary> 
        /// Returns a List of "circle rectangles" for drawing with the currently visible
        /// transport layer points. Requires that generateCurrentVisibleIShapes() has been
        /// called before!
        /// </summary>
        /// <returns></returns>
        public void generatePointList(ref List<ShapeBBInformation> returnList)
        {
            int count = currentlyVisibleIShapes.Count;
            ShapeBBInformation info;
            returnList.Clear();
            for (int i = count - 1; i >= 0; i--)
                if (currentlyVisibleIShapes[i].Parent.Type == ShapeLib.ShapeType.MultiPoint)
                {
                    info.BoundingBox = currentlyVisibleIShapes[i].Parent.
                        getDisplayBoundingBox(dX,
                        dY,
                        config.ExPntLayerPointWidth,
                        scale, 0);
                    info.Shape = currentlyVisibleIShapes[i].Parent;

                    returnList.Add(info);
                }
        }

        /// <summary> 
        /// Returns a List of point arrays for drawing with the currently visible
        /// transport layer olylines. Requires that generateCurrentVisibleIShapes() has been
        /// called before!
        /// </summary>
        /// <returns></returns>
        public void generatePolylineList(ref List<PolyShapeBBInformation> returnList)
        {
            int count = currentlyVisibleIShapes.Count;
            PolyShapeBBInformation info;

            returnList.Clear();
            for (int i = count - 1; i >= 0; i--)
                if (currentlyVisibleIShapes[i].Parent.Type == ShapeLib.ShapeType.PolyLine)
                {
                    info.Pointlist = currentlyVisibleIShapes[i].Parent.getPointList(
                        Convert.ToInt32(dX),
                        Convert.ToInt32(dY),
                        scale);
                    info.Shape = currentlyVisibleIShapes[i].Parent;
                    info.BoundingBox = info.Shape.getDisplayBoundingBox(dX, dY,
                        config.ExPLineLayerPointWidth, scale, 0);
                    returnList.Add(info);
                }
        }

        /// <summary> 
        /// Returns a List of point arrays for drawing with the currently visible
        /// transport layer polygons. Requires that generateCurrentVisibleIShapes() has been
        /// called before!
        /// </summary>
        /// <returns></returns>
        public void generatePolygonList(ref List<PolyShapeBBInformation> returnList)
        {
            int count = currentlyVisibleIShapes.Count;
            PolyShapeBBInformation info;

            returnList.Clear();
            for (int i = count - 1; i >= 0; i--)
                if (currentlyVisibleIShapes[i].Parent.Type == ShapeLib.ShapeType.Polygon)
                {
                    info.Pointlist = currentlyVisibleIShapes[i].Parent.getPointList(
                        Convert.ToInt32(dX),
                        Convert.ToInt32(dY),
                        scale);
                    info.Shape = currentlyVisibleIShapes[i].Parent;
                    info.BoundingBox = info.Shape.getDisplayBoundingBox(dX, dY,
                        config.ExPGonLayerPointWidth, scale, 0);

                    returnList.Add(info);
                }
        }

        public string CategoyOfSelevtedShape
        {
            get { return selectedTransportQuadtreeItem.Parent.Category; }
            set { selectedTransportQuadtreeItem.Parent.Category = value; }
        }

        public string CommentOfSelectedShape
        {
            get { return selectedTransportQuadtreeItem.Parent.Commment;  }
            set { selectedTransportQuadtreeItem.Parent.Commment = value; }
        }

        public IntPtr CreateNewGDALContainer(int width, int height)
        {
            if(cGDALContainer == IntPtr.Zero) cGDALContainer = MapPanelBindings.InitGDAL(width, height, scale);           
            if(cGDALContainer == IntPtr.Zero) MessageBox.Show("Fehler 0x4000: Konnte keinen C++ GDAL-Container erstellen!");
            return cGDALContainer;
        }

        public IntPtr CGDALContainer
        {
            get { return cGDALContainer; }
        }

        public GravurGIS.MapPanelBindings.ImageLayerInfo GDALAddImageToContainer(string filename)
        {
            IntPtr id = MapPanelBindings.AddFileToGDALContainer(cGDALContainer, filename);
            GravurGIS.MapPanelBindings.ImageLayerInfo ilinfo = new GravurGIS.MapPanelBindings.ImageLayerInfo();
            Marshal.PtrToStructure(id, ilinfo);

            if (ilinfo.id == -1)
                MessageBox.Show("Fehler 0x4001: Konnte dem GDALContainer die Datei \"" + filename + "\" nicht hinzufügen!");
            return ilinfo;
        }

        public IntPtr CreateNewOGRContainer(int width, int height)
        {
            if (cOGRContainer == IntPtr.Zero) cOGRContainer = MapPanelBindings.InitOGR(width, height);
            if (cOGRContainer == IntPtr.Zero) MessageBox.Show("Fehler 0x4210: Konnte keinen C++ OGR-Container erstellen!");
            return cOGRContainer;
        }

        public GravurGIS.MapPanelBindings.VectorLayerInfo OGRAddLayerToContainer(string filename, string SpatialReference)
        {
            IntPtr id = MapPanelBindings.AddFileToOGRContainer(cOGRContainer, filename, SpatialReference);
            GravurGIS.MapPanelBindings.VectorLayerInfo vlinfo = new GravurGIS.MapPanelBindings.VectorLayerInfo();
            Marshal.PtrToStructure(id, vlinfo);

            if (vlinfo.id == -1)
                MessageBox.Show("Fehler 0x4001: Konnte dem OGRContainer die Datei \"" + filename + "\" nicht hinzufügen!");
            return vlinfo;
        }

        public void resetLayers()
        {
            for (int i = 0; i < LayerCount; i++) {
                layerArray[i].reset();
            }
        }

        public void addMandelbrotLayer(double startDX, double startDY, int width, int height,
            int maxIterations, double xPos, double yPos, double size)
        {
            IntPtr p_mbl = MapPanelBindings.GetNewMandelbrotLayer(width, height);

            MandelbrotLayer mbl = new MandelbrotLayer(
                p_mbl, startDX, startDY, width, height, maxIterations, xPos, yPos, size);
            layerListViewMappingList.Add(LayerCount, LayerCount); // ListView-Index, Layer-Index
            layerArray.Add(mbl);
            addLayerPostProcessing(mbl);
        }

        public void addGPSLayer()
        {
            if (gpsLayer == null)
                gpsLayer = new GPSLayer(this.mainControler);
        }

        public GKCoord CurrentGPSPositon
        {
            get
            {
                if (gpsLayer != null)
                    return gpsLayer.CurrentPositon;
                else
                    throw new ApplicationException("GPSLayer ist nicht aktiviert");
            }
        }

        public void addMapserverLayer(MapServerLayer mapserverLayer)
        {
            layerListViewMappingList.Add(LayerCount, LayerCount);
            layerArray.Add(mapserverLayer);
            
            if (LayerCount == 1)
                this.scale = this.firstScale = mainControler.MapPanel.calculateZoomFactor(mapserverLayer.Width, mapserverLayer.Height);

            if (LayerCount == 1) {
                mainControler.createNewCanvas(
                    mapserverLayer.BoundingBox.Left * firstScale,
                    mapserverLayer.BoundingBox.Top * firstScale, scale);
                mainControler.MapPanel.onLayerZoom(0);
            }

            addLayerPostProcessing(mapserverLayer);
        }

        public void removeGPSLayer()
        {
            // gpsLayer = null;
            // Do here nothing - if GPSLayer was already initializes we do not throw it away for now
        }

        public void ClearTransportLayer(LayerType type)
        {
            switch (type)
            {
                case LayerType.PointCanvas:
                    if (this.transportPointLayer != null) transportPointLayer.clear();
                    break;
                case LayerType.PolylineCanvas:
                    if (this.transportPolylineLayer != null) transportPolylineLayer.clear();
                    break;
                case LayerType.PolygonCanvas:
                    if (this.transportPolygonLayer != null) transportPolygonLayer.clear();
                    break;
            }
        }
        void extendWorldBBox(Layer newLayer)
        {
            //if (wordBBox == null)
            //    wordBBox = newLayer.BoundingBox;
            //else{
            //    DRect newLayerBBox = newLayer.BoundingBox;
            //    wordBBox.Left   = Math.Min(wordBBox.Left,   newLayerBBox.Left);
            //    wordBBox.Bottom = Math.Min(wordBBox.Bottom, newLayerBBox.Bottom);
            //    wordBBox.Right  = Math.Min(wordBBox.Right,  newLayerBBox.Right);
            //    wordBBox.Top    = Math.Min(wordBBox.Top,    newLayerBBox.Top);
            //}
        }

        private DRect computeWordBBox()
        {
            Vector2 tempMin = new Vector2(Double.MaxValue, Double.MaxValue);
            Vector2 tempMax = new Vector2(Double.MinValue, Double.MinValue);

            for (int i = 0; i < LayerCount; ++i)
            {
                tempMin.X = Math.Min(tempMin.X, layerArray[i].BoundingBox.Left);
                tempMin.Y = Math.Min(tempMin.Y, layerArray[i].BoundingBox.Bottom);
                tempMax.X = Math.Max(tempMax.X, layerArray[i].BoundingBox.Right);
                tempMax.Y = Math.Max(tempMax.Y, layerArray[i].BoundingBox.Top);
            }
            //if (transportPointLayer.Count > 0)
            //{
            //    tempMin.X = Math.Min(transportPointLayer.BoundingBox.Left, tempMin.X);                  
            //    tempMin.Y = Math.Min(transportPointLayer.BoundingBox.Bottom,tempMin.Y);
            //    tempMax.X = Math.Max(transportPointLayer.BoundingBox.Top, tempMax.X);
            //    tempMax.Y = Math.Max(transportPointLayer.BoundingBox.Right, tempMax.Y);
            //}
            //if (transportPolygonLayer.Count > 0)
            //{
            //    tempMin.X = Math.Min(transportPolygonLayer.BoundingBox.Left, tempMin.X);
            //    tempMin.Y = Math.Min(transportPolygonLayer.BoundingBox.Bottom, tempMin.Y);
            //    tempMax.X = Math.Max(transportPolygonLayer.BoundingBox.Top, tempMax.X);
            //    tempMax.Y = Math.Max(transportPolygonLayer.BoundingBox.Right, tempMax.Y);
            //}
            //if (transportPolylineLayer.Count > 0)
            //{
            //    tempMin.X = Math.Min(transportPolylineLayer.BoundingBox.Left, tempMin.X);
            //    tempMin.Y = Math.Min(transportPolylineLayer.BoundingBox.Bottom, tempMin.Y);
            //    tempMax.X = Math.Max(transportPolylineLayer.BoundingBox.Top, tempMax.X);
            //    tempMax.Y = Math.Max(transportPolylineLayer.BoundingBox.Right, tempMax.Y);
            //}
            return new DRect(tempMin, tempMax);
        }

        #region Getter/Setter

        public List<Layer> LayerArray
        {
            get { return layerArray; }
        }
        public int LayerCount
        {
            get { return layerArray.Count; }
        }
        /// <summary>
        /// Gets a layer by the list view index
        /// </summary>
        /// <param name="index"></param>
        /// <returns>the layer</returns>
        public Layer getLayerFromMapping(int index)
        {
            return layerArray[layerListViewMappingList[index]];
        }
        public int getLayerIndexFromMapping(int index)
        {
            return layerListViewMappingList[index];
        }

        public double Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                double absoluteZoom = scale / firstScale;
                ScaleChanged(scale, absoluteZoom);
            }
        }
        public SortedList<int, int> ShpListMappingList
        {
            get { return layerListViewMappingList; }
            set { layerListViewMappingList = value; }
        }
        public double FirstScale
        {
            get { return firstScale; }
        }
        public byte RefLayer
        {
            get { return refLayer; }
        }
        internal TransportMultiPointLayer TransportPointLayer
        {
            get { return transportPointLayer; }
        }
        public QuadTreePositionItem<IShape> SelectedTransportQuadtreeItem
        {
            get { return selectedTransportQuadtreeItem; }
            set { selectedTransportQuadtreeItem = value; }
        }
        public int TransportMultipointBBWidth
        {
            get { return transportPointLayer.BbWidthMargin; }
        }
        public int TransportMultipointBBHeight
        {
            get {return transportPointLayer.BbHeightMargin;}
        }
        internal TransportPolylineLayer TransportPolylineLayer
        {
            get { return transportPolylineLayer; }
        }
        internal TransportPolygonLayer TransportPolygonLayer
        {
            get { return transportPolygonLayer; }
        }

        public GPSLayer GPSLayer
        {
            get { return gpsLayer; }
        }

        public int PointSize
        {
            get { return pointSize; }
        }
        public int ShpSize
        {
            get { return defaultShapeSize; }
        }
        public MainControler GetMainControler() {
            return mainControler;
        }
        public GravurGIS.Styles.NewLayerStyle.INewLayerStyle NewLayerStyle
        {
            get { return newLayerStyle; }
            set { newLayerStyle = value; }
        }

        /// <summary>
        /// provides Information about shape, which is drawed currently
        /// </summary>
        /// <returns>don't forget testing return values against null!</returns>
        public ShapeInformation getDrawShapeInformation()
        {   
            ShapeInformation shpInfo;
            shpInfo.iShapeInf = this.currentlyEditedIShape;
            shpInfo.quadTreePosItemInf = this.selectedTransportQuadtreeItem;
            return shpInfo;
        }

        public void setDrawShapeInformation(DrawShapeInformation drawEnum,ShapeInformation shpInfo,LayerType lType)
        {
            
               
            this.currentlyEditedIShape = shpInfo.iShapeInf;
            this.selectedTransportQuadtreeItem = shpInfo.quadTreePosItemInf;

            this.DrawInfChanged(drawEnum,lType);
        }
        public double FirstMapMeassure
        { get { return this.dFirstMapMeassure; } }

        

        public DRect getWorldBBox()
        {
            return computeWordBBox();
        }

        public double getMaxAbsoluteZoom()
        {
            DRect worldBox = computeWordBBox();
            return Int32.MaxValue / (FirstScale * Math.Max(worldBox.Right, worldBox.Top));
        }
        #endregion
    }
}

