using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Forms {
	/// <summary>
	/// Èárový graf
	/// </summary>
	public class LineGraph : Graph {
		private LineBox pbLines;

		/// <summary>
		/// Konstruktor pro designer
		/// </summary>
		public LineGraph() : base() {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="variable">Data do grafu</param>
		public LineGraph(Variable variable) : base(variable) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="variable">Data do grafu</param>
		/// <param name="index">Index (pro GraphArray)</param>
		public LineGraph(Variable variable, int index) : base(variable, index) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="variable">Data do grafu</param>
		/// <param name="index">Index pro GraphArray</param>
		/// <param name="position">Pozice okna relativnì vzhledem k rodièi</param>
		public LineGraph(Variable variable, int index, RectangleF position) : base(variable, index, position) {}

		/// <summary>
		/// Graf
		/// </summary>
		protected new Expression.LineGraph GraphItem {get {return base.GraphItem as Expression.LineGraph;}}

		/// <summary>
		/// Zobrazí graf
		/// </summary>
		protected override void ShowGraph() {
			base.ShowGraph();

			if(this.Item is Vector || this.Item is PointVector) {
				Expression.Array array = new PavelStransky.Expression.Array();
				array.Add(this.Item);

				Expression.Array errors = null; 
				if(this.GraphItem.Errors != null) {
					errors = new PavelStransky.Expression.Array();
					errors.Add(this.GraphItem.Errors);
				}

				this.pbLines.SetData(array, errors);
			}
			else if(this.Item as Expression.Array != null) 
				this.pbLines.SetData(this.Item as Expression.Array, this.GraphItem.Errors as Expression.Array);
			else
				return;

			this.pbLines.SetMinMax(this.GraphItem.MinX, this.GraphItem.MaxX, this.GraphItem.MinY, this.GraphItem.MaxY); 
			this.pbLines.SetProperties(this.GraphItem.GraphsProperty(), this.GraphItem.AxesProperty());
			this.pbLines.Shift = this.GraphItem.Shift;
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		protected override void InitializeComponent() {
			base.InitializeComponent();

			this.pbLines = new LineBox();
			this.SuspendLayout();
			// 
			// pbLines
			// 
			this.pbLines.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.pbLines.BackColor = System.Drawing.SystemColors.Window;
			this.pbLines.Location = new System.Drawing.Point(0, 24);
			this.pbLines.Name = "pbLines";
			this.pbLines.Size = new System.Drawing.Size(456, 408);
			this.pbLines.TabIndex = 0;
			this.pbLines.TabStop = false;
			// 
			// LineGraph
			// 
			this.Controls.Add(this.pbLines);
			this.Name = "LineGraph";
			this.Size = new System.Drawing.Size(456, 504);
			this.Controls.SetChildIndex(this.pbLines, 0);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
