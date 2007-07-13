using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Descending sort
    /// </summary>
    public class SortDesc: FunctionDefinition {
        public override string Help { get { return Messages.HelpSortDesc; } }

        protected override void CreateParameters() {
            this.NumParams(2);

            this.SetParam(0, true, true, false, Messages.PSort1, Messages.PSort1Description, null, typeof(ISortable));
            this.SetParam(1, false, true, false, Messages.PSort2, Messages.PSort2Description, null, typeof(ISortable));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            ISortable item = arguments[0] as ISortable;
            ISortable keys = arguments[1] as ISortable;

            if(keys != null)
                return item.SortDesc(arguments[1] as ISortable);
            else
                return item.SortDesc();
        }
    }
}
