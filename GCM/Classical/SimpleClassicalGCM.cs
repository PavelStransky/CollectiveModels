using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.GCM {
    public class SimpleClassicalGCM: GCM, IExportable, IDynamicalSystem {
        // Gener�tor n�hodn�ch ��sel
        private Random random = new Random();

        /// <summary>
        /// Kinetick� energie
        /// </summary>
        /// <param name="x">Sou�adnice x</param>
        /// <param name="y">Sou�adnice y</param>
        /// <param name="px">Hybnost x</param>
        /// <param name="py">Hybnost y</param>
        public virtual double T(double x, double y, double px, double py) {
            return 1.0 / (2.0 * this.K) * (px * px + py * py);
        }

        /// <summary>
        /// Celkov� energie
        /// </summary>
        /// <param name="x">Sou�adnice x</param>
        /// <param name="y">Sou�adnice y</param>
        /// <param name="px">Sou�adnice x</param>
        /// <param name="py">Sou�adnice y</param>
        public double E(double x, double y, double px, double py) {
            return this.T(x, y, px, py) + this.V(x, y);
        }

        /// <summary>
        /// Celkov� energie
        /// </summary>
        /// <param name="x">Sou�adnice a hybnosti</param>
        public double E(Vector x) {
            return this.T(x[0], x[1], x[2], x[3]) + this.V(x[0], x[1]);
        }

        /// <summary>
        /// Prav� strana Hamiltonov�ch pohybov�ch rovnic
        /// </summary>
        /// <param name="x">Sou�adnice a hybnosti</param>
        public virtual Vector Equation(Vector x) {
            Vector result = new Vector(4);

            double b2 = x[0] * x[0] + x[1] * x[1];
            double dVdx = 2.0 * this.A * x[0] + 3.0 * this.B * (x[0] * x[0] - x[1] * x[1]) + 4.0 * this.C * x[0] * b2;
            double dVdy = 2.0 * x[1] * (this.A - 3.0 * this.B * x[0] + 2.0 * this.C * b2);

            result[0] = x[2] / this.K;
            result[1] = x[3] / this.K;

            result[2] = - dVdx;
            result[3] = - dVdy;

            return result;
        }

        /// <summary>
        /// Matice pro v�po�et SALI (Jakobi�n)
        /// </summary>
        /// <param name="x">Vektor x v �ase t</param>
        public virtual Matrix Jacobian(Vector x) {
            Matrix result = new Matrix(4, 4);

            double b2 = x[0] * x[0] + x[1] * x[1];
            double dV2dxdx = 2.0 * this.A + 6.0 * this.B * x[0] + 4.0 * this.C * (3.0 * x[0] * x[0] + x[1] * x[1]);
            double dV2dxdy = (-6.0 * this.B + 8.0 * this.C * x[0]) * x[1];
            double dV2dydy = 2.0 * (this.A - 3.0 * this.B * x[0] + 2.0 * this.C * (x[0] * x[0] + 3.0 * x[1] * x[1]));

            result[0, 2] = 1 / this.K;
            result[1, 3] = result[0, 2];

            result[2, 0] = -dV2dxdx;
            result[2, 1] = -dV2dxdy;

            result[3, 0] = result[2, 1];
            result[3, 1] = -dV2dydy;

            return result;
        }

        /// <summary>
        /// Po�et stup�� volnosti
        /// </summary>
        public int DegreesOfFreedom { get { return degreesOfFreedom; } }

        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        public SimpleClassicalGCM() { }

        /// <summary>
        /// Konstruktor standardn�ho Lagrangi�nu
        /// </summary>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr K</param>
        public SimpleClassicalGCM(double a, double b, double c, double k)
            : base(a, b, c, k) {
        }

        /// <summary>
        /// Generuje po��te�n� podm�nky rovnom�rn� ve FP
        /// </summary>
        /// <param name="e">Energie</param>
        /// <returns>Po��te�n� podm�nky ve form�tu (x, y, px, py)</returns>
        public Vector IC(double e) {
            Vector result = new Vector(4);
            
            Vector r = this.Roots(e, 0);
            if(r.Length == 0)
                throw new GCMException(string.Format(errorMessageInitialCondition, e));

            // Nalezen� nejv�t��ho ko�enu (v absolutn� hodnot�)
            double rmax = System.Math.Abs(r[0]);
            for(int i = 1; i < r.Length; i++)
                if(System.Math.Abs(r[i]) > rmax)
                    rmax = System.Math.Abs(r[i]);
            do {
                // Po��te�n� podm�nky v poloze hled�me ve �verci (-rmax, rmax) x (-rmax, rmax)
                result[0] = (this.random.NextDouble() * 2.0 - 1) * rmax;
                result[1] = (this.random.NextDouble() * 2.0 - 1) * rmax;

                result[2] = 0.0;
                result[3] = 0.0;

                if(this.E(result) < e)
                    if(this.IC(result, e))
                        break;

            } while(true);

            return result;
        }

        /// <summary>
        /// Generov�n� hybnost� v po��te�n� podm�nce
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="ic">Polohy po�. podm�nky</param>
        /// <returns>True, pokud se generov�n� poda�ilo</returns>        
        public bool IC(Vector ic, double e) {
            // Koeficient p�ed kinetick�m �lenem
            double koef = this.T(ic[0], ic[1], 1.0, 0.0);
            double tbracket = (e - this.V(ic[0], ic[1])) / koef;

            if(ic[2] == 0.0) {
                ic[2] = this.random.NextDouble() * System.Math.Sqrt(tbracket);

                if(this.random.Next(2) == 0)
                    ic[2] = -ic[2];
            }

            tbracket -= ic[2] * ic[2];
            if(tbracket < 0.0)
                return false;

            ic[3] = System.Math.Sqrt(tbracket);

            if(this.random.Next(2) == 0)
                ic[3] = -ic[3];

            return true;
        }

        /// <summary>
        /// Generuje po��te�n� podm�nky ve FP
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="l">Impulsmoment</param>
        public Vector IC(double e, double l) {
            if(l == 0.0)
                return this.IC(e);
            else
                throw new GCMException(string.Format(errorMessageNonzeroJ, this.GetType().FullName, typeof(ClassicalGCMJ).FullName));
        }

        /// <summary>
        /// Vyp�e parametry GCM modelu
        /// </summary>
        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("A = {0,10:#####0.000}\nB = {1,10:#####0.000}\nC = {2,10:#####0.000}\nK = {3,10:#####0.000}", this.A, this.B, this.C, this.K));
            s.Append(string.Format("I = {0,10:#####0.000}", this.Invariant));
            s.Append("\n\n");

            Vector beta = this.ExtremalBeta(0.0);
            s.Append(string.Format("Extr�my:"));

            for(int i = 0; i < beta.Length; i++)
                s.Append(string.Format("\nV({0,1:0.000}) = {1,1:0.000}", beta[i], this.VBG(beta[i], 0.0)));

            return s.ToString();
        }

        #region Implementace IExportable
        /// <summary>
        /// Ulo�� GCM t��du do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public virtual void Export(Export export) {
            if(export.Binary) {
                // Bin�rn�
                BinaryWriter b = export.B;
                b.Write(this.A);
                b.Write(this.B);
                b.Write(this.C);
                b.Write(this.K);
            }
            else {
                // Textov�
                StreamWriter t = export.T;
                t.WriteLine("{0}\t{1}\t{2}\t{3}", this.A, this.B, this.C, this.K);
            }
        }

        /// <summary>
        /// Na�te GCM t��du ze souboru textov�
        /// </summary>
        /// <param name="import">Import</param>
        public virtual void Import(Import import) {
            if(import.Binary) {
                // Bin�rn�
                BinaryReader b = import.B;
                this.A = b.ReadDouble();
                this.B = b.ReadDouble();
                this.C = b.ReadDouble();
                this.K = b.ReadDouble();
            }
            else {
                // Textov�
                StreamReader t = import.T;
                string line = t.ReadLine();
                string[] s = line.Split('\t');
                this.A = double.Parse(s[0]);
                this.B = double.Parse(s[1]);
                this.C = double.Parse(s[2]);
                this.K = double.Parse(s[3]);
            }
        }
        #endregion

        private const double poincareTime = 100;
        private const int degreesOfFreedom = 2;

        private const string errorMessageInitialCondition = "Pro zadanou energii {0} nelze nagenerovat po��te�n� podm�nky.";
        private const string errorMessageNonzeroJ = "T��da {0} um� po��tat pouze s nulov�m �hlov�m momentem. Pro nenulov� �hlov� moment pou�ij {1}.";
    }
}