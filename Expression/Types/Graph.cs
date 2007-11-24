using System;
using System.Drawing;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.Expression.Functions;

namespace PavelStransky.Expression {
    /// <summary>
    /// Tøída, která zapouzdøuje graf (urèuje jeho obsah, popø. další parametry)
    /// </summary>
    public partial class Graph : IExportable {
        // Parametry celého grafu
        private GraphParameterValues graphParamValues;
        // Parametry jednotlivých køivek
        private TArray itemParamValues;
        // Parametry jednotlivých pozadí
        private TArray groupParamValues;

        /// <summary>
        /// Poèet skupin grafu
        /// </summary>
        public int NumGroups { get { return (int)this.graphParamValues[ParametersIndications.NumGroups]; } }

        /// <summary>
        /// Maximální èas pro skupinu
        /// </summary>
        /// <param name="group">Èíslo skupiny</param>
        public int MaxTime(int group) {
            return (int)(this.groupParamValues[group] as GraphParameterValues)[ParametersIndications.GroupMaxLength];
        }

        /// <summary>
        /// True, pokud se má animovat skupina grafu
        /// </summary>
        public bool AnimGroup { get { return (bool)this.graphParamValues[ParametersIndications.AnimGroup]; } }

        /// <summary>
        /// True, pokud se má animovat køivka grafu
        /// </summary>
        /// <param name="group">Èíslo skupiny</param>
        public bool AnimCurves(int group) {
            return (bool)(this.groupParamValues[group] as GraphParameterValues)[ParametersIndications.AnimCurve];
        }

        /// <summary>
        /// Interval pro animaci
        /// </summary>
        public int AnimInterval { get { return (int)this.graphParamValues[ParametersIndications.Interval]; } }

        /// <summary>
        /// Krok pøi rolování
        /// </summary>
        public int ScrollStep { get { return (int)this.graphParamValues[ParametersIndications.ScrollStep]; } }

        /// <summary>
        /// Pøevede souøadnice okna (myši) na souøadnice skuteèné
        /// </summary>
        /// <param name="x">X - ová souøadnice</param>
        /// <param name="y">Y - ová souøadnice</param>
        /// <param name="group">Èíslo skupiny</param>
        /// <param name="rectangle">Obdélník, ve kterém je zobrazení</param>
        public PointD CoordinatesFromPosition(int group, Rectangle rectangle, int x, int y) {
            GraphParameterValues gv = this.groupParamValues[group] as GraphParameterValues;

            Rectangle rectangleM = this.AddMargins(gv, rectangle);
            PointD offset = this.GetFitOffset(gv, rectangleM);
            PointD amplify = this.GetFitAmplify(gv, rectangleM);

            return new PointD((x - offset.X) / amplify.X, -(y - offset.Y) / amplify.Y);
        }

        /// <summary>
        /// Vrátí hodnotu matice z pozadí; pokud na dané pozici matice pozadí není, vrátí NaN
        /// </summary>
        /// <param name="group">Èíslo skupiny</param>
        /// <param name="p">Bod, z kterého chceme hodnotu pozadí</param>
        public double BackgroundValue(int group, PointD p) {
            GraphParameterValues gv = this.groupParamValues[group] as GraphParameterValues;

            Matrix m = (Matrix)gv[ParametersIndications.DataBackground];
            if(m.NumItems() == 0)
                return double.NaN;

            double bminX = (double)gv[ParametersIndications.BMinX];
            double bmaxX = (double)gv[ParametersIndications.BMaxX];
            double bminY = (double)gv[ParametersIndications.BMinY];
            double bmaxY = (double)gv[ParametersIndications.BMaxY];

            int x = (int)System.Math.Round((m.LengthX - 1) / (bmaxX - bminX) * (p.X - bminX));
            int y = (int)System.Math.Round((m.LengthY - 1) / (bmaxY - bminY) * (p.Y - bminY));

            if(x < 0 || x >= m.LengthX || y < 0 || y >= m.LengthY)
                return double.NaN;

            return m[x, y];
        }

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
            this.CreateWorkers();

            int nGroups = item.GetLength(0);

            this.graphParamValues = new GraphParameterValues(globalParams, graphContext, null, null);
            this.groupParamValues = new TArray(typeof(GraphParameterValues), nGroups);
            this.itemParamValues = new TArray(typeof(TArray), nGroups);

            int gcl = groupContext.Length;
            int icl = itemContext.Length;

            int el = errors.Length;
            int bl = background.Length;

            this.graphParamValues[ParametersIndications.NumGroups] = nGroups;

            for(int g = 0; g < nGroups; g++) {
                TArray ac = item[g] as TArray;
                Matrix ab = bl > g ? background[g] as Matrix : new Matrix(0);
                TArray ae = el > g ? errors[g] as TArray : new TArray(typeof(Vector), 0);

                int nCurves = ac.GetLength(0);

                Context gc = gcl > g ? groupContext[g] as Context : null;
                TArray aic = icl > g ? itemContext[g] as TArray : null;

                int icll = icl > g ? aic.Length : 0;
                int ael = ae.Length;

                GraphParameterValues gv = new GraphParameterValues(groupParams, gc, graphContext, null);
                gv[ParametersIndications.NumCurves] = nCurves;
                gv[ParametersIndications.DataBackground] = ab;

                int maxLength = 0;

                TArray a = new TArray(typeof(GraphParameterValues), nCurves);
                for(int i = 0; i < nCurves; i++) {
                    GraphParameterValues cv = new GraphParameterValues(curveParams, icll > i ? aic[i] as Context : null, gc, graphContext);
                    cv[ParametersIndications.DataCurves] = ac[i];
                    cv[ParametersIndications.DataErrors] = ael > i ? ae[i] : new Vector(0);
                    a[i] = cv;

                    maxLength = System.Math.Max(maxLength, (ac[i] as PointVector).Length);
                }

                gv[ParametersIndications.GroupMaxLength] = maxLength;

                this.FindMinMax(gv, a);
                this.SetMinMax(gv);
                this.SetBorders(gv);

                this.itemParamValues[g] = a;
                this.groupParamValues[g] = gv;
            }

            this.bitmap = new Bitmap[nGroups];
        }

        /// <summary>
        /// Nastaví okraje, aby byly správnì zobrazovány, pokud máme èi nemáme osy a podobnì
        /// </summary>
        /// <param name="gv">Parametry aktuální skupiny</param>
        private void SetBorders(GraphParameterValues gv) {
            bool isDefMarginT = gv.IsDefault(ParametersIndications.MarginT);
            bool isDefMarginB = gv.IsDefault(ParametersIndications.MarginB);
            bool isDefMarginL = gv.IsDefault(ParametersIndications.MarginL);
            bool isDefMarginR = gv.IsDefault(ParametersIndications.MarginR);

            if(isDefMarginT) {
                int m = 10;

                bool isAxeT = (bool)gv[ParametersIndications.AShowT];
                bool isLabelX = (bool)gv[ParametersIndications.AShowLabelX];
                bool isTitleX = (string)gv[ParametersIndications.ATitleX] != string.Empty;
                bool isGTitle = (string)gv[ParametersIndications.GTitle] != string.Empty;
                bool isTitle = (string)this.graphParamValues[ParametersIndications.GTitle] != string.Empty;

                if(isAxeT) {
                    m += 5;
                    if(isLabelX)
                        m += 10;
                    if(isTitleX)
                        m += 20;
                }

                if(isGTitle || isTitle)
                    m += 25;

                gv[ParametersIndications.MarginT] = m;
            }

            if(isDefMarginB) {
                int m = 10;

                bool isAxeB = (bool)gv[ParametersIndications.AShowB];
                bool isLabelX = (bool)gv[ParametersIndications.AShowLabelX];
                bool isTitleX = (string)gv[ParametersIndications.ATitleX] != string.Empty;

                if(isAxeB) {
                    m += 5;
                    if(isLabelX)
                        m += 10;
                    if(isTitleX)
                        m += 20;
                }

                gv[ParametersIndications.MarginB] = m;
            }

            if(isDefMarginL) {
                int m = 10;

                bool isAxeL = (bool)gv[ParametersIndications.AShowL];
                bool isLabelY = (bool)gv[ParametersIndications.AShowLabelY];
                bool isTitleY = (string)gv[ParametersIndications.ATitleY] != string.Empty;

                if(isAxeL) {
                    m += 5;
                    if(isLabelY)
                        m += 20;
                    if(isTitleY)
                        m += 20;
                }

                gv[ParametersIndications.MarginL] = m;
            }

            if(isDefMarginR) {
                int m = 10;

                bool isAxeR = (bool)gv[ParametersIndications.AShowR];
                bool isLabelY = (bool)gv[ParametersIndications.AShowLabelY];
                bool isTitleY = (string)gv[ParametersIndications.ATitleY] != string.Empty;

                if(isAxeR) {
                    m += 5;
                    if(isLabelY)
                        m += 20;
                    if(isTitleY)
                        m += 20;
                }

                gv[ParametersIndications.MarginR] = m;
            }
        }

        /// <summary>
        /// Nastaví pole s minimálními a maximálními hodnotami
        /// </summary>
        /// <param name="gv">Parametry aktuální skupiny</param>
        private void SetMinMax(GraphParameterValues gv) {
            double minX = (double)gv[ParametersIndications.MinX];
            double maxX = (double)gv[ParametersIndications.MaxX];
            double minY = (double)gv[ParametersIndications.MinY];
            double maxY = (double)gv[ParametersIndications.MaxY];

            bool isDefMinX = gv.IsDefault(ParametersIndications.MinX);
            bool isDefMaxX = gv.IsDefault(ParametersIndications.MaxX);
            bool isDefMinY = gv.IsDefault(ParametersIndications.MinY);
            bool isDefMaxY = gv.IsDefault(ParametersIndications.MaxY);

            if(isDefMinX || isDefMaxX || isDefMinY || isDefMaxY) {                
                if((bool)gv[ParametersIndications.FoundDataMinMax]) {
                    if(isDefMinX)
                        minX = (double)gv[ParametersIndications.DataMinX];
                    if(isDefMaxX)
                        maxX = (double)gv[ParametersIndications.DataMaxX];
                    if(isDefMinY)
                        minY = (double)gv[ParametersIndications.DataMinY];
                    if(isDefMaxY)
                        maxY = (double)gv[ParametersIndications.DataMaxY];
                }
            }

            double bminX = (double)gv[ParametersIndications.BMinX];
            double bmaxX = (double)gv[ParametersIndications.BMaxX];
            double bminY = (double)gv[ParametersIndications.BMinY];
            double bmaxY = (double)gv[ParametersIndications.BMaxY];

            bool isDefBMinX = gv.IsDefault(ParametersIndications.BMinX);
            bool isDefBMaxX = gv.IsDefault(ParametersIndications.BMaxX);
            bool isDefBMinY = gv.IsDefault(ParametersIndications.BMinY);
            bool isDefBMaxY = gv.IsDefault(ParametersIndications.BMaxY);

            Matrix m = gv[ParametersIndications.DataBackground] as Matrix;
            double maxAbs = 0.0;

            if(m.NumItems() > 0) {
                if((bool)gv[ParametersIndications.FoundDataMinMax]) {
                    if(isDefBMinX)
                        bminX = minX;
                    if(isDefBMaxX)
                        bmaxX = maxX;
                    if(isDefBMinY)
                        bminY = minY;
                    if(isDefBMaxY)
                        bmaxY = maxY;
                }
                else {
                    if(isDefBMinX)
                        bminX = 0;
                    if(isDefBMaxX)
                        bmaxX = m.LengthX;
                    if(isDefBMinY)
                        bminY = 0;
                    if(isDefBMaxY)
                        bmaxY = m.LengthY;

                    if(isDefMinX)
                        minX = bminX;
                    if(isDefBMaxX)
                        maxX = bmaxX;
                    if(isDefBMinY)
                        minY = bminY;
                    if(isDefBMaxY)
                        maxY = bmaxY;
                }

                maxAbs = m.MaxAbs();
            }

            minX = System.Math.Min(minX, maxX);
            maxX = System.Math.Max(minX, maxX);
            minY = System.Math.Min(minY, maxY);
            maxY = System.Math.Max(minY, maxY);

            // Minimální a maximální hodnoty nesmí být stejné
            if(minX == maxX) {
                if(minX < 0.0) {
                    minX *= 2.0;
                    maxX = 0.0;
                }
                else if(maxX > 0) {
                    minX = 0.0;
                    maxX *= 2.0;
                }
                else {
                    minX = -1.0;
                    maxX = 1.0;
                }
            }
            if(minY == maxY) {
                if(minY < 0.0) {
                    minY *= 2.0;
                    maxY = 0.0;
                }
                else if(maxY > 0) {
                    minY = 0.0;
                    maxY *= 2.0;
                }
                else {
                    minY = -1.0;
                    maxY = 1.0;
                }
            }

            // Hranice pro vykreslení pozadí (v pixelech)
            if(m.NumItems() > 0) {
                double pixelDX = ((m.LengthX - 1) * (int)gv[ParametersIndications.BPSizeX]) / (bmaxX - bminX);
                double pixelDY = ((m.LengthY - 1) * (int)gv[ParametersIndications.BPSizeY]) / (bmaxY - bminY);

                gv[ParametersIndications.PixelL] = (minX - bminX) * pixelDX;
                gv[ParametersIndications.PixelT] = (bmaxY - maxY) * pixelDY;
                gv[ParametersIndications.PixelW] = (maxX - minX) * pixelDX;
                gv[ParametersIndications.PixelH] = (maxY - minY) * pixelDY;
            }

            gv[ParametersIndications.MatrixAbs] = maxAbs;

            gv[ParametersIndications.MinX] = minX;
            gv[ParametersIndications.MaxX] = maxX;
            gv[ParametersIndications.MinY] = minY;
            gv[ParametersIndications.MaxY] = maxY;

            gv[ParametersIndications.BMinX] = bminX;
            gv[ParametersIndications.BMaxX] = bmaxX;
            gv[ParametersIndications.BMinY] = bminY;
            gv[ParametersIndications.BMaxY] = bmaxY;
        }

        /// <summary>
        /// Nalezne minimální a maximální hodnotu souøadnic x a y v grafu
        /// </summary>
        /// <param name="gv">Parametry skupiny</param>
        /// <param name="cv">Parametry køivek pro danou skupinu</param>
        private void FindMinMax(GraphParameterValues gv, TArray cv) {
            gv[ParametersIndications.FoundDataMinMax] = false;
            int nCurves = cv.Length;

            if(nCurves == 0)
                return;

            bool first = true;
            double minX = 0.0, maxX = 0.0, minY = 0.0, maxY = 0.0;

            for(int i = 0; i < nCurves; i++) {
                GraphParameterValues cv1 = cv[i] as GraphParameterValues;

                PointVector pv = (PointVector)cv1[ParametersIndications.DataCurves];
                Vector ev = (Vector)cv1[ParametersIndications.DataErrors];

                if(pv.Length == 0)
                    continue;

                if(first) {
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

                    first = false;
                }
                else {
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
            }

            if(!first) {
                gv[ParametersIndications.FoundDataMinMax] = true;
                gv[ParametersIndications.DataMinX] = minX;
                gv[ParametersIndications.DataMaxX] = maxX;
                gv[ParametersIndications.DataMinY] = minY;
                gv[ParametersIndications.DataMaxY] = maxY;
            }
        }

        /// <summary>
        /// Vytvoøí øadu bodù k vykreslení
        /// </summary>
        /// <param name="data">Data k vykreslení</param>
        /// <param name="offset">Posunutí vhledem k osám</param>
        /// <param name="amplify">Zesílení v osách</param>
        /// <param name="maxLength">Maximální délka dat</param>
        public Point[] PointArrayToDraw(PointVector data, PointD offset, PointD amplify, int maxLength) {
            Point[] result = new Point[0];

            if(data.Length > 0){
                int length = maxLength < 0 ? data.Length : System.Math.Min(data.Length, maxLength);
                result = new Point[length];

                for(int i = 0; i < length; i++)
                    result[i] = new Point((int)(data[i].X * amplify.X + offset.X), (int)(-data[i].Y * amplify.Y + offset.Y));
            }

            return result;
        }

        /// <summary>
        /// Vytvoøí øadu bodù k vykreslení
        /// </summary>
        /// <param name="data">Data k vykreslení</param>
        /// <param name="offset">Posunutí vhledem k osám</param>
        /// <param name="amplify">Zesílení v osách</param>
        public Point[] PointArrayToDraw(PointVector data, PointD offset, PointD amplify) {
            return this.PointArrayToDraw(data, offset, amplify, -1);
        }

        /// <summary>
        /// Vytvoøí poèáteèní a koncový bod pro chybovou úseèku
        /// </summary>
        /// <param name="data">Data k vykreslení</param>
        /// <param name="error">Chybové úseèky</param>
        /// <param name="offset">Posunutí vhledem k osám</param>
        /// <param name="amplify">Zesílení v osách</param>
        /// <returns>Øadu bodù - poèátek a konec chybové úseèky</returns>
        public Point[] GetErrorLines(PointVector data, Vector error, PointD offset, PointD amplify) {
            Point[] result = new Point[0];

            if(error.Length > 0 && data.Length > 0) {
                int length = System.Math.Min(data.Length, error.Length);
                result = new Point[2 * length];

                for(int i = 0; i < length; i++) {
                    int x = (int)(data[i].X * amplify.X + offset.X);
                    result[2 * i] = new Point(x, (int)(-(data[i].Y + error[i]) * amplify.Y + offset.Y));
                    result[2 * i + 1] = new Point(x, (int)(-(data[i].Y - error[i]) * amplify.Y + offset.Y));
                }
            }

            return result;
        }

        /// <summary>
        /// Vypoèítá offset takový, aby se celý graf právì vešel do okna
        /// </summary>
        /// <param name="gv">Parametry skupiny grafu</param>
        /// <param name="rectangle">Obdélník, do kterého se kreslí</param>
        private PointD GetFitOffset(GraphParameterValues gv, Rectangle rectangle) {
            return new PointD(
                -(double)gv[ParametersIndications.MinX] * rectangle.Width / ((double)gv[ParametersIndications.MaxX] - (double)gv[ParametersIndications.MinX]) + rectangle.Left,
                (double)gv[ParametersIndications.MaxY] * rectangle.Height / ((double)gv[ParametersIndications.MaxY] - (double)gv[ParametersIndications.MinY]) + rectangle.Top);
        }

        /// <summary>
        /// Vypoèítá zvìtšení takové, aby se celý graf právì vešel do okna
        /// </summary>
        /// <param name="gv">Parametry skupiny grafu</param>
        /// <param name="rectangle">Obdélník, do kterého se kreslí</param>
        private PointD GetFitAmplify(GraphParameterValues gv, Rectangle rectangle) {
            return new PointD(
                rectangle.Width / ((double)gv[ParametersIndications.MaxX] - (double)gv[ParametersIndications.MinX]),
                rectangle.Height / ((double)gv[ParametersIndications.MaxY] - (double)gv[ParametersIndications.MinY]));
        }

        /// <summary>
        /// Pøidá okraje k obdélníku, do kterého se má vykreslovat
        /// </summary>
        /// <param name="gv">Parametry skupiny grafu</param>
        /// <param name="rectangle">Obdélník k vykreslení</param>
        private Rectangle AddMargins(GraphParameterValues gv, Rectangle rectangle) {
            rectangle.X += (int)gv[ParametersIndications.MarginL];
            rectangle.Y += (int)gv[ParametersIndications.MarginT];

            rectangle.Width -= (int)gv[ParametersIndications.MarginL] + (int)gv[ParametersIndications.MarginR];
            rectangle.Height -= (int)gv[ParametersIndications.MarginT] + (int)gv[ParametersIndications.MarginB];

            return rectangle;
        }

        /// <summary>
        /// Provede vykreslení grafu
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="time">Èas k vykreslení, -1 pro vykreslení všeho</param>
        /// <param name="group">Skupina køivek pro vykreslení</param>
        /// <param name="rectangle">Obdélník, do kterého se bude vykreslovat</param>
        public void PaintGraph(Graphics g, Rectangle rectangle, int group, int time) {
            GraphParameterValues gv = this.groupParamValues[group] as GraphParameterValues;
            TArray aiv = this.itemParamValues[group] as TArray;

            Color backgroundColor = (Color)gv[ParametersIndications.BColor];
            Brush backgroundBrush = (new Pen(backgroundColor)).Brush;
            g.FillRectangle(backgroundBrush, rectangle);

            bool shift = (bool)this.graphParamValues[ParametersIndications.Shift];

            Rectangle rectangleM = this.AddMargins(gv, rectangle);

            if(this.bitmap[group] != null) {
                RectangleF rf = new RectangleF(
                    (float)(double)gv[ParametersIndications.PixelL],
                    (float)(double)gv[ParametersIndications.PixelT],
                    (float)(double)gv[ParametersIndications.PixelW],
                    (float)(double)gv[ParametersIndications.PixelH]);

                //rf.Width -= rf.X;

                g.DrawImage(this.bitmap[group], rectangleM, rf, GraphicsUnit.Pixel);
            }

            PointD offset = this.GetFitOffset(gv, rectangleM);
            PointD amplify = this.GetFitAmplify(gv, rectangleM);

            if(shift) {
                offset.Y = 0.0;
                amplify.Y = 8.0;
            }

            int nCurves = (int)gv[ParametersIndications.NumCurves];
            Matrix background = (Matrix)gv[ParametersIndications.DataBackground];

            // X - ová møížka
            bool showGridX = (bool)gv[ParametersIndications.ShowGridX];

            if(showGridX) {
                int yT = rectangleM.Top;
                int yB = rectangleM.Bottom;

                Color gridColorX = (Color)gv[ParametersIndications.GridColorX];
                float gridWidthX = (int)gv[ParametersIndications.GridWidthX];
                Pen gridPen = new Pen(gridColorX, gridWidthX);

                int smallIntervals;
                int smallIntervalsOffset;
                Vector v = this.GetAxesPoints(out smallIntervals, out smallIntervalsOffset, rectangleM.Width, (double)gv[ParametersIndications.MinX], (double)gv[ParametersIndications.MaxX]);

                int length = v.Length;

                for(int i = 0; i < length; i++) {
                    if(((i - smallIntervalsOffset) % smallIntervals) != 0)
                        continue;

                    int x = (int)(v[i] * amplify.X + offset.X);

                    Point[] gp = new Point[2];
                    gp[0] = new Point(x, yT);
                    gp[1] = new Point(x, yB);
                    this.DrawLine(g, gp, gridPen, Graph.LineStyles.Line);
                }
            }

            // Y - ová møížka
            bool showGridY = (bool)gv[ParametersIndications.ShowGridY];

            if(showGridY && !shift) {
                int xL = rectangleM.Left;
                int xR = rectangleM.Right;

                Color gridColorY = (Color)gv[ParametersIndications.GridColorY];
                float gridWidthY = (int)gv[ParametersIndications.GridWidthY];
                Pen gridPen = new Pen(gridColorY, gridWidthY);

                int smallIntervals;
                int smallIntervalsOffset;
                Vector v = this.GetAxesPoints(out smallIntervals, out smallIntervalsOffset, rectangleM.Height, (double)gv[ParametersIndications.MinY], (double)gv[ParametersIndications.MaxY]);

                int length = v.Length;

                for(int i = 0; i < length; i++) {
                    if(((i - smallIntervalsOffset) % smallIntervals) != 0)
                        continue;

                    int y = (int)(-v[i] * amplify.Y + offset.Y);

                    Point[] gp = new Point[2];
                    gp[0] = new Point(xL, y);
                    gp[1] = new Point(xR, y);
                    this.DrawLine(g, gp, gridPen, Graph.LineStyles.Line);
                }
            }

            // Køivky
            if(nCurves > 0) {
                for(int i = 0; i < nCurves; i++) {
                    GraphParameterValues iv = aiv[i] as GraphParameterValues;
                    PointVector data = (PointVector)iv[ParametersIndications.DataCurves];
                    Vector errors = (Vector)iv[ParametersIndications.DataErrors];

                    if(shift)
                        offset.Y = (i + 1) * rectangleM.Height / (nCurves + 2);

                    Point[] p = this.PointArrayToDraw(data, offset, amplify, time);

                    Color lineColor = (Color)iv[ParametersIndications.LColor];
                    float lineWidth = (int)iv[ParametersIndications.LWidth];
                    Graph.LineStyles lineStyle = (Graph.LineStyles)iv[ParametersIndications.LStyle];
                    Color pointColor = (Color)iv[ParametersIndications.PColor];
                    Graph.PointStyles pointStyle = (Graph.PointStyles)iv[ParametersIndications.PStyle];
                    int pointSize = (int)iv[ParametersIndications.PSize];

                    Pen linePen = new Pen(lineColor, lineWidth);
                    Pen pointPen = new Pen(pointColor);

                    if(p.Length >= 2)
                        this.DrawLine(g, p, linePen, lineStyle);

                    // Chybové úseèky
                    if(errors.Length > 0) {
                        Point[] errorPoints = this.GetErrorLines(data, errors, offset, amplify);
                        Point[] ep = new Point[2];
                        int length = errorPoints.Length / 2;

                        for(int j = 0; j < length; j++) {
                            ep[0] = errorPoints[2 * j];
                            ep[1] = errorPoints[2 * j + 1];
                            this.DrawLine(g, ep, pointPen, Graph.LineStyles.Line);
                        }

                        this.DrawPoints(g, errorPoints, pointPen, Graph.PointStyles.HLines, 5);
                    }

                    string lineName = (string)iv[ParametersIndications.LName];

                    if(lineName != string.Empty) {
                        Color labelColor = (Color)iv[ParametersIndications.LabelColor];
                        Brush labelBrush = (new Pen(labelColor)).Brush;
                        Font font = new Font(baseFontFamilyName, (int)iv[ParametersIndications.LabelFSize]);
                        float stringHeight = g.MeasureString(lineName, font).Height;
                        g.DrawString(lineName, font, labelBrush, rectangleM.Left, (float)(offset.Y - stringHeight / 2.0));
                    }

                    this.DrawPoints(g, p, pointPen, pointStyle, (int)pointSize);

                    // První a poslední bod
                    if(p.Length > 0) {
                        Point[] lastPoint = new Point[1]; lastPoint[0] = p[p.Length - 1];
                        Color lastPointColor = (Color)iv[ParametersIndications.LPColor];
                        Graph.PointStyles lastPointStyle = (Graph.PointStyles)iv[ParametersIndications.LPStyle];
                        int lastPointSize = (int)iv[ParametersIndications.LPSize];
                        Pen lastPointPen = new Pen(lastPointColor);
                        this.DrawPoints(g, lastPoint, lastPointPen, lastPointStyle, lastPointSize);

                        Point[] firstPoint = new Point[1]; firstPoint[0] = p[0];
                        Color firstPointColor = (Color)iv[ParametersIndications.FPColor];
                        Graph.PointStyles firstPointStyle = (Graph.PointStyles)iv[ParametersIndications.FPStyle];
                        int firstPointSize = (int)iv[ParametersIndications.FPSize];
                        Pen firstPointPen = new Pen(firstPointColor);
                        this.DrawPoints(g, firstPoint, firstPointPen, firstPointStyle, firstPointSize);
                    }
                }
            }

            // X - ová osa
            bool showAxeT = (bool)gv[ParametersIndications.AShowT];
            bool showAxeB = (bool)gv[ParametersIndications.AShowB];

            if(showAxeT || showAxeB) {
                int yT = rectangleM.Top;
                int yB = rectangleM.Bottom;

                Color lineColorX = (Color)gv[ParametersIndications.ALColorX];
                float lineWidthX = (int)gv[ParametersIndications.ALWidthX];
                Pen linePen = new Pen(lineColorX, lineWidthX);
                Brush numBrush = linePen.Brush;

                Color pointColorX = (Color)gv[ParametersIndications.APColorX];
                Pen pointPen = new Pen(pointColorX, lineWidthX);

                Graph.PointStyles pointStyleX = (Graph.PointStyles)gv[ParametersIndications.APStyleX];
                int pointSizeX = (int)gv[ParametersIndications.APSizeX];

                Font font = new Font(baseFontFamilyName, (int)gv[ParametersIndications.ALabelFSizeX], FontStyle.Bold);

                if(showAxeT) {
                    Point[] line = new Point[2];
                    line[0] = new Point(rectangleM.Left, yT);
                    line[1] = new Point(rectangleM.Right, yT);
                    this.DrawLine(g, line, linePen, Graph.LineStyles.Line);
                }

                if(showAxeB) {
                    Point[] line = new Point[2];
                    line[0] = new Point(rectangleM.Left, yB);
                    line[1] = new Point(rectangleM.Right, yB);
                    this.DrawLine(g, line, linePen, Graph.LineStyles.Line);
                }

                int smallIntervals;
                int smallIntervalsOffset;
                Vector v = this.GetAxesPoints(out smallIntervals, out smallIntervalsOffset, rectangleM.Width, (double)gv[ParametersIndications.MinX], (double)gv[ParametersIndications.MaxX]);

                int length = v.Length;
                Point[] pT = new Point[length];
                Point[] pB = new Point[length];

                for(int i = 0; i < length; i++) {
                    int x = (int)(v[i] * amplify.X + offset.X);
                    pT[i] = new Point(x, yT);
                    pB[i] = new Point(x, yB);
                    if(((i - smallIntervalsOffset) % smallIntervals) != 0)
                        continue;

                    string numString = v[i].ToString();
                    SizeF ns = g.MeasureString(numString, font);

                    int xf = x - (int)(ns.Width / 2F);

                    if(showAxeT)
                        g.DrawString(numString, font, numBrush, xf, yT - 5 - (int)ns.Height);

                    if(showAxeB)
                        g.DrawString(numString, font, numBrush, xf, yB + 5);
                }

                if(showAxeT)
                    this.DrawPoints(g, pT, pointPen, pointStyleX, pointSizeX, smallIntervals, smallIntervalsOffset,
                        pointStyleX, pointSizeX / 2);

                if(showAxeB)
                    this.DrawPoints(g, pB, pointPen, pointStyleX, pointSizeX, smallIntervals, smallIntervalsOffset,
                        pointStyleX, pointSizeX / 2);
            }

            // Y - ová osa
            bool showAxeL = (bool)gv[ParametersIndications.AShowL];
            bool showAxeR = (bool)gv[ParametersIndications.AShowR];

            if((showAxeL || showAxeR) && !shift) {
                int xL = rectangleM.Left;
                int xR = rectangleM.Right;

                Color lineColorY = (Color)gv[ParametersIndications.ALColorY];
                float lineWidthY = (int)gv[ParametersIndications.ALWidthY];
                Pen linePen = new Pen(lineColorY, lineWidthY);
                Brush numBrush = linePen.Brush;

                Color pointColorY = (Color)gv[ParametersIndications.APColorY];
                Pen pointPen = new Pen(pointColorY, lineWidthY);

                Graph.PointStyles pointStyleY = (Graph.PointStyles)gv[ParametersIndications.APStyleY];
                int pointSizeY = (int)gv[ParametersIndications.APSizeY];

                Font font = new Font(baseFontFamilyName, (int)gv[ParametersIndications.ALabelFSizeY], FontStyle.Bold);

                if(showAxeL) {
                    Point[] line = new Point[2];
                    line[0] = new Point(xL, rectangleM.Top);
                    line[1] = new Point(xL, rectangleM.Bottom);
                    this.DrawLine(g, line, linePen, Graph.LineStyles.Line);
                }

                if(showAxeR) {
                    Point[] line = new Point[2];
                    line[0] = new Point(xR, rectangleM.Top);
                    line[1] = new Point(xR, rectangleM.Bottom);
                    this.DrawLine(g, line, linePen, Graph.LineStyles.Line);
                }

                int smallIntervals;
                int smallIntervalsOffset;
                Vector v = this.GetAxesPoints(out smallIntervals, out smallIntervalsOffset, rectangleM.Height, (double)gv[ParametersIndications.MinY], (double)gv[ParametersIndications.MaxY]);

                int length = v.Length;
                Point[] pL = new Point[length];
                Point[] pR = new Point[length];

                for(int i = 0; i < length; i++) {
                    int y = (int)(-v[i] * amplify.Y + offset.Y);
                    pL[i] = new Point(xL, y);
                    pR[i] = new Point(xR, y);
                    if(((i - smallIntervalsOffset) % smallIntervals) != 0)
                        continue;

                    string numString = v[i].ToString();
                    SizeF ns = g.MeasureString(numString, font);

                    int yf = y - (int)(ns.Height / 2F);

                    if(showAxeL)
                        g.DrawString(numString, font, numBrush, xL - 5 - (int)ns.Width, yf);

                    if(showAxeR)
                        g.DrawString(numString, font, numBrush, xR + 5, yf);
                }

                if(showAxeL)
                    this.DrawPoints(g, pL, pointPen, pointStyleY, pointSizeY, smallIntervals, smallIntervalsOffset, pointStyleY, pointSizeY / 2);

                if(showAxeR)
                    this.DrawPoints(g, pR, pointPen, pointStyleY, pointSizeY, smallIntervals, smallIntervalsOffset, pointStyleY, pointSizeY / 2);
            }

            // X - ový popisek
            string titleX = (string)gv[ParametersIndications.ATitleX];

            if(titleX != string.Empty && (showAxeT || showAxeB)) {
                int x = rectangleM.Width / 2 + rectangleM.Left;
                int yT = rectangleM.Top - 20;
                int yB = rectangleM.Bottom + 20;

                Font font = new Font(baseFontFamilyName, (int)gv[ParametersIndications.ATitleFSizeX], FontStyle.Bold | FontStyle.Italic);
                Color titleColor = (Color)gv[ParametersIndications.ATitleColorX];
                Brush titleBrush = new Pen(titleColor).Brush;

                SizeF p = g.MeasureString(titleX, font);
                x -= (int)(p.Width / 2F);
                yT -= (int)p.Height;

                if(showAxeT)
                    g.DrawString(titleX, font, titleBrush, x, yT);

                if(showAxeB)
                    g.DrawString(titleX, font, titleBrush, x, yB);
            }

            // Y - ový popisek
            string titleY = (string)gv[ParametersIndications.ATitleY];

            if(titleY != string.Empty && (showAxeL || showAxeR)) {
                int y = rectangleM.Height / 2 + rectangleM.Top;
                int xL = rectangleM.Left - 15;
                int xR = rectangleM.Right + 15;

                Font font = new Font(baseFontFamilyName, (int)gv[ParametersIndications.ATitleFSizeY], FontStyle.Bold | FontStyle.Italic);
                Color titleColor = (Color)gv[ParametersIndications.ATitleColorY];
                Brush titleBrush = new Pen(titleColor).Brush;

                SizeF p = g.MeasureString(titleY, font);
                y -= (int)(p.Height / 2F);
                xL -= (int)p.Width;

                if(showAxeL)
                    g.DrawString(titleY, font, titleBrush, xL, y);

                if(showAxeR)
                    g.DrawString(titleY, font, titleBrush, xR, y);
            }

            // Titulek
            string title = (string)this.graphParamValues[ParametersIndications.Title];
            if(title != string.Empty) {
                Color titleColor = (Color)this.graphParamValues[ParametersIndications.TitleColor];
                Pen titlePen = new Pen(titleColor);
                Font font = new Font(baseFontFamilyName, (int)this.graphParamValues[ParametersIndications.TitleFSize], FontStyle.Bold);

                int x = rectangleM.Left - 5;
                int y = rectangle.Top + 5;

                g.DrawString(title, font, titlePen.Brush, x, y);
            }

            // Podtitulek
            string subTitle = (string)gv[ParametersIndications.GTitle];
            if(subTitle != string.Empty) {
                Color subTitleColor = (Color)gv[ParametersIndications.GTitleColor];
                Pen subTitlePen = new Pen(subTitleColor);
                Font font = new Font(baseFontFamilyName, (int)gv[ParametersIndications.GTitleFSize], FontStyle.Bold);

                SizeF size = g.MeasureString(subTitle, font);

                int x = rectangleM.Right - (int)size.Width + 10;
                int y = rectangle.Top + 10;

                g.DrawString(subTitle, font, subTitlePen.Brush, x, y);
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
            x = System.Math.Ceiling(x);
            x *= interval;

            Vector result = new Vector((int)(max / interval) - (int)(min / interval) + 1);

            for(int i = 1; i <= result.Length; i++)
                result[i - 1] = x + (i - 1) * interval;

            smallIntervalsOffset = (int)(((System.Math.Floor(x / largeInterval) + 1) * largeInterval - x) / interval + 0.5);
            return result;
        }

        /// <summary>
        /// Nakreslí èáru
        /// </summary>
        /// <param name="g">Object Graphics</param>
        /// <param name="points">Body pro vykreslení èáry</param>
        /// <param name="pen">Pero</param>
        /// <param name="lineStyle">Styl èáry</param>
        private void DrawLine(Graphics g, Point[] points, Pen pen, Graph.LineStyles lineStyle) {
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
        /// Nakreslí body (bez význaèných bodù)
        /// </summary>
        /// <param name="g">Objekt Graphics</param>
        /// <param name="points">Body</param>
        /// <param name="pen">Pero</param>
        /// <param name="pointStyle">Styl bodù</param>
        /// <param name="pointSize">Velikost bodù</param>
        private void DrawPoints(Graphics g, Point[] points, Pen pen, Graph.PointStyles pointStyle, int pointSize) {
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
        private void DrawPoints(Graphics g, Point[] points, Pen pen, Graph.PointStyles pointStyle, int pointSize,
            int hlStep, int hlOffset, Graph.PointStyles hlPointStyle, int hlPointSize) {

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

        #region Implementace IExportable
        /// <summary>
        /// Uloží obsah do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public virtual void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.graphParamValues, "Parametry grafu");
            param.Add(this.groupParamValues, "Parametry køivek");
            param.Add(this.itemParamValues, "Parametry pozadí");

            param.Export(export);
        }

        /// <summary>
        /// Naète obsah ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public Graph(Core.Import import) {
            this.CreateWorkers();

            IEParam param = new IEParam(import);

            if(import.VersionNumber >= 5) {
                this.graphParamValues = (GraphParameterValues)param.Get();
                this.groupParamValues = (TArray)param.Get();
                this.itemParamValues = (TArray)param.Get();

                this.graphParamValues.AddDefaultParams(globalParams);
                int nGroups = (int)this.graphParamValues[ParametersIndications.NumGroups];

                for(int g = 0; g < nGroups; g++) {
                    GraphParameterValues gv = this.groupParamValues[g] as GraphParameterValues;
                    TArray acv = this.itemParamValues[g] as TArray;

                    gv.AddDefaultParams(groupParams);
                    int nCurves = (int)gv[ParametersIndications.NumCurves];

                    for(int i = 0; i < nCurves; i++)
                        (acv[i] as GraphParameterValues).AddDefaultParams(curveParams);
                
                    this.SetMinMax(gv);
                    this.SetBorders(gv);
                }

                this.bitmap = new Bitmap[nGroups];
            }
            else if(import.VersionNumber >= 4) {
                //this.item = (TArray)param.Get();
                //this.background = (TArray)param.Get();
                //this.errors = (TArray)param.Get();

                //this.graphContext = (Context)param.Get();
                //this.itemContext = (TArray)param.Get();
                //this.groupContext = (TArray)param.Get();
            }
        }
        #endregion

        // Chyby
        private const string errorMessageBadType = "Parametr {0} grafu má špatný typ.";
        private const string errorMessageBadTypeDetail = "Požadovaný typ: {0}\nZadaný typ: {1}\nZadaná hodnota: {2}";

        private const string errorMessageBadLength = "Chybná délka vektoru s chybovými úseèkami.";
        private const string errorMessageBadLengthDetail = "Index skupiny s daty: {0}\nIndex vektoru s daty: {1}\nDélka vektoru: {2}\nDélka vektoru s úseèkami: {3}";

        private string baseFontFamilyName = "Arial";

        // Interval mezi dvìma body s èísly na ose (v obrazových bodech)
        private const int axePointInterval = 50;
        private const double multiplierMinMax = 0.02;
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