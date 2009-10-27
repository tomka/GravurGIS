using System;
using System.Collections.Generic;
using System.Text;

namespace GravurGIS.GUI.Controls
{
    /// <summary>
    /// Would hold whether event is fired by got or lost focus event
    /// </summary>
    public class FocusEventArgs : EventArgs
    {
        private bool focused;

        public FocusEventArgs(bool focused)
        {
            this.focused = focused;
        }

        public bool Focused
        {
            get { return focused; }
        }
    }
    public delegate void ChangeFocusEventDelegate(object sender, FocusEventArgs e);
    public delegate void ValueChangedEventDelegate(object sender, EventArgs e);
}
