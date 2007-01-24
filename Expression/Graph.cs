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
        private TArray backgroundContext;

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
        /// Po�et k�ivek grafu ve skupin�)
        /// </summary>
        /// <param name="group">��slo skupiny k�ivky</param>
        public int NumCurves(int group) {
            if(group < this.NumGroups())
                return (this.item[group] as TArray).Count;
            else
                return 0;
        }

        /// <summary>
        /// Po�et skupin k�ivek
        /// </summary>
        /// <returns></returns>
        public int NumGroups() {
            return this.item.Count;
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
            if(group < this.NumGroups() && i < this.NumCurves(group))
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
            return this.background[group] as Matrix;
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
            return this.GetParameter(this.GetCurveContext(group, i), name, def);
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
            return this.GetParameter(this.backgroundContext[i] as Context, name, def);
        }

        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        public Graph() { }

        /// <summary>
        /// Ze zadan�ho objektu vytvo�� 2D TArray pole
        /// </summary>
        /// <param name="item">Data pro graf</param>
        private void CreateItemArray(object item) {
            this.item = new TArray();

            if(item is PointVector) {
                TArray a = new TArray();
                a.Add(item);
                this.item.Add(a);
            }
            else if(item is Vector) {
                TArray a = new TArray();
                a.Add(new PointVector(item as Vector));
                this.item.Add(a);
            }

            else if(item is TArray) {
                Type t = (item as TArray).ItemType;
                if(t == typeof(PointVector))
                    this.item.Add(item);

                else if(t == typeof(Vector)) {
                    TArray a = new TArray();
                    foreach(Vector v in item as TArray)
                        a.Add(new PointVector(v));
                    this.item.Add(a);
                }

                else if(t == typeof(TArray)) {
                    t = ((item as TArray)[0] as TArray).ItemType;
                    if(t == typeof(PointVector))
                        this.item = item as TArray;

                    else if(t == typeof(Vector)) {
                        foreach(TArray ta in item as TArray) {
                            TArray a = new TArray();
                            foreach(Vector v in ta)
                                a.Add(new PointVector(v));
                            this.item.Add(a);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Ze zadan�ho objektu vytvo�� 2D TArray pole, kter� bude kop�rovat pole s daty
        /// Pokud nejsou pro k�ivku zadan� chyby, prvek obsahuje Vector(0)
        /// </summary>
        /// <param name="errors">Objekt s chybami</param>
        private void CreateErrorArray(object errors) {
            int nGroups = this.NumGroups();
            this.errors = null;

            // Rozkop�rov�n� na v�echny k�ivky s kontrolou d�lky
            if(errors is Vector) {
                this.errors = new TArray();
                int le = (errors as Vector).Length;

                for(int g = 0; g < nGroups; g++) {
                    TArray group = this.item[g] as TArray;
                    int nCurves = group.Count;

                    for(int i = 0; i < nCurves; i++) {
                        int li = (group[i] as PointVector).Length;

                        if(li != le)
                            throw new GraphException(errorMessageBadLength,
                                string.Format(errorMessageBadLengthDetail, g, i, li, le));
                    }

                    TArray e = new TArray();
                    e.Add(errors, nCurves);
                    this.errors.Add(e);
                }
            }

            else if(errors is TArray) {
                Type t = (errors as TArray).ItemType;

                // Rozkop�rov�n� na v�echny prvky skupiny
                if(t == typeof(Vector)) {
                    this.errors = new TArray();
                    int cv = (errors as TArray).Count;

                    for(int g = 0; g < nGroups; g++) {
                        TArray group = this.item[g] as TArray;
                        int nCurves = group.Count;

                        if(g >= cv) {
                            TArray e = new TArray();
                            e.Add(new Vector(0), nCurves);
                            this.errors.Add(e);
                        }
                        else {
                            Vector ev = (errors as TArray)[g] as Vector;
                            int le = ev.Length;

                            for(int i = 0; i < nCurves; i++) {
                                int li = (group[i] as PointVector).Length;

                                if(li != le)
                                    throw new GraphException(errorMessageBadLength,
                                        string.Format(errorMessageBadLengthDetail, g, i, li, le));
                            }

                            TArray e = new TArray();
                            e.Add(errors, nCurves);
                            this.errors.Add(e);
                        }
                    }
                }

                else if(t == typeof(TArray) && ((errors as TArray)[0] as TArray).ItemType == typeof(Vector)) {
                    this.errors = new TArray();
                    int ce = (errors as TArray).Count;

                    for(int g = 0; g < nGroups; g++) {
                        TArray group = this.item[g] as TArray;
                        int nCurves = group.Count;

                        if(g >= ce) {
                            TArray e = new TArray();
                            e.Add(new PointVector(0), nCurves);
                            this.errors.Add(e);
                        }
                        else {
                            TArray eGroup = (errors as TArray)[g] as TArray;
                            int cce = eGroup.Count;

                            TArray e = new TArray(typeof(PointVector));

                            for(int i = 0; i < nCurves; i++) {
                                int li = (group[i] as PointVector).Length;

                                if(i >= cce)
                                    e.Add(new Vector(0));
                                else {
                                    int le = (eGroup[i] as Vector).Length;

                                    if(li != le)
                                        throw new GraphException(errorMessageBadLength,
                                            string.Format(errorMessageBadLengthDetail, g, i, li, le));

                                    e.Add(eGroup[i]);
                                }
                            }

                            this.errors.Add(e);
                        }
                    }
                }
            }

            // Nem�me zadan� ��dn� chyby
            if(this.errors == null) {
                this.errors = new TArray();
                for(int g = 0; g < nGroups; g++) {
                    TArray e = new TArray();
                    e.Add(new Vector(0), this.NumCurves(g));
                    this.errors.Add(e);
                }
            }
        }

        /// <summary>
        /// Ze zadan�ho objektu vytvo�� TArray pole
        /// (prvk� bude stejn�, jako je grup vstupn�ch dat, p��padn� vstupn� data rozkop�ruje)
        /// Pokud nen� pozad�, prvek obsahuje Matrix(0)
        /// </summary>
        /// <param name="background">Objekt s pozad�m</param>
        private void CreateBackgroundArray(object background) {
            int nGroups = this.NumGroups();
            this.background = null;

            if(background is Matrix) {
                this.background = new TArray();

                if(nGroups == 0) {
                    this.background.Add(background);
                    this.item.Add(new TArray(typeof(PointVector)));
                    this.errors.Add(new TArray(typeof(Vector)));
                }
                else
                    this.background.Add(background, nGroups);
            }

            else if(background is TArray && (background as TArray).ItemType == typeof(Matrix)) {
                this.background = background as TArray;
                int nBack = this.background.Count;
                if(nGroups == 1 && nBack > 1) {
                    this.item.Add(this.item[0], nBack - 1);
                    this.errors.Add(this.errors[0], nBack - 1);
                    nGroups = this.NumGroups();
                }

                int limit = System.Math.Max(nGroups, nBack);

                for(int i = 0; i < limit; i++) {
                    if(i >= nBack)
                        this.background.Add(new Matrix(0));
                    else if(i >= nGroups) {
                        this.item.Add(new TArray(typeof(PointVector)));
                        this.errors.Add(new TArray(typeof(Vector)));
                    }
                }
            }

            if(this.background == null) {
                this.background = new TArray();
                this.background.Add(new Matrix(0), nGroups);
            }
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="item">Data pro graf</param>
        /// <param name="background">Pozad� (densityGraph)</param>
        /// <param name="errors">Chyby k�ivek grafu</param>
        /// <param name="graphParams">Parametry cel�ho grafu</param>
        /// <param name="itemParams">Parametry jednotliv�ch k�ivek grafu</param>
        /// <param name="backgroundParams">Parametry pozad�</param>
        public Graph(object item, object background, object errors, object graphParams, object itemParams, object backgroundParams) {
            // Data
            this.CreateItemArray(item);
            // Chyby
            this.CreateErrorArray(errors);
            // Pozad�
            this.CreateBackgroundArray(background);
            
            int nGroups = this.NumGroups();

            // Parametry grafu
            this.graphContext = null;
            if(graphParams is string && graphParams as string != string.Empty) {
                this.graphContext = new Context();
                (new Expression(graphParams as string)).Evaluate(this.graphContext);
            }
            else if(graphParams is Context)
                this.graphContext = graphParams as Context;
            if(this.graphContext == null)
                this.graphContext = new Context();

            // Parametry k�ivek - nejprve v�e p�evedeme na kontexty
            if(itemParams is string && itemParams as string != string.Empty){
                Context c = new Context();
                (new Expression(itemParams as string)).Evaluate(c);
                itemParams = c;
            }

            else if(itemParams is TArray) {
                int inGroups = (itemParams as TArray).Count;
                TArray ipn = new TArray();

                for(int g = 0; g < inGroups; g++) {
                    object ip = (itemParams as TArray)[g];
                    if(ip is string && ip as string != string.Empty) {
                        Context c = new Context();
                        (new Expression(ip as string)).Evaluate(c);
                        ipn.Add(c);
                    }

                    else if(ip is Context)
                        ipn.Add(ip);

                    else if(ip is TArray) {
                        int inCurves = (ip as TArray).Count;
                        TArray iipn = new TArray();

                        for(int i = 0; i < inCurves; i++) {
                            object iip = (ip as TArray)[i];
                            if(iip is string && iip as string != string.Empty) {
                                Context c = new Context();
                                (new Expression(ip as string)).Evaluate(c);
                                iipn.Add(c);
                            }

                            else if(iip is Context)
                                iipn.Add(iip);
                        }

                        ipn.Add(iipn);
                    }
                }

                itemParams = ipn;
            }

            // Nyn� za�ad�me
            this.itemContext = new TArray();
            for(int g = 0; g < nGroups; g++) {
                int curves = this.NumCurves(g);

                object ip = null;
                if(itemParams != null) 
                    ip = itemParams;
                if(ip is TArray) {
                    if((ip as TArray).Count > g)
                        ip = (ip as TArray)[g];
                    else
                        ip = null;
                }

                TArray ca = new TArray();

                if(ip is Context) 
                    ca.Add(ip, curves);

                else if(ip is TArray) {
                    for(int i = 0; i < curves; i++) {
                        Context iip = null;
                        if((ip as TArray).Count > i)
                            iip = (ip as TArray)[i] as Context;

                        if(iip is Context)
                            ca.Add(iip);
                        else
                            ca.Add(new Context());
                    }
                }

                else {
                    Context c = new Context();
                    ca.Add(c, curves);
                }

                this.itemContext.Add(ca);
            }

            // Parametry pozad� - p�evod na kontexty
            if(backgroundParams is string && backgroundParams as string != string.Empty) {
                Context c = new Context();
                (new Expression(backgroundParams as string)).Evaluate(c);
                itemParams = c;
            }

            else if(backgroundParams is TArray) {
                int bnGroups = (backgroundParams as TArray).Count;
                TArray bpn = new TArray();

                for(int g = 0; g < bnGroups; g++) {
                    object bp = (backgroundParams as TArray)[g];
                    if(bp is string && bp as string != string.Empty) {
                        Context c = new Context();
                        (new Expression(bp as string)).Evaluate(c);
                        bpn.Add(c);
                    }
                    else if(bp is Context)
                        bpn.Add(bp);
                }

                backgroundParams = bpn;
            }            
            
            // Za�azen�
            this.backgroundContext = new TArray();
            for(int g = 0; g < nGroups; g++) {
                Context c = new Context();

                if(backgroundParams is Context)
                    this.backgroundContext.Add(backgroundParams);
                else if(backgroundParams is TArray && (backgroundParams as TArray).Count > g)
                    this.backgroundContext.Add((backgroundParams as TArray)[g]);
                else
                    this.backgroundContext.Add(new Context());
            }
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
            TArray e = this.errors[group] as TArray;

            int nCurves = a.Count;

            if(nCurves == 0)
                return null;

            PointVector pv = a[0] as PointVector;
            Vector ev = e[0] as Vector;
            minX = pv.MinX();
            maxX = pv.MaxX();

            if(ev.Length > 0) {
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
                ev = e[i] as Vector;

                minX = System.Math.Min(minX, pv.MinX());
                maxX = System.Math.Max(maxX, pv.MaxX());

                if(ev.Length > 0) {
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
            param.Add(this.backgroundContext, "Parametry pozad�");

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
                this.backgroundContext = (TArray)param.Get();
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