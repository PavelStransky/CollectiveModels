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

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 2);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 4);

            this.CheckArgumentsType(evaluatedArguments, 1, typeof(Vector));

            if(evaluatedArguments.Count > 2 && evaluatedArguments[2] != null)
                this.CheckArgumentsType(evaluatedArguments, 2, typeof(string));

            if(evaluatedArguments.Count > 3 && evaluatedArguments[3] != null) {
                this.ConvertInt2Double(evaluatedArguments, 3);
                this.CheckArgumentsType(evaluatedArguments, 3, typeof(double));
            }

            return evaluatedArguments;
        }

        private IOutputWriter writer;
        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.writer = writer;
            return base.Evaluate(context, arguments, writer);
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item as IDynamicalSystem != null) {
                IDynamicalSystem dynamicalSystem = item as IDynamicalSystem;

                Vector ic;
                if((arguments[1] as Vector).Length == 2 * dynamicalSystem.DegreesOfFreedom) {
                    ic = (Vector)arguments[1];
                }
                else
                    return this.BadTypeError(arguments[1], 1);

                RungeKuttaMethods rkMethod = defaultRKMethod;
                if(arguments.Count > 2 && arguments[2] != null)
                    rkMethod = (RungeKuttaMethods)Enum.Parse(typeof(RungeKuttaMethods), (string)arguments[2], true);

                double precision = 0;
                if(arguments.Count > 3 && arguments[3] != null)
                    precision = (double)arguments[3];

                SALI sali = new SALI(dynamicalSystem, precision, rkMethod);

                if(sali.IsRegular(ic, writer))
                    return 1;
                else
                    return 0;
            }

            else if(item is Array)
                return this.EvaluateArray(depth, item as Array, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const RungeKuttaMethods defaultRKMethod = RungeKuttaMethods.Normal;

        private const string help = "Vrátí 1, pokud je trejektorie podle SALI regulární, jinak vrátí 0";
        private const string parameters = "Dynamický systém; poèáteèní podmínky (x, y, vx, vy) (Vector) [; metoda výpoètu RK [; pøesnost výpoètu (double)]]";
    }
}
