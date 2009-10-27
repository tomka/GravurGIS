using System.Windows.Forms;

namespace GravurGIS.GUI.Controls
{
    public class ColorIcon : System.Windows.Forms.Control
    {
        private System.Drawing.Pen pen;

        public ColorIcon() : base()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            pen = new System.Drawing.Pen(System.Drawing.Color.Black);
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
            e.Graphics.DrawRectangle(pen, new System.Drawing.Rectangle(0,0,this.Width - 1, this.Height - 1));
        }
    }
}
