using PavelStransky.Math;

namespace PavelStransky.Forms {
    public partial class ResultForm {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        protected void InitializeComponent() {
            this.lblCommand = new System.Windows.Forms.Label();
            this.txtCommand = new System.Windows.Forms.TextBox();
            this.lblResult = new System.Windows.Forms.Label();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.btInterrupt = new System.Windows.Forms.Button();
            this.lblComputing = new System.Windows.Forms.Label();
            this.btPause = new System.Windows.Forms.Button();
            this.btContinue = new System.Windows.Forms.Button();
            this.btRecalculate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblCommand
            // 
            this.lblCommand.AutoSize = true;
            this.lblCommand.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblCommand.Location = new System.Drawing.Point(4, 14);
            this.lblCommand.Name = "lblCommand";
            this.lblCommand.Size = new System.Drawing.Size(49, 13);
            this.lblCommand.TabIndex = 0;
            this.lblCommand.Text = "Pøíkaz:";
            // 
            // txtCommand
            // 
            this.txtCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCommand.Font = new System.Drawing.Font("Courier New", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtCommand.Location = new System.Drawing.Point(7, 30);
            this.txtCommand.Multiline = true;
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.ReadOnly = true;
            this.txtCommand.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtCommand.Size = new System.Drawing.Size(330, 130);
            this.txtCommand.TabIndex = 1;
            this.txtCommand.WordWrap = false;
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblResult.Location = new System.Drawing.Point(4, 163);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(62, 13);
            this.lblResult.TabIndex = 2;
            this.lblResult.Text = "Výsledek:";
            // 
            // txtResult
            // 
            this.txtResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResult.Font = new System.Drawing.Font("Courier New", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtResult.Location = new System.Drawing.Point(7, 179);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResult.Size = new System.Drawing.Size(330, 130);
            this.txtResult.TabIndex = 3;
            this.txtResult.WordWrap = false;
            // 
            // btInterrupt
            // 
            this.btInterrupt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btInterrupt.Location = new System.Drawing.Point(95, 315);
            this.btInterrupt.Name = "btInterrupt";
            this.btInterrupt.Size = new System.Drawing.Size(86, 29);
            this.btInterrupt.TabIndex = 4;
            this.btInterrupt.Text = "Pøerušit";
            this.btInterrupt.UseVisualStyleBackColor = true;
            this.btInterrupt.Visible = false;
            this.btInterrupt.Click += new System.EventHandler(this.btInterrupt_Click);
            // 
            // lblComputing
            // 
            this.lblComputing.AutoSize = true;
            this.lblComputing.Location = new System.Drawing.Point(4, 163);
            this.lblComputing.Name = "lblComputing";
            this.lblComputing.Size = new System.Drawing.Size(129, 13);
            this.lblComputing.TabIndex = 5;
            this.lblComputing.Text = "Aktuální poèítaný pøíkaz:";
            this.lblComputing.Visible = false;
            // 
            // btPause
            // 
            this.btPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btPause.Location = new System.Drawing.Point(7, 315);
            this.btPause.Name = "btPause";
            this.btPause.Size = new System.Drawing.Size(82, 29);
            this.btPause.TabIndex = 6;
            this.btPause.Text = "Pozastavit";
            this.btPause.UseVisualStyleBackColor = true;
            this.btPause.Visible = false;
            this.btPause.Click += new System.EventHandler(this.btPause_Click);
            // 
            // btContinue
            // 
            this.btContinue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btContinue.Location = new System.Drawing.Point(7, 316);
            this.btContinue.Name = "btContinue";
            this.btContinue.Size = new System.Drawing.Size(82, 28);
            this.btContinue.TabIndex = 7;
            this.btContinue.Text = "Pokraèovat";
            this.btContinue.UseVisualStyleBackColor = true;
            this.btContinue.Visible = false;
            this.btContinue.Click += new System.EventHandler(this.btContinue_Click);
            // 
            // btRecalculate
            // 
            this.btRecalculate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btRecalculate.Location = new System.Drawing.Point(244, 316);
            this.btRecalculate.Name = "btRecalculate";
            this.btRecalculate.Size = new System.Drawing.Size(93, 28);
            this.btRecalculate.TabIndex = 8;
            this.btRecalculate.Text = "Znovu spoèítat";
            this.btRecalculate.UseVisualStyleBackColor = true;
            this.btRecalculate.Visible = false;
            this.btRecalculate.Click += new System.EventHandler(this.btRecalculate_Click);
            // 
            // ResultForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(341, 347);
            this.Controls.Add(this.btRecalculate);
            this.Controls.Add(this.btContinue);
            this.Controls.Add(this.btPause);
            this.Controls.Add(this.lblComputing);
            this.Controls.Add(this.btInterrupt);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.txtCommand);
            this.Controls.Add(this.lblCommand);
            this.Name = "ResultForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Výpoèet...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCommand;
        private System.Windows.Forms.TextBox txtCommand;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Button btInterrupt;
        private System.Windows.Forms.Label lblComputing;
        private System.Windows.Forms.Button btPause;
        private System.Windows.Forms.Button btContinue;
        private System.Windows.Forms.Button btRecalculate;
    }
}