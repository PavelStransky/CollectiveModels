using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Math {
    /// <summary>
    /// Vypo��t� Lyapunovovy exponenty
    /// </summary>
    public class Lyapunov {
        // P�esnost v�po�tu trajektorie
        protected double precisionT;
        protected RungeKutta rungeKuttaT;

        // P�esnost v�po�tu deviac�
        protected double precisionW;
        protected RungeKutta rungeKuttaW;

        protected IDynamicalSystem dynamicalSystem;

        // Aktu�ln� sou�adnice a rychlosti
        private Vector x;
        // Jen pro zrychlen�
        private Matrix jacobian;
        private bool xChanged = true;

        /// <summary>
        /// Aktu�ln� X
        /// </summary>
        protected Vector X { get { return this.x; } set { this.x = value; xChanged = true; } }

        // Aktu�ln� odchylky
        protected Vector[] w;

        /// <summary>
        /// Rovnice pro v�po�et deviace
        /// </summary>
        /// <param name="w">Vektor deviace</param>
        protected Vector DeviationEquation(Vector w) {
            if(xChanged) {
                jacobian = this.dynamicalSystem.Jacobian(this.X);
                xChanged = false;
            }

            return jacobian * w;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamick� syst�m</param>
        public Lyapunov(IDynamicalSystem dynamicalSystem, double precision, RungeKuttaMethods rkMethod)
            : this(dynamicalSystem, precision, rkMethod, precision, rkMethod) {
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamick� syst�m</param>
        public Lyapunov(IDynamicalSystem dynamicalSystem, double precisionT, RungeKuttaMethods rkMethodT, double precisionW, RungeKuttaMethods rkMethodW) {
            this.rungeKuttaW = RungeKutta.CreateRungeKutta(this.DeviationEquation, precisionW, rkMethodW);
            this.rungeKuttaT = RungeKutta.CreateRungeKutta(dynamicalSystem, precisionT, rkMethodT);
            this.precisionW = precisionW > 0.0 ? precisionW : this.rungeKuttaW.Precision;
            this.precisionT = precisionT > 0.0 ? precisionT : this.rungeKuttaT.Precision;

            this.dynamicalSystem = dynamicalSystem;
        }

        /// <summary>
        /// Inicializuje v�po�et
        /// </summary>
        /// <param name="initialX">Po��te�n� podm�nky</param>
        protected void Init(Vector initialX) {
            this.X = initialX;
            int length = initialX.Length;

            this.w = new Vector[length];
            for(int i = 0; i < length; i++) {
                this.w[i] = new Vector(length);
                this.w[i][i] = 1.0;
            }

            this.rungeKuttaT.Init(initialX);

            if(this.rungeKuttaW is RungeKuttaAdaptive) {
                Vector scale = new Vector(length);
                for(int i = 0; i < length; i++)
                    scale[i] = 1.0;
                (this.rungeKuttaW as RungeKuttaAdaptive).SetScale(scale, true);
            }
        }

        /// <summary>
        /// Jeden krok v�po�tu
        /// </summary>
        /// <param name="step">Krok</param>
        /// <returns>Nov� krok</returns>
        protected double Step(ref double step) {
            double newStep;
            int length = this.x.Length;
            double[] oldStepW = new double[length];
            Vector[] addW = new Vector[length];

            Vector addX = this.rungeKuttaT.Step(this.X, ref step, out newStep);

            double tStep;
            double minStep = step;
            double minNewStep = newStep;
            for(int i = 0; i < length; i++) {
                oldStepW[i] = step;
                addW[i] = this.rungeKuttaW.Step(this.w[i], ref step, out tStep);
                minStep = System.Math.Min(minStep, step);
                minNewStep = System.Math.Min(minNewStep, tStep);
            }

            if(step != minStep) {
                addX = this.rungeKuttaT.Step(this.X, ref step, out newStep);

                minNewStep = newStep;
                for(int i = 0; i < length; i++) {
                    addW[i] = this.rungeKuttaW.Step(this.w[i], ref step, out tStep);
                    minNewStep = System.Math.Min(minNewStep, tStep);
                }
            }

            this.X += addX;

            for(int i = 0; i < length; i++)
                this.w[i] += addW[i];

            return minNewStep;
        }

        /// <summary>
        /// Lyapunov�v exponent v �ase t
        /// </summary>
        /// <param name="t">�as t</param>
        public Vector LyapunovExponent(double t) {
            int length = this.w.Length;
            Vector result = new Vector(length);

            if(t == 0.0)
                t = 1.0;

            for(int i = 0; i < length; i++)
                result[i] = System.Math.Log(this.w[i].EuklideanNorm()) / t;

            return result;
        }

        /// <summary>
        /// Objemy v �ase t
        /// </summary>
        /// <returns></returns>
        public Vector Volumes() {
            GramSchmidt gs = new GramSchmidt(this.w);
            Vector result = gs.Norms();
            int length = result.Length;

            for(int i = 1; i < length; i++)
                result[i] *= result[i - 1];

            return result;
        }

        /// <summary>
        /// Lyapunovovy exponenty v �ase t
        /// </summary>
        /// <param name="t">�as t</param>
        public Vector LyapunovExponents(double t) {
            Vector volumes = this.Volumes();
            int length = volumes.Length;

            if(t == 0.0)
                t = 1.0;

            Vector result = new Vector(length);
            for(int i = 0; i < length; i++)
                result[i] = System.Math.Log(volumes[i]) / t;

            for(int i = 1; i < length; i++)
                result[i] -= result[i - 1];

            return result;
        }

        /// <summary>
        /// Vypo��t� z�vislost exponent� na �ase
        /// </summary>
        /// <param name="initialX">Po��te�n� podm�nky</param>
        /// <param name="time">�as</param>
        /// <returns>Seznam s prvky {Lyapunov exponent pro v�echny dostupn� dimenze, Objemy v jednotliv�ch dimenz�ch,
        /// v�echny Lyapunovovy exponenty</returns>
        public ArrayList TimeDependence(Vector initialX, double time, double timeStep) {
            int length = initialX.Length;
            ArrayList lyapunov = new ArrayList();
            ArrayList volumes = new ArrayList();
            ArrayList lyapunovs = new ArrayList();
            ArrayList times = new ArrayList();

            double step = System.Math.Min(this.precisionT, this.precisionW);
            double t = 0.0;
            double tNext = 0.0;

            this.Init(initialX);

            do {
                while(t < tNext) {
                    double newStep = this.Step(ref step);
                    t += step;
                    step = newStep;
                }

                Vector v = new Vector(length + 1);
                v[0] = t;

                times.Add(t);
                lyapunov.Add(this.LyapunovExponent(t));
                volumes.Add(this.Volumes());
                lyapunovs.Add(this.LyapunovExponents(t));

                tNext += timeStep;
            } while(t < time);

            // P�evod v�sledku na �adu
            PointVector[] result1 = new PointVector[length];
            PointVector[] result2 = new PointVector[length];
            PointVector[] result3 = new PointVector[length];

            int count = times.Count;
            for(int i = 0; i < length; i++) {
                result1[i] = new PointVector(count);
                result2[i] = new PointVector(count);
                result3[i] = new PointVector(count);
            }

            for(int i = 0; i < count; i++) {
                t = (double)times[i];
                Vector l = (Vector)lyapunov[i];
                Vector v = (Vector)volumes[i];
                Vector ls = (Vector)lyapunovs[i];

                for(int j = 0; j < length; j++){
                    result1[j][i] = new PointD(t, l[j]);
                    result2[j][i] = new PointD(t, v[j]);
                    result3[j][i] = new PointD(t, ls[j]);
                }
            }

            ArrayList result = new ArrayList();
            result.Add(result1);
            result.Add(result2);
            result.Add(result3);
            return result;
        }
    }
}
