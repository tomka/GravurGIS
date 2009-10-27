using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;


namespace GravurGIS.GUI.Controls
{
    public enum PropertyType
    {
        Checkbox, EditField, Radio, Container
    }
    public enum PropertyState
    {
        Openened, Closed, Checked, Unchecked
    }

   

    public class PropertyItem : Control,IPropertyItem
    {
        protected PropertyType type;
        protected PropertyState state;
        private TextBox textBox;
        protected bool clickStarted = false;
        protected int itemNr;
        private int moveY = 0;
        private int actualMove = 0;
        private Rectangle textRectangle;
        Rectangle drawRectangle = new Rectangle(0, 2, 10, 10);
        protected const int change = 15;
      
        public delegate void OpenContainerDelegate(int itemNr, int childCount);
        public event OpenContainerDelegate ContainerOpened;

        public delegate void CloseContainerDelegate(int itemNr, int childCount);
        public event CloseContainerDelegate ContainerClosed;

       
        public PropertyItem(string name) : this(name, PropertyType.Checkbox) { }

        public PropertyItem(string name, PropertyType type)
        {
            this.Name = name;
            this.type = type;
            this.Height = 15;
            this.Width = 150;
           

            if (type == PropertyType.EditField)
            {
                textBox = new TextBox();
                this.Controls.Add(textBox);
            }
            if (   (type == PropertyType.Container) || (Type == PropertyType.EditField) )
                state = PropertyState.Closed;
            else state = PropertyState.Checked;

            this.MouseDown += new MouseEventHandler(PropertyItem_MouseDown);
            this.MouseUp += new MouseEventHandler(PropertyItem_MouseUp);
            
            this.textRectangle = new Rectangle(18, 0,0, Height);

        }
        /// <summary>
        /// says, if/how much propertyItem moves after one container is opened
        /// </summary>
        /// <param name="itemNr">nr of opened container</param>
        /// <param name="childCount">nr of childs the container has</param>

        public void AsContainerOpened(int itemNr, int childCount) {
            //if ((itemNr < this.itemNr) //just move down if under opened container and if you not be a child of openend container
            //    &&(  (this.Parent.Name == "propertyEditor")
            //       ||( ((PropertyItem)(Parent)).itemNr != itemNr  )
            //      )
            //   )
            if (itemNr < this.itemNr)
            {
                actualMove = childCount * change;
                moveY += actualMove;
                Rectangle newBounds = new Rectangle(Bounds.X, Bounds.Y + actualMove, Bounds.Width, Bounds.Height);
                Bounds = newBounds;
                this.Invalidate();
            }
        }
        /// <summary>
        /// says, if/how much propertyItem moves after one container is closed
        /// </summary>
        /// <param name="itemNr">nr of closed container</param>
        /// <param name="childCount">nr of childs the container has</param>

        public void AsContainerClosed(int itemNr, int childCount)
        {
            //if ((itemNr < this.itemNr) //just move up if under opened container and if you not be a child of openend container
            //    && ((this.Parent.Name == "propertyEditor")
            //       || (((PropertyItem)(Parent)).itemNr != itemNr)
            //      )
            //   )
            if (itemNr < this.itemNr)
            {
                actualMove = -1 * childCount * change;
                moveY += actualMove;
                Rectangle newBounds = new Rectangle(Bounds.X, Bounds.Y + actualMove, Bounds.Width, Bounds.Height);
                Bounds = newBounds;
                this.Invalidate();
            }
        }

        

        public void PropertyItem_MouseDown(object sender, MouseEventArgs e)
        {
            if ( this.drawRectangle.Contains(e.X,e.Y) 
              || this.textRectangle.Contains(e.X,e.Y)
               )
            clickStarted = true;
        }

        public void PropertyItem_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.Enabled && clickStarted)
            {
                if (clickStarted)
                {
                    if (state == PropertyState.Checked)
                    {
                        state = PropertyState.Unchecked;
                    }
                    else if (state == PropertyState.Unchecked)
                    {
                        state = PropertyState.Checked;
                        
                    }
                    else if (state == PropertyState.Closed)
                    {
                        if (type == PropertyType.Container) //open container
                        {
                            state = PropertyState.Openened;
                            foreach (PropertyItem item in Controls)
                                item.Show();
                            actualMove = 1 * this.Controls.Count * change;
                            changeBounds();
                            ContainerOpened(this.ItemNr, this.Controls.Count);
                        }
                        else //open type field for editfield
                        { 
                            
                        
                        }
                    }
                    else
                    {
                        state = PropertyState.Closed;
                        foreach (PropertyItem item in Controls)
                            item.Hide();
                        actualMove = -1 * this.Controls.Count * change;
                        changeBounds();
                        ContainerClosed(this.itemNr, this.Controls.Count);
                    }
                }
                this.Invalidate();
            }
            clickStarted = false;
        }

        private void changeBounds()
        {
            /*
            //change own bounds
            Rectangle newBounds = Bounds;
            newBounds.Height = Bounds.Height + actualMove;
            Bounds = newBounds;

            //change bounds of parent
            newBounds = Parent.Bounds;
            newBounds.Height = newBounds.Height + actualMove;
            Parent.Bounds = newBounds;
            */
            Size tempSize = new Size(ClientSize.Width, ClientSize.Height + actualMove);
            ClientSize = tempSize;

            tempSize = Parent.ClientSize;
            tempSize.Height = tempSize.Height + actualMove;
            Parent.ClientSize = tempSize;

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if  (Parent.Name == "propertyEditor") PaintNow(e);
            else
            {
                PropertyItem tempParent = (PropertyItem) this.Parent;
                if (tempParent.Type != PropertyType.Container) PaintNow(e);
                else if (tempParent.State == PropertyState.Openened) PaintNow(e);
                else this.Hide();
            }
        }
        

        protected void PaintNow(PaintEventArgs e)
        {
            
            Graphics gx = e.Graphics;
            SolidBrush backColorBrush = new SolidBrush(this.BackColor);
            SolidBrush textBrush = new SolidBrush(Color.Black);
            Pen linePen = new Pen(Color.Black);

            if (this.type == PropertyType.Radio)
            {
                gx.DrawEllipse(linePen, drawRectangle);
                if (state == PropertyState.Checked)
                    gx.FillEllipse(textBrush, new Rectangle(2, 4, 6, 6));
            }
            else if (this.type == PropertyType.Container)
            {
                gx.DrawRectangle(linePen, drawRectangle);
                gx.DrawLine(linePen, 2, 7, 8, 7);
                if (state == PropertyState.Closed) gx.DrawLine(linePen, 5, 4, 5, 10);
            }
            else if (this.type == PropertyType.Checkbox)
            {

                gx.DrawRectangle(linePen, drawRectangle);
                if (state == PropertyState.Checked)
                {
                    linePen.Color = Color.Green;
                    Point pt = new Point(2, 3);
                    gx.DrawLine(linePen, pt.X, pt.Y + 2, pt.X + 3, pt.Y + 5);
                    gx.DrawLine(linePen, pt.X, pt.Y + 3, pt.X + 3, pt.Y + 6);
                    gx.DrawLine(linePen, pt.X, pt.Y + 4, pt.X + 3, pt.Y + 7);
                    gx.DrawLine(linePen, pt.X + 4, pt.Y + 4, pt.X + 10, pt.Y);
                    gx.DrawLine(linePen, pt.X + 4, pt.Y + 5, pt.X + 10, pt.Y + 1);
                    gx.DrawLine(linePen, pt.X + 4, pt.Y + 6, pt.X + 10, pt.Y + 2);
                }
            }
            textRectangle.Width = Convert.ToInt32(  gx.MeasureString(this.Name, this.Font).Width);
            gx.DrawString(this.Name, this.Font, textBrush,textRectangle);            
            backColorBrush.Dispose();
            textBrush.Dispose();
            linePen.Dispose();
        }

        public virtual void AddItem(PropertyItem item)
        {
            if (this.Type == PropertyType.Container)
            {
                Rectangle newBounds = item.Bounds;
                this.Controls.Add(item);
                newBounds.X += change;
                newBounds.Y += change * Controls.Count;
                item.Bounds = newBounds;
            }
        }


        #region Getter/Setter
        public PropertyType Type
        {
            get { return type; }
            set { type = value; }
        }

        public PropertyState State
        {
            get { return state; }
            set { state = value; }
        }

        public int ItemNr
        {
            get { return itemNr; }
            set { itemNr = value; }
        }
        #endregion

        #region IPropertyItem Members

        #endregion
    }
}
