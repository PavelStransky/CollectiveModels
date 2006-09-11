using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Pro 2D systém zobrazí Poincarého øez rovinou y = 0 okonturovaný podle SALI
    /// </summary>
    public class SALIG: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 5);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 7);

            this.ConvertInt2Double(evaluatedArguments, 1);
            this.ConvertInt2Double(evaluatedArguments, 2);
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));
            this.CheckArgumentsType(evaluatedArguments, 2, typeof(double));
            this.CheckArgumentsType(evaluatedArguments, 3, typeof(int));
            this.CheckArgumentsType(evaluatedArguments, 4, typeof(int));

            if(evaluatedArguments.Count > 5 && evaluatedArguments[5] != null)
                this.CheckArgumentsType(evaluatedArguments, 5, typeof(string));

            if(evaluatedArguments.Count > 6 && evaluatedArguments[6] != null) {
                this.ConvertInt2Double(evaluatedArguments, 6);
                this.CheckArgumentsType(evaluatedArguments, 6, typeof(double));
            } 

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item as IDynamicalSystem != null) {
                IDynamicalSystem dynamicalSystem = item as IDynamicalSystem;

                double e = (double)arguments[1];
                double time = (double)arguments[2];
                int sizex = (int)arguments[3];
                int sizey = (int)arguments[4];

                RungeKuttaMethods rkMethod = defaultRKMethod;
                if(arguments.Count > 5 && arguments[5] != null)
                    rkMethod = (RungeKuttaMethods)Enum.Parse(typeof(RungeKuttaMethods), (string)arguments[5], true);

                double precision = 0;
                if(arguments.Count > 6 && arguments[6] != null)
                    precision = (double)arguments[6];

                SALIContourGraph sali = new SALIContourGraph(dynamicalSystem, precision, rkMethod);

                return sali.Compute(e, time, sizex, sizey);
            }

            else if(item is Array)
                return this.EvaluateArray(depth, item as Array, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const RungeKuttaMethods defaultRKMethod = RungeKuttaMethods.Normal;

        private const string help = "Pro 2D systém vrátí matici Poincarého øezu rovinou y = 0 okonturovaného podle SALI";
        private const string parameters = "Dynamický systém; energie (double); èas(double); rozmìr x (int); rozmìr y (int) [; metoda výpoètu RK [; pøesnost výpoètu (double)]]";
    }
}
