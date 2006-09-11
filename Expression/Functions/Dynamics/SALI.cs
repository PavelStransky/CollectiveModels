using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vypo��t� n�hodn� vybranou trajektorii s danou energi�, resp. trajektorii s danou po��te�n� podm�nkou a dan�m �asem
    /// </summary>
    public class FnSALI: FunctionDefinition {
        public override string Name { get { return name; } }
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 3);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 5);

            this.CheckArgumentsType(evaluatedArguments, 1, typeof(Vector));

            this.ConvertInt2Double(evaluatedArguments, 2);
            this.CheckArgumentsType(evaluatedArguments, 2, typeof(double));

            if(evaluatedArguments.Count > 3 && evaluatedArguments[3] != null)
                this.CheckArgumentsType(evaluatedArguments, 3, typeof(string));

            if(evaluatedArguments.Count > 4 && evaluatedArguments[4] != null) {
                this.ConvertInt2Double(evaluatedArguments, 4);
                this.CheckArgumentsType(evaluatedArguments, 4, typeof(double));
            }

            return evaluatedArguments;
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

                double time = (double)arguments[2];

                RungeKuttaMethods rkMethod = defaultRKMethod;
                if(arguments.Count > 3 && arguments[3] != null)
                    rkMethod = (RungeKuttaMethods)Enum.Parse(typeof(RungeKuttaMethods), (string)arguments[3], true);

                double precision = 0;
                if(arguments.Count > 4 && arguments[4] != null)
                    precision = (double)arguments[4];

                SALI sali = new SALI(dynamicalSystem, precision, rkMethod);

                return sali.TimeDependence(ic, time);
            }

            else if(item is Array)
                return this.EvaluateArray(depth, item as Array, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const RungeKuttaMethods defaultRKMethod = RungeKuttaMethods.Normal;

        private const string name = "sali";
        private const string help = "Vypo��t� z�vislost SALI pro jednu tajektorii na �ase";
        private const string parameters = "Dynamick� syst�m; po��te�n� podm�nky (x, y, vx, vy) (Vector); �as (double) [; metoda v�po�tu RK [; p�esnost v�po�tu (double)]]";
    }
}
