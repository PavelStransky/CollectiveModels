using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Provede import dat
	/// </summary>
	public class Import: FunctionDefinitionIE {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 2);
			this.CheckArgumentsType(evaluatedArguments, 0, typeof(string));

			if(evaluatedArguments.Count == 2)
				this.CheckArgumentsType(evaluatedArguments, 1, typeof(string));

			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			object result = null;

			string fileName = arguments[0] as string;
			FileStream f = new FileStream(fileName, FileMode.Open);

			if(arguments.Count == 2 && (string)arguments[1] == "matlab") {
				// Import dat z matlabu - matice, nevíme, jaké má rozmìry
				StreamReader t = new StreamReader(f);
				return this.ImportMatlab(t);
			}
			else if(this.Binary(arguments, 1)) {
				BinaryReader b = new BinaryReader(f);
				string typeName = b.ReadString();
				b.BaseStream.Position = 0;

                if(typeName == typeof(Array).FullName)
                    result = new Array(b);
                else if(typeName == typeof(Vector).FullName)
                    result = new Vector(b);
                else if(typeName == typeof(PointD).FullName)
                    result = new PointD(b);
                else if(typeName == typeof(PointVector).FullName)
                    result = new PointVector(b);
                else if(typeName == typeof(Matrix).FullName)
                    result = new Matrix(b);

				b.Close();
			}
			else {
				StreamReader t = new StreamReader(f);
				string typeName = t.ReadLine();
				t.BaseStream.Position = 0;
				t.DiscardBufferedData();

                if(typeName == typeof(Array).FullName)
                    result = new Array(t);
                else if(typeName == typeof(Vector).FullName)
                    result = new Vector(t);
                else if(typeName == typeof(PointD).FullName)
                    result = new PointD(t);
                else if(typeName == typeof(PointVector).FullName)
                    result = new PointVector(t);
                else if(typeName == typeof(Matrix).FullName)
                    result = new Matrix(t);

				t.Close();
			}

			f.Close();

			return result;
		}

		/// <summary>
		/// Provede import dat z matlabu
		/// </summary>
		/// <param name="t">StreamReader</param>
		private object ImportMatlab(StreamReader t) {
			ArrayList rows = new ArrayList();

			int columns = -1;

			string s;
			while((s = t.ReadLine()) != null) {
				string [] items = s.Replace('\t', ' ').Split(' ');
				
				if(columns < 0) {
					columns = 0;
					foreach(string item in items)
						if(item.Trim() != string.Empty)
							columns++;
				}
				
				Vector v = new Vector(columns);
				int index = 0;
				foreach(string item in items) {
					string trim = item.Trim();
					if(trim != string.Empty)
						v[index++] = double.Parse(trim);
				}

				rows.Add(v);
			}

			if(rows.Count == 0)
				return null;
			else if(columns == 1) {
				// Jeden sloupec - vracíme vektor
				Vector result = new Vector(rows.Count);
				for(int i = 0; i < rows.Count; i++)
					result[i] = (rows[i] as Vector)[0];
				return result;
			}
			else if(rows.Count == 1)
				// Jeden øádek - vracíme vektor
				return rows[0];
			else {
				// Jinak matice
				Matrix result = new Matrix(rows.Count, columns);

				for(int i = 0; i < rows.Count; i++) {
					Vector v = rows[i] as Vector;

					for(int j = 0; j < columns; j++)
						result[i, j] = v[j];
				}

				return result;
			}
		}

		private const string paramMatlab = "matlab";
		private const string help = "Provede import dat ze souboru (implicitnì binárnì).";
		private const string parameters = "jméno souboru (string) [;\"binary\" | \"text\" | \"matlab\"]";
	}
}
