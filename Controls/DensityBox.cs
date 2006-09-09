using System;
using System.Windows.Forms;
using System.Drawing;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Controls {
	/// <summary>
	/// Summary description for DensityPanel.
	/// </summary>
	public class DensityBox: System.Windows.Forms.PictureBox {
		// Matice s daty k vykreslen�
		private Matrix matrix = null;
		// Bitmapa, do kter� se obr�zek vykresl� (kv�li rychlosti)
		private Bitmap bitmap;
		// Popisky
		private Expression.Array labelsX, labelsY;

		/// <summary>
		/// Z�kladn� konstruktor
		/// </summary>
		public DensityBox() : base() {
			this.SizeMode = PictureBoxSizeMode.StretchImage;
		}

		/// <summary>
		/// Nastav� matici k zobrazen�
		/// </summary>
		/// <param name="m">Matice</param>
		public void SetMatrix(Matrix m) {
			// Normov�n� matice
			this.matrix = m * 255.0 / m.MaxAbs();
			this.CreateBitmap();
		}

		/// <summary>
		/// Nastav� popisky
		/// </summary>
		/// <param name="labelsX">Popisky matice na ose X</param>
		/// <param name="labelsY">Popisky matice na ose Y</param>
		public void SetLabels(Expression.Array labelsX, Expression.Array labelsY) {
			this.labelsX = labelsX;
			this.labelsY = labelsY;
		}

		/// <summary>
		/// Na z�klad� matice a velikosti okna vytvo�� bitmapu;
		/// </summary>
		private void CreateBitmap() {
			this.bitmap = new Bitmap(this.matrix.LengthX * pointSizeX, this.matrix.LengthY * pointSizeY);

			int sizeX = pointSizeX * this.matrix.LengthX;
			int sizeY = pointSizeY * this.matrix.LengthY;

			for(int i = 0; i < sizeX; i++)
				for(int j = 0; j < sizeY; j++) {
					int m = (int)this.matrix[i / pointSizeX, j / pointSizeY];

					int r = m > 0 ? m : 0;
					int g = 0;
					int b = m < 0 ? -m : 0;

					this.bitmap.SetPixel(i, j, Color.FromArgb(r, g, b));
				}

			this.Image = this.bitmap;
		}

		/// <summary>
		/// Vykreslen�
		/// </summary>
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint (e);

			if(this.matrix != null && (this.labelsX != null || this.labelsY != null)) {
				Graphics g = e.Graphics;

				float koefX = (float)this.Width / this.matrix.LengthX;
				float koefY = (float)this.Height / this.matrix.LengthY;
				float fontHeight = 1.2F * g.MeasureString(defaultMeasuredString, baseFont).Height;

				if(this.labelsX != null) {
					int lx = System.Math.Min(this.matrix.LengthX, this.labelsX.Count);
					for(int i = 0; i < lx; i++)
						e.Graphics.DrawString(this.labelsX[i] as string, baseFont, Brushes.White, i * koefX, 0);
				}
				
				if(this.labelsY != null) {
					int ly = System.Math.Min(this.matrix.LengthY, this.labelsY.Count);
					for(int i = 0; i < ly; i++)
						e.Graphics.DrawString(this.labelsY[i] as string, baseFont, Brushes.White, 0, (i + 1) * koefY - fontHeight);
				}
			}
		}

		private const int pointSizeX = 5;
		private const int pointSizeY = 5;

		private Font baseFont = new Font("Arial", 8);
		private const string defaultMeasuredString = "A";
	}
}