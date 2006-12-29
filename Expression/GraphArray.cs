using System;

namespace PavelStransky.Expression {
	/// <summary>
	/// Øada grafù
	/// </summary>
	public class GraphArray: TArray {
		private int lengthX = 1;
		private int lengthY = 1;

		// Délky se poèítají automaticky podle poètu jednotlivých grafù
		private bool autoLengths = true;

		/// <summary>
		/// Poèet grafù vedle sebe svisle
		/// </summary>
		public int LengthX {get {return this.lengthX;}}

		/// <summary>
		/// Poèet grafù vedle sebe vodorovnì
		/// </summary>
		public int LengthY {get {return this.lengthY;}}

		/// <summary>
		/// Konstruktor
		/// </summary>
		public GraphArray(): base(false) {}

		public GraphArray(int lengthX, int lengthY): this() {
			this.lengthX = lengthX;
			this.lengthY = lengthY;
			this.autoLengths = false;
		}

		/// <summary>
		/// Indexer
		/// </summary>
		public new Graph this[int index] {get {return base[index] as Graph;} set {base[index] = value;}}

		public override int Add(object value) {
			int result = base.Add(value);

			if(this.autoLengths) {
				this.AutoSetLengths();
			}

			return result;
		}

		/// <summary>
		/// Nastaví LengthX a LengthY podle algoritmu automaticky
		/// </summary>
		public void AutoSetLengths() {
			this.lengthX = 1;
			this.lengthY = 1;
			
			bool changeX = true;

			while(this.lengthX * this.lengthY < this.Count) {
				if(changeX) 
					this.lengthX++;
				else
					this.lengthY++;

				changeX = !changeX;
			}
		}
	}
}
