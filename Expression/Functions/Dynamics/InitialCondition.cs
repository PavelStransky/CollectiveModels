using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Nageneruje n�hodn� po��te�n� podm�nky pro jednu trajektorii se zadanou energi�
    /// </summary>
    public class InitialCondition: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 2);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 3);

            this.ConvertInt2Double(evaluatedArguments, 1);
            this.ConvertInt2Double(evaluatedArguments, 2);

            this.CheckArgumentsType(evaluatedArguments, 0, typeof(IDynamicalSystem));
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));
            this.CheckArgumentsType(evaluatedArguments, 2, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IDynamicalSystem dynamicalSystem = arguments[0] as IDynamicalSystem;

            if(arguments.Count > 2) {
                double l = (double)arguments[2];
                return dynamicalSystem.IC(e, l);
            }
            else
                return dynamicalSystem.IC(e);
        }

        private const string help = "Nageneruje pro danou energii jednu po��te�n� podm�nku a vr�t� ji jako vektor (x, y, vx, vy)";
        private const string parameters = "Dynamick� syst�m; energie (double) [; �hlov� moment (double)]";
    }
}
