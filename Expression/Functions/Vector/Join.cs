using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
    /// Joins 1D Arrays into one array
	/// </summary>
	public class JoinArray: Fnc {
		public override string Help {get {return Messages.HelpJoinArray;}}

        protected override void CreateParameters() {
            this.SetNumParams(1, true);
            this.SetParam(0, true, true, false, Messages.P1JoinArray, Messages.P1JoinArrayDescription, null, typeof(TArray));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int count = arguments.Count;
            int items = 0;
            Type type = null;

            for(int i = 0; i < count; i++) {
                if(!(arguments[i] is TArray))
                    this.BadTypeError(arguments[i], i);

                TArray a = arguments[i] as TArray;
                if(type == null)
                    type = a.GetItemType();

                if(a.Rank > 1)
                    throw new FncException(Messages.EMNot1D, string.Format(Messages.EMRankDetail, a.Rank));

                if(type != a.GetItemType())
                    throw new FncException(Messages.EMBadArrayItemType,
                        string.Format(Messages.EMBadArrayItemTypeDetail, type, a.GetItemType()));

                items += a.GetNumElements();
            }

            TArray result = new TArray(type, items);
            int ri = 0;

            for(int i = 0; i < count; i++) {
                TArray a = arguments[i] as TArray;
                int length = a.Length;

                for(int j = 0; j < length; j++)
                    result[ri++] = a[j];
            }

            return result;
        }
    }
}
