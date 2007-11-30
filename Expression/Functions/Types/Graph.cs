using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Vytvoøí graf
	/// </summary>
	/// <param name="args">Argumenty funkce
	/// 0 ... data k vykreslení (vektor, øada, øada øad)
    /// 1 ... pozadí
	/// 2 ... parametry (string)
	/// 3 ... parametry jednotlivých køivek (array of string)
    /// 4 ... parametry pozadí
	/// 5 ... chyby ke køivkám
    /// Možnosti zadání (data, pozadí):
    /// (1, 1)
    /// (Array(N), 1) - k jednomu pozadí více køivek najednou
    /// (Array(N, 1), 1) - k jednomu pozadí více køivek, ale postupnì (animace)
    /// (Array(N), Array(N)) - jedna køivka k jednomu pozadí
    /// (Array(N, M), Array(N)) - M køivek ke každému pozadí
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
            // Druhý parametr - pozadí
            object item = arguments.Count > 1 ? arguments[1] : null;
            bool oneBackground;
            int groupsB;
            TArray background = this.PrepareBackground(out groupsB, out oneBackground, item);            

            // První parametr - data
            item = arguments[0];
            int groupsI;
            TArray data = this.ProceedData(out groupsI, groupsB, item);

            // Doplníme poèet pozadí do poètu skupin køivek
            background = this.FillBackground(oneBackground, groupsB, groupsI, background);

            // Od této chvíle už je skupin daných poètem pozadí a skupin daný poètem køivek stejný
            groupsB = groupsI;

            // Poèet køivek ve skupinì grafu
            int[] groups = this.NumCurvesFromData(data);

            // Tøetí parametr - chybové úseèky
            item = arguments.Count > 2 ? arguments[2] : null;
            TArray errors = this.ProceedErrors(groups, item);

            // 4. parametr - vlastnosti grafu (string nebo Context)
            item = arguments.Count > 3 ? arguments[3] : null;
            Context graphParams = this.ProceedGlobalParams(guider, item);

			// 5. parametr - vlastnosti jednotlivých skupin (pozadí) grafu
            // (string nebo Context nebo Array of Context)
            item = arguments.Count > 4 ? arguments[4] : null;
            TArray groupParams = this.ProceedGroupParams(guider, groupsI, item);
                        
            // 6. parametr - vlastnosti jednotlivých køivek grafu
            item = arguments.Count > 5 ? arguments[5] : null;
            TArray itemParams = this.ProceedItemParams(guider, groups, item);

            return new Graph(data, background, errors, graphParams, groupParams, itemParams);
		}

        private const string name = "graph";
	}
}
