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
        private Context[] itemContext;

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
        /// Jsou zadané chyby?
        /// </summary>
        public bool IsErrors { get { return this.errors != null; } }

        /// <summary>
        /// Je promìnná Vector?
        /// </summary>
        public bool IsVector { get { return this.Count > 0 ? this.item[0] is Vector : false; } }


        /// <summary>
        /// Je promìnná PointVector?
        /// </summary>
        public bool IsPointVector { get { return this.Count > 0 ? this.item[0] is PointVector : false; } }

        /// <summary>
        /// Poèet køivek grafu
        /// </summary>
        public int Count { get { return (this.item as TArray).Count; } }

        /// <summary>
        /// Vrátí délku i - tého datového objektu
        /// </summary>
        /// <param name="i">Index objektu</param>
        public int GetLength(int i) {
            if(i < this.Count) {
                if(this.IsVector)
                    return (this.item[i] as Vector).Length;
                else if(this.IsPointVector)
                    return (this.item[i] as PointVector).Length;
            }

            return 0;
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
                    else if(item is TArray && (item as TArray).Count == 3) {
                        TArray a = item as TArray;
                        return Color.FromArgb((int)a[0], (int)a[1], (int)a[2]);
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
        /// <param name="i">Index køivky</param>
        /// <param name="name">Název parametru</param>
        /// <param name="context">Kontext, na kterém se hledá</param>
        /// <param name="def">Default hodnota</param>
        /// <returns>Hodnota parametru, default, pokud parametr není zadán</returns>
        public object GetCurveParameter(int i, string name, object def) {
            return this.GetParameter(this.itemContext[i], name, def);
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
        /// <param name="graphParams">Parametry celého grafu</param>
        /// <param name="itemParams">Parametry jednotlivých køivek grafu</param>
        /// <param name="errors">Chyby køivek grafu</param>
        public Graph(object item, object background, string graphParams, TArray itemParams, object errors) {
            // item bude vždy Array
            if(item as TArray == null) {
                this.item = new TArray();

                if(background as TArray == null) {
                    this.item.Add(item);
                    this.background = new TArray();
                    this.background.Add(background);
                }
                else {
                    this.background = background as TArray;
                    int count = this.background.Count;
                    for(int i = 0; i < count; i++)
                        this.item.Add(item);
                }
            }
            else {
                this.item = item as TArray;

                if(background as TArray == null) {
                    this.background = new TArray();
                    int count = this.item.Count;
                    for(int i = 0; i < count; i++)
                        this.background.Add(background);
                }
                else
                    this.background = background;
            }

            this.errors = this.CreateErrorArray(errors);

            int count = this.Count;

            this.graphContext = new Context();
            if(graphParams != null && graphParams != string.Empty) {
                Expression e = new Expression(graphParams);
                e.Evaluate(this.graphContext);
            }

            this.itemContext = new Context[count];
            for(int i = 0; i < count; i++) {
                this.itemContext[i] = new Context();
                if(itemParams != null && itemParams.Count > i && itemParams[i] != null && (itemParams[i] as string) != string.Empty) {
                    Expression e = new Expression(itemParams[i] as string);
                    e.Evaluate(this.itemContext[i]);
                }
            }
        }

        /// <summary>
        /// Vytvoøí øadu s daty k chybovým úseèkám
        /// </summary>
        /// <param name="errors">Vstupní objekt s chybovými úseèkami</param>
        private TArray CreateErrorArray(object errors) {
            int count = this.Count;
            TArray result = null;

            // Vytvoøení chybových úseèek
            if(this.IsVector || this.IsPointVector) {
                // Máme øadu dat, ale jen jeden vektor s chybovými úseèkami - rozkopírujeme
                if(errors != null && errors as TArray == null) {
                    Vector v = errors as Vector;

                    if((v as object) == null)
                        throw new GraphException(errorMessageBadErrorType, string.Format(errorMessageBadErrorTypeDetail, errors.GetType().FullName));

                    int length = v.Length;

                    result = new TArray();
                    for(int i = 0; i < count; i++) {
                        int l = this.GetLength(i);
                        if(l != length)
                            throw new GraphException(errorMessageBadLength, string.Format(errorMessageBadLengthDetail, i, l, length));

                        result.Add(errors);
                    }
                }
                else if(errors as TArray != null) {
                    TArray a = errors as TArray;

                    if(a.ItemType != typeof(Vector))
                        throw new GraphException(errorMessageBadErrorType, string.Format(errorMessageBadErrorTypeDetail, a.ItemTypeName));

                    int ecount = (errors as TArray).Count;
                    result = new TArray();

                    for(int i = 0; i < count; i++) {
                        int l = this.GetLength(i);
                        Vector e = (i < ecount) ? (errors as TArray)[i] as Vector : null;

                        if(e == null)
                            result.Add(new Vector(l));  // Pøidáme vektor se samými nulami
                        else {
                            if(l != e.Length)
                                throw new GraphException(errorMessageBadLength, string.Format(errorMessageBadLengthDetail, i, l, e.Length));

                            result.Add(e);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Vrátí maximální délku vektoru
        /// </summary>
        public int GetMaxLength() {
            int count = this.Count;
            int result = 0;

            if(this.IsVector) {
                foreach(Vector v in this.item)
                    result = System.Math.Max(v.Length, result);
            }
            else if(this.IsPointVector)
                foreach(PointVector pv in this.item)
                    result = System.Math.Max(pv.Length, result);

            return result;
        }

        /// <summary>
        /// Vrátí minimální a maximální hodnotu
        /// </summary>
        /// <returns>Vektor se složkami (xmin, xmax, ymin, ymax)</returns>
        public Vector GetMinMax() {
            int count = this.Count;

            double minX = 0.0, maxX = 0.0, minY = 0.0, maxY = 0.0;

            if(this.IsVector) {
                TArray a = this.item as TArray;

                minX = 0;
                maxX = (a[0] as Vector).Length;

                Vector v = a[0] as Vector;

                if(this.IsErrors) {
                    Vector e = this.errors[0] as Vector;

                    minY = (v - e).Min();
                    maxY = (v + e).Max();

                    for(int i = 1; i < count; i++) {
                        v = this.item[i] as Vector;
                        e = this.errors[i] as Vector;
                        minY = System.Math.Min(minY, (v - e).Min());
                        maxY = System.Math.Max(maxY, (v + e).Max());
                    }
                }
                else {
                    minY = v.Min();
                    maxY = v.Max();

                    for(int i = 1; i < count; i++) {
                        v = this.item[i] as Vector;
                        minY = System.Math.Min(minY, v.Min());
                        maxY = System.Math.Max(maxY, v.Max());
                    }
                }
            }

            else if(this.IsPointVector) {
                TArray a = this.item as TArray;

                PointVector pv = a[0] as PointVector;
                minX = pv.MinX();
                maxX = pv.MaxX();

                for(int i = 1; i < count; i++) {
                    pv = a[i] as PointVector;
                    minX = System.Math.Min(minX, pv.MinX());
                    maxX = System.Math.Max(maxX, pv.MaxX());
                }

                Vector vy = (a[0] as PointVector).VectorY;

                if(this.IsErrors) {
                    Vector ey = this.errors[0] as Vector;
                    minY = (vy - ey).Min();
                    maxY = (vy + ey).Max();

                    for(int i = 1; i < count; i++) {
                        vy = (a[i] as PointVector).VectorY;
                        ey = this.errors[i] as Vector;
                        minY = System.Math.Min(minY, (vy - ey).Min());
                        maxY = System.Math.Max(maxY, (vy - ey).Max());
                    }
                }
                else {
                    minY = vy.Min();
                    maxY = vy.Max();

                    for(int i = 1; i < count; i++) {
                        vy = (a[i] as PointVector).VectorY;
                        minY = System.Math.Min(minY, vy.Min());
                        maxY = System.Math.Max(maxY, vy.Max());
                    }
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
        /// <param name="index">Index vektoru</param>
        /// <param name="offsetX">Posunutí vhledem k ose X</param>
        /// <param name="amplifyX">Zesílení v ose X</param>
        /// <param name="offsetY">Posunutí vhledem k ose Y</param>
        /// <param name="amplifyY">Zesílení v ose Y</param>
        /// <param name="maxLength">Maximální délka dat</param>
        public Point[] PointArrayToDraw(int index, double offsetX, double amplifyX, double offsetY, double amplifyY, int maxLength) {
            Point[] result = new Point[0];

            if(index < this.Count) {
                if(this.IsVector) {
                    Vector v = this.item[index] as Vector;
                    int length = maxLength < 0 ? v.Length : System.Math.Min(v.Length, maxLength);
                    result = new Point[length];

                    for(int i = 0; i < length; i++)
                        result[i] = new Point((int)(i * amplifyX + offsetX), (int)(-v[i] * amplifyY + offsetY));
                }
                else if(this.IsPointVector) {
                    PointVector pv = this.item[index] as PointVector;
                    int length = maxLength < 0 ? pv.Length : System.Math.Min(pv.Length, maxLength);
                    result = new Point[length];

                    for(int i = 0; i < length; i++)
                        result[i] = new Point((int)(pv[i].X * amplifyX + offsetX), (int)(-pv[i].Y * amplifyY + offsetY));
                }
            }

            return result;
        }

        /// <summary>
        /// Vytvoøí øadu bodù k vykreslení
        /// </summary>
        /// <param name="index">Index vektoru</param>
        /// <param name="offsetX">Posunutí vhledem k ose X</param>
        /// <param name="amplifyX">Zesílení v ose X</param>
        /// <param name="offsetY">Posunutí vhledem k ose Y</param>
        /// <param name="amplifyY">Zesílení v ose Y</param>
        public Point[] PointArrayToDraw(int index, double offsetX, double amplifyX, double offsetY, double amplifyY) {
            return this.PointArrayToDraw(index, offsetX, amplifyX, offsetY, amplifyY, -1);
        }

        /// <summary>
        /// Vytvoøí poèáteèní a koncový bod pro chybovou úseèku
        /// </summary>
        /// <param name="index">Index vektoru</param>
        /// <param name="offsetX">Posunutí vhledem k ose X</param>
        /// <param name="amplifyX">Zesílení v ose X</param>
        /// <param name="offsetY">Posunutí vhledem k ose Y</param>
        /// <param name="amplifyY">Zesílení v ose Y</param>
        /// <returns>Øadu bodù - poèátek a konec chybové úseèky</returns>
        public Point[] GetErrorLines(int index, double offsetX, double amplifyX, double offsetY, double amplifyY) {
            Point[] result = new Point[0];

            if(this.IsErrors && index < this.Count) {
                if(this.IsVector) {
                    Vector v = this.item[index] as Vector;
                    Vector e = this.errors[index] as Vector;

                    int length = v.Length;
                    result = new Point[2 * length];

                    for(int i = 0; i < length; i++) {
                        int x = (int)(i * amplifyX + offsetX);
                        result[2 * i] = new Point(x, (int)(-(v[i] + e[i]) * amplifyY + offsetY));
                        result[2 * i + 1] = new Point(x, (int)(-(v[i] - e[i]) * amplifyY + offsetY));
                    }
                }
                else if(this.IsPointVector) {
                    PointVector pv = this.item[index] as PointVector;
                    Vector e = this.errors[index] as Vector;

                    int length = pv.Length;
                    result = new Point[2 * length];

                    for(int i = 0; i < length; i++) {
                        int x = (int)(pv[i].X * amplifyX + offsetX);
                        result[2 * i] = new Point(x, (int)(-(pv[i].Y + e[i]) * amplifyY + offsetY));
                        result[2 * i + 1] = new Point(x, (int)(-(pv[i].Y - e[i]) * amplifyY + offsetY));
                    }
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
            param.Add(this.graphContext, "Parametry grafu");
            param.Add(this.errors, "Chyby");

            int length = this.itemContext.Length;
            param.Add(length);
            for(int i = 0; i < length; i++)
                param.Add(this.itemContext[i], string.Format("Parametry køivky {0}", i));

            param.Export(export);
        }

        /// <summary>
        /// Naète obsah ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public virtual void Import(PavelStransky.Math.Import import) {
            IEParam param = new IEParam(import);
            
            this.item = (TArray)param.Get();
            this.graphContext = (Context)param.Get();
            this.errors = (TArray)param.Get();

            int length = (int)param.Get(0);
            this.itemContext = new Context[length];

            for(int i = 0; i < length; i++)
                this.itemContext[i] = (Context)param.Get();
        }
        #endregion

        // Chyby
        private const string errorMessageBadType = "Parametr {0} grafu má špatný typ.";
        private const string errorMessageBadTypeDetail = "Požadovaný typ: {0}\nZadaný typ: {1}\nZadaná hodnota: {2}";

        private const string errorMessageBadErrorType = "Zadané chyby mají špatný typ.";
        private const string errorMessageBadErrorTypeDetail = "Požadovaný typ: Array of Vectors | Vector\nZadaný typ: {1}";

        private const string errorMessageBadLength = "Chybná délka vektoru s chybovými úseèkami.";
        private const string errorMessageBadLengthDetail = "Index vektoru s daty: {0}\nDélka vektoru: {1}\nDélka vektoru s úseèkami: {2}";
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