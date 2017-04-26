using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Resultant of a matrix in the form A + l B
    /// </summary>
    public class Resultant : Fnc {
        public class Polynomial {
            public Matrix coefs;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="coefs">Matrix of the coefs in the form ((1, lambda, lambda^2, ...), (E, E lambda, E lambda^2, ...), (E^2, ...))</param>
            public Polynomial(Matrix coefs) {
                this.coefs = coefs;
            }

            /// <summary>
            /// Maximum power in energy
            /// </summary>
            public int MaxPowerE { get { return this.coefs.LengthX - 1; } }

            /// <summary>
            /// Maximum power in lambda
            /// </summary>
            public int MaxPowerLambda { get { return this.coefs.LengthY - 1; } }

            /// <summary>
            /// Coefficients
            /// </summary>
            public double this[int i, int j] { get { return this.coefs[i, j]; } }

            /// <summary>
            /// Multiplying of two polynomials
            /// </summary>
            public static Polynomial operator *(Polynomial p1, Polynomial p2) {
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
                int I = p.MaxPowerE;
                int J = p.MaxPowerLambda;

                Matrix m = new Matrix(I + 1, J + 1);
                for(int i = 0; i <= I; i++)
                    for(int j = 0; j <= J; j++)
                        m[i, j] = d * p[i, j];
                return new Polynomial(m);
            }

            public static Polynomial operator +(Polynomial p1, Polynomial p2) {
                Matrix m = new Matrix(System.Math.Max(p1.MaxPowerE, p2.MaxPowerE) + 1, System.Math.Max(p1.MaxPowerLambda, p2.MaxPowerLambda) + 1);
                for(int i = 0; i < m.LengthX; i++)
                    for(int j = 0; j < m.LengthY; j++)
                        m[i, j] = (i <= p1.MaxPowerE && j <= p1.MaxPowerLambda ? p1[i, j] : 0.0) + (i <= p2.MaxPowerE && j <= p2.MaxPowerLambda ? p2[i, j] : 0.0);
                return new Polynomial(m);
            }
        }

        /// <summary>
        /// Square matrix with elements made of polynomials
        /// </summary>
        public class PolynomialMatrix {
            private Polynomial[,] matrix;       // Elements
            private int length;                 // Dimension of the matrix

            public int Length { get { return this.length; } }

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

            private PolynomialMatrix(int length) {
                this.length = length;
                this.matrix = new Polynomial[this.length, this.length];
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

            public Polynomial Determinant() {
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
            return pm.Determinant().coefs;
        }
    }
}
