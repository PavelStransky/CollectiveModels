using System;
using System.Drawing;

namespace PavelStransky.Expression {
	/// <summary>
	/// Tøída, která zapouzdøuje graf (urèuje jeho obsah, popø. další parametry)
	/// </summary>
	public class Graph {
		protected object item;

		// Kontext a výraz celého grafu
		protected Context graphContext;
		private Expression graphExpression;

		/// <summary>
		/// Prvek grafu
		/// </summary>
		public object Item {get {return this.item;}}

		#region Parametry
		/// <summary>
		/// Titulek grafu
		/// </summary>
		public string Title {
			get {
				return this.GetParameter(this.graphContext, paramTitle, typeof(string), string.Empty) as string;
			}
		}

		/// <summary>
		/// Zobrazit èi nezobrazit samostatný graf (v grapharray se zobrazuje vždy)
		/// </summary>
		public bool Show {
			get {
				return (bool)this.GetParameter(this.graphContext, paramShow, typeof(bool), defaultShow);
			}
		}
		#endregion

		/// <summary>
		/// Vrací hodnotu parametru a kontroluje, jestli má zadaný parametr správný typ
		/// </summary>
		/// <param name="name">Název parametru</param>
		/// <param name="context">Kontext, na kterém se hledá</param>
		/// <param name="type">Typ parametru</param>
		/// <param name="def">Default hodnota</param>
		/// <returns>Hodnota parametru, null, pokud parametr není zadán</returns>
		protected object GetParameter(Context context, string name, Type type, object def) {
			if(context.Contains(name)) {
				object item = context[name].Item;

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

				if(type == typeof(float)) {
					if(item is double)
						item = (float)(double)item;
					else if(item is int)
						item = (float)(int)item;
				}
                else if(type == typeof(double)) {
                    if(item is int)
                        item = (double)(int)item;
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
		/// Konstruktor
		/// </summary>
		/// <param name="item">Data pro graf</param>
		/// <param name="graphParams">Parametry celého grafu</param>
		public Graph(object item, string graphParams) {
			this.item = item;

			this.graphContext = new Context();
			if(graphParams != null && graphParams != string.Empty)
				this.graphExpression = new Expression(this.graphContext, graphParams);
			else
				this.graphExpression = null;

			this.Evaluate();
		}

		/// <summary>
		/// Provede výpoèet všech výrazù v parametrech
		/// </summary>
		public virtual void Evaluate() {
			if(this.graphExpression != null)
				this.graphExpression.Evaluate();
		}

		// Parametry grafu
		private const string paramTitle = "title";
		private const string paramShow = "show";

		// Default hodnoty
		private const bool defaultShow = true;

		// Chyby
		private const string errorMessageBadType = "Parametr {0} grafu má špatný typ.";
		private const string errorMessageBadTypeDetail = "Požadovaný typ: {0}\nZadaný typ: {1}\nZadaná hodnota: {2}";
	}

	/// <summary>
	/// Tøída, která zapouzdøuje èárový graf
	/// </summary>
	public class LineGraph : Graph {
		private object errors;

		// Kontexty a výrazy jednotlivých køivek
		private Context [] itemContext;
		private Expression [] itemExpression;

		/// <summary>
		/// Chyby grafu
		/// </summary>
		public object Errors {get {return this.errors;}}

		/// <summary>
		/// Poèet køivek grafu
		/// </summary>
		public int Count {
			get {
				if(this.item is Array)
					return (this.item as Array).Count;
				else if(this.item != null)
					return 1;
				else
					return 0;
			}
		}

		#region Parametry
		/// <summary>
		/// Posun více èástí grafu vùèi sobì
		/// </summary>
		public bool Shift {
			get {
				return (bool)this.GetParameter(this.graphContext, paramShift, typeof(bool), defaultShift);
			}
		}

		/// <summary>
		/// Minimální hodnota X
		/// </summary>
		public double MinX {
			get {
				return (double)this.GetParameter(this.graphContext, paramMinX, typeof(double), defaultMinX);
			}
		}

		/// <summary>
		/// Maximální hodnota X
		/// </summary>
		public double MaxX {
			get {
				return (double)this.GetParameter(this.graphContext, paramMaxX, typeof(double), defaultMaxX);
			}
		}

		/// <summary>
		/// Minimální hodnota Y
		/// </summary>
		public double MinY {
			get {
				return (double)this.GetParameter(this.graphContext, paramMinY, typeof(double), defaultMinY);
			}
		}

		/// <summary>
		/// Maximální hodnota Y
		/// </summary>
		public double MaxY {
			get {
				return (double)this.GetParameter(this.graphContext, paramMaxY, typeof(double), defaultMaxY);
			}
		}

		/// <summary>
		/// Vlastnosti jednotlivých èar grafu
		/// </summary>
		public GraphProperty [] GraphsProperty() {
			int count = this.Count;
			GraphProperty [] gp = new GraphProperty[count];

            for(int i = 0; i < count; i++) 
                gp[i] = new GraphProperty(
                    (string)this.GetParameter(this.itemContext[i], paramTitle, typeof(string), string.Empty),
                    (string)this.GetParameter(this.itemContext[i], paramLineStyle, typeof(string), defaultLineStyle),
                    (Color)this.GetParameter(this.itemContext[i], paramLineColor, typeof(Color), Color.FromName(defaultLineColor)),
                    (float)this.GetParameter(this.itemContext[i], paramLineWidth, typeof(float), defaultLineWidth),
                    (string)this.GetParameter(this.itemContext[i], paramPointStyle, typeof(string), defaultPointStyle),
                    (Color)this.GetParameter(this.itemContext[i], paramPointColor, typeof(Color), Color.FromName(defaultPointColor)),
                    (int)this.GetParameter(this.itemContext[i], paramPointSize, typeof(int), defaultPointSize),
                    (bool)this.GetParameter(this.itemContext[i], paramShowLabel, typeof(bool), defaultShowLabel),
                    (Color)this.GetParameter(this.itemContext[i], paramLabelColor, typeof(Color), Color.FromName(defaultLabelColor)));

			return gp;
		}

		/// <summary>
		/// Vlastnosti os grafu
		/// </summary>
		public AxeProperty [] AxesProperty() {
			AxeProperty [] ap = new AxeProperty[2];

			// Osa x
			ap[0] = new AxeProperty(
				(string)this.GetParameter(this.graphContext, paramTitleX, typeof(string), defaultTitleX),
				defaultLineStyle,
				(Color)this.GetParameter(this.graphContext, paramLineColorX, typeof(Color), Color.FromName(defaultLineColorX)),
				(float)this.GetParameter(this.graphContext, paramLineWidthX, typeof(float), defaultLineWidthX),
				(string)this.GetParameter(this.graphContext, paramPointStyleX, typeof(string), defaultPointStyleX),
				(Color)this.GetParameter(this.graphContext, paramPointColorX, typeof(Color), Color.FromName(defaultPointColorX)),
				(int)this.GetParameter(this.graphContext, paramPointSizeX, typeof(int), defaultPointSizeX),
				(bool)this.GetParameter(this.graphContext, paramShowLabelX, typeof(bool), defaultShowLabelX),
				(Color)this.GetParameter(this.graphContext, paramLabelColorX, typeof(Color), Color.FromName(defaultLabelColorX)),
				(bool)this.GetParameter(this.graphContext, paramShowAxeX, typeof(bool), defaultShowAxeX));

			// Osa y
			ap[1] = new AxeProperty(
				(string)this.GetParameter(this.graphContext, paramTitleY, typeof(string), defaultTitleY),
				defaultLineStyle,
				(Color)this.GetParameter(this.graphContext, paramLineColorY, typeof(Color), Color.FromName(defaultLineColorY)),
				(float)this.GetParameter(this.graphContext, paramLineWidthY, typeof(float), defaultLineWidthY),
				(string)this.GetParameter(this.graphContext, paramPointStyleY, typeof(string), defaultPointStyleY),
				(Color)this.GetParameter(this.graphContext, paramPointColorY, typeof(Color), Color.FromName(defaultPointColorY)),
				(int)this.GetParameter(this.graphContext, paramPointSizeY, typeof(int), defaultPointSizeY),
				(bool)this.GetParameter(this.graphContext, paramShowLabelY, typeof(bool), defaultShowLabelY),
				(Color)this.GetParameter(this.graphContext, paramLabelColorY, typeof(Color), Color.FromName(defaultLabelColorY)),
				(bool)this.GetParameter(this.graphContext, paramShowAxeY, typeof(bool), defaultShowAxeY));

			return ap;
		}
		#endregion

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="item">Data pro graf</param>
		/// <param name="errors">Chyby køivek grafu</param>
		/// <param name="graphParams">Parametry celého grafu</param>
		/// <param name="itemParams">Parametry jednotlivých køivek grafu</param>
		/// <param name="labelsX">Popisky osy X</param>
		/// <param name="labelsY">Popisky osy Y</param>
		public LineGraph(object item, object errors, string graphParams, Array itemParams) : base(item, graphParams) {
			this.errors = errors;

			int count = this.Count;

			this.itemContext = new Context[count];
			this.itemExpression = new Expression[count];

			for(int i = 0; i < count; i++) {
				this.itemContext[i] = new Context();
				if(itemParams != null && itemParams.Count > i && itemParams[i] != null && (itemParams[i] as string) != string.Empty)
					this.itemExpression[i] = new Expression(this.itemContext[i], itemParams[i] as string);
			}

			this.Evaluate();
		}

		/// <summary>
		/// Provede výpoèet všech výrazù v parametrech
		/// </summary>
		public override void Evaluate() {
			base.Evaluate();

			if(this.itemExpression == null)
				return;

			int count = this.Count;

			for(int i = 0; i < count; i++)
				if(this.itemExpression[i] != null)
					this.itemExpression[i].Evaluate();
		}

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
	}

	/// <summary>
	/// Tøída, která zapouzdøuje graf hustot
	/// </summary>
	public class DensityGraph: Graph {
		private Array labelsX;
		private Array labelsY;

		/// <summary>
		/// Zobrazit èi nezobrazit popisky (vždy se zobrazují v tooltip)
		/// </summary>
		public bool ShowLabels {
			get {
				return (bool)this.GetParameter(this.graphContext, paramShowLabels, typeof(bool), defaultShowLabels);
			}
		}

		/// <summary>
		/// Popisky osy X (pro DensityGraph)
		/// </summary>
		public Array LabelsX {get {return this.labelsX;}}

		/// <summary>
		/// Popisky osy Y (pro DensityGraph)
		/// </summary>
		public Array LabelsY {get {return this.labelsY;}}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="item">Data pro graf</param>
		/// <param name="graphParams">Parametry celého grafu</param>
		/// <param name="labelsX">Popisky osy X</param>
		/// <param name="labelsY">Popisky osy Y</param>
		public DensityGraph(object item, string graphParams, Array labelsX, Array labelsY) : base(item, graphParams) {
			this.labelsX = labelsX;
			this.labelsY = labelsY;
		}

		private const string paramShowLabels = "showlabels";
		private const bool defaultShowLabels = true;
	}
}
