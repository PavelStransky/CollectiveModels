using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace PavelStransky.Forms {
    /// <summary>
    /// Summary description for WinMain.
    /// </summary>
    public class WinMain {
        private static string directory = defaultDirectory;

        /// <summary>
        /// Aktu�ln� adres��
        /// </summary>
        public static string Directory { get { return directory; }}

        /// <summary>
        /// Nastav� adres�� podle n�zvu souboru
        /// </summary>
        /// <param name="fName">N�zev souboru</param>
        public static void SetDirectoryFromFile(string fName) {
            FileInfo f = new FileInfo(fName);
            directory = f.DirectoryName;
        }

        public static string FileFilterTxt { get { return defaultFileFilterTxt; } }
        public static string FileFilterGif { get { return defaultFileFilterGif; } }
        public static string FileFilterGcm { get { return defaultFileFilterGcm; } }
        public static string FileFilterPicture { get { return defaultFileFilterPicture; } }
        public static string FileExtGcm { get { return defaultFileExtGcm; } }
        public static string FileExtTxt { get { return defaultFileExtTxt; } }
        public static string FileExtGif { get { return defaultFileExtGif; } }
        public static string FileExtPicture { get { return defaultFileExtPicture; } }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            if(args.Length == 0)
                Application.Run(new MainForm());
            else
                Application.Run(new MainForm(args[0]));
        }

        private const string defaultFileFilterTxt = "Textov� soubory (*.txt)|*.txt|V�echny soubory (*.*)|*.*";
        private const string defaultFileFilterGif = "Obr�zek (*.gif)|*.gif|V�echny soubory (*.*)|*.*";
        private const string defaultFileFilterPicture = "Obr�zek GIF (*.gif)|*.gif|Obr�zek JPG (*.jpg)|*.jpg|Obr�zek PNG (*.png)|*.png";
        private const string defaultFileFilterGcm = "Soubory historie (*.gcm)|*.gcm|V�echny soubory (*.*)|*.*";
        private const string defaultFileExtGcm = "gcm";
        private const string defaultFileExtTxt = "txt";
        private const string defaultFileExtGif = "gif";
        private const string defaultFileExtPicture = "png";
        private const string defaultDirectory = "c:\\gcm";
    }
}
