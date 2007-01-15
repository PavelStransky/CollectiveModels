using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Timers;
using System.Drawing;
using System.Drawing.Imaging;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Forms {
	/// <summary>
	/// Summary description for DensityPanel.
	/// </summary>
	public class DensityBox: System.Windows.Forms.PictureBox, IGraphControl {
		// Daty k vykreslení
		private Graph graph;

        // Èasovaè pro animace
        private System.Timers.Timer timer = new System.Timers.Timer();

        // Èasovaè pro ukládání jako (animovaný) GIF
        private BackgroundWorker backgroundWorkerSaveGif = new BackgroundWorker();
        // Èasovaè pro ukládání jako (sekvence) obrázkù
        private BackgroundWorker backgroundWorkerSavePicture = new BackgroundWorker();
        // Èasovaè pro vytvoøení bitmap
        private BackgroundWorker backgroundWorkerCreate = new BackgroundWorker();

        // Index vykreslovaných dat
        private int index;

		// Bitmapa, do které se obrázek vykreslí (kvùli rychlosti)
        private Bitmap[] bitmap;

		/// <summary>
		/// Základní konstruktor
		/// </summary>
		public DensityBox() : base() {
			this.SizeMode = PictureBoxSizeMode.StretchImage;

            this.backgroundWorkerSaveGif.WorkerReportsProgress = true;
            this.backgroundWorkerSaveGif.WorkerSupportsCancellation = true;
            this.backgroundWorkerSaveGif.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);

            this.backgroundWorkerCreate.WorkerReportsProgress = true;
            this.backgroundWorkerCreate.WorkerSupportsCancellation = true;
            this.backgroundWorkerCreate.DoWork += new DoWorkEventHandler(backgroundWorkerCreate_DoWork);
            this.backgroundWorkerCreate.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorkerCreate_RunWorkerCompleted);

            this.backgroundWorkerSavePicture.WorkerReportsProgress = true;
            this.backgroundWorkerSavePicture.WorkerSupportsCancellation = true;
            this.backgroundWorkerSavePicture.DoWork += new DoWorkEventHandler(backgroundWorkerSavePicture_DoWork);
        }

        #region Vytvoøení bitmap
        /// <summary>
        /// Základní pracovní metoda, která vytváøí bitmapu
        /// </summary>
        void backgroundWorkerCreate_DoWork(object sender, DoWorkEventArgs e) {
            int count = this.graph.Count;

            for(int i = 0; i < count; i++) {
                this.backgroundWorkerCreate.ReportProgress(0, string.Format("Tvorba bitmapy {0} z {1}", i + 1, count));

                // Normování matice
                Matrix matrix = this.GetMatrix(i);
                this.bitmap[i] = this.CreateBitmap(i, matrix);

                // Žádost o pøerušení procesu
                if(this.backgroundWorkerCreate.CancellationPending) 
                    break;

                this.Image = this.bitmap[i];
                this.Invalidate();
            }
        }

        /// <summary>
        /// Na základì matice a velikosti okna vytvoøí bitmapu;
        /// </summary>
        /// <param name="index">Index matice</param>
        /// <param name="matrix">Matice k vykreslení</param>
        private Bitmap CreateBitmap(int index, Matrix matrix) {
            int pointSizeX = (int)this.graph.GetGeneralParameter(paramPointSizeX, defaultPointSizeX);
            int pointSizeY = (int)this.graph.GetGeneralParameter(paramPointSizeY, defaultPointSizeY);
            bool legend = (bool)this.graph.GetGeneralParameter(paramLegend, defaultLegend);
            int legendWidth = (int)this.graph.GetGeneralParameter(paramLegendWidth, defaultLegendWidth);
            string title = (string)this.graph.GetCurveParameter(index, paramTitle, defaultTitle);

            int sizeX = pointSizeX * matrix.LengthX;
            int sizeY = pointSizeY * matrix.LengthY;
            int lengthX = matrix.LengthX;
            int lengthY = matrix.LengthY;

            Bitmap result = new Bitmap(sizeX + (legend ? legendWidth : 0), sizeY);

            Color colorPlus = (Color)this.graph.GetCurveParameter(index, paramColorPlus, defaultColorPlus);
            Color colorZero = (Color)this.graph.GetCurveParameter(index, paramColorZero, defaultColorZero);
            Color colorMinus = (Color)this.graph.GetCurveParameter(index, paramColorMinus, defaultColorMinus);

            double maxAbs = System.Math.Abs(matrix.MaxAbs());

            for(int i = 0; i < lengthX; i++) {
                for(int j = 0; j < lengthY; j++) {
                    double m = matrix[i, j] / maxAbs;

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

                    int i1 = i * pointSizeX;
                    int j1 = j * pointSizeY;

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

            if(title != string.Empty) {
                Graphics g = Graphics.FromImage(result);
                int fontWidth = (int)g.MeasureString(title, baseFont).Width;
                g.DrawString(title, baseFont, Brushes.Black, (sizeX - fontWidth) / 2, 5);
            }

            return result;
        }

        void backgroundWorkerCreate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            // Žádost o pøerušení procesu
            if(this.backgroundWorkerCreate.CancellationPending)
                return;

            this.index = 0;

            if(this.graph.Count > 1) {
                this.timer.Interval = (double)this.graph.GetGeneralParameter(paramInterval, defaultInterval);
                this.timer.AutoReset = true;
                this.timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                this.timer.Start();
            }
        }
        #endregion

        #region Uložení obrázku
        /// <summary>
        /// Uložení jako obrázku
        /// </summary>
        /// <param name="fName">Jméno souboru</param>
        public void SavePicture(string fName) {
            (this.Parent.Parent as GraphForm).NewProcess("Ukládání obrázku :", this.backgroundWorkerSavePicture, fName);
        }

        /// <summary>
        /// Základní pracovní metoda, která ukládá obrázek
        /// </summary>
        void backgroundWorkerSavePicture_DoWork(object sender, DoWorkEventArgs e) {
            string fName = e.Argument as string;
            int count = this.graph.Count;

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

            // Více obrázkù
            if(count > 1) {
                for(int i = 0; i < count; i++) {
                    System.Drawing.Image image = this.bitmap[i];
                    this.PaintGraph(Graphics.FromImage(image), i);
                    image.Save(string.Format("{0}{1}.{2}", name, i, extension), format);

                    this.backgroundWorkerSavePicture.ReportProgress(i * 100 / count);

                    // Požadavek ukonèení procesu
                    if(this.backgroundWorkerSavePicture.CancellationPending)
                        break;
                }
            }
            // Jeden obrázek
            else {
                System.Drawing.Image image = this.bitmap[0];
                this.PaintGraph(Graphics.FromImage(image), 0);
                image.Save(fName, format);

                this.backgroundWorkerSaveGif.ReportProgress(100);
            }
        }
        
        /// <summary>
        /// Uloží jako GIF
        /// </summary>
        /// <param name="fname">Jméno souboru</param>
        public void SaveGIF(string fName) {
            (this.Parent.Parent as GraphForm).NewProcess("Ukládání GIF :", this.backgroundWorkerSaveGif, fName);
        }

        /// <summary>
        /// Základní pracovní metoda, která ukládá obrázek
        /// </summary>
        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
            string fName = e.Argument as string;
            int count = this.graph.Count;

            if(count > 1) {
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

                for(int i = 0; i < count; i++) {
                    System.Drawing.Image image = this.bitmap[i];
                    this.PaintGraph(Graphics.FromImage(image), i);
                    image.Save(m, ImageFormat.Gif);

                    buf1 = m.ToArray();

                    if(i == 0) {
                        //only write these the first time....
                        b.Write(buf1, 0, 781);  //Header & global color table
                        b.Write(buf2, 0, 19);   //Application extension
                    }

                    b.Write(buf3, 0, 8);        //Graphic extension
                    b.Write(buf1, 789, buf1.Length - 790); //Image data

                    m.SetLength(0);

                    if(this.backgroundWorkerSaveGif.CancellationPending)
                        break;

                    this.backgroundWorkerSaveGif.ReportProgress(i * 100 / count);
                }

                b.Write((byte)0x3B); //Image terminator
                b.Close();
                f.Close();
                m.Close();
            }

            else {
                System.Drawing.Image image = this.bitmap[0];
                this.PaintGraph(Graphics.FromImage(image), 0);
                image.Save(fName, ImageFormat.Gif);

                this.backgroundWorkerSaveGif.ReportProgress(100);
            }
        }
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="graph">Objekt grafu</param>
        public DensityBox(Graph graph)
            : this() {
            this.SetGraph(graph);
        }

        /// <summary>
        /// Matice
        /// </summary>
        /// <param name="i">Index dat</param>
        private Matrix GetMatrix(int i) {
            return this.graph.Item[i] as Matrix;
        }

        /// <summary>
        /// Vrátí objekt s daty
        /// </summary>
        public Graph GetGraph() {
            return this.graph;
        }

		/// <summary>
		/// Nastaví matici k zobrazení
		/// </summary>
		/// <param name="graph">Objekt grafu</param>
		public void SetGraph(Graph graph) {
            this.graph = graph;
            
            int count = this.graph.Count;
            this.bitmap = new Bitmap[count];

            (this.Parent.Parent as GraphForm).NewProcess(string.Empty, this.backgroundWorkerCreate);
        }

        /// <summary>
        /// Event èasovaèe - postupné vykreslování køivky
        /// </summary>
        void timer_Elapsed(object sender, ElapsedEventArgs e) {
            this.index++;

            if(this.index >= this.graph.Count)
                this.index = 0;

            // Ještì nebylo spoèítáno
            if(this.bitmap[this.index] == null)
                this.index--;
            else {
                this.Image = this.bitmap[this.index];
                this.Invalidate();
            }
        }

		/// <summary>
		/// Vykreslení
		/// </summary>
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint (e);
            this.PaintGraph(e.Graphics, this.index);
        }

        /// <summary>
        /// Provede vykreslení grafu
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="index">Index k vykreslení</param>
        private void PaintGraph(Graphics g, int index) {
            TArray labelsX = (TArray)this.graph.GetCurveParameter(index, paramLabelsX, defaultLabelsX);
            TArray labelsY = (TArray)this.graph.GetCurveParameter(index, paramLabelsY, defaultLabelsY);

            float fontHeight = 1.2F * g.MeasureString(defaultMeasuredString, baseFont).Height;
            Matrix matrix = this.GetMatrix(index);

            if(labelsX.Count > 0) {
                float koefX = (float)this.Width / matrix.LengthX;

                int lx = System.Math.Min(matrix.LengthX, labelsX.Count);
                for(int i = 0; i < lx; i++)
                    g.DrawString(labelsX[i].ToString(), baseFont, baseBrush, i * koefX, 0);
            }

            if(labelsX.Count > 0) {
                float koefY = (float)this.Height / matrix.LengthY;

                int ly = System.Math.Min(matrix.LengthY, labelsY.Count);
                for(int i = 0; i < ly; i++)
                    g.DrawString(labelsY[i].ToString(), baseFont, baseBrush, 0, (i + 1) * koefY - fontHeight);
            }
        }

        /// <summary>
        /// Nastaví ToolTip
        /// </summary>
        /// <param name="x">X - ová souøadnice myši</param>
        /// <param name="y">Y - ová souøadnice myši</param>
        public string ToolTip(int x, int y) {
            Matrix m = this.GetMatrix(this.index);

            TArray labelsX = (TArray)this.graph.GetCurveParameter(this.index, paramLabelsX, defaultLabelsX);
            TArray labelsY = (TArray)this.graph.GetCurveParameter(this.index, paramLabelsY, defaultLabelsY);

            bool legend = (bool)this.graph.GetGeneralParameter(paramLegend, defaultLegend);
            int legendWidth = (int)this.graph.GetGeneralParameter(paramLegendWidth, defaultLegendWidth);

            int i = m.LengthX * x / (this.Width - (legend ? legendWidth : 0));
            int j = m.LengthY * y / this.Height;

            string tip = i < m.LengthX ?
                    string.Format("({0}, {1}) = {2,4:F}",
                    labelsX.Count > i ? labelsX[i] as string : i.ToString(),
                    labelsY.Count > j ? labelsY[j] as string : j.ToString(),
                    m[i, j]) : string.Empty;

            return tip;
        }

		private static Font baseFont = new Font("Arial", 8);
        private static Brush baseBrush = Brushes.White;
		private const string defaultMeasuredString = "A";

        // Parametry
        private const string paramLabelsX = "labelsx";
        private const string paramLabelsY = "labelsy";
        private const string paramInterval = "interval";
        private const string paramPointSizeX = "psizex";
        private const string paramPointSizeY = "psizey";
        private const string paramColorZero = "colorzero";
        private const string paramColorPlus = "colorplus";
        private const string paramColorMinus = "colorminus";
        private const string paramLegend = "legend";
        private const string paramLegendWidth = "legendwidth";
        private const string paramTitle = "title";

        private static TArray defaultLabelsX = new TArray();
        private static TArray defaultLabelsY = new TArray();
        private const double defaultInterval = 1000.0;
        private const int defaultPointSizeX = 3;
        private const int defaultPointSizeY = 3;
        private static Color defaultColorZero = Color.FromName("black");
        private static Color defaultColorPlus = Color.FromName("blue");
        private static Color defaultColorMinus = Color.FromName("red");
        private const bool defaultLegend = true;
        private const int defaultLegendWidth = 50;
        private const string defaultTitle = "";

        private const string errorMessageBadFileName = "Chybný název souboru pro uložení obrázku: {0}";
    }
}