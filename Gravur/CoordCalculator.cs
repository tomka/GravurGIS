using System;
using System.Collections.Generic;
using System.Text;
using GravurGIS.GPS;
using GravurGIS.Topology;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace GravurGIS
{
    public struct TWinkel {
        public int grad;
        public int min;
        public double sek;

        public TWinkel(int grad, int min, double sek) {
            this.grad = grad;
            this.min = min;
            this.sek = sek;
        }

        public override string ToString()
        {
            return grad.ToString() + "° " + min.ToString() + "' " + sek.ToString();
        }
    }

    public struct GKCoord {
        public double  r_value;
        public double  h_value;
        public long    stripe;

        public override string ToString()
        {
            return"Easting: " + this.r_value.ToString() + "\nNorthing: " + this.h_value.ToString() + "\nStreifen: " + this.stripe.ToString();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class Coord
    {
        public double x;
        public double y;
        public int state;
    }

    public struct GeoCoord
    {
        /// <summary>
        /// East
        /// </summary>
        public DegreesMinutesSeconds longitude;

        /// <summary>
        /// North
        /// </summary>
        public DegreesMinutesSeconds latitude;

        public override string ToString()
        {
            return "Long: " + this.longitude.ToString() + "\n"
                 + "Lat: " + this.latitude.ToString();
        }
    }


    public class CoordCalculator
    {
        // 26
        const double rho = 180 / Math.PI;
        static IntPtr GDALCoordTransformer = IntPtr.Zero;
        static Object mapPanelLock = new Object();
        ///////////////////////
        // Geographieparameter
        ///////////////////////

        private static double e_x = 7.16 * Math.Pow(10, -6);  // Angabe in Bogenmaß (Rad): 1,4770 * pi / 180 = 7.16 * Math.Pow(10, -6)
        private static double e_y = -3.57 * Math.Pow(10, -7); // Angabe in Bogenmaß (Rad)
        private static double e_z = -7.07 * Math.Pow(10, -6); // Angabe in Bogenmaß (Rad)
        private static double m = -9.82 * Math.Pow(10, -6);


        public CoordCalculator(MainControler mainControler) {
            GDALCoordTransformer = GetNewCoordTransformer();
            if (GDALCoordTransformer == IntPtr.Zero)
            {
                MessageBox.Show("Fehler 0x4122: Konnte Koordinaten Transformer nicht erstellen.");
            }

            mainControler.SettingsLoaded += new MainControler.SettingsLoadedDelegate(mainControler_SettingsLoaded);
        }

        void mainControler_SettingsLoaded(Config config)
        {
            try
            {
                if (GDALCoordTransformer == IntPtr.Zero)
                    throw new Exception("Koordinaten-Transformator ist eine Null-Referenz");

                MapPanelBindings.clearSource(GDALCoordTransformer);
                MapPanelBindings.clearTarget(GDALCoordTransformer);
                if (!MapPanelBindings.setTargetTM(GDALCoordTransformer,
                    0.0, 3.0 * config.CoordGaußKruegerStripe, 1.0,
                    500000 + 1000000.0 * config.CoordGaußKruegerStripe,
                    0.0))
                    throw new Exception("Konnte TM-Projektion nicht erstellen");
                if (!MapPanelBindings.setTargetGeoCS(GDALCoordTransformer, config.Datum.Ellipsoid.SemiMajorAxis,
                    config.Datum.Ellipsoid.InverseFlattening))
                    throw new Exception("Konnte Ellipsoiden nicht erstellen");
                if (!MapPanelBindings.setTargetTOWGS84(GDALCoordTransformer,
                    config.Datum.Wgs84Parameters.Dx,
                    config.Datum.Wgs84Parameters.Dy,
                    config.Datum.Wgs84Parameters.Dz,
                    config.Datum.Wgs84Parameters.Ex,
                    config.Datum.Wgs84Parameters.Ey,
                    config.Datum.Wgs84Parameters.Ez,
                    config.Datum.Wgs84Parameters.Ppm))
                    throw new Exception("Konnte WGS84-Konvertierungs-Informationen nicht hinzufügen");
                if (!MapPanelBindings.setSourceGeoCS(GDALCoordTransformer))
                    throw new Exception("Konnte Quellsystem nicht auf WGS84 setzen");

                if (!MapPanelBindings.createTrafo_SourceToTarget(GDALCoordTransformer))
                    throw new Exception("Konnte die Kooridnatentransformation nicht erstellen.");

                //TransformerSetSourceWGS_TargetGK(GDALCoordTransformer, config.CoordGaußKruegerStripe);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler 0x9112: Kooridnaten-Rechner hat einen Fehler festgestellt: " + ex.Message);
            }
        }

        #region P/Invokes

        [DllImport("MapPanel.dll")]
        private static extern IntPtr GetNewCoordTransformer();

        [DllImport("MapPanel.dll")]
        private static extern IntPtr Transform(IntPtr CoordTransformer, double x, double y);

        [DllImport("MapPanel.dll")]
        private static extern bool TransformerSetSourceGK_TargetWGS(IntPtr CoordTransformer, int stripe);

        [DllImport("MapPanel.dll")]
        private static extern bool TransformerSetSourceWGS_TargetGK(IntPtr CoordTransformer, int stripe);

        [DllImport("MapPanel.dll", EntryPoint = "deleteGKCoordPointerWrapper")]
        private static extern void deleteGKCoordPointer(IntPtr CoordTransformer, IntPtr ptr);

        #endregion

        private double GetDegreesFromAngular(double dbl)
        {
            dbl /= 100;
            
            int i = (int)dbl;
            double frac = (dbl - (double)i) * (100.0 / 60.0);

            return ((double)i + frac);
        }

        public GKCoord GeoGkGDAL(GpsPosition pos, int sy) {
            // Only for use with Emulator!!!
            //return GeoGkGDAL(GetDegreesFromAngular(pos.Latitude),
            //    GetDegreesFromAngular(pos.Longitude), sy);

            // on Hardware, use:
            return GeoGkGDAL(pos.Latitude, pos.Longitude, sy);
        }

        //private static bool temp = true;

        /// <summary>
        /// This function transforms geographic lat/longs to projected hauß krüger coordinates
        /// using the GDAL library (which uses Proj4)
        /// </summary>
        /// <param name="x">Geographic Latitude</param>
        /// <param name="y">Geographic Longtitude</param>
        /// <param name="stripe"></param>
        /// <returns></returns>
        public static GKCoord GeoGkGDAL(double x, double y, int stripe)
        {
            GKCoord gkCoord;
            lock (mapPanelLock)
            {
                if (GDALCoordTransformer == IntPtr.Zero) GDALCoordTransformer = GetNewCoordTransformer();
                gkCoord = new GKCoord();
                if (GDALCoordTransformer == IntPtr.Zero) MessageBox.Show("Fehler 0x1283: Fehler bei Koordinatenumrechnung.");
                else
                {
                    IntPtr coordPtr = Transform(GDALCoordTransformer, x, y);
                    Coord coord = new Coord();
                    Marshal.PtrToStructure(coordPtr, coord);
                    deleteGKCoordPointer(GDALCoordTransformer, coordPtr);

                    gkCoord.r_value = coord.x;
                    gkCoord.h_value = coord.y;
                    gkCoord.stripe = stripe;
                }
            }

            return gkCoord;
        }

        public static PointList calculateDisplayCoords(GKCoord[] gkList,double dX, double dY, double scale)
        {
            PointList pList = new PointList(gkList.Length);
            PointD p;
            for (int i = 0; i < gkList.Length; i++)
            {
                p.x = gkList[i].r_value * scale - dX + 0.5; //0.5 for rounding
                p.y = -(gkList[i].h_value * scale - dY) + 0.5;
                pList.add(gkList[i].r_value, gkList[i].h_value, (int)p.x, (int)p.y, i);
            }
                return pList;
        }

        public static PointD calculateDisplayCoords(GKCoord gk, double dX, double dY, double scale)
        {
            PointD p;
            p.x = gk.r_value * scale - dX;
            p.y = -(gk.h_value * scale - dY);
            return p;
        }
    }
}