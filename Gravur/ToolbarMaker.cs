using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

namespace GravurGIS
{
    /// <summary>
    /// Component that handles the issue of transparent icons in toolbars.
    /// All you have to do to get this working, is:
    /// 
    /// 1. Prepare the icons. They must be of 16x16 size, 256 colours palette
    /// 2. Add icons to your project, compile action must be set to "Embedded resource"
    /// 3. Drop the toolbar on the form.
    /// 4. Assign it to the Toolbar property of this static class.
    /// 5. Make a number of calls to AddIcon method, giving it as parameters 
    /// names of icons representing subsequent buttons on the form.
    /// When giving names of the icons, you do not have to specify the .ico
    /// extension, it will be added there for you, which makes the code 
    /// just looking a bit nicer.
    /// 
    /// AddIcon function takes care of automatic skipping the separators, 
    /// thus absolutely no need to add fake AddIcon calls to skip them!
    /// 
    /// Finally, all this code should be best placed in form's constructor, right 
    /// after a call to InitializeComponent() method.
    /// 
    /// Example:
    /// 
    /// ToolbarMaker.ToolBar = myToolBar;
    /// ToolbarMaker.AddIcon("icon_close");
    /// ToolbarMaker.AddIcon("icon_dostuff");
    /// ToolbarMaker.AddIcon("icon_domorestuff");
    /// ToolbarMaker.AddIcon("icon_help");
    /// 
    /// </summary>
    public class ToolbarMaker
    {
        public ToolbarMaker()
        {
        }

        /// <summary>
        /// Toolbar being handled
        /// </summary>
        public static ToolBar ToolBar
        {
            get { return fToolBar; }
            set
            {
                fToolBar = value;
                ButtonCounter = -1;
                if (fToolBar != null)
                {
                    ToolBarImages = new ImageList();
                    if (DisplayResolution == DisplayResolution.QVGA)
                        ToolBarImages.ImageSize = new Size(16, 16);
                    else
                        ToolBarImages.ImageSize = new Size(32, 32);
                    fToolBar.ImageList = ToolBarImages;
                }
            }
        }

        /// <summary>
        /// Adds icon to the toolbar, assigns it to 
        /// the next subsequent button.
        /// </summary>
        public static void AddIcon(string IconName)
        {
            // exit if no toolbar or not enough buttons for the new image
            if (ToolBar == null)
                return;
            if (ButtonCounter >= ToolBar.Buttons.Count)
                return;
            // move the counter until next non-separator button is found.
            ButtonCounter++;
            bool Found = false;
            while (!Found)
            {
                // no more buttons - leave
                if (ButtonCounter >= ToolBar.Buttons.Count)
                    break;

                // non-separator button found!
                if (ToolBar.Buttons[ButtonCounter].Style != ToolBarButtonStyle.Separator)
                {
                    Found = true;
                    break;
                }

                // try next one
                ButtonCounter++;
            }
            // if no button found, leave.
            if (!Found)
                return;
            // otherwise get the icon
            Icon icon = GetIconFromFile(IconName);
            if (icon != null)
            {
                // store in the image list
                ToolBarImages.Images.Add(icon);
                // and assign to the button
                ToolBar.Buttons[ButtonCounter].ImageIndex = ToolBarImages.Images.Count - 1;
            }
        }

        /// <summary>
        /// Retrieves an icon from resources.
        /// Icon must be specified by resource name,
        /// for convenience .ico extension is optional.
        /// </summary>
        public static Icon GetIconFromResource(string IconName)
        {
            // load the icon from resources
            Stream iconStream;
            String tempStr = String.Format("{0}.{1}", ThisAssembly.GetName().Name,IconName);

            if (DisplayResolution == DisplayResolution.QVGA)
                iconStream = ThisAssembly.GetManifestResourceStream(tempStr + ".ico");
            else
                iconStream = ThisAssembly.GetManifestResourceStream(tempStr +  "32.ico");

            Icon theIcon = new Icon(iconStream);
            iconStream.Close();


            return theIcon;
        }

        public static Icon GetIconFromFile(string name)
        {
            try
            {
                FileStream theStream;
                if (DisplayResolution == DisplayResolution.QVGA)
                    theStream = new FileStream(name + ".ico", System.IO.FileMode.Open, FileAccess.Read, FileShare.None);
                else
                    theStream = new FileStream(name + "32.ico", System.IO.FileMode.Open, FileAccess.Read, FileShare.None);
                
                Icon theIcon = new Icon(theStream);

                theStream.Close();
                return theIcon;
            }
            catch (Exception)
            {
                return GetIconFromResource("Icons.notfound");
            }
        }

        public static DisplayResolution DisplayResolution
        {
            get { return displayResolution; }
            set { displayResolution = value; }
        }

        #region Private members
        private static int ButtonCounter = 0;
        private static ImageList ToolBarImages = null;
        private static ToolBar fToolBar = null;
        private static Assembly ThisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
        private static DisplayResolution displayResolution = DisplayResolution.QVGA;
        #endregion
    }
}