namespace PavelStransky.Forms {
    partial class CurveToExport {
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
            this.lblCaption = new System.Windows.Forms.Label();
            this.cbCurves = new System.Windows.Forms.ComboBox();
            this.btOK = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblCaption
            // 
            this.lblCaption.AutoSize = true;
            this.lblCaption.Location = new System.Drawing.Point(12, 9);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(119, 13);
            this.lblCaption.TabIndex = 0;
            this.lblCaption.Text = "Group - curve to export:";
            // 
            // cbCurves
            // 
            this.cbCurves.FormattingEnabled = true;
            this.cbCurves.Location = new System.Drawing.Point(137, 6);
            this.cbCurves.Name = "cbCurves";
            this.cbCurves.Size = new System.Drawing.Size(254, 21);
            this.cbCurves.TabIndex = 1;
            // 
            // btOK
            // 
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Location = new System.Drawing.Point(234, 33);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(78, 26);
            this.btOK.TabIndex = 2;
            this.btOK.Text = "&OK";
            this.btOK.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(318, 33);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(73, 26);
            this.button2.TabIndex = 3;
            this.button2.Text = "&Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // CurveToExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 69);
            this.ControlBox = false;
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.cbCurves);
            this.Controls.Add(this.lblCaption);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CurveToExport";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Data to export";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.ComboBox cbCurves;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button button2;
    }
}