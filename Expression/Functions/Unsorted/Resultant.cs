using System;
using System.Collections;

using PavelStransky.DLLWrapper;
using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Resultant of a matrix in the form A + l B
    /// </summary>
    public class Resultant : Fnc {
        public class Polynomial: ICloneable {
            public Matrix coefs;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="coefs">Matrix of the coefs in the form ((1, lambda, lambda^2, ...), (E, E lambda, E lambda^2, ...), (E^2, ...))</param>
            public Polynomial(Matrix coefs) {
                this.coefs = coefs;
            }

            /// <summary>
            /// Empty (null) polynomial
            /// </summary>
            public Polynomial() {
                this.coefs = null;
            }

            /// <summary>
            /// Maximum power in energy
            /// </summary>
            public int MaxPowerE { get { return this.coefs.LengthX - 1; } }

            /// <summary>
            /// Maximum power in lambda
            /// </summary>
            public int MaxPowerLambda { get { return this.coefs.LengthY - 1; } }

            public bool IsNull { get { return this.coefs == null; } }

            /// <summary>
            /// Coefficients
            /// </summary>
            public double this[int i, int j] { get { return this.coefs[i, j]; } }

            /// <summary>
            /// Multiplying of two polynomials
            /// </summary>
            public static Polynomial operator *(Polynomial p1, Polynomial p2) {
                if(p1.IsNull || p2.IsNull)
                    return new Polynomial();

                int I = p1.MaxPowerE;
                int J = p1.MaxPowerLambda;
                int K = p2.MaxPowerE;
                int L = p2.MaxPowerLambda;

                Matrix m = new Matrix(I + K + 1, J + L + 1);

                for(int i = 0; i <= I; i++)
                    for(int j = 0; j <= J; j++)
                        for(int k = 0; k <= K; k++)
                            for(int l = 0; l <= L; l++) {
                                m[i + k, j + l] += p1[i, j] * p2[k, l];
                            }

                return new Polynomial(m);
            }

            public static Polynomial operator *(double d, Polynomial p) {
                if(p.IsNull)
                    return new Polynomial();

                int I = p.MaxPowerE;
                int J = p.MaxPowerLambda;

                Matrix m = new Matrix(I + 1, J + 1);
                for(int i = 0; i <= I; i++)
                    for(int j = 0; j <= J; j++)
                        m[i, j] = d * p[i, j];
                return new Polynomial(m);
            }

            public static Polynomial operator +(Polynomial p1, Polynomial p2) {
                if(p1.IsNull)
                    return p2.Clone() as Polynomial;
                if(p2.IsNull)
                    return p1.Clone() as Polynomial;

                Matrix m = new Matrix(System.Math.Max(p1.MaxPowerE, p2.MaxPowerE) + 1, System.Math.Max(p1.MaxPowerLambda, p2.MaxPowerLambda) + 1);
                for(int i = 0; i < m.LengthX; i++)
                    for(int j = 0; j < m.LengthY; j++)
                        m[i, j] = (i <= p1.MaxPowerE && j <= p1.MaxPowerLambda ? p1[i, j] : 0.0) + (i <= p2.MaxPowerE && j <= p2.MaxPowerLambda ? p2[i, j] : 0.0);
                return new Polynomial(m);
            }

            public Polynomial DE() {
                Matrix m = new Matrix(this.MaxPowerE, this.MaxPowerLambda);

                for(int i = 1; i <= this.MaxPowerE; i++)
                    for(int j = 1; j <= this.MaxPowerLambda; j++ )
                        m[i - 1, j - 1] = i * this[i, j - 1];

                return new Polynomial(m);
            }

            public Polynomial GetPowerE(int i) {
                int j = 0;
                for(j = this.MaxPowerLambda; j >= 0; j--)
                    if(this[i, j] != 0)
                        break;

                if(j < 0)
                    return new Polynomial();

                Matrix m = new Matrix(1, j + 1);
                for(; j >= 0; j--)
                    m[0, j] = this[i, j];

                return new Polynomial(m);
            }

            public PolynomialMatrix Resultant() {
                PolynomialMatrix result = new PolynomialMatrix(2 * this.MaxPowerE - 1);

                for(int i = 0; i <= this.MaxPowerE; i++) {
                    Polynomial p = this.GetPowerE(i);
                    for(int j = 0; j < this.MaxPowerE - 1; j++)
                        result[i + j, j] = p;

                    if(i > 0) {
                        p = i * p;

                        for(int j = 0; j < this.MaxPowerE; j++)
                            result[i + j - 1, j + this.MaxPowerE - 1] = p;
                    }
                }

                return result;

            }

            public double MaxAbsCoef() {
                if(this.IsNull)
                    return 0.0;

                return this.coefs.MaxAbs();
            }

            public object Clone() {
                if(this.IsNull)
                    return new Polynomial();
                else
                    return new Polynomial(this.coefs.Clone() as Matrix);
            }
        }


        /// <summary>
        /// Square matrix with elements made of polynomials
        /// </summary>
        public class PolynomialMatrix {
            private Polynomial[,] matrix;       // Elements
            private int length;                 // Dimension of the matrix

            /// <summary>
            /// Délka
            /// </summary>
            public int Length { get { return this.length; } }

            /// <summary>
            /// Indexer
            /// </summary>
            public Polynomial this[int i, int j] { get { return this.matrix[i, j]; } set { this.matrix[i, j] = value; } }

            /// <summary>
            /// Constructor of a matrix A + lambda B - E
            /// </summary>
            public PolynomialMatrix(Matrix a, Matrix b) {
                this.length = a.Length;

                this.matrix = new Polynomial[this.length, this.length];
                for(int i = 0; i < this.length; i++)
                    for(int j = 0; j < this.length; j++) {
                        Matrix m = new Matrix(2);
                        m[0, 0] = a[i, j];
                        m[0, 1] = b[i, j];
                        if(i == j)
                            m[1, 0] = -1;
                        this.matrix[i, j] = new Polynomial(m);
                    }
            }

            public PolynomialMatrix(int length) {
                this.length = length;
                this.matrix = new Polynomial[this.length, this.length];
                for(int i = 0; i < length; i++)
                    for(int j = 0; j < length; j++)
                        this.matrix[i, j] = new Polynomial();
            }

            /// <summary>
            /// Minor matrix constructed by removing row i and column 0
            /// </summary>
            /// <param name="i">Row to be removed</param>
            private PolynomialMatrix Minor(int i) {
                int length = this.Length;

                PolynomialMatrix result = new PolynomialMatrix(length - 1);
                for(int k = 0; k < length; k++)
                    for(int l = 1; l < length; l++) {
                        if(k < i)
                            result.matrix[k, l - 1] = this.matrix[k, l];

                        if(k > i)
                            result.matrix[k - 1, l - 1] = this.matrix[k, l];

                    }
                return result;
            }

            /// <summary>
            /// Minor matrix constructed by removing row i and column j
            /// </summary>
            /// <param name="i">Row to be removed</param>
            /// <param name="j">Column to be removed</param>           
            private PolynomialMatrix Minor(int i, int j) {
                PolynomialMatrix result = new PolynomialMatrix(this.length - 1);
                for(int k = 0; k < this.length; k++)
                    for(int l = 0; l < this.length; l++) {
                        if(k < i) {
                            if(l < j)
                                result.matrix[k, l] = this.matrix[k, l];
                            if(l > j)
                                result.matrix[k, l - 1] = this.matrix[k, l];
                        }
                        if(k > i) {
                            if(l < j)
                                result.matrix[k - 1, l] = this.matrix[k, l];
                            if(l > j)
                                result.matrix[k - 1, l - 1] = this.matrix[k, l];
                        }
                    }
                return result;
            }

            private PolynomialMatrix Mu() {
                PolynomialMatrix result = new PolynomialMatrix(this.length);
                for(int i = 0; i < this.length; i++)
                    for(int j = i + 1; j < this.length; j++) {
                        result.matrix[i, j] = this.matrix[i, j];
                        result.matrix[j, i] = new Polynomial();
                    }

                Polynomial sum = new Polynomial(null);
                for(int i = this.length - 2; i >= 0; i--) {
                    sum += (-1) * this.matrix[i + 1, i + 1];
                    result.matrix[i, i] = sum;
                }

                return result;
            }

            private PolynomialMatrix F(PolynomialMatrix a) {
                PolynomialMatrix mu = this.Mu();
                PolynomialMatrix result = mu * a;
                return result;
            }

            public double Normalize() {
                double result = 0.0;
                for(int i = 0; i < this.length; i++)
                    for(int j = 0; j < this.length; j++)
                        result = System.Math.Max(result, System.Math.Abs(this[i, j].MaxAbsCoef()));

                double d = System.Math.Sqrt(1.0 / result);
                for(int i = 0; i < this.length; i++)
                    for(int j = 0; j < this.length; j++)
                        this[i, j] = d * this[i, j];

                return result;
            }

            public static PolynomialMatrix operator *(PolynomialMatrix a, PolynomialMatrix b) {
                int length = a.Length;
                PolynomialMatrix result = new PolynomialMatrix(length);

                for(int i = 0; i < length; i++)
                    for(int j = 0; j < length; j++) 
                        for(int k = 0; k < length; k++)
                            result.matrix[i, j] += a.matrix[i, k] * b.matrix[k, j];                    

                return result;
            }

            /// <summary>
            /// Determinant by R.S. Bird, Information Processing Letters 111, 1072 (2011) 
            /// </summary>
            /// <remarks>http://dx.doi.org/10.1016/j.ipl.2011.08.006</remarks>
            public Polynomial Determinant() {
                PolynomialMatrix result = this;
                for(int i = 1; i < this.length; i++)
                    result = result.F(this);
                if(this.length % 2 == 0)
                    return (-1) * result.matrix[0, 0];
                return result.matrix[0, 0];
            }

            /// <summary>
            /// Determinant using minors (extremely slow, n!)
            /// </summary>
            public Polynomial DeterminantMinor() {
                // Last element
                if(this.length == 1)
                    return this.matrix[0, 0];

                Polynomial result = new Polynomial(new Matrix(1));
                for(int i = 0; i < this.length; i++)
                    result = result + ((-2 * (i % 2) + 1) * this.matrix[i, 0]) * this.Minor(i).Determinant();

                return result;
            }
        }

        public override string Help { get { return Messages.HelpResultant; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Polynomial p1 = new Polynomial(arguments[0] as Matrix);
            Polynomial p2 = new Polynomial(arguments[1] as Matrix);

            PolynomialMatrix pm = new PolynomialMatrix(arguments[0] as Matrix, arguments[1] as Matrix);
            if(guider != null)
                guider.Write("P");
            Polynomial p = pm.Determinant();
            if(guider != null)
                guider.Write("D");
            PolynomialMatrix rm = p.Resultant();
            if(guider != null)
                guider.Write("R");

            double d = rm.Normalize();

            Polynomial r = rm.Determinant().GetPowerE(0);
            if(guider != null)
                guider.Write("D");

            // Construct a Frobenius companion matrix
            int l = r.MaxPowerLambda;

            Matrix m = new Matrix(l);
            for(int i = 1; i <= l; i++) {
                m[0, i - 1] = -(double)r[0, l-i] / (double)r[0, l];
                if(i < l)
                    m[i, i - 1] = 1;
            }

            Vector[] v = LAPackDLL.dgeev(m, false);

            if(guider != null)
                guider.WriteLine("M");

            PointVector result = new PointVector(l);
            result.VectorX = v[0];
            result.VectorY = v[1];

            return result;
        }
    }
}
