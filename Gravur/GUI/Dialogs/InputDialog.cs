using System; 
using System.Drawing;
using System.Windows.Forms;

namespace GravurGIS.GUI.Dialogs
{
    /// <summary> 
    /// Summary description for Form2. 
    /// </summary> 
    public class InputDialog : IDialog
    {
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label Caption;
        private System.Windows.Forms.Label text;
        private TextBox textBox1;
        private System.Windows.Forms.Panel panel1;

        protected InputDialog()
        {
            // 
            // Required for Windows Form Designer support 
            // 
            InitializeComponent();
            saveButton.DialogResult = DialogResult.OK;
            cancelButton.DialogResult = DialogResult.Cancel;
        }
        public InputDialog(String Caption, String Message, Rectangle visibleRect)
            : this()
        {
            this.Location = new System.Drawing.Point((visibleRect.Width - this.Width) / 2,
                (visibleRect.Height - this.Height) / 2 + visibleRect.Y);

            this.Caption.Text = Caption;
            this.text.Text = Message;
        }

        /// <summary> 
        /// Clean up any resources being used. 
        /// </summary> 
        protected override void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.text = new System.Windows.Forms.Label();
            this.Caption = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Controls.Add(this.text);
            this.panel1.Location = new System.Drawing.Point(3, 22);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(182, 100);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 45);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(159, 21);
            this.textBox1.TabIndex = 8;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveButton.Location = new System.Drawing.Point(12, 74);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(72, 20);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.Location = new System.Drawing.Point(99, 74);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(72, 20);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Abbrechen";
            // 
            // text
            // 
            this.text.Location = new System.Drawing.Point(12, 10);
            this.text.Name = "text";
            this.text.Size = new System.Drawing.Size(161, 31);
            this.text.Text = "Text";
            // 
            // Caption
            // 
            this.Caption.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular);
            this.Caption.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Caption.Location = new System.Drawing.Point(6, 4);
            this.Caption.Name = "Caption";
            this.Caption.Size = new System.Drawing.Size(172, 18);
            this.Caption.Text = "Caption";
            // 
            // InputDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(188, 124);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Caption);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimizeBox = false;
            this.Name = "InputDialog";
            this.TopMost = true;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseUp);
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

        public string UserInput
        {
            get { return this.textBox1.Text; }
            set { this.textBox1.Text = value; }
        }

        public void resetComment()
        {
            this.textBox1.Text = "";
        }
        public new DialogResult ShowDialog()
        {
            this.textBox1.Focus();
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
