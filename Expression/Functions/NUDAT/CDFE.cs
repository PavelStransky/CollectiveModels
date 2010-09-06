using System;
using System.IO;
using System.Net;
using System.Collections;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Reads all accessible pieces of information about an izotope from http://cdfe.sinp.msu.ru/cgi-bin/muh/radchartnucl.cgi
    /// </summary>
    public class CDFE: Fnc {
        public override string Help { get { return Messages.HelpCDFE; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PProtonNumber, Messages.PProtonNumberDescription, null, typeof(int));
            this.SetParam(1, true, true, false, Messages.PMassNumber, Messages.PMassNumberDescription, null, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int Z = (int)arguments[0];
            int A = (int)arguments[1];

            DateTime startTime = DateTime.Now;

            UriBuilder ub = new UriBuilder();
            ub.Scheme = "http";
            ub.Host = "cdfe.sinp.msu.ru";
            ub.Path = "cgi-bin/muh/radcard.cgi";
            ub.Query = string.Format("z={0}&a={1}&td=123456", Z, A);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(ub.Uri);
            request.UserAgent = "CollectiveModels";

            WebResponse response = null;
            StreamReader reader = null;
            string pageContent = string.Empty;

            List result = new List();

            guider.Write(string.Format(Messages.CDFENucleusPage, Z, A));
            guider.Write("...");

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

            int i1 = pageContent.IndexOf("<body", StringComparison.InvariantCultureIgnoreCase);
            i1 = pageContent.IndexOf(">", i1) + 1;

            int i2 = pageContent.IndexOf("</body", StringComparison.InvariantCultureIgnoreCase);
            pageContent = pageContent.Substring(i1, i2 - i1);

            i1 = pageContent.IndexOf("</sup>", StringComparison.InvariantCultureIgnoreCase) + 6;
            i2 = pageContent.IndexOf("(");

            string element = pageContent.Substring(i1, i2 - i1 - 1).Trim();
            result.Add(element);

            List data = new List();
            i1 = pageContent.IndexOf("Q<SUB>mom</SUB>", StringComparison.InvariantCultureIgnoreCase);
            if(i1 > 0) {
                int c = 0;
                i1 = pageContent.IndexOf("<tr", i1, StringComparison.InvariantCultureIgnoreCase) + 3;
                i2 = pageContent.IndexOf("<tr", i1, StringComparison.InvariantCultureIgnoreCase) + 3;
                do {
                    i1 = pageContent.IndexOf("<td", i1, StringComparison.InvariantCultureIgnoreCase) + 3;
                    c++;
                } while(i2 > i1);

                c -= 2;

                i1 = pageContent.IndexOf("for", i1, StringComparison.InvariantCultureIgnoreCase) + 3;
                i2 = pageContent.IndexOf("</sup", i1);

                string level = pageContent.Substring(i1, i2 - i1);
                level = level.Replace("<sup>", string.Empty).Trim();

                i1 = pageContent.IndexOf("=", i2, StringComparison.InvariantCultureIgnoreCase) + 1;
                i2 = pageContent.IndexOf("MeV", i1, StringComparison.InvariantCultureIgnoreCase);
                double energy = double.Parse(pageContent.Substring(i1, i2 - i1).Trim());

                i1 = pageContent.IndexOf("<td", i1, StringComparison.InvariantCultureIgnoreCase);
                i1 = pageContent.IndexOf(">", i1 + 1) + 1;

                // Quadrupole moment
                for(int i = 0; i < c; i++) {
                    List d = new List();
                    i2 = pageContent.IndexOf("</td", i1, StringComparison.InvariantCultureIgnoreCase);

                    string number = pageContent.Substring(i1, i2 - i1);

                    number = number.Replace("&#177", "#");
                    number = number.Replace("&nbsp", string.Empty);
                    number = number.Replace(";", string.Empty);

                    if(number[0] == '+')
                        d.Add(1);
                    else if(number[0] == '-')
                        d.Add(-1);
                    else
                        d.Add(0);

                    i2 = number.IndexOf("#");
                    if(i2 >= 0) {
                        d.Add(double.Parse(number.Substring(0, i2)));
                        d.Add(double.Parse(number.Substring(i2 + 1, number.Length - i2 - 1)));
                    }
                    else {
                        d.Add(double.Parse(number));
                        d.Add(0.0);
                    }
                    i1 = pageContent.IndexOf("<td", i1, StringComparison.InvariantCultureIgnoreCase);
                    i1 = pageContent.IndexOf(">", i1 + 1) + 1;

                    data.Add(d);
                }

                i1 = pageContent.IndexOf("<td", i1, StringComparison.InvariantCultureIgnoreCase);
                i1 = pageContent.IndexOf(">", i1 + 1) + 1;

                // Deformation parameter
                for(int i = 0; i < c; i++) {
                    i2 = pageContent.IndexOf("</td", i1, StringComparison.InvariantCultureIgnoreCase);

                    string number = pageContent.Substring(i1, i2 - i1);
                    number = number.Replace("<b>", string.Empty);
                    number = number.Replace("</b>", string.Empty);
                    number = number.Replace("&#177", "#");
                    number = number.Replace("&nbsp", string.Empty);
                    number = number.Replace(";", string.Empty);

                    if(number[0] == '+')
                        (data[i] as List).Add(1);
                    else if(number[0] == '-')
                        (data[i] as List).Add(-1);
                    else
                        (data[i] as List).Add(0);

                    i2 = number.IndexOf("#");
                    if(i2 >= 0) {
                        (data[i] as List).Add(double.Parse(number.Substring(0, i2)));
                        (data[i] as List).Add(double.Parse(number.Substring(i2 + 1, number.Length - i2 - 1)));
                    }
                    else {
                        (data[i] as List).Add(double.Parse(number));
                        (data[i] as List).Add(0.0);
                    }

                    i1 = pageContent.IndexOf("<td", i1, StringComparison.InvariantCultureIgnoreCase);
                    i1 = pageContent.IndexOf(">", i1 + 1) + 1;
                }

                i1 = pageContent.IndexOf("<td", i1, StringComparison.InvariantCultureIgnoreCase);
                i1 = pageContent.IndexOf(">", i1 + 1) + 1;

                // NSR reference
                for(int i = 0; i < c; i++) {
                    i1 = pageContent.IndexOf("<a href", i1, StringComparison.InvariantCultureIgnoreCase);
                    i1 = pageContent.IndexOf(">", i1, StringComparison.InvariantCultureIgnoreCase) + 1;
                    i2 = pageContent.IndexOf("</a", i1, StringComparison.InvariantCultureIgnoreCase);

                    string link = pageContent.Substring(i1, i2 - i1);
                    link.Replace("&nbsp;", " ").Trim();
                    (data[i] as List).Add(link);

                    i1 = pageContent.IndexOf("<td", i1, StringComparison.InvariantCultureIgnoreCase);
                    i1 = pageContent.IndexOf(">", i1 + 1) + 1;
                }

                i1 = pageContent.IndexOf("<td", i1, StringComparison.InvariantCultureIgnoreCase);
                i1 = pageContent.IndexOf(">", i1 + 1) + 1;

                // Journal reference
                for(int i = 0; i < c; i++) {
                    i2 = pageContent.IndexOf("</td", i1, StringComparison.InvariantCultureIgnoreCase);

                    string link = pageContent.Substring(i1, i2 - i1);
                    link.Replace("&nbsp;", " ").Trim();
                    (data[i] as List).Add(link);

                    i1 = pageContent.IndexOf("<td", i1, StringComparison.InvariantCultureIgnoreCase);
                    i1 = pageContent.IndexOf(">", i1 + 1) + 1;
                }

                data.Add(level);
                data.Add(energy);
            }
            result.Add(data);

            i1 = pageContent.IndexOf("B(E2)", StringComparison.InvariantCultureIgnoreCase);
            data = new List();
            if(i1 > 0) {
                int c = 0;
                i1 = pageContent.IndexOf("<tr", i1, StringComparison.InvariantCultureIgnoreCase) + 3;
                i2 = pageContent.IndexOf("<tr", i1, StringComparison.InvariantCultureIgnoreCase) + 3;
                do {
                    i1 = pageContent.IndexOf("<td", i1, StringComparison.InvariantCultureIgnoreCase) + 3;
                    c++;
                } while(i2 > i1);

                c -= 2;

                i1 = pageContent.IndexOf("<td", i1, StringComparison.InvariantCultureIgnoreCase);
                i1 = pageContent.IndexOf(">", i1 + 1) + 1;

                // Deformation parameter
                for(int i = 0; i < c; i++) {
                    List d = new List();
                    i2 = pageContent.IndexOf("</td", i1, StringComparison.InvariantCultureIgnoreCase);

                    string number = pageContent.Substring(i1, i2 - i1);

                    number = number.Replace("<b>", string.Empty);
                    number = number.Replace("</b>", string.Empty);
                    number = number.Replace("&#177", "#");
                    number = number.Replace("&nbsp", string.Empty);
                    number = number.Replace(";", string.Empty);

                    if(number[0] == '+')
                        d.Add(1);
                    else if(number[0] == '-')
                        d.Add(-1);
                    else
                        d.Add(0);

                    i2 = number.IndexOf("#");
                    if(i2 >= 0) {
                        d.Add(double.Parse(number.Substring(0, i2)));
                        d.Add(double.Parse(number.Substring(i2 + 1, number.Length - i2 - 1)));
                    }
                    else {
                        d.Add(double.Parse(number));
                        d.Add(0.0);
                    } 

                    i1 = pageContent.IndexOf("<td", i1, StringComparison.InvariantCultureIgnoreCase);
                    i1 = pageContent.IndexOf(">", i1 + 1) + 1;

                    data.Add(d);
                }
            }
            result.Add(data);

            i1 = pageContent.IndexOf("-calc", StringComparison.InvariantCultureIgnoreCase);
            data = new List();
            if(i1 > 0) {
                int c = 0;
                i1 = pageContent.IndexOf("<tr", i1, StringComparison.InvariantCultureIgnoreCase) + 3;
                i2 = pageContent.IndexOf("<tr", i1, StringComparison.InvariantCultureIgnoreCase) + 3;
                do {
                    i1 = pageContent.IndexOf("<td", i1, StringComparison.InvariantCultureIgnoreCase) + 3;
                    c++;
                } while(i2 > i1);

                c -= 2;

                i1 = pageContent.IndexOf("<td", i1, StringComparison.InvariantCultureIgnoreCase);
                i1 = pageContent.IndexOf(">", i1 + 1) + 1;

                // Deformation parameter
                for(int i = 0; i < c; i++) {
                    List d = new List();

                    i2 = pageContent.IndexOf("</td", i1, StringComparison.InvariantCultureIgnoreCase);

                    string number = pageContent.Substring(i1, i2 - i1);

                    number = number.Replace("<b>", string.Empty);
                    number = number.Replace("</b>", string.Empty);
                    number = number.Replace("&#177", "#");
                    number = number.Replace("&nbsp", string.Empty);
                    number = number.Replace(";", string.Empty);

                    if(number[0] == '+')
                        d.Add(1);
                    else if(number[0] == '-')
                        d.Add(-1);
                    else
                        d.Add(0);

                    i2 = number.IndexOf("#");
                    if(i2 >= 0) {
                        d.Add(double.Parse(number.Substring(0, i2)));
                        d.Add(double.Parse(number.Substring(i2 + 1, number.Length - i2 - 1)));
                    }
                    else {
                        d.Add(double.Parse(number));
                        d.Add(0.0);
                    } 

                    i1 = pageContent.IndexOf("<td", i1, StringComparison.InvariantCultureIgnoreCase);
                    i1 = pageContent.IndexOf(">", i1 + 1) + 1;

                    data.Add(d);
                }
            }
            result.Add(data);

            guider.Write(string.Format(Messages.MsgBytes, pageContent.Length));
            guider.Write("...");
            guider.WriteLine(SpecialFormat.Format(DateTime.Now - startTime));

            return result;
        }
    }
}
