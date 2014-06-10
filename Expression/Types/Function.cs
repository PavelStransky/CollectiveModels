using System;
using System.IO;
using System.Collections;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Expression {
    /// <summary>
    /// Funkce - objekt pro vlastní funkci
    /// </summary>
    public class UserFunction: IExportable {
        private string text;
        private string retVariable;
        private Expression expression;
        private Context context;

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        public UserFunction() {
            this.text = string.Empty;
            this.context = null;
            this.retVariable = string.Empty;
            this.expression = new Expression(this.text);
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="text">Text funkce</param>
        /// <param name="context">Context - je-li null, používá se pøi každém spuštìní nový kontext</param>
        /// <param name="retVariable">Promìnná, kterou funkce vrací; je-li string.Empty, vrací celý použitý Context</param>
        public UserFunction(string text, Context context, string retVariable) {
            this.text = text;
            this.context = context;
            this.retVariable = retVariable;
            this.expression = new Expression(this.text);
        }

        /// <summary>
        /// Text funkce
        /// </summary>
        public string Text { get { return this.text; } }

        /// <summary>
        /// Kontext pro výpoèet
        /// </summary>
        /// <returns></returns>
        private Context CalculationContext(Guider guider) {
            Context result = this.context;
            if(result == null) {
                if(guider == null)
                    result = new Context();
                else {
                    result = new Context(guider.Context.Directory);
                    guider.Context.OnEvent(new ContextEventArgs(ContextEventType.NewContext, result));
                }
            }
            return result;
        }

        /// <summary>
        /// Vypoèítá funkci
        /// </summary>
        /// <param name="arguments">Argumenty funkce</param>
        /// <param name="guider">Guider</param>
        public object Evaluate(ArrayList arguments, Guider guider) {
            Context result = this.CalculationContext(guider);

            // Vytvoøíme parametry funkce
            int count = arguments.Count;
            for(int i = 0; i < count; i++) {
                result.SetVariable(string.Format(variableName, i + 1), arguments[i]);
            }

            this.expression.Evaluate(new Guider(result, guider.LocalContext, guider));

            // Vymažeme parametry funkce
            for(int i = 0; i < count; i++) {
                result.Clear(string.Format(variableName, i + 1));
            }

            if(this.retVariable == string.Empty)
                return result;

            return result[this.retVariable];           
        }

        /// <summary>
        /// Vypoèítá funkce
        /// </summary>
        /// <param name="arguments">Argumenty</param>
        public object EvaluateP(params object []arguments) {
            Context result = this.CalculationContext(null);

            // Vytvoøíme parametry funkce
            int length = arguments.Length;
            for(int i = 0; i < length; i++) {
                result.SetVariable(string.Format(variableName, i + 1), arguments[i]);
            }

            this.expression.Evaluate(new Guider(result));

            // Vymažeme parametry funkce
            for(int i = 0; i < length; i++) {
                result.Clear(string.Format(variableName, i + 1));
            }

            if(this.retVariable == string.Empty)
                return result;

            return result[this.retVariable].Item;           
        }

        /// <summary>
        /// Text funkce
        /// </summary>
        public override string ToString() {
            return this.text;
        }

        #region Implementace IExportable
        /// <summary>
        /// Uloží funkci do souboru
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
        /// Naète funkci ze souboru
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
