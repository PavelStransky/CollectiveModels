using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Tøída, která v sobì zapouzdøuje indexy
    /// pro bázi Sturm-Coulombova problému
    /// </summary>
    public class SCBasisIndex: BasisIndex {
        private int maxn, minl, maxl;
        private bool parity;

        private int[] n, l;

        // True, pokud uvažujeme trojúhelníkový tvar matice
        private bool triangular;

        // Kvantové èíslo m (projekce úhlového momentu)
        private int m;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public SCBasisIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro naètení dat
        /// </summary>
        /// <param name="import">Import</param>
        public SCBasisIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastaví parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);

            this.parity = true;
            if(basisParams.Length > 2 && basisParams[2] < 0.0)
                this.parity = false;

            this.m = (int)basisParams[1];
            this.minl = System.Math.Abs(m);

            // positive parity
            if(this.parity) {
                if(this.minl % 2 != 0)
                    this.minl++;
            }
            // negative parity
            else {
                if(this.minl % 2 == 0)
                    this.minl++;
            } 
            
            if(basisParams[0] < 0) {
                this.triangular = true;

                this.maxn = System.Math.Abs((int)basisParams[0]);
                this.maxl = this.maxn + this.minl;

                int length = 0;
                for(int i = this.minl; i < this.maxl; i += 2)
                    for(int j = 0; j + i - this.minl < this.maxn; j++) 
                        length++;

                this.n = new int[length];
                this.l = new int[length];

                int k = 0;
                for(int i = this.minl; i < this.maxl; i+= 2)
                    for(int j = 0; j + i - this.minl < this.maxn; j++) {
                        this.n[k] = j;
                        this.l[k] = i;
                        k++;
                    }
            }
            else {
                this.triangular = false;
                this.maxn = (int)basisParams[0];
                this.maxl = this.minl + (int)basisParams[1];

                int length = this.maxn * (this.maxl - this.minl) / 2;
                this.n = new int[length];
                this.l = new int[length];

                int k = 0;
                for(int i = 0; i < this.maxl; i += 2)
                    for(int j = 0; j < this.maxn; j++) {
                        this.n[k] = j;
                        this.l[k] = i;
                        k++;
                    }
            }
        }

        /// <summary>
        /// Hlavní kvantové èíslo
        /// </summary>
        public int[] N { get { return this.n; } }

        /// <summary>
        /// Moment hybnosti
        /// </summary>
        public int[] L { get { return this.l; } }

        /// <summary>
        /// Projekce momentu hybnosti
        /// </summary>
        public int M { get { return this.m; } }

        /// <summary>
        /// Poèet kvantových èísel
        /// </summary>
        public override int Rank { get { return 2; } }

        /// <summary>
        /// Poèet prvkù
        /// </summary>
        public override int Length { get { return this.l.Length; } }

        /// <summary>
        /// Šíøka pásu matice
        /// </summary>
        public override int BandWidth { get { return 2 * this.maxn + 1; } }

        /// <summary>
        /// Maximální hodnota kvantového èísla l
        /// </summary>
        public int MaxL { get { return this.maxl; } }

        /// <summary>
        /// Maximální hodnota kvantového èísla n
        /// </summary>
        public int MaxN { get { return this.maxn; } }

        /// <summary>
        /// Vrací index prvku s kvantovými èísly n, l
        /// </summary>
        /// <param name="n">Hlavní kvantové èíslo</param>
        /// <param name="l">Úhlový moment</param>
        public int this[int n, int l] {
            get {
                if(this.triangular)
                    return n * this.maxl - n * (n + 1) / 2 + l;
                else
                    return n * this.maxl + l;
            }
        }

        /// <summary>
        /// Vrací index prvku s kvantovými èísly n, l
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
                    return this.maxn;
                case 1:
                    return this.maxl;
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }

        public override int GetBasisQuantumNumber(int qn, int i) {
            switch(qn) {
                case 0:
                    return this.n[i];
                case 1:
                    return this.l[i];
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }
    }
}
