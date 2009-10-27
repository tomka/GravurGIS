namespace GravurGIS.GUI.Controls
{
    partial class DecimalUpDown
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DecimalUpDown));
            this.txtValue = new System.Windows.Forms.TextBox();
            this.picUp = new UserIcon();
            this.picDown = new UserIcon();
            this.tmrUpDown = new System.Windows.Forms.Timer();
            this.SuspendLayout();
            // 
            // txtValue
            // 
            this.txtValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtValue.Location = new System.Drawing.Point(0, 0);
            this.txtValue.MaxLength = 12;
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(56, 22);
            this.txtValue.TabIndex = 0;
            this.txtValue.Text = "0.00";
            this.txtValue.LostFocus += new System.EventHandler(this.Control_ChangeFocus);
            this.txtValue.GotFocus += new System.EventHandler(this.Control_ChangeFocus);
            this.txtValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtValue_KeyPress);
            this.txtValue.Validating += new System.ComponentModel.CancelEventHandler(this.txtValue_Validating);
            this.txtValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtValue_KeyDown);
            // 
            // picUp
            // 
            this.picUp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.picUp.Location = new System.Drawing.Point(56, 0);
            this.picUp.Name = "picUp";
            this.picUp.Tag = "UP";
            this.picUp.Click += new System.EventHandler(this.UpDown_Click);
            this.picUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UpDown_MouseDown);
            this.picUp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UpDown_MouseUp);
            this.picUp.Size = new System.Drawing.Size(14, 22);
            this.picUp.Type = UserIcon.IconType.UpArrow;
            // 
            // picDown
            // 
            this.picDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.picDown.BackColor = System.Drawing.Color.Black;
            this.picDown.Location = new System.Drawing.Point(70, 0);
            this.picDown.Name = "picDown";
            this.picDown.Size = new System.Drawing.Size(14, 22);
            this.picDown.Tag = "DOWN";
            this.picDown.Click += new System.EventHandler(this.UpDown_Click);
            this.picDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UpDown_MouseDown);
            this.picDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UpDown_MouseUp);
            this.picDown.Type = UserIcon.IconType.DownArrow;
            // 
            // tmrUpDown
            // 
            this.tmrUpDown.Interval = 500;
            this.tmrUpDown.Tick += new System.EventHandler(this.tmrUpDown_Tick);
            // 
            // DecimalUpDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(84F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.txtValue);
            this.Controls.Add(this.picDown);
            this.Controls.Add(this.picUp);
            this.Name = "DecimalUpDown";
            this.Size = new System.Drawing.Size(82, 21);
            this.Resize += new System.EventHandler(this.UpDown_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtValue;
        private UserIcon picUp;
        private UserIcon picDown;
        private System.Windows.Forms.Timer tmrUpDown;
    }
}
