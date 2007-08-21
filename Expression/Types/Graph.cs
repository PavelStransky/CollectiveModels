using System;
using System.Drawing;

using PavelStransky.Math;

namespace PavelStransky.Expression {
    /// <summary>
    /// Tøída, která zapouzdøuje graf (urèuje jeho obsah, popø. další parametry)
    /// </summary>
    public class Graph : IExportable {
        private TArray item, background;
        private TArray errors;

        /// <summary>
        /// Styly spojnic bodù
        /// </summary>
        public enum LineStyles { Line, Curve, None }

        /// <summary>
        /// Styly bodù
        /// </summary>
        public enum PointStyles { Circle, Square, None, HLines, VLines, FCircle, FSquare }

        // Kontext a výraz celého grafu
        private Context graphContext;
        // Kontexty jednotlivých køivek
        private TArray itemContext;
        // Kontexty jednotlivých pozadí
        private TArray groupContext;

        /// <summary>
        /// Prvek grafu
        /// </summary>
        public TArray Item { get { return this.item; } }

        /// <summary>
        /// Pozadí - DensityGraph
        /// </summary>
        public TArray Background { get { return this.background; } }

        /// <summary>
        /// Chyby grafu
        /// </summary>
        public TArray Errors { get { return this.errors; } }

        /// <summary>
        /// Poèet køivek grafu ve skupinì
        /// </summary>
        /// <param name="group">Èíslo skupiny køivky</param>
        public int NumCurves(int group) {
            if(group < this.NumGroups())
                return (this.item[group] as TArray).GetLength(0);
            else
                return 0;
        }

        /// <summary>
        /// Poèet skupin køivek
        /// </summary>
        /// <returns></returns>
        public int NumGroups() {
            return this.item.GetLength(0);
        }

        /// <summary>
        /// Vrátí køivku
        /// </summary>
        /// <param name="group">Index skupiny køivky</param>
        /// <param name="i">Index køivky</param>
        public PointVector GetCurve(int group, int i) {
            if(group < this.NumGroups() && i < this.NumCurves(group))
                return (this.item[group] as TArray)[i] as PointVector;
            else
                return new PointVector(0);
        }

        /// <summary>
        /// Vrátí vektor s chybami
        /// </summary>
        /// <param name="group">Index skupiny køivky</param>
        /// <param name="i">Index køivky</param>
        public Vector GetError(int group, int i) {
            if(group < this.errors.Length && i < (this.errors[group] as TArray).Length)
                return (this.errors[group] as TArray)[i] as Vector;
            else
                return new Vector(0);
        }

        /// <summary>
        /// True, pokud jsou pro zadanou skupinu a køivku zadány chyby
        /// </summary>
        /// <param name="group">Index skupiny køivky</param>
        /// <param name="i">Index køivky</param>
        public bool IsErrors(int group, int i) {
            return this.GetError(group, i).Length > 0;
        }

        /// <summary>
        /// Vrátí matici pozadí
        /// </summary>
        /// <param name="group">Index skupiny</param>
        public Matrix GetMatrix(int group) {
            if(group < this.background.Length)
                return this.background[group] as Matrix;
            else
                return new Matrix(0);
        }

        /// <summary>
        /// Vrátí délku i - té køivky
        /// </summary>
        /// <param name="group">Èíslo skupiny køivky</param>
        /// <param name="i">Index køivky</param>
        public int GetLength(int group, int i) {
            return this.GetCurve(group, i).Length;
        }

        /// <summary>
        /// Vrací kontext zadané køivky
        /// </summary>
        /// <param name="group">Index skupiny køivky</param>
        /// <param name="i">Index køivky</param>
        private Context GetCurveContext(int group, int i) {
            if(group < this.NumGroups() && i < this.NumCurves(group))
                return (this.itemContext[group] as TArray)[i] as Context;
            else
                return new Context();
        }

        /// <summary>
        /// Vrací hodnotu parametru a kontroluje, jestli má zadaný parametr správný typ
        /// </summary>
        /// <param name="name">Název parametru</param>
        /// <param name="context">Kontext, na kterém se hledá</param>
        /// <param name="def">Default hodnota</param>
        /// <returns>Hodnota parametru, default, pokud parametr není zadán</returns>
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
        /// Vrací hodnotu z obecných parametrù
        /// </summary>
        /// <param name="name">Název parametru</param>
        /// <param name="context">Kontext, na kterém se hledá</param>
        /// <param name="def">Default hodnota</param>
        /// <returns>Hodnota parametru, default, pokud parametr není zadán</returns>
        public object GetGeneralParameter(string name, object def) {
            return this.GetParameter(this.graphContext, name, def);
        }

        /// <summary>
        /// Vrací hodnotu parametru køivky i
        /// </summary>
        /// <param name="group">Index skupiny køivky</param>
        /// <param name="i">Index køivky</param>
        /// <param name="name">Název parametru</param>
        /// <param name="context">Kontext, na kterém se hledá</param>
        /// <param name="def">Default hodnota</param>
        /// <returns>Hodnota parametru, default, pokud parametr není zadán</returns>
        public object GetCurveParameter(int group, int i, string name, object def) {
            // Nejdøíve koukáme, jestli parametr není v obecných 
            def = this.GetParameter(this.graphContext, name, def);

            if(this.itemContext.Length > group && (this.itemContext[group] as TArray).Length > i)
                return this.GetParameter(this.GetCurveContext(group, i), name, def);
            else
                return def;
        }

        /// <summary>
        /// Vrací hodnotu parametru pozadí i
        /// </summary>
        /// <param name="i">Index pozadí</param>
        /// <param name="name">Název parametru</param>
        /// <param name="context">Kontext, na kterém se hledá</param>
        /// <param name="def">Default hodnota</param>
        /// <returns>Hodnota parametru, default, pokud parametr není zadán</returns>
        public object GetBackgroundParameter(int i, string name, object def) {
            // Nejdøíve koukáme, jestli parametr není v obecných 
            def = this.GetParameter(this.graphContext, name, def);

            if(this.groupContext.Length > i)
                return this.GetParameter(this.groupContext[i] as Context, name, def);
            else
                return def;
        }

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        public Graph() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="item">Data pro graf</param>
        /// <param name="background">Pozadí (densityGraph)</param>
        /// <param name="errors">Chyby køivek grafu</param>
        /// <param name="graphContext">Parametry celého grafu</param>
        /// <param name="itemContext">Parametry jednotlivých køivek grafu</param>
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
        /// Vrátí maximální délku vektoru ve skupinì
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
        /// Vrátí minimální a maximální hodnotu
        /// </summary>
        /// <param name="group">Index skupiny</param>
        /// <returns>Vektor se složkami (xmin, xmax, ymin, ymax), null, pokud pro zadanou skupinu neexistuje
        /// žádná køivka</returns>
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
        /// Vytvoøí øadu bodù k vykreslení
        /// </summary>
        /// <param name="group">Index skupiny</param>
        /// <param name="index">Index dat</param>
        /// <param name="offsetX">Posunutí vhledem k ose X</param>
        /// <param name="amplifyX">Zesílení v ose X</param>
        /// <param name="offsetY">Posunutí vhledem k ose Y</param>
        /// <param name="amplifyY">Zesílení v ose Y</param>
        /// <param name="maxLength">Maximální délka dat</param>
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
        /// Vytvoøí øadu bodù k vykreslení
        /// </summary>
        /// <param name="group">Index skupiny</param>
        /// <param name="index">Index vektoru</param>
        /// <param name="offsetX">Posunutí vhledem k ose X</param>
        /// <param name="amplifyX">Zesílení v ose X</param>
        /// <param name="offsetY">Posunutí vhledem k ose Y</param>
        /// <param name="amplifyY">Zesílení v ose Y</param>
        public Point[] PointArrayToDraw(int group, int index, double offsetX, double amplifyX, double offsetY, double amplifyY) {
            return this.PointArrayToDraw(group, index, offsetX, amplifyX, offsetY, amplifyY, -1);
        }

        /// <summary>
        /// Vytvoøí poèáteèní a koncový bod pro chybovou úseèku
        /// </summary>
        /// <param name="group">Index skupiny</param>
        /// <param name="index">Index vektoru</param>
        /// <param name="offsetX">Posunutí vhledem k ose X</param>
        /// <param name="amplifyX">Zesílení v ose X</param>
        /// <param name="offsetY">Posunutí vhledem k ose Y</param>
        /// <param name="amplifyY">Zesílení v ose Y</param>
        /// <returns>Øadu bodù - poèátek a konec chybové úseèky</returns>
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
        /// Uloží obsah do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public virtual void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.item, "Data");
            param.Add(this.background, "Pozadí");
            param.Add(this.errors, "Chyby");

            param.Add(this.graphContext, "Parametry grafu");
            param.Add(this.itemContext, "Parametry køivek");
            param.Add(this.groupContext, "Parametry pozadí");

            param.Export(export);
        }

        /// <summary>
        /// Naète obsah ze souboru
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
        private const string errorMessageBadType = "Parametr {0} grafu má špatný typ.";
        private const string errorMessageBadTypeDetail = "Požadovaný typ: {0}\nZadaný typ: {1}\nZadaná hodnota: {2}";

        private const string errorMessageBadLength = "Chybná délka vektoru s chybovými úseèkami.";
        private const string errorMessageBadLengthDetail = "Index skupiny s daty: {0}\nIndex vektoru s daty: {1}\nDélka vektoru: {2}\nDélka vektoru s úseèkami: {3}";
    }

    /// <summary>
    /// Výjimka ve tøídì Graph
    /// </summary>
    public class GraphException : DetailException {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        public GraphException(string message) : base(errMessage + message) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        public GraphException(string message, Exception innerException) : base(errMessage + message, innerException) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Text chybového hlášení</param>
        /// <param name="detailMessage">Detail chyby</param>
        public GraphException(string message, string detailMessage)
            : base(message, detailMessage) {
        }

        private const string errMessage = "V grafu došlo k chybì: ";
    }
}