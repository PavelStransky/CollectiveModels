namespace PavelStransky.Forms {
    partial class SingleGraphForm {
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
            this.components = new System.ComponentModel.Container();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.lblRestProcess = new System.Windows.Forms.Label();
            this.graphicsBox = new PavelStransky.Forms.GraphicsBox();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmnSaveOneText = new System.Windows.Forms.ToolStripMenuItem();
            this.cmnSaveOneAnim = new System.Windows.Forms.ToolStripMenuItem();
            this.cmnSaveOneSeq = new System.Windows.Forms.ToolStripMenuItem();
            this.cmnSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmnSaveAllText = new System.Windows.Forms.ToolStripMenuItem();
            this.cmnSaveAllAnim = new System.Windows.Forms.ToolStripMenuItem();
            this.cmnSaveAllSeq = new System.Windows.Forms.ToolStripMenuItem();
            this.cmnSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cmnStopAnimation = new System.Windows.Forms.ToolStripMenuItem();
            this.cmnStartAnimation = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.sfdText = new System.Windows.Forms.SaveFileDialog();
            this.sfdAnim = new System.Windows.Forms.SaveFileDialog();
            this.sfdSeq = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.graphicsBox)).BeginInit();
            this.contextMenu.SuspendLayout();
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
            // graphicsBox
            // 
            this.graphicsBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.graphicsBox.BackColor = System.Drawing.Color.White;
            this.graphicsBox.ContextMenuStrip = this.contextMenu;
            this.graphicsBox.Location = new System.Drawing.Point(5, 2);
            this.graphicsBox.Name = "graphicsBox";
            this.graphicsBox.Size = new System.Drawing.Size(385, 257);
            this.graphicsBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.graphicsBox.TabIndex = 3;
            this.graphicsBox.TabStop = false;
            this.graphicsBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.graphicsBox_MouseMove);
            this.graphicsBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.graphicsBox_MouseDown);
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmnSaveOneText,
            this.cmnSaveOneAnim,
            this.cmnSaveOneSeq,
            this.cmnSeparator1,
            this.cmnSaveAllText,
            this.cmnSaveAllAnim,
            this.cmnSaveAllSeq,
            this.cmnSeparator2,
            this.cmnStopAnimation,
            this.cmnStartAnimation});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(250, 214);
            this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
            // 
            // cmnSaveOneText
            // 
            this.cmnSaveOneText.Name = "cmnSaveOneText";
            this.cmnSaveOneText.Size = new System.Drawing.Size(249, 22);
            this.cmnSaveOneText.Text = "&Uložit data jako text...";
            this.cmnSaveOneText.Click += new System.EventHandler(this.cmnSaveOneText_Click);
            // 
            // cmnSaveOneAnim
            // 
            this.cmnSaveOneAnim.Name = "cmnSaveOneAnim";
            this.cmnSaveOneAnim.Size = new System.Drawing.Size(249, 22);
            this.cmnSaveOneAnim.Text = "Uložit jako &GIF (animovaný)...";
            this.cmnSaveOneAnim.Click += new System.EventHandler(this.cmnSaveOneAnim_Click);
            // 
            // cmnSaveOneSeq
            // 
            this.cmnSaveOneSeq.Name = "cmnSaveOneSeq";
            this.cmnSaveOneSeq.Size = new System.Drawing.Size(249, 22);
            this.cmnSaveOneSeq.Text = "Uložit jako &obrázek (sekvenènì)...";
            this.cmnSaveOneSeq.Click += new System.EventHandler(this.cmnSaveOneSeq_Click);
            // 
            // cmnSeparator1
            // 
            this.cmnSeparator1.Name = "cmnSeparator1";
            this.cmnSeparator1.Size = new System.Drawing.Size(246, 6);
            // 
            // cmnSaveAllText
            // 
            this.cmnSaveAllText.Name = "cmnSaveAllText";
            this.cmnSaveAllText.Size = new System.Drawing.Size(249, 22);
            this.cmnSaveAllText.Text = "Vše jako &text...";
            this.cmnSaveAllText.Click += new System.EventHandler(this.cmnSaveAllText_Click);
            // 
            // cmnSaveAllAnim
            // 
            this.cmnSaveAllAnim.Name = "cmnSaveAllAnim";
            this.cmnSaveAllAnim.Size = new System.Drawing.Size(249, 22);
            this.cmnSaveAllAnim.Text = "Vše jako GIF (&animovanì)...";
            this.cmnSaveAllAnim.Click += new System.EventHandler(this.cmnSaveAllAnim_Click);
            // 
            // cmnSaveAllSeq
            // 
            this.cmnSaveAllSeq.Name = "cmnSaveAllSeq";
            this.cmnSaveAllSeq.Size = new System.Drawing.Size(249, 22);
            this.cmnSaveAllSeq.Text = "Vše jako obrázek (&sekvenènì)...";
            this.cmnSaveAllSeq.Click += new System.EventHandler(this.cmnSaveAllSeq_Click);
            // 
            // cmnSeparator2
            // 
            this.cmnSeparator2.Name = "cmnSeparator2";
            this.cmnSeparator2.Size = new System.Drawing.Size(246, 6);
            // 
            // cmnStopAnimation
            // 
            this.cmnStopAnimation.Name = "cmnStopAnimation";
            this.cmnStopAnimation.Size = new System.Drawing.Size(249, 22);
            this.cmnStopAnimation.Text = "Pozastavit animaci";
            this.cmnStopAnimation.Click += new System.EventHandler(this.cmnStopAnimation_Click);
            // 
            // cmnStartAnimation
            // 
            this.cmnStartAnimation.Name = "cmnStartAnimation";
            this.cmnStartAnimation.Size = new System.Drawing.Size(249, 22);
            this.cmnStartAnimation.Text = "Spustit animaci";
            this.cmnStartAnimation.Click += new System.EventHandler(this.cmnStartAnimation_Click);
            // 
            // sfdText
            // 
            this.sfdText.FileOk += new System.ComponentModel.CancelEventHandler(this.sfdText_FileOk);
            // 
            // sfdAnim
            // 
            this.sfdAnim.FileOk += new System.ComponentModel.CancelEventHandler(this.sfdAnim_FileOk);
            // 
            // sfdSeq
            // 
            this.sfdSeq.FileOk += new System.ComponentModel.CancelEventHandler(this.sfdSeq_FileOk);
            // 
            // SingleGraphForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(392, 279);
            this.Controls.Add(this.graphicsBox);
            this.Controls.Add(this.lblRestProcess);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.progress);
            this.Name = "SingleGraphForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "GraphForm";
            ((System.ComponentModel.ISupportInitialize)(this.graphicsBox)).EndInit();
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label lblRestProcess;
        private GraphicsBox graphicsBox;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem cmnSaveOneText;
        private System.Windows.Forms.ToolStripMenuItem cmnSaveOneAnim;
        private System.Windows.Forms.ToolStripMenuItem cmnSaveOneSeq;
        private System.Windows.Forms.ToolStripSeparator cmnSeparator1;
        private System.Windows.Forms.ToolStripMenuItem cmnSaveAllText;
        private System.Windows.Forms.ToolStripMenuItem cmnSaveAllAnim;
        private System.Windows.Forms.ToolStripMenuItem cmnSaveAllSeq;
        private System.Windows.Forms.SaveFileDialog sfdText;
        private System.Windows.Forms.SaveFileDialog sfdAnim;
        private System.Windows.Forms.SaveFileDialog sfdSeq;
        private System.Windows.Forms.ToolStripSeparator cmnSeparator2;
        private System.Windows.Forms.ToolStripMenuItem cmnStopAnimation;
        private System.Windows.Forms.ToolStripMenuItem cmnStartAnimation;
    }
}
