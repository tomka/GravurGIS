namespace GravurGIS.Styles
{
    /// <summary>
    /// Describes how to form the end of a dash in a drawn line.
    /// </summary>
    public enum LineDashCap
    {
        /// <summary>
        /// Makes the cap of the dash flat, so it is square to the sides of the dash.
        /// </summary>
        Flat = 0

        /* We can currently not use this
        /// <summary>
        /// Makes the cap of the dash rounded, so it forms an arc from the sides
        /// of the dash.
        /// </summary>
        Round = 2,

        /// <summary>
        /// Makes each side of the cap of the dash an obtuse angle 
        /// from the side of the dash, so a triangle is formed for the cap
        /// when they meet.
        /// </summary>
        Triangle = 3
         
        */
    }
}