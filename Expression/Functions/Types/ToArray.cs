using System;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates array from some non-array types
    /// </summary>
    public class ToArray: Fnc {
        public override string Help { get { return Messages.HelpToArray; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PToArray1, Messages.PToArray1Description, null, typeof(FileData), typeof(List));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is FileData) {
                FileData f = item as FileData;
                if(f.Binary)
                    throw new FncException(Messages.ToArrayEMNotTextFile);
                return f.Lines.Clone();
            }

            else if(item is List)
                return (item as List).ToTArray();

            return null;
        }
    }
}
