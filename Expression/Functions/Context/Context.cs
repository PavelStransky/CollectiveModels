using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vytvo�� nov� kontext
    /// </summary>
    public class FnContext : FunctionDefinition {
        public override string Name { get { return name; } }
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            int count = evaluatedArguments.Count;

            // Prvn� argument m��e b�t null
            if(count > 0 && evaluatedArguments[0] != null)
                this.CheckArgumentsType(evaluatedArguments, 0, typeof(string));

            for(int i = 1; i < count; i++)
                this.CheckArgumentsType(evaluatedArguments, i, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Context result = new Context(guider.Context.Directory);
            guider.Context.OnEvent(new ContextEventArgs(ContextEventType.NewContext, result));

            int count = arguments.Count;

            for(int i = 1; i < count; i++) {
                Variable v = guider.Context[arguments[i] as string];
                result.SetVariable(v.Name, v.Item);
            }

            if(count > 0 && arguments[0] != null) 
                Atom.EvaluateAtomObject(new Guider(result), arguments[0]);

            return result;
        }

        private const string name = "context";
        private const string help = "Vytvo�� nov� kontext.";
        private const string parameters = "[P��kazy nov�ho kontextu (string);][Prom�nn� kop�rovan� z aktu�ln�ho kontextu (string) ...]";
    }
}
