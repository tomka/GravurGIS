using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GravurGIS.GUI.Menu
{
    class FileMenu : ContextMenu
    {
        private MainControler mainControler;
        private MenuItem openMenuItem;
        private MenuItem saveMenuItem;
        private MenuItem saveAsMenuItem;
        private MenuItem importMenuItem;
        private MenuItem importTransportMenuItem;
        private MenuItem openTransportMenuItem;
        private MenuItem closeMenuItem;
        private MenuItem loadDefaultSettingsMenuItem;
        private MenuItem openProjectMenuItem;
        private MenuItem importProjectMenuItem;
        private MenuItem aboutMenuItem;
        private MenuItem programMenuItem;
        private MenuItem settingsMenuItem;
#if DEVELOP
		private MenuItem stressTest;
#endif
        
        public FileMenu(MainControler mainControler)
            : base()
        {
            // Das Öffnen-Menu
            openMenuItem = new MenuItem();
            openMenuItem.Text = "Öffnen";
            this.MenuItems.Add(openMenuItem);

            openProjectMenuItem = new MenuItem();
            openProjectMenuItem.Text = "Projekt";
            openProjectMenuItem.Click += new System.EventHandler(menuItemClick);
            openMenuItem.MenuItems.Add(openProjectMenuItem);

            openTransportMenuItem = new MenuItem();
            openTransportMenuItem.Text = "Austauschlayer";
            openTransportMenuItem.Click += new System.EventHandler(menuItemClick);
            openTransportMenuItem.Enabled = false;
            openMenuItem.MenuItems.Add(openTransportMenuItem);

            // Das Importieren-Menu
            importMenuItem = new MenuItem();
            importMenuItem.Text = "Importieren";
            this.MenuItems.Add(importMenuItem);

            importProjectMenuItem = new MenuItem();
            importProjectMenuItem.Text = "Projekt";
            importProjectMenuItem.Click += new System.EventHandler(menuItemClick);
            importMenuItem.MenuItems.Add(importProjectMenuItem);

            importTransportMenuItem = new MenuItem();
            importTransportMenuItem.Text = "Austauschlayer";
            importTransportMenuItem.Click += new System.EventHandler(menuItemClick);
            importTransportMenuItem.Enabled = false;
            importMenuItem.MenuItems.Add(importTransportMenuItem);

            // Das Speichern-Menu

            saveMenuItem = new MenuItem();
            saveMenuItem.Text = "Speichern";
            saveMenuItem.Click += new System.EventHandler(menuItemClick);
            this.MenuItems.Add(saveMenuItem);

            saveAsMenuItem = new MenuItem();
            saveAsMenuItem.Text = "Speichern unter...";
            saveAsMenuItem.Click += new System.EventHandler(menuItemClick);
            this.MenuItems.Add(saveAsMenuItem);
            
            MenuItem separator1 = new MenuItem();
            separator1.Text = "-";
            this.MenuItems.Add(separator1);

            settingsMenuItem = new MenuItem();
            settingsMenuItem.Text = "Einstellungen";
            settingsMenuItem.Click += new System.EventHandler(menuItemClick);
            this.MenuItems.Add(settingsMenuItem);

            loadDefaultSettingsMenuItem = new MenuItem();
            loadDefaultSettingsMenuItem.Text = "Standardeinstellungen laden";
            loadDefaultSettingsMenuItem.Click += new System.EventHandler(menuItemClick);
            this.MenuItems.Add(loadDefaultSettingsMenuItem);

            MenuItem separator2 = new MenuItem();
            separator2.Text = "-";
            this.MenuItems.Add(separator2);

#if DEVELOP
			stressTest = new MenuItem();
            stressTest.Text = "Stresstest";
            stressTest.Click += new EventHandler(menuItemClick);
            this.MenuItems.Add(stressTest);
#endif

            programMenuItem = new MenuItem();
            programMenuItem.Text = "Programm";
            this.MenuItems.Add(programMenuItem);

            aboutMenuItem = new MenuItem();
            aboutMenuItem.Text = "Über...";
            aboutMenuItem.Click += new System.EventHandler(menuItemClick);
            programMenuItem.MenuItems.Add(aboutMenuItem);

            MenuItem separator3 = new MenuItem();
            separator3.Text = "-";
            programMenuItem.MenuItems.Add(separator3);

            closeMenuItem = new MenuItem();
            closeMenuItem.Text = "Beenden";
            closeMenuItem.Click += new System.EventHandler(menuItemClick);
            programMenuItem.MenuItems.Add(closeMenuItem);

            this.mainControler = mainControler;

            mainControler.LayerManager.FirstLayerAdded += new LayerManager.LayerAddedDelegate(LayerManager_FirstLayerAdded);
        }

        void LayerManager_FirstLayerAdded(GravurGIS.Layers.Layer newShapeObject)
        {
            openTransportMenuItem.Enabled = true;
            importTransportMenuItem.Enabled = true;
        }

        private void menuItemClick(object sender, EventArgs e)
        {
            if (sender == openProjectMenuItem)
				mainControler.openMWD();
            else if (sender == openTransportMenuItem)
				mainControler.importTransportLayer(true);
            else if (sender == importProjectMenuItem)
                mainControler.importMWD();
            else if (sender == importTransportMenuItem)
				mainControler.importTransportLayer(false);
            else if (sender == saveMenuItem)
				mainControler.save();

#if DEVELOP
			else if (sender == stressTest)
				mainControler.stressTest();
#endif
            else if (sender == saveAsMenuItem)
				mainControler.showSaveDialog();
            else if (sender == closeMenuItem)
				mainControler.ExitProgram();
            else if (sender == loadDefaultSettingsMenuItem)
				mainControler.LoadDefaultSettings();
            else if (sender == aboutMenuItem)
				mainControler.showAboutDialog();
            else if (sender == settingsMenuItem)
				mainControler.showSettingsDialog();
        }
    }
}
