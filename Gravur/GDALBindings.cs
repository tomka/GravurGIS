using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GravurGIS
{
    public class GDALBindings
    {
        public enum GA_Access
        {
            /*! Read only (no update) access */
            GA_ReadOnly = 0,
            /*! Read/write access. */
            GA_Update = 1
        }

        public enum ColorInterp
        {
            GCI_Undefined = 0,
            GCI_GrayIndex = 1,
            GCI_PaletteIndex = 2,
            GCI_RedBand = 3,
            GCI_GreenBand = 4,
            GCI_BlueBand = 5,
            GCI_AlphaBand = 6,
            GCI_HueBand = 7,
            GCI_SaturationBand = 8,
            GCI_LightnessBand = 9,
            GCI_CyanBand = 10,
            GCI_MagentaBand = 11,
            GCI_YellowBand = 12,
            GCI_BlackBand = 13,
            GCI_YCbCr_YBand = 14,
            GCI_YCbCr_CbBand = 15,
            GCI_YCbCr_CrBand = 16,
            GCI_Max = 16
        }

        public enum DataType
        {
            GDT_Unknown = 0,
            GDT_Byte = 1,
            GDT_UInt16 = 2,
            GDT_Int16 = 3,
            GDT_UInt32 = 4,
            GDT_Int32 = 5,
            GDT_Float32 = 6,
            GDT_Float64 = 7,
            GDT_CInt16 = 8,
            GDT_CInt32 = 9,
            GDT_CFloat32 = 10,
            GDT_CFloat64 = 11,
            GDT_TypeCount = 12
        }

        public enum PaletteInterp
        {
            GPI_Gray = 0,
            GPI_RGB = 1,
            GPI_CMYK = 2,
            GPI_HLS = 3
        }

        public enum GDALRWFlag {
            GF_Read  = 0,   // 	Read data 
            GF_Write = 1    // 	Write data
        }

        public enum CPLErr
        {
            CE_None = 0,
            CE_Debug = 1,
            CE_Warning = 2,
            CE_Failure = 3,
            CE_Fatal = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        public class GDALColorEntry
        {
            /*! gray, red, cyan or hue */
            public short c1;      

            /*! green, magenta, or lightness */
            public short c2;      

            /*! blue, yellow, or saturation */
            public short c3;      

            /*! alpha or blackband */
            public short c4;      
        }

        [DllImport("gdalce.dll")]
        public static extern IntPtr
            GDALOpen(string pszFilename, GA_Access eAccess);

        [DllImport("gdalce.dll")]
        public static extern IntPtr
            GDALGetDatasetDriver(IntPtr GDALDatasetH);

        [DllImport("gdalce.dll")]
        public static extern void GDALAllRegister();

        // Unicode Probleme?
        [DllImport("gdalce.dll", CharSet = CharSet.Unicode)]
        public static extern string
            GDALGetDriverShortName(IntPtr GDALDriverH);

        // Unicode Probleme?
        [DllImport("gdalce.dll", CharSet = CharSet.Unicode)]
        public static extern string
            GDALGetDriverLongName(IntPtr GDALDriverH);

        [DllImport("gdalce.dll")]
        public static extern int GDALGetRasterXSize(IntPtr GDALDatasetH);

        [DllImport("gdalce.dll")]
        public static extern int GDALGetRasterYSize(IntPtr GDALDatasetH);

        [DllImport("gdalce.dll")]
        public static extern int GDALGetRasterCount(IntPtr GDALDatasetH);

        [DllImport("gdalce.dll")]
        public static extern IntPtr GDALGetRasterBand(IntPtr GDALDatasetH, int nBandId);

        [DllImport("gdalce.dll")]
        public static extern int GDALGetRasterBandXSize(IntPtr GDALRasterBandH);

        [DllImport("gdalce.dll")]
        public static extern int GDALGetRasterBandYSize(IntPtr GDALRasterBandH); 

        [DllImport("gdalce.dll")]
        public static extern ColorInterp GDALGetRasterColorInterpretation(IntPtr GDALRasterBandH);

        [DllImport("gdalce.dll")]
        public static extern DataType GDALGetRasterDataType(IntPtr GDALRasterBandH);

        [DllImport("gdalce.dll")]
        public static extern int GDALGetOverviewCount(IntPtr GDALRasterBandH);

        [DllImport("gdalce.dll")]
        public static extern IntPtr GDALGetOverview(IntPtr GDALRasterBandH, int i);

        [DllImport("gdalce.dll")]
        public static extern IntPtr GDALGetRasterColorTable(IntPtr GDALRasterBandH);

        [DllImport("gdalce.dll")]
        public static extern PaletteInterp GDALGetPaletteInterpretation(IntPtr GDALColorTableH);

        [DllImport("gdalce.dll")]
        public static extern CPLErr GDALRasterIO(
            IntPtr GDALRasterBandH,
            GDALRWFlag eRWFlag,
            int nXOff,
            int nYOff,
            int nXSize,
            int nYSize,
            [OutAttribute] IntPtr pData,
            int nBufXSize,
            int nBufYSize,
            DataType eBufType,
            int nPixelSpace,
            int nLineSpace
        );

        [DllImport("gdalce.dll")]
        public static extern IntPtr GDALGetColorEntry(IntPtr GDALColorTableH, int i);

        [DllImport("gdalce.dll")]
        public static extern int GDALGetColorEntryCount(IntPtr GDALColorTableH);

        
            // Unicode Probleme?
        [DllImport("gdalce.dll", CharSet = CharSet.Unicode)]
        public static extern string GDALGetProjectionRef(IntPtr GDALDatasetH);

        

        [DllImport("gdalce.dll")]
        public static extern CPLErr GDALGetGeoTransform(IntPtr GDALDatasetH, [In, Out] double[] Transform);

        [DllImport("gdalce.dll")]
        public static extern IntPtr
        GDALGetDriverByName(string s);

        [DllImport("gdalce.dll")]
        public static extern IntPtr
        GDALCreateCopy(IntPtr GDALDriverH, string s, IntPtr GDALDatasetH,
                        int i, IntPtr j, IntPtr k, IntPtr l);


        [DllImport("gdalce.dll")]
        public static extern void
            GDALClose(IntPtr GDALDriverH);


        [DllImport("gdalce.dll")]
        public static extern void
            GDALDestroyDriverManager();
    }
}
