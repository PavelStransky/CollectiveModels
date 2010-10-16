using System;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.IO;

using PavelStransky.Math;

namespace PavelStransky.Expression {
    public partial class Graph {
        // �asova� pro vytvo�en� bitmap
        private BackgroundWorker bwCreate = new BackgroundWorker();

        // Vytvo�en� pozad�
        public delegate void FinishedBackgroundEventHandler(object sender, FinishedBackgroundEventArgs e);
        public event FinishedBackgroundEventHandler FinishedBackground;

        /// <summary>
        /// Vol� se p�i jak�koliv ud�losti na kontextu
        /// </summary>
        protected void OnFinishedBackground(FinishedBackgroundEventArgs e) {
            if(this.FinishedBackground != null)
                this.FinishedBackground(this, e);
        }

        /// <summary>
        /// Proces na pozad�, kter� vytvo�� animace
        /// </summary>
        public BackgroundWorker BWCreate { get { return this.bwCreate; } }

        /// <summary>
        /// Vytvo�en� �asova��
        /// </summary>
        private void CreateWorker() {
            this.bwCreate.WorkerReportsProgress = true;
            this.bwCreate.WorkerSupportsCancellation = true;
            this.bwCreate.DoWork += new DoWorkEventHandler(backgroundWorkerCreate_DoWork);
        }

        #region Vytvo�en� bitmap
        // Bitmapa, do kter� se obr�zek pozad� vykresl� (kv�li rychlosti)
        private Bitmap[] bitmap;

        /// <summary>
        /// Z�kladn� pracovn� metoda, kter� vytv��� bitmapu
        /// </summary>
        void backgroundWorkerCreate_DoWork(object sender, DoWorkEventArgs e) {
            int nGroups = (int)this.graphParamValues[ParametersIndications.NumGroups];

            for(int g = 0; g < nGroups; g++) {
                this.bwCreate.ReportProgress(0, string.Format("Tvorba bitmapy {0} z {1}", g + 1, nGroups));

                GraphParameterValues gv = this.groupParamValues[g] as GraphParameterValues;
                Matrix matrix = (Matrix)gv[ParametersIndications.DataBackground];

                if(matrix.NumItems() > 0) {
                    this.bitmap[g] = this.CreateBitmap(gv, matrix);
                    this.OnFinishedBackground(new FinishedBackgroundEventArgs(g));
                }
                else
                    this.bitmap[g] = null;

                // ��dost o p�eru�en� procesu
                if(this.bwCreate.CancellationPending) 
                    break;
            }
        }

        /// <summary>
        /// Na z�klad� matice a velikosti okna vytvo�� bitmapu;
        /// </summary>
        /// <param name="gv">Parametry skupiny grafu</param>
        /// <param name="matrix">Matice k vykreslen�</param>
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
            BColor bcolor = new BColor(gv);

            for(int i = 0; i < lengthX; i++) {
                for(int j = 0; j < lengthY; j++) {
                    double m = matrix[i, lengthY - j - 1];

                    Color color = bcolor[i, lengthY - j - 1, m];

                    int i1 = i * pointSizeX;
                    int j1 = j * pointSizeY;

                    for(int k = 0; k < pointSizeX; k++)
                        for(int l = 0; l < pointSizeY; l++) {
                            int x = i1 + k - pointSizeX / 2;
                            int y = j1 + l - pointSizeY / 2;
                            if(x >= 0 && y >= 0)
                                result.SetPixel(x, y, color);
                        }
                }

                // ��dost o p�eru�en� procesu
                if(this.bwCreate.CancellationPending)
                    break;

                this.bwCreate.ReportProgress(i * 100 / lengthX);
            }

            return result;
        }
        #endregion
    }
}
