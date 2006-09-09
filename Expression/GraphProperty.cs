using System;
using System.Drawing;

namespace PavelStransky.Expression {
	/// <summary>
	/// Vlastnosti jedn� k�ivky grafu
	/// </summary>
	public class GraphProperty {
		/// <summary>
		/// Styly spojnic bod�
		/// </summary>
		public enum LineStyles {Line, Curve, None}

		/// <summary>
		/// Styly bod�
		/// </summary>
		public enum PointStyles {Circle, Square, None, HLines, VLines, FCircle, FSquare}

		private string name;
		private LineStyles lineStyle;
		private Color lineColor;
		private float lineWidth;
		private PointStyles pointStyle;
		private Color pointColor;
		private int pointSize;
		private bool showLabel;
		private Color labelColor;

		/// <summary>
		/// Jm�no k�ivky
		/// </summary>
		public string Name {get {return this.name;}}

		/// <summary>
		/// Styl ��ry
		/// </summary>
		public LineStyles LineStyle {get {return this.lineStyle;}}

		/// <summary>
		/// Barva k�ivky
		/// </summary>
		public Color LineColor {get {return this.lineColor;}}

		/// <summary>
		/// ���ka ��ry
		/// </summary>
		public float LineWidth {get {return this.lineWidth;}}

		/// <summary>
		/// Styl bodu
		/// </summary>
		public PointStyles PointStyle {get {return this.pointStyle;}}

		/// <summary>
		/// Barva bod�
		/// </summary>
		public Color PointColor {get {return this.pointColor;}}

		/// <summary>
		/// Velikost bodu
		/// </summary>
		public int PointSize {get {return this.pointSize;}}

		/// <summary>
		/// Maj� se zobrazit popisky u grafu?
		/// </summary>
		public bool ShowLabel {get {return this.showLabel;}}

		/// <summary>
		/// Barva popisku grafu
		/// </summary>
		public Color LabelColor {get {return this.labelColor;}}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="name">Jm�no k�ivky</param>
		/// <param name="lineStyle">Styl ��ry</param>
		/// <param name="lineColor">Barva k�ivky</param>
		/// <param name="lineWidth">���ka ��ry</param>
		/// <param name="pointStyle">Styl bodu</param>
		/// <param name="pointColor">Barva bodu</param>
		/// <param name="pointSize">Velikost bodu</param>
		/// <param name="showLabel">Zobrazit popisky u ��ry grafu? (label)</param>
		/// <param name="labelColor">Barva popisk� k�ivky</param>
		public GraphProperty(string name, string lineStyle, Color lineColor, float lineWidth,
			string pointStyle, Color pointColor, int pointSize, bool showLabel, Color labelColor) {
			this.name = name;
			this.lineStyle = (LineStyles)Enum.Parse(typeof(LineStyles), lineStyle, true);
			this.lineColor = lineColor;
			this.lineWidth = lineWidth;
			this.pointColor = pointColor;
			this.pointStyle = (PointStyles)Enum.Parse(typeof(PointStyles), pointStyle, true);
			this.pointSize = pointSize;
			this.showLabel = showLabel;
			this.labelColor = labelColor;
		}
	}

	/// <summary>
	/// Vlastnosti jedn� osy grafu
	/// </summary>
	public class AxeProperty: GraphProperty {
		private bool showAxe;

		/// <summary>
		/// M� se zobrazit osa?
		/// </summary>
		public bool ShowAxe {get {return this.showAxe;}}

		public AxeProperty(string name, string lineStyle, Color lineColor, float lineWidth, 
			string pointStyle, Color pointColor, int pointSize, bool showLabel, Color labelColor,
			bool showAxe) 
			: base (name, lineStyle, lineColor, lineWidth, pointStyle, pointColor, pointSize, showLabel, labelColor) {
			this.showAxe = showAxe;
		}
	}
}
