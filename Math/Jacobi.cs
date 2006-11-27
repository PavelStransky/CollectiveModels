using System;
using System.IO;

namespace PavelStransky.Math {
    /// <summary>
    /// Výpoèet vlastních èísel a vlastních vektorù symetrické matice Jacobiho metodou
    /// </summary>
    public class Jacobi : IExportable {
        // Maximální poèet iterací
        private const int jacobiMaxIteration = 500;

        // Výsledná data
        private Vector[] eigenVector;
        private double[] eigenValue;

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        public Jacobi() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="source">Zdrojová symetrická matice</param>
        public Jacobi(Matrix source) : this(source, null) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="source">Zdrojová symetrická matice</param>
        /// <param name="writer">Writer pro výpis na konzoli</param>
        public Jacobi(Matrix source, IOutputWriter writer) {
            this.Compute(source, writer);
        }

        /// <summary>
        /// Provede jednu Jacobiho rotaci
        /// </summary>
        /// <param name="p">Matice</param>
        /// <param name="i">Index øádku</param>
        /// <param name="j">Index sloupce</param>
        /// <param name="s">Sinus</param>
        /// <param name="c">Cosinus</param>
        /// <returns>Parametr tau</returns>
        private double JacobiRotation(Matrix p, int i, int j, double s, double c) {
            double tau = s / (1.0 + c);
            double pom1, pom2;
            
            p[i, j] = 0;
            p[j, i] = 0;

            // Cyklus 0 <= k < i
            for(int k = 0; k < i; k++) {
                pom1 = p[k, i];
                pom2 = p[k, j];

                p[k, i] -= s * (pom2 + tau * pom1);
                p[k, j] += s * (pom1 - tau * pom2);
            }

            // Cyklus i + 1 <= k < j
            for(int k = i + 1; k < j; k++) {
                pom1 = p[i, k];
                pom2 = p[k, j];

                p[i, k] -= s * (pom2 + tau * pom1);
                p[k, j] += s * (pom1 - tau * pom2);
            }

            // Cyklus j + 1 <= k < Size
            int length = p.Length;
            for(int k = j + 1; k < length; k++) {
                pom1 = p[i, k];
                pom2 = p[j, k];

                p[i, k] -= s * (pom2 + tau * pom1);
                p[j, k] += s * (pom1 - tau * pom2);
            }

            return tau;
        }

        /// <summary>
        /// Jacobiho metoda diagonalizace symetrických matic
        /// </summary>
        private void Compute(Matrix source, IOutputWriter writer) {
            int size = source.LengthX;

            this.eigenVector = new Vector[size];
            this.eigenValue = new double[size];

            Matrix p = source.Clone();

            Vector b = new Vector(size);
            Vector z = new Vector(size);

            for(int i = 0; i < size; i++) {
                this.eigenVector[i] = new Vector(size);
                this.eigenVector[i].Clear();
                this.eigenVector[i][i] = 1;

                b[i] = this.eigenValue[i] = p[i, i];
            }

            const double epsilon = 0;
            int iter = 0;

            if(writer != null)
                writer.WriteLine("Jacobi:");

            do {
                double sum = p.NondiagonalAbsSum();
                if(sum <= epsilon) break;

                double koef = 0;
                if(iter < 4)
                    koef = sum / (5.0 * size * size);

                for(int i = 0; i < size - 1; i++)
                    for(int j = i + 1; j < size; j++) {
                        double g = 100.0 * System.Math.Abs(p[i, j]);
                        if(iter > 4 && System.Math.Abs(this.eigenValue[i]) + g == System.Math.Abs(this.eigenValue[i]) &&
                            System.Math.Abs(this.eigenValue[j]) + g == System.Math.Abs(this.eigenValue[j])) {
                            p[i, j] = 0.0;
                            p[j, i] = 0.0;
                        }

                        else if(System.Math.Abs(p[i, j]) > koef) {
                            double diff = this.eigenValue[j] - this.eigenValue[i];
                            double t = 0.0, theta = 0.0;

                            if(System.Math.Abs(diff) + g == System.Math.Abs(diff))
                                t = p[i, j] / diff;
                            else {
                                theta = 0.5 * diff / p[i, j];
                                t = 1.0 / (System.Math.Abs(theta) + System.Math.Sqrt(1.0 + theta * theta));
                                if(theta < 0.0) t = -t;
                            }

                            double c = 1.0 / System.Math.Sqrt(1.0 + t * t);
                            double s = t * c;
                            double h = t * p[i, j];

                            z[i] -= h;
                            z[j] += h;
                            this.eigenValue[i] -= h;
                            this.eigenValue[j] += h;

                            double tau = this.JacobiRotation(p, i, j, s, c);

                            for(int k = 0; k < size; k++) {
                                double pom1 = this.eigenVector[i][k];
                                double pom2 = this.eigenVector[j][k];

                                this.eigenVector[i][k] -= s * (pom2 + tau * pom1);
                                this.eigenVector[j][k] += s * (pom1 - tau * pom2);
                            }
                        }
                    }

                b += z;
                z.Clear();

                for(int i = 0; i < size; i++)
                    this.eigenValue[i] = b[i];

                if(writer != null)
                    writer.WriteLine(string.Format("{0} {1}", iter, sum));

            } while(iter++ < jacobiMaxIteration);
        }

        /// <summary>
        /// Tøídìní podle vlastních èísel vzestupnì
        /// </summary>
        public void SortAsc() {
            // Tøídìní podle vlastních vektorù vzestupnì
            for(int i = 0; i < this.eigenValue.Length - 1; i++) {
                int minj = i;

                // Vyhledani nejmenšího prvku
                for(int j = i + 1; j < this.eigenValue.Length; j++)
                    if(this.eigenValue[minj] > this.eigenValue[j])
                        minj = j;

                // Prohozeni prvkù
                if(minj != i) {
                    //Prohození vlastních vektorù
                    Vector pomv = this.eigenVector[minj];
                    this.eigenVector[minj] = this.eigenVector[i];
                    this.eigenVector[i] = pomv;

                    // Prohození vlastních èísel
                    double pomd = this.eigenValue[minj];
                    this.eigenValue[minj] = this.eigenValue[i];
                    this.eigenValue[i] = pomd;
                }
            }
        }

        /// <summary>
        /// Tøídìní podle vlastních èísel sestupnì
        /// </summary>
        public void SortDesc() {
            // Tøídìní podle vlastních vektorù sestupnì
            for(int i = 0; i < this.eigenValue.Length - 1; i++) {
                int maxj = i;

                // Vyhledani nejvetsiho prvku
                for(int j = i + 1; j < this.eigenValue.Length; j++)
                    if(this.eigenValue[maxj] < this.eigenValue[j])
                        maxj = j;

                // Prohozeni prvkù
                if(maxj != i) {
                    //Prohození vlastních vektorù
                    Vector pomv = this.eigenVector[maxj];
                    this.eigenVector[maxj] = this.eigenVector[i];
                    this.eigenVector[i] = pomv;

                    // Prohození vlastních èísel
                    double pomd = this.eigenValue[maxj];
                    this.eigenValue[maxj] = this.eigenValue[i];
                    this.eigenValue[i] = pomd;
                }
            }
        }

        /// <summary>
        /// Vrátí vlastní vektor
        /// </summary>
        public Vector[] EigenVector { get { return this.eigenVector; } }

        /// <summary>
        /// Vrátí vlastní hodnotu
        /// </summary>
        public double[] EigenValue { get { return this.eigenValue; } }

        /// <summary>
        /// Vrátí vlastní hodnoty a èísla jako matici
        /// </summary>
        /// <returns>Vlastní hodnota, vlastní vektory ve sloupcích</returns>
        public Matrix EigenMatrix() {
            int length = this.eigenValue.Length;
            Matrix retValue = new Matrix(length + 1, length);

            for(int i = 0; i < length; i++) {
                retValue[0, i] = this.eigenValue[i];

                for(int j = 0; j < length; j++)
                    retValue[j + 1, i] = this.eigenVector[i][j];
            }

            return retValue;
        }

        #region Implementace IExportable
        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            if(export.Binary) {
                // Binárnì
                BinaryWriter b = export.B;
                b.Write(this.eigenValue.Length);
                for(int i = 0; i < this.eigenValue.Length; i++) {
                    b.Write(this.eigenValue[i]);
                    for(int j = 0; j < this.eigenVector.Length; j++)
                        b.Write(this.eigenVector[i][j]);
                }
            }
            else {
                // Textovì
                StreamWriter t = export.T;
                t.WriteLine(this.eigenValue.Length);
                for(int i = 0; i < this.eigenValue.Length; i++) {
                    t.WriteLine("Vlastní hodnota:\t{0}", this.eigenValue[i]);
                    t.Write("Vlastní vektor:");

                    for(int j = 0; j < this.eigenVector.Length; j++)
                        t.Write("\t{0}", this.eigenVector[i][j]);
                    t.WriteLine();
                }
            }
        }

        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public void Import(Import import) {
            if(import.Binary) {
                // Binárnì
                BinaryReader b = import.B;
                int size = b.ReadInt32();
                this.eigenValue = new double[size];
                this.eigenVector = new Vector[size];

                for(int i = 0; i < size; i++) {
                    this.eigenValue[i] = b.ReadDouble();

                    this.eigenVector[i] = new Vector(size);
                    for(int j = 0; j < size; j++)
                        this.eigenVector[i][j] = b.ReadDouble();
                }
            }
            else {
                // Textovì
                StreamReader t = import.T;
                int size = int.Parse(t.ReadLine());
                this.eigenValue = new double[size];
                this.eigenVector = new Vector[size];

                for(int i = 0; i < size; i++) {
                    string line = t.ReadLine();
                    string[] s = line.Split('\t');
                    this.eigenValue[i] = double.Parse(s[1]);

                    line = t.ReadLine();
                    s = line.Split('\t');
                    this.eigenVector[i] = new Vector(size);
                    for(int j = 0; j < size; j++)
                        this.eigenVector[i][j] = double.Parse(s[j + 1]);
                }
            }
        }
        #endregion
    }
}
