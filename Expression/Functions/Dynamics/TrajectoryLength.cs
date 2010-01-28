using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// For given energy or a trajectory given by its initial condition calculates the length of the trajectory
    /// </summary>
    public class TrajectoryLength: TrajectoryM {
        public override string Help { get { return Messages.HelpTrajectoryLength; } }

        protected override object Result(Trajectory trajectory, Vector ic, double t, double timeStep) {
            return trajectory.Length(ic, t, timeStep);
        }
    }
}
