using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Z glob�ln�ho kontextu vyzvedne prom�nnou
    /// </summary>
    public class GetGlobalVar: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsNumber(arguments, 1);

            ArrayList args = new ArrayList(); args.Add(arguments[0]);
            this.CheckArgumentsType(args, 0, typeof(string));

            Context c;
            Import import = new Import(Context.GlobalContextFileName, true);
            c = import.Read() as Context;
            import.Close();

            string name = args[0] as string;
            return c[name];
        }

        private const string help = "Z glob�ln�ho kontextu vr�t� ur�enou prom�nnou.";
        private const string parameters = "N�zev prom�nn�";
    }
}
