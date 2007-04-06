using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Urèí, zda trajektorie s danou poèáteèní podmínkou je regulární (1) nebo chaotická (0)
    /// </summary>
    public class SALIR: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 2);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 3);

            this.CheckArgumentsType(evaluatedArguments, 0, typeof(IDynamicalSystem));
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(Vector));

            this.ConvertInt2Double(evaluatedArguments, 2);
            this.CheckArgumentsType(evaluatedArguments, 2, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IDynamicalSystem dynamicalSystem = arguments[0] as IDynamicalSystem;

            Vector ic;
            if((arguments[1] as Vector).Length == 2 * dynamicalSystem.DegreesOfFreedom) {
                ic = (Vector)arguments[1];
            }
            else
                return this.BadTypeError(arguments[1], 1);

            double precision = 0;
            if(arguments.Count > 2 && arguments[2] != null)
                precision = (double)arguments[2];

            SALI sali = new SALI(dynamicalSystem, precision);

            if(sali.IsRegular(ic))
                return 1;
            else
                return 0;
        }

        private const RungeKuttaMethods defaultRKMethod = RungeKuttaMethods.Normal;

        private const string help = "Vrátí 1, pokud je trejektorie podle SALI regulární, jinak vrátí 0";
        private const string parameters = "Dynamický systém; poèáteèní podmínky (x, y, vx, vy) (Vector) [; pøesnost výpoètu (double)]]";
    }
}
