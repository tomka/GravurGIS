using System;
using System.Collections.Generic;
using System.Text;
using GravurGIS.Data;
using GravurGIS.Rendering;

namespace GravurGIS.Layers
{
    public class LabelLayer : Layer
    {
        //private LayerManager lm;
        private ILayerProvider _DataSource = null;
        private string _LabelColumn;
        private string _RotationColumn;
        private int _Priority;


        public LabelLayer(String name, ILayerProvider dataSource)
        {
            this.Visible = true;
            this._DataSource = dataSource;
            this.LayerName = name;
            this.Style = new GravurGIS.Styles.LabelStyle();
        }

        ///// <summary>
        ///// Gets or sets the datasource
        ///// </summary>
        //public new ILayerProvider DataSource
        //{
        //    get { return _DataSource; }
        //    set { _DataSource = value; }
        //}

        /// <summary>
        /// Data column or expression where label text is extracted from.
        /// </summary>
        /// <remarks>
        /// This property is overriden by the <see cref="LabelStringDelegate"/>.
        /// </remarks>
        public string LabelColumn
        {
            get { return _LabelColumn; }
            set { _LabelColumn = value; }
        }

        /// <summary>
        /// Data column from where the label rotation is derived.
        /// If this is empty, rotation will be zero, or aligned to a linestring.
        /// Rotation are in degrees (positive = clockwise).
        /// </summary>
        public string RotationColumn
        {
            get { return _RotationColumn; }
            set { _RotationColumn = value; }
        }

        /// <summary>
        /// A value indication the priority of the label in cases of label-collision detection
        /// </summary>
        public int Priority
        {
            get { return _Priority; }
            set { _Priority = value; }
        }


        public override bool Render(RenderProperties rp)
        {
            //if (this.Visible && Style.Enabled && this.Style.MaxVisible >= absoluteZoom && this.Style.MinVisible <= absoluteZoom)
            //{
            //    if (this.DataSource == null)
            //        throw (new ApplicationException("DataSource property not set on layer '" + this.LayerName + "'"));

            //}
            return true;
        }

        public override void reset()
        {
            throw new NotImplementedException();
        }

        public override void recalculateData(double absoluteZoom, double scale, double xOff, double yOff)
        {
            throw new NotImplementedException();
        }
    }
}
