using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Copies a given object i-times and returns as TArray
    /// </summary>
    public class Copy : Fnc {
        public override string Help { get { return Messages.HelpCopy; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PObject, Messages.PObjectDescription, null);
            this.SetParam(1, true, true, false, Messages.PNumberOfPoints, Messages.PNumberOfPointsDetail, 1, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object o = arguments[0];
            int length = (int)arguments[1];

            List result = new List();
            for(int i = 0; i < length; i++)
                result.Add(o);

            return result.ToTArray();
        }
    }
}
