using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using PavelStransky.Math;

namespace PavelStransky.GCM {
    public class QuantumGCM : GCM {
        // Parametry báze kvadratického oscilátoru
        private double b2s, c2s;

        // true, pokud jsou parametry báze poèítány kódem
        // false, pokud kód pøebírá parametry báze z b2s, c2s
        private bool ifBase;

        // true, pokud již byly radiální kvadrupólové maticové elementy pøeškálovány
        // bázových parametrù b2s, c2s (viz metoda QBE2)
        private bool lRadQ;

        // Poèet nejnižších stavù, pro které se bude poèítat B(E2)
        // (musí platit nEnEiv >= nBQ)
        private int nBQ;

        // Parametry výpoètu
        private double bE202;
        private double tE2mode;

        // Radiální maticové elementy
        private static RadialMatrixElement rme = new RadialMatrixElement(1, 6);

        // Úhlové maticové elementy
        private static AngularMatrixElement ame = new AngularMatrixElement();

        /// <summary>
        /// Konstruktor standardního Lagrangiánu
        /// </summary>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr K</param>
        public QuantumGCM(double a, double b, double c, double k)
            : base(a, b, c, k) {
        }

        /// <summary>
        /// Nastaví testovací parametry podle fortranovského kódu
        /// </summary>
        public void Test() {
            this.C2 = -51.5;
            this.C3 = 541.5;
            this.C4 = 1793.0;

            this.B2 = 61.0;

            //          this.b2s = 90.0;
            //          this.c2s = 100.0;

            this.ifBase = false;

            this.nBQ = 4;

            //this.lMinB = 0;
            //this.lMaxB = 8;
            //this.na = 100;
            //this.nz = 100;

            this.bE202 = 0;
            this.tE2mode = 2;
            //this.ia = 0;
            //this.ib = 0;
        }

        /// <summary>
        /// Vypíše všechno pøehlednì do souboru
        /// </summary>
        /// <param name="t">StreamWriter</param>
        private void WriteInfo(StreamWriter t) {
            t.WriteLine("GCM - Geometric Collective Model");

            t.WriteLine();
            t.WriteLine("C2\t{0}", this.C2);
            t.WriteLine("C3\t{0}", this.C3);
            t.WriteLine("C4\t{0}", this.C4);
            t.WriteLine("B2\t{0}", this.B2);

            t.WriteLine();
            //t.WriteLine("NA\t{0}", this.na);
            //t.WriteLine("NZ\t{0}", this.nz);
            t.WriteLine("tE2mode\t{0}", this.tE2mode);

            t.WriteLine();
            //            t.WriteLine("nPh\t{0}", this.nPh);
            t.WriteLine("B2S\t{0}", this.b2s);
            t.WriteLine("C2S\t{0}", this.c2s);

            t.WriteLine();
            if(this.IsGammaSoft)
                t.WriteLine("SGammaSoft\t{0}", this.SGammaSoft);
            else {
                t.WriteLine("D\t{0}", this.Dp);
                t.WriteLine("E\t{0}", this.Ep);
                t.WriteLine("F\t{0}", this.Fp);
                t.WriteLine("S\t{0}", this.Sp);
            }

            // Hodnoty extrémù
            t.WriteLine();
            if(this.C3 == 0.0 && this.C2 > 0) {
                t.WriteLine("C2 > 0 -- minimum v poèátku");
            }
            else if(this.Dp > 1) {
                t.WriteLine("D > 1 -- minimum v poèátku");
            }
            else {
                t.WriteLine("Minima :");
                Vector betaMin = this.ExtremalBeta(0);
                t.WriteLine("BetaMin1\t{0}\tV(BetaMin1)\t{1}", betaMin[0], this.VBG(betaMin[0], 0.0));
                t.WriteLine("BetaMin2\t{0}\tV(BetaMin2)\t{1}", betaMin[1], this.VBG(betaMin[1], 0.0));
            }
        }

        /// <summary>
        /// Výpoèet exponentu ze vstupních parametrù
        /// </summary>
        private double[,] GetVKoef() {
            double[,] v = new double[5, 2];
            double sqrt = System.Math.Sqrt(hbar / System.Math.Sqrt(this.b2s * this.c2s));
            v[2, 0] = this.A * System.Math.Pow(sqrt, 2);
            v[3, 1] = this.B * System.Math.Pow(sqrt, 3);
            v[4, 0] = this.C * System.Math.Pow(sqrt, 4);

            return v;
        }

        /// <summary>
        /// Koeficient P2
        /// </summary>
        private double P2 { get { return hbar * System.Math.Sqrt(this.b2s * this.c2s) / this.K; } }

        /// <summary>
        /// Nastavuje bázové parametry b2s, c2s tak, aby bylo dosaženo minima souètu 
        /// prvních diagonálních maticových elementù (viz èlánek)
        /// </summary>
        private void SMin(int nPhonons) {
            // Flag, který je true, pokud byly radiální kvadrupólové maticové elementy pøeškálovány
            this.lRadQ = false;
            this.b2s = this.B2;

            double sumMin = float.MaxValue;

            // 0 - aktuální úhlový moment
            RAIndex ra = new RAIndex(0, nPhonons);

            double s;

            // s posouváme postupnì smìrem k minimu
            for(s = 7.0; s < 40.0; s += 0.5) {
                // souèet diagonálních èlenù
                double sum = 0;

                this.c2s = hbar * hbar * System.Math.Pow(s, 4) / this.b2s;

                // Poèet vysèítaných diagonálních maticových elementù
                int num = 10;

                // Potenciální energie
                double[,] v = this.GetVKoef();

                for(int i = 0; i < num; i++)
                    for(int mue = 0; mue < 2; mue++) {
                        for(int irho = 2; irho <= 4; irho++) {
                            if(System.Math.Abs(v[irho, mue]) < 1E-5)
                                continue;

                            sum += v[irho, mue] * ame[ra.IA[i], ra.IA[i], mue, 0] * rme[ra.IR[i], ra.IR[i], irho];
                        }
                    }

                // Kinetická energie
                for(int i = 0; i < num; i++)
                    sum += this.P2 * ame[ra.IA[i], ra.IA[i], 0, 0] * rme[ra.IR[i], ra.IR[i], -1];

                // Nalezli jsme minimum
                if(sumMin < sum)
                    break;

                sumMin = sum;
            }
        }


        /// <summary>
        /// Poèítá Hamiltonovu matici
        /// </summary>
        /// <param name="l">Úhlový moment</param>
        /// <param name="nPhonons">Poèet fononù</param>
        public Vector EnergyLevels(int l, int nPhonons) {
            Debug.Assert(l == 0);
            this.SMin(nPhonons);

            // Koeficienty potenciálního èlenu (beta^(i + 2) * cos(3 * gamma)^j)
            double[,] v = this.GetVKoef();

            if(l == 1)
                return new Vector(0);

            RAIndex ra = new RAIndex(l, nPhonons);
            Matrix h = new Matrix(ra.Length);
            try {
                for(int mue = 0; mue < 2; mue++) {
                    for(int irho = 2; irho <= 4; irho++) {
                        if(System.Math.Abs(v[irho, mue]) < 1E-5)
                            continue;
                        if(irho == 5 && mue != 0)
                            continue;

                        // Výpoèet maticových elementù Hamiltoniánu
                        for(int i = 0; i < h.Length; i++)
                            for(int j = 0; j <= i; j++) {
                                h[i, j] += v[irho, mue] * ame[ra.IA[i], ra.IA[j], mue, l] * rme[ra.IR[i], ra.IR[j], irho];
                                h[j, i] = h[i, j];
                            }
                    }
                }
            }
            catch(Exception e) {
                throw e;
            }

            // Kinetické èleny
            for(int i = 0; i < h.Length; i++)
                for(int j = 0; j <= i; j++) {
                    h[i, j] += this.P2 * ame[ra.IA[i], ra.IA[j], 0, l] * rme[ra.IR[i], ra.IR[j], -1];
                    h[j, i] = h[i, j];
                }

            Jacobi jacobi = new Jacobi(h);
            jacobi.SortAsc();

//            jacobi.EigenVector[0].Export("c:\\temp\\vec0.txt", false);

            return new Vector(jacobi.EigenValue);
        }

        /// <summary>
        /// Provádí výpoèet
        /// </summary>
        public void Calculate() {
            string fName = "c:\\out.txt";
            FileStream f = new FileStream(fName, FileMode.Create);
            StreamWriter t = new StreamWriter(f);

            this.WriteInfo(t);
            t.Close();
            f.Close();
            this.EnergyLevels(0, 30);
        }

        /// <summary>
        /// Vypíše parametry GCM modelu
        /// </summary>
        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("A = {0,10:#####0.000} (C2 = {1,10:#####0.000})\n", this.A, this.C2));
            s.Append(string.Format("B = {0,10:#####0.000} (C3 = {1,10:#####0.000})\n", this.B, this.C3));
            s.Append(string.Format("C = {0,10:#####0.000} (C4 = {1,10:#####0.000})\n", this.C, this.C4));
            s.Append(string.Format("K = {0,10:#####0.000} (B2 = {1,10:#####0.000})\n", this.K, this.B2));
            s.Append(string.Format("I = {0,10:#####0.000}\n", this.Invariant));
            s.Append('\n');
            s.Append(string.Format("d = {0,10:#####0.000}, e = {1,10:#####0.000}, f = {2, 10:#####0.000}\n", this.Dp, this.Ep, this.Fp));
            s.Append('\n');

            Vector beta = this.ExtremalBeta(0.0);
            s.Append(string.Format("Extrémy:"));

            for(int i = 0; i < beta.Length; i++)
                s.Append(string.Format("\nV({0,1:0.000}) = {1,1:0.000}", beta[i], this.VBG(beta[i], 0.0)));

            return s.ToString();
        }

        public const double hbar = 0.1;
//        public const double hbar = 0.6582183;
        //        public int maxI = 190;
    }
}