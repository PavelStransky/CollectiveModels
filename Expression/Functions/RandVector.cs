using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vrací vektor zadané délky s náhodnými èísly
    /// </summary>
    public class RandVector: FunctionDefinition {
        private Random random = new Random();

        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 1);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 3);

            this.CheckArgumentsType(evaluatedArguments, 0, typeof(int));

            if(evaluatedArguments.Count > 1) {
                this.CheckArgumentsNumber(evaluatedArguments, 3);
                this.ConvertInt2Double(evaluatedArguments, 1);
                this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));
                this.ConvertInt2Double(evaluatedArguments, 2);
                this.CheckArgumentsType(evaluatedArguments, 2, typeof(double));
            }

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            int length = (int)arguments[0];

            double min = 0.0;
            double max = 1.0;

            if(arguments.Count > 1) {
                min = (double)arguments[1];
                max = (double)arguments[2];
            }

            Vector result = new Vector(length);
            for(int i = 0; i < length; i++)
                result[i] = this.random.NextDouble() * (max - min) + min;

            return result;
        }

        private const string help = "Vrací náhodný vektor";
        private const string parameters = "délka vektoru (int) [; minimální mez (double); maximální mez (double)]";
    }
}
