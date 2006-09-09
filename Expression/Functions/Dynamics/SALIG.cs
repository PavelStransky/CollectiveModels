using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Pro 2D syst�m zobraz� Poincar�ho �ez rovinou y = 0 okonturovan� podle SALI
    /// </summary>
    public class SALIG: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 5);
            this.ConvertInt2Double(evaluatedArguments, 1);
            this.ConvertInt2Double(evaluatedArguments, 2);
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));
            this.CheckArgumentsType(evaluatedArguments, 2, typeof(double));
            this.CheckArgumentsType(evaluatedArguments, 3, typeof(int));
            this.CheckArgumentsType(evaluatedArguments, 4, typeof(int));

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item as IDynamicalSystem != null) {
                IDynamicalSystem dynamicalSystem = item as IDynamicalSystem;
                SALIContourGraph sali = new SALIContourGraph(dynamicalSystem);

                double e = (double)arguments[1];
                double time = (double)arguments[2];
                int sizex = (int)arguments[3];
                int sizey = (int)arguments[4];

                return sali.Compute(e, time, sizex, sizey);
            }

            else if(item is Array)
                return this.EvaluateArray(depth, item as Array, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const RungeKuttaMethods defaultRKMethod = RungeKuttaMethods.Normal;

        private const string help = "Pro 2D syst�m vr�t� matici Poincar�ho �ezu rovinou y = 0 okonturovan�ho podle SALI";
        private const string parameters = "Dynamick� syst�m; energie (double); �as(double); rozm�r x (int); rozm�r y (int)";
    }
}
