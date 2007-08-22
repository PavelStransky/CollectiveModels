using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Shows a graph
	/// </summary>
	public class Show: Fnc {
		public override string Help {get {return Messages.HelpShow;}}

        protected override void CreateParameters() {
            this.SetNumParams(5);

            this.SetParam(0, true, true, false, Messages.PGraph, Messages.PGraphDescription, null, typeof(Graph), typeof(TArray));
            this.SetParam(1, false, true, false, Messages.PGraphName, Messages.PGraphNameDescription, "Graph", typeof(string));
            this.SetParam(2, false, true, false, Messages.PNumColumns, Messages.PNumColumnsDescription, 1, typeof(int));
            this.SetParam(3, false, true, false, Messages.PPositionWindow, Messages.PPositionWindowDescription, new PointD(-1.0, -1.0), typeof(PointD));
            this.SetParam(4, false, true, false, Messages.PSizeWindow, Messages.PSizeWindowDescription, new PointD(-1.0, -1.0), typeof(PointD));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int count = arguments.Count;

            string name = (string)arguments[1];
            int numColumns = (int)arguments[2];

            PointD position = (PointD)arguments[3];
            PointD size = (PointD)arguments[4];

            TArray graph = null;
            if(arguments[0] is Graph) {
                graph = new TArray(typeof(Graph), 1);
                graph[0] = arguments[0];
            }
            else 
                graph = arguments[0] as TArray;

            guider.Context.OnEvent(new ContextEventArgs(ContextEventType.GraphRequest, graph, name, numColumns, position, size));

            return graph;
        }
	}
}