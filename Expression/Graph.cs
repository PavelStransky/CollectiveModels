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
        private TArray backgroundContext;

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
        /// Poèet køivek grafu ve skupinì)
        /// </summary>
        /// <param name="group">Èíslo skupiny køivky</param>
        public int NumCurves(int group) {
            if(group < this.NumGroups())
                return (this.item[group] as TArray).Count;
            else
                return 0;
        }

        /// <summary>
        /// Poèet skupin køivek
        /// </summary>
        /// <returns></returns>
        public int NumGroups() {
            return this.item.Count;
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
                return null;
        }

        /// <summary>
        /// Vrátí vektor s chybami
        /// </summary>
        /// <param name="group">Index skupiny køivky</param>
        /// <param name="i">Index køivky</param>
        private Vector GetError(int group, int i) {
            if(group < this.NumGroups() && i < this.NumCurves(group))
                return (this.errors[group] as TArray)[i] as Vector;
            else
                return null;
        }

        /// <summary>
        /// Vrátí délku i - té køivky
        /// </summary>
        /// <param name="group">Èíslo skupiny køivky</param>
        /// <param name="i">Index køivky</param>
        public int GetLength(int group, int i) {
            object curve = this.GetCurve(group, i);
            if(curve != null) 
                return (curve as PointVector).Length;
            else
                return 0;
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
        /// <param name="group">Index skupiny køivky</param>
        /// <param name="i">Index køivky</param>
        /// <param name="name">Název parametru</param>
        /// <param name="context">Kontext, na kterém se hledá</param>
        /// <param name="def">Default hodnota</param>
        /// <returns>Hodnota parametru, default, pokud parametr není zadán</returns>
        public object GetCurveParameter(int group, int i, string name, object def) {
            // Nejdøíve koukáme, jestli parametr není v obecných 
            def = this.GetParameter(this.graphContext, name, def);
            return this.GetParameter(this.GetCurveContext(group, i), name, def);
        }

        /// <summary>
        /// Vrací hodnotu parametru pozadí i
        /// </summary>
        /// <param name="i">Index pozadí</param>
        /// <param name="name">Název parametru</param>
        /// <param name="context">Kontext, na kterém se hledá</param>
        /// <param name="def">Default hodnota</param>
        /// <returns>Hodnota parametru, default, pokud parametr není zadán</returns>
        public object BackgroundParameter(int i, string name, object def) {
            // Nejdøíve koukáme, jestli parametr není v obecných 
            def = this.GetParameter(this.graphContext, name, def);
            return this.GetParameter(this.backgroundContext[i], name, def);
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
        public Graph(object item, object background, string graphParams, TArray itemParams, TArray backgroundParams, object errors) {
            // Data a chyby
            this.item = new TArray();
            this.errors = new TArray();
            if(item != null) {
                if(item is PointVector || item is Vector) {
                    TArray a = new TArray();
                    a.Add(item);
                    this.item.Add(a);

                    TArray e = new TArray();
                    if(errors != null){ 
                        if(errors is Vector) 
                            e.Add(errors);
                        else
                            throw new GraphException(errorMessageBadErrorType);
                    }
                    else
                        e.Add(null);
                    this.errors.Add(a);
                }

                else if(item is TArray) {
                    Type t = (item as TArray).ItemType;
                    if(t == typeof(PointVector) || t == typeof(Vector)) {
                        this.item.Add(item);

                        if(errors != null) {
                            int count = (item as TArray).Count;

                            if(errors is Vector) {
                                TArray e = new TArray();
                                int length = (errors as Vector).Length;

                                for(int i = 0; i < count; i++) {
                                    int li = ((item as TArray)[i] as PointVector).Length;
                                    if(li == length)
                                        e.Add(errors, (item as TArray).Count);
                                    else
                                        throw new GraphException(errorMessageBadLength,
                                            string.Format(errorMessageBadLengthDetail, 0, i, li, length));
                                }
                                this.errors.Add(e);
                            }
                            else if(errors is TArray && (errors as TArray).ItemType == typeof(Vector)) {
                                int ce = (errors as TArray).Count;
                                TArray e = errors;

                                for(int i = 0; i < count; i++) {
                                    if(ce <= i)
                                        e.Add(null);

                                    int li = ((item as TArray)[i] as PointVector).Length;
                                    PointVector pe = (errors as TArray)[i] as PointVector;
                                    if(pe != null) {
                                        int le = ((errors as TArray)[i] as PointVector).Length;
                                        if(li != le)
                                            throw new GraphException(errorMessageBadLength,
                                                string.Format(errorMessageBadLengthDetail, 0, i, li, le));
                                    }
                                }

                                this.errors.Add(e);
                            }
                        }
                    }

                    else if(t == typeof(TArray)) {
                        t = (item as TArray)[0].GetType();
                        if(t == typeof(PointVector) || t == typeof(Vector)) {
                            this.item = item;
                        }
                    }
                }
            }

            // Pøevedeme na PointVectory
            int nGroups = this.NumGroups();
            for(int g = 0; g < nGroups; g++) {
                TArray group = this.item[g];
                int curves = group.Count;
                for(int i = 0; i < curves; i++)
                    if(group[i] is Vector)
                        group[i] = new PointVector(group[i] as Vector);
            }

            // Pozadí - musí jich být stejnì jako grup
            this.background = null;
            if(background != null) {
                if(background is Matrix) {
                    this.background = new TArray();
                    this.background.Add(background, nGroups);
                }
                else if(background is TArray && (background as TArray).ItemType == typeof(Matrix)) {
                    this.background = background as TArray;
                    int nBack = this.background.Count;
                    if(nGroups == 1 && nBack > 1) {
                        this.item.Add(this.item[0], nBack - 1);
                        nGroups = this.NumGroups();
                    }

                    int limit = System.Math.Max(nGroups, nBack);

                    for(int i = 0; i < limit; i++) {
                        if(i >= nBack)
                            this.background.Add(null);
                        else if(i >= nGroups) {
                            TArray a = new TArray();
                            a.Add(null);
                            this.item.Add(a);
                        }
                    }
                }
            }

            if(this.background == null)
                this.background.Add(null, nGroups);

            // Chyby
            this.errors = this.CreateErrorArray(errors);
            
            nGroups = this.NumGroups();

            // Kontexty - pokud máme v kontextech pro øadu èi pro pozadí jen jednu hodnotu
            // (string), pøiøadíme ji ke kontextu grafu
            if(graphParams == null)
                graphParams = string.Empty;
            if(itemParams != null && itemParams is string) {
                graphParams = (string)graphParams + (string)itemParams;
                itemParams = null;
            }
            if(backgroundParams != null && backgroundParams is string) {
                graphParams = (string)graphParams + (string)backgroundParams;
                backgroundParams = null;
            }   

            this.graphContext = new Context();
            if(graphParams != null && graphParams != string.Empty) {
                Expression e = new Expression(graphParams);
                e.Evaluate(this.graphContext);
            }

            this.itemContext = new TArray();
            for(int g = 0; g < nGroups; g++) {
                int curves = this.NumCurves(g);

                object ip = null;
                if(itemParams != null)
                    ip = itemParams as TArray;
                if(ip != null){
                    if((ip as TArray).Count > g)
                        ip = ip[g];
                    else
                        ip = null;
                }

                TArray ca = new TArray();
                if(ip is string && ip as string != string.Empty)
                {
                    Context c = new Context();
                    (new Expression(ip as string)).Evaluate(c);
                    ca.Add(c, curves);
                }
                else if(ip is TArray) {
                    for(int i = 0; i < curves; i++) {
                        Context c = new Context();

                        string iip = null;
                        if((ip as TArray).Count > i)
                            iip = ip[i] as string;

                        if(iip != null && iip != string.Empty)
                            new Expression(iip).Evaluate(c);

                        ca.Add(c);
                    }
                }

                this.itemContext.Add(ca);
            }

            this.backgroundContext = new TArray();
            for(int g = 0; g < nGroups; g++) {
                Context c = new Context();

                object bp = null;
                if(backgroundParams as TArray != null && (backgroundParams as TArray).Count > g)
                    bp = (backgroundParams as TArray)[i] as string;

                if(bp != null && bp != string.Empty)
                    (new Expression(bp as string)).Evaluate(c);

                this.backgroundContext.Add(c);
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
            int result = 0;

            foreach(TArray a in this.item)
                foreach(PointVector pv in a)
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
            TArray e = this.errors[group] as TArray;

            int curves = a.Count;

            if(curves == 0)
                return null;

            PointVector pv = a[0] as PointVector;
            Vector ev = e[0] as Vector;
            minX = pv.MinX();
            maxX = pv.MaxX();

            if(ev != null) {
                Vector vy = pv.VectorY;
                minY = (vy - ev).Min();
                maxY = (vy + ev).Max();
            }
            else {
                minY = pv.MinY();
                maxY = pv.MaxY();
            }

            for(int i = 1; i < count; i++) {
                pv = a[i] as PointVector;
                Vector ev = e[i] as Vector;

                minX = System.Math.Min(minX, pv.MinX());
                maxX = System.Math.Max(maxX, pv.MaxX());

                if(ev != null) {
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

            if(curve != null){
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

            if(error != null && curve != null) {
                int length = curve.Length;
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
            param.Add(this.backgroundContext, "Parametry pozadí");

            param.Export(export);
        }

        /// <summary>
        /// Naète obsah ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public virtual void Import(PavelStransky.Math.Import import) {
            IEParam param = new IEParam(import);
            
            this.item = (TArray)param.Get();
            this.background = (TArray)param.Get();
            this.errors = (TArray)param.Get();

            this.graphContext = (Context)param.Get();
            this.itemContext = (Context)param.Get();
            this.backgroundContext = (Context)param.Get();
        }
        #endregion

        // Chyby
        private const string errorMessageBadType = "Parametr {0} grafu má špatný typ.";
        private const string errorMessageBadTypeDetail = "Požadovaný typ: {0}\nZadaný typ: {1}\nZadaná hodnota: {2}";

        private const string errorMessageBadErrorType = "Zadané chyby mají špatný typ.";

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