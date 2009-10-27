using System;
using MapTools;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using PDAShpTool.Shapes;
using System.Drawing;
using PDAShpTool.Topology.QuadTree;
using PDAShpTool.Topology.Grid;
using PDAShpTool.Topology;

namespace PDAShpTool
{
    public class ShapeObject
    {
        #region Properties

        // Shape related
        private ShapeLib.ShapeType shapeType;
        
        private double[] minB;
        private double[] maxB;
       
        private bool isPoint = false;

        private String access;
        private int numberOfShapes;
        private IntPtr hShp;
        private bool hShpIsOpen;
        public String comment;

        // Daten
        private PointList[] polyLinePointList;
        private bool useSeperateObjects = false;
        private Grid partGrid;
        private int partCount;

        // Framework      
        private LayerManager layerManager;
        private String name;
       
        private bool isShapeInfoRead;
       
       
        // Display
        /// <summary>
        /// The scaled, but un-zooomed, most left y-coordinate of the shape
        /// </summary>
        public double relativeLeft;
        /// <summary>
        /// The scaled, but un-zoomed, top most x-coordinate of the shape
        /// </summary>
        public double relativeTop;
        public double relLayerZoom = 1;
        
        //mwd Attribute
        private Color fillColor = Color.White;
        private Color pointColor = Color.Black;
        private bool basePoints = false;
        private String filePath;
        private String fileName;
       
       
        private Pen shapePen;
        public bool visible = true;

        public bool changed = true;

        #endregion

        public ShapeObject(LayerManager layerManager, String filePath) {
            this.layerManager = layerManager;
            this.filePath = filePath;
            this.fileName = filePath.Split('\\')[filePath.Split('\\').Length-1];
            
            this.access = "rb";
            this.shapePen = new Pen(layerManager.GetRandomColor(), 1.0f);

            int i = filePath.LastIndexOf("\\");

            name = filePath.Substring(i + 1,
                filePath.LastIndexOf(".") - i - 1);
        }

        public void init()
        {
            this.hShpIsOpen = false;
            this.isShapeInfoRead = false;
            layerManager.generateGeometryStructureInDLL(this, ref partGrid);
        }

        public String getFilePath()
        {
            return filePath;
        }

        private void generateShapeInfo()
        {
            if (!this.hShpIsOpen)
                hShp = ShapeLib.SHPOpenW(this.filePath, this.access);
            if (hShp.Equals(IntPtr.Zero))
                MessageBox.Show("Oops - hShp = 0");
            else
            {
                // get shape info and verify shapes were created correctly
                double[] minB = new double[4];
                double[] maxB = new double[4];
                int nEntities = 0;
                ShapeLib.ShapeType shapeType = ShapeLib.ShapeType.PolyLine;
                ShapeLib.SHPGetInfo(hShp, ref nEntities, ref shapeType, minB, maxB);
                this.numberOfShapes = nEntities;
                this.shapeType = shapeType;
                this.minB = minB;
                this.maxB = maxB;

                ShapeLib.SHPClose(hShp);
                this.hShpIsOpen = false;
                this.isShapeInfoRead = true;
            }
        }

        public String showShapeInfo()
        {
            if (!this.isShapeInfoRead)
                generateShapeInfo();

            return "Einträge: " + this.numberOfShapes.ToString()
                + "\nTyp: " + this.shapeType.ToString() + "\nMin XY: " + this.minB[0].ToString() + ", " + this.minB[1].ToString()
                + "\nMax XY: " + this.maxB[0].ToString() + ", " + this.maxB[1].ToString()
                + "\n\nScale: " + layerManager.Scale;
        }
    
        #region Getter/Setter
        
        public bool UseSeperateObjects
        {
            get { return useSeperateObjects; }
        }
        public PointList[] PolyLinePointList
        {
            get { return polyLinePointList; }
            set { polyLinePointList = value; }
        }
        public int NumberOfShapes
        {
            get { return numberOfShapes; }
            set { numberOfShapes = value; }
        }
        public Color LineColor
        {
            get { return shapePen.Color; }
            set { shapePen.Color = value; }
        }
        public Color PointColor
        {
            get { return pointColor; }
            set { pointColor = value; }
        }
        public Color FillColor
        {
            get { return fillColor; }
            set { fillColor = value; }
        }
        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        /// <summary>
        /// The unscaled (!) width of the layer
        /// </summary>
        public double Width
        {
            get 
            {
                if ((maxB[0] - minB[0]) != 0) return maxB[0] - minB[0];
                else return layerManager.ShpSize;        
            }
        }
        /// <summary>
        /// The unscaled (!) height of the layer
        /// </summary>
        public double Height
        {
            get
            {
                if ((maxB[1] - minB[1]) != 0) return maxB[1] - minB[1];
                else return layerManager.ShpSize;
            }
        }
        /// <summary>
        /// unscaled minimal Point of Bounding Box: {x, y, z, m}
        /// </summary>
        public double[] MinB
        {
            get { return minB; }
            set { minB = value; }
        }
        /// <summary>
        /// unscaled maximal point of bounding Box: {x, y, z, m}
        /// </summary>
        public double[] MaxB
        {
            get { return maxB; }
            set { maxB = value; }
        }
        public ShapeLib.ShapeType ShapeType
        {
            get { return shapeType; }
            set { shapeType = value; }
        }
        public String FileName
        {
            get { return fileName; }
        }

        public String FilePath
        {
            get { return filePath; }
        }

        public bool BasePoints
        {
            get { return basePoints; }
        }

        public Pen ShapePen
        {
            get { return shapePen; }
        }

        
        public String Access
        {
            get { return access; }
        }
        public Grid PartGrid
        {
            get { return partGrid; }
        }
        public int PartCount
        {
            get { return partCount; }
            set { partCount = value; }
        }

        public bool IsOnePoint
        {
            get { return isPoint; }
            set { isPoint = value; }
        }

        #endregion

    }
}
