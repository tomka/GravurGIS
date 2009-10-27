using System;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Net;
using AppOneCopy_CS;
using GravurGIS.Utilities;
using System.IO;

namespace GravurGIS
{
    static class Program
    {
        const string appName = "GravurGIS";

		static string appSavePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + Config.ProgramName;
		
        internal static int startTicks;
		private static Utilities.NotifyIcon notifyIcon;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
                startTicks = Environment.TickCount;

                // make sure only one instance exists
                using (AppExecutionManager execMgr = new AppExecutionManager(appName))
                {
                    if (execMgr.IsFirstInstance)
                    {
                        // Set the maximum allowed amount of connections up
                        // This is needed for MapServer and other outgoing connections
                        // since the garbage collector is not that fast
                        ServicePointManager.DefaultConnectionLimit = 100;
						
						MainControler mainController = null;
						
						try
						{
							// create a notify icon
							notifyIcon = new Utilities.NotifyIcon();
							
							// load the application icon (Resource ID 32512)
							IntPtr hIcon = Win32.LoadIcon(Win32.GetModuleHandle(null), "#32512");
							// add the Icon to the systray
							notifyIcon.Add(hIcon);
							
	                        mainController = new MainControler(appName);
						}
						catch (Exception e)
						{
							logException(e);
							
							MessageBox.Show(String.Format("Es trat ein Fehler beim Laden der Anwendung auf.{0}Nachricht: {1}", Environment.NewLine, e.Message), "Fehler");
							notifyIcon.Remove();
							execMgr.Dispose();
						}
						
						try
						{
							Application.Run(mainController.MainForm);
						}
						catch (Exception e)
						{
							string logfile = logException(e);
							MessageBox.Show(String.Format("Es trat leider ein Programmfehler auf.{0}Nachricht: {1}{0}Log: {2}", Environment.NewLine, e.Message, logfile), "Fehler");
						}
						finally {
							notifyIcon.Remove();
							execMgr.Dispose();
						}
                    }
                    else
                    {
                        try
                        {
                            if (!execMgr.ActivateFirstInstance())
                                MessageBox.Show(
                                    String.Format("Es kann nur eine Programm-Instanz von {0} ausgeführt werden.\nDie bereits laufende Instanz konnte jedoch leider nicht aktiviert werden.", appName), "Hinweis",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        }
                        catch (Exception ex)
                        { 
                            MessageBox.Show("Es läuft bereits eine Instanz von GravurGIS. Es konnt diese jedoch nicht aktiviert werden, da folgender Fehler auftrat: " + ex.Message);
                        }
                    }
                }
        }
        
        public static string AppDataPath {
			get { return appSavePath; }
		}
        
        private static string logException(Exception e) {
			StreamWriter logFS = null;
			try {
				Random ran = new Random();
				string logfile = String.Format("{0}\\{1}_Error_{2}.log", appSavePath, Config.ProgramName, ran.Next());
				
				logFS = new StreamWriter(logfile, true);
				
				logFS.WriteLine(String.Format("GravurGIS Error Log of {0}", DateTime.Now));
				logFS.Write("Exception: {0}{1}InnerException: {2}{1}Stacktrace: {3}{1}",
					e.Message,
					Environment.NewLine,
					e.InnerException != null ? e.InnerException.ToString() : "None",
					e.StackTrace);
				
				return logfile;
			}
			catch {
				return "-";
			}
			finally {
				if (logFS != null) logFS.Close();
			}
        }
        
        public static NotifyIcon NotifyIcon {
			get { return notifyIcon; }
		}
    }
}