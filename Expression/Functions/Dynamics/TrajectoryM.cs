using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vypo��t� n�hodn� vybranou trajektorii s danou energi�, resp. trajektorii s danou po��te�n� podm�nkou a dan�m �asem
    /// a vr�t� v�sledek jako matici
    /// </summary>
    public class TrajectoryM: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 3);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 6);

            this.CheckArgumentsType(evaluatedArguments, 0, typeof(IDynamicalSystem));
            
            this.ConvertInt2Double(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(Vector), typeof(double));

            this.ConvertInt2Double(evaluatedArguments, 2);
            this.CheckArgumentsType(evaluatedArguments, 2, typeof(double));

            if(evaluatedArguments.Count > 3 && evaluatedArguments[3] != null) {
                this.ConvertInt2Double(evaluatedArguments, 3);
                this.CheckArgumentsType(evaluatedArguments, 3, typeof(double));
            }

            if(evaluatedArguments.Count > 4 && evaluatedArguments[4] != null)
                this.CheckArgumentsType(evaluatedArguments, 4, typeof(string));

            if(evaluatedArguments.Count > 5) {
                this.ConvertInt2Double(evaluatedArguments, 5);
                this.CheckArgumentsType(evaluatedArguments, 5, typeof(double));
            }
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IDynamicalSystem dynamicalSystem = arguments[0] is IDynamicalSystem;
            double t = (double)arguments[2];
            double precision = 0;
            double timeStep = 0;
            RungeKuttaMethods rkMethod = defaultRKMethod;

            if(arguments.Count > 3 && arguments[3] != null)
                timeStep = (double)arguments[3];

            if(arguments.Count > 4 && arguments[4] != null)
                rkMethod = (RungeKuttaMethods)Enum.Parse(typeof(RungeKuttaMethods), (string)arguments[4], true);

            if(arguments.Count > 5 && arguments[5] != null)
                precision = (double)arguments[5];

            if(item as IDynamicalSystem != null) {
                IDynamicalSystem dynamicalSystem = item as IDynamicalSystem;
                Vector ic;

                // N�hodn� v�b�r trajektorie
                if(arguments[1] is double) {
                    double e = (double)arguments[1];
                    ic = dynamicalSystem.IC(e);
                }
                else if(arguments[1] is Vector && (arguments[1] as Vector).Length == 2 * dynamicalSystem.DegreesOfFreedom) {
                    ic = (Vector)arguments[1];
                }
                else
                    return this.BadTypeError(arguments[1], 1);

                Trajectory trajectory = new Trajectory(dynamicalSystem, precision, timeStep, rkMethod);
                return trajectory.Compute(ic, t);

            }

            else if(item is TArray)
                return this.EvaluateArray(depth, item as TArray, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const RungeKuttaMethods defaultRKMethod = RungeKuttaMethods.Normal;

        private const string help = "Vypo��t� jednu trajektorii pro danou energii nebo pro danou po��te�n� podm�nku a vr�t� kompletn� matici (time, x, y, vx, vy)";
        private const string parameters = "GCM; energie - trajektorie bude n�hodn� vybr�na (double) | po��te�n� podm�nky (x, y, vx, vy) (Vector); �as(double) [; �asov� krok v�sledku (double) [; metoda v�po�tu RK (\"normal\" | \"energy\" | \"adapted\") [; p�esnost v�po�tu (double)]]]";
    }
}
