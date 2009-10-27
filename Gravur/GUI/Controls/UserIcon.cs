using System.Windows.Forms;
using System.Drawing;
using System;

namespace GravurGIS.GUI.Controls
{
    public class UserIcon : System.Windows.Forms.Control
    {
        public enum IconType
        {
            None, UpArrow, DownArrow
        }

        private IconType type = IconType.None;
        private bool hasBorder = true;
        private Point[] points;
        private Color borderColor;

        public UserIcon()
            : base()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.ForeColor = this.BorderColor = Color.Black;
            this.BackColor = Color.White;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // ColorIcon1
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(20, 20);
            this.Name = "ColorIcon1";
        }
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            if (type != IconType.None)
                e.Graphics.FillPolygon(new System.Drawing.SolidBrush(this.ForeColor), points);

            // finally draw the border if neccecary
            if (hasBorder)
                e.Graphics.DrawRectangle(new Pen(borderColor), new System.Drawing.Rectangle(0,0,
                    this.Width - 1, this.Height - 1));
        }

        public IconType Type
        {
            get { return type; }
            set
            {
                type = value;
                switch (type)
                {
                    case IconType.UpArrow:
                        points = new Point[3];
                        points[0] = new Point((int)Math.Ceiling(this.Width / 2.0),
                                                (int)Math.Ceiling(this.Height / 4.0));
                        points[1] = new Point((int)Math.Floor(this.Width / 4.0),
                                                (int)Math.Ceiling(this.Height - this.Height / 4.0));
                        points[2] = new Point((int)Math.Ceiling(this.Width - this.Width / 4.0),
                                                (int)Math.Ceiling(this.Height - this.Height / 4.0));
                        break;
                    case IconType.DownArrow:
                        points = new Point[3];
                        points[0] = new Point((int)Math.Ceiling(this.Width / 2.0),
                                                (int)Math.Ceiling(this.Height - this.Height / 4.0));
                        points[1] = new Point((int)Math.Floor(this.Width / 4.0),
                                                (int)Math.Ceiling(this.Height / 4.0));
                        points[2] = new Point((int)Math.Ceiling(this.Width - this.Width / 4.0),
                                                (int)Math.Ceiling(this.Height / 4.0));
                        break;
                    default:
                        break;
                }
            }
        }
        public bool HasBorder
        {
            get { return hasBorder; }
            set { hasBorder = value; }
        }
        public Color BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; }
        }
    }
}
