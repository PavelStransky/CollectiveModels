using System;
using System.Collections;
using System.Windows.Forms;
using System.Text;

namespace PavelStransky.Forms {
    /// <summary>
    /// Tøída, která spravuje poslední otevøené soubory
    /// </summary>
    public class LastOpenedFiles {
        private MenuItem menu;
        private MenuItem[] item;
        private ArrayList file;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="index">Index, od kterého se bude pøidávat (záporný - pøidává se z konce)</param>
        /// <param name="menu">Hlavní menu</param>
        public LastOpenedFiles(int index, MenuItem menu) {
            this.menu = menu;

            // Vytvoøíme položky menu
            this.item = new MenuItem[numLastOpenedFiles + 1];
            for(int i = 0; i <= numLastOpenedFiles; i++) {
                if(i != numLastOpenedFiles)
                    this.item[i] = new MenuItem(string.Empty, this.MenuClick);
                else
                    this.item[i] = new MenuItem("-"); // Separator

                if(index >= 0)
                    this.menu.MenuItems.Add(index, this.item[i]);
                else
                    this.menu.MenuItems.Add(this.menu.MenuItems.Count + index, this.item[i]);
            }

            // Naèteme seznam naposledy otevøených souborù
            this.file = new ArrayList();
            for(int i = 0; i < numLastOpenedFiles; i++) {
                object f = WinMain.GetRegistryValue(string.Format(registryKeyLastOpenedFile, i));
                if(f != null && f is string && (f as string != string.Empty))
                    this.file.Add(f);
            }

            // Aktualizujeme menu a viditelnost
            this.Refresh();
        }

        public event FileNameEventHandler Click;

        /// <summary>
        /// Pøi vybrání položky menu
        /// </summary>
        private void MenuClick(object sender, EventArgs e) {
            for(int i = 0; i < numLastOpenedFiles; i++)
                if(this.item[i] == (MenuItem)sender && this.Click != null) {
                    this.Click(this, new FileNameEventArgs(this.file[i] as string));
                    break;
                }
        }

        /// <summary>
        /// Aktualizuje menu
        /// </summary>
        private void Refresh() {
            int count = this.file.Count;
            for(int i = 0; i < numLastOpenedFiles; i++) {
                if(i < count) {
                    this.item[i].Text = string.Format(itemFormat, i + 1, this.file[i]);
                    this.item[i].Visible = true;
                }
                else
                    this.item[i].Visible = false;
            }

            // Separátor
            if(count == 0)
                this.item[numLastOpenedFiles].Visible = false;
            else
                this.item[numLastOpenedFiles].Visible = true;
        }

        /// <summary>
        /// Pøidá nový soubor do fronty
        /// </summary>
        public void AddFile(object sender, FileNameEventArgs e) {
            this.AddFile(e.FileName);
        }

        /// <summary>
        /// Pøidá nový soubor do fronty
        /// </summary>
        /// <param name="fileName">Jméno souboru</param>
        public void AddFile(string fileName) {
            if(this.file.Contains(fileName)) 
                this.file.RemoveAt(this.file.IndexOf(fileName));            

            this.file.Insert(0, fileName);
            if(this.file.Count > numLastOpenedFiles)
                this.file.RemoveAt(numLastOpenedFiles);

            this.Refresh();
        }

        /// <summary>
        /// Uloží zmìny do registrù
        /// </summary>
        public void Save() {
            int count = this.file.Count;
            for(int i = 0; i < numLastOpenedFiles; i++) {
                string keyName = string.Format(registryKeyLastOpenedFile, i);
                if(i < count)
                    WinMain.SetRegistryValue(keyName, this.file[i]);
                else
                    WinMain.GetRegistryValue(keyName, true);
            }
        }

        private const int numLastOpenedFiles = 4;
        private const string itemFormat = "&{0} - {1}";

        private const string registryKeyLastOpenedFile = "LastFile{0}";
    }
}
