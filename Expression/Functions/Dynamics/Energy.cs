using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Pro zadan� dynamick� syst�m a zadan� sou�adnice a hybnosti spo��t� energii
    /// </summary>
    public class Energy : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 2);

            // Kontrola na dynamick� syst�m
            IDynamicalSystem dynamicalSystem = evaluatedArguments[0] as IDynamicalSystem;
            if(dynamicalSystem == null)
                this.BadTypeError(dynamicalSystem, 0);
            
            // Prohod� argumenty (abychom mohli po��tat energie i pro v�ce po��te�n�ch podm�nek najednou)
            object ea = evaluatedArguments[1];
            evaluatedArguments[1] = evaluatedArguments[0];
            evaluatedArguments[0] = ea;

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is Vector) {
                IDynamicalSystem dynamicalSystem = arguments[1] as IDynamicalSystem;
                return dynamicalSystem.E(item as Vector);
            }
            else if(item is TArray)
                return this.EvaluateArray(depth, item as TArray, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Pro zadan� dynamick� syst�m a zadan� sou�adnice a hybnosti vr�t� energii syst�mu.";
        private const string parameters = "Dynamick� syst�m; sou�adnice a hybnosti (Vector) | Array of Vectors";
    }
}
