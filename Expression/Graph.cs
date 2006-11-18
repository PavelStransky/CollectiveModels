using System;
using System.Drawing;

using PavelStransky.Math;

namespace PavelStransky.Expression {
	/// <summary>
	/// T��da, kter� zapouzd�uje graf (ur�uje jeho obsah, pop�. dal�� parametry)
	/// </summary>
	public class Graph: IExportable {
		private Array item;
        private Array errors;

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
        private Context[] itemContext;

		/// <summary>
		/// Prvek grafu
		/// </summary>
		public Array Item {get {return this.item;}}

        /// <summary>
        /// Chyby grafu
        /// </summary>
        public Array Errors { get { return this.errors; } }

        /// <summary>
        /// Jsou zadan� chyby?
        /// </summary>
        public bool IsErrors { get { return this.errors != null; } }

        /// <summary>
        /// Je prom�nn� Vector?
        /// </summary>
        public bool IsVector { get { return this.Count > 0 ? this.item[0] is Vector : false; } }


        /// <summary>
        /// Je prom�nn� PointVector?
        /// </summary>
        public bool IsPointVector { get { return this.Count > 0 ? this.item[0] is PointVector : false; } }
        
        /// <summary>
        /// Po�et k�ivek grafu
        /// </summary>
        public int Count { get { return (this.item as Array).Count; } }

        /// <summary>
        /// Vr�t� d�lku i - t�ho datov�ho objektu
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
        /// Je graf ��rov�?
        /// </summary>
        public bool IsLineGraph {
            get {
                if(this.Count > 0 && (this.item[0] is Vector || this.item[0] is PointVector))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Je graf hustot?
        /// </summary>
        public bool IsDensityGraph {
            get {
                if(this.Count > 0 && this.item[0] is Matrix)
                    return true;
                else
                    return false;
            }
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
                    else if(item is Array && (item as Array).Count == 3) {
                        Array a = item as Array;
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
        /// <param name="i">Index k�ivky</param>
        /// <param name="name">N�zev parametru</param>
        /// <param name="context">Kontext, na kter�m se hled�</param>
        /// <param name="def">Default hodnota</param>
        /// <returns>Hodnota parametru, default, pokud parametr nen� zad�n</returns>
        public object GetCurveParameter(int i, string name, object def) {
            return this.GetParameter(this.itemContext[i], name, def);
        }

        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        public Graph() { }

        /// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="item">Data pro graf</param>
		/// <param name="graphParams">Parametry cel�ho grafu</param>
        /// <param name="itemParams">Parametry jednotliv�ch k�ivek grafu</param>
        /// <param name="errors">Chyby k�ivek grafu</param>
        public Graph(object item, string graphParams, Array itemParams, object errors) {
            // item bude v�dy Array
            if(item as Array == null) {
                this.item = new Array();
                this.item.Add(item);
            }
            else
                this.item = item as Array;

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
        /// Vytvo�� �adu s daty k chybov�m �se�k�m
        /// </summary>
        /// <param name="errors">Vstupn� objekt s chybov�mi �se�kami</param>
        private Array CreateErrorArray(object errors) {
            int count = this.Count;
            Array result = null;

            // Vytvo�en� chybov�ch �se�ek
            if(this.IsVector || this.IsPointVector) {
                // M�me �adu dat, ale jen jeden vektor s chybov�mi �se�kami - rozkop�rujeme
                if(errors != null && errors as Array == null) {
                    Vector v = errors as Vector;

                    if((v as object) == null)
                        throw new GraphException(errorMessageBadErrorType, string.Format(errorMessageBadErrorTypeDetail, errors.GetType().FullName));

                    int length = v.Length;

                    result = new Array();
                    for(int i = 0; i < count; i++) {
                        int l = this.GetLength(i);
                        if(l!= length)
                            throw new GraphException(errorMessageBadLength, string.Format(errorMessageBadLengthDetail, i, l, length));

                        result.Add(errors);
                    }
                }
                else if(errors as Array != null) {
                    Array a = errors as Array;

                    if(a.ItemType != typeof(Vector))
                        throw new GraphException(errorMessageBadErrorType, string.Format(errorMessageBadErrorTypeDetail, a.ItemTypeName));

                    int ecount = (errors as Array).Count;
                    result = new Array();

                    for(int i = 0; i < count; i++) {
                        int l = this.GetLength(i);
                        Vector e = (i < ecount) ? (errors as Array)[i] as Vector : null;

                        if(e == null)
                            result.Add(new Vector(l));  // P�id�me vektor se sam�mi nulami
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
        /// Vr�t� maxim�ln� d�lku vektoru
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
		/// Vr�t� minim�ln� a maxim�ln� hodnotu
		/// </summary>
        /// <returns>Vektor se slo�kami (xmin, xmax, ymin, ymax)</returns>
        public Vector GetMinMax() {
            int count = this.Count;

            double minX = 0.0, maxX = 0.0, minY = 0.0, maxY = 0.0;

            if(this.IsVector) {
                Array a = this.item as Array;

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
                Array a = this.item as Array;

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
        /// Vytvo�� �adu bod� k vykreslen�
        /// </summary>
        /// <param name="index">Index vektoru</param>
        /// <param name="offsetX">Posunut� vhledem k ose X</param>
        /// <param name="amplifyX">Zes�len� v ose X</param>
        /// <param name="offsetY">Posunut� vhledem k ose Y</param>
        /// <param name="amplifyY">Zes�len� v ose Y</param>
        /// <param name="maxLength">Maxim�ln� d�lka dat</param>
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
        /// Vytvo�� �adu bod� k vykreslen�
        /// </summary>
        /// <param name="index">Index vektoru</param>
        /// <param name="offsetX">Posunut� vhledem k ose X</param>
        /// <param name="amplifyX">Zes�len� v ose X</param>
        /// <param name="offsetY">Posunut� vhledem k ose Y</param>
        /// <param name="amplifyY">Zes�len� v ose Y</param>
        public Point[] PointArrayToDraw(int index, double offsetX, double amplifyX, double offsetY, double amplifyY) {
            return this.PointArrayToDraw(index, offsetX, amplifyX, offsetY, amplifyY, -1);
        }

        /// <summary>
        /// Vytvo�� po��te�n� a koncov� bod pro chybovou �se�ku
        /// </summary>
        /// <param name="index">Index vektoru</param>
        /// <param name="offsetX">Posunut� vhledem k ose X</param>
        /// <param name="amplifyX">Zes�len� v ose X</param>
        /// <param name="offsetY">Posunut� vhledem k ose Y</param>
        /// <param name="amplifyY">Zes�len� v ose Y</param>
        /// <returns>�adu bod� - po��tek a konec chybov� �se�ky</returns>
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
        /// Ulo�� obsah do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public virtual void Export(Export export) {
            export.Write(this.item);
            export.Write(this.graphContext);

            export.Write(this.errors);

            if(export.Binary)
                export.B.Write(this.itemContext.Length);
            else
                export.T.WriteLine(this.itemContext.Length);

            for(int i = 0; i < this.itemContext.Length; i++)
                export.Write(this.itemContext[i]);
        }

        /// <summary>
        /// Na�te obsah ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public virtual void Import(PavelStransky.Math.Import import) {
            this.item = import.Read() as Array;
            this.graphContext = import.Read() as Context;

            this.errors = import.Read() as Array;

            int count = 0;
            if(import.Binary)
                count = import.B.ReadInt32();
            else
                count = Int32.Parse(import.T.ReadLine());

            this.itemContext = new Context[count];
            for(int i = 0; i < count; i++)
                this.itemContext[i] = import.Read() as Context;
        }
        #endregion

		// Chyby
		private const string errorMessageBadType = "Parametr {0} grafu m� �patn� typ.";
		private const string errorMessageBadTypeDetail = "Po�adovan� typ: {0}\nZadan� typ: {1}\nZadan� hodnota: {2}";

        private const string errorMessageBadErrorType = "Zadan� chyby maj� �patn� typ.";
        private const string errorMessageBadErrorTypeDetail = "Po�adovan� typ: Array of Vectors | Vector\nZadan� typ: {1}";

        private const string errorMessageBadLength = "Chybn� d�lka vektoru s chybov�mi �se�kami.";
        private const string errorMessageBadLengthDetail = "Index vektoru s daty: {0}\nD�lka vektoru: {1}\nD�lka vektoru s �se�kami: {2}";
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
/*
		// Parametry grafu
		private const string paramTitle = "title";
		private const string paramShow = "show";

		// Default hodnoty
		private const bool defaultShow = true;

        // Parametry grafu
        private const string paramTitle = "title";
        private const string paramShift = "shift";
        private const string paramLineColor = "lcolor";
        private const string paramLineStyle = "lstyle";
        private const string paramLineWidth = "lwidth";
        private const string paramPointColor = "pcolor";
        private const string paramPointStyle = "pstyle";
        private const string paramPointSize = "psize";
        private const string paramShowLabel = "showlabel";
        private const string paramLabelColor = "labelcolor";

        private const string paramMinX = "minx";
        private const string paramMaxX = "maxx";
        private const string paramMinY = "miny";
        private const string paramMaxY = "maxy";

        // Osy
        private const string paramTitleX = "titlex";
        private const string paramLineColorX = "lcolorx";
        private const string paramLineWidthX = "lwidthx";
        private const string paramPointColorX = "pcolorx";
        private const string paramPointStyleX = "pstylex";
        private const string paramPointSizeX = "psizex";
        private const string paramShowLabelX = "showlabelx";
        private const string paramLabelColorX = "labelcolorx";
        private const string paramShowAxeX = "showaxex";

        private const string paramTitleY = "titley";
        private const string paramLineColorY = "lcolory";
        private const string paramLineWidthY = "lwidthy";
        private const string paramPointColorY = "pcolory";
        private const string paramPointStyleY = "pstyley";
        private const string paramPointSizeY = "psizey";
        private const string paramShowLabelY = "showlabely";
        private const string paramLabelColorY = "labelcolory";
        private const string paramShowAxeY = "showaxey";

        // Default hodnoty
        private const bool defaultShift = false;
        private const string defaultLineColor = "blue";
        private const string defaultLineStyle = "line";
        private const float defaultLineWidth = 1.0F;
        private const string defaultPointColor = "brown";
        private const string defaultPointStyle = "circle";
        private const int defaultPointSize = 2;
        private const bool defaultShowLabel = true;
        private const string defaultLabelColor = "black";

        private const double defaultMinX = double.NaN;
        private const double defaultMaxX = defaultMinX;
        private const double defaultMinY = defaultMinX;
        private const double defaultMaxY = defaultMinX;

        private const string defaultTitleX = "X";
        private const string defaultLineColorX = "red";
        private const float defaultLineWidthX = 1.0F;
        private const string defaultPointColorX = "red";
        private const string defaultPointStyleX = "vlines";
        private const int defaultPointSizeX = 5;
        private const bool defaultShowLabelX = false;
        private const string defaultLabelColorX = defaultLineColorX;
        private const bool defaultShowAxeX = true;

        private const string defaultTitleY = "Y";
        private const string defaultLineColorY = "red";
        private const float defaultLineWidthY = 1.0F;
        private const string defaultPointColorY = "red";
        private const string defaultPointStyleY = "hlines";
        private const int defaultPointSizeY = 5;
        private const bool defaultShowLabelY = false;
        private const string defaultLabelColorY = defaultLineColorY;
        private const bool defaultShowAxeY = true;

         private const string paramShowLabels = "showlabels";
        private const bool defaultShowLabels = true;*/