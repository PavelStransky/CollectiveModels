using System;
using System.Drawing;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.Expression.Functions;

namespace PavelStransky.Expression {
    /// <summary>
    /// Tøída, která zapouzdøuje graf (urèuje jeho obsah, popø. další parametry)
    /// </summary>
    public partial class Graph {
        /// <summary>
        /// Vytvoøí øadu bodù k vykreslení
        /// </summary>
        /// <param name="data">Data k vykreslení</param>
        /// <param name="offset">Posunutí vhledem k osám</param>
        /// <param name="amplify">Zesílení v osách</param>
        /// <param name="maxLength">Maximální délka dat</param>
        public Point[] PointArrayToDraw(PointVector data, PointD offset, PointD amplify, int maxLength) {
            Point[] result = new Point[0];

            if(data.Length > 0) {
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
        /// Daný obdélník zmenší podle definovaného relativního okna
        /// </summary>
        /// <param name="rectangle">Obdélník okna</param>
        public Rectangle RelativeWindow(Rectangle rectangle) {
            rectangle.X += (int)(rectangle.Width * (double)this.graphParamValues[ParametersIndications.RWindowL]);
            rectangle.Y += (int)(rectangle.Height * (double)this.graphParamValues[ParametersIndications.RWindowT]);
            rectangle.Width = (int)(rectangle.Width * (double)this.graphParamValues[ParametersIndications.RWindowW]);
            rectangle.Height = (int)(rectangle.Height * (double)this.graphParamValues[ParametersIndications.RWindowH]);

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

            bool shift = (bool)this.graphParamValues[ParametersIndications.Shift];

            rectangle = this.RelativeWindow(rectangle);
            Rectangle rectangleM = this.AddMargins(gv, rectangle);

            Color backgroundColor = (Color)gv[ParametersIndications.BColor];
            Brush backgroundBrush = (new Pen(backgroundColor)).Brush;
            g.FillRectangle(backgroundBrush, rectangle);

            if(this.bitmap[group] != null) {
                RectangleF rf = new RectangleF(
                    (float)(double)gv[ParametersIndications.PixelL],
                    (float)(double)gv[ParametersIndications.PixelT],
                    (float)(double)gv[ParametersIndications.PixelW],
                    (float)(double)gv[ParametersIndications.PixelH]);

                lock(this.bitmap[group]) {
                    g.DrawImage(this.bitmap[group], rectangleM, rf, GraphicsUnit.Pixel);
                }
            }

            PointD offset = this.GetFitOffset(gv, rectangleM);
            PointD amplify = this.GetFitAmplify(gv, rectangleM);

            if(shift) {
                offset.Y = 0.0;
                amplify.Y = 8.0;
            }

            int nCurves = (int)gv[ParametersIndications.NumCurves];
            Matrix background = (Matrix)gv[ParametersIndications.DataBackground];

            // Legenda ke køivkám - potøebujeme její šíøku
            bool cLegend = (bool)gv[ParametersIndications.CLegend];
            int cLegendWidth = (int)gv[ParametersIndications.CLegendWidth];
            
            // Legenda k pozadí
            bool bLegend = (bool)gv[ParametersIndications.BLegend];

            if(bLegend) {
                double bLegendMinY = (double)gv[ParametersIndications.BLegendMinY];
                double bLegendMaxY = (double)gv[ParametersIndications.BLegendMaxY];
                int bLegendWidth = (int)gv[ParametersIndications.BLegendWidth];

                double bColorMinValue = (double)gv[ParametersIndications.BColorMinValue];
                double bColorMaxValue = (double)gv[ParametersIndications.BColorMaxValue];

                int bLegendB = (int)(-bLegendMinY * amplify.Y + offset.Y);
                int bLegendT = (int)(-bLegendMaxY * amplify.Y + offset.Y);
                
                double bLegendCoef = (bColorMaxValue - bColorMinValue) / (bLegendB - bLegendT);

                int bLegendR = rectangle.Right - 5 - (cLegend ? cLegendWidth + 10 : 0);
                int bLegendL = bLegendR - bLegendWidth;

                BColor bcolor = new BColor(gv);

                for(int i = bLegendT; i < bLegendB; i++) {
                    double v = bColorMaxValue - (i - bLegendT) * bLegendCoef;
                    Pen p = new Pen(bcolor[v]);
                    g.DrawLine(p, bLegendL, i, bLegendR, i);
                }

                // Label k legendì
                bool bLegendLabel = (bool)gv[ParametersIndications.BLegendLabel];

                if(bLegendLabel) {
                    Color bLegendFColor = (Color)gv[ParametersIndications.BLegendFColor];
                    Brush bLegendBrush = (new Pen(bLegendFColor)).Brush;
                    Font font = new Font(baseFontFamilyName, (int)gv[ParametersIndications.BLegendFSize]);

                    string s = bColorMaxValue.ToString("0.000");
                    SizeF fSize = g.MeasureString(s, font);
                    g.DrawString(s, font, bLegendBrush, bLegendL + (bLegendWidth - fSize.Width) / 2, bLegendT - fSize.Height);

                    s = bColorMinValue.ToString("0.000");
                    fSize = g.MeasureString(s, font);
                    g.DrawString(s, font, bLegendBrush, bLegendL + (bLegendWidth - fSize.Width) / 2, bLegendB);
                }
            }

            // Legenda ke køivkám
            if(cLegend) {
                int cLegendR = rectangle.Right - 5;
                int cLegendL = cLegendR - cLegendWidth;

                Color cLegendFColor = (Color)gv[ParametersIndications.CLegendFColor];
                Brush cLegendBrush = (new Pen(cLegendFColor)).Brush;
                Font font = new Font(baseFontFamilyName, (int)gv[ParametersIndications.CLegendFSize]);

                SizeF fSize = g.MeasureString("0", font);
                int d = (int)(fSize.Height + 4);

                Point[] p = new Point[4];
                for(int i = 0; i < 4; i++)
                    p[i] = new Point(cLegendL + i * cLegendWidth / 9, rectangleM.Bottom - d / 2);

                for(int i = nCurves - 1; i >= 0; i--) {
                    GraphParameterValues iv = aiv[i] as GraphParameterValues;

                    Color lineColor = (Color)iv[ParametersIndications.LColor];
                    float lineWidth = (int)iv[ParametersIndications.LWidth];
                    Graph.LineStyles lineStyle = (Graph.LineStyles)iv[ParametersIndications.LStyle];
                    Color pointColor = (Color)iv[ParametersIndications.PColor];
                    Graph.PointStyles pointStyle = (Graph.PointStyles)iv[ParametersIndications.PStyle];
                    int pointSize = (int)iv[ParametersIndications.PSize];

                    Pen linePen = new Pen(lineColor, lineWidth);
                    Pen pointPen = new Pen(pointColor);

                    this.SetDashStyle(linePen, iv[ParametersIndications.LDash]);

                    string s = (string)iv[ParametersIndications.LName];
                    g.DrawString(s, font, cLegendBrush, cLegendL + cLegendWidth / 3 + 5, rectangleM.Bottom - (nCurves - i) * d + 2);

                    this.DrawPoints(g, p, pointPen, pointStyle, (int)pointSize);
                    this.DrawLine(g, p, linePen, lineStyle);

                    for(int j = 0; j < 4; j++)
                        p[j].Y -= d;
                }
            }

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

                    bool clip = (bool)iv[ParametersIndications.Clip];
                    if(clip)
                        g.SetClip(rectangleM);

                    Pen linePen = new Pen(lineColor, lineWidth);
                    Pen pointPen = new Pen(pointColor);

                    this.SetDashStyle(linePen, iv[ParametersIndications.LDash]);

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

                    // Barva bodù podle funkce
                    string colorFnc = (string)iv[ParametersIndications.PColorFnc];
                    TArray colorBuffer = (TArray)iv[ParametersIndications.PColorFncBuffer];

                    if(colorFnc == string.Empty || colorBuffer == null)
                        this.DrawPoints(g, p, pointPen, pointStyle, (int)pointSize);
                    else
                        this.DrawPoints(g, p, colorBuffer, pointStyle, pointSize);

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

                    if(clip)
                        g.ResetClip();
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
                int xL = rectangle.Left + 5;
                int xR = rectangle.Right - 5;

                Font font = new Font(baseFontFamilyName, (int)gv[ParametersIndications.ATitleFSizeY], FontStyle.Bold | FontStyle.Italic);
                Color titleColor = (Color)gv[ParametersIndications.ATitleColorY];
                Brush titleBrush = new Pen(titleColor).Brush;

                SizeF p = g.MeasureString(titleY, font);
                y -= (int)(p.Height / 2F);
                xR -= (int)p.Width;

                if(showAxeL)
                    g.DrawString(titleY, font, titleBrush, xL, y);

                if(showAxeR)
                    g.DrawString(titleY, font, titleBrush, xR, y);
            }

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
            if(numPoints <= 0) {
                smallIntervals = 0;
                smallIntervalsOffset = 0;
                return new Vector(0);
            }

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

            int length = (int)(max / interval) - (int)(min / interval);
            if(min * max <= 0)
                length++;

            Vector result = new Vector(length);

            for(int i = 1; i <= length; i++)
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
        /// Nakreslí body s danou vybarvovací funkcí
        /// </summary>
        /// <param name="g">Objekt Graphics</param>
        /// <param name="points">Body</param>
        /// <param name="colorBuffer">Buffer s barvami bodù</param>
        /// <param name="pointStyle">Styl bodù</param>
        /// <param name="pointSize">Velikost bodù</param>
        private void DrawPoints(Graphics g, Point[] points, TArray colorBuffer, Graph.PointStyles pointStyle, int pointSize) {
            int num = points.Length;
            for(int i = 0; i < num; i++) {
                Pen pen = new Pen((Color)colorBuffer[i]);
                this.DrawPoint(g, points[i], pen, pointStyle, pointSize);
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

        /// <summary>
        /// Nastaví pro dané pero styl èáry
        /// </summary>
        /// <param name="pen">Dané pero</param>
        /// <param name="dash">Objekt pro dash</param>
        private void SetDashStyle(Pen pen, object dash) {
            if(dash is Vector) {
                Vector v = dash as Vector;
                int vl = v.Length;
                float[] f = new float[vl];
                for(int j = 0; j < vl; j++)
                    f[j] = (float)v[j];
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
                pen.DashPattern = f;
            }
            else if(dash is System.Drawing.Drawing2D.DashStyle)
                pen.DashStyle = (System.Drawing.Drawing2D.DashStyle)dash;
        }

        private string baseFontFamilyName = "Arial";

        // Interval mezi dvìma body s èísly na ose (v obrazových bodech)
        private const int axePointInterval = 50;
        private const double multiplierMinMax = 0.02;
    }
}