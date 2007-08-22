using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Saves a variable to a file
	/// </summary>
	public class FnExport: FncIE {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpExport; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PExpression, Messages.PExpressionDescription, null);
            this.SetParam(1, true, true, false, Messages.PFileName, Messages.PFileNameDescription, null, typeof(string));
            this.SetParam(2, false, true, false, Messages.PFileType, Messages.PFileTypeDescription, "binary", typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Export export = new Export((string)arguments[0], this.Binary(arguments, 2));
            export.Write(arguments[1]);
            export.Close();

			return null;
		}

        private const string name = "export";
	}
}
