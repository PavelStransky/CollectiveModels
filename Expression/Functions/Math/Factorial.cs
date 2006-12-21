using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vrac� faktori�l objektu v argumentu (po prvc�ch)
    /// </summary>
    public class Factorial : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is int)
                return SpecialFunctions.Factorial((int)item);
            else if(item is double)
                return SpecialFunctions.Factorial((double)item);
            else if(item is PointD)
                return new PointD((item as PointD).X, SpecialFunctions.Factorial((item as PointD).Y));
            else if(item is Vector)
                return (item as Vector).Transform(SpecialFunctions.Factorial);
            else if(item is PointVector)
                return (item as PointVector).Transform(null, SpecialFunctions.Factorial);
            else if(item is Matrix)
                return (item as Matrix).Transform(SpecialFunctions.Factorial);
            else if(item is Array)
                return this.EvaluateArray(depth, item as Array, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Vrac� faktori�l objektu v argumentu (po prvc�ch), u bodu nebo vektoru bod� d�l� absolutn� hodnotu pouze z Y slo�ky";
        private const string parameters = "int | double | Point | Vector | PointVector | Matrix";
    }
}
