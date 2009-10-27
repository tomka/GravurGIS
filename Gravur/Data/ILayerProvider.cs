using System;
//using GeoAPI.CoordinateSystems;
//using GeoAPI.CoordinateSystems.Transformations;
//using GeoAPI.Geometries;

namespace GravurGIS.Data
{
    /// <summary>
    /// Interface for layer data providers.
    /// </summary>
    public interface ILayerProvider : IDisposable
    {
        #warning ICoordinateTransformation/ICoordinateSystem/IExtents auskommentiert, da noch nicht genutzt
        /// <summary>
        /// Applies a coordinate transformation to the geometries in 
        /// this provider.
        /// </summary>
        //ICoordinateTransformation CoordinateTransformation { get; set; }

        /// <summary>
        /// The dataum, projection and coordinate system used in this 
        /// provider.
        /// </summary>
        //ICoordinateSystem SpatialReference { get; }

        /// <summary>
        /// Returns true if the datasource is currently open.
        /// </summary>
        Boolean IsOpen { get; }

        /// <summary>
        /// The spatial reference ID for the provider.
        /// </summary>
        Int32? Srid { get; set; }

        /// <summary>
        /// Geometric extent of the entire dataset.
        /// </summary>
        /// <returns>The extents of the dataset as a BoundingBox.</returns>
        //IExtents GetExtents();

        /// <summary>
        /// Gets the connection ID of the datasource.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The ConnectionId should be unique to the datasource 
        /// (for instance the filename or the connectionstring), and is meant 
        /// to be used for connection pooling.
        /// </para>
        /// <para>
        /// If connection pooling doesn't apply to this datasource, 
        /// the ConnectionId should return String.Empty.
        /// </para>
        /// </remarks>
        String ConnectionId { get; }

        /// <summary>
        /// Opens the datasource.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the datasource.
        /// </summary>
        void Close();
    }
}