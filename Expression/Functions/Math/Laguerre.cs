using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vrac� hodnotu Laguerrova polynomu
    /// </summary>
    public class Laguerre : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        private int n, m;

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 2);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 3);

            this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));
            this.CheckArgumentsType(evaluatedArguments, 2, typeof(int));
        }

        /// <summary>
        /// Vypo��t� logaritmus o z�kladu logBase
        /// </summary>
        private double ComputeLaguerre(double x) {
            return SpecialFunctions.Laguerre(this.n, this.m, x);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            this.n = (int)arguments[1];

            if(arguments.Count > 2)
                this.m = (int)arguments[2];
            else
                this.m = 0;

            object item = arguments[0];

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
            else 
                return (item as Matrix).Transform(this.ComputeLaguerre);
        }

        private const string help = "Vrac� hodnotu Laguerrovy funkce, u bodu nebo vektoru bod� po��t� pouze u Y slo�ky";
        private const string parameters = "int | double | Point | Vector | PointVector | Matrix | Array; ��d polynomu [; stupe� polynomu]";
    }
}
