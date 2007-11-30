using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Vytvo�� graf
	/// </summary>
	/// <param name="args">Argumenty funkce
	/// 0 ... data k vykreslen� (vektor, �ada, �ada �ad)
    /// 1 ... pozad�
	/// 2 ... parametry (string)
	/// 3 ... parametry jednotliv�ch k�ivek (array of string)
    /// 4 ... parametry pozad�
	/// 5 ... chyby ke k�ivk�m
    /// Mo�nosti zad�n� (data, pozad�):
    /// (1, 1)
    /// (Array(N), 1) - k jednomu pozad� v�ce k�ivek najednou
    /// (Array(N, 1), 1) - k jednomu pozad� v�ce k�ivek, ale postupn� (animace)
    /// (Array(N), Array(N)) - jedna k�ivka k jednomu pozad�
    /// (Array(N, M), Array(N)) - M k�ivek ke ka�d�mu pozad�
	/// </param>
	public class FnGraph: FncGraph {
        public override string Name { get { return name; } }
		public override string Help {get {return Messages.HelpGraph;}}

        protected override void CreateParameters() {
            this.SetNumParams(6);

            this.SetParam(0, false, true, false, Messages.PGraph1, Messages.P1GraphDescription, null,
                typeof(TArray), typeof(Vector), typeof(PointVector));
            this.SetParam(1, false, true, false, Messages.P2Graph, Messages.P2GraphDescription, null,
                typeof(TArray), typeof(Matrix));
            this.SetParam(2, false, true, false, Messages.P3Graph, Messages.P3GraphDescription, null,
                typeof(TArray), typeof(Vector));
            this.SetParam(3, false, true, false, Messages.P4Graph, Messages.P4GraphDescription, null,
                typeof(string), typeof(Context));
            this.SetParam(4, false, true, false, Messages.P5Graph, Messages.P5GraphDescription, null,
                typeof(TArray), typeof(string), typeof(Context));
            this.SetParam(5, false, true, false, Messages.P6Graph, Messages.P6GraphDescription, null,
                typeof(TArray), typeof(string), typeof(Context));
        }

		protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            // Druh� parametr - pozad�
            object item = arguments.Count > 1 ? arguments[1] : null;
            bool oneBackground;
            int groupsB;
            TArray background = this.PrepareBackground(out groupsB, out oneBackground, item);            

            // Prvn� parametr - data
            item = arguments[0];
            int groupsI;
            TArray data = this.ProceedData(out groupsI, groupsB, item);

            // Dopln�me po�et pozad� do po�tu skupin k�ivek
            background = this.FillBackground(oneBackground, groupsB, groupsI, background);

            // Od t�to chv�le u� je skupin dan�ch po�tem pozad� a skupin dan� po�tem k�ivek stejn�
            groupsB = groupsI;

            // Po�et k�ivek ve skupin� grafu
            int[] groups = this.NumCurvesFromData(data);

            // T�et� parametr - chybov� �se�ky
            item = arguments.Count > 2 ? arguments[2] : null;
            TArray errors = this.ProceedErrors(groups, item);

            // 4. parametr - vlastnosti grafu (string nebo Context)
            item = arguments.Count > 3 ? arguments[3] : null;
            Context graphParams = this.ProceedGlobalParams(guider, item);

			// 5. parametr - vlastnosti jednotliv�ch skupin (pozad�) grafu
            // (string nebo Context nebo Array of Context)
            item = arguments.Count > 4 ? arguments[4] : null;
            TArray groupParams = this.ProceedGroupParams(guider, groupsI, item);
                        
            // 6. parametr - vlastnosti jednotliv�ch k�ivek grafu
            item = arguments.Count > 5 ? arguments[5] : null;
            TArray itemParams = this.ProceedItemParams(guider, groups, item);

            return new Graph(data, background, errors, graphParams, groupParams, itemParams);
		}

        private const string name = "graph";
	}
}
