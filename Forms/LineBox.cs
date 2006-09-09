using System;
using System.Windows.Forms;
using System.Drawing;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Forms {
	/// <summary>
	/// Summary description for DensityPanel.
	/// </summary>
	public class LineBox: System.Windows.Forms.PictureBox {
		// Øada s daty k vykreslení
		private Expression.Array array = null;
		// Chyby dat
		private Expression.Array errors = null;
		private bool showErrors = true;

		// Zesílení køivek v ose Y
		private double amplifyY = baseAmplifyY;

		// Vlastnosti èar grafu
		private GraphProperty [] graphsProperty;
		// Vlastnosti os grafu
		private AxeProperty [] axesProperty;

		// Posun jednotlivých èar grafu vùèi sobì
		private bool shift;

		// Minimální a maximální hodnota
		private bool autoMinMax = true;
		private double minX, maxX, minY, maxY;

		// Velikosti okrajù (aby graf nepøelézal a nedotýkal se)
		private int marginL = defaultMargin;
		private int marginR = defaultMargin;
		private int marginT = defaultMargin;
		private int marginB = defaultMargin;

		/// <summary>
		/// Základní konstruktor
		/// </summary>
		public LineBox() : base() {}

		/// <summary>
		/// Nastaví øadu vektorù k zobrazení
		/// </summary>
		/// <param name="array">Øada vektorù</param>
		/// <param name="errors">Chyby k vektorùm</param>
		public void SetData(Expression.Array array, Expression.Array errors) {
			this.array = array;
			this.errors = errors;

			if(this.errors == null) {
				this.showErrors = false;
				this.errors = new Expression.Array(false);  // Nechceme kontrolovat velikost

				int count = this.array.Count;
				for(int i = 0; i < count; i++)
					this.errors.Add(new Vector(this.GetLength(i)));
			}
			else
				this.showErrors = true;

			if(this.autoMinMax)
				this.SetMinMax();

			this.Invalidate();
		}

		/// <summary>
		/// Nastaví vlastnosti
		/// </summary>
		/// <param name="graphsProperty">Vlastnosti èar grafu</param>
		/// <param name="axesProperty">Vlastnosti os grafu</param>
		public void SetProperties(GraphProperty [] graphsProperty, AxeProperty [] axesProperty) {
			this.graphsProperty = graphsProperty;
			this.axesProperty = axesProperty;
			
			if(this.axesProperty[0].ShowAxe)
				this.marginB = defaultMarginWithAxeB;
			else
				this.marginB = defaultMargin;

			if(this.axesProperty[1].ShowAxe)
				this.marginL = defaultMarginWithAxeL;
			else
				this.marginL = defaultMargin;

			this.Invalidate();
		}

		/// <summary>
		/// Posun jednotlivých èar grafu vùèi sobì
		/// </summary>
		public bool Shift {get {return this.shift;} set {this.shift = value; this.Invalidate();}}

		/// <summary>
		/// Nastaví minimální a maximální hodnotu
		/// </summary>
		private void SetMinMax() {
			if(this.array != null && this.array.Count != 0) {
				if(this.array[0] is Vector) {
					Vector v = this.array[0] as Vector;
					Vector e = this.errors[0] as Vector;
					this.minY = (v - e).Min();
					this.maxY = (v + e).Max();

					for(int i = 1; i < this.array.Count; i++) {
						v = this.array[i] as Vector;
						e = this.errors[i] as Vector;
						this.minY = System.Math.Min(this.minY, (v - e).Min());
						this.maxY = System.Math.Max(this.maxY, (v + e).Max());
					}

					this.minX = 0;
					this.maxX = (this.array[0] as Vector).Length;
				}
				else if(this.array[0] is PointVector) {
					PointVector pv = this.array[0] as PointVector;
					this.minX = pv.MinX();
					this.maxX = pv.MaxX();

					Vector vy = pv.VectorY;
					Vector ey = this.errors[0] as Vector;
					this.minY = (vy - ey).Min();
					this.maxY = (vy + ey).Max();

					for(int i = 1; i < this.array.Count; i++) {
						pv = this.array[i] as PointVector;
						this.minX = System.Math.Min(this.minX, pv.MinX());
						this.maxX = System.Math.Max(this.maxX, pv.MaxX());

						vy = pv.VectorY;
						ey = this.errors[i] as Vector;
						this.minY = System.Math.Min(this.minY, (vy - ey).Min());
						this.maxY = System.Math.Max(this.maxY, (vy - ey).Max());
					}
				}
			}
		}

		/// <summary>
		/// Nastaví minimální a maximální hodnotu (pokud je NaN, nastavuje se automaticky)
		/// </summary>
		/// <param name="minX">Minimální hodnota X</param>
		/// <param name="minY">Minimální hodnota Y</param>
		/// <param name="maxX">Maximální hodnota X</param>
		/// <param name="maxY">Maximální hodnota Y</param>
		public void SetMinMax(double minX, double maxX, double minY, double maxY) {
			this.SetMinMax();

			if(!double.IsNaN(minX))
				this.minX = minX;
			if(!double.IsNaN(maxX))
				this.maxX = maxX;
			if(!double.IsNaN(minY))
				this.minY = minY;
			if(!double.IsNaN(maxY))
				this.maxY = maxY;

			if(double.IsNaN(minX) && double.IsNaN(maxX) && double.IsNaN(minY) && double.IsNaN(maxY))
				this.autoMinMax = true;
			else
				this.autoMinMax = false;
		}

		/// <summary>
		/// Vrací délku i-té køivky k vykreslení
		/// </summary>
        private int GetLength(int i) {
            if(this.array.Count <= i)
                return 0;
            else if(this.array.ItemType == typeof(Vector))
                return (this.array[i] as Vector).Length;
            else if(this.array.ItemType == typeof(PointVector))
                return (this.array[i] as PointVector).Length;
            else
                return 0;
        }

		/// <summary>
		/// Vypoèítá offset takový, aby se celý graf právì vešel do okna
		/// </summary>
		private double GetFitOffsetY() {
			return this.maxY * (this.Height - this.marginT - this.marginB) / (this.maxY - this.minY) + this.marginT;
		}

		/// <summary>
		/// Vypoèítá offset takový, aby se celý graf právì vešel do okna
		/// </summary>
		private double GetFitOffsetX() {
			return -this.minX * (this.Width - this.marginL - this.marginR) / (this.maxX - this.minX) + this.marginL;
		}

		/// <summary>
		/// Vypoèítá zvìtšení takové, aby se celý graf právì vešel do okna
		/// </summary>
		private double GetFitAmplifyY() {
			return (this.Height - this.marginT - this.marginB) / (this.maxY - this.minY);
		}

		/// <summary>
		/// Vypoèítá zvìtšení takové, aby se celý graf právì vešel do okna
		/// </summary>
		private double GetFitAmplifyX() {
			return (this.Width -  this.marginL - this.marginR) / (this.maxX - this.minX);
		}

		/// <summary>
		/// Vytvoøí øadu bodù k vykreslení
		/// </summary>
		/// <param name="index">Index vektoru</param>
		/// <param name="offsetY">Posunutí vhledem k ose Y</param>
		/// <param name="amplifyY">Zesílení v ose Y</param>
		private Point[] PointArrayToDraw(int index, int offsetY, double amplifyY) {
			Point [] retValue = null;

			if(this.array != null && this.array.Count > index) {
				double offsetX = this.GetFitOffsetX();
				double amplifyX = this.GetFitAmplifyX();

				if(this.array[index] is Vector) {
					Vector v = this.array[index] as Vector;
					int length = v.Length;
					retValue = new Point[length];

					for(int i = 0; i < length; i++)
						retValue[i] = new Point((int)(i * amplifyX + offsetX), (int)(-v[i] * amplifyY + offsetY));
				}
				else if(this.array[index] is PointVector) {
					PointVector pv = this.array[index] as PointVector;
					int length = pv.Length;
					retValue = new Point[length];

					for(int i = 0; i < length; i++)
						retValue[i] = new Point((int)(pv[i].X * amplifyX + offsetX), (int)(-pv[i].Y * amplifyY + offsetY));
				}
			}

			return retValue;
		}

		/// <summary>
		/// Vytvoøí poèáteèní a koncový bod pro chybovou úseèku
		/// </summary>
		/// <param name="index">Index vektoru</param>
		/// <param name="offsetY">Posunutí vhledem k ose Y</param>
		/// <param name="amplifyY">Zesílení v ose Y</param>
		/// <returns>Øadu bodù - poèátek a konec chybové úseèky</returns>
		private Point[] GetErrorLines(int index, int offsetY, double amplifyY) {
			Point [] result = null;

			if(this.array != null && this.array.Count > index) {
				double offsetX = this.GetFitOffsetX();
				double amplifyX = this.GetFitAmplifyX();

				if(this.array[index] is Vector) {
					Vector v = this.array[index] as Vector;
					Vector e = this.errors[index] as Vector;

					int length = v.Length;
					result = new Point[2 * length];

					for(int i = 0; i < length; i++) {
						int x = (int)(i * amplifyX + offsetX);
						result[2 * i] = new Point(x, (int)(-(v[i] + e[i]) * amplifyY + offsetY));
						result[2 * i + 1] = new Point(x, (int)(-(v[i] - e[i]) * amplifyY + offsetY));
					}
				}
				else if(this.array[index] is PointVector) {
					PointVector pv = this.array[index] as PointVector;
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

		/// <summary>
		/// Vykreslení
		/// </summary>
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint (e);

			if(this.array != null) {
				Graphics g = e.Graphics;
				int stringHeight = (int)g.MeasureString("M", baseFont).Height;
				int offsetY;
				double amplifyY;

				if(!this.shift) {
					offsetY = (int)this.GetFitOffsetY();
					amplifyY = this.GetFitAmplifyY();
				}
				else {
					offsetY = 0;
					amplifyY = this.amplifyY;
				}

				for(int i = 0; i < this.array.Count; i++) {
					if(this.shift)
						offsetY = (i + 1) * (this.Height - this.marginT - this.marginB) / (this.array.Count + 2) + this.marginT;

					Point [] p = this.PointArrayToDraw(i, offsetY, amplifyY);
					Pen linePen = new Pen(this.graphsProperty[i].LineColor, this.graphsProperty[i].LineWidth);
                    Pen pointPen = new Pen(this.graphsProperty[i].PointColor);
                    
                    this.DrawLine(g, p, linePen, this.graphsProperty[i].LineStyle);

					if(this.showErrors) {
						Point [] errorPoints = this.GetErrorLines(i, offsetY, amplifyY);
						Point [] ep = new Point[2];
						int length = errorPoints.Length / 2;

						for(int j = 0; j < length; j++) {
							ep[0] = errorPoints[2 * j];
							ep[1] = errorPoints[2 * j + 1];
							this.DrawLine(g, ep, pointPen, Expression.GraphProperty.LineStyles.Line);
						}

						this.DrawPoints(g, errorPoints, pointPen, Expression.GraphProperty.PointStyles.HLines, 5);
					}

					if(this.graphsProperty[i].ShowLabel && this.graphsProperty[i].Name != string.Empty) {
						Brush labelBrush = (new Pen(this.graphsProperty[i].LabelColor)).Brush;
						g.DrawString(this.graphsProperty[i].Name, baseFont, labelBrush, this.marginL, offsetY - stringHeight / 2); 
					}

					this.DrawPoints(g, p, pointPen, this.graphsProperty[i].PointStyle, this.graphsProperty[i].PointSize);
				}

				// X - ová osa
				if(this.axesProperty[0].ShowAxe) {
					int y = this.Height - this.marginB;

					Point [] line = new Point[2];
					line[0] = new Point(this.marginL, y);
					line[1] = new Point(this.Width - this.marginR, y);
					
					Pen linePen = new Pen(this.axesProperty[0].LineColor, this.axesProperty[0].LineWidth);
					this.DrawLine(g, line, linePen, this.axesProperty[0].LineStyle);

					int smallIntervals;
					int smallIntervalsOffset;
					Vector v = this.GetAxesPoints(out smallIntervals, out smallIntervalsOffset, this.Width - this.marginL - this.marginR, this.minX, this.maxX);

					double offsetX = this.GetFitOffsetX();
					double amplifyX = this.GetFitAmplifyX();

					Brush numBrush = linePen.Brush;
					Point [] p = new Point[v.Length];
					for(int i = 0; i < v.Length; i++) {
						p[i] = new Point((int)(v[i] * amplifyX + offsetX), y);
						if(((i - smallIntervalsOffset) % smallIntervals) != 0)
							continue;

						string numString = v[i].ToString();
						float nsWidth = g.MeasureString(numString, baseFont).Width;
						g.DrawString(numString, baseFont, numBrush, p[i].X - (int)(nsWidth / 2), y + 5); 
					}

					Pen pointPen = new Pen(this.axesProperty[0].PointColor);
					this.DrawPoints(g, p, pointPen, this.axesProperty[0].PointStyle, this.axesProperty[0].PointSize, smallIntervals, smallIntervalsOffset, this.axesProperty[0].PointStyle, this.axesProperty[0].PointSize / 2);
				}

				// Y - ová osa
				if(this.axesProperty[1].ShowAxe && !this.shift) {
					int x = this.marginL;

					Point [] line = new Point[2];
					line[0] = new Point(x, this.marginT);
					line[1] = new Point(x, this.Height - this.marginB);
					
					Pen linePen = new Pen(this.axesProperty[1].LineColor, this.axesProperty[1].LineWidth);
					this.DrawLine(g, line, linePen, this.axesProperty[1].LineStyle);

					int smallIntervals;
					int smallIntervalsOffset;
					Vector v = this.GetAxesPoints(out smallIntervals, out smallIntervalsOffset, this.Height - this.marginT - this.marginB, this.minY, this.maxY);

					offsetY = (int)this.GetFitOffsetY();
					
					Brush numBrush = linePen.Brush;
					Point [] p = new Point[v.Length];
					for(int i = 0; i < v.Length; i++) {
						p[i] = new Point(x, (int)(-v[i] * amplifyY + offsetY));
						if(((i - smallIntervalsOffset) % smallIntervals) != 0)
							continue;

						string numString = v[i].ToString();
						SizeF nsSize = g.MeasureString(numString, baseFont);
						g.DrawString(numString, baseFont, numBrush, x - nsSize.Width - 5, p[i].Y - nsSize.Height / 2);
					}

					Pen pointPen = new Pen(this.axesProperty[1].PointColor);
					this.DrawPoints(g, p, pointPen, this.axesProperty[1].PointStyle, this.axesProperty[1].PointSize, smallIntervals, smallIntervalsOffset, this.axesProperty[1].PointStyle, this.axesProperty[1].PointSize / 2);
				}
			}
		}

		/// <summary>
		/// Vytvoøí body, ve kterých bude label na ose
		/// </summary>
		/// <param name="pixels">Poèet bodù na obrazovce</param>
		/// <param name="min">Minimální hodnota</param>
		/// <param name="max">Maximální hodnota</param>
		private Vector GetAxesPoints(out int smallIntervals, out int smallIntervalsOffset, int pixels, double min, double max) {
			double wholeInterval = max - min;

			int numPoints = pixels / axePointInterval;
			double logInterval = System.Math.Log10(wholeInterval / numPoints);
			double intervalOrder = System.Math.Floor(logInterval);
			logInterval = logInterval - intervalOrder;

			double largeInterval = System.Math.Pow(10.0, intervalOrder + 1);
			smallIntervals = 1;

			if(logInterval < System.Math.Log10(2.0)) {
				largeInterval /= 5.0;
				smallIntervals = 2;
			}
			else if(logInterval < System.Math.Log10(5.0)) {
				largeInterval /= 2.0;
				smallIntervals = 5;
			}
			else
				smallIntervals = 2;

			double x = min;
			double interval = largeInterval / smallIntervals;
			x /= interval;
			x = System.Math.Floor(x) + 1;
			x *= interval;

			Vector result = new Vector((int)(wholeInterval / interval) + 1);

			for(int i = 1; i < result.Length; i++)
				result[i] = x + (i - 1) * interval;

			smallIntervalsOffset = (int)(((System.Math.Floor(x / largeInterval) + 1) * largeInterval - x) / interval) + 1;
			return result;
		}

		/// <summary>
		/// Nakreslí èáru
		/// </summary>
		/// <param name="g">Object Graphics</param>
		/// <param name="points">Body pro vykreslení èáry</param>
		/// <param name="pen">Pero</param>
		/// <param name="lineStyle">Styl èáry</param>
		private void DrawLine(Graphics g, Point [] points, Pen pen, GraphProperty.LineStyles lineStyle) {
			switch(lineStyle) {
				case GraphProperty.LineStyles.Curve:
					g.DrawCurve(pen, points);
					break;
				case GraphProperty.LineStyles.Line:
					g.DrawLines(pen, points);
					break;
			}
		}

		/// <summary>
		/// Nakreslí body (bez význaèných bodù)
		/// </summary>
		/// <param name="g">Objekt Graphics</param>
		/// <param name="points">Body</param>
		/// <param name="pen">Pero</param>
		/// <param name="pointStyle">Styl bodù</param>
		/// <param name="pointSize">Velikost bodù</param>
		private void DrawPoints(Graphics g, Point [] points, Pen pen, GraphProperty.PointStyles pointStyle, int pointSize) {
			this.DrawPoints(g, points, pen, pointStyle, pointSize, 1, 0, pointStyle, pointSize);
		}

		/// <summary>
		/// Nakreslí body (s význaènými body)
		/// </summary>
		/// <param name="g">Objekt Graphics</param>
		/// <param name="points">Body</param>
		/// <param name="pen">Pero</param>
		/// <param name="pointStyle">Styl bodù</param>
		/// <param name="pointSize">Velikost bodù</param>
		/// <param name="hlStep">Krok pro význaèné body</param>
		/// <param name="hlOffset">Kolikátý krok zaèíná význaèný bod</param>
		/// <param name="hlPointStyle">Styl význaèných bodù</param>
		/// <param name="hlPointSize">Velikost význaèných bodù</param>
		private void DrawPoints(Graphics g, Point [] points, Pen pen, GraphProperty.PointStyles pointStyle, int pointSize, 
			int hlStep, int hlOffset, GraphProperty.PointStyles hlPointStyle, int hlPointSize) {

			for(int j = 0; j < points.Length; j++) {
				if(((j - hlOffset) % hlStep) != 0)
					this.DrawPoint(g, points[j], pen, hlPointStyle, hlPointSize);
				else
					this.DrawPoint(g, points[j], pen, pointStyle, pointSize);
			}
		}

		/// <summary>
		/// Nakreslí bod
		/// </summary>
		/// <param name="g">Objekt Graphics</param>
		/// <param name="point">Body</param>
		/// <param name="pen">Pero</param>
		/// <param name="pointStyle">Styl bodu</param>
		/// <param name="pointSize">Velikost bodu</param>
		private void DrawPoint(Graphics g, Point point, Pen pen, GraphProperty.PointStyles pointStyle, int pointSize) {
			int pointSized2 = pointSize / 2;

			switch(pointStyle) {
				case GraphProperty.PointStyles.Circle:
					g.DrawEllipse(pen, point.X - pointSized2, point.Y - pointSized2, pointSize, pointSize);
					break;
				case GraphProperty.PointStyles.FCircle:
					g.FillEllipse(pen.Brush, point.X - pointSized2, point.Y - pointSized2, pointSize, pointSize);
					break;
				case GraphProperty.PointStyles.Square:
					g.DrawRectangle(pen, point.X - pointSized2, point.Y - pointSized2, pointSize, pointSize);
					break;
				case GraphProperty.PointStyles.FSquare:
					g.FillRectangle(pen.Brush, point.X - pointSized2, point.Y - pointSized2, pointSize, pointSize);
					break;
				case GraphProperty.PointStyles.VLines:
					g.DrawLine(pen, point.X, point.Y - pointSized2, point.X, point.Y + pointSize - pointSized2);
					break;
				case GraphProperty.PointStyles.HLines:
					g.DrawLine(pen, point.X - pointSized2, point.Y, point.X + pointSize - pointSized2, point.Y);
					break;
			}
		}

		/// <summary>
		/// Pøi zmìnì velikosti
		/// </summary>
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged (e);
			this.Invalidate();
		}

		private Font baseFont = new Font("Arial", 8);
		private const double baseAmplifyY = 8;
		private const int defaultMargin = 8;
		private const int defaultMarginWithAxeL = 40;
		private const int defaultMarginWithAxeB = 20;
		// Interval mezi dvìma body s èísly na ose (v obrazových bodech)
		private const int axePointInterval = 50;
	}
}