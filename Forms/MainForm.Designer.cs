using System.ComponentModel;
using System.Windows.Forms;

namespace PavelStransky.Forms {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if(disposing) {
                if(components != null) {
                    components.Dispose();
                }
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
            this.mnMenu = new System.Windows.Forms.MainMenu(this.components);
            this.mnFile = new System.Windows.Forms.MenuItem();
            this.mnFileNew = new System.Windows.Forms.MenuItem();
            this.mnFileOpen = new System.Windows.Forms.MenuItem();
            this.mnFileSave = new System.Windows.Forms.MenuItem();
            this.mnFileSaveAs = new System.Windows.Forms.MenuItem();
            this.mnFileSeparator = new System.Windows.Forms.MenuItem();
            this.mnExit = new System.Windows.Forms.MenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.lblCommand = new System.Windows.Forms.Label();
            this.txtCommand = new PavelStransky.Forms.CommandTextBox();
            this.SuspendLayout();
            // 
            // mnMenu
            // 
            this.mnMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnFile});
            // 
            // mnFile
            // 
            this.mnFile.Index = 0;
            this.mnFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnFileNew,
            this.mnFileOpen,
            this.mnFileSave,
            this.mnFileSaveAs,
            this.mnFileSeparator,
            this.mnExit});
            this.mnFile.Text = "&Soubor";
            // 
            // mnFileNew
            // 
            this.mnFileNew.Index = 0;
            this.mnFileNew.Text = "&Nové";
            this.mnFileNew.Click += new System.EventHandler(this.mnFileNew_Click);
            // 
            // mnFileOpen
            // 
            this.mnFileOpen.Index = 1;
            this.mnFileOpen.Text = "&Otevøít pøíkazy...";
            this.mnFileOpen.Click += new System.EventHandler(this.mnFileOpen_Click);
            // 
            // mnFileSave
            // 
            this.mnFileSave.Index = 2;
            this.mnFileSave.Text = "&Uložit pøíkazy";
            this.mnFileSave.Click += new System.EventHandler(this.mnFileSave_Click);
            // 
            // mnFileSaveAs
            // 
            this.mnFileSaveAs.Index = 3;
            this.mnFileSaveAs.Text = "&Uložit pøíkazy jako...";
            this.mnFileSaveAs.Click += new System.EventHandler(this.mnFileSaveAs_Click);
            // 
            // mnFileSeparator
            // 
            this.mnFileSeparator.Index = 4;
            this.mnFileSeparator.Text = "-";
            // 
            // mnExit
            // 
            this.mnExit.Index = 5;
            this.mnExit.Text = "&Konec";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_FileOk);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog_FileOk);
            // 
            // lblCommand
            // 
            this.lblCommand.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblCommand.Location = new System.Drawing.Point(4, 1);
            this.lblCommand.Name = "lblCommand";
            this.lblCommand.Size = new System.Drawing.Size(128, 16);
            this.lblCommand.TabIndex = 7;
            this.lblCommand.Text = "Pøíkazy:";
            // 
            // txtCommand
            // 
            this.txtCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCommand.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtCommand.Location = new System.Drawing.Point(7, 20);
            this.txtCommand.Multiline = true;
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtCommand.Size = new System.Drawing.Size(548, 435);
            this.txtCommand.TabIndex = 6;
            this.txtCommand.WordWrap = false;
            this.txtCommand.ExecuteCommand += new PavelStransky.Forms.CommandTextBox.ExecuteCommandEventHandler(this.txtCommand_ExecuteCommand);
            this.txtCommand.TextChanged += new System.EventHandler(this.txtCommand_TextChanged);
            this.txtCommand.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(561, 467);
            this.Controls.Add(this.lblCommand);
            this.Controls.Add(this.txtCommand);
            this.Menu = this.mnMenu;
            this.Name = "MainForm";
            this.Text = "Prográmek";
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.MainMenu mnMenu;
        private System.Windows.Forms.MenuItem mnFile;
        private System.Windows.Forms.MenuItem mnFileOpen;
        private System.Windows.Forms.MenuItem mnFileSave;
        private System.Windows.Forms.MenuItem mnFileSaveAs;
        private System.Windows.Forms.MenuItem mnExit;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.MenuItem mnFileSeparator;
        private System.Windows.Forms.MenuItem mnFileNew;
        private Label lblCommand;
        private CommandTextBox txtCommand;
    }
}