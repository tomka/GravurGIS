using System; 
using System.Drawing; 
using System.Collections; 
using System.ComponentModel; 
using System.Windows.Forms;

namespace GravurGIS.GUI.Dialogs
{
    public enum SaveType
	{
	         Composition, TransPortLayer, Project
	}

    /// <summary> 
    /// Summary description for Form2. 
    /// </summary> 
    public class SaveDialog : IDialog
    {
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label Caption;
        private System.Windows.Forms.Label text;
        private System.Windows.Forms.Panel panel1;
        private Button cancelButton;
        private CheckBox chkProject;
        private CheckBox chkTransport;
        private MainControler mainControler;

        public SaveDialog()
        {
            // 
            // Required for Windows Form Designer support 
            // 
            InitializeComponent();
            okButton.DialogResult = DialogResult.OK;
            cancelButton.DialogResult = DialogResult.Cancel;
            base.HideToolBar();
        }
        public SaveDialog(Rectangle visibleRect, MainControler mainControler)
            : this()
        {
            this.mainControler = mainControler;
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.text = new System.Windows.Forms.Label();
            this.Caption = new System.Windows.Forms.Label();
            this.chkTransport = new System.Windows.Forms.CheckBox();
            this.chkProject = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkProject);
            this.panel1.Controls.Add(this.chkTransport);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Controls.Add(this.okButton);
            this.panel1.Controls.Add(this.text);
            this.panel1.Location = new System.Drawing.Point(1, 22);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(184, 151);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(12, 121);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(78, 20);
            this.cancelButton.TabIndex = 9;
            this.cancelButton.Text = "Abbrechen";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(95, 121);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(78, 20);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            // 
            // text
            // 
            this.text.Location = new System.Drawing.Point(12, 10);
            this.text.Name = "text";
            this.text.Size = new System.Drawing.Size(161, 73);
            this.text.Text = "Möchten Sie das Projekt, die Austausch-Layer oder beides speichern?";
            // 
            // Caption
            // 
            this.Caption.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular);
            this.Caption.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Caption.Location = new System.Drawing.Point(6, 4);
            this.Caption.Name = "Caption";
            this.Caption.Size = new System.Drawing.Size(172, 18);
            this.Caption.Text = "Speichern";
            // 
            // chkTransport
            // 
            this.chkTransport.Checked = true;
            this.chkTransport.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTransport.Location = new System.Drawing.Point(12, 89);
            this.chkTransport.Name = "chkTransport";
            this.chkTransport.Size = new System.Drawing.Size(161, 20);
            this.chkTransport.TabIndex = 11;
            this.chkTransport.Text = "Austausch-Layer";
            // 
            // chkProject
            // 
            this.chkProject.Checked = true;
            this.chkProject.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkProject.Location = new System.Drawing.Point(12, 63);
            this.chkProject.Name = "chkProject";
            this.chkProject.Size = new System.Drawing.Size(119, 20);
            this.chkProject.TabIndex = 12;
            this.chkProject.Text = "Projekt";
            // 
            // SaveDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(186, 175);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Caption);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimizeBox = false;
            this.Name = "SaveDialog";
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

        public new DialogResult ShowDialog()
        {
            this.chkProject.Checked = true;
            this.chkTransport.Checked = true;

            return base.ShowDialog();
        }

        public override void resizeToRect(Rectangle visibleRect)
        {
            this.Location = new System.Drawing.Point((visibleRect.Width - this.Width) / 2,
                (visibleRect.Height - this.Height) / 2 + visibleRect.Y);
            this.Invalidate();
        }
        public SaveType SaveType
        {
            get
            {
                if (chkProject.Checked && chkTransport.Checked) return SaveType.Composition;
                else if (chkTransport.Checked) return SaveType.TransPortLayer;
                else return SaveType.Project;
            }
        }
    }
}
