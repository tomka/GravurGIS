using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GravurGIS
{
    /// 
    /// This class shall keep the User32 APIs used in our program.
    /// 
    public class PlatformInvokeUSER32
    {
        #region Class Variables
        public const int SM_CXSCREEN = 0;
        public const int SM_CYSCREEN = 1;
        #endregion

        #region Class Functions
        [DllImport("coredll.dll", EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("coredll.dll", EntryPoint = "GetDC")]
        public static extern IntPtr GetDC(IntPtr ptr);

        [DllImport("coredll.dll", EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics(int abc);

        [DllImport("coredll.dll", EntryPoint = "GetWindowDC")]
        public static extern IntPtr GetWindowDC(Int32 ptr);

        [DllImport("coredll.dll", EntryPoint = "ReleaseDC")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

        #endregion

        #region Public Constructor
        public PlatformInvokeUSER32()
        {}
        #endregion
    }
}
