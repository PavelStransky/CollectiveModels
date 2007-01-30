using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Provede vyhlazení vektoru
    /// </summary>
    public class Smooth : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, typeof(Vector), typeof(PointVector), typeof(TArray));
            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is Vector)
                return (item as Vector).Smooth();
            else if(item is PointVector) {
                return new PointVector((item as PointVector).VectorX, (item as PointVector).VectorY.Smooth());
            }
            else if(item is TArray)
                return this.EvaluateArray(depth, item as TArray, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Provede vyhlazení vektoru";
        private const string parameters = "Vector | PointVector";
    }
}
