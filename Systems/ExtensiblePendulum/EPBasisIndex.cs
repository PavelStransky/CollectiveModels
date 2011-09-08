using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Tøída, která v sobì zapouzdøuje indexy
    /// pro bázi natahovacího kyvadla
    /// </summary>
    public class EPBasisIndex: BasisIndex {
        private int maxn, maxm;
        private bool positive;

        // True, pokud uvažujeme trojúhelníkový tvar matice
        private bool triangular; 
        
        private int[] n, m;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public EPBasisIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro naètení dat
        /// </summary>
        /// <param name="import">Import</param>
        public EPBasisIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastaví parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);

            if(basisParams.Length > 2 && basisParams[2] > 0)
                this.positive = true;
            else
                this.positive = false;

            if((int)basisParams[0] > 0) {
                this.triangular = false;

                this.maxn = (int)basisParams[0];                     // Maximální hlavní kvantové èíslo
                this.maxm = (int)basisParams[1];                     // Maximální orbitální kvantové èíslo

                int length = this.positive ? (this.maxn + 1) * (this.maxm + 1) : (this.maxn + 1) * (2 * this.maxm + 1);
                this.n = new int[length];
                this.m = new int[length];

                int minm = this.positive ? 0 : -this.maxm;

                int k = 0;
                for(int i = minm; i <= this.maxm; i++)
                    for(int j = 0; j <= this.maxn; j++) {
                        this.m[k] = i;
                        this.n[k] = j;
                        k++;
                    }
            }
        }

        /// <summary>
        /// Hlavní kvantové èíslo
        /// </summary>
        public int[] N { get { return this.n; } }

        /// <summary>
        /// Orbitální kvantové èíslo
        /// </summary>
        public int[] M { get { return this.m; } }

        /// <summary>
        /// Poèet indexù báze
        /// </summary>
        public override int Rank { get { return 2; } }

        /// <summary>
        /// Poèet prvkù
        /// </summary>
        public override int Length {
            get {
                return this.n.Length;
            }
        }

        /// <summary>
        /// Šíøka pásu pásové matice
        /// </summary>
        public override int BandWidth { get { return this.positive ? this.maxn + 1 : 2 * this.maxn + 1; } }

        /// <summary>
        /// Maximální hodnota hlavního kvantového èísla n
        /// </summary>
        public int MaxN { get { return this.maxn; } }

        /// <summary>
        /// Maximální hodnota orbitálního kvantového èísla m
        /// </summary>
        public int MaxM { get { return this.maxm; } }

        /// <summary>
        /// True, pokud poèítáme jen pozitivní rotace
        /// </summary>
        public bool Positive { get { return this.positive; } }

        /// <summary>
        /// Vrací index prvku s kvantovými èísly n, m
        /// Pokud prvek neexistuje, vrací -1
        /// </summary>
        /// <param name="n">Hlavní kvantové èíslo</param>
        /// <param name="m">Orbitální kvantové èíslo</param>
        public int this[int n, int m] {
            get {
                if(n < 0 || n > this.maxn)
                    return -1;

                if(this.positive) {
                    if(m < 0 || m > this.maxm)
                        return -1;
                    return m * (this.maxn + 1) + n;
                }
                else {
                    if(System.Math.Abs(m) > this.maxm)
                        return -1;
                    return (m + this.maxm) * (maxn + 1) + n;
                }
            }
        }

        /// <summary>
        /// Vrací index prvku s kvantovými èísly m1, m2.
        /// Pokud prvek neexistuje, vrací -1
        /// </summary>
        /// <param name="basisIndex">Vektor s indexy</param>
        public override int this[Vector basisIndex] {
            get {
                return this[(int)basisIndex[0], (int)basisIndex[1]];
            }
        }

        public override int BasisQuantumNumberLength(int qn) {
            switch(qn) {
                case 0:
                    return this.maxn + 1;
                case 1:
                    if(this.positive)
                        return this.maxm + 1;
                    else
                        return 2 * this.maxm + 1;
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }

        public override int GetBasisQuantumNumber(int qn, int i) {
            switch(qn) {
                case 0:
                    return this.n[i];
                case 1:
                    return this.m[i];
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }
    }
}
