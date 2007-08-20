using System.Collections;
using System;

using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// FullHelp operator
    /// </summary>
    public class OpFullHelp: Operator {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpFullHelp; } }
        public override OperatorPriority Priority { get { return OperatorPriority.MaxPriority; } }

        private FunctionDefinitions functions;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="functions">Slovník zaregistrovaných funkcí</param>
        public OpFullHelp(FunctionDefinitions functions)
            : base() {
            this.functions = functions;
        }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, false, false, Messages.PFnName, Messages.PFnNameDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            string fnName = string.Empty;
            if(item is string)
                fnName = item as string;
            else if(item is Function)
                fnName = (item as Function).Name;
            else
                throw new FunctionDefinitionException(Messages.EMBadHelpParameter, item.GetType().Name);

            return (this.functions[fnName] as FunctionDefinition).FullHelp;
        }

        private const string name = "??";
    }
}
