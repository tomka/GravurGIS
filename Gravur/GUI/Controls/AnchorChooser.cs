using System.Windows.Forms;
using System.Drawing;
using System.Text;
using GravurGIS.Styles;

namespace GravurGIS.GUI.Controls
{
    public class AnchorChooser : System.Windows.Forms.Control
    {
        public delegate void ChangedDelegate();
        public event ChangedDelegate Changed;

        public GravurGIS.Styles.HorizontalAlignment HorizontalAlginment
        {
            get;
            set;
        }
        public VerticalAlignment VerticalAlignment
        {
            get;
            set;
        }
        public Color SelectedColor
        {
            get;
            set;
        }

        public AnchorChooser()
            : base()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            SelectedColor = SystemColors.Highlight;
            ForeColor = Color.Black;
            BackColor = Color.White;
            VerticalAlignment = VerticalAlignment.Middle;
            HorizontalAlginment = GravurGIS.Styles.HorizontalAlignment.Center;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            int cellWidth = (this.Width - (this.Width % 3)) / 3;
            int cellHeight = (this.Height - (this.Height % 3)) / 3;

            // get the app. cell and make sure we do not get overboard
            this.HorizontalAlginment = (GravurGIS.Styles.HorizontalAlignment) System.Math.Min((int)(e.X / (float)cellWidth),2);
            this.VerticalAlignment = (VerticalAlignment) System.Math.Min((int)(e.Y / (float)cellHeight), 2);
            
            this.Invalidate();

            if (Changed != null) Changed();
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
            Pen borderPen = new Pen(ForeColor);

            // the border
            e.Graphics.DrawRectangle(
                borderPen,
                new System.Drawing.Rectangle(0,0,this.Width - 1, this.Height - 1));

            //vertical splitters
            int xRest = this.Width % 3;
            int cellWidth = (this.Width - xRest) / 3;
            e.Graphics.DrawLine(borderPen,
                cellWidth, 0, cellWidth, this.Height);
            e.Graphics.DrawLine(borderPen,
                2* cellWidth, 0, 2 * cellWidth, this.Height);

            //horizontal splitters
            int yRest = this.Height % 3;
            int cellHeight = (this.Height - yRest) / 3;
            e.Graphics.DrawLine(borderPen,
                0, cellHeight, this.Width, cellHeight);
            e.Graphics.DrawLine(borderPen,
                0, 2 * cellHeight, this.Width, 2 * cellHeight);

            //selected area
            int xMultipli = (int)HorizontalAlginment;
            int yMultipli = (int)VerticalAlignment;

            e.Graphics.FillRectangle(new SolidBrush(SelectedColor),
                new Rectangle(
                    xMultipli * cellWidth + 1,
                    yMultipli * cellHeight + 1,
                    cellWidth + ((xMultipli < 2) ? 0 : --xRest) - 1,
                    cellHeight + ((yMultipli < 2) ? 0 : --yRest) - 1));
        }

        public override string ToString()
        {
            if (HorizontalAlginment == GravurGIS.Styles.HorizontalAlignment.Center &&
                VerticalAlignment == VerticalAlignment.Middle)
                return "die Bildschirmmitte";

            StringBuilder result = new StringBuilder();

            switch (HorizontalAlginment)
            {
                case GravurGIS.Styles.HorizontalAlignment.Left:
                    result.Append("links");
                    break;
                case GravurGIS.Styles.HorizontalAlignment.Center:
                    result.Append("mitte");
                    break;
                case GravurGIS.Styles.HorizontalAlignment.Right:
                    result.Append("rechts");
                    break;
            }

            switch (VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    result.Append(" oben");
                    break;
                case VerticalAlignment.Middle:
                    result.Append(" mitte");
                    break;
                case VerticalAlignment.Bottom:
                    result.Append(" unten");
                    break;
            }

            return result.ToString();
        }
    }
}
