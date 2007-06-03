using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using Microsoft.Win32;

using PavelStransky.Expression;

namespace PavelStransky.Forms {
    /// <summary>
    /// Summary description for WinMain.
    /// </summary>
    public class WinMain {
        private static string directory;
        private static bool playSounds;

        /// <summary>
        /// Aktuální adresáø
        /// </summary>
        public static string Directory { get { return directory; }}

        /// <summary>
        /// True, pokud se pøehrávají zvuky
        /// </summary>
        public static bool PlaySounds { get { return playSounds; } set { playSounds = value; } }

        /// <summary>
        /// Nastaví adresáø podle názvu souboru
        /// </summary>
        /// <param name="fName">Název souboru</param>
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
        public static string FileExtWmf { get { return defaultFileExtWmf; } }
        public static string FileExtPicture { get { return defaultFileExtPicture; } }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args) {
            if(args.Length == 0)
                Application.Run(new MainForm());
            else
                Application.Run(new MainForm(args[0]));
        }

        /// <summary>
        /// Statický konstruktor - provede naètení základních parametrù z registru
        /// </summary>
        static WinMain() {
            object dir = GetRegistryValue(registryKeyDirectory);
            if(dir is string && dir as string != string.Empty)
                directory = dir as string;
            else
                directory = defaultDirectory;

            object fncDir = GetRegistryValue(registryKeyFncDirectory);
            if(fncDir is string && fncDir as string != string.Empty)
                Context.FncDirectory = fncDir as string;
            else
                Context.FncDirectory = defaultDirectory;

            object gcDir = GetRegistryValue(registryKeyGlobalContextDirectory);
            if(gcDir is string && gcDir as string != string.Empty)
                Context.GlobalContextDirectory = gcDir as string;
            else
                Context.GlobalContextDirectory = defaultDirectory;

            Context.ExecDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            Context.WorkingDirectory = Application.UserAppDataPath;

            object ps = GetRegistryValue(registryKeyPlaySounds);
            if(ps is string && !bool.Parse(ps as String))
                playSounds = false;
            else
                playSounds = true;
        }

        /// <summary>
        /// Provede uložení nastavení
        /// </summary>
        public static void SaveSettings(){
            SetRegistryValue(registryKeyDirectory, directory);
            SetRegistryValue(registryKeyFncDirectory, Context.FncDirectory);
            SetRegistryValue(registryKeyGlobalContextDirectory, Context.GlobalContextDirectory);
            SetRegistryValue(registryKeyPlaySounds, playSounds);
        }

        /// <summary>
        /// Vytvoøí název aplikace pro registr Windows
        /// </summary>
        public static string RegistryEntryName {
            get {
                string companyName = Application.CompanyName.Trim().Replace(" ", string.Empty);
                string productName = Application.ProductName.Trim().Replace(" ", string.Empty);
                string version = Application.ProductVersion; version = version.Substring(0, version.IndexOf('.'));
                return string.Format("{0}.{1}.{2}", companyName, productName, version);
            }
        }

        /// <summary>
        /// Je program zaregistrován?
        /// </summary>
        public static bool IsRegistered {
            get {
                string path = Application.ExecutablePath;
                string keyName = RegistryEntryName;

                // Existuje klíè v registrech?
                RegistryKey rk = Registry.ClassesRoot.OpenSubKey(string.Format(commandEntryName, keyName));
                if(rk == null)
                    return false;

                // Existuje záznam s cestou?
                string commandEntry = rk.GetValue(string.Empty) as string;
                if(commandEntry == null || commandEntry == string.Empty)
                    return false;

                if(string.Format(commandEntryFormat, path) != commandEntry)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Vyzvedne z registrù hodnotu klíèe
        /// </summary>
        /// <param name="keyName">Název klíèe</param>
        /// <param name="delete">True, pokud má být klíè po vyzvednutí vymazán</param>
        public static object GetRegistryValue(string keyName, bool delete) {
            object result = Application.UserAppDataRegistry.GetValue(keyName);
            if(delete)
                Application.UserAppDataRegistry.DeleteValue(keyName, false);
            return result;
        }

        /// <summary>
        /// Vyzvedne z registrù hodnotu klíèe
        /// </summary>
        /// <param name="keyName">Název klíèe</param>
        public static object GetRegistryValue(string keyName) {
            return GetRegistryValue(keyName, false);
        }

        /// <summary>
        /// Zapíše hodnotu klíèe do registrù
        /// </summary>
        /// <param name="keyName">Název klíèe</param>
        /// <param name="value">Hodnota klíèe</param>
        public static void SetRegistryValue(string keyName, object value) {
            Application.UserAppDataRegistry.SetValue(keyName, value);
        }

        /// <summary>
        /// Zaregistruje pøíponu programu do Windows
        /// </summary>
        /// <param name="state">True pro zaregistrování, False pro odregistrování</param>
        public static void Register(bool state) {
            string path = Application.ExecutablePath;
            string keyName = RegistryEntryName;

            if(state) {
                Registry.ClassesRoot.CreateSubKey('.' + WinMain.FileExtGcm).SetValue(string.Empty, keyName);
                Registry.ClassesRoot.CreateSubKey(keyName).SetValue(string.Empty, programDescription);
                Registry.ClassesRoot.CreateSubKey(string.Format("{0}\\DefaultIcon", keyName)).SetValue(string.Empty, string.Format("{0},0", path));
                Registry.ClassesRoot.CreateSubKey(string.Format(commandEntryName, keyName)).SetValue(string.Empty, string.Format(commandEntryFormat, path));
            }
            else {
                try {
                    Registry.ClassesRoot.DeleteSubKeyTree(keyName);
                    Registry.ClassesRoot.DeleteSubKeyTree('.' + WinMain.FileExtGcm);
                }
                catch(Exception) {
                }
            }
        }

        private const string defaultFileFilterTxt = "Textové soubory (*.txt)|*.txt|Všechny soubory (*.*)|*.*";
        private const string defaultFileFilterGif = "Obrázek (*.gif)|*.gif|Všechny soubory (*.*)|*.*";
        private const string defaultFileFilterPicture = "Obrázek GIF (*.gif)|*.gif|Obrázek JPG (*.jpg)|*.jpg|Obrázek PNG (*.png)|*.png|Obrázek WMF (*.wmf)|*.wmf";
        private const string defaultFileFilterGcm = "Soubory s daty (GCM) (*.gcm)|*.gcm|Všechny soubory (*.*)|*.*";
        private const string defaultFileExtGcm = "gcm";
        private const string defaultFileExtTxt = "txt";
        private const string defaultFileExtGif = "gif";
        private const string defaultFileExtWmf = "wmf";
        private const string defaultFileExtPicture = "png";
        private const string defaultDirectory = "c:\\gcm";

        private const string commandEntryName = "{0}\\shell\\open\\command";
        private const string commandEntryFormat = "\"{0}\" \"%1\"";
        private const string programDescription = "Program for analysing nuclear collective models (GCM, IBM)";

        private const string registryKeyDirectory = "Directory";
        private const string registryKeyFncDirectory = "FncDirectory";
        private const string registryKeyGlobalContextDirectory = "GlobalContextDirectory";
        private const string registryKeyPlaySounds = "PlaySounds";
    }
}
