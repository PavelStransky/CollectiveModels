using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public class ClassicalEP: ExtensiblePendulum, IDynamicalSystem, IGeometricalMethod {
        // Gener�tor n�hodn�ch ��sel
        private Random random = new Random();

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="nu">Parametr modelu</param>
        public ClassicalEP(double nu)
            : base(nu) { }

        /// <summary>
        /// Kinetick� energie
        /// </summary>
        /// <param name="px">Hybnost x</param>
        /// <param name="py">Hybnost y</param>
        public double T(double px, double py) {
            return 0.5 * (px * px + py * py);
        }

        /// <summary>
        /// Potenci�ln� energie
        /// </summary>
        /// <param name="x">Vertik�ln� sou�adnice</param>
        /// <param name="y">Horizont�ln� sou�adnice</param>
        public double V(double x, double y) {
            double d = System.Math.Sqrt(x * x + y * y) - 1.0;
            return -this.Nu * x + 0.5 * d * d;
        }

        /// <summary>
        /// Celkov� energie
        /// </summary>
        /// <param name="px">Hybnost x</param>
        /// <param name="py">Hybnost y</param>
        /// <param name="x">Horizont�ln� sou�adnice</param>
        /// <param name="y">Vertik�ln� sou�adnice</param>
        public double E(double x, double y, double px, double py) {
            return this.T(px, py) + this.V(x, y);
        }

        /// <summary>
        /// Minim�ln� energie (kyvadlo vis� dol�, d�lka je L)
        /// </summary>
        public double Emin() {
            return -this.Nu * (this.Nu / 2.0 + 1.0);
        }

        /// <summary>
        /// Celkov� energie
        /// </summary>
        /// <param name="x">Sou�adnice a hybnosti</param>
        public double E(Vector x) {
            return this.T(x[2], x[3]) + this.V(x[0], x[1]);
        }

        /// <summary>
        /// Prav� strana Hamiltonov�ch pohybov�ch rovnic
        /// </summary>
        /// <param name="v">Sou�adnice a hybnosti</param>
        public Vector Equation(Vector v) {
            Vector result = new Vector(4);

            double x = v[0];
            double y = v[1];

            double r = System.Math.Sqrt(x * x + y * y);
            double d = 1.0 - 1.0 / r;

            double dVdx = -this.Nu + x * d;
            double dVdy = y * d;

            result[0] = v[2];
            result[1] = v[3];

            result[2] = -dVdx;
            result[3] = -dVdy;

            return result;
        }

        /// <summary>
        /// Matice pro v�po�et SALI (Jakobi�n)
        /// </summary>
        /// <param name="x">Vektor x v �ase t</param>
        public Matrix Jacobian(Vector v) {
            Matrix result = new Matrix(4);

            double x = v[0];
            double y = v[1];

            double r = System.Math.Sqrt(x * x + y * y);
            double r3 = r * r * r;

            double dV2dxdx = 1.0 - y * y / r3;
            double dV2dxdy = x * y / r3;
            double dV2dydy = 1.0 - x * x / r3;

            result[0, 2] = 1.0;
            result[1, 3] = result[0, 2];

            result[2, 0] = -dV2dxdx;
            result[2, 1] = -dV2dxdy;

            result[3, 0] = result[2, 1];
            result[3, 1] = -dV2dydy; 
            
            return result;
        }

        #region Implementace IExportable
        /// <summary>
        /// Konstruktor pro import
        /// </summary>
        /// <param name="import">Import</param>
        public ClassicalEP(Import import) : base(import) { }
        #endregion

        #region IDynamicalSystem Members
        /// <summary>
        /// Po��te�n� podm�nky p�i dan� energii
        /// </summary>
        /// <param name="e">Energie</param>
        public Vector IC(double e) {
            Vector result = new Vector(4);

            if(e < this.Emin())
                throw new SystemException();

            double d = 0.0;
            double cnu = 0.0;
            double phi = 0.0;
            do {
                // Nejlep�� je generovat po��te�n� podm�nky v rovin� r, phi (pol�rn� sou�adnice)
                phi = 2.0 * System.Math.PI * this.random.NextDouble();
                // Diskriminant            
                cnu = System.Math.Cos(phi) * this.Nu;
                d = 2.0 * e + cnu * (cnu + 2);
            } while(d < 0);

            d = System.Math.Sqrt(d);

            // Ko�eny
            double r1 = 1 + cnu - d;
            double r2 = 1 + cnu + d;

            double r = 2.0 * this.random.NextDouble() * d + r1;

            double x = r * System.Math.Cos(phi);
            double y = r * System.Math.Sin(phi);

            double t = e - this.V(x, y);

            double px = this.random.NextDouble() * System.Math.Sqrt(2.0 * t);
            if(this.random.Next(2) == 0) px = -px;
            double py = System.Math.Sqrt(2.0 * t - px * px);
            if(this.random.Next(2) == 0) py = -py;

            result[0] = x;
            result[1] = y;
            result[2] = px;
            result[3] = py;

            return result;
        }

        /// <summary>
        /// Po��te�n� podm�nky p�i dan� energii; dopo��t�v�me jen hodnoty, kter� jsou double.NaN
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="ic">Po��te�n� podm�nky</param>
        /// <returns>True, pokud se po��te�n� podm�nky poda�ilo nagenerovat</returns>
        public bool IC(Vector ic, double e) {
            double x = ic[0];
            double y = ic[1];
            double px = ic[2];
            double py = ic[3];

            double t = e - this.V(x, y);
            if(t < 0)
                return false;

            if(double.IsNaN(py)) {
                if(double.IsNaN(px)) {
                    px = this.random.NextDouble() * System.Math.Sqrt(2.0 * t);
                    if(this.random.Next(2) == 0) px = -px;
                }
                py = System.Math.Sqrt(2.0 * t - px * px);
                if(this.random.Next(2) == 0) py = -py;
            }
            else if(double.IsNaN(px)) {
                px = System.Math.Sqrt(2.0 * t - py * py);
                if(this.random.Next(2) == 0) px = -px;
            }

            ic[2] = px;
            ic[3] = py;

            if(double.IsNaN(px) || double.IsNaN(py))
                return false;

            return true;
        }

        public Vector IC(double e, double l) {
            throw new Exception("The method or operation is not implemented.");
        }

        private class MinimumFunction {
            private double nu, e;
            private bool isX;

            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="nu">Parametr modelu</param>
            /// <param name="e">Energie</param>
            /// <param name="isX">True, pokud hled�me extr�m x, jinak False</param>
            public MinimumFunction(double nu, double e, bool isX) {
                this.nu = nu;
                this.e = e;
                this.isX = isX;
            }

            public double Minimum(double phi) {
                // Diskriminant            
                double cnu = System.Math.Cos(phi) * this.nu;
                double d = 2.0 * this.e + cnu * (cnu + 2);
                if(d < 0)
                    return this.isX ? 2.0 + this.nu + System.Math.Sqrt(2.0 * this.e + this.nu * (this.nu + 2.0)) : 0.0;

                d = System.Math.Sqrt(d);
                double r1 = 1 + cnu - d;
                double r2 = 1 + cnu + d;

                double x1 = this.isX ? r1 * System.Math.Cos(phi) : -r1 * System.Math.Sin(phi);
                double x2 = this.isX ? r2 * System.Math.Cos(phi) : -r2 * System.Math.Sin(phi);

                return System.Math.Min(x1, x2);
            }
        }

        /// <summary>
        /// Meze dynamick�ch prom�nn�ch p�i dan� energii
        /// </summary>
        /// <param name="e">Energie</param>
        public Vector Bounds(double e) {
            Vector result = new Vector(8);

            result[1] = 1.0 + this.Nu + System.Math.Sqrt(2.0 * e + this.Nu * (this.Nu + 2.0));

            // Zbytek mez� trivi�ln�m proch�zen�m
            double maxy = 0, minx = result[1];
            for(double phi = 0; phi <= System.Math.PI; phi += System.Math.PI / 100000) {
                double cnu = System.Math.Cos(phi) * this.Nu;
                double d = 2.0 * e + cnu * (cnu + 2);
                if(d < 0)
                    continue;

                d = System.Math.Sqrt(d);
                double r1 = 1 + cnu - d;
                double r2 = 1 + cnu + d;

                maxy = System.Math.Max(maxy, System.Math.Max(r1 * System.Math.Sin(phi), r2 * System.Math.Sin(phi)));
                minx = System.Math.Min(minx, System.Math.Min(r1 * System.Math.Cos(phi), r2 * System.Math.Cos(phi)));
                double ee = this.V(minx, 0);
//                if(ee > e)
//                    break;
            }
            result[0] = minx;
            result[2] = -maxy;
            result[3] = maxy;

            result[5] = System.Math.Sqrt(2.0 * (e-this.Emin()));
            result[4] = -result[5];

            result[6] = result[4];
            result[7] = result[5];

            return result;
        }

        /// <summary>
        /// Kontrola po��te�n�ch podm�nek
        /// </summary>
        /// <param name="bounds">Po��te�n� podm�nky</param>
        public Vector CheckBounds(Vector bounds) {
            return bounds;
        }

        public int DegreesOfFreedom { get { return 2; } }

        public double PeresInvariant(Vector x) {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Postprocessing
        /// </summary>
        /// <param name="x">Sou�adnice a hybnosti</param>
        public bool PostProcess(Vector x) {
            return false;
        }

        /// <summary>
        /// Napo��t� matici V
        /// (podle PRL 98, 234301 (2007))
        /// </summary>
        /// <param name="e">Energie</param>
        public Matrix VMatrix(double e, double x, double y) {
            double b = 1.0 / System.Math.Sqrt(x * x + y * y);
            double b3 = b * b * b;

            double vx = x * (1.0 - b) - this.Nu;
            double vy = y * (1.0 - b);

            double vxx = 1 - y * y * b3;
            double vxy = x * y * b3;
            double vyy = 1 - x * x * b3;

            double a = 3.0 / System.Math.Abs((2.0 * (e - this.V(x, y))));

            Matrix result = new Matrix(2);
            result[0, 0] = a * vx * vx + vxx;
            result[0, 1] = a * vx * vy + vxy;
            result[1, 0] = result[0, 1];
            result[1, 1] = a * vy * vy + vyy;

            return result;
        }

        /// <summary>
        /// Vytvo�� t��du ekvipotenci�ln� k�ivky
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="div">D�len� intervalu 2pi (celkov� po�et bod� v�po�tu)</param>
        private Contour CreateContour(double e, int div) {
            Contour contour = new Contour();

            if(div <= 0)
                div = defaultDivision;

            contour.Begin();
            for(int i = 1; i < div; i++) {
                double phi = 2.0 * i * System.Math.PI / div;
                double b = 1.0 + this.Nu * System.Math.Cos(phi);
                double d = b * b - 1.0 + 2.0 * e;

                if(d >= 0.0) {
                    d = System.Math.Sqrt(d);
                    double r1 = b + d;
                    double r2 = b - d;

                    if(r1 > 0)
                        contour.Add(0, r1, phi);
                    if(r2 > 0)
                        contour.Add(1, r2, phi);
                }
            }
            contour.End();

            contour.RemoveShort();
            contour.Join();

            return contour;
        }

        /// <summary>
        /// Vypo��t� ekvipotenci�ln� k�ivky
        /// </summary>
        /// <param name="e">Enegie</param>
        /// <param name="n">Maxim�ln� po�et bod� v jedn� k�ivce</param>
        /// <param name="div">D�len� intervalu 2pi (celkov� po�et bod� v�po�tu)</param>
        public PointVector[] EquipotentialContours(double e, int n, int div) {
            Contour contour = this.CreateContour(e, div);
            return contour.GetPointVector(n);
        }

        public PointVector[] VMatrixContours(double e, int n, int div, int ei) {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Body pro rozhodnut�, zda je podle SALI dan� trajektorie regul�rn� nebo chaotick�
        /// </summary>
        /// <returns>[time chaotick�, SALI chaotick�, time regul�rn�, SALI regul�rn�, time koncov� bod, SALI koncov� bod]</returns>
        public double[] SALIDecisionPoints() {
            return new double[] { 0, 4, 1000, 0, 5000, 6 };
        }
        #endregion

        protected const int defaultDivision = 4001;
    }
}