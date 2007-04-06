using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vrac� logaritmus objektu v argumentu (po prvc�ch)
    /// </summary>
    public class Log: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        private double logBase;

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 2);

            this.ConvertInt2Double(evaluatedArguments, 1);

            this.CheckArgumentsType(evaluatedArguments, 0,
                typeof(int), typeof(double), typeof(Vector), typeof(PointVector), typeof(PointD), typeof(Matrix));
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));
        }

        /// <summary>
        /// Vypo��t� logaritmus o z�kladu logBase
        /// </summary>
        private double ComputeLog(double x) {
            if(x == 0.0)
                return 0.0;

            if(double.IsNaN(this.logBase))
                return System.Math.Log(x);
            else if(this.logBase == 10)
                return System.Math.Log10(x);
            else
                return System.Math.Log(x, this.logBase);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(arguments.Count > 1)
                this.logBase = (double)arguments[1];
            else
                this.logBase = double.NaN;

            if(item is int)
                return this.ComputeLog((int)item);
            else if(item is double)
                return this.ComputeLog((double)item);
            else if(item is PointD)
                return new PointD((item as PointD).X, this.ComputeLog((item as PointD).Y));
            else if(item is Vector)
                return (item as Vector).Transform(new RealFunction(this.ComputeLog));
            else if(item is PointVector)
                return (item as PointVector).Transform(null, new RealFunction(this.ComputeLog));
            else
                return (item as Matrix).Transform(new RealFunction(this.ComputeLog));
        }

        private const string help = "Vrac� logaritmus objektu v argumentu (po prvc�ch), u bodu nebo vektoru bod� d�l� logaritmus pouze z Y slo�ky";
        private const string parameters = "int | double | Point | Vector | PointVector | Matrix";
    }
}
