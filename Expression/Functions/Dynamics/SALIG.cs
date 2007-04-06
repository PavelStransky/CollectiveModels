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

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 4);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 5);

            this.CheckArgumentsType(evaluatedArguments, 0, typeof(IDynamicalSystem));

            this.ConvertInt2Double(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));
            this.CheckArgumentsType(evaluatedArguments, 2, typeof(int));
            this.CheckArgumentsType(evaluatedArguments, 3, typeof(int));

            this.ConvertInt2Double(evaluatedArguments, 4);
            this.CheckArgumentsType(evaluatedArguments, 4, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IDynamicalSystem dynamicalSystem = arguments[0] as IDynamicalSystem;

            double e = (double)arguments[1];
            int sizex = (int)arguments[2];
            int sizey = (int)arguments[3];

            double precision = 0.0;
            if(arguments.Count > 4)
                precision = (double)arguments[4];

            SALIContourGraph sali = new SALIContourGraph(dynamicalSystem, precision);
            return sali.Compute(e, sizex, sizey, this.writer);
        }

        private const RungeKuttaMethods defaultRKMethod = RungeKuttaMethods.Normal;

        private const string help = "Pro 2D systém vrátí matici Poincarého øezu rovinou y = 0 okonturovaného podle SALI";
        private const string parameters = "Dynamický systém; energie (double); rozmìr x (int); rozmìr y (int) [; pøesnost výpoètu (double)]";
    }
}
