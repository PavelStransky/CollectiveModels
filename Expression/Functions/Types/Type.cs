using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Type of the value
	/// </summary>
	public class FnType: Fnc {
		public override string Name {get {return name;}}
		public override string Help {get {return Messages.HelpType;}}

        protected override void CreateParameters() {
            this.SetNumParams(1);

            this.SetParam(0, true, true, false, Messages.PValue, Messages.PValueDescription, null);
        }

		protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is int)
                return "int";
            else if(item is TArray)
                return "array";
            else
                return item.GetType().Name.ToLower();
		}

		private const string name = "type";
	}

}
