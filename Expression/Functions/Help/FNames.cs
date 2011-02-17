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
            this.SetNumParams(2);
            this.SetParam(0, false, true, false, Messages.PFnName, Messages.PFnNameDescription, string.Empty, typeof(string));
            this.SetParam(1, false, true, false, Messages.PInfo, Messages.PInfoDescription, false, typeof(bool));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            List names = new List();
            List ticks = new List();
            List times = new List();
            List dates = new List();

            string begining = (arguments[0] as string).ToLower();
            bool info = (bool)arguments[1];

            List rnames = null;
            List rticks = null;
            List rdates = null;
            if(info){
                List[] r = Atom.ReadStatistics();
                rnames = r[0];
                rticks = r[1];
                rdates = r[2];
            }

            int count = 0;
            foreach(Fnc fnc in functions.Values)
                if(fnc.Name.ToLower().IndexOf(begining) == 0) {
                    names.Add(fnc.Name);

                    if(info) {
                        int ind = rnames.IndexOf(fnc.Name.ToLower());

                        if(ind >= 0) {
                            long t = (long)rticks[ind] + fnc.TotalTicks;
                            ticks.Add(t);
                            times.Add(TimeSpan.FromTicks(t));
                            if(fnc.TotalTicks > 0)
                                dates.Add(DateTime.Now);
                            else
                                dates.Add(new DateTime((long)rdates[ind]));
                        }
                        else {
                            ticks.Add((long)0);
                            times.Add(TimeSpan.FromTicks(0));
                            dates.Add(new DateTime(0));
                        }

                    }

                    count++;
                }

            if(info) {
                List fullNames = new List();

                for(int i = 0; i < count; i++) {
                    if(((DateTime)dates[i]).Ticks == 0)
                        fullNames.Add(names[i]);
                    else
                        fullNames.Add(string.Format("{0} ({1}, {2})",
                            names[i], (TimeSpan)times[i], (DateTime)dates[i]));
                }

                TArray result = new TArray(typeof(TArray), 5);
                result[0] = fullNames.ToTArray();
                result[1] = names.ToTArray();
                result[2] = ticks.ToTArray();
                result[3] = times.ToTArray();
                result[4] = dates.ToTArray();
                return result;
            }
            else
                return names.ToTArray();
		}
	}
}