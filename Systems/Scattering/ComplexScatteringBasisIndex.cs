using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Třída, která v sobě zapouzdřuje indexy pro ComplexScattering
    /// </summary>
    public class ComplexScatteringBasisIndex : BasisIndex {
        private double l;
        private int division;
        private int maxN;
        private double hbar;
        private int real;

        private int[] n;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public ComplexScatteringBasisIndex(Vector basisParams) : base(basisParams) { }

        /// <summary>
        /// Konstruktor pro načtení dat
        /// </summary>
        /// <param name="import">Import</param>
        public ComplexScatteringBasisIndex(Import import) : base(import) { }

        /// <summary>
        /// Nastaví parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        protected override void Init(Vector basisParams) {
            base.Init(basisParams);

            this.hbar = basisParams[0];
            this.l = basisParams[1];
            this.maxN = (int)basisParams[2];
            this.division = basisParams.Length > 3 ? (int)basisParams[3] : 10000;
            this.real = basisParams.Length > 4 ? (int)basisParams[4] : 0;

            this.n = new int[this.maxN];
            for (int i = 0; i < this.maxN; i++)
                this.n[i] = i + 1;
        }

        /// <summary>
        /// True pokud nás zajímá reálná část matice (pro funkci HamiltonianMatrix)
        /// </summary>
        public bool Real { get { if (this.real > 0) return true; else return false; } }

        /// <summary>
        /// Šířka jámy
        /// </summary>
        public double L { get { return this.l; } }

        /// <summary>
        /// Dělení jámy pro integraci
        /// </summary>
        public int Division { get { return this.division; } }

        /// <summary>
        /// Planckova konstanta
        /// </summary>
        public double Hbar { get { return this.hbar; } }

        /// <summary>
        /// Hlavní kvantové číslo
        /// </summary>
        public int[] N { get { return this.n; } }

        /// <summary>
        /// Počet kvantových čísel
        /// </summary>
        public override int Rank { get { return 1; } }

        /// <summary>
        /// Počet prvků
        /// </summary>
        public override int Length { get { return this.maxN; } }

        /// <summary>
        /// Šířka pásu matice
        /// </summary>
        public override int BandWidth { get { return this.maxN; } }

        /// <summary>
        /// Vrací index prvku s kvantovými čísly n, l
        /// Pokud prvek neexistuje, vrací -1
        /// </summary>
        /// <param name="basisIndex">Vektor s indexy</param>
        public override int this[Vector basisIndex] { get { return this.N[(int)basisIndex[0]]; } }

        public override int BasisQuantumNumberLength(int qn) {
            switch (qn) {
                case 0:
                    return this.maxN;
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }

        public override int GetBasisQuantumNumber(int qn, int i) {
            switch (qn) {
                case 0:
                    return this.n[i];
            }

            throw new SystemException(string.Format(Messages.EMBadQNIndex, qn));
        }
    }
}
