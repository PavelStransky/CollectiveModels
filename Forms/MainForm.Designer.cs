namespace PavelStransky.Forms {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mnMenu = new System.Windows.Forms.MainMenu(this.components);
            this.mnFile = new System.Windows.Forms.MenuItem();
            this.mnFileNew = new System.Windows.Forms.MenuItem();
            this.mnFileOpen = new System.Windows.Forms.MenuItem();
            this.mnFileClose = new System.Windows.Forms.MenuItem();
            this.mnFileSeparator1 = new System.Windows.Forms.MenuItem();
            this.mnFileSave = new System.Windows.Forms.MenuItem();
            this.mnFileSaveAs = new System.Windows.Forms.MenuItem();
            this.mnFileSeparator2 = new System.Windows.Forms.MenuItem();
            this.mnExit = new System.Windows.Forms.MenuItem();
            this.mnSettings = new System.Windows.Forms.MenuItem();
            this.mnSetttingsRegistry = new System.Windows.Forms.MenuItem();
            this.mnSettingsPlaySounds = new System.Windows.Forms.MenuItem();
            this.mnWindow = new System.Windows.Forms.MenuItem();
            this.MnCascade = new System.Windows.Forms.MenuItem();
            this.mnTileHorizontal = new System.Windows.Forms.MenuItem();
            this.mnTileVertical = new System.Windows.Forms.MenuItem();
            this.mnArrangeIcons = new System.Windows.Forms.MenuItem();
            this.mnHideToTray = new System.Windows.Forms.MenuItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmTrayActualInformation = new System.Windows.Forms.ToolStripMenuItem();
            this.cmTrayFinishInformation = new System.Windows.Forms.ToolStripMenuItem();
            this.cmTrayClearWriter = new System.Windows.Forms.ToolStripMenuItem();
            this.cmTray.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnMenu
            // 
            this.mnMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnFile,
            this.mnSettings,
            this.mnWindow,
            this.mnHideToTray});
            // 
            // mnFile
            // 
            this.mnFile.Index = 0;
            this.mnFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnFileNew,
            this.mnFileOpen,
            this.mnFileClose,
            this.mnFileSeparator1,
            this.mnFileSave,
            this.mnFileSaveAs,
            this.mnFileSeparator2,
            this.mnExit});
            this.mnFile.Text = "&Soubor";
            // 
            // mnFileNew
            // 
            this.mnFileNew.Index = 0;
            this.mnFileNew.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
            this.mnFileNew.Text = "&Nové";
            this.mnFileNew.Click += new System.EventHandler(this.mnFileNew_Click);
            // 
            // mnFileOpen
            // 
            this.mnFileOpen.Index = 1;
            this.mnFileOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.mnFileOpen.Text = "&Otevøít...";
            this.mnFileOpen.Click += new System.EventHandler(this.mnFileOpen_Click);
            // 
            // mnFileClose
            // 
            this.mnFileClose.Index = 2;
            this.mnFileClose.Text = "&Zavøít";
            this.mnFileClose.Click += new System.EventHandler(this.mnFileClose_Click);
            // 
            // mnFileSeparator1
            // 
            this.mnFileSeparator1.Index = 3;
            this.mnFileSeparator1.Text = "-";
            // 
            // mnFileSave
            // 
            this.mnFileSave.Index = 4;
            this.mnFileSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this.mnFileSave.Text = "&Uložit";
            this.mnFileSave.Click += new System.EventHandler(this.mnFileSave_Click);
            // 
            // mnFileSaveAs
            // 
            this.mnFileSaveAs.Index = 5;
            this.mnFileSaveAs.Text = "&Uložit jako...";
            this.mnFileSaveAs.Click += new System.EventHandler(this.mnFileSaveAs_Click);
            // 
            // mnFileSeparator2
            // 
            this.mnFileSeparator2.Index = 6;
            this.mnFileSeparator2.Text = "-";
            // 
            // mnExit
            // 
            this.mnExit.Index = 7;
            this.mnExit.Text = "&Konec";
            this.mnExit.Click += new System.EventHandler(this.mnExit_Click);
            // 
            // mnSettings
            // 
            this.mnSettings.Index = 1;
            this.mnSettings.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnSetttingsRegistry,
            this.mnSettingsPlaySounds});
            this.mnSettings.Text = "&Nastavení";
            // 
            // mnSetttingsRegistry
            // 
            this.mnSetttingsRegistry.Index = 0;
            this.mnSetttingsRegistry.Text = "&Registrace ve Windows";
            this.mnSetttingsRegistry.Click += new System.EventHandler(this.mnSetttingsRegistry_Click);
            // 
            // mnSettingsPlaySounds
            // 
            this.mnSettingsPlaySounds.Index = 1;
            this.mnSettingsPlaySounds.Text = "&Pøehrávat zvuky";
            this.mnSettingsPlaySounds.Click += new System.EventHandler(this.mnSettingsPlaySounds_Click);
            // 
            // mnWindow
            // 
            this.mnWindow.Index = 2;
            this.mnWindow.MdiList = true;
            this.mnWindow.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.MnCascade,
            this.mnTileHorizontal,
            this.mnTileVertical,
            this.mnArrangeIcons});
            this.mnWindow.Text = "&Okno";
            // 
            // MnCascade
            // 
            this.MnCascade.Index = 0;
            this.MnCascade.Text = "&Uspoøádat";
            this.MnCascade.Click += new System.EventHandler(this.MnCascade_Click);
            // 
            // mnTileHorizontal
            // 
            this.mnTileHorizontal.Index = 1;
            this.mnTileHorizontal.Text = "&Vodorovnì";
            this.mnTileHorizontal.Click += new System.EventHandler(this.mnTileHorizontal_Click);
            // 
            // mnTileVertical
            // 
            this.mnTileVertical.Index = 2;
            this.mnTileVertical.Text = "&Svisle";
            this.mnTileVertical.Click += new System.EventHandler(this.mnTileVertical_Click);
            // 
            // mnArrangeIcons
            // 
            this.mnArrangeIcons.Index = 3;
            this.mnArrangeIcons.Text = "Seøadit &ikony";
            this.mnArrangeIcons.Click += new System.EventHandler(this.mnArrangeIcons_Click);
            // 
            // mnHideToTray
            // 
            this.mnHideToTray.Index = 3;
            this.mnHideToTray.Text = "S&krýt";
            this.mnHideToTray.Click += new System.EventHandler(this.mnHideToTray_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_FileOk);
            // 
            // trayIcon
            // 
            this.trayIcon.ContextMenuStrip = this.cmTray;
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "Collective Models";
            this.trayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseClick);
            // 
            // cmTray
            // 
            this.cmTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmTrayActualInformation,
            this.cmTrayFinishInformation,
            this.cmTrayClearWriter});
            this.cmTray.Name = "cmTray";
            this.cmTray.ShowCheckMargin = true;
            this.cmTray.ShowImageMargin = false;
            this.cmTray.Size = new System.Drawing.Size(235, 70);
            // 
            // cmTrayActualInformation
            // 
            this.cmTrayActualInformation.Name = "cmTrayActualInformation";
            this.cmTrayActualInformation.Size = new System.Drawing.Size(234, 22);
            this.cmTrayActualInformation.Text = "&Prùbìžné informace";
            this.cmTrayActualInformation.Click += new System.EventHandler(this.cmTrayActualInformation_Click);
            // 
            // cmTrayFinishInformation
            // 
            this.cmTrayFinishInformation.Name = "cmTrayFinishInformation";
            this.cmTrayFinishInformation.Size = new System.Drawing.Size(234, 22);
            this.cmTrayFinishInformation.Text = "Informace o &ukonèení výpoètu";
            this.cmTrayFinishInformation.Click += new System.EventHandler(this.cmTrayFinishInformation_Click);
            // 
            // cmTrayClearWriter
            // 
            this.cmTrayClearWriter.Name = "cmTrayClearWriter";
            this.cmTrayClearWriter.Size = new System.Drawing.Size(234, 22);
            this.cmTrayClearWriter.Text = "Informace pøi vymazání &writeru";
            this.cmTrayClearWriter.Click += new System.EventHandler(this.cmTrayClearWriter_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1016, 713);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.cmTray.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MainMenu mnMenu;
        private System.Windows.Forms.MenuItem mnFile;
        private System.Windows.Forms.MenuItem mnFileNew;
        private System.Windows.Forms.MenuItem mnFileOpen;
        private System.Windows.Forms.MenuItem mnFileSave;
        private System.Windows.Forms.MenuItem mnFileSaveAs;
        private System.Windows.Forms.MenuItem mnFileSeparator2;
        private System.Windows.Forms.MenuItem mnExit;
        private System.Windows.Forms.MenuItem mnWindow;
        private System.Windows.Forms.MenuItem MnCascade;
        private System.Windows.Forms.MenuItem mnTileHorizontal;
        private System.Windows.Forms.MenuItem mnTileVertical;
        private System.Windows.Forms.MenuItem mnArrangeIcons;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.MenuItem mnFileClose;
        private System.Windows.Forms.MenuItem mnFileSeparator1;
        private System.Windows.Forms.MenuItem mnSettings;
        private System.Windows.Forms.MenuItem mnSetttingsRegistry;
        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.MenuItem mnHideToTray;
        private System.Windows.Forms.MenuItem mnSettingsPlaySounds;
        private System.Windows.Forms.ContextMenuStrip cmTray;
        private System.Windows.Forms.ToolStripMenuItem cmTrayActualInformation;
        private System.Windows.Forms.ToolStripMenuItem cmTrayFinishInformation;
        private System.Windows.Forms.ToolStripMenuItem cmTrayClearWriter;
    }
}