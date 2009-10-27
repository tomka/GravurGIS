namespace GravurGIS.GUI.Controls
{
    public class ResultButton : System.Windows.Forms.Button
    {
        public ResultButton()
        {
            this.Visible = false;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        protected override void OnClick(System.EventArgs e)
        {
            base.OnClick(e);
        }

        public void raiseclick()
        {
            System.EventArgs e = new System.EventArgs();
            OnClick(e);
        }
    }
}