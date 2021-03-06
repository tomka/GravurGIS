using System;
using MapTools;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using GravurGIS.Shapes;
using System.Collections;
using System.Threading;
using GravurGIS.GPS;
using GravurGIS.GUI.Menu;
using Microsoft.WindowsCE.Forms;
using GravurGIS.GUI.Controls;
using GravurGIS.Layers;
using GravurGIS.Actions;

namespace GravurGIS
{
    public enum ViewMode { Map,Layers};
    public partial class MainForm : Form
    {
        private MainControler mainControler;
        private MapPanel mapPanel;
        private Config config;
        /// <summary>
        /// Workaround, um zu verhindern, dass beim Hinzufügen von einem Layer etwas im ItemCheckEvent geschieht
        /// </summary>
        private bool bListViewClicked = false;
        private Tool tempTool = Tool.None;
        private System.Windows.Forms.ToolBar dialogToolBar;
        private System.Windows.Forms.ToolBarButton tbOkButton;
        private System.Windows.Forms.ToolBarButton tbCancelButton;

        private ToolMenu toolMenu;
        private DrawMenu drawMenu;
        private FileMenu fileMenu;
        private AddMenu addMenu;
        private UndoMenu undoMenu;
        private ViewMode viewMode;

        private System.Windows.Forms.Timer statusTimer = new System.Windows.Forms.Timer();

        private enum layerType {shpObject,PointTrans,PolylineTrans,PolygonTrans};

        ListViewItem lPoints = new ListViewItem();

        private int itemNr;

        private bool screenIsLocked;

        private bool bTbBackEnabled = false;

        private string oldStatusText;
        //private Utilities.HookKeys keysHook;
        //private bool notFirstCallBack = false;

        public MainForm(MainControler mainControler, Config config, string appName)
        {
            InitializeComponent();
            this.Text = appName;

            this.mainControler = mainControler;
            
            statusTimer.Enabled = false;
            statusTimer.Tick += new EventHandler(StatusTimerEvent);

            mainControler.LayerManager.FirstLayerAdded += new LayerManager.LayerAddedDelegate(LayerManager_FirstLayerAdded);
            mainControler.LayerManager.LayerAdded += new LayerManager.LayerAddedDelegate(layerAdded);
			mainControler.LayerManager.LayerChanged += new LayerManager.LayerChangedDelegate(LayerManager_LayerChanged);
            mainControler.SettingsLoaded += new MainControler.SettingsLoadedDelegate(mainControler_SettingsLoaded);
            this.config = config;

            // Menus
            toolMenu = new ToolMenu(this);
            drawMenu = new DrawMenu(this);
            fileMenu = new FileMenu(mainControler);
            addMenu = new AddMenu(mainControler);
            undoMenu = new UndoMenu(mainControler);

            this.layerListView.ContextMenu = new LayerMenu(this);

            ToolbarMaker.DisplayResolution = mainControler.DisplayResolution;
           

            //this.keysHook = new GravurGIS.Utilities.HookKeys();
            //this.keysHook.HookEvent += new HookKeys.HookEventHandler(keysHook_HookEvent);
            //this.keysHook.Start();
            
        }

		void LayerManager_LayerChanged(ILayer layer)
		{
			updateListView();
		}

        //void keysHook_HookEvent(HookEventArgs e, KeyBoardInfo keyBoardInfo)
        //{
        //    if (notFirstCallBack)
        //        if (keyBoardInfo.vkCode == 133)
        //            lockPictureBox_Clicked(null, null);

        //    notFirstCallBack = true;
        //}

        void LayerManager_FirstLayerAdded(Layer newShapeObject)
        {
            // attach the listener after first layer has been loaded
            this.statusBar.MouseUp += new TextBoxEx.MouseDelgate(statusBar_MouseUp);
        }

        public void InitializeMyOwnComponents(MapPanel mapPanel)
        {

            this.mapPanel = mapPanel;
            this.mapTab.Controls.Add(this.mapPanel);

            //this.Load += new System.EventHandler(this.Form1_Load);

            // ToolBar stuff
            this.dialogToolBar = new ToolBar();
            tbOkButton = new ToolBarButton();
            tbCancelButton = new ToolBarButton();
            dialogToolBar.Buttons.Add(tbOkButton);
            dialogToolBar.Buttons.Add(tbCancelButton);

            lockPictureBox.Icon = ToolbarMaker.GetIconFromFile(config.ApplicationDirectory + @"\Icons\stLockOpen");

            ToolbarMaker.ToolBar = this.toolBar;
            ToolbarMaker.AddIcon(config.ApplicationDirectory + @"\Icons\tbOpen");
            ToolbarMaker.AddIcon(config.ApplicationDirectory + @"\Icons\tbSave");
            ToolbarMaker.AddIcon(config.ApplicationDirectory + @"\Icons\tbTools");
            ToolbarMaker.AddIcon(config.ApplicationDirectory + @"\Icons\tbAddLayer");
            ToolbarMaker.AddIcon(config.ApplicationDirectory + @"\Icons\tbDraw");
            ToolbarMaker.AddIcon(config.ApplicationDirectory + @"\Icons\tbBack");
            //ToolbarMaker.AddIcon("Icons.tbSettings.ico");
            //ToolbarMaker.AddIcon(config.ApplicationDirectory + @"\Icons\tbHelp");
            this.tbTools.Style = System.Windows.Forms.ToolBarButtonStyle.DropDownButton;
            this.tbTools.DropDownMenu = this.toolMenu;
            this.tbDraw.DropDownMenu = this.drawMenu;
            this.tbOpenButton.DropDownMenu = this.fileMenu;
            this.tbAddLayer.DropDownMenu = this.addMenu;
            this.tbBack.DropDownMenu = this.undoMenu;

            ToolbarMaker.ToolBar = dialogToolBar;
            ToolbarMaker.AddIcon(config.ApplicationDirectory + @"\Icons\tbOk");
            ToolbarMaker.AddIcon(config.ApplicationDirectory + @"\Icons\tbCancel");

            // ProperyEditor
            //   PropertyItem  item = new PropertyItem("TestContainer", PropertyType.Container);
            //   item.State = PropertyState.Closed;
            //   item.AddItem(new PropertyItem("Testkind1", PropertyType.Checkbox));
            //   item.AddItem(new PropertyItem("Testkind2", PropertyType.Radio));
            ////   item.AddItem(new PropertyItem("EditTest", PropertyType.EditField));
            //   this.propertyEditor.addItem(item);

            //   item = new PropertyItem("TestRadio", PropertyType.Radio);
            //   item.State = PropertyState.Checked;
            //   this.propertyEditor.addItem(item);


            //   item = new PropertyItem("TestContainer", PropertyType.Container);
            //   item.State = PropertyState.Closed;
            //   item.AddItem(new PropertyItem("Testkind1", PropertyType.Checkbox));
            //   PropertyItem childContainer = new PropertyItem("Testkind2", PropertyType.Container);
            //   childContainer.AddItem(new PropertyItem("Testkind3", PropertyType.Checkbox));
            //   item.AddItem(childContainer);

            //   this.propertyEditor.addItem(item);

            //   updateItemNr();

            //   setDelegates(propertyEditor);      
            //   propertyEditor.Invalidate();

            this.KeyUp += new KeyEventHandler(MainForm_KeyUp);

            inputPanel1.EnabledChanged += new EventHandler(inputPanel1_EnabledChanged);

            // SelectedTool Observer events
            mapPanel.SelectedToolChanged += new MapPanel.SelectToolDelegate(changeSelectedToolIcon);
            mapPanel.SelectedToolChanged += new MapPanel.SelectToolDelegate(toolMenu.SelectedToolChanged);
            mapPanel.SelectedToolChanged += new MapPanel.SelectToolDelegate(drawMenu.SelectedToolChanged);

            // EditModeInterrupted Observer
            mapPanel.EditModeInterrupted += new MapPanel.EditModeInterruptedDelegate(drawMenu.EditModeInterrupted);
            mapPanel.EditModeInterrupted += new MapPanel.EditModeInterruptedDelegate(mapPanel_EditModeInterrupted);

            //
            mainControler.ActionlistCountChanged += new MainControler.ActionCountStuffDelegate(OnActionlistCountChanged);
            
            lockPictureBox.MouseUp += new MouseEventHandler(lockPictureBox_Clicked);

            this.Resize += new EventHandler(MainForm_Resize);

            mapPanel.SelectedTool = Tool.Pointer;

            tabControl.SelectedIndex = 1;

            // Events for setting items
            //TODO: abh. von Auflösungsvariable machen!
        }

        void MainForm_Resize(object sender, EventArgs e)
        {
            if (mainControler.LayerManager.LayerCount > 0)
            {
                layerListView.View = View.Details;
                layerInfoLabel.Visible = false;
            }
        }

        void mainControler_SettingsLoaded(Config config)
        {
            setStatus_Timed("Einstellungen aktualisiert", 2000);
        }

        void lockPictureBox_Clicked(object sender, MouseEventArgs e)
        {
            Win32.keybd_event(0x85, 0, 0, 0);
            //if (screenIsLocked)
            //{
            //    this.keysHook.HookEvent -= keysHook_HookEvent;
            //    lockPictureBox.Icon = ToolbarMaker.GetIconFromFile(config.ApplicationDirectory + @"\Icons\stLockOpen");
            //    screenIsLocked = false;
            //    tabControl.Enabled = true;
            //}
            //else
            //{
            //    notFirstCallBack = false;
            //    this.keysHook.HookEvent += new HookKeys.HookEventHandler(keysHook_HookEvent);
            //    Win32.keybd_event(0x85, 0, 0, 0);
            //    tabControl.Enabled = false;
            //    lockPictureBox.Icon = ToolbarMaker.GetIconFromFile(config.ApplicationDirectory + @"\Icons\stLockClosed");
            //    screenIsLocked = true;
            //}
        }

        private void updateItemNr()
        {
            itemNr = 1;
            for (int i = 0; i < propertyEditor.Controls.Count; i++)
            {
                PropertyItem item = ((PropertyItem)(propertyEditor.Controls[i]));
                item.ItemNr = itemNr;
                itemNr++;
                if (item.Controls.Count > 0) updateItemNr(item,ref itemNr);
            
            }
        }

        private void updateItemNr(PropertyItem parentItem, ref int itemNr)
        {
            if (parentItem.Type != PropertyType.EditField)
            {
                for (int i = 0; i < parentItem.Controls.Count; i++)
                {
                    PropertyItem item = (PropertyItem)parentItem.Controls[i];
                    item.ItemNr = itemNr;
                    itemNr++;
                    if (item.Controls.Count > 0) updateItemNr(item, ref itemNr);
                }
            }
        }

        private void setDelegates(Control parentControl)
        {
            PropertyItem delegateItem;
            for (int i = parentControl.Controls.Count - 1; i >= 0; i--)
            {
                delegateItem = (PropertyItem)parentControl.Controls[i];

                addDelegates(delegateItem,propertyEditor);
            }
        }

        private void addDelegates(PropertyItem delegateItem, Control parentControl)
        {
            PropertyItem eventItem;
            PropertyItem.CloseContainerDelegate closeDelegate = new PropertyItem.CloseContainerDelegate(delegateItem.AsContainerClosed);
            PropertyItem.OpenContainerDelegate openDelegate = new PropertyItem.OpenContainerDelegate(delegateItem.AsContainerOpened);
            for (int j = parentControl.Controls.Count - 1; j >= 0; j--)
            {
                eventItem = (PropertyItem)parentControl.Controls[j]; //TODO: Editfield hat Textbox
                if (eventItem.Controls.Count > 0) addDelegates(delegateItem, eventItem);
                if (eventItem.Type == PropertyType.Container)
                {
                    eventItem.ContainerClosed += closeDelegate;
                    eventItem.ContainerOpened += openDelegate;
                }
            }
        }

         //Draw Information Changed Event
        public void OnDrawInfChanged(DrawShapeInformation inf,LayerType lType)
        {
            switch (inf)
            {
                case DrawShapeInformation.EditStarted:
                    this.showReadyButton();
                    break;
                case DrawShapeInformation.EditStoppedUndone: 
                    this.showReadyButton();
                    break;
                case DrawShapeInformation.EditStopped:
                    this.readyButton.Hide();
                    toolBar.Buttons[4].Enabled = true;
                    break;
                default:
                    break;
            }
        }

        public void OnActionlistCountChanged(int currentAction, int listSize)
        {
            if (currentAction == 0)
            {
                this.bTbBackEnabled = false;
                (this.undoMenu.MenuItems[0]).Enabled = false;
            }
            else
            {
                this.bTbBackEnabled = true;
                (this.undoMenu.MenuItems[0]).Enabled = true;
            }

            if (currentAction == listSize)
                this.undoMenu.MenuItems[1].Enabled = false;
            else this.undoMenu.MenuItems[1].Enabled = true;
        }


        void mapPanel_EditModeInterrupted(bool interrupted, Tool interruptedTool)
        {
            if (interrupted)
                tbDraw.Enabled = true;
            else
                tbDraw.Enabled = false;
        }

        void inputPanel1_EnabledChanged(object sender, EventArgs e)
        {
            mainControler.SIPStateChanged();
        }
        
        public Rectangle VisibleDesktop
        {
            get
            {
                Rectangle visRect = inputPanel1.VisibleDesktop;

                // is needed as the toolbar is not taken care of in VisibleDesktop
                if (!inputPanel1.Enabled)
                    visRect.Height -= visRect.Y;

                return visRect;
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (!screenIsLocked)
            {
                if ((e.KeyCode == System.Windows.Forms.Keys.Up))
                {
                    mainControler.HardwareKeyDown(HardwareKeys.Up);
                }
                if ((e.KeyCode == System.Windows.Forms.Keys.Down))
                {
					mainControler.HardwareKeyDown(HardwareKeys.Down);
                }
                if ((e.KeyCode == System.Windows.Forms.Keys.Left))
                {
					mainControler.HardwareKeyDown(HardwareKeys.Left);
                }
                if ((e.KeyCode == System.Windows.Forms.Keys.Right))
                {
					mainControler.HardwareKeyDown(HardwareKeys.Right);
                }
                if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
                {
					mainControler.HardwareKeyDown(HardwareKeys.Enter);
                }
            }
        }

		void MainForm_KeyUp(object sender, KeyEventArgs e)
		{
			if (!screenIsLocked)
			{
				if ((e.KeyCode == System.Windows.Forms.Keys.Up))
				{
					mainControler.HardwareKeyUp(HardwareKeys.Up);
				}
				if ((e.KeyCode == System.Windows.Forms.Keys.Down))
				{
					mainControler.HardwareKeyUp(HardwareKeys.Down);
				}
				if ((e.KeyCode == System.Windows.Forms.Keys.Left))
				{
					mainControler.HardwareKeyUp(HardwareKeys.Left);
				}
				if ((e.KeyCode == System.Windows.Forms.Keys.Right))
				{
					mainControler.HardwareKeyUp(HardwareKeys.Right);
				}
				if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
				{
					mainControler.HardwareKeyUp(HardwareKeys.Enter);
				}
			}
		}
		
		public void toggleTemporaryTool(Tool TempTool) {
			if (tempTool == Tool.None ) {
				tempTool = mapPanel.SelectedTool;
				changeTool(TempTool);
			}
			else {
				changeTool(tempTool);
				tempTool = Tool.None;
			}
		}

        public void addLogMessage(String msg)
        {
            //logLabel.Text = logLabel.Text + "\n" + msg;

        }

        public MapPanel getMapPanel() {
            return this.mapPanel;
        }

        public void removeLayer()
        {
            foreach (int index in layerListView.SelectedIndices)
            {
                mainControler.LayerManager.removeLayer(index);
                layerListView.Items.Remove(layerListView.Items[index]);
            }
            clearLayerList();
        }

        public void clearLayerList()
        {
            if (layerListView.Items.Count == 0)
            {
                layerListView.View = View.List;
                layerInfoLabel.Visible = true;
            }
        }

        private void newLayerButton_Click(object sender, EventArgs e)
        {
            mainControler.addLayer();
        }

        private void layerAdded(Layer newLayer)
        {     
            this.bListViewClicked = false;
            ListViewItem item = new ListViewItem(newLayer.LayerName);
            item.SubItems.Add(newLayer.Description);
            layerListView.Items.Add(item);
            item.Checked = true;
            this.bListViewClicked = true;
            if (layerInfoLabel.Visible)
            {
                layerListView.View = View.Details;
                layerInfoLabel.Visible = false;
            }
            setStatus_Timed(String.Format("{0}. Layer erfolgreich hinzugefügt...", mainControler.LayerManager.LayerCount), 2000);
        }

        /// <summary>
        /// This method is bound to the ItemCheck event of the layerListView which occurs when
        /// the check state of an item has changed.
        /// </summary>
        /// <param name="sender">the ListView</param>
        /// <param name="e"></param>
        void layerListView_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
        {
            if (bListViewClicked)
            {
                mainControler.LayerManager.switchLayerVisibility(e.Index, e.NewValue == CheckState.Checked);
                mapPanel.Invalidate();
            }
        }
           
        public void updateListView()
        {
            bListViewClicked = false;
            layerListView.Items.Clear();

            int count = mainControler.LayerManager.LayerCount;
            Layer layer;
            for (int i = 0; i < count; i++ )
            {
                layer = mainControler.LayerManager.getLayerFromMapping(i);
                layerListView.Items.Add(
                    new ListViewItem(new String[] { layer.LayerName, layer.Description }));
                layerListView.Items[i].Checked = layer.Visible;
            }
            bListViewClicked = true;
        }

        public void moveSelectedLayerOneStepDown()
        {
            int count = layerListView.SelectedIndices.Count;

            // if we have sth. selected && if the selected index entry is not the last
            if (count > 0)
            {
                int index = layerListView.SelectedIndices[0];
                if (index < (layerListView.Items.Count - 1))
                {
                    mainControler.LayerManager.moveLayerDownOneStep(index);
                    updateListView();
                    layerListView.Items[index + 1].Selected = true;
                }
            }

            layerListView.Focus();
        }

        public void moveSelectedLayerOneStepUp()
        {
            int count = layerListView.SelectedIndices.Count;

            // if we have sth. selected && if the selected index entry is not the last
            if (count > 0)
            {
                int index = layerListView.SelectedIndices[0];
                if (index > 0)
                {
                    mainControler.LayerManager.moveLayerUpOneStep(index);
                    updateListView();
                    layerListView.Items[index - 1].Selected = true;
                }
            }

            layerListView.Focus();
        }

        public void moveSelectedLayerHighest()
        {
            int count = layerListView.Items.Count;

            // if we have sth. selected && if the selected index entry is not the last
            if (count > 1)
            {
                int index = layerListView.SelectedIndices[0];
                for (int i = index; i > 0; i--)
                    mainControler.LayerManager.moveLayerUpOneStep(i);
                updateListView();
                layerListView.Items[0].Selected = true;
            }

            layerListView.Focus();
        }

        public void moveSelectedLayerLowest()
        {
            int count = layerListView.Items.Count - 1;

            // if we have sth. selected && if the selected index entry is not the last
            if (count > 0)
            {
                int index = layerListView.SelectedIndices[0];
                for (int i = index; i < count; i++)
                    mainControler.LayerManager.moveLayerDownOneStep(i);
                updateListView();
                layerListView.Items[0].Selected = true;
            }

            layerListView.Focus();
        }      

        public System.Drawing.Point getMapTabClientArea()
        {
            return new System.Drawing.Point(this.mapTab.ClientRectangle.Width,
                this.mapTab.ClientRectangle.Height);
        }

        public void setStatusBarText(string text)
        {
            if (statusTimer.Enabled)
                oldStatusText = text;
            else
                statusBar.Text = text;
        }

        /// <summary>
        /// Shows the specified status text for the specified time in the status bar
        /// After the time has elapsed the old text is shown
        /// </summary>
        /// <param name="text">The text to show</param>
        /// <param name="msec">Time in milliseconds</param>
        public void setStatus_Timed(string text, int msec)
        {
            statusTimer.Interval = msec;
            if (!statusTimer.Enabled)
                oldStatusText = statusBar.Text;
            
            statusBar.Text = text;
            statusTimer.Enabled = true;
        }

        public void StatusTimerEvent(object source, EventArgs e)
        {
            statusBar.Text = oldStatusText;
            statusTimer.Enabled = false;
        }

        public System.Windows.Forms.ListView LayerListView
        {
            get { return layerListView; }
        }

        private void toolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (!screenIsLocked)
            {
                if (e.Button == tbTools)
                    changeTool(Tool.Move);
                else if (e.Button == tbAddLayer)
                    mainControler.addLayer();
                else if (e.Button == tbSaveButton)
                    mainControler.save();
                else if (e.Button == tbSettings)
                    mainControler.showSettingsDialog();
                else if ((e.Button == tbBack) && this.bTbBackEnabled)
                    mainControler.UndoAction();
            }
        }

        public void changeTool(Tool tool)
        {
            mapPanel.SelectedTool = tool;  
        }

        public void changeSelectedToolIcon(Tool tool)
        {
            // change the icon of the toolPictureBox
            if (tool == Tool.DrawPoint || tool == Tool.DrawPolygon || tool == Tool.DrawPolyline)
                this.toolPictureBox.Icon = ToolbarMaker.GetIconFromFile(config.ApplicationDirectory + @"\Icons\tbDraw");
            else if (tool == Tool.Move)
                this.toolPictureBox.Icon = ToolbarMaker.GetIconFromFile(config.ApplicationDirectory + @"\Icons\toolPan");
            else if (tool == Tool.ZoomIn)
                this.toolPictureBox.Icon = ToolbarMaker.GetIconFromFile(config.ApplicationDirectory + @"\Icons\toolZoomIn");
            else if (tool == Tool.ZoomOut)
                this.toolPictureBox.Icon = ToolbarMaker.GetIconFromFile(config.ApplicationDirectory + @"\Icons\toolZoomOut");
            else if (tool == Tool.Pointer)
                this.toolPictureBox.Icon = ToolbarMaker.GetIconFromFile(config.ApplicationDirectory + @"\Icons\toolPointer");
        }

        public MainControler MainControler {
            get { return mainControler; }
        }

        private void readyButton_Click(object sender, EventArgs e)
        {
            if (!screenIsLocked)
            {
                if (mapPanel.SelectedTool == Tool.DrawPolyline)
                    mainControler.PerformAction(new FinishDrawingAction(LayerType.PolylineCanvas, mainControler.LayerManager));
                else if (mapPanel.SelectedTool == Tool.DrawPolygon)
                    mainControler.PerformAction(new FinishDrawingAction(LayerType.PolygonCanvas, mainControler.LayerManager));
            }
        }

        public void showReadyButton() {
            readyButton.Visible = true;
            // disable the drawing tools button
            toolBar.Buttons[4].Enabled = false;
        }
        //show/hide keyboard
        public void enableInputPanel(bool enable)
        {
            if (enable) this.inputPanel1.Enabled = true;
            else this.inputPanel1.Enabled = false;
        }

        public void layerSettingsButton_Click(object sender, EventArgs e)
        {
            if (layerListView.SelectedIndices.Count > 0)
                mainControler.showLayerLayerSettingsEditor(layerListView.SelectedIndices[0],
                    mainControler.LayerManager.getLayerFromMapping(layerListView.SelectedIndices[0]).Type);
        }

        private void layerListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (layerListView.SelectedIndices.Count != 0)
                layerSettingsButton.Enabled = true;
            else
                layerSettingsButton.Enabled = false;
        }

        void statusBar_MouseUp()
        {
            mainControler.showNavigator();
        }
        public ViewMode ViewMode
        {
            get { return viewMode; }
            set { viewMode = value;
            switch (viewMode)
            {
                case ViewMode.Map:
                    this.tabControl.SelectedIndex = 0;
                    break;
                case ViewMode.Layers:
                    this.tabControl.SelectedIndex = 1;
                    break;
                default:
                    break;
            }
            }
        }
           
    }
}
