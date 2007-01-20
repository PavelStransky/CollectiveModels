using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Timers;
using System.IO;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Forms {
	/// <summary>
	/// Zobrazí graf i s pozadím
	/// </summary>
	public class GraphicsBox: System.Windows.Forms.PictureBox, IGraphControl {
		// Data k vykreslení
        private Graph graph;

        // Èasovaè pro animace
        private System.Timers.Timer timer = new System.Timers.Timer();

        // Èasovaè pro ukládání jako GIF
        private BackgroundWorker backgroundWorkerSaveGif = new BackgroundWorker();
        // Èasovaè pro ukládání jako sekvence obrázkù
        private BackgroundWorker backgroundWorkerSavePicture = new BackgroundWorker();
        // Èasovaè pro vytvoøení bitmap
        private BackgroundWorker backgroundWorkerCreate = new BackgroundWorker();

		// Bitmapa, do které se obrázek pozadí vykreslí (kvùli rychlosti)
        private Bitmap[] bitmap;

        // Minimalní a maximální hodnoty skupiny grafu
        private MinMaxCache[] minMax;

        // Minimální a maximální hodnoty pozadí
        private MinMaxCache[] minMaxB;

		// Velikosti okrajù (aby graf nepøelézal a nedotýkal se)
        private int marginL = 3 * defaultMargin;
        private int marginR = defaultMargin;
        private int marginT = defaultMargin;
        private int marginB = 3 * defaultMargin;

        // True, pokud se bude animovat èasový vývoj køivky
        private bool evalCurve = false;
        // True, pokud se bude animovat èasový vývoj skupiny
        private bool evalGroup = false;

        // Index vykreslovaných dat (pro animaci)
        private int time, group;

        // Stisknuté levé tlaèítko myši
        private bool leftMouseButton;

		/// <summary>
		/// Základní konstruktor
		/// </summary>
		public GraphicsBox() : base() {
            this.SizeMode = PictureBoxSizeMode.StretchImage;

            this.backgroundWorkerSaveGif.WorkerReportsProgress = true;
            this.backgroundWorkerSaveGif.WorkerSupportsCancellation = true;
            this.backgroundWorkerSaveGif.DoWork += new DoWorkEventHandler(backgroundWorkerSaveGif_DoWork);

            this.backgroundWorkerSavePicture.WorkerReportsProgress = true;
            this.backgroundWorkerSavePicture.WorkerSupportsCancellation = true;
            this.backgroundWorkerSavePicture.DoWork += new DoWorkEventHandler(backgroundWorkerSavePicture_DoWork);

            this.backgroundWorkerCreate.WorkerReportsProgress = true;
            this.backgroundWorkerCreate.WorkerSupportsCancellation = true;
            this.backgroundWorkerCreate.DoWork += new DoWorkEventHandler(backgroundWorkerCreate_DoWork);
        }

        #region Vytvoøení bitmap
        /// <summary>
        /// Základní pracovní metoda, která vytváøí bitmapu
        /// </summary>
        void backgroundWorkerCreate_DoWork(object sender, DoWorkEventArgs e) {
            int nGroups = this.graph.NumGroups();

            for(int g = 0; g < nGroups; g++) {
                this.backgroundWorkerCreate.ReportProgress(0, string.Format("Tvorba bitmapy {0} z {1}", g + 1, nGroups));

                Matrix matrix = this.graph.GetMatrix(g);

                if(matrix.NumItems() > 0)
                    this.bitmap[g] = this.CreateBitmap(g, matrix);
                else
                    this.bitmap = null;

                // Žádost o pøerušení procesu
                if(this.backgroundWorkerCreate.CancellationPending) 
                    break;

                if(g == this.group && this.bitmap[g] != null) {
                    this.Image = this.bitmap[g];
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Na základì matice a velikosti okna vytvoøí bitmapu;
        /// </summary>
        /// <param name="group">Index skupiny</param>
        /// <param name="matrix">Matice k vykreslení</param>
        private Bitmap CreateBitmap(int group, Matrix matrix) {
            int pointSizeX = (int)this.graph.GetGeneralParameter(paramPointSizeX, defaultPointSizeX);
            int pointSizeY = (int)this.graph.GetGeneralParameter(paramPointSizeY, defaultPointSizeY);
            bool legend = (bool)this.graph.GetGeneralParameter(paramLegend, defaultLegend);
            int legendWidth = (int)this.graph.GetGeneralParameter(paramLegendWidth, defaultLegendWidth);

            int lengthX = matrix.LengthX;
            int lengthY = matrix.LengthY;

            MinMaxCache mm = this.minMax[group];
            MinMaxCache mmB = this.minMaxB[group];

            int iMin = (int)((mm.MinX - mmB.MinX) * lengthX / mmB.Width);
            int iMax = (int)((mm.MaxX - mmB.MinX) * lengthX / mmB.Width);
            int jMin = (int)((mm.MinY - mmB.MinY) * lengthY / mmB.Height);
            int jMax = (int)((mm.MaxY - mmB.MinY) * lengthY / mmB.Height);

            int lx = iMax - iMin;
            int ly = jMax - jMin;

            int sizeX = pointSizeX * lx;
            int sizeY = pointSizeY * ly;
            int marginXMin = sizeX * this.marginL / 1000 + 2;
            int marginXMax = sizeX * this.marginR / 1000 + 1;
            int marginYMin = sizeY * this.marginB / 1000 + 2;
            int marginYMax = sizeY * this.marginT / 1000 + 1;

            int id = iMin;
            int jd = jMin;

            iMin = System.Math.Max(0, iMin);
            iMax = System.Math.Min(lengthX, iMax);
            jMin = System.Math.Max(0, jMin);
            jMax = System.Math.Min(lengthY, jMax);

            Color colorBackground = (Color)this.graph.GetGeneralParameter(paramBackgroundColor, defaultBackgroundColor);
            Bitmap result = new Bitmap(sizeX + (legend ? legendWidth : 0) + marginXMin + marginXMax, sizeY + marginYMin + marginYMax);
            Graphics.FromImage(result).FillRectangle(new Pen(colorBackground).Brush, 0, 0, result.Width, result.Height);

            Color colorPlus = (Color)this.graph.GetBackgroundParameter(group, paramColorPlus, defaultColorPlus);
            Color colorZero = (Color)this.graph.GetBackgroundParameter(group, paramColorZero, defaultColorZero);
            Color colorMinus = (Color)this.graph.GetBackgroundParameter(group, paramColorMinus, defaultColorMinus);

            double maxAbs = System.Math.Abs(matrix.MaxAbs());

            for(int i = iMin; i < iMax; i++) {
                for(int j = jMin; j < jMax; j++) {
                    double m = matrix[i, lengthY - j - 1] / maxAbs;

                    Color color;
                    if(m < 0) 
                        color = Color.FromArgb(
                            (int)((1 + m) * colorZero.R - m * colorMinus.R),
                            (int)((1 + m) * colorZero.G - m * colorMinus.G),
                            (int)((1 + m) * colorZero.B - m * colorMinus.B));
                    else
                        color = Color.FromArgb(
                            (int)((1 - m) * colorZero.R + m * colorPlus.R),
                            (int)((1 - m) * colorZero.G + m * colorPlus.G),
                            (int)((1 - m) * colorZero.B + m * colorPlus.B));

                    int i1 = (i - id) * pointSizeX + marginXMin;
                    int j1 = (j - jd) * pointSizeY + marginYMax;

                    for(int k = 0; k < pointSizeX; k++)
                        for(int l = 0; l < pointSizeY; l++)
                            result.SetPixel(i1 + k, j1 + l, color);
                }

                // Žádost o pøerušení procesu
                if(this.backgroundWorkerCreate.CancellationPending)
                    break;

                this.backgroundWorkerCreate.ReportProgress(i * 100 / lengthX);
            }

            // Vykreslení legendy
            if(legend) {
                for(int i = 5; i < legendWidth - 5; i++) {
                    int intervalY = (sizeY - 50) / 2;
                    for(int j = 0; j < intervalY; j++) {
                        Color color = Color.FromArgb(
                            ((1 - j) * colorZero.R + j * colorPlus.R) / intervalY,
                            ((1 - j) * colorZero.G + j * colorPlus.G) / intervalY,
                            ((1 - j) * colorZero.B + j * colorPlus.B) / intervalY);
                        result.SetPixel(i + sizeX, intervalY - j + 25, color);
                    }

                    for(int j = 0; j < intervalY; j++) {
                        Color color = Color.FromArgb(
                            ((1 - j) * colorZero.R + j * colorMinus.R) / intervalY,
                            ((1 - j) * colorZero.G + j * colorMinus.G) / intervalY,
                            ((1 - j) * colorZero.B + j * colorMinus.B) / intervalY);
                        result.SetPixel(i + sizeX, intervalY + j + 25, color);
                    }
                }

                // Text
                Graphics g = Graphics.FromImage(result);
                string sMin = string.Format("-{0,4:0.000}", maxAbs);
                string sMax = string.Format("{0,4:0.000}", maxAbs);
                int fontHeight = (int)g.MeasureString(sMin, baseFont).Height;
                int fontWidthMin = (int)g.MeasureString(sMin, baseFont).Width;
                int fontWidthMax = (int)g.MeasureString(sMax, baseFont).Width;
                g.DrawString(sMax, baseFont, Brushes.Black, sizeX + 5 + (legendWidth - fontWidthMax) / 2, 20 - fontHeight);
                g.DrawString(sMin, baseFont, Brushes.Black, sizeX + 5 + (legendWidth - fontWidthMin) / 2, sizeY - 20);
            }

            return result;
        }
        #endregion

        #region Uložení obrázku
        /// <summary>
        /// Uloží jako sekvenci obrázkù
        /// </summary>
        /// <param name="fName">Jméno souboru</param>
        public void SavePicture(string fName) {
            (this.Parent.Parent as GraphForm).NewProcess("Ukládání obrázku :", this.backgroundWorkerSavePicture, fName);
        }

        /// <summary>
        /// Základní pracovní metoda, která ukládá obrázek
        /// </summary>        
        void backgroundWorkerSavePicture_DoWork(object sender, DoWorkEventArgs e) {
/*            string fName = e.Argument as string;

            if(fName.Length < 3 || fName.IndexOf('.') < 0)
                throw new FormsException(string.Format(errorMessageBadFileName, fName));

            string name = fName.Substring(0, fName.LastIndexOf('.'));
            string extension = fName.Substring(name.Length + 1, fName.Length - name.Length).ToLower();
            ImageFormat format = ImageFormat.Png;

            if(extension == "gif")
                format = ImageFormat.Gif;
            else if(extension == "jpg" || extension == "jpeg")
                format = ImageFormat.Jpeg;
            else if(extension == "png")
                format = ImageFormat.Png;

            // Více obrázkù - spojíme dohromady pøípady s èasovým vývojem køivky a s postupným vývojem pozadí
            if(this.eval && (this.graph.NumGroups() > 1 || this.evalCurves)) {
                System.Drawing.Image image = null;

                for(int i = 0; i < this.maxIndex; i++){
                    // Vyvíjíme všechny køivky
                    if(this.evalCurves) {
                        image = new Bitmap(this.bitmap[0], this.Width, this.Height);
                        this.PaintGraph(Graphics.FromImage(image), -1, i);
                    }
                    else {
                        image = new Bitmap(this.bitmap[i], this.Width, this.Height);
                        this.PaintGraph(Graphics.FromImage(image), i, -1);
                    }

                    image.Save(string.Format("{0}{1}.{2}", name, i, extension), format);

                    this.backgroundWorkerSavePicture.ReportProgress(i * 100 / this.maxIndex);

                    // Požadavek ukonèení procesu
                    if(this.backgroundWorkerSavePicture.CancellationPending)
                        break;       
                }
            }
            // Jeden obrázek
            else {
                System.Drawing.Image image = new Bitmap(this.bitmap[0], this.Width, this.Height);
                this.PaintGraph(Graphics.FromImage(image), -1, -1);
                image.Save(fName, format);

                this.backgroundWorkerSaveGif.ReportProgress(100);
            }
*/        }

        /// <summary>
        /// Uloží jako GIF
        /// </summary>
        /// <param name="fName">Jméno souboru</param>
        public void SaveGIF(string fName) {
            (this.Parent.Parent as GraphForm).NewProcess("Ukládání GIF :", this.backgroundWorkerSaveGif, fName);
        }

        /// <summary>
        /// Základní pracovní metoda, která ukládá obrázek
        /// </summary>
        private void backgroundWorkerSaveGif_DoWork(object sender, DoWorkEventArgs e) {
/*            string fName = e.Argument as string;

            // Více obrázkù - spojíme dohromady pøípady s èasovým vývojem køivky a s postupným vývojem pozadí
            if(this.eval && (this.graph.Count > 1 || this.evalCurves)) {
                int interval = (int)(double)this.graph.GetGeneralParameter(paramInterval, defaultInterval) / 10;

                MemoryStream m = new MemoryStream();
                FileStream f = new FileStream(fName, FileMode.Create);
                BinaryWriter b = new BinaryWriter(f);

                byte[] buf1;
                byte[] buf2 = new Byte[19];
                byte[] buf3 = new Byte[8];
                buf2[0] = 33;                   // extension introducer
                buf2[1] = 255;                  // application extension
                buf2[2] = 11;                   // size of block
                buf2[3] = 78;                   // N
                buf2[4] = 69;                   // E
                buf2[5] = 84;                   // T
                buf2[6] = 83;                   // S
                buf2[7] = 67;                   // C
                buf2[8] = 65;                   // A
                buf2[9] = 80;                   // P
                buf2[10] = 69;                  // E
                buf2[11] = 50;                  // 2
                buf2[12] = 46;                  // .
                buf2[13] = 48;                  // 0
                buf2[14] = 3;                   // Size of block
                buf2[15] = 1;                   //
                buf2[16] = 0;                   //
                buf2[17] = 0;                   //
                buf2[18] = 0;                   // Block terminator

                buf3[0] = 33;                   // Extension introducer
                buf3[1] = 249;                  // Graphic control extension
                buf3[2] = 4;                    // Size of block
                buf3[3] = 9;                    // Flags: reserved, disposal method, user input, transparent color
                buf3[4] = (byte)(interval % 256);// Delay time low byte
                buf3[5] = (byte)(interval / 256);// Delay time high byte
                buf3[6] = 255;                  // Transparent color index
                buf3[7] = 0;                    // Block terminator

                for(int i = 0; i < this.maxIndex; i++) {
                    System.Drawing.Image image = null;

                    if(this.evalCurves) {
                        image = new Bitmap(this.bitmap[0], this.Width, this.Height);
                        this.PaintGraph(Graphics.FromImage(image), -1, i);
                    }
                    else {
                        image = new Bitmap(this.bitmap[i], this.Width, this.Height);
                        this.PaintGraph(Graphics.FromImage(image), i, -1);
                    }

                    image.Save(m, ImageFormat.Gif);

                    buf1 = m.ToArray();

                    if(i == 0) {
                        //only write these the first time....
                        b.Write(buf1, 0, 781); //Header & global color table
                        b.Write(buf2, 0, 19); //Application extension
                    }

                    b.Write(buf3, 0, 8); //Graphic extension
                    b.Write(buf1, 789, buf1.Length - 790); //Image data

                    m.SetLength(0);
                    this.backgroundWorkerSaveGif.ReportProgress(i * 100 / this.maxIndex);

                    // Požadavek ukonèení procesu
                    if(this.backgroundWorkerSaveGif.CancellationPending)
                        break;
                }

                b.Write((byte)0x3B); //Image terminator
                b.Close();
                f.Close();
                m.Close();
            }

            else {
                System.Drawing.Image image = new Bitmap(this.bitmap[0], this.Width, this.Height);
                this.PaintGraph(Graphics.FromImage(image), -1, -1);
                image.Save(fName, ImageFormat.Gif);

                this.backgroundWorkerSaveGif.ReportProgress(100);
            }
 */       }
        #endregion

        /// <param name="graph">Objekt grafu</param>
        public GraphicsBox(Graph graph)
            : this() {
            this.SetGraph(graph);
        }

        /// <summary>
        /// Vrátí objekt s daty
        /// </summary>
        public Graph GetGraph() {
            return this.graph;
        }

		/// <summary>
		/// Nastaví øadu vektorù k zobrazení
		/// </summary>
		/// <param name="graph">Objekt grafu</param>
		public void SetGraph(Graph graph) {
			this.graph = graph;

            this.marginL = (bool)this.graph.GetGeneralParameter(paramShowAxeY, defaultShowAxeY) ? defaultMarginWithAxeL : defaultMargin;
            this.marginR = defaultMargin;
            this.marginT = defaultMargin;
            this.marginB = (bool)this.graph.GetGeneralParameter(paramShowAxeX, defaultShowAxeX) ? defaultMarginWithAxeB : defaultMargin;

            this.evalGroup = (bool)this.graph.GetGeneralParameter(paramEvaluateGroup, defaultEvaluateGroup);
            this.evalCurve = (bool)this.graph.GetGeneralParameter(paramEvaluateCurve, defaultEvaluateCurve);

            this.group = 0;

            this.SetMinMax();

            int nGroups = this.graph.NumGroups();

            this.bitmap = new Bitmap[nGroups];
            (this.Parent.Parent as GraphForm).NewProcess(string.Empty, this.backgroundWorkerCreate);            

            this.time = this.minMax[this.group].MaxLength;

            if(this.evalGroup || nGroups > 1) {
                if(this.evalCurve) 
                    this.time = 1;

                this.timer.Elapsed -= timer_Elapsed;
                this.timer.Interval = (double)this.graph.GetGeneralParameter(paramInterval, defaultInterval);
                this.timer.AutoReset = true;
                this.timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                this.timer.Start();
            }

			this.Invalidate();
		}

        /// <summary>
        /// Event èasovaèe - postupné vykreslování køivky
        /// </summary>
        void timer_Elapsed(object sender, ElapsedEventArgs e) {
            int nGroups = this.graph.NumGroups();
            bool moveGroup = this.evalGroup;
            bool changed = moveGroup;

            if(this.evalCurve) {
                this.time++;
                if(this.time > this.minMax[this.group].MaxLength) {
                    this.time = 1;
                    moveGroup = true;
                }
                changed = true;
            }

            if(moveGroup) {
                this.group = (this.group + 1) % nGroups;

                if(!this.evalCurve)
                    this.time = this.minMax[this.group].MaxLength;
            }

            if(changed)
                this.Invalidate();
        }

        /// <summary>
        /// Nastaví pole s minimálními a maximálními hodnotami
        /// </summary>
        public void SetMinMax() {
            int nGroups = this.graph.NumGroups();
            this.minMax = new MinMaxCache[nGroups];
            this.minMaxB = new MinMaxCache[nGroups];

            for(int g = 0; g < nGroups; g++) {
                Vector minmax = this.graph.GetMinMax(g);

                if(!object.Equals(minmax, null)) {
                    minmax[0] = (double)this.graph.GetGeneralParameter(paramMinX, minmax[0]);
                    minmax[1] = (double)this.graph.GetGeneralParameter(paramMaxX, minmax[1]);
                    minmax[2] = (double)this.graph.GetGeneralParameter(paramMinY, minmax[2]);
                    minmax[3] = (double)this.graph.GetGeneralParameter(paramMaxY, minmax[3]);
                }
                else {
                    minmax = new Vector(4);

                    minmax[0] = (double)this.graph.GetGeneralParameter(paramMinX, defaultMinX);
                    minmax[1] = (double)this.graph.GetGeneralParameter(paramMaxX, defaultMaxX);
                    minmax[2] = (double)this.graph.GetGeneralParameter(paramMinY, defaultMinY);
                    minmax[3] = (double)this.graph.GetGeneralParameter(paramMaxY, defaultMaxY);
                }

                Vector minmaxB = new Vector(4);
                minmaxB[0] = (double)this.graph.GetBackgroundParameter(group, paramMinXBackground, minmax[0]);
                minmaxB[1] = (double)this.graph.GetBackgroundParameter(group, paramMaxXBackground, minmax[1]);
                minmaxB[2] = (double)this.graph.GetBackgroundParameter(group, paramMinYBackground, minmax[2]);
                minmaxB[3] = (double)this.graph.GetBackgroundParameter(group, paramMaxYBackground, minmax[3]);

                Matrix m = this.graph.GetMatrix(g);
                if(m.NumItems() > 0) {
                    if(double.IsNaN(minmaxB[0]))
                        minmaxB[0] = 0;
                    if(double.IsNaN(minmaxB[1]))
                        minmaxB[1] = m.LengthX;
                    if(double.IsNaN(minmaxB[2]))
                        minmaxB[2] = 0;
                    if(double.IsNaN(minmaxB[3]))
                        minmaxB[3] = m.LengthY;

                    if(double.IsNaN(minmax[0]))
                        minmax[0] = minmaxB[0];
                    if(double.IsNaN(minmax[1]))
                        minmax[1] = minmaxB[1];
                    if(double.IsNaN(minmax[2]))
                        minmax[2] = minmaxB[2];
                    if(double.IsNaN(minmax[3]))
                        minmax[3] = minmaxB[3];
                }

                this.minMax[g] = new MinMaxCache(minmax, this.graph.GetMaxLength(g));
                this.minMaxB[g] = new MinMaxCache(minmaxB);
            }
        }

        /// <summary>
        /// Šíøka bez okrajù
        /// </summary>
        private int WidthWM { get { return (int)(this.Width * (1.0 - (this.marginL + this.marginR) / 1000.0)); } }

        /// <summary>
        /// Výška bez okrajù
        /// </summary>
        private int HeightWM { get { return (int)(this.Height * (1.0 - (this.marginT + this.marginB) / 1000.0)); } }

        /// <summary>
        /// Velikost horního okraje v pixelech
        /// </summary>
        private int AbsMarginT { get { return this.marginT * this.Height / 1000; } }

        /// <summary>
        /// Velikost dolního okraje v pixelech
        /// </summary>
        private int AbsMarginB { get { return this.marginB * this.Height / 1000; } }

        /// <summary>
        /// Velikost levého okraje v pixelech
        /// </summary>
        private int AbsMarginL { get { return this.marginL * this.Width / 1000; } }

        /// <summary>
        /// Velikost pravého okraje v pixelech
        /// </summary>
        private int AbsMarginR { get { return this.marginR * this.Width / 1000; } }
        
        /// <summary>
		/// Vypoèítá offset takový, aby se celý graf právì vešel do okna
		/// </summary>
        /// <param name="group">Index skupiny grafu</param>
		private double GetFitOffsetY(int group) {
			return this.minMax[group].MaxY * this.HeightWM / this.minMax[group].Height + this.AbsMarginT;
		}

		/// <summary>
		/// Vypoèítá offset takový, aby se celý graf právì vešel do okna
		/// </summary>
        /// <param name="group">Index skupiny grafu</param>
        private double GetFitOffsetX(int group) {
            return -this.minMax[group].MinX * this.WidthWM / this.minMax[group].Width + this.AbsMarginL;
		}

		/// <summary>
		/// Vypoèítá zvìtšení takové, aby se celý graf právì vešel do okna
		/// </summary>
        /// <param name="group">Index skupiny grafu</param>
        private double GetFitAmplifyY(int group) {
            return this.HeightWM / this.minMax[group].Height;
		}

		/// <summary>
		/// Vypoèítá zvìtšení takové, aby se celý graf právì vešel do okna
		/// </summary>
        /// <param name="group">Index skupiny grafu</param>
        private double GetFitAmplifyX(int group) {
            return this.WidthWM / this.minMax[group].Width;
		}

		/// <summary>
		/// Pøekreslení
		/// </summary>
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint (e);
            this.PaintGraph(e.Graphics, this.group, this.time);
        }

        /// <summary>
        /// Provede vykreslení grafu
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="time">Èas k vykreslení, -1 pro vykreslení všeho</param>
        /// <param name="group">Skupina køivek pro vykreslení</param>
        private void PaintGraph(Graphics g, int group, int time) {
            // Barva pozadí
            if(this.Image == null) {
                Color backgroundColor = (Color)this.graph.GetGeneralParameter(paramBackgroundColor, defaultBackgroundColor);
                Brush backgroundBrush = (new Pen(backgroundColor)).Brush;
                g.FillRectangle(backgroundBrush, this.ClientRectangle);
            }

            bool shift = (bool)this.graph.GetGeneralParameter(paramShift, defaultShift);

            double offsetX = this.GetFitOffsetX(group);
            double amplifyX = this.GetFitAmplifyX(group);
            double offsetY;
            double amplifyY;

            if(!shift) {
                offsetY = this.GetFitOffsetY(group);
                amplifyY = this.GetFitAmplifyY(group);
            }
            else {
                offsetY = 0.0;
                amplifyY = baseAmplifyY;
            }

            int nCurves = this.graph.NumCurves(group);

            if(nCurves > 0) {
                int stringHeight = (int)g.MeasureString("M", baseFont).Height;

                for(int i = 0; i < nCurves; i++) {
                    if(shift)
                        offsetY = (i + 1) * this.HeightWM / (nCurves + 2) + this.AbsMarginT;

                    Point[] p = this.graph.PointArrayToDraw(group, i, offsetX, amplifyX, offsetY, amplifyY, time);

                    Color lineColor = (Color)this.graph.GetCurveParameter(group, i, paramLineColor, defaultLineColor);
                    float lineWidth = (float)this.graph.GetCurveParameter(group, i, paramLineWidth, defaultLineWidth);
                    Graph.LineStyles lineStyle = (Graph.LineStyles)this.graph.GetCurveParameter(group, i, paramLineStyle, defaultLineStyle);
                    Color pointColor = (Color)this.graph.GetCurveParameter(group, i, paramPointColor, defaultPointColor);
                    Graph.PointStyles pointStyle = (Graph.PointStyles)this.graph.GetCurveParameter(group, i, paramPointStyle, defaultPointStyle);
                    int pointSize = (int)this.graph.GetCurveParameter(group, i, paramPointSize, defaultPointSize);

                    Pen linePen = new Pen(lineColor, lineWidth);
                    Pen pointPen = new Pen(pointColor);

                    if(p.Length >= 2)
                        this.DrawLine(g, p, linePen, lineStyle);

                    // Chybové úseèky
                    if(this.graph.IsErrors(group, i)) {
                        Point[] errorPoints = this.graph.GetErrorLines(group, i, offsetX, amplifyX, offsetY, amplifyY);
                        Point[] ep = new Point[2];
                        int length = errorPoints.Length / 2;

                        for(int j = 0; j < length; j++) {
                            ep[0] = errorPoints[2 * j];
                            ep[1] = errorPoints[2 * j + 1];
                            this.DrawLine(g, ep, pointPen, Graph.LineStyles.Line);
                        }

                        this.DrawPoints(g, errorPoints, pointPen, Graph.PointStyles.HLines, 5);
                    }

                    bool showLabels = (bool)this.graph.GetCurveParameter(group, i, paramShowLabel, defaultShowLabel);

                    if(showLabels) {
                        Color labelColor = (Color)this.graph.GetCurveParameter(group, i, paramLabelColor, defaultLabelColor);
                        string lineName = (string)this.graph.GetCurveParameter(group, i, paramLineName, defaultLineName);
                        Brush labelBrush = (new Pen(labelColor)).Brush;
                        g.DrawString(lineName, baseFont, labelBrush, this.AbsMarginL, (float)(offsetY - stringHeight / 2.0));
                    }

                    this.DrawPoints(g, p, pointPen, pointStyle, (int)pointSize);

                    // První a poslední bod
                    if(p.Length > 0) {
                        Point[] lastPoint = new Point[1]; lastPoint[0] = p[p.Length - 1];
                        Color lastPointColor = (Color)this.graph.GetCurveParameter(group, i, paramLastPointColor, defaultLastPointColor);
                        Graph.PointStyles lastPointStyle = (Graph.PointStyles)this.graph.GetCurveParameter(group, i, paramLastPointStyle, defaultLastPointStyle);
                        int lastPointSize = (int)this.graph.GetCurveParameter(group, i, paramLastPointSize, defaultLastPointSize);
                        Pen lastPointPen = new Pen(lastPointColor);
                        this.DrawPoints(g, lastPoint, lastPointPen, lastPointStyle, lastPointSize);

                        Point[] firstPoint = new Point[1]; firstPoint[0] = p[0];
                        Color firstPointColor = (Color)this.graph.GetCurveParameter(group, i, paramFirstPointColor, defaultFirstPointColor);
                        Graph.PointStyles firstPointStyle = (Graph.PointStyles)this.graph.GetCurveParameter(group, i, paramFirstPointStyle, defaultFirstPointStyle);
                        int firstPointSize = (int)this.graph.GetCurveParameter(group, i, paramFirstPointSize, defaultFirstPointSize);
                        Pen firstPointPen = new Pen(firstPointColor);
                        this.DrawPoints(g, firstPoint, firstPointPen, firstPointStyle, firstPointSize);
                    }
                }
            }
            bool showAxeX = (bool)this.graph.GetGeneralParameter(paramShowAxeX, defaultShowAxeX);
            bool showAxeY = (bool)this.graph.GetGeneralParameter(paramShowAxeY, defaultShowAxeY);

            // X - ová osa
            if(showAxeX) {
                int y = this.Height - this.AbsMarginB;

                Point[] line = new Point[2];
                line[0] = new Point(this.AbsMarginL, y);
                line[1] = new Point(this.Width - this.AbsMarginR, y);

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
                Vector v = this.GetAxesPoints(out smallIntervals, out smallIntervalsOffset, this.WidthWM, this.minMax[group].MinX, this.minMax[group].MaxX);

                Brush numBrush = linePen.Brush;
                Point[] p = new Point[v.Length];
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

            // Y - ová osa
            if(showAxeY && !shift) {
                int x = this.AbsMarginL;

                Point[] line = new Point[2];
                line[0] = new Point(x, this.AbsMarginT);
                line[1] = new Point(x, this.Height - this.AbsMarginB);

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
                Vector v = this.GetAxesPoints(out smallIntervals, out smallIntervalsOffset, this.HeightWM, this.minMax[group].MinY, this.minMax[group].MaxY);

                Brush numBrush = linePen.Brush;
                Point[] p = new Point[v.Length];
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

			Vector result = new Vector((int)(wholeInterval / interval));

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
		/// Nakreslí body (bez význaèných bodù)
		/// </summary>
		/// <param name="g">Objekt Graphics</param>
		/// <param name="points">Body</param>
		/// <param name="pen">Pero</param>
		/// <param name="pointStyle">Styl bodù</param>
		/// <param name="pointSize">Velikost bodù</param>
		private void DrawPoints(Graphics g, Point [] points, Pen pen, Graph.PointStyles pointStyle, int pointSize) {
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
        /// Nastaví ToolTip
        /// </summary>
        /// <param name="x">X - ová souøadnice myši</param>
        /// <param name="y">Y - ová souøadnice myši</param>
        public string ToolTip(int x, int y) {
            double offsetX = this.GetFitOffsetX(this.group);
            double amplifyX = this.GetFitAmplifyX(this.group);
            double offsetY = this.GetFitOffsetY(this.group);
            double amplifyY = this.GetFitAmplifyY(this.group);

            double xl = (x - offsetX) / amplifyX;
            double yl = -(y - offsetY) / amplifyY;

            Matrix m = this.graph.GetMatrix(this.group);
            if(m.NumItems() > 0) {
                double id = (xl - this.minMaxB[this.group].MinX) * m.LengthX / this.minMaxB[this.group].Width;
                double jd = (yl - this.minMaxB[this.group].MinY) * m.LengthY / this.minMaxB[this.group].Height;

                if(id >= 0.0 && id < m.LengthX && jd >= 0.0 && jd < m.LengthY)
                    return string.Format("({0,5:F}, {1,5:F}) = {2,5:F}", xl, yl, m[(int)id, (int)jd]);
            }

            return string.Format("({0,5:F}, {1,5:F})", xl, yl);
        }

		/// <summary>
		/// Pøi zmìnì velikosti
		/// </summary>
        protected override void OnSizeChanged(EventArgs e) {
            base.OnSizeChanged(e);
            this.Invalidate();
        }

        /// <summary>
        /// Zmìna skupiny
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);

            // S tlaèítkem vyvíjíme køivku
            if(this.leftMouseButton) {
                int newTime = this.time + System.Math.Sign(e.Delta);
                int maxTime = this.minMax[this.group].MaxLength;
                if(newTime <= 0)
                    newTime = 1;
                else if(newTime > maxTime)
                    newTime = maxTime;
                this.time = newTime;
            }

            else {
                int nGroups = this.graph.NumGroups();
                int newGroup = this.group + System.Math.Sign(e.Delta);
                if(newGroup < 0)
                    newGroup = 0;
                if(newGroup >= nGroups)
                    newGroup = nGroups - 1;
                this.group = newGroup;

                if(this.evalCurve)
                    this.time = 1;
                else
                    this.time = this.minMax[this.group].MaxLength;
            }

            this.Invalidate();
        }

        /// <summary>
        /// Stisk tlaèítka myši
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            if(e.Button == MouseButtons.Left)
                this.leftMouseButton = true;
        }

        /// <summary>
        /// Uvolnìní tlaèítka myši
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);

            if(e.Button == MouseButtons.Left)
                this.leftMouseButton = false;
        }

        /// <summary>
        /// Musíme nastavit focus, jinak nefunguje OnMouseWheel
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if(!this.Focused)
                this.Focus();
        }

		private Font baseFont = new Font("Arial", 8);
		private const double baseAmplifyY = 8;
        // Okraj v promilích
		private const int defaultMargin = 20;
		private const int defaultMarginWithAxeL = 70;
		private const int defaultMarginWithAxeB = 100;
		// Interval mezi dvìma body s èísly na ose (v obrazových bodech)
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
        private const string paramBackgroundColor = "bcolor";

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

        private const string paramEvaluateGroup = "evalgroup";
        private const string paramEvaluateCurve = "evalcurve";
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

        private const string paramColorZero = "colorzero";
        private const string paramColorPlus = "colorplus";
        private const string paramColorMinus = "colorminus";
        private const string paramLegend = "legend";
        private const string paramLegendWidth = "legendwidth";

        // Pozadí
        private const string paramMinXBackground = "minxb";
        private const string paramMaxXBackground = "maxxb";
        private const string paramMinYBackground = "minyb";
        private const string paramMaxYBackground = "maxyb";

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
        private static Color defaultBackgroundColor = Color.FromName("white");

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

        private static Color defaultColorZero = Color.FromName("black");
        private static Color defaultColorPlus = Color.FromName("blue");
        private static Color defaultColorMinus = Color.FromName("red");
        private const bool defaultLegend = false;
        private const int defaultLegendWidth = 50;
        private const string defaultTitle = "";

        private const bool defaultEvaluateGroup = false;
        private const bool defaultEvaluateCurve = false;
        private const double defaultInterval = 1000.0;

        private const double defaultMinXBackground = double.NaN;
        private const double defaultMaxXBackground = defaultMinX;
        private const double defaultMinYBackground = defaultMinX;
        private const double defaultMaxYBackground = defaultMinX;

        private const string errorMessageBadFileName = "Chybný název souboru pro uložení obrázku: {0}";
    }
}