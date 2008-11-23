using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PavelStransky.Forms {
    public partial class CurveToExport: Form {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public CurveToExport() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Combo box k�ivek
        /// </summary>
        public ComboBox CBCurves { get { return this.cbCurves; } }

        /// <summary>
        /// True, pokud chceme ukl�dat jen y hodnoty
        /// </summary>
        public bool OnlyY { get { return this.chkOnlyY.Checked; } set { this.chkOnlyY.Checked = value; } }
    }
}