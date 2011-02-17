using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates values of specified function and returns it as pointvector
    /// </summary>
    public class FnVal: Fnc {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpTime; } }

        protected override object Evaluate(Guider guider, ArrayList arguments) {
            this.CheckArgumentsNumber(arguments, 1);

            DateTime startTime = DateTime.Now;
            base.Evaluate(guider, arguments);
            return DateTime.Now - startTime;
        }

        private const string name = "fnval";
    }
}
