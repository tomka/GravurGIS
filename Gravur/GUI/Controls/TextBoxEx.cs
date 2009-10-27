using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.WindowsCE.Forms;
using System.Globalization;

namespace GravurGIS.GUI.Controls
{
    //public enum GetWindowLongParam
    //{

    //    GWL_WNDPROC = (-4),

    //    GWL_HINSTANCE = (-6),

    //    GWL_HWNDPARENT = (-8),

    //    GWL_STYLE = (-16),

    //    GWL_EXSTYLE = (-20),

    //    GWL_USERDATA = (-21),

    //    GWL_ID = (-12)

    //}

    //public enum EditStyle
    //{

    //    ES_LEFT = 0x0000,

    //    ES_CENTER = 0x0001,

    //    ES_RIGHT = 0x0002,

    //    ES_MULTILINE = 0x0004,

    //    ES_UPPERCASE = 0x0008,

    //    ES_LOWERCASE = 0x0010,

    //    ES_PASSWORD = 0x0020,

    //    ES_AUTOVSCROLL = 0x0040,

    //    ES_AUTOHSCROLL = 0x0080,

    //    ES_NOHIDESEL = 0x0100,

    //    ES_COMBOBOX = 0x0200,

    //    ES_OEMCONVERT = 0x0400,

    //    ES_READONLY = 0x0800,

    //    ES_WANTRETURN = 0x1000,

    //    ES_NUMBER = 0x2000

    //}

    public partial class TextBoxEx : TextBox
    {
        #region Imports

        [DllImport("coredll.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("coredll.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        #endregion

        // The coordinates of the cursor the last time we saw a WM_MOUSEMOVE,
        // WM_LBUTTONDOWN or WM_LBUTTONUP message.
        Point lastCursorCoordinates;

        public delegate void MouseDelgate();
        public new event MouseDelgate MouseUp;
        //private EditStyle _editStyle = EditStyle.ES_LEFT;

        public TextBoxEx()
        {
            InitializeComponent();

            WndProcHooker.HookWndProc(this,
            new WndProcHooker.WndProcCallback(this.WM_LButtonUp_Handler),
            Win32.WM_LBUTTONUP);

            AllowSpace = true;

            //_editStyle = (EditStyle) GetWindowLong(this.Handle, (int)GetWindowLongParam.GWL_STYLE);
        }

        //public EditStyle EditStyle {
        //    get { return this._editStyle;}
        //    set {
        //        int style = GetWindowLong(this.Handle, (int)GetWindowLongParam.GWL_STYLE);
        //        _editStyle = (value);
        //        int result = SetWindowLong(this.Handle, (int)GetWindowLongParam.GWL_STYLE, (uint) _editStyle);
        //        style = GetWindowLong(this.Handle, (int)GetWindowLongParam.GWL_STYLE);
        //    }
        //}

        //public void AddEditStyle(EditStyle styleToAdd)
        //{
        //    _editStyle |= (styleToAdd);
        //    SetWindowLong(this.Handle, (int)GetWindowLongParam.GWL_STYLE, (uint)_editStyle);
            
        //}


        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (NumbersOnly)
            {
                NumberFormatInfo numberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
                string decimalSeparator = numberFormatInfo.NumberDecimalSeparator;
                string groupSeparator = numberFormatInfo.NumberGroupSeparator;
                string negativeSign = numberFormatInfo.NegativeSign;

                string keyInput = e.KeyChar.ToString();

                if (Char.IsDigit(e.KeyChar))
                {
                    // Digits are OK
                }
                else if (keyInput.Equals(decimalSeparator) || keyInput.Equals(groupSeparator) ||
                 keyInput.Equals(negativeSign))
                {
                    // Decimal separator is OK
                }
                else if (e.KeyChar == '\b')
                {
                    // Backspace key is OK
                }
                //    else if ((ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
                //    {
                //     // Let the edit control handle control and alt key combinations
                //    }
                else if (this.AllowSpace && e.KeyChar == ' ')
                {

                }
                else
                {
                    // Consume this invalid key and beep
                    e.Handled = true;
                    //    MessageBeep();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if one is allowed to insert spaces
        /// </summary>
        public bool AllowSpace
        {
            set;
            get;
        }
        /// <summary>
        /// Gets or sets a value indicating if one can insert only numbers
        /// </summary>
        public bool NumbersOnly
        {
            set;
            get;
        }

        // The callback called when the window receives a WM_LBUTTONUP
        // message. We release capture on the mouse, draw the button in the
        // "un-pushed" state and fire the  OnMouseUp event if the cursor was
        // let go of inside our client area.
        // hwnd - The handle to the window that received the
        // message
        // wParam - Indicates whether various virtual keys are
        // down.
        // lParam - The coordinates of the cursor
        // handled - Set to true if we don't want to pass this
        // message
        // on to the original window procedure
        // Returns zero if we process this message.
        int WM_LButtonUp_Handler(
            IntPtr hwnd, uint msg, uint wParam, int lParam,
            ref bool handled)
        {
            this.Capture = false;

            lastCursorCoordinates = Win32.LParamToPoint(lParam);
            if (this.ClientRectangle.Contains(lastCursorCoordinates))
                OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1,
                    lastCursorCoordinates.X, lastCursorCoordinates.Y, 0));
            handled = true;

            if (MouseUp != null)
                MouseUp();

            return 0;
        }
    }
}
