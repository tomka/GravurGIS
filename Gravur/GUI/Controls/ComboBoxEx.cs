using System;

namespace GravurGIS.GUI.Controls
{
    /// 
    /// Editable ComboBox.
    /// 
    public class ComboBoxEx : System.Windows.Forms.Control
    {
        private TextBoxEx tb = new TextBoxEx();
        private System.Windows.Forms.ComboBox cb = new System.Windows.Forms.ComboBox();

        private bool c_blnUpdatingValue = false;

        public event System.EventHandler SelectedIndexChanged;

        public event EventHandler EnterHit;

        public ComboBoxEx()
        {
            //Load the textbox and comboBox and add them to this control
            tb.Show();
            this.Controls.Add(tb);
            cb.Show();

            this.Controls.Add(cb);
            tb.BringToFront();

            //tb.Multiline = true; // we need this to set the size properly
            //tb.WordWrap = false;

            cb.SelectedValueChanged += new EventHandler(cb_SelectedValueChanged);
            cb.SelectedIndexChanged += new EventHandler(cb_SelectedIndexChanged);

            tb.LostFocus += new EventHandler(tb_TextChanged);

            tb.KeyUp += new System.Windows.Forms.KeyEventHandler(ComboBox_KeyUp);
        }

        /// <summary>
        /// Gets or sets a value indicating if one can insert only numbers in the edit field
        /// </summary>
        public bool NumbersOnly
        {
            get { return tb.NumbersOnly; }
            set { tb.NumbersOnly = value; }
        }

        /// 
        /// Gets or sets the Width of the control
        /// 
        public new int Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                //set the width of the textbox and combo box
                base.Width = value;
                tb.Width = base.Width - 14; //- the Drop Down button Size
                cb.Width = base.Width;
            }
        }

        /// 
        /// Gets or sets the Height of the control
        /// 
        public new int Height
        {
            get
            {
                return base.Height;
            }
            set
            {
                //set the height of the textbox and combo box
                cb.Height = value;
                tb.Height = base.Height = cb.Height;
            }
        }

        public new System.Drawing.Size Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
                this.Height = base.Height;
                this.Width = base.Width;
            }
        }

        /// 
        /// Gets or sets the Background color of the control
        /// 
        public new System.Drawing.Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                tb.BackColor = base.BackColor;
                cb.BackColor = base.BackColor;
            }
        }

        /// 
        /// Gets or sets the text associated with the control
        /// 
        public new string Text
        {
            get
            {
                return tb.Text;
            }
            set
            {
                cb.Text = value;
            }
        }

        public new bool Focus()
        {
            return tb.Focus();
        }

        /// 
        /// Gets an object representing the collection of the objects contained
        /// in this comboBox
        /// 
        public System.Windows.Forms.ComboBox.ObjectCollection Items
        {
            get
            {
                return cb.Items;
            }
        }

        /// 
        /// Gets or set the value of the member property specified by the
        /// System.Windows.Forms.ListControl.ValueMember property
        /// 
        public object SelectedValue
        {
            get
            {
                return cb.SelectedValue;
            }
            set
            {
                cb.SelectedValue = value;
                //this is used because the cb.SelectedValue change does not get triggered
                //from here for some reason.
                cb.Refresh();
                tb.Text = cb.Text;
            }
        }

        /// 
        /// Gets or sets the dataSource for the System.Windows.Forms.ListControl
        /// 
        public object DataSource
        {
            get
            {
                return cb.DataSource;
            }
            set
            {
                cb.DataSource = value;
            }
        }

        /// 
        /// Gets the dataBindings for the control
        /// 
        public new System.Windows.Forms.ControlBindingsCollection DataBindings
        {
            get
            {
                return cb.DataBindings;
            }
        }

        /// 
        /// Gets or sets the string the specifies the property of the datasource
        /// whose contents you want to display
        /// 
        public String DisplayMember
        {
            get
            {
                return cb.DisplayMember;
            }
            set
            {
                cb.DisplayMember = value;
            }
        }

        /// 
        /// Gets or sets the index specifing the selected item.
        /// 
        public int SelectedIndex
        {
            get
            {
                return cb.SelectedIndex;
            }
            set
            {
                cb.SelectedIndex = value;
            }
        }

        /// 
        /// Gets or sets a string the specifies the property of the datasource 
        /// from which to draw the value.
        /// 
        public string ValueMember
        {
            get
            {
                return cb.ValueMember;
            }
            set
            {
                cb.ValueMember = value;
            }
        }

        public int FindStringExact(String strText)
        {
            try
            {
                for (int i = 0; i < cb.Items.Count; i++)
                {
                    if (strText == cb.GetItemText(cb.Items[i]))
                    {
                        return i;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new System.Exception("Error finding String. KAL.Handheld.Controls.ComboBox.FindExact", ex);
            }
            throw new System.Exception("Entry not found in drop down");
        }

        /// 
        /// when the ComboBox is changed, change the textbox text to reflect the
        /// text.
        /// 
        /// 
        /// 
        private void cb_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                //So that does not cause endless loop of updating
                if (this.c_blnUpdatingValue == false)
                {
                    this.c_blnUpdatingValue = true;
                    tb.Text = cb.Text;
                    this.c_blnUpdatingValue = false;
                }
            }
            catch { }
        }

        /// 
        /// send notice of change
        /// 
        /// 
        /// 
        private void cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.SelectedIndexChanged(this, e);
            }
            catch//This is used in case there is nothing listening for this change
            {
            }
        }

        /// 
        /// set the comboBox value to this value
        /// 
        /// 
        /// 
        private void tb_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //So that does not cause endless loop of updating
                if (this.c_blnUpdatingValue == false)
                {
                    this.c_blnUpdatingValue = true;
                    cb.Text = tb.Text;
                    this.c_blnUpdatingValue = false;
                }
            }
            catch
            { }
        }

        private void ComboBox_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    this.Focus();
                    EnterHit(this, e);
                }
            }
            catch { }//This is used in case there is nothing listening for this event
        }
    }
}