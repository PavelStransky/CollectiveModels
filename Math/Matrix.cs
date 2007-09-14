using System;
using System.Collections;
using System.IO;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Math {
	/// <summary>
	/// Implementace operací s maticemi
	/// </summary>
	public class Matrix: IExportable {
		private double [,] item;

		// +--------> Y
		// |
		// |	// rozmìry
		// v
        // X

        #region Konstruktory
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="lengthX">Délka X (poèet øádkù)</param>
		/// <param name="lengthY">Délka Y (poèet sloupcù)</param>
		public Matrix(int lengthX, int lengthY) {
            this.item = new double[lengthX, lengthY];
		}

		/// <summary>
		/// Vytvoøí ètvercovou matici
		/// </summary>
		/// <param name="length">Rozmìr</param>
		public Matrix(int length) {
            this.item = new double[length, length];
		}

		/// <summary>
		/// Vytvoøí matici s referencí na prvky
		/// </summary>
		/// <param name="item">Pole [x,y] s prvky matice</param>
        public Matrix(double[,] item) {
			this.item = item;
        }
        #endregion

        #region Vlastnosti
        /// <summary>
		/// Poèet øádkù matice
		/// </summary>
		public int LengthX {get {return this.item.GetLength(0);}}

		/// <summary>
		/// Poèet sloupcù matice
		/// </summary>
		public int LengthY {get {return this.item.GetLength(1);}}

		/// <summary>
		/// Rozmìr ètvercové matice
		/// </summary>
		public int Length {
			get {
				if(!this.IsSquare) 
					throw new MatrixException(errorMessageNotSquare);
		 
				return this.LengthX; 
			}
		}

		/// <summary>
		/// Je matice ètvercová?
		/// </summary>
		public bool IsSquare {get {return this.LengthX == this.LengthY;} }
        #endregion

        /// <summary>
        /// Checks whether the matrix is square one
        /// </summary>
        public void CheckSquare() {
            if(!this.IsSquare)
                throw new MatrixException(errorMessageNotSquare);
        }

        /// <summary>
        /// Indexer
        /// </summary>
        public double this[int i, int j] { get { return this.item[i, j]; } set { this.item[i, j] = value; } }

        /// <summary>
		/// Rozmìr jako funkce GetLength
		/// </summary>
		/// <param name="dim">Dimenze</param>
		public int GetLength(int dim) {return this.item.GetLength(dim); }

        #region Operátory
        /// <summary>
		/// Seète dvì matice stejných rozmìrù
		/// </summary>
		public static Matrix operator +(Matrix m1, Matrix m2) {
			if(m1.LengthX != m2.LengthX || m1.LengthY != m2.LengthY)
				throw new MatrixException(errorMessageDifferentDimension);

            int lengthX = m1.LengthX;
            int lengthY = m1.LengthY;
			Matrix result = new Matrix(lengthX, lengthY);

			for(int i = 0; i < lengthX; i++)
				for(int j = 0; j < lengthY; j++)
					result[i,j] = m1[i,j] + m2[i,j];

			return result;
		}

		/// <summary>
		/// Pøiète ke každé složce matice èíslo
		/// </summary>
		public static Matrix operator +(Matrix m, double d) {
            int lengthX = m.LengthX;
            int lengthY = m.LengthY;
            Matrix result = new Matrix(lengthX, lengthY);

			for(int i = 0; i < lengthX; i++)
				for(int j = 0; j < lengthY; j++)
					result[i,j] = m[i,j] + d;

			return result;
		}

		/// <summary>
		/// Odeète dvì matice stejných rozmìrù
		/// </summary>
		public static Matrix operator -(Matrix m1, Matrix m2) {
			if(m1.LengthX != m2.LengthX || m1.LengthY != m2.LengthY)
				throw new MatrixException(errorMessageDifferentDimension);

            int lengthX = m1.LengthX;
            int lengthY = m1.LengthY;
            Matrix result = new Matrix(lengthX, lengthY);

			for(int i = 0; i < lengthX; i++)
				for(int j = 0; j < lengthY; j++)
					result[i,j] = m1[i,j] - m2[i,j];

			return result;
		}
		
		/// <summary>
		/// Odeète od každé složky matice èíslo
		/// </summary>
		public static Matrix operator -(Matrix m, double d) {
            int lengthX = m.LengthX;
            int lengthY = m.LengthY;
            Matrix result = new Matrix(lengthX, lengthY);

			for(int i = 0; i < lengthX; i++)
				for(int j = 0; j < lengthY; j++)
					result[i,j] = m[i,j] - d;

			return result;
		}
		
		/// <summary>
		/// Odeète od èísla každou složku vektoru
		/// </summary>
		public static Matrix operator -(double d, Matrix m) {
            int lengthX = m.LengthX;
            int lengthY = m.LengthY;
            Matrix result = new Matrix(lengthX, lengthY);

			for(int i = 0; i < lengthX; i++)
				for(int j = 0; j < lengthY; j++)
					result[i,j] = d - m[i,j];

			return result;
		}
		
		/// <summary>
		/// Vynásobí matici èíslem
		/// </summary>
		public static Matrix operator *(Matrix m, double koef) {
            int lengthX = m.LengthX;
            int lengthY = m.LengthY;
            Matrix result = new Matrix(lengthX, lengthY);

			for(int i = 0; i < lengthX; i++)
				for(int j = 0; j < lengthY; j++)
					result[i,j] = koef * m[i,j];

			return result;
		}

		/// <summary>
		/// Vydìlí matici èíslem
		/// </summary>
		public static Matrix operator /(Matrix m, double koef) {
            int lengthX = m.LengthX;
            int lengthY = m.LengthY;
            Matrix result = new Matrix(lengthX, lengthY);

			for(int i = 0; i < lengthX; i++)
				for(int j = 0; j < lengthY; j++)
					result[i,j] = m[i,j] / koef;

			return result;
		}

		/// <summary>
		/// Dìlení èísla maticí
		/// </summary>
		public static Matrix operator /(double koef, Matrix m) {
			return m.Inv() * koef;
		}

		/// <summary>
		/// Dìlení dvou matic
		/// </summary>
		public static Matrix operator /(Matrix m1, Matrix m2) {
			return m1 * m2.Inv();
		}

		/// <summary>
		/// Vynásobí dvì matice kompatibilních rozmìrù
		/// </summary>
		public static Matrix operator *(Matrix m1, Matrix m2) {
			if(m1.LengthY != m2.LengthX)
				throw new MatrixException(errorMessageMultiplication);

            int lengthX = m1.LengthX;
            int lengthY = m2.LengthY;
            int l = m1.LengthY;
            Matrix result = new Matrix(lengthX, lengthY);

			for(int i = 0; i < lengthX; i++)
				for(int j = 0; j < lengthY; j++) {
					result[i,j] = 0;
					for(int k = 0; k < l; k++)
						result[i,j] += m1[i,k] * m2[k,j];
				}

			return result;
		}

        /// <summary>
        /// Vynásobí dvì matice po prvcích
        /// </summary>
        public static object ItemMul(Matrix m1, Matrix m2) {
            if(m1.LengthX != m2.LengthX || m1.LengthY != m2.LengthY)
                throw new MatrixException(errorMessageDifferentDimension);

            int lengthX = m1.LengthX;
            int lengthY = m1.LengthY;
            Matrix result = new Matrix(lengthX, lengthY);

            for(int i = 0; i < lengthX; i++)
                for(int j = 0; j < lengthY; j++)
                    result[i, j] = m1[i, j] * m2[i, j];

            return result;
        }

		public override bool Equals(object obj) {
			Matrix m = obj as Matrix;

			if(this.LengthX != m.LengthX || this.LengthY != m.LengthY)
				return false;

            int lengthX = m.LengthX;
            int lengthY = m.LengthY;

			for(int i = 0; i < lengthX; i++)
				for(int j = 0; j < lengthY; j++)
					if(this[i, j] != m[i, j])
						return false;

			return true;
		}

		public override int GetHashCode() {
            int lengthX = this.LengthX;
            int lengthY = this.LengthY;
            double result = 0;
			
            for(int i = 0; i < lengthX; i++)
				for(int j = 0; j < lengthY; j++)
					result += this[i, j];
			return (int)(result / System.Math.Pow(10, System.Math.Ceiling(System.Math.Log10(result))) * ((double)int.MaxValue - (double)int.MinValue) + int.MinValue);
        }
        #endregion

		/// <summary>
		/// Stopa
		/// </summary>
		public double Trace() {
			if(!this.IsSquare)
				throw new MatrixException(errorMessageNotSquare);

            int length = this.Length;
			double trace = 0;

			for(int i = 0; i < length; i++)
				trace += item[i,i];

			return trace;
		}

		/// <summary>
		/// Vypoèítá inverzní matici
		/// </summary>
		public Matrix Inv() {
			if(!this.IsSquare)
				throw new MatrixException(errorMessageNotSquare);

			double diskriminant = 1;
			double [,] item = (double [,]) this.item.Clone();

			// Jednotková matice
            int length = this.Length;
            Matrix result = Matrix.Unit(length);

			// Úprava matice na trojúhelníkovou
			for(int i = 0; i < length; i++) {
				int maxi = i;
				double max = System.Math.Abs(item[i,i]);

				// Vyhledání maximálního prvku - pivotace
				for(int j = i + 1; j < length; j++)
					if(max < System.Math.Abs(item[j,i])) {
						maxi = j;
						max = System.Math.Abs(item[j,i]);
					}

				// Prohození øádkù i a maxi
				if(maxi != i) {
					for(int j = 0; j < length; j++) {
						double p = item[maxi,j];
						item[maxi,j] = item[i,j];
						item[i,j] = p;
						p = result[maxi,j];
						result[maxi,j] = result[i,j];
						result[i,j] = p;
					}
					diskriminant = -diskriminant;
				}

				double d = item[i,i];
				
				if(System.Math.Abs(d) == 0)
					throw new MatrixException(errorMessageSingular);

				diskriminant *= d;

				for(int j = 0; j < length; j++) {
					item[i,j] /= d;
					result[i,j] /= d;
				}

				for(int j = 0; j < length; j++) {
					if(i != j) {
						d = item[j,i];
						for(int k = 0; k < result.Length; k++) {
							item[j,k] -= d*item[i,k];
							result[j,k] -= d*result[i,k];
						}
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Transpozice matice
		/// </summary>
		public Matrix Transpose() {
            int lengthX = this.LengthX;
            int lengthY = this.LengthY;
            Matrix result = new Matrix(lengthY, lengthX);

			for(int i = 0; i < lengthX; i++)
				for(int j = 0; j < lengthY; j++)
					result[j, i] = this[i, j];

			return result;
		}

		/// <summary>
		/// Vytvoøí jednotkovou matici
		/// </summary>
		public static Matrix Unit(int length) {
            Matrix result = new Matrix(length);

            for(int i = 0; i < length; i++)
                result[i, i] = 1;

            return result;
		}

		/// <summary>
		/// Matici vynuluje
		/// </summary>
		public void Clear() {
            int lengthX = this.LengthX;
            int lengthY = this.LengthY;
            
            for(int i = 0; i < lengthX; i++)
				for(int j = 0; j < lengthY; j++)
					this[i,j] = 0;
		}

		/// <summary>
		/// Vytvoøí kopii matice
		/// </summary>
		public Matrix Clone() {
			return new Matrix(this.item.Clone() as double[,]);
		}

		/// <summary>
        /// Transformation of the items of the matrix according to the specified transformation function
		/// </summary>
        /// <param name="function">Transformation function</param>
        public Matrix Transform(RealFunction function) {
            int lengthX = this.LengthX;
            int lengthY = this.LengthY;
            Matrix result = new Matrix(lengthX, lengthY);

			for(int i = 0; i < lengthX; i++)
				for(int j = 0; j < lengthY; j++)
					result[i, j] = function(this[i, j]);

			return result;
		}

        /// <summary>
        /// Transformation of the items of the matrix according to the specified transformation function
        /// </summary>
        /// <param name="function">Transformation function</param>
        /// <param name="p">Additional parameters for the transformation function</param>
        public Matrix Transform(RealFunctionWithParams function, params object[] p) {
            int lengthX = this.LengthX;
            int lengthY = this.LengthY;
            Matrix result = new Matrix(lengthX, lengthY);

            for(int i = 0; i < lengthX; i++)
                for(int j = 0; j < lengthY; j++)
                    result[i, j] = function(this[i, j]);

            return result;
        }
        
        /// <summary>
		/// Vrátí øádek matice jako vektor
		/// </summary>
		/// <param name="i">Index øádku</param>
		public Vector GetRowVector(int i) {
            int length = this.LengthY;
            Vector result = new Vector(length);
			
			for(int j = 0; j < length; j++)
				result[j] = this[i,j];

			return result;
		}

		/// <summary>
		/// Vrátí sloupec matice jako vektor
		/// </summary>
		/// <param name="j">Index sloupce</param>
		public Vector GetColumnVector(int j) {
            int length = this.LengthX;
            Vector result = new Vector(length);
			
			for(int i = 0; i < length; i++)
				result[i] = this[i,j];

			return result;
		}
		
		/// <summary>
		/// Nastaví øádek matice podle vstupního vektoru
		/// </summary>
		/// <param name="i">Index øádku</param>
		/// <param name="v">Vektor</param>
		public void SetRowVector(int i, Vector v) {
			if(this.LengthY != v.Length) 
				throw new MatrixException(errorMessageDifferentLength);

            int lengthY = this.LengthY;

			for(int j = 0; j < lengthY; j++)
				this[i,j] = v[j];
		}

		/// <summary>
		/// Nastaví sloupec matice podle vstupního vektoru
		/// </summary>
		/// <param name="j">Index sloupce</param>
		/// <param name="v">Vektor</param>
		public void SetColumnVector(int j, Vector v) {
			if(this.LengthX != v.Length) 
				throw new MatrixException(errorMessageDifferentLength);

            int lengthX = this.LengthX;

			for(int i = 0; i < lengthX; i++)
				this[i,j] = v[i];
		}

        /// <summary>
        /// Poèet nenulových prvkù matice
        /// </summary>
        public int NumNonzeroItems() {
            int result = 0;

            int lengthX = this.LengthX;
            int lengthY = this.LengthY;

            for(int i = 0; i < lengthX; i++)
                for(int j = 0; j < lengthY; j++)
                    if(this[i, j] != 0.0)
                        result++;

            return result;
        }

        /// <summary>
        /// Poèet prvkù matice
        /// </summary>
        public int NumItems() {
            return this.LengthX * this.LengthY;
        }

        /// <summary>
        /// Šíøka pásu, pokud na matici pohlížíme jako na pásovou
        /// </summary>
        public int BandWidth() {
            if(!this.IsSquare)
                throw new MatrixException(errorMessageNotSquare);

            int length = this.Length;
            int lastNonZeroBand = 0;

            for(int i = 0; i < length; i++) {
                bool zeroBand = true;

                for(int j = 0; j < length - i; j++) 
                    if(this[i + j, j] != 0.0 || this[j, i + j] != 0.0) {
                        zeroBand = false;
                        break;
                    }

                if(!zeroBand)
                    lastNonZeroBand = i;
            }

            return 2 * lastNonZeroBand + 1;
        }

        /// <summary>
        /// Souèet všech prvkù
        /// </summary>
        public double Sum() {
            int lengthX = this.LengthX;
            int lengthY = this.LengthY;
            double result = 0.0;

            for(int i = 0; i < lengthX; i++)
                for(int j = 0; j < lengthY; j++)
                    result += this[i, j];

            return result;
        }

        /// <summary>
        /// Souèet všech prvkù v absolutní hodnotì
        /// </summary>
        public double SumAbs() {
            int lengthX = this.LengthX;
            int lengthY = this.LengthY;
            double result = 0.0;

            for(int i = 0; i < lengthX; i++)
                for(int j = 0; j < lengthY; j++)
                    result += System.Math.Abs(this[i, j]);

            return result;
        }

        /// <summary>
        /// Souèet prvkù mimo diagonálu
        /// </summary>
        public double NondiagonalSum() {
			if(!this.IsSquare)
				throw new MatrixException(errorMessageNotSquare);

            int length = this.Length;

            double sum = 0;
			for(int i = 0; i < length; i++)
				for(int j = 0; j < length; j++)
					if(i != j) sum += this[i,j];

			return sum;
		}

		/// <summary>
		/// Souèet absolutních hodnot prvkù mimo diagonálu
		/// </summary>
		public double NondiagonalAbsSum() {
			if(!this.IsSquare)
				throw new MatrixException(errorMessageNotSquare);

            int length = this.Length;

			double sum = 0;
			for(int i = 0; i < length; i++)
				for(int j = 0; j < length; j++)
					if(i != j) sum += System.Math.Abs(this[i, j]);

			return sum;
		}

		/// <summary>
		/// Spoèítá euklidovskou normu øádku
		/// </summary>
		/// <param name="row">Index øádku</param>
		public double EuklideanRowNorm(int row) {
			double result = 0;

            int lengthY = this.LengthY;

			for(int j = 0; j < lengthY; j++)
				result += this[row, j] * this[row, j];

			return System.Math.Sqrt(result);
		}

		/// <summary>
		/// Spoèítá absolutní normu øádku
		/// </summary>
		/// <param name="row">Index øádku</param>
		public double AbsRowNorm(int row) {
			double result = 0;

            int lengthY = this.LengthY;

			for(int j = 0; j < lengthY; j++)
				result += System.Math.Abs(this[row, j]);

			return result;
		}
		
		/// <summary>
		/// Spoèítá euklidovskou normu sloupce
		/// </summary>
		/// <param name="column">Index sloupce</param>
		public double EuklideanColumnNorm(int column) {
			double result = 0;

            int lengthX = this.LengthX;

			for(int i = 0; i < lengthX; i++)
				result += this[i, column] * this[i, column];

			return System.Math.Sqrt(result);
		}

		/// <summary>
		/// Spoèítá euklidovskou normu sloupce
		/// </summary>
		/// <param name="column">Index sloupce</param>
		public double AbsColumnNorm(int column) {
			double result = 0;

            int lengthX = this.LengthX;

			for(int i = 0; i < lengthX; i++)
				result += System.Math.Abs(this[i, column]);

			return result;
		}

		/// <summary>
		/// Spoèítá druhou mocninu euklidovské normy matice
		/// </summary>
		public double SquaredEuklideanNorm() {
			double result = 0;

            int lengthX = this.LengthX;
            int lengthY = this.LengthY;

			for(int i = 0; i < lengthX; i++)
				for(int j = 0; j < lengthY; j++)
					result += this[i, j] * this[i, j];

			return result;
		}

        /// <summary>
        /// Spoèítá euklidovskou normu matice
        /// </summary>
        public double EuklideanNorm() {
            return System.Math.Abs(this.SquaredEuklideanNorm());
        }

		/// <summary>
		/// Spoèítá absolutní normu matice
		/// </summary>
		public double AbsNorm() {
			double result = 0;

            int lengthX = this.LengthX;
            int lengthY = this.LengthY;

			for(int i = 0; i < lengthX; i++)
				for(int j = 0; j < lengthY; j++)
					result += System.Math.Abs(this[i, j]);

			return result;
		}

		/// <summary>
		/// Násobí øádek matice èíslem
		/// </summary>
		/// <param name="row">Index øádku</param>
		/// <param name="d">Èíslo</param>
		public void MultiplyRow(int row, double d) {
            int lengthY = this.LengthY;
            for(int j = 0; j < lengthY; j++)
				this[row, j] *= d;
		}

		/// <summary>
		/// Násobí sloupec matice èíslem
		/// </summary>
		/// <param name="column">Index sloupce</param>
		/// <param name="d">Èíslo</param>
		public void MultiplyColumn(int column, double d) {
            int lengthX = this.LengthX;
            for(int i = 0; i < lengthX; i++)
				this[i, column] *= d;
		}

		/// <summary>
		/// Prohodí dva øádky
		/// </summary>
		/// <param name="row1">Øádek 1</param>
		/// <param name="row2">Øádek 2</param>
		public void SwapRows(int row1, int row2) {
            int lengthY = this.LengthY;
            for(int j = 0; j < this.LengthY; j++) {
				double item = this[row1, j];
				this[row1, j] = this[row2, j];
				this[row2, j] = item;
			}
		}

		/// <summary>
		/// Prohodí dva sloupce
		/// </summary>
		/// <param name="column1">Sloupec 1</param>
		/// <param name="column2">Sloupec 2</param>
		public void SwapColumns(int column1, int column2) {
            int lengthX = this.LengthX;
            for(int i = 0; i < lengthX; i++) {
				double item = this[i, column1];
				this[i, column1] = this[i, column2];
				this[i, column2] = item;
			}
		}

        /// <summary>
        /// Symmetrize given matrix
        /// </summary>
        public Matrix Symmetrize() {
            if(!this.IsSquare)
                throw new MatrixException(errorMessageNotSquare);

            int length = this.Length;
            Matrix result = new Matrix(length);

            for(int i = 0; i < length; i++)
                for(int j = 0; j < length; j++)
                    result[i, j] = 0.5 * (this[i, j] + this[j, i]);

            return result;
        }


        #region Funkce Min, Max
        /// <summary>
        /// Vrací index nejvìtšího prvku matice
        /// </summary>
        public int[] MaxIndex() {
			if(this.LengthX == 0 || this.LengthY == 0)
				throw new MatrixException(errorMessageNoData);

            int lengthX = this.LengthX;
            int lengthY = this.LengthY;

            int[] result = { 0, 0 };
			double max = this[0, 0];

			for(int i = 0; i < lengthX; i++) 
				for(int j = 0; j < lengthY; j++)
					if(max < this[i, j]) {
						result[0] = i;
						result[1] = j;
						max = this[i, j];
					}

			return result;
		}

		/// <summary>
		/// Vybere maximální prvek z matice
		/// </summary>
		public double Max() {
			int [] index = this.MaxIndex();
			return this[index[0], index[1]];
		}

		/// <summary>
		/// Vrací index nejvìtšího prvku matice (v absolutní hodnotì)
		/// </summary>
		public int [] MaxAbsIndex() {
			if(this.LengthX == 0 || this.LengthY == 0)
				throw new MatrixException(errorMessageNoData);

            int lengthX = this.LengthX;
            int lengthY = this.LengthY;

			int [] result = {0, 0};
			double max = System.Math.Abs(this[0, 0]);

			for(int i = 0; i < lengthX; i++) 
				for(int j = 0; j < lengthY; j++) {
					double d = System.Math.Abs(this[i, j]);
					if(max < d) {
						result[0] = i;
						result[1] = j;
						max = d;
					}
				}

			return result;
		}

		/// <summary>
		/// Vybere maximální prvek z matice (v absolutní hodnotì)
		/// </summary>
		public double MaxAbs() {
			int [] index = this.MaxAbsIndex();
			return this[index[0], index[1]];
		}

		/// <summary>
		/// Vrací index nejvmenšího prvku matice
		/// </summary>
		public int [] MinIndex() {
			if(this.LengthX == 0 || this.LengthY == 0)
				throw new MatrixException(errorMessageNoData);

            int lengthX = this.LengthX;
            int lengthY = this.LengthY;

			int [] result = {0, 0};
			double min = this[0, 0];

			for(int i = 0; i < lengthX; i++) 
				for(int j = 0; j < lengthY; j++)
					if(min > this[i, j]) {
						result[0] = i;
						result[1] = j;
						min = this[i, j];
					}

			return result;
		}

		/// <summary>
		/// Vybere minimální prvek z matice
		/// </summary>
		public double Min() {
			int [] index = this.MinIndex();
			return this[index[0], index[1]];
		}
		
		/// <summary>
		/// Vrací index nejvmenšího prvku matice (v absolutní hodnotì)
		/// </summary>
		public int [] MinAbsIndex() {
			if(this.LengthX == 0 || this.LengthY == 0)
				throw new MatrixException(errorMessageNoData);

            int lengthX = this.LengthX;
            int lengthY = this.LengthY;

			int [] result = {0, 0};
			double min = System.Math.Abs(this[0, 0]);

			for(int i = 0; i < lengthX; i++) 
				for(int j = 0; j < lengthY; j++) {
					double d = System.Math.Abs(this[i, j]);
					if(min > d) {
						result[0] = i;
						result[1] = j;
						min = d;
					}
				}

			return result;
		}

		/// <summary>
		/// Vybere minimální prvek z matice (v absolutní hodnotì)
		/// </summary>
		public double MinAbs() {
			int [] index = this.MinAbsIndex();
			return this[index[0], index[1]];
		}
		#endregion

        #region Implementace IExportable
        /// <summary>
        /// Uloží obsah matice do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            int lengthX = this.LengthX;
            int lengthY = this.LengthY;

            if(export.Binary) {
                // Binárnì
                BinaryWriter b = export.B;
                b.Write(lengthX);
                b.Write(lengthY);
                for(int i = 0; i < lengthX; i++)
                    for(int j = 0; j < lengthY; j++)
                        b.Write(this[i, j]);
            }
            else {
                // Textovì
                StreamWriter t = export.T;
                t.WriteLine("{0}\t{1}", lengthX, lengthY);
                for(int i = 0; i < lengthX; i++) {
                    for(int j = 0; j < lengthY; j++)
                        t.Write("{0}\t", this[i, j]);
                    t.WriteLine();
                }
            }
        }

        /// <summary>
        /// Naète obsah matice ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public Matrix(Core.Import import) {
            if(import.Binary) {
                // Binárnì
                BinaryReader b = import.B;
                int lengthX = b.ReadInt32();
                int lengthY = b.ReadInt32();

                this.item = new double[lengthX, lengthY];
                for(int i = 0; i < lengthX; i++)
                    for(int j = 0; j < lengthY; j++)
                        this[i, j] = b.ReadDouble();
            }
            else {
                // Textovì
                StreamReader t = import.T;
                string line = t.ReadLine();
                string[] s = line.Split('\t');
                int lengthX = int.Parse(s[0]);
                int lengthY = int.Parse(s[1]);

                this.item = new double[lengthX, lengthY];
                for(int i = 0; i < lengthX; i++) {
                    line = t.ReadLine();
                    s = line.Split('\t');
                    for(int j = 0; j < lengthY; j++)
                        this[i, j] = double.Parse(s[j]);
                }
            }
        }
        #endregion

		/// <summary>
		/// Pøevede matici na øetìzec
		/// </summary>
		public override string ToString() {
			StringBuilder s = new StringBuilder();

            int lengthX = this.LengthX;
            int lengthY = this.LengthY;

			for(int i = 0; i < lengthX; i++) {
				for(int j = 0; j < lengthY; j++)
					s.Append(string.Format("{0,10:#####0.000}\t", this[i, j]));
				s.Append('\n');
			}

			return s.ToString();
		}

		#region Výpoèet vlastních hodnot
		/// <summary>
		/// Provede ekvivalentní úpravy matice tak, aby odpovídající si sloupce
		/// a øádky mìly stejnou normu
		/// </summary>
		private Matrix Balance() {
			Matrix result = this.Clone();

			const double radix = 2.0;
			const double sqradix = 4.0;
			const double boundary = 0.95;

			bool finish = false;
			while (!finish) {
				finish = true;

				for(int i = 0; i < result.Length; i++) {
					double rowNorm = result.AbsRowNorm(i) - result[i, i];
					double colNorm = result.AbsColumnNorm(i) - result[i, i];
				
					if(rowNorm != 0 && colNorm != 0) {
						double g = rowNorm / radix;
						double f = 0;
						double sumNorm = rowNorm + colNorm;

						// Nalezneme mocninu radixu (základní jednotky strojového èísla),
						// která je nejblíže k balancované matici
						while(colNorm < g) {
							f *= radix;
							colNorm *= sqradix;
						}
						g = rowNorm * radix;
						while (colNorm > g) {
							f /= radix;
							colNorm /= sqradix;
						}
						
						if((colNorm + rowNorm) / f < boundary * sumNorm) {
							finish = false;
							g = 1.0 / f;

							// Podobnostní tranformace
							result.MultiplyColumn(i, f);
							result.MultiplyRow(i, g);
						}
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Pøevede matici podobnostními transformacemi na Hessenbergùv tvar
		/// (matice, která má pod diagonálou nenulový prvek a vše níž je nulové)
		/// </summary>
		private Matrix Hessenberg() {
			Matrix result = this.Clone();

			for(int i = 1; i < result.Length - 1; i++) {
				int jMax = i;

				// Nalezneme pivot
				for(int j = i + 1; j < result.Length; j++)
					if(System.Math.Abs(result[j, i - 1]) > System.Math.Abs(result[jMax, i - 1]))
						jMax = j;

				double maxItem = result[jMax, i - 1];

				// Prohodíme sloupce a øádky
				if(jMax != i) {
					result.SwapRows(jMax, i);
					result.SwapColumns(jMax, i);
				}

				// Vlastní eliminace
				if(maxItem != 0) {
					for(int j = i + 1; j < result.Length; j++) {
						double item = result[j, i - 1];
						if(item != 0) {
							item /= maxItem;
							result[j, i - 1] = item;

							for(int k = i; k < result.Length; k++)
								result[j, k] -= item * result[i, k];
							for(int k = 0; k < result.Length; k++)
								result[k, i] += item * result[k, j];
						}
					}
				}

				// Nulování nepotøebných prvkù
				for(int j = i + 1; j < result.Length; j++) {
					result[j, i - 1] = 0;
				}
			}

			return result;
		}

		/// <summary>
		/// Vypoèítá vlastní hodnoty matice v Hessenbergovì tvaru
		/// </summary>
		public ComplexVector EigenValues() {
			Matrix work = this.Balance().Hessenberg();
			ComplexVector result = new ComplexVector(work.Length);

			double wNorm = work.AbsNorm();
			double t = 0;
			int wPos = work.Length - 1;

			while(wPos >= 0) {
				// Poèet itegrací
				int iterations = 0;
				const int maxIterations = 3000;
				const int shiftIterations = 10;

				// Poèet nediagonálních elementù
				int smallIndex = 0;

				do {
					for(smallIndex = wPos; smallIndex >= 1; smallIndex--) {
						double s = System.Math.Abs(work[smallIndex - 1, smallIndex - 1]) + System.Math.Abs(work[smallIndex, smallIndex]);
						if(s == 0)
							s = wNorm;

						// Našli jsme malý nediagonální element?
						if(System.Math.Abs(work[smallIndex, smallIndex - 1]) + s == s)
							break;
					}
					
					// Elementy submatice 2 x 2 na diagonále
					double x = work[wPos, wPos];

					// Našli jsme jeden koøen
					if(smallIndex == wPos) {
						result[wPos].Re = x + t;
						result[wPos].Im = 0;
						wPos--;
					}
						// Našli jsme dva koøeny
					else {
						double y = work[wPos - 1, wPos - 1];
						double z = work[wPos, wPos - 1] * work[wPos - 1, wPos];

						if(smallIndex == wPos - 1) {
							double p = 0.5 * (y - x);
							double q = p * p + z;
							double w = System.Math.Sqrt(System.Math.Abs(q));
							
							x += t;
	
							// Reálná dvojice
							if(q >= 0.0) {
								w = p + w * (p >= 0.0 ? 1.0 : -1.0);
								result[wPos - 1].Re = x + w;
								if(w != 0)
									result[wPos].Re = x - z/w;
								else
									result[wPos].Re = x + w;
								result[wPos].Im = result[wPos - 1].Im = 0;
							}
								// Komplexní dvojice
							else {
								result[wPos].Re = result[wPos - 1].Re = x + p;
								result[wPos].Im = w;
								result[wPos - 1].Im = -w;
							}
							wPos -= 2;
						}
							// Žádný koøen, pokraèujeme v iteraci
						else {
							if(iterations > maxIterations)
								throw new Exception("Pøekroèen maximální poèet iterací pøi výpoètu vlastních hodnot matice!");
							// Mimoøádné posunutí
							if(iterations != 0 && (iterations % shiftIterations) == 0) {
								double s = System.Math.Abs(work[wPos, wPos - 1]) + System.Math.Abs(work[wPos - 1, wPos - 2]);

								for(int i = 0; i <= wPos; i++)
									work[i, i] = -x;

								t += x;
								x = y = 0.75 * s;
								z = -0.4375 * s * s;
							}

							iterations++;

							int index = 0;

							// Householdùv vektor
							Vector hhVector = new Vector(3);

							// Posunutí a nalezení dvou po sobì jdoucích malých nediagonálních elementù
							for(index = wPos - 2; index >= smallIndex; index--) {
								double w = work[index, index];
								double r = x - w;
								double s = y - w;

								hhVector[0] = (r*s - z) / work[index + 1, index] + work[index, index + 1];
								hhVector[1] = work[index + 1, index + 1] - w - r - s;
								hhVector[2] = work[index + 2, index + 1];

								// Škálování (snažíme se pøedejít pøeteèení)
								hhVector = hhVector.AbsNormalization();

								if(index == smallIndex)
									break;

								r = System.Math.Abs(work[index, index - 1]) * (System.Math.Abs(hhVector[1]) + System.Math.Abs(hhVector[2]));
								s = System.Math.Abs(hhVector[0]) * (System.Math.Abs(work[index - 1, index - 1]) + System.Math.Abs(w) + System.Math.Abs(work[index + 1, index + 1]));
								if(r + s == s)
									break;
							}

							for(int i = index + 2; i <= wPos; i++) {
								work[i, i - 2] = 0;
								if(i != index + 2)
									work[i, i - 3] = 0;
							}

							for(int i = index; i < wPos; i++) {
								if(i != index) {
									hhVector[0] = work[i, i - 1];
									hhVector[1] = work[i + 1, i - 1];
									if(i == wPos - 1)
										hhVector[2] = 0;
									else
										hhVector[2] = work[i + 2, i - 1];

									x = hhVector.AbsNorm();
									if(x != 0) 
										hhVector /= x;
								}

								double s = hhVector.EuklideanNorm() * (hhVector[0] >= 0.0 ? 1.0 : -1.0);
								if(s != 0) {
									if(i == index) {
										if(smallIndex != index)
											work[i, i - 1] = -work[i, i - 1];
									}
									else
										work[i, i - 1] = -s * x;

									hhVector[0] += s;

									Vector vector = hhVector / s;
									hhVector[1] /= hhVector[0];
									hhVector[2] /= hhVector[0];

									// Modifikace øádkù
									for(int j = i; j <= wPos; j++) {
										hhVector[0] = work[i, j] + hhVector[1] * work[i + 1, j];
										if(i != wPos - 1) {
											hhVector[0] += hhVector[2] * work[i + 2, j];
											work[i + 2, j] -= hhVector[0] * vector[2];
										}
										work[i + 1, j] -= hhVector[0] * vector[1];
										work[i, j] -= hhVector[0] * vector[0];
									}

									int mmin = wPos < i + 3 ? wPos : i + 3;

									// Modifikace sloupcù
									for(int j = smallIndex; j <= mmin; j++) {
										hhVector[0] = vector[0] * work[j, i] + vector[1] * work[j, i + 1];
										if(i != wPos - 1) {
											hhVector[0] += vector[2] * work[j, i + 2];
											work[j, i + 2] -= hhVector[0] * hhVector[2];
										}

										work[j, i + 1] -= hhVector[0] * hhVector[1];
										work[j, i] -= hhVector[0];
									}
								}
							}
						}
					}
				} while(smallIndex < wPos - 1);
			}

			return result;
		}

        /// <summary>
        /// Vrátí pouze reálné vlastní hodnoty
        /// </summary>
        public Vector RealEigenValues() {
            ComplexVector cv = this.EigenValues();
            ArrayList a = new ArrayList();

            for(int i = 0; i < cv.Length; i++)
                if(cv[i].Re != 0 && System.Math.Abs(cv[i].Im / cv[i].Re) < zero)
                    a.Add(cv[i].Re);
                else if(cv[i].Re == 0 && System.Math.Abs(cv[i].Im) < zero)
                    a.Add(0.0);

            Vector result = new Vector(a.Count);
            for(int i = 0; i < a.Count; i++)
                result[i] = (double)a[i];

            return result;
        }
        #endregion

        private double zero = 1E-10;

		private const string errorMessageNoData = "K provedení operace je nutné, aby velikost matice nebyla nulová.";
		private const string errorMessageNotSquare = "Pro urèení jedineèné velikosti matice musí být matice ètvercová.";
		private const string errorMessageDifferentDimension = "K provedení operace musí mít matice stejné rozmìry.";
		private const string errorMessageMultiplication = "Pøi násobení matic musí mít matice kompatibilní rozmìry.";
		private const string errorMessageSingular = "Matrix je singulární. Inverzi nelze spoèítat.";
		private const string errorMessageDifferentLength = "K provedení operace musí mít vektor stejnou délku jako odpovídající rozmìr matice.";
    }

	/// <summary>
	/// Výjimka ve tøídì Matrix
	/// </summary>
	public class MatrixException: ApplicationException {
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public MatrixException(string message) : base(errMessage + message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public MatrixException(string message, Exception innerException) : base(errMessage + message, innerException) {}

		private const string errMessage = "Ve tøídì Matrix došlo k chybì: ";
	}
}
