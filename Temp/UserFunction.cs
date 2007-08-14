using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vytvo�� novou u�ivatelskou funkci
    /// </summary>
    public class UserFunction : FunctionDefinition {
        private string fncName;
        private bool userContext;   // True, pokud bude prvn� parametr funkce context, na kter�m se bude funkce spou�t�t

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fncName">Jm�no funkce (v�etn� prefixu)</param>
        public UserFunction(string fncName) {
            if(fncName[0] == fncFirstChar) {
                fncName = fncName.Substring(1);
                this.userContext = true;
            }
            else
                this.userContext = false;

            this.fncName = fncName;
        }

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            string fileName = Path.Combine(Context.FncDirectory, string.Format("{0}.{1}", fncName, fncExtension));
            string fileText = string.Empty;
            try {
                FileStream fileStream = new FileStream(fileName, FileMode.Open);
                StreamReader s = new StreamReader(fileStream, System.Text.Encoding.GetEncoding(1250));
                fileText = s.ReadToEnd();
                s.Close();
                fileStream.Close();
            }
            catch {
                throw new ExpressionException(string.Format(errorMessageFunctionNotExists, this.fncName));
            }

            Expression e = new Expression(fileText, writer);

            ArrayList evaluatedArguments = this.EvaluateArguments(context, arguments, writer);

            Context result = null;
            if(this.userContext) {
                this.CheckArgumentsType(evaluatedArguments, 0, typeof(Context));
                result = evaluatedArguments[0] as Context;
                evaluatedArguments.RemoveAt(0);
            }
            else {
                result = new Context(context.Directory);
                context.OnEvent(new ContextEventArgs(ContextEventType.NewContext, result));
            }

            // Vytvo��me parametry funkce
            int count = evaluatedArguments.Count;
            for(int i = 0; i < count; i++) {
                result.SetVariable(string.Format(variableName, i + 1), evaluatedArguments[i]);
            }

            e.Evaluate(result);

            // Vyma�eme parametry funkce
            for(int i = 0; i < count; i++) {
                result.Clear(string.Format(variableName, i + 1));
            }

            return result;
        }

        /// <summary>
        /// Vytvo�� u�ivatelskou funkci
        /// </summary>
        /// <param name="fncName">Jm�no funkce</param>
        /// <returns>Pokud se nepoda�ilo vytvo�it, vrac� null</returns>
        public static UserFunction CreateUserFunction(string fncName) {
            if(fncName.Length > 0 && fncName[0] == fncFirstChar)
                return new UserFunction(fncName.Substring(1));
            else
                return null;
        }

        private const string fncExtension = "txt";
        private const char fncFirstChar = '@';

        private const string variableName = "_p{0}";

        private const string errorMessageFunctionNotExists = "Funkce {0} nebyla nalezena.";
    }
}
