using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Security.Permissions;

using Microsoft.Win32;

using PavelStransky.Expression;

[assembly: RegistryPermissionAttribute(SecurityAction.RequestMinimum,
    ViewAndModify = "HKEY_CURRENT_USER")]
namespace PavelStransky.Forms
{
    /// <summary>
    /// Summary description for WinMain.
    /// </summary>
    public class WinMain {
        private static string directory;
        private static string exportDirectory;

        private static string seqExtension;

        private static bool playSounds;

        /// <summary>
        /// Aktu�ln� adres��, kam se ukl�daj� soubory *.gcm
        /// </summary>
        public static string Directory { get { return directory; }}

        /// <summary>
        /// Adres��, kam se exportuje a ukl�daj� obr�zky
        /// </summary>
        public static string ExportDirectory { get { return exportDirectory; } }

        /// <summary>
        /// Nastaven� p��pona sekvence obr�zk�
        /// </summary>
        public static string SeqExtension { get { return seqExtension; } }

        /// <summary>
        /// True, pokud se p�ehr�vaj� zvuky
        /// </summary>
        public static bool PlaySounds { get { return playSounds; } set { playSounds = value; } }

        /// <summary>
        /// Nastav� adres�� podle n�zvu souboru
        /// </summary>
        /// <param name="fName">N�zev souboru</param>
        public static void SetDirectoryFromFile(string fName) {
            FileInfo f = new FileInfo(fName);
            directory = f.DirectoryName;
        }

        /// <summary>
        /// Nastav� adres�� pro export podle n�zvu souboru
        /// </summary>
        /// <param name="fName">N�zev souboru</param>
        public static void SetExportDirectoryFromFile(string fName) {
            FileInfo f = new FileInfo(fName);
            exportDirectory = f.DirectoryName;
        }

        /// <summary>
        /// Nastav� p��ponu pro sekvenci obr�zk� podle n�zvu souboru
        /// </summary>
        /// <param name="fName">N�zev souboru</param>
        public static void SetSeqExtensionFromFile(string fName) {
            FileInfo f = new FileInfo(fName);
            seqExtension = f.Extension.Replace(".", "");
        }

        public static string FileFilterTxt { get { return Messages.FilterTxt; } }
        public static string FileFilterGif { get { return Messages.FilterGif; } }
        public static string FileFilterGcm { get { return Messages.FilterGCM; } }
        public static string FileFilterPicture { get { return Messages.FilterPicture; } }
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
        /// Statick� konstruktor - provede na�ten� z�kladn�ch parametr� z registru
        /// </summary>
        static WinMain() {
            object dir = GetRegistryValue(registryKeyDirectory);
            if(dir is string && dir as string != string.Empty)
                directory = dir as string;
            else
                directory = defaultDirectory;

            object exportDir = GetRegistryValue(registryKeyExportDirectory);
            if(exportDir is string && exportDir as string != string.Empty)
                exportDirectory = exportDir as string;
            else
                exportDirectory = directory;

            object seqExt = GetRegistryValue(registryKeySeqExtension);
            if(seqExt is string && seqExt as string != string.Empty)
                seqExtension = seqExt as string;
            else
                seqExt = FileExtPicture;

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
        /// Provede ulo�en� nastaven�
        /// </summary>
        public static void SaveSettings(){
            SetRegistryValue(registryKeyDirectory, directory);
            SetRegistryValue(registryKeyExportDirectory, exportDirectory);
            SetRegistryValue(registryKeySeqExtension, seqExtension == null ? defaultFileExtPicture : seqExtension);
            SetRegistryValue(registryKeyFncDirectory, Context.FncDirectory);
            SetRegistryValue(registryKeyGlobalContextDirectory, Context.GlobalContextDirectory);
            SetRegistryValue(registryKeyPlaySounds, playSounds);
        }

        /// <summary>
        /// Vytvo�� n�zev aplikace pro registr Windows
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
        /// Je program zaregistrov�n?
        /// </summary>
        public static bool IsRegistered {
            get {
                string path = Application.ExecutablePath;
                string keyName = RegistryEntryName;

                // Existuje kl�� v registrech?
                RegistryKey rk = Registry.ClassesRoot.OpenSubKey(string.Format(commandEntryName, keyName));
                if(rk == null)
                    return false;

                // Existuje z�znam s cestou?
                string commandEntry = rk.GetValue(string.Empty) as string;
                if(commandEntry == null || commandEntry == string.Empty)
                    return false;

                if(string.Format(commandEntryFormat, path) != commandEntry)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Vyzvedne z registr� hodnotu kl��e
        /// </summary>
        /// <param name="keyName">N�zev kl��e</param>
        /// <param name="delete">True, pokud m� b�t kl�� po vyzvednut� vymaz�n</param>
        public static object GetRegistryValue(string keyName, bool delete) {
            object result = Application.UserAppDataRegistry.GetValue(keyName);
            if(delete)
                Application.UserAppDataRegistry.DeleteValue(keyName, false);
            return result;
        }

        /// <summary>
        /// Vyzvedne z registr� hodnotu kl��e
        /// </summary>
        /// <param name="keyName">N�zev kl��e</param>
        public static object GetRegistryValue(string keyName) {
            return GetRegistryValue(keyName, false);
        }

        /// <summary>
        /// Zap�e hodnotu kl��e do registr�
        /// </summary>
        /// <param name="keyName">N�zev kl��e</param>
        /// <param name="value">Hodnota kl��e</param>
        public static void SetRegistryValue(string keyName, object value) {
            Application.UserAppDataRegistry.SetValue(keyName, value);
        }

        /// <summary>
        /// Zaregistruje p��ponu programu do Windows
        /// </summary>
        /// <param name="state">True pro zaregistrov�n�, False pro odregistrov�n�</param>
        public static void Register(bool state) {
            string path = Application.ExecutablePath;
            string keyName = RegistryEntryName;

            if(state) {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("software", true).OpenSubKey("classes", true);
                key.CreateSubKey('.' + WinMain.FileExtGcm).SetValue(string.Empty, keyName);
                key.CreateSubKey(keyName).SetValue(string.Empty, Messages.ProgramDescription);
                key.CreateSubKey(string.Format("{0}\\DefaultIcon", keyName)).SetValue(string.Empty, string.Format("{0},0", path));
                key.CreateSubKey(string.Format(commandEntryName, keyName)).SetValue(string.Empty, string.Format(commandEntryFormat, path));
            }
            else {
                try {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey("software").OpenSubKey("classes");
                    key.DeleteSubKeyTree(keyName);
                    key.DeleteSubKeyTree('.' + WinMain.FileExtGcm);
                }
                catch(Exception) {
                }
            }
        }

        private const string defaultFileExtGcm = "gcm";
        private const string defaultFileExtTxt = "txt";
        private const string defaultFileExtGif = "gif";
        private const string defaultFileExtWmf = "wmf";
        private const string defaultFileExtPicture = "png";
        private const string defaultDirectory = "c:\\gcm";

        private const string commandEntryName = "{0}\\shell\\open\\command";
        private const string commandEntryFormat = "\"{0}\" \"%1\"";

        private const string registryKeyDirectory = "Directory";
        private const string registryKeyExportDirectory = "ExportDirectory";
        private const string registryKeySeqExtension = "SeqExtension";
        private const string registryKeyFncDirectory = "FncDirectory";
        private const string registryKeyGlobalContextDirectory = "GlobalContextDirectory";
        private const string registryKeyPlaySounds = "PlaySounds";
    }
}
