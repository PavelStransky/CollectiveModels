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

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0,
                typeof(int), typeof(double), typeof(Vector), typeof(PointVector), typeof(PointD), typeof(Matrix));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is int)
                return SpecialFunctions.FactorialI((int)item);
            else if(item is double)
                return SpecialFunctions.Factorial((double)item);
            else if(item is PointD)
                return new PointD((item as PointD).X, SpecialFunctions.Factorial((item as PointD).Y));
            else if(item is Vector)
                return (item as Vector).Transform(SpecialFunctions.Factorial);
            else if(item is PointVector)
                return (item as PointVector).Transform(null, SpecialFunctions.Factorial);
            else
                return (item as Matrix).Transform(SpecialFunctions.Factorial);
        }

        private const string help = "Vrac� faktori�l objektu v argumentu (po prvc�ch), u bodu nebo vektoru bod� d�l� absolutn� hodnotu pouze z Y slo�ky";
        private const string parameters = "int | double | Point | Vector | PointVector | Matrix";
    }
}
