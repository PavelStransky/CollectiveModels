using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Po��t� trajektorii dynamick�ho syst�mu
    /// </summary>
    public class Trajectory: DynamicalSystem {
        private double timeStep;
        private RungeKutta rk;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamick� syst�m</param>
        /// <param name="precision">P�esnost v�sledku</param>
        /// <param name="timeStep">�asov� krok v�sledku</param>
        /// <param name="rkMethod">Metoda k v�po�tu RK</param>
        public Trajectory(IDynamicalSystem dynamicalSystem, double precision, double timeStep, RungeKuttaMethods rkMethod)
            : base(dynamicalSystem) {

            this.timeStep = timeStep;
            this.rk = this.CreateRungeKutta(precision, rkMethod);
        }

        /// <summary>
        /// Spo��t� trajektorii a vr�t� ji v matici o sloupc�ch
        /// T - �as
        /// x, y, ...
        /// vx, vy, ...
        /// </summary>
        /// <param name="ic">Po��te�n� podm�nky (sou�adnice, rychlosti)</param>
        /// <param name="time">Doba, po kterou bude trajektorie po��t�na</param>
        public Matrix Compute(Vector x, double time) {
            this.rk.Solve(x, time);

            Matrix m;

            if(this.timeStep == 0)
                m = this.rk.GetMatrix();
            else
                m = this.rk.GetMatrix(timeStep);

            for(int i = 0; i < m.LengthX; i += 100) {
                Vector y = new Vector(m.LengthY - 1);
                for(int j = 1; j < m.LengthY; j++)
                    y[j - 1] = m[i, j];

                Console.WriteLine("Time = {0}\tEnergy = {1}", m[i, 0], this.dynamicalSystem.E(y));
            }

            return m;
        }
    }
}
