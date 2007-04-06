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

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 2);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 5);

            this.CheckArgumentsType(evaluatedArguments, 0, typeof(LHOQuantumGCM));

            this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));
            this.CheckArgumentsType(evaluatedArguments, 2, typeof(bool));
            this.CheckArgumentsType(evaluatedArguments, 3, typeof(int));
            this.CheckArgumentsType(evaluatedArguments, 4, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            LHOQuantumGCM item = arguments[0] as LHOQuantumGCM;

            int maxE = (int)arguments[1];
            int numSteps = 0;               // 0 - numsteps je dopoèítáno automaticky
            bool ev = false;
            int numev = 0;                  // 0 - berou se všechny vlastní hodnoty

            if(arguments.Count > 2 && arguments[2] != null) 
                ev = (bool)arguments[2];

            if(arguments.Count > 3 && arguments[3] != null) 
                numev = (int)arguments[3];

                if(arguments.Count > 4 && arguments[4] != null) 
                    numSteps = (int)arguments[4];

                item.Compute(maxE, numSteps, ev, numev, guider.Writer);

                return null;
        }
    
        private const string help = "Vypoèítá spektrum LHOQuantumGCM tøídy";
        private const string parameters = "LHOQuantumGCM objekt; MaxE (int)[; True pro vlastní vektory [; Poèet vlastních hodnot (int) [; NumSteps - dìlení møíže (int)]]]";
    }
}