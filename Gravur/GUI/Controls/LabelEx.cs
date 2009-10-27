using System.Windows.Forms;
using System.Drawing;
using System.Text;
using System;
using System.Collections.Generic;

namespace GravurGIS.GUI.Controls
{
	public class LabelEx : UserControl
	{
		public char LinebreakCharacter { get; set; }
		public Boolean ShowToolTip { get; set; }
		private ToolTip toolTip;

		public LabelEx()
		{
			LinebreakCharacter ='\\';
			ShowToolTip = true;

			toolTip = new GravurGIS.GUI.Controls.ToolTip();
			toolTip.Visible = false;

			this.Controls.Add(toolTip);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (!String.IsNullOrEmpty(this.Text))
			{
				toolTip.Location = new Point(e.X, e.Y);
				toolTip.Text = this.Text;
				toolTip.Visible = true;
			}
			
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			toolTip.Visible = false;

			base.OnMouseUp(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			SizeF _stingSize = g.MeasureString(this.Text, this.Font);
			SizeF currentWordSize, currentLineSize;

			int width = this.Width;

			if (_stingSize.Width < width)
			{
				base.OnPaint(e);
			}
			else
			{
				// Return empty list of strings if the text was empty
				if (this.Text.Length == 0) return;

				var words = this.Text.Split(LinebreakCharacter);

				var lines = new List<string>();
				var currentLine = "";

				foreach (var currentWord in words)
				{
					currentWordSize = g.MeasureString(currentWord, this.Font);
					currentLineSize = g.MeasureString(currentLine, this.Font);

					if ((currentWordSize.Width > width) ||
						((currentLineSize.Width + currentWordSize.Width) > width))
					{
						lines.Add(currentLine);
						currentLine = "";
					}

					if (currentLineSize.Width > 0)
						currentLine += "\\" + currentWord;
					else
						currentLine += currentWord;
				}

				if (currentLine.Length > 0)
					lines.Add(currentLine);

				StringBuilder _text = new StringBuilder();

				foreach (string inputString in lines)
					_text.Append(inputString + Environment.NewLine);

				g.DrawString(_text.ToString(), this.Font, new SolidBrush(this.ForeColor), 0, 0);
			}

		}

		


	}
}
