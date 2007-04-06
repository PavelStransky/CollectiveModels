using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Pro zadan� dynamick� syst�m a energii ur�� meze (v�t�inou horn� odhad), ve kter�ch se m��e �e�en� pohybovat
    /// </summary>
    public class Bounds : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 2);
            this.CheckArgumentsType(evaluatedArguments, 0, typeof(IQuantumSystem));
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IDynamicalSystem dynamicalSystem = arguments[0] as IDynamicalSystem;
            return dynamicalSystem.Bounds((double)arguments[1]);
        }

        private const string help = "Pro zadan� dynamick� syst�m a zadanou energii vr�t� meze (horn� odhad), ve kter�ch se �e�en� m��e pohybovat";
        private const string parameters = "Dynamick� syst�m; energie (double)";
    }
}
