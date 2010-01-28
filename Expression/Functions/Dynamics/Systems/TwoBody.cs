using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates TwoBody class
    /// </summary>
    public class FnTwoBody: Fnc {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpCGCM; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, false, true, false, Messages.PMasses, Messages.PMassesDescription, new Vector(0), typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector m = arguments[0] as Vector;
            if(m.Length != 2)
                throw new FncException(this, string.Format(Messages.EMBadPMasses, 2, m.Length));
            return new TwoBody(m);
        }

        private const string name = "twobody";
    }
}