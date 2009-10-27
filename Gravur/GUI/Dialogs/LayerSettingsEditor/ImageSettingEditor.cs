using System; 
using System.Drawing; 
using System.Collections; 
using System.ComponentModel; 
using System.Windows.Forms;
using GravurGIS.GUI.Controls;
using GravurGIS.Layers;

namespace GravurGIS.GUI.Dialogs
{
    /// <summary> 
    /// Summary description for Form2. 
    /// </summary> 
    public class ImageSettingEditor : IDialog
    {
        private System.Windows.Forms.Label Caption;
        private TabControl layerTabControl;
        private TabPage generalTab;
        private TabPage scaleTab;
        private TextBox layerNameTextBox;
        private Label label2;
        private Label layerFileNameLabel;
        private Label label5;
        private TextBox layerCommentTextBox;
        private Label label4;
        private System.Windows.Forms.Panel panel1;
        private ImageLayer currentLayer;
        private TextBox layerFilePathLabel;
        private Label label1;
        private TabPage infoTab;
        private Label layerShapeTypeLabel;
        private Label label3;
        private Label layerHeightLabel;
        private Label label13;
        private Label layerWidthLabel;
        private Label layer6;
        private Label layerMaxYLayer;
        private Label label11;
        private Label layerMaxXLabel;
        private Label label10;
        private Label layerMinYLabel;
        private Label label9;
        private Label layerMinXLabel;
        private Label label8;

        private Microsoft.WindowsCE.Forms.InputPanel inputPanel1;
        private MainControler mainControler;
        private Label resYLabel;
        private Label resXLabel;
        private Label resYField;
        private CheckBox settingsWhiteIsTP;
        private Label resXField;

        private Config config;

        /// <summary>
        /// A Dialog for editing the properties of a layer.
        /// </summary>
        public ImageSettingEditor()
        {
            // 
            // Required for Windows Form Designer support 
            // 
            InitializeComponent();
        }
        public ImageSettingEditor(Rectangle visibleRect, MainControler mainControler)
            : this()
        {
            this.config = mainControler.Config;

            this.Location = new System.Drawing.Point(0, visibleRect.Y);
            this.ClientSize = new System.Drawing.Size(visibleRect.Width, visibleRect.Height);
            this.layerTabControl.Height = this.ClientSize.Height - Caption.Height - 3;
            this.panel1.Height = layerTabControl.Height;
            this.mainControler = mainControler;
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
            this.layerTabControl = new System.Windows.Forms.TabControl();
            this.generalTab = new System.Windows.Forms.TabPage();
            this.settingsWhiteIsTP = new System.Windows.Forms.CheckBox();
            this.layerFilePathLabel = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.layerFileNameLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.layerCommentTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.layerNameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.infoTab = new System.Windows.Forms.TabPage();
            this.resYField = new System.Windows.Forms.Label();
            this.resXField = new System.Windows.Forms.Label();
            this.resXLabel = new System.Windows.Forms.Label();
            this.resYLabel = new System.Windows.Forms.Label();
            this.layerHeightLabel = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.layerWidthLabel = new System.Windows.Forms.Label();
            this.layer6 = new System.Windows.Forms.Label();
            this.layerMaxYLayer = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.layerMaxXLabel = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.layerMinYLabel = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.layerMinXLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.layerShapeTypeLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.scaleTab = new System.Windows.Forms.TabPage();
            this.Caption = new System.Windows.Forms.Label();
            this.inputPanel1 = new Microsoft.WindowsCE.Forms.InputPanel();
            this.panel1.SuspendLayout();
            this.layerTabControl.SuspendLayout();
            this.generalTab.SuspendLayout();
            this.infoTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.layerTabControl);
            this.panel1.Location = new System.Drawing.Point(1, 22);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(238, 175);
            // 
            // layerTabControl
            // 
            this.layerTabControl.Controls.Add(this.generalTab);
            this.layerTabControl.Controls.Add(this.infoTab);
            this.layerTabControl.Location = new System.Drawing.Point(0, 0);
            this.layerTabControl.Name = "layerTabControl";
            this.layerTabControl.SelectedIndex = 0;
            this.layerTabControl.Size = new System.Drawing.Size(238, 172);
            this.layerTabControl.TabIndex = 0;
            // 
            // generalTab
            // 
            this.generalTab.Controls.Add(this.settingsWhiteIsTP);
            this.generalTab.Controls.Add(this.layerFilePathLabel);
            this.generalTab.Controls.Add(this.label1);
            this.generalTab.Controls.Add(this.layerFileNameLabel);
            this.generalTab.Controls.Add(this.label5);
            this.generalTab.Controls.Add(this.layerCommentTextBox);
            this.generalTab.Controls.Add(this.label4);
            this.generalTab.Controls.Add(this.layerNameTextBox);
            this.generalTab.Controls.Add(this.label2);
            this.generalTab.Location = new System.Drawing.Point(0, 0);
            this.generalTab.Name = "generalTab";
            this.generalTab.Size = new System.Drawing.Size(238, 149);
            this.generalTab.Text = "Allgemein";
            // 
            // settingsWhiteIsTP
            // 
            this.settingsWhiteIsTP.Checked = true;
            this.settingsWhiteIsTP.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingsWhiteIsTP.Location = new System.Drawing.Point(5, 122);
            this.settingsWhiteIsTP.Name = "settingsWhiteIsTP";
            this.settingsWhiteIsTP.Size = new System.Drawing.Size(178, 20);
            this.settingsWhiteIsTP.TabIndex = 6;
            this.settingsWhiteIsTP.Text = "Farbe Weiß ist transparent";
            // 
            // layerFilePathLabel
            // 
            this.layerFilePathLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.layerFilePathLabel.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.layerFilePathLabel.Location = new System.Drawing.Point(99, 95);
            this.layerFilePathLabel.Multiline = true;
            this.layerFilePathLabel.Name = "layerFilePathLabel";
            this.layerFilePathLabel.ReadOnly = true;
            this.layerFilePathLabel.Size = new System.Drawing.Size(132, 21);
            this.layerFilePathLabel.TabIndex = 0;
            this.layerFilePathLabel.WordWrap = false;
            this.layerFilePathLabel.BackColor = SystemColors.Window;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(7, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 15);
            this.label1.Text = "Pfad:";
            // 
            // layerFileNameLabel
            // 
            this.layerFileNameLabel.Location = new System.Drawing.Point(99, 76);
            this.layerFileNameLabel.Name = "layerFileNameLabel";
            this.layerFileNameLabel.Size = new System.Drawing.Size(132, 15);
            this.layerFilePathLabel.BackColor = SystemColors.Window;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(7, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 15);
            this.label5.Text = "Dateiname:";
            // 
            // layerCommentTextBox
            // 
            this.layerCommentTextBox.Location = new System.Drawing.Point(99, 34);
            this.layerCommentTextBox.Multiline = true;
            this.layerCommentTextBox.Name = "layerCommentTextBox";
            this.layerCommentTextBox.Size = new System.Drawing.Size(132, 39);
            this.layerCommentTextBox.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(7, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 20);
            this.label4.Text = "Bemerkungen:";
            // 
            // layerNameTextBox
            // 
            this.layerNameTextBox.Location = new System.Drawing.Point(99, 7);
            this.layerNameTextBox.Name = "layerNameTextBox";
            this.layerNameTextBox.Size = new System.Drawing.Size(132, 21);
            this.layerNameTextBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(7, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 15);
            this.label2.Text = "Name:";
            // 
            // infoTab
            // 
            this.infoTab.AutoScroll = true;
            this.infoTab.Controls.Add(this.resYField);
            this.infoTab.Controls.Add(this.resXField);
            this.infoTab.Controls.Add(this.resXLabel);
            this.infoTab.Controls.Add(this.resYLabel);
            this.infoTab.Controls.Add(this.layerHeightLabel);
            this.infoTab.Controls.Add(this.label13);
            this.infoTab.Controls.Add(this.layerWidthLabel);
            this.infoTab.Controls.Add(this.layer6);
            this.infoTab.Controls.Add(this.layerMaxYLayer);
            this.infoTab.Controls.Add(this.label11);
            this.infoTab.Controls.Add(this.layerMaxXLabel);
            this.infoTab.Controls.Add(this.label10);
            this.infoTab.Controls.Add(this.layerMinYLabel);
            this.infoTab.Controls.Add(this.label9);
            this.infoTab.Controls.Add(this.layerMinXLabel);
            this.infoTab.Controls.Add(this.label8);
            this.infoTab.Controls.Add(this.layerShapeTypeLabel);
            this.infoTab.Controls.Add(this.label3);
            this.infoTab.Location = new System.Drawing.Point(0, 0);
            this.infoTab.Name = "infoTab";
            this.infoTab.Size = new System.Drawing.Size(230, 146);
            this.infoTab.Text = "Info";
            // 
            // resYField
            // 
            this.resYField.Location = new System.Drawing.Point(119, 127);
            this.resYField.Name = "resYField";
            this.resYField.Size = new System.Drawing.Size(112, 15);
            // 
            // resXField
            // 
            this.resXField.Location = new System.Drawing.Point(119, 112);
            this.resXField.Name = "resXField";
            this.resXField.Size = new System.Drawing.Size(112, 15);
            // 
            // resXLabel
            // 
            this.resXLabel.Location = new System.Drawing.Point(8, 112);
            this.resXLabel.Name = "resXLabel";
            this.resXLabel.Size = new System.Drawing.Size(112, 15);
            this.resXLabel.Text = "Bodenauflösung X:";
            // 
            // resYLabel
            // 
            this.resYLabel.Location = new System.Drawing.Point(8, 127);
            this.resYLabel.Name = "resYLabel";
            this.resYLabel.Size = new System.Drawing.Size(112, 15);
            this.resYLabel.Text = "Bodenauflösung Y:";
            // 
            // layerHeightLabel
            // 
            this.layerHeightLabel.Location = new System.Drawing.Point(119, 97);
            this.layerHeightLabel.Name = "layerHeightLabel";
            this.layerHeightLabel.Size = new System.Drawing.Size(112, 15);
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(8, 97);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(70, 15);
            this.label13.Text = "Höhe [px]:";
            // 
            // layerWidthLabel
            // 
            this.layerWidthLabel.Location = new System.Drawing.Point(119, 82);
            this.layerWidthLabel.Name = "layerWidthLabel";
            this.layerWidthLabel.Size = new System.Drawing.Size(112, 15);
            // 
            // layer6
            // 
            this.layer6.Location = new System.Drawing.Point(8, 82);
            this.layer6.Name = "layer6";
            this.layer6.Size = new System.Drawing.Size(70, 15);
            this.layer6.Text = "Breite [px]:";
            // 
            // layerMaxYLayer
            // 
            this.layerMaxYLayer.Location = new System.Drawing.Point(119, 67);
            this.layerMaxYLayer.Name = "layerMaxYLayer";
            this.layerMaxYLayer.Size = new System.Drawing.Size(112, 15);
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(8, 67);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(70, 15);
            this.label11.Text = "Max Y:";
            // 
            // layerMaxXLabel
            // 
            this.layerMaxXLabel.Location = new System.Drawing.Point(119, 52);
            this.layerMaxXLabel.Name = "layerMaxXLabel";
            this.layerMaxXLabel.Size = new System.Drawing.Size(112, 15);
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(8, 52);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(70, 15);
            this.label10.Text = "Max X:";
            // 
            // layerMinYLabel
            // 
            this.layerMinYLabel.Location = new System.Drawing.Point(119, 38);
            this.layerMinYLabel.Name = "layerMinYLabel";
            this.layerMinYLabel.Size = new System.Drawing.Size(112, 15);
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(8, 38);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 15);
            this.label9.Text = "Min Y:";
            // 
            // layerMinXLabel
            // 
            this.layerMinXLabel.Location = new System.Drawing.Point(119, 23);
            this.layerMinXLabel.Name = "layerMinXLabel";
            this.layerMinXLabel.Size = new System.Drawing.Size(112, 15);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(8, 23);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 15);
            this.label8.Text = "Min X:";
            // 
            // layerShapeTypeLabel
            // 
            this.layerShapeTypeLabel.Location = new System.Drawing.Point(119, 8);
            this.layerShapeTypeLabel.Name = "layerShapeTypeLabel";
            this.layerShapeTypeLabel.Size = new System.Drawing.Size(112, 15);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 15);
            this.label3.Text = "Layer-Typ:";
            // 
            // scaleTab
            // 
            this.scaleTab.Location = new System.Drawing.Point(0, 0);
            this.scaleTab.Name = "scaleTab";
            this.scaleTab.Size = new System.Drawing.Size(230, 146);
            this.scaleTab.Text = "Maßstab";
            // 
            // Caption
            // 
            this.Caption.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular);
            this.Caption.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Caption.Location = new System.Drawing.Point(4, 3);
            this.Caption.Name = "Caption";
            this.Caption.Size = new System.Drawing.Size(172, 18);
            this.Caption.Text = "Layereinstellungen bearbeiten";
            // 
            // ImageSettingEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(240, 200);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Caption);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(0, 15);
            this.MinimizeBox = false;
            this.Name = "ImageSettingEditor";
            this.TopMost = true;
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseDown);
            this.panel1.ResumeLayout(false);
            this.layerTabControl.ResumeLayout(false);
            this.generalTab.ResumeLayout(false);
            this.infoTab.ResumeLayout(false);
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

        public DialogResult ShowDialog(ImageLayer currentLayer)
        {
            this.currentLayer = currentLayer;
            fillDialogWithData();
            return base.ShowDialog();
        }

        private void fillDialogWithData()
        {

            if (currentLayer != null)
            {
                this.settingsWhiteIsTP.Checked = currentLayer.WhiteIsTransparent;
                this.layerNameTextBox.Text = currentLayer.LayerName;
                this.layerCommentTextBox.Text = currentLayer.Comment;
                this.layerFileNameLabel.Text = currentLayer.FileName;
                this.layerFilePathLabel.Text = currentLayer.FilePath;
                this.layerShapeTypeLabel.Text = "Bild";
                this.layerMinXLabel.Text = currentLayer.BoundingBox.Left.ToString();
                this.layerMinYLabel.Text = currentLayer.BoundingBox.Bottom.ToString();
                this.layerMaxXLabel.Text = currentLayer.BoundingBox.Right.ToString();
                this.layerMaxYLayer.Text = currentLayer.BoundingBox.Top.ToString();
                this.layerWidthLabel.Text = currentLayer.Width.ToString();
                this.layerHeightLabel.Text = currentLayer.Height.ToString();
                this.resXField.Text = currentLayer.ResolutionX.ToString();
                this.resYField.Text = currentLayer.ResolutionY.ToString();
            }
            else
            {
                MessageBox.Show("Fehler beim Lesen von Shape");
                mainControler.addLogMessage("[LayerEdit] Fehler beim Lesen von Bild");
                currentLayer = null;
            }
        }

        protected override void toolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button.Equals(tbOkButton))
                if (currentLayer != null)
                {
                    currentLayer.LayerName = this.layerNameTextBox.Text;
                    currentLayer.Comment = this.layerCommentTextBox.Text;
                    currentLayer.WhiteIsTransparent = settingsWhiteIsTP.Checked;
                }

            base.toolBar_ButtonClick(sender, e);
        }

        public override void resizeToRect(Rectangle visibleRect)
        {
            this.Location = new System.Drawing.Point(0, visibleRect.Y);
            this.ClientSize = new System.Drawing.Size(visibleRect.Width, visibleRect.Height);
            this.layerTabControl.Height = this.ClientSize.Height - Caption.Height - 3;
            this.panel1.Height = layerTabControl.Height;
            this.Invalidate();
        }
    }
}
