using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Pro zadaný dynamický systém a energii urèí meze (vìtšinou horní odhad), ve kterých se mùže øešení pohybovat
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

        private const string help = "Pro zadaný dynamický systém a zadanou energii vrátí meze (horní odhad), ve kterých se øešení mùže pohybovat";
        private const string parameters = "Dynamický systém; energie (double)";
    }
}
