using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Sets new parameters to a graph
    /// </summary>
    public class SetGraphParams: FncGraph {
        public override string Help { get { return Messages.HelpSetGraphParams; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);

            this.SetParam(0, true, true, false, Messages.PGraph, Messages.PGraphDescription, null,
                typeof(Graph));
            this.SetParam(1, false, true, false, Messages.P4Graph, Messages.P4GraphDescription, null,
                typeof(string), typeof(Context));
            this.SetParam(2, false, true, false, Messages.P5Graph, Messages.P5GraphDescription, null,
                typeof(TArray), typeof(string), typeof(Context));
            this.SetParam(3, false, true, false, Messages.P6Graph, Messages.P6GraphDescription, null,
                typeof(TArray), typeof(string), typeof(Context));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Graph graph = arguments[0] as Graph;

            int[] groups = graph.NumCurves();

            // 2. parametr - vlastnosti grafu (string nebo Context)
            object item = arguments.Count > 1 ? arguments[1] : null;
            Context graphParams = this.ProceedGlobalParams(guider, item);

            // 3. parametr - vlastnosti jednotlivých skupin (pozadí) grafu
            // (string nebo Context nebo Array of Context)
            item = arguments.Count > 2 ? arguments[2] : null;
            TArray groupParams = this.ProceedGroupParams(guider, groups.Length, item);

            // 4. parametr - vlastnosti jednotlivých køivek grafu
            item = arguments.Count > 3 ? arguments[3] : null;
            TArray itemParams = this.ProceedItemParams(guider, groups, item);

            graph.SetParams(graphParams, groupParams, itemParams);

            return graph;
        }
    }
}