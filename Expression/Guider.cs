using System;

using PavelStransky.Math;

namespace PavelStransky.Expression {
    /// <summary>
    /// Prùvodce výpoètem
    /// </summary>
    public class Guider: IOutputWriter {
        private IOutputWriter writer;
        private Context context;
        private string execDir;
        private string tmpDir;
        private bool arrayEvaluation = false;
        private bool mute = false;

        public string ExecDir { get { return this.execDir; } set { this.execDir = value; } }
        public string TmpDir { get { return this.tmpDir; } set { this.tmpDir = value; } }

        /// <summary>
        /// Kontext výpoètu
        /// </summary>
        public Context Context { get { return this.context; } }

        /// <summary>
        /// Bude provádìn výpoèet øadou?
        /// </summary>
        public bool ArrayEvaluation { get { return this.arrayEvaluation; } set { this.arrayEvaluation = value; } }

        /// <summary>
        /// True pokud bude utlumen výstup do writeru
        /// </summary>
        public bool Mute { get { return this.mute; } set { this.mute = value; } }

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
            result.arrayEvaluation = this.arrayEvaluation;
            result.execDir = this.execDir;
            result.tmpDir = this.tmpDir;
            result.mute = this.mute;

            return result;
        }

        #region Implementace IOutputWriter
        /// <summary>
        /// Vypíše objekt do writeru
        /// </summary>
        /// <param name="o">Object</param>
        public string Write(object o) {
            if(this.writer != null && !this.mute)
                return this.writer.Write(o);
            else
                return string.Empty;
        }

        /// <summary>
        /// Vypíše objekt do writeru a zalomí øádku
        /// </summary>
        /// <param name="o">Object</param>
        public string WriteLine(object o) {
            if(this.writer != null && !this.mute)
                return this.writer.WriteLine(o);
            else
                return string.Empty;
        }

        /// <summary>
        /// Zalomí øádku na writeru
        /// </summary>
        /// <param name="o">Object</param>
        public string WriteLine() {
            if(this.writer != null && !this.mute)
                return this.writer.WriteLine();
            else
                return string.Empty;
        }

        /// <summary>
        /// Vymaže vše na writeru
        /// </summary>
        public void Clear() {
            if(this.writer != null && !this.mute)
                this.writer.Clear();
        }

        /// <summary>
        /// Odsazení výsledku
        /// </summary>
        public int Indent(int i) {
            if(this.writer != null && !this.mute)
                return this.writer.Indent(i);
            else
                return i;
        }
        #endregion
    }
}
