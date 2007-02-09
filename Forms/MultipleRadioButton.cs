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
    /// Obsahuje seznam radioButtonù
    /// </summary>
    public partial class MultipleRadioButton : UserControl {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public MultipleRadioButton() {
            this.InitializeComponent();
        }

#region Obsluha vlastních událostí
        public delegate void MultipleRadioButtonEventHandler(object sender, MultipleRadioButtonEventArgs e);
        public event MultipleRadioButtonEventHandler RBClick;

        protected void OnRBClick(MultipleRadioButtonEventArgs e) {
            if(this.RBClick != null)
                this.RBClick(this, e);
        }
#endregion

        /// <summary>
        /// Pøidá RadioButton do controlu
        /// </summary>
        /// <param name="name">Název nového RadioButtonu</param>
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
        /// Pøi kliknutí na RadioButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_Click(object sender, EventArgs e) {
            this.OnRBClick(new MultipleRadioButtonEventArgs(this.GetCheckedName()));
        }

        /// <summary>
        /// Vymaže RadioControl s daným názvem z controlu
        /// </summary>
        /// <param name="name">Název RadioButtonu</param>
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

            // Vymažu odstranìný RadioButton z controlù
            if(rbdel != null) {
                if(rbdel.Checked)
                    this.rbNew.Checked = true;
                this.Controls.Remove(rbdel);
                rbdel.Dispose();
            }
        }

        /// <summary>
        /// Je zaškrtnuto tlaèítko Nový
        /// </summary>
        public bool IsNewChecked { get { return this.rbNew.Checked; } }

        /// <summary>
        /// Vrátí RadioButton, který je zaškrtnutý
        /// </summary>
        public string GetCheckedName() {
            foreach(RadioButton rb in this.Controls)
                if(rb.Checked && rb != this.rbNew)
                    return rb.Name;
            return null;
        }

        /// <summary>
        /// Nastaví barvu pozadí RadioButtonu
        /// </summary>
        /// <param name="name">Jméno RadioButtonu</param>
        /// <param name="color">Nová barva</param>
        public void SetBackColor(string name, Color color) {
            RadioButton rb = this.GetRBFromName(name);
            if(rb != null)
                rb.BackColor = color;
        }

        /// <summary>
        /// Nastaví defaultní barvu pozadí RadioButtonu
        /// </summary>
        /// <param name="name">Jméno RadioButtonu</param>
        /// <param name="color">Nová barva</param>
        public void SetDefaultBackColor(string name) {
            RadioButton rb = this.GetRBFromName(name);
            if(rb != null)
                rb.BackColor = this.rbNew.BackColor;
        }

        /// <summary>
        /// Vrátí RadioButton se zadaným názvem
        /// </summary>
        /// <param name="name">Název</param>
        private RadioButton GetRBFromName(string name) {
            foreach(RadioButton rb in this.Controls)
                if(rb.Name == name)
                    return rb;
            return null;
        }
    }
}
