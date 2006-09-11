using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Pro zadaný dynamický systém a energii urèí meze (vìtšinou horní odhad), ve kterých se mùže øešení pohybovat
    /// </summary>
    public class Bounds : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 2);

            // Kontrola na dynamický systém
            IDynamicalSystem dynamicalSystem = evaluatedArguments[0] as IDynamicalSystem;
            if(dynamicalSystem == null)
                this.BadTypeError(dynamicalSystem, 0);

            this.ConvertInt2Double(evaluatedArguments, 1);

            // Prohodí argumenty (abychom mohli poèítat energie i pro více poèáteèních podmínek najednou)
            object ea = evaluatedArguments[1];
            evaluatedArguments[1] = evaluatedArguments[0];
            evaluatedArguments[0] = ea;

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is double) {
                IDynamicalSystem dynamicalSystem = arguments[1] as IDynamicalSystem;
                return dynamicalSystem.Bounds((double)item);
            }
            else if(item is Array)
                return this.EvaluateArray(depth, item as Array, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Pro zadaný dynamický systém a zadanou energii vrátí meze (horní odhad), ve kterých se øešení mùže pohybovat";
        private const string parameters = "Dynamický systém; energie (double) | Array of double";
    }
}
