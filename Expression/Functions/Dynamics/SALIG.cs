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
            this.CheckArgumentsMinNumber(evaluatedArguments, 4);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 5);

            this.ConvertInt2Double(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 2, typeof(int));
            this.CheckArgumentsType(evaluatedArguments, 3, typeof(int));

            if(evaluatedArguments.Count > 4) {
                this.ConvertInt2Double(evaluatedArguments, 4);
                this.CheckArgumentsType(evaluatedArguments, 4, typeof(double));
            }

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item as IDynamicalSystem != null) {
                IDynamicalSystem dynamicalSystem = item as IDynamicalSystem;

                double e = (double)arguments[1];
                int sizex = (int)arguments[2];
                int sizey = (int)arguments[3];

                double precision = 0.0;
                if(arguments.Count > 4 && arguments[4] != null)
                    precision = (double)arguments[4];

                SALIContourGraph sali = new SALIContourGraph(dynamicalSystem, precision);
                return sali.Compute(e, sizex, sizey, this.writer);
            }

            else if(item is TArray)
                return this.EvaluateArray(depth, item as TArray, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const RungeKuttaMethods defaultRKMethod = RungeKuttaMethods.Normal;

        private const string help = "Pro 2D systém vrátí matici Poincarého øezu rovinou y = 0 okonturovaného podle SALI";
        private const string parameters = "Dynamický systém; energie (double); rozmìr x (int); rozmìr y (int) [; pøesnost výpoètu (double)]";
    }
}
