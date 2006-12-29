using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vysèítá prvky v absolutní hodnotì
    /// </summary>
    public class SumAbs : FunctionDefinitionMinMax {
        public override string Help { get { return help; } }

        protected override object EvaluateGoodDepth(int depth, object item, ArrayList arguments) {
            if(item is int)
                return (double)(int)item;
            else if(item is double)
                return item;
            else if(item is Vector)
                return (item as Vector).SumAbs();
            else if(item is Matrix)
                return (item as Matrix).SumAbs();
            else if(item is TArray) {
                TArray result = this.EvaluateArray(depth, item as TArray, arguments);
                this.CheckResultLength(result, depth);

                double sum = 0.0;
                int count = result.Count;

                for(int i = 0; i < count; i++)
                    sum += System.Math.Abs((double)result[i]);

                return sum;
            }
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Vrací souèet absolutních hodnot prvkù (do zadané hloubky)";
    }
}
