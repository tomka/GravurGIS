using System;

namespace GravurGIS.Styles
{
    /// <summary>
    /// Represents a brush which fills a region with a single, solid color.
    /// </summary>
    public class SolidStyleBrush : StyleBrush
    {
        /// <summary>
        /// Creates an instance of a <see cref="SolidStyleBrush"/> with the given color.
        /// </summary>
        /// <param name="color">The color of the brush.</param>
        public SolidStyleBrush(StyleColor color)
            : base(color)
        {
        }

        public override String ToString()
        {
            return String.Format("[SolidStyleBrush] Color: {0}", Color);
        }
    }
}