using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vypoèítá Poincarého øez pro danou energii, resp. trajektorii a daný poèet bodù
    /// </summary>
    public class Poincare: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 6);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 8);

            this.ConvertInt2Double(evaluatedArguments, 1);

            this.CheckArgumentsType(evaluatedArguments, 2, typeof(int));
            this.CheckArgumentsType(evaluatedArguments, 3, typeof(Vector));
            this.CheckArgumentsType(evaluatedArguments, 4, typeof(int));
            this.CheckArgumentsType(evaluatedArguments, 5, typeof(int));

            if(evaluatedArguments.Count > 6 && evaluatedArguments[6] != null)
                this.CheckArgumentsType(evaluatedArguments, 6, typeof(string));

            if(evaluatedArguments.Count > 7 && evaluatedArguments[7] != null) {
                this.ConvertInt2Double(evaluatedArguments, 7);
                this.CheckArgumentsType(evaluatedArguments, 7, typeof(double));
            } 

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            int numPoints = (int)arguments[2];
            Vector plane = (Vector)arguments[3];
            int i1 = (int)arguments[4];
            int i2 = (int)arguments[5];
            double precision = 0;

            RungeKuttaMethods rkMethod = defaultRKMethod;

            if(arguments.Count > 6 && arguments[6] != null)
                rkMethod = (RungeKuttaMethods)Enum.Parse(typeof(RungeKuttaMethods), (string)arguments[6], true);

            if(arguments.Count > 7 && arguments[7] != null)
                precision = (double)arguments[7];

            if(item as IDynamicalSystem != null) {
                IDynamicalSystem dynamicalSystem = item as IDynamicalSystem;
                Vector ic;

                // Náhodný výbìr trajektorie
                if(arguments[1] is double) {
                    double e = (double)arguments[1];
                    ic = dynamicalSystem.IC(e);
                }
                else if(arguments[1] is Vector && (arguments[1] as Vector).Length == 2 * dynamicalSystem.DegreesOfFreedom)
                    ic = (Vector)arguments[1];
                else
                    return this.BadTypeError(arguments[1], 1);

                PoincareSection ps = new PoincareSection(dynamicalSystem, plane, i1, i2, precision, rkMethod);

                PointVector result;
                int numAttemps = 0;
                while(true) {
                    try {
                        result = ps.Compute(ic, numPoints);
                        break;
                    }
                    catch(PoincareSectionException exc) {
                        if(arguments[1] is double && numAttemps < maxNumAttemps) {
                            double e = (double)arguments[1];
                            ic = dynamicalSystem.IC(e);
                        }
                        else
                            throw exc;
                    }
                    catch(Exception exc) {
                        throw exc;
                    }
                }

                return result;
            }

            else if(item is Array)
                return this.EvaluateArray(depth, item as Array, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const RungeKuttaMethods defaultRKMethod = RungeKuttaMethods.Normal;

        // Maximální poèet pokusù, než bude 
        private const int maxNumAttemps = 10;

        private const string help = "Vypoèítá Poincarého øez pro danou poèáteèní podmínku";
        private const string parameters = "Dynamický systém; energie - trajektorie bude náhodnì vybrána (double) | poèáteèní podmínky (x, y, vx, vy) (Vector); poèet bodù øezu; rovina øezu (Vector); index 1 (int); index 2 (int) [; metoda výpoètu RK [; pøesnost výpoètu (double)]]";
    }
}
