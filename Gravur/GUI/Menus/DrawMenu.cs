using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GravurGIS.GUI.Menu
{
    class DrawMenu : ContextMenu
    {
        private MainForm mainForm;
        private MenuItem pointMenuItem;
        private MenuItem polylineMenuItem;
        private MenuItem polygonMenuItem;

        public DrawMenu(MainForm mainForm)
            : base()
        {
            pointMenuItem = new MenuItem();
            pointMenuItem.Text = "Punkt";
            pointMenuItem.Click += new System.EventHandler(menuItemClick);
            this.MenuItems.Add(pointMenuItem);
            polylineMenuItem = new MenuItem();
            polylineMenuItem.Text = "Linienzug";
            polylineMenuItem.Click += new System.EventHandler(menuItemClick);
            this.MenuItems.Add(polylineMenuItem);
            polygonMenuItem = new MenuItem();
            polygonMenuItem.Text = "Polygon";
            polygonMenuItem.Click += new System.EventHandler(menuItemClick);
            this.MenuItems.Add(polygonMenuItem);
            this.mainForm = mainForm;

            for (int i = 0; i < MenuItems.Count; i++)
                MenuItems[i].Enabled = false;

            mainForm.MainControler.LayerManager.FirstLayerAdded += new LayerManager.LayerAddedDelegate(LayerManager_FirstLayerAdded);
        }

        void LayerManager_FirstLayerAdded(GravurGIS.Layers.Layer newShapeObject)
        {
            for (int i = 0; i < MenuItems.Count; i++)
                MenuItems[i].Enabled = true;
        }

        private void menuItemClick(object sender, EventArgs e)
        {
            if (sender == pointMenuItem)
                mainForm.changeTool(Tool.DrawPoint);
            else if (sender == polylineMenuItem)
                mainForm.changeTool(Tool.DrawPolyline);
                
            else if (sender == polygonMenuItem)
                mainForm.changeTool(Tool.DrawPolygon);
        }

        public void SelectedToolChanged(Tool tool)
        {
            for (int i = 0; i < this.MenuItems.Count; i++)
                this.MenuItems[i].Checked = false;

            if (tool == Tool.DrawPoint)
                pointMenuItem.Checked = true;
            else if (tool == Tool.DrawPolyline)
                polylineMenuItem.Checked = true;
            else if (tool == Tool.DrawPolygon)
                polygonMenuItem.Checked = true;
        }

        public void EditModeInterrupted(bool interrupted, Tool tool)
        {
            if (interrupted)
            {
                for (int i = 0; i < this.MenuItems.Count; i++)
                    this.MenuItems[i].Enabled = false;

                if (tool == Tool.DrawPolyline)
                    polylineMenuItem.Enabled = true;
                else if (tool == Tool.DrawPolygon)
                    polygonMenuItem.Enabled = true;
            }
            else
            {
                for (int i = 0; i < this.MenuItems.Count; i++)
                    this.MenuItems[i].Enabled = true;
            }
        }
    }
}
