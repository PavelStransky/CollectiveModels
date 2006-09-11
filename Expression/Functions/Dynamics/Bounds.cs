using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Pro zadan� dynamick� syst�m a energii ur�� meze (v�t�inou horn� odhad), ve kter�ch se m��e �e�en� pohybovat
    /// </summary>
    public class Bounds : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 2);

            // Kontrola na dynamick� syst�m
            IDynamicalSystem dynamicalSystem = evaluatedArguments[0] as IDynamicalSystem;
            if(dynamicalSystem == null)
                this.BadTypeError(dynamicalSystem, 0);

            this.ConvertInt2Double(evaluatedArguments, 1);

            // Prohod� argumenty (abychom mohli po��tat energie i pro v�ce po��te�n�ch podm�nek najednou)
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

        private const string help = "Pro zadan� dynamick� syst�m a zadanou energii vr�t� meze (horn� odhad), ve kter�ch se �e�en� m��e pohybovat";
        private const string parameters = "Dynamick� syst�m; energie (double) | Array of double";
    }
}
