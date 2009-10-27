using System;
using System.IO;

namespace Bootstrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                System.Diagnostics.Process.Start("msiexec", @" /i files\Installer.msi REINSTALLMODE=vomus REINSTALL=ALL");
            }
            catch (FileNotFoundException)
            {
                System.Windows.Forms.MessageBox.Show("Leider konnte der Installer nicht ausgeführt werden, da die Datei \"files\\Installer.msi\" nicht gefunden wurde.");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(String.Format("Leider ist ein Fehler aufgetreten:{0}{1}", Environment.NewLine, ex.Message));
            }
        }
    }
}
