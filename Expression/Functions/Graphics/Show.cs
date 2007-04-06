using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Zobrazí graf
	/// </summary>
	public class Show: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 1);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 3);

            this.CheckArgumentsType(evaluatedArguments, 0, typeof(Graph));
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(string));
            this.CheckArgumentsType(evaluatedArguments, 2, typeof(int));
        }

        protected override object EvaluateArray(Guider guider, ArrayList arguments) {
            return this.EvaluateFn(guider, arguments);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            string name = defaultName;
            int count = arguments.Count;
            int numColumns = 1;

            if(count > 1)
                name = (string)arguments[1];

            if(count > 2)
                numColumns = (int)arguments[2];

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

        private const string defaultName = "Graph";

		private const string help = "Zobrazí graf";
		private const string parameters = "data k vykreslení (Graph) [; název grafu (string) [; poèet sloupcù (int)]";	
	}
}