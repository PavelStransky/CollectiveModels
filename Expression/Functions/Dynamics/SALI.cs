using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vypoèítá SALI pro zadanou trajektorii
    /// </summary>
    public class FnSALI: FunctionDefinition {
        public override string Name { get { return name; } }
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 3);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 4);

            this.CheckArgumentsType(evaluatedArguments, 0, typeof(IDynamicalSystem));
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(Vector));

            this.ConvertInt2Double(evaluatedArguments, 2);
            this.CheckArgumentsType(evaluatedArguments, 2, typeof(double));

            this.ConvertInt2Double(evaluatedArguments, 3);
            this.CheckArgumentsType(evaluatedArguments, 3, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IDynamicalSystem dynamicalSystem = arguments[0] as IDynamicalSystem;

            Vector ic;
            if((arguments[1] as Vector).Length == 2 * dynamicalSystem.DegreesOfFreedom)
                ic = (Vector)arguments[1];
            else
                return this.BadTypeError(arguments[1], 1);

            double time = (double)arguments[2];

            double precision = 0;
            if(arguments.Count > 4)
                precision = (double)arguments[4];

            SALI sali = new SALI(dynamicalSystem, precision);
            return sali.TimeDependence(ic, time);
        }

        private const RungeKuttaMethods defaultRKMethod = RungeKuttaMethods.Normal;

        private const string name = "sali";
        private const string help = "Vypoèítá závislost SALI pro jednu tajektorii na èase";
        private const string parameters = "Dynamický systém; poèáteèní podmínky (x, y, vx, vy) (Vector); èas (double) [; pøesnost výpoètu (double)]";
    }
}
