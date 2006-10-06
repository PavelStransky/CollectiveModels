using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vyp�e text (prom�nnou) do writeru
    /// </summary>
    public class FnPrint : FunctionDefinition {
        private int counter = 0;
        private int maxCounter = 1;

        public override string Name { get { return name; } }
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsMinNumber(arguments, 1);
            this.CheckArgumentsMaxNumber(arguments, 2);

            this.CheckArgumentsType(arguments, 1, typeof(int));

            if(arguments.Count > 1)
                this.maxCounter = (int)arguments[1];

            this.counter++;

            if(this.counter >= this.maxCounter) {
                object toPrint = Atom.EvaluateAtomObject(context, arguments[0]);
                if(writer != null)
                    writer.WriteLine(toPrint.ToString());

                this.counter = 0;
            }

            return this.counter;
        }

        private const string name = "print";
        private const string help = "Vyp�e v�sledek v�razu do prom�nn�";
        private const string parameters = "v�raz [; po�et vol�n�, po kolika dojde k v�pisu (int)]";
    }
}
