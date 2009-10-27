using System;
//using SharpMap.Rendering.Rendering2D;

namespace GravurGIS.Styles
{
	/// <summary>
	/// Defines a style used for rendering a geometry.
	/// </summary>
	public class GeometryStyle : Style
	{
        #region Private fields
        private StylePen _lineStyle;
        private StylePen _highlightLineStyle;
        private StylePen _selectLineStyle;
        private Boolean _outline;
        private StylePen _outlineStyle;
        private StylePen _highlightOutlineStyle;
        private StylePen _selectOutlineStyle;
        private StyleBrush _fillStyle;
        private StyleBrush _highlightFillStyle;
        private StyleBrush _selectionFillStyle;

        //private Symbol2D _symbol;
        //private Symbol2D _highlightSymbol;
        //private Symbol2D _selectSymbol;

		#endregion

		/// <summary>
		/// Initializes a new VectorStyle with default values.
		/// </summary>
		/// <remarks>
		/// Default style values when initialized:<br/>
		/// <list type="table">
		/// <item>
		/// <term>AreFeaturesSelectable</term>
		/// <description>True</description>
		/// </item>
		/// <item>
		/// <term>LineStyle</term>
		/// <description>1px solid black</description>
		/// </item>
		/// <item>
		/// <term>FillStyle</term>
		/// <description>Solid black</description>
		/// </item>
		/// <item>
		/// <term>Outline</term>
		/// <description>No Outline</description>
		/// </item>
		/// <item>
		/// <term>Symbol</term>
		/// <description>Null reference (uses the geometry renderer default)</description>
		/// </item>
		/// </list>
		/// </remarks>
        public GeometryStyle()
		{
			Outline = new StylePen(StyleColor.Black, 1);
			Line = new StylePen(StyleColor.Black, 1);
			Fill = new SolidStyleBrush(StyleColor.Black);
			EnableOutline = false;
		}

        #region Properties

        /// <summary>
        /// Linestyle for line geometries
        /// </summary>
        public StylePen Line
        {
            get { return _lineStyle; }
            set { _lineStyle = value; }
        }

        /// <summary>
        /// Highlighted line style for line geometries
        /// </summary>
        public StylePen HighlightLine
        {
            get { return _highlightLineStyle; }
            set { _highlightLineStyle = value; }
        }

        /// <summary>
        /// Selected line style for line geometries
        /// </summary>
        public StylePen SelectLine
        {
            get { return _selectLineStyle; }
            set { _selectLineStyle = value; }
        }

        /// <summary>
        /// Specified whether the objects are rendered with or without outlining
        /// </summary>
        public Boolean EnableOutline
        {
            get { return _outline; }
            set { _outline = value; }
        }

        /// <summary>
        /// Normal outline style for line and polygon geometries
        /// </summary>
        public StylePen Outline
        {
            get { return _outlineStyle; }
            set { _outlineStyle = value; }
        }

        /// <summary>
        /// Highlighted outline style for line and polygon geometries
        /// </summary>
        public StylePen HighlightOutline
        {
            get { return _highlightOutlineStyle; }
            set { _highlightOutlineStyle = value; }
        }

        /// <summary>
        /// Selected outline style for line and polygon geometries.
        /// </summary>
        public StylePen SelectOutline
        {
            get { return _selectOutlineStyle; }
            set { _selectOutlineStyle = value; }
        }

        /// <summary>
        /// Fill style for closed geometries.
        /// </summary>
        public StyleBrush Fill
        {
            get { return _fillStyle; }
            set { _fillStyle = value; }
        }

        /// <summary>
        /// Fill style for closed geometries when they are in a selected state.
        /// </summary>
        public StyleBrush SelectFill
        {
            get { return _selectionFillStyle; }
            set { _selectionFillStyle = value; }
        }

        /// <summary>
        /// Gets or sets a fill style for closed geometries when they are in a highlighted state.
        /// </summary>
        public StyleBrush HighlightFill
        {
            get { return _highlightFillStyle; }
            set { _highlightFillStyle = value; }
        }

		/// <summary>
        /// Gets or sets a symbol used for rendering point features.
		/// </summary>
        //public Symbol2D Symbol
        //{
        //    get { return _symbol; }
        //    set { _symbol = value; }
        //}

		/// <summary>
        /// Gets or sets a symbol used for rendering highlighted point features.
		/// </summary>
        //public Symbol2D HighlightSymbol
        //{
        //    get { return _highlightSymbol; }
        //    set { _highlightSymbol = value; }
        //}

		/// <summary>
        /// Gets or sets a symbol used for rendering selected point features.
		/// </summary>
        //public Symbol2D SelectSymbol
        //{
        //    get { return _selectSymbol; }
        //    set { _selectSymbol = value; }
        //}

		#endregion
	}
}