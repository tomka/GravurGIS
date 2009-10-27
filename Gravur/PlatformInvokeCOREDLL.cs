using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GravurGIS
{
    /// 
    /// This class shall keep the User32 APIs used in our program.
    /// 
    public class PlatformInvokeCOREDLL
    {
        public static byte VK_TAB = 0x09;
        public static byte VK_Shift = 0x10;
        public static byte VK_KEYLOCK = 0x85;
        private const byte KEYEVENTF_KEYUP = 0x2;
        private const byte KEYEVENTF_KEYDOWN = 0x0;
        private const byte KEYEVENTF_SILENT = 0x0004;

        #region Class Variables
        public  const int SRCCOPY = 13369376;
        #endregion

        [DllImport("coredll.dll", EntryPoint = "DeleteDC")]
        public static extern IntPtr DeleteDC(IntPtr hDc);

        [DllImport("coredll.dll", EntryPoint = "DeleteObject")]
        public static extern IntPtr DeleteObject(IntPtr hDc);

        [DllImport("coredll.dll", EntryPoint = "BitBlt")]
        public static extern bool BitBlt(IntPtr hdcDest,int xDest,int yDest,int wDest,int hDest,IntPtr hdcSource,int xSrc,int ySrc,int RasterOp);

        [DllImport("coredll.dll", EntryPoint = "CreateCompatibleBitmap")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("coredll.dll", EntryPoint = "CreateCompatibleDC")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("coredll.dll", EntryPoint = "SelectObject")]
        public static extern IntPtr SelectObject(IntPtr hdc,IntPtr bmp);

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public static void PressLockButton()
        {
            keybd_event(VK_KEYLOCK, 0, KEYEVENTF_KEYDOWN, 0); //press keylock
            keybd_event(VK_KEYLOCK, 0, KEYEVENTF_SILENT, 0); //press keylock
            keybd_event(VK_KEYLOCK, 0, KEYEVENTF_KEYUP, 0);//release keylock
        }
 
      #region Public Constructor
      public PlatformInvokeCOREDLL()
      {}
      #endregion
    }
}
