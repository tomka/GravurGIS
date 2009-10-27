#define DRAW_QUARDTREE

// // // // // // // // // // // // //
// QuadTree and supporting code
// by Kyle Schouviller
// http://www.kyleschouviller.com
//
// December 2006: Original version
// May 06, 2007:  Updated for XNA Framework 1.0
//                and public release.
//
// You may use and modify this code however you
// wish, under the following condition:
// *) I must be credited
// A little line in the credits is all I ask -
// to show your appreciation.
// 
// If you have any questions, please use the
// contact form on my website.
//
// Now get back to making great games!
// // // // // // // // // // // // //
// Modified by Tom
// // // // // // // // // // // // //

#region Using declarations

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace GravurGIS.Topology.QuadTree
{
    /// <summary>
    /// A position item in a quadtree
    /// </summary>
    /// <typeparam name="T">The type of the QuadTree item's parent</typeparam>
    public class QuadTreePositionItem<T>
    {
        // Alle Sachen rausgenommen die mit Umsortierung zu tun haben
        #region Events and Event Handlers

        /// <summary>
        /// A movement handler delegate
        /// </summary>
        /// <param name="positionItem">The item that fired the event</param>
        public delegate void MoveHandler(QuadTreePositionItem<T> positionItem);

        /// <summary>
        /// A destruction handler delegate - fired when the item is destroyed
        /// </summary>
        /// <param name="positionItem">The item that fired the event</param>
        public delegate void DestroyHandler(QuadTreePositionItem<T> positionItem);

        /// <summary>
        /// Event handler for the move action
        /// </summary>
        public event MoveHandler Move;

        /// <summary>
        /// Event handler for the destroy action
        /// </summary>
        public event DestroyHandler Destroy;

        /// <summary>
        /// Handles the move event
        /// </summary>
        protected void OnMove()
        {
            // Update rectangles
            // As we mirror the coordinate system on the y axis we cant use the following:
            rect.TopLeft = position - (size * .5d);
            rect.BottomRight = position + (size * .5d);
            // rect.TopLeft = new Vector2(position.X - (size.X * .5d), position.Y + (size.Y * .5d));
            // rect.BottomRight = new Vector2(position.X + (size.X * .5d), position.Y - (size.Y * .5d));

            // Call event handler
            if (Move != null) Move(this);
        }

        /// <summary>
        /// Handles the destroy event
        /// </summary>
        protected void OnDestroy()
        {
            if (Destroy != null) Destroy(this);
        }

        #endregion

        // Keine Umsortier-Sachen drin
        #region Properties

        /// <summary>
        /// The center position of this item
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// Gets or sets the center position of this item
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                OnMove();
            }
        }

        /// <summary>
        /// Changes position and size in one function
        /// </summary>
        /// <param name="position">Center position of the item</param>
        /// <param name="size">Size of the item</param>
        public void changePosAndSize(Vector2 position, Vector2 size) {
            this.size = size;
            this.position = position;
            OnMove();
        }

        /// <summary>
        /// The size of this item
        /// </summary>
        private Vector2 size;

        /// <summary>
        /// Gets or sets the size of this item
        /// </summary>
        public Vector2 Size
        {
            //get { return size; }
            set
            {
                size = value;
                rect.TopLeft = position - (size / 2d);
                rect.BottomRight = position + (size / 2d);
                OnMove();
            }
        }

        /// <summary>
        /// The rectangle containing this item
        /// </summary>
        private DRect rect;

        /// <summary>
        /// Gets a rectangle containing this item
        /// </summary>
        public DRect Rect
        {
            get { return rect; }
        }

        /// <summary>
        /// The parent of this item
        /// </summary>
        /// <remarks>The Parent accessor is used to gain access to the item controlling this position item</remarks>
        private T parent;

        /// <summary>
        /// Gets the parent of this item
        /// </summary>
        public T Parent
        {
            get { return parent; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a position item in a QuadTree
        /// </summary>
        /// <param name="parent">The parent of this item</param>
        /// <param name="position">The position of this item</param>
        /// <param name="size">The size of this item</param>
        public QuadTreePositionItem(T parent, Vector2 position, Vector2 size)
        {
            this.rect = new DRect(0d, 0d, 1d, 1d);

            this.parent = parent;
            this.position = position;
            this.size = size;
            OnMove();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Destroys this item and removes it from the QuadTree
        /// </summary>
        public void Delete()
        {
            OnDestroy();
        }

        /// <summary>
        /// Moves the selected quadtree position item to another position
        /// in the tree.
        /// </summary>
        /// <param name="x">Destination x</param>
        /// <param name="y">Destination y</param>
        /// <param name="differenceMove">Should be true if x and y are differences to the old position, otherwise false.</param>
        public void move(double x, double y, bool differenceMove)
        {
            if (differenceMove)
            {
                position.X += x;
                position.Y += y;
            }
            else
            {
                position.X = x;
                position.Y = y;
            }
            OnMove();
        }
        /// <summary>
        /// prohibits, that BBox around a Point changes when Zooming In/Out
        /// </summary>
        /// <param name="extent">defines size of BBox around Point</param>

        public void reactOnZoom(double extent)
        {
            Vector2 ev;
            ev.X = ev.Y = extent;

            rect.TopLeft = position - (size * .5d) - ev;
            rect.BottomRight = position + (size * .5d) + ev;
            // rect.TopLeft = new Vector2(position.X - (size.X * .5d), position.Y + (size.Y * .5d));
            // rect.BottomRight = new Vector2(position.X + (size.X * .5d), position.Y - (size.Y * .5d));

            // Call event handler
            if (Move != null) Move(this);
        }

        #endregion
    }

    /// <summary>
    /// A node in a QuadTree
    /// </summary>
    /// <typeparam name="T">The type of the QuadTree's items' parents</typeparam>
    public class QuadTreeNode<T>
    {
        #region Delegates

        /// <summary>
        /// World resize delegate
        /// </summary>
        /// <param name="newSize">The new world size</param>
        public delegate void ResizeDelegate(DRect newSize);

        #endregion

        #region Properties

        /// <summary>
        /// The rectangle of this node
        /// </summary>
        protected DRect rect;

        /// <summary>
        /// Gets the rectangle of this node
        /// </summary>
        public DRect Rect
        {
            get { return rect; }
            protected set { rect = value; }
        }

        /// <summary>
        /// The maximum number of items in this node before partitioning
        /// </summary>
        protected int MaxItems;

        /// <summary>
        /// Whether or not this node has been partitioned
        /// </summary>
        protected bool IsPartitioned;

        /// <summary>
        /// The parent node
        /// </summary>
        protected QuadTreeNode<T> ParentNode;
        

        /// <summary>
        /// The top left node
        /// </summary>
        protected QuadTreeNode<T> TopLeftNode;

        /// <summary>
        /// The top right node
        /// </summary>
        protected QuadTreeNode<T> TopRightNode;

        /// <summary>
        /// The bottom left node
        /// </summary>
        protected QuadTreeNode<T> BottomLeftNode;

        /// <summary>
        /// The bottom right node
        /// </summary>
        protected QuadTreeNode<T> BottomRightNode;

        /// <summary>
        /// The items in this node
        /// </summary>
        protected List<QuadTreePositionItem<T>> Items;

        /// <summary>
        /// Resize the world
        /// </summary>
        /// <param name="newSize">The new world size</param>
        protected ResizeDelegate WorldResize;

        #endregion

        #region Initialization

        /// <summary>
        /// QuadTreeNode constructor
        /// </summary>
        /// <param name="parentNode">The parent node of this QuadTreeNode</param>
        /// <param name="rect">The rectangle of the QuadTreeNode</param>
        /// <param name="maxItems">Maximum number of items in the QuadTreeNode before partitioning</param>
        public QuadTreeNode(QuadTreeNode<T> parentNode, DRect rect, int maxItems)
        {
            ParentNode = parentNode;
            Rect = rect;
            MaxItems = maxItems;
            IsPartitioned = false;
            Items = new List<QuadTreePositionItem<T>>();
        }

        /// <summary>
        /// QuadTreeNode constructor
        /// </summary>
        /// <param name="rect">The rectangle of the QuadTreeNode</param>
        /// <param name="maxItems">Maximum number of items in the QuadTreeNode before partitioning</param>
        /// <param name="worldResize">The function to return the size</param>
        public QuadTreeNode(DRect rect, int maxItems, ResizeDelegate worldResize)
        {
            ParentNode = null;
            Rect = rect;
            MaxItems = maxItems;
            WorldResize = worldResize;
            IsPartitioned = false;
            Items = new List<QuadTreePositionItem<T>>();
        }

        #endregion

        #region Insertion methods

        /// <summary>
        /// Insert an item in this node
        /// </summary>
        /// <param name="item">The item to insert</param>
        public void Insert(QuadTreePositionItem<T> item)
        {
            // If partitioned, try to find child node to add to
            if (!InsertInChild(item))
            {
                item.Destroy += new QuadTreePositionItem<T>.DestroyHandler(ItemDestroy);
                item.Move += new QuadTreePositionItem<T>.MoveHandler(ItemMove);
                Items.Add(item);

                // Check if this node needs to be partitioned
                if (!IsPartitioned && Items.Count >= MaxItems)
                {
                    Partition();
                }
            }
        }

        /// <summary>
        /// Inserts an item into one of this node's children
        /// </summary>
        /// <param name="item">The item to insert in a child</param>
        /// <returns>Whether or not the insert succeeded</returns>
        protected bool InsertInChild(QuadTreePositionItem<T> item)
        {
            if (!IsPartitioned) return false; // If not partitioned return false

            if (TopLeftNode.ContainsRect(item.Rect)) // Partitioned = Knoten hat unterknoten und keine eigenen PositionItems
                TopLeftNode.Insert(item);
            else if (TopRightNode.ContainsRect(item.Rect))
                TopRightNode.Insert(item);
            else if (BottomLeftNode.ContainsRect(item.Rect))
                BottomLeftNode.Insert(item);
            else if (BottomRightNode.ContainsRect(item.Rect))
                BottomRightNode.Insert(item);

            else return false; // insert in child failed

            return true;
        }

        /// <summary>
        /// Pushes an item down to one of this node's children
        /// </summary>
        /// <param name="i">The index of the item to push down</param>
        /// <returns>Whether or not the push was successful</returns>
        public bool PushItemDown(int i)
        {
            if (InsertInChild(Items[i]))
            {
                RemoveItem(i);
                return true;
            }

            else return false;
        }

        /// <summary>
        /// Push an item up to this node's parent
        /// </summary>
        /// <param name="i">The index of the item to push up</param>
        public void PushItemUp(int i)
        {
            QuadTreePositionItem<T> m = Items[i];

            RemoveItem(i);
            ParentNode.Insert(m);
        }

        /// <summary>
        /// Repartitions this node
        /// </summary>
        protected void Partition()
        {
            // Create the nodes
            Vector2 MidPoint = Vector2.Divide(Vector2.Add(Rect.TopLeft, Rect.BottomRight), 2.0d);

            TopLeftNode = new QuadTreeNode<T>(this,new DRect(Rect.TopLeft, MidPoint), MaxItems);
            TopRightNode = new QuadTreeNode<T>(this, new DRect(new Vector2(MidPoint.X, Rect.Top), new Vector2(Rect.Right, MidPoint.Y)), MaxItems);
            BottomLeftNode = new QuadTreeNode<T>(this, new DRect(new Vector2(Rect.Left, MidPoint.Y), new Vector2(MidPoint.X, Rect.Bottom)), MaxItems);
            BottomRightNode = new QuadTreeNode<T>(this, new DRect(MidPoint, Rect.BottomRight), MaxItems);

            IsPartitioned = true;

            // Try to push items down to child nodes
            int i = 0;
            while (i < Items.Count)
            {
                if (!PushItemDown(i))
                {
                    i++;
                }
            }
        }

        public void ReactOnZoom(double newExtent)
        {
            // tell every position item to resize
            //foreach (QuadTreePositionItem<T> Item in Items)
            //{
            //    Item.reactOnZoom(newExtent);
            //}
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].reactOnZoom(newExtent);
            }

            // query all subtrees
            if (IsPartitioned)
            {
                TopLeftNode.ReactOnZoom(newExtent);
                TopRightNode.ReactOnZoom(newExtent);
                BottomLeftNode.ReactOnZoom(newExtent);
                BottomRightNode.ReactOnZoom(newExtent);
            }

        }

        #endregion

        #region Query methods

        /// <summary>
        /// Gets a list of items containing a specified point
        /// </summary>
        /// <param name="Point">The point</param>
        /// <param name="ItemsFound">The list to add found items to (list will not be cleared first)</param>
        /// <remarks>ItemsFound is assumed to be initialized, and will not be cleared</remarks>
        public void GetItems(Vector2 Point, ref List<QuadTreePositionItem<T>> ItemsFound)
        {
            // test the point against this node
            if (Rect.Contains(Point))
            {
                // test the point in each item
                foreach (QuadTreePositionItem<T> Item in Items)
                {
                    if (Item.Rect.Contains(Point)) ItemsFound.Add(Item);
                }

                // query all subtrees
                if (IsPartitioned)
                {
                    TopLeftNode.GetItems(Point, ref ItemsFound);
                    TopRightNode.GetItems(Point, ref ItemsFound);
                    BottomLeftNode.GetItems(Point, ref ItemsFound);
                    BottomRightNode.GetItems(Point, ref ItemsFound);
                }
            }
        }

        /// <summary>
        /// Gets a list of items intersecting a specified rectangle
        /// </summary>
        /// <param name="Rect">The rectangle</param>
        /// <param name="ItemsFound">The list to add found items to (list will not be cleared first)</param>
        /// <remarks>ItemsFound is assumed to be initialized, and will not be cleared</remarks>
        public void GetItems(DRect Rect, ref List<QuadTreePositionItem<T>> ItemsFound)
        {
            // test the point against this node
            if (this.Rect.Intersects(Rect))
            {
                // test the point in each item
                foreach (QuadTreePositionItem<T> Item in Items)
                {
                    if (Item.Rect.Intersects(Rect)) ItemsFound.Add(Item);
                }

                // query all subtrees
                if (IsPartitioned)
                {
                    TopLeftNode.GetItems(Rect, ref ItemsFound);
                    TopRightNode.GetItems(Rect, ref ItemsFound);
                    BottomLeftNode.GetItems(Rect, ref ItemsFound);
                    BottomRightNode.GetItems(Rect, ref ItemsFound);
                }
            }
        }

        /// <summary>
        /// Gets a list of all items within this node
        /// </summary>
        /// <param name="ItemsFound">The list to add found items to (list will not be cleared first)</param>
        /// <remarks>ItemsFound is assumed to be initialized, and will not be cleared</remarks>
        public void GetAllItems(ref List<QuadTreePositionItem<T>> ItemsFound)
        {
            ItemsFound.AddRange(Items);

            // query all subtrees
            if (IsPartitioned)
            {
                TopLeftNode.GetAllItems(ref ItemsFound);
                TopRightNode.GetAllItems(ref ItemsFound);
                BottomLeftNode.GetAllItems(ref ItemsFound);
                BottomRightNode.GetAllItems(ref ItemsFound);
            }
        }

        /// <summary>
        /// Finds the node containing a specified item
        /// </summary>
        /// <param name="Item">The item to find</param>
        /// <returns>The node containing the item</returns>
        public QuadTreeNode<T> FindItemNode(QuadTreePositionItem<T> Item)
        {
            if (Items.Contains(Item)) return this;

            else if (IsPartitioned)
            {
                QuadTreeNode<T> n = null;

                // Check the nodes that could contain the item
                if (TopLeftNode.ContainsRect(Item.Rect))
                {
                    n = TopLeftNode.FindItemNode(Item);
                }
                if (n == null &&
                    TopRightNode.ContainsRect(Item.Rect))
                {
                    n = TopRightNode.FindItemNode(Item);
                }
                if (n == null &&
                    BottomLeftNode.ContainsRect(Item.Rect))
                {
                    n = BottomLeftNode.FindItemNode(Item);
                }
                if (n == null &&
                    BottomRightNode.ContainsRect(Item.Rect))
                {
                    n = BottomRightNode.FindItemNode(Item);
                }
                
                return n;
            }

            else return null;
        }

        #endregion

        #region Destruction

        /// <summary>
        /// Destroys this node
        /// </summary>
        public void Destroy()
        {
            // Destroy all child nodes
            if (IsPartitioned)
            {
                TopLeftNode.Destroy();
                TopRightNode.Destroy();
                BottomLeftNode.Destroy();
                BottomRightNode.Destroy();

                TopLeftNode = null;
                TopRightNode = null;
                BottomLeftNode = null;
                BottomRightNode = null;
            }

            // Remove all items
            while (Items.Count > 0)
            {
                RemoveItem(0);
            }
        }

        /// <summary>
        /// Removes an item from this node
        /// </summary>
        /// <param name="item">The item to remove</param>
        public void RemoveItem(QuadTreePositionItem<T> item)
        {
            // Find and remove the item
            if (Items.Contains(item))
            {
                item.Move -= new QuadTreePositionItem<T>.MoveHandler(ItemMove);
                item.Destroy -= new QuadTreePositionItem<T>.DestroyHandler(ItemDestroy);
                Items.Remove(item);
            }
        }

        /// <summary>
        /// Removes an item from this node at a specific index
        /// </summary>
        /// <param name="i">the index of the item to remove</param>
        protected void RemoveItem(int i)
        {
            if (i < Items.Count)
            {
                Items[i].Move -= new QuadTreePositionItem<T>.MoveHandler(ItemMove);
                Items[i].Destroy -= new QuadTreePositionItem<T>.DestroyHandler(ItemDestroy);
                Items.RemoveAt(i);
            }
        }

        #endregion

        #region Observer methods

        /// <summary>
        /// Handles item movement
        /// </summary>
        /// <param name="item">The item that moved</param>
        public void ItemMove(QuadTreePositionItem<T> item)
        {
            // Find the item
            if (Items.Contains(item))
            {
                int i = Items.IndexOf(item);

                // Try to push the item down to the child
                if (!PushItemDown(i))
                {
                    // otherwise, if not root, push up
                    if (ParentNode != null)
                    {
                        PushItemUp(i);
                    }
                    else if (!ContainsRect(item.Rect))
                    {
                        WorldResize(new DRect(
                             Vector2.Min(Rect.TopLeft, item.Rect.TopLeft) * 2,
                             Vector2.Max(Rect.BottomRight, item.Rect.BottomRight) * 2));
                    }

                }
            }
            else
            {
                // this node doesn't contain that item, stop notifying it about it
                item.Move -= new QuadTreePositionItem<T>.MoveHandler(ItemMove);
            }
        }

        /// <summary>
        /// Handles item destruction
        /// </summary>
        /// <param name="item">The item that is being destroyed</param>
        public void ItemDestroy(QuadTreePositionItem<T> item)
        {
            RemoveItem(item);
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Tests whether this node contains a rectangle
        /// </summary>
        /// <param name="rect">The rectangle to test</param>
        /// <returns>Whether or not this node contains the specified rectangle</returns>
        public bool ContainsRect(DRect rect)
        {
            return (rect.TopLeft.X >= Rect.TopLeft.X &&
                    rect.TopLeft.Y >= Rect.TopLeft.Y &&
                    rect.BottomRight.X <= Rect.BottomRight.X &&
                    rect.BottomRight.Y <= Rect.BottomRight.Y);
        }

        #endregion

        
    }

    /// <summary>
    /// A QuadTree for partitioning a space into rectangles
    /// </summary>
    /// <typeparam name="T">The type of the QuadTree's items' parents</typeparam>
    /// <remarks>This QuadTree automatically resizes as needed</remarks>
    public class QuadTree<T>
    {
        #region Properties

        /// <summary>
        /// The head node of the QuadTree
        /// </summary>
        protected QuadTreeNode<T> headNode;

        /// <summary>
        /// Gets the world rectangle
        /// </summary>
        public DRect WorldRect
        {
            get { return headNode.Rect; }
        }

        /// <summary>
        /// The maximum number of items in any node before partitioning
        /// </summary>
        protected int maxItems;

        #endregion

        #region Initialization

        /// <summary>
        /// QuadTree constructor
        /// </summary>
        /// <param name="worldRect">The world rectangle for this QuadTree (a rectangle containing all items at all times)</param>
        /// <param name="maxItems">Maximum number of items in any cell of the QuadTree before partitioning</param>
        public QuadTree(DRect worldRect, int maxItems)
        {
            this.headNode = new QuadTreeNode<T>(worldRect, maxItems, Resize);
            this.maxItems = maxItems;
        }

        /// <summary>
        /// QuadTree constructor
        /// </summary>
        /// <param name="size">The size of the QuadTree (i.e. the bottom-right with a top-left of (0,0))</param>
        /// <param name="maxItems">Maximum number of items in any cell of the QuadTree before partitioning</param>
        /// <remarks>This constructor is for ease of use</remarks>
        public QuadTree(Vector2 size, int maxItems)
            : this(new DRect(Vector2.Zero(), size), maxItems)
        {
            // Nothing extra to initialize
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts an item into the QuadTree
        /// </summary>
        /// <param name="item">The item to insert</param>
        /// <remarks>Checks to see if the world needs resizing and does so if needed</remarks>
        public void Insert(QuadTreePositionItem<T> item)
        {
            // check if the world needs resizing
            if (!headNode.ContainsRect(item.Rect))
            {
                Resize(new DRect(
                    Vector2.Min(headNode.Rect.TopLeft, item.Rect.TopLeft)*2,
                    Vector2.Max(headNode.Rect.BottomRight, item.Rect.BottomRight)*2));
            }

            headNode.Insert(item);
        }

        /// <summary>
        /// Inserts an item into the QuadTree
        /// </summary>
        /// <param name="parent">The parent of the new position item</param>
        /// <param name="position">The position of the new position item</param>
        /// <param name="size">The size of the new position item</param>
        /// <returns>A new position item</returns>
        /// <remarks>Checks to see if the world needs resizing and does so if needed</remarks>
        public QuadTreePositionItem<T> Insert(T parent, Vector2 position, Vector2 size)
        {
            QuadTreePositionItem<T> item = new QuadTreePositionItem<T>(parent, position, size);

            // check if the world needs resizing
            if (!headNode.ContainsRect(item.Rect))
            {
                Resize(new DRect(
                    Vector2.Min(headNode.Rect.TopLeft, item.Rect.TopLeft) * 2,
                    Vector2.Max(headNode.Rect.BottomRight, item.Rect.BottomRight) * 2));
            }

            headNode.Insert(item);

            return item;
        }
        public QuadTreePositionItem<T> RemoveItem(T parent, Vector2 position, Vector2 size)
        {

            QuadTreePositionItem<T> item = new QuadTreePositionItem<T>(parent, position, size);

            // check if the world needs resizing
            if (!headNode.ContainsRect(item.Rect))
            {
                Resize(new DRect(
                    Vector2.Min(headNode.Rect.TopLeft, item.Rect.TopLeft) * 2,
                    Vector2.Max(headNode.Rect.BottomRight, item.Rect.BottomRight) * 2));
            }

            headNode.RemoveItem(item);

            return item;
        }

        /// <summary>
        /// Resizes the Quadtree field
        /// </summary>
        /// <param name="newWorld">The new field</param>
        /// <remarks>This is an expensive operation, so try to initialize the world to a big enough size</remarks>
        public void Resize(DRect newWorld)
        {
            // Get all of the items in the tree
            List<QuadTreePositionItem<T>> Components = new List<QuadTreePositionItem<T>>();
            GetAllItems(ref Components);

            // Destroy the head node
            headNode.Destroy();
            headNode = null;
            
            // Create a new head
            headNode = new QuadTreeNode<T>(newWorld, maxItems, Resize);

            // Reinsert the items
            foreach (QuadTreePositionItem<T> m in Components)
            {
                headNode.Insert(m);
            }
        }

        public void ZoomReactor(double scale, double absoluteZoom)
        {
            // 2.0 is the current bounding box extension for better hitting the box :)
            headNode.ReactOnZoom(2.0 / absoluteZoom);
        }

        #endregion

        #region Query methods

        /// <summary>
        /// Gets a list of items containing a specified point
        /// </summary>
        /// <param name="Point">The point</param>
        /// <param name="ItemsFound">The list to add found items to (list will not be cleared first)</param>
        public void GetItems(Vector2 Point, ref List<QuadTreePositionItem<T>> ItemsList)
        {
            if (ItemsList != null)
                headNode.GetItems(Point, ref ItemsList);
        }

        /// <summary>
        /// Gets a list of items intersecting a specified rectangle
        /// </summary>
        /// <param name="Rect">The rectangle</param>
        /// <param name="ItemsFound">The list to add found items to (list will not be cleared first)</param>
        public void GetItems(DRect Rect, ref List<QuadTreePositionItem<T>> ItemsList)
        {
            if (ItemsList != null)
            {
                headNode.GetItems(Rect, ref ItemsList);
            }
        }

        /// <summary>
        /// Get a list of all items in the quadtree
        /// </summary>
        /// <param name="ItemsFound">The list to add found items to (list will not be cleared first)</param>
        public void GetAllItems(ref List<QuadTreePositionItem<T>> ItemsList)
        {
            if (ItemsList != null)
            {
                headNode.GetAllItems(ref ItemsList);
            }
        }

        #endregion
    }
}
