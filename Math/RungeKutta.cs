using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    public class RungeKutta {
        // Prav� strana rovnice
        protected VectorFunction equation;

        protected Vector[] x;
        protected Vector time;
        protected double precision;

        public double Precision { get { return this.precision; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="equation">Rovnice</param>
        /// <param name="precision">P�esnost v�po�tu</param>
        public RungeKutta(VectorFunction equation, double precision) {
            this.equation = equation;
            this.precision = (precision == 0.0 ? defaultPrecision : precision);
        }

        /// <summary>
        /// Zm�n� d�lku pole v�sledku
        /// </summary>
        /// <param name="numPoints">Nov� po�et bod�</param>
        protected void ChangeLength(int numPoints) {
            Vector[] newx = new Vector[numPoints];
            this.time.Length = numPoints;

            numPoints = System.Math.Min(this.x.Length, numPoints);
            for(int i = 0; i < numPoints; i++)
                newx[i] = this.x[i];

            this.x = newx;
        }

        /// <summary>
        /// Jeden krok v�po�tu
        /// </summary>
        /// <param name="x">Vektor v �ase x</param>
        /// <param name="step">Krok</param>
        /// <param name="newStep">Nov� krok</param>
        /// <returns>Vypo��tan� p��r�stek</returns>
        public virtual Vector Step(Vector x, ref double step, out double newStep) {
            Vector rightSide1 = this.equation(x);
            Vector rightSide2 = this.equation(x + 0.5 * step * rightSide1);
            Vector rightSide3 = this.equation(x + 0.5 * step * rightSide2);
            Vector rightSide4 = this.equation(x + step * rightSide3);

            newStep = step;
            return (rightSide1 / 6.0 + rightSide2 / 3.0 + rightSide3 / 3.0 + rightSide4 / 6.0) * step;
        }

        /// <summary>
        /// �e�� rovnici s po��te�n�mi podm�nkami po �as time
        /// </summary>
        /// <param name="t">Doba v�po�tu</param>
        /// <param name="initialX">Po��te�n� podm�nky</param>
        /// <param name="precision">P�esnost v�po�tu</param>
        public virtual void Solve(Vector initialX, double t) {
            // Po�et prom�nn�ch
            int length = initialX.Length;
            int numPoints = initialNumPoints;

            this.x = new Vector[numPoints];
            this.x[0] = initialX;

            this.time = new Vector(numPoints);
            this.time[0] = 0;

            double step = this.precision;
            int i = 0;

            while(this.time[i] < t) {
                if(i + 1 >= numPoints) {
                    numPoints = numPoints * 3 / 2;
                    this.ChangeLength(numPoints);
                }

                double newStep;
                this.x[i + 1] = this.x[i] + this.Step(this.x[i], ref step, out newStep);
                this.time[i + 1] = this.time[i] + step;

                i++;

                step = newStep;
            }

            this.ChangeLength(i);
        }

        /// <summary>
        /// Vr�t� v�sledky v matici o sloupc�ch time, x, vx
        /// </summary>
        /// <param name="timeStep">�asov� krok v�sledku</param>
        public Matrix GetMatrix(double timeStep) {
            int length = this.x[0].Length;
            double interval = this.time[this.time.Length - 1] - this.time[0];
            int numPoints = (int)(interval / timeStep) + 1;

            Matrix result = new Matrix(numPoints, 2 * length + 1);

            for(int i = 0; i < this.time.Length; i++) {
                int index = (int)((this.time[i] - this.time[0]) / timeStep);
                result[index, 0] = this.time[i];

                for(int j = 0; j < length; j++)
                    result[index, j + 1] = this.x[i][j];
            }

            return result;
        }

        /// <summary>
        /// Vr�t� v�sledky v matici o sloupc�ch time, x, vx
        /// </summary>
        public Matrix GetMatrix() {
            int length = this.x[0].Length;
            int numPoints = this.time.Length;
            Matrix result = new Matrix(numPoints, 2 * length + 1);

            for(int i = 0; i < numPoints; i++) {
                result[i, 0] = this.time[i];

                for(int j = 0; j < length; j++)
                    result[i, j + 1] = this.x[i][j];
            }

            return result;
        }

        private const double defaultPrecision = 1E-4;
        private const int initialNumPoints = 10000;
    }
}