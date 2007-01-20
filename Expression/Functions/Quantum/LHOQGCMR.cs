using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vytvoøí LHOQuantumGCMR tøídu (poèítanou v radiálních souøadnicích)
    /// </summary>
    public class LHOQGCMR : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 1);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 8);

            if(evaluatedArguments[0] is int)
                evaluatedArguments[0] = (double)(int)evaluatedArguments[0];

            if(evaluatedArguments.Count > 1) {
                this.CheckArgumentsMinNumber(evaluatedArguments, 4);

                this.ConvertInt2Double(evaluatedArguments, 1);
                this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));

                this.ConvertInt2Double(evaluatedArguments, 2);
                this.CheckArgumentsType(evaluatedArguments, 2, typeof(double));

                this.ConvertInt2Double(evaluatedArguments, 3);
                this.CheckArgumentsType(evaluatedArguments, 3, typeof(double));
            }

            if(evaluatedArguments.Count > 4) {
                this.ConvertInt2Double(evaluatedArguments, 4);
                this.CheckArgumentsType(evaluatedArguments, 4, typeof(double));
            }

            if(evaluatedArguments.Count > 5) {
                this.ConvertInt2Double(evaluatedArguments, 5);
                this.CheckArgumentsType(evaluatedArguments, 5, typeof(double));
            }

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is double) {
                double b = 1;
                double c = 1;
                double k = 1;
                double a0 = (double)item;

                double hbar = 0.1;      // Default hodnota Planckovy konstanty

                if(arguments.Count > 1) {
                    b = (double)arguments[1];
                    c = (double)arguments[2];
                    k = (double)arguments[3];
                }

                if(arguments.Count > 4)
                    a0 = (double)arguments[4];

                if(arguments.Count > 5)
                    hbar = (double)arguments[5];

                LHOQuantumGCMR qgcm = new LHOQuantumGCMR((double)item, b, c, k, a0, hbar);
                return qgcm;
            }

            else if(item is TArray)
                return this.EvaluateArray(depth, item as TArray, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Vytvoøí LHOQuantumGCMR tøídu pro dané parametry";
        private const string parameters = "A (double) | Array of A (double); [B (double); C (double); K (double); [A0 (double); [hbar (double)]]";
    }
}