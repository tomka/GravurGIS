using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using GravurGIS.Topology;
using GravurGIS.Data;
using GravurGIS.Styles;

namespace GravurGIS.Layers
{
    public enum LayerType
    {
        Shape, Image, PointCanvas, PolylineCanvas, PolygonCanvas, GPSPosition, MapServer, OGRLayer, Undefined
    }

    public interface ILayer
    {
        void Refresh();

        LayerType Type
        {
            get;
        }

        ///// <summary>
        ///// Gets the data source used to create this layer.
        ///// </summary>
        //ILayerProvider DataSource { get; }

        /// <summary>
        /// The style for the layer.
        /// </summary>
        IStyle Style { get; set; }
        
        String Description
        {
            get;
            set;
        }

        String LayerName
        {
            get;
            set;
        }

        String Comment
        {
            get;
            set;
        }

        Boolean Visible
        {
            get;
            set;
        }

        /// <summary>
        /// The unscaled width of the layer
        /// </summary>
        double Width
        {
            get;
        }

        /// <summary>
        /// The unscaled height of the layer
        /// </summary>
        double Height
        {
            get;
        }

        /// <summary>
        /// Indicates whether the layer has changed and needs a redraw or not
        /// </summary>
        bool Changed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the extends of the layer in world coordinates
        /// </summary>
        WorldBoundingBoxD BoundingBox
        {
            get;
        }

        Boolean AreFeaturesSelectable { get; set; }
    }
}
