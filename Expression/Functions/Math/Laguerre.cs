using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vrací hodnotu Laguerrova polynomu
    /// </summary>
    public class Laguerre : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        private int n, m;

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 2);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 3);

            this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));

            if(evaluatedArguments.Count > 2)
                this.CheckArgumentsType(evaluatedArguments, 2, typeof(int));

            return evaluatedArguments;
        }

        /// <summary>
        /// Vypoèítá logaritmus o základu logBase
        /// </summary>
        private double ComputeLaguerre(double x) {
            return LaguerrePolynom.GetValueS(this.n, this.m, x);
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            this.n = (int)arguments[1];

            if(arguments.Count > 2)
                this.m = (int)arguments[2];
            else
                this.m = 0;

            if(item is int)
                return this.ComputeLaguerre((int)item);
            else if(item is double)
                return this.ComputeLaguerre((double)item);
            else if(item is PointD)
                return new PointD((item as PointD).X, this.ComputeLaguerre((item as PointD).Y));
            else if(item is Vector)
                return (item as Vector).Transform(this.ComputeLaguerre);
            else if(item is PointVector)
                return (item as PointVector).Transform(null, this.ComputeLaguerre);
            else if(item is Matrix)
                return (item as Matrix).Transform(this.ComputeLaguerre);
            else if(item is Array)
                return this.EvaluateArray(depth, item as Array, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Vrací hodnotu Laguerrovy funkce, u bodu nebo vektoru bodù poèítá pouze u Y složky";
        private const string parameters = "int | double | Point | Vector | PointVector | Matrix | Array; øád polynomu [; stupeò polynomu]";
    }
}
