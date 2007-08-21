using System;
using System.Drawing;

using PavelStransky.Math;

namespace PavelStransky.Expression {
    /// <summary>
    /// T��da, kter� zapouzd�uje graf (ur�uje jeho obsah, pop�. dal�� parametry)
    /// </summary>
    public class Graph : IExportable {
        private TArray item, background;
        private TArray errors;

        /// <summary>
        /// Styly spojnic bod�
        /// </summary>
        public enum LineStyles { Line, Curve, None }

        /// <summary>
        /// Styly bod�
        /// </summary>
        public enum PointStyles { Circle, Square, None, HLines, VLines, FCircle, FSquare }

        // Kontext a v�raz cel�ho grafu
        private Context graphContext;
        // Kontexty jednotliv�ch k�ivek
        private TArray itemContext;
        // Kontexty jednotliv�ch pozad�
        private TArray groupContext;

        /// <summary>
        /// Prvek grafu
        /// </summary>
        public TArray Item { get { return this.item; } }

        /// <summary>
        /// Pozad� - DensityGraph
        /// </summary>
        public TArray Background { get { return this.background; } }

        /// <summary>
        /// Chyby grafu
        /// </summary>
        public TArray Errors { get { return this.errors; } }

        /// <summary>
        /// Po�et k�ivek grafu ve skupin�
        /// </summary>
        /// <param name="group">��slo skupiny k�ivky</param>
        public int NumCurves(int group) {
            if(group < this.NumGroups())
                return (this.item[group] as TArray).GetLength(0);
            else
                return 0;
        }

        /// <summary>
        /// Po�et skupin k�ivek
        /// </summary>
        /// <returns></returns>
        public int NumGroups() {
            return this.item.GetLength(0);
        }

        /// <summary>
        /// Vr�t� k�ivku
        /// </summary>
        /// <param name="group">Index skupiny k�ivky</param>
        /// <param name="i">Index k�ivky</param>
        public PointVector GetCurve(int group, int i) {
            if(group < this.NumGroups() && i < this.NumCurves(group))
                return (this.item[group] as TArray)[i] as PointVector;
            else
                return new PointVector(0);
        }

        /// <summary>
        /// Vr�t� vektor s chybami
        /// </summary>
        /// <param name="group">Index skupiny k�ivky</param>
        /// <param name="i">Index k�ivky</param>
        public Vector GetError(int group, int i) {
            if(group < this.errors.Length && i < (this.errors[group] as TArray).Length)
                return (this.errors[group] as TArray)[i] as Vector;
            else
                return new Vector(0);
        }

        /// <summary>
        /// True, pokud jsou pro zadanou skupinu a k�ivku zad�ny chyby
        /// </summary>
        /// <param name="group">Index skupiny k�ivky</param>
        /// <param name="i">Index k�ivky</param>
        public bool IsErrors(int group, int i) {
            return this.GetError(group, i).Length > 0;
        }

        /// <summary>
        /// Vr�t� matici pozad�
        /// </summary>
        /// <param name="group">Index skupiny</param>
        public Matrix GetMatrix(int group) {
            if(group < this.background.Length)
                return this.background[group] as Matrix;
            else
                return new Matrix(0);
        }

        /// <summary>
        /// Vr�t� d�lku i - t� k�ivky
        /// </summary>
        /// <param name="group">��slo skupiny k�ivky</param>
        /// <param name="i">Index k�ivky</param>
        public int GetLength(int group, int i) {
            return this.GetCurve(group, i).Length;
        }

        /// <summary>
        /// Vrac� kontext zadan� k�ivky
        /// </summary>
        /// <param name="group">Index skupiny k�ivky</param>
        /// <param name="i">Index k�ivky</param>
        private Context GetCurveContext(int group, int i) {
            if(group < this.NumGroups() && i < this.NumCurves(group))
                return (this.itemContext[group] as TArray)[i] as Context;
            else
                return new Context();
        }

        /// <summary>
        /// Vrac� hodnotu parametru a kontroluje, jestli m� zadan� parametr spr�vn� typ
        /// </summary>
        /// <param name="name">N�zev parametru</param>
        /// <param name="context">Kontext, na kter�m se hled�</param>
        /// <param name="def">Default hodnota</param>
        /// <returns>Hodnota parametru, default, pokud parametr nen� zad�n</returns>
        private object GetParameter(Context context, string name, object def) {
            if(context.Contains(name)) {
                object item = context[name].Item;
                Type type = def.GetType();

                // Barva
                if(type == typeof(Color)) {
                    if(item is string)
                        return Color.FromName((string)item);
                    else if(item is Vector && (item as Vector).Length == 3) {
                        Vector v = item as Vector;
                        return Color.FromArgb((int)(255.0 * v[0]), (int)(255.0 * v[1]), (int)(255.0 * v[2]));
                    }
                    else if(item is int)
                        return ColorArray.GetColor((int)item);
                }

                else if(type == typeof(float)) {
                    if(item is double)
                        item = (float)(double)item;
                    else if(item is int)
                        item = (float)(int)item;
                }

                else if(type == typeof(double)) {
                    if(item is int)
                        item = (double)(int)item;
                }

                else if(type == typeof(LineStyles)) {
                    if(item is string)
                        item = (LineStyles)Enum.Parse(typeof(LineStyles), (string)item, true);
                }

                else if(type == typeof(PointStyles)) {
                    if(item is string)
                        item = (PointStyles)Enum.Parse(typeof(PointStyles), (string)item, true);
                }

                if(item.GetType() != type)
                    throw new ExpressionException(string.Format(errorMessageBadType, name),
                        string.Format(errorMessageBadTypeDetail, type.FullName, item.GetType().FullName, item.ToString()));

                return item;
            }
            else
                return def;
        }

        /// <summary>
        /// Vrac� hodnotu z obecn�ch parametr�
        /// </summary>
        /// <param name="name">N�zev parametru</param>
        /// <param name="context">Kontext, na kter�m se hled�</param>
        /// <param name="def">Default hodnota</param>
        /// <returns>Hodnota parametru, default, pokud parametr nen� zad�n</returns>
        public object GetGeneralParameter(string name, object def) {
            return this.GetParameter(this.graphContext, name, def);
        }

        /// <summary>
        /// Vrac� hodnotu parametru k�ivky i
        /// </summary>
        /// <param name="group">Index skupiny k�ivky</param>
        /// <param name="i">Index k�ivky</param>
        /// <param name="name">N�zev parametru</param>
        /// <param name="context">Kontext, na kter�m se hled�</param>
        /// <param name="def">Default hodnota</param>
        /// <returns>Hodnota parametru, default, pokud parametr nen� zad�n</returns>
        public object GetCurveParameter(int group, int i, string name, object def) {
            // Nejd��ve kouk�me, jestli parametr nen� v obecn�ch 
            def = this.GetParameter(this.graphContext, name, def);

            if(this.itemContext.Length > group && (this.itemContext[group] as TArray).Length > i)
                return this.GetParameter(this.GetCurveContext(group, i), name, def);
            else
                return def;
        }

        /// <summary>
        /// Vrac� hodnotu parametru pozad� i
        /// </summary>
        /// <param name="i">Index pozad�</param>
        /// <param name="name">N�zev parametru</param>
        /// <param name="context">Kontext, na kter�m se hled�</param>
        /// <param name="def">Default hodnota</param>
        /// <returns>Hodnota parametru, default, pokud parametr nen� zad�n</returns>
        public object GetBackgroundParameter(int i, string name, object def) {
            // Nejd��ve kouk�me, jestli parametr nen� v obecn�ch 
            def = this.GetParameter(this.graphContext, name, def);

            if(this.groupContext.Length > i)
                return this.GetParameter(this.groupContext[i] as Context, name, def);
            else
                return def;
        }

        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        public Graph() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="item">Data pro graf</param>
        /// <param name="background">Pozad� (densityGraph)</param>
        /// <param name="errors">Chyby k�ivek grafu</param>
        /// <param name="graphContext">Parametry cel�ho grafu</param>
        /// <param name="itemContext">Parametry jednotliv�ch k�ivek grafu</param>
        /// <param name="groupContext">Parametry skupin</param>
        public Graph(TArray item, TArray background, TArray errors, Context graphContext, TArray groupContext, TArray itemContext) {
            this.item = item;
            this.background = background;
            this.errors = errors;

            this.graphContext = graphContext;
            this.groupContext = groupContext;
            this.itemContext = itemContext;
        }

        /// <summary>
        /// Vr�t� maxim�ln� d�lku vektoru ve skupin�
        /// </summary>
        /// <param name="group">Skupina</param>
        public int GetMaxLength(int group) {
            int result = 0;

            if(group < this.NumGroups())
                foreach(PointVector pv in (this.item[group] as TArray))
                    result = System.Math.Max(pv.Length, result);

            return result;
        }

        /// <summary>
        /// Vr�t� minim�ln� a maxim�ln� hodnotu
        /// </summary>
        /// <param name="group">Index skupiny</param>
        /// <returns>Vektor se slo�kami (xmin, xmax, ymin, ymax), null, pokud pro zadanou skupinu neexistuje
        /// ��dn� k�ivka</returns>
        public Vector GetMinMax(int group) {
            double minX = 0.0, maxX = 0.0, minY = 0.0, maxY = 0.0;

            if(group >= this.NumGroups())
                return null;

            TArray a = this.item[group] as TArray;
            TArray e = this.errors.Length > group ? this.errors[group] as TArray : null;

            int nCurves = a.Length;

            if(nCurves == 0)
                return null;

            PointVector pv = a[0] as PointVector;
            Vector ev = (e != null && e.Length > 0) ? e[0] as Vector : null;

            if(pv.Length == 0)
                return null;

            minX = pv.MinX();
            maxX = pv.MaxX();

            if(ev != null && ev.Length > 0) {
                Vector vy = pv.VectorY;
                minY = (vy - ev).Min();
                maxY = (vy + ev).Max();
            }
            else {
                minY = pv.MinY();
                maxY = pv.MaxY();
            }

            for(int i = 1; i < nCurves; i++) {
                pv = a[i] as PointVector;
                ev = (e != null && e.Length > i) ? e[i] as Vector : null;

                minX = System.Math.Min(minX, pv.MinX());
                maxX = System.Math.Max(maxX, pv.MaxX());

                if(ev != null && ev.Length > 0) {
                    Vector vy = pv.VectorY;
                    minY = System.Math.Min(minY, (vy - ev).Min());
                    maxY = System.Math.Max(maxY, (vy + ev).Max());
                }
                else {
                    minY = System.Math.Min(minY, pv.MinY());
                    maxY = System.Math.Max(maxY, pv.MaxY());
                }
            }

            Vector result = new Vector(4);
            result[0] = minX;
            result[1] = maxX;
            result[2] = minY;
            result[3] = maxY;

            return result;
        }

        /// <summary>
        /// Vytvo�� �adu bod� k vykreslen�
        /// </summary>
        /// <param name="group">Index skupiny</param>
        /// <param name="index">Index dat</param>
        /// <param name="offsetX">Posunut� vhledem k ose X</param>
        /// <param name="amplifyX">Zes�len� v ose X</param>
        /// <param name="offsetY">Posunut� vhledem k ose Y</param>
        /// <param name="amplifyY">Zes�len� v ose Y</param>
        /// <param name="maxLength">Maxim�ln� d�lka dat</param>
        public Point[] PointArrayToDraw(int group, int index, double offsetX, double amplifyX, double offsetY, double amplifyY, int maxLength) {
            Point[] result = new Point[0];

            PointVector curve = this.GetCurve(group, index);

            if(curve.Length > 0){
                int length = maxLength < 0 ? curve.Length : System.Math.Min(curve.Length, maxLength);
                result = new Point[length];

                for(int i = 0; i < length; i++)
                    result[i] = new Point((int)(curve[i].X * amplifyX + offsetX), (int)(-curve[i].Y * amplifyY + offsetY));
            }

            return result;
        }

        /// <summary>
        /// Vytvo�� �adu bod� k vykreslen�
        /// </summary>
        /// <param name="group">Index skupiny</param>
        /// <param name="index">Index vektoru</param>
        /// <param name="offsetX">Posunut� vhledem k ose X</param>
        /// <param name="amplifyX">Zes�len� v ose X</param>
        /// <param name="offsetY">Posunut� vhledem k ose Y</param>
        /// <param name="amplifyY">Zes�len� v ose Y</param>
        public Point[] PointArrayToDraw(int group, int index, double offsetX, double amplifyX, double offsetY, double amplifyY) {
            return this.PointArrayToDraw(group, index, offsetX, amplifyX, offsetY, amplifyY, -1);
        }

        /// <summary>
        /// Vytvo�� po��te�n� a koncov� bod pro chybovou �se�ku
        /// </summary>
        /// <param name="group">Index skupiny</param>
        /// <param name="index">Index vektoru</param>
        /// <param name="offsetX">Posunut� vhledem k ose X</param>
        /// <param name="amplifyX">Zes�len� v ose X</param>
        /// <param name="offsetY">Posunut� vhledem k ose Y</param>
        /// <param name="amplifyY">Zes�len� v ose Y</param>
        /// <returns>�adu bod� - po��tek a konec chybov� �se�ky</returns>
        public Point[] GetErrorLines(int group, int index, double offsetX, double amplifyX, double offsetY, double amplifyY) {
            Point[] result = new Point[0];

            Vector error = this.GetError(group, index);
            PointVector curve = this.GetCurve(group, index);

            if(error.Length > 0 && curve.Length > 0) {
                int length = System.Math.Min(curve.Length, error.Length);
                result = new Point[2 * length];

                for(int i = 0; i < length; i++) {
                    int x = (int)(curve[i].X * amplifyX + offsetX);
                    result[2 * i] = new Point(x, (int)(-(curve[i].Y + error[i]) * amplifyY + offsetY));
                    result[2 * i + 1] = new Point(x, (int)(-(curve[i].Y - error[i]) * amplifyY + offsetY));
                }
            }

            return result;
        }

        #region Implementace IExportable
        /// <summary>
        /// Ulo�� obsah do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public virtual void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.item, "Data");
            param.Add(this.background, "Pozad�");
            param.Add(this.errors, "Chyby");

            param.Add(this.graphContext, "Parametry grafu");
            param.Add(this.itemContext, "Parametry k�ivek");
            param.Add(this.groupContext, "Parametry pozad�");

            param.Export(export);
        }

        /// <summary>
        /// Na�te obsah ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public virtual void Import(PavelStransky.Math.Import import) {
            IEParam param = new IEParam(import);

            if(import.VersionNumber >= 4) {
                this.item = (TArray)param.Get();
                this.background = (TArray)param.Get();
                this.errors = (TArray)param.Get();

                this.graphContext = (Context)param.Get();
                this.itemContext = (TArray)param.Get();
                this.groupContext = (TArray)param.Get();
            }
        }
        #endregion

        // Chyby
        private const string errorMessageBadType = "Parametr {0} grafu m� �patn� typ.";
        private const string errorMessageBadTypeDetail = "Po�adovan� typ: {0}\nZadan� typ: {1}\nZadan� hodnota: {2}";

        private const string errorMessageBadLength = "Chybn� d�lka vektoru s chybov�mi �se�kami.";
        private const string errorMessageBadLengthDetail = "Index skupiny s daty: {0}\nIndex vektoru s daty: {1}\nD�lka vektoru: {2}\nD�lka vektoru s �se�kami: {3}";
    }

    /// <summary>
    /// V�jimka ve t��d� Graph
    /// </summary>
    public class GraphException : DetailException {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        public GraphException(string message) : base(errMessage + message) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        public GraphException(string message, Exception innerException) : base(errMessage + message, innerException) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybov�ho hl�en�</param>
        /// <param name="detailMessage">Detail chyby</param>
        public GraphException(string message, string detailMessage)
            : base(message, detailMessage) {
        }

        private const string errMessage = "V grafu do�lo k chyb�: ";
    }
}