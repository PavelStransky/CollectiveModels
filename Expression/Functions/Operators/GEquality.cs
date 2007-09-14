using System.Collections;
using System.Text;
using System;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Operator of inequality (greater or equal)
    /// </summary>
    public class GEquality: Operator {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpGEquality; } }
        public override OperatorPriority Priority { get { return OperatorPriority.ComparePriority; } }

        protected override void CreateParameters() {
            this.SetNumParams(1, true);
            this.SetParam(0, true, true, false, Messages.PValue, Messages.PValueDescription, null,
                typeof(int), typeof(double));

            this.AddCompatibility(typeof(int), typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int[] lengths = this.GetTypesLength(arguments, false);
            int count = arguments.Count;

            object item = arguments[0];

            if(lengths[0] > 0 && lengths[2] > 0) {  // Double i int hodnoty
                for(int k = 1; k < count; k++) {
                    item = arguments[k - 1];
                    double d = (item is int) ? (int)item : (double)item;

                    item = arguments[k];
                    if(item is double) {
                        if(d < (double)item)
                            return false;
                    }
                    else if(item is int) {
                        if(d < (int)item)
                            return false;
                    }
                }
                return true;
            }

            else if(item is int) {
                for(int k = 1; k < count; k++) {
                    int i = (int)arguments[k - 1];
                    if(i < (int)arguments[k])
                        return false;
                }
                return true;
            }

            else if(item is double) {
                for(int k = 1; k < count; k++) {
                    double d = (double)arguments[k - 1];
                    if(d < (double)arguments[k])
                        return false;
                }
                return true;
            }

            return null;
        }

        private const string name = ">=";
    }
}
