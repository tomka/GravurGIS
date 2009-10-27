using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace GravurGIS.GUI.Controls
{
    /// <summary>
    /// A class that does the decimal up-down operation in compact framework
    /// </summary>
    public partial class DecimalUpDown : UserControl
    {
        public event ChangeFocusEventDelegate ChangeFocusEvent;
        public event ValueChangedEventDelegate ValueChanged;
        #region PUBLIC PROPERTIES
        //Properties
        public Decimal Value
        {
            set { txtValue.Text = string.Format("{0:0.00}", value);  }
            get { return Convert.ToDecimal(txtValue.Text); }
        }
        private Decimal increment = 0.5m;
        public Decimal Increment
        {
            set { increment = value; }
            get { return increment; }
        }
        private Decimal max = 100m;
        public Decimal Maximum
        {
            set { max = value; }
            get { return max; }
        }
        private Decimal min = 0m;
        public Decimal Minimum
        {
            set { min = value; }
            get { return min; }
        } 
        #endregion


        private object sender = null;
        public DecimalUpDown()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Don't Let the user increase the height of the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpDown_Resize(object sender, EventArgs e)
        {
            if (this.Height > txtValue.Height)
                this.Height = txtValue.Height;
        }


        /// <summary>
        /// If data in textbox is not decimal, stay on the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtValue_Validating(object sender, CancelEventArgs e)
        {
            tmrUpDown.Enabled = false;
            if (this.IsDecimal(txtValue.Text) == false)
            {
                e.Cancel = true;
                return;
            }
            
            // IF user have entered manually a higher value, don't validate
            decimal currentValue = Convert.ToDecimal(txtValue.Text);
            if (currentValue > max || currentValue < min)
                e.Cancel = true;
        }
        /// <summary>
        /// Increase or decrease the value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpDown_Click(object sender, EventArgs e)
        {
            if (this.IsDecimal(txtValue.Text) == false)
            {
                txtValue.Text = "0.00";
                return;
            }
            Decimal newValue = Convert.ToDecimal(txtValue.Text);
            // Chec the value is in the range
            if (((UserIcon)sender).Tag.ToString() == "UP") // Up Button Pressed
            {
                if (newValue < min) newValue = min;
                if (newValue + increment > max)
                    newValue = max;
                else
                    newValue += increment;
            }
            else
            {
                if (newValue > max) newValue = max;
                if (newValue - increment < min)
                    newValue = min;
                else
                    newValue -= increment;
            }

            txtValue.Text = string.Format("{0:0.00}", newValue);
        }

        /// <summary>
        /// Start timer once User click on the up down pictures
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpDown_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            tmrUpDown.Enabled = true;
            this.sender = sender;
        }
        /// <summary>
        /// Stop the timer when user lift up the mouse click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpDown_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            tmrUpDown.Enabled = false;
            if (ValueChanged != null) ValueChanged(this, e);
        }
        /// <summary>
        /// Check if this is Decimal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsDecimal(string value)
        {
            try
            {
                Decimal ret = Convert.ToDecimal(value);
                return true;
            }
            catch { return false; }
        }

        private void Control_ChangeFocus(object sender, EventArgs e)
        {
           tmrUpDown.Enabled = false;
           if (ChangeFocusEvent != null)
                ChangeFocusEvent(this, new FocusEventArgs(((Control) sender).Focused));
        }
        /// <summary>
        /// Change the value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrUpDown_Tick(object sender, EventArgs e)
        {
            UpDown_Click(this.sender, e);
        }


        // Boolean flag used to determine when a character other than a number is entered.
        private bool nonNumberEntered = false;
        private void txtValue_KeyDown(object sender, KeyEventArgs e)
        {
            // Initialize the flag to false.
            nonNumberEntered = false;
            // Determine whether the keystroke is a number, decimal or back space
            int keyValue = e.KeyValue;
            if (!(keyValue == 8 || keyValue == 190 ||
               (keyValue >= 48 && keyValue <= 57)))
                nonNumberEntered = true;
            if (keyValue == 190 && txtValue.Text.IndexOf(".") > -1) // Check decimal already exists
                nonNumberEntered = true;
        }
        private void txtValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for the flag being set in the KeyDown event.
            if (nonNumberEntered == true)
            {
                // Stop the character from being entered into the control since it is non-numerical.
                e.Handled = true;
            }
            if (ValueChanged != null) ValueChanged(this, e);
        }

    }
}