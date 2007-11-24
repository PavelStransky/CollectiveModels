using System;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.IO;

using PavelStransky.Math;

namespace PavelStransky.Expression {
    public partial class Graph {
        // »asovaË pro ukl·d·nÌ jako GIF
        private BackgroundWorker backgroundWorkerSaveGif = new BackgroundWorker();
        // »asovaË pro ukl·d·nÌ jako sekvence obr·zk˘
        private BackgroundWorker backgroundWorkerSavePicture = new BackgroundWorker();
        // »asovaË pro vytvo¯enÌ bitmap
        private BackgroundWorker backgroundWorkerCreate = new BackgroundWorker();

        // Vytvo¯enÈ pozadÌ
        public delegate void FinishedBackgroundEventHandler(object sender, FinishedBackgroundEventArgs e);
        public event FinishedBackgroundEventHandler FinishedBackground;

        /// <summary>
        /// Vol· se p¯i jakÈkoliv ud·losti na kontextu
        /// </summary>
        protected void OnFinishedBackground(FinishedBackgroundEventArgs e) {
            if(this.FinishedBackground != null)
                this.FinishedBackground(this, e);
        }

        /// <summary>
        /// Proces na pozadÌ, kter˝ vytvo¯Ì animace
        /// </summary>
        public BackgroundWorker BackgroundWorkerCreate { get { return this.backgroundWorkerCreate; } }

        /// <summary>
        /// Vytvo¯enÌ ËasovaË˘
        /// </summary>
        private void CreateWorkers() {
            //this.backgroundWorkerSaveGif.WorkerReportsProgress = true;
            //this.backgroundWorkerSaveGif.WorkerSupportsCancellation = true;
            //this.backgroundWorkerSaveGif.DoWork += new DoWorkEventHandler(backgroundWorkerSaveGif_DoWork);

            //this.backgroundWorkerSavePicture.WorkerReportsProgress = true;
            //this.backgroundWorkerSavePicture.WorkerSupportsCancellation = true;
            //this.backgroundWorkerSavePicture.DoWork += new DoWorkEventHandler(backgroundWorkerSavePicture_DoWork);

            this.backgroundWorkerCreate.WorkerReportsProgress = true;
            this.backgroundWorkerCreate.WorkerSupportsCancellation = true;
            this.backgroundWorkerCreate.DoWork += new DoWorkEventHandler(backgroundWorkerCreate_DoWork);
        }

        #region Vytvo¯enÌ bitmap
        // Bitmapa, do kterÈ se obr·zek pozadÌ vykreslÌ (kv˘li rychlosti)
        private Bitmap[] bitmap;

        /// <summary>
        /// Z·kladnÌ pracovnÌ metoda, kter· vytv·¯Ì bitmapu
        /// </summary>
        void backgroundWorkerCreate_DoWork(object sender, DoWorkEventArgs e) {
            int nGroups = (int)this.graphParamValues[ParametersIndications.NumGroups];

            for(int g = 0; g < nGroups; g++) {
                this.backgroundWorkerCreate.ReportProgress(0, string.Format("Tvorba bitmapy {0} z {1}", g + 1, nGroups));

                GraphParameterValues gv = this.groupParamValues[g] as GraphParameterValues;
                Matrix matrix = (Matrix)gv[ParametersIndications.DataBackground];

                if(matrix.NumItems() > 0) {
                    this.bitmap[g] = this.CreateBitmap(gv, matrix);
                    this.OnFinishedBackground(new FinishedBackgroundEventArgs(g));
                }
                else
                    this.bitmap[g] = null;

                // é·dost o p¯eruöenÌ procesu
                if(this.backgroundWorkerCreate.CancellationPending) 
                    break;
            }
        }

        /// <summary>
        /// Na z·kladÏ matice a velikosti okna vytvo¯Ì bitmapu;
        /// </summary>
        /// <param name="gv">Parametry skupiny grafu</param>
        /// <param name="matrix">Matice k vykreslenÌ</param>
        private Bitmap CreateBitmap(GraphParameterValues gv, Matrix matrix) {
            int pointSizeX = (int)gv[ParametersIndications.BPSizeX];
            int pointSizeY = (int)gv[ParametersIndications.BPSizeY];
            bool legend = (bool)gv[ParametersIndications.BLegend];
            int legendWidth = (int)gv[ParametersIndications.BLegendWidth];

            int lengthX = matrix.LengthX;
            int lengthY = matrix.LengthY;

            int sizeX = lengthX * pointSizeX;
            int sizeY = lengthY * pointSizeY;

            Bitmap result = new Bitmap(sizeX, sizeY);

            Color colorPlus = (Color)gv[ParametersIndications.BColorPlus];
            Color colorZero = (Color)gv[ParametersIndications.BColorZero];
            Color colorMinus = (Color)gv[ParametersIndications.BColorMinus];

            double maxAbs = System.Math.Abs((double)gv[ParametersIndications.MatrixAbs]);

            for(int i = 0; i < lengthX; i++) {
                for(int j = 0; j < lengthY; j++) {
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

                    int i1 = i * pointSizeX;
                    int j1 = j * pointSizeY;

                    for(int k = 0; k < pointSizeX; k++)
                        for(int l = 0; l < pointSizeY; l++)
                             result.SetPixel(i1 + k, j1 + l, color);
                }

                // é·dost o p¯eruöenÌ procesu
                if(this.backgroundWorkerCreate.CancellationPending)
                    break;

                this.backgroundWorkerCreate.ReportProgress(i * 100 / lengthX);
            }

            /*
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
            */

            return result;
        }
        #endregion

        /*
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
        private static void InitializeGIFBuffer() {
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
         */
    }
}
