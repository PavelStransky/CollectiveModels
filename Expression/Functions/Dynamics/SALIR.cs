using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Ur��, zda trajektorie s danou po��te�n� podm�nkou je regul�rn� (1) nebo chaotick� (0)
    /// </summary>
    public class SALIR: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 2);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 3);

            this.CheckArgumentsType(evaluatedArguments, 1, typeof(Vector));

            if(evaluatedArguments.Count > 2 && evaluatedArguments[2] != null) {
                this.ConvertInt2Double(evaluatedArguments, 2);
                this.CheckArgumentsType(evaluatedArguments, 2, typeof(double));
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

                double precision = 0;
                if(arguments.Count > 2 && arguments[2] != null)
                    precision = (double)arguments[2];

                SALI sali = new SALI(dynamicalSystem, precision);

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

        private const string help = "Vr�t� 1, pokud je trejektorie podle SALI regul�rn�, jinak vr�t� 0";
        private const string parameters = "Dynamick� syst�m; po��te�n� podm�nky (x, y, vx, vy) (Vector) [; p�esnost v�po�tu (double)]]";
    }
}
