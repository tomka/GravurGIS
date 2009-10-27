using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace GravurGIS.GUI.Controls
{
    public class ToolTip : Control
    {
        private Pen pen;
        private bool sizeCalculated = false;

        //public ToolTip(String Text, Point Position)
        //{
        //    this.Location = Position;
        //    this.Text = Text;
        //    pen = new System.Drawing.Pen(System.Drawing.Color.Black);
        //}
        public ToolTip()
            : base()
        {
            pen = new System.Drawing.Pen(System.Drawing.Color.Black);
            this.BackColor = Color.LemonChiffon;
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                sizeCalculated = false;
                base.Text = value;
            }
        }
        public new Point Location
        {
            get
            {
                return base.Location;
            }
            set
            {
                sizeCalculated = false;
                base.Location = value;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            pen.Dispose();
            base.Dispose(disposing);
        }


        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (!sizeCalculated)
            {
                // Get boundingbox size of string
                SizeF sizeF =  e.Graphics.MeasureString(Text, Font);
                Size size = new Size((int)(Math.Ceiling(sizeF.Width)), (int)(Math.Ceiling(sizeF.Height)));
                Size screenSize = Parent.ClientSize; // should be the MapPanel in our case

                this.Width = size.Width + 10;
                this.Height = size.Height + 10;

                int right = this.Width + this.Left + 2; // + 2 wegen dem Rahmen des MapPanels
                int top = this.Height + this.Top + 2;

                if (right > screenSize.Width)
                    this.Left = Math.Max(3, this.Left - (right - screenSize.Width));
                if (top > screenSize.Height)
                    this.Top = Math.Max(3, this.Top - (top - screenSize.Height));
                
                this.sizeCalculated = true;
                this.Invalidate();
                return;

            }
            base.OnPaintBackground(e);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            if (sizeCalculated)
            {
                e.Graphics.DrawString(Text, Font, new SolidBrush(Color.Black),
                    new Rectangle(3, 2, Width - 4, Height - 4));
                e.Graphics.DrawRectangle(pen, new System.Drawing.Rectangle(0, 0, this.Width - 1, this.Height - 1));
            }
        }
    }
}
