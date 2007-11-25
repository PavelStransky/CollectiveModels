namespace PavelStransky.Forms {
    partial class GraphForm {
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
        protected void InitializeComponent() {
            this.progress = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.lblRestProcess = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progress
            // 
            this.progress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progress.Location = new System.Drawing.Point(139, 262);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(223, 16);
            this.progress.Step = 1;
            this.progress.TabIndex = 0;
            this.progress.UseWaitCursor = true;
            this.progress.Value = 7;
            // 
            // lblProgress
            // 
            this.lblProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(2, 261);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(48, 13);
            this.lblProgress.TabIndex = 1;
            this.lblProgress.Text = "Progress";
            // 
            // lblRestProcess
            // 
            this.lblRestProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRestProcess.AutoSize = true;
            this.lblRestProcess.Location = new System.Drawing.Point(368, 262);
            this.lblRestProcess.Name = "lblRestProcess";
            this.lblRestProcess.Size = new System.Drawing.Size(13, 13);
            this.lblRestProcess.TabIndex = 2;
            this.lblRestProcess.Text = "0";
            this.lblRestProcess.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // GraphForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(392, 279);
            this.Controls.Add(this.lblRestProcess);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.progress);
            this.Name = "GraphForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "GraphForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label lblRestProcess;
    }
}
