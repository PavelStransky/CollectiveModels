using System;

using PavelStransky.Math;

namespace PavelStransky.Expression {
    /// <summary>
    /// Prùvodce výpoètem
    /// </summary>
    public class Guider {
        private IOutputWriter writer;
        private Context context;

        /// <summary>
        /// Kontext výpoètu
        /// </summary>
        public Context Context { get { return this.context; } }

        /// <summary>
        /// Writer
        /// </summary>
        public IOutputWriter Writer { get { return this.writer; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">Kontext, na kterém se bude provádìt výpoèet</param>
        public Guider(Context context) {
            this.context = context;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">Kontext, na kterém se bude provádìt výpoèet</param>
        /// <param name="writer">Vypisovaè na obrazovku</param>
        public Guider(Context context, IOutputWriter writer)
            : this(context) {
            this.writer = writer;
        }

        /// <summary>
        /// Vytvoøí nový Guider, do kterého pøekopíruje data ze
        /// stávajícího Guideru a vymìní kontext
        /// </summary>
        /// <param name="context">Nový kontext</param>
        public Guider ChangeContext(Context context) {
            Guider result = new Guider(context);
            result.writer = this.writer;
            return result;
        }
    }
}
