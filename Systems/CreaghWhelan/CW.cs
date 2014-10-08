using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Core;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
    /// <summary>
    /// Creagh-Whelan system (PRL 82, 5237, 1999) with an additional linear term
    /// </summary>
    public class CW : IExportable {
        // Linear parameter x, parameter of the rigidity b
        private double a, b, c;
        
        // Power of the principal term (x^2 - 1)
        private int power;

        // Quadratic parameter y^2
        private double mu;

        public double A { get { return this.a; } }
        public double B { get { return this.b; } }
        public double C { get { return this.c; } }
        public double Mu { get { return this.mu; } }
        public int Power { get { return this.power; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="mu">Parametr MU</param>
        /// <param name="power">Parametr power</param>
        public CW(double a, double b, double c, double mu, int power) {
            this.a = a;
            this.b = b;
            this.c = c;

            if(power != 2 && power != 4)
                throw new SystemsException(string.Format(Messages.EMBadPower, power));

            this.mu = mu;
            this.power = power;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        protected CW() { }

        #region Implementace IExportable
        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public virtual void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.a, "A");
            param.Add(this.b, "B");
            param.Add(this.mu, "Mu");
            param.Add(this.power, "Power");
            param.Add(this.c, "C");

            param.Export(export);
        }

        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public CW(Core.Import import) {
            IEParam param = new IEParam(import);

            this.a = (double)param.Get(0.0);
            this.b = (double)param.Get(0.0);
            this.mu = (double)param.Get(1.0);
            this.power = (int)param.Get(2);
            this.c = (double)param.Get(1.0);
        }
        #endregion

        public double V(double x, double y) {
            double x2 = x * x;
            double y2 = y * y;
            double p = x2 - 1.0;

            double bracket = p * p;
            if(this.Power == 4)
                bracket *= bracket;
            else if(this.Power == -4)
                bracket = bracket + bracket * bracket;

            return bracket + this.Mu * y2 + this.C * x2 * y2 + this.A * x + this.B * x * y2;
        }

        protected class BisectionPotential {
            private CW cg;
            private double x, y, e;

            public BisectionPotential(CW cg, double x, double y, double e) {
                this.cg = cg;
                this.x = x;
                this.y = y;
                this.e = e;
            }

            public double BisectionX(double x) {
                return cg.V(x, this.y) - e;
            }

            public double BisectionY(double y) {
                return cg.V(this.x, y) - e;
            }
        }

        protected class BisectionDxPotential {
            private CW cg;

            public BisectionDxPotential(CW cg) {
                this.cg = cg;
            }

            public double Bisection(double x) {
                return 2.0 * cg.Power * x * System.Math.Pow(x * x - 1.0, cg.Power - 1) + cg.A;
            }
        }

        protected class BisectionY {
            private CW cg;
            private double e;

            public BisectionY(CW cg, double e) {
                this.cg = cg;
                this.e = e;
            }

            public double Bisection(double x) {
                double d = x * x - 1;
                return -System.Math.Sqrt((this.e - d * d - cg.A * x) / (cg.B * x + cg.C * x * x + cg.mu));
            }
        }
    }
}