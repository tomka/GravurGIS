using System;
using System.Collections.Generic;
using System.Text;

namespace GravurGIS.Styles.NewLayerStyle
{
    public class RandomColorStyle : INewLayerStyle
    {
        #region INewLayerStyle Members

        public System.Drawing.Color NewColor
        {
            get
            {
                Random random = new Random();
                return System.Drawing.Color.FromArgb(random.Next(230), random.Next(230), random.Next(230));
            }
        }

        #endregion
    }
}
