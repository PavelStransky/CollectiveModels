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

        protected override object CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 2);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 3);

            this.ConvertInt2Double(evaluatedArguments, 1);

            this.CheckArgumentsType(evaluatedArguments, 0, typeof(PavelStransky.GCM.GCM));
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));
            this.CheckArgumentsType(evaluatedArguments, 2, typeof(int));
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            PavelStransky.GCM.GCM gcm = arguments[0] as PavelStransky.GCM.GCM;
            double e = (double)arguments[1];

            PointVector[] equipotentials;
            if(arguments.Count > 2)
                equipotentials = gcm.EquipotentialContours(e, (int)arguments[2]);
            else
                equipotentials = gcm.EquipotentialContours(e);

            int length = equipotentials.Length;
            TArray result = new TArray(typeof(PointVector), length);
            for(int i = 0; i < length; i++)
                result[i] = equipotentials[i];
            return result;
        }

        private const string help = "Vypoèítá ekvipotenciální plochu GCM pro zadanou energii";
        private const string parameters = "GCM; energie (double) [; poèet bodù køivky (int)]";
    }
}
