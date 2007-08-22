using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Converts given value to a double precision number
    /// </summary>
    public class FnDouble : Fnc {
        public override string Help { get { return Messages.HelpDouble; } }
        public override string Name { get { return name; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, true, Messages.PValue, Messages.PValueDescription, null,
                typeof(double), typeof(string), typeof(TimeSpan));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is TimeSpan)
                return ((TimeSpan)item).TotalSeconds;
            else if(item is string)
                return double.Parse(item as string);
            else
                return item;
        }

        private const string name = "double";
    }
}
