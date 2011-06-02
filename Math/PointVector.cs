using System;
using System.IO;
using System.Text;
using System.Collections;

using PavelStransky.Core;

namespace PavelStransky.Math {
	/// <summary>
	/// �ada bod� (x, y)
	/// </summary>
    public class PointVector: ICloneable, IExportable, ISortable, IMinMax {
        // slo�ky
        private PointD[] item;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="x">Vektor x - ov�ch hodnot</param>
        /// <param name="y">Vektor y - ov�ch hodnot</param>
        public PointVector(Vector x, Vector y) {
            int length = System.Math.Max(x.Length, y.Length);
            this.item = new PointD[length];

            for(int i = 0; i < this.item.Length; i++)
                this.item[i] = new PointD(x.Length > i ? x[i] : 0.0, y.Length > i ? y[i] : 0.0);
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="y">Vektor y - ov�ch hodnot</param>
        /// <param name="dx">Diference v hodnot�ch x (implicitn� se za��n� od 0)</param>
        public PointVector(double dx, Vector y) {
            this.item = new PointD[y.Length];

            for(int i = 0; i < this.item.Length; i++)
                this.item[i] = new PointD(i * dx, y[i]);
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="x">Vektor x - ov�ch hodnot</param>
        /// <param name="dy">Diference v hodnot�ch y (implicitn� se za��n� od 0)</param>
        public PointVector(Vector x, double dy) {
            this.item = new PointD[x.Length];

            for(int i = 0; i < this.item.Length; i++)
                this.item[i] = new PointD(x[i], i * dy);
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="y">Vektor y - ov�ch hodnot</param>
        public PointVector(Vector y) : this(1.0, y) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="item">Data jako �ada bod�</param>
        public PointVector(PointD[] item) {
            this.item = item;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="length">D�lka vektoru</param>
        public PointVector(int length) {
            this.item = new PointD[length];
            for(int i = 0; i < this.Length; i++)
                this.item[i] = new PointD();
        }

        /// <summary>
        /// Indexer
        /// </summary>
        public PointD this[int i] { get { return this.item[i]; } set { this.item[i] = value; } }

        /// <summary>
        /// Prvn� prvek vektoru
        /// </summary>
        public PointD FirstItem { get { return this.item[0]; } set { this.item[0] = value; } }

        /// <summary>
        /// Posledn� prvek vektoru
        /// </summary>
        public PointD LastItem { get { return this.item[this.Length - 1]; } set { this.item[this.Length - 1] = value; } }

        /// <summary>
        /// D�lka vektoru bod�
        /// </summary>
        public int Length {
            get {
                return this.item.Length;
            }

            set {
                PointD[] newItem = new PointD[value];

                int minLength = System.Math.Min(this.Length, value);
                for(int i = 0; i < minLength; i++)
                    newItem[i] = this.item[i];

                for(int i = minLength; i < value; i++)
                    newItem[i] = new PointD();

                this.item = newItem;
            }
        }

        /// <summary>
        /// Minim�ln� x-ov� hodnota
        /// </summary>
        public double MinX() {
            if(this.Length == 0)
                throw new PointVectorException(errorMessageNoData);

            double result = this[0].X;

            for(int i = 1; i < this.Length; i++)
                if(result > this[i].X)
                    result = this[i].X;

            return result;
        }

        /// <summary>
        /// Maxim�ln� x-ov� hodnota
        /// </summary>
        public double MaxX() {
            if(this.Length == 0)
                throw new PointVectorException(errorMessageNoData);

            double result = this[0].X;

            for(int i = 1; i < this.Length; i++)
                if(result < this[i].X)
                    result = this[i].X;

            return result;
        }

        /// <summary>
        /// Minim�ln� y-ov� hodnota
        /// </summary>
        public double MinY() {
            if(this.Length == 0)
                throw new PointVectorException(errorMessageNoData);

            double result = this[0].Y;

            for(int i = 1; i < this.Length; i++)
                if(result > this[i].Y)
                    result = this[i].Y;

            return result;
        }

        /// <summary>
        /// Maxim�ln� y-ov� hodnota
        /// </summary>
        public double MaxY() {
            if(this.Length == 0)
                throw new PointVectorException(errorMessageNoData);

            double result = this[0].Y;

            for(int i = 1; i < this.Length; i++)
                if(result < this[i].Y)
                    result = this[i].Y;

            return result;
        }

        /// <summary>
        /// N�soben� vektoru bodem (x - ov� slo�ka se n�sob� hodnotou x, y - ov� hodnotou y)
        /// </summary>
        /// <param name="pv">Vektor</param>
        /// <param name="point">Bod</param>
        public static PointVector operator *(PointVector pv, PointD point) {
            PointVector result = new PointVector(pv.Length);

            for(int i = 0; i < result.Length; i++) {
                result[i].X = pv[i].X * point.X;
                result[i].Y = pv[i].Y * point.Y;
            }

            return result;
        }

        /// <summary>
        /// D�len� vektoru bodem (x - ov� slo�ka se n�sob� hodnotou x, y - ov� hodnotou y)
        /// </summary>
        /// <param name="pv">Vektor</param>
        /// <param name="point">Bod</param>
        public static PointVector operator /(PointVector pv, PointD point) {
            PointVector result = new PointVector(pv.Length);

            for(int i = 0; i < result.Length; i++) {
                result[i].X = pv[i].X / point.X;
                result[i].Y = pv[i].Y / point.Y;
            }

            return result;
        }

        #region Implementace IExportable
        /// <summary>
        /// Ulo�� obsah vektoru do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            if(export.Binary) {
                // Bin�rn�
                BinaryWriter b = export.B;
                b.Write(this.Length);
                for(int i = 0; i < this.Length; i++) {
                    b.Write(this[i].X);
                    b.Write(this[i].Y);
                }
            }
            else {
                // Textov�
                StreamWriter t = export.T;
                t.WriteLine(this.Length);
                for(int i = 0; i < this.Length; i++)
                    t.WriteLine("{0}\t{1}", this[i].X, this[i].Y);
            }
        }

        /// <summary>
        /// Na�te obsah vektoru ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public PointVector(Core.Import import) {
            if(import.Binary) {
                // Bin�rn�
                BinaryReader b = import.B;
                this.item = new PointD[b.ReadInt32()];
                for(int i = 0; i < this.Length; i++)
                    this.item[i] = new PointD(b.ReadDouble(), b.ReadDouble());
            }
            else {
                // Textov�
                StreamReader t = import.T;
                this.item = new PointD[int.Parse(t.ReadLine())];
                for(int i = 0; i < this.Length; i++) {
                    string line = t.ReadLine();
                    string[] s = line.Split('\t');
                    this.item[i] = new PointD(double.Parse(s[0]), double.Parse(s[1]));
                }
            }
        }
        #endregion

        /// <summary>
        /// Provede vyhlazen�
        /// </summary>
        /// <param name="interval">Interval k vyhlazen�</param>
        public void Smooth(int interval) {
            PointD[] result = new PointD[this.Length];
            result[0] = new PointD(this[0].X, this[0].Y);

            // Prvn� hodnota
            for(int i = 1; i < this.Length; i++) {
                int i1 = i - (interval / 2);
                int i2 = i1 + interval;

                if(i1 < 0) i1 = 0;
                if(i2 >= this.Length) i2 = this.Length - 1;

                result[i] = new PointD(this[i].X, result[i - 1].Y + (this[i2].Y - this[i1].Y) / interval);
            }

            this.item = result;
        }

        /// <summary>
        /// Provede integraci pod k�ivkou (lichob�n�kov� pravidlo)
        /// </summary>
        public double Integrate() {
            double result = 0;
            PointVector sorted = this.SortX();

            for(int i = 1; i < this.Length; i++)
                result += (sorted[i].X - sorted[i - 1].X) * (sorted[i].Y + sorted[i - 1].Y) / 2.0;

            return result;
        }

        /// <summary>
        /// Vytvo�� kopii vektoru
        /// </summary>
        public object Clone() {
            int length = this.Length;
            PointVector result = new PointVector(length);
            for(int i = 0; i < length; i++) {
                result[i].X = this[i].X;
                result[i].Y = this[i].Y;
            }
            return result;
        }

        /// <summary>
        /// Prohod� sou�adnice X, Y
        /// </summary>
        public PointVector SwapXY() {
            int length = this.Length;
            PointVector result = new PointVector(length);

            for(int i = 0; i < length; i++)
                result[i] = new PointD(this[i].Y, this[i].X);

            return result;
        }

        /// <summary>
        /// Vektor x-ov�ch hodnot
        /// </summary>
        public Vector VectorX {
            get {
                Vector result = new Vector(this.Length);

                for(int i = 0; i < result.Length; i++)
                    result[i] = this[i].X;

                return result;
            }
            set {
                if(this.Length != value.Length)
                    throw new PointVectorException(errorMessageDifferentLength);

                for(int i = 0; i < this.Length; i++)
                    this[i].X = value[i];
            }
        }

        /// <summary>
        /// Vektor y-ov�ch hodnot
        /// </summary>
        public Vector VectorY {
            get {
                Vector result = new Vector(this.Length);

                for(int i = 0; i < result.Length; i++)
                    result[i] = this[i].Y;

                return result;
            }
            set {
                if(this.Length != value.Length)
                    throw new PointVectorException(errorMessageDifferentLength);

                for(int i = 0; i < this.Length; i++)
                    this[i].Y = value[i];
            }
        }

        #region T��d�n�
        /// <summary>
        /// Keys for sorting
        /// </summary>
        public Array GetKeys() {
            return this.VectorY.GetKeys();
        }

        /// <summary>
        /// T��d�n� podle X
        /// </summary>
        public PointVector SortX() {
            return this.Sort(this.VectorX) as PointVector;
        }

        /// <summary>
        /// T��d�n� podle Y
        /// </summary>
        public PointVector SortY() {
            return this.Sort(this.VectorY) as PointVector;
        }

        /// <summary>
        /// T��d�n� vzestupn�
        /// </summary>
        /// <returns></returns>
        public object Sort() {
            return this.SortX();
        }

        /// <summary>
        /// T��d�n� sestupn�
        /// </summary>
        /// <returns></returns>
        public object SortDesc() {
            PointVector result = this.Sort() as PointVector;
            System.Array.Reverse(result.item);
            return result;
        }

        /// <summary>
        /// T��d�n� vzestupn� s kl��i
        /// </summary>
        /// <param name="keys">Kl��e</param>
        /// <returns></returns>
        public object Sort(ISortable keys) {
            PointVector result = this.Clone() as PointVector;
            Array.Sort(keys.GetKeys(), result.item);
            return result;
        }

        /// Pou�ije kl��e k set��d�n� vektoru sestupn�
        /// </summary>
        /// <param name="keys">Kl��e k set��d�n�</param>
        public object SortDesc(ISortable keys) {
            PointVector result = this.Sort(keys) as PointVector;
            System.Array.Reverse(result.item);
            return result;
        }
        #endregion

        /// <summary>
        /// P�ehod� x-ovou a y-ovou slo�ku vektoru
        /// </summary>
        public PointVector Transposition() {
            PointVector result = new PointVector(this.Length);

            for(int i = 0; i < this.Length; i++)
                result[i] = new PointD(this[i].Y, this[i].X);

            return result;
        }

        /// <summary>
        /// Normalizuje dan� vektor vzhledem k hodnot�m Y
        /// </summary>
        public PointVector Normalization() {
            PointVector result = new PointVector(this.Length);

            double sum = 0;
            for(int i = 0; i < this.Length; i++)
                sum += this[i].Y;

            for(int i = 0; i < this.Length; i++)
                result[i] = new PointD(this[i].X, this[i].Y / sum);

            return result;
        }

        /// <summary>
        /// Perform the tranformation of all components of the vector
        /// </summary>
        /// <param name="fx">Transformation function x</param>
        /// <param name="fy">Transformation function y</param>
        public PointVector Transform(RealFunction fx, RealFunction fy) {
            int length = this.Length;
            PointVector result = new PointVector(length);

            for(int i = 0; i < length; i++) {
                if(fx != null)
                    result[i].X = fx(this[i].X);
                else
                    result[i].X = this[i].X;

                if(fy != null)
                    result[i].Y = fy(this[i].Y);
                else
                    result[i].Y = this[i].Y;
            }

            return result;
        }

        /// <summary>
        /// Perform the tranformation of all components of the vector
        /// </summary>
        /// <param name="f">Transformation function y</param>
        public PointVector Transform(RealFunction f) {
            int length = this.Length;
            PointVector result = new PointVector(length);

            for(int i = 0; i < length; i++) {
                double x = this[i].X;
                result[i].X = this[i].X;
                result[i].Y = f(this[i].Y);
            }

            return result;
        }

        /// <summary>
        /// Perform the tranformation of all components of the vector
        /// </summary>
        /// <param name="f">Transformation function y</param>
        /// <param name="p">Additional parameters for the transformation function</param>
        public PointVector Transform(RealFunctionWithParams f, params object[] p) {
            int length = this.Length;
            PointVector result = new PointVector(length);

            for(int i = 0; i < length; i++) {
                double x = this[i].X;
                result[i].X = this[i].X;
                result[i].Y = f(this[i].Y, p);
            }

            return result;
        }

        /// <summary>
        /// Spoj� vektory do jednoho
        /// </summary>
        /// <param name="vArray">�ada vektor�</param>
        public static PointVector Join(PointVector[] vArray) {
            // Po�et prvk� v�sledn�ho vektoru
            int numItems = 0;
            for(int i = 0; i < vArray.Length; i++)
                numItems += vArray[i].Length;

            PointVector result = new PointVector(numItems);

            int item = 0;
            for(int i = 0; i < vArray.Length; i++)
                for(int j = 0; j < vArray[i].Length; j++)
                    result[item++] = vArray[i][j];

            return result;
        }

        /// <summary>
        /// Najde v�echny body, ve kter�ch se dan� k�ivky prot�naj�
        /// </summary>
        /// <param name="v">Druh� k�ivka</param>
        public PointVector Intersection(PointVector v) {
            int length1 = this.Length;
            int length2 = v.Length;

            ArrayList r = new ArrayList();

            for(int i = 1; i < length1; i++) {
                double x1min = this[i - 1].X;
                double y1min = this[i - 1].Y;
                double x1max = this[i].X;
                double y1max = this[i].Y;

                for(int j = 1; j < length2; j++) {
                    double x2min = v[j - 1].X;
                    double y2min = v[j - 1].Y;
                    double x2max = v[j].X;
                    double y2max = v[j].Y;

                    double d = (x1max - x1min) * (y2max - y2min) - (y1max - y1min) * (x2max - x2min);
                    double x = ((y1min * x1max - y1max * x1min) * (x2max - x2min) + (y2max * x2min - y2min * x2max) * (x1max - x1min));
                    x /= d;
                    double y = ((y1min * x1max - y1max * x1min) * (y2max - y2min) + (y2max * x2min - y2min * x2max) * (y1max - y1min));
                    y /= d;

                    /*                  ((bc-ad)(h-g)+(d-c)(fg-eh))/((d-c)(f-e)-(b-a)(h-g))
                                        ((ad-bc)(f-e)+(b-a)(fg-eh))/((d-c)(f-e)-(b-a)(h-g))

                                        x -> (a d (g - h) + b c (-g + h) + (c - d) (f g - e h))/(
                                         d (e - f) + c (-e + f) + (a - b) (g - h)), y -> (
                                         a (d (e - f) + f g - e h) + b (c (-e + f) - f g + e h))/(
                                         d (e - f) + c (-e + f) + (a - b) (g - h)) */

                    // Existuje pr�nik
                    if(((x >= x1min && x <= x1max) || (x >= x1max && x <= x1min) || (x1min == x1max)) && ((y >= y1min && y <= y1max) || (y >= y1max && y <= y1min) || (y1min == y1max))
                        && ((x >= x2min && x <= x2max) || (x >= x2max && x <= x2min) || (x2min == x2max)) && ((y >= y2min && y <= y2max) || (y >= y2max && y <= y2min) || (y2min == y2max)))
                        r.Add(new PointD(x, y));
                }
            }

            int count = r.Count;
            PointVector result = new PointVector(count);
            for(int i = 0; i < count; i++)
                result[i] = (PointD)r[i];

            return result;
        }

        /// <summary>
        /// Vrac� true, pokud je dan� bod uvnit� k�ivky zadan� vektorem bod�
        /// </summary>
        /// <param name="point">Bod, kter� zkoum�me</param>
        /// <remarks>Algoritmus - Point in polygon</remarks>
        public bool IsInside(PointD point) {
            int length = this.Length;
            bool result = false;

            double x = point.X;
            double y = point.Y;

            int j = length - 1;

            for(int i = 0; i < length; i++) {
                if((this[i].Y < y && this[j].Y >= y) || (this[j].Y < y && this[i].Y >= y)) {
                    if(this[i].X + (y - this[i].Y) / (this[j].Y - this[i].Y) * (this[j].X - this[i].X) < x)
                        result = !result;
                }
                j = i;
            }

            return result;
        }

        /// <summary>
        /// Pointvector p�etvo�� do tvaru schodu
        /// </summary>
        public PointVector StairsX() {
            int length = this.Length;
            if(length <= 1)
                return (PointVector)this.Clone();

            double x = this[0].X - (this[1].X - this[0].X) / 2;

            PointVector result = new PointVector(2 * length);
            for(int i = 0; i < length; i++) {
                result[2 * i] = new PointD(x, this[i].Y);
                if(i < length - 1)
                    x = this[i].X + (this[i + 1].X - this[i].X) / 2;
                else
                    x = this[i].X + (this[i].X - this[i - 1].X) / 2;
                result[2 * i + 1] = new PointD(x, this[i].Y);
            }
            return result;
        }

        /// <summary>
        /// Vektor p�evede na textov� �et�zec
        /// </summary>
        public override string ToString() {
            StringBuilder result = new StringBuilder();

            for(int i = 0; i < this.Length; i++) {
                result.Append(this[i].ToString());
                result.Append("\r\n");
            }

            return result.ToString();
        }

        /// <summary>
        /// Vr�t� zpr�m�rovan� vektor p�es biny
        /// </summary>
        /// <param name="intervals">Po�et interval�</param>
        /// <param name="min">Po��te�n� hodnota, od kter� se histogram po��t�</param>
        /// <param name="max">Maxim�ln� hodnota, do kter� se histogram po��t�</param>
        public PointVector BinMean(int intervals, double min, double max) {
            ArrayList bins = new ArrayList();
            PointVector sorted = this.SortX() as PointVector;

            double step = (max - min) / intervals;

            int j = 0;
            for(int i = 0; i < intervals; i++) {
                double x = 0;
                double y = 0;
                int k = 0;
                while((sorted[j].X <= min + step * (i + 1)) && (j < sorted.Length - 1)) {
                    x += sorted[j].X;
                    y += sorted[j].Y;
                    j++;
                    k++;
                }

                if(k != 0)
                    bins.Add(new PointD(x / k, y / k));
            }

            // P�eveden� na PointVector
            PointVector result = new PointVector(bins.Count);
            j = 0;
            foreach(PointD bin in bins)
                result[j++] = bin;

            return result;
        }

        /// <summary>
        /// Vr�t� zpr�m�rovan� vektor p�es biny
        /// </summary>
        /// <param name="interval">Points of the interval</param>
        public PointVector BinMean(Vector interval) {
            int length = this.Length;

            ArrayList bins = new ArrayList();

            for(int i = 0; i < length - 1; i++) {
                double minx = interval[i];
                double maxx = interval[i + 1];

                double x = 0;
                double y = 0;
                int k = 0;

                for(int j = 0; j < length; j++) {
                    double x1 = this[j].X;

                    if(x1 >= minx && x1 < maxx){
                        x += x1;
                        y += this[j].Y;
                        k++;
                    }
                }

                if(k != 0)
                    bins.Add(new PointD(x / k, y / k));
            }

            // P�eveden� na PointVector
            PointVector result = new PointVector(bins.Count);
            int l = 0;
            foreach(PointD bin in bins)
                result[l++] = bin;

            return result;
        }

        /// <summary>
        /// Spoj� body do kruhu tak, aby vzd�lenosti mezi sousedn�mi body byly co nejkrat�� (nemus� nutn� zahrnout v�echny body)
        /// </summary>
        /// <param name="minCircle">Minim�ln� d�lka kruhu</param>
        public PointVector JoinCircle(int minCircle) {
            int length = this.Length;

            // Pou�it� body
            bool[] used = new bool[length];
            for(int i = 0; i < length; i++)
                used[i] = false;

            // Connecting
            PointD x0 = this[0];
            PointD x1 = x0;
            used[0] = true;

            ArrayList circle = new ArrayList();
            circle.Add(x0);

            int usedPoints = 1;
            bool fullCircle = false;

            while(!fullCircle) {
                double distance = -1.0;
                int k = -1;

                for(int i = 0; i < length; i++) {
                    if(used[i])
                        continue;

                    double d = PointD.Distance(x1, this[i]);
                    if(distance <= 0.0 || distance > d) {
                        distance = d;
                        k = i;
                    }
                }

                x1 = this[k];
                circle.Add(x1);
                used[k] = true;
                usedPoints++;

                if(usedPoints > minCircle)
                    used[0] = false;

                if(k == 0)
                    fullCircle = true;
            }

            PointVector result = new PointVector(circle.Count);
            int j = 0;
            foreach(PointD p in circle) 
                result[j++] = p;

            return result;
        }

        /// <summary>
        /// Vr�t� v�echny body, kter� jsou lok�ln�mi maximy
        /// </summary>
        public PointVector Maxima() {
            return this.Maxima(false);
        }

        /// <summary>
        /// Vr�t� v�echny body, kter� jsou lok�ln�mi maximy
        /// </summary>
        /// <param name="flat">True, pokud se za lok�ln� maxima budou po��tat i body typu 0 1M 1 2M 2 ...</param>
        public PointVector Maxima(bool flat) {
            int length = this.Length;
            ArrayList maxima = new ArrayList();

//            PointVector sorted = this.Sort() as PointVector;  // Ze z�hadn�ch d�vod� nefunguje pro skokovou funkci
            PointVector sorted = this;

            int lastIndex = 0;
            bool max = true;
            bool flatMax = false;

            for(int i = 1; i < length; i++) {
                if(sorted[i].Y > sorted[lastIndex].Y) {
                    if(flatMax && flat)
                        maxima.Add(sorted[lastIndex]);

                    lastIndex = i;
                    max = true;
                    flatMax = false;
                }
                else if(sorted[i].Y < sorted[lastIndex].Y) {
                    if(max)
                        maxima.Add(new PointD(0.5 * (sorted[lastIndex].X + sorted[i - 1].X), sorted[lastIndex].Y));
                    else if(flatMax && flat)
                        maxima.Add(sorted[i - 1]);

                    flatMax = false;
                    max = false;
                    lastIndex = i;
                }
                else if(sorted[i].Y == sorted[lastIndex].Y)
                    flatMax = true;
            }

            // Last element
            if(max)
                maxima.Add(new PointD(0.5 * (sorted[lastIndex].X + sorted.LastItem.X), sorted[lastIndex].Y));

            PointVector result = new PointVector(maxima.Count);
            int j = 0;
            foreach(PointD p in maxima)
                result[j++] = p;
            return result;
        }

        /// <summary>
        /// Minima (implementace IMinMax)
        /// </summary>
        /// <param name="precision">Irelevantn� paramete</param>
        public PointVector Minima(double precision) {
            return this.Minima();
        }

        /// <summary>
        /// Maxima (implementace IMinMax)
        /// </summary>
        /// <param name="precision">Irelevantn� parametr</param>
        public PointVector Maxima(double precision) {
            return this.Maxima();
        }

        /// <summary>
        /// Vr�t� v�echny body, kter� jsou lok�ln�mi minimy
        /// </summary>
        public PointVector Minima() {
            return this.Minima(false);
        }

        /// <summary>
        /// Vr�t� v�echny body, kter� jsou lok�ln�mi minimy
        /// </summary>
        /// <param name="flat">True, pokud se za lok�ln� minima budou po��tat i body typu 0 1 1m 2 2m 3 ...</param>
        public PointVector Minima(bool flat) {
            int length = this.Length;
            ArrayList minima = new ArrayList();

//            PointVector sorted = this.Sort() as PointVector;  // Ze z�hadn�ch d�vod� nefunguje pro skokovou funkci
            PointVector sorted = this;

            int lastIndex = 0;
            bool min = true;
            bool flatMin = false;

            for(int i = 1; i < length; i++) {
                if(sorted[i].Y < sorted[lastIndex].Y) {
                    lastIndex = i;
                    min = true;
                }
                else if(sorted[i].Y > sorted[lastIndex].Y) {
                    if(min)
                        minima.Add(new PointD(0.5 * (sorted[lastIndex].X + sorted[i - 1].X), sorted[lastIndex].Y));
                    else if(flatMin && flat)
                        minima.Add(sorted[i-1]);

                    flatMin = false;
                    min = false;
                    lastIndex = i;
                }
                else if(sorted[i].Y == sorted[lastIndex].Y) 
                    flatMin = true;                
            }

            // Last element
            if(min)
                minima.Add(new PointD(0.5 * (sorted[lastIndex].X + sorted.LastItem.X), sorted[lastIndex].Y));

            PointVector result = new PointVector(minima.Count);
            int j = 0;
            foreach(PointD p in minima)
                result[j++] = p;
            return result;
        }

        /// <summary>
        /// Line�rn� navzorkuje dan� vektor v dan�ch bodech (pou�ije line�rn� interpolaci a extrapolaci)
        /// </summary>
        /// <param name="points">Koncov� body</param>
        public PointVector Sample(Vector points) {
            PointVector pv = this.Clone() as PointVector;

            int length = points.Length;
            PointVector result = new PointVector(length);
            for(int i = 0; i < length; i++) {
                double x = points[i];

                if(pv.FirstItem.X > x)
                    result[i] = PointD.Interpolate(pv[0], pv[1], x);
                else if(pv.LastItem.X < x)
                    result[i] = PointD.Interpolate(pv.LastItem, pv[pv.Length - 2], x);
                else
                for(int j = 0; j < pv.Length; j++) {
                    if(pv[j].X == x) {
                        result[i] = new PointD(x, pv[j].Y);
                        break;
                    }
                    if(pv[j].X > x) {
                        result[i] = PointD.Interpolate(pv[j - 1], pv[j], x);
                        break;
                    }
                }
            }

            return result;
        }

        private const string errorMessageNoData = "K proveden� operace je nutn�, aby d�lka vektoru nebyla nulov�.";
        private const string errorMessageDifferentLength = "K proveden� operace mus� m�t vektory stejnou d�lku.";
    }

	/// <summary>
	/// V�jimka ve t��d� Vector
	/// </summary>
	public class PointVectorException: ApplicationException {
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		public PointVectorException(string message) : base(errMessage + message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		public PointVectorException(string message, Exception innerException) : base(errMessage + message, innerException) {}

		private const string errMessage = "Ve t��d� PointVector do�lo k chyb�: ";
	}
}
