using System;
using System.Collections.Generic;

namespace GravurGIS.Topology.Grid
{
    public class GridElementItem
    {
        private List<PointList> elements;
        private int ty;
        private int lx;

        public GridElementItem(int lx, int ty)
        {
            elements = new List<PointList>();
            this.ty = ty;
            this.lx = lx;            
        }

        public void insert(PointList element)
        {
            elements.Add(element);
        }

        #region Getters/Setters

        public int LX
        {
            get { return lx; }
            set { lx = value; }
        }
        public int TY
        {
            get { return ty; }
            set { ty = value; }
        }
        public List<PointList> Elements
        {
            get { return elements; }
        }
        #endregion

    }

    public class Grid
    {
        #region Properties

        private int h_size;
        private int v_size;
        private int cell_height;
        private int cell_width;
        private GridElementItem[] items;
        private int itemCount;
        private int shape_width;
        private int shape_height;

        #endregion

        #region Initialization

        public Grid(int width, int height,
            int shape_width, int shape_height, bool cellMeassureDefined)
        {
            this.shape_height = shape_height;
            this.shape_width  = shape_width;
            int i;
            if (cellMeassureDefined)
            {
                this.cell_width = width;
                this.cell_height = height;
                this.v_size = shape_height / height;
                this.h_size = shape_width / width;
                for (i = 0; shape_width >= ((i + h_size) * cell_width); i++) { }
                h_size += i;
                for (i = 0; shape_height >= ((i + v_size) * cell_height); i++) { }
                v_size += i;
            }
            else
            {
                this.h_size = width;
                this.v_size = height;
                this.cell_width = shape_width / h_size;
                this.cell_height = shape_height / v_size;
                for (i = 0; shape_width >= (h_size * (i + cell_width)); i++) { }
                cell_width += i;
                for (i = 0; shape_height >= (v_size * (i + cell_height)); i++) { }
                cell_height += i;
            }
            itemCount = h_size * v_size;
            items = new GridElementItem[itemCount];

            // Initialize array
            for (i = 0; i < itemCount; i++)
                items[i] = new GridElementItem((i * cell_width) % h_size,
                    (i * cell_height) % v_size);
        }

        #endregion

        #region Methods

        public void insert(PointList parent, System.Drawing.Point position,
            System.Drawing.Point size)
        {
                items[position.X / cell_width
                    + ((position.Y / cell_height) * h_size)].insert(parent);
        }

        public void insert(PointList parent, System.Drawing.Rectangle rect)
        {

            // TODO: Was ist hier mit Points und Multipoints? Geht das so?

            int x2, y2, difX, difY, endX, endY, countX, countY, x, y;
            int length = parent.displayPointList.Length;
            bool inserted = false;
            
            x = Math.Abs(parent.displayPointList[0].X / cell_width);
            y = Math.Abs(parent.displayPointList[0].Y / cell_height);
            length = parent.displayPointList.Length - 1;

            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    // insert the current point
                    // items[x + y].insert(parent);

                    // the coordinates of the following point
                    x2 = Math.Abs(parent.displayPointList[i + 1].X / cell_width);
                    y2 = Math.Abs(parent.displayPointList[i + 1].Y / cell_height);

                    difX = x2 - x;
                    difY = y2 - y;

                    // the following needs only to be done if the points of interest are in different cells
                    if (difX != 0 || difY != 0)
                    {
                        endX = Math.Abs(difX);
                        endY = Math.Abs(difY);

                        if (difX >= 0 && difY <= 0)         // 1st quadrant
                        {
                            difX = 1;
                            difY = -1;
                        }
                        else if (difX < 0 && difY <= 0)     // 2nd quadrant
                        {
                            difX = -1;
                            difY = -1;
                        }
                        else if (difX < 0 && difY > 0)      // 3rd quardrant
                        {
                            difX = -1;
                            difY = 1;
                        }
                        else                                // 4th quadrant
                        {
                            difX = 1;
                            difY = 1;
                        }

                        // now insert the area spanned up with the current and the following point
                        countX = 0;
                        for (int k = 0; k <= endX; k++)
                        {
                            countY = 0;
                            for (int j = 0; j <= endY; j++)
                            {
                                items[x + countX + (y + countY) * h_size].insert(parent);
                                countY = countY + difY;
                            }
                            countX = countX + difX;
                        }
                        if(inserted) inserted = false;
                    }
                    else
                    {
                        if (!inserted) items[x + y * h_size].insert(parent);
                        inserted = true;
                    }

                    x = parent.displayPointList[i].X / cell_width;
                    y = parent.displayPointList[i].Y / cell_height;
                }
            }
            else // we only have one part
            {

                // and for the last one:
                items[x + y * h_size].insert(parent);
            }

            //for (int i = 0; i < endX; i += cell_width)
            //{
            //    countY = 0;
            //    for (int j = 0; j < endY; j += cell_height)
            //    {
            //        items[x + y - countY * h_size + countX].insert(parent);
            //        countY++;
            //    }

            //    countX++;
            //}
            
            // if the parent to insert reaches right hand out of this cell add it also to the next
            //int dif = rect.X + rect.Width - (x + 1) * cell_width;
            //int countX = 1;
            //for (int i = 0; i < dif; i += cell_width)
            //{
            //    items[x + y + countX].insert(parent);
            //    countX++;
            //}
            // if the parent to insert reaches downwards (e.g. in direction of the top of the screen)
            // out of this cell add it also to the next ones which do have the part in
            //dif = (((rect.Y / cell_height) - 1) * h_size) - (rect.Y - rect.Height);
            //int countY = 1;
            //for (int i = 0; i < dif; i += h_size)
            //{
            //    items[x + y - countY * h_size].insert(parent);
            //    countY++;
            //}
        }

        public void contains(System.Drawing.Rectangle rect, ref List<int> returnList)
        {
            System.Drawing.Point tl = new System.Drawing.Point(rect.X / cell_width,
                (rect.Y / cell_height) * h_size);
            System.Drawing.Point br = new System.Drawing.Point((rect.X + rect.Width) / cell_width,
                ((rect.Y + rect.Height) / cell_height) * h_size);

            for (int i = tl.X; i <= br.X; i++)
                for (int j = tl.Y; j <= br.Y; j +=h_size)
                    returnList.Add(i+j);
        }

        /// <summary>
        /// ColldindingPoints gibt eine List von <T> (hier ist T = PointList) zurück welche innerhalb der Zellen
        /// des Grids liegen.
        /// TODO: Im Moment verhält sich die Funktion sehr radikal: Wenn die Boundingbox über die Ränder des Shapes
        /// geht, wird alles zurückgegeben was es gibt :) - könnte man noch etwas verbessern.
        /// </summary>
        /// <param name="rect">Die aufgespannte Boundingbox</param>
        /// <returns></returns>
        public void CollidingPoints(System.Drawing.Rectangle rect, ref List<PointList> pointList)
        {
            if (rect.X < 0) rect.X = 0;
            if (rect.Y < 0) rect.Y = 0;
            if (((rect.Height + rect.Y) > shape_height)) rect.Height = shape_height - rect.Y;
            if ((rect.Width + rect.X) > shape_width) rect.Width = shape_width - rect.X;

            if (((rect.X > shape_height) && rect.Height > 0)
                || (rect.Y > shape_width) && rect.Width > 0)
                rect.Height = 0;

            pointList.Clear();
            if (rect.Width == 0 || rect.Height == 0)
            {
               for (int i = items.Length - 1; i >= 0; i--)
                   pointList.AddRange(items[i].Elements);
            } else {
                List<int> cells = new List<int>();
                contains(rect, ref cells);
                for (int i = cells.Count - 1; i >= 0; i--)
                    pointList.AddRange(items[cells[i]].Elements);
            }
        }

        #endregion

        #region Getters/Setters

        public int Cell_height
        {
            get { return cell_height; }
        }
        public int Cell_width
        {
            get { return cell_width; }
        }
        public int Shape_height
        {
            get { return shape_height; }
            set { shape_height = value; }
        }
        public int Shape_width
        {
            get { return shape_width; }
            set { shape_width = value; }
        }
        #endregion

    }
}
