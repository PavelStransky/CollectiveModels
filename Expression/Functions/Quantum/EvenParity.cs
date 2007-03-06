using System;
using System.Collections;

using PavelStransky.GCM;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vr�t� ��sla stav�, kter� maj� sudou paritu
    /// </summary>
    public class EvenParity: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is LHOQuantumGCMR) {
                LHOQuantumGCMR q = item as LHOQuantumGCMR;
                return q.EvenParity();
            }

            else if(item is TArray)
                return this.EvaluateArray(depth, item as TArray, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Vr�t� ��sla stav�, kter� maj� sudou paritu";
        private const string parameters = "LHOQuantumGCMR";
    }
}
