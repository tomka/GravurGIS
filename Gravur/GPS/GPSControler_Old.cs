//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Runtime.InteropServices;
//using System.Threading;
//using System.Windows.Forms;

//namespace Gravur.GPS
//{
//    public class GPSControler_Old : IDisposable
//    {
//        #region Structs & Konstanten

//        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
//        struct GPSPosition
//        {
//            public int dwVersion;
//            public int dwSize;
//            public int dwValidFields;
//            public int dwFlags;
//            public SystemTime stUTCTime;
//            public double dblLatitude;
//            public double dblLongitude;
//            public float flSpeed;
//            public float flHeading;
//            public double dblMagneticVariation;
//            public float flAltitudeWRTSeaLevel;
//            public float flAltitudeWRTEllipsoid;
//            public FixQuality FixQuality;
//            public FixType FixType;
//            public FixSelection SelectionType;
//            public float flPositionDilutionOfPrecision;
//            public float flHorizontalDilutionOfPrecision;
//            public float flVerticalDilutionOfPrecision;
//            public int dwSatelliteCount;
//            public SatelliteArray rgdwSatellitesUsedPRNs;
//            public int dwSatellitesInView;
//            public SatelliteArray rgdwSatellitesInViewPRNs;
//            public SatelliteArray rgdwSatellitesInViewElevation;
//            public SatelliteArray rgdwSatellitesInViewAzimuth;
//            public SatelliteArray rgdwSatellitesInViewSignalToNoiseRatio;
//        }


//        [StructLayout(LayoutKind.Sequential)]
//        struct SystemTime
//        {
//            public short wYear;
//            public short wMonth;
//            public short wDayOfWeek;
//            public short wDay;
//            public short wHour;
//            public short wMinute;
//            public short wSecond;
//            public short wMilliseconds;
//        }

//        [StructLayout(LayoutKind.Sequential)]
//        internal struct SatelliteArray
//        {
//            int a, b, c, d, e, f, g, h, i, j, k, l;
//        }

//        enum FixQuality : int
//        {
//            Unknown = 0,
//            Gps,
//            DGps
//        }
//        enum FixType : int
//        {
//            Unknown = 0,
//            XyD,
//            XyzD
//        }

//        enum FixSelection : int
//        {
//            Unknown = 0,
//            Auto,
//            Manual
//        }

//        //Struktur für Devicestate
//        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
//        struct DeviceState
//        {
//            public int dwVersion;
//            public int dwSize;
//            public int dwServiceState;
//            public int dwDeviceState;
//            public long ftLastDataReceived;
//            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
//            public string szGPSDriverPrefix;
//            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
//            public string szGPSMultiplexPrefix;
//            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
//            public string szGPSFriendlyName;
//        }


//        //Einige Konstanten
//        internal static int GPS_VALID_UTC_TIME = 0x00000001;
//        internal static int GPS_VALID_LATITUDE = 0x00000002;
//        internal static int GPS_VALID_LONGITUDE = 0x00000004;
//        internal static int GPS_VALID_SPEED = 0x00000008;
//        internal static int GPS_VALID_HEADING = 0x00000010;
//        internal static int GPS_VALID_MAGNETIC_VARIATION = 0x00000020;
//        internal static int GPS_VALID_ALTITUDE_WRT_SEA_LEVEL = 0x00000040;
//        internal static int GPS_VALID_ALTITUDE_WRT_ELLIPSOID = 0x00000080;
//        internal static int GPS_VALID_POSITION_DILUTION_OF_PRECISION = 0x00000100;
//        internal static int GPS_VALID_HORIZONTAL_DILUTION_OF_PRECISION = 0x00000200;
//        internal static int GPS_VALID_VERTICAL_DILUTION_OF_PRECISION = 0x00000400;
//        internal static int GPS_VALID_SATELLITE_COUNT = 0x00000800;
//        internal static int GPS_VALID_SATELLITES_USED_PRNS = 0x00001000;
//        internal static int GPS_VALID_SATELLITES_IN_VIEW = 0x00002000;
//        internal static int GPS_VALID_SATELLITES_IN_VIEW_PRNS = 0x00004000;
//        internal static int GPS_VALID_SATELLITES_IN_VIEW_ELEVATION = 0x00008000;
//        internal static int GPS_VALID_SATELLITES_IN_VIEW_AZIMUTH = 0x00010000;
//        internal static int GPS_VALID_SATELLITES_IN_VIEW_SIGNAL_TO_NOISE_RATIO = 0x00020000;

//        #endregion

//        #region P/Invoke Funktionen

//        //Externe Funktionen
//        [DllImport("gpsapi.dll")]
//        static extern IntPtr GPSOpenDevice(IntPtr hNewLocationData, IntPtr hDeviceStateChange, string szDeviceName, int dwFlags);
//        [DllImport("gpsapi.dll")]
//        static extern int GPSCloseDevice(IntPtr hGPSDevice);
//        [DllImport("gpsapi.dll")]
//        static extern int GPSGetPosition(IntPtr hGPSDevice, ref GPSPosition pGPSPosition, int dwMaximumAge, int dwFlags);
//        [DllImport("gpsapi.dll")]
//        static extern int GPSGetDeviceState(ref DeviceState pGPSDevice);

//        [DllImport("coredll.dll")]
//        static extern IntPtr CreateEvent(IntPtr lpEventAttributes, int bManualReset, int bInitialState, StringBuilder lpName);
//        [DllImport("coredll.dll")]
//        static extern int CloseHandle(IntPtr hObject);
//        const int waitFailed = -1;
//        [DllImport("coredll.dll")]
//        static extern int WaitForMultipleObjects(int nCount, IntPtr lpHandles, int fWaitAll, int dwMilliseconds);
//        [DllImport("coredll.dll")]
//        static extern int EventModify(IntPtr hHandle, int dwFunc);
//        public const int LMEM_ZEROINIT = 0x40;
//        [DllImport("coredll.dll", EntryPoint = "#33", SetLastError = true)]
//        public static extern IntPtr LocalAlloc(int flags, int byteCount);
//        [DllImport("coredll.dll", EntryPoint = "#36", SetLastError = true)]
//        public static extern IntPtr LocalFree(IntPtr hMem);

//        #endregion

//        #region Members

//        private double geoTopGK;
//        private double geoLeftGK;
//        private MainControler mainControler;

//        //Variable für GPS-Handle und EventHandle
//        IntPtr myGpsHandle = IntPtr.Zero;
//        IntPtr myNewLocationHandle = IntPtr.Zero;
//        IntPtr myDeviceStateChangedHandle = IntPtr.Zero;
//        IntPtr myStopHandle = IntPtr.Zero;

//        int countDSCEvents = 0;
//        int countNLEvents = 0;

//        Thread myGPSEventThread = null;

//        #endregion

//        event LocationChangedEventHandler locationChanged;

//        /// <summary>
//        /// Event that is raised when the GPS locaction data changes
//        /// </summary>
//        public event LocationChangedEventHandler LocationChanged
//        {
//            add
//            {
//                locationChanged += value;

//                // create our event thread only if the user decides to listen
//                CreateGpsEventThread();
//            }
//            remove
//            {
//                locationChanged -= value;
//            }
//        }


//        event DeviceStateChangedEventHandler deviceStateChanged;

//        /// <summary>
//        /// Event that is raised when the GPS device state changes
//        /// </summary>
//        public event DeviceStateChangedEventHandler DeviceStateChanged
//        {
//            add
//            {
//                deviceStateChanged += value;

//                // create our event thread only if the user decides to listen
//                CreateGpsEventThread();
//            }
//            remove
//            {
//                deviceStateChanged -= value;
//            }
//        }

//        public GPSControler_Old(MainControler mainControler)
//        {
//            this.mainControler = mainControler;
//        }

//        ~GPSControler_Old()
//        {
//            // make sure that the GPS was closed.
//            Close();
//        }

//        /// <summary>
//        /// Opens the GPS device and prepares to receive data from it.
//        /// </summary>
//        public void Open()
//        {
//            if (!Opened)
//            {
//                // create handles for GPS events
//                newLocationHandle = CreateEvent(IntPtr.Zero, 0, 0, null);
//                deviceStateChangedHandle = CreateEvent(IntPtr.Zero, 0, 0, null);
//                stopHandle = CreateEvent(IntPtr.Zero, 0, 0, null);

//                //Verbindung zum GPSID herstellen
//                myGpsHandle = GPSOpenDevice(newLocationHandle, deviceStateChangedHandle, null, 0);

//                // if events were hooked up before the device was opened, we'll need
//                // to create the gps event thread.
//                if (locationChanged != null || deviceStateChanged != null)
//                {
//                    CreateGpsEventThread();
//                }
//            }
//        }

//        /// <summary>
//        /// Closes the gps device.
//        /// </summary>
//        public void Close()
//        {
//            //Nativen Event auslösen, der zum Beenden des Eventthreads führt
//            if (myStopHandle != IntPtr.Zero)
//            {
//                EventModify(myStopHandle, 3);
//            }

//            ////Warten, bis der GPSEventThread beendet wurde (Threadsynchronisation für Arme)
//            //while (myGPSEventThread != null)
//            //{
//            //    Thread.Sleep(100);
//            //}

//            // block until our event thread is finished before
//            // we close our native event handles
//            lock (this)
//            {
//                if (myNewLocationHandle != IntPtr.Zero)
//                {
//                    CloseHandle(myNewLocationHandle);
//                    myNewLocationHandle = IntPtr.Zero;
//                }
//                if (myDeviceStateChangedHandle != IntPtr.Zero)
//                {
//                    CloseHandle(myDeviceStateChangedHandle);
//                    myDeviceStateChangedHandle = IntPtr.Zero;
//                }
//                if (myStopHandle != IntPtr.Zero)
//                {
//                    CloseHandle(myStopHandle);
//                    myStopHandle = IntPtr.Zero;
//                }
//            }

//            //Verbindung zu GPSID beenden
//            if (myGpsHandle != IntPtr.Zero)
//            {
//                GPSCloseDevice(myGpsHandle);
//                myGpsHandle = IntPtr.Zero;
//            }
//        }

//        /// <summary>
//        /// Creates our event thread that will receive native events
//        /// </summary>
//        private void CreateGpsEventThread()
//        {
//            // we only want to create the thread if we don't have one created already 
//            // and we have opened the gps device
//            if (gpsEventThread == null && gpsHandle != IntPtr.Zero)
//            {
//                // Create and start thread to listen for GPS events
//                gpsEventThread = new System.Threading.Thread(new System.Threading.ThreadStart(WaitForGpsEvents));
//                gpsEventThread.Start();
//            }
//        }

//        //Dieser Delegate wird benötigt, um threadsicher in eine Textbox schreiben zu können
//        delegate void SetTextBoxEntryCallback(string myWert, TextBox myTextBox);

//        //Dieser Methode wird benötigt, um threadsicher in die Textbox schreiben zu können
//        public void SetTextBoxEntry(string myWert, TextBox myTextBox)
//        {
//            //if (myTextBox.InvokeRequired)
//            //{
//            //    SetTextBoxEntryCallback myDelegate = new SetTextBoxEntryCallback(SetTextBoxEntry);
//            //    this.Invoke(myDelegate, new object[] { myWert, myTextBox });
//            //}
//            //else
//            //{
//            //    myTextBox.Text = myWert;
//            //}
//        }


//        //Dieser Delegate wird benötigt, um threadsicher in eine Checkbox schreiben zu können
//        delegate void SetCheckBoxEntryCallback(bool myWert, CheckBox myCheckBox);

//        //Dieser Methode wird benötigt, um threadsicher in die Textbox schreiben zu können
//        public void SetCheckBoxEntry(bool myWert, CheckBox myCheckBox)
//        {
//            //if (myCheckBox.InvokeRequired)
//            //{
//            //    SetCheckBoxEntryCallback myDelegate = new SetCheckBoxEntryCallback(SetCheckBoxEntry);
//            //    this.Invoke(myDelegate, new object[] { myWert, myCheckBox });
//            //}
//            //else
//            //{
//            //    myCheckBox.Checked = myWert;
//            //}
//        }


//        //Aktuelle Position ermitteln
//        private int MyGetGPSPosition(IntPtr gpsHandle, ref GPSPosition myGPSPosition)
//        {
//            //Die Struktur muss in den Felder dwVersion und dwSize genau diese Werte enthalten
//            myGPSPosition.dwVersion = 1;
//            myGPSPosition.dwSize = Marshal.SizeOf(typeof(GPSPosition));

//            //Der Aufruf der Betriebssystemfunktion. 
//            //Es werden nur Werte zurückgegeben, die nicht älter als 1.000.000 Millisekunden sind
//            int result = GPSGetPosition(gpsHandle, ref myGPSPosition, 1000000, 0);

//            return result;
//        }

//        //Aktuellen Status von GPSID und GPS-Gerät ermitteln
//        private int MyGetDeviceState(ref DeviceState myDeviceState)
//        {
//            //Die Struktur muss in den Felder dwVersion und dwSize genau diese Werte enthalten
//            myDeviceState.dwVersion = 1;
//            myDeviceState.dwSize = Marshal.SizeOf(typeof(DeviceState));

//            //Der Aufruf der Betriebssystemfunktion. 
//            int result = GPSGetDeviceState(ref myDeviceState);

//            return result;
//        }

//        //In diesem Thread werden die nativen Events aufgefangen und verarbeitet
//        private void WaitForGpsEvents()
//        {
//            bool listening = true;

//            //Speicherbereich reservieren und Adressen der Ereignishandles hineinschreiben
//            IntPtr handles = LocalAlloc(LMEM_ZEROINIT, 12);
//            Marshal.WriteInt32(handles, 0, myStopHandle.ToInt32());
//            Marshal.WriteInt32(handles, 4, myDeviceStateChangedHandle.ToInt32());
//            Marshal.WriteInt32(handles, 8, myNewLocationHandle.ToInt32());

//            //Diesen Blok wird solange durchlaufen, bis der StopEvent kommt
//            try
//            {
//                while (listening)
//                {
//                    //WaitForMultipleObjects wartet darauf, dass ein Event auftritt, zurückgegeben wird die Position des Handles im Speicherblock
//                    int myWFMO = WaitForMultipleObjects(3, handles, 0, -1);
//                    if (myWFMO != -1) //das ist WaitFailed
//                    {
//                        switch (myWFMO)
//                        {
//                            case 0:
//                                //Dieses Ereignis tritt ein, wenn der StopEvent ausgelöst wurde
//                                listening = false;
//                                break;
//                            case 1:
//                                //Dieses Ereignis tritt ein, wenn ein neuer DeviceServiceState ausgelöst wurde
//                                //Eventzähler hochsetzen (dient nur zur Information)
//                                countDSCEvents++;
//                                SetTextBoxEntry(countDSCEvents.ToString(), txtDSCEvents);

//                                //Struktur initialisieren, DeviceState ermittlen und in Steuerlemente schreiben
//                                DeviceState currDeviceState = new DeviceState();
//                                if (MyGetDeviceState(ref currDeviceState) == 0)
//                                {
//                                    if (currDeviceState.dwServiceState == 0)
//                                    {
//                                        SetCheckBoxEntry(false, chkServiceState);
//                                    }
//                                    else
//                                    {
//                                        SetCheckBoxEntry(true, chkServiceState);
//                                    }
//                                    if (currDeviceState.dwDeviceState == 0)
//                                    {
//                                        SetCheckBoxEntry(false, chkDeviceState);
//                                    }
//                                    else
//                                    {
//                                        SetCheckBoxEntry(true, chkDeviceState);
//                                    }
//                                }
//                                break;
//                            case 2:
//                                //Dieses Ereignis tritt ein, wenn ein neuer DeviceServiceState ausgelöst wurde
//                                //Eventzähler hochsetzen (dient nur zur Information)
//                                countNLEvents++;
//                                SetTextBoxEntry(countNLEvents.ToString(), txtNLEvents);

//                                //Struktur initialisieren, Position ermitteln und in Steuerelemente schreiben
//                                GPSPosition currPosition = new GPSPosition();
//                                if (MyGetGPSPosition(myGpsHandle, ref currPosition) == 0)
//                                {
//                                    //Überprüfen, ob der jeweilige Wert gültig ist und in die entsprechende Textbox eintragen
//                                    if ((currPosition.dwValidFields & GPS_VALID_LATITUDE) != 0)
//                                    {
//                                        SetTextBoxEntry(currPosition.dblLatitude.ToString(), txtLat);
//                                    }
//                                    else
//                                    {
//                                        SetTextBoxEntry("Ungültig", txtLat);
//                                    }

//                                    if ((currPosition.dwValidFields & GPS_VALID_LONGITUDE) != 0)
//                                    {
//                                        SetTextBoxEntry(currPosition.dblLongitude.ToString(), txtLong);
//                                    }
//                                    else
//                                    {
//                                        SetTextBoxEntry("Ungültig", txtLong);
//                                    }

//                                    if ((currPosition.dwValidFields & GPS_VALID_ALTITUDE_WRT_SEA_LEVEL) != 0)
//                                    {
//                                        SetTextBoxEntry(currPosition.flAltitudeWRTSeaLevel.ToString(), txtAltSea);
//                                    }
//                                    else
//                                    {
//                                        SetTextBoxEntry("Ungültig", txtLong);
//                                    }
//                                }
//                                break;
//                        }
//                    }
//                }
//            }
//            finally
//            {
//                //Speicherbereich für Handles freigeben
//                LocalFree(handles);
//                myGPSEventThread = null;
//            }
//        }



//        #region Getters/Setters

//        public double GeoTopGK
//        {
//            get { return geoTopGK; }
//            set { geoTopGK = value; }
//        }
//        public double GeoLeftGK
//        {
//            get { return geoLeftGK; }
//            set { geoLeftGK = value; }
//        }

//        /// <summary>
//        /// True: The GPS device has been opened. False: It has not been opened
//        /// </summary>
//        public bool Opened
//        {
//            get { return myGpsHandle != IntPtr.Zero; }
//        }

//        #endregion

//        #region IDisposable Members

//        public void Dispose()
//        {
//            this.Close();
//        }

//        #endregion
//    }
//}
