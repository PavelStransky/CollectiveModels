using System;
using System.Windows.Forms;

namespace PavelStransky.Forms
{
	/// <summary>
	/// Summary description for WinMain.
	/// </summary>
	public class WinMain
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
        [STAThread]
        static void Main(string[] args) {
            if(args.Length == 0)
                Application.Run(new MainForm());
  //          else
//                Application.Run(new MainForm(args[0]));
        }

        private const string defaultFileName = @"c:\gcm\Regularita.his";
	}
}
