using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Threading;

using PavelStransky.Core;
using PavelStransky.Expression;

using PavelStransky.Math;

namespace PavelStransky.Forms {
    /// <summary>
    /// Pr�zdn� formul��. Grafy do n�j p�id�v�me my
    /// </summary>
    public partial class SingleGraphForm {
        // �asova� pro ukl�d�n� jako GIF
        private BackgroundWorker bwSaveAnim = new BackgroundWorker();
        // �asova� pro ukl�d�n� jako sekvence obr�zk�
        private BackgroundWorker bwSaveSeq = new BackgroundWorker();        
        
        // Graf pod stisknut�m prav�m tla��tkem my�i
        private int activeGraph = -1;

        // True, pokud jsme zvolili, �e se bude ukl�dat v�e
        private bool saveAll;

        // True, pokud ukl�d�me text pro web
        private bool saveWeb;

        // K�ivky k exportu
        private CurveToExport curveToExport = new CurveToExport();

        /// <summary>
        /// Vytvo�� pozad� pracant�m
        /// </summary>
        private void CreateWorkers() {
            this.bwSaveAnim.WorkerReportsProgress = true;
            this.bwSaveAnim.WorkerSupportsCancellation = true;
            this.bwSaveAnim.DoWork += new DoWorkEventHandler(bwSaveAnim_DoWork);

            this.bwSaveSeq.WorkerReportsProgress = true;
            this.bwSaveSeq.WorkerSupportsCancellation = true;
            this.bwSaveSeq.DoWork += new DoWorkEventHandler(bwSaveSeq_DoWork);
        }

        /// <summary>
        /// P�i otev�r�n� kontextov�ho menu skryjeme polo�ky ulo�en� jedn� ��sti grafu,
        /// pokud neum�me ur�it, o kterou ��st se jedn�
        /// </summary>
        private void contextMenu_Opening(object sender, CancelEventArgs e) {
            if(this.activeGraph < 0) {
                this.cmnSaveOneAnim.Enabled = false;
                this.cmnSaveOneSeq.Enabled = false;
                this.cmnSaveOneText.Enabled = false;
            }
            else {
                this.cmnSaveOneAnim.Enabled = true;
                this.cmnSaveOneSeq.Enabled = true;
                this.cmnSaveOneText.Enabled = true;
            }
        }

        private void cmnSaveOneText_Click(object sender, EventArgs e) {
            this.sfdText.InitialDirectory = WinMain.ExportDirectory;

            if(this.sfdText.FileName == string.Empty)
                this.sfdText.FileName = this.Text;

            this.sfdText.Title = Messages.TSaveOneText;
            this.saveAll = false;
            this.saveWeb = false;

            this.sfdText.ShowDialog();
        }

        private void cmnSaveForWeb_Click(object sender, EventArgs e) {
            this.sfdText.InitialDirectory = WinMain.ExportDirectory;

            if(this.sfdText.FileName == string.Empty)
                this.sfdText.FileName = this.Text;

            this.sfdText.Title = Messages.TSaveForWeb;
            this.saveWeb = true;

            this.sfdText.ShowDialog();
        }

        private void cmnSaveAllText_Click(object sender, EventArgs e) {
            this.sfdText.InitialDirectory = WinMain.ExportDirectory;

            if(this.sfdText.FileName == string.Empty)
                this.sfdText.FileName = this.Text;

            this.sfdText.Title = Messages.TSaveAllText;
            this.saveAll = true;
            this.saveWeb = false;

            this.sfdText.ShowDialog();
        }

        /// <summary>
        /// Soukrom� t��da, kter� shroma��uje informace skupin�ch a k�ivk�ch
        /// </summary>
        private class CurvesInfo {
            private int group;
            private int curve;
            private int points;

            /// <summary>
            /// Skupina
            /// </summary>
            public int Group { get { return this.group; } }

            /// <summary>
            /// K�ivka ve skupin�
            /// </summary>
            public int Curve { get { return this.curve; } }

            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="group">Skupina</param>
            /// <param name="curve">K�ivka</param>
            /// <param name="points">Po�et bod�</param>
            public CurvesInfo(int group, int curve, int points) {
                this.group = group;
                this.curve = curve;
                this.points = points;
            }

            public override string ToString() {
                return string.Format("{0} - {1} ({2})", this.group, this.curve, this.points);
            }
        }

        private void sfdText_FileOk(object sender, CancelEventArgs e) {
            WinMain.SetExportDirectoryFromFile(this.sfdText.FileName);

            if(this.saveWeb) {
                Graph graph = this.graphs[this.activeGraph] as Graph;
                int groups = graph.NumGroups();

                this.curveToExport.CBCurves.Items.Clear();
                for(int g = 0; g < groups; g++) {
                    int curves = graph.NumCurves(g);
                    for(int c = 0; c < curves; c++)
                        this.curveToExport.CBCurves.Items.Add(
                            new CurvesInfo(g, c, graph.NumPoints(g, c)));
                }

                this.curveToExport.CBCurves.SelectedIndex = 0;
                this.curveToExport.OnlyY = false;
                if(this.curveToExport.ShowDialog() == DialogResult.Cancel) 
                    return;
            }

            Export export = new Export(this.sfdText.FileName, IETypes.Text);

            // Ukl�d� jen jednu prvn� k�ivku (m��e b�t dost nekonzistentn� se v��m!)
            if(this.saveWeb) {
                Graph g = this.graphs[this.activeGraph] as Graph;
                CurvesInfo c = this.curveToExport.CBCurves.SelectedItem as CurvesInfo;
                g.ExportWWW(export, c.Group, c.Curve, this.realWidth, this.realHeight, this.curveToExport.OnlyY);
            }
            else if(this.saveAll)
                this.graphs.Export(export);
            else
                (this.graphs[this.activeGraph] as Graph).Export(export);

            export.Close();
        }

        private void cmnSaveOneAnim_Click(object sender, EventArgs e) {
            this.sfdAnim.InitialDirectory = WinMain.ExportDirectory;

            if(this.sfdAnim.FileName == string.Empty)
                this.sfdAnim.FileName = this.Text;

            this.sfdAnim.Title = Messages.TSaveOneAnim;
            this.saveAll = false;

            this.sfdAnim.ShowDialog();
        }

        private void cmnSaveAllAnim_Click(object sender, EventArgs e) {
            this.sfdAnim.InitialDirectory = WinMain.ExportDirectory;

            if(this.sfdAnim.FileName == string.Empty)
                this.sfdAnim.FileName = this.Text;

            this.sfdAnim.Title = Messages.TSaveAllAnim;
            this.saveAll = true;

            this.sfdAnim.ShowDialog();
        }

        private void cmnSaveOneSeq_Click(object sender, EventArgs e) {
            this.sfdSeq.InitialDirectory = WinMain.ExportDirectory;
            this.sfdSeq.DefaultExt = WinMain.SeqExtension;

            if(this.sfdSeq.FileName == string.Empty)
                this.sfdSeq.FileName = this.Text;

            this.sfdSeq.Title = Messages.TSaveOneSeq;
            this.saveAll = false;

            this.sfdSeq.ShowDialog();
        }

        private void cmnSaveAllSeq_Click(object sender, EventArgs e) {
            this.sfdSeq.InitialDirectory = WinMain.ExportDirectory;
            this.sfdSeq.DefaultExt = WinMain.SeqExtension;

            if(this.sfdSeq.FileName == string.Empty)
                this.sfdSeq.FileName = this.Text;

            this.sfdSeq.Title = Messages.TSaveAllSeq;
            this.saveAll = true;

            this.sfdSeq.ShowDialog();
        }

        private void sfdAnim_FileOk(object sender, CancelEventArgs e) {
            WinMain.SetExportDirectoryFromFile(this.sfdAnim.FileName);
            this.SaveAnim(this.sfdAnim.FileName);
        }

        private void sfdSeq_FileOk(object sender, CancelEventArgs e) {
            WinMain.SetExportDirectoryFromFile(this.sfdSeq.FileName);
            WinMain.SetSeqExtensionFromFile(this.sfdSeq.FileName);
            this.SaveSeq(this.sfdSeq.FileName);
        }

        /// <summary>
        /// Ulo�� jako sekvenci obr�zk�
        /// </summary>
        /// <param name="fName">Jm�no souboru</param>
        public void SaveSeq(string fName) {
            this.NewProcess("Ukl�d�n� sekvence :", this.bwSaveSeq, fName);
        }

        /// <summary>
        /// Zji��uje, jak� je maxim�ln� po�et animovan�ch skupin;
        /// pokud skupiny neanimujeme, vrac� -1
        /// </summary>
        private int MaxNumGroups(GraphItem[] graphItems) {
            int maxNumGroups = -1;
            int nGraphs = graphItems.Length;

            for(int i = 0; i < nGraphs; i++) {
                if(graphItems[i].Graph.AnimGroup)
                    maxNumGroups = System.Math.Max(maxNumGroups, graphItems[i].Graph.NumGroups());
            }

            return maxNumGroups;
        }

        /// <summary>
        /// Zjist� pr�m�rn� interval pro animaci cel�ho grafu;
        /// pokud zjist�, �e se graf neanimuje, vrac� 0
        /// </summary>
        /// <param name="graphItems">Jednotliv� grafy</param>
        private int AverageInterval(GraphItem[] graphItems) {
            int sumIntervals = 0;
            int numIntervals = 0;
            int nGraphs = graphItems.Length;

            for(int i = 0; i < nGraphs; i++) {
                GraphItem gi = graphItems[i];
                if(gi.Graph.AnimGroup || gi.Graph.AnimCurves(gi.ActualGroup)) {
                    sumIntervals += gi.Graph.AnimInterval;
                    numIntervals++;
                }
            }

            if(numIntervals > 0)
                return sumIntervals / numIntervals;
            else
                return 0;
        }

        /// <summary>
        /// Zji��uje, jak� je maxim�ln� d�lka animovan�ch k�ivek
        /// </summary>
        private int MaxTime(GraphItem[] graphItems, int group) {
            int maxTime = -1;
            int nGraphs = graphItems.Length;

            for(int i = 0; i < nGraphs; i++) {
                Graph graph = graphItems[i].Graph;

                int g = group;
                if(graph.AnimGroup) {
                    if(group >= graph.NumGroups())
                        g = graph.NumGroups() - 1;
                }
                else
                    g = graphItems[i].ActualGroup;

                if(graph.AnimCurves(g))
                    maxTime = System.Math.Max(maxTime, graph.MaxTime(g));
            }

            return maxTime;
        }

        /// <summary>
        /// Ulo�� jeden jednotliv� obr�zek do souboru
        /// </summary>
        /// <param name="group">Skupina</param>
        /// <param name="time">�as</param>
        /// <param name="graphItems">Jednotliv� grafy</param>
        private Bitmap PaintOne(Image baseImage, GraphItem[] graphItems, int group, int time) {
            Bitmap image = new Bitmap(baseImage);
            Graphics graphics = Graphics.FromImage(image);
            Rectangle rectangle = new Rectangle(0, 0, image.Width, image.Height);

            int nGraphs = graphItems.Length;

            for(int i = 0; i < nGraphs; i++)
                graphItems[i].PaintActual(graphics, rectangle, group, time);

            return image;
        }

        /// <summary>
        /// Z�kladn� pracovn� metoda, kter� ukl�d� obr�zek
        /// </summary>        
        void bwSaveSeq_DoWork(object sender, DoWorkEventArgs e) {
            string fName = e.Argument as string;

            if(fName.Length < 3 || fName.IndexOf('.') < 0)
                throw new FormsException(string.Format(Messages.EMBadFileName, fName));

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

//          Rectangle rectangle = this.graphicsBox.ClientRectangle;
            int width = this.RealWidth;
            if(width <= 0)
                width = this.graphicsBox.ClientRectangle.Width;

            int height = this.RealHeight;
            if(height <= 0)
                height = this.graphicsBox.ClientRectangle.Height;

            Rectangle rectangle = new Rectangle(0, 0, width, height);
            Bitmap baseImage = new Bitmap(rectangle.Width, rectangle.Height);
            Graphics.FromImage(baseImage).FillRectangle(Brushes.White, rectangle);

            GraphItem[] graphItems = this.graphicsBox.GraphItems;
            int nGraphs = graphItems.Length;

            int maxNumGroups = this.MaxNumGroups(graphItems);
            int scrollStep = graphItems[0].Graph.ScrollStep;
            
            // Animujeme skupiny
            if(maxNumGroups > 0) {
                for(int g = 0; g < maxNumGroups; g++) {
                    int maxTime = this.MaxTime(graphItems, g);

                    // Animujeme i k�ivky
                    if(maxTime > 0)
                        for(int t = 1; t <= maxTime; t += scrollStep)
                            this.PaintOne(baseImage, graphItems, g, t).Save(string.Format("{0}_{1}-{2}.{3}", name, g, t, extension), format);

                    // K�ivky neanimujeme
                    else
                        this.PaintOne(baseImage, graphItems, g, -1).Save(string.Format("{0}_{1}.{2}", name, g, extension), format);

                    this.bwSaveSeq.ReportProgress(g * 100 / maxNumGroups);

                    // Po�adavek ukon�en� procesu
                    if(this.bwSaveSeq.CancellationPending)
                        break;
                }
            }

            // Neanimujeme skupiny
            else {
                int maxTime = this.MaxTime(graphItems, 1);

                // Animujeme k�ivky
                if(maxTime > 0)
                    for(int t = 1; t <= maxTime; t += scrollStep) {
                        this.PaintOne(baseImage, graphItems, -1, t).Save(string.Format("{0}-{1}.{2}", name, t, extension), format);

                        this.bwSaveSeq.ReportProgress(t * 100 / maxTime);

                        // Po�adavek ukon�en� procesu
                        if(this.bwSaveSeq.CancellationPending)
                            break;
                    }

                // Neanimujeme nic
                else {
                    this.PaintOne(baseImage, graphItems, -1, -1).Save(string.Format("{0}.{1}", name, extension), format);
                    this.bwSaveSeq.ReportProgress(100);
                }
            }
        }

        /// <summary>
        /// Ulo�� jako GIF
        /// </summary>
        /// <param name="fName">Jm�no souboru</param>
        public void SaveAnim(string fName) {
            this.NewProcess("Ukl�d�n� animace :", this.bwSaveAnim, fName);
        }

        /// <summary>
        /// Z�kladn� pracovn� metoda, kter� ukl�d� obr�zek
        /// </summary>
        private void bwSaveAnim_DoWork(object sender, DoWorkEventArgs e) {
            string fName = e.Argument as string;

            if(fName.Length < 3 || fName.IndexOf('.') < 0)
                throw new FormsException(string.Format(Messages.EMBadFileName, fName));

//          Rectangle rectangle = this.graphicsBox.ClientRectangle;

            int width = this.RealWidth;
            if(width <= 0)
                width = this.graphicsBox.ClientRectangle.Width;

            int height = this.RealHeight;
            if(height <= 0)
                height = this.graphicsBox.ClientRectangle.Height;

            Rectangle rectangle = new Rectangle(0, 0, width, height);
            Bitmap baseImage = new Bitmap(rectangle.Width, rectangle.Height);
            Graphics.FromImage(baseImage).FillRectangle(Brushes.White, rectangle);

            GraphItem[] graphItems = this.graphicsBox.GraphItems;
            int nGraphs = graphItems.Length;

            int maxNumGroups = this.MaxNumGroups(graphItems);
            int interval = this.AverageInterval(graphItems) / 10;

            // Animujeme skupiny
            if(maxNumGroups > 0) {
                AnimGIFBuffer buffer = new AnimGIFBuffer(interval, fName);
                int scrollStep = graphItems[0].Graph.ScrollStep;

                for(int g = 0; g < maxNumGroups; g++) {
                    int maxTime = this.MaxTime(graphItems, g);

                    // Animujeme i k�ivky
                    if(maxTime > 0)
                        for(int t = 1; t <= maxTime; t += scrollStep)
                            buffer.Add(this.PaintOne(baseImage, graphItems, g, t));

                    // K�ivky neanimujeme
                    else
                        buffer.Add(this.PaintOne(baseImage, graphItems, g, -1));

                    this.bwSaveAnim.ReportProgress(g * 100 / maxNumGroups);

                    // Po�adavek ukon�en� procesu
                    if(this.bwSaveAnim.CancellationPending)
                        break;
                }

                buffer.Close();
            }

            // Neanimujeme skupiny
            else {
                int maxTime = this.MaxTime(graphItems, 1);

                // Animujeme k�ivky
                if(maxTime > 0) {
                    AnimGIFBuffer buffer = new AnimGIFBuffer(interval, fName);
                    int scrollStep = graphItems[0].Graph.ScrollStep;

                    for(int t = 1; t <= maxTime; t += scrollStep) {
                        buffer.Add(this.PaintOne(baseImage, graphItems, -1, t));

                        this.bwSaveAnim.ReportProgress(t * 100 / maxTime);

                        // Po�adavek ukon�en� procesu
                        if(this.bwSaveAnim.CancellationPending)
                            break;
                    }

                    buffer.Close();
                }

                // Neanimujeme nic
                else {
                    this.PaintOne(baseImage, graphItems, -1, -1).Save(fName, ImageFormat.Gif);
                    this.bwSaveAnim.ReportProgress(100);
                }
            }
        }
    }
}
