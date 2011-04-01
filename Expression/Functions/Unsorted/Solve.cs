using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Solves an equation "the user function == zero"
    /// </summary>
    public class FnSolve: Fnc {
        public override string Help { get { return Messages.HelpSolve; } }
        public override string Name { get { return name; } }

        protected override void CreateParameters() {
            this.SetNumParams(5, true);
            this.SetParam(0, true, true, false, Messages.PFnc, Messages.PFncDescription, null, typeof(UserFunction));
            this.SetParam(1, true, true, true, Messages.PMinX, Messages.PMinXDescription, null, typeof(double));
            this.SetParam(2, true, true, true, Messages.PMaxX, Messages.PMaxXDescription, null, typeof(double));
            this.SetParam(3, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 0.0, typeof(double));
            this.SetParam(4, false, true, false, Messages.PParam, Messages.PParamDescription, null);
        }

        /// <summary>
        /// Class used to solve the equation (stores all additional parameters and Guider)
        /// </summary>
        private class BisectionFunction {
            private UserFunction function;
            private ArrayList arguments;
            private Guider guider;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="function">Function</param>
            /// <param name="arguments">Additional arguments of the function</param>
            /// <param name="guider">Guider</param>
            public BisectionFunction(UserFunction function, ArrayList arguments, Guider guider) {
                this.function = function;
                this.arguments = arguments;
                this.guider = guider;
            }

            /// <summary>
            /// Value of the function at the point x
            /// </summary>
            /// <param name="x">Point x</param>
            public double Function(double x) {
                this.arguments[0] = x;
                return (double)(function.Evaluate(this.arguments, this.guider) as Variable).Item;
            }
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            UserFunction function = (UserFunction)arguments[0];
            double minX = (double)arguments[1];
            double maxX = (double)arguments[2];
            double precision = (double)arguments[3];

            arguments.RemoveAt(0);      // function
            arguments.RemoveAt(0);      // minx
            arguments.RemoveAt(0);      // maxx; we keep precision to be replaced by the actual value x
            if(arguments[1] == null)    // Last argument
                arguments.RemoveAt(1);

            BisectionFunction fnc = new BisectionFunction(function, arguments, guider);
            Bisection b = new Bisection(fnc.Function);
            return b.Solve(minX, maxX, precision);
        }

        private const string name = "solve";
    }
}
