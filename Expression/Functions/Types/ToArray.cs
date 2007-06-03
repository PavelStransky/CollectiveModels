using System;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Creates array from some non-array types
    /// </summary>
    public class ToArray: FunctionDefinition {
        public override string Help { get { return Messages.ToArrayHelp; } }
        public override string Parameters { get { return Messages.ToArrayParameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is FileData) {
                FileData f = item as FileData;
                if(f.Binary)
                    throw new FunctionDefinitionException(Messages.ToArrayEMNotTextFile);
                return f.Lines.Clone();
            }

            else if(item is List)
                return (item as List).ToTArray();

            return null;
        }
    }
}
