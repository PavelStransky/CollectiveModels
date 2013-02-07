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
        private double a, b;
        
        // Power of the principal term (x^2 - 1)
        private int power;

        // Quadratic parameter y^2
        private double mu;

        public double A { get { return this.a; } }
        public double B { get { return this.b; } }
        public double Mu { get { return this.mu; } }
        public int Power { get { return this.power; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="mu">Parametr MU</param>
        /// <param name="power">Parametr power</param>
        public CW(double a, double b, double mu, int power) {
            this.a = a;
            this.b = b;

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
        }
        #endregion
    }
}