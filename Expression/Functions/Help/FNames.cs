using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
    /// Returns names of all registered functions which begin with specified string
	/// </summary>
	public class FNames: Fnc {
		private FncList functions;
		
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="functions">Slovník zaregistrovaných funkcí</param>
		public FNames(FncList functions) : base() {
			this.functions = functions;
		}

		public override string Help {get {return Messages.HelpFNames;}}

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, false, true, false, Messages.PFnName, Messages.PFnNameDescription, string.Empty, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            ArrayList l = new ArrayList();
            string begining = arguments[0] as string;

            foreach(string fName in functions.Keys)
                if(fName.IndexOf(begining) == 0)
                    l.Add(fName);

            int count = l.Count;
            TArray result = new TArray(typeof(string), count);

            int i = 0;
            foreach(string fName in l)
                result[i++] = fName;

            return result;
		}
	}
}