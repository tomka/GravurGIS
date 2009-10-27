using System;
using System.Windows.Forms;
using GravurGIS.Actions;

/// <summary>
/// Summary description for Class1
/// </summary>
namespace GravurGIS.GUI.Menu
{
    public class ShapeEditMenu : ContextMenu
    {
        private MainForm mainForm;
        private MenuItem removeShapeMenuItem;
        //private MenuItem loadAtributesMenuItem;
        //private MenuItem propertiesMenuItem = null;
        private MenuItem moveMenuItem;
        private MenuItem commentMenuItem;
        private MenuItem moveCommentMenuItem;

        public ShapeEditMenu(MainForm mainForm)
            : base()
        {
            this.mainForm = mainForm;

            //propertiesMenuItem = new MenuItem();
            //propertiesMenuItem.Text = "Eigenschaften";
            //propertiesMenuItem.Checked = false;
            //propertiesMenuItem.Click += new System.EventHandler(menuItemClick);

            //loadAtributesMenuItem = new MenuItem();
            //loadAtributesMenuItem.Text = "Attribute laden";
            //loadAtributesMenuItem.Checked = false;
            //loadAtributesMenuItem.Click += new System.EventHandler(menuItemClick);

            moveMenuItem = new MenuItem();
            moveMenuItem.Text = "Objekt verschieben";
            moveMenuItem.Checked = false;
            moveMenuItem.Click += new System.EventHandler(menuItemClick);

            removeShapeMenuItem = new MenuItem();
            removeShapeMenuItem.Text = "Objekt löschen";
            removeShapeMenuItem.Checked = false;
            removeShapeMenuItem.Click += new System.EventHandler(menuItemClick);

            commentMenuItem = new MenuItem();
            commentMenuItem.Text = "Kommentar bearbeiten";
            commentMenuItem.Checked = false;
            commentMenuItem.Click += new System.EventHandler(menuItemClick);

            moveCommentMenuItem = new MenuItem();
            moveCommentMenuItem.Text = "Kommentar verschieben";
            moveCommentMenuItem.Checked = false;
            moveCommentMenuItem.Click += new System.EventHandler(menuItemClick);

            this.MenuItems.Add(moveMenuItem);
            this.MenuItems.Add(commentMenuItem);
            this.MenuItems.Add(removeShapeMenuItem);
            //this.MenuItems.Add(propertiesMenuItem);
            //this.MenuItems.Add(loadAtributesMenuItem);
        }

        

        private void menuItemClick(object sender, EventArgs e)
        {
            if (sender == removeShapeMenuItem)
                mainForm.MainControler.LayerManager.removeSelectedTransportShape();
            else if (sender == commentMenuItem)
                mainForm.MainControler.showCommentDialog();
            //else if (sender == loadAtributesMenuItem)
            //    MessageBox.Show("TODO: Attribute laden");
            //else if (sender == propertiesMenuItem)
            //    MessageBox.Show("TODO: Eigenschaften zeigen");
            else if (sender == moveMenuItem)
                mainForm.changeTool(Tool.TransportShapeMove);
            else if (sender == moveCommentMenuItem)
                mainForm.changeTool(Tool.MoveComment);
        }
    }
}
