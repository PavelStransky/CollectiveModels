using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vr�t� �adu s n�zvy v�ech objekt� na kontextu
	/// </summary>
	public class Objects: FunctionDefinition {
		public override string Help {get {return help;}}

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsNumber(arguments, 0);
			return context.ObjectNames();
		}

		private const string help = "Vr�t� �adu v�ech objekt� na kontextu";
	}
}
