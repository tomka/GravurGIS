namespace GravurGIS.Styles
{
    /// <summary>
    /// An enumeration of the types of dash styles available to a StylePen.
    /// </summary>
    public enum LineDashStyle
    {
        /// <summary>
        /// Draws a solid line.
        /// </summary>
        Solid,

        /// <summary>
        /// Draws a dashed line.
        /// </summary>
        Dash
            
        /* we can currently not use this

        /// <summary>
        /// Draws a dotted line.
        /// </summary>
        Dot,

        /// <summary>
        /// Draws an alternating dash and dot.
        /// </summary>
        DashDot,

        /// <summary>
        /// Draws an alternating dash and two dots.
        /// </summary>
        DashDotDot,

        /// <summary>
        /// Uses the <see cref="StylePen.DashPattern"/> values for the dash style.
        /// </summary>
        Custom
        
         */
    }
}