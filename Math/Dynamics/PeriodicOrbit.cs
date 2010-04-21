using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Math {
    public class PeriodicOrbit {
        private IDynamicalSystem dynamicalSystem;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamický systém</param>
        public PeriodicOrbit(IDynamicalSystem dynamicalSystem) {
            this.dynamicalSystem = dynamicalSystem;
        }

        public ArrayList Compute(Vector initialX, int psSize, int minCircle, double precision, IOutputWriter writer) {
            if(precision <= 0.0)
                precision = defaultPrecision;
            if(psSize <= 0)
                psSize = defaultPSSize;
            if(minCircle <= 0)
                minCircle = defaultMinCircle;

            SALIContourGraph salig = new SALIContourGraph(this.dynamicalSystem, 0, RungeKuttaMethods.Normal);
            PoincareSection section = new PoincareSection(this.dynamicalSystem, 0, RungeKuttaMethods.Adapted);

            initialX = (Vector)initialX.Clone();

            double e = dynamicalSystem.E(initialX);
            PointVector circle = null;

            double sizeX = 0.0;
            double sizeY = 0.0;

            ArrayList ps = new ArrayList();

            int iteration = 0;
            do {
                if(writer != null)
                    writer.WriteLine(initialX);

                PointVector pvsection = section.Compute(initialX, psSize);
                circle = pvsection.JoinCircle(minCircle);

                sizeX = circle.MaxX() - circle.MinX();
                sizeY = circle.MaxY() - circle.MinY();

                initialX[0] = circle.VectorX.Mean();
                initialX[1] = 0;
                initialX[2] = circle.VectorY.Mean();
                initialX[3] = double.NaN;

                if(!dynamicalSystem.IC(initialX, e)) {
                    throw new Exception();
                }

                if(writer != null) {
                    writer.WriteLine(string.Format("{0}, {1}...({2}, {3})", 
                        pvsection.Length, circle.Length, sizeX, sizeY));
                }

            } while((System.Math.Abs(sizeX) + System.Math.Abs(sizeY) > precision) && (iteration++ < maxIteration));

            RungeKutta rk = new RungeKutta(this.dynamicalSystem);

            double oldy = initialX[1];
            double step = rk.Precision;
            double t = 0.0;

            Vector x = initialX;
            rk.Init(initialX);

            do {
                Vector oldx = x;

                double newStep;
                x += rk.Step(x, ref step, out newStep);

                double y = x[1];

                if(((y <= 0 && oldy > 0) || (y > 0 && oldy <= 0)) && t != 0) {
                    Vector v = (oldx - x) * (oldy / (y - oldy)) + oldx;
                    if((v[0] - initialX[0] < precision) && (v[2] - initialX[2] < precision) && (v[3] - initialX[3] < precision)) {
                        t += oldy / (oldy - y) * step;
                        break;
                    }
                }
                t += step;
                step = newStep;
                oldy = y;
            } while(true);

            ArrayList result = new ArrayList();
            result.Add(t);
            result.Add(initialX);
            return result;
        }

        private const int defaultMinCircle = 100;
        private const int defaultPSSize = 2000;
        private const int maxIteration = 10;
        private const double defaultPrecision = 1E-3;
    }
}