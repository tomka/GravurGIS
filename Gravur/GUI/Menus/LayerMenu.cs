using System;
using System.Windows.Forms;

/// <summary>
/// The context menu for layers
/// </summary>
namespace GravurGIS.GUI.Menu
{
    public class LayerMenu : ContextMenu
    {
        private MainForm mainForm;
        private MenuItem layerZoomItem;
        private MenuItem layerUpItem;
        private MenuItem layerDownItem;
        private MenuItem layerHighestItem;
        private MenuItem layerLowestItem;
        private MenuItem attributeItem;
        private MenuItem layerRemoveItem;
        private MenuItem layerSettingsItem;

        private System.Windows.Forms.OpenFileDialog addAttributeDialog;

        public LayerMenu(MainForm mainForm)
            : base()
        {
            this.mainForm = mainForm;
            addAttributeDialog = new System.Windows.Forms.OpenFileDialog();

            layerHighestItem = new MenuItem();
            layerHighestItem.Text = "Layer nach ganz oben";
            layerHighestItem.Checked = false;
            layerHighestItem.Click += new System.EventHandler(menuItemClick);

            layerUpItem = new MenuItem();
            layerUpItem.Text = "Layer nach oben";
            layerUpItem.Checked = false;
            layerUpItem.Click += new System.EventHandler(menuItemClick);

            layerDownItem = new MenuItem();
            layerDownItem.Text = "Layer nach unten";
            layerDownItem.Checked = false;
            layerDownItem.Click += new System.EventHandler(menuItemClick);

            layerLowestItem = new MenuItem();
            layerLowestItem.Text = "Layer nach ganz unten";
            layerLowestItem.Checked = false;
            layerLowestItem.Click += new System.EventHandler(menuItemClick);

            MenuItem separator = new MenuItem();
            separator.Text = "-";

            layerZoomItem = new MenuItem();
            layerZoomItem.Text = "Auf Layergröße zoomen";
            layerZoomItem.Checked = false;
            layerZoomItem.Click += new System.EventHandler(menuItemClick);

            attributeItem = new MenuItem();
            attributeItem.Text = "Attribute laden";
            attributeItem.Checked = false;
            attributeItem.Click += new System.EventHandler(menuItemClick);

            layerRemoveItem = new MenuItem();
            layerRemoveItem.Text = "Löschen";
            layerRemoveItem.Checked = false;
            layerRemoveItem.Click += new System.EventHandler(menuItemClick);

            layerSettingsItem = new MenuItem();
            layerSettingsItem.Text = "Eigenschaften";
            layerSettingsItem.Checked = false;
            layerSettingsItem.Click += new System.EventHandler(menuItemClick);

            this.MenuItems.Add(layerHighestItem);
            this.MenuItems.Add(layerUpItem);
            this.MenuItems.Add(layerDownItem);
            this.MenuItems.Add(layerLowestItem);
            this.MenuItems.Add(separator);
            this.MenuItems.Add(layerZoomItem);
            this.MenuItems.Add(attributeItem);
            this.MenuItems.Add(layerRemoveItem);
            this.MenuItems.Add(layerSettingsItem);
        }

        private void menuItemClick(object sender, EventArgs e)
        {
            if (sender == layerZoomItem)
            {
                try
                {
                    mainForm.getMapPanel().onLayerZoom(mainForm.LayerListView.SelectedIndices[0]);
                }
                catch { }
            }
            else if (sender == layerUpItem) mainForm.moveSelectedLayerOneStepUp();
            else if (sender == layerDownItem) mainForm.moveSelectedLayerOneStepDown();
            else if (sender == attributeItem) addAttribute();
            else if (sender == layerRemoveItem) mainForm.removeLayer();
            else if (sender == layerSettingsItem) mainForm.layerSettingsButton_Click(this, null);
            else if (sender == layerHighestItem) mainForm.moveSelectedLayerHighest();
            else if (sender == layerLowestItem) mainForm.moveSelectedLayerLowest();
        }

        private void addAttribute() {
            addAttributeDialog.Filter = "MWL Files|*.mwl";
            if (addAttributeDialog.ShowDialog() == DialogResult.OK) 
                   mainForm.MainControler.LayerManager.changeLayer(mainForm.LayerListView.SelectedIndices[0],addAttributeDialog.FileName);
           
        }

        protected override void OnPopup(EventArgs e)
        {
            base.OnPopup(e);
            if (mainForm.LayerListView.SelectedIndices.Count == 0)
            {
                foreach (MenuItem item in MenuItems)
                    item.Enabled = false;
            } else
                foreach (MenuItem item in MenuItems)
                    item.Enabled = true;
        }
    }
}
