using System;
using System.IO;
using System.Net;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Tries to read text from specified URI
    /// </summary>
    public class TestWWW: FunctionDefinition {
        public override string Help { get { return Messages.HelpTestWWW; } }

        protected override void CreateParameters() {
            this.NumParams(1);
            this.SetParam(0, false, true, false, Messages.PURI, Messages.PURIDescription, "www.seznam.cz", typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            string s = arguments[0] as string;
            UriBuilder ub = new UriBuilder();
            ub.Host = s;
            ub.Scheme = "http";
            
            WebRequest request = WebRequest.Create(ub.Uri);
            WebResponse response = null;
            StreamReader reader = null;
            string result = string.Empty;

            try {
                response = request.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();
            }
            finally {
                if(reader != null)
                    reader.Close();
                if(response != null)
                    response.Close();
            }

            return result;
        }
    }
}
