using System;
using System.IO;
using System.Collections;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;
using PavelStransky.Systems;

namespace PavelStransky.Systems {
    /// <summary>
    /// Tøída GCM modelu s nenulovým úhlovým momentem
    /// </summary>
    public class ClassicalGCMJ: GCMParameters, IDynamicalSystem, IExportable {
        // Generátor náhodných èísel
        private Random random = new Random();

        /// <summary>
        /// Kinetická energie
        /// </summary>
        /// <param name="px">Hybnost x</param>
        /// <param name="py">Hybnost y</param>
        /// <param name="pz">Hybnost z</param>
        /// <param name="pu">Hybnost u</param>
        /// <param name="pv">Hybnost v</param>
        /// <returns></returns>
        public double T(double px, double py, double pz, double pu, double pv) {
            return 1.0 / (2.0 * this.K) * (px * px + py * py + pz * pz + pu * pu + pv * pv);
        }

        /// <summary>
        /// Kinetická energie
        /// </summary>
        /// <param name="px">Vektor hybností</param>
        public double T(Vector px) {
            return 1.0 / (2 * this.K) * px.SquaredEuklideanNorm(5, 10);
        }

        /// <summary>
        /// Koeficienty pøed rovnicí V(x_{i}) = e. Pro dané i uspoøádané v øádku matice
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="x">Souøadnice</param>
        public Matrix Coeficient(double e, Vector x) {
            Matrix result = new Matrix(5);

            double b2 = x.SquaredEuklideanNorm(0, 5);

            Vector c = new Vector(28);
            Matrix p = new Matrix(28, 5);

            c[0] = c[1] = c[2] = c[3] = c[4] = this.A; p[0, 0] = p[1, 1] = p[2, 2] = p[3, 3] = p[4, 4] = 2;

            c[5] = this.B; p[5, 0] = 3;
            c[6] = c[7] = -3.0 * this.B; p[6, 0] = p[7, 0] = 1; p[6, 1] = p[7, 2] = 2;
            c[8] = c[9] = 1.5 * this.B; p[8, 0] = p[9, 0] = 1; p[8, 3] = p[9, 4] = 2;
            c[10] = 1.5 * System.Math.Sqrt(3.0) * this.B; p[10, 1] = 1; p[10, 3] = 2;
            c[11] = c[10]; p[11, 1] = 1; p[11, 4] = 2;
            c[12] = 3.0 * System.Math.Sqrt(3.0) * this.B; p[12, 2] = p[12, 3] = p[12, 4] = 1;

            c[13] = c[14] = c[15] = c[16] = c[17] = this.C; p[13, 0] = p[14, 1] = p[15, 2] = p[16, 3] = p[17, 4] = 4;
            c[18] = c[19] = c[20] = c[21] = 2.0 * this.C; p[18, 0] = p[19, 0] = p[20, 0] = p[21, 0] = 2; p[18, 1] = p[19, 2] = p[20, 3] = p[21, 4] = 2;
            c[22] = c[23] = c[24] = c[18]; p[22, 1] = p[23, 1] = p[24, 1] = 2; p[22, 2] = p[23, 3] = p[24, 4] = 2;
            c[25] = c[26] = c[18]; p[25, 2] = p[26, 2] = 2; p[25, 3] = p[26, 4] = 2;
            c[27] = c[18]; p[27, 3] = 2; p[27, 4] = 2;

            // Cyklus pøes jednotlivé promìnné
            for(int i = 0; i < result.LengthX; i++)
                // Cyklus pøes jednotlivé mocniny
                for(int j = 0; j < result.LengthY; j++) {
                    // Cyklus pøes všechny pøíspìvky
                    for(int k = 0; k < p.LengthX; k++) {
                        double d = 0;

                        // Pøíspìvek má správnou mocninu
                        if(p[k, i] == j) {
                            d = c[k];

                            // Cyklus pøes všechny promìnné
                            for(int l = 0; l < p.LengthY; l++)
                                // Nemáme promìnnou, pro kterou koeficienty urèujeme - násobíme mocninou x[l]
                                if(l != i && p[k, l] != 0)
                                    d *= System.Math.Pow(x[l], p[k, l]);

                        }

                        result[i, j] += d;
                    }

                    if(j == 0)
                        result[i, j] -= e;
                }

            return result;
        }

        /// <summary>
        /// Potenciál
        /// </summary>
        /// <param name="x">Souøadnice x</param>
        /// <param name="y">Souøadnice y</param>
        /// <param name="z">Souøadnice z</param>
        /// <param name="u">Souøadnice u</param>
        /// <param name="v">Souøadnice v</param>
        public double V(double x, double y, double z, double u, double v) {
            double b = x * x + y * y + z * z + u * u + v * v;
            return this.A * b + this.B * (x * (x * x - 3 * (y * y + z * z)) + 1.5 * (v * v * (x - System.Math.Sqrt(3.0) * y) + u * u * (x + System.Math.Sqrt(3.0) * y)) + 3.0 * System.Math.Sqrt(3.0) * u * v * z) + this.C * b * b;
        }

        /// <summary>
        /// Potenciál
        /// </summary>
        /// <param name="x">Souøadnice (a hybnosti)</param>
        public double V(Vector x) {
            double b = x.SquaredEuklideanNorm(0, 5);
            return this.A * b + this.B * (x[iX] * (x[iX] * x[iX] - 3 * (x[iY] * x[iY] + x[iZ] * x[iZ])) + 1.5 * (x[iV] * x[iV] * (x[iX] - System.Math.Sqrt(3.0) * x[iY]) + x[iU] * x[iU] * (x[iX] + System.Math.Sqrt(3.0) * x[iY])) + 3.0 * System.Math.Sqrt(3.0) * x[iU] * x[iV] * x[iZ]) + this.C * b * b;
        }

        /// <summary>
        /// Celková energie
        /// </summary>
        /// <param name="x">Souøadnice a hybnosti</param>
        public double E(Vector x) {
            return this.T(x) + this.V(x);
        }

        /// <summary>
        /// Úhlový moment
        /// </summary>
        /// <param name="x">Souøadnice a hybnosti</param>
        public Vector J(Vector x) {
            Vector result = new Vector(3);

            result[0] = -x[iVd] * (System.Math.Sqrt(3.0) * x[iX] + x[iY]) - x[iV] * (System.Math.Sqrt(3.0) * x[iXd] + x[iYd]) - x[iUd] * x[iZ] - x[iU] * x[iZd];
            result[1] = -x[iUd] * (System.Math.Sqrt(3.0) * x[iX] - x[iY]) + x[iU] * (System.Math.Sqrt(3.0) * x[iXd] - x[iYd]) - x[iVd] * x[iZ] + x[iV] * x[iZd];
            result[2] = -x[iU] * x[iVd] - x[iUd] * x[iV] - 2.0 * (x[iY] * x[iZd] + x[iYd] * x[iZ]);

            return result;
        }

        /// <summary>
        /// Pravá strana pohybové rovnice (rovnice 2. øádu)
        /// </summary>
        /// <param name="x">Souøadnice a rychlosti</param>
        public Vector Equation(Vector x) {
            Vector result = new Vector(10);

            result[0] = x[iXd] / this.K;
            result[1] = x[iYd] / this.K;
            result[2] = x[iZd] / this.K;
            result[3] = x[iUd] / this.K;
            result[4] = x[iVd] / this.K;

            double b2 = x.SquaredEuklideanNorm(0, 5);

            double dVdx = 2.0 * this.A * x[iX] + 3.0 * this.B * (x[iX] * x[iX] - x[iY] * x[iY] - x[iZ] * x[iZ] + 0.5 * (x[iU] * x[iU] + x[iV] * x[iV])) + 4.0 * this.C * x[iX] * b2;
            double dVdy = 2.0 * this.A * x[iY] - 3.0 * this.B * (2.0 * x[iX] * x[iY] + 0.5 * System.Math.Sqrt(3.0) * (x[iV] * x[iV] - x[iU] * x[iU])) + 4.0 * this.C * x[iY] * b2;
            double dVdz = 2.0 * this.A * x[iZ] - 3.0 * this.B * (2.0 * x[iX] * x[iZ] - System.Math.Sqrt(3.0) * x[iU] * x[iV]) + 4.0 * this.C * x[iZ] * b2;
            double dVdu = 2.0 * this.A * x[iU] + 3.0 * this.B * (x[iX] * x[iU] + System.Math.Sqrt(3.0) * (x[iU] * x[iY] + x[iV] * x[iZ])) + 4.0 * this.C * x[iU] * b2;
            double dVdv = 2.0 * this.A * x[iV] + 3.0 * this.B * (x[iX] * x[iV] + System.Math.Sqrt(3.0) * (x[iU] * x[iZ] - x[iV] * x[iY])) + 4.0 * this.C * x[iV] * b2;

            result[5] = -dVdx;
            result[6] = -dVdy;
            result[7] = -dVdz;
            result[8] = -dVdu;
            result[9] = -dVdv;

            return result;
        }

        /// <summary>
        /// Matice pro výpoèet SALI (Jakobián)
        /// </summary>
        /// <param name="x">Vektor x v èase t</param>
        public Matrix Jacobian(Vector x) {
            Matrix result = new Matrix(10, 10);

            double b2 = x.SquaredEuklideanNorm(0, 5);

            double dV2dxdx = 2.0 * this.A + 6.0 * this.B * x[iX] + 4.0 * this.C * b2 + 8.0 * this.C * x[iX] * x[iX];
            double dV2dxdy = (-6.0 * this.B + 8.0 * this.C * x[iX]) * x[iY];
            double dV2dxdz = (-6.0 * this.B + 8.0 * this.C * x[iX]) * x[iZ];
            double dV2dxdu = (3.0 * this.B + 8.0 * this.C * x[iX]) * x[iU];
            double dV2dxdv = (3.0 * this.B + 8.0 * this.C * x[iX]) * x[iV];

            double dV2dydy = 2.0 * this.A - 6.0 * this.B * x[iX] + 4.0 * this.C * b2 + 8.0 * this.C * x[iY] * x[iY];
            double dV2dydz = 8.0 * this.C * x[iY] * x[iZ];
            double dV2dydu = (3.0 * System.Math.Sqrt(3.0) * this.B + 8.0 * this.C * x[iY]) * x[iU];
            double dV2dydv = (-3.0 * System.Math.Sqrt(3.0) * this.B + 8.0 * this.C * x[iY]) * x[iV];

            double dV2dzdz = 2.0 * this.A - 6.0 * this.B * x[iX] + 4.0 * this.C * b2 + 8.0 * this.C * x[iZ] * x[iZ];
            double dV2dzdu = 3.0 * System.Math.Sqrt(3.0) * this.B * x[iV] + 8.0 * this.C * x[iZ] * x[iU];
            double dV2dzdv = 3.0 * System.Math.Sqrt(3.0) * this.B * x[iU] + 8.0 * this.C * x[iZ] * x[iV];

            double dV2dudu = 2.0 * this.A + 3.0 * this.B * (x[iX] + System.Math.Sqrt(3.0) * x[iY]) + 4.0 * this.C * b2 + 8.0 * this.C * x[iU] * x[iU];
            double dV2dudv = 3.0 * System.Math.Sqrt(3.0) * this.B * x[iZ] + 8.0 * this.C * x[iU] * x[iV];

            double dV2dvdv = 2.0 * this.A + 3.0 * this.B * (x[iX] - System.Math.Sqrt(3.0) * x[iY]) + 4.0 * this.C * b2 + 8.0 * this.C * x[iV] * x[iV];

            result[0, 5] = 1 / this.K;
            result[1, 6] = 1 / this.K;
            result[2, 7] = 1 / this.K;
            result[3, 8] = 1 / this.K;
            result[4, 9] = 1 / this.K;

            result[5, 0] = -dV2dxdx;
            result[5, 1] = -dV2dxdy;
            result[5, 2] = -dV2dxdz;
            result[5, 3] = -dV2dxdu;
            result[5, 4] = -dV2dxdv;

            result[6, 0] = result[5, 1];
            result[6, 1] = -dV2dydy;
            result[6, 2] = -dV2dydz;
            result[6, 3] = -dV2dydu;
            result[6, 4] = -dV2dydv;

            result[7, 0] = result[5, 2];
            result[7, 1] = result[6, 2];
            result[7, 2] = -dV2dzdz;
            result[7, 3] = -dV2dzdu;
            result[7, 4] = -dV2dzdv;

            result[8, 0] = result[5, 3];
            result[8, 1] = result[6, 3];
            result[8, 2] = result[7, 3];
            result[8, 3] = -dV2dudu;
            result[8, 4] = -dV2dudv;

            result[9, 0] = result[5, 4];
            result[9, 1] = result[6, 4];
            result[9, 2] = result[7, 4];
            result[9, 3] = result[8, 4];
            result[9, 4] = -dV2dvdv;

            return result;
        }

        /// <summary>
        /// Kostruktor standardního lagranžiánu
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <param name="c">C</param>
        /// <param name="k">D</param>
        public ClassicalGCMJ(double a, double b, double c, double k)
            : base(a, b, c, k) {
        }

        /// <summary>
        /// Poèítá øešení rovnice V(x_{i})|x,y,z,u,v = E a vrací všechny nalezené koøeny
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="i">Index promìnné, pro kterou se meze poèítají</param>
        /// <param name="x">Souøadnice</param>
        public Vector Roots(double e, int i, Vector x) {
            Matrix c = this.Coeficient(e, x);
            Vector polynom = c.GetRowVector(i);

            return Polynom.SolveR(polynom).Sort() as Vector;
        }

        /// <summary>
        /// Poèítá øešení rovnice V(x_{i})|x,y,z,u,v = E a vrací všechny nalezené nenulové koøeny
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="i">Index promìnné, pro kterou se meze poèítají</param>
        /// <param name="x">Souøadnice</param>
        public Vector NonzeroRoots(double e, int i, Vector x) {
            Vector r = this.Roots(e, i, x);

            int zeroRoots = 0;

            for(int j = 0; j < r.Length - zeroRoots; j++) {
                if(System.Math.Abs(r[j]) < zeroValue) {
                    zeroRoots++;

                    for(int k = j + 1; k < r.Length; k++)
                        r[k - 1] = r[k];

                    j--;
                }
            }

            r.Length = r.Length - zeroRoots;

            return r;
        }

        #region IDynamicalSystem Members
        /// <summary>
        /// Generuje poèáteèní podmínky
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="l">Úhlový moment</param>
        public Vector IC(double e, double l) {
            int iteration = 0;

            // Nalezení maximálního rozmìru x
            Vector nullX = new Vector(5);
            double maxX = 0;
            for(int i = 0; i <= iV; i++)
                maxX = System.Math.Max(maxX, System.Math.Abs(this.NonzeroRoots(e, i, nullX).MaxAbs()));

            // Hledáme, dokud se netrefíme do takových poèáteèních podmínek, které umí splnit podmínku na L i E
            for(int iterationX = 0; iterationX < maxIteration; iteration++) {
                Vector result = new Vector(10);

                try {
                    // Potenciální èlen
                    for(int i = 0; i <= iV; i++)
                        result[i] = this.random.NextDouble() * 2 * maxX - maxX;

                    // Energie je nižší než nagenerovaný potenciál - musíme generovat znovu
                    if(e < this.V(result))
                        continue;

                    // Zkoušíme nejprve nìkolikrát pro daný potenciální èlen najít èlen kinetický
                    for(int iterationP = 0; iterationP < maxIteration; iterationP++) {
                        // Kinetický èlen
                        double tbracket = (e - this.V(result)) * 2.0 / this.K;

                        // Generování xd, yd, zd
                        for(int i = iXd; i <= iZd; i++) {
                            result[i] = this.random.NextDouble() * System.Math.Sqrt(tbracket);
                            if(this.random.Next(2) == 0)
                                result[i] = -result[i];
                            tbracket -= result[i] * result[i];
                        }

                        // Dopoèítání ud, vd
                        double Ax = System.Math.Sqrt(3.0) * result[iX] + result[iY];
                        double Bx = -(System.Math.Sqrt(3.0) * result[iXd] + result[iYd]) * result[iV] + result[iU] * result[iZd];
                        double Cx = -(System.Math.Sqrt(3.0) * result[iX] - result[iY]);
                        double Dx = (System.Math.Sqrt(3.0) * result[iXd] - result[iYd]) * result[iU] - result[iV] * result[iZd];
                        double Ex = 2.0 * (result[iY] * result[iZd] - result[iYd] * result[iZ]);

                        double Xx = Ax * Ax + result[iZ] * result[iZ] + result[iU] * result[iU];
                        double Yx = result[iZ] * result[iZ] + Cx * Cx + result[iV] * result[iV];
                        double Zx = 2.0 * (-Ax * result[iZ] + Cx * result[iZ] - result[iU] * result[iV]);
                        double Ux = 2.0 * (-Bx * result[iZ] + Cx * Dx - result[iV] * Ex);
                        double Vx = 2.0 * (Ax * Bx + result[iZ] * Dx + result[iU] * Ex);
                        double Wx = Bx * Bx + Dx * Dx + Ex * Ex;

                        double lambda = l * l / (this.K * this.K) - Wx;
                        double h = tbracket;

                        double Px = lambda - h * Xx;
                        double Qx = Xx - Yx;

                        Vector polynom = new Vector(5);
                        polynom[0] = Px * Px - Vx * Vx * h;
                        polynom[1] = -2.0 * (Px * Ux + Zx * Vx * h);
                        polynom[2] = Ux * Ux + 2.0 * Px * Qx - Zx * Zx * h + Vx * Vx;
                        polynom[3] = -2.0 * Ux * Qx + 2.0 * Zx * Vx;
                        polynom[4] = Qx * Qx + Zx * Zx;

                        Vector roots = Polynom.SolveR(polynom).Sort() as Vector;

                        // Zkoušíme náhodnì všechny koøeny
                        while(roots.Length != 0) {
                            int i = this.random.Next(roots.Length);
                            result[iUd] = roots[i];
                            roots[i] = roots.LastItem;
                            roots.Length -= 1;

                            double t = tbracket - result[iUd] * result[iUd];
                            if(t < 0)
                                continue;

                            result[iVd] = System.Math.Sqrt(t);
                            result[iZd] = -result[iZd];

                            for(int k = 5; k < 10; k++)
                                result[k] *= this.K;

                            // Mùže být ještì obrácené znaménko?
                            if(System.Math.Abs(this.J(result).EuklideanNorm() - System.Math.Abs(l)) < zeroValue) {
                                double ex = this.E(result);
                                double Jx = this.J(result).EuklideanNorm();

                                return result;
                            }

                            // Mùže být ještì obrácené znaménko?
                            result[iVd] = -result[iVd];

                            if(System.Math.Abs(this.J(result).EuklideanNorm() - System.Math.Abs(l)) < zeroValue) {
                                double ex = this.E(result);
                                double Jx = this.J(result).EuklideanNorm();

                                return result;
                            }
                        }
                    }
                }
                catch(Exception) {
                }
            }

            throw new Exception("Chyba, pøekroèen poèet iterací");
        }

        /// <summary>
        /// Generuje poèáteèní podmínky pro nulový úhlový moment
        /// </summary>
        /// <param name="e">Energie</param>
        public Vector IC(double e) {
            return this.IC(e, 0.0);
        }

        /// <summary>
        /// Postprocessing
        /// </summary>
        /// <param name="x">Souøadnice a hybnosti</param>
        public bool PostProcess(Vector x) {
            return false;
        }

        public bool IC(Vector ic, double e) {
            throw new Exception("The method or operation is not implemented.");
        }

        public Vector Bounds(double e) {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Kontrola poèáteèních podmínek
        /// </summary>
        /// <param name="bounds">Poèáteèní podmínky</param>
        public Vector CheckBounds(Vector bounds) {
            return bounds;
        }

        public double PeresInvariant(Vector x) {
            throw new Exception("The method or operation is not implemented.");
        }

        public int DegreesOfFreedom {
            get { return degreesOfFreedom; }
        }

        /// <summary>
        /// Body pro rozhodnutí, zda je podle SALI daná trajektorie regulární nebo chaotická
        /// </summary>
        /// <returns>[time chaotická, SALI chaotická, time regulární, SALI regulární, time koncový bod, SALI koncový bod]</returns>
        public double[] SALIDecisionPoints() {
            return new double[] { 0, 5, 500, 0, 1000, 10 };
        }
        #endregion

        #region Implementace IExportable
        /// <summary>
        /// Uloží GCM tøídu do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.A, "A");
            param.Add(this.B, "B");
            param.Add(this.C, "C");
            param.Add(this.K, "K");

            param.Export(export);
        }

        /// <summary>
        /// Naète GCM tøídu ze souboru textovì
        /// </summary>
        /// <param name="import">Import</param>
        public ClassicalGCMJ(Core.Import import) {
            IEParam param = new IEParam(import);

            this.A = (double)param.Get(-1.0);
            this.B = (double)param.Get(1.0);
            this.C = (double)param.Get(1.0);
            this.K = (double)param.Get(1.0);
        }
        #endregion

        private const int degreesOfFreedom = 5;
        private const double zeroValue = 1E-6;

        private const int iX = 0, iY = 1, iZ = 2, iU = 3, iV = 4;
        private const int iXd = 5, iYd = 6, iZd = 7, iUd = 8, iVd = 9;

        private const int maxIteration = 100;
    }
}