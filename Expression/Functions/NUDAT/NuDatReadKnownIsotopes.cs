using System;
using System.IO;
using System.Net;
using System.Collections;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Reads all known isotopes from http://www-nds.iaea.org
    /// </summary>
    public class NuDatReadKnownIsotopes: Fnc {
        public override string Help { get { return Messages.HelpNuDatReadKnownIsotopes; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            UriBuilder ub = new UriBuilder();
            ub.Scheme = "http";
            ub.Host = "www-nds.iaea.org";
            ub.Path = "nudat2/index.jsp";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(ub.Uri);
            request.UserAgent = "CollectiveModels";

            WebResponse response = null;
            StreamReader reader = null;
            string pageContent = string.Empty;

            List item = null;
            List all = new List();

            DateTime startTime = DateTime.Now;

            startTime = DateTime.Now;
            guider.Write(Messages.MsgReadingPage);

            try {
                response = request.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                pageContent = reader.ReadToEnd();
            }
            finally {
                if(reader != null)
                    reader.Close();
                if(response != null)
                    response.Close();
            }

            int iMap = pageContent.IndexOf("<map");
            int iArea = iMap;

            guider.Write(Messages.MsgParsing);

            while((iArea = pageContent.IndexOf("<area", iArea + 1)) >= 0) {
                int iJSP = pageContent.IndexOf(".jsp?", iArea);
                int iQuotation = pageContent.IndexOf('"', iJSP);
                string[] sNZ = pageContent.Substring(iJSP + 5, iQuotation - iJSP - 5).Split('=', '&');

                item = new List();
                item.Add(int.Parse(sNZ[1]));
                item.Add(int.Parse(sNZ[3]));

                int iPu = pageContent.IndexOf("pu('", iQuotation);
                iQuotation = pageContent.IndexOf('\'', iPu + 4);

                item.Add(pageContent.Substring(iPu + 4, iQuotation - iPu - 4));

                all.Add(item);
            }

            guider.WriteLine(SpecialFormat.Format(DateTime.Now - startTime));

            return all;
        }
    }
}
