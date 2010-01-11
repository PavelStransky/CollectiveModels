using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Provede import dat
	/// </summary>
	public class FnImport: FncIE {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpImport; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PFileName, Messages.PFileNameDescription, null, typeof(string));
            this.SetParam(1, false, true, false, Messages.PFileType, Messages.PFileTypeDescription, "binary", typeof(string));
            this.SetParam(2, false, true, false, Messages.PLinesOmit, Messages.PLinesOmitDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object result = null;

			string fileName = arguments[0] as string;

            if((string)arguments[1] == "matlab") {
                int linesOmit = (int)arguments[2];

                // Import dat z matlabu - matice, nevíme, jaké má rozmìry
                FileStream f = new FileStream(fileName, FileMode.Open);
                StreamReader t = new StreamReader(f);
                result = this.ImportMatlab(t, linesOmit);
                t.Close();
                f.Close();
            }
            else {
                Import import = new Import(fileName, this.Binary(arguments, 1));
                result = import.Read();
                import.Close();
            }

			return result;
		}

		/// <summary>
		/// Provede import dat z matlabu
		/// </summary>
		/// <param name="t">StreamReader</param>
        /// <param name="linesOmit">Poèet vynechaných øádek</param>
		private object ImportMatlab(StreamReader t, int linesOmit) {
			ArrayList rows = new ArrayList();

			int columns = -1;

            for(int i = 0; i < linesOmit; i++)
                t.ReadLine();

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

        private const string name = "import";
	}
}
