using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Sifting step for the EMD
    /// </summary>
    public class SiftingStep {
        private double errorU = 0.0;
        private double errorL = 0.0;
        private int symmetryBreak = 0;
        private PointVector result;

        private ExtremesEnvelope minima, maxima;

        /// <summary>
        /// Result - a candidate for an IMF
        /// </summary>
        public PointVector Result { get { return this.result; } }

        /// <summary>
        /// Number of (real) maxima
        /// </summary>
        public int MaxNum { get { return this.maxima.NumExtremes; } }

        /// <summary>
        /// Number of (real) minima
        /// </summary>
        public int MinNum { get { return this.minima.NumExtremes; } }

        /// <summary>
        /// Upper cumulative error
        /// </summary>
        public double ErrorU { get { return this.errorU; } }

        /// <summary>
        /// Lower cumulative error
        /// </summary>
        public double ErrorL { get { return this.errorL; } }

        /// <summary>
        /// Number of points that breaks the symmetry condition |U+L|/|U,L| leq delta
        /// </summary>
        public int SymmetryBreak { get { return this.symmetryBreak; } }

        /// <summary>
        /// Number of extreemes that are on the wrong side of the x axis
        /// (and therefore determine the difference between extreemes and zero crossings)
        /// </summary>
        public int NumAssymetryExtreemes {
            get {
                return this.maxima.NumExtremesBelow0 + this.minima.NumExtremesBelow0;
            }
        }

        /// <summary>
        /// We found the residuum
        /// </summary>
        public bool IsResiduum { get { return this.maxima.NumExtremes < 2 || this.minima.NumExtremes < 2; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="flat">True if the flat parts of the level density is going to be considered 
        /// as a source of maxima / minima</param>
        /// <param name="delta">A special parameter for the symmetry condition |U+L|/|U,L| leq delta</param>
        public SiftingStep(PointVector data, bool flat, double delta) {
            this.maxima = new ExtremesEnvelope(data, flat);
            this.minima = new ExtremesEnvelope(-data, flat);

            if(this.IsResiduum)
                return;

            int length = data.Length;
            this.result = new PointVector(length);

            Vector x = data.VectorX;
            Vector u = maxima.GetValue(x);
            Vector l = -minima.GetValue(x);

            for(int i = 0; i < length; i++) {
                double m = 0.5 * (u[i] + l[i]);

                this.result[i] = new PointD(data[i].X, data[i].Y - m);

                double uc = System.Math.Abs(m) / System.Math.Abs(u[i]);
                double lc = System.Math.Abs(m) / System.Math.Abs(l[i]);

                if(delta > 0.0 && (uc > delta || lc > delta))
                    this.symmetryBreak++;

                this.errorU += uc;
                this.errorL += lc;
            }
        }
    }
}

