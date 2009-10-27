using System; 
using System.Drawing; 
using System.Collections; 
using System.ComponentModel; 
using System.Windows.Forms;
using GravurGIS.GUI.Controls;

namespace GravurGIS.GUI.Dialogs
{
    [FlagsAttribute]
    public enum SaveChagesType
	{
	    NothingSpecial = 0,
        SaveOpenedProject = 1,
        SetAsDefault = 2,
        SaveTransportLayers = 4
	}

    /// <summary> 
    /// Summary description for Form2. 
    /// </summary> 
    public class CloseDialog : IDialog
    {
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Label Caption;
        private System.Windows.Forms.Label text;
        private System.Windows.Forms.Panel panel1;
        private Button cancelButton;
        private CheckBox cbSaveDefault;
        private CheckBox cbSaveTransportLayers;
        private CheckBox cbSaveOpenProject;
        private LabelEx currentProjectLabel;
        private Button button1;
        private MainControler mainControler;

        public CloseDialog()
        {
            // 
            // Required for Windows Form Designer support 
            // 
            InitializeComponent();
            base.HideToolBar();
            
            saveButton.DialogResult = DialogResult.Yes;
			cancelButton.DialogResult = DialogResult.Cancel;
			button1.DialogResult = DialogResult.Ignore;
        }
        public CloseDialog(Rectangle visibleRect, MainControler mainControler)
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
			this.button1 = new System.Windows.Forms.Button();
			this.currentProjectLabel = new GravurGIS.GUI.Controls.LabelEx();
			this.cbSaveOpenProject = new System.Windows.Forms.CheckBox();
			this.cbSaveTransportLayers = new System.Windows.Forms.CheckBox();
			this.cbSaveDefault = new System.Windows.Forms.CheckBox();
			this.cancelButton = new System.Windows.Forms.Button();
			this.saveButton = new System.Windows.Forms.Button();
			this.text = new System.Windows.Forms.Label();
			this.Caption = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.button1);
			this.panel1.Controls.Add(this.currentProjectLabel);
			this.panel1.Controls.Add(this.cbSaveOpenProject);
			this.panel1.Controls.Add(this.cbSaveTransportLayers);
			this.panel1.Controls.Add(this.cbSaveDefault);
			this.panel1.Controls.Add(this.cancelButton);
			this.panel1.Controls.Add(this.saveButton);
			this.panel1.Controls.Add(this.text);
			this.panel1.Location = new System.Drawing.Point(1, 22);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(238, 187);
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Ignore;
			this.button1.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
			this.button1.Location = new System.Drawing.Point(82, 162);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(71, 20);
			this.button1.TabIndex = 16;
			this.button1.Text = "Verwerfen";
			// 
			// currentProjectLabel
			// 
			this.currentProjectLabel.BackColor = System.Drawing.Color.White;
			this.currentProjectLabel.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.currentProjectLabel.Location = new System.Drawing.Point(27, 120);
			this.currentProjectLabel.Name = "currentProjectLabel";
			this.currentProjectLabel.Size = new System.Drawing.Size(207, 31);
			this.currentProjectLabel.TabIndex = 17;
			// 
			// cbSaveOpenProject
			// 
			this.cbSaveOpenProject.Location = new System.Drawing.Point(3, 99);
			this.cbSaveOpenProject.Name = "cbSaveOpenProject";
			this.cbSaveOpenProject.Size = new System.Drawing.Size(172, 20);
			this.cbSaveOpenProject.TabIndex = 14;
			this.cbSaveOpenProject.Text = "Projekt speichern unter:";
			// 
			// cbSaveTransportLayers
			// 
			this.cbSaveTransportLayers.Location = new System.Drawing.Point(3, 77);
			this.cbSaveTransportLayers.Name = "cbSaveTransportLayers";
			this.cbSaveTransportLayers.Size = new System.Drawing.Size(168, 20);
			this.cbSaveTransportLayers.TabIndex = 13;
			this.cbSaveTransportLayers.Text = "Austauschlayer speichern";
			// 
			// cbSaveDefault
			// 
			this.cbSaveDefault.Location = new System.Drawing.Point(3, 56);
			this.cbSaveDefault.Name = "cbSaveDefault";
			this.cbSaveDefault.Size = new System.Drawing.Size(213, 20);
			this.cbSaveDefault.TabIndex = 12;
			this.cbSaveDefault.Text = "Als Standardprojekt setzen";
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
			this.cancelButton.Location = new System.Drawing.Point(5, 162);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(69, 20);
			this.cancelButton.TabIndex = 9;
			this.cancelButton.Text = "Abbrechen";
			// 
			// saveButton
			// 
			this.saveButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.saveButton.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
			this.saveButton.Location = new System.Drawing.Point(160, 162);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(71, 20);
			this.saveButton.TabIndex = 0;
			this.saveButton.Text = "Speichern";
			// 
			// text
			// 
			this.text.Location = new System.Drawing.Point(5, 7);
			this.text.Name = "text";
			this.text.Size = new System.Drawing.Size(230, 48);
			this.text.Text = "Es wurden ungespeicherte Änderungen am Projekt vorgenommen. Möchten Sie diese Änd" +
				"erungen speichern?\r\n";
			// 
			// Caption
			// 
			this.Caption.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular);
			this.Caption.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.Caption.Location = new System.Drawing.Point(6, 4);
			this.Caption.Name = "Caption";
			this.Caption.Size = new System.Drawing.Size(172, 18);
			this.Caption.Text = "Änderungen Speichern?";
			// 
			// CloseDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.ClientSize = new System.Drawing.Size(240, 210);
			this.ControlBox = false;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.Caption);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MinimizeBox = false;
			this.Name = "CloseDialog";
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

        public new DialogResult ShowDialog()
        {
            if (mainControler.OpenendFileName != "")
            {
                currentProjectLabel.Text = mainControler.OpenendFileName;
                currentProjectLabel.Enabled = true;
                cbSaveOpenProject.Enabled = true;
                
                if (mainControler.OpenendFileName == mainControler.Config.DefaultProject
						&& mainControler.Config.UseDefaultProject) {
					cbSaveDefault.Text = "Als Standardprojekt beibehalten";
					
					cbSaveDefault.Checked = true;
                }

				cbSaveOpenProject.Checked = true;
					
            }
            else
            {
                currentProjectLabel.Enabled = false;
                cbSaveOpenProject.Enabled = false;
                currentProjectLabel.Text = "(Kein Projekt geöffnet)";
            }

            return base.ShowDialog();
        }

        public override void resizeToRect(Rectangle visibleRect)
        {
            this.Location = new System.Drawing.Point((visibleRect.Width - this.Width) / 2,
                (visibleRect.Height - this.Height) / 2 + visibleRect.Y);
            this.Invalidate();
        }
        public SaveChagesType SaveType
        {
            get
            {
                SaveChagesType saveChanges = SaveChagesType.NothingSpecial;

                if (cbSaveDefault.Checked)
                    saveChanges = saveChanges | SaveChagesType.SetAsDefault;
                if (cbSaveTransportLayers.Checked)
                    saveChanges = saveChanges | SaveChagesType.SaveTransportLayers;
                if (cbSaveOpenProject.Checked)
                    saveChanges = saveChanges | SaveChagesType.SaveOpenedProject;

                return saveChanges;
            }
        }
    }
}
