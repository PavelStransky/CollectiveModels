using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vytvoøí LHOQuantumGCMRFull tøídu (poèítanou v radiálních souøadnicích, bereme úplnou bázi)
    /// </summary>
    public class LHOQGCMRF : FunctionDefinition {
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

            if(evaluatedArguments.Count > 4)
                this.CheckArgumentsType(evaluatedArguments, 4, typeof(int));

            if(evaluatedArguments.Count > 5) {
                this.ConvertInt2Double(evaluatedArguments, 5);
                this.CheckArgumentsType(evaluatedArguments, 5, typeof(double));
            }

            if(evaluatedArguments.Count > 6) {
                this.ConvertInt2Double(evaluatedArguments, 6);
                this.CheckArgumentsType(evaluatedArguments, 6, typeof(double));
            }

            if(evaluatedArguments.Count > 7)
                this.CheckArgumentsType(evaluatedArguments, 7, typeof(int));

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is double) {
                double b = 1;
                double c = 1;
                double k = 1;
                double a0 = (double)item;

                int maxn = 25;
                int numSteps = 0;               // 0 - numsteps je dopoèítáno automaticky
                double hbar = 0.1;                 // defaultní hodnota

                if(arguments.Count > 1) {
                    b = (double)arguments[1];
                    c = (double)arguments[2];
                    k = (double)arguments[3];
                }

                if(arguments.Count > 4)
                    maxn = (int)arguments[4];

                if(arguments.Count > 5)
                    hbar = (double)arguments[5];

                if(arguments.Count > 6)
                    a0 = (double)arguments[6];

                if(arguments.Count > 7)
                    numSteps = (int)arguments[7];

                LHOQuantumGCMRFull qgcm = new LHOQuantumGCMRFull((double)item, b, c, k, a0, hbar);
                qgcm.Compute(maxn, numSteps, this.writer);
                return qgcm;
            }

            else if(item is TArray)
                return this.EvaluateArray(depth, item as TArray, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Vytvoøí LHOQuantumGCMRFull tøídu pro dané parametry (GCM poèítáno v radiálních souøadnicích, bereme úplnou bázi)";
        private const string parameters = "A (double) | Array of A (double); [B (double); C (double); K (double); [MaxN (int); [hbar (double); [A0 (double); [NumSteps - dìlení møíže (int)]]]]]";
    }
}