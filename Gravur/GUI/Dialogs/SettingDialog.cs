using System; 
using System.Drawing; 
using System.Collections; 
using System.ComponentModel; 
using System.Windows.Forms;
using GravurGIS.GUI.Controls;
using GravurGIS.Layers;
using System.Collections.Generic;
using GravurGIS.CoordinateSystems;
using System.IO;

namespace GravurGIS.GUI.Dialogs
{
    
    /// <summary> 
    /// Summary description for Form2. 
    /// </summary> 
    public class SettingDialog : IDialog
    {
        private System.Windows.Forms.Label Caption;
        private TabControl layerTabControl;
        private TabPage generalTab;
        private TabPage behaviourTab;
        private TabPage scaleTab;
        private System.Windows.Forms.Panel panel1;
        private TabPage exchangeTab;
        private Microsoft.WindowsCE.Forms.InputPanel inputPanel1;
        private MainControler mainControler;
        private TransportPolylineLayer tLine;
        private TransportMultiPointLayer tPoint;
        private TransportPolygonLayer tPolygon;
        private IContainer components;
        private TabPage gpsTab;
        private TabPage miscTab;
        private TabPage coordTab;
        private IconBox stOpenIconBox;
        private LabelEx settingDefaultProjectTB;
        private CheckBox checkBox1;
        private Label label6;
        private IconBox removeDirIconBox;
        private IconBox addDirIconBox;
        private ListBox settingsSearchDirListbox;
        private Label label10;
        private Label label9;
        private CheckBox sesstingsSpecialLayers;
        private CheckBox ShowGPSPositionCB;
        private NumericUpDown zoomOutPercent;
        private NumericUpDown zoomInPercent;
        private Label label11;
        private Label label12;
        private IconBox PolygonLayerSettings;
        private IconBox PolylineLayerSettings;
        private IconBox PointLayerSettings;
        private TextBox settingsTrasferPolygonName;
        private Label labelTransferPolygon;
        private TextBox settingsTrasferPolyLineName;
        private TextBox settingsTrasferPointName;
        private Label label3;
        private Label label2;
        private Config config;
        private Button btnConfigOtherProj;
        private RadioButton chkCoordOther;
        private RadioButton chkCoordGK;
        private RadioButton chkCoordGeo;
        private NumericUpDown coordGKStripe;
        private TabPage datumTab;
        private TextBox tBElName;
        private Label txtName;
        private ComboBox comboDatums;
        private Label lblA;
        private TextBox tbElA;
        private Label label1;
        private TextBox tbElB;
        private Label label4;
        private TextBox tbElRx;
        private Label label5;
        private TextBox tbElDx;
        private Label label7;
        private TextBox tbElRy;
        private Label label8;
        private TextBox tbElDy;
        private Label label13;
        private TextBox tbElRz;
        private Label label14;
        private TextBox tbElDz;
        private Label label25;
        private Label label24;
        private TextBox tbElAprev;
        private TextBox tbElS;
        private Label label15;
        private TextBox textBox10;
        private Label label16;
        private TextBox textBox11;
        private Label label17;
        private TextBox textBox12;
        private Label label18;
        private TextBox textBox13;
        private Label label19;
        private TextBox textBox14;
        private Label label20;
        private TextBox textBox15;
        private Label label21;
        private TextBox textBox16;
        private Label label22;
        private TextBox textBox17;
        private TextBox textBox18;
        private Label label23;
        private ComboBox comboBox2;
        private Button bElSave;

        private bool isMoveable = false;

        private bool initialized = false;

        private Dictionary<string, Ellipsoid> referenceEllipsoids = new Dictionary<string, Ellipsoid>();
        private List<HorizontalDatum> datums = new List<HorizontalDatum>();
        private Label lblGpsColor;
        private ColorIcon gpsColor;
        private NumericUpDown gpsWidth;
        private Label lblGpsWidth;
        private IconBox PolygonLayerFile;
        private IconBox PolyLineLayerFile;
        private IconBox PointLayerFile;
        private CheckBox cbShowScaleOnMap;
        private ColorIcon ciNewLayerColor;
        private RadioButton newLayerSpecificColor;
        private RadioButton newLayerRandomColor;
        private Label label26;
        private IconBox bRemoveCategory;
        private IconBox bAddCategory;
        private ListBox lbCategories;
        private Label label27;
        private ComboBox gpsViewSelection;
        private Label lGPSViewMode;
        private CheckBox cbShowNorthArrow;
        private NumericUpDown maxSeletClickDist;
		private Label label29;
		private CheckBox cbGPSTrackingActivate;
		private ComboBox cbGPSTrackingStartMode;
		private Label lGPSTrackingStartMode;
		private RadioButton rbGPSTrackingTriggerKey;
		private Label lGPSTriggerTimer;
		private TextBoxEx tbGPSTrackingTimeInterval;
		private Label lTrigger;
		private TextBox tbGPSTrackingComment;
		private Label lGPSTrackingComment;
		private RadioButton rbGPSTrackingTriggerTime;
		private ComboBox cbKeyMappingsDown;
		private ComboBox cbKeyMappingsUp;
		private Label lKeyMappingsUp;
		private ComboBox cbKeyMappingsEnter;
		private ComboBox cbKeyMappingsRight;
		private ComboBox cbKeyMappingsLeft;
		private Label label28;
		private Label lKeyMappings;
		private CheckBox cbGPSTrackingDiscardSamePos;
		private NumericUpDown nGPSUpdateInterval;
		private Label lGPSUpdateInterval;
        private Dictionary<string, int> datumsHashTable = new Dictionary<string, int>();

        //private HashedSet   

        //private bool infoTab;
        /// <summary>
        /// A Dialog for editing the properties of a layer.
        /// </summary>
        public SettingDialog()
        {
            // 
            // Required for Windows Form Designer support 
            // 
            InitializeComponent();
        }
        public SettingDialog(Rectangle visibleRect, MainControler mainControler)
            : this()
        {
            this.config = mainControler.Config;

            this.Location = new System.Drawing.Point(0, visibleRect.Y);
            this.ClientSize = new System.Drawing.Size(visibleRect.Width, visibleRect.Height);
            this.layerTabControl.Height = this.ClientSize.Height - Caption.Height - 3;
            this.panel1.Height = layerTabControl.Height;

            resizeToRect(visibleRect);
            this.mainControler = mainControler;
            this.tLine = mainControler.LayerManager.TransportPolylineLayer;
            this.tPoint = mainControler.LayerManager.TransportPointLayer;
            this.tPolygon = mainControler.LayerManager.TransportPolygonLayer;
            this.config = mainControler.Config;

            PointLayerSettings.Icon = ToolbarMaker.GetIconFromFile(config.ApplicationDirectory + @"\Icons\tbSettings");
            PointLayerFile.Icon = ToolbarMaker.GetIconFromFile(config.ApplicationDirectory + @"\Icons\tbOpen");
            PolylineLayerSettings.Icon = ToolbarMaker.GetIconFromFile(config.ApplicationDirectory + @"\Icons\tbSettings");
            PolyLineLayerFile.Icon = ToolbarMaker.GetIconFromFile(config.ApplicationDirectory + @"\Icons\tbOpen");
            PolygonLayerSettings.Icon = ToolbarMaker.GetIconFromFile(config.ApplicationDirectory + @"\Icons\tbSettings");
            PolygonLayerFile.Icon = ToolbarMaker.GetIconFromFile(config.ApplicationDirectory + @"\Icons\tbOpen");
            
            
            bAddCategory.Icon = addDirIconBox.Icon = ToolbarMaker.GetIconFromFile(config.ApplicationDirectory + @"\Icons\plus");
            bRemoveCategory.Icon = removeDirIconBox.Icon = ToolbarMaker.GetIconFromFile(config.ApplicationDirectory + @"\Icons\minus");
            stOpenIconBox.Icon = ToolbarMaker.GetIconFromFile(config.ApplicationDirectory + @"\Icons\tbOpen");

            PointLayerSettings.Normal_BackColor
            = PolylineLayerSettings.Normal_BackColor
            = PolygonLayerSettings.Normal_BackColor
            = PointLayerFile.Normal_BackColor
            = PolyLineLayerFile.Normal_BackColor
            = PolygonLayerFile.Normal_BackColor
            
            = addDirIconBox.Normal_BackColor
            = removeDirIconBox.Normal_BackColor
            = stOpenIconBox.Normal_BackColor
            = bAddCategory.Normal_BackColor
            = bRemoveCategory.Normal_BackColor
            = settingDefaultProjectTB.BackColor = SystemColors.Window;

            if (mainControler.DisplayResolution != DisplayResolution.QVGA)
            {
                this.addDirIconBox.Location =
                    new Point(addDirIconBox.Location.X - 3, addDirIconBox.Location.Y - 4);
                this.removeDirIconBox.Location =
                    new Point(this.removeDirIconBox.Location.X - 3, this.removeDirIconBox.Location.Y + 8);
                this.settingsSearchDirListbox.Size =
                    new Size(settingsSearchDirListbox.Size.Width, settingsSearchDirListbox.Height + 4);

                this.bAddCategory.Location =
                    new Point(bAddCategory.Location.X - 3, bAddCategory.Location.Y - 4);
                this.bRemoveCategory.Location =
                    new Point(this.bRemoveCategory.Location.X - 3, this.bRemoveCategory.Location.Y + 8);
                this.lbCategories.Size =
                    new Size(lbCategories.Size.Width, lbCategories.Height + 4);
            }
            if (config.GpsViewMode == GpsView.StaticMap)
                gpsViewSelection.SelectedItem = gpsViewSelection.Items[0];
            else gpsViewSelection.SelectedItem = gpsViewSelection.Items[1];
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
			System.Windows.Forms.Label lKeyMappingsRight;
			System.Windows.Forms.Label lKeyMappingsEnter;
			System.Windows.Forms.Label lKeyMappingsDown;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingDialog));
			this.rbGPSTrackingTriggerTime = new System.Windows.Forms.RadioButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.layerTabControl = new System.Windows.Forms.TabControl();
			this.generalTab = new System.Windows.Forms.TabPage();
			this.removeDirIconBox = new GravurGIS.GUI.Controls.IconBox();
			this.addDirIconBox = new GravurGIS.GUI.Controls.IconBox();
			this.settingsSearchDirListbox = new System.Windows.Forms.ListBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.stOpenIconBox = new GravurGIS.GUI.Controls.IconBox();
			this.settingDefaultProjectTB = new GravurGIS.GUI.Controls.LabelEx();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.label6 = new System.Windows.Forms.Label();
			this.behaviourTab = new System.Windows.Forms.TabPage();
			this.maxSeletClickDist = new System.Windows.Forms.NumericUpDown();
			this.label29 = new System.Windows.Forms.Label();
			this.zoomInPercent = new System.Windows.Forms.NumericUpDown();
			this.zoomOutPercent = new System.Windows.Forms.NumericUpDown();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.exchangeTab = new System.Windows.Forms.TabPage();
			this.bRemoveCategory = new GravurGIS.GUI.Controls.IconBox();
			this.bAddCategory = new GravurGIS.GUI.Controls.IconBox();
			this.lbCategories = new System.Windows.Forms.ListBox();
			this.label27 = new System.Windows.Forms.Label();
			this.PolygonLayerFile = new GravurGIS.GUI.Controls.IconBox();
			this.PolyLineLayerFile = new GravurGIS.GUI.Controls.IconBox();
			this.PointLayerFile = new GravurGIS.GUI.Controls.IconBox();
			this.PolygonLayerSettings = new GravurGIS.GUI.Controls.IconBox();
			this.PolylineLayerSettings = new GravurGIS.GUI.Controls.IconBox();
			this.PointLayerSettings = new GravurGIS.GUI.Controls.IconBox();
			this.settingsTrasferPolygonName = new System.Windows.Forms.TextBox();
			this.labelTransferPolygon = new System.Windows.Forms.Label();
			this.settingsTrasferPolyLineName = new System.Windows.Forms.TextBox();
			this.settingsTrasferPointName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.gpsTab = new System.Windows.Forms.TabPage();
			this.nGPSUpdateInterval = new System.Windows.Forms.NumericUpDown();
			this.lGPSUpdateInterval = new System.Windows.Forms.Label();
			this.cbGPSTrackingDiscardSamePos = new System.Windows.Forms.CheckBox();
			this.rbGPSTrackingTriggerKey = new System.Windows.Forms.RadioButton();
			this.lGPSTriggerTimer = new System.Windows.Forms.Label();
			this.tbGPSTrackingTimeInterval = new GravurGIS.GUI.Controls.TextBoxEx();
			this.lTrigger = new System.Windows.Forms.Label();
			this.tbGPSTrackingComment = new System.Windows.Forms.TextBox();
			this.lGPSTrackingComment = new System.Windows.Forms.Label();
			this.cbGPSTrackingStartMode = new System.Windows.Forms.ComboBox();
			this.lGPSTrackingStartMode = new System.Windows.Forms.Label();
			this.cbGPSTrackingActivate = new System.Windows.Forms.CheckBox();
			this.lGPSViewMode = new System.Windows.Forms.Label();
			this.gpsViewSelection = new System.Windows.Forms.ComboBox();
			this.gpsWidth = new System.Windows.Forms.NumericUpDown();
			this.lblGpsWidth = new System.Windows.Forms.Label();
			this.lblGpsColor = new System.Windows.Forms.Label();
			this.gpsColor = new GravurGIS.GUI.Controls.ColorIcon();
			this.ShowGPSPositionCB = new System.Windows.Forms.CheckBox();
			this.coordTab = new System.Windows.Forms.TabPage();
			this.coordGKStripe = new System.Windows.Forms.NumericUpDown();
			this.btnConfigOtherProj = new System.Windows.Forms.Button();
			this.chkCoordOther = new System.Windows.Forms.RadioButton();
			this.chkCoordGK = new System.Windows.Forms.RadioButton();
			this.chkCoordGeo = new System.Windows.Forms.RadioButton();
			this.datumTab = new System.Windows.Forms.TabPage();
			this.bElSave = new System.Windows.Forms.Button();
			this.tbElS = new System.Windows.Forms.TextBox();
			this.label25 = new System.Windows.Forms.Label();
			this.label24 = new System.Windows.Forms.Label();
			this.tbElAprev = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.tbElRz = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.tbElDz = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.tbElRy = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.tbElDy = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.tbElRx = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.tbElDx = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tbElB = new System.Windows.Forms.TextBox();
			this.lblA = new System.Windows.Forms.Label();
			this.tbElA = new System.Windows.Forms.TextBox();
			this.tBElName = new System.Windows.Forms.TextBox();
			this.txtName = new System.Windows.Forms.Label();
			this.comboDatums = new System.Windows.Forms.ComboBox();
			this.miscTab = new System.Windows.Forms.TabPage();
			this.cbKeyMappingsDown = new System.Windows.Forms.ComboBox();
			this.cbKeyMappingsUp = new System.Windows.Forms.ComboBox();
			this.lKeyMappingsUp = new System.Windows.Forms.Label();
			this.cbKeyMappingsEnter = new System.Windows.Forms.ComboBox();
			this.cbKeyMappingsRight = new System.Windows.Forms.ComboBox();
			this.cbKeyMappingsLeft = new System.Windows.Forms.ComboBox();
			this.label28 = new System.Windows.Forms.Label();
			this.lKeyMappings = new System.Windows.Forms.Label();
			this.cbShowNorthArrow = new System.Windows.Forms.CheckBox();
			this.ciNewLayerColor = new GravurGIS.GUI.Controls.ColorIcon();
			this.newLayerSpecificColor = new System.Windows.Forms.RadioButton();
			this.newLayerRandomColor = new System.Windows.Forms.RadioButton();
			this.label26 = new System.Windows.Forms.Label();
			this.cbShowScaleOnMap = new System.Windows.Forms.CheckBox();
			this.sesstingsSpecialLayers = new System.Windows.Forms.CheckBox();
			this.scaleTab = new System.Windows.Forms.TabPage();
			this.Caption = new System.Windows.Forms.Label();
			this.inputPanel1 = new Microsoft.WindowsCE.Forms.InputPanel(this.components);
			this.label15 = new System.Windows.Forms.Label();
			this.textBox10 = new System.Windows.Forms.TextBox();
			this.label16 = new System.Windows.Forms.Label();
			this.textBox11 = new System.Windows.Forms.TextBox();
			this.label17 = new System.Windows.Forms.Label();
			this.textBox12 = new System.Windows.Forms.TextBox();
			this.label18 = new System.Windows.Forms.Label();
			this.textBox13 = new System.Windows.Forms.TextBox();
			this.label19 = new System.Windows.Forms.Label();
			this.textBox14 = new System.Windows.Forms.TextBox();
			this.label20 = new System.Windows.Forms.Label();
			this.textBox15 = new System.Windows.Forms.TextBox();
			this.label21 = new System.Windows.Forms.Label();
			this.textBox16 = new System.Windows.Forms.TextBox();
			this.label22 = new System.Windows.Forms.Label();
			this.textBox17 = new System.Windows.Forms.TextBox();
			this.textBox18 = new System.Windows.Forms.TextBox();
			this.label23 = new System.Windows.Forms.Label();
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			lKeyMappingsRight = new System.Windows.Forms.Label();
			lKeyMappingsEnter = new System.Windows.Forms.Label();
			lKeyMappingsDown = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.layerTabControl.SuspendLayout();
			this.generalTab.SuspendLayout();
			this.behaviourTab.SuspendLayout();
			this.exchangeTab.SuspendLayout();
			this.gpsTab.SuspendLayout();
			this.coordTab.SuspendLayout();
			this.datumTab.SuspendLayout();
			this.miscTab.SuspendLayout();
			this.SuspendLayout();
			// 
			// lKeyMappingsRight
			// 
			lKeyMappingsRight.Location = new System.Drawing.Point(22, 262);
			lKeyMappingsRight.Name = "lKeyMappingsRight";
			lKeyMappingsRight.Size = new System.Drawing.Size(47, 20);
			lKeyMappingsRight.Text = "Rechts";
			// 
			// lKeyMappingsEnter
			// 
			lKeyMappingsEnter.Location = new System.Drawing.Point(22, 290);
			lKeyMappingsEnter.Name = "lKeyMappingsEnter";
			lKeyMappingsEnter.Size = new System.Drawing.Size(39, 20);
			lKeyMappingsEnter.Text = "Enter";
			// 
			// lKeyMappingsDown
			// 
			lKeyMappingsDown.Location = new System.Drawing.Point(22, 207);
			lKeyMappingsDown.Name = "lKeyMappingsDown";
			lKeyMappingsDown.Size = new System.Drawing.Size(47, 20);
			lKeyMappingsDown.Text = "Unten";
			// 
			// rbGPSTrackingTriggerTime
			// 
			this.rbGPSTrackingTriggerTime.Location = new System.Drawing.Point(14, 284);
			this.rbGPSTrackingTriggerTime.Name = "rbGPSTrackingTriggerTime";
			this.rbGPSTrackingTriggerTime.Size = new System.Drawing.Size(100, 20);
			this.rbGPSTrackingTriggerTime.TabIndex = 154;
			this.rbGPSTrackingTriggerTime.Text = "Zeitintervall";
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
			this.layerTabControl.Controls.Add(this.behaviourTab);
			this.layerTabControl.Controls.Add(this.exchangeTab);
			this.layerTabControl.Controls.Add(this.gpsTab);
			this.layerTabControl.Controls.Add(this.coordTab);
			this.layerTabControl.Controls.Add(this.datumTab);
			this.layerTabControl.Controls.Add(this.miscTab);
			this.layerTabControl.Location = new System.Drawing.Point(0, 0);
			this.layerTabControl.Name = "layerTabControl";
			this.layerTabControl.SelectedIndex = 0;
			this.layerTabControl.Size = new System.Drawing.Size(238, 172);
			this.layerTabControl.TabIndex = 0;
			// 
			// generalTab
			// 
			this.generalTab.AutoScroll = true;
			this.generalTab.Controls.Add(this.settingDefaultProjectTB);
			this.generalTab.Controls.Add(this.removeDirIconBox);
			this.generalTab.Controls.Add(this.addDirIconBox);
			this.generalTab.Controls.Add(this.settingsSearchDirListbox);
			this.generalTab.Controls.Add(this.label10);
			this.generalTab.Controls.Add(this.label9);
			this.generalTab.Controls.Add(this.stOpenIconBox);
			this.generalTab.Controls.Add(this.checkBox1);
			this.generalTab.Controls.Add(this.label6);
			this.generalTab.Location = new System.Drawing.Point(0, 0);
			this.generalTab.Name = "generalTab";
			this.generalTab.Size = new System.Drawing.Size(238, 149);
			this.generalTab.Text = "Allgemein";
			// 
			// removeDirIconBox
			// 
			this.removeDirIconBox.Location = new System.Drawing.Point(200, 137);
			this.removeDirIconBox.Name = "removeDirIconBox";
			this.removeDirIconBox.Size = new System.Drawing.Size(16, 21);
			this.removeDirIconBox.TabIndex = 129;
			this.removeDirIconBox.Click += new System.EventHandler(this.removeDirIconBox_Click_1);
			// 
			// addDirIconBox
			// 
			this.addDirIconBox.Location = new System.Drawing.Point(200, 114);
			this.addDirIconBox.Name = "addDirIconBox";
			this.addDirIconBox.Size = new System.Drawing.Size(16, 21);
			this.addDirIconBox.TabIndex = 128;
			this.addDirIconBox.Click += new System.EventHandler(this.addDirIconBox_Click_1);
			// 
			// settingsSearchDirListbox
			// 
			this.settingsSearchDirListbox.Location = new System.Drawing.Point(42, 112);
			this.settingsSearchDirListbox.Name = "settingsSearchDirListbox";
			this.settingsSearchDirListbox.Size = new System.Drawing.Size(153, 44);
			this.settingsSearchDirListbox.TabIndex = 127;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(27, 90);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(192, 77);
			this.label10.Text = "Zusätzliche Such-Orte für Dateien:";
			// 
			// label9
			// 
			this.label9.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
			this.label9.Location = new System.Drawing.Point(7, 72);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(99, 19);
			this.label9.Text = "Ordner:";
			// 
			// stOpenIconBox
			// 
			this.stOpenIconBox.Enabled = false;
			this.stOpenIconBox.Icon = ((System.Drawing.Icon)(resources.GetObject("stOpenIconBox.Icon")));
			this.stOpenIconBox.Location = new System.Drawing.Point(43, 44);
			this.stOpenIconBox.Name = "stOpenIconBox";
			this.stOpenIconBox.Size = new System.Drawing.Size(16, 16);
			this.stOpenIconBox.TabIndex = 121;
			this.stOpenIconBox.Click += new System.EventHandler(this.stOpenIconBox_Click_1);
			// 
			// settingDefaultProjectTB
			// 
			this.settingDefaultProjectTB.Enabled = false;
			this.settingDefaultProjectTB.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.settingDefaultProjectTB.Location = new System.Drawing.Point(63, 42);
			this.settingDefaultProjectTB.Name = "settingDefaultProjectTB";
			this.settingDefaultProjectTB.Size = new System.Drawing.Size(156, 45);
			this.settingDefaultProjectTB.Text = "(keins)";
			// 
			// checkBox1
			// 
			this.checkBox1.Location = new System.Drawing.Point(27, 21);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(126, 25);
			this.checkBox1.TabIndex = 123;
			this.checkBox1.Text = "Standard-Projekt:";
			this.checkBox1.CheckStateChanged += new System.EventHandler(this.checkBox1_CheckStateChanged_1);
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
			this.label6.Location = new System.Drawing.Point(7, 4);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(105, 19);
			this.label6.Text = "Programmstart:";
			// 
			// behaviourTab
			// 
			this.behaviourTab.AutoScroll = true;
			this.behaviourTab.Controls.Add(this.maxSeletClickDist);
			this.behaviourTab.Controls.Add(this.label29);
			this.behaviourTab.Controls.Add(this.zoomInPercent);
			this.behaviourTab.Controls.Add(this.zoomOutPercent);
			this.behaviourTab.Controls.Add(this.label11);
			this.behaviourTab.Controls.Add(this.label12);
			this.behaviourTab.Location = new System.Drawing.Point(0, 0);
			this.behaviourTab.Name = "behaviourTab";
			this.behaviourTab.Size = new System.Drawing.Size(230, 146);
			this.behaviourTab.Text = "Verhalten";
			// 
			// maxSeletClickDist
			// 
			this.maxSeletClickDist.Location = new System.Drawing.Point(96, 89);
			this.maxSeletClickDist.Name = "maxSeletClickDist";
			this.maxSeletClickDist.Size = new System.Drawing.Size(100, 22);
			this.maxSeletClickDist.TabIndex = 161;
			this.maxSeletClickDist.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
			// 
			// label29
			// 
			this.label29.Location = new System.Drawing.Point(7, 64);
			this.label29.Name = "label29";
			this.label29.Size = new System.Drawing.Size(181, 20);
			this.label29.Text = "Selektions-Klick-Toleranz [px]:";
			// 
			// zoomInPercent
			// 
			this.zoomInPercent.ForeColor = System.Drawing.Color.Black;
			this.zoomInPercent.Location = new System.Drawing.Point(96, 7);
			this.zoomInPercent.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.zoomInPercent.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.zoomInPercent.Name = "zoomInPercent";
			this.zoomInPercent.Size = new System.Drawing.Size(100, 22);
			this.zoomInPercent.TabIndex = 156;
			this.zoomInPercent.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
			// 
			// zoomOutPercent
			// 
			this.zoomOutPercent.Location = new System.Drawing.Point(96, 33);
			this.zoomOutPercent.Name = "zoomOutPercent";
			this.zoomOutPercent.Size = new System.Drawing.Size(100, 22);
			this.zoomOutPercent.TabIndex = 157;
			this.zoomOutPercent.Value = new decimal(new int[] {
            85,
            0,
            0,
            0});
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(7, 10);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(77, 18);
			this.label11.Text = "Zoom In [%]";
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(7, 37);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(93, 18);
			this.label12.Text = "Zoom Out [%]";
			// 
			// exchangeTab
			// 
			this.exchangeTab.AutoScroll = true;
			this.exchangeTab.AutoScrollMargin = new System.Drawing.Size(0, 10);
			this.exchangeTab.Controls.Add(this.bRemoveCategory);
			this.exchangeTab.Controls.Add(this.bAddCategory);
			this.exchangeTab.Controls.Add(this.lbCategories);
			this.exchangeTab.Controls.Add(this.label27);
			this.exchangeTab.Controls.Add(this.PolygonLayerFile);
			this.exchangeTab.Controls.Add(this.PolyLineLayerFile);
			this.exchangeTab.Controls.Add(this.PointLayerFile);
			this.exchangeTab.Controls.Add(this.PolygonLayerSettings);
			this.exchangeTab.Controls.Add(this.PolylineLayerSettings);
			this.exchangeTab.Controls.Add(this.PointLayerSettings);
			this.exchangeTab.Controls.Add(this.settingsTrasferPolygonName);
			this.exchangeTab.Controls.Add(this.labelTransferPolygon);
			this.exchangeTab.Controls.Add(this.settingsTrasferPolyLineName);
			this.exchangeTab.Controls.Add(this.settingsTrasferPointName);
			this.exchangeTab.Controls.Add(this.label3);
			this.exchangeTab.Controls.Add(this.label2);
			this.exchangeTab.Location = new System.Drawing.Point(0, 0);
			this.exchangeTab.Name = "exchangeTab";
			this.exchangeTab.Size = new System.Drawing.Size(230, 146);
			this.exchangeTab.Text = "Datenaustausch";
			// 
			// bRemoveCategory
			// 
			this.bRemoveCategory.Location = new System.Drawing.Point(180, 135);
			this.bRemoveCategory.Name = "bRemoveCategory";
			this.bRemoveCategory.Size = new System.Drawing.Size(16, 21);
			this.bRemoveCategory.TabIndex = 133;
			this.bRemoveCategory.Click += new System.EventHandler(this.bRemoveCategory_Click);
			// 
			// bAddCategory
			// 
			this.bAddCategory.Location = new System.Drawing.Point(180, 112);
			this.bAddCategory.Name = "bAddCategory";
			this.bAddCategory.Size = new System.Drawing.Size(16, 21);
			this.bAddCategory.TabIndex = 132;
			this.bAddCategory.Click += new System.EventHandler(this.bAddCategory_Click);
			// 
			// lbCategories
			// 
			this.lbCategories.Location = new System.Drawing.Point(22, 110);
			this.lbCategories.Name = "lbCategories";
			this.lbCategories.Size = new System.Drawing.Size(153, 44);
			this.lbCategories.TabIndex = 131;
			// 
			// label27
			// 
			this.label27.Location = new System.Drawing.Point(7, 88);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(192, 19);
			this.label27.Text = "Mögliche Kategorien der Objekte:";
			// 
			// PolygonLayerFile
			// 
			this.PolygonLayerFile.Location = new System.Drawing.Point(181, 55);
			this.PolygonLayerFile.Name = "PolygonLayerFile";
			this.PolygonLayerFile.Size = new System.Drawing.Size(16, 16);
			this.PolygonLayerFile.TabIndex = 33;
			this.PolygonLayerFile.Click += new System.EventHandler(this.TransPortLayerPath_Click);
			// 
			// PolyLineLayerFile
			// 
			this.PolyLineLayerFile.Location = new System.Drawing.Point(181, 32);
			this.PolyLineLayerFile.Name = "PolyLineLayerFile";
			this.PolyLineLayerFile.Size = new System.Drawing.Size(16, 16);
			this.PolyLineLayerFile.TabIndex = 32;
			this.PolyLineLayerFile.Click += new System.EventHandler(this.TransPortLayerPath_Click);
			// 
			// PointLayerFile
			// 
			this.PointLayerFile.Location = new System.Drawing.Point(181, 10);
			this.PointLayerFile.Name = "PointLayerFile";
			this.PointLayerFile.Size = new System.Drawing.Size(16, 16);
			this.PointLayerFile.TabIndex = 31;
			this.PointLayerFile.Click += new System.EventHandler(this.TransPortLayerPath_Click);
			// 
			// PolygonLayerSettings
			// 
			this.PolygonLayerSettings.Location = new System.Drawing.Point(202, 55);
			this.PolygonLayerSettings.Name = "PolygonLayerSettings";
			this.PolygonLayerSettings.Size = new System.Drawing.Size(16, 16);
			this.PolygonLayerSettings.TabIndex = 27;
			this.PolygonLayerSettings.Click += new System.EventHandler(this.PolygonLayerSettings_Click);
			// 
			// PolylineLayerSettings
			// 
			this.PolylineLayerSettings.Location = new System.Drawing.Point(202, 32);
			this.PolylineLayerSettings.Name = "PolylineLayerSettings";
			this.PolylineLayerSettings.Size = new System.Drawing.Size(16, 16);
			this.PolylineLayerSettings.TabIndex = 26;
			this.PolylineLayerSettings.Click += new System.EventHandler(this.PolylineLayerSettings_Click);
			// 
			// PointLayerSettings
			// 
			this.PointLayerSettings.Location = new System.Drawing.Point(202, 10);
			this.PointLayerSettings.Name = "PointLayerSettings";
			this.PointLayerSettings.Size = new System.Drawing.Size(16, 16);
			this.PointLayerSettings.TabIndex = 25;
			this.PointLayerSettings.Click += new System.EventHandler(this.PointLayerSettings_Click);
			// 
			// settingsTrasferPolygonName
			// 
			this.settingsTrasferPolygonName.Location = new System.Drawing.Point(90, 53);
			this.settingsTrasferPolygonName.Name = "settingsTrasferPolygonName";
			this.settingsTrasferPolygonName.Size = new System.Drawing.Size(85, 21);
			this.settingsTrasferPolygonName.TabIndex = 24;
			this.settingsTrasferPolygonName.Text = "\\My Documents\\shp\\transferPolygon.shp";
			// 
			// labelTransferPolygon
			// 
			this.labelTransferPolygon.Location = new System.Drawing.Point(7, 54);
			this.labelTransferPolygon.Name = "labelTransferPolygon";
			this.labelTransferPolygon.Size = new System.Drawing.Size(100, 20);
			this.labelTransferPolygon.Text = "Polygonlayer:";
			// 
			// settingsTrasferPolyLineName
			// 
			this.settingsTrasferPolyLineName.Location = new System.Drawing.Point(90, 31);
			this.settingsTrasferPolyLineName.Name = "settingsTrasferPolyLineName";
			this.settingsTrasferPolyLineName.Size = new System.Drawing.Size(85, 21);
			this.settingsTrasferPolyLineName.TabIndex = 23;
			this.settingsTrasferPolyLineName.Text = "\\My Documents\\shp\\transferPolylinie.shp";
			// 
			// settingsTrasferPointName
			// 
			this.settingsTrasferPointName.Location = new System.Drawing.Point(90, 9);
			this.settingsTrasferPointName.Name = "settingsTrasferPointName";
			this.settingsTrasferPointName.Size = new System.Drawing.Size(85, 21);
			this.settingsTrasferPointName.TabIndex = 22;
			this.settingsTrasferPointName.Text = "\\My Documents\\shp\\transferPunkt.shp";
			this.settingsTrasferPointName.WordWrap = false;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(7, 32);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(89, 20);
			this.label3.Text = "Linienlayer:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(7, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(66, 20);
			this.label2.Text = "Punktlayer:";
			// 
			// gpsTab
			// 
			this.gpsTab.AutoScroll = true;
			this.gpsTab.AutoScrollMargin = new System.Drawing.Size(0, 10);
			this.gpsTab.Controls.Add(this.nGPSUpdateInterval);
			this.gpsTab.Controls.Add(this.lGPSUpdateInterval);
			this.gpsTab.Controls.Add(this.cbGPSTrackingDiscardSamePos);
			this.gpsTab.Controls.Add(this.rbGPSTrackingTriggerKey);
			this.gpsTab.Controls.Add(this.lGPSTriggerTimer);
			this.gpsTab.Controls.Add(this.tbGPSTrackingTimeInterval);
			this.gpsTab.Controls.Add(this.rbGPSTrackingTriggerTime);
			this.gpsTab.Controls.Add(this.lTrigger);
			this.gpsTab.Controls.Add(this.tbGPSTrackingComment);
			this.gpsTab.Controls.Add(this.lGPSTrackingComment);
			this.gpsTab.Controls.Add(this.cbGPSTrackingStartMode);
			this.gpsTab.Controls.Add(this.lGPSTrackingStartMode);
			this.gpsTab.Controls.Add(this.cbGPSTrackingActivate);
			this.gpsTab.Controls.Add(this.lGPSViewMode);
			this.gpsTab.Controls.Add(this.gpsViewSelection);
			this.gpsTab.Controls.Add(this.gpsWidth);
			this.gpsTab.Controls.Add(this.lblGpsWidth);
			this.gpsTab.Controls.Add(this.lblGpsColor);
			this.gpsTab.Controls.Add(this.gpsColor);
			this.gpsTab.Controls.Add(this.ShowGPSPositionCB);
			this.gpsTab.Location = new System.Drawing.Point(0, 0);
			this.gpsTab.Name = "gpsTab";
			this.gpsTab.Size = new System.Drawing.Size(230, 146);
			this.gpsTab.Text = "GPS";
			// 
			// nGPSUpdateInterval
			// 
			this.nGPSUpdateInterval.Location = new System.Drawing.Point(164, 124);
			this.nGPSUpdateInterval.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
			this.nGPSUpdateInterval.Name = "nGPSUpdateInterval";
			this.nGPSUpdateInterval.Size = new System.Drawing.Size(51, 22);
			this.nGPSUpdateInterval.TabIndex = 174;
			this.nGPSUpdateInterval.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
			// 
			// lGPSUpdateInterval
			// 
			this.lGPSUpdateInterval.Location = new System.Drawing.Point(11, 124);
			this.lGPSUpdateInterval.Name = "lGPSUpdateInterval";
			this.lGPSUpdateInterval.Size = new System.Drawing.Size(147, 20);
			this.lGPSUpdateInterval.Text = "Aktualisierungsrate (sec):";
			// 
			// cbGPSTrackingDiscardSamePos
			// 
			this.cbGPSTrackingDiscardSamePos.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
			this.cbGPSTrackingDiscardSamePos.Location = new System.Drawing.Point(14, 233);
			this.cbGPSTrackingDiscardSamePos.Name = "cbGPSTrackingDiscardSamePos";
			this.cbGPSTrackingDiscardSamePos.Size = new System.Drawing.Size(201, 26);
			this.cbGPSTrackingDiscardSamePos.TabIndex = 165;
			this.cbGPSTrackingDiscardSamePos.Text = "Mehrfach selbe Position verbieten";
			// 
			// rbGPSTrackingTriggerKey
			// 
			this.rbGPSTrackingTriggerKey.Location = new System.Drawing.Point(14, 310);
			this.rbGPSTrackingTriggerKey.Name = "rbGPSTrackingTriggerKey";
			this.rbGPSTrackingTriggerKey.Size = new System.Drawing.Size(100, 20);
			this.rbGPSTrackingTriggerKey.TabIndex = 157;
			this.rbGPSTrackingTriggerKey.Text = "Tastendruck";
			// 
			// lGPSTriggerTimer
			// 
			this.lGPSTriggerTimer.Location = new System.Drawing.Point(194, 285);
			this.lGPSTriggerTimer.Name = "lGPSTriggerTimer";
			this.lGPSTriggerTimer.Size = new System.Drawing.Size(21, 20);
			this.lGPSTriggerTimer.Text = "ms";
			// 
			// tbGPSTrackingTimeInterval
			// 
			this.tbGPSTrackingTimeInterval.AllowSpace = false;
			this.tbGPSTrackingTimeInterval.Location = new System.Drawing.Point(117, 283);
			this.tbGPSTrackingTimeInterval.Name = "tbGPSTrackingTimeInterval";
			this.tbGPSTrackingTimeInterval.NumbersOnly = true;
			this.tbGPSTrackingTimeInterval.Size = new System.Drawing.Size(71, 21);
			this.tbGPSTrackingTimeInterval.TabIndex = 155;
			this.tbGPSTrackingTimeInterval.Text = "5000";
			// 
			// lTrigger
			// 
			this.lTrigger.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
			this.lTrigger.Location = new System.Drawing.Point(14, 262);
			this.lTrigger.Name = "lTrigger";
			this.lTrigger.Size = new System.Drawing.Size(60, 20);
			this.lTrigger.Text = "Auslöser";
			// 
			// tbGPSTrackingComment
			// 
			this.tbGPSTrackingComment.Location = new System.Drawing.Point(104, 206);
			this.tbGPSTrackingComment.Name = "tbGPSTrackingComment";
			this.tbGPSTrackingComment.Size = new System.Drawing.Size(111, 21);
			this.tbGPSTrackingComment.TabIndex = 152;
			// 
			// lGPSTrackingComment
			// 
			this.lGPSTrackingComment.Location = new System.Drawing.Point(14, 207);
			this.lGPSTrackingComment.Name = "lGPSTrackingComment";
			this.lGPSTrackingComment.Size = new System.Drawing.Size(72, 20);
			this.lGPSTrackingComment.Text = "Kommentar";
			// 
			// cbGPSTrackingStartMode
			// 
			this.cbGPSTrackingStartMode.Items.Add("Nach Tastendruck");
			this.cbGPSTrackingStartMode.Items.Add("Nach Dialogende");
			this.cbGPSTrackingStartMode.Location = new System.Drawing.Point(104, 174);
			this.cbGPSTrackingStartMode.Name = "cbGPSTrackingStartMode";
			this.cbGPSTrackingStartMode.Size = new System.Drawing.Size(111, 22);
			this.cbGPSTrackingStartMode.TabIndex = 150;
			// 
			// lGPSTrackingStartMode
			// 
			this.lGPSTrackingStartMode.Location = new System.Drawing.Point(14, 176);
			this.lGPSTrackingStartMode.Name = "lGPSTrackingStartMode";
			this.lGPSTrackingStartMode.Size = new System.Drawing.Size(72, 20);
			this.lGPSTrackingStartMode.Text = "Startmodus";
			// 
			// cbGPSTrackingActivate
			// 
			this.cbGPSTrackingActivate.Location = new System.Drawing.Point(7, 152);
			this.cbGPSTrackingActivate.Name = "cbGPSTrackingActivate";
			this.cbGPSTrackingActivate.Size = new System.Drawing.Size(142, 20);
			this.cbGPSTrackingActivate.TabIndex = 148;
			this.cbGPSTrackingActivate.Text = "Tracking aktivieren";
			this.cbGPSTrackingActivate.CheckStateChanged += new System.EventHandler(this.cbGPSTrackingActivate_CheckStateChanged);
			// 
			// lGPSViewMode
			// 
			this.lGPSViewMode.Location = new System.Drawing.Point(11, 94);
			this.lGPSViewMode.Name = "lGPSViewMode";
			this.lGPSViewMode.Size = new System.Drawing.Size(83, 19);
			this.lGPSViewMode.Text = "Anzeigemodus";
			// 
			// gpsViewSelection
			// 
			this.gpsViewSelection.Items.Add("Statische Karte");
			this.gpsViewSelection.Items.Add("Statischer Punkt");
			this.gpsViewSelection.Location = new System.Drawing.Point(103, 91);
			this.gpsViewSelection.Name = "gpsViewSelection";
			this.gpsViewSelection.Size = new System.Drawing.Size(112, 22);
			this.gpsViewSelection.TabIndex = 145;
			// 
			// gpsWidth
			// 
			this.gpsWidth.Location = new System.Drawing.Point(168, 55);
			this.gpsWidth.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this.gpsWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.gpsWidth.Name = "gpsWidth";
			this.gpsWidth.Size = new System.Drawing.Size(47, 22);
			this.gpsWidth.TabIndex = 142;
			this.gpsWidth.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
			// 
			// lblGpsWidth
			// 
			this.lblGpsWidth.Location = new System.Drawing.Point(11, 55);
			this.lblGpsWidth.Name = "lblGpsWidth";
			this.lblGpsWidth.Size = new System.Drawing.Size(164, 20);
			this.lblGpsWidth.Text = "Punktgröße v. Position";
			// 
			// lblGpsColor
			// 
			this.lblGpsColor.Location = new System.Drawing.Point(11, 30);
			this.lblGpsColor.Name = "lblGpsColor";
			this.lblGpsColor.Size = new System.Drawing.Size(142, 20);
			this.lblGpsColor.Text = "Farbe der GPS-Position";
			// 
			// gpsColor
			// 
			this.gpsColor.Location = new System.Drawing.Point(200, 30);
			this.gpsColor.Name = "gpsColor";
			this.gpsColor.Size = new System.Drawing.Size(15, 15);
			this.gpsColor.TabIndex = 139;
			this.gpsColor.Click += new System.EventHandler(this.gpsColor_Click);
			// 
			// ShowGPSPositionCB
			// 
			this.ShowGPSPositionCB.Location = new System.Drawing.Point(7, 7);
			this.ShowGPSPositionCB.Name = "ShowGPSPositionCB";
			this.ShowGPSPositionCB.Size = new System.Drawing.Size(137, 20);
			this.ShowGPSPositionCB.TabIndex = 138;
			this.ShowGPSPositionCB.Text = "Position anzeigen";
			this.ShowGPSPositionCB.CheckStateChanged += new System.EventHandler(this.ShowGPSPositionCB_CheckStateChanged);
			// 
			// coordTab
			// 
			this.coordTab.Controls.Add(this.coordGKStripe);
			this.coordTab.Controls.Add(this.btnConfigOtherProj);
			this.coordTab.Controls.Add(this.chkCoordOther);
			this.coordTab.Controls.Add(this.chkCoordGK);
			this.coordTab.Controls.Add(this.chkCoordGeo);
			this.coordTab.Location = new System.Drawing.Point(0, 0);
			this.coordTab.Name = "coordTab";
			this.coordTab.Size = new System.Drawing.Size(230, 146);
			this.coordTab.Text = "Koordinaten";
			// 
			// coordGKStripe
			// 
			this.coordGKStripe.Location = new System.Drawing.Point(149, 34);
			this.coordGKStripe.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.coordGKStripe.Name = "coordGKStripe";
			this.coordGKStripe.Size = new System.Drawing.Size(82, 22);
			this.coordGKStripe.TabIndex = 4;
			this.coordGKStripe.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// btnConfigOtherProj
			// 
			this.btnConfigOtherProj.Enabled = false;
			this.btnConfigOtherProj.Location = new System.Drawing.Point(149, 62);
			this.btnConfigOtherProj.Name = "btnConfigOtherProj";
			this.btnConfigOtherProj.Size = new System.Drawing.Size(82, 20);
			this.btnConfigOtherProj.TabIndex = 3;
			this.btnConfigOtherProj.Text = "Bearbeiten";
			// 
			// chkCoordOther
			// 
			this.chkCoordOther.Enabled = false;
			this.chkCoordOther.Location = new System.Drawing.Point(8, 62);
			this.chkCoordOther.Name = "chkCoordOther";
			this.chkCoordOther.Size = new System.Drawing.Size(135, 20);
			this.chkCoordOther.TabIndex = 2;
			this.chkCoordOther.TabStop = false;
			this.chkCoordOther.Text = "Andere Projektion";
			this.chkCoordOther.CheckedChanged += new System.EventHandler(this.coordCheck_CheckedChanged);
			// 
			// chkCoordGK
			// 
			this.chkCoordGK.Checked = true;
			this.chkCoordGK.Location = new System.Drawing.Point(8, 35);
			this.chkCoordGK.Name = "chkCoordGK";
			this.chkCoordGK.Size = new System.Drawing.Size(144, 20);
			this.chkCoordGK.TabIndex = 1;
			this.chkCoordGK.Text = "Gauß Krüger - Zone:";
			this.chkCoordGK.CheckedChanged += new System.EventHandler(this.coordCheck_CheckedChanged);
			// 
			// chkCoordGeo
			// 
			this.chkCoordGeo.Enabled = false;
			this.chkCoordGeo.Location = new System.Drawing.Point(8, 8);
			this.chkCoordGeo.Name = "chkCoordGeo";
			this.chkCoordGeo.Size = new System.Drawing.Size(183, 20);
			this.chkCoordGeo.TabIndex = 0;
			this.chkCoordGeo.TabStop = false;
			this.chkCoordGeo.Text = "Geographische Koordinaten";
			// 
			// datumTab
			// 
			this.datumTab.AutoScroll = true;
			this.datumTab.AutoScrollMargin = new System.Drawing.Size(0, 10);
			this.datumTab.Controls.Add(this.bElSave);
			this.datumTab.Controls.Add(this.tbElS);
			this.datumTab.Controls.Add(this.label25);
			this.datumTab.Controls.Add(this.label24);
			this.datumTab.Controls.Add(this.tbElAprev);
			this.datumTab.Controls.Add(this.label13);
			this.datumTab.Controls.Add(this.tbElRz);
			this.datumTab.Controls.Add(this.label14);
			this.datumTab.Controls.Add(this.tbElDz);
			this.datumTab.Controls.Add(this.label7);
			this.datumTab.Controls.Add(this.tbElRy);
			this.datumTab.Controls.Add(this.label8);
			this.datumTab.Controls.Add(this.tbElDy);
			this.datumTab.Controls.Add(this.label4);
			this.datumTab.Controls.Add(this.tbElRx);
			this.datumTab.Controls.Add(this.label5);
			this.datumTab.Controls.Add(this.tbElDx);
			this.datumTab.Controls.Add(this.label1);
			this.datumTab.Controls.Add(this.tbElB);
			this.datumTab.Controls.Add(this.lblA);
			this.datumTab.Controls.Add(this.tbElA);
			this.datumTab.Controls.Add(this.tBElName);
			this.datumTab.Controls.Add(this.txtName);
			this.datumTab.Controls.Add(this.comboDatums);
			this.datumTab.Location = new System.Drawing.Point(0, 0);
			this.datumTab.Name = "datumTab";
			this.datumTab.Size = new System.Drawing.Size(230, 146);
			this.datumTab.Text = "Bezugssystem";
			// 
			// bElSave
			// 
			this.bElSave.Location = new System.Drawing.Point(121, 209);
			this.bElSave.Name = "bElSave";
			this.bElSave.Size = new System.Drawing.Size(95, 20);
			this.bElSave.TabIndex = 55;
			this.bElSave.Text = "Speichern";
			this.bElSave.Click += new System.EventHandler(this.bElSave_Click);
			// 
			// tbElS
			// 
			this.tbElS.Location = new System.Drawing.Point(56, 177);
			this.tbElS.Name = "tbElS";
			this.tbElS.Size = new System.Drawing.Size(53, 21);
			this.tbElS.TabIndex = 49;
			// 
			// label25
			// 
			this.label25.Location = new System.Drawing.Point(8, 178);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(54, 20);
			this.label25.Text = "s [ppm]";
			// 
			// label24
			// 
			this.label24.Location = new System.Drawing.Point(121, 179);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(42, 19);
			this.label24.Text = "Code";
			// 
			// tbElAprev
			// 
			this.tbElAprev.Enabled = false;
			this.tbElAprev.Location = new System.Drawing.Point(163, 177);
			this.tbElAprev.Name = "tbElAprev";
			this.tbElAprev.Size = new System.Drawing.Size(53, 21);
			this.tbElAprev.TabIndex = 50;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(121, 152);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(42, 19);
			this.label13.Text = "RZ [\"]";
			// 
			// tbElRz
			// 
			this.tbElRz.Location = new System.Drawing.Point(163, 150);
			this.tbElRz.Name = "tbElRz";
			this.tbElRz.Size = new System.Drawing.Size(53, 21);
			this.tbElRz.TabIndex = 38;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(8, 151);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(42, 20);
			this.label14.Text = "dZ [m]";
			// 
			// tbElDz
			// 
			this.tbElDz.Location = new System.Drawing.Point(56, 150);
			this.tbElDz.Name = "tbElDz";
			this.tbElDz.Size = new System.Drawing.Size(53, 21);
			this.tbElDz.TabIndex = 37;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(121, 125);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(42, 19);
			this.label7.Text = "RY [\"]";
			// 
			// tbElRy
			// 
			this.tbElRy.Location = new System.Drawing.Point(163, 122);
			this.tbElRy.Name = "tbElRy";
			this.tbElRy.Size = new System.Drawing.Size(53, 21);
			this.tbElRy.TabIndex = 20;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(8, 123);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(42, 20);
			this.label8.Text = "dY [m]";
			// 
			// tbElDy
			// 
			this.tbElDy.Location = new System.Drawing.Point(56, 123);
			this.tbElDy.Name = "tbElDy";
			this.tbElDy.Size = new System.Drawing.Size(53, 21);
			this.tbElDy.TabIndex = 19;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(121, 98);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(42, 19);
			this.label4.Text = "RX [\"]";
			// 
			// tbElRx
			// 
			this.tbElRx.Location = new System.Drawing.Point(163, 95);
			this.tbElRx.Name = "tbElRx";
			this.tbElRx.Size = new System.Drawing.Size(53, 21);
			this.tbElRx.TabIndex = 14;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 96);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(42, 20);
			this.label5.Text = "dX [m]";
			// 
			// tbElDx
			// 
			this.tbElDx.Location = new System.Drawing.Point(56, 96);
			this.tbElDx.Name = "tbElDx";
			this.tbElDx.Size = new System.Drawing.Size(53, 21);
			this.tbElDx.TabIndex = 13;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(121, 71);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(42, 19);
			this.label1.Text = "b [m]";
			// 
			// tbElB
			// 
			this.tbElB.Location = new System.Drawing.Point(163, 68);
			this.tbElB.Name = "tbElB";
			this.tbElB.Size = new System.Drawing.Size(53, 21);
			this.tbElB.TabIndex = 8;
			// 
			// lblA
			// 
			this.lblA.Location = new System.Drawing.Point(8, 69);
			this.lblA.Name = "lblA";
			this.lblA.Size = new System.Drawing.Size(42, 20);
			this.lblA.Text = "a [m]";
			// 
			// tbElA
			// 
			this.tbElA.Location = new System.Drawing.Point(56, 69);
			this.tbElA.Name = "tbElA";
			this.tbElA.Size = new System.Drawing.Size(53, 21);
			this.tbElA.TabIndex = 4;
			// 
			// tBElName
			// 
			this.tBElName.Location = new System.Drawing.Point(56, 42);
			this.tBElName.Name = "tBElName";
			this.tBElName.Size = new System.Drawing.Size(160, 21);
			this.tBElName.TabIndex = 2;
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(8, 43);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(42, 20);
			this.txtName.Text = "Name:";
			// 
			// comboDatums
			// 
			this.comboDatums.Location = new System.Drawing.Point(8, 8);
			this.comboDatums.Name = "comboDatums";
			this.comboDatums.Size = new System.Drawing.Size(208, 22);
			this.comboDatums.TabIndex = 0;
			// 
			// miscTab
			// 
			this.miscTab.AutoScroll = true;
			this.miscTab.AutoScrollMargin = new System.Drawing.Size(0, 10);
			this.miscTab.Controls.Add(this.cbKeyMappingsDown);
			this.miscTab.Controls.Add(lKeyMappingsDown);
			this.miscTab.Controls.Add(this.cbKeyMappingsUp);
			this.miscTab.Controls.Add(this.lKeyMappingsUp);
			this.miscTab.Controls.Add(this.cbKeyMappingsEnter);
			this.miscTab.Controls.Add(lKeyMappingsEnter);
			this.miscTab.Controls.Add(this.cbKeyMappingsRight);
			this.miscTab.Controls.Add(lKeyMappingsRight);
			this.miscTab.Controls.Add(this.cbKeyMappingsLeft);
			this.miscTab.Controls.Add(this.label28);
			this.miscTab.Controls.Add(this.lKeyMappings);
			this.miscTab.Controls.Add(this.cbShowNorthArrow);
			this.miscTab.Controls.Add(this.ciNewLayerColor);
			this.miscTab.Controls.Add(this.newLayerSpecificColor);
			this.miscTab.Controls.Add(this.newLayerRandomColor);
			this.miscTab.Controls.Add(this.label26);
			this.miscTab.Controls.Add(this.cbShowScaleOnMap);
			this.miscTab.Controls.Add(this.sesstingsSpecialLayers);
			this.miscTab.Location = new System.Drawing.Point(0, 0);
			this.miscTab.Name = "miscTab";
			this.miscTab.Size = new System.Drawing.Size(230, 146);
			this.miscTab.Text = "Verschiedenes";
			// 
			// cbKeyMappingsDown
			// 
			this.cbKeyMappingsDown.Location = new System.Drawing.Point(89, 205);
			this.cbKeyMappingsDown.Name = "cbKeyMappingsDown";
			this.cbKeyMappingsDown.Size = new System.Drawing.Size(133, 22);
			this.cbKeyMappingsDown.TabIndex = 23;
			// 
			// cbKeyMappingsUp
			// 
			this.cbKeyMappingsUp.Location = new System.Drawing.Point(89, 177);
			this.cbKeyMappingsUp.Name = "cbKeyMappingsUp";
			this.cbKeyMappingsUp.Size = new System.Drawing.Size(133, 22);
			this.cbKeyMappingsUp.TabIndex = 22;
			// 
			// lKeyMappingsUp
			// 
			this.lKeyMappingsUp.Location = new System.Drawing.Point(22, 179);
			this.lKeyMappingsUp.Name = "lKeyMappingsUp";
			this.lKeyMappingsUp.Size = new System.Drawing.Size(39, 20);
			this.lKeyMappingsUp.Text = "Hoch";
			// 
			// cbKeyMappingsEnter
			// 
			this.cbKeyMappingsEnter.Location = new System.Drawing.Point(89, 288);
			this.cbKeyMappingsEnter.Name = "cbKeyMappingsEnter";
			this.cbKeyMappingsEnter.Size = new System.Drawing.Size(133, 22);
			this.cbKeyMappingsEnter.TabIndex = 18;
			// 
			// cbKeyMappingsRight
			// 
			this.cbKeyMappingsRight.Location = new System.Drawing.Point(89, 260);
			this.cbKeyMappingsRight.Name = "cbKeyMappingsRight";
			this.cbKeyMappingsRight.Size = new System.Drawing.Size(133, 22);
			this.cbKeyMappingsRight.TabIndex = 15;
			// 
			// cbKeyMappingsLeft
			// 
			this.cbKeyMappingsLeft.Location = new System.Drawing.Point(89, 232);
			this.cbKeyMappingsLeft.Name = "cbKeyMappingsLeft";
			this.cbKeyMappingsLeft.Size = new System.Drawing.Size(133, 22);
			this.cbKeyMappingsLeft.TabIndex = 13;
			// 
			// label28
			// 
			this.label28.Location = new System.Drawing.Point(22, 234);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(39, 20);
			this.label28.Text = "Links";
			// 
			// lKeyMappings
			// 
			this.lKeyMappings.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
			this.lKeyMappings.Location = new System.Drawing.Point(7, 151);
			this.lKeyMappings.Name = "lKeyMappings";
			this.lKeyMappings.Size = new System.Drawing.Size(115, 16);
			this.lKeyMappings.Text = "Tastenbelegung";
			// 
			// cbShowNorthArrow
			// 
			this.cbShowNorthArrow.Location = new System.Drawing.Point(7, 53);
			this.cbShowNorthArrow.Name = "cbShowNorthArrow";
			this.cbShowNorthArrow.Size = new System.Drawing.Size(187, 20);
			this.cbShowNorthArrow.TabIndex = 9;
			this.cbShowNorthArrow.Text = "Nordpfeil anzeigen";
			// 
			// ciNewLayerColor
			// 
			this.ciNewLayerColor.BackColor = System.Drawing.SystemColors.InactiveBorder;
			this.ciNewLayerColor.Location = new System.Drawing.Point(174, 121);
			this.ciNewLayerColor.Name = "ciNewLayerColor";
			this.ciNewLayerColor.Size = new System.Drawing.Size(15, 15);
			this.ciNewLayerColor.TabIndex = 7;
			this.ciNewLayerColor.Text = "newLayerColor";
			this.ciNewLayerColor.Click += new System.EventHandler(this.colorIcon1_Click);
			// 
			// newLayerSpecificColor
			// 
			this.newLayerSpecificColor.Location = new System.Drawing.Point(22, 119);
			this.newLayerSpecificColor.Name = "newLayerSpecificColor";
			this.newLayerSpecificColor.Size = new System.Drawing.Size(146, 20);
			this.newLayerSpecificColor.TabIndex = 6;
			this.newLayerSpecificColor.Text = "Voreingestellte Farbe:";
			// 
			// newLayerRandomColor
			// 
			this.newLayerRandomColor.Location = new System.Drawing.Point(22, 96);
			this.newLayerRandomColor.Name = "newLayerRandomColor";
			this.newLayerRandomColor.Size = new System.Drawing.Size(100, 20);
			this.newLayerRandomColor.TabIndex = 5;
			this.newLayerRandomColor.Text = "Zufallsfarbe";
			// 
			// label26
			// 
			this.label26.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
			this.label26.Location = new System.Drawing.Point(7, 76);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(145, 20);
			this.label26.Text = "Farbe für neue Layer";
			// 
			// cbShowScaleOnMap
			// 
			this.cbShowScaleOnMap.Location = new System.Drawing.Point(7, 30);
			this.cbShowScaleOnMap.Name = "cbShowScaleOnMap";
			this.cbShowScaleOnMap.Size = new System.Drawing.Size(187, 20);
			this.cbShowScaleOnMap.TabIndex = 3;
			this.cbShowScaleOnMap.Text = "Maßstab auf Karte anzeigen";
			// 
			// sesstingsSpecialLayers
			// 
			this.sesstingsSpecialLayers.Checked = true;
			this.sesstingsSpecialLayers.CheckState = System.Windows.Forms.CheckState.Checked;
			this.sesstingsSpecialLayers.Location = new System.Drawing.Point(7, 7);
			this.sesstingsSpecialLayers.Name = "sesstingsSpecialLayers";
			this.sesstingsSpecialLayers.Size = new System.Drawing.Size(197, 21);
			this.sesstingsSpecialLayers.TabIndex = 2;
			this.sesstingsSpecialLayers.Text = "Mandelbrot-Testlayer nutzbar";
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
			this.Caption.Text = "Einstellungen bearbeiten";
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(121, 146);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(42, 19);
			this.label15.Text = "RZ [\"]";
			// 
			// textBox10
			// 
			this.textBox10.Location = new System.Drawing.Point(163, 144);
			this.textBox10.Name = "textBox10";
			this.textBox10.Size = new System.Drawing.Size(53, 21);
			this.textBox10.TabIndex = 38;
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(8, 145);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(42, 20);
			this.label16.Text = "dZ [m]";
			// 
			// textBox11
			// 
			this.textBox11.Location = new System.Drawing.Point(56, 144);
			this.textBox11.Name = "textBox11";
			this.textBox11.Size = new System.Drawing.Size(53, 21);
			this.textBox11.TabIndex = 37;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(121, 119);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(42, 19);
			this.label17.Text = "RY [\"]";
			// 
			// textBox12
			// 
			this.textBox12.Location = new System.Drawing.Point(163, 116);
			this.textBox12.Name = "textBox12";
			this.textBox12.Size = new System.Drawing.Size(53, 21);
			this.textBox12.TabIndex = 20;
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(8, 117);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(42, 20);
			this.label18.Text = "dY [m]";
			// 
			// textBox13
			// 
			this.textBox13.Location = new System.Drawing.Point(56, 117);
			this.textBox13.Name = "textBox13";
			this.textBox13.Size = new System.Drawing.Size(53, 21);
			this.textBox13.TabIndex = 19;
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(121, 92);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(42, 19);
			this.label19.Text = "RX [\"]";
			// 
			// textBox14
			// 
			this.textBox14.Location = new System.Drawing.Point(163, 89);
			this.textBox14.Name = "textBox14";
			this.textBox14.Size = new System.Drawing.Size(53, 21);
			this.textBox14.TabIndex = 14;
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(8, 90);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(42, 20);
			this.label20.Text = "dX [m]";
			// 
			// textBox15
			// 
			this.textBox15.Location = new System.Drawing.Point(56, 90);
			this.textBox15.Name = "textBox15";
			this.textBox15.Size = new System.Drawing.Size(53, 21);
			this.textBox15.TabIndex = 13;
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(121, 65);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(42, 19);
			this.label21.Text = "b [m]";
			// 
			// textBox16
			// 
			this.textBox16.Location = new System.Drawing.Point(163, 62);
			this.textBox16.Name = "textBox16";
			this.textBox16.Size = new System.Drawing.Size(53, 21);
			this.textBox16.TabIndex = 8;
			// 
			// label22
			// 
			this.label22.Location = new System.Drawing.Point(8, 63);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(42, 20);
			this.label22.Text = "a [m]";
			// 
			// textBox17
			// 
			this.textBox17.Location = new System.Drawing.Point(56, 63);
			this.textBox17.Name = "textBox17";
			this.textBox17.Size = new System.Drawing.Size(53, 21);
			this.textBox17.TabIndex = 4;
			// 
			// textBox18
			// 
			this.textBox18.Location = new System.Drawing.Point(56, 36);
			this.textBox18.Name = "textBox18";
			this.textBox18.Size = new System.Drawing.Size(160, 21);
			this.textBox18.TabIndex = 2;
			// 
			// label23
			// 
			this.label23.Location = new System.Drawing.Point(8, 37);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(42, 20);
			this.label23.Text = "Name:";
			// 
			// comboBox2
			// 
			this.comboBox2.Location = new System.Drawing.Point(8, 8);
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Size = new System.Drawing.Size(208, 22);
			this.comboBox2.TabIndex = 0;
			// 
			// SettingDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.ClientSize = new System.Drawing.Size(240, 294);
			this.ControlBox = false;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.Caption);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.KeyPreview = true;
			this.Location = new System.Drawing.Point(0, 0);
			this.MinimizeBox = false;
			this.Name = "SettingDialog";
			this.TopMost = true;
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseDown);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseUp);
			this.panel1.ResumeLayout(false);
			this.layerTabControl.ResumeLayout(false);
			this.generalTab.ResumeLayout(false);
			this.behaviourTab.ResumeLayout(false);
			this.exchangeTab.ResumeLayout(false);
			this.gpsTab.ResumeLayout(false);
			this.coordTab.ResumeLayout(false);
			this.datumTab.ResumeLayout(false);
			this.miscTab.ResumeLayout(false);
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
            if (isMoveable)
            {
                Location = new Point(Location.X + e.X - Xdif, Location.Y + e.Y - Ydif);
            }
        }
        public DialogResult ShowDialog(Config config)
        {
            fillDialogWithData(config);
            this.BringToFront();
            return base.ShowDialog();
        }

        private void fillDialogWithData(Config config)
        {
            this.config = config;

            this.sesstingsSpecialLayers.Checked = config.ShowSpecialLayers;

            settingDefaultProjectTB.Text = config.DefaultProject;

            settingsTrasferPointName.Text = config.ExPntLayerFile;
            settingsTrasferPolyLineName.Text = config.ExPLineLayerFile;
            settingsTrasferPolygonName.Text = config.ExPGonLayerFile;
            settingsSearchDirListbox.Items.Clear();
            checkBox1.Checked = config.UseDefaultProject;

            for (int i = 0; i < config.SearchDirList.Count; i++)
            {
                settingsSearchDirListbox.Items.Add(config.SearchDirList[i]);
            }
            zoomInPercent.Value = Math.Round(Convert.ToDecimal(100 * config.ZoomInFactor), 2);
            zoomOutPercent.Value = Math.Round(Convert.ToDecimal(100 * config.ZoomOutFactor), 2);
            this.ShowGPSPositionCB.Checked = config.UseGPS;
            this.gpsWidth.Value = config.GpsLayerPointWidth;
            this.gpsColor.BackColor = config.GpsLayerPointColor;
            
			if (config.GpsViewMode == GpsView.StaticMap)
				this.gpsViewSelection.SelectedIndex = 0;
			else
				this.gpsViewSelection.SelectedIndex = 1;
			
			nGPSUpdateInterval.Value = Convert.ToUInt32( Math.Min( config.GPSUpdateInterval / 1000.0,
					Convert.ToDouble(nGPSUpdateInterval.Maximum) ) );
            
            this.cbGPSTrackingActivate.Checked = config.GPSTracking;
            
            if (config.GPSTrackingStartupMode == GpsTrackingStartingMode.AfterKeyPress)
				this.cbGPSTrackingStartMode.SelectedIndex = 0;
			else
				this.cbGPSTrackingStartMode.SelectedIndex = 1;
			
            this.tbGPSTrackingComment.Text = config.GPSTrackingComment;
            this.tbGPSTrackingTimeInterval.Text = config.GPSTrackingTimeInterval.ToString();
            this.rbGPSTrackingTriggerKey.Checked = (config.GPSTrackingTriggerMode == GpsTrackingTriggerMode.KeyPress);
            this.rbGPSTrackingTriggerTime.Checked = ! this.rbGPSTrackingTriggerKey.Checked;
            this.cbGPSTrackingDiscardSamePos.Checked = config.GPSTrackingDiscardSamePositions;
            
            CheckGpsToolsEnabled();

			SortedList<String, HardwareKeyMappings> mappingTools = new SortedList<String, HardwareKeyMappings>();
			mappingTools.Add("Keine Funktion", HardwareKeyMappings.None);
			mappingTools.Add("Verschieben (Temp.)", HardwareKeyMappings.TempMove);
			mappingTools.Add("Starte GPS-Tracking", HardwareKeyMappings.GPSTrackingTrigger);
			mappingTools.Add("Stoppe GPS-Tracking", HardwareKeyMappings.GPSTrackingStop);
			
			
			BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = mappingTools;
			cbKeyMappingsUp.DisplayMember = "Key";
			cbKeyMappingsUp.DataSource = bindingSource;
			cbKeyMappingsUp.ValueMember = "Value";
			cbKeyMappingsUp.SelectedValue = config.KeyMapping_Up;

			BindingSource bindingSource2 = new BindingSource();
			bindingSource2.DataSource = mappingTools;
			cbKeyMappingsDown.DisplayMember = "Key";
			cbKeyMappingsDown.DataSource = bindingSource2;
			cbKeyMappingsDown.ValueMember = "Value";
			cbKeyMappingsDown.SelectedValue = config.KeyMapping_Down;

			BindingSource bindingSource3 = new BindingSource();
			bindingSource3.DataSource = mappingTools;
			cbKeyMappingsLeft.DisplayMember = "Key";
			cbKeyMappingsLeft.DataSource = bindingSource3;
			cbKeyMappingsLeft.ValueMember = "Value";
			cbKeyMappingsLeft.SelectedValue = config.KeyMapping_Left;

			BindingSource bindingSource4 = new BindingSource();
			bindingSource4.DataSource = mappingTools;
			cbKeyMappingsRight.DisplayMember = "Key";
			cbKeyMappingsRight.DataSource = bindingSource4;
			cbKeyMappingsRight.ValueMember = "Value";
			cbKeyMappingsRight.SelectedValue = config.KeyMapping_Right;

			BindingSource bindingSource5 = new BindingSource();
			bindingSource5.DataSource = mappingTools;
			cbKeyMappingsEnter.DisplayMember = "Key";
			cbKeyMappingsEnter.DataSource = bindingSource5;
			cbKeyMappingsEnter.ValueMember = "Value";
			cbKeyMappingsEnter.SelectedValue = config.KeyMapping_Enter;
			

            this.coordGKStripe.Value = config.CoordGaußKruegerStripe;
            sesstingsSpecialLayers.Checked = config.ShowSpecialLayers;

            if (!initialized)
            {
                // Add at least the datum the user chose the last time
                if (config.Datum != null && config.Datum.Alias != "USERDEFINED") addDatum(config.Datum);

                // Add some often used Datums manually:
                addDatum(HorizontalDatum.WGS84);
                addDatum(HorizontalDatum.Bessel1841);

                // Read the reference ellipses and datums from file
                parseDatumFiles();
                datums.Sort(delegate(HorizontalDatum datum1, HorizontalDatum datum2)
                { return String.Compare(datum1.Alias, datum2.Alias); });

                // Add a changable datum:
                if (config.UserDatum != null) addDatum(config.UserDatum);
                else addDatum(new HorizontalDatum(Ellipsoid.Bessel, new Wgs84ConversionInfo(),
                    DatumType.HD_Geocentric, "Benutzerdefiniert",
                    String.Empty, 0, "Benutzerdefiniert", "USERDEFINED", String.Empty));

                updateDatumComboBox();
                comboDatums.SelectedIndexChanged += new EventHandler(comboEllipsoids_SelectedIndexChanged);
                initialized = true;
            }

            if (comboDatums.Items.Count > 0)
            {
                if (config.Datum != null)
                    comboDatums.SelectedItem = config.Datum;
                else
                    comboDatums.SelectedIndex = 0;

                updateDatumFields();
            }

            cbShowScaleOnMap.Checked = config.ShowDistanceLine;
            cbShowNorthArrow.Checked = config.ShowNorthArrow;

            switch (config.NewLayerStyle)
            {
                case GravurGIS.Styles.NewLayerStyle.NewLayerStyles.SpecificColor:
                    newLayerSpecificColor.Checked = true;
                    break;
                default: // Random Color
                    newLayerRandomColor.Checked = true;
                    break;
            }
            ciNewLayerColor.BackColor = config.NewLayerStaticColor;

            lbCategories.Items.Clear();

            for (int i = 0; i < config.CategoryList.Count; i++)
            {
                lbCategories.Items.Add(config.CategoryList[i]);
            }

            maxSeletClickDist.Value = config.PxMaxSelectDistance;


        }

        private void addDatum(HorizontalDatum datum)
        {
            if (!datumsHashTable.ContainsKey(datum.Alias))
            {
                datumsHashTable.Add(datum.Alias, 0);
                datums.Add(datum);
            }
        }


        void comboEllipsoids_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateDatumFields();
        }

        private void updateDatumComboBox()
        {
            // TODO: Sort referenceEllipsoids alphabetically

            comboDatums.DataSource = datums;
            comboDatums.ValueMember = "Alias";
            comboDatums.DisplayMember = "Alias";

            //comboEllipsoids.da

            //foreach (KeyValuePair<string, RefEllipsoid> ellipsoid in referenceEllipsoids)
            //{
            //    comboEllipsoids.ValueMember
            //    comboEllipsoids.Items.Add(String.Format("{0} - {1}", ellipsoid.Key, ellipsoid.Value.Name));
            //}
            //if (comboEllipsoids.Items.Count > 0)
            //{
            //    comboEllipsoids.SelectedIndex[0];
            //}
        }

        private void updateDatumFields()
        {
            HorizontalDatum datum = comboDatums.SelectedItem as HorizontalDatum;

            if (datum != null)
            {

                tBElName.Text = datum.Name;
                tbElAprev.Text = datum.Abbreviation;
                tbElA.Text = datum.Ellipsoid.SemiMajorAxis.ToString();
                tbElB.Text = datum.Ellipsoid.SemiMinorAxis.ToString();
                if (datum.Wgs84Parameters != null)
                {
                    tbElDx.Text = datum.Wgs84Parameters.Dx.ToString();
                    tbElDy.Text = datum.Wgs84Parameters.Dy.ToString();
                    tbElDz.Text = datum.Wgs84Parameters.Dz.ToString();
                    tbElRx.Text = datum.Wgs84Parameters.Ex.ToString();
                    tbElRy.Text = datum.Wgs84Parameters.Ey.ToString();
                    tbElRz.Text = datum.Wgs84Parameters.Ez.ToString();
                    tbElS.Text = datum.Wgs84Parameters.Ppm.ToString();
                }
                else
                {
                    tbElDx.Text = tbElDy.Text = tbElDz.Text = tbElRx.Text
                    = tbElRy.Text = tbElRz.Text = tbElS.Text = "0";
                }
                tbElAprev.Text = datum.Abbreviation;

                this.ElFields = bElSave.Visible = (datum.Abbreviation == "USERDEFINED");

            }
            else
            {
                tBElName.Text = "Fehler: Daten nicht lesbar!";
                tbElA.Text = tbElAprev.Text = tbElB.Text
                    = tbElDx.Text = tbElDy.Text = tbElDz.Text = tbElRx.Text
                    = tbElRy.Text = tbElRz.Text = tbElS.Text = tbElAprev.Text
                    = "";
                this.ElFields = false;
                bElSave.Visible = false;
            }
        }

        private void parseDatumFiles()
        {
            string filename = null;
            string line = null;
            string[] parts = null;
            StreamReader sr = null;
            FileStream file = null;
            char[] chSplit = { ',' };

            System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.CurrentCulture;
            System.Globalization.NumberFormatInfo ni = null;
            ni = (System.Globalization.NumberFormatInfo)ci.NumberFormat.Clone();
            ni.NumberDecimalSeparator = ".";

            for (int i = config.EllipsoidFilList.Count - 1; i >= 0; i--)
            {
                filename = String.Format("{0}//{1}", config.ApplicationDirectory, config.EllipsoidFilList[i]);
                if (File.Exists(filename))
                {
                    // test if there are valid ellipsoid information in and add them to our map
                    try
                    {
                        file = new FileStream(filename, FileMode.Open, FileAccess.Read);
                        sr = new StreamReader(file);

                        while ((line = sr.ReadLine()) != null)
                        {
                            // the file is organized in this way: NAME,CODE,A,B,RF
                            // A and B are double precission numbers with a decimal point "."

                            try
                            {
                                parts = line.Split(chSplit);
                                referenceEllipsoids.Add(parts[1].Trim(),
                                    new Ellipsoid(Double.Parse(parts[2], ni),
                                         Double.Parse(parts[3], ni),
                                          Double.Parse(parts[4], ni),
                                          false, LinearUnit.Metre,
                                          parts[0].Trim(),
                                          String.Empty,
                                          0,
                                          parts[0].Trim(),
                                          parts[1].Trim(),
                                          String.Empty));
                            }
                            catch
                            {
                                // just ignore the wrong line and keep on
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // we ignore the errors, clean up and move on to the next file
                        // if data has been inserted into the reference layers dicronary we will use it
                    }
                    finally
                    {
                        if (sr != null) sr.Close();
                        if (file != null) file.Close();
                    }

                }
            }

            // parse datum files
            Ellipsoid el = null;
            String tempStr = String.Empty;
            HorizontalDatum datum;

            for (int i = config.DatumFileList.Count - 1; i >= 0; i--)
            {
                filename = String.Format("{0}\\{1}", config.ApplicationDirectory, config.DatumFileList[i]);
                if (File.Exists(filename))
                {
                    // test if there is valid datum information in and add them to our dictionary
                    try
                    {
                        file = new FileStream(filename, FileMode.Open, FileAccess.Read);
                        sr = new StreamReader(file);

                        while ((line = sr.ReadLine()) != null)
                        {
                            // the file is organized in this way: CODE,NAME,ELLIPSOID,DELTAX,SIGMAX,DELTAY,SIGMAY,DELTAZ,SIGMAZ,ROTX,ROTY,ROTZ,SCALE
                            // A and B are double precission numbers with a decimal point "."

                            try
                            {
                                parts = splitEx(line, chSplit, new char[] { '\"' });
                                if (referenceEllipsoids.TryGetValue(parts[2].Trim(), out el))
                                {
                                    tempStr = parts[0].Trim();

                                    datum = new HorizontalDatum(el,
                                        new Wgs84ConversionInfo(
                                            Double.Parse(parts[3].Trim()),
                                            Double.Parse(parts[5].Trim()),
                                            Double.Parse(parts[7].Trim()),
                                            Double.Parse(parts[9].Trim()),
                                            Double.Parse(parts[10].Trim()),
                                            Double.Parse(parts[11].Trim()),
                                            Double.Parse(parts[12].Trim())),
                                        DatumType.HD_Geocentric,
                                        parts[1].Trim('\"'),
                                        String.Empty,
                                        0,
                                        String.Empty,
                                        tempStr,
                                        String.Empty);

                                    addDatum(datum);
                                }

                            }
                            catch
                            {
                                // just ignore the wrong line and keep going
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // we ignore the errors, clean up and move on to the next file
                        // if data has been inserted into the reference layers dicronary we will use it
                    }
                    finally
                    {
                        datum = null;
                        if (sr != null) sr.Close();
                        if (file != null) file.Close();
                    }

                }
            }
        }

        private string[] splitEx(string source, char[] separator, char[] enclosure)
        {
            string[] outer_parts = source.Split(enclosure);
            if (outer_parts.Length > 1)
            {
                List<string> result = new List<string>();

                result.AddRange((outer_parts[0].Trim(separator)).Split(separator));
                result.Add(outer_parts[1].Trim());
                result.AddRange((outer_parts[2].Trim(separator)).Split(separator));
                return result.ToArray();
            }

            return outer_parts;
        }

        public void saveSettingsToModel()
        {
            try
            {
                config.ExPGonLayerFile = settingsTrasferPolygonName.Text;
                config.ExPLineLayerFile = settingsTrasferPolyLineName.Text;
                config.ExPntLayerFile = settingsTrasferPointName.Text;
                config.DefaultProject = settingDefaultProjectTB.Text;
                config.ZoomInFactor = Math.Round(Convert.ToDouble(zoomInPercent.Value / 100), 2);
                config.ZoomOutFactor = Math.Round(Convert.ToDouble(zoomOutPercent.Value / 100), 2);
                config.UseDefaultProject = checkBox1.Checked;
                config.UseGPS = this.ShowGPSPositionCB.Checked;

                config.GpsLayerPointWidth = Convert.ToInt32(this.gpsWidth.Value);
                config.GpsLayerPointColor = this.gpsColor.BackColor;

                config.CoordGaußKruegerStripe = Decimal.ToInt32(coordGKStripe.Value);
                config.ShowSpecialLayers = sesstingsSpecialLayers.Checked;
                //config.SelectedEllispoidIndex = refEllipsoids.SelectedIndex;

                for (int i = 0; i < settingsSearchDirListbox.Items.Count; i++)
                {
                    config.SearchDirList.Clear();
                    config.SearchDirList.Add((string)(settingsSearchDirListbox.Items[i]));
                }

                HorizontalDatum datum = comboDatums.SelectedItem as HorizontalDatum;
                if (datum == null)
                    config.Datum = HorizontalDatum.Bessel1841;
                else
                    config.Datum = datum;

                config.ShowDistanceLine = cbShowScaleOnMap.Checked;
                config.ShowNorthArrow = cbShowNorthArrow.Checked;

                if (newLayerRandomColor.Checked)
                    config.NewLayerStyle = GravurGIS.Styles.NewLayerStyle.NewLayerStyles.RandomColor;
                else
                {
                    config.NewLayerStyle = GravurGIS.Styles.NewLayerStyle.NewLayerStyles.SpecificColor;
                    config.NewLayerStaticColor = ciNewLayerColor.BackColor;
                }

                config.CategoryList.Clear();
                for (int i = 0; i < lbCategories.Items.Count; i++)
                {
                    config.CategoryList.Add(lbCategories.Items[i].ToString());
                }

                if (gpsViewSelection.SelectedIndex == 0)
                    config.GpsViewMode = GpsView.StaticMap;
                else config.GpsViewMode = GpsView.StaticPoint; //selected Item: Kartenausschnitt beibehalten

				if (nGPSUpdateInterval.Value >= 0) config.GPSUpdateInterval = Convert.ToUInt32( nGPSUpdateInterval.Value * 1000 );
                
                // GPS Tracking

				config.GPSTracking = this.cbGPSTrackingActivate.Checked;

				if (this.cbGPSTrackingStartMode.SelectedIndex == 0)
					config.GPSTrackingStartupMode = GpsTrackingStartingMode.AfterKeyPress;
				else
					config.GPSTrackingStartupMode = GpsTrackingStartingMode.AfterDialogClose;

				config.GPSTrackingComment = this.tbGPSTrackingComment.Text;
				
				config.GPSTrackingTimeInterval = Int32.Parse(this.tbGPSTrackingTimeInterval.Text);
				
				if (this.rbGPSTrackingTriggerKey.Checked)
					config.GPSTrackingTriggerMode = GpsTrackingTriggerMode.KeyPress;
				else
					config.GPSTrackingTriggerMode = GpsTrackingTriggerMode.TimeInterval;
				
				config.GPSTrackingDiscardSamePositions = cbGPSTrackingDiscardSamePos.Checked;	

                config.PxMaxSelectDistance = Convert.ToInt32(maxSeletClickDist.Value);

				config.KeyMapping_Up = (cbKeyMappingsUp.SelectedValue is HardwareKeyMappings) ? (HardwareKeyMappings)cbKeyMappingsUp.SelectedValue : HardwareKeyMappings.None;
				config.KeyMapping_Down = (cbKeyMappingsDown.SelectedValue is HardwareKeyMappings) ? (HardwareKeyMappings)cbKeyMappingsDown.SelectedValue : HardwareKeyMappings.None;
				config.KeyMapping_Left = (cbKeyMappingsLeft.SelectedValue is HardwareKeyMappings) ? (HardwareKeyMappings)cbKeyMappingsLeft.SelectedValue : HardwareKeyMappings.None;
				config.KeyMapping_Right = (cbKeyMappingsRight.SelectedValue is HardwareKeyMappings) ? (HardwareKeyMappings)cbKeyMappingsRight.SelectedValue : HardwareKeyMappings.None;
				config.KeyMapping_Enter = (cbKeyMappingsEnter.SelectedValue is HardwareKeyMappings) ? (HardwareKeyMappings)cbKeyMappingsEnter.SelectedValue : HardwareKeyMappings.None;
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format("Fehlerhafte Eingabe, bitte korrigieren Sie die eingegebenen Daten!{0}Fehler: {1}", Environment.NewLine, e.Message), "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        protected override void toolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button.Equals(tbOkButton))
            {
                try
                {
                    saveSettingsToModel();
                    
                }
                catch
                {
                    MessageBox.Show(
                        String.Format(
                        "Fehler 0x1221: Es ist ein Fehler beim Speichern der Einstellungen aufgetreten!{0}Bitte kontrollieren Sie Ihre Eingaben.", Environment.NewLine));
                    return;
                }
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

        private void checkBox1_CheckStateChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                stOpenIconBox.Enabled = true;
                settingDefaultProjectTB.Enabled = true;
            }
            else
            {
                stOpenIconBox.Enabled = false;
                settingDefaultProjectTB.Enabled = false;
            }
        }

        private void stOpenIconBox_Click_1(object sender, EventArgs e)
        {
            this.settingDefaultProjectTB.Text = this.mainControler.askForDefaultLayer();
        }

        private void addDirIconBox_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog(mainControler);
            if (folderBrowser.ShowDialog() == DialogResult.OK)
                settingsSearchDirListbox.Items.Add(folderBrowser.SelectedPath);
        }

        private void removeDirIconBox_Click_1(object sender, EventArgs e)
        {
            int index = settingsSearchDirListbox.SelectedIndex;

            if (index >= 0 && index < settingsSearchDirListbox.Items.Count)
                settingsSearchDirListbox.Items.Remove(settingsSearchDirListbox.Items[index]);
        }

        private void PointLayerSettings_Click(object sender, EventArgs e)
        {
            mainControler.showLayerLayerSettingsEditor(-1, LayerType.PointCanvas);
        }

        private void PolylineLayerSettings_Click(object sender, EventArgs e)
        {
            mainControler.showLayerLayerSettingsEditor(-1, LayerType.PolylineCanvas);
        }

        private void PolygonLayerSettings_Click(object sender, EventArgs e)
        {
            mainControler.showLayerLayerSettingsEditor(-1, LayerType.PolygonCanvas);
        }

        private void coordCheck_CheckedChanged(object sender, EventArgs e)
        {
            btnConfigOtherProj.Enabled = chkCoordOther.Checked;
            coordGKStripe.Enabled = chkCoordGK.Checked;
        }

        private bool ElFields
        {
            set
            {
                tBElName.Enabled = tbElA.Enabled = tbElB.Enabled
                    = tbElDx.Enabled = tbElDy.Enabled = tbElDz.Enabled = tbElRx.Enabled
                    = tbElRy.Enabled = tbElRz.Enabled = tbElS.Enabled
                    = value;
            }
        }

        private void bElSave_Click(object sender, EventArgs e)
        {
            HorizontalDatum datum = comboDatums.SelectedItem as HorizontalDatum;

            try
            {
                datum.Name = tBElName.Text;
                datum.Ellipsoid.SemiMajorAxis = Double.Parse(tbElA.Text);
                datum.Ellipsoid.SemiMinorAxis = Double.Parse(tbElB.Text);

                Wgs84ConversionInfo wgsInfo =
                    new Wgs84ConversionInfo(
                    Double.Parse(tbElDx.Text),
                    Double.Parse(tbElDy.Text),
                    Double.Parse(tbElDz.Text),
                    Double.Parse(tbElRx.Text),
                    Double.Parse(tbElRy.Text),
                    Double.Parse(tbElRz.Text),
                    Double.Parse(tbElS.Text));

                config.UserDatum = datum;
            }
            catch (Exception)
            {
                MessageBox.Show("Fehler: Bitte überprüfen Sie Ihre Eingaben!");
            }
        }

        private void gpsColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog(this.Handle);
            if (colorDialog.ShowDialog() == DialogResult.OK)
                    gpsColor.BackColor = colorDialog.Color;
        }

        private void ShowGPSPositionCB_CheckStateChanged(object sender, EventArgs e)
        {
            CheckGpsToolsEnabled();
        }

        private void CheckGpsToolsEnabled()
        {
            lblGpsColor.Enabled = lblGpsWidth.Enabled
                = gpsColor.Enabled = gpsWidth.Enabled
                = lGPSViewMode.Enabled = gpsViewSelection.Enabled
                = lGPSUpdateInterval.Enabled = nGPSUpdateInterval.Enabled
                = cbGPSTrackingActivate.Enabled
                = ShowGPSPositionCB.Checked;

			CheckGpsTrackingEnabled();
        }
        
        private void CheckGpsTrackingEnabled() {
			lGPSTrackingStartMode.Enabled = lGPSTrackingComment.Enabled
				= lGPSTriggerTimer.Enabled = cbGPSTrackingStartMode.Enabled
				= rbGPSTrackingTriggerKey.Enabled = lTrigger.Enabled
				= tbGPSTrackingComment.Enabled = tbGPSTrackingTimeInterval.Enabled
				= rbGPSTrackingTriggerTime.Enabled = cbGPSTrackingDiscardSamePos.Enabled
				= cbGPSTrackingActivate.Checked && ShowGPSPositionCB.Checked;
        }

        private void TransPortLayerPath_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                try
                {
                    if (sender == PointLayerFile)
                        saveDialog.FileName = Path.GetFileNameWithoutExtension(settingsTrasferPointName.Text);
                    else if (sender == PolyLineLayerFile)
                        saveDialog.FileName = Path.GetFileNameWithoutExtension(settingsTrasferPolyLineName.Text);
                    else if (sender == PolygonLayerFile)
                        saveDialog.FileName = Path.GetFileNameWithoutExtension(settingsTrasferPolygonName.Text);
                }
                catch (Exception)
                {
                    saveDialog.FileName = "";
                }

                saveDialog.Filter = "ESRI-Shapedatei (*.shp)|*.shp";
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    if (sender == PointLayerFile)
                        settingsTrasferPointName.Text = saveDialog.FileName;
                    else if (sender == PolyLineLayerFile)
                        settingsTrasferPolyLineName.Text = saveDialog.FileName;
                    else if (sender == PolygonLayerFile)
                        settingsTrasferPolygonName.Text = saveDialog.FileName;
                }
            }
        }

        private void colorIcon1_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog(this.Handle);
            if (colorDialog.ShowDialog() == DialogResult.OK)
                ciNewLayerColor.BackColor = colorDialog.Color;
            colorDialog.Dispose();
        }

        private void bAddCategory_Click(object sender, EventArgs e)
        {
            String result = mainControler.GetUserInput("Neue Kategorie",
                "Bitte geben Sie hier eine neue Kategorie ein:");

            if (result != String.Empty && !lbCategories.Items.Contains(result))
            {
                this.lbCategories.Items.Add(result);
            }
        }

        private void bRemoveCategory_Click(object sender, EventArgs e)
        {
            if (lbCategories.SelectedIndex >=0 )
                lbCategories.Items.RemoveAt(lbCategories.SelectedIndex);
        }

		private void cbGPSTrackingActivate_CheckStateChanged(object sender, EventArgs e)
		{
			CheckGpsTrackingEnabled();
		}
    }
}
