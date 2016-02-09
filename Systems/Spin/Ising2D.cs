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
        private BigInteger[] polynomial;          // Coefficients of the characteristic polynomial

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
                    rp = rp * x + (double)this.polynomial[i];
                    rn = rn * xn + (double)this.polynomial[i];
                }

                if(maxk % 2 == 0) {
                    rp *= Complex.Sqrt(x);
                    rn *= Complex.Sqrt(xn);
                }
                else {
                    rp *= x;
                    rn *= xn;
                    rp += (double)this.polynomial[maxk / 2];
                }

                return rp + rn;
            }
        }

        public PointVector GetZeros(IOutputWriter writer) {
            if(this.polynomial != null) {
                // First let's get rid of the roots == 1
                BigInteger[] r = this.polynomial.Clone() as BigInteger[];
                BigInteger[] p = null;
                BigInteger[] pw = null;

                if(writer != null)
                    writer.Write("ZeroZ");

                DateTime t = DateTime.Now;

                int roots = 0;
                do {
                    p = r.Clone() as BigInteger[];
                    pw = r;
                    r = new BigInteger[pw.Length - 1];
                    for(int i = 0; i < r.Length; i++) {
                        r[i] = pw[i];
                        pw[i + 1] -= pw[i];
                        pw[i] = 0;
                    }
                    roots++;
                } while(pw[r.Length] == 0);
                roots--;

                if(writer != null)
                    writer.Write(string.Format("({0})", roots));

                // Construct a matrix
                int l = p.Length - 1;

                Matrix m = new Matrix(l);
                for(int i = 1; i <= l; i++) {
                    m[0, i - 1] = -(double)p[i] / (double)p[0];
                    if(i < l)
                        m[i, i - 1] = 1;
                }

                Vector[] v = LAPackDLL.dgeev(m, false);

                if(writer != null)
                    writer.WriteLine(DateTime.Now - t);

                PointVector result = new PointVector(l);
                result.VectorX = v[0];
                result.VectorY = v[1];

                result.Length = l + roots;
                for(int i = l; i < l + roots; i++)
                    result[i].X = -1;

                return result;
            }

            return null;
        }

        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("Ising 2D ({0} x {1})", this.sizeX, this.sizeY));
            s.Append(Environment.NewLine);

            if(this.polynomial != null) {
                for(int i = 0; i < this.polynomial.Length; i++) {
                    s.Append(this.polynomial[i]);
                    s.Append(Environment.NewLine);
                }
            }
            return s.ToString();
        }

        #region IExportable
        /// <summary>
        /// Save the class in a file
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.sizeX, "Length");

            if(this.polynomial == null)
                param.Add(0, "Polynomial");
            else {
                param.Add(this.polynomial.Length, "Polynomial");
                for(int i = 0; i < this.polynomial.Length; i++)
                    param.Add(this.polynomial[i]);
            }
            param.Export(export);
        }

        /// <summary>
        /// Načte výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public Ising2D(Core.Import import) {
            IEParam param = new IEParam(import);

            this.sizeX = (int)param.Get(1.0);
            int l = (int)param.Get(0);
            if(l == 0)
                this.polynomial = null;
            else {
                this.polynomial = new BigInteger[l];
                for(int i = 0; i < l; i++)
                    this.polynomial[i] = (BigInteger)param.Get();
            }
        }
        #endregion
    }
}

