using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Definice funkce, kter� hled� nejmen�� / nejv�t�� hodnotu
	/// </summary>
	public abstract class FncMinMax: Fnc {
        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PValue, Messages.PValueDescription, null, typeof(Vector), typeof(Matrix));
        }
	}
}
