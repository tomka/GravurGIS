using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GravurGIS.GUI.Menu
{
    class UndoMenu : ContextMenu
    {
        private MainControler mainControler;
        private MenuItem undoItem;
        private MenuItem redoItem;

        public UndoMenu(MainControler mainControler)
            : base()
        {
            undoItem = new MenuItem();
            undoItem.Text = "Rückgängig";
            undoItem.Click += new System.EventHandler(menuItemClick);
            this.MenuItems.Add(undoItem);
            redoItem = new MenuItem();
            redoItem.Text = "Wiederherstellen";
            redoItem.Click += new System.EventHandler(menuItemClick);
            this.MenuItems.Add(redoItem);

            this.mainControler = mainControler;
        }

        private void menuItemClick(object sender, EventArgs e)
        {
            if (sender == undoItem)
                mainControler.UndoAction();
            else if (sender == redoItem)
                mainControler.RedoAction();
        }
    }
}
