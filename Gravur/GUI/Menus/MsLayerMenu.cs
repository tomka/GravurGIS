using System;

using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using GravurGIS.GUI.Dialogs;

namespace GravurGIS.GUI.Menus
{
    class MsLayerMenu : ContextMenu
    {
        private MenuItem layerUpItem;
        private MenuItem layerDownItem;
        private MenuItem layerHighestItem;
        private MenuItem layerLowestItem;
        private MapServerSettings msSettings;

    public  MsLayerMenu(MapServerSettings msSettings)
            : base()
        {
            this.msSettings = msSettings;
            layerUpItem = new MenuItem();
            layerUpItem.Text = "Layer nach oben";
            layerUpItem.Checked = false;
            layerUpItem.Click += new System.EventHandler(menuItemClick);

            layerDownItem = new MenuItem();
            layerDownItem.Text = "Layer nach unten";
            layerDownItem.Checked = false;
            layerDownItem.Click += new System.EventHandler(menuItemClick);

            layerHighestItem = new MenuItem();
            layerHighestItem.Text = "Layer nach ganz oben";
            layerHighestItem.Checked = false;
            layerHighestItem.Click += new System.EventHandler(menuItemClick);

            layerLowestItem = new MenuItem();
            layerLowestItem.Text = "Layer nach ganz unten";
            layerLowestItem.Checked = false;
            layerLowestItem.Click += new System.EventHandler(menuItemClick);

            this.MenuItems.Add(layerUpItem);
            this.MenuItems.Add(layerDownItem);
            this.MenuItems.Add(layerHighestItem);
            this.MenuItems.Add(layerLowestItem);
        }

    private void menuItemClick(object sender, EventArgs e)
    {
        if (sender == layerUpItem) msSettings.moveSelectedLayerOneStepUp();
        else if (sender == layerDownItem) msSettings.moveSelectedLayerOneStepDown();
        else if (sender == layerHighestItem) msSettings.moveSelectedLayerHighest();
        else if (sender == layerLowestItem) msSettings.moveSelectedLayerLowest();
    }



    }
}
