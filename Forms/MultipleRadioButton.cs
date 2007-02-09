using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PavelStransky.Forms {
    /// <summary>
    /// Obsahuje seznam radioButton�
    /// </summary>
    public partial class MultipleRadioButton : UserControl {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public MultipleRadioButton() {
            this.InitializeComponent();
        }

#region Obsluha vlastn�ch ud�lost�
        public delegate void MultipleRadioButtonEventHandler(object sender, MultipleRadioButtonEventArgs e);
        public event MultipleRadioButtonEventHandler RBClick;

        protected void OnRBClick(MultipleRadioButtonEventArgs e) {
            if(this.RBClick != null)
                this.RBClick(this, e);
        }
#endregion

        /// <summary>
        /// P�id� RadioButton do controlu
        /// </summary>
        /// <param name="name">N�zev nov�ho RadioButtonu</param>
        /// <param name="text">Text RadioButtonu</param>
        public void Add(string name, string text) {
            RadioButton rb = new RadioButton();

            this.SuspendLayout();
            rb.Anchor = this.rbNew.Anchor;
            rb.AutoSize = this.rbNew.AutoSize;
            rb.BackColor = this.rbNew.BackColor;
            rb.Checked = true;
            rb.Location = this.rbNew.Location;
            rb.Name = name;
            rb.Size = this.rbNew.Size;
            rb.TabStop = true;
            rb.Text = text;
            rb.UseVisualStyleBackColor = this.rbNew.UseVisualStyleBackColor;

            rb.Click += new EventHandler(radioButton_Click);

            foreach(RadioButton r in this.Controls)
                r.Checked = false;

            this.rbNew.Left += 70;

            this.Controls.Add(rb);
            this.ResumeLayout();
        }

        /// <summary>
        /// P�i kliknut� na RadioButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_Click(object sender, EventArgs e) {
            this.OnRBClick(new MultipleRadioButtonEventArgs(this.GetCheckedName()));
        }

        /// <summary>
        /// Vyma�e RadioControl s dan�m n�zvem z controlu
        /// </summary>
        /// <param name="name">N�zev RadioButtonu</param>
        public void Remove(string name) {
            bool found = false;
            RadioButton rbdel = null;
            Point rblastloc = new Point();

            foreach(RadioButton rb in this.Controls) {
                if(found) {
                    Point rbnewloc = rb.Location;
                    rb.Location = rblastloc;
                    rblastloc = rbnewloc;
                }

                if(rb.Name == name && !found) {
                    found = true;
                    rblastloc = rb.Location;
                    rbdel = rb;
                }
            }

            this.rbNew.Location = rblastloc;

            // Vyma�u odstran�n� RadioButton z control�
            if(rbdel != null) {
                if(rbdel.Checked)
                    this.rbNew.Checked = true;
                this.Controls.Remove(rbdel);
                rbdel.Dispose();
            }
        }

        /// <summary>
        /// Je za�krtnuto tla��tko Nov�
        /// </summary>
        public bool IsNewChecked { get { return this.rbNew.Checked; } }

        /// <summary>
        /// Vr�t� RadioButton, kter� je za�krtnut�
        /// </summary>
        public string GetCheckedName() {
            foreach(RadioButton rb in this.Controls)
                if(rb.Checked && rb != this.rbNew)
                    return rb.Name;
            return null;
        }

        /// <summary>
        /// Nastav� barvu pozad� RadioButtonu
        /// </summary>
        /// <param name="name">Jm�no RadioButtonu</param>
        /// <param name="color">Nov� barva</param>
        public void SetBackColor(string name, Color color) {
            RadioButton rb = this.GetRBFromName(name);
            if(rb != null)
                rb.BackColor = color;
        }

        /// <summary>
        /// Nastav� defaultn� barvu pozad� RadioButtonu
        /// </summary>
        /// <param name="name">Jm�no RadioButtonu</param>
        /// <param name="color">Nov� barva</param>
        public void SetDefaultBackColor(string name) {
            RadioButton rb = this.GetRBFromName(name);
            if(rb != null)
                rb.BackColor = this.rbNew.BackColor;
        }

        /// <summary>
        /// Vr�t� RadioButton se zadan�m n�zvem
        /// </summary>
        /// <param name="name">N�zev</param>
        private RadioButton GetRBFromName(string name) {
            foreach(RadioButton rb in this.Controls)
                if(rb.Name == name)
                    return rb;
            return null;
        }
    }
}
