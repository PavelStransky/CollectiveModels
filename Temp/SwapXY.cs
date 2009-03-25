using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Swaps X and Y coordinates in a Point or PointVector
    /// </summary>
    public class SwapXY : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is PointVector)
                return (item as PointVector).SwapXY();
            else if(item is PointD)
                return new PointD((item as PointD).Y, (item as PointD).X);
            else if(item is TArray)
                return this.EvaluateArray(depth, item as TArray, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "V bodu (PointD) nebo vektoru bodù (PointVector) prohodí souøadnice X a Y";
        private const string parameters = "PointD | PointVector";
    }
}
