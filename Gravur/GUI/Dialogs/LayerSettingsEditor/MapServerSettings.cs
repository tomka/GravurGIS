using System; 
using System.Drawing; 
using System.Collections; 
using System.ComponentModel; 
using System.Windows.Forms;
using GravurGIS.GUI.Controls;
using GravurGIS.Layers;
using GravurGIS.Layers.MapServer;
using GravurGIS.GUI.Menus;

namespace GravurGIS.GUI.Dialogs
{
    /// <summary> 
    /// Summary description for Form2. 
    /// </summary> 
    public class MapServerSettings : IDialog
    {
        private System.Windows.Forms.Label Caption;
        private TabControl layerTabControl;
        private TabPage generalTab;
        private TabPage layerTab;
        private TabPage scaleTab;
        private TextBox layerNameTextBox;
        private Label label2;
        private TextBox layerCommentTextBox;
        private Label label4;
        private System.Windows.Forms.Panel panel1;
        private MapServerLayer currentLayer;
        private TabPage infoTab;
        private Microsoft.WindowsCE.Forms.InputPanel inputPanel1;
        private MainControler mainControler;
        private TabPage connectionTab;
        private Label label12;
        private TextBox msServer;
        private Label label5;
        private Label label15;
        private Label label16;
        private ListView availLayersView;
        private TreeView infoTree;
        private ToolBarButton tbUpdateData;
        private Label label7;
        private Label label3;
        private Label label1;
        private IContainer components;
        private bool bListViewClicked = false;

        private Config config;
        private MSLayerManager msLm = null;


        //private bool infoTab;
        /// <summary>
        /// A Dialog for editing the properties of a layer.
        /// </summary>
        public MapServerSettings()
        {
            // 
            // Required for Windows Form Designer support 
            // 
            InitializeComponent();
        }
        public MapServerSettings(Rectangle visibleRect, MainControler mainControler)
            : this()
        {
            this.config = mainControler.Config;

            this.tbUpdateData = new System.Windows.Forms.ToolBarButton();
            this.toolBar.Buttons.Add(this.tbUpdateData);
            ToolbarMaker.AddIcon(config.ApplicationDirectory + @"\Icons\update");

            this.Location = new System.Drawing.Point(0, visibleRect.Y);
            this.ClientSize = new System.Drawing.Size(visibleRect.Width, visibleRect.Height);
            this.layerTabControl.Height = this.ClientSize.Height - Caption.Height - 3;
            this.panel1.Height = layerTabControl.Height;
            this.mainControler = mainControler;
            this.config = mainControler.Config;
            this.availLayersView.ContextMenu = new MsLayerMenu(this);
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
            this.layerCommentTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.layerNameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.connectionTab = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.msServer = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.layerTab = new System.Windows.Forms.TabPage();
            this.availLayersView = new System.Windows.Forms.ListView();
            this.infoTab = new System.Windows.Forms.TabPage();
            this.infoTree = new System.Windows.Forms.TreeView();
            this.scaleTab = new System.Windows.Forms.TabPage();
            this.Caption = new System.Windows.Forms.Label();
            this.tbUpdateData = new System.Windows.Forms.ToolBarButton();
            this.inputPanel1 = new Microsoft.WindowsCE.Forms.InputPanel(this.components);
            this.panel1.SuspendLayout();
            this.layerTabControl.SuspendLayout();
            this.generalTab.SuspendLayout();
            this.connectionTab.SuspendLayout();
            this.layerTab.SuspendLayout();
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
            this.layerTabControl.Controls.Add(this.connectionTab);
            this.layerTabControl.Location = new System.Drawing.Point(0, 0);
            this.layerTabControl.Name = "layerTabControl";
            this.layerTabControl.SelectedIndex = 0;
            this.layerTabControl.Size = new System.Drawing.Size(238, 172);
            this.layerTabControl.TabIndex = 0;
            // 
            // generalTab
            // 
            this.generalTab.Controls.Add(this.layerCommentTextBox);
            this.generalTab.Controls.Add(this.label4);
            this.generalTab.Controls.Add(this.layerNameTextBox);
            this.generalTab.Controls.Add(this.label2);
            this.generalTab.Location = new System.Drawing.Point(0, 0);
            this.generalTab.Name = "generalTab";
            this.generalTab.Size = new System.Drawing.Size(238, 149);
            this.generalTab.Text = "Allgemein";
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
            this.layerNameTextBox.Text = "WMS-Layer";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(7, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 15);
            this.label2.Text = "Name:";
            // 
            // connectionTab
            // 
            this.connectionTab.AutoScroll = true;
            this.connectionTab.Controls.Add(this.label7);
            this.connectionTab.Controls.Add(this.label3);
            this.connectionTab.Controls.Add(this.label1);
            this.connectionTab.Controls.Add(this.label16);
            this.connectionTab.Controls.Add(this.label15);
            this.connectionTab.Controls.Add(this.label12);
            this.connectionTab.Controls.Add(this.msServer);
            this.connectionTab.Controls.Add(this.label5);
            this.connectionTab.Location = new System.Drawing.Point(0, 0);
            this.connectionTab.Name = "connectionTab";
            this.connectionTab.Size = new System.Drawing.Size(238, 149);
            this.connectionTab.Text = "Verbindung";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(85, 110);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 20);
            this.label7.Text = "GetCapabilities()";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(85, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 20);
            this.label3.Text = "1.1.1";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(85, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 20);
            this.label1.Text = "WMS";
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(9, 110);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(55, 20);
            this.label16.Text = "Anfrage:";
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(9, 85);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(59, 20);
            this.label15.Text = "Version:";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(9, 58);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(76, 20);
            this.label12.Text = "Dienst:";
            // 
            // msServer
            // 
            this.msServer.Location = new System.Drawing.Point(9, 32);
            this.msServer.Name = "msServer";
            this.msServer.Size = new System.Drawing.Size(222, 21);
            this.msServer.TabIndex = 3;
            this.msServer.Text = "http://maps.webs.idu.de/wms.ogc.iws?prj=BAU";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(9, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 20);
            this.label5.Text = "Anfrage-Adresse:";
            // 
            // layerTab
            // 
            this.layerTab.AutoScroll = true;
            this.layerTab.Controls.Add(this.availLayersView);
            this.layerTab.Location = new System.Drawing.Point(0, 0);
            this.layerTab.Name = "layerTab";
            this.layerTab.Size = new System.Drawing.Size(238, 149);
            this.layerTab.Text = "Layer";
            // 
            // availLayersView
            // 
            this.availLayersView.CheckBoxes = true;
            this.availLayersView.Location = new System.Drawing.Point(8, 8);
            this.availLayersView.Name = "availLayersView";
            this.availLayersView.Size = new System.Drawing.Size(203, 127);
            this.availLayersView.TabIndex = 0;
            this.availLayersView.View = System.Windows.Forms.View.Details;
            this.availLayersView.SelectedIndexChanged += new System.EventHandler(this.availLayersView_SelectedIndexChanged);
            this.availLayersView.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.availLayersView_ItemCheck);
            // 
            // infoTab
            // 
            this.infoTab.AutoScroll = true;
            this.infoTab.Controls.Add(this.infoTree);
            this.infoTab.Location = new System.Drawing.Point(0, 0);
            this.infoTab.Name = "infoTab";
            this.infoTab.Size = new System.Drawing.Size(230, 146);
            this.infoTab.Text = "Info";
            // 
            // infoTree
            // 
            this.infoTree.Location = new System.Drawing.Point(7, 7);
            this.infoTree.Name = "infoTree";
            this.infoTree.Size = new System.Drawing.Size(223, 98);
            this.infoTree.TabIndex = 0;
            // 
            // scaleTab
            // 
            this.scaleTab.Location = new System.Drawing.Point(0, 0);
            this.scaleTab.Name = "scaleTab";
            this.scaleTab.Size = new System.Drawing.Size(230, 146);
            this.scaleTab.Text = "Maﬂstab";
            // 
            // Caption
            // 
            this.Caption.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular);
            this.Caption.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Caption.Location = new System.Drawing.Point(4, 3);
            this.Caption.Name = "Caption";
            this.Caption.Size = new System.Drawing.Size(172, 18);
            this.Caption.Text = "WMS-Layer bearbeiten";
            // 
            // MapServerSettings
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
            this.Name = "MapServerSettings";
            this.TopMost = true;
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseDown);
            this.panel1.ResumeLayout(false);
            this.layerTabControl.ResumeLayout(false);
            this.generalTab.ResumeLayout(false);
            this.connectionTab.ResumeLayout(false);
            this.layerTab.ResumeLayout(false);
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
        public DialogResult ShowDialog(MapServerLayer layer)
        {
            this.currentLayer = layer;

            if (layer == null)
                layerTabControl.SelectedIndex = 1;
            else
            {
                layerTabControl.SelectedIndex = 0;
                fillDialogWithData(layer);
            }

            return base.ShowDialog();
        }

        private void fillDialogWithData(MapServerLayer layer)
        {
            if (layer.IsUptodate)
            {
                this.layerTabControl.Controls.Add(this.layerTab);
                this.layerTabControl.Controls.Add(this.infoTab);

                this.bListViewClicked = false;
                availLayersView.Clear();
                this.availLayersView.Columns.Add("Name", 120, HorizontalAlignment.Left);
                msLm = layer.ConcreteLayer.LayerManager;
                for (int i = 0; i < msLm.LayerCount; i++)
                {
                    MSLayer concreteLayer = msLm.getLayer(i);
                    ListViewItem lItem = new ListViewItem(concreteLayer.Title);
                    availLayersView.Items.Add(lItem);
                    availLayersView.Items[i].Checked = concreteLayer.Visible;
                }
                this.bListViewClicked = true;

                layerNameTextBox.Text = currentLayer.LayerName;
                layerCommentTextBox.Text = currentLayer.Comment;
                msServer.Text = currentLayer.MapserverURL;

                // build tree

                this.infoTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            new TreeNode("Server"),
            new TreeNode("Dienste"),
            new TreeNode("Layer")});

                infoTree.Nodes[0].Nodes.Add("Name: " + currentLayer.ConcreteLayer.Name);
                infoTree.Nodes[0].Nodes.Add("Title: " + currentLayer.ConcreteLayer.Title);
                infoTree.Nodes[0].Nodes.Add("Abstract: " + currentLayer.ConcreteLayer.AbstractDesc);
                infoTree.Nodes[0].Nodes.Add("Kontakt: " + currentLayer.ConcreteLayer.ContactInfo);

                infoTree.Nodes[1].Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            new TreeNode("Anfragen"),
            new TreeNode("Ausnahmen"),
            new TreeNode("Benutzermˆglichkeiten")});

                if (currentLayer.ConcreteLayer.Capabilities.GetCapabilitiesEnabled)
                    infoTree.Nodes[1].Nodes[0].Nodes.Add("GetCapabilities");
                if (currentLayer.ConcreteLayer.Capabilities.GetMapEnabled)
                    infoTree.Nodes[1].Nodes[0].Nodes.Add("GetMap");
                if (currentLayer.ConcreteLayer.Capabilities.GetFeatureInfoEnabled)
                    infoTree.Nodes[1].Nodes[0].Nodes.Add("GetFeatureInfo");
                if (currentLayer.ConcreteLayer.Capabilities.DescribeLayerEnabled)
                    infoTree.Nodes[1].Nodes[0].Nodes.Add("DescribeLayer");
                if (currentLayer.ConcreteLayer.Capabilities.GetLegendGraphicEnabled)
                    infoTree.Nodes[1].Nodes[0].Nodes.Add("GetLegendGraphic");
                if (currentLayer.ConcreteLayer.Capabilities.GetStylesEnabled)
                    infoTree.Nodes[1].Nodes[0].Nodes.Add("GetStyles");
            }
        }

        public void updateListView()
        {
            this.bListViewClicked = false;
            availLayersView.Items.Clear();
            if (msLm != null)
            {
                for (int i = 0; i < msLm.LayerCount; i++)
                {
                    MSLayer concreteLayer = msLm.getLayer(i);
                    ListViewItem lItem = new ListViewItem(concreteLayer.Title);
                    availLayersView.Items.Add(lItem);
                    availLayersView.Items[i].Checked = concreteLayer.Visible;
                }
            }
            this.bListViewClicked = true;
            
        }

        public void moveSelectedLayerOneStepUp()
        {
            int count = availLayersView.SelectedIndices.Count;

            // if we have sth. selected && if the selected index entry is not the last
            if (count > 0)
            {
                int index = availLayersView.SelectedIndices[0];
                if (index > 0)
                {
                    msLm.moveLayerUpOneStep(index);
                    updateListView();
                    availLayersView.Items[index - 1].Selected = true;
                }
            }

            availLayersView.Focus();
        }

        public void moveSelectedLayerOneStepDown()
        {
            int count = availLayersView.SelectedIndices.Count;

            // if we have sth. selected && if the selected index entry is not the last
            if (count > 0)
            {
                int index = availLayersView.SelectedIndices[0];
                if (index < (availLayersView.Items.Count - 1))
                {
                    this.msLm.moveLayerDownOneStep(index);
                    updateListView();
                    availLayersView.Items[index + 1].Selected = true;
                }
            }

            availLayersView.Focus();
        }

        public void moveSelectedLayerHighest()
        {
            int count = availLayersView.Items.Count;

            // if we have sth. selected && if the selected index entry is not the last
            if (count > 1)
            {
                int index = availLayersView.SelectedIndices[0];
                for (int i = index; i > 0; i--)
                    this.msLm.moveLayerUpOneStep(i);
                updateListView();
                availLayersView.Items[0].Selected = true;
            }

            availLayersView.Focus();
        }

        public void moveSelectedLayerLowest()
        {
            int count = availLayersView.Items.Count - 1;

            // if we have sth. selected && if the selected index entry is not the last
            if (count > 0)
            {
                int index = availLayersView.SelectedIndices[0];
                for (int i = index; i < count; i++)
                    this.msLm.moveLayerDownOneStep(i);
                updateListView();
                availLayersView.Items[0].Selected = true;
            }

            availLayersView.Focus();
        }

        private void saveData()
        {
            currentLayer.LayerName = layerNameTextBox.Text;
            currentLayer.Comment = layerCommentTextBox.Text;
            currentLayer.MapserverURL = msServer.Text;
        }


        protected override void toolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button.Equals(tbUpdateData))
            {
                if (currentLayer == null)
                    currentLayer = new MapServerLayer(msServer.Text,
                        MapServerService.WMS, MapServerVersion.v1_1_1);
                else
                    currentLayer.GetData();
                fillDialogWithData(currentLayer);
                return;
            }
            else if (e.Button.Equals(tbOkButton))
            {
                if (currentLayer == null)
                    currentLayer = new MapServerLayer(msServer.Text,
                        MapServerService.WMS, MapServerVersion.v1_1_1);

                saveData();
            } 
            else if (e.Button.Equals(tbCancelButton))

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

        public MapServerLayer CurrentLayer
        {
            get { return currentLayer; }
        }

        void availLayersView_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
        {
            if (this.bListViewClicked)
            {
                this.msLm.switchLayerVisibility(e.Index);
                this.mainControler.MapPanel.Invalidate();
            }
        }

        void availLayersView_SelectedIndexChanged(object sender, EventArgs e)
        { }

    }
}

