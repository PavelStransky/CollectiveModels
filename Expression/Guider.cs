using System;

using PavelStransky.Math;

namespace PavelStransky.Expression {
    /// <summary>
    /// Pr�vodce v�po�tem
    /// </summary>
    public class Guider {
        private IOutputWriter writer;
        private Context context;

        /// <summary>
        /// Kontext v�po�tu
        /// </summary>
        public Context Context { get { return this.context; } }

        /// <summary>
        /// Writer
        /// </summary>
        public IOutputWriter Writer { get { return this.writer; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">Kontext, na kter�m se bude prov�d�t v�po�et</param>
        public Guider(Context context) {
            this.context = context;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">Kontext, na kter�m se bude prov�d�t v�po�et</param>
        /// <param name="writer">Vypisova� na obrazovku</param>
        public Guider(Context context, IOutputWriter writer)
            : this(context) {
            this.writer = writer;
        }

        /// <summary>
        /// Vytvo�� nov� Guider, do kter�ho p�ekop�ruje data ze
        /// st�vaj�c�ho Guideru a vym�n� kontext
        /// </summary>
        /// <param name="context">Nov� kontext</param>
        public Guider ChangeContext(Context context) {
            Guider result = new Guider(context);
            result.writer = this.writer;
            return result;
        }
    }
}
