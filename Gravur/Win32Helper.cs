using System;
using System.Drawing;
using System.Runtime.InteropServices;

// Contains managed wrappers and implementations of Win32
// structures, delegates, constants and platform invokes
// used by the GradientFill and Subclassing samples.

namespace GravurGIS
{
    public sealed class Win32
    {
        public struct TRIVERTEX
        {
            public int x;
            public int y;
            public ushort Red;
            public ushort Green;
            public ushort Blue;
            public ushort Alpha;
            public TRIVERTEX(int x, int y, Color color)
                : this(x, y, color.R, color.G, color.B, color.A)
            {
            }
            public TRIVERTEX(
                int x, int y,
                ushort red, ushort green, ushort blue,
                ushort alpha)
            {
                this.x = x;
                this.y = y;
                this.Red = (ushort)(red << 8);
                this.Green = (ushort)(green << 8);
                this.Blue = (ushort)(blue << 8);
                this.Alpha = (ushort)(alpha << 8);
            }
        }
        public struct GRADIENT_RECT
        {
            public uint UpperLeft;
            public uint LowerRight;
            public GRADIENT_RECT(uint ul, uint lr)
            {
                this.UpperLeft = ul;
                this.LowerRight = lr;
            }
        }
        public struct GRADIENT_TRIANGLE
        {
            public uint Vertex1;
            public uint Vertex2;
            public uint Vertex3;
            public GRADIENT_TRIANGLE(uint v1, uint v2, uint v3)
            {
                this.Vertex1 = v1;
                this.Vertex2 = v2;
                this.Vertex3 = v3;
            }
        }

        // WM_NOTIFY notification message header.
        [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
        public class NMHDR
        {
            private IntPtr hwndFrom;
            public uint idFrom;
            public uint code;
        }

        //[System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
        public struct TVITEM
        {
            public int mask;
            public IntPtr hItem;
            public int state;
            public int stateMask;
            public IntPtr pszText;
            public int cchTextMax;
            public int iImage;
            public int iSelectedImage;
            public int cChildren;
            public IntPtr lParam;
        }

        // Native representation of a point.
        public struct POINT
        {
            public int X;
            public int Y;
        }

        public struct TVHITTESTINFO
        {
            public POINT pt;
            public uint flags;
            public IntPtr hItem;
        }

        // A callback to a Win32 window procedure (wndproc):
        // Parameters:
        //   hwnd - The handle of the window receiving a message.
        //   msg - The message
        //   wParam - The message's parameters (part 1).
        //   lParam - The message's parameters (part 2).
        //  Returns an integer as described for the given message in MSDN.
        public delegate int WndProc(IntPtr hwnd, uint msg, uint wParam, int lParam);

        [DllImport("coredll.dll", SetLastError = true, EntryPoint = "GradientFill")]
        public extern static bool GradientFill(
            IntPtr hdc,
            TRIVERTEX[] pVertex,
            uint dwNumVertex,
            GRADIENT_RECT[] pMesh,
            uint dwNumMesh,
            uint dwMode);

        public const int GRADIENT_FILL_RECT_H = 0x00000000;
        public const int GRADIENT_FILL_RECT_V = 0x00000001;

        // Not supported on Windows CE:
        public const int GRADIENT_FILL_TRIANGLE = 0x00000002;


        [DllImport("coredll.dll")]
        public extern static IntPtr SetWindowLong(
            IntPtr hwnd, int nIndex, IntPtr dwNewLong);
        public const int GWL_WNDPROC = -4;

        [DllImport("coredll.dll")]
        public extern static int CallWindowProc(
            IntPtr lpPrevWndFunc, IntPtr hwnd, uint msg, uint wParam, int lParam);

        [DllImport("coredll.dll")]
        public extern static int DefWindowProc(
            IntPtr hwnd, uint msg, uint wParam, int lParam);

        [DllImport("coredll.dll")]
        public extern static int SendMessage(
            IntPtr hwnd, uint msg, uint wParam, ref TVHITTESTINFO lParam);

        [DllImport("coredll.dll", SetLastError = true)]
        public extern static int SendMessage(
            IntPtr hwnd, uint msg, uint wParam, ref TVITEM lParam);

        [DllImport("coredll.dll")]
        public extern static uint GetMessagePos();

        [DllImport("coredll.dll")]
        public extern static IntPtr BeginPaint(IntPtr hwnd, ref PAINTSTRUCT ps);

        [DllImport("coredll.dll")]
        public extern static bool EndPaint(IntPtr hwnd, ref PAINTSTRUCT ps);

        public struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public bool fErase;
            public Rectangle rcPaint;
            public bool fRestore;
            public bool fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] rgbReserved;
        }

        [DllImport("coredll.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void keybd_event(byte bVk, byte bScan,
                int dwFlags, int dwExtraInfo);

        [DllImport("coredll.dll")]
        public extern static IntPtr GetDC(IntPtr hwnd);

        [DllImport("coredll.dll")]
        public extern static bool ReleaseDC(IntPtr hwnd, IntPtr hdc);

        // Helper function to convert a Windows lParam into a Point.
        //   lParam - The parameter to convert.
        // Returns a Point where X is the low 16 bits and Y is the
        // high 16 bits of the value passed in.
        public static Point LParamToPoint(int lParam)
        {
            uint ulParam = (uint)lParam;
            return new Point(
                (int)(ulParam & 0x0000ffff),
                (int)((ulParam & 0xffff0000) >> 16));
        }

        // Windows messages
        public const uint WM_PAINT = 0x000F;
        public const uint WM_ERASEBKGND = 0x0014;
        public const uint WM_KEYDOWN = 0x0100;
        public const uint WM_KEYUP = 0x0101;
        public const uint WM_MOUSEMOVE = 0x0200;
        public const uint WM_LBUTTONDOWN = 0x0201;
        public const uint WM_LBUTTONUP = 0x0202;
        public const uint WM_NOTIFY = 0x4E;

        // Notifications
        public const uint NM_CLICK = 0xFFFFFFFE;
        public const uint NM_DBLCLK = 0xFFFFFFFD;
        public const uint NM_RCLICK = 0xFFFFFFFB;
        public const uint NM_RDBLCLK = 0xFFFFFFFA;

        // Key
        public const uint VK_SPACE = 0x20;
        public const uint VK_RETURN = 0x0D;

        // Treeview
        public const uint TV_FIRST = 0x1100;
        public const uint TVM_HITTEST = TV_FIRST + 17;

        public const uint TVHT_NOWHERE = 0x0001;
        public const uint TVHT_ONITEMICON = 0x0002;
        public const uint TVHT_ONITEMLABEL = 0x0004;
        public const uint TVHT_ONITEM = (TVHT_ONITEMICON | TVHT_ONITEMLABEL | TVHT_ONITEMSTATEICON);
        public const uint TVHT_ONITEMINDENT = 0x0008;
        public const uint TVHT_ONITEMBUTTON = 0x0010;
        public const uint TVHT_ONITEMRIGHT = 0x0020;
        public const uint TVHT_ONITEMSTATEICON = 0x0040;
        public const uint TVHT_ABOVE = 0x0100;
        public const uint TVHT_BELOW = 0x0200;
        public const uint TVHT_TORIGHT = 0x0400;
        public const uint TVHT_TOLEFT = 0x0800;

        public const uint TVM_GETITEM = TV_FIRST + 62;  //TVM_GETITEMW

        public const uint TVIF_TEXT = 0x0001;
        public const uint TVIF_IMAGE = 0x0002;
        public const uint TVIF_PARAM = 0x0004;
        public const uint TVIF_STATE = 0x0008;
        public const uint TVIF_HANDLE = 0x0010;
        public const uint TVIF_SELECTEDIMAGE = 0x0020;
        public const uint TVIF_CHILDREN = 0x0040;
        public const uint TVIF_DI_SETITEM = 0x1000;

        [DllImport("coredll.dll")]
        public static extern IntPtr LoadIcon(IntPtr hInst, string IconName);

        [DllImport("coredll.dll")]
        public static extern IntPtr GetModuleHandle(String lpModuleName);

    }
}



