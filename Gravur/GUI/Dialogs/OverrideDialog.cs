using System; 
using System.Drawing; 
using System.Collections; 
using System.ComponentModel; 
using System.Windows.Forms;
using System.IO;

namespace GravurGIS.GUI.Dialogs
{
    public struct OverrideInformation
    {
        public bool PointChecked;
        public bool PolylineChecked;
        public bool PolygonChecked;
        public DialogResult dialogResult;

        public OverrideInformation(bool a, bool b, bool c, DialogResult d) {
            PointChecked = a;
            PolylineChecked = b;
            PolygonChecked = c;
            dialogResult = d;
        }
    }
    /// <summary> 
    /// Summary description for Form2. 
    /// </summary> 
    public class OverrideDialog : IDialog
    {
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label Caption;
        private System.Windows.Forms.Label text;
        private System.Windows.Forms.Panel panel1;
        private Button cancelButton;
        private CheckBox checkPolygon;
        private CheckBox checkPolyline;
        private CheckBox checkPoint;
        private MainControler mainControler;

        public OverrideDialog()
        {
            // 
            // Required for Windows Form Designer support 
            // 
            InitializeComponent();
            okButton.DialogResult = DialogResult.OK;
            cancelButton.DialogResult = DialogResult.Cancel;
            HideToolBar();
        }
        public OverrideDialog(Rectangle visibleRect, MainControler mainControler)
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
            this.checkPolygon = new System.Windows.Forms.CheckBox();
            this.checkPolyline = new System.Windows.Forms.CheckBox();
            this.checkPoint = new System.Windows.Forms.CheckBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.text = new System.Windows.Forms.Label();
            this.Caption = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkPolygon);
            this.panel1.Controls.Add(this.checkPolyline);
            this.panel1.Controls.Add(this.checkPoint);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Controls.Add(this.okButton);
            this.panel1.Controls.Add(this.text);
            this.panel1.Location = new System.Drawing.Point(1, 22);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(184, 175);
            // 
            // checkPolygon
            // 
            this.checkPolygon.Location = new System.Drawing.Point(12, 119);
            this.checkPolygon.Name = "checkPolygon";
            this.checkPolygon.Size = new System.Drawing.Size(100, 20);
            this.checkPolygon.TabIndex = 13;
            this.checkPolygon.Text = "Polygone";
            this.checkPolygon.Click += new System.EventHandler(this.checkPolygon_Click);
            // 
            // checkPolyline
            // 
            this.checkPolyline.Location = new System.Drawing.Point(12, 97);
            this.checkPolyline.Name = "checkPolyline";
            this.checkPolyline.Size = new System.Drawing.Size(100, 20);
            this.checkPolyline.TabIndex = 12;
            this.checkPolyline.Text = "Linienzüge";
            this.checkPolyline.Click += new System.EventHandler(this.checkPolyline_Click);
            // 
            // checkPoint
            // 
            this.checkPoint.Location = new System.Drawing.Point(12, 75);
            this.checkPoint.Name = "checkPoint";
            this.checkPoint.Size = new System.Drawing.Size(100, 20);
            this.checkPoint.TabIndex = 11;
            this.checkPoint.Text = "Punkte";
            this.checkPoint.Click += new System.EventHandler(this.checkPoint_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(12, 147);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(78, 20);
            this.cancelButton.TabIndex = 9;
            this.cancelButton.Text = "Abbrechen";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(95, 147);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(78, 20);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            // 
            // text
            // 
            this.text.Location = new System.Drawing.Point(12, 10);
            this.text.Name = "text";
            this.text.Size = new System.Drawing.Size(161, 61);
            this.text.Text = "Es sind bereits Austausch-Layer Dateien vorhanden. Welche möchten Sie davon übers" +
                "chreiben?";
            // 
            // Caption
            // 
            this.Caption.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular);
            this.Caption.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Caption.Location = new System.Drawing.Point(6, 4);
            this.Caption.Name = "Caption";
            this.Caption.Size = new System.Drawing.Size(172, 18);
            this.Caption.Text = "Überschreiben von Dateien";
            // 
            // OverrideDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(186, 198);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Caption);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimizeBox = false;
            this.Name = "OverrideDialog";
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseDown);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        void checkPolygon_Click(object sender, EventArgs e)
        {
            mainControler.setStatus(Path.ChangeExtension(mainControler.Config.ExPGonLayerFile, "shp"));
        }

        void checkPolyline_Click(object sender, EventArgs e)
        {
            mainControler.setStatus(Path.ChangeExtension(mainControler.Config.ExPLineLayerFile, "shp"));
        }

        void checkPoint_Click(object sender, EventArgs e)
        {
            mainControler.setStatus(Path.ChangeExtension(mainControler.Config.ExPntLayerFile, "shp"));
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

        public DialogResult ShowDialog(ref OverrideInformation enabledInformation)
        {
            if (enabledInformation.PointChecked)
            {
                this.checkPoint.Checked = false;
                this.checkPoint.Enabled = true;
            }
            else
            {
                this.checkPoint.Checked = true;
                this.checkPoint.Enabled = false;
            }
            if (enabledInformation.PolylineChecked)
            {
                this.checkPolyline.Checked = false;
                this.checkPolyline.Enabled = true;
            }
            else
            {
                this.checkPolyline.Checked = true;
                this.checkPolyline.Enabled = false;
            }
            if (enabledInformation.PolygonChecked)
            {
                this.checkPolygon.Checked = false;
                this.checkPolygon.Enabled = true;
            }
            else
            {
                this.checkPolygon.Checked = true;
                this.checkPolygon.Enabled = false;
            }

            return base.ShowDialog();
        }

        public override void resizeToRect(Rectangle visibleRect)
        {
            this.Location = new System.Drawing.Point((visibleRect.Width - this.Width) / 2,
                (visibleRect.Height - this.Height) / 2 + visibleRect.Y);
            this.Invalidate();
        }

        public OverrideInformation WhatToOverride
        {
            get
            {
                return new OverrideInformation(checkPoint.Checked,
                    checkPolyline.Checked, checkPolygon.Checked, DialogResult.OK);
            }
        }
    }
}
