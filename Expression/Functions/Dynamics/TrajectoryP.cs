using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// For given energy or a trajectory given by its initial condition calculates the trajectory; 
    /// the x, y coordinates of the result is returned by a PointVector
    /// </summary>
    public class TrajectoryP: TrajectoryM {
        public override string Help { get { return Messages.HelpTrajectoryP; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = base.EvaluateFn(guider, arguments) as Matrix;
            return new PointVector(m.GetColumnVector(1), m.GetColumnVector(2));
        }
    }
}
