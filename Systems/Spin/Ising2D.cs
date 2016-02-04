using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public PointD GetValue(PointD x, bool temperature) {
            double r = 0.0;

            double uRp = 0.0;
            double uIp = 0.0;
            double uRm = 0.0;
            double uIm = 0.0;

            if(temperature) {
                uRp = System.Math.Exp(-x.X) * System.Math.Cos(x.Y);
                uIp = -System.Math.Exp(-x.X) * System.Math.Sin(x.Y);
                uRm = System.Math.Exp(x.X) * System.Math.Cos(x.Y);
                uIm = System.Math.Exp(x.X) * System.Math.Sin(x.Y);
            }
            else {
                uRp = x.X;
                uIp = x.Y;

                r = uRp * uRp + uIp * uIp;

                uRm = uRp / r;
                uIm = -uIp / r;
            }

            double resultRp = 0.0;
            double resultIp = 0.0;
            double resultRm = 0.0;
            double resultIm = 0.0;

            if(this.polynomial == null) {
                Direct d = new Direct(this.sizeX, this.sizeY);
                return d.Compute(new PointD(uRp, uIp), new PointD(uRm, uIm));
            }
            else {
                int maxk = this.polynomial.Length;

                for(int i = 0; i < maxk / 2; i++) {
                    r = resultRp * uRp - resultIp * uIp + this.polynomial[i];
                    resultIp = resultRp * uIp + resultIp * uRp;
                    resultRp = r;

                    r = resultRm * uRm - resultIm * uIm + this.polynomial[i];
                    resultIm = resultRm * uIm + resultIm * uRm;
                    resultRm = r;
                }

                r = resultRp * uRp - resultIp * uIp;
                resultIp = resultRp * uIp + resultIp * uRp;
                resultRp = r;

                r = resultRm * uRm - resultIm * uIm;
                resultIm = resultRm * uIm + resultIm * uRm;
                resultRm = r;

                if(maxk % 2 == 1)


                double resultR = resultRp + resultRm;
                double resultI = resultIp + resultIm;

                if(maxk % 2 == 1)
                    resultR += this.polynomial[maxk / 2];
                else

                return new PointD(resultR, resultI);
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

