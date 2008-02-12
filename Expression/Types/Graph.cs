using System;
using System.Drawing;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.Expression.Functions;

namespace PavelStransky.Expression {
    /// <summary>
    /// Tøída, která zapouzdøuje graf (urèuje jeho obsah, popø. další parametry)
    /// </summary>
    public partial class Graph : IExportable, ICloneable {
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
        /// Poèet køivek grafu
        /// </summary>
        public int[] NumCurves {
            get {
                int nGroups = (int)this.graphParamValues[ParametersIndications.NumGroups];
                int[] result = new int[nGroups];

                for(int i = 0; i < nGroups; i++)
                    result[i] = (int)(this.groupParamValues[i] as GraphParameterValues)[ParametersIndications.NumCurves];

                return result;
            }
        }

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
        /// Vrací true, pokud je daný graf pod danými souøadnicemi
        /// </summary>
        /// <param name="x">X - ová souøadnice</param>
        /// <param name="y">Y - ová souøadnice</param>
        /// <param name="group">Èíslo skupiny</param>
        /// <param name="rectangle">Obdélník, ve kterém je zobrazení</param>
        public bool IsActive(int group, Rectangle rectangle, int x, int y) {
            GraphParameterValues gv = this.groupParamValues[group] as GraphParameterValues;

            rectangle = this.RelativeWindow(rectangle);
            Rectangle rectangleM = this.AddMargins(gv, rectangle);

            if(x >= rectangleM.Left && x <= rectangleM.Right
                && y >= rectangleM.Top && y <= rectangleM.Bottom)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Pøevede souøadnice okna (myši) na souøadnice skuteèné
        /// </summary>
        /// <param name="x">X - ová souøadnice</param>
        /// <param name="y">Y - ová souøadnice</param>
        /// <param name="group">Èíslo skupiny</param>
        /// <param name="rectangle">Obdélník, ve kterém je zobrazení</param>
        public PointD CoordinatesFromPosition(int group, Rectangle rectangle, int x, int y) {
            GraphParameterValues gv = this.groupParamValues[group] as GraphParameterValues;

            rectangle = this.RelativeWindow(rectangle);
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
        /// Nastaví relativní pozici okna (pokud není zadána v parametrech grafu)
        /// </summary>
        /// <param name="left">Levý okraj</param>
        /// <param name="top">Pravý okraj</param>
        /// <param name="width">Šíøka</param>
        /// <param name="height">Výška</param>
        public void SetRelativeWindow(double left, double top, double width, double height) {
            if(this.graphParamValues.IsDefault(ParametersIndications.RWindowL))
                this.graphParamValues[ParametersIndications.RWindowL] = left;
            if(this.graphParamValues.IsDefault(ParametersIndications.RWindowT))
                this.graphParamValues[ParametersIndications.RWindowT] = top;
            if(this.graphParamValues.IsDefault(ParametersIndications.RWindowW))
                this.graphParamValues[ParametersIndications.RWindowW] = width;
            if(this.graphParamValues.IsDefault(ParametersIndications.RWindowH))
                this.graphParamValues[ParametersIndications.RWindowH] = height;
        }

        /// <summary>
        /// Nastaví nové parametry grafu
        /// </summary>
        /// <param name="graphContext">Parametry celého grafu</param>
        /// <param name="itemContext">Parametry jednotlivých køivek grafu</param>
        /// <param name="groupContext">Parametry skupin</param>
        public void SetParams(Context graphContext, TArray groupContext, TArray itemContext) {
            int nGroups = (int)this.graphParamValues[ParametersIndications.NumGroups];

            int gcl = groupContext.Length;
            int icl = itemContext.Length;

            this.graphParamValues.SetParams(graphContext, null, null);

            for(int g = 0; g < nGroups; g++) {
                GraphParameterValues gv = this.groupParamValues[g] as GraphParameterValues;
                TArray cv = this.itemParamValues[g] as TArray;

                Context gc = gcl > g ? groupContext[g] as Context : null;
                TArray aic = icl > g ? itemContext[g] as TArray : null;

                gv.SetParams(gc, graphContext, null);

                int nCurves = (int)gv[ParametersIndications.NumCurves];

                int icll = icl > g ? aic.Length : 0;

                for(int i = 0; i < nCurves; i++)
                    (cv[i] as GraphParameterValues).SetParams(icll > i ? aic[i] as Context : null, gc, graphContext);
            }
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
            this.CreateWorker();

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

                    this.SetColors(cv, i);
                    this.SetColorFncBuffer(cv);
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
        /// Nastaví barvy køivkám a bodùm
        /// </summary>
        /// <param name="cv">Parametry køivky</param>
        /// <param name="i">Index køivky</param>
        private void SetColors(GraphParameterValues cv, int i) {
            bool isDefLineColor = cv.IsDefault(ParametersIndications.LColor);
            bool isDefPointColor = cv.IsDefault(ParametersIndications.PColor);

            if(isDefLineColor)
                cv[ParametersIndications.LColor] = ColorArray.GetColor(i);

            if(isDefPointColor)
                cv[ParametersIndications.PColor] = ColorArray.GetColor(i);
        }

        /// <summary>
        /// Pokud máme funkci pro barvy, vytvoøí barvový buffer
        /// </summary>
        /// <param name="cv">Parametry køivky</param>
        private void SetColorFncBuffer(GraphParameterValues cv) {
            string colorFnc = (string)cv[ParametersIndications.PColorFnc];

            if(colorFnc != string.Empty) {
                PointVector data = (PointVector)cv[ParametersIndications.DataCurves];

                int num = data.Length;
                TArray buffer = new TArray(typeof(Color), num);
                int i = 0;

                try {
                    Function fnc = new Function(string.Format("getvar({0}(x; y); cf)", colorFnc), null);
                    Context c = new Context();

                    for(i = 0; i < num; i++) {
                        PointD p = data[i];
                        c.SetVariable("x", p.X);
                        c.SetVariable("y", p.Y);

                        Vector v = (fnc.Evaluate(c) as Variable).Item as Vector;
                        buffer[i] = Color.FromArgb((int)v[0], (int)v[1], (int)v[2]);
                    }
                }

                catch(Exception) {
                    Color dc = (Color)cv[ParametersIndications.PColor];

                    for(; i < num; i++)
                        buffer[i] = dc;
                }

                cv[ParametersIndications.PColorFncBuffer] = buffer;
            }
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

            // Bude legenda k obrázku pozadí
            bool isBLegend = (bool)gv[ParametersIndications.BLegend];

            if(isBLegend) {
                int m = (int)gv[ParametersIndications.MarginR];

                m += (int)gv[ParametersIndications.BLegendWidth];
                m += 10;

                gv[ParametersIndications.MarginR] = m;
            }

            // Bude legenda ke køivkám
            bool isCLegend = (bool)gv[ParametersIndications.CLegend];

            if(isCLegend) {
                int m = (int)gv[ParametersIndications.MarginR];

                m += (int)gv[ParametersIndications.CLegendWidth];
                m += 10;

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

                bool isDefBColorMinValue = gv.IsDefault(ParametersIndications.BColorMinValue);
                bool isDefBColorMiddleValue = gv.IsDefault(ParametersIndications.BColorMiddleValue);
                bool isDefBColorMaxValue = gv.IsDefault(ParametersIndications.BColorMaxValue);

                double mMax = m.Max();
                double mMin = m.Min();

                if(isDefBColorMinValue)
                    gv[ParametersIndications.BColorMinValue] = mMin;
                if(isDefBColorMiddleValue)
                    gv[ParametersIndications.BColorMiddleValue] = (mMax + mMin) / 2.0;
                if(isDefBColorMaxValue)
                    gv[ParametersIndications.BColorMaxValue] = mMax;

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

            // Hranice pro legendu
            if(gv.IsDefault(ParametersIndications.BLegendMinY))
                gv[ParametersIndications.BLegendMinY] = minY;
            if(gv.IsDefault(ParametersIndications.BLegendMaxY))
                gv[ParametersIndications.BLegendMaxY] = maxY;

            // Uložení zmìn
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
            this.CreateWorker();

            IEParam param = new IEParam(import);

            if(import.VersionNumber >= 6) {
                this.graphParamValues = (GraphParameterValues)param.Get();
                this.groupParamValues = (TArray)param.Get();
                this.itemParamValues = (TArray)param.Get();

                if(this.graphParamValues != null && this.groupParamValues != null && this.itemParamValues != null) {
                    this.graphParamValues.AddDefaultParams(globalParams);
                    int nGroups = (int)this.graphParamValues[ParametersIndications.NumGroups];

                    for(int g = 0; g < nGroups; g++) {
                        GraphParameterValues gv = this.groupParamValues[g] as GraphParameterValues;
                        TArray acv = this.itemParamValues[g] as TArray;

                        gv.AddDefaultParams(groupParams);
                        int nCurves = (int)gv[ParametersIndications.NumCurves];

                        for(int i = 0; i < nCurves; i++) {
                            (acv[i] as GraphParameterValues).AddDefaultParams(curveParams);
                            this.SetColors(acv[i] as GraphParameterValues, i);
                        }

                        this.SetMinMax(gv);
                        this.SetBorders(gv);
                    }

                    this.bitmap = new Bitmap[nGroups];
                }
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

        #region Implementace klonování
        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        private Graph() {
            this.CreateWorker();
        }

        public object Clone() {
            Graph result = new Graph();

            result.graphParamValues = (GraphParameterValues)this.graphParamValues.Clone();
            result.groupParamValues = (TArray)this.groupParamValues.Clone();
            result.itemParamValues = (TArray)this.itemParamValues.Clone();

            result.bitmap = (Bitmap[])this.bitmap.Clone();

            return result;
        }
        #endregion
    }
}