using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PavelStransky.Forms {
    /// <summary>
    /// Tøída, která obsahuje odkaz na Editor (parent), ke kterému pøísluší
    /// </summary>
    public class ChildForm : Form {
        private Editor parent;

        public Editor ParentEditor { get { return this.parent; } set { this.parent = value; } }
    }
}