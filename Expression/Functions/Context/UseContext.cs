using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Uses the given context for specified calculations
    /// </summary>
    public class UseContext : Fnc {
        public override string Help { get { return Messages.HelpUseContext; } }

        protected override void CreateParameters() {
            this.SetNumParams(3, true);

            this.SetParam(0, true, true, false, Messages.PContext, Messages.PContextDescription, null, typeof(Context));
            this.SetParam(1, false, false, false, Messages.PCommands, Messages.PCommandsDescription, null);
            this.SetParam(2, false, false, false, Messages.PVariable, Messages.PVariableDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Context result = arguments[0] as Context;

            int count = arguments.Count;
            for(int i = 2; i < count; i++) {
                if(arguments[i] != null) {
                    Variable v = guider.Context[arguments[i] as string];
                    result.SetVariable(v.Name, v.Item);
                }
            }

            if(count > 1 && arguments[1] != null) 
                this.EvaluateAtomObject(new Guider(result), arguments[1]);            

            return result;
        }
    }
}
