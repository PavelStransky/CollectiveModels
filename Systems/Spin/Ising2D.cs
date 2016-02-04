using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
    public partial class Ising2D : IExportable {
        private int sizeX, sizeY;           // Lattice sizes
        private Vector polynomial;          // Coefficients of the characteristic polynomial

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sizeX">Lattice size X</param>
        /// <param name="sizeX">Lattice size Y</param>
        /// <param name="isPolynomial">True for polynomial method</param>
        public Ising2D(int sizeX, int sizeY, bool isPolynomial, IOutputWriter writer) {
            this.sizeX = sizeX;
            this.sizeY = sizeY;

            if(isPolynomial) {
                Polynomial p = new Polynomial(this.sizeX, this.sizeY);
                this.polynomial = p.Compute(writer);
            }
            else
                this.polynomial = null;
        }

        public Complex GetValue(Complex x, bool temperature) {
            if(temperature) 
                x = Complex.Exp(-x);

            Complex xn = 1.0 / x;

            Complex rp = new Complex();
            Complex rn = new Complex();

            if(this.polynomial == null) {
                Direct d = new Direct(this.sizeX, this.sizeY);
                return d.Compute(x);
            }
            else {
                int maxk = this.polynomial.Length;

                for(int i = 0; i < maxk / 2; i++) {
                    rp = rp * x + this.polynomial[i];
                    rn = rn * xn + this.polynomial[i];
                }

                if(maxk % 2 == 0) {
                    rp *= Complex.Sqrt(x);
                    rn *= Complex.Sqrt(xn);
                }
                else {
                    rp *= x;
                    rn *= xn;
                    rp += this.polynomial[maxk / 2];
                }

                return rp + rn;
            }
        }

        public PointVector GetZeros() {
            if(this.polynomial != null) {
                // Construct a matrix
                int l = this.polynomial.Length - 1;

                Matrix m = new Matrix(l);
                for(int i = 1; i <= l; i++) {
                    m[0, i - 1] = -this.polynomial[i] / this.polynomial[0];
                    if(i < l)
                        m[i, i - 1] = 1;
                }

                Vector[] v = LAPackDLL.dgeev(m, false);

                PointVector result = new PointVector(l);
                result.VectorX = v[0];
                result.VectorY = v[1];
                return result;
            }

            return null;
        }

        #region IExportable
        /// <summary>
        /// Save the class in a file
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.sizeX, "Length");
            param.Add(this.polynomial, "Polynomial");

            param.Export(export);
        }

        /// <summary>
        /// Načte výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public Ising2D(Core.Import import) {
            IEParam param = new IEParam(import);

            this.sizeX = (int)param.Get(1.0);
            this.polynomial = (Vector)param.Get(null);
        }
        #endregion
    }
}

