using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace PavelStransky.Controls {
    /// <summary>
    /// Základní pøedek pro okna, která se budou ukládat do kontextu
    /// </summary>
    public class ContextForm: System.Windows.Forms.Form {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        protected System.ComponentModel.Container components = null;

        // Kontext
        protected Controls.Context context;

        /// <summary>
		/// Konstruktor
		/// </summary>
        public ContextForm() {
            this.InitializeComponent();
        }

		/// <summary>
		/// Konstruktor s kontextem
		/// </summary>
		/// <param name="context">Kontext</param>
		public ContextForm(Controls.Context context) : this() {
			this.context = context;
		}

        /// <summary>
        /// Pøedek pro inicializaci komponent
        /// </summary>
        protected virtual void InitializeComponent() {
        }

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

        /// <summary>
        /// Voláno pøi zavøení formuláøe
        /// </summary>
        protected override void OnFormClosed(FormClosedEventArgs e) {
            base.OnFormClosed(e);
            this.context.Clear(this.Name);
        }

        /// <summary>
        /// Uzavøe formuláø; pokud formuláø nelze uzavøít, vrátí false
        /// </summary>
        public virtual new bool Close() {
            base.Close();
            return false;
        }
    }
}
