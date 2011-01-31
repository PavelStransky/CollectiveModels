using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Tøída, která v sobì zapouzdøuje indexy
    /// pro bázi elipsoidní pravoúhlé jámy
    /// </summary>
    public class SpheroidBasisIndex: BasisIndex {
        private int maxn, maxl;

        private int[] n, l, m;

        // Energie vlastních stavù
        private Vector e;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public SpheroidBasisIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro naètení dat
        /// </summary>
        /// <param name="import">Import</param>
        public SpheroidBasisIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastaví parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);
            this.maxn = (int)basisParams[0];                     // Maximální index kyvadla 1
            this.maxl = (int)basisParams[1];                     // Maximální energie kyvadla 2

            Vector[] zeros = new Vector[this.maxl];
            for(int l = 0; l < this.maxl; l++) {
                BesselJZero b = new BesselJZero(l, precision);
                zeros[l] = b.Solve(this.maxn);
            }

            double e0 = 0.0;
            double emax = zeros[this.maxl - 1][this.maxn - 1];

            ArrayList ae = new ArrayList();
            ArrayList al = new ArrayList();
            ArrayList an = new ArrayList();

            while(e0 < emax) {
                double ecur = emax;
                int lcur = this.maxl - 1;
                int ncur = this.maxn - 1;

                for(int l = 0; l < this.maxl; l++)
                    for(int n = 0; n < this.maxn; n++)
                        if(zeros[l][n] < ecur && zeros[l][n] > e0) {
                            ecur = zeros[l][n];
                            lcur = l;
                            ncur = n;
                        }

                ae.Add(ecur);
                al.Add(lcur);
                an.Add(ncur);

                e0 = ecur;
            }

            int count = ae.Count;
            int length = 0;

            for(int i = 0; i < count; i++) 
                length += 2 * (int)al[i] + 1;

            this.n = new int[length];
            this.l = new int[length];
            this.m = new int[length];
            this.e = new Vector(length);

            int j = 0;
            for(int i = 0; i < count; i++) {
                int l = (int)al[i];
                for(int m = -l; m <= l; m++) {
                    this.n[j] = (int)an[i];
                    this.l[j] = l;
                    this.m[j] = m;
                    this.e[j] = (double)ae[i];
                    j++;
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
        public int[] M { get { return this.m; } }

        /// <summary>
        /// Energie
        /// </summary>
        public Vector E { get { return this.e; } }

        /// <summary>
        /// Poèet kvantových èísel
        /// </summary>
        public override int Rank { get { return 3; } }

        /// <summary>
        /// Poèet prvkù
        /// </summary>
        public override int Length {
            get {
                return this.l.Length;
            }
        }

        /// <summary>
        /// Maximální hodnota kvantového èísla l
        /// </summary>
        public int MaxL { get { return this.maxl; } }

        /// <summary>
        /// Maximální hodnota kvantového èísla n
        /// </summary>
        public int MaxN { get { return this.maxn; } }

        /// <summary>
        /// Vrací index prvku s kvantovými èísly n, l, m
        /// Pokud prvek neexistuje, vrací -1
        /// </summary>
        /// <param name="n">Hlavní kvantové èíslo</param>
        /// <param name="l">Úhlový moment</param>
        /// <param name="m">Projekce úhlového momentu</param>
        public int this[int n, int l, int m] {
            get {
                int length = this.Length;
                for(int i = 0; i < length; i++)
                    if(this.n[i] == n && this.l[i] == l && this.m[i] == m)
                        return i;
                return -1;
            }
        }

        /// <summary>
        /// Vrací index prvku s kvantovými èísly n, l, m
        /// Pokud prvek neexistuje, vrací -1
        /// </summary>
        /// <param name="basisIndex">Vektor s indexy</param>
        public override int this[Vector basisIndex] {
            get {
                return this[(int)basisIndex[0], (int)basisIndex[1], (int)basisIndex[2]];
            }
        }

        public override int BasisQuantumNumberLength(int qn) {
            switch(qn) {
                case 0:
                    return this.maxn;
                case 1:
                    return this.maxl;
                case 2:
                    return 2 * this.maxl + 1;
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }

        public override int GetBasisQuantumNumber(int qn, int i) {
            switch(qn) {
                case 0:
                    return this.n[i];
                case 1:
                    return this.l[i];
                case 2:
                    return this.m[i];
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }

        private const double precision = 1E-10;
    }
}
