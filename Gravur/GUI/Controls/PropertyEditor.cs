using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

namespace GravurGIS.GUI.Controls
{
    public class PropertyEditor : System.Windows.Forms.Control
    {
        private int dX = 0, dY = 0;
        private List<PropertyItem> propertyItemList = new List<PropertyItem>();

        public PropertyEditor()
            : base()
        {
            //
            // Required for Windows Form Designer support
            //
            
            InitializeComponent();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // PropertyEditor
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(200, 200);
            this.Name = "PropertyEditor"; //WARNING: do not change, pItems work with this name.. *uuh..dirty*
        }
        #endregion

        public void addItem(string name, PropertyType type)
        {
            PropertyItem item = new PropertyItem(name, type);
            item.Location = new Point(dX, dY);
            dY += item.Height + 4;
            this.propertyItemList.Add(item);
            this.Controls.Add(item);
        }

        public void addItem(PropertyItem item)
        {
            item.Location = new Point(dX, dY);
            dY += item.Height + 4;
            this.propertyItemList.Add(item);
            this.Controls.Add(item);
        }
        #region Getter/Setter
        
        public PropertyItem getPropertyItemFromList(int i)
        {
            PropertyItem returnItem = null;
            foreach (PropertyItem item in propertyItemList)
                if (propertyItemList.IndexOf(item) == i)
                    returnItem = item;

            if (returnItem == null) MessageBox.Show("Fehler bei Rückgabe von Item aus PropertyEditor");

            return returnItem;
         
        
        }

        

        #endregion
        
    }
}