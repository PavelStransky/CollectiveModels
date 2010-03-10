using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Vr�t� vlastn� hodnoty
    /// </summary>
    public class EValues : Fnc {
        public override string Help { get { return help; } }
        public override string ParametersHelp { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(IQuantumSystem));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return (arguments[0] as IQuantumSystem).EigenSystem.GetEigenValues();
        }

        private const string help = "Vr�t� vypo��tan� vlastn� hodnoty kvantov�ho syst�mu";
        private const string parameters = "QuantumSystem";
    }
}
