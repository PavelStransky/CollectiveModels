using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Controls {
	/// <summary>
	/// Graf hustot
	/// </summary>
	public class DensityGraph : Controls.Graph {
		private System.Windows.Forms.ToolTip toolTip;
		private System.ComponentModel.IContainer components;
		private PavelStransky.Controls.DensityBox pbDensity;

		/// <summary>
		/// Z�kladn� konstruktor (pro designer)
		/// </summary>
		public DensityGraph() : base() {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="variable">Data do grafu</param>
		public DensityGraph(Variable variable) : base(variable) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="variable">Data do grafu</param>
		/// <param name="index">Index pro GraphArray</param>
		public DensityGraph(Variable variable, int index) : base(variable, index) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="variable">Data do grafu</param>
		/// <param name="index">Index pro GraphArray</param>
		/// <param name="position">Pozice okna relativn� vzhledem k rodi�i</param>
		public DensityGraph(Variable variable, int index, RectangleF position) : base(variable, index, position) {}

		/// <summary>
		/// Zobraz� graf pro ty typy prom�nn�, pro kter� um� vykreslit. Jinak v�jimka
		/// </summary>
		protected override void ShowGraph() {
			base.ShowGraph();

			if(!(this.Item is Matrix))
				this.pbDensity.Image = null;
			else {
				this.pbDensity.SetMatrix(this.Item as Matrix);
				if(this.GraphItem.ShowLabels)
					this.pbDensity.SetLabels(this.GraphItem.LabelsX, this.GraphItem.LabelsY);
				else
					this.pbDensity.SetLabels(null, null);

				this.toolTip.RemoveAll();
			}
		}

		/// <summary>
		/// Graf
		/// </summary>
		protected new Expression.DensityGraph GraphItem {get {return base.GraphItem as Expression.DensityGraph;}}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		protected override void InitializeComponent() {
			base.InitializeComponent();

			this.components = new System.ComponentModel.Container();
			this.pbDensity = new PavelStransky.Controls.DensityBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// pbDensity
			// 
			this.pbDensity.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.pbDensity.Location = new System.Drawing.Point(0, 24);
			this.pbDensity.Name = "pbDensity";
			this.pbDensity.Size = new System.Drawing.Size(640, 448);
			this.pbDensity.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pbDensity.TabIndex = 0;
			this.pbDensity.TabStop = false;
			this.pbDensity.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbDensity_MouseMove);
			// 
			// DensityGraph
			// 
			this.Controls.Add(this.pbDensity);
			this.Name = "DensityGraph";
			this.Size = new System.Drawing.Size(640, 496);
			this.Controls.SetChildIndex(this.pbDensity, 0);
			this.ResumeLayout(false);
		}
		#endregion

		/// <summary>
		/// Pohyb my�i - nastav�me ToolTip
		/// </summary>
		private void pbDensity_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			this.SetToolTip(e.X, e.Y);
		}

		/// <summary>
		/// Nastav� ToolTip
		/// </summary>
		/// <param name="x">X - ov� sou�adnice my�i</param>
		/// <param name="y">Y - ov� sou�adnice my�i</param>
		private void SetToolTip(int x, int y) {
			Matrix m = this.Item as Matrix;
			if(m != null) {
				int i = m.LengthX * x / this.pbDensity.Width;
				int j = m.LengthY * y / this.pbDensity.Height;

				string tip = string.Format("({0}, {1}) = {2,4:F}",
					this.GraphItem.LabelsX != null && this.GraphItem.LabelsX.Count > i ? this.GraphItem.LabelsX[i] as string : i.ToString(),
					this.GraphItem.LabelsY != null && this.GraphItem.LabelsY.Count > j ? this.GraphItem.LabelsY[j] as string : j.ToString(),
					m[i, j]);

				this.toolTip.SetToolTip(this.pbDensity, tip);
			}
		}
	}
}