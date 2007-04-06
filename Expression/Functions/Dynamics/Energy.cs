using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Pro zadaný dynamický systém a zadané souøadnice a hybnosti spoèítá energii
    /// </summary>
    public class Energy : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 2);
            
            this.CheckArgumentsType(evaluatedArguments, 0, typeof(IDynamicalSystem));
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IDynamicalSystem dynamicalSystem = arguments[0] as IDynamicalSystem;
            Vector v = arguments[1] as Vector;
            return dynamicalSystem.E(v);
        }

        private const string help = "Pro zadaný dynamický systém a zadané souøadnice a hybnosti vrátí energii systému.";
        private const string parameters = "Dynamický systém; souøadnice a hybnosti (Vector)";
    }
}
