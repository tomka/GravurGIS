using System; 
using System.Drawing; 
using System.Collections; 
using System.ComponentModel; 
using System.Windows.Forms;
using GravurGIS.Topology;
using GravurGIS.GUI.Controls;
using GravurGIS.Styles;

namespace GravurGIS.GUI.Dialogs
{
    /// <summary> 
    /// Summary description for Form2. 
    /// </summary> 
    public class Navigator : IDialog
    {
        private System.Windows.Forms.Label Caption;
        private RadioButton radioGPS;
        private TabControl tabControl1;
        private TabPage navTab;
        private TabPage relTab;
        private Label label4;
        private RadioButton radioManual;
        private Label label5;
        private Label label3;
        private TextBoxEx tBRW;
        private Label label2;
        private TextBoxEx tbHW;
        private Label label1;
        private GravurGIS.GUI.Controls.AnchorChooser anchorChooser1;
        private GravurGIS.GUI.Controls.ComboBoxEx cbScale;
        private Label label6;
        private Label lblAnchorSTatus;
        private MainControler mainControler;

        public Navigator()
        {
            // 
            // Required for Windows Form Designer support 
            // 
            InitializeComponent();
            tbHW.NumbersOnly = true;
            tbHW.AllowSpace = false;
            tBRW.NumbersOnly = true;
            tBRW.AllowSpace = false;

        }
        public Navigator(Rectangle visibleRect, MainControler mainControler)
            : this()
        {
            this.mainControler = mainControler;
            this.Location = new System.Drawing.Point((visibleRect.Width - this.Width) / 2,
                (visibleRect.Height - this.Height) / 2 + visibleRect.Y);

            this.cbScale.NumbersOnly = true;

            fillDialogWithData();

            // add addtional data
            this.cbScale.Items.Add("5.000");
            this.cbScale.Items.Add("10.000");
            this.cbScale.Items.Add("25.000");
            this.cbScale.Items.Add("100.000");
            this.cbScale.Items.Add("250.000");
            this.cbScale.Items.Add("500.000");
            this.cbScale.Items.Add("750.000");
            this.cbScale.Items.Add("1.000.000");
            this.cbScale.Items.Add("2.500.000");
            this.cbScale.Items.Add("5.000.000");
        }

        private void fillDialogWithData()
        {
            if (mainControler.HasActiveDisplay)
            {
                UpdateMapPos();

                cbScale.Items.Add(mainControler.MapPanel.ScaleDivisor.ToString("#,##"));
                cbScale.SelectedIndex = 0;

                // make the GPS option only available if there is GPS
                radioGPS.Enabled = mainControler.GPSAvailable;

                lblAnchorSTatus.Text = String.Format("Die Navigations-Parameter beziehen sich im Moment auf {0}.", anchorChooser1.ToString());
            }
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
            this.radioGPS = new System.Windows.Forms.RadioButton();
            this.Caption = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.navTab = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.cbScale = new GravurGIS.GUI.Controls.ComboBoxEx();
            this.radioManual = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tBRW = new TextBoxEx();
            this.label2 = new System.Windows.Forms.Label();
            this.tbHW = new TextBoxEx();
            this.label1 = new System.Windows.Forms.Label();
            this.relTab = new System.Windows.Forms.TabPage();
            this.lblAnchorSTatus = new System.Windows.Forms.Label();
            this.anchorChooser1 = new GravurGIS.GUI.Controls.AnchorChooser();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.navTab.SuspendLayout();
            this.relTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioGPS
            // 
            this.radioGPS.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.radioGPS.Location = new System.Drawing.Point(14, 71);
            this.radioGPS.Name = "radioGPS";
            this.radioGPS.Size = new System.Drawing.Size(161, 20);
            this.radioGPS.TabIndex = 7;
            this.radioGPS.TabStop = false;
            this.radioGPS.Text = "Aktueller Standpunkt (GPS)";
            this.radioGPS.CheckedChanged += new System.EventHandler(this.radios_CheckedChanged);
            // 
            // Caption
            // 
            this.Caption.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular);
            this.Caption.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Caption.Location = new System.Drawing.Point(6, 4);
            this.Caption.Name = "Caption";
            this.Caption.Size = new System.Drawing.Size(172, 18);
            this.Caption.Text = "Navigator";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tabControl1.Controls.Add(this.navTab);
            this.tabControl1.Controls.Add(this.relTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.None;
            this.tabControl1.Location = new System.Drawing.Point(3, 22);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(222, 187);
            this.tabControl1.TabIndex = 2;
            // 
            // navTab
            // 
            this.navTab.Controls.Add(this.label6);
            this.navTab.Controls.Add(this.cbScale);
            this.navTab.Controls.Add(this.radioManual);
            this.navTab.Controls.Add(this.radioGPS);
            this.navTab.Controls.Add(this.label5);
            this.navTab.Controls.Add(this.label3);
            this.navTab.Controls.Add(this.tBRW);
            this.navTab.Controls.Add(this.label2);
            this.navTab.Controls.Add(this.tbHW);
            this.navTab.Controls.Add(this.label1);
            this.navTab.Location = new System.Drawing.Point(0, 0);
            this.navTab.Name = "navTab";
            this.navTab.Size = new System.Drawing.Size(222, 164);
            this.navTab.Text = "Navigation";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(14, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(22, 20);
            this.label6.Text = "1 :";
            // 
            // cbScale
            // 
            this.cbScale.Location = new System.Drawing.Point(39, 23);
            this.cbScale.Name = "cbScale";
            this.cbScale.Size = new System.Drawing.Size(135, 23);
            this.cbScale.TabIndex = 28;
            this.cbScale.Text = "comboBoxEx1";
            // 
            // radioManual
            // 
            this.radioManual.Checked = true;
            this.radioManual.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.radioManual.Location = new System.Drawing.Point(14, 90);
            this.radioManual.Name = "radioManual";
            this.radioManual.Size = new System.Drawing.Size(161, 20);
            this.radioManual.TabIndex = 6;
            this.radioManual.Text = "Koordinaten eingeben";
            this.radioManual.CheckedChanged += new System.EventHandler(this.radios_CheckedChanged);
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(3, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 20);
            this.label5.Text = "Position";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(3, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 20);
            this.label3.Text = "Maßstab";
            // 
            // tBRW
            // 
            this.tBRW.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.tBRW.Location = new System.Drawing.Point(97, 116);
            this.tBRW.Name = "tBRW";
            this.tBRW.Size = new System.Drawing.Size(77, 19);
            this.tBRW.TabIndex = 17;
            this.tBRW.Text = "0";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.label2.Location = new System.Drawing.Point(33, 118);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 15);
            this.label2.Text = "Rechtswert:";
            // 
            // tbHW
            // 
            this.tbHW.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.tbHW.Location = new System.Drawing.Point(97, 138);
            this.tbHW.Name = "tbHW";
            this.tbHW.Size = new System.Drawing.Size(77, 19);
            this.tbHW.TabIndex = 16;
            this.tbHW.Text = "0";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.label1.Location = new System.Drawing.Point(33, 140);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 15);
            this.label1.Text = "Hochwert:";
            // 
            // relTab
            // 
            this.relTab.Controls.Add(this.lblAnchorSTatus);
            this.relTab.Controls.Add(this.anchorChooser1);
            this.relTab.Controls.Add(this.label4);
            this.relTab.Location = new System.Drawing.Point(0, 0);
            this.relTab.Name = "relTab";
            this.relTab.Size = new System.Drawing.Size(222, 164);
            this.relTab.Text = "Bezugspunkt";
            // 
            // lblAnchorSTatus
            // 
            this.lblAnchorSTatus.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.lblAnchorSTatus.Location = new System.Drawing.Point(6, 124);
            this.lblAnchorSTatus.Name = "lblAnchorSTatus";
            this.lblAnchorSTatus.Size = new System.Drawing.Size(209, 31);
            this.lblAnchorSTatus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // anchorChooser1
            // 
            this.anchorChooser1.Location = new System.Drawing.Point(64, 55);
            this.anchorChooser1.Name = "anchorChooser1";
            this.anchorChooser1.Size = new System.Drawing.Size(95, 54);
            this.anchorChooser1.TabIndex = 1;
            this.anchorChooser1.Text = "anchorChooser1";
            this.anchorChooser1.Changed += new GravurGIS.GUI.Controls.AnchorChooser.ChangedDelegate(this.anchorChooser1_Changed);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.label4.Location = new System.Drawing.Point(4, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(212, 48);
            this.label4.Text = "Hier können Sie einstellen, auf welchen Teil des Bildschrims sich Ihre Eingabe be" +
                "ziehen soll.";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Navigator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(228, 211);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.Caption);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimizeBox = false;
            this.Name = "Navigator";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseUp);
            this.tabControl1.ResumeLayout(false);
            this.navTab.ResumeLayout(false);
            this.relTab.ResumeLayout(false);
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
            this.radioManual.Checked = true;
            this.radioGPS.Checked = false;
            return base.ShowDialog();
        }

        public override void resizeToRect(Rectangle visibleRect)
        {
            this.Location = new System.Drawing.Point((visibleRect.Width - this.Width) / 2,
                (visibleRect.Height - this.Height) / 2 + visibleRect.Y);
            this.Invalidate();
        }

        /// <summary>
        /// Returns the scale x defined in the navigator dialog
        /// where x would be 5000 if one has selected "1:5000"
        /// </summary>
        public int NavigatorScale
        {
            get { return Int32.Parse(cbScale.Text.Replace(".", "")); }
        }

        /// <summary>
        /// Returns the defined position. The x-Value is the "Rechtswert" (Easting),
        /// and the y-Value is the "Hochwert" (Northing)
        /// </summary>
        public GravurGIS.Topology.PointD NavigatorPosition
        {
            get
            {
                return new GravurGIS.Topology.PointD(
                    Double.Parse(tBRW.Text),
                    Double.Parse(tbHW.Text));
            }
        }

        public GravurGIS.Styles.HorizontalAlignment HorizontalAlignment
        {
            get { return anchorChooser1.HorizontalAlginment; }
        }
        public VerticalAlignment VerticalAlignment
        {
            get { return anchorChooser1.VerticalAlignment; }
        }

        private void radios_CheckedChanged(object sender, EventArgs e)
        {
            label1.Enabled = label2.Enabled = tbHW.Enabled = tBRW.Enabled = radioManual.Checked;

            if (radioGPS.Checked)
            {
                try
                {
                    GKCoord pos = mainControler.LayerManager.CurrentGPSPositon;
                    tBRW.Text = pos.r_value.ToString();
                    tbHW.Text = pos.h_value.ToString();
                }
                catch
                {
                    MessageBox.Show("Leider werden im Moment keine aktuellen Positionsdaten empfangen.");
                    radioManual.Checked = true;
                }
            }
            else
            {
                UpdateMapPos();
            }
        }

        private void UpdateMapPos()
        {
            PointD center = mainControler.MapPanel.GetPosition(GravurGIS.Styles.HorizontalAlignment.Center,
                VerticalAlignment.Middle);
            tBRW.Text = ((int)center.x).ToString();
            tbHW.Text = ((int)center.y).ToString();
        }

        private void anchorChooser1_Changed()
        {
            lblAnchorSTatus.Text = String.Format("Die Navigations-Parameter beziehen sich im Moment auf {0}.", anchorChooser1.ToString());
        }

        protected override void toolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (sender.Equals(this.tbOkButton))
            {
                try
                {
                    PointD testPoint = NavigatorPosition;
                    int testScale = NavigatorScale;
                }
                catch
                {
                    MessageBox.Show("Bitte überprüfen Sie Ihre eingaben - es dürfen nur Zahlen und Punkte eingegeben werden.", "Fehler");
                    return;
                }
            }

            base.toolBar_ButtonClick(sender, e);
        }
    }
}
