using System;
using System.Collections;
using System.Numerics;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates the zeros of the partition function of a 2D Spin system
    /// </summary>
    public class SpinZZero : Fnc {
        public override string Help { get { return Messages.HelpSpinZZero; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PIsing2D, Messages.PIsing2DDescription, null, typeof(Ising2D));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Ising2D ising = arguments[0] as Ising2D;
            PointVector u = ising.GetZeros(guider);
            PointVector beta = new PointVector(u.Length);
            PointVector u4 = new PointVector(u.Length);

            for(int i = 0; i < u.Length; i++){
                Complex c = new Complex(u[i].X, u[i].Y);
                Complex d = 1.0 / c;
                d = d * d;
                c = Complex.Log(c);
                c /= 2.0;
                beta[i] = new PointD(c.Real, c.Imaginary);
                u4[i] = new PointD(d.Real, d.Imaginary);
            }

            PointVector[] result = new PointVector[3];
            result[0] = u;
            result[1] = beta;
            result[2] = u4;
            return (TArray)result;
        }
    }
}