using System; 
using System.Drawing; 
using System.Collections; 
using System.ComponentModel; 
using System.Windows.Forms;

namespace GravurGIS.GUI.Dialogs
{
    /// <summary> 
    /// Summary description for Form2. 
    /// </summary> 
    public class ErrorDialog : IDialog
    {
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Label Caption;
        private System.Windows.Forms.Label text;
        private TextBox textBox1;
        private System.Windows.Forms.Panel panel1;

        public ErrorDialog()
            : base()
        {
            // 
            // Required for Windows Form Designer support 
            // 
            InitializeComponent();
            saveButton.DialogResult = DialogResult.OK;
        }
        public ErrorDialog(Rectangle visibleRect, Config config)
            : this()
        {
            this.Location = new System.Drawing.Point((visibleRect.Width - this.Width) / 2,
                (visibleRect.Height - this.Height) / 2 + visibleRect.Y);
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.text = new System.Windows.Forms.Label();
            this.Caption = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.text);
            this.panel1.Location = new System.Drawing.Point(1, 22);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(197, 176);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(11, 29);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(173, 118);
            this.textBox1.TabIndex = 3;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(11, 153);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(173, 20);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "OK";
            // 
            // text
            // 
            this.text.Location = new System.Drawing.Point(11, 10);
            this.text.Name = "text";
            this.text.Size = new System.Drawing.Size(172, 16);
            this.text.Text = "Es ist ein Fehler aufgetreten.";
            // 
            // Caption
            // 
            this.Caption.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular);
            this.Caption.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Caption.Location = new System.Drawing.Point(6, 4);
            this.Caption.Name = "Caption";
            this.Caption.Size = new System.Drawing.Size(170, 18);
            this.Caption.Text = "Programmfehler";
            // 
            // ErrorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(200, 200);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Caption);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimizeBox = false;
            this.Name = "ErrorDialog";
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseDown);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        int Xdif, Ydif;
        private void Form2_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Xdif = e.X;
            Ydif = e.Y;
        }
        private void Form2_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Location = new Point(Location.X + e.X - Xdif, Location.Y + e.Y - Ydif);
        }

        public string Comment
        {
            get { return this.textBox1.Text; }
            set { this.textBox1.Text = value; }
        }
        public void resetComment()
        {
            this.textBox1.Text = "";
        }
        public DialogResult ShowDialog(string errMsg)
        {
            this.textBox1.Text = errMsg;
            return base.ShowDialog();
        }

        public override void resizeToRect(Rectangle visibleRect)
        {
            this.Location = new System.Drawing.Point((visibleRect.Width - this.Width) / 2,
                (visibleRect.Height - this.Height) / 2 + visibleRect.Y);
            this.Invalidate();
        }
    }
}
