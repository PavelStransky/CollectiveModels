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

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.writer = writer;

            ArrayList evaluatedArguments = this.EvaluateArguments(context, arguments, writer);
            this.CheckArgumentsMinNumber(evaluatedArguments, 1);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 3);

            string name = defaultName;
            if(evaluatedArguments.Count > 1 && evaluatedArguments[1] is string)
                name = (string)evaluatedArguments[1];
            else if(arguments[0] is string)
                name = (string)arguments[0];
            else if(arguments[0] is Atom)
                name = (arguments[0] as Atom).Expression;

            int numColumns = 1;
            if(evaluatedArguments[0] is TArray)
                numColumns = (int)System.Math.Sqrt((evaluatedArguments[0] as TArray).Count - 1) + 1;
            if(evaluatedArguments.Count > 1 && evaluatedArguments[1] is int)
                numColumns = (int)evaluatedArguments[1];
            else if(evaluatedArguments.Count > 2 && evaluatedArguments[2] is int)
                numColumns = (int)evaluatedArguments[2];

            TArray graph = null;
            if(evaluatedArguments[0] is Graph) {
                graph = new TArray();
                graph.Add(evaluatedArguments[0]);
            }
            else if(evaluatedArguments[0] is TArray && (evaluatedArguments[0] as TArray).ItemType == typeof(Graph))
                graph = evaluatedArguments[0] as TArray;
            else
                this.BadTypeError(evaluatedArguments[0], 0);

            context.OnGraphRequest(new GraphRequestEventArgs(graph, name, numColumns));

            return evaluatedArguments[0];
        }

        private const string defaultName = "Graph";

		private const string help = "Zobrazí graf";
		private const string parameters = "data k vykreslení (Graph | Array of Graphs) [; název grafu (string) [; poèet sloupcù (int)]]";	
	}
}