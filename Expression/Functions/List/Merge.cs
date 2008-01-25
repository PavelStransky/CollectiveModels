using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Merges lists into one list
    /// </summary>
    public class Merge: Fnc {
        public override string Help { get { return Messages.HelpMerge; } }

        protected override void CreateParameters() {
            this.SetNumParams(1, true);
            this.SetParam(0, false, true, false, Messages.P1Merge, Messages.P1MergeDescription, null, typeof(List));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            List result = new List();

            foreach(List list in arguments)
                result.AddRange(list);

            return result;
        }
    }
}
