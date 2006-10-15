using System;
using System.Windows.Forms;
using System.Drawing;
using System.Timers;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Forms {
	/// <summary>
	/// Summary description for DensityPanel.
	/// </summary>
	public class LineBox: System.Windows.Forms.PictureBox, IGraphControl {
		// Data k vykreslen�
        private Graph graph;
        private System.Timers.Timer timer = new System.Timers.Timer();
        private int time, maxTime;

		// Zes�len� k�ivek v ose Y
		private double amplifyY = baseAmplifyY;

		// Minim�ln� a maxim�ln� hodnota
		private double minX, maxX, minY, maxY;

		// Velikosti okraj� (aby graf nep�el�zal a nedot�kal se)
		private int marginL = 3 * defaultMargin;
		private int marginR = defaultMargin;
		private int marginT = defaultMargin;
		private int marginB = 3 * defaultMargin;

		/// <summary>
		/// Z�kladn� konstruktor
		/// </summary>
		public LineBox() : base() {}

        /// <param name="graph">Objekt grafu</param>
        public LineBox(Graph graph)
            : this() {
            this.SetGraph(graph);
        }

        /// <summary>
        /// Vr�t� objekt s daty
        /// </summary>
        public Graph GetGraph() {
            return this.graph;
        }

		/// <summary>
		/// Nastav� �adu vektor� k zobrazen�
		/// </summary>
		/// <param name="graph">Objekt grafu</param>
		public void SetGraph(Graph graph) {
			this.graph = graph;

            this.marginL = (bool)this.graph.GetGeneralParameter(paramShowAxeY, defaultShowAxeY) ? defaultMarginWithAxeL : defaultMargin;
            this.marginR = defaultMargin;
            this.marginT = defaultMargin;
            this.marginB = (bool)this.graph.GetGeneralParameter(paramShowAxeX, defaultShowAxeX) ? defaultMarginWithAxeB : defaultMargin;

            if((bool)this.graph.GetGeneralParameter(paramEvaluate, defaultEvaluate)) {
                this.time = 0;
                this.maxTime = this.graph.GetMaxLength();
                this.timer.Interval = (double)this.graph.GetGeneralParameter(paramInterval, defaultInterval);
                this.timer.AutoReset = true;
                this.timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                this.timer.Start();
            }
            else
                this.time = -1;

            this.SetMinMax();

			this.Invalidate();
		}

        /// <summary>
        /// Event �asova�e - postupn� vykreslov�n� k�ivky
        /// </summary>
        void timer_Elapsed(object sender, ElapsedEventArgs e) {
            if(this.time++ > this.maxTime) {
                this.timer.Stop();
                this.time = -1;
            }

            this.Invalidate();
        }

        /// <summary>
        /// Nastav� minim�ln� a maxim�ln� hodnotu
        /// </summary>
        public void SetMinMax() {
            Vector minmax = this.graph.GetMinMax();

            this.minX = (double)this.graph.GetGeneralParameter(paramMinX, minmax[0]);
            this.maxX = (double)this.graph.GetGeneralParameter(paramMaxX, minmax[1]);
            this.minY = (double)this.graph.GetGeneralParameter(paramMinY, minmax[2]);
            this.maxY = (double)this.graph.GetGeneralParameter(paramMaxY, minmax[3]);
        }

		/// <summary>
		/// Vypo��t� offset takov�, aby se cel� graf pr�v� ve�el do okna
		/// </summary>
		private double GetFitOffsetY() {
			return this.maxY * (this.Height - this.marginT - this.marginB) / (this.maxY - this.minY) + this.marginT;
		}

		/// <summary>
		/// Vypo��t� offset takov�, aby se cel� graf pr�v� ve�el do okna
		/// </summary>
		private double GetFitOffsetX() {
			return -this.minX * (this.Width - this.marginL - this.marginR) / (this.maxX - this.minX) + this.marginL;
		}

		/// <summary>
		/// Vypo��t� zv�t�en� takov�, aby se cel� graf pr�v� ve�el do okna
		/// </summary>
		private double GetFitAmplifyY() {
			return (this.Height - this.marginT - this.marginB) / (this.maxY - this.minY);
		}

		/// <summary>
		/// Vypo��t� zv�t�en� takov�, aby se cel� graf pr�v� ve�el do okna
		/// </summary>
		private double GetFitAmplifyX() {
			return (this.Width -  this.marginL - this.marginR) / (this.maxX - this.minX);
		}

		/// <summary>
		/// Vykreslen�
		/// </summary>
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint (e);

			if(this.graph.Count > 0) {
                bool shift = (bool)this.graph.GetGeneralParameter(paramShift, defaultShift);

				Graphics g = e.Graphics;
				int stringHeight = (int)g.MeasureString("M", baseFont).Height;

                double offsetX = this.GetFitOffsetX();
                double amplifyX = this.GetFitAmplifyX();
				double offsetY;
				double amplifyY;

				if(!shift) {
					offsetY = this.GetFitOffsetY();
					amplifyY = this.GetFitAmplifyY();
				}
				else {
					offsetY = 0.0;
					amplifyY = this.amplifyY;
				}

				for(int i = 0; i < this.graph.Count; i++) {
					if(shift)
						offsetY = (i + 1) * (this.Height - this.marginT - this.marginB) / (this.graph.Count + 2) + this.marginT;

					Point [] p = this.graph.PointArrayToDraw(i, offsetX, amplifyX, offsetY, amplifyY, this.time);

                    Color lineColor = (Color)this.graph.GetCurveParameter(i, paramLineColor, defaultLineColor);
                    float lineWidth = (float)this.graph.GetCurveParameter(i, paramLineWidth, defaultLineWidth);
                    Graph.LineStyles lineStyle = (Graph.LineStyles)this.graph.GetCurveParameter(i, paramLineStyle, defaultLineStyle);
                    Color pointColor = (Color)this.graph.GetCurveParameter(i, paramPointColor, defaultPointColor);
                    Graph.PointStyles pointStyle = (Graph.PointStyles)this.graph.GetCurveParameter(i, paramPointStyle, defaultPointStyle);
                    int pointSize = (int)this.graph.GetCurveParameter(i, paramPointSize, defaultPointSize);

                    Pen linePen = new Pen(lineColor, lineWidth);
                    Pen pointPen = new Pen(pointColor);
                    
                    if(p.Length >= 2)
                        this.DrawLine(g, p, linePen, lineStyle);

					if(this.graph.IsErrors) {
						Point [] errorPoints = this.graph.GetErrorLines(i, offsetX, amplifyX, offsetY, amplifyY);
						Point [] ep = new Point[2];
						int length = errorPoints.Length / 2;

						for(int j = 0; j < length; j++) {
							ep[0] = errorPoints[2 * j];
							ep[1] = errorPoints[2 * j + 1];
							this.DrawLine(g, ep, pointPen, Graph.LineStyles.Line);
						}

						this.DrawPoints(g, errorPoints, pointPen, Graph.PointStyles.HLines, 5);
					}

                    bool showLabels = (bool)this.graph.GetCurveParameter(i, paramShowLabel, defaultShowLabel);

					if(showLabels) {
                        Color labelColor = (Color)this.graph.GetCurveParameter(i, paramLabelColor, defaultLabelColor);
                        string lineName = (string)this.graph.GetCurveParameter(i, paramLineName, defaultLineName);
                        Brush labelBrush = (new Pen(labelColor)).Brush;
						g.DrawString(lineName, baseFont, labelBrush, this.marginL, (float)(offsetY - stringHeight / 2.0)); 
					}

                    this.DrawPoints(g, p, pointPen, pointStyle, (int)pointSize);

                    // Prvn� a posledn� bod
                    if(p.Length > 0) {
                        Point[] lastPoint = new Point[1]; lastPoint[0] = p[p.Length - 1];
                        Color lastPointColor = (Color)this.graph.GetCurveParameter(i, paramLastPointColor, defaultLastPointColor);
                        Graph.PointStyles lastPointStyle = (Graph.PointStyles)this.graph.GetCurveParameter(i, paramLastPointStyle, defaultLastPointStyle);
                        int lastPointSize = (int)this.graph.GetCurveParameter(i, paramLastPointSize, defaultLastPointSize);
                        Pen lastPointPen = new Pen(lastPointColor);
                        this.DrawPoints(g, lastPoint, lastPointPen, lastPointStyle, lastPointSize);

                        Point[] firstPoint = new Point[1]; firstPoint[0] = p[0];
                        Color firstPointColor = (Color)this.graph.GetCurveParameter(i, paramFirstPointColor, defaultFirstPointColor);
                        Graph.PointStyles firstPointStyle = (Graph.PointStyles)this.graph.GetCurveParameter(i, paramFirstPointStyle, defaultFirstPointStyle);
                        int firstPointSize = (int)this.graph.GetCurveParameter(i, paramFirstPointSize, defaultFirstPointSize);
                        Pen firstPointPen = new Pen(firstPointColor);
                        this.DrawPoints(g, firstPoint, firstPointPen, firstPointStyle, firstPointSize);
                    }
				}

                bool showAxeX = (bool)this.graph.GetGeneralParameter(paramShowAxeX, defaultShowAxeX);
                bool showAxeY = (bool)this.graph.GetGeneralParameter(paramShowAxeY, defaultShowAxeY);

				// X - ov� osa
				if(showAxeX) {
					int y = this.Height - this.marginB;

					Point [] line = new Point[2];
					line[0] = new Point(this.marginL, y);
					line[1] = new Point(this.Width - this.marginR, y);

                    Color labelColorX = (Color)this.graph.GetGeneralParameter(paramLabelColorX, defaultLabelColorX);
                    float lineWidthX = (float)this.graph.GetGeneralParameter(paramLineWidthX, defaultLineWidthX);
                    Pen linePen = new Pen(labelColorX, lineWidthX);

                    Color pointColorX = (Color)this.graph.GetGeneralParameter(paramPointColorX, defaultPointColorX);
                    Pen pointPen = new Pen(pointColorX);

                    Graph.PointStyles pointStyleX = (Graph.PointStyles)this.graph.GetGeneralParameter(paramPointStyleX, defaultPointStyleX);
                    int pointSizeX = (int)this.graph.GetGeneralParameter(paramPointSizeX, defaultPointSizeX);

					this.DrawLine(g, line, linePen, Graph.LineStyles.Line);

					int smallIntervals;
					int smallIntervalsOffset;
					Vector v = this.GetAxesPoints(out smallIntervals, out smallIntervalsOffset, this.Width - this.marginL - this.marginR, this.minX, this.maxX);

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


                    this.DrawPoints(g, p, pointPen, pointStyleX, pointSizeX, smallIntervals, smallIntervalsOffset, 
                        pointStyleX, pointSizeX / 2);
				}

				// Y - ov� osa
				if(showAxeY && !shift) {
					int x = this.marginL;

					Point [] line = new Point[2];
					line[0] = new Point(x, this.marginT);
					line[1] = new Point(x, this.Height - this.marginB);
					
                    Color labelColorY = (Color)this.graph.GetGeneralParameter(paramLabelColorY, defaultLabelColorY);
                    float lineWidthY = (float)this.graph.GetGeneralParameter(paramLineWidthY, defaultLineWidthY);
                    Pen linePen = new Pen(labelColorY, lineWidthY);

                    Color pointColorY = (Color)this.graph.GetGeneralParameter(paramPointColorY, defaultPointColorY);
                    Pen pointPen = new Pen(pointColorY);
                    
                    Graph.PointStyles pointStyleY = (Graph.PointStyles)this.graph.GetGeneralParameter(paramPointStyleY, defaultPointStyleY);
                    int pointSizeY = (int)this.graph.GetGeneralParameter(paramPointSizeY, defaultPointSizeY);

					this.DrawLine(g, line, linePen, Graph.LineStyles.Line);

					int smallIntervals;
					int smallIntervalsOffset;
					Vector v = this.GetAxesPoints(out smallIntervals, out smallIntervalsOffset, this.Height - this.marginT - this.marginB, this.minY, this.maxY);

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

					this.DrawPoints(g, p, pointPen, pointStyleY, pointSizeY, smallIntervals, smallIntervalsOffset, pointStyleY, pointSizeY / 2);
				}
			}
		}

		/// <summary>
		/// Vytvo�� body, ve kter�ch bude label na ose
		/// </summary>
		/// <param name="pixels">Po�et bod� na obrazovce</param>
		/// <param name="min">Minim�ln� hodnota</param>
		/// <param name="max">Maxim�ln� hodnota</param>
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

			Vector result = new Vector((int)(wholeInterval / interval));

			for(int i = 1; i <= result.Length; i++)
				result[i - 1] = x + (i - 1) * interval;

			smallIntervalsOffset = (int)(((System.Math.Floor(x / largeInterval) + 1) * largeInterval - x) / interval + 0.5);
			return result;
		}

		/// <summary>
		/// Nakresl� ��ru
		/// </summary>
		/// <param name="g">Object Graphics</param>
		/// <param name="points">Body pro vykreslen� ��ry</param>
		/// <param name="pen">Pero</param>
		/// <param name="lineStyle">Styl ��ry</param>
		private void DrawLine(Graphics g, Point [] points, Pen pen, Graph.LineStyles lineStyle) {
			switch(lineStyle) {
				case Graph.LineStyles.Curve:
					g.DrawCurve(pen, points);
					break;
				case Graph.LineStyles.Line:
					g.DrawLines(pen, points);
					break;
			}
		}

		/// <summary>
		/// Nakresl� body (bez v�zna�n�ch bod�)
		/// </summary>
		/// <param name="g">Objekt Graphics</param>
		/// <param name="points">Body</param>
		/// <param name="pen">Pero</param>
		/// <param name="pointStyle">Styl bod�</param>
		/// <param name="pointSize">Velikost bod�</param>
		private void DrawPoints(Graphics g, Point [] points, Pen pen, Graph.PointStyles pointStyle, int pointSize) {
			this.DrawPoints(g, points, pen, pointStyle, pointSize, 1, 0, pointStyle, pointSize);
		}

		/// <summary>
		/// Nakresl� body (s v�zna�n�mi body)
		/// </summary>
		/// <param name="g">Objekt Graphics</param>
		/// <param name="points">Body</param>
		/// <param name="pen">Pero</param>
		/// <param name="pointStyle">Styl bod�</param>
		/// <param name="pointSize">Velikost bod�</param>
		/// <param name="hlStep">Krok pro v�zna�n� body</param>
		/// <param name="hlOffset">Kolik�t� krok za��n� v�zna�n� bod</param>
		/// <param name="hlPointStyle">Styl v�zna�n�ch bod�</param>
		/// <param name="hlPointSize">Velikost v�zna�n�ch bod�</param>
		private void DrawPoints(Graphics g, Point [] points, Pen pen, Graph.PointStyles pointStyle, int pointSize, 
			int hlStep, int hlOffset, Graph.PointStyles hlPointStyle, int hlPointSize) {

			for(int j = 0; j < points.Length; j++) {
				if(((j - hlOffset) % hlStep) != 0)
					this.DrawPoint(g, points[j], pen, hlPointStyle, hlPointSize);
				else
					this.DrawPoint(g, points[j], pen, pointStyle, pointSize);
			}
		}

		/// <summary>
		/// Nakresl� bod
		/// </summary>
		/// <param name="g">Objekt Graphics</param>
		/// <param name="point">Body</param>
		/// <param name="pen">Pero</param>
		/// <param name="pointStyle">Styl bodu</param>
		/// <param name="pointSize">Velikost bodu</param>
		private void DrawPoint(Graphics g, Point point, Pen pen, Graph.PointStyles pointStyle, int pointSize) {
			int pointSized2 = pointSize / 2;

			switch(pointStyle) {
				case Graph.PointStyles.Circle:
					g.DrawEllipse(pen, point.X - pointSized2, point.Y - pointSized2, pointSize, pointSize);
					break;
				case Graph.PointStyles.FCircle:
					g.FillEllipse(pen.Brush, point.X - pointSized2, point.Y - pointSized2, pointSize, pointSize);
					break;
				case Graph.PointStyles.Square:
					g.DrawRectangle(pen, point.X - pointSized2, point.Y - pointSized2, pointSize, pointSize);
					break;
				case Graph.PointStyles.FSquare:
					g.FillRectangle(pen.Brush, point.X - pointSized2, point.Y - pointSized2, pointSize, pointSize);
					break;
				case Graph.PointStyles.VLines:
					g.DrawLine(pen, point.X, point.Y - pointSized2, point.X, point.Y + pointSize - pointSized2);
					break;
				case Graph.PointStyles.HLines:
					g.DrawLine(pen, point.X - pointSized2, point.Y, point.X + pointSize - pointSized2, point.Y);
					break;
			}
		}

        /// <summary>
        /// Nastav� ToolTip
        /// </summary>
        /// <param name="x">X - ov� sou�adnice my�i</param>
        /// <param name="y">Y - ov� sou�adnice my�i</param>
        public string ToolTip(int x, int y) {
            double offsetX = this.GetFitOffsetX();
            double amplifyX = this.GetFitAmplifyX();
            double offsetY = this.GetFitOffsetY();
            double amplifyY = this.GetFitAmplifyY();

            double xl = (x - offsetX) / amplifyX;
            double yl = -(y - offsetY) / amplifyY;

            return string.Format("({0,5:F}, {1,5:F})", xl, yl);
        }

		/// <summary>
		/// P�i zm�n� velikosti
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
		// Interval mezi dv�ma body s ��sly na ose (v obrazov�ch bodech)
		private const int axePointInterval = 50;
        private const double multiplierMinMax = 0.02;

        // Parametry grafu
        private const string paramTitle = "title";
        private const string paramShift = "shift";
        private const string paramLineColor = "lcolor";
        private const string paramLineStyle = "lstyle";
        private const string paramLineWidth = "lwidth";
        private const string paramLineName = "lname";
        private const string paramPointColor = "pcolor";
        private const string paramPointStyle = "pstyle";
        private const string paramPointSize = "psize";
        private const string paramShowLabel = "showlabel";
        private const string paramLabelColor = "labelcolor";

        private const string paramFirstPointColor = "fpcolor";
        private const string paramFirstPointStyle = "fpstyle";
        private const string paramFirstPointSize = "fpsize";
        private const string paramLastPointColor = "lpcolor";
        private const string paramLastPointStyle = "lpstyle";
        private const string paramLastPointSize = "lpsize";

        private const string paramMinX = "minx";
        private const string paramMaxX = "maxx";
        private const string paramMinY = "miny";
        private const string paramMaxY = "maxy";

        private const string paramEvaluate = "eval";
        private const string paramInterval = "interval";

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

        private const bool defaultEvaluate = false;
        private const double defaultInterval = 1000.0;

        // Default hodnoty
        private const bool defaultShift = false;
        private static Color defaultLineColor = Color.FromName("blue");
        private static Graph.LineStyles defaultLineStyle = Graph.LineStyles.Line;
        private const float defaultLineWidth = 1.0F;
        private static Color defaultPointColor = Color.FromName("brown");
        private static Graph.PointStyles defaultPointStyle = Graph.PointStyles.Circle;
        private const int defaultPointSize = 2;
        private const bool defaultShowLabel = true;
        private static Color defaultLabelColor = Color.FromName("black");
        private const string defaultLineName = "";

        private static Color defaultFirstPointColor = Color.FromName("darkred");
        private static Graph.PointStyles defaultFirstPointStyle = Graph.PointStyles.FCircle;
        private const int defaultFirstPointSize = 5;
        private static Color defaultLastPointColor = Color.FromName("darkgreen");
        private static Graph.PointStyles defaultLastPointStyle = Graph.PointStyles.FCircle;
        private const int defaultLastPointSize = 5;

        private const double defaultMinX = double.NaN;
        private const double defaultMaxX = defaultMinX;
        private const double defaultMinY = defaultMinX;
        private const double defaultMaxY = defaultMinX;

        private const string defaultTitleX = "X";
        private static Color defaultLineColorX = Color.FromName("red");
        private const float defaultLineWidthX = 1.0F;
        private static Color defaultPointColorX = Color.FromName("red");
        private const Graph.PointStyles defaultPointStyleX = Graph.PointStyles.VLines;
        private const int defaultPointSizeX = 5;
        private const bool defaultShowLabelX = false;
        private static Color defaultLabelColorX = defaultLineColorX;
        private const bool defaultShowAxeX = true;

        private const string defaultTitleY = "Y";
        private static Color defaultLineColorY = Color.FromName("red");
        private const float defaultLineWidthY = 1.0F;
        private static Color defaultPointColorY = Color.FromName("red");
        private const Graph.PointStyles defaultPointStyleY = Graph.PointStyles.HLines;
        private const int defaultPointSizeY = 5;
        private const bool defaultShowLabelY = false;
        private static Color defaultLabelColorY = defaultLineColorY;
        private const bool defaultShowAxeY = true;
    }
}