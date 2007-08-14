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
	/// ZobrazÌ graf i s pozadÌm
	/// </summary>
	public class GraphicsBox: System.Windows.Forms.PictureBox, IGraphControl {
		// Data k vykreslenÌ
        private Graph graph;

        // »asovaË pro animace
        private System.Timers.Timer timer = new System.Timers.Timer();

        // »asovaË pro ukl·d·nÌ jako GIF
        private BackgroundWorker backgroundWorkerSaveGif = new BackgroundWorker();
        // »asovaË pro ukl·d·nÌ jako sekvence obr·zk˘
        private BackgroundWorker backgroundWorkerSavePicture = new BackgroundWorker();
        // »asovaË pro vytvo¯enÌ bitmap
        private BackgroundWorker backgroundWorkerCreate = new BackgroundWorker();

		// Bitmapa, do kterÈ se obr·zek pozadÌ vykreslÌ (kv˘li rychlosti)
        private Bitmap[] bitmap;

        // MinimalnÌ a maxim·lnÌ hodnoty skupiny grafu
        private MinMaxCache[] minMax;

        // Minim·lnÌ a maxim·lnÌ hodnoty pozadÌ
        private MinMaxCache[] minMaxB;

		// Velikosti okraj˘ (aby graf nep¯elÈzal a nedot˝kal se)
        private int marginL = 3 * defaultMargin;
        private int marginR = defaultMargin;
        private int marginT = defaultMargin;
        private int marginB = 3 * defaultMargin;

        // True, pokud se bude animovat Ëasov˝ v˝voj k¯ivky
        private bool evalCurve = false;
        // True, pokud se bude animovat Ëasov˝ v˝voj skupiny
        private bool evalGroup = false;

        // Index vykreslovan˝ch dat (pro animaci)
        private int time, group;

        // StisknutÈ levÈ tlaËÌtko myöi
        private bool leftMouseButton;

		/// <summary>
		/// Z·kladnÌ konstruktor
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

            this.timer.AutoReset = true;
            this.timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        }

        #region Vytvo¯enÌ bitmap
        /// <summary>
        /// Z·kladnÌ pracovnÌ metoda, kter· vytv·¯Ì bitmapu
        /// </summary>
        void backgroundWorkerCreate_DoWork(object sender, DoWorkEventArgs e) {
            int nGroups = this.graph.NumGroups();

            for(int g = 0; g < nGroups; g++) {
                this.backgroundWorkerCreate.ReportProgress(0, string.Format("Tvorba bitmapy {0} z {1}", g + 1, nGroups));

                Matrix matrix = this.graph.GetMatrix(g);

                if(matrix.NumItems() > 0)
                    this.bitmap[g] = this.CreateBitmap(g, matrix);
                else
                    this.bitmap[g] = null;

                // é·dost o p¯eruöenÌ procesu
                if(this.backgroundWorkerCreate.CancellationPending) 
                    break;

                if(g == this.group && this.bitmap[g] != null) {
                    this.Image = this.bitmap[g];
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Na z·kladÏ matice a velikosti okna vytvo¯Ì bitmapu;
        /// </summary>
        /// <param name="group">Index skupiny</param>
        /// <param name="matrix">Matice k vykreslenÌ</param>
        private Bitmap CreateBitmap(int group, Matrix matrix) {
            int pointSizeX = (int)this.graph.GetBackgroundParameter(group, paramPointSizeX, defaultPointSizeX);
            int pointSizeY = (int)this.graph.GetBackgroundParameter(group, paramPointSizeY, defaultPointSizeY);
            bool legend = (bool)this.graph.GetBackgroundParameter(group, paramLegend, defaultLegend);
            int legendWidth = (int)this.graph.GetBackgroundParameter(group, paramLegendWidth, defaultLegendWidth);

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

            int sizeX = (int)(pointSizeX * lx / (1.0 - (this.marginL + this.marginR) / 1000.0) + 0.5);
            int sizeY = (int)(pointSizeY * ly / (1.0 - (this.marginT + this.marginB) / 1000.0) + 0.5);
            int marginROld = this.marginR;

            if(legend) {
                double d = 1.0 - (this.marginL + this.marginR) / 1000.0;
                this.marginR += (int)(1000.0 * d / (1.0 + lx * pointSizeX / (d * legendWidth)) + 0.5);
                sizeX = (int)(pointSizeX * lx / (1.0 - (this.marginL + this.marginR) / 1000.0) + 0.5);
            }

            int marginXMaxOld = sizeX * marginROld / 1000 + 1;
            int marginXMin = sizeX * this.marginL / 1000 + 1;
            int marginXMax = sizeX * this.marginR / 1000 + 1;
            int marginYMin = sizeY * this.marginB / 1000 + 1;
            int marginYMax = sizeY * this.marginT / 1000 + 1;

            int id = iMin;
            int jd = jMin;

            iMin = System.Math.Max(0, iMin);
            iMax = System.Math.Min(lengthX, iMax);
            jMin = System.Math.Max(0, jMin);
            jMax = System.Math.Min(lengthY, jMax);

            Color colorBackground = (Color)this.graph.GetGeneralParameter(paramBackgroundColor, defaultBackgroundColor);
            Bitmap result = new Bitmap(sizeX, sizeY);
            Graphics.FromImage(result).FillRectangle(new Pen(colorBackground).Brush, 0, 0, result.Width, result.Height);

            Color colorPlus = (Color)this.graph.GetBackgroundParameter(group, paramColorPlus, defaultColorPlus);
            Color colorZero = (Color)this.graph.GetBackgroundParameter(group, paramColorZero, defaultColorZero);
            Color colorMinus = (Color)this.graph.GetBackgroundParameter(group, paramColorMinus, defaultColorMinus);

            double maxAbs = System.Math.Abs(this.minMaxB[group].MaxAbsValue);

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
                            if(i1 + k < sizeX && j1 + l < sizeY)
                                result.SetPixel(i1 + k, j1 + l, color);
                }

                // é·dost o p¯eruöenÌ procesu
                if(this.backgroundWorkerCreate.CancellationPending)
                    break;

                this.backgroundWorkerCreate.ReportProgress(i * 100 / lengthX);
            }

            // VykreslenÌ legendy
            if(legend) {
                for(int i = marginXMaxOld; i < legendWidth - marginXMaxOld - 1; i++) {
                    int intervalY = (sizeY - (3 * marginYMin + marginYMax)) / 2;
                    for(int j = 0; j < intervalY; j++) {
                        Color color = Color.FromArgb(
                            ((intervalY - j) * colorZero.R + j * colorPlus.R) / intervalY,
                            ((intervalY - j) * colorZero.G + j * colorPlus.G) / intervalY,
                            ((intervalY - j) * colorZero.B + j * colorPlus.B) / intervalY);
                        result.SetPixel(i + sizeX - marginXMax, intervalY - j + marginYMin + marginYMax, color);
                    }

                    for(int j = 0; j < intervalY; j++) {
                        Color color = Color.FromArgb(
                            ((intervalY - j) * colorZero.R + j * colorMinus.R) / intervalY,
                            ((intervalY - j) * colorZero.G + j * colorMinus.G) / intervalY,
                            ((intervalY - j) * colorZero.B + j * colorMinus.B) / intervalY);
                        result.SetPixel(i + sizeX - marginXMax, intervalY + j + marginYMin + marginYMax, color);
                    }
                }
            }

            return result;
        }
        #endregion

        #region UloûenÌ obr·zku
        /// <summary>
        /// UloûÌ jako sekvenci obr·zk˘
        /// </summary>
        /// <param name="fName">JmÈno souboru</param>
        public void SavePicture(string fName) {
            (this.Parent.Parent as GraphForm).NewProcess("Ukl·d·nÌ obr·zku :", this.backgroundWorkerSavePicture, fName);
        }

        /// <summary>
        /// Vr·tÌ obr·zek pozadÌ p¯eökl·lovan˝ v nov˝ch rozmÏrech.
        /// Pokud obr·zek pozadÌ neexistuje, vytvo¯Ì obr·zek pr·zdn˝
        /// </summary>
        /// <param name="group">Index skupiny</param>
        private Image GetResizedImage(int group) {
            Image result;
            if(this.bitmap[group] != null)
                result = new Bitmap(this.bitmap[group], this.Width, this.Height);
            else
                result = new Bitmap(this.Width, this.Height);
            return result;
        }

        /// <summary>
        /// Z·kladnÌ pracovnÌ metoda, kter· ukl·d· obr·zek
        /// </summary>        
        void backgroundWorkerSavePicture_DoWork(object sender, DoWorkEventArgs e) {
            string fName = e.Argument as string;

            if(fName.Length < 3 || fName.IndexOf('.') < 0)
                throw new FormsException(string.Format(errorMessageBadFileName, fName));

            string name = fName.Substring(0, fName.LastIndexOf('.'));
            string extension = fName.Substring(name.Length + 1, fName.Length - name.Length - 1).ToLower();
            ImageFormat format = ImageFormat.Png;

            if(extension == "gif")
                format = ImageFormat.Gif;
            else if(extension == "jpg" || extension == "jpeg")
                format = ImageFormat.Jpeg;
            else if(extension == "png")
                format = ImageFormat.Png;
            else if(extension == "wmf")
                format = ImageFormat.Wmf;

            int nGroups = this.graph.NumGroups();

            // Ukl·d·me vöechny obr·zky
            if(nGroups > 1) {
                for(int g = 0; g < nGroups; g++) {
                    // VyvÌjÌme k¯ivky
                    if(this.evalCurve) {
                        int maxTime = this.minMax[g].MaxLength;
                        for(int t = 0; t <= maxTime; t++) {
                            Image image = this.GetResizedImage(g);
                            this.PaintGraph(Graphics.FromImage(image), g, t);
                            image.Save(string.Format("{0}{1}-{2}.{3}", name, g, t, extension), format);
                        }
                    }
                    else {
                        Image image = this.GetResizedImage(g);
                        this.PaintGraph(Graphics.FromImage(image), g, -1);
                        image.Save(string.Format("{0}{1}.{2}", name, g, extension), format);
                    }

                    this.backgroundWorkerSavePicture.ReportProgress(g * 100 / nGroups);

                    // Poûadavek ukonËenÌ procesu
                    if(this.backgroundWorkerSavePicture.CancellationPending)
                        break;
                }
            }

                // Jeden obr·zek
            else {
                // VyvÌjÌme k¯ivky
                if(this.evalCurve) {
                    int maxTime = this.minMax[this.group].MaxLength;
                    for(int t = 0; t <= maxTime; t++) {
                        Image image = this.GetResizedImage(this.group);
                        this.PaintGraph(Graphics.FromImage(image), this.group, t);
                        image.Save(string.Format("{0}-{1}.{2}", name, t, extension), format);

                        this.backgroundWorkerSavePicture.ReportProgress(t * 100 / maxTime);

                        // Poûadavek ukonËenÌ procesu
                        if(this.backgroundWorkerSavePicture.CancellationPending)
                            break;
                    }
                }
                else {
                    Image image = this.GetResizedImage(this.group);
                    this.PaintGraph(Graphics.FromImage(image), this.group, -1);
                    image.Save(string.Format("{0}.{1}", name, extension), format);

                    this.backgroundWorkerSavePicture.ReportProgress(100);
                }
            }
        }

        /// <summary>
        /// UloûÌ jako GIF
        /// </summary>
        /// <param name="fName">JmÈno souboru</param>
        public void SaveGIF(string fName) {
            (this.Parent.Parent as GraphForm).NewProcess("Ukl·d·nÌ GIF :", this.backgroundWorkerSaveGif, fName);
        }

        private static byte[] buf2, buf3;

        /// <summary>
        /// Statick˝ konstruktor
        /// </summary>
        static GraphicsBox() {
            buf2 = new Byte[19];
            buf3 = new Byte[8];
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
            buf3[6] = 255;                  // Transparent color index
            buf3[7] = 0;                    // Block terminator
        }

        /// <summary>
        /// P¯id· GIF do pamÏùovÈho streamu
        /// </summary>
        private void AddGIF(MemoryStream m, BinaryWriter b, Image image, bool first) {
            image.Save(m, ImageFormat.Gif);
            byte[] buf1 = m.ToArray();

            if(first) {
                //only write these the first time....
                b.Write(buf1, 0, 781); //Header & global color table
                b.Write(buf2, 0, 19); //Application extension
                first = false;
            }

            b.Write(buf3, 0, 8); //Graphic extension
            b.Write(buf1, 789, buf1.Length - 790); //Image data

            m.SetLength(0);
        }

        /// <summary>
        /// Z·kladnÌ pracovnÌ metoda, kter· ukl·d· obr·zek
        /// </summary>
        private void backgroundWorkerSaveGif_DoWork(object sender, DoWorkEventArgs e) {
            string fName = e.Argument as string;
            int nGroups = this.graph.NumGroups();

            if(this.evalGroup || this.evalCurve) {
                int interval = (int)this.graph.GetGeneralParameter(paramInterval, defaultInterval) / 10;

                buf3[4] = (byte)(interval % 256);// Delay time low byte
                buf3[5] = (byte)(interval / 256);// Delay time high byte

                MemoryStream m = new MemoryStream();
                FileStream f = new FileStream(fName, FileMode.Create);
                BinaryWriter b = new BinaryWriter(f);

                bool first = true;

                if(this.evalGroup && nGroups > 1) {
                    for(int g = 0; g < nGroups; g++) {
                        // VyvÌjÌme k¯ivky
                        if(this.evalCurve) {
                            int maxTime = this.minMax[g].MaxLength;
                            for(int t = 0; t <= maxTime; t++) {
                                Image image = this.GetResizedImage(g);
                                this.PaintGraph(Graphics.FromImage(image), g, t);
                                this.AddGIF(m, b, image, first);
                                first = false;
                            }
                        }
                        else {
                            Image image = this.GetResizedImage(g);
                            this.PaintGraph(Graphics.FromImage(image), g, -1);
                            this.AddGIF(m, b, image, first);
                            first = false;
                        }

                        this.backgroundWorkerSaveGif.ReportProgress(g * 100 / nGroups);

                        // Poûadavek ukonËenÌ procesu
                        if(this.backgroundWorkerSaveGif.CancellationPending)
                            break;
                    }
                }
                else {
                    // VyvÌjÌme k¯ivky
                    if(this.evalCurve) {
                        int maxTime = this.minMax[this.group].MaxLength;
                        for(int t = 0; t <= maxTime; t++) {
                            Image image = this.GetResizedImage(this.group);
                            this.PaintGraph(Graphics.FromImage(image), this.group, t);
                            this.AddGIF(m, b, image, first);
                            first = false;

                            this.backgroundWorkerSaveGif.ReportProgress(t * 100 / maxTime);

                            // Poûadavek ukonËenÌ procesu
                            if(this.backgroundWorkerSaveGif.CancellationPending)
                                break;
                        }
                    }
                }
                b.Write((byte)0x3B); //Image terminator
                b.Close();
                f.Close();
                m.Close();
            }

            else {
                Image image = this.GetResizedImage(this.group);
                this.PaintGraph(Graphics.FromImage(image), this.group, -1);
                image.Save(fName, ImageFormat.Gif);

                this.backgroundWorkerSaveGif.ReportProgress(100);
            }
        }
        #endregion

        /// <param name="graph">Objekt grafu</param>
        public GraphicsBox(Graph graph)
            : this() {
            this.SetGraph(graph);
        }

        /// <summary>
        /// Vr·tÌ objekt s daty
        /// </summary>
        public Graph GetGraph() {
            return this.graph;
        }

		/// <summary>
		/// NastavÌ ¯adu vektor˘ k zobrazenÌ
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

                this.timer.Interval = (int)this.graph.GetGeneralParameter(paramInterval, defaultInterval);
                this.timer.Start();
            }

			this.Invalidate();
		}

        /// <summary>
        /// Event ËasovaËe - postupnÈ vykreslov·nÌ k¯ivky
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
                if(this.bitmap[this.group] != null)
                    this.Image = this.bitmap[this.group];
                else
                    this.Image = new Bitmap(1, 1);
            
                if(!this.evalCurve)
                    this.time = this.minMax[this.group].MaxLength;
            }

            if(changed)
                this.Invalidate();
        }

        /// <summary>
        /// NastavÌ pole s minim·lnÌmi a maxim·lnÌmi hodnotami
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

                // Minim·lnÌ a maxim·lnÌ hodnoty nesmÌ b˝t stejnÈ
                if(minmax[0] == minmax[1]) {
                    if(minmax[0] < 0.0) {
                        minmax[0] *= 2.0;
                        minmax[1] = 0.0;
                    }
                    else if(minmax[0] > 0) {
                        minmax[0] = 0.0;
                        minmax[1] *= 2.0;
                    }
                    else {
                        minmax[0] = -1.0;
                        minmax[1] = 1.0;
                    }
                }
                if(minmax[2] == minmax[3]) {
                    if(minmax[2] < 0.0) {
                        minmax[2] *= 2.0;
                        minmax[3] = 0.0;
                    }
                    else if(minmax[2] > 0) {
                        minmax[2] = 0.0;
                        minmax[3] *= 2.0;
                    }
                    else {
                        minmax[2] = -1.0;
                        minmax[3] = 1.0;
                    }
                }

                Vector minmaxB = new Vector(4);
                minmaxB[0] = (double)this.graph.GetBackgroundParameter(group, paramMinXBackground, minmax[0]);
                minmaxB[1] = (double)this.graph.GetBackgroundParameter(group, paramMaxXBackground, minmax[1]);
                minmaxB[2] = (double)this.graph.GetBackgroundParameter(group, paramMinYBackground, minmax[2]);
                minmaxB[3] = (double)this.graph.GetBackgroundParameter(group, paramMaxYBackground, minmax[3]);

                Matrix m = this.graph.GetMatrix(g);
                double maxAbs = 0.0;

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

                    maxAbs = m.MaxAbs();
                }

                this.minMax[g] = new MinMaxCache(minmax, this.graph.GetMaxLength(g));
                this.minMaxB[g] = new MinMaxCache(minmaxB, maxAbs);
            }
        }

        /// <summary>
        /// äÌ¯ka bez okraj˘
        /// </summary>
        private int WidthWM { get { return (int)(this.Width * (1.0 - (this.marginL + this.marginR) / 1000.0)); } }

        /// <summary>
        /// V˝öka bez okraj˘
        /// </summary>
        private int HeightWM { get { return (int)(this.Height * (1.0 - (this.marginT + this.marginB) / 1000.0)); } }

        /// <summary>
        /// Velikost hornÌho okraje v pixelech
        /// </summary>
        private int AbsMarginT { get { return this.marginT * this.Height / 1000; } }

        /// <summary>
        /// Velikost dolnÌho okraje v pixelech
        /// </summary>
        private int AbsMarginB { get { return this.marginB * this.Height / 1000; } }

        /// <summary>
        /// Velikost levÈho okraje v pixelech
        /// </summary>
        private int AbsMarginL { get { return this.marginL * this.Width / 1000; } }

        /// <summary>
        /// Velikost pravÈho okraje v pixelech
        /// </summary>
        private int AbsMarginR { get { return this.marginR * this.Width / 1000; } }
        
        /// <summary>
		/// VypoËÌt· offset takov˝, aby se cel˝ graf pr·vÏ veöel do okna
		/// </summary>
        /// <param name="group">Index skupiny grafu</param>
		private double GetFitOffsetY(int group) {
			return this.minMax[group].MaxY * this.HeightWM / this.minMax[group].Height + this.AbsMarginT;
		}

		/// <summary>
		/// VypoËÌt· offset takov˝, aby se cel˝ graf pr·vÏ veöel do okna
		/// </summary>
        /// <param name="group">Index skupiny grafu</param>
        private double GetFitOffsetX(int group) {
            return -this.minMax[group].MinX * this.WidthWM / this.minMax[group].Width + this.AbsMarginL;
		}

		/// <summary>
		/// VypoËÌt· zvÏtöenÌ takovÈ, aby se cel˝ graf pr·vÏ veöel do okna
		/// </summary>
        /// <param name="group">Index skupiny grafu</param>
        private double GetFitAmplifyY(int group) {
            return this.HeightWM / this.minMax[group].Height;
		}

		/// <summary>
		/// VypoËÌt· zvÏtöenÌ takovÈ, aby se cel˝ graf pr·vÏ veöel do okna
		/// </summary>
        /// <param name="group">Index skupiny grafu</param>
        private double GetFitAmplifyX(int group) {
            return this.WidthWM / this.minMax[group].Width;
		}

		/// <summary>
		/// P¯ekreslenÌ
		/// </summary>
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint (e);
            this.PaintGraph(e.Graphics, this.group, this.time);
        }

        /// <summary>
        /// Provede vykreslenÌ grafu
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="time">»as k vykreslenÌ, -1 pro vykreslenÌ vöeho</param>
        /// <param name="group">Skupina k¯ivek pro vykreslenÌ</param>
        private void PaintGraph(Graphics g, int group, int time) {
            // Barva pozadÌ
            if(this.bitmap[group] == null) {
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

                    // ChybovÈ ˙seËky
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
                        Font font = new Font(baseFontFamilyName, (int)this.graph.GetCurveParameter(group, i, paramLabelFontSize, defaultLabelFontSize));
                        float stringHeight = g.MeasureString(lineName, font).Height;
                        g.DrawString(lineName, font, labelBrush, this.AbsMarginL, (float)(offsetY - stringHeight / 2.0));
                    }

                    this.DrawPoints(g, p, pointPen, pointStyle, (int)pointSize);

                    // PrvnÌ a poslednÌ bod
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

            // X - ov· osa
            if(showAxeX) {
                int y = this.Height - this.AbsMarginB;

                Point[] line = new Point[2];
                line[0] = new Point(this.AbsMarginL, y);
                line[1] = new Point(this.Width - this.AbsMarginR, y);

                Color lineColorX = (Color)this.graph.GetGeneralParameter(paramLineColorX, defaultLineColorX);
                float lineWidthX = (float)this.graph.GetGeneralParameter(paramLineWidthX, defaultLineWidthX);
                Pen linePen = new Pen(lineColorX, lineWidthX);

                Color pointColorX = (Color)this.graph.GetGeneralParameter(paramPointColorX, defaultPointColorX);
                Pen pointPen = new Pen(pointColorX, lineWidthX);

                Graph.PointStyles pointStyleX = (Graph.PointStyles)this.graph.GetGeneralParameter(paramPointStyleX, defaultPointStyleX);
                int pointSizeX = (int)this.graph.GetGeneralParameter(paramPointSizeX, defaultPointSizeX);

                Font font = new Font(baseFontFamilyName, (int)this.graph.GetGeneralParameter(paramFontSizeX, defaultFontSizeX), FontStyle.Bold);

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
                    float nsWidth = g.MeasureString(numString, font).Width;
                    g.DrawString(numString, font, numBrush, p[i].X - (int)(nsWidth / 2), y + 5);
                }


                this.DrawPoints(g, p, pointPen, pointStyleX, pointSizeX, smallIntervals, smallIntervalsOffset,
                    pointStyleX, pointSizeX / 2);
            }

            // Y - ov· osa
            if(showAxeY && !shift) {
                int x = this.AbsMarginL;

                Point[] line = new Point[2];
                line[0] = new Point(x, this.AbsMarginT);
                line[1] = new Point(x, this.Height - this.AbsMarginB);

                Color lineColorY = (Color)this.graph.GetGeneralParameter(paramLineColorY, defaultLineColorY);
                float lineWidthY = (float)this.graph.GetGeneralParameter(paramLineWidthY, defaultLineWidthY);
                Pen linePen = new Pen(lineColorY, lineWidthY);

                Color pointColorY = (Color)this.graph.GetGeneralParameter(paramPointColorY, defaultPointColorY);
                Pen pointPen = new Pen(pointColorY, lineWidthY);

                Graph.PointStyles pointStyleY = (Graph.PointStyles)this.graph.GetGeneralParameter(paramPointStyleY, defaultPointStyleY);
                int pointSizeY = (int)this.graph.GetGeneralParameter(paramPointSizeY, defaultPointSizeY);

                Font font = new Font(baseFontFamilyName, (int)this.graph.GetGeneralParameter(paramFontSizeY, defaultFontSizeY), FontStyle.Bold);

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
                    SizeF nsSize = g.MeasureString(numString, font);
                    g.DrawString(numString, font, numBrush, x - nsSize.Width - 5, p[i].Y - nsSize.Height / 2);
                }

                this.DrawPoints(g, p, pointPen, pointStyleY, pointSizeY, smallIntervals, smallIntervalsOffset, pointStyleY, pointSizeY / 2);
            }

            // Legenda
            bool showLegend = (bool)this.graph.GetBackgroundParameter(group, paramLegend, defaultLegend);
            if(showLegend && this.graph.GetMatrix(group).NumItems() > 0) {
                Color colorPlus = (Color)this.graph.GetBackgroundParameter(group, paramColorPlus, defaultColorPlus);
                Color colorMinus = (Color)this.graph.GetBackgroundParameter(group, paramColorMinus, defaultColorMinus);
                Pen legendPlus = new Pen(colorPlus);
                Pen legendMinus = new Pen(colorMinus);

                Font font = new Font(baseFontFamilyName, (int)this.graph.GetBackgroundParameter(group, paramLegendFontSize, defaultLegendFontSize));

                double maxAbs = System.Math.Abs(this.minMaxB[group].MaxAbsValue);
                string legendString1 = string.Format("{0,5:F}", maxAbs);
                string legendString2 = string.Format("{0,5:F}", -maxAbs);
                SizeF lSize1 = g.MeasureString(legendString1, font);
                SizeF lSize2 = g.MeasureString(legendString2, font);

                int x1 = this.WidthWM + this.AbsMarginL + (int)((this.AbsMarginR - lSize1.Width) / 2F);
                int y1 = (int)(2 * this.AbsMarginT + this.AbsMarginB - 1.5F * lSize1.Height);

                int x2 = this.WidthWM + this.AbsMarginL + (int)((this.AbsMarginR - lSize2.Width) / 2F);
                int y2 = (int)(this.Height - 2 * this.AbsMarginB - this.AbsMarginT + 0.5F * lSize2.Height);

                g.DrawString(legendString1, font, legendPlus.Brush, x1, y1);
                g.DrawString(legendString2, font, legendMinus.Brush, x2, y2);
            }

            string subTitle = (string)this.graph.GetBackgroundParameter(group, paramSubTitle, defaultSubTitle);
            if(subTitle != string.Empty) {
                Color subTitleColor = (Color)this.graph.GetBackgroundParameter(group, paramSubTitleColor, defaultSubTitleColor);
                Pen subTitlePen = new Pen(subTitleColor);
                Font font = new Font(baseFontFamilyName, (int)this.graph.GetBackgroundParameter(group, paramSubTitleFontSize, defaultSubTitleFontSize), FontStyle.Bold);
                SizeF size = g.MeasureString(subTitle, font);

                int x = (int)((this.WidthWM - size.Width) / 2) + this.AbsMarginL;
                int y = this.AbsMarginT;

                g.DrawString(subTitle, font, subTitlePen.Brush, x, y);
            }
        }

		/// <summary>
		/// Vytvo¯Ì body, ve kter˝ch bude label na ose
		/// </summary>
		/// <param name="pixels">PoËet bod˘ na obrazovce</param>
		/// <param name="min">Minim·lnÌ hodnota</param>
		/// <param name="max">Maxim·lnÌ hodnota</param>
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
		/// NakreslÌ Ë·ru
		/// </summary>
		/// <param name="g">Object Graphics</param>
		/// <param name="points">Body pro vykreslenÌ Ë·ry</param>
		/// <param name="pen">Pero</param>
		/// <param name="lineStyle">Styl Ë·ry</param>
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
		/// NakreslÌ body (bez v˝znaËn˝ch bod˘)
		/// </summary>
		/// <param name="g">Objekt Graphics</param>
		/// <param name="points">Body</param>
		/// <param name="pen">Pero</param>
		/// <param name="pointStyle">Styl bod˘</param>
		/// <param name="pointSize">Velikost bod˘</param>
		private void DrawPoints(Graphics g, Point [] points, Pen pen, Graph.PointStyles pointStyle, int pointSize) {
			this.DrawPoints(g, points, pen, pointStyle, pointSize, 1, 0, pointStyle, pointSize);
		}

		/// <summary>
		/// NakreslÌ body (s v˝znaËn˝mi body)
		/// </summary>
		/// <param name="g">Objekt Graphics</param>
		/// <param name="points">Body</param>
		/// <param name="pen">Pero</param>
		/// <param name="pointStyle">Styl bod˘</param>
		/// <param name="pointSize">Velikost bod˘</param>
		/// <param name="hlStep">Krok pro v˝znaËnÈ body</param>
		/// <param name="hlOffset">Kolik·t˝ krok zaËÌn· v˝znaËn˝ bod</param>
		/// <param name="hlPointStyle">Styl v˝znaËn˝ch bod˘</param>
		/// <param name="hlPointSize">Velikost v˝znaËn˝ch bod˘</param>
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
		/// NakreslÌ bod
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
        /// NastavÌ ToolTip
        /// </summary>
        /// <param name="x">X - ov· sou¯adnice myöi</param>
        /// <param name="y">Y - ov· sou¯adnice myöi</param>
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
		/// P¯i zmÏnÏ velikosti
		/// </summary>
        protected override void OnSizeChanged(EventArgs e) {
            base.OnSizeChanged(e);
            this.Invalidate();
        }

        /// <summary>
        /// ZmÏna skupiny
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);

            // S tlaËÌtkem vyvÌjÌme k¯ivku
            if(this.leftMouseButton) {
                int scrollStep = (int)this.graph.GetGeneralParameter(paramScrollStep, defaultScrollStep);
                int newTime = this.time + scrollStep * System.Math.Sign(e.Delta);
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

                if(this.bitmap[this.group] != null)
                    this.Image = this.bitmap[this.group];
                else
                    this.Image = new Bitmap(1, 1);

                if(this.evalCurve)
                    this.time = 1;
                else
                    this.time = this.minMax[this.group].MaxLength;
            }

            this.Invalidate();
        }

        /// <summary>
        /// Stisk tlaËÌtka myöi
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            if(e.Button == MouseButtons.Left)
                this.leftMouseButton = true;
        }

        /// <summary>
        /// UvolnÏnÌ tlaËÌtka myöi
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);

            if(e.Button == MouseButtons.Left)
                this.leftMouseButton = false;
        }

        /// <summary>
        /// MusÌme nastavit focus, jinak nefunguje OnMouseWheel
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if(!this.Focused && this.Parent.Focused)
                this.Focus();
        }

		private string baseFontFamilyName = "Arial";
		private const double baseAmplifyY = 8;
        // Okraj v promilÌch
		private const int defaultMargin = 20;
		private const int defaultMarginWithAxeL = 120;
		private const int defaultMarginWithAxeB = 100;
		// Interval mezi dvÏma body s ËÌsly na ose (v obrazov˝ch bodech)
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
        private const string paramLabelFontSize = "labelfsize";
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
        private const string paramScrollStep = "scrollstep";

        // Osy
        private const string paramTitleX = "titlex";
        private const string paramLineColorX = "lcolorx";
        private const string paramLineWidthX = "lwidthx";
        private const string paramPointColorX = "pcolorx";
        private const string paramPointStyleX = "pstylex";
        private const string paramPointSizeX = "psizex";
        private const string paramShowLabelX = "showlabelx";
        private const string paramLabelColorX = "labelcolorx";
        private const string paramFontSizeX = "fsizex";
        private const string paramShowAxeX = "showaxex";

        private const string paramTitleY = "titley";
        private const string paramLineColorY = "lcolory";
        private const string paramLineWidthY = "lwidthy";
        private const string paramPointColorY = "pcolory";
        private const string paramPointStyleY = "pstyley";
        private const string paramPointSizeY = "psizey";
        private const string paramShowLabelY = "showlabely";
        private const string paramLabelColorY = "labelcolory";
        private const string paramFontSizeY = "fsizey";
        private const string paramShowAxeY = "showaxey";

        private const string paramColorZero = "colorzero";
        private const string paramColorPlus = "colorplus";
        private const string paramColorMinus = "colorminus";
        private const string paramLegend = "legend";
        private const string paramLegendWidth = "legendwidth";
        private const string paramLegendFontSize = "legendfsize";

        private const string paramSubTitle = "subtitle";
        private const string paramSubTitleColor = "stcolor";
        private const string paramSubTitleFontSize = "stfsize";

        // PozadÌ
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
        private const int defaultLabelFontSize = 8;
        private const string defaultLineName = "";
        private static Color defaultBackgroundColor = Color.FromName("white");

        private static Color defaultFirstPointColor = Color.FromName("darkred");
        private static Graph.PointStyles defaultFirstPointStyle = Graph.PointStyles.None;
        private const int defaultFirstPointSize = 5;
        private static Color defaultLastPointColor = Color.FromName("darkgreen");
        private static Graph.PointStyles defaultLastPointStyle = Graph.PointStyles.None;
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
        private const int defaultPointSizeX = 3;
        private const bool defaultShowLabelX = false;
        private static Color defaultLabelColorX = defaultLineColorX;
        private const int defaultFontSizeX = 8;
        private const bool defaultShowAxeX = true;

        private const string defaultTitleY = "Y";
        private static Color defaultLineColorY = Color.FromName("red");
        private const float defaultLineWidthY = 1.0F;
        private static Color defaultPointColorY = Color.FromName("red");
        private const Graph.PointStyles defaultPointStyleY = Graph.PointStyles.HLines;
        private const int defaultPointSizeY = 3;
        private const bool defaultShowLabelY = false;
        private static Color defaultLabelColorY = defaultLineColorY;
        private const int defaultFontSizeY = defaultFontSizeX;
        private const bool defaultShowAxeY = true;

        private static Color defaultColorZero = Color.FromName("white");
        private static Color defaultColorPlus = Color.FromName("blue");
        private static Color defaultColorMinus = Color.FromName("red");
        private const bool defaultLegend = false;
        private const int defaultLegendWidth = 50;
        private const int defaultLegendFontSize = 8;
        private const string defaultTitle = "";

        private const bool defaultEvaluateGroup = false;
        private const bool defaultEvaluateCurve = false;
        private const int defaultInterval = 1000;
        private const int defaultScrollStep = 5;

        private const string defaultSubTitle = "";
        private static Color defaultSubTitleColor = Color.FromName("black");
        private const int defaultSubTitleFontSize = 10;

        private const double defaultMinXBackground = double.NaN;
        private const double defaultMaxXBackground = defaultMinX;
        private const double defaultMinYBackground = defaultMinX;
        private const double defaultMaxYBackground = defaultMinX;

        private const string errorMessageBadFileName = "Chybn˝ n·zev souboru pro uloûenÌ obr·zku: {0}";
    }
}