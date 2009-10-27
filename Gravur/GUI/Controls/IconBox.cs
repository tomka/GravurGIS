using System.Windows.Forms;
using System.Drawing;

namespace GravurGIS.GUI.Controls
{
    public class IconBox : System.Windows.Forms.Control
    {
        private Icon icon;
        private Color selected_BackColor = Color.LightGray;
        private Color normal_BackColor = SystemColors.Control;
        private bool drawBorder = false;
        private Pen BorderPen;

        public Icon Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                if (icon != null)
                {
                    this.Width = icon.Width;
                    this.Height = icon.Height;
                    this.Invalidate();
                }
            }
        }
	
        public IconBox() : base()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.BorderPen = new Pen(Color.Indigo);
            this.MouseDown += new MouseEventHandler(IconBox_MouseDown);
            this.MouseUp += new MouseEventHandler(IconBox_MouseUp);
        }

        void IconBox_MouseUp(object sender, MouseEventArgs e)
        {
            this.BackColor = Normal_BackColor;
            this.drawBorder = false;
            this.Invalidate();
        }

        void IconBox_MouseDown(object sender, MouseEventArgs e)
        {
            this.BackColor = Selected_BackColor;
            this.drawBorder = true;
            this.Invalidate();
        }

        public IconBox(Icon icon)
        {
            this.icon = icon;
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
            // IconBox
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(16, 16);
            this.Name = "ColorIcon1";
        }
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            if (icon != null)
                e.Graphics.DrawIcon(icon, 0, 0);
            //else
            //    e.Graphics.FillRectangle(new SolidBrush(BackColor), new Rectangle(0, 0, Width, Height));

            if (drawBorder)
                e.Graphics.DrawRectangle(BorderPen,
                    new System.Drawing.Rectangle(0, 0, this.Width - 1, this.Height - 1));
        }

        public Color Normal_BackColor
        {
            get { return normal_BackColor; }
            set
            {
                normal_BackColor = value;
                BackColor = value;
            }
        }

        public Color Selected_BackColor
        {
            get { return selected_BackColor; }
            set { selected_BackColor = value; }
        }
    }
}
