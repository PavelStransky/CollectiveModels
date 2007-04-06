using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Do glob�ln�ho kontextu ulo�� prom�nn�
    /// </summary>
    public class SetGlobalVar: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            int count = evaluatedArguments.Count;

            for(int i = 0; i < count; i++)
                this.CheckArgumentsType(evaluatedArguments, i, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            string fileName = Context.GlobalContextFileName;
            FileInfo fileInfo = new FileInfo(fileName);

            Context c;
            if(fileInfo.Exists) {
                Import import = new Import(Context.GlobalContextFileName, true);
                c = import.Read() as Context;
                import.Close();
            }
            else
                c = new Context();

            int count = arguments.Count;
            for(int i = 0; i < count; i++) {
                Variable v = guider.Context[arguments[i] as string];
                c.SetVariable(v.Name, v.Item);
            }

            Export export = new Export(Context.GlobalContextFileName, true);
            export.Write(c);
            export.Close();

            return c;
        }

        private const string help = "Do glob�ln�ho kontextu ulo�� prom�nn�.";
        private const string parameters = "[Prom�nn� kop�rovan� z aktu�ln�ho kontextu (string) ...]";
    }
}
