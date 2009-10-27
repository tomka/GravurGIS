//==========================================================================================
//
//		OpenNETCF.Drawing.ColorTranslator
//		Copyright (c) 2003, OpenNETCF.org
//
//		This library is free software; you can redistribute it and/or modify it under 
//		the terms of the OpenNETCF.org Shared Source License.
//
//		This library is distributed in the hope that it will be useful, but 
//		WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
//		FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
//		for more details.
//
//		You should have received a copy of the OpenNETCF.org Shared Source License 
//		along with this library; if not, email licensing@opennetcf.org to request a copy.
//
//		If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
//		email licensing@opennetcf.org.
//
//		For general enquiries, email enquiries@opennetcf.org or visit our website at:
//		http://www.opennetcf.org
//
//==========================================================================================
using System;
using System.Drawing;

namespace GravurGIS.GUI
{
	/// <summary>
	/// Translates colors to and from <see cref="T:System.Drawing.Color"/> structures.
	/// </summary>
	/// <seealso cref="T:System.Drawing.ColorTranslator">System.Drawing.ColorTranslator Class</seealso>
	public sealed class ColorTranslator
	{
		private ColorTranslator(){}

		#region To Html
		/// <summary>
		/// Translates the specified <see cref="T:System.Drawing.Color"/> structure to an HTML string color representation.
		/// </summary>
		/// <param name="c">The <see cref="T:System.Drawing.Color"/> structure to translate.</param>
		/// <returns>The string that represents the HTML color.</returns>
		/// <remarks>Unlike the desktop version of this function it does not check for named colors but instead always returns the hex notation values - e.g. Color.Red = "#FF0000"</remarks>
		/// <seealso cref="M:System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color)">System.Drawing.ColorTranslator.ToHtml Method</seealso>
		public static string ToHtml(Color c)
		{		
			return string.Format("#{0:X6}", (c.R << 16) + (c.G << 8) + c.B);
		}
		#endregion

		#region To Win32
		/// <summary>
		/// Translates the specified <see cref="T:System.Drawing.Color"/> structure to a Windows color.
		/// </summary>
		/// <param name="c">The <see cref="T:System.Drawing.Color"/> structure to translate.</param>
		/// <returns>The Windows color value.</returns>
		/// <seealso cref="M:System.Drawing.ColorTranslator.ToWin32(System.Drawing.Color)">System.Drawing.ColorTranslator.ToWin32 Method</seealso>
		public static int ToWin32(System.Drawing.Color c)
		{
			return c.R + (c.G << 8) + (c.B << 16);
		}
		#endregion

		#region From Html
		/// <summary>
		/// Translates an HTML color representation to a <see cref="T:System.Drawing.Color"/> structure.
		/// </summary>
		/// <param name="htmlColor">The string representation of the Html color to translate.</param>
		/// <returns>The <see cref="T:System.Drawing.Color"/> structure that represents the translated HTML color.</returns>
		/// <seealso cref="M:System.Drawing.ColorTranslator.FromHtml(System.String)">System.Drawing.ColorTranslator.FromHtml Method</seealso>
		public static System.Drawing.Color FromHtml(string htmlColor)
		{
			Color c = Color.Empty;
			if ((htmlColor != null) && (htmlColor.Length != 0))
			{
				if ((htmlColor[0] == '#') && (htmlColor.Length == 7 || htmlColor.Length == 4))
				{
					if (htmlColor.Length == 7) // #rrggbb format
					{
						c = Color.FromArgb(Convert.ToInt32(htmlColor.Substring(1, 2), 0x10), Convert.ToInt32(htmlColor.Substring(3, 2), 0x10), Convert.ToInt32(htmlColor.Substring(5, 2), 0x10));
					}
					else // #rgb format
					{
						string r = char.ToString(htmlColor[1]);
						string g = char.ToString(htmlColor[2]);
						string b = char.ToString(htmlColor[3]);
						c = Color.FromArgb(Convert.ToInt32(r + r, 16), Convert.ToInt32(g + g, 16), Convert.ToInt32(b + b, 16));
					}
				}
			}
			if (c.IsEmpty)
			{
				//a string
				try
				{
					c = (Color)typeof(System.Drawing.Color).GetProperty(htmlColor).GetValue(null,null);
				}
				catch
				{
					throw new ArgumentException("Unable to convert color name");
				}
			}

			return c;
		}
		#endregion

		#region From Win32
		/// <summary>
		/// Translates a Windows color value to a <see cref="T:System.Drawing.Color"/> structure.
		/// </summary>
		/// <param name="win32Color">The Windows color to translate.</param>
		/// <returns>The <see cref="T:System.Drawing.Color"/> structure that represents the translated Windows color.</returns>
		/// <seealso cref="M:System.Drawing.ColorTranslator.FromWin32(System.Int32)">System.Drawing.ColorTranslator.FromWin32 Method</seealso>
		public static Color FromWin32(int win32Color)
		{
			return Color.FromArgb( win32Color & 0xff, (win32Color >> 8) & 0xff,(win32Color >> 16) & 0xff);
		}
		#endregion
	}
}
