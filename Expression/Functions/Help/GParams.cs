using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// List of all graph parameters
    /// </summary>
    public class GParams: Fnc {
        public override string Help { get { return Messages.HelpGParams; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int globalc = Graph.GlobalParams.Count;
            int groupc = Graph.GroupParams.Count;
            int curvec = Graph.CurveParams.Count;

            TArray result = new TArray(typeof(string), globalc + groupc + curvec);

            int i = 0;
            foreach(GraphParameterItem g in Graph.GlobalParams.Definitions)
                result[i++] = g.Name;

            foreach(GraphParameterItem g in Graph.GroupParams.Definitions)
                result[i++] = g.Name;

            foreach(GraphParameterItem g in Graph.CurveParams.Definitions)
                result[i++] = g.Name;

            return result;
        }
    }
}
