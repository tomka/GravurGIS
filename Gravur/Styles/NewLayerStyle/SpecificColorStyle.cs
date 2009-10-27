using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace GravurGIS.Styles.NewLayerStyle
{
    public class SpecificColorStyle : INewLayerStyle
    {
        private Color _color;
        #region INewLayerStyle Members

        public SpecificColorStyle(Color color)
        {
            this._color = color;
        }

        public System.Drawing.Color NewColor
        {
            get
            {
                return _color;
            }
        }

        #endregion
    }
}
