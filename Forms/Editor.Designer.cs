using System.ComponentModel;
using System.Windows.Forms;

namespace PavelStransky.Forms {
    partial class Editor {
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
            this.txtCommand = new PavelStransky.Forms.CommandTextBox();
            this.SuspendLayout();
            // 
            // txtCommand
            // 
            this.txtCommand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCommand.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtCommand.Location = new System.Drawing.Point(0, 0);
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.Size = new System.Drawing.Size(561, 341);
            this.txtCommand.TabIndex = 6;
            this.txtCommand.Text = "";
            this.txtCommand.WordWrap = false;
            this.txtCommand.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Editor_KeyDown);
            this.txtCommand.ExecuteCommand += new PavelStransky.Forms.CommandTextBox.ExecuteCommandEventHandler(this.txtCommand_ExecuteCommand);
            this.txtCommand.TextChanged += new System.EventHandler(this.txtCommand_TextChanged);
            // 
            // Editor
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(561, 341);
            this.Controls.Add(this.txtCommand);
            this.Name = "Editor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Prográmek";
            this.Activated += new System.EventHandler(this.Editor_Activated);
            this.ResumeLayout(false);

        }
        #endregion

        private CommandTextBox txtCommand;
    }
}