using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Action of an operator on a given ket
    /// </summary>
    public class OperatorAction: Fnc {
        public override string Help { get { return Messages.HelpOperatorAction; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PDoublePendulum, Messages.PDoublePendulumDescription, null, typeof(QuantumDP));
            this.SetParam(1, true, true, false, Messages.PKet, Messages.PKetDescription, null, typeof(PointVector), typeof(Vector));
            this.SetParam(2, false, true, false, Messages.POperatorType, Messages.POperatorTypeDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            QuantumDP system = (QuantumDP)arguments[0];
            
            PointVector state = null;
            if(arguments[1] is Vector) {
                Vector v = (Vector)arguments[1];
                state = new PointVector(v, new Vector(v.Length));
            }
            else
                state = (PointVector)arguments[1];

            int operatorType = (int)arguments[2];

            return system.OperatorAction(state, operatorType);
        }
    }
}