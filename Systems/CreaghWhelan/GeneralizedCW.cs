using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Core;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
    /// <summary>
    /// Generalized Creagh-Whelan H=T+sum_{ij} a_{ij} x^i y^j
    /// </summary>
    public class GeneralizedCW : IExportable {
        private Matrix a;

        public Matrix A { get { return this.a; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a">Parametr A</param>
        public GeneralizedCW(Matrix a) {
            this.a = a;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        protected GeneralizedCW() { }

        #region Implementace IExportable
        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public virtual void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.a, "A");
            param.Export(export);
        }

        /// <summary>
        /// Načte výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public GeneralizedCW(Core.Import import) {
            IEParam param = new IEParam(import);
            this.a = (Matrix)param.Get(null);
        }
        #endregion

        public double V(double x, double y) {
            Vector xi = new Vector(maxPower);
            Vector yj = new Vector(maxPower);

            xi[0] = 1.0; yj[0] = 1.0;
            for(int i = 1; i < maxPower; i++) {
                xi[i] = xi[i - 1] * x;
                yj[i] = yj[i - 1] * y;
            }

            double result = 0.0;
            for(int i = 0; i < maxPower; i++)
                for(int j = 0; j < maxPower; j++)
                    result += this.a[i, j] * xi[i] * yj[j];

            return result;
        }

        private const int maxPower = 8;
    }
}