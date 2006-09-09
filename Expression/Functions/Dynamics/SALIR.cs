using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Urèí, zda náhodnì vybraná trajektorie s danou energií, resp. trajektorie s danou poèáteèní 
    /// podmínkou je regulární (1) nebo chaotická (0)
    /// </summary>
    public class SALIR: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 2);
            this.ConvertInt2Double(evaluatedArguments, 1);

            if(evaluatedArguments[1] is double && evaluatedArguments.Count > 2) {
                this.CheckArgumentsMaxNumber(evaluatedArguments, 3);

                this.ConvertInt2Double(evaluatedArguments, 2);
                this.CheckArgumentsType(evaluatedArguments, 2, typeof(double));
            }
            else
                this.CheckArgumentsMaxNumber(evaluatedArguments, 2);

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item as IDynamicalSystem != null) {
                IDynamicalSystem dynamicalSystem = item as IDynamicalSystem;
                SALI sali = new SALI(dynamicalSystem);
                Vector ic;

                // Náhodný výbìr trajektorie
                if(arguments[1] is double) {
                    double e = (double)arguments[1];

                    if(arguments.Count > 2)
                        ic = dynamicalSystem.IC(e, (double)arguments[2]);
                    else
                        ic = dynamicalSystem.IC(e);
                }
                else if(arguments[1] is Vector && (arguments[1] as Vector).Length == 2 * dynamicalSystem.DegreesOfFreedom) {
                    ic = (Vector)arguments[1];
                }
                else
                    return this.BadTypeError(arguments[1], 1);

                if(sali.IsRegular(ic))
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
        private const string parameters = "Dynamický systém; {energie - trajektorie bude náhodnì vybrána (double); [úhlový moment (double)]} | poèáteèní podmínky (x, y, vx, vy) (Vector)";
    }
}
