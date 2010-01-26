using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Math {
    /// <summary>
    /// Ur��, zda zadan� trajektorie je regul�rn� nebo chaotick� na z�klad� metody SALI
    /// (Smaller Alignment Index)
    /// </summary>
    public class SALI {
        // P�esnost v�po�tu trajektorie
        protected double precisionT;
        protected RungeKutta rungeKuttaT;

        // P�esnost v�po�tu deviac�
        protected double precisionW;
        protected RungeKutta rungeKuttaW;

        protected IDynamicalSystem dynamicalSystem;

        // Aktu�ln� sou�adnice a rychlosti
        protected Vector x;

        // Aktu�ln� odchylky
        protected Vector w1, w2;

        /// <summary>
        /// Rovnice pro v�po�et deviace
        /// </summary>
        /// <param name="w">Vektor deviace</param>
        protected Vector DeviationEquation(Vector w) {
            return this.dynamicalSystem.Jacobian(this.x) * w;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamick� syst�m</param>
        public SALI(IDynamicalSystem dynamicalSystem, double precision, RungeKuttaMethods rkMethod)
            : this(dynamicalSystem, precision, rkMethod, precision, rkMethod) {
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamick� syst�m</param>
        public SALI(IDynamicalSystem dynamicalSystem, double precisionT, RungeKuttaMethods rkMethodT, double precisionW, RungeKuttaMethods rkMethodW) {
            this.rungeKuttaW = RungeKutta.CreateRungeKutta(this.DeviationEquation, precisionW, rkMethodW);
            this.rungeKuttaT = RungeKutta.CreateRungeKutta(dynamicalSystem, precisionT, rkMethodT);
            this.precisionW = precisionW > 0.0 ? precisionW : this.rungeKuttaW.Precision;
            this.precisionT = precisionT > 0.0 ? precisionT : this.rungeKuttaT.Precision;

            this.dynamicalSystem = dynamicalSystem;
        }

        /// <summary>
        /// Alignment index SALI
        /// </summary>
        protected double AlignmentIndex() {
            Vector w1n = this.w1.EuklideanNormalization();
            Vector w2n = this.w2.EuklideanNormalization();
            Vector antiparalel = w1n - w2n;
            Vector paralel = w1n + w2n;
            
            return System.Math.Min(paralel.EuklideanNorm(), antiparalel.EuklideanNorm());
        }

        /// <summary>
        /// Inicializuje v�po�et
        /// </summary>
        /// <param name="initialX">Po��te�n� podm�nky</param>
        protected void Init(Vector initialX) {
            this.x = initialX;
            int length = initialX.Length;

            this.w1 = new Vector(length);
            this.w2 = new Vector(length);

            this.w1[0] = 1.0;
            this.w2[length / 2] = 1.0;

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
            double newStep, tStep1, tStep2;

            Vector addX = this.rungeKuttaT.Step(this.x, ref step, out newStep);

            double oldStepW1 = step;
            Vector addW1 = this.rungeKuttaW.Step(this.w1, ref step, out tStep1);

            double oldStepW2 = step;
            Vector addW2 = this.rungeKuttaW.Step(this.w2, ref step, out tStep2);

            if(step != oldStepW1 || step != oldStepW2) {
                step = System.Math.Min(System.Math.Min(step, oldStepW1), oldStepW2);

                addX = this.rungeKuttaT.Step(this.x, ref step, out newStep);
                addW1 = this.rungeKuttaW.Step(w1, ref step, out tStep1);
                addW2 = this.rungeKuttaW.Step(w2, ref step, out tStep2);
            }

            this.x += addX;
            this.w1 += addW1;
            this.w2 += addW2;

            return System.Math.Min(System.Math.Min(newStep, tStep1), tStep2);
        }

        /// <summary>
        /// Vypo��t� z�vislost SALI na �ase
        /// </summary>
        /// <param name="initialX">Po��te�n� podm�nky</param>
        /// <param name="time">�as</param>
        public PointVector TimeDependence(Vector initialX, double time, double timeStep) {
            ArrayList sali = new ArrayList();

            double step = System.Math.Min(this.precisionT, this.precisionW);
            double t = 0.0;
            double tNext = 0.0;
            
            this.Init(initialX);

            do {
                while(t < tNext){
                    double newStep = this.Step(ref step);
                    t += step;
                    step = newStep;
                }

                sali.Add(new PointD(t, this.AlignmentIndex()));
                tNext += timeStep;
            } while(t < time);

            // P�evod v�sledku na �adu
            PointVector result = new PointVector(sali.Count);
            int j = 0;
            foreach(PointD p in sali)
                result[j++] = p;

            return result;
        }

        /// <summary>
        /// Vr�t� true, pokud dan� trajektorie je podle SALI regul�rn�
        /// </summary>
        /// <param name="initialX">Po��te�n� podm�nky</param>
        public bool IsRegular(Vector initialX, IOutputWriter writer) {
            double step = System.Math.Min(this.precisionT, this.precisionW);
            double timeStep = 1.0;
            double t = 0.0;
            double tNext = timeStep;

            MeanQueue queue = new MeanQueue(window);

            this.Init(initialX);

            int result = 0;
            do {
                while(t < tNext){
                    double newStep = this.Step(ref step);
                    t += step;
                    step = newStep;
                }

                result = this.SALIDecision(t, queue);
                if(result >= 0)
                    break;

                tNext += timeStep;
            } while(true);

            if(writer != null)
                writer.Write(t);

            return (result == 1) ? true : false;
        }

        /// <summary>
        /// Rozhodne, zda je trajektorie regul�rn� �i chaotick�
        /// </summary>
        /// <param name="queue">Pr�m�rovan� data</param>
        /// <param name="t">�as</param>
        /// <returns>-1 pro nerozhodnutou trajektorii, 0 pro chaotickou, 1 pro regul�rn�</returns>
        protected int SALIDecision(double t, MeanQueue queue) {
            double ai = this.AlignmentIndex();
            double logAI = (ai <= 0.0 ? 20.0 : -System.Math.Log10(ai));
            queue.Set(logAI);

            double meanSALI = queue.Mean;

            if(meanSALI > 5.0 + t / 200.0)
                return 0;
            if(meanSALI < (t - 500.0) / 50.0) 
                return 1;
            
            return -1;
        }

        protected const int window = 20;
    }

    /// <summary>
    /// Realizuje frontu k vyst�edov�v�n�
    /// </summary>
    public class MeanQueue {
        private Vector queue;
        private int index;

        /// <summary>
        /// D�lka fronty
        /// </summary>
        public int Length { get { return this.queue.Length; } }

        /// <summary>
        /// St�edn� hodnota
        /// </summary>
        public double Mean { get { return this.queue.Mean(); } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="length">D�lka fronty</param>
        public MeanQueue(int length) {
            this.queue = new Vector(length);
            this.index = 0;
        }

        /// <summary>
        /// Nastav� hodnotu na aktu�ln� pozici
        /// </summary>
        /// <param name="value">Hodnota</param>
        /// <returns>Hodnota z aktu�ln� pozice</returns>
        public void Set(double value) {
            this.queue[this.index++] = value;
            this.index %= this.Length;
        }
    }
}
