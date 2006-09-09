using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vypoèítá náhodnì vybranou trajektorii s danou energií, resp. trajektorii s danou poèáteèní podmínkou a daným èasem
    /// </summary>
    public class FnSALI: FunctionDefinition {
        public override string Name { get { return name; } }
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 3);
            this.ConvertInt2Double(evaluatedArguments, 1);
            this.ConvertInt2Double(evaluatedArguments, 2);
            this.CheckArgumentsType(evaluatedArguments, 2, typeof(double));

            if(evaluatedArguments[1] is double && evaluatedArguments.Count > 3) {
                this.CheckArgumentsMaxNumber(evaluatedArguments, 4);

                this.ConvertInt2Double(evaluatedArguments, 3);
                this.CheckArgumentsType(evaluatedArguments, 3, typeof(double));
            }
            else
                this.CheckArgumentsMaxNumber(evaluatedArguments, 3);

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item as IDynamicalSystem != null) {
                IDynamicalSystem dynamicalSystem = item as IDynamicalSystem;
                SALI sali = new SALI(dynamicalSystem);
                Vector ic;

                double time = (double)arguments[2];

                // Náhodný výbìr trajektorie
                if(arguments[1] is double) {
                    double e = (double)arguments[1];

                    if(arguments.Count > 3) {
                        ic = dynamicalSystem.IC(e, (double)arguments[2]);
                        time = (double)arguments[3];
                    }
                    else
                        ic = dynamicalSystem.IC(e);
                }
                else if(arguments[1] is Vector && (arguments[1] as Vector).Length == 2 * dynamicalSystem.DegreesOfFreedom) {
                    ic = (Vector)arguments[1];
                }
                else
                    return this.BadTypeError(arguments[1], 1);

                return sali.TimeDependence(ic, time);
            }

            else if(item is Array)
                return this.EvaluateArray(depth, item as Array, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const RungeKuttaMethods defaultRKMethod = RungeKuttaMethods.Normal;

        private const string name = "sali";
        private const string help = "Vypoèítá závislost SALI pro jednu tajektorii na èase";
        private const string parameters = "Dynamický systém; {energie - trajektorie bude náhodnì vybrána (double); [úhlový moment (double)]} | poèáteèní podmínky (x, y, vx, vy) (Vector); èas (double)";
    }
}
