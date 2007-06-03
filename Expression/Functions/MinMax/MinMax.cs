using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Definice funkce, která hledá nejmenší / nejvìtší hodnotu
	/// </summary>
	public abstract class MinMax: FunctionDefinition {
        protected override void CreateParameters() {
            this.NumParams(1);
            this.SetParam(0, true, true, false, Messages.PValue, Messages.PValueDescription, null, typeof(Vector), typeof(Matrix));
        }
	}
}
