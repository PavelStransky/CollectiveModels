namespace PavelStransky.Forms {
    partial class MultipleRadioButton {
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.rbNew = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // rbNew
            // 
            this.rbNew.AutoSize = true;
            this.rbNew.BackColor = System.Drawing.SystemColors.Control;
            this.rbNew.Checked = true;
            this.rbNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.rbNew.Location = new System.Drawing.Point(0, 0);
            this.rbNew.Name = "rbNew";
            this.rbNew.Size = new System.Drawing.Size(55, 17);
            this.rbNew.TabIndex = 7;
            this.rbNew.TabStop = true;
            this.rbNew.Text = "Nové";
            this.rbNew.UseVisualStyleBackColor = false;

            this.Controls.Add(this.rbNew);
            this.ResumeLayout();
        }

        #endregion

        private System.Windows.Forms.RadioButton rbNew;
    }
}
