//
// Common Dialog wrapper class for FolderBrowserForm
// Designed to follow object model of desktop framework control
// (c) 2004 Peter Foot, OpenNETCF
//
using System;
using System.Windows.Forms;

namespace GravurGIS.GUI.Dialogs
{
	/// <summary>
	/// Represents a common dialog box that allows the user to choose a folder.
	/// </summary>
	public class FolderBrowserDialog : CommonDialog
	{
		private FolderBrowserForm m_dialog;

		/// <summary>
		/// Initializes a new instance of the OpenNETCF.Windows.Forms.FolderBrowserDialog class.
		/// </summary>
		public FolderBrowserDialog(MainControler mc)
		{
			m_dialog = new FolderBrowserForm(mc);
		}

		/// <summary>
		/// Runs a common dialog box with a default owner.
		/// </summary>
		/// <returns></returns>
		public new DialogResult ShowDialog()
		{
			return m_dialog.ShowDialog();
		}

		/// <summary>
		/// Resets properties to their default values.
		/// </summary>
		public void Reset()
		{
			m_dialog.Reset();
		}

		/// <summary>
		/// Gets or sets the path selected by the user.
		/// </summary>
		public string SelectedPath
		{
			get
			{
				return m_dialog.SelectedPath;
			}
			set
			{
				m_dialog.SelectedPath = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the New Folder button appears in the folder browser dialog box.
		/// </summary>
        //public bool ShowNewFolderButton
        //{
        //    get
        //    {
        //        return m_dialog.ShowNewFolderButton;
        //    }
        //    set
        //    {
        //        m_dialog.ShowNewFolderButton = value;
        //    }
        //}

		/// <summary>
		/// </summary>
		public new void Dispose()
		{
			m_dialog.Dispose();
            base.Dispose();
		}
	}
}
