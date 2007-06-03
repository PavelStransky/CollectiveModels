using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Pou�ije kontext k v�po�t�m
    /// </summary>
    public class UseContext : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 1);

            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(Context));

            int count = evaluatedArguments.Count;

            // P��kazy mohou b�t null
            if(count > 1 && evaluatedArguments[1] != null)
                this.CheckArgumentsType(evaluatedArguments, 1, evaluateArray, typeof(string));

            for(int i = 2; i < count; i++)
                this.CheckArgumentsType(evaluatedArguments, i, evaluateArray, typeof(string));

        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Context result = arguments[0] as Context;

            int count = arguments.Count;
            for(int i = 2; i < count; i++) {
                Variable v = guider.Context[arguments[i] as string];
                result.SetVariable(v.Name, v.Item);
            }

            if(count > 1 && arguments[1] != null)
                Atom.EvaluateAtomObject(new Guider(result), arguments[1]);

            return result;
        }

        private const string help = "Pou�ije zadan� kontext k v�po�t�m.";
        private const string parameters = "Kontext; [P��kazy;][Prom�nn� kop�rovan� z aktu�ln�ho kontextu...]";
    }
}
