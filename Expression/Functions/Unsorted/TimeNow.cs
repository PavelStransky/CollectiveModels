using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Current time
    /// </summary>
    public class TimeNow : Fnc {
        public override string Help { get { return Messages.HelpTimeNow; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return DateTime.Now;
        }
    }
}
