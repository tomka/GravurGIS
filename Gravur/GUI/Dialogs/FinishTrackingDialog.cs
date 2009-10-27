using System; 
using System.Drawing; 
using System.Collections; 
using System.ComponentModel; 
using System.Windows.Forms;
using System.Collections.Generic;

namespace GravurGIS.GUI.Dialogs
{
    /// <summary> 
    /// Summary description for Form2. 
    /// </summary> 
    public class FinishTrackingDialog : IDialog
    {
    
		public enum TrackingInterpretations { AsPoints, AsPolyline, AsPolygon }
		
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label Caption;
        private System.Windows.Forms.Label text;
        private TextBox tbComment;
        private ComboBox comboBox1;
        private Label label1;
		private Label label2;
		private RadioButton rbInterpretAsPolygon;
		private RadioButton rbInterpretAsPolyline;
		private RadioButton rbInterpretAsPoints;
        private System.Windows.Forms.Panel panel1;

        public FinishTrackingDialog(MainControler mainControler)
        {
            // 
            // Required for Windows Form Designer support 
            // 
            InitializeComponent();
            saveButton.DialogResult = DialogResult.OK;
            cancelButton.DialogResult = DialogResult.Cancel;
            
            List<String> cats = mainControler.Config.CategoryList;

            for (int i = 0; i < cats.Count; i++)
            {
                comboBox1.Items.Add(cats[i]);
            }

            //this.comboBox1.DisplayMember = this.comboBox1.ValueMember = "";
        }
		public FinishTrackingDialog(MainControler mc, Rectangle visibleRect, int nrTrackedPoints)
            : this(mc)
        {
            this.Location = new System.Drawing.Point((visibleRect.Width - this.Width) / 2,
                (visibleRect.Height - this.Height) / 2 + visibleRect.Y);

			rbInterpretAsPoints.Enabled = (nrTrackedPoints > 0);
            rbInterpretAsPolyline.Enabled = (nrTrackedPoints > 1);
            rbInterpretAsPolygon.Enabled = (nrTrackedPoints > 2);
        }

        /// <summary> 
        /// Clean up any resources being used. 
        /// </summary> 
        protected override void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor. 
        /// </summary> 
        private void InitializeComponent()
        {
			this.panel1 = new System.Windows.Forms.Panel();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tbComment = new System.Windows.Forms.TextBox();
			this.saveButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.text = new System.Windows.Forms.Label();
			this.Caption = new System.Windows.Forms.Label();
			this.rbInterpretAsPoints = new System.Windows.Forms.RadioButton();
			this.rbInterpretAsPolyline = new System.Windows.Forms.RadioButton();
			this.rbInterpretAsPolygon = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.rbInterpretAsPolygon);
			this.panel1.Controls.Add(this.rbInterpretAsPolyline);
			this.panel1.Controls.Add(this.rbInterpretAsPoints);
			this.panel1.Controls.Add(this.comboBox1);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.tbComment);
			this.panel1.Controls.Add(this.saveButton);
			this.panel1.Controls.Add(this.cancelButton);
			this.panel1.Controls.Add(this.text);
			this.panel1.Location = new System.Drawing.Point(1, 22);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(208, 205);
			// 
			// comboBox1
			// 
			this.comboBox1.Location = new System.Drawing.Point(79, 148);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(121, 22);
			this.comboBox1.TabIndex = 5;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(11, 149);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(71, 20);
			this.label1.Text = "Kategorie:";
			// 
			// textBox1
			// 
			this.tbComment.Location = new System.Drawing.Point(11, 118);
			this.tbComment.Multiline = true;
			this.tbComment.Name = "textBox1";
			this.tbComment.Size = new System.Drawing.Size(189, 24);
			this.tbComment.TabIndex = 3;
			// 
			// saveButton
			// 
			this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.saveButton.Location = new System.Drawing.Point(112, 178);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(88, 20);
			this.saveButton.TabIndex = 0;
			this.saveButton.Text = "OK";
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cancelButton.Location = new System.Drawing.Point(11, 178);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(89, 20);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "Abbrechen";
			// 
			// text
			// 
			this.text.Location = new System.Drawing.Point(7, 97);
			this.text.Name = "text";
			this.text.Size = new System.Drawing.Size(75, 18);
			this.text.Text = "Kommentar:";
			// 
			// Caption
			// 
			this.Caption.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular);
			this.Caption.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.Caption.Location = new System.Drawing.Point(6, 4);
			this.Caption.Name = "Caption";
			this.Caption.Size = new System.Drawing.Size(172, 18);
			this.Caption.Text = "Tracking abschlieﬂen";
			// 
			// rbInterpretAsPoints
			// 
			this.rbInterpretAsPoints.Checked = true;
			this.rbInterpretAsPoints.Location = new System.Drawing.Point(13, 30);
			this.rbInterpretAsPoints.Name = "rbInterpretAsPoints";
			this.rbInterpretAsPoints.Size = new System.Drawing.Size(118, 20);
			this.rbInterpretAsPoints.TabIndex = 8;
			this.rbInterpretAsPoints.Text = "Einzelne Punkte";
			// 
			// rbInterpretAsPolyline
			// 
			this.rbInterpretAsPolyline.Location = new System.Drawing.Point(13, 49);
			this.rbInterpretAsPolyline.Name = "rbInterpretAsPolyline";
			this.rbInterpretAsPolyline.Size = new System.Drawing.Size(86, 20);
			this.rbInterpretAsPolyline.TabIndex = 9;
			this.rbInterpretAsPolyline.TabStop = false;
			this.rbInterpretAsPolyline.Text = "Linienzug";
			// 
			// rbInterpretAsPolygon
			// 
			this.rbInterpretAsPolygon.Location = new System.Drawing.Point(13, 68);
			this.rbInterpretAsPolygon.Name = "rbInterpretAsPolygon";
			this.rbInterpretAsPolygon.Size = new System.Drawing.Size(72, 20);
			this.rbInterpretAsPolygon.TabIndex = 10;
			this.rbInterpretAsPolygon.TabStop = false;
			this.rbInterpretAsPolygon.Text = "Polygon";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(190, 15);
			this.label2.Text = "Interpretiere Wegpunkte als ...";
			// 
			// FinishTrackingDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.ClientSize = new System.Drawing.Size(210, 230);
			this.ControlBox = false;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.Caption);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MinimizeBox = false;
			this.Name = "FinishTrackingDialog";
			this.TopMost = true;
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseDown);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form2_MouseUp);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

        }
        #endregion

        int Xdif, Ydif;
        private void Form2_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Xdif = e.X;
            Ydif = e.Y;
        }
        private void Form2_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Location = new Point(Location.X + e.X - Xdif, Location.Y + e.Y - Ydif);
        }

        public string Comment
        {
            get { return this.tbComment.Text; }
            set { this.tbComment.Text = value; }
        }
        
        public TrackingInterpretations TrackingInterpretation {
			get { 
				if (rbInterpretAsPolyline.Checked)
					return TrackingInterpretations.AsPolyline;
				else if (rbInterpretAsPolygon.Checked)
					return TrackingInterpretations.AsPolygon;
				else
					return TrackingInterpretations.AsPoints;
			}
        }

        public string Category
        {
            get { return (comboBox1.SelectedItem == null) ? String.Empty : this.comboBox1.SelectedItem.ToString(); }
            set
            {
                if (!comboBox1.Items.Contains(value))
                    comboBox1.Items.Add(value);
                comboBox1.SelectedItem = value;
            }
        }

        public void resetComment()
        {
            this.tbComment.Text = "";
        }
        public new DialogResult ShowDialog()
        {
            this.tbComment.Focus();
            return base.ShowDialog();
        }

        public override void resizeToRect(Rectangle visibleRect)
        {
            this.Location = new System.Drawing.Point((visibleRect.Width - this.Width) / 2,
                (visibleRect.Height - this.Height) / 2 + visibleRect.Y);
            this.Invalidate();
        }
    }
}
