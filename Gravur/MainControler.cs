using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using GravurGIS.Shapes;
using GravurGIS.Topology.QuadTree;
using GravurGIS.GUI.Dialogs;
using System.IO;
using System.Reflection;
using MapTools;
using GravurGIS.GPS;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using GravurGIS.Layers;
using GravurGIS.Actions;
using System.Collections;
using System.Xml.Serialization;

namespace GravurGIS
{
    public enum DisplayResolution
    {
        QVGA, VGA
    }

    public class MainControler
    {
        private MainForm mainForm;
        private MapPanel mapPanel;
        private CoordCalculator coordCalculator;
        private LayerManager layerManager;
        private LayerSettingEditor layerSettingsEditor;
        private ImageSettingEditor imageSettingsEditor;
        private Config config = new Config();

        bool m_stateExiting = false;
        private SaveDialog saveDialog = null;
        private SaveFileDialog safeFileDialog = null;
        private OpenFileDialog openFileDialog = null;
        private OverrideDialog overrideDialog = null;
        public Rectangle VisibleRect;
        private SettingDialog settingsDialog = null;

        private List<IDialog> activeDialogs;
        private string openendFileName = "";

        private bool changed = false;

        private bool hasActiveDisplay = false;
        private IIdentifyable selectionLayer = null;

        // Action management
        private int currentAction = 0;
        private ArrayList actionList = new ArrayList();
        private int actionCount = 10;

        public delegate void ActionCountStuffDelegate(int currentAction, int listSize);
        public event ActionCountStuffDelegate ActionlistCountChanged;

        private string defaultProject = "";
		public static double nearlyZero = 0.00001;

        #region GPS
        
        private GPSControler gpsControler = null;
        private bool gpsTrackingRegistered = false;
        private GKCoord currentPositon;
        private LinkedList<ShapeInformation> gpsTrackingList;
        private System.Threading.Timer trackingTimer;
        private System.Threading.TimerCallback timerCallback;

		#endregion
		
        // System Environment
        private DisplayResolution displayResolution = DisplayResolution.QVGA;
       
        #region Events

        public event SettingsLoadedDelegate SettingsLoaded;
		public event WaypointAddedDelegate WaypointAdded;
        private static event WakeUpDelegate systemWakeUp;
		public event WaypointAddedDelegate TrackingStarted;
		public event NoParamDelegate TrackingStopped;
        
        #endregion
        
        #region Delegates

		public delegate void NoParamDelegate();
		public delegate void WaypointAddedDelegate();
		public delegate void SettingsLoadedDelegate(Config config);
		public delegate void WakeUpDelegate();
		public delegate void KeyPressedDelegate();
		private delegate void CloseDelegate();

		private KeyPressedDelegate upButtonDownAction;
		private KeyPressedDelegate downButtonDownAction;
		private KeyPressedDelegate leftButtonDownAction;
		private KeyPressedDelegate rightButtonDownAction;
		private KeyPressedDelegate enterButtonDownAction;

		private KeyPressedDelegate upButtonUpAction;
		private KeyPressedDelegate downButtonUpAction;
		private KeyPressedDelegate leftButtonUpAction;
		private KeyPressedDelegate rightButtonUpAction;
		private KeyPressedDelegate enterButtonUpAction;

		private SortedList<HardwareKeyMappings, KeyPressedDelegate> hardwareKeyDownFunctionMapping;
		private SortedList<HardwareKeyMappings, KeyPressedDelegate> hardwareKeyUpFunctionMapping;
		
        #endregion

        private const int NOTIFICATION_EVENT_WAKEUP = 11;
        private const UInt32 INFINITE = 0xFFFFFFFF;
        private const int NATIVE_EVENT_SET = 3;
        private static System.Threading.Thread _wakeupServiceThread;
        private static bool _close = false;
        private static IntPtr hWakeUpEvent;

        #region Imports

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern bool CeRunAppAtEvent(string AppName,
            int WhichEvent);

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern int WaitForSingleObject(IntPtr hHandle,
            UInt32 dwMilliseconds);

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern IntPtr CreateEvent(IntPtr lpEventAttributes,
                bool bManualReset, bool bInitialState, string spName);

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern bool EventModify(IntPtr hEvent,
            [In, MarshalAs(UnmanagedType.U4)] int dEvent);

        #endregion

        ////////////////////////////////
        // Constructors & Destructors
        ////////////////////////////////

        public MainControler(String appName)
        {
            CheckEnvironment();

            coordCalculator = new CoordCalculator(this);

            config.SetDefaults();

            layerManager    = new LayerManager(this, config); // must be initialized first
            mainForm        = new MainForm(this, config, appName);

			// initialize the hardware key function mappings
			hardwareKeyDownFunctionMapping = new SortedList<HardwareKeyMappings, KeyPressedDelegate>();
			hardwareKeyDownFunctionMapping.Add(HardwareKeyMappings.TempMove, delegate() { mainForm.toggleTemporaryTool(Tool.Move); });
			hardwareKeyDownFunctionMapping.Add(HardwareKeyMappings.GPSTrackingTrigger, StartGPSTracking);
			hardwareKeyDownFunctionMapping.Add(HardwareKeyMappings.GPSTrackingStop, StopGPSTracking);
			hardwareKeyDownFunctionMapping.Add(HardwareKeyMappings.None, null);

			hardwareKeyUpFunctionMapping = new SortedList<HardwareKeyMappings, KeyPressedDelegate>();
			hardwareKeyUpFunctionMapping.Add(HardwareKeyMappings.TempMove, delegate() { mainForm.toggleTemporaryTool(Tool.Move); });
			hardwareKeyUpFunctionMapping.Add(HardwareKeyMappings.GPSTrackingTrigger, null);
			hardwareKeyUpFunctionMapping.Add(HardwareKeyMappings.GPSTrackingStop, null);
			hardwareKeyUpFunctionMapping.Add(HardwareKeyMappings.None, null);

            Point mapTabClientArea = mainForm.getMapTabClientArea();
            mapPanel = new MapPanel(this, config, mainForm, mapTabClientArea.X,
                mapTabClientArea.Y, 2);

            

            this.SettingsLoaded += new SettingsLoadedDelegate(MainControler_SettingsLoaded);

            LoadSettings(Program.AppDataPath + "\\" + config.ConfigFileName);

            mainForm.InitializeMyOwnComponents(mapPanel);
            activeDialogs = new List<IDialog>();

            //layerManager.ScaleChanged += new LayerManager.ScaleChangedDelegate(layerManager_ScaleChanged);
            //layerManager.DrawInfChanged += new LayerManager.DrawShapeInformationDelegate(layerManager_DrawInfChanged);
            layerManager.DrawInfChanged += new LayerManager.DrawShapeInformationDelegate(mainForm.OnDrawInfChanged);
            layerManager.DrawInfChanged += new LayerManager.DrawShapeInformationDelegate(mapPanel.OnDrawInfChanged);

            this.SettingsLoaded += new SettingsLoadedDelegate(this.SaveSettings);
            actionList.Clear();

            if (config.UseDefaultProject && File.Exists(config.DefaultProject))
                if (!openMWD(config.DefaultProject)) config.UseDefaultProject = false;
                else this.MainForm.ViewMode = ViewMode.Map;


            SystemWakeUp += new WakeUpDelegate(MainControler_SystemWakeUp);

            
            Program.NotifyIcon.Click += new EventHandler(notifyIcon_Click);
            mainForm.Deactivate += new EventHandler(mainForm_Deactivate);
            mainForm.Activated += new EventHandler(mainForm_Activated);
        }
        
        public void HardwareKeyDown(HardwareKeys key) {
			if ( key == HardwareKeys.Up)
			{
				if (upButtonDownAction != null) upButtonDownAction();
			}
			if (key == HardwareKeys.Down)
			{
				if (downButtonDownAction != null) downButtonDownAction();
			}
			if (key == HardwareKeys.Left)
			{
				if (leftButtonDownAction != null) leftButtonDownAction();
			}
			if (key == HardwareKeys.Right)
			{
				if (rightButtonDownAction != null) rightButtonDownAction();
			}
			if (key == HardwareKeys.Enter)
			{
				if (enterButtonDownAction != null) enterButtonDownAction();
			}
        }

		public void HardwareKeyUp(HardwareKeys key)
		{
			if (key == HardwareKeys.Up)
			{
				if (upButtonUpAction != null) upButtonUpAction();
			}
			if (key == HardwareKeys.Down)
			{
				if (downButtonUpAction != null) downButtonUpAction();
			}
			if (key == HardwareKeys.Left)
			{
				if (leftButtonUpAction != null) leftButtonUpAction();
			}
			if (key == HardwareKeys.Right)
			{
				if (rightButtonUpAction != null) rightButtonUpAction();
			}
			if (key == HardwareKeys.Enter)
			{
				if (enterButtonUpAction != null) enterButtonUpAction();
			}
		}

        void mainForm_Activated(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Is called when the mainform gets deactivated / hidden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mainForm_Deactivate(object sender, EventArgs e)
        {
            
        }

        void notifyIcon_Click(object sender, EventArgs e)
        {
            // activate the form again
            mainForm.Activate();
        }

        void MainControler_SystemWakeUp()
        {
            if (gpsControler != null)
            {
                gpsControler.EnsureDeviceConnection();
            }
        }

        void MainControler_SettingsLoaded(Config config)
        {
            if ( ( config.UseGPS && layerManager.GPSLayer == null)
				|| gpsControler == null || !gpsControler.Opened) {
                addGPSLayer();
            } else if (!config.UseGPS) {
                removeGPSLayer();
            }
            
			this.hardwareKeyDownFunctionMapping.TryGetValue(config.KeyMapping_Up, out upButtonDownAction);
			this.hardwareKeyDownFunctionMapping.TryGetValue(config.KeyMapping_Down, out downButtonDownAction);
			this.hardwareKeyDownFunctionMapping.TryGetValue(config.KeyMapping_Left, out leftButtonDownAction);
			this.hardwareKeyDownFunctionMapping.TryGetValue(config.KeyMapping_Right, out rightButtonDownAction);
			this.hardwareKeyDownFunctionMapping.TryGetValue(config.KeyMapping_Enter, out enterButtonDownAction);

			this.hardwareKeyUpFunctionMapping.TryGetValue(config.KeyMapping_Up, out upButtonUpAction);
			this.hardwareKeyUpFunctionMapping.TryGetValue(config.KeyMapping_Down, out downButtonUpAction);
			this.hardwareKeyUpFunctionMapping.TryGetValue(config.KeyMapping_Left, out leftButtonUpAction);
			this.hardwareKeyUpFunctionMapping.TryGetValue(config.KeyMapping_Right, out rightButtonUpAction);
			this.hardwareKeyUpFunctionMapping.TryGetValue(config.KeyMapping_Enter, out enterButtonUpAction);
        }

        ~MainControler()
        {
            
        }

        ////////////////
        // WakeUp Event
        ////////////////

        public static event WakeUpDelegate SystemWakeUp
        {
            add
            {
                if (systemWakeUp == null)
                    StartOnWakeUpService();
                systemWakeUp += value;
            }
            remove
            {
                // For closing the thread is cared in closing the application
                systemWakeUp -= value;
            }
        }

        private static void StartOnWakeUpService()
        {
            try
            {
                CeRunAppAtEvent(
                     "\\\\.\\Notifications\\NamedEvents\\NativeDeviceWakeUpEvent",
                     NOTIFICATION_EVENT_WAKEUP);

                _wakeupServiceThread = new System.Threading.Thread(new System.Threading.ThreadStart(WakeupProc));
                _wakeupServiceThread.IsBackground = true;
                _wakeupServiceThread.Start();
            }
            catch { }
        }

        private static void WakeupProc()
        {
            hWakeUpEvent = CreateEvent(IntPtr.Zero, false, false,
                 "NativeDeviceWakeUpEvent");
            while (!_close)
            {
                WaitForSingleObject(hWakeUpEvent, INFINITE);
                if (systemWakeUp != null && !_close)
                    systemWakeUp();
            }
        }

        ////////////////
        // Methods
        ////////////////

        private void CheckEnvironment()
        {           
            // display
            int resolutionWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            int resolutionHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            if (resolutionHeight >= 640 && resolutionWidth >= 480) this.displayResolution = DisplayResolution.VGA;
            else this.displayResolution = DisplayResolution.QVGA;

            SetUpAppLink();
        }

        /// <summary>
        /// This functions restores a potentially lost program link in
        /// the main menu of Windows Mobile
        /// </summary>
        private void SetUpAppLink()
        {
            // get application name
            string appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            // typical path for link file: \Windows\Startmenu\Program Files\Gravur.lnk
            string pathLinkFile = Environment.GetFolderPath(Environment.SpecialFolder.Programs) + "\\" + appName + ".lnk";

            // get path of application and add "'s
            string exePath = "\"" + System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase + "\"";

            // create link file, content here: 34#"\Program Files\Gravur\Gravur.exe"
            System.IO.StreamWriter swLnkFile = System.IO.File.CreateText(pathLinkFile);
            swLnkFile.WriteLine(exePath.Length.ToString() + "#" + exePath);
            swLnkFile.Close();
        }

        public void ClearTransportLayer(LayerType type, string name)
        {
            if (MessageBox.Show(
                String.Format("Möchten Sie die {0} wirklich leeren?{1}Dabei gehen alle Informationen auf dieser Ebene verloren!", name, Environment.NewLine),
                "Sind Sie sicher?",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                layerManager.ClearTransportLayer(type);                
            }
        }

        private static bool SetEvent(IntPtr hEvent)
        {
            return EventModify(hEvent, NATIVE_EVENT_SET);
        }

        public void ExitProgram()
        {
            m_stateExiting = true;

            DialogResult result = DialogResult.Ignore;
            if (changed) result = showSaveChangesDialog();
            if (result == DialogResult.Ignore || result == DialogResult.Yes)
            {
                _close = true;
                if (_wakeupServiceThread != null)
                {
                    // if we have created the WakeUpEvent we need to fire it in order
                    // to get out of WaitForSingleObject(hEvent, INFINITE) in the waiting thread
                    if (hWakeUpEvent != IntPtr.Zero)
                        SetEvent(hWakeUpEvent);
                }

                StopGPS();

                // Fixes Ticket #52
                // in principal we do not need this - but if we leave it away
                // we will get a ObjectDisposedException when closing down
                // (I think this is caused by some event which the form receives
                // but is already in disposed state)
                // we can work around this by calling the close method asynchonely
                // as a PostMessage - this is done by BeginInvoke
                mainForm.BeginInvoke(new CloseDelegate(mainForm.Close));
                
                Application.Exit();
            }
        }

        private void LoadSettings(string filePath)
        {
            FileStream fs = null;

            try
            {
                if (File.Exists(filePath))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(Config));
                    
                    fs = new FileStream(filePath, FileMode.Open);
                    config = (Config)ser.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                //File.Delete(filePath);
                MessageBox.Show(
                    String.Format("Fehler beim Laden der Einstellungen!\nVerwende Standardwerte.{0}Fehlermeldung: {1}", Environment.NewLine, ex.Message));
            }
            finally
            {
                if (fs != null) fs.Close();

                // we have to initialize some default values which are not explicitely saved in the XML file
                // for this reason we have to this no matter if we loaded the file or not
                config.InitStaticData();
            }

            OnSettingsChanged();
        }
        
        public void OnSettingsChanged() {
			if (SettingsLoaded != null) SettingsLoaded(config);
        }

        public void LoadDefaultSettings()
        {
            if (MessageBox.Show("Aktuelle Einstellungen überschreiben\nund Standard-Einstellungen laden?",
                "Standard-Einstellungen",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {

                this.config = new Config();
                config.SetDefaults();
                SettingsLoaded(this.config);
            }
        }

        public void SaveSettings(Config configToSave)
        {
            string appSavePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + Config.ProgramName;
            TextWriter tw = null;
            try
            {
                if (!Directory.Exists(appSavePath))
                    Directory.CreateDirectory(appSavePath);

                XmlSerializer ser = new XmlSerializer(typeof(Config));
                tw = new StreamWriter(appSavePath + "\\" + configToSave.ConfigFileName, false);
                ser.Serialize(tw, configToSave);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    String.Format(
                    "Fehler 0x8000 - Konnte Einstellungen nicht in Datei {0}{1} speichern!{2}Fehlermeldung: {3}", appSavePath,
                    configToSave.ConfigFileName, Environment.NewLine, ex.Message));
            }
            finally
            {
                if (tw != null) tw.Close();
            }
        }

        public void openMWD()
        {
            if (openFileDialog == null) openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "MWD Dateien (*.mwd)|*.mwd";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
                openMWD(openFileDialog.FileName);
        }
        public bool openMWD(string fileName)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                layerManager.openMWD(fileName);
                openendFileName = fileName;
            }
            catch (Exception)
            {
                MessageBox.Show("Fehler 0x2312: Konnte MWD-Datei \"" + fileName + "\" nicht laden!");
                return false;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            return true;
        }

        public void importMWD()
        {
            if (openFileDialog == null) openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "MWD Dateien (*.mwd)|*.mwd";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                layerManager.importMWD(openFileDialog.FileName);
                Cursor.Current = Cursors.Default;
            }
        }

        public void addLogMessage(String str)
        {
            mainForm.addLogMessage(str);
        }

        public void deleteAllLayers() {
            for (int i = mainForm.LayerListView.Items.Count - 1; i>=0; i--)
            {
                layerManager.removeLayer(i);
                mainForm.LayerListView.Items.Remove(mainForm.LayerListView.Items[i]);
            }
            mainForm.clearLayerList();
        }

        /// <summary>
        /// Sorts a list of quadtree items of IShapes in a way that points are first
        /// followed by polylines and polygons.
        /// </summary>
        /// <param name="?">To sorting itemList</param>
        public void sortItemList(ref List<QuadTreePositionItem<IShape>> itemList) {
            int lastIndex = itemList.Count - 1;
            int stop = 0;
            for (int i = lastIndex; i >= stop; i--)
            {
                if (itemList[i].Parent.Type == MapTools.ShapeLib.ShapeType.MultiPoint)
                {
                    itemList.Insert(0, itemList[i]);
                    itemList.RemoveAt(i + 1);
                    stop++;
                }
                else if (itemList[i].Parent.Type == MapTools.ShapeLib.ShapeType.Polygon)
                {
                    itemList.Insert(lastIndex + 1, itemList[i]);
                    itemList.RemoveAt(i);
                }
            }
        }

        public void showErrorMessage(String message)
        {
            MessageBox.Show(message);
        }

        public String GetUserInput(String caption, String message)
        {
            InputDialog inputDialog =
                new InputDialog(caption, message,
                    mainForm.VisibleDesktop);

            activeDialogs.Add(inputDialog);
            mainForm.enableInputPanel(true);

            String result = String.Empty;
            if (inputDialog.ShowDialog() == DialogResult.OK)
            {
                result = inputDialog.UserInput;
            }

            activeDialogs.Remove(inputDialog);
            mainForm.enableInputPanel(false);

            inputDialog.Dispose();

            return result;
        }

        public void showCommentDialog()
        {
            CommentDialog commentDialog = new CommentDialog(this, mainForm.VisibleDesktop);
            commentDialog.Comment = layerManager.CommentOfSelectedShape;
            commentDialog.Category = layerManager.CategoyOfSelevtedShape;

            activeDialogs.Add(commentDialog);
            
            mainForm.enableInputPanel(true);

            if (commentDialog.ShowDialog() == DialogResult.OK)
            {
                layerManager.CommentOfSelectedShape =commentDialog.Comment;
                layerManager.CategoyOfSelevtedShape =commentDialog.Category;
                commentDialog.resetComment();
            }
            activeDialogs.Remove(commentDialog);
            mainForm.enableInputPanel(false);

            commentDialog.Dispose();
        }

        public void showAboutDialog()
        {
            MessageBox.Show(String.Format("timberNet GravurGIS{0}Version: {1}",
					Environment.NewLine,
					Assembly.GetExecutingAssembly().GetName().Version),
                String.Format("Über {0}", Config.ProgramName),
                MessageBoxButtons.OK,
                MessageBoxIcon.Asterisk,
                MessageBoxDefaultButton.Button1);
        }

        public void showSettingsDialog()
        {
            if (settingsDialog == null)
                settingsDialog = new SettingDialog(mainForm.VisibleDesktop, this);

            activeDialogs.Add(settingsDialog);
            if (settingsDialog.ShowDialog(config) == DialogResult.OK)
                SettingsLoaded(config);
            activeDialogs.Remove(settingsDialog);
        }

        public void showLayerLayerSettingsEditor(int selectedLayerListIndex, LayerType layerType)
        {
            Layer layer = null;
            if (selectedLayerListIndex >= 0)
                layer = layerManager.getLayerFromMapping(selectedLayerListIndex);

            if (layerType == LayerType.Shape && selectedLayerListIndex >= 0)
            {
                if (layerSettingsEditor == null)
                    layerSettingsEditor = LayerSettingEditor.Instance(mainForm.VisibleDesktop, this);
                activeDialogs.Add(layerSettingsEditor);

                if (layer.Type == GravurGIS.Layers.LayerType.Shape)
                {
                    mainForm.enableInputPanel(true);
                    if (layerSettingsEditor.ShowDialog((ShapeObject)layer) == DialogResult.OK)
                    {
                        mapPanel.ScreenChanged = true;
                        mainForm.updateListView();
                    }
                }
                activeDialogs.Remove(layerSettingsEditor);
            }
            else if ((layerType == LayerType.PointCanvas
                || layerType == LayerType.PolylineCanvas
                || layerType == LayerType.PolygonCanvas) && selectedLayerListIndex == -1)
            {
                if (layerSettingsEditor == null)
                    layerSettingsEditor = LayerSettingEditor.Instance(mainForm.VisibleDesktop, this);
                        activeDialogs.Add(layerSettingsEditor);

                mainForm.enableInputPanel(true);
                if (layerSettingsEditor.ShowDialog(layerType) == DialogResult.OK)
                {
                    mapPanel.ScreenChanged = true;
                    mainForm.updateListView();
                }
                activeDialogs.Remove(layerSettingsEditor);
            }
            else if (layerType == LayerType.Image)
            {
                if (imageSettingsEditor == null)
                    imageSettingsEditor = new ImageSettingEditor(mainForm.VisibleDesktop, this);
                activeDialogs.Add(imageSettingsEditor);

                mainForm.enableInputPanel(true);
                if (imageSettingsEditor.ShowDialog((ImageLayer)layer) == DialogResult.OK)
                {
                    mapPanel.ScreenChanged = true;
                    mainForm.updateListView();
                }
                activeDialogs.Remove(imageSettingsEditor);
            }
            else if (layerType == LayerType.MapServer)
            {
                MapServerSettings mapServerDialog = new MapServerSettings(mainForm.VisibleDesktop, this);
                activeDialogs.Add(mapServerDialog);

                mainForm.enableInputPanel(true);
                if (mapServerDialog.ShowDialog((MapServerLayer)layer) == DialogResult.OK)
                {
                    mapPanel.ScreenChanged = true;
                    mainForm.updateListView();
                }
                activeDialogs.Remove(mapServerDialog);
            }
            
            mainForm.enableInputPanel(false);
        }

        public string askForDefaultLayer()
        {
            if (openFileDialog == null) openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MWD Projekt Dateien (*.mwd)|*.mwd";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.defaultProject = openFileDialog.FileName;
                return this.defaultProject;
            }
            return "(keins)";

        }

        public bool identify(double x, double y)
        {
            ILayer layer;
            IIdentifyable idLayer;

            if (mainForm.LayerListView.SelectedIndices.Count > 0)
            {
                layer = layerManager.getLayerFromMapping(mainForm.LayerListView.SelectedIndices[0]);

                if (layer is IIdentifyable)
                {
                    idLayer = layer as IIdentifyable;

                    // clear the old selection
                    if (selectionLayer != null)
                        selectionLayer.clearSelection();

                    if (idLayer != null)
                    {
                        selectionLayer = idLayer;

                        Cursor.Current = Cursors.WaitCursor;
                        if (!idLayer.IsIndexed) setStatus("Erstelle Layerindex...");
                        int[] results = idLayer.identify(x, y);

                        Cursor.Current = Cursors.Default;

                        if (results != null && results.Length != 0)
                        {
                            mainForm.setStatus_Timed(results.Length.ToString() + " Elemente gefunden", 3000);
                            return true;
                        }
                        else
                            mainForm.setStatus_Timed("Keine Elemente gefunden", 3000);
                    }
                }
            }
            else
            {
                mainForm.setStatus_Timed("Es wurde kein Layer ausgewählt!", 1000);
            }

            return false;
        }

        // TODO: Does not work when shape layers are already initialized
        // Bzgl. ToDo: Sicher dass das so ist?
        public void createNewCanvas(double dX, double dY,double scale)
        {
            if (layerManager.LayerCount == 0)
            {
                mapPanel.DX = dX;
                mapPanel.DY = dY;
                mapPanel.AbsolutZoom = 1;
                layerManager.Scale = scale;
            }

            hasActiveDisplay = true;
            mapPanel.Visible = true;
        }


        public void save()
        {
            lock (openendFileName)
            {
                if (openendFileName == "") //safe new mwd
                    showSaveDialog();
                else //modify loaded mwd --> no safeFileDialog
                {
                    try
                    {
                        layerManager.saveAsMWD(openendFileName);
                        this.changed = false;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Fehler 0x9120: Speichern nicht möglich!\n" + e.Message);
                        showSaveDialog();
                    }
                    layerManager.saveTransPortLayers();
                }
            }
        }

        private void saveNewProject() // Geht noch nicht so wie es soll
        {
            if (safeFileDialog == null) safeFileDialog = new SaveFileDialog();

            safeFileDialog.Filter = "MWD Projekt Dateien (*.mwd)|*.mwd";
            if (safeFileDialog.ShowDialog() == DialogResult.OK)
            {
                openendFileName = safeFileDialog.FileName;
                layerManager.saveAsMWD(openendFileName);
            }
        }

        private DialogResult showSaveChangesDialog()
        {
			DialogResult dlgResult;
            using ( CloseDialog closeDialog = new CloseDialog(mainForm.VisibleDesktop, this) ) {
				activeDialogs.Add(closeDialog);
				dlgResult = closeDialog.ShowDialog();
				if (dlgResult == DialogResult.Yes)
				{
					SaveChagesType result = closeDialog.SaveType;
					this.changed = false;

					if ((result & SaveChagesType.SaveOpenedProject)
						== SaveChagesType.SaveOpenedProject)
					{
						try
						{
							layerManager.saveAsMWD(openendFileName);
						}
						catch (Exception e)
						{
							MessageBox.Show("Fehler 0x9121: Speichern in geöffnetem Projekt nicht möglich!\n" + e.Message);
							saveNewProject();
						}
					}
					else
					{
						saveNewProject();
					}

					if ((result & SaveChagesType.SaveTransportLayers)
						== SaveChagesType.SaveTransportLayers)
					{
						layerManager.saveTransPortLayers();
					}

					if ((result & SaveChagesType.SetAsDefault)
						== SaveChagesType.SetAsDefault)
					{
						config.UseDefaultProject = true;
						config.DefaultProject = openendFileName;
					} else {
						if (config.DefaultProject == openendFileName)
							config.UseDefaultProject = false;
					}
					SaveSettings(config);
				}
	            
				activeDialogs.Remove(closeDialog);
            }

            return dlgResult;
        }

        public void showSaveDialog()
        {
            if (saveDialog == null)
                saveDialog = new SaveDialog(mainForm.VisibleDesktop, this);
            activeDialogs.Add(saveDialog);
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                if (saveDialog.SaveType == SaveType.Composition || saveDialog.SaveType == SaveType.Project)
                    saveNewProject();
                if (saveDialog.SaveType == SaveType.Composition || saveDialog.SaveType == SaveType.TransPortLayer)
                    layerManager.saveTransPortLayers();
            }
            activeDialogs.Remove(saveDialog);
        }

        public void SIPStateChanged()
        {
            Rectangle visRect = mainForm.VisibleDesktop;
            foreach (IDialog dialog in activeDialogs)
                dialog.resizeToRect(visRect);
        }

        public bool GPSAvailable
        {
            get
            {
                return (gpsControler != null && gpsControler.Opened);
            }
        }

        public void showNavigator()
        {
            Navigator navigator = new Navigator(mainForm.VisibleDesktop, this);
            activeDialogs.Add(navigator);
            mainForm.enableInputPanel(false);

            if (navigator.ShowDialog() == DialogResult.OK)
            {
                // apply the values
                mapPanel.ScaleDivisor = navigator.NavigatorScale;
                mapPanel.SetPosition(navigator.NavigatorPosition, navigator.HorizontalAlignment, navigator.VerticalAlignment);
            }

            activeDialogs.Remove(navigator);
            mainForm.enableInputPanel(false);

            navigator.Dispose();
        }

        public OverrideInformation showOverrideDialog(OverrideInformation enabledInformation)
        {
            string tempName;
            if (overrideDialog == null)
                overrideDialog = new OverrideDialog(mainForm.VisibleDesktop, this);
            if (overrideDialog.ShowDialog(ref enabledInformation) == DialogResult.OK)
            {
                try
                {
                    OverrideInformation overrideInfo = overrideDialog.WhatToOverride;

                    if (overrideInfo.PointChecked)
                    {
                        tempName = Path.ChangeExtension(config.ExPntLayerFile, "shp");
                        if (File.Exists(tempName)) File.Delete(tempName);
                        tempName = Path.ChangeExtension(tempName,"shx");
                        if (File.Exists(tempName)) File.Delete(tempName);
                        tempName = Path.ChangeExtension(tempName, "dbf");
                        if (File.Exists(tempName)) File.Delete(tempName);
                    }
                    if (overrideInfo.PolylineChecked)
                    {
                        tempName = Path.ChangeExtension(config.ExPLineLayerFile, "shp");
                        if (File.Exists(tempName)) System.IO.File.Delete(tempName);
                        tempName = Path.ChangeExtension(tempName, "shx");
                        if (File.Exists(tempName)) System.IO.File.Delete(tempName);
                        tempName = Path.ChangeExtension(tempName, "dbf");
                        if (File.Exists(tempName)) System.IO.File.Delete(tempName);
                    }
                    if (overrideInfo.PolygonChecked)
                    {
                        tempName = Path.ChangeExtension(config.ExPGonLayerFile, "shp");
                        if (File.Exists(tempName)) System.IO.File.Delete(tempName);
                        tempName = Path.ChangeExtension(tempName, "shx");
                        if (File.Exists(tempName)) System.IO.File.Delete(tempName);
                        tempName = Path.ChangeExtension(tempName, "dbf");
                        if (File.Exists(tempName)) System.IO.File.Delete(tempName);
                    }
                    return overrideInfo;
                }
                catch (Exception)
                {
                    MessageBox.Show("Fehler beim Überschreiben. - Vorgang abgebrochen!");
                    return new OverrideInformation(false, false, false, DialogResult.Abort);
                }
            }
            return new OverrideInformation(false, false, false, DialogResult.Cancel);
        }

        public void setStatus(string str)
        {
            mainForm.setStatusBarText(str);
        }
        public void setStatusTimed(string str, int msec)
        {
            mainForm.setStatus_Timed(str, msec);
        }

        /// <summary>
        /// Importiert Transportlayer - also Shapefiles die bearbeitet werden können.
        /// Je nach Parameter werden die "alten" TransportLayer vorher gelöscht.
        /// </summary>
        /// <param name="clearBeforeImport">true: Der alte Transportlayer vom geladenen Typ wird gelöscht</param>
        /// <returns></returns>
        public void importTransportLayer(bool clearBeforeImport)
        {
            if (openFileDialog == null) openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Shapefile (*.shp)|*.shp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                layerManager.ImportAsTransportLayer(openFileDialog.FileName, clearBeforeImport);
                Cursor.Current = Cursors.Default;
            }
        }

        public void addShapeFile()
        {
            if (openFileDialog == null) openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Shapefile (*.shp)|*.shp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                layerManager.addShapefileLayer(openFileDialog.FileName);
                Cursor.Current = Cursors.Default;
            }
        }

        public void addMapserverLayer()
        {
            MapServerSettings mapServerDialog = new MapServerSettings(mainForm.VisibleDesktop, this);
            activeDialogs.Add(mapServerDialog);
            mainForm.enableInputPanel(true);
            if (mapServerDialog.ShowDialog(null) == DialogResult.OK)
            {
                if (mapServerDialog.CurrentLayer != null)
                    layerManager.addMapserverLayer(mapServerDialog.CurrentLayer);
            }

            mainForm.enableInputPanel(false);
            activeDialogs.Remove(mapServerDialog);
        }

        public void addLayer()
        {
			try
			{
                if (openFileDialog == null) openFileDialog = new OpenFileDialog();

                openFileDialog.Filter = "Shapefile (*.shp)|*.shp|GeoTiff (*.tif, *.tiff)|*.tif;*.tiff";
                openFileDialog.FilterIndex = 1; // FilterIndex startes with 1!

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    if (openFileDialog.FilterIndex == 1)
                        layerManager.addShapefileLayer(openFileDialog.FileName);
                    else if (openFileDialog.FilterIndex == 2)
                        layerManager.addImageLayer(openFileDialog.FileName);

                }
			}
			catch (Exception e)
			{
			    MessageBox.Show(String.Format(
			        "Der neue Layer konnte nicht hinzugefügt werden.{0}Fehler: {1}", Environment.NewLine, e.Message), "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
			}
			finally
			{
			    Cursor.Current = Cursors.Default;
			}
        }

        public void addGeoImage()
        {
            if (openFileDialog == null) openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "GeoTIFF (*.tif)|*.tif";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                layerManager.addImageLayer(openFileDialog.FileName);
                Cursor.Current = Cursors.Default;
            }
        }

        public void addOGRLayer()
        {
            if (openFileDialog == null) openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Shapefile (*.shp)|*.shp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                layerManager.addOGRLayer(openFileDialog.FileName);
                Cursor.Current = Cursors.Default;
            }
        }

        public void addMandelbrot()
        {
            Cursor.Current = Cursors.WaitCursor;
            layerManager.addMandelbrotLayer(0, 0, mapPanel.Width,
                mapPanel.Height, 100, -0.7, 0, 3);
            Cursor.Current = Cursors.Default;
        }

		private void OnTrackingStarted()
		{
			if (TrackingStarted != null) TrackingStarted();
		}

		private void OnTrackingStopped()
		{
			if (TrackingStopped != null) TrackingStopped();
		}
        
        public void StartGPSTracking() {
			if (config.GPSTracking) {
				if (gpsTrackingList == null)
					gpsTrackingList = new LinkedList<ShapeInformation>();

				if (gpsTrackingList.Count == 0)
				{
					OnTrackingStarted();
				}

				if (config.GPSTrackingTriggerMode == GpsTrackingTriggerMode.TimeInterval)
					startTimedGPSTracker();
				else
					trackCurrentPosition();
			}
        }
        
        public void StopGPSTracking() {
			if (config.GPSTracking) {
				if (config.GPSTrackingTriggerMode == GpsTrackingTriggerMode.TimeInterval)
					stopTimedGPSTracker();
					
				GPS_FinishTrackedShape();
				OnTrackingStopped();
			}
        }
        
        private void startTimedGPSTracker() {
			if (trackingTimer == null) {
				// TODO: das klappt so nicht da da ja ein neuer Thread erstelt wird und in der Draw-Action dann invalidiert
				// wird- vielleicht sollten wir das umbauen auf Events (kein direkte Invalidate Aufruf mehr)
				
				//trackingTimer = new System.Threading.Timer(
				//    delegate(object param) { (param as MainControler).trackCurrentPosition(); },
				//    this, 0, config.GPSTrackingTimeInterval);
			}
        }
               
        private void stopTimedGPSTracker() {
			if (trackingTimer != null) trackingTimer.Dispose();
        }
        
        public void trackCurrentPosition() {

			// have we to filter the positon?
			if (config.GPSTrackingDiscardSamePositions)	{		
				foreach (ShapeInformation shpInfo in gpsTrackingList) {
					// check if we can discard this point
					// we have to add the h_value since the TODO below is still to be done
					if (   (Math.Abs(shpInfo.iShapeInf.CenterX - currentPositon.r_value) < nearlyZero)
						&& (Math.Abs(shpInfo.iShapeInf.CenterY + currentPositon.h_value) < nearlyZero) )
						return;
				}
			}
			
			// TODO: The "-" in front of currentPosition.h_value is actually wrong - but there is a bug
			// in the model, so that one needs to pass the negative variable - FIX THIS
			DrawAction action = new DrawAction(MapPanel.AbsolutZoom, currentPositon.r_value, -currentPositon.h_value, this);
			action.ShapeAdded += new DrawAction.ShapeAddedDelegate(GPSTrackedShapeAdded);
			
			if (PerformAction(action)) {
				setStatusTimed("Neuen Wegpunkt (GPS) gesetzt!", 2000);
				OnWaypointAdded();
			} else {
				// delete the event handler, it is not needed anymore
				action.ShapeAdded -= GPSTrackedShapeAdded;
				setStatusTimed("Wegpunkt (GPS): Nicht gesetzt (Fehler)", 3000);
			}
        }

		private void OnWaypointAdded()
		{
			if (WaypointAdded != null) WaypointAdded();
		}

		void GPSTrackedShapeAdded(ShapeInformation info, DrawAction sender)
		{
			// wir brauchen den Service ja nur einmal in Anspruch nehmen
			sender.ShapeAdded -= GPSTrackedShapeAdded;

			this.gpsTrackingList.AddLast(info);
		}
		
		void GPS_FinishTrackedShape() {
			// ask for interpretation of the points
			// (Points, Polyline, Polygon
			
			int nrTrackedPoints = gpsTrackingList.Count;
			
			if (nrTrackedPoints < 1) return;
			
			FinishTrackingDialog ftd = new FinishTrackingDialog(this, mainForm.VisibleDesktop,
					nrTrackedPoints);

			ftd.Comment = config.GPSTrackingComment;
			// ftd.Category = config.GPSTrackingCategory;

			activeDialogs.Add(ftd);

			mainForm.enableInputPanel(false);

			if (ftd.ShowDialog() == DialogResult.OK)
			{				
				switch (ftd.TrackingInterpretation)
				{
					case FinishTrackingDialog.TrackingInterpretations.AsPoints:
						foreach (ShapeInformation shp in gpsTrackingList) {
							shp.iShapeInf.Commment = ftd.Comment;
							shp.iShapeInf.Category  = ftd.Category;
						}
						this.gpsTrackingList.Clear();
						
						break;
					case FinishTrackingDialog.TrackingInterpretations.AsPolyline:
						if (nrTrackedPoints < 2) {
							MessageBox.Show("Um Punkte als Linienzug zu interpretieren müssen mind. zwei vorhanden sein!");
							break;
						}

						ShapeInformation shpinfo = gpsTrackingList.First.Value;

						if (PerformAction(new DrawAction(LayerType.PolylineCanvas,
								shpinfo.iShapeInf.CenterX, shpinfo.iShapeInf.CenterY, false, this, ftd.Comment, ftd.Category)))
						{
							PerformAction(new RemoveShapeAction(shpinfo.quadTreePosItemInf, layerManager));
							gpsTrackingList.RemoveFirst();
						} else {
							MessageBox.Show("Ein Fehler bei der Umwandlung der Punkte in einen Linienzug trat auf.");
							break;
						}

						bool result = true;

						// add points
						foreach (ShapeInformation shp in gpsTrackingList)
						{
							result = PerformAction(new DrawAction(LayerType.PolylineCanvas,
								shp.iShapeInf.CenterX, shp.iShapeInf.CenterY, true, this)) && result;
							result = PerformAction(new RemoveShapeAction(shp.quadTreePosItemInf, layerManager)) && result;
						}
						
						// end polyline editing
						result = result && PerformAction(new FinishDrawingAction(LayerType.PolylineCanvas, layerManager));
						
						if (result)
							this.gpsTrackingList.Clear();
						else
							MessageBox.Show("Ein Fehler bei der Umwandlung der Punkte in einen Linienzug trat auf.");
						
						break;
					case FinishTrackingDialog.TrackingInterpretations.AsPolygon:
						if (nrTrackedPoints < 3)
						{
							MessageBox.Show("Um Punkte als Polygon zu interpretieren müssen mind. drei vorhanden sein!");
							break;
						}

						shpinfo = gpsTrackingList.First.Value;

						if (PerformAction(new DrawAction(LayerType.PolygonCanvas,
								shpinfo.iShapeInf.CenterX, shpinfo.iShapeInf.CenterY, false, this, ftd.Comment, ftd.Category)))
						{
							PerformAction(new RemoveShapeAction(shpinfo.quadTreePosItemInf, layerManager));
							gpsTrackingList.RemoveFirst();
						}
						else
						{
							MessageBox.Show("Ein Fehler bei der Umwandlung der Punkte in ein Polygon trat auf.");
							break;
						}

						result = true;

						// add points
						foreach (ShapeInformation shp in gpsTrackingList)
						{
							result = PerformAction(new DrawAction(LayerType.PolygonCanvas,
								shp.iShapeInf.CenterX, shp.iShapeInf.CenterY, true, this)) && result;
							result = PerformAction(new RemoveShapeAction(shp.quadTreePosItemInf, layerManager)) && result;
						}

						// end polyline editing
						result = result && PerformAction(new FinishDrawingAction(LayerType.PolygonCanvas, layerManager));

						if (result)
							this.gpsTrackingList.Clear();
						else
							MessageBox.Show("Ein Fehler bei der Umwandlung der Punkte in ein Polygon trat auf.");
					
						break;
					default:
						break;
						
				}
			}

			activeDialogs.Remove(ftd);
			mainForm.enableInputPanel(false);

			ftd.Dispose();
		}

        public void addGPSLayer()
        {
            Cursor.Current = Cursors.WaitCursor;
            StartGPS();
            layerManager.addGPSLayer();
			
			if (!gpsTrackingRegistered) {
				layerManager.GPSLayer.PositionChanged += new GPSLayer.PositionChangedDelegate(GPSLayer_PositionChanged);
				gpsTrackingRegistered = true;
			}
			
            Cursor.Current = Cursors.Default;
        }

		void GPSLayer_PositionChanged(GKCoord position)
		{
			currentPositon = position;
		}

        public void removeGPSLayer()
        {
            Cursor.Current = Cursors.WaitCursor;
            //config.UseGPS = false;
            StopGPS();

			if (gpsTrackingRegistered)
			{
				layerManager.GPSLayer.PositionChanged -= GPSLayer_PositionChanged;
				gpsTrackingRegistered = false;
			}
            //layerManager.removeGPSLayer();
            Cursor.Current = Cursors.Default;
        }

        public void removeVertex(IShape container, ShpPoint vertex)
        {
            if (container != null && vertex != null)
            {
               DialogResult result = MessageBox.Show("Möchten Sie diesen Punkt wirklich löschen?", "Punkt löschen", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2);
               if (result == DialogResult.Yes)
               {
                   if (PerformAction(new RemoveGeometryAction(vertex, container, layerManager)))
                   {
                       mainForm.setStatus_Timed("Punkt erfolgreich gelöscht.", 1000);
                       mapPanel.Invalidate();
                   }
                   else
                       mainForm.setStatus_Timed("Punkt nicht gelöscht.", 1000);
               }
            }
        }

        public bool PerformAction(IAction action)
        {
            if (currentAction < actionList.Count)
            {   //perform an action after some Redo's
                int wegSchmeissCount = actionList.Count - currentAction;
                for (int i = 1; i <= wegSchmeissCount; i++)
                    actionList.RemoveAt(actionList.Count-1);
            }

            bool validAction = action.Execute();
            if (validAction)
            {
                actionList.Add(action);
                if (actionList.Count > actionCount)
                {
                    IAction temp = actionList[0] as IAction;
                    temp.Dispose();
                    actionList.RemoveAt(0);
                } //more than actionCount actions in List
                else currentAction++;

                ActionlistCountChanged(currentAction, actionList.Count);

                changed = true;
            }

            return validAction;
        }

        public void UndoAction()
        {
            try
            {
                if (currentAction > 0) //otherwise no Action already performed
                {
                    if (actionList.Count >= currentAction)
                    {
                        IAction action = actionList[--currentAction] as IAction;
                        action.UnExecute();
                    }
                    else
                        MessageBox.Show("Undo-Fehler");
                }
                else MessageBox.Show("Es konnte nichts rückgängig gemacht werden.");
                ActionlistCountChanged(currentAction, actionList.Count);
            }
            catch (Exception e) {
                MessageBox.Show(String.Format(
                    "Fehler beim Zurückgehen (Undo):{0}{1}", Environment.NewLine, e.Message));
            }

        }

        public void RedoAction()
        {
            if (currentAction < actionList.Count)
            {
                IAction action = actionList[currentAction] as IAction;
                action.Execute();
                currentAction++;
                ActionlistCountChanged(currentAction, actionList.Count);
            }
            
           
        }

        public GPSControler StartGPS() 
        {
            if (this.gpsControler == null)
                this.gpsControler = new GPSControler();
            if (!gpsControler.Opened)
                gpsControler.Open();

            return this.gpsControler;
        }

        public void StopGPS()
        {
            if (gpsControler != null && gpsControler.Opened)
                gpsControler.Close();
        }
        
        #region Getter/Setter

		public int CurrentlyCollectedWaypoints { get { return gpsTrackingList.Count; } }

        public MainForm MainForm
        {
            get { return mainForm; }
        }
        public MapPanel MapPanel
        {
            get { return mapPanel; }
        }
        public CoordCalculator CoordCalculator
        {
            get
            {
                if (coordCalculator == null) coordCalculator = new CoordCalculator(this);
                return coordCalculator;
            }
        }
        public LayerManager LayerManager
        {
            get { return layerManager; }
        }

        public string OpenendFileName
        {
            get { return openendFileName; }
            set { openendFileName = value; }
        }
        public string DefaultProject
        {
            get { return defaultProject; }
            set { defaultProject = value; }
        }
        public Config Config
        {
            get { return config; }
        }
        internal DisplayResolution DisplayResolution
        {
            get { return displayResolution; }
        }
        public GPSControler GpsControler
        {
            get { return gpsControler; }
        }

        public bool HasActiveDisplay
        {
            get { return hasActiveDisplay; }
            set { hasActiveDisplay = value; }
        }
        

        #endregion

#if DEVELOP

        internal void stressTest()
        {
            mapPanel.DoStressTest();
        }
        
#endif
    }
}
