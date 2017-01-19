using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates a Poincaré section for given energy or trajectory given by its initial condition
    /// </summary>
    public class Poincare: Fnc {
        public override string Help { get { return Messages.HelpPoincare; } }

        protected override void CreateParameters() {
            this.SetNumParams(9);

            this.SetParam(0, true, true, false, Messages.PDynamicalSystem, Messages.PDynamicalSystemDescription, null, typeof(IDynamicalSystem));
            this.SetParam(1, true, true, true, Messages.P2Poincare, Messages.P2PoincareDescription, null, typeof(Vector), typeof(double));
            this.SetParam(2, true, true, false, Messages.PPSPoints, Messages.PPSPointsDescription, null, typeof(int));
            this.SetParam(3, true, true, false, Messages.P4Poincare, Messages.P4PoincareDescription, null, typeof(Vector));
            this.SetParam(4, true, true, false, Messages.P5Poincare, Messages.P5PoincareDescription, null, typeof(int));
            this.SetParam(5, true, true, false, Messages.P6Poincare, Messages.P6PoincareDescription, null, typeof(int));
            this.SetParam(6, false, true, false, Messages.PRungeKuttaMethod, Messages.PRungeKuttaDescription, "normal", typeof(string));
            this.SetParam(7, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 0.0, typeof(double));
            this.SetParam(8, false, true, false, Messages.POneOrientation, Messages.POneOrientationDescription, false, typeof(bool));
        }

        /// <summary>
        /// Provede výpoèet
        /// </summary>
        protected virtual object Compute(PoincareSection ps, Vector plane, int i1, int i2, Vector ic, int numPoints, bool oneOrientation) {
            return ps.Compute(plane, i1, i2, ic, numPoints, oneOrientation);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IDynamicalSystem dynamicalSystem = arguments[0] as IDynamicalSystem;
            int numPoints = (int)arguments[2];
            Vector plane = (Vector)arguments[3];
            int i1 = (int)arguments[4];
            int i2 = (int)arguments[5];

            RungeKuttaMethods rkMethod = (RungeKuttaMethods)Enum.Parse(typeof(RungeKuttaMethods), (string)arguments[6], true);
            double precision = (double)arguments[7];

            bool oneOrientation = (bool)arguments[8];

            Vector ic;

            // Trajectory is generated randomly
            if(arguments[1] is double) {
                double e = (double)arguments[1];
                ic = dynamicalSystem.IC(e);
            }
            else if(arguments[1] is Vector && (arguments[1] as Vector).Length / 2 == dynamicalSystem.DegreesOfFreedom)
                ic = (Vector)arguments[1];
            else
                return this.BadTypeError(arguments[1], 1);

            PoincareSection ps = new PoincareSection(dynamicalSystem, precision, rkMethod);

            object result;
            int numAttemps = 0;
            while(true) {
                try {
                    result = this.Compute(ps, plane, i1, i2, ic, numPoints, oneOrientation);
                    break;
                }
                catch(PoincareSectionException exc) {
                    if(arguments[1] is double && numAttemps < maxNumAttemps) {
                        double e = (double)arguments[1];
                        ic = dynamicalSystem.IC(e);
                    }
                    else {
                        //                        throw exc;
                        result = new PointVector(0);
                        break;
                    }
                }
                catch(Exception exc) {
                    throw exc;
                }
            }

            return result;
        }

        // Maximální poèet pokusù, než bude vyvolána výjimka
        private const int maxNumAttemps = 20;
    }
}
