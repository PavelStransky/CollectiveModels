using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Help to one graph parameter
    /// </summary>
    public class GParamHelp: Fnc {
        public override string Help { get { return Messages.HelpGParamHelp; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, false, false, Messages.PParamName, Messages.PParamNameDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            GraphParameterItem gi = null;
            string name = arguments[0] as string;

            StringBuilder result = new StringBuilder();

            foreach(GraphParameterItem g in Graph.GlobalParams.Definitions)
                if(g.Name == name) {
                    result.Append(Messages.GParamHelpGlobalParam);
                    gi = g;
                    break;
                }

            if(gi == null)
                foreach(GraphParameterItem g in Graph.GroupParams.Definitions)
                    if(g.Name == name) {
                        result.Append(Messages.GParamHelpGroupParam);
                        gi = g;
                        break;
                    }

            if(gi == null)
                foreach(GraphParameterItem g in Graph.CurveParams.Definitions)
                    if(g.Name == name) {
                        result.Append(Messages.GParamHelpCurveParam);
                        gi = g;
                        break;
                    }

            if(gi == null) 
                throw new FncException(string.Format(Messages.EMBadParameterName, name));

            result.Append(' ');
            result.Append('\'');
            result.Append(name);
            result.Append("' (");
            result.Append((int)gi.Indication);
            result.Append(')');
            result.Append(Environment.NewLine);

            result.Append(' ');
            result.Append(gi.Description);
            result.Append(Environment.NewLine);

            result.Append(' ');
            result.Append(name);
            result.Append(" = ");
            result.Append(gi.DefaultValue);
            result.Append(" (");
            result.Append(gi.DefaultValue.GetType().Name);
            result.Append(')');

            return result.ToString();
        }
    }
}
