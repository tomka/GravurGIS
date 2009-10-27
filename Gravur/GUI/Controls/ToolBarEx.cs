using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace GravurGIS.GUI.Controls
{
    public partial class ToolBarEx : Control
    {
        private List<ToolbarButtonEx> buttons = new List<ToolbarButtonEx>();
        private uint _marginBorder = 1;
        
        /// <summary>
        /// Gets or sets the distance between each button and the control border
        /// </summary>
        public uint MarginBorder
        {
            get { return _marginBorder; }
            set { _marginBorder = value; }
        }

        public ToolBarEx()
        {
            InitializeComponent();
        }

        public ToolBarEx(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            for (int i = 0; i < buttons.Count; i++)
            {
                
            }
        }

        /// <summary>
        /// Gets or sets whether the toolbar defines its size alone according to its content and environment
        /// </summary>
        public bool AutoSize
        {
            get;
            set;
        }

        public List<ToolbarButtonEx> Buttons
        {
            get { return this.buttons; }
            set { this.buttons = value; this.Invalidate(); }
        }
    }
}
