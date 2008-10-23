using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.ManyBody;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates ThreeBody class
    /// </summary>
    public class FnThreeBody: Fnc {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpThreeBody; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, false, true, false, Messages.PMasses, Messages.PMassesDescription, new Vector(0), typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector m = arguments[0] as Vector;
            if(m.Length != 3)
                throw new FncException(string.Format(Messages.EMBadPMasses, 3, m.Length));
            return new ThreeBody(m);
        }

        private const string name = "threebody";
    }
}