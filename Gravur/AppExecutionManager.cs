using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace AppOneCopy_CS
{
    class AppExecutionManager : IDisposable
    {
        public AppExecutionManager(string appName)
        {
            this.appName = appName;
            _eventHandle = CreateEvent(IntPtr.Zero, true, false, appName + "Event");
            _isFirstInstance = (Marshal.GetLastWin32Error() == 0);
        }

        public bool IsFirstInstance
        {
            get { return _isFirstInstance; }
        }

        public void Dispose()
        {
            if (_eventHandle != IntPtr.Zero)
                CloseHandle(_eventHandle);
        }

        public bool ActivateFirstInstance()
        {
            IntPtr handle = FindWindow("#NETCF_AGL_BASE_", appName);
            if (handle == IntPtr.Zero) return false;
            return SetForegroundWindow(handle);
        }

        private bool _isFirstInstance;
        private IntPtr _eventHandle = IntPtr.Zero;
        private string appName = String.Empty;

        #region Imports

        [DllImport("Coredll.dll", SetLastError = true)]
        static extern IntPtr CreateEvent(IntPtr alwaysZero, bool manualReset, bool initialState, string name);
        
        [DllImport("Coredll.dll", SetLastError = true)]
        static extern int CloseHandle(IntPtr handle);
        
        [DllImport("coredll.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("coredll.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        #endregion

   }
}
