using System;

using PavelStransky.Math;

namespace PavelStransky.Expression {
    /// <summary>
    /// Pr�vodce v�po�tem
    /// </summary>
    public class Guider: IOutputWriter {
        private IOutputWriter writer;
        private Context context;
        private string execDir;
        private string tmpDir;

        public string ExecDir { get { return this.execDir; } set { this.execDir = value; } }
        public string TmpDir { get { return this.tmpDir; } set { this.tmpDir = value; } }

        /// <summary>
        /// Kontext v�po�tu
        /// </summary>
        public Context Context { get { return this.context; } }

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

        #region Implementace IOutputWriter
        /// <summary>
        /// Vyp�e objekt do writeru
        /// </summary>
        /// <param name="o">Object</param>
        public string Write(object o) {
            if(this.writer != null)
                return this.writer.Write(o);
            else
                return string.Empty;
        }

        /// <summary>
        /// Vyp�e objekt do writeru a zalom� ��dku
        /// </summary>
        /// <param name="o">Object</param>
        public string WriteLine(object o) {
            if(this.writer != null)
                return this.writer.WriteLine(o);
            else
                return string.Empty;
        }

        /// <summary>
        /// Zalom� ��dku na writeru
        /// </summary>
        /// <param name="o">Object</param>
        public string WriteLine() {
            if(this.writer != null)
                return this.writer.WriteLine();
            else
                return string.Empty;
        }

        /// <summary>
        /// Vyma�e v�e na writeru
        /// </summary>
        public void Clear() {
            if(this.writer != null)
                this.writer.Clear();
        }

        /// <summary>
        /// Odsazen� v�sledku
        /// </summary>
        public int Indent(int i) {
            if(this.writer != null)
                return this.writer.Indent(i);
            else
                return i;
        }
        #endregion
    }
}
