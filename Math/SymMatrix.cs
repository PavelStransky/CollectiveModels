using System;
using System.Collections;
using System.IO;
using System.Text;
/*
namespace PavelStransky.Math {
    /// <summary>
    /// Implementace operac� se symetrick�mi maticemi
    /// </summary>
    public class SymMatrix : IExportable {
        private double[] item;
        private int length;

        // +--------> Y
        // |
        // |	// rozm�ry
        // v
        // X

        #region Konstruktory
        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        public SymMatrix() { }

        /// <summary>
        /// Vytvo�� symetrickou matici
        /// </summary>
        /// <param name="length">Rozm�r</param>
        public SymMatrix(int length) {
            this.length = length;
            this.item = new double[length * (length + 1) / 2];
        }
        #endregion

        /// <summary>
        /// Rozm�r matice
        /// </summary>
        public int Length { get { return this.length; } }

        /// <summary>
        /// Po�et prvk� matice
        /// </summary>
        public int Size { get { return this.item.Length; } }

        /// <summary>
        /// Indexer
        /// </summary>
        public double this[int i, int j] {
            get {
                if(i < j)
                    return this.item[i * length + j - i * (i + 1) / 2];
                else
                    return this.item[j * length + i - j * (j + 1) / 2];
            }
            set {
                if(i < j)
                    this.item[i * length + j - i * (i + 1) / 2] = value;
                else
                    this.item[j * length + i - j * (j + 1) / 2] = value;
            }
        }

        /// <summary>
        /// Indexer pro p��mou indexaci prvk�
        /// </summary>
        public double this[int i] { get { return this.item[i]; } set { this.item[i] = value; } }

        #region Oper�tory
        /// <summary>
        /// Se�te dv� matice stejn�ch rozm�r�
        /// </summary>
        public static SymMatrix operator +(SymMatrix m1, SymMatrix m2) {
            if(m1.Length != m2.Length)
                throw new MatrixException(errorMessageDifferentDimension);

            int size = m1.Size;
            SymMatrix result = new SymMatrix(m1.Length);

            for(int i = 0; i < size; i++)
                    result[i] = m1[i] + m2[i];

            return result;
        }

        /// <summary>
        /// P�i�te ke ka�d� slo�ce matice ��slo
        /// </summary>
        public static SymMatrix operator +(SymMatrix m, double d) {
            int size = m.Size;
            SymMatrix result = new SymMatrix(m.Length);

            for(int i = 0; i < size; i++)
                    result[i] = m[i] + d;

            return result;
        }

        /// <summary>
        /// Ode�te dv� matice stejn�ch rozm�r�
        /// </summary>
        public static SymMatrix operator -(SymMatrix m1, SymMatrix m2) {
            if(m1.Length != m2.Length)
                throw new MatrixException(errorMessageDifferentDimension);

            int size = m1.Size;
            SymMatrix result = new SymMatrix(m1.Length);

            for(int i = 0; i < size; i++)
                    result[i] = m1[i] - m2[i];

            return result;
        }

        /// <summary>
        /// Ode�te od ka�d� slo�ky matice ��slo
        /// </summary>
        public static SymMatrix operator -(SymMatrix m, double d) {
            int size = m.Size;
            SymMatrix result = new SymMatrix(m.Length);

            for(int i = 0; i < size; i++)
                result[i] = m[i] - d;

            return result;
        }

        /// <summary>
        /// Ode�te od ��sla ka�dou slo�ku matice
        /// </summary>
        public static SymMatrix operator -(double d, SymMatrix m) {
            int size = m.Size;
            SymMatrix result = new SymMatrix(m.Length);

            for(int i = 0; i < size; i++)
                result[i] = d - m[i];

            return result;
        }

        /// <summary>
        /// Vyn�sob� matici ��slem
        /// </summary>
        public static SymMatrix operator *(SymMatrix m, double koef) {
            int size = m.Size;
            SymMatrix result = new SymMatrix(m.Length);

            for(int i = 0; i < size; i++)
                result[i] = koef * m[i];

            return result;
        }

        /// <summary>
        /// Vyd�l� matici ��slem
        /// </summary>
        public static SymMatrix operator /(SymMatrix m, double koef) {
            int size = m.Size;
            SymMatrix result = new SymMatrix(m.Length);

            for(int i = 0; i < size; i++)
                result[i] = m[i] / koef;

            return result;
        }

        /// <summary>
        /// D�len� ��sla matic�
        /// </summary>
        public static SymMatrix operator /(double koef, SymMatrix m) {
            return m.Inv() * koef;
        }

        /// <summary>
        /// D�len� dvou matic
        /// </summary>
        public static SymMatrix operator /(SymMatrix m1, SymMatrix m2) {
            return m1 * m2.Inv();
        }

        /// <summary>
        /// Vyn�sob� dv� matice
        /// </summary>
        public static SymMatrix operator *(SymMatrix m1, SymMatrix m2) {
            if(m1.Length != m2.Length)
                throw new MatrixException(errorMessageMultiplication);

            int length = m1.Length;
            SymMatrix result = new SymMatrix(length);

            for(int i = 0; i < length; i++)
                for(int j = i; j < length; j++) {
                    result[i, j] = 0;
                    for(int k = 0; k < length; k++)
                        result[i, j] += m1[i, k] * m2[k, j];
                }

            return result;
        }

        public override bool Equals(object obj) {
            SymMatrix m = obj as SymMatrix;

            if(this.Length != m.Length)
                return false;

            int size = m.Size;

            for(int i = 0; i < size; i++)
                if(this[i] != m[i])
                    return false;

            return true;
        }

        public override int GetHashCode() {
            int size = this.Size;
            double result = 0;

            for(int i = 0; i < size; i++)
                result += this[i];
            return (int)(result / System.Math.Pow(10, System.Math.Ceiling(System.Math.Log10(result))) * ((double)int.MaxValue - (double)int.MinValue) + int.MinValue);
        }
        #endregion

        /// <summary>
        /// Stopa
        /// </summary>
        public double Trace() {
            int length = this.Length;
            double trace = 0;

            for(int i = 0; i < length; i++)
                trace += item[i, i];

            return trace;
        }

        /// <summary>
        /// Vypo��t� inverzn� matici
        /// </summary>
        public Matrix Inv() {
            double diskriminant = 1;
            SymMatrix work = this.Clone();

            // Jednotkov� matice
            int length = this.Length;
            SymMatrix result = new SymMatrix(length);
            result.SetUnit();

            // �prava matice na troj�heln�kovou
            for(int i = 0; i < length; i++) {
                int maxi = i;
                double max = System.Math.Abs(work[i, i]);

                // Vyhled�n� maxim�ln�ho prvku - pivotace
                for(int j = i + 1; j < length; j++)
                    if(max < System.Math.Abs(work[j, i])) {
                        maxi = j;
                        max = System.Math.Abs(work[j, i]);
                    }

                // Prohozen� ��dk� i a maxi
                if(maxi != i) {
                    for(int j = 0; j < length; j++) {
                        double p = work[maxi, j];
                        work[maxi, j] = work[i, j];
                        work[i, j] = p;
                        p = result[maxi, j];
                        result[maxi, j] = result[i, j];
                        result[i, j] = p;
                    }
                    diskriminant = -diskriminant;
                }

                double d = work[i, i];

                if(System.Math.Abs(d) == 0)
                    throw new MatrixException(errorMessageSingular);

                diskriminant *= d;

                for(int j = 0; j < length; j++) {
                    work[i, j] /= d;
                    result[i, j] /= d;
                }

                for(int j = 0; j < length; j++) {
                    if(i != j) {
                        d = work[j, i];
                        for(int k = 0; k < result.Length; k++) {
                            work[j, k] -= d * work[i, k];
                            result[j, k] -= d * result[i, k];
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Matici p�evede na jednotkovou
        /// </summary>
        public void SetUnit() {
            int length = this.Length;

            for(int i = 0; i < length; i++) {
                this[i, i] = 1;
                for(int j = i + 1; j < length; j++)
                    this[i, j] = 0;
            }
        }

        /// <summary>
        /// Matici vynuluje
        /// </summary>
        public void Clear() {
            int size = this.Size;
            for(int i = 0; i < size; i++)
                this[i] = 0;
        }

        /// <summary>
        /// Vytvo�� kopii matice
        /// </summary>
        public SymMatrix Clone() {
            SymMatrix result = new SymMatrix();
            result.item = this.item.Clone() as double[];
            return result;
        }

        /// <summary>
        /// Transformace prvk� matice podle zadan� transforma�n� funkce
        /// </summary>
        public SymMatrix Transform(RealFunction function) {
            int size = this.Size;
            SymMatrix result = new SymMatrix(this.Length);

            for(int i = 0; i < size; i++)
                result[i] = function(this[i]);

            return result;
        }

        /// <summary>
        /// Vr�t� ��dek matice jako vektor
        /// </summary>
        /// <param name="i">Index ��dku</param>
        public Vector GetRowVector(int i) {
            int length = this.Length;
            Vector result = new Vector(length);

            for(int j = 0; j < length; j++)
                result[j] = this[i, j];

            return result;
        }

        /// <summary>
        /// Vr�t� sloupec matice jako vektor
        /// </summary>
        /// <param name="j">Index sloupce</param>
        public Vector GetColumnVector(int j) {
            int length = this.Length;
            Vector result = new Vector(length);

            for(int i = 0; i < length; i++)
                result[i] = this[i, j];

            return result;
        }

        /// <summary>
        /// Nastav� ��dek matice podle vstupn�ho vektoru
        /// </summary>
        /// <param name="i">Index ��dku</param>
        /// <param name="v">Vektor</param>
        public void SetRowVector(int i, Vector v) {
            if(this.Length != v.Length)
                throw new MatrixException(errorMessageDifferentLength);

            int length = this.Length;

            for(int j = 0; j < length; j++)
                this[i, j] = v[j];
        }

        /// <summary>
        /// Nastav� sloupec matice podle vstupn�ho vektoru
        /// </summary>
        /// <param name="j">Index sloupce</param>
        /// <param name="v">Vektor</param>
        public void SetColumnVector(int j, Vector v) {
            if(this.Length != v.Length)
                throw new MatrixException(errorMessageDifferentLength);

            int length = this.Length;

            for(int i = 0; i < length; i++)
                this[i, j] = v[i];
        }

        /// <summary>
        /// Sou�et v�ech prvk�
        /// </summary>
        public double Sum() {
            int size = this.Size;
            int rowLength = this.Length;
            int rowRemain = 0;
            double result = 0.0;

            for(int i = 0; i < size; i++) {
                if(rowRemain == 0) {
                    rowLength--;
                    rowRemain = rowLength;
                    result += this[i];
                }
                else
                    result += 2.0 * this[i];
            }

            return result;
        }

        /// <summary>
        /// Sou�et v�ech prvk� v absolutn� hodnot�
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
        /// Sou�et prvk� mimo diagon�lu
        /// </summary>
        public double NondiagonalSum() {
            if(!this.IsSquare)
                throw new MatrixException(errorMessageNotSquare);

            int length = this.Length;

            double sum = 0;
            for(int i = 0; i < length; i++)
                for(int j = 0; j < length; j++)
                    if(i != j) sum += this[i, j];

            return sum;
        }

        /// <summary>
        /// Sou�et absolutn�ch hodnot prvk� mimo diagon�lu
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
        /// Spo��t� euklidovskou normu ��dku
        /// </summary>
        /// <param name="row">Index ��dku</param>
        public double EuklideanRowNorm(int row) {
            double result = 0;

            int lengthY = this.LengthY;

            for(int j = 0; j < lengthY; j++)
                result += this[row, j] * this[row, j];

            return System.Math.Sqrt(result);
        }

        /// <summary>
        /// Spo��t� absolutn� normu ��dku
        /// </summary>
        /// <param name="row">Index ��dku</param>
        public double AbsRowNorm(int row) {
            double result = 0;

            int lengthY = this.LengthY;

            for(int j = 0; j < lengthY; j++)
                result += System.Math.Abs(this[row, j]);

            return result;
        }

        /// <summary>
        /// Spo��t� euklidovskou normu sloupce
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
        /// Spo��t� euklidovskou normu sloupce
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
        /// Spo��t� druhou mocninu euklidovsk� normy matice
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
        /// Spo��t� euklidovskou normu matice
        /// </summary>
        public double EuklideanNorm() {
            return System.Math.Abs(this.SquaredEuklideanNorm());
        }

        /// <summary>
        /// Spo��t� absolutn� normu matice
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
        /// N�sob� ��dek matice ��slem
        /// </summary>
        /// <param name="row">Index ��dku</param>
        /// <param name="d">��slo</param>
        public void MultiplyRow(int row, double d) {
            int lengthY = this.LengthY;
            for(int j = 0; j < lengthY; j++)
                this[row, j] *= d;
        }

        /// <summary>
        /// N�sob� sloupec matice ��slem
        /// </summary>
        /// <param name="column">Index sloupce</param>
        /// <param name="d">��slo</param>
        public void MultiplyColumn(int column, double d) {
            int lengthX = this.LengthX;
            for(int i = 0; i < lengthX; i++)
                this[i, column] *= d;
        }

        /// <summary>
        /// Prohod� dva ��dky
        /// </summary>
        /// <param name="row1">��dek 1</param>
        /// <param name="row2">��dek 2</param>
        public void SwapRows(int row1, int row2) {
            int lengthY = this.LengthY;
            for(int j = 0; j < this.LengthY; j++) {
                double item = this[row1, j];
                this[row1, j] = this[row2, j];
                this[row2, j] = item;
            }
        }

        /// <summary>
        /// Prohod� dva sloupce
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

        #region Funkce Min, Max
        /// <summary>
        /// Vrac� index nejv�t��ho prvku matice
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
        /// Vybere maxim�ln� prvek z matice
        /// </summary>
        public double Max() {
            int[] index = this.MaxIndex();
            return this[index[0], index[1]];
        }

        /// <summary>
        /// Vrac� index nejv�t��ho prvku matice (v absolutn� hodnot�)
        /// </summary>
        public int[] MaxAbsIndex() {
            if(this.LengthX == 0 || this.LengthY == 0)
                throw new MatrixException(errorMessageNoData);

            int lengthX = this.LengthX;
            int lengthY = this.LengthY;

            int[] result = { 0, 0 };
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
        /// Vybere maxim�ln� prvek z matice (v absolutn� hodnot�)
        /// </summary>
        public double MaxAbs() {
            int[] index = this.MaxAbsIndex();
            return this[index[0], index[1]];
        }

        /// <summary>
        /// Vrac� index nejvmen��ho prvku matice
        /// </summary>
        public int[] MinIndex() {
            if(this.LengthX == 0 || this.LengthY == 0)
                throw new MatrixException(errorMessageNoData);

            int lengthX = this.LengthX;
            int lengthY = this.LengthY;

            int[] result = { 0, 0 };
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
        /// Vybere minim�ln� prvek z matice
        /// </summary>
        public double Min() {
            int[] index = this.MinIndex();
            return this[index[0], index[1]];
        }

        /// <summary>
        /// Vrac� index nejvmen��ho prvku matice (v absolutn� hodnot�)
        /// </summary>
        public int[] MinAbsIndex() {
            if(this.LengthX == 0 || this.LengthY == 0)
                throw new MatrixException(errorMessageNoData);

            int lengthX = this.LengthX;
            int lengthY = this.LengthY;

            int[] result = { 0, 0 };
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
        /// Vybere minim�ln� prvek z matice (v absolutn� hodnot�)
        /// </summary>
        public double MinAbs() {
            int[] index = this.MinAbsIndex();
            return this[index[0], index[1]];
        }
        #endregion

        #region Implementace IExportable
        /// <summary>
        /// Ulo�� obsah matice do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            int lengthX = this.LengthX;
            int lengthY = this.LengthY;

            if(export.Binary) {
                // Bin�rn�
                BinaryWriter b = export.B;
                b.Write(lengthX);
                b.Write(lengthY);
                for(int i = 0; i < lengthX; i++)
                    for(int j = 0; j < lengthY; j++)
                        b.Write(this[i, j]);
            }
            else {
                // Textov�
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
        /// Na�te obsah matice ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public void Import(Import import) {
            if(import.Binary) {
                // Bin�rn�
                BinaryReader b = import.B;
                int lengthX = b.ReadInt32();
                int lengthY = b.ReadInt32();

                this.item = new double[lengthX, lengthY];
                for(int i = 0; i < lengthX; i++)
                    for(int j = 0; j < lengthY; j++)
                        this[i, j] = b.ReadDouble();
            }
            else {
                // Textov�
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
        /// P�evede matici na �et�zec
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

        #region V�po�et vlastn�ch hodnot
        /// <summary>
        /// Provede ekvivalentn� �pravy matice tak, aby odpov�daj�c� si sloupce
        /// a ��dky m�ly stejnou normu
        /// </summary>
        private Matrix Balance() {
            Matrix result = this.Clone();

            const double radix = 2.0;
            const double sqradix = 4.0;
            const double boundary = 0.95;

            bool finish = false;
            while(!finish) {
                finish = true;

                for(int i = 0; i < result.Length; i++) {
                    double rowNorm = result.AbsRowNorm(i) - result[i, i];
                    double colNorm = result.AbsColumnNorm(i) - result[i, i];

                    if(rowNorm != 0 && colNorm != 0) {
                        double g = rowNorm / radix;
                        double f = 0;
                        double sumNorm = rowNorm + colNorm;

                        // Nalezneme mocninu radixu (z�kladn� jednotky strojov�ho ��sla),
                        // kter� je nejbl�e k balancovan� matici
                        while(colNorm < g) {
                            f *= radix;
                            colNorm *= sqradix;
                        }
                        g = rowNorm * radix;
                        while(colNorm > g) {
                            f /= radix;
                            colNorm /= sqradix;
                        }

                        if((colNorm + rowNorm) / f < boundary * sumNorm) {
                            finish = false;
                            g = 1.0 / f;

                            // Podobnostn� tranformace
                            result.MultiplyColumn(i, f);
                            result.MultiplyRow(i, g);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// P�evede matici podobnostn�mi transformacemi na Hessenberg�v tvar
        /// (matice, kter� m� pod diagon�lou nenulov� prvek a v�e n� je nulov�)
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

                // Prohod�me sloupce a ��dky
                if(jMax != i) {
                    result.SwapRows(jMax, i);
                    result.SwapColumns(jMax, i);
                }

                // Vlastn� eliminace
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

                // Nulov�n� nepot�ebn�ch prvk�
                for(int j = i + 1; j < result.Length; j++) {
                    result[j, i - 1] = 0;
                }
            }

            return result;
        }

        /// <summary>
        /// Vypo��t� vlastn� hodnoty matice v Hessenbergov� tvaru
        /// </summary>
        public ComplexVector EigenValues() {
            Matrix work = this.Balance().Hessenberg();
            ComplexVector result = new ComplexVector(work.Length);

            double wNorm = work.AbsNorm();
            double t = 0;
            int wPos = work.Length - 1;

            while(wPos >= 0) {
                // Po�et itegrac�
                int iterations = 0;
                const int maxIterations = 3000;
                const int shiftIterations = 10;

                // Po�et nediagon�ln�ch element�
                int smallIndex = 0;

                do {
                    for(smallIndex = wPos; smallIndex >= 1; smallIndex--) {
                        double s = System.Math.Abs(work[smallIndex - 1, smallIndex - 1]) + System.Math.Abs(work[smallIndex, smallIndex]);
                        if(s == 0)
                            s = wNorm;

                        // Na�li jsme mal� nediagon�ln� element?
                        if(System.Math.Abs(work[smallIndex, smallIndex - 1]) + s == s)
                            break;
                    }

                    // Elementy submatice 2 x 2 na diagon�le
                    double x = work[wPos, wPos];

                    // Na�li jsme jeden ko�en
                    if(smallIndex == wPos) {
                        result[wPos].Re = x + t;
                        result[wPos].Im = 0;
                        wPos--;
                    }
                    // Na�li jsme dva ko�eny
                    else {
                        double y = work[wPos - 1, wPos - 1];
                        double z = work[wPos, wPos - 1] * work[wPos - 1, wPos];

                        if(smallIndex == wPos - 1) {
                            double p = 0.5 * (y - x);
                            double q = p * p + z;
                            double w = System.Math.Sqrt(System.Math.Abs(q));

                            x += t;

                            // Re�ln� dvojice
                            if(q >= 0.0) {
                                w = p + w * (p >= 0.0 ? 1.0 : -1.0);
                                result[wPos - 1].Re = x + w;
                                if(w != 0)
                                    result[wPos].Re = x - z / w;
                                else
                                    result[wPos].Re = x + w;
                                result[wPos].Im = result[wPos - 1].Im = 0;
                            }
                            // Komplexn� dvojice
                            else {
                                result[wPos].Re = result[wPos - 1].Re = x + p;
                                result[wPos].Im = w;
                                result[wPos - 1].Im = -w;
                            }
                            wPos -= 2;
                        }
                        // ��dn� ko�en, pokra�ujeme v iteraci
                        else {
                            if(iterations > maxIterations)
                                throw new Exception("P�ekro�en maxim�ln� po�et iterac� p�i v�po�tu vlastn�ch hodnot matice!");
                            // Mimo��dn� posunut�
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

                            // Household�v vektor
                            Vector hhVector = new Vector(3);

                            // Posunut� a nalezen� dvou po sob� jdouc�ch mal�ch nediagon�ln�ch element�
                            for(index = wPos - 2; index >= smallIndex; index--) {
                                double w = work[index, index];
                                double r = x - w;
                                double s = y - w;

                                hhVector[0] = (r * s - z) / work[index + 1, index] + work[index, index + 1];
                                hhVector[1] = work[index + 1, index + 1] - w - r - s;
                                hhVector[2] = work[index + 2, index + 1];

                                // �k�lov�n� (sna��me se p�edej�t p�ete�en�)
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

                                    // Modifikace ��dk�
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

                                    // Modifikace sloupc�
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
        /// Vr�t� pouze re�ln� vlastn� hodnoty
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

        private const string errorMessageNoData = "K proveden� operace je nutn�, aby velikost matice nebyla nulov�.";
        private const string errorMessageNotSquare = "Pro ur�en� jedine�n� velikosti matice mus� b�t matice �tvercov�.";
        private const string errorMessageDifferentDimension = "K proveden� operace mus� m�t matice stejn� rozm�ry.";
        private const string errorMessageMultiplication = "P�i n�soben� matic mus� m�t matice kompatibiln� rozm�ry.";
        private const string errorMessageSingular = "Matrix je singul�rn�. Inverzi nelze spo��tat.";
        private const string errorMessageDifferentLength = "K proveden� operace mus� m�t vektor stejnou d�lku jako odpov�daj�c� rozm�r matice.";
    }
}
*/