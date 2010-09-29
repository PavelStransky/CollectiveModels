using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Time evolution of a given ket
    /// </summary>
    public class TimeEvolution: Fnc {
        public override string Help { get { return Messages.HelpTimeEvolution; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PQuantumSystem, Messages.PQuantumSystemDescription, null, typeof(IQuantumSystem));
            this.SetParam(1, true, true, false, Messages.PKet, Messages.PKetDescription, null, typeof(PointVector), typeof(Vector));
            this.SetParam(2, true, true, true, Messages.PTime, Messages.PTimeDescription, false, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IQuantumSystem system = (IQuantumSystem)arguments[0];

            PointVector state = null;
            if(arguments[1] is Vector) {
                Vector v = (Vector)arguments[1];
                state = new PointVector(v, new Vector(v.Length));
            }
            else
                state = (PointVector)arguments[1];

            double time = (double)arguments[2];

            return system.EigenSystem.TimeEvolution(state, time);
        }
    }
}