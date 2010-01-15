using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Po��t� trajektorii dynamick�ho syst�mu
    /// </summary>
    public class Trajectory {
        private RungeKutta rungeKutta;
        private IDynamicalSystem dynamicalSystem;
        private double precision;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamick� syst�m</param>
        /// <param name="precision">P�esnost v�sledku</param>
        /// <param name="rkMethod">Metoda k v�po�tu RK</param>
        public Trajectory(IDynamicalSystem dynamicalSystem, double precision, RungeKuttaMethods rkMethod) {
            this.dynamicalSystem = dynamicalSystem;
            this.precision = precision;
            this.rungeKutta = RungeKutta.CreateRungeKutta(dynamicalSystem, precision, rkMethod);
        }

        /// <summary>
        /// Spo��t� trajektorii a vr�t� ji v matici o sloupc�ch
        /// T - �as
        /// x, y, ...
        /// vx, vy, ...
        /// </summary>
        /// <param name="initialX">Po��te�n� podm�nky (sou�adnice, rychlosti)</param>
        /// <param name="time">Doba, po kterou bude trajektorie po��t�na</param>
        /// <param name="timeStep">�asov� krok v�sledku</param>
        public Matrix Compute(Vector initialX, double time, double timeStep) {
            // Po�et prom�nn�ch
            int length = initialX.Length;
            double t = 0.0;
            double tNext = timeStep;

            Vector x = initialX;

            ArrayList trajectory = new ArrayList();
            ArrayList tt = new ArrayList();

            double step = this.precision;
            this.rungeKutta.Init(initialX);

            do {
                while(t < tNext) {
                    double newStep;
                    x += this.rungeKutta.Step(x, ref step, out newStep);
                    t += step;

                    step = System.Math.Min(newStep, tNext);
                }

                trajectory.Add(x);
                tt.Add(t);
                tNext += timeStep;
            } while(t < time);

            int count = trajectory.Count;

            // Transformace �asu
            Vector tv = new Vector(count);
            int j = 0;
            foreach(double d in tt)
                tv[j++] = d;

            // Transformace dat
            Matrix result = new Matrix(count, length + 1);
            j = 0;
            foreach(Vector v in trajectory) {
                for(int k = 0; k < length; k++)
                    result[j, k + 1] = v[k];
                result[j, 0] = tv[j];
                j++;
            }

            return result;
        }

        /// <summary>
        /// Spo��t� d�lku trajektorie v z�vislosti na �ase
        /// </summary>
        /// <param name="initialX">Po��te�n� podm�nky (sou�adnice, rychlosti)</param>
        /// <param name="time">Doba, po kterou bude trajektorie po��t�na</param>
        /// <param name="timeStep">�asov� krok v�sledku</param>
        public PointVector Length(Vector initialX, double time, double timeStep) {
            // Po�et prom�nn�ch
            int length = initialX.Length;
            double t = 0.0;
            double tNext = 0.0;

            Vector x = initialX;

            ArrayList data = new ArrayList();

            double step = this.precision;
            this.rungeKutta.Init(initialX);

            double totalLength = 0.0;
            do {
                while(t < tNext) {
                    double newStep;
                    Vector newX = this.rungeKutta.Step(x, ref step, out newStep);

                    double l = 0.0;
                    for(int i = 0; i < length / 2; i++)
                        l += newX[i] * newX[i];
                    
                    totalLength += System.Math.Sqrt(l);

                    t += step;
                    step = System.Math.Min(newStep, tNext);
                }

                data.Add(new PointD(t, totalLength));
                tNext += timeStep;
            } while(t < time);

            int count = data.Count;

            // Transformace dat
            PointVector result = new PointVector(count);
            int j = 0;
            foreach(PointD p in data)
                result[j++] = p;

            return result;
        }
    }
}
