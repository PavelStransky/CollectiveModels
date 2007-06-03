using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Z bodu nebo vektoru bodù vybere souøadnice Y
    /// </summary>
    public class GetY: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(PointVector), typeof(PointD));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            if(arguments[0] is PointVector)
                return (arguments[0] as PointVector).VectorY;
            else
                return (arguments[0] as PointD).Y;
        }

        private const string help = "Z bodu nebo vektoru bodù vybere souøadnice Y";
        private const string parameters = "PointD | PointVector";
    }
}
