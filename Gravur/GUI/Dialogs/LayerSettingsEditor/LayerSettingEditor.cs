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
    public class LayerSettingEditor : IDialog
    {
        private System.Windows.Forms.Label Caption;
        private TabControl layerTabControl;
        private TabPage generalTab;
        private TabPage viewTab;
        private TabPage scaleTab;
        private Label layerLineWidthLabel;
        private Label lineColorLabel;
        private NumericUpDown layerLineWidth;
        private ColorIcon layerLineColorPanel;
        private TextBox layerNameTextBox;
        private Label label2;
        private ComboBox layerDashTypeCombo;
        private Label layerDashTypeLabel;
        private Label layerFileNameLabel;
        private Label label5;
        private TextBox layerCommentTextBox;
        private Label label4;
        private ColorIcon layerFillColorPanel;
        private System.Windows.Forms.Panel panel1;
        private ColorIcon layerPointColorPanel;
        private Label pointColorLabel;
        private ShapeObject currentShape;
        private TextBox layerFilePathLabel;
        private Label label1;
        private TabPage infoTab;
        private Label layerShapeCountLabel;
        private Label label7;
        private Label layerShapeTypeLabel;
        private Label label3;
        private Label layerScaleLabel;
        private Label label14;
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
        //TODO: rewrite as enumeration: layerType {shObject, pTrans, plineTrans, polygonTrans}
        private LayerType layerType;
        private TransportPolylineLayer tLine;
        private TransportMultiPointLayer tPoint;
        private CheckBox checkDrawComment;
        private TransportPolygonLayer tPolygon;
        private Button clearLayerButton;
        private Label lblMaxCommentSize;
        private NumericUpDown numCommentMaxSize;

        // singelton reference;
        private static LayerSettingEditor reference;
        private CheckBox fillColorLabel;
        private IContainer components;
		private NumericUpDown layerPointSize;
		private Label lblPointsize;

        private Config config;

        //private bool infoTab;
        /// <summary>
        /// A Dialog for editing the properties of a layer.
        /// </summary>
        public LayerSettingEditor()
        {
            // 
            // Required for Windows Form Designer support 
            // 
            InitializeComponent();
        }

        public static LayerSettingEditor Instance(Rectangle visibleRect, MainControler mainControler)
        {
            if (reference == null) reference = new LayerSettingEditor(visibleRect, mainControler);

            return reference;
        }

        private LayerSettingEditor(Rectangle visibleRect, MainControler mainControler) : this()
        {
            this.config = mainControler.Config;

            this.Location = new System.Drawing.Point(0, visibleRect.Y);
            this.ClientSize = new System.Drawing.Size(visibleRect.Width, visibleRect.Height);
            this.layerTabControl.Height = this.ClientSize.Height - Caption.Height - 3;
            this.panel1.Height = layerTabControl.Height;
            this.mainControler = mainControler;
            this.tLine = mainControler.LayerManager.TransportPolylineLayer;
            this.tPoint = mainControler.LayerManager.TransportPointLayer;
            this.tPolygon = mainControler.LayerManager.TransportPolygonLayer;
            this.config = mainControler.Config; 
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
			this.components = new System.ComponentModel.Container();
			this.panel1 = new System.Windows.Forms.Panel();
			this.layerTabControl = new System.Windows.Forms.TabControl();
			this.generalTab = new System.Windows.Forms.TabPage();
			this.clearLayerButton = new System.Windows.Forms.Button();
			this.layerFilePathLabel = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.layerFileNameLabel = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.layerCommentTextBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.layerNameTextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.viewTab = new System.Windows.Forms.TabPage();
			this.layerFillColorPanel = new GravurGIS.GUI.Controls.ColorIcon();
			this.fillColorLabel = new System.Windows.Forms.CheckBox();
			this.numCommentMaxSize = new System.Windows.Forms.NumericUpDown();
			this.lblMaxCommentSize = new System.Windows.Forms.Label();
			this.checkDrawComment = new System.Windows.Forms.CheckBox();
			this.layerLineColorPanel = new GravurGIS.GUI.Controls.ColorIcon();
			this.layerPointColorPanel = new GravurGIS.GUI.Controls.ColorIcon();
			this.pointColorLabel = new System.Windows.Forms.Label();
			this.layerDashTypeCombo = new System.Windows.Forms.ComboBox();
			this.layerDashTypeLabel = new System.Windows.Forms.Label();
			this.layerLineWidth = new System.Windows.Forms.NumericUpDown();
			this.layerLineWidthLabel = new System.Windows.Forms.Label();
			this.lineColorLabel = new System.Windows.Forms.Label();
			this.infoTab = new System.Windows.Forms.TabPage();
			this.layerScaleLabel = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
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
			this.layerShapeCountLabel = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.layerShapeTypeLabel = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.scaleTab = new System.Windows.Forms.TabPage();
			this.Caption = new System.Windows.Forms.Label();
			this.inputPanel1 = new Microsoft.WindowsCE.Forms.InputPanel(this.components);
			this.layerPointSize = new System.Windows.Forms.NumericUpDown();
			this.lblPointsize = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.layerTabControl.SuspendLayout();
			this.generalTab.SuspendLayout();
			this.viewTab.SuspendLayout();
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
			this.layerTabControl.Controls.Add(this.viewTab);
			this.layerTabControl.Controls.Add(this.infoTab);
			this.layerTabControl.Location = new System.Drawing.Point(0, 0);
			this.layerTabControl.Name = "layerTabControl";
			this.layerTabControl.SelectedIndex = 0;
			this.layerTabControl.Size = new System.Drawing.Size(238, 172);
			this.layerTabControl.TabIndex = 0;
			// 
			// generalTab
			// 
			this.generalTab.Controls.Add(this.clearLayerButton);
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
			// clearLayerButton
			// 
			this.clearLayerButton.Location = new System.Drawing.Point(132, 117);
			this.clearLayerButton.Name = "clearLayerButton";
			this.clearLayerButton.Size = new System.Drawing.Size(99, 19);
			this.clearLayerButton.TabIndex = 6;
			this.clearLayerButton.Text = "Layer leeren";
			this.clearLayerButton.Click += new System.EventHandler(this.clearLayerButton_Click);
			// 
			// layerFilePathLabel
			// 
			this.layerFilePathLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.layerFilePathLabel.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.layerFilePathLabel.Location = new System.Drawing.Point(99, 108);
			this.layerFilePathLabel.Multiline = true;
			this.layerFilePathLabel.Name = "layerFilePathLabel";
			this.layerFilePathLabel.ReadOnly = true;
			this.layerFilePathLabel.Size = new System.Drawing.Size(132, 15);
			this.layerFilePathLabel.TabIndex = 0;
			this.layerFilePathLabel.WordWrap = false;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(7, 108);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(68, 15);
			this.label1.Text = "Pfad:";
			// 
			// layerFileNameLabel
			// 
			this.layerFileNameLabel.Location = new System.Drawing.Point(99, 89);
			this.layerFileNameLabel.Name = "layerFileNameLabel";
			this.layerFileNameLabel.Size = new System.Drawing.Size(132, 15);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(7, 89);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(68, 15);
			this.label5.Text = "Dateiname:";
			// 
			// layerCommentTextBox
			// 
			this.layerCommentTextBox.Location = new System.Drawing.Point(99, 34);
			this.layerCommentTextBox.Multiline = true;
			this.layerCommentTextBox.Name = "layerCommentTextBox";
			this.layerCommentTextBox.Size = new System.Drawing.Size(132, 52);
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
			// viewTab
			// 
			this.viewTab.AutoScroll = true;
			this.viewTab.Controls.Add(this.layerPointSize);
			this.viewTab.Controls.Add(this.lblPointsize);
			this.viewTab.Controls.Add(this.layerFillColorPanel);
			this.viewTab.Controls.Add(this.fillColorLabel);
			this.viewTab.Controls.Add(this.numCommentMaxSize);
			this.viewTab.Controls.Add(this.lblMaxCommentSize);
			this.viewTab.Controls.Add(this.checkDrawComment);
			this.viewTab.Controls.Add(this.layerLineColorPanel);
			this.viewTab.Controls.Add(this.layerPointColorPanel);
			this.viewTab.Controls.Add(this.pointColorLabel);
			this.viewTab.Controls.Add(this.layerDashTypeCombo);
			this.viewTab.Controls.Add(this.layerDashTypeLabel);
			this.viewTab.Controls.Add(this.layerLineWidth);
			this.viewTab.Controls.Add(this.layerLineWidthLabel);
			this.viewTab.Controls.Add(this.lineColorLabel);
			this.viewTab.Location = new System.Drawing.Point(0, 0);
			this.viewTab.Name = "viewTab";
			this.viewTab.Size = new System.Drawing.Size(238, 149);
			this.viewTab.Text = "Aussehen";
			// 
			// layerFillColorPanel
			// 
			this.layerFillColorPanel.BackColor = System.Drawing.Color.Blue;
			this.layerFillColorPanel.Location = new System.Drawing.Point(95, 53);
			this.layerFillColorPanel.Name = "layerFillColorPanel";
			this.layerFillColorPanel.Size = new System.Drawing.Size(15, 15);
			this.layerFillColorPanel.TabIndex = 0;
			this.layerFillColorPanel.Click += new System.EventHandler(this.layerColorPanel_Click);
			// 
			// fillColorLabel
			// 
			this.fillColorLabel.Location = new System.Drawing.Point(8, 50);
			this.fillColorLabel.Name = "fillColorLabel";
			this.fillColorLabel.Size = new System.Drawing.Size(100, 20);
			this.fillColorLabel.TabIndex = 23;
			this.fillColorLabel.Text = "Füllfarbe:";
			// 
			// numCommentMaxSize
			// 
			this.numCommentMaxSize.Location = new System.Drawing.Point(144, 176);
			this.numCommentMaxSize.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
			this.numCommentMaxSize.Name = "numCommentMaxSize";
			this.numCommentMaxSize.Size = new System.Drawing.Size(51, 22);
			this.numCommentMaxSize.TabIndex = 16;
			this.numCommentMaxSize.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// lblMaxCommentSize
			// 
			this.lblMaxCommentSize.Location = new System.Drawing.Point(34, 179);
			this.lblMaxCommentSize.Name = "lblMaxCommentSize";
			this.lblMaxCommentSize.Size = new System.Drawing.Size(106, 20);
			this.lblMaxCommentSize.Text = "Maximale Zeichen:";
			// 
			// checkDrawComment
			// 
			this.checkDrawComment.Checked = true;
			this.checkDrawComment.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkDrawComment.Location = new System.Drawing.Point(10, 153);
			this.checkDrawComment.Name = "checkDrawComment";
			this.checkDrawComment.Size = new System.Drawing.Size(157, 20);
			this.checkDrawComment.TabIndex = 10;
			this.checkDrawComment.Text = "Kommentare Zeichnen";
			this.checkDrawComment.CheckStateChanged += new System.EventHandler(this.checkDrawComment_CheckStateChanged);
			// 
			// layerLineColorPanel
			// 
			this.layerLineColorPanel.BackColor = System.Drawing.Color.Blue;
			this.layerLineColorPanel.Location = new System.Drawing.Point(95, 32);
			this.layerLineColorPanel.Name = "layerLineColorPanel";
			this.layerLineColorPanel.Size = new System.Drawing.Size(15, 15);
			this.layerLineColorPanel.TabIndex = 1;
			this.layerLineColorPanel.Click += new System.EventHandler(this.layerColorPanel_Click);
			// 
			// layerPointColorPanel
			// 
			this.layerPointColorPanel.BackColor = System.Drawing.Color.Blue;
			this.layerPointColorPanel.Location = new System.Drawing.Point(95, 12);
			this.layerPointColorPanel.Name = "layerPointColorPanel";
			this.layerPointColorPanel.Size = new System.Drawing.Size(15, 15);
			this.layerPointColorPanel.TabIndex = 2;
			this.layerPointColorPanel.Click += new System.EventHandler(this.layerColorPanel_Click);
			// 
			// pointColorLabel
			// 
			this.pointColorLabel.Location = new System.Drawing.Point(10, 12);
			this.pointColorLabel.Name = "pointColorLabel";
			this.pointColorLabel.Size = new System.Drawing.Size(80, 15);
			this.pointColorLabel.Text = "Punktfarbe:";
			// 
			// layerDashTypeCombo
			// 
			this.layerDashTypeCombo.Enabled = false;
			this.layerDashTypeCombo.Location = new System.Drawing.Point(95, 128);
			this.layerDashTypeCombo.Name = "layerDashTypeCombo";
			this.layerDashTypeCombo.Size = new System.Drawing.Size(100, 22);
			this.layerDashTypeCombo.TabIndex = 6;
			this.layerDashTypeCombo.Visible = false;
			// 
			// layerDashTypeLabel
			// 
			this.layerDashTypeLabel.Enabled = false;
			this.layerDashTypeLabel.Location = new System.Drawing.Point(10, 130);
			this.layerDashTypeLabel.Name = "layerDashTypeLabel";
			this.layerDashTypeLabel.Size = new System.Drawing.Size(65, 20);
			this.layerDashTypeLabel.Text = "Linientyp:";
			this.layerDashTypeLabel.Visible = false;
			// 
			// layerLineWidth
			// 
			this.layerLineWidth.Location = new System.Drawing.Point(95, 100);
			this.layerLineWidth.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this.layerLineWidth.Name = "layerLineWidth";
			this.layerLineWidth.Size = new System.Drawing.Size(100, 22);
			this.layerLineWidth.TabIndex = 4;
			this.layerLineWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// layerLineWidthLabel
			// 
			this.layerLineWidthLabel.Location = new System.Drawing.Point(10, 105);
			this.layerLineWidthLabel.Name = "layerLineWidthLabel";
			this.layerLineWidthLabel.Size = new System.Drawing.Size(100, 17);
			this.layerLineWidthLabel.Text = "Linienstärke:";
			// 
			// lineColorLabel
			// 
			this.lineColorLabel.Location = new System.Drawing.Point(10, 31);
			this.lineColorLabel.Name = "lineColorLabel";
			this.lineColorLabel.Size = new System.Drawing.Size(80, 15);
			this.lineColorLabel.Text = "Linienfarbe:";
			// 
			// infoTab
			// 
			this.infoTab.AutoScroll = true;
			this.infoTab.Controls.Add(this.layerScaleLabel);
			this.infoTab.Controls.Add(this.label14);
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
			this.infoTab.Controls.Add(this.layerShapeCountLabel);
			this.infoTab.Controls.Add(this.label7);
			this.infoTab.Controls.Add(this.layerShapeTypeLabel);
			this.infoTab.Controls.Add(this.label3);
			this.infoTab.Location = new System.Drawing.Point(0, 0);
			this.infoTab.Name = "infoTab";
			this.infoTab.Size = new System.Drawing.Size(230, 146);
			this.infoTab.Text = "Info";
			// 
			// layerScaleLabel
			// 
			this.layerScaleLabel.Location = new System.Drawing.Point(94, 127);
			this.layerScaleLabel.Name = "layerScaleLabel";
			this.layerScaleLabel.Size = new System.Drawing.Size(137, 15);
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(8, 127);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(94, 15);
			this.label14.Text = "Skalierung:";
			// 
			// layerHeightLabel
			// 
			this.layerHeightLabel.Location = new System.Drawing.Point(94, 112);
			this.layerHeightLabel.Name = "layerHeightLabel";
			this.layerHeightLabel.Size = new System.Drawing.Size(137, 15);
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(8, 112);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(70, 15);
			this.label13.Text = "Höhe:";
			// 
			// layerWidthLabel
			// 
			this.layerWidthLabel.Location = new System.Drawing.Point(94, 97);
			this.layerWidthLabel.Name = "layerWidthLabel";
			this.layerWidthLabel.Size = new System.Drawing.Size(137, 15);
			// 
			// layer6
			// 
			this.layer6.Location = new System.Drawing.Point(8, 97);
			this.layer6.Name = "layer6";
			this.layer6.Size = new System.Drawing.Size(70, 15);
			this.layer6.Text = "Breite:";
			// 
			// layerMaxYLayer
			// 
			this.layerMaxYLayer.Location = new System.Drawing.Point(94, 82);
			this.layerMaxYLayer.Name = "layerMaxYLayer";
			this.layerMaxYLayer.Size = new System.Drawing.Size(137, 15);
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(8, 82);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(70, 15);
			this.label11.Text = "Max Y:";
			// 
			// layerMaxXLabel
			// 
			this.layerMaxXLabel.Location = new System.Drawing.Point(94, 67);
			this.layerMaxXLabel.Name = "layerMaxXLabel";
			this.layerMaxXLabel.Size = new System.Drawing.Size(137, 15);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(8, 67);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(70, 15);
			this.label10.Text = "Max X:";
			// 
			// layerMinYLabel
			// 
			this.layerMinYLabel.Location = new System.Drawing.Point(94, 53);
			this.layerMinYLabel.Name = "layerMinYLabel";
			this.layerMinYLabel.Size = new System.Drawing.Size(137, 15);
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(8, 53);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(70, 15);
			this.label9.Text = "Min Y:";
			// 
			// layerMinXLabel
			// 
			this.layerMinXLabel.Location = new System.Drawing.Point(94, 38);
			this.layerMinXLabel.Name = "layerMinXLabel";
			this.layerMinXLabel.Size = new System.Drawing.Size(137, 15);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(8, 38);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(53, 15);
			this.label8.Text = "Min X:";
			// 
			// layerShapeCountLabel
			// 
			this.layerShapeCountLabel.Location = new System.Drawing.Point(94, 23);
			this.layerShapeCountLabel.Name = "layerShapeCountLabel";
			this.layerShapeCountLabel.Size = new System.Drawing.Size(137, 15);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(8, 23);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(70, 15);
			this.label7.Text = "Einträge:";
			// 
			// layerShapeTypeLabel
			// 
			this.layerShapeTypeLabel.Location = new System.Drawing.Point(94, 8);
			this.layerShapeTypeLabel.Name = "layerShapeTypeLabel";
			this.layerShapeTypeLabel.Size = new System.Drawing.Size(137, 15);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 8);
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
			// layerPointSize
			// 
			this.layerPointSize.Location = new System.Drawing.Point(95, 74);
			this.layerPointSize.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this.layerPointSize.Name = "layerPointSize";
			this.layerPointSize.Size = new System.Drawing.Size(100, 22);
			this.layerPointSize.TabIndex = 30;
			this.layerPointSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// lblPointsize
			// 
			this.lblPointsize.Location = new System.Drawing.Point(10, 79);
			this.lblPointsize.Name = "lblPointsize";
			this.lblPointsize.Size = new System.Drawing.Size(100, 17);
			this.lblPointsize.Text = "Punktgröße:";
			// 
			// LayerSettingEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.ClientSize = new System.Drawing.Size(240, 200);
			this.ControlBox = false;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.Caption);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Location = new System.Drawing.Point(0, 15);
			this.MinimizeBox = false;
			this.Name = "LayerSettingEditor";
			this.TopMost = true;
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseDown);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseUp);
			this.panel1.ResumeLayout(false);
			this.layerTabControl.ResumeLayout(false);
			this.generalTab.ResumeLayout(false);
			this.viewTab.ResumeLayout(false);
			this.infoTab.ResumeLayout(false);
			this.ResumeLayout(false);

        }

#endregion

        void layerColorPanel_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog(this.Handle);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                if (sender.Equals(layerPointColorPanel))
                    layerPointColorPanel.BackColor = colorDialog.Color;
                else if (sender.Equals(layerLineColorPanel))
                    layerLineColorPanel.BackColor = colorDialog.Color;
                else if (sender.Equals(layerFillColorPanel))
                    layerFillColorPanel.BackColor = colorDialog.Color;
            }
            colorDialog.Dispose();
        }

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
        public DialogResult ShowDialog(ShapeObject currentShape)
        {
            this.layerType = 0;
            this.currentShape = currentShape;
            fillDialogWithData();
            return base.ShowDialog();
        }
        
        public DialogResult ShowDialog(LayerType transLayerType)
        {
            this.layerType = transLayerType;
            this.currentShape = null;
            fillDialogWithData();
            return base.ShowDialog();
        }
        

        private void fillDialogWithData()
        {
            switch (layerType)
            {
                case LayerType.Shape:
                    {
                        if (currentShape != null)
                        {
                            this.label5.Enabled = true;
                            this.label1.Enabled = true;
                            this.layerFileNameLabel.Enabled = true;
                            this.layerFilePathLabel.Enabled = true;
                            this.checkDrawComment.Visible = false;
                            this.numCommentMaxSize.Visible = false;
                            this.label1.Visible = true;
                            this.layerFilePathLabel.Visible = true;
                            this.clearLayerButton.Visible = false;
                            this.lblMaxCommentSize.Visible = false;

                            LayerInfo tempLayerInfo = currentShape.LayerInfo;
                            VectorInfo tempVectorInfo = currentShape.VectorInfo;

                            this.layerNameTextBox.Text = currentShape.LayerName;
                            this.layerCommentTextBox.Text = currentShape.Comment;
                            this.layerFileNameLabel.Text = tempLayerInfo.FileName;
                            this.layerFilePathLabel.Text = tempLayerInfo.FilePath;
                            this.layerShapeTypeLabel.Text = currentShape.ShapeType.ToString();
                            this.layerShapeCountLabel.Text = currentShape.NumberOfShapes.ToString();
                            this.layerMinXLabel.Text = currentShape.BoundingBox.Left.ToString();
                            this.layerMinYLabel.Text = currentShape.BoundingBox.Bottom.ToString();
                            this.layerMaxXLabel.Text = currentShape.BoundingBox.Right.ToString();
                            this.layerMaxYLayer.Text = currentShape.BoundingBox.Top.ToString();
                            this.layerWidthLabel.Text = currentShape.Width.ToString();
                            this.layerHeightLabel.Text = currentShape.Height.ToString();
                            this.layerScaleLabel.Text = mainControler.LayerManager.Scale.ToString();
                            //this.layerTabControl.Controls.Add(this.scaleTab);
                            this.layerTabControl.Controls.Add(this.infoTab);
                            
                            if (currentShape.ShapeType == MapTools.ShapeLib.ShapeType.Point)
                            {
                                layerLineColorPanel.Enabled = false;
                                lineColorLabel.Enabled = false;
                                layerLineColorPanel.BackColor = Color.LightGray;
                                layerFillColorPanel.Enabled = false;
                                fillColorLabel.Enabled = false;
                                layerFillColorPanel.BackColor = Color.LightGray;
                                layerPointColorPanel.BackColor = tempVectorInfo.PointColor;
                                layerLineWidth.Enabled = false;
                                layerLineWidthLabel.Enabled = false;
                                layerDashTypeCombo.Enabled = false;
                                layerDashTypeLabel.Enabled = false;
                            } 
                            if (currentShape.ShapeType == MapTools.ShapeLib.ShapeType.PolyLine)
                            {
                                layerLineColorPanel.Enabled = true;
                                lineColorLabel.Enabled = true;
                                layerFillColorPanel.Enabled = false;
                                fillColorLabel.Enabled = false;
                                layerFillColorPanel.BackColor = Color.LightGray;
                                layerPointColorPanel.BackColor = tempVectorInfo.PointColor;
                                layerLineColorPanel.BackColor = tempVectorInfo.LineColor;
                                layerLineWidth.Enabled = true;
                                layerLineWidthLabel.Enabled = true;
                                layerLineWidth.Value = Convert.ToDecimal(tempVectorInfo.LayerPen.Width);
                                layerDashTypeCombo.Enabled = true;
                                layerDashTypeLabel.Enabled = true;
                            }
                            else if (currentShape.ShapeType == MapTools.ShapeLib.ShapeType.Polygon)
                            {
                                layerLineColorPanel.Enabled = true;
                                lineColorLabel.Enabled = true;
                                layerFillColorPanel.Enabled = true;
                                fillColorLabel.Enabled = true;
                                fillColorLabel.Checked = tempVectorInfo.Fill;
                                layerPointColorPanel.BackColor = tempVectorInfo.PointColor;
                                layerLineColorPanel.BackColor = tempVectorInfo.LineColor;
                                layerFillColorPanel.BackColor = tempVectorInfo.FillColor;
                                layerLineWidth.Enabled = true;
                                layerLineWidthLabel.Enabled = true;
                                layerLineWidth.Value = Convert.ToDecimal(tempVectorInfo.LayerPen.Width);
                                layerDashTypeCombo.Enabled = true;
                                layerDashTypeLabel.Enabled = true;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Fehler beim Lesen von Shape");
                            mainControler.addLogMessage("[LayerEdit] Fehler beim Lesen von Shape");
                            currentShape = null;
                        }
                        break;
                    }
                case LayerType.PointCanvas: //layerType == plineTransport
                    {
                        this.layerNameTextBox.Text = config.ExPntLayerName;
                        this.layerCommentTextBox.Text = config.ExPntLayerComment;
                        this.fillColorLabel.Enabled = false;
                        this.layerFillColorPanel.Enabled = false;
                        this.layerFillColorPanel.BackColor = Color.LightGray;
                        this.lineColorLabel.Enabled = false;
                        this.layerLineColorPanel.Enabled = false;
                        this.layerLineColorPanel.BackColor = Color.LightGray;
                        this.layerPointColorPanel.Enabled = true;
                        this.pointColorLabel.Enabled = true;
                        this.layerPointColorPanel.BackColor = config.ExPntLayerPointColor;
						this.layerLineWidth.Visible = false;
						this.layerLineWidthLabel.Visible = false;
						this.lblPointsize.Visible = true;
						this.layerPointSize.Visible = true;
						this.layerPointSize.Value = config.ExPntLayerPointWidth;
                        
						this.checkDrawComment.Checked = config.ExPntLayerDisplayComments;
                        this.numCommentMaxSize.Value = config.ExPntLayerDisplayCommentMaxLength;
                        this.label1.Visible = false;
                        this.layerFilePathLabel.Visible = false;
                        this.clearLayerButton.Visible = true;

                        this.layerShapeTypeLabel.Text = "Punkt Austauschlayer";
                        this.layerShapeCountLabel.Text = tPoint.Count.ToString();
                        this.layerMinXLabel.Text = tPoint.BoundingBox.Left.ToString();
                        this.layerMinYLabel.Text = tPoint.BoundingBox.Bottom.ToString();
                        this.layerMaxXLabel.Text = tPoint.BoundingBox.Right.ToString();
                        this.layerMaxYLayer.Text = tPoint.BoundingBox.Top.ToString();
                        this.layerWidthLabel.Text = tPoint.Width.ToString();
                        this.layerHeightLabel.Text = tPoint.Height.ToString();
                        this.layerScaleLabel.Text = mainControler.LayerManager.Scale.ToString();

                        break;
                    }
                case LayerType.PolygonCanvas: //layerType == plineTransport
                    {
                        this.layerNameTextBox.Text = config.ExPGonLayerName;
                        this.layerCommentTextBox.Text = config.ExPGonLayerComment;
                        this.fillColorLabel.Enabled = true;
                        this.fillColorLabel.Checked = config.ExPGonLayerFill;
                        this.layerFillColorPanel.Enabled = true;
                        this.layerFillColorPanel.BackColor = config.ExPGonLayerFillColor;
                        this.lineColorLabel.Enabled = true;
                        this.layerLineColorPanel.Enabled = true;
                        this.layerLineColorPanel.BackColor = config.ExPGonLayerLineColor;
                        this.layerPointColorPanel.Enabled = true;
                        this.pointColorLabel.Enabled = true;
                        this.layerPointColorPanel.BackColor = config.ExPGonLayerPointColor;
                        this.layerLineWidth.Value = Math.Round((decimal)config.exPGonLayerLineWidth);
						this.layerPointSize.Value = Math.Round((decimal)config.ExPGonLayerPointWidth);
						this.layerLineWidth.Visible = true;
						this.layerLineWidthLabel.Visible = true;

                        this.checkDrawComment.Checked = config.ExPGonLayerDisplayComments;
                        this.label1.Visible = false;
                        this.layerFilePathLabel.Visible = false;
                        this.clearLayerButton.Visible = true;
                        this.numCommentMaxSize.Value = config.ExPGonLayerDisplayCommentMaxLength;

                        this.layerShapeTypeLabel.Text = "Polygon Austauschlayer";
                        this.layerShapeCountLabel.Text = tPolygon.Count.ToString();
                        this.layerMinXLabel.Text = tPolygon.BoundingBox.Left.ToString();
                        this.layerMinYLabel.Text = tPolygon.BoundingBox.Bottom.ToString();
                        this.layerMaxXLabel.Text = tPolygon.BoundingBox.Right.ToString();
                        this.layerMaxYLayer.Text = tPolygon.BoundingBox.Top.ToString();
                        this.layerWidthLabel.Text = tPolygon.Width.ToString();
                        this.layerHeightLabel.Text = tPolygon.Height.ToString();
                        this.layerScaleLabel.Text = mainControler.LayerManager.Scale.ToString();

                        break;
                    }
                case LayerType.PolylineCanvas:
                    {
                        this.layerNameTextBox.Text = config.ExPLineLayerName;
                        this.layerCommentTextBox.Text = config.ExPLineLayerComment;
                        this.fillColorLabel.Enabled = false;
                        this.layerFillColorPanel.Enabled = false;
                        this.layerFillColorPanel.BackColor = Color.LightGray;
                        this.lineColorLabel.Enabled = true;
                        this.layerLineColorPanel.Enabled = true;
                        this.layerLineColorPanel.BackColor = config.ExPLineLayerLineColor;
                        this.layerPointColorPanel.Enabled = true;
                        this.pointColorLabel.Enabled = true;
                        this.layerPointColorPanel.BackColor = config.ExPLineLayerPointColor;
                        this.layerLineWidth.Value = Math.Round((decimal)config.exPLineLayerLineWidth);
						this.layerPointSize.Value = Math.Round((decimal)config.ExPLineLayerPointWidth);
						this.layerLineWidth.Visible = true;
						this.layerLineWidthLabel.Visible = true;

                        this.checkDrawComment.Checked = config.ExPLineLayerDisplayComments;
                        this.label1.Visible = false;
                        this.layerFilePathLabel.Visible = false;
                        this.clearLayerButton.Visible = true;
                        this.numCommentMaxSize.Value = config.ExPLineLayerDisplayCommentMaxLength;

                        this.layerShapeTypeLabel.Text = "Linienzug Austauschlayer";
                        this.layerShapeCountLabel.Text = tLine.Count.ToString();
                        this.layerMinXLabel.Text = tLine.BoundingBox.Left.ToString();
                        this.layerMinYLabel.Text = tLine.BoundingBox.Bottom.ToString();
                        this.layerMaxXLabel.Text = tLine.BoundingBox.Right.ToString();
                        this.layerMaxYLayer.Text = tLine.BoundingBox.Top.ToString();
                        this.layerWidthLabel.Text = tLine.Width.ToString();
                        this.layerHeightLabel.Text = tLine.Height.ToString();
                        this.layerScaleLabel.Text = mainControler.LayerManager.Scale.ToString();
                        break;
                    }
                default: break;

            }
            if (layerType != LayerType.Shape)
            {
                this.label5.Enabled = false;
                this.label1.Enabled = false;
                this.layerFileNameLabel.Visible = false;
                this.layerFilePathLabel.Visible = false;
                this.checkDrawComment.Visible = true;
                this.numCommentMaxSize.Visible = true;
                this.lblMaxCommentSize.Visible = true;

                if (checkDrawComment.Checked)
                {
                    this.lblMaxCommentSize.Enabled = true;
                    this.numCommentMaxSize.Enabled = true;
                }
                else
                {
                    this.lblMaxCommentSize.Enabled = false;
                    this.numCommentMaxSize.Enabled = false;
                }
            }
        }

        protected override void toolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if(e.Button.Equals(tbOkButton))
            switch (layerType)
            {
                case LayerType.Shape:
                    if (currentShape != null)
                    {
                        VectorInfo tempVectorInfo       = currentShape.VectorInfo;
                        currentShape.LayerName          = this.layerNameTextBox.Text;
                        currentShape.Comment            = this.layerCommentTextBox.Text;
                        tempVectorInfo.PointColor       = this.layerPointColorPanel.BackColor;
                        tempVectorInfo.LineColor        = this.layerLineColorPanel.BackColor;
                        tempVectorInfo.FillColor        = this.layerFillColorPanel.BackColor;
                        tempVectorInfo.Fill             = this.fillColorLabel.Checked;
                        tempVectorInfo.LayerPen.Width   = (float)this.layerLineWidth.Value;
                        currentShape.Changed            = true;
                        // TODO: [12] Dash-Type?
                    }
                    else sender = tbCancelButton;
                    break;
                case LayerType.PointCanvas:
                    config.ExPntLayerName                   = this.layerNameTextBox.Text;
                    config.ExPntLayerComment                = this.layerCommentTextBox.Text;
                    config.ExPntLayerPointColor             = this.layerPointColorPanel.BackColor;
                    config.ExPntLayerPointWidth             = (int)this.layerPointSize.Value;
                    config.ExPntLayerDisplayComments        = this.checkDrawComment.Checked;
                    config.ExPntLayerDisplayCommentMaxLength= (int)this.numCommentMaxSize.Value;
                    break;
                case LayerType.PolygonCanvas:
                    config.ExPGonLayerName                  = this.layerNameTextBox.Text;
                    config.ExPGonLayerComment               = this.layerCommentTextBox.Text;
                    config.ExPGonLayerLineColor             = this.layerLineColorPanel.BackColor;
                    config.ExPGonLayerFillColor             = this.layerFillColorPanel.BackColor;
                    config.ExPGonLayerPointColor            = this.layerPointColorPanel.BackColor;
                    config.ExPGonLayerPointWidth            = (int)this.layerPointSize.Value;
					config.exPGonLayerLineWidth				= (int)this.layerLineWidth.Value;
                    config.ExPGonLayerDisplayComments       = this.checkDrawComment.Checked;
                    config.ExPGonLayerDisplayCommentMaxLength = (int)this.numCommentMaxSize.Value;
                    config.ExPGonLayerFill                  = this.fillColorLabel.Checked;
                    break;
                case LayerType.PolylineCanvas:
                    config.ExPLineLayerName                 = this.layerNameTextBox.Text;
                    config.ExPLineLayerComment              = this.layerCommentTextBox.Text;
                    config.ExPLineLayerLineColor            = this.layerLineColorPanel.BackColor;
					config.ExPLineLayerPointWidth			= (int)this.layerPointSize.Value;
					config.exPLineLayerLineWidth			= (int)this.layerLineWidth.Value;
                    config.ExPLineLayerDisplayComments      = this.checkDrawComment.Checked;
                    config.ExPLineLayerPointColor           = this.layerPointColorPanel.BackColor;
                    config.ExPLineLayerDisplayCommentMaxLength = (int)this.numCommentMaxSize.Value;
                    break;
                case LayerType.Image:

                    break;
                default: break;
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

        private void clearLayerButton_Click(object sender, EventArgs e)
        {
            switch (layerType)
	        {
                case LayerType.PointCanvas:
                    mainControler.ClearTransportLayer(LayerType.PointCanvas, "Punkt-Austauschebene");
                    break;
                case LayerType.PolygonCanvas:
                    mainControler.ClearTransportLayer(LayerType.PolygonCanvas, "Polygon-Austauschebene");
                    break;
                case LayerType.PolylineCanvas:
                    mainControler.ClearTransportLayer(LayerType.PolylineCanvas, "Polyline-Austauschebene");
                    break;
	        }
        }

        private void checkDrawComment_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkDrawComment.Checked)
            {
                this.lblMaxCommentSize.Enabled = true;
                this.numCommentMaxSize.Enabled = true;
            }
            else
            {
                this.lblMaxCommentSize.Enabled = false;
                this.numCommentMaxSize.Enabled = false;
            }
        }
    }
}
