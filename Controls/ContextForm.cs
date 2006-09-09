using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace PavelStransky.Controls {
    /// <summary>
    /// Z�kladn� p�edek pro okna, kter� se budou ukl�dat do kontextu
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
        /// P�edek pro inicializaci komponent
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
        /// Vol�no p�i zav�en� formul��e
        /// </summary>
        protected override void OnFormClosed(FormClosedEventArgs e) {
            base.OnFormClosed(e);
            this.context.Clear(this.Name);
        }

        /// <summary>
        /// Uzav�e formul��; pokud formul�� nelze uzav��t, vr�t� false
        /// </summary>
        public virtual new bool Close() {
            base.Close();
            return false;
        }
    }
}
