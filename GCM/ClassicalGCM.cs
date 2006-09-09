using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.GCM {
    public class ClassicalGCM: GCM {
        // Standardn� RK metoda
        private RungeKutta rk;

        // Metoda RK s adaptivn�m krokem
        private RungeKuttaAdapted rkA;

        // Gener�tor n�hodn�ch ��sel
        private Random random = new Random(10000);

        /// <summary>
        /// Prav� strana pohybov� rovnice (rovnice 2. ��du)
        /// </summary>
        /// <param name="rightSide">Prav� strana</param>
        /// <param name="polar">Pol�rn� (Bohrovy) sou�adnice</param>
        /// <param name="vpolar">Rychlosti</param>
        private void Equation(Vector rightSide, Vector polar, Vector vpolar) {
            double beta = polar[0], gamma = polar[1];
            double vbeta = vpolar[0], vgamma = vpolar[1];
            rightSide[0] = vgamma * vgamma * beta - (2.0 * this.A + 3.0 * this.B * beta * System.Math.Cos(3.0 * gamma) + 4.0 * this.C * beta * beta) * beta / this.K;
            rightSide[1] = -2.0 * vbeta * vgamma / beta + 3.0 * this.B * beta * System.Math.Sin(3.0 * gamma) / this.K;
        }

        /// <summary>
        /// Prav� strana pohybov� rovnice (rovnice 2. ��du)
        /// </summary>
        /// <param name="rightSide">Prav� strana</param>
        /// <param name="x">Sou�adnice</param>
        /// <param name="vx">Rychlosti</param>
        private void EquationXY(Vector rightSide, Vector x, Vector vx) {
            double beta = x[0] * x[0] + x[1] * x[1];
            rightSide[0] = (-2.0 * this.A * x[0] - 3.0 * this.B * (x[0] * x[0] - x[1] * x[1]) - 4.0 * this.C * x[0] * beta) / this.K;
            rightSide[1] = -2.0 * x[1] * (this.A - 3.0 * this.B * x[0] + 2.0 * this.C * beta) / this.K;
        }

        /// <summary>
        /// Energie
        /// </summary>
        /// <param name="x">Sou�adnice</param>
        /// <param name="vx">Rychlosti</param>
        private double EnergyFunction(Vector x, Vector vx) {
            return this.EX(x[0], x[1], vx[0], vx[1]);
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr K</param>
        public ClassicalGCM(double a, double b, double c, double k)
            : base(a, b, c, k) {
            this.rkA = new RungeKuttaAdapted(new Equation(this.EquationXY), new EnergyFunction(this.EnergyFunction));
            this.rk = new RungeKutta(new Equation(this.EquationXY));
        }

        /// <summary>
        /// Generuje po��te�n� hodnoty
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="polar">Pol�rn� (Bohrovy) sou�adnice</param>
        /// <param name="vpolar">Rychlosti</param>
        public void GenerateInitial(double e, Vector polar, Vector vpolar) {
            do {
                double gamma = this.random.NextDouble() * 2.0 * System.Math.PI;
                Vector r = this.Roots(e, gamma);

                if(r.Length == 0)
                    continue;

                int i = 2 * this.random.Next(r.Length / 2);
                double beta = r[i] + this.random.NextDouble() * (r[i + 1] - r[i]);
                double v = this.V(beta, gamma);
                double tbracket = 2.0 * (e - v) / this.K;

                double vgammaMax = System.Math.Sqrt(tbracket / (beta * beta));
                double vgamma = 2.0 * this.random.NextDouble() * vgammaMax - vgammaMax;
                double vbeta = System.Math.Sqrt(tbracket - vgamma * vgamma * beta * beta);

                if(this.random.Next(2) == 0)
                    vbeta = -vbeta;

                double chkE = this.E(beta, gamma, vbeta, vgamma);

                polar[0] = beta;
                polar[1] = gamma;
                vpolar[0] = vbeta;
                vpolar[1] = vgamma;
                return;

            } while(true);
        }

        /// <summary>
        /// Generuje po��te�n� podm�nky pro prom�nn� x, y
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="x">Sou�adnice</param>
        /// <param name="vx">Rychlosti</param>
        public void GenerateInitialXY(double e, Vector x, Vector vx) {
            Vector polar = new Vector(2);
            Vector vpolar = new Vector(2);

            this.GenerateInitial(e, polar, vpolar);

            double sg = System.Math.Sin(polar[1]);
            double cg = System.Math.Cos(polar[1]);

            x[0] = polar[0] * cg;
            x[1] = polar[0] * sg;
            vx[0] = vpolar[0] * cg - polar[0] * vpolar[1] * sg;
            vx[1] = vpolar[0] * sg + polar[0] * vpolar[1] * cg;
        }

        /// <summary>
        /// Spo��t� n�hodnou trajektorii a vr�t� ji v matici o sloupc�ch
        /// T - �as
        /// x, y
        /// vx, vy
        /// </summary>
        /// <param name="x">Po��te�n� sou�adnice</param>
        /// <param name="vx">Po��te�n� rychlosti</param>
        /// <param name="time">Doba, po kterou bude trajektorie po��t�na</param>
        /// <param name="precision">P�esnost v�sledku</param>
        /// <param name="timeStep">�asov� krok v�sledku</param>
        public Matrix TrajectoryMatrix(Vector x, Vector vx, double time, double precision, double timeStep) {
            if(precision == 0.0)
                this.rkA.Solve(time, x, vx);
            else
                this.rkA.Solve(time, precision, x, vx);

            Matrix m;
            if(timeStep == 0)
                m = this.rkA.GetMatrix();
            else
                m = this.rkA.GetMatrix(timeStep);

            return m;
        }

        /// <summary>
        /// Spo��t� n�hodnou trajektorii a vr�t� ji v matici o sloupc�ch
        /// T - �as
        /// x, y
        /// vx, vy
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="time">Doba, po kterou bude trajektorie po��t�na</param>
        /// <param name="precision">P�esnost v�sledku</param>
        /// <param name="timeStep">�asov� krok v�sledku</param>
        public Matrix RandomTrajectoryMatrix(double e, double time, double precision, double timeStep) {
            Vector x = new Vector(2);
            Vector vx = new Vector(2);

            this.GenerateInitialXY(e, x, vx);
            return this.TrajectoryMatrix(x, vx, time, precision, timeStep);
        }

        /// <summary>
        /// Spo��t� n�hodnou trajektorii a jej� sou�adnice x, y
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="time">Doba, po kterou bude trajektorie po��t�na</param>
        /// <param name="precision">P�esnost v�sledku</param>
        /// <param name="timeStep">�asov� krok v�sledku</param>
        public PointVector RandomTrajectory(double e, double time, double precision, double timeStep) {
            Matrix m = this.RandomTrajectoryMatrix(e, time, precision, timeStep);
            return new PointVector(m.GetColumnVector(1), m.GetColumnVector(2));
        }

        /// <summary>
        /// Spo��t� trajektorii se zadan�mi po��te�n�mi podm�nkami
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="vx">vx</param>
        /// <param name="vy">vy</param>
        /// <param name="time">Doba v�po�tu</param>
        /// <param name="precision">P�esnost v�po�tu</param>
        /// <param name="timeStep">�asov� krok v�po�tu</param>
        public PointVector Trajectory(double x, double y, double vx, double vy, double time, double precision, double timeStep) {
            Vector px = new Vector(2);
            Vector pvx = new Vector(2);
            px[0] = x; px[1] = y;
            pvx[0] = vx; pvx[1] = vy;

            Matrix m = this.TrajectoryMatrix(px, pvx, time, precision, timeStep);
            return new PointVector(m.GetColumnVector(1), m.GetColumnVector(2));
        }

        /// <summary>
        /// Ve v�sledku RK v�po�tu nalezne body pro Poincar�ho �ez
        /// </summary>
        /// <param name="m">V�sledek RK v�po�tu</param>
        /// <param name="result">Poincar�ho body</param>
        /// <param name="finished">Po�et doposud vypo�ten�ch bod�</param>
        /// <returns>Nov� po�et doposud vypo�ten�ch bod�</returns>
        private int FindPoincarePoints(Matrix m, PointVector points, int finished) {
            // �ez d�l�me rovinou y = 0
            double oldy = m[0, 2];
            PointD oldp = new PointD(m[0, 1], m[0, 3]);
            PointD h = new PointD(0.5, 0.5);

            for(int i = 1; i < m.LengthX; i++) {
                double newy = m[i, 2];
                PointD newp = new PointD(m[i, 1], m[i, 3]);

                if(oldy == 0)
                    points[finished++] = oldp;
                else if((oldy < 0 && newy > 0) || (oldy > 0 && newy < 0))
                    points[finished++] = (oldp + newp) * h;

                if(finished >= points.Length)
                    break;

                oldp = newp;
                oldy = newy;
            }

            return finished;
        }

        /// <summary>
        /// Po��t� Poincar� section pro jednu trajektorii
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="vx">vx</param>
        /// <param name="vy">vy</param>
        /// <param name="precision">P�esnost v�sledku</param>
        /// <param name="numPoints">Po�et bod� �ezu ve v�sledku</param>
        public PointVector PoincareSection(double x, double y, double vx, double vy, int numPoints, double precision) {
            Vector px = new Vector(2);
            Vector pvx = new Vector(2);
            px[0] = x; px[1] = y;
            pvx[0] = vx; pvx[1] = vy;

            return this.PoincareSection(px, pvx, numPoints, precision);
        }

        /// <summary>
        /// Po��t� Poincar� section pro jednu n�hodn� vybranou trajektorii
        /// </summary>
        /// <param name="x">Sou�adnice</param>
        /// <param name="vx">Rychlosti</param>
        /// <param name="precision">P�esnost v�sledku</param>
        /// <param name="numPoints">Po�et bod� �ezu ve v�sledku</param>
        public PointVector RandomPoincareSection(double e, int numPoints, double precision) {
            Vector x = new Vector(2);
            Vector vx = new Vector(2);

            this.GenerateInitialXY(e, x, vx);
            return this.PoincareSection(x, vx, numPoints, precision);
        }

        /// <summary>
        /// Po��t� Poincar� section pro jednu trajektorii
        /// </summary>
        /// <param name="x">Sou�adnice</param>
        /// <param name="vx">Rychlosti</param>
        /// <param name="precision">P�esnost v�sledku</param>
        /// <param name="numPoints">Po�et bod� �ezu ve v�sledku</param>
        public PointVector PoincareSection(Vector x, Vector vx, int numPoints, double precision) {
            PointVector result = new PointVector(numPoints);
            int finished = 0;

            do {
                Matrix m = this.TrajectoryMatrix(x, vx, poincareTime, precision, 0.0);
                int last = m.LengthX - 1;

                finished = this.FindPoincarePoints(m, result, finished);

                x[0] = m[last, 1]; x[1] = m[last, 2];
                vx[0] = m[last, 3]; vx[1] = m[last, 4];

            } while(finished < numPoints || finished == 0);

            return result;
        }

        private const double poincareTime = 100;
    }
}