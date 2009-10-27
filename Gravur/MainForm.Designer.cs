using GravurGIS.GUI.Controls;
namespace GravurGIS
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.mapTab = new System.Windows.Forms.TabPage();
            this.readyButton = new System.Windows.Forms.Button();
            this.layerTab = new System.Windows.Forms.TabPage();
            this.layerSettingsButton = new System.Windows.Forms.Button();
            this.layerInfoLabel = new System.Windows.Forms.Label();
            this.layerListView = new System.Windows.Forms.ListView();
            this.name = new System.Windows.Forms.ColumnHeader();
            this.typeColumn = new System.Windows.Forms.ColumnHeader();
            this.newLayerButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.propertyEditor = new GravurGIS.GUI.Controls.PropertyEditor();
            this.toolBar = new System.Windows.Forms.ToolBar();
            this.tbOpenButton = new System.Windows.Forms.ToolBarButton();
            this.tbSaveButton = new System.Windows.Forms.ToolBarButton();
            this.tbTools = new System.Windows.Forms.ToolBarButton();
            this.tbAddLayer = new System.Windows.Forms.ToolBarButton();
            this.tbDraw = new System.Windows.Forms.ToolBarButton();
            this.tbBack = new System.Windows.Forms.ToolBarButton();
            this.tbSettings = new System.Windows.Forms.ToolBarButton();
            this.tbHelp = new System.Windows.Forms.ToolBarButton();
            this.toolPictureBox = new GravurGIS.GUI.Controls.IconBox();
            this.inputPanel1 = new Microsoft.WindowsCE.Forms.InputPanel(this.components);
            this.statusBar = new GravurGIS.GUI.Controls.TextBoxEx();
            this.lockPictureBox = new GravurGIS.GUI.Controls.IconBox();
            this.tabControl.SuspendLayout();
            this.mapTab.SuspendLayout();
            this.layerTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.mapTab);
            this.tabControl.Controls.Add(this.layerTab);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.None;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(240, 246);
            this.tabControl.TabIndex = 1;
            // 
            // mapTab
            // 
            this.mapTab.AutoScroll = true;
            this.mapTab.Controls.Add(this.readyButton);
            this.mapTab.Location = new System.Drawing.Point(0, 0);
            this.mapTab.Name = "mapTab";
            this.mapTab.Size = new System.Drawing.Size(240, 223);
            this.mapTab.Text = "Karte";
            // 
            // readyButton
            // 
            this.readyButton.BackColor = System.Drawing.SystemColors.Highlight;
            this.readyButton.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.readyButton.ForeColor = System.Drawing.SystemColors.Info;
            this.readyButton.Location = new System.Drawing.Point(175, 197);
            this.readyButton.Name = "readyButton";
            this.readyButton.Size = new System.Drawing.Size(58, 18);
            this.readyButton.TabIndex = 4;
            this.readyButton.Text = "Fertig";
            this.readyButton.Visible = false;
            this.readyButton.Click += new System.EventHandler(this.readyButton_Click);
            // 
            // layerTab
            // 
            this.layerTab.Controls.Add(this.layerSettingsButton);
            this.layerTab.Controls.Add(this.layerInfoLabel);
            this.layerTab.Controls.Add(this.layerListView);
            this.layerTab.Controls.Add(this.newLayerButton);
            this.layerTab.Location = new System.Drawing.Point(0, 0);
            this.layerTab.Name = "layerTab";
            this.layerTab.Size = new System.Drawing.Size(232, 220);
            this.layerTab.Text = "Layer";
            // 
            // layerSettingsButton
            // 
            this.layerSettingsButton.Enabled = false;
            this.layerSettingsButton.Location = new System.Drawing.Point(130, 195);
            this.layerSettingsButton.Name = "layerSettingsButton";
            this.layerSettingsButton.Size = new System.Drawing.Size(103, 20);
            this.layerSettingsButton.TabIndex = 4;
            this.layerSettingsButton.Text = "Eigenschaften";
            this.layerSettingsButton.Click += new System.EventHandler(this.layerSettingsButton_Click);
            // 
            // layerInfoLabel
            // 
            this.layerInfoLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.layerInfoLabel.Location = new System.Drawing.Point(19, 17);
            this.layerInfoLabel.Name = "layerInfoLabel";
            this.layerInfoLabel.Size = new System.Drawing.Size(203, 144);
            this.layerInfoLabel.Text = resources.GetString("layerInfoLabel.Text");
            // 
            // layerListView
            // 
            this.layerListView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.layerListView.CheckBoxes = true;
            this.layerListView.Columns.Add(this.name);
            this.layerListView.Columns.Add(this.typeColumn);
            this.layerListView.Location = new System.Drawing.Point(8, 7);
            this.layerListView.Name = "layerListView";
            this.layerListView.Size = new System.Drawing.Size(225, 182);
            this.layerListView.TabIndex = 3;
            this.layerListView.View = System.Windows.Forms.View.List;
            this.layerListView.SelectedIndexChanged += new System.EventHandler(this.layerListView_SelectedIndexChanged);
            this.layerListView.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.layerListView_ItemCheck);
            // 
            // name
            // 
            this.name.Text = "Name";
            this.name.Width = 102;
            // 
            // typeColumn
            // 
            this.typeColumn.Text = "Typ";
            this.typeColumn.Width = 120;
            // 
            // newLayerButton
            // 
            this.newLayerButton.Location = new System.Drawing.Point(7, 195);
            this.newLayerButton.Name = "newLayerButton";
            this.newLayerButton.Size = new System.Drawing.Size(117, 20);
            this.newLayerButton.TabIndex = 2;
            this.newLayerButton.Text = "Layer hinzufügen";
            this.newLayerButton.Click += new System.EventHandler(this.newLayerButton_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(111, 14);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(107, 20);
            this.button2.TabIndex = 3;
            this.button2.Text = "MapPanel Test";
            // 
            // propertyEditor
            // 
            this.propertyEditor.Location = new System.Drawing.Point(14, 67);
            this.propertyEditor.Name = "propertyEditor";
            this.propertyEditor.Size = new System.Drawing.Size(204, 137);
            this.propertyEditor.TabIndex = 0;
            // 
            // toolBar
            // 
            this.toolBar.Buttons.Add(this.tbOpenButton);
            this.toolBar.Buttons.Add(this.tbSaveButton);
            this.toolBar.Buttons.Add(this.tbTools);
            this.toolBar.Buttons.Add(this.tbAddLayer);
            this.toolBar.Buttons.Add(this.tbDraw);
            this.toolBar.Buttons.Add(this.tbBack);
            this.toolBar.Name = "toolBar";
            this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
            // 
            // tbOpenButton
            // 
            this.tbOpenButton.Style = System.Windows.Forms.ToolBarButtonStyle.DropDownButton;
            this.tbOpenButton.ToolTipText = "Öffnen einer gespeicherten Zusammenstellung";
            // 
            // tbSaveButton
            // 
            this.tbSaveButton.ToolTipText = "Eine Zusammenstellung speichern";
            // 
            // tbTools
            // 
            this.tbTools.ToolTipText = "Ein Werkzeug auswählen";
            // 
            // tbAddLayer
            // 
            this.tbAddLayer.Style = System.Windows.Forms.ToolBarButtonStyle.DropDownButton;
            this.tbAddLayer.ToolTipText = "Einen neuen Layer hinzufügen";
            // 
            // tbDraw
            // 
            this.tbDraw.Style = System.Windows.Forms.ToolBarButtonStyle.DropDownButton;
            this.tbDraw.ToolTipText = "Das Zeichenmenü";
            // 
            // tbBack
            // 
            this.tbBack.Style = System.Windows.Forms.ToolBarButtonStyle.DropDownButton;
            this.tbBack.ToolTipText = "Zu letzter Ansicht zurück";
            // 
            // tbSettings
            // 
            this.tbSettings.ToolTipText = "Einstellungen";
            // 
            // tbHelp
            // 
            this.tbHelp.ToolTipText = "Die Hilfe anzeigen";
            // 
            // toolPictureBox
            // 
            this.toolPictureBox.BackColor = System.Drawing.SystemColors.Control;
            this.toolPictureBox.Icon = ((System.Drawing.Icon)(resources.GetObject("toolPictureBox.Icon")));
            this.toolPictureBox.Location = new System.Drawing.Point(219, 249);
            this.toolPictureBox.Name = "toolPictureBox";
            this.toolPictureBox.Size = new System.Drawing.Size(16, 16);
            this.toolPictureBox.TabIndex = 5;
            // 
            // statusBar
            // 
            this.statusBar.BackColor = System.Drawing.SystemColors.Control;
            this.statusBar.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.statusBar.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.statusBar.Location = new System.Drawing.Point(24, 249);
            this.statusBar.Multiline = true;
            this.statusBar.Name = "statusBar";
            this.statusBar.ReadOnly = true;
            this.statusBar.Size = new System.Drawing.Size(189, 19);
            this.statusBar.TabIndex = 8;
            this.statusBar.Text = "Willkommen";
            this.statusBar.WordWrap = false;
            // 
            // lockPictureBox
            // 
            this.lockPictureBox.BackColor = System.Drawing.SystemColors.Control;
            this.lockPictureBox.Icon = ((System.Drawing.Icon)(resources.GetObject("lockPictureBox.Icon")));
            this.lockPictureBox.Location = new System.Drawing.Point(3, 249);
            this.lockPictureBox.Name = "lockPictureBox";
            this.lockPictureBox.Selected_BackColor = System.Drawing.SystemColors.Control;
            this.lockPictureBox.Size = new System.Drawing.Size(16, 16);
            this.lockPictureBox.TabIndex = 9;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.lockPictureBox);
            this.Controls.Add(this.toolPictureBox);
            this.Controls.Add(this.toolBar);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.statusBar);
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "GravurGIS";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.tabControl.ResumeLayout(false);
            this.mapTab.ResumeLayout(false);
            this.layerTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage mapTab;
        private System.Windows.Forms.TabPage layerTab;
        private System.Windows.Forms.Button newLayerButton;
        private System.Windows.Forms.ListView layerListView;
        private System.Windows.Forms.ColumnHeader name;
        private System.Windows.Forms.Label layerInfoLabel;
        //private System.Windows.Forms.TabPage gpsTab;
        private System.Windows.Forms.ToolBar toolBar;
        private System.Windows.Forms.ToolBarButton tbOpenButton;
        private System.Windows.Forms.ToolBarButton tbSaveButton;
        private System.Windows.Forms.ToolBarButton tbAddLayer;
        private System.Windows.Forms.ToolBarButton tbBack;
        private System.Windows.Forms.ToolBarButton tbSettings;
        private System.Windows.Forms.ToolBarButton tbHelp;
        private System.Windows.Forms.ToolBarButton tbTools;
        private System.Windows.Forms.ToolBarButton tbDraw;
        private GravurGIS.GUI.Controls.IconBox toolPictureBox;
        private System.Windows.Forms.Button readyButton;
        private Microsoft.WindowsCE.Forms.InputPanel inputPanel1;
        private System.Windows.Forms.Button layerSettingsButton;
        private TextBoxEx statusBar;
        private PropertyEditor propertyEditor;
        private System.Windows.Forms.Button button2;
        private IconBox lockPictureBox;
        private System.Windows.Forms.ColumnHeader typeColumn;
    }
}

