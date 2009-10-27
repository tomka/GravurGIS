using System;
using System.Drawing;
using GravurGIS.Topology;
using GravurGIS.Data;
using System.ComponentModel;
using GravurGIS.Styles;
using GravurGIS.Rendering;

namespace GravurGIS.Layers
{
    public abstract class Layer : ILayer
    {
        private static readonly PropertyDescriptorCollection _properties;

        static Layer()
        {
            _properties = TypeDescriptor.GetProperties(typeof(Layer));

        }
        #region PropertyDescriptors
        /// <summary>
        /// Gets a PropertyDescriptor for Layer's <see cref="Enabled"/> property.
        /// </summary>
        public static PropertyDescriptor EnabledProperty
        {
            get { return _properties.Find("Enabled", false); }
        }

        /// <summary>
        /// Gets a PropertyDescriptor for Layer's <see cref="LayerName"/> property.
        /// </summary>
        public static PropertyDescriptor LayerNameProperty
        {
            get { return _properties.Find("LayerName", false); }
        }

        /// <summary>
        /// Gets a PropertyDescriptor for Layer's <see cref="Style"/> property.
        /// </summary>
        public static PropertyDescriptor StyleProperty
        {
            get { return _properties.Find("Style", false); }
        }

        #endregion

        #region Instance fields
        
        //private readonly ILayerProvider _dataSource;
        private String _layerName;
        private IStyle _style;

        #endregion

        protected LayerType _layerType = LayerType.Undefined;
        protected WorldBoundingBoxD _boundingBox = new WorldBoundingBoxD();

        public abstract bool Render(RenderProperties rp);

        public abstract void reset();

        public abstract void recalculateData(double absoluteZoom, double scale, double xOff, double yOff);

        public LayerType Type { get { return _layerType; } }
        public String Description { get; set; }

        public void Refresh(){
            this.Changed = true;
        }

        /// <summary>
        /// Gets or sets the name of the layer.
        /// </summary>
        public String LayerName
        {
            get { return _layerName; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("LayerName must not be null or empty.");
                }

                if (_layerName == value)
                {
                    return;
                }

                _layerName = value;
                OnLayerNameChanged();
            }
        }

        /// <summary>
        /// Gets or sets the style for the layer.
        /// </summary>
        public IStyle Style
        {
            get { return _style; }
            set
            {
                if (_style == value)
                {
                    return;
                }

                _style = value;
                OnStyleChanged();
            }
        }


        public String Comment
        {
            get;
            set;
        }

        public Boolean Visible
        {
            get;
            set;
        }

        /// <summary>
        /// The unscaled width of the layer
        /// </summary>
        public double Width
        {
            get { return this._boundingBox.Width; }
        }

        /// <summary>
        /// The unscaled height of the layer
        /// </summary>
        public double Height
        {
            get { return this._boundingBox.Height; }
        }

        /// <summary>
        /// Indicates whether the layer has changed and needs a redraw or not
        /// </summary>
        public bool Changed
        {
            get;
            set;
        }

        public WorldBoundingBoxD BoundingBox
        {
            get { return this._boundingBox; }
        }

        public virtual Boolean AreFeaturesSelectable
        {
            get
            {
                return false;
            }
            set
            {
                ; // do nothing
            }
        }

        ///// <summary>
        ///// Gets the data source used to create this layer.
        ///// </summary>
        //public ILayerProvider DataSource
        //{
        //    get { return _dataSource; }
        //}

        #region Protected members

        protected virtual void OnEnabledChanged()
        {
            OnPropertyChanged(EnabledProperty.Name);
        }


        protected virtual void OnLayerNameChanged()
        {
            OnPropertyChanged(LayerNameProperty.Name);
        }

        protected virtual void OnStyleChanged()
        {
            OnPropertyChanged(StyleProperty.Name);
        }
        #endregion


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region OnPropertyChanged

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property changed.</param>
        protected virtual void OnPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler e = PropertyChanged;

            if (e != null)
            {
                PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
                e(this, args);
            }
        }

        #endregion
    }
    /// <summary>
    /// those information are relevant for types of layers, which can be saved in an project file, e.g. ShapeObject and Image
    /// </summary>
    public class LayerInfo
    {
        private String filePath;
        private String fileName;

        public LayerInfo()
        {
            this.filePath = "";
            this.fileName = "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath">path contains directories + filename + extension</param>
        public LayerInfo(String filePath)
        {
            this.fileName = System.IO.Path.GetFileName(filePath);
            this.filePath = filePath;
        }

        public String FileName
        {
            get { return System.IO.Path.GetFileName(filePath); }
        }

        public String FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }
    }
    /// <summary>
    /// those information are relevant for types of layers, which cope with vector data e.g. ShapeObject
    /// </summary>
    public class VectorInfo
    {
        private Pen shapePen;
        private Color fillColor;
        private Color pointColor;
        private bool useFillColor;

        public VectorInfo()
        {
            this.shapePen = new Pen(Color.Black);
            this.fillColor = Color.White;
            this.pointColor = Color.Black;
            this.useFillColor = false;
            this.IsVisible = true;
        }

        public VectorInfo(Color pointColor, Color fillColor, Pen layerPen)
        {
            this.fillColor = fillColor;
            this.pointColor = pointColor;
            this.shapePen = layerPen;
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
        public Boolean Fill
        {
            get { return useFillColor; }
            set { useFillColor = value; }
        }

        public Pen LayerPen
        {
            get { return shapePen; }
        }
        public Boolean IsVisible { get; set;}

    }
}
