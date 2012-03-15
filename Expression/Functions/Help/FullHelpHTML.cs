using System;
using System.Text;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Full help for all functions (including names and types of the parameters) in the HTML format
    /// </summary>
    public class FullHelpHTML: Fnc {
        private FncList functions;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="functions">Slovník zaregistrovaných funkcí</param>
        public FullHelpHTML(FncList functions)
            : base() {
            this.functions = functions;
        }

        public override string Help { get { return Messages.HelpFullHelpHTML; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int length = this.functions.Count;
            StringBuilder result = new StringBuilder();
            
            string [] fnames = new string[length];
            int i = 0;
            foreach(Fnc fnc in this.functions.Values)
                fnames[i++] = fnc.Name;

            Array.Sort(fnames);

            result.Append("<table>");
            result.Append(Environment.NewLine);
            for(i = 0; i < length; i++) {
                Fnc fnc = this.functions[fnames[i]] as Fnc;
                result.Append("  <tr id=\"" + fnc.Name + "\">\n");
                result.Append("    <th>" + fnc.Name + "</th>\n");
                result.Append("    <td>" + this.NewLineToBR(fnc.Help) + "</td>\n");
                result.Append("    <td>" + this.NewLineToBR(fnc.ParametersHelp) + "</td>\n");
                result.Append("  </tr>\n");
            }
            result.Append("</table>\n");

            return result.ToString();
        }

        private string NewLineToBR(string s) {
            return s.Trim('\r', '\n', ' ').Replace("\r", string.Empty).Replace("\n", "<br />");
        }
    }
}
