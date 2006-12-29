using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vypoèítá ekvipotenciální plochu GCM pro danou energii
    /// </summary>
    public class Equipotential: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 2);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 3);

            this.ConvertInt2Double(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));

            if(evaluatedArguments.Count > 2)
                this.CheckArgumentsType(evaluatedArguments, 2, typeof(int));

            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is PavelStransky.GCM.GCM) {
                double e = (double)arguments[1];
                PavelStransky.GCM.GCM gcm = item as PavelStransky.GCM.GCM;

                PointVector[] equipotentials;
                if(arguments.Count > 2)
                    equipotentials = gcm.EquipotentialContours(e, (int)arguments[2]);
                else
                    equipotentials = gcm.EquipotentialContours(e);

                if(equipotentials.Length == 1)
                    return equipotentials[0];
                else {
                    TArray result = new TArray();
                    for(int i = 0; i < equipotentials.Length; i++)
                        result.Add(equipotentials[i]);
                    return result;
                }
            }
            else if(item is TArray)
                return this.EvaluateArray(depth, item as TArray, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Vypoèítá ekvipotenciální plochu GCM pro zadanou energii (energie)";
        private const string parameters = "GCM; energie (double) [; poèet bodù køivky (int)]";
    }
}
