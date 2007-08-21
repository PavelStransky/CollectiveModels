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
            this.components = new System.ComponentModel.Container();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.btStart = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.mrbResult = new PavelStransky.Forms.MultipleRadioButton();
            this.txtCommand = new PavelStransky.Forms.CommandTextBox();
            this.SuspendLayout();
            // 
            // btStart
            // 
            this.btStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btStart.Location = new System.Drawing.Point(2, 361);
            this.btStart.Name = "btStart";
            this.btStart.Size = new System.Drawing.Size(25, 22);
            this.btStart.TabIndex = 8;
            this.btStart.Text = ">";
            this.toolTip.SetToolTip(this.btStart, "Spustit výpoèet (F5)");
            this.btStart.UseVisualStyleBackColor = true;
            // 
            // mrbResult
            // 
            this.mrbResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mrbResult.Location = new System.Drawing.Point(32, 363);
            this.mrbResult.Name = "mrbResult";
            this.mrbResult.Size = new System.Drawing.Size(529, 18);
            this.mrbResult.TabIndex = 9;
            this.mrbResult.RBClick += new PavelStransky.Forms.MultipleRadioButton.MultipleRadioButtonEventHandler(this.mrbResult_RBClick);
            // 
            // txtCommand
            // 
            this.txtCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCommand.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtCommand.ForeColor = System.Drawing.Color.Blue;
            this.txtCommand.Location = new System.Drawing.Point(0, 0);
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.Size = new System.Drawing.Size(561, 360);
            this.txtCommand.TabIndex = 6;
            this.txtCommand.Text = "";
            this.txtCommand.WordWrap = false;
            this.txtCommand.MouseMove += new System.Windows.Forms.MouseEventHandler(this.txtCommand_MouseMove);
            this.txtCommand.ExecuteCommand += new PavelStransky.Forms.CommandTextBox.ExecuteCommandEventHandler(this.txtCommand_ExecuteCommand);
            this.txtCommand.TextChanged += new System.EventHandler(this.txtCommand_TextChanged);
            // 
            // Editor
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(561, 384);
            this.Controls.Add(this.mrbResult);
            this.Controls.Add(this.btStart);
            this.Controls.Add(this.txtCommand);
            this.Name = "Editor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Prográmek";
            this.Activated += new System.EventHandler(this.Editor_Activated);
            this.ResumeLayout(false);

        }
        #endregion

        private CommandTextBox txtCommand;
        private SaveFileDialog saveFileDialog;
        private Button btStart;
        private ToolTip toolTip;
        private MultipleRadioButton mrbResult;
    }
}