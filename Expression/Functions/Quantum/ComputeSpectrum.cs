using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vypoèítá spektrum LHOQuantumGCM objektu
    /// </summary>
    public class ComputeSpectrum : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 2);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 5);

            this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));

            if(evaluatedArguments.Count > 2)
                this.CheckArgumentsType(evaluatedArguments, 2, typeof(bool));
            
            if(evaluatedArguments.Count > 3)
                this.CheckArgumentsType(evaluatedArguments, 3, typeof(int));

            if(evaluatedArguments.Count > 4)
                this.CheckArgumentsType(evaluatedArguments, 4, typeof(int));

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item as LHOQuantumGCM != null) {
                int maxE = (int)arguments[1];
                int numSteps = 0;               // 0 - numsteps je dopoèítáno automaticky
                bool ev = false;
                int numev = 0;                  // 0 - berou se všechny vlastní hodnoty

                if(arguments.Count > 2 && arguments[2] != null) {
                    ev = (bool)arguments[2];
                }

                if(arguments.Count > 3 && arguments[3] != null) {
                    numev = (int)arguments[3];
                }

                if(arguments.Count > 4 && arguments[4] != null) {
                    numSteps = (int)arguments[4];
                }

                (item as LHOQuantumGCM).Compute(maxE, numSteps, ev, numev, this.writer);

                return null;
            }

            else if(item is TArray)
                return this.EvaluateArray(depth, item as TArray, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Vytvoøí LHOQuantumGCMR tøídu pro dané parametry";
        private const string parameters = "LHOQuantumGCM objekt; MaxE (int)[; True pro vlastní vektory [; Poèet vlastních hodnot [; NumSteps - dìlení møíže (int)]]]";
    }
}