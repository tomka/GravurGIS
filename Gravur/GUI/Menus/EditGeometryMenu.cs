using System;
using System.Windows.Forms;
using GravurGIS.Actions;
using GravurGIS.Shapes;

/// <summary>
/// Summary description for Class1
/// </summary>
namespace GravurGIS.GUI.Menu
{
    public class EditGeometryMenu : ContextMenu
    {
        private MainForm mainForm;
        private MenuItem removeShapeMenuItem;
        private MenuItem moveMenuItem;
        //private MenuItem choosePositionMenuItem;
        private IShape container = null;
        private ShpPoint vertex = null;

        public EditGeometryMenu(MainForm mainForm, IShape container, ShpPoint vertex)
            : base()
        {
            this.mainForm = mainForm;
            this.container = container;
            this.vertex = vertex;

            moveMenuItem = new MenuItem();
            moveMenuItem.Text = "verschieben";
            moveMenuItem.Checked = false;
            moveMenuItem.Click += new System.EventHandler(menuItemClick);

            //choosePositionMenuItem = new MenuItem();
            //choosePositionMenuItem.Text = "Position eingeben";
            //choosePositionMenuItem.Checked = false;
            //choosePositionMenuItem.Click += new System.EventHandler(menuItemClick);

            removeShapeMenuItem = new MenuItem();
            removeShapeMenuItem.Text = "löschen";
            removeShapeMenuItem.Checked = false;
            removeShapeMenuItem.Click += new System.EventHandler(menuItemClick);
            
            // you can not delete a point if there are less then three points in the polygon
            if ((container.PointCount <= 4 &&
                (container as ShpPolygon) != null)
                ||
                (container.PointCount <= 2 &&
                (container as ShpPolyline) != null))
                removeShapeMenuItem.Enabled = false;

            this.MenuItems.Add(moveMenuItem);
            //this.MenuItems.Add(choosePositionMenuItem);
            this.MenuItems.Add(removeShapeMenuItem);
        }

        private void menuItemClick(object sender, EventArgs e)
        {
            if (sender == removeShapeMenuItem)
                mainForm.MainControler.removeVertex(container, vertex);
            else if (sender == moveMenuItem)
                mainForm.changeTool(Tool.MoveGeometry);
        }
    }
}
