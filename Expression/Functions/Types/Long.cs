using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Converts given value to a long integer number
    /// </summary>
    public class FnLong: Fnc {
        public override string Help { get { return Messages.HelpLong; } }
        public override string Name { get { return name; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PValue, Messages.PValueDescription, null,
                typeof(int), typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is int)
                return new LongNumber((int)item);
            else if(item is string)
                return LongNumber.Parse(item as string);

            return null;
        }

        private const string name = "long";
    }
}
