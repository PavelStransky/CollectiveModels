using System;
using System.Collections;
using System.Text;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Expression;
using PavelStransky.Math;

using MPIR;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Resultant of a matrix in the form A + l B
    /// </summary>
    public class ResultantMPIR : Fnc {
        public class Polynomial : ICloneable {
            private HugeFloat[,] coefs;
            private int maxPowerE, maxPowerLambda;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="coefs">Matrix of the coefs in the form ((1, lambda, lambda^2, ...), (E, E lambda, E lambda^2, ...), (E^2, ...))</param>
            public Polynomial(Matrix coefs) {
                this.maxPowerE = coefs.LengthX - 1;
                this.maxPowerLambda = coefs.LengthY - 1;

                this.coefs = new HugeFloat[this.maxPowerE + 1, this.maxPowerLambda + 1];
                for(int i = 0; i <= this.maxPowerE; i++)
                    for(int j = 0; j <= this.maxPowerLambda; j++)
                        this.coefs[i, j] = new HugeFloat(coefs[i, j]);
            }

            /// <summary>
            /// Constructor (free polynomial)
            /// </summary>
            /// <param name="maxPowerE">Maximum energy degree</param>
            /// <param name="maxPowerLambda">Maximum lambda degree</param>
            public Polynomial(int maxPowerE, int maxPowerLambda) {
                this.maxPowerE = maxPowerE;
                this.maxPowerLambda = maxPowerLambda;

                this.coefs = new HugeFloat[this.maxPowerE + 1, this.maxPowerLambda + 1];
                for(int i = 0; i <= this.maxPowerE; i++)
                    for(int j = 0; j <= this.maxPowerLambda; j++)
                        this.coefs[i, j] = new HugeFloat();
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
            public int MaxPowerE { get { return this.maxPowerE; } }

            /// <summary>
            /// Maximum power in lambda
            /// </summary>
            public int MaxPowerLambda { get { return this.maxPowerLambda; } }

            /// <summary>
            /// Null polynomial (equals zero polynomial)
            /// </summary>
            public bool IsNull { get { return this.coefs == null; } }

            /// <summary>
            /// Coefficients
            /// </summary>
            public HugeFloat this[int i, int j] { get { return this.coefs[i, j]; } set { this.coefs[i, j].Value = value; } }

            /// <summary>
            /// Multiplication of two polynomials
            /// </summary>
            public static Polynomial operator *(Polynomial p1, Polynomial p2) {
                if(p1.IsNull || p2.IsNull)
                    return new Polynomial();

                int I = p1.maxPowerE;
                int J = p1.maxPowerLambda;
                int K = p2.maxPowerE;
                int L = p2.maxPowerLambda;

                Polynomial result = new Polynomial(I + K, J + L);

                for(int i = 0; i <= I; i++)
                    for(int j = 0; j <= J; j++) {
                        if(p1[i, j] == 0.0)
                            continue;
                        for(int k = 0; k <= K; k++)
                            for(int l = 0; l <= L; l++) 
                                result.coefs[i + k, j + l].Value += p1.coefs[i, j] * p2.coefs[k, l];
                    }

                return result;
            }

            /// <summary>
            /// Multiplication of a polynomial and a double
            /// </summary>
            /// <param name="d">Double number</param>
            /// <param name="p">Polynomial</param>
            public static Polynomial operator *(double d, Polynomial p) {
                if(p.IsNull || d == 0.0)
                    return new Polynomial();

                int I = p.maxPowerE;
                int J = p.maxPowerLambda;

                HugeFloat hf = new HugeFloat(d);

                Polynomial result = new Polynomial(I, J);
                for(int i = 0; i <= I; i++)
                    for(int j = 0; j <= J; j++)
                        result.coefs[i, j].Value = hf * p.coefs[i, j];

                return result;
            }

            /// <summary>
            /// Addition of two polynomials
            /// </summary>
            public static Polynomial operator +(Polynomial p1, Polynomial p2) {
                if(p1.IsNull)
                    return p2.Clone() as Polynomial;
                if(p2.IsNull)
                    return p1.Clone() as Polynomial;

                Polynomial result = new Polynomial(System.Math.Max(p1.maxPowerE, p2.maxPowerE), System.Math.Max(p1.maxPowerLambda, p2.maxPowerLambda));

                for(int i = 0; i <= result.maxPowerE; i++)
                    for(int j = 0; j <= result.maxPowerLambda; j++)
                        result.coefs[i, j].Value = (i <= p1.MaxPowerE && j <= p1.MaxPowerLambda ? p1.coefs[i, j] : new HugeFloat()) + (i <= p2.MaxPowerE && j <= p2.MaxPowerLambda ? p2.coefs[i, j] : new HugeFloat());

                return result;
            }

            /// <summary>
            /// Finds nonzero rows and columns and removes them
            /// </summary>
            public void Reduce() {
                if(this.IsNull)
                    return;

                int maxPowerE = -1;
                for(int i = this.maxPowerE; i >= 0; i--) {
                    bool n = false;
                    for(int j = this.maxPowerLambda; j >= 0; j--)
                        if(this[i, j] != 0.0) {
                            n = true;
                            break;
                        }
                    if(n) {
                        maxPowerE = i;
                        break;
                    }
                }

                if(maxPowerE < 0) {
                    this.maxPowerE = 0;
                    this.maxPowerLambda = 0;
                    this.coefs = null;
                    return;
                }

                int maxPowerLambda = -1;
                for(int j = this.maxPowerLambda; j >= 0; j--) {
                    bool n = false;
                    for(int i = maxPowerE; i >= 0; i--)
                        if(this[i, j] != 0.0) {
                            n = true;
                            break;
                        }
                    if(n) {
                        maxPowerLambda = j;
                        break;
                    }
                }

                if(maxPowerE == this.maxPowerE && maxPowerLambda == this.maxPowerLambda)
                    return;

                HugeFloat [,]coefs = new HugeFloat[maxPowerE + 1, maxPowerLambda + 1];
                for(int i = 0; i <= maxPowerE; i++)
                    for(int j = 0; j <= maxPowerLambda; j++)
                        coefs[i, j] = this.coefs[i, j];

                this.maxPowerE = maxPowerE;
                this.maxPowerLambda = maxPowerLambda;
            }

            /// <summary>
            /// Derivative with respect of energy (for the resultant)
            /// </summary>
            public Polynomial DE() {
                Polynomial result = new Polynomial(this.maxPowerE - 1, this.maxPowerLambda);

                for(int i = 1; i <= this.MaxPowerE; i++) {
                    HugeFloat hf = new HugeFloat(i);
                    for(int j = 1; j <= this.MaxPowerLambda; j++)
                        result.coefs[i - 1, j - 1].Value = hf * this[i, j - 1];
                }

                return result;
            }

            /// <summary>
            /// Gets one power of energy
            /// </summary>
            /// <param name="i">Power of energy</param>
            public Polynomial GetPowerE(int i) {
                int j = 0;
                for(j = this.MaxPowerLambda; j >= 0; j--)
                    if(this.coefs[i, j] != 0.0)
                        break;

                if(j < 0)
                    return new Polynomial();

                Polynomial result = new Polynomial(0, j);
                for(; j >= 0; j--)
                    result.coefs[0, j] = this.coefs[i, j];

                return result;
            }

            /// <summary>
            /// Calculates the resultant matrix of the polynomial
            /// </summary>
            public PolynomialMatrix Resultant(Guider guider) {
                if(guider != null) {
                    guider.Write("R");
                    guider.Write(this.maxPowerE);
                }

                DateTime t = DateTime.Now;
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

                    if(guider != null)
                        guider.Write(".");
                }

                if(guider != null)
                    guider.WriteLine(SpecialFormat.Format(DateTime.Now - t));

                return result;

            }

            public object Clone() {
                if(this.IsNull)
                    return new Polynomial();
                Polynomial result = new Polynomial(this.maxPowerE, this.maxPowerLambda);

                for(int i = 0; i <= this.maxPowerE; i++)
                    for(int j = 0; j <= this.maxPowerLambda; j++)
                        result.coefs[i, j].Value = new HugeFloat(this[i, j]);

                return result;
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
                        Matrix m = new Matrix(i==j ? 2 : 1, 2);
                        m[0, 0] = a[i, j];
                        m[0, 1] = b[i, j];
                        if(i == j)
                            m[1, 0] = -1;
                        this.matrix[i, j] = new Polynomial(m);
                    }
            }

            /// <summary>
            /// Constructor of a matrix A + lambda B + lambda^2 C - E (for the Lipkin model for example)
            /// </summary>
            public PolynomialMatrix(Matrix a, Matrix b, Matrix c) {
                this.length = a.Length;

                this.matrix = new Polynomial[this.length, this.length];
                for(int i = 0; i < this.length; i++)
                    for(int j = 0; j < this.length; j++) {
                        Matrix m = new Matrix(i == j ? 2 : 1, 3);
                        m[0, 0] = a[i, j];
                        m[0, 1] = b[i, j];
                        m[0, 2] = c[i, j];
                        if(i == j)
                            m[1, 0] = -1;
                        this.matrix[i, j] = new Polynomial(m);
                    }
            }

            /// <summary>
            /// Constructor of an empty polynomial matrix
            /// </summary>
            /// <param name="length">Dimension of the matrix</param>
            public PolynomialMatrix(int length) {
                this.length = length;
                this.matrix = new Polynomial[this.length, this.length];
                for(int i = 0; i < length; i++)
                    for(int j = 0; j < length; j++)
                        this.matrix[i, j] = new Polynomial();
            }

            /// <summary>
            /// Matrix Mu for the calculation of the determinant
            /// </summary>
            private PolynomialMatrix Mu() {
                PolynomialMatrix result = new PolynomialMatrix(this.length);
                for(int i = 0; i < this.length; i++)
                    for(int j = i + 1; j < this.length; j++) {
                        result.matrix[i, j] = this.matrix[i, j];
                        result.matrix[j, i] = new Polynomial();
                    }

                Polynomial sum = new Polynomial();
                for(int i = this.length - 2; i >= 0; i--) {
                    sum += (-1) * this.matrix[i + 1, i + 1];
                    sum.Reduce();
                    result.matrix[i, i] = sum;
                }

                return result;
            }

            /// <summary>
            /// Operator F for the calculation of the determinant
            /// </summary>
            private PolynomialMatrix F(PolynomialMatrix a) {
                PolynomialMatrix mu = this.Mu();
                PolynomialMatrix result = mu * a;
                return result;
            }

            /// <summary>
            /// Polynomial matrix multiplication
            /// </summary>
            public static PolynomialMatrix operator *(PolynomialMatrix a, PolynomialMatrix b) {
                int length = a.Length;
                PolynomialMatrix result = new PolynomialMatrix(length);

                for(int i = 0; i < length; i++)
                    for(int j = 0; j < length; j++) {
                        for(int k = 0; k < length; k++)
                            result.matrix[i, j] += a.matrix[i, k] * b.matrix[k, j];
                        result.matrix[i, j].Reduce();
                    }

                return result;
            }

            /// <summary>
            /// Determinant by R.S. Bird, Information Processing Letters 111, 1072 (2011) 
            /// </summary>
            /// <remarks>http://dx.doi.org/10.1016/j.ipl.2011.08.006</remarks>
            public Polynomial Determinant(Guider guider) {
                DateTime t = DateTime.Now;

                if(guider != null) {
                    guider.Write("D");
                    guider.Write(this.length);
                }

                PolynomialMatrix result = this;

                for(int i = 1; i < this.length; i++) {
                    result = result.F(this);
                    if(guider != null) {
                        guider.Write(".");
                        if(i % 10 == 0)
                            guider.Write(SpecialFormat.Format(DateTime.Now - t));
                    }

                }

                if(guider != null)
                    guider.WriteLine(SpecialFormat.Format(DateTime.Now - t));

                if(this.length % 2 == 0)
                    return (-1) * result.matrix[0, 0];

                return result.matrix[0, 0];
            }
        }

        public override string Help { get { return Messages.HelpResultant; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);

            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(2, false, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(3, false, true, false, Messages.PPrecision, Messages.PPrecisionDescription, 1024, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            HugeFloat.DefaultPrecision = (uint)((int)arguments[3]);
            int size = (arguments[0] as Matrix).Length;

            DateTime t = DateTime.Now;

            bool quadratic = arguments[2] != null;

            if(guider != null)
                guider.Write("P...");
            PolynomialMatrix pm = (!quadratic ? new PolynomialMatrix(arguments[0] as Matrix, arguments[1] as Matrix) : new PolynomialMatrix(arguments[0] as Matrix, arguments[1] as Matrix, arguments[2] as Matrix));
            if(guider != null)
                guider.WriteLine(SpecialFormat.Format(DateTime.Now - t));

            Polynomial p = pm.Determinant(guider);
            PolynomialMatrix rm = p.Resultant(guider);
            Polynomial r = rm.Determinant(guider).GetPowerE(0);

            // Construct a Frobenius companion matrix
            int l = r.MaxPowerLambda;

            // Number of roots (what is above should be zero)
            l = size * (size - 1);
            if(quadratic)
                l *= 2;

            if(guider != null) {
                guider.Write("M");
                guider.Write(l);
                guider.Write("...");
            }

            DateTime t0 = DateTime.Now;

            Matrix m = new Matrix(l);
            for(int i = 1; i <= l; i++) {
                HugeFloat f = new HugeFloat(-r[0, l - i] / r[0, l]);
                m[0, i - 1] = f.ToDouble();
                if(i < l)
                    m[i, i - 1] = 1;
            }

            StringBuilder s = new StringBuilder();
            for(int i = 0; i <= l; i++) {
                if(r[0, l - i] >= 0)
                    s.Append("+");
                s.Append(r[0, l - i].ToString().Replace("@", "*10^"));
                s.Append("*x^" + i);
            }
            Export e = new Export("d:\\prd.txt", IETypes.Text);
            e.Write(s.ToString());
            e.Close();

            Vector[] v = LAPackDLL.dgeev(m, false);

            PointVector result = new PointVector(l);
            result.VectorX = v[0];
            result.VectorY = v[1];

            if(guider != null) {
                guider.WriteLine(SpecialFormat.Format(DateTime.Now - t0));
                guider.Write("Total time:");
                guider.WriteLine(SpecialFormat.Format(DateTime.Now - t));
            }

            return result;
        }
    }
}
