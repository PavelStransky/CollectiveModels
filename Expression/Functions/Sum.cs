using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vys��t� prvky
    /// </summary>
    public class Sum : FunctionDefinitionMinMax {
        public override string Help { get { return help; } }

        protected override object EvaluateGoodDepth(int depth, object item, ArrayList arguments) {
            if(item is int)
                return (double)(int)item;
            else if(item is double)
                return item;
            else if(item is Vector)
                return (item as Vector).Sum();
            else if(item is Matrix)
                return (item as Matrix).Sum();
            else if(item is Array) {
                Array result = this.EvaluateArray(depth, item as Array, arguments);
                this.CheckResultLength(result, depth);

                double sum = 0.0;
                int count = result.Count;

                for(int i = 0; i < count; i++)
                    sum += (double)result[i];

                return sum;
            }
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Vrac� sou�et prvk� (do zadan� hloubky)";
    }
}
