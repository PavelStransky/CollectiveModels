using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Shows a graph
	/// </summary>
	public class Show: FunctionDefinition {
		public override string Help {get {return Messages.ShowHelp;}}

        protected override void CreateParameters() {
            this.NumParams(3);

            this.SetParam(0, true, true, false, Messages.PGraph, Messages.PGraphDescription, null, typeof(Graph), typeof(TArray));
            this.SetParam(1, false, true, false, Messages.PGraphName, Messages.PGraphNameDescription, "Graph", typeof(string));
            this.SetParam(2, false, true, false, Messages.PNumColumns, Messages.PNumColumnsDescription, 1, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int count = arguments.Count;

            string name = (string)arguments[1];
            int numColumns = (int)arguments[2];

            TArray graph = null;
            if(arguments[0] is Graph) {
                graph = new TArray(typeof(Graph), 1);
                graph[0] = arguments[0];
            }
            else 
                graph = arguments[0] as TArray;

            guider.Context.OnEvent(new ContextEventArgs(ContextEventType.GraphRequest, graph, name, numColumns));

            return graph;
        }
	}
}