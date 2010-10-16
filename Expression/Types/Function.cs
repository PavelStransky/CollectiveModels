using System;
using System.IO;
using System.Collections;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Expression {
    /// <summary>
    /// Funkce - objekt pro vlastn� funkci
    /// </summary>
    public class UserFunction: IExportable {
        private string text;
        private string retVariable;
        private Expression expression;
        private Context context;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="text">Text funkce</param>
        /// <param name="context">Context - je-li null, pou��v� se p�i ka�d�m spu�t�n� nov� kontext</param>
        /// <param name="retVariable">Prom�nn�, kterou funkce vrac�; je-li string.Empty, vrac� cel� pou�it� Context</param>
        public UserFunction(string text, Context context, string retVariable) {
            this.text = text;
            this.context = context;
            this.retVariable = retVariable;
            this.expression = new Expression(this.text);
        }

        /// <summary>
        /// Vypo��t� funkci
        /// </summary>
        /// <param name="arguments">Argumenty funkce</param>
        /// <param name="guider">Guider</param>
        public object Evaluate(ArrayList arguments, Guider guider) {
            Context result = this.context;
            if(result == null) {
                result = new Context(guider.Context.Directory);
                guider.Context.OnEvent(new ContextEventArgs(ContextEventType.NewContext, result));
            }

            // Vytvo��me parametry funkce
            int count = arguments.Count;
            for(int i = 0; i < count; i++) {
                result.SetVariable(string.Format(variableName, i + 1), arguments[i]);
            }

            this.expression.Evaluate(new Guider(result, guider));

            // Vyma�eme parametry funkce
            for(int i = 0; i < count; i++) {
                result.Clear(string.Format(variableName, i + 1));
            }

            if(this.retVariable == string.Empty)
                return result;

            return result[this.retVariable];           
        }

        /// <summary>
        /// Text funkce
        /// </summary>
        public override string ToString() {
            return this.text;
        }

        #region Implementace IExportable
        /// <summary>
        /// Ulo�� funkci do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam p = new IEParam();
            p.Add(this.text, "Function content");
            p.Add(this.context, "Context");
            p.Add(this.retVariable, "Return variable");
            p.Export(export);
        }

        /// <summary>
        /// Na�te funkci ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public UserFunction(Core.Import import) {
            IEParam p = new IEParam(import);
            this.text = (string)p.Get(string.Empty);
            this.context = (Context)p.Get();
            this.retVariable = (string)p.Get(string.Empty);
            this.expression = new Expression(text);
        }
        #endregion

        private const string variableName = "_p{0}";
    }
}
