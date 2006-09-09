using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vypoèítá pro kvantový GCM energetické hladiny
    /// </summary>
    public class EnergyLevels: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 3);
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));
            this.CheckArgumentsType(evaluatedArguments, 2, typeof(int));

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is QuantumGCM) {
                int l = (int)arguments[1];
                int nPhonons = (int)arguments[2];

                return (item as QuantumGCM).EnergyLevels(l, nPhonons);
            }

            else if(item is Array)
                return this.EvaluateArray(depth, item as Array, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Pro kvantový GCM vypoèítá nejnižší energetické hladiny";
        private const string parameters = "GCM; Úhlový moment (int); Poèet fononù (int)";
    }
}