using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace GravurGIS.GUI.Controls
{
    class RBContainer: PropertyItem
    {
        public RBContainer(string name) : base(name, PropertyType.Checkbox) { }
        
        public override void AddItem(PropertyItem item)
        {
            Rectangle newBounds = item.Bounds;
            if (item.Type != PropertyType.Radio)
            {
                MessageBox.Show("Radiobutton Container können nur Radiobuttons hinzugefügt werden");
                item.Type = PropertyType.Radio;
            }
            this.Controls.Add(item);
            newBounds.X += change;
            newBounds.Y += change * Controls.Count;
            item.Bounds = newBounds;
        }
        
    }
}
