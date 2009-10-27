using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GravurGIS.GUI.Menu
{
    class AddMenu : ContextMenu
    {
        private MainControler _mainControler;
        private MenuItem newLayerMenuItem;
        private MenuItem newGeoImageMenuItem;
        private MenuItem newMandelbrotMenuItem;
        private MenuItem newMapServerLayer;
        
        private MenuItem newOGRLayer;

        public AddMenu(MainControler mainControler)
            : base()
        {
            newLayerMenuItem = new MenuItem();
            newLayerMenuItem.Text = "Layer hinzufügen...";
            newLayerMenuItem.Click += new System.EventHandler(menuItemClick);
            this.MenuItems.Add(newLayerMenuItem);
            newGeoImageMenuItem = new MenuItem();
            newGeoImageMenuItem.Text = "Geo-Bild hinzufügen...";
            newGeoImageMenuItem.Click += new System.EventHandler(menuItemClick);
            this.MenuItems.Add(newGeoImageMenuItem);
            newMandelbrotMenuItem = new MenuItem();
            newMandelbrotMenuItem.Text = "Mandelbrot hinzufügen...";
            newMandelbrotMenuItem.Click += new System.EventHandler(menuItemClick);

#if DEVELOP

            newOGRLayer = new MenuItem();
            newOGRLayer.Text = "OGR Layer hinzufügen...";
            newOGRLayer.Click += new System.EventHandler(menuItemClick);
            this.MenuItems.Add(newOGRLayer);
#endif	
            newMapServerLayer = new MenuItem();
            newMapServerLayer.Text = "WMS-Layer hinzufügen...";
            newMapServerLayer.Click += new System.EventHandler(menuItemClick);
            this.MenuItems.Add(newMapServerLayer);

			this._mainControler = mainControler;

			this._mainControler.SettingsLoaded += new MainControler.SettingsLoadedDelegate(MainControler_SettingsLoaded);

        }

        void MainControler_SettingsLoaded(Config config)
        {
            if (config.ShowSpecialLayers)
            {
                if (!this.MenuItems.Contains(newMandelbrotMenuItem))
                    this.MenuItems.Add(newMandelbrotMenuItem);
            }
            else
            {
                if (this.MenuItems.Contains(newMandelbrotMenuItem))
                    this.MenuItems.Remove(newMandelbrotMenuItem);
            }

        }

        private void menuItemClick(object sender, EventArgs e)
        {
            if (sender == newGeoImageMenuItem)
				_mainControler.addGeoImage();
            else if (sender == newLayerMenuItem)
				_mainControler.addShapeFile();
            else if (sender == newMandelbrotMenuItem)
				_mainControler.addMandelbrot();
            else if (sender == newMapServerLayer)
				_mainControler.addMapserverLayer();
#if DEVELOP
            else if (sender == newOGRLayer)
				_mainControler.addOGRLayer();
#endif
        }
    }
}