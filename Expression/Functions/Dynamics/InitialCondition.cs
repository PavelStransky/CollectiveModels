using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Nageneruje náhodné poèáteèní podmínky pro jednu trajektorii se zadanou energií
    /// </summary>
    public class InitialCondition: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 2);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 3);

            this.ConvertInt2Double(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));

            if(evaluatedArguments.Count > 2) {
                this.ConvertInt2Double(evaluatedArguments, 2);
                this.CheckArgumentsType(evaluatedArguments, 2, typeof(double));
            }

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item as IDynamicalSystem != null) {
                IDynamicalSystem dynamicalSystem = item as IDynamicalSystem;

                double e = (double)arguments[1];

                if(arguments.Count > 2) {
                    double l = (double)arguments[2];
                    return dynamicalSystem.IC(e, l);
                }
                else
                    return dynamicalSystem.IC(e);
            }
            else if(item is Array)
                return this.EvaluateArray(depth, item as Array, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Nageneruje pro danou energii jednu poèáteèní podmínku a vrátí ji jako vektor (x, y, vx, vy)";
        private const string parameters = "Dynamický systém; energie (double) [; úhlový moment]";
    }
}
