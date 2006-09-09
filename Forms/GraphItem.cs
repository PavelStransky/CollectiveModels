using System;
using System.Drawing;

using PavelStransky.Math;

namespace PavelStransky.Forms {
	/// <summary>
	/// Obecn� objekt do grafu
	/// </summary>
	public class GraphItem {
		protected object data;
		protected string name;

		/// <summary>
		/// Data k vykreslen�
		/// </summary>
		public object Data {get {return this.data;}}

		/// <summary>
		/// Jm�no objektu grafu
		/// </summary>
		public object Name {get {return this.name;}}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="data">Data k vykreslen�</param>
		public GraphItem(object data, string name) {
			this.data = data;
			this.name = name;
		}
	}

	/// <summary>
	/// Objekt do ��rov�ho grafu
	/// </summary>
	public class GraphItemLine: GraphItem {
		/// <summary>
		/// Typy spojnic grafu - p��m� ��ra, hladk� k�ivka, nic
		/// </summary>
		public enum GraphLineTypes {Line, Curve, None}

		/// <summary>
		/// Typy bod� grafu - vypln�n� �tvere�ek, vypln�n� kole�ko, nic
		/// </summary>
		public enum GraphPointTypes {FillRectangle, FillCircle, None}

		/// <summary>
		/// Barvy (pro jednodu��� indexaci)
		/// </summary>
		private Color [] colors = {Color.Black, Color.Blue, Color.Magenta, Color.Yellow, Color.Red, 
									  Color.Cyan, Color.Pink};

		private Pen lineColor;
		private Brush pointColor;

		private GraphLineTypes graphLineType;
		private GraphPointTypes graphPointType;

		/// <summary>
		/// Vr�t� data ve form� vektoru
		/// </summary>
		public PointVector DataVector {get {return this.data as PointVector;}}

		/// <summary>
		/// Barva �ar
		/// </summary>
		public Pen LineColor {get {return this.lineColor;}}

		/// <summary>
		/// Barva bod�
		/// </summary>
		public Brush PointColor {get {return this.pointColor;}}

		/// <summary>
		/// Typ spojnic bod�
		/// </summary>
		public GraphLineTypes GraphLineType {get {return this.graphLineType;}}

		/// <summary>
		/// Typy bod�
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
		/// Data ve form� matice
		/// </summary>
		public Matrix DataMatrix {get {return this.data as Matrix;}}
	
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="data"></param>
		public GraphItemDensity(Matrix data, string name) : base(data, name) {}
	}
}
