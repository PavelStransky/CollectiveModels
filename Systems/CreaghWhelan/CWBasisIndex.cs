using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Tøída, která v sobì zapouzdøuje dvojice indexù - kvantových èísel n, m
    /// pro bázi lineárního harmonického oscilátoru v kartézských souøadnicích
    /// </summary>
    public class CWBasisIndex : BasisIndex {
        private int[] indexn, indexm;
        private int[] lengthm;              // Poèet n pro jednotlivé m
        
        private int maxE;

        // Tuhosti oscilátorù ve smìru x, y
        private double a0x, a0y;

        // Frekvence oscilátorù ve smìru x, y
        private double omegax, omegay;

        // Maximální indexy
        private int maxM, maxN;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public CWBasisIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro naètení dat
        /// </summary>
        /// <param name="import">Import</param>
        public CWBasisIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastaví parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);

            this.a0x = basisParams[0];
            this.a0y = basisParams[1];
            int num = (int)basisParams[2];

            this.omegax = System.Math.Sqrt(2.0 * this.a0x);
            this.omegay = System.Math.Sqrt(2.0 * this.a0y);

            double coef = System.Math.Sqrt(2.0 * this.omegax * this.omegay * num);

            // Zjistíme poèet
            int length = 0;
            int rows = 0;
            for (int m = 0; m < num; m++) {
                for (int n = 0; n < num; n++)
                    if(this.omegax * n + this.omegay * m < coef)
                        length++;
                rows++;
            }

            // Generujeme
            this.indexn = new int[length];
            this.indexm = new int[length];
            this.lengthm = new int[rows];

            int i = 0;
            int j = 0;

            this.maxM = 0;
            this.maxN = 0;

            for (int m = 0; m < num; m++) {
                this.lengthm[j++] = i;
                for (int n = 0; n < num; n++) {
                    if(this.omegax * n + this.omegay * m < coef) {
                        this.indexm[i] = m;
                        this.indexn[i] = n;

                        this.maxM = System.Math.Max(this.maxM, m);
                        this.maxN = System.Math.Max(this.maxN, n);

                        i++;
                    }
                }
            }
        }

        /// <summary>
        /// Vrací index prvku s kvantovými èísly m, n.
        /// Pokud prvek neexistuje, vrací -1
        /// </summary>
        /// <param name="n">Index n</param>
        /// <param name="m">Index m</param>
        public int this[int n, int m] {
            get {
                int length = this.Length;
                int llength = this.lengthm.Length;

                for (int i = 0; i < llength; i++) {
                    int j = this.lengthm[i];
                    if (this.indexm[j] == m) {
                        int result = j + n;
                        if (result < length && this.indexm[result] == m)
                            return result;
                        else
                            return -1;
                    }
                }

                return -1;
            }
        }

        /// <summary>
        /// Vrací index prvku s kvantovými èísly m, n.
        /// Pokud prvek neexistuje, vrací -1
        /// </summary>
        /// <param name="basisIndex">Vektor s indexy</param>
        public override int this[Vector basisIndex] {
            get {
                return this[(int)basisIndex[0], (int)basisIndex[1]];
            }
        }

        /// <summary>
        /// Poèet indexù báze
        /// </summary>
        public override int Rank { get { return 2; } }

        /// <summary>
        /// Poèet prvkù
        /// </summary>
        public override int Length { get { return this.indexn.Length; } }

        /// <summary>
        /// Kvantové èíslo N
        /// </summary>
        public int[] N { get { return this.indexn; } }

        /// <summary>
        /// Kvantové èíslo M
        /// </summary>
        public int[] M { get { return this.indexm; } }

        /// <summary>
        /// Maximální hodnota kvantového èísla N
        /// </summary>
        public int MaxN { get { return this.maxN; } }

        /// <summary>
        /// Maximální hodnota kvantového èísla M
        /// </summary>
        public int MaxM { get { return this.maxM; } }

        /// <summary>
        /// Úhlová frekvence ve smìru X
        /// </summary>
        public double OmegaX { get { return this.omegax; } }

        /// <summary>
        /// Úhlová frekvence ve smìru Y
        /// </summary>
        public double OmegaY { get { return this.omegay; } }

        public double A0x { get { return this.a0x; } }

        public double A0y { get { return this.a0y; } }

        /// <summary>
        /// Velikost pásu pásové matice
        /// </summary>
        public override int BandWidth { get { return 2 * this.maxN + 4; } }

        public override int BasisQuantumNumberLength(int qn) {
            if (qn == 0)
                return this.MaxN + 1;
            else
                return this.MaxM + 1;
        }

        public override int GetBasisQuantumNumber(int qn, int i) {
            if (qn == 0)
                return this.N[i];
            else
                return this.M[i];
        }
    }
}
