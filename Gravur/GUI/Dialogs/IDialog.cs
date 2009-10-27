using System.Windows.Forms;
using GravurGIS.GUI.Controls;

namespace GravurGIS.GUI.Dialogs
{
    public class IDialog : Form // for designer suppert - change two code below for runtime
    //public abstract class IDialog : Form
    {
        protected ToolBarButton tbOkButton;
        protected ToolBarButton tbCancelButton;
        protected ToolBar toolBar;
        private ResultButton resultButton;

        #region Imports

        [System.Runtime.InteropServices.DllImport("coredll.dll")]
        private static extern bool SetForegroundWindow(System.IntPtr hWnd);

        #endregion

        public IDialog() : base()
        {
            // Button
            resultButton = new ResultButton();
            this.Controls.Add(resultButton);

            this.toolBar = new System.Windows.Forms.ToolBar();
            this.tbOkButton = new System.Windows.Forms.ToolBarButton();
            this.tbCancelButton = new System.Windows.Forms.ToolBarButton();

            // 
            // toolBar
            // 
            this.toolBar.Buttons.Add(this.tbOkButton);
            this.toolBar.Buttons.Add(this.tbCancelButton);
            this.toolBar.Name = "toolBar";
            this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);

            this.Controls.Add(this.toolBar);

            // Toolbar
            string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            ToolbarMaker.ToolBar = toolBar;
            ToolbarMaker.AddIcon(appPath + @"\Icons\tbOk");
            ToolbarMaker.AddIcon(appPath + @"\Icons\tbCancel");


            this.TopMost = true;
        }

        private void InitializeComponent() {

        }

        public void ActivateToolBar() {

            for (int i = 0; i < toolBar.Buttons.Count; i++)
                toolBar.Buttons[i].Visible = true;
        }

        public void HideToolBar()
        {
            for (int i = 0; i < toolBar.Buttons.Count; i++)
                toolBar.Buttons[i].Visible = false;
        }


        protected virtual void toolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button.Equals(tbOkButton))
                resultButton.DialogResult = DialogResult.OK;
            else resultButton.DialogResult = DialogResult.Cancel;
            
            resultButton.raiseclick();
        }


        public virtual void resizeToRect(System.Drawing.Rectangle visibleRect) { } // for designer suppert - change two code below for runtime
        //public abstract void resizeToRect(System.Drawing.Rectangle visibleRect);
    }
}
