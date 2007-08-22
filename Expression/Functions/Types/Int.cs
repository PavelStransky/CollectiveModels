using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Converts given value to an integer number
    /// </summary>
    public class FnInt : Fnc {
        public override string Help { get { return Messages.HelpInt; } }
        public override string Name { get { return name; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PValue, Messages.PValueDescription, null,
                typeof(int), typeof(double), typeof(string), typeof(TimeSpan));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is double)
                return (int)(double)item;
            else if(item is TimeSpan)
                return (int)((TimeSpan)item).TotalSeconds;
            else if(item is string)
                return int.Parse(item as string);
            else
                return item;
        }

        private const string name = "int";
    }
}
