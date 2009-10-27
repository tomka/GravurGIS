using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace GravurGIS.Styles.NewLayerStyle
{
    public enum NewLayerStyles
    {
        RandomColor, SpecificColor
    }
    /// <summary>
	/// Defines an interface for defining the color of a new layer
	/// </summary>
	public interface INewLayerStyle
	{
		/// <summary>
		/// Gets a new Color according to the implementation
		/// </summary>
        Color NewColor
        {
            get;
        }
	}
}