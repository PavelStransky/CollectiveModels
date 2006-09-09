using System;
using System.Drawing;

using PavelStransky.Math;

namespace PavelStransky.Forms {
	/// <summary>
	/// Obecný objekt do grafu
	/// </summary>
	public class GraphItem {
		protected object data;
		protected string name;

		/// <summary>
		/// Data k vykreslení
		/// </summary>
		public object Data {get {return this.data;}}

		/// <summary>
		/// Jméno objektu grafu
		/// </summary>
		public object Name {get {return this.name;}}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="data">Data k vykreslení</param>
		public GraphItem(object data, string name) {
			this.data = data;
			this.name = name;
		}
	}

	/// <summary>
	/// Objekt do èárového grafu
	/// </summary>
	public class GraphItemLine: GraphItem {
		/// <summary>
		/// Typy spojnic grafu - pøímá èára, hladká køivka, nic
		/// </summary>
		public enum GraphLineTypes {Line, Curve, None}

		/// <summary>
		/// Typy bodù grafu - vyplnìný ètvereèek, vyplnìné koleèko, nic
		/// </summary>
		public enum GraphPointTypes {FillRectangle, FillCircle, None}

		/// <summary>
		/// Barvy (pro jednodušší indexaci)
		/// </summary>
		private Color [] colors = {Color.Black, Color.Blue, Color.Magenta, Color.Yellow, Color.Red, 
									  Color.Cyan, Color.Pink};

		private Pen lineColor;
		private Brush pointColor;

		private GraphLineTypes graphLineType;
		private GraphPointTypes graphPointType;

		/// <summary>
		/// Vrátí data ve formì vektoru
		/// </summary>
		public PointVector DataVector {get {return this.data as PointVector;}}

		/// <summary>
		/// Barva èar
		/// </summary>
		public Pen LineColor {get {return this.lineColor;}}

		/// <summary>
		/// Barva bodù
		/// </summary>
		public Brush PointColor {get {return this.pointColor;}}

		/// <summary>
		/// Typ spojnic bodù
		/// </summary>
		public GraphLineTypes GraphLineType {get {return this.graphLineType;}}

		/// <summary>
		/// Typy bodù
		/// </summary>
		public GraphPointTypes GraphPointType {get {return this.graphPointType;}}

		/// <summary>
		/// Konstruktor
		/// </summary>
		public GraphItemLine(PointVector data, string name, Pen lineColor, Brush pointColor, GraphLineTypes graphLineType, GraphPointTypes graphPointType) : base(data, name) {
			this.lineColor = lineColor;
			this.pointColor = pointColor;
			this.graphLineType = graphLineType;
			this.graphPointType = graphPointType;
		}
	}

	/// <summary>
	/// Objekt do grafu s hustotou
	/// </summary>
	public class GraphItemDensity: GraphItem {
		/// <summary>
		/// Data ve formì matice
		/// </summary>
		public Matrix DataMatrix {get {return this.data as Matrix;}}
	
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="data"></param>
		public GraphItemDensity(Matrix data, string name) : base(data, name) {}
	}
}
