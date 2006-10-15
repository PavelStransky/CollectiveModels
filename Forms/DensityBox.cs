using System;
using System.Windows.Forms;
using System.Drawing;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Forms {
	/// <summary>
	/// Summary description for DensityPanel.
	/// </summary>
	public class DensityBox: System.Windows.Forms.PictureBox, IGraphControl {
		// Daty k vykreslení
		private Graph graph;

		// Bitmapa, do které se obrázek vykreslí (kvùli rychlosti)
		private Bitmap bitmap;

        /// <summary>
        /// Matice
        /// </summary>
        private Matrix Matrix { get { return this.graph.Item[0] as Matrix; } }

		/// <summary>
		/// Základní konstruktor
		/// </summary>
		public DensityBox() : base() {
			this.SizeMode = PictureBoxSizeMode.StretchImage;
		}

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="graph">Objekt grafu</param>
        public DensityBox(Graph graph)
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
		/// Nastaví matici k zobrazení
		/// </summary>
		/// <param name="graph">Objekt grafu</param>
		public void SetGraph(Graph graph) {
            this.graph = graph;

			// Normování matice
            Matrix matrix = this.Matrix;
			matrix = matrix * (255.0 / matrix.MaxAbs());

			this.CreateBitmap(matrix);
		}

		/// <summary>
		/// Na základì matice a velikosti okna vytvoøí bitmapu;
		/// </summary>
        /// <param name="matrix">Matice k vykreslení</param>
		private void CreateBitmap(Matrix matrix) {
            this.bitmap = new Bitmap(matrix.LengthX * pointSizeX, matrix.LengthY * pointSizeY);

            int sizeX = pointSizeX * matrix.LengthX;
            int sizeY = pointSizeY * matrix.LengthY;
            int lengthX = matrix.LengthX;
            int lengthY = matrix.LengthY;

			for(int i = 0; i < lengthX; i++)
				for(int j = 0; j < lengthY; j++) {
					int m = (int)matrix[i, j];

					int r = m > 0 ? m : 0;
					int g = 0;
					int b = m < 0 ? -m : 0;

                    Color color = Color.FromArgb(r, g, b);

                    int i1 = i * pointSizeX;
                    int j1 = j * pointSizeY;

                    for(int k = 0; k < pointSizeX; k++)
                        for(int l = 0; l < pointSizeY; l++)
        					this.bitmap.SetPixel(i1 + k, j1 + l, color);
				}

			this.Image = this.bitmap;
		}

		/// <summary>
		/// Vykreslení
		/// </summary>
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint (e);

            Graphics g = e.Graphics;

            Expression.Array labelsX = (Expression.Array)this.graph.GetGeneralParameter(paramLabelsX, defaultLabelsX);
            Expression.Array labelsY = (Expression.Array)this.graph.GetGeneralParameter(paramLabelsY, defaultLabelsY);

            float fontHeight = 1.2F * g.MeasureString(defaultMeasuredString, baseFont).Height;
            Matrix matrix = this.graph.Item[0] as Matrix;

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
					e.Graphics.DrawString(labelsY[i].ToString(), baseFont, baseBrush, 0, (i + 1) * koefY - fontHeight);
			}
		}

        /// <summary>
        /// Nastaví ToolTip
        /// </summary>
        /// <param name="x">X - ová souøadnice myši</param>
        /// <param name="y">Y - ová souøadnice myši</param>
        public string ToolTip(int x, int y) {
            Matrix m = this.Matrix;

            Expression.Array labelsX = (Expression.Array)this.graph.GetGeneralParameter(paramLabelsX, defaultLabelsX);
            Expression.Array labelsY = (Expression.Array)this.graph.GetGeneralParameter(paramLabelsY, defaultLabelsY);

            int i = m.LengthX * x / this.Width;
            int j = m.LengthY * y / this.Height;

            string tip = string.Format("({0}, {1}) = {2,4:F}",
                    labelsX.Count > i ? labelsX[i] as string : i.ToString(),
                    labelsY.Count > j ? labelsY[j] as string : j.ToString(),
                    m[i, j]);

            return tip;
        }

		private const int pointSizeX = 3;
		private const int pointSizeY = 3;

		private static Font baseFont = new Font("Arial", 8);
        private static Brush baseBrush = Brushes.White;
		private const string defaultMeasuredString = "A";

        // Parametry
        private const string paramLabelsX = "labelsx";
        private const string paramLabelsY = "labelsy";

        private static Expression.Array defaultLabelsX = new Expression.Array();
        private static Expression.Array defaultLabelsY = new Expression.Array();
	}
}