using System.Runtime.InteropServices;
using System;

namespace GravurGIS
{
    public class MapPanelBindings
    {
        static IntPtr container;
        static IntPtr OGRcontainer;

        [StructLayout(LayoutKind.Sequential)]
        public class ImageLayerInfo
        {
            public int id;
            public double minBX;
            public double minBY;
            public double maxBX;
            public double maxBY;
            public double resolutionX;
            public double resolutionY;
        };

        [StructLayout(LayoutKind.Sequential)]
        public class VectorLayerInfo
        {
            public int id;
            public double minBX;
            public double minBY;
            public double maxBX;
            public double maxBY;
        };

        [DllImport("MapPanel.dll")]
        public static extern bool WriteImageInformationToFile(string imageFile, string infoFile);

        [DllImport("MapPanel.dll", EntryPoint = "InitGDAL")]
        private static extern IntPtr _InitGDAL(int width, int height, double scale);
        public static IntPtr InitGDAL(int width, int height, double scale)
        {
            container = _InitGDAL(width, height, scale);
            return container;
        }

        [DllImport("MapPanel.dll")]
        public static extern bool CloseGDAL(IntPtr CGDALContainer);

        [DllImport("MapPanel.dll")]
        public static extern IntPtr AddFileToGDALContainer(IntPtr CGDALContainer, string file);

        [DllImport("MapPanel.dll", EntryPoint = "GDALDrawAllImages")]
        public static extern void GDALDrawAllImages(IntPtr CGDALContainer, IntPtr HDC, double dWorldOriginX, double dWorldOriginY);

        [DllImport("MapPanel.dll")]
        public static extern void OnPanWrapper(IntPtr CGDALContainer, int dX, int dY);

        [DllImport("MapPanel.dll")]
        public static extern void OnZoomWrapper(IntPtr CGDALContainer, double dCurrentZoomFactor, int zoomRecX,
								  int zoomRecY, int zommRecWidth, int zoomRectHeight);

        [DllImport("MapPanel.dll")]
        private static extern void OnScaleChangedWrapper(IntPtr CGDALContainer, double scale);
        public static void OnScaleChanged(double scale)
        {
            if (container != null) OnScaleChangedWrapper(container, scale);
        }

        [DllImport("MapPanel.dll")]
        private static extern bool RecalculateImagesWrapper(IntPtr CGDALContainer, double scale, double dXWorldOffset, double dYWorldOffset);
        public static void RecalculateImages(double scale, double dXWorldOffset, double dYWorldOffset)
        {
            if (container != null) RecalculateImagesWrapper(container, scale, dXWorldOffset, dYWorldOffset);
        }

        [DllImport("MapPanel.dll")]
        private static extern bool RecalculateImageWrapper(IntPtr CGDALContainer, double scale, double dXWorldOffset, double dYWorldOffset, int index);
        public static void RecalculateImage(double scale, double dXWorldOffset, double dYWorldOffset, int index)
        {
            if (container != null) RecalculateImageWrapper(container, scale, dXWorldOffset, dYWorldOffset, index);
        }

        [DllImport("MapPanel.dll")]
        public static extern void DrawMandelbrot(IntPtr mbr, IntPtr hDC, double dX, double dY,
                                          int maxIterations, double xPos, double yPos, double size);


        [DllImport("MapPanel.dll")]
        public static extern IntPtr GetNewMandelbrotLayer(int width, int height);

        [DllImport("MapPanel.dll")]
        public static extern void GDALDrawImage(IntPtr container, IntPtr hDC, double dX, double dY, int index);

        [DllImport("MapPanel.dll", EntryPoint = "SetLayerTransparencyWrapper")]
        private static extern void _SetLayerTransparency(IntPtr container, int index, bool isTransparent);
        public static void SetLayerTransparency(int index, bool isTransparent)
        {
            if (container != null) _SetLayerTransparency(container, index, isTransparent);
        }

        [DllImport("MapPanel.dll", EntryPoint = "clearSourceWrapper")]
        public static extern void clearSource(IntPtr trans);

        [DllImport("MapPanel.dll", EntryPoint = "clearTargetWrapper")]
        public static extern void clearTarget(IntPtr trans);

        [DllImport("MapPanel.dll", EntryPoint = "setTargetTMWrapper")]
        public static extern bool setTargetTM(IntPtr trans, double dfCenterLat,
                                       double dfCenterLong, double dfScale, double dfFalseEasting,
                                       double dfFalseNorthing);

        [DllImport("MapPanel.dll", EntryPoint = "setTargetGeoCSWrapper")]
        public static extern bool setTargetGeoCS(IntPtr trans, double dfSemiMajor, double dfInvFlattening);

        [DllImport("MapPanel.dll", EntryPoint = "setTargetTOWGS84Wrapper")]
        public static extern bool setTargetTOWGS84(IntPtr trans,
                                          double dfDX, double dfDY, double dfDZ, double dfEX,
                                          double dfEY, double dfEZ, double dfPPM);

        [DllImport("MapPanel.dll", EntryPoint = "createTrafo_SourceToTargetWrapper")]
        public static extern bool createTrafo_SourceToTarget(IntPtr trans);

        [DllImport("MapPanel.dll", EntryPoint = "setSourceGeoCSWrapper")]
        public static extern bool setSourceGeoCS(IntPtr trans);

        [DllImport("MapPanel.dll", EntryPoint = "GetRotatedBitmapNT")]
        public static extern IntPtr GetRotatedBitmapNT(IntPtr hBitmap, float radians, int clrBackCOLORREF);

        [DllImport("MapPanel.dll", EntryPoint = "GetRotatedBitmap")]
        public static extern IntPtr GetRotatedBitmap(IntPtr hBitmap, float radians, int clrBackCOLORREF);



        #region OGR

        [DllImport("MapPanel.dll", EntryPoint = "InitOGR")]
        private static extern IntPtr _InitOGR(int width, int height);

        public static IntPtr InitOGR(int width, int height)
        {
            OGRcontainer = _InitOGR(width, height);
            return OGRcontainer;
        }

        [DllImport("MapPanel.dll")]
        public static extern bool CloseOGR(IntPtr OGRContainer);

        [DllImport("MapPanel.dll")]
        public static extern void OGRDrawImage(IntPtr OGRContainer, IntPtr hDC, double scale,
            double dX, double dY, int index);

        [DllImport("MapPanel.dll")]
        public static extern IntPtr AddFileToOGRContainer(IntPtr OGRContainer, string file, string SpatialReference);

        #endregion
    }
}
