using System;
using System.Collections.Generic;
using System.Text;

namespace GravurGIS.Styles
{
    /// <summary>
    /// Represents an abstract brush to fill regions with color or patterns.
    /// </summary>
    public abstract class StyleBrush
    {
        private StyleColor _color;

        /// <summary>
        /// Creates a new instance of a StyleBrush with a transparent color.
        /// </summary>
        protected StyleBrush() : this(StyleColor.Transparent) { }

        /// <summary>
        /// Creates a new instance of a StyleBrush with the given <paramref name="color"/>.
        /// </summary>
        /// <param name="color">Base color of the brush.</param>
        public StyleBrush(StyleColor color)
        {
            _color = color;
        }

        /// <summary>
        /// Gets or sets the base color of the brush.
        /// </summary>
        public StyleColor Color
        {
            get { return _color; }
            set { _color = value; }
        }

        /// <summary>
        /// Generates a hash code for use in hashtables and dictionaries.
        /// </summary>
        /// <returns>An integer usable as a hash value in hashtables.</returns>
        public override Int32 GetHashCode()
        {
            return 7602046 ^ _color.GetHashCode();
        }
    }
}
