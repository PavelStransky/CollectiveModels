using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// T��da, kter� v sob� zapouzd�uje dvojice index� - kvantov�ch ��sel n, m
    /// pro b�zi line�rn�ho harmonick�ho oscil�toru v pol�rn�ch sou�adnic�ch
    /// </summary>
    public class CWPolarBasisIndex : BasisIndex {
        private int[] indexn, indexm;
        private int[] lengthm;              // Po�et n pro jednotliv� m
        private int maxE;

        private double a0, omega;

        // Maxim�ln� indexy
        private int maxM, maxN;

        private int bandWidth;

        /// <summary>
        /// �hlov� frekvence
        /// </summary>
        public double Omega { get { return this.omega; } }

        public double A0 { get { return this.a0; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public CWPolarBasisIndex(Vector basisParams)
            : base(basisParams) {
            this.a0 = basisParams[0];
            this.omega = System.Math.Sqrt(2.0 * this.a0);
        }

        /// <summary>
        /// Konstruktor pro na�ten� dat
        /// </summary>
        /// <param name="import">Import</param>
        public CWPolarBasisIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastav� parametry b�ze
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);

            if(basisParams.Length == 3) {
                this.maxE = (int)basisParams[1];                     // Maxim�ln� energie v n�sobc�ch hbar * Omega

                int mBoundMax = this.maxE - 1;
                int mBoundMin = -mBoundMax;

                // Zjist�me po�et
                int length = 0;
                int rows = 0;
                for(int m = mBoundMin; m <= mBoundMax; m++) {
                    for(int n = 0; n <= (this.maxE - 1 - System.Math.Abs(m)) / 2; n++)
                        length++;
                    rows++;
                }

                // Generujeme
                this.indexn = new int[length];
                this.indexm = new int[length];
                this.lengthm = new int[rows];

                int i = 0;
                int j = 0;
                for(int m = mBoundMin; m <= mBoundMax; m++) {
                    this.lengthm[j++] = i;
                    for(int n = 0; n <= (this.maxE - 1 - System.Math.Abs(m)) / 2; n++) {
                        this.indexm[i] = m;
                        this.indexn[i] = n;
                        i++;
                    }
                }

                this.maxM = System.Math.Max(mBoundMax, System.Math.Abs(mBoundMin));
                this.maxN = (this.maxE - 1) / 2;

                if(basisParams[2] == 2)
                    this.bandWidth = 2 * (this.maxE + 1);
                else
                    this.bandWidth = 4 * (this.maxE);
            }
            else {
                this.maxN = (int)basisParams[1];
                this.maxM = (int)basisParams[2];

                this.maxE = 2 * this.maxN + this.maxM + 1;
                int length = (this.maxN + 1) * (2 * this.maxM + 1);

                // Generujeme
                this.indexn = new int[length];
                this.indexm = new int[length];

                int rows = 2 * this.maxM + 1;
                this.lengthm = new int[rows];

                int i = 0;
                int j = 0;
                for(int m = -this.maxM; m <= this.maxM; m++) {
                    this.lengthm[j++] = i;
                    for(int n = 0; n <= this.maxN; n++) {
                        this.indexm[i] = m;
                        this.indexn[i] = n;
                        i++;
                    }
                }

                this.bandWidth = length;
            }
        }

        /// <summary>
        /// Vrac� index prvku s kvantov�mi ��sly m, n.
        /// Pokud prvek neexistuje, vrac� -1
        /// </summary>
        /// <param name="n">Index n</param>
        /// <param name="m">Index m</param>
        public int this[int n, int m] {
            get {
                int length = this.Length;
                int llength = this.lengthm.Length;

                for(int i = 0; i < llength; i++) {
                    int j = this.lengthm[i];
                    if(this.indexm[j] == m) {
                        int result = j + n;
                        if(result < length && this.indexm[result] == m)
                            return result;
                        else
                            return -1;
                    }
                }

                return -1;
            }
        }

        /// <summary>
        /// Vrac� index prvku s kvantov�mi ��sly m, n.
        /// Pokud prvek neexistuje, vrac� -1
        /// </summary>
        /// <param name="basisIndex">Vektor s indexy</param>
        public override int this[Vector basisIndex] {
            get {
                return this[(int)basisIndex[0], (int)basisIndex[1]];
            }
        }

        /// <summary>
        /// Po�et index� b�ze
        /// </summary>
        public override int Rank { get { return 2; } }

        /// <summary>
        /// Maxim�ln� energie (vyj�d�en� v n�sobc�ch hbar * Omega)
        /// </summary>
        public int MaxE { get { return this.maxE; } }

        /// <summary>
        /// Po�et prvk�
        /// </summary>
        public override int Length { get { return this.indexn.Length; } }

        /// <summary>
        /// Kvantov� ��slo N
        /// </summary>
        public int[] N { get { return this.indexn; } }

        /// <summary>
        /// Kvantov� ��slo M
        /// </summary>
        public int[] M { get { return this.indexm; } }

        /// <summary>
        /// Maxim�ln� hodnota kvantov�ho ��sla N
        /// </summary>
        public int MaxN { get { return this.maxN; } }

        /// <summary>
        /// Maxim�ln� hodnota kvantov�ho ��sla M
        /// </summary>
        public int MaxM { get { return this.maxM; } }

        /// <summary>
        /// Velikost p�su p�sov� matice
        /// </summary>
        public override int BandWidth { get { return this.bandWidth; } }

        public override int BasisQuantumNumberLength(int qn) {
            if(qn == 0)
                return this.MaxN + 1;
            else
                return 2 * this.MaxM + 1;
        }

        public override int GetBasisQuantumNumber(int qn, int i) {
            if(qn == 0)
                return this.N[i];
            else
                return this.M[i];
        }
    }
}
