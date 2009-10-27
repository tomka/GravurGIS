using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GravurGIS.GUI.Menu
{
    class ToolMenu : ContextMenu
    {
        private MainForm mainForm;
        private MenuItem pointerMenuItem;
        private MenuItem panMenuItem;
        private MenuItem zoomInMenuItem;
        private MenuItem zoomOutMenuItem;
        private MenuItem resetMenuItem;
#if DEVELOP
        private MenuItem identifyMenuItem;
#endif
        public ToolMenu(MainForm mainForm)
            : base()
        {
            pointerMenuItem = new MenuItem();
            pointerMenuItem.Text = "Zeiger";
            pointerMenuItem.Click += new System.EventHandler(menuItemClick);
            pointerMenuItem.Checked = true;
           
            this.MenuItems.Add(pointerMenuItem);
            panMenuItem = new MenuItem();
            panMenuItem.Text = "Verschieben";
            panMenuItem.Click += new System.EventHandler(menuItemClick);
            this.MenuItems.Add(panMenuItem);
#if DEVELOP
            identifyMenuItem = new MenuItem();
            identifyMenuItem.Text = "Identifizieren";
            identifyMenuItem.Click += new System.EventHandler(menuItemClick);
            this.MenuItems.Add(identifyMenuItem);
#endif
            zoomInMenuItem = new MenuItem();
            zoomInMenuItem.Text = "ZoomIn (+)";
            zoomInMenuItem.Click += new System.EventHandler(menuItemClick);
            this.MenuItems.Add(zoomInMenuItem);
            
            zoomOutMenuItem = new MenuItem();
            zoomOutMenuItem.Text = "ZoomOut (-)";
            zoomOutMenuItem.Click += new System.EventHandler(menuItemClick);
            this.MenuItems.Add(zoomOutMenuItem);
            
            MenuItem separator = new MenuItem();
            separator.Text = "-";
            this.MenuItems.Add(separator);
            
            resetMenuItem = new MenuItem();
            resetMenuItem.Text = "Weltansicht";
            resetMenuItem.Click += new System.EventHandler(resetItemClick);
            this.MenuItems.Add(resetMenuItem);
            
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
            if (sender == panMenuItem)
                mainForm.changeTool(Tool.Move);
            else if (sender == zoomInMenuItem)
                mainForm.changeTool(Tool.ZoomIn);
            else if (sender == zoomOutMenuItem)
                mainForm.changeTool(Tool.ZoomOut);
            else if (sender == pointerMenuItem)
                mainForm.changeTool(Tool.Pointer);
#if DEVELOP
            else if (sender == identifyMenuItem)
                mainForm.changeTool(Tool.Identify);
#endif
        }

        private void resetItemClick(object sender, EventArgs e)
        {
            mainForm.getMapPanel().reset();
        }

        public void SelectedToolChanged(Tool tool)
        {
            pointerMenuItem.Enabled = true;
            for (int i = 0; i < this.MenuItems.Count; i++)
                this.MenuItems[i].Checked = false;

            if (tool == Tool.Pointer)
                pointerMenuItem.Checked = true;
            else if (tool == Tool.Move)
                panMenuItem.Checked = true;
            else if (tool == Tool.ZoomIn)
                zoomInMenuItem.Checked = true;
            else if (tool == Tool.ZoomOut)
                zoomOutMenuItem.Checked = true;
            else if (tool == Tool.DrawPolygon || tool == Tool.DrawPolyline)
                pointerMenuItem.Enabled = false;
        }
    }
}