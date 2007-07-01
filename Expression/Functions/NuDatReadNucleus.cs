using System;
using System.IO;
using System.Net;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Reads all accessible pieces of information about a nucleus from http://www-nds.iaea.org
    /// </summary>
    public class NuDatReadNucleus: FunctionDefinition {
        private enum ColumnType {
            ELevelKeV,
            XREF,
            JPi,
            T12,
            EGamma,
            IGamma,
            GammaMult,
            FinalLevel,
            Unknown
        }

        public override string Help { get { return Messages.NuDatReadNucleusHelp; } }

        protected override void CreateParameters() {
            this.NumParams(1);
            this.SetParam(0, true, true, false, Messages.PNucleus, Messages.PNucleusDescription, null, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            string nucleus = arguments[0] as string;
            
            DateTime startTime = DateTime.Now;

            UriBuilder ub = new UriBuilder();
            ub.Scheme = "http";
            ub.Host = "www-nds.iaea.org";
            ub.Path = "nudat2/getdataset.jsp";
            ub.Query = string.Format("nucleus={0}", nucleus);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(ub.Uri);
            request.UserAgent = "CollectiveModels";

            WebResponse response = null;
            StreamReader reader = null;
            string pageContent = string.Empty;

            List result = new List();

            guider.Write(string.Format(Messages.NuDatReadNucleusPage, nucleus));

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

            guider.Write(Messages.MsgParsing);

            int next = 0;

            // Author
            string author;
            int iAuthor = this.ReadString(out author, pageContent, "Author:", next, 11);
            if(iAuthor > 0)
                next = iAuthor;

            // Q(&beta;.)
            string signBeta;
            int iBeta = this.ReadString(out signBeta, pageContent, "Q(&beta", next, 8);
            if(signBeta.Length > 0)
                signBeta = signBeta.Substring(0, 1);

            PointD qBeta;
            iBeta = this.ReadDouble(out qBeta, pageContent, "Q(&beta", next, 11);
            qBeta.X = System.Math.Abs(qBeta.X);
            if(signBeta == "-")
                qBeta.X = -qBeta.X;

            if(iBeta > 0)
                next = iBeta;

            // Sn
            PointD sn;
            int isn = this.ReadDouble(out sn, pageContent, "S<sub>n</sub>", next, 14);
            if(isn > 0)
                next = isn;

            // Sp
            PointD sp;
            int isp = this.ReadDouble(out sp, pageContent, "S<sub>p</sub>", next, 14);
            if(isp > 0)
                next = isp;

            // References
            string sReferences;
            int iReferences = this.ReadString(out sReferences, pageContent, "References", next, 16);
            if(iReferences > 0)
                next = iReferences;

            sReferences = sReferences.Replace("<br>", "<p>");

            List references = new List();
            string ref1;
            int iRef1 = 0;
            char chRef = 'A';

            while((iRef1 = this.ReadString(out ref1, sReferences, string.Format("{0}:", chRef++), iRef1, 2)) >= 0) {
                if(chRef == 'Z')
                    chRef = 'a';
                references.Add(ref1);
            }

            // Table
            int iTable = pageContent.IndexOf("<TABLE", next);
            if(iTable < 0)
                iTable = pageContent.IndexOf("<table", next);

            List levels = new List();

            if(iTable >= 0) {
                int iTrStart = pageContent.IndexOf("<tr", iTable);
                iTrStart = pageContent.IndexOf('>', iTrStart);
                int iTrEnd = pageContent.IndexOf("</tr>", iTrStart);
                string firstRow = pageContent.Substring(iTrStart + 1, iTrEnd - iTrStart - 1);

                // Types of columns
                List columnTypes = new List();

                int iTdStart = 0;
                int iTdEnd = 0;
                while((iTdStart = firstRow.IndexOf("<td", iTdStart)) >= 0) {
                    iTdStart = firstRow.IndexOf('>', iTdStart);
                    iTdEnd = firstRow.IndexOf("</td>", iTdStart);
                    string td = firstRow.Substring(iTdStart + 1, iTdEnd - iTdStart - 1);

                    if(td.IndexOf("E<sub>level</sub>") >= 0)
                        columnTypes.Add(ColumnType.ELevelKeV);
                    else if(td.IndexOf("XREF") >= 0)
                        columnTypes.Add(ColumnType.XREF);
                    else if(td.IndexOf("J&pi") >= 0)
                        columnTypes.Add(ColumnType.JPi);
                    else if(td.IndexOf("T<sub>1/2</sub>") >= 0)
                        columnTypes.Add(ColumnType.T12);
                    else if(td.IndexOf("E<sub>&gamma;</sub>") >= 0)
                        columnTypes.Add(ColumnType.EGamma);
                    else if(td.IndexOf("I<sub>&gamma;</sub>") >= 0)
                        columnTypes.Add(ColumnType.IGamma);
                    else if(td.IndexOf("&gamma;") >= 0 && td.IndexOf("mult.") >= 0)
                        columnTypes.Add(ColumnType.GammaMult);
                    else if(td.IndexOf("Final") >= 0 && td.IndexOf("level") >= 0)
                        columnTypes.Add(ColumnType.FinalLevel);
                    else
                        columnTypes.Add(ColumnType.Unknown);
                }

                int count = columnTypes.Count;

                while((iTrStart = pageContent.IndexOf("<tr", iTrEnd)) >= 0) {
                    iTrStart = pageContent.IndexOf('>', iTrStart);
                    iTrEnd = pageContent.IndexOf("</tr>", iTrStart);
                    if(iTrEnd < 0)
                        iTrEnd = pageContent.IndexOf("<tr", iTrStart);
                    if(iTrEnd < 0)
                        iTrEnd = pageContent.IndexOf("</table>", iTrStart);

                    string tr = pageContent.Substring(iTrStart + 1, iTrEnd - iTrStart - 1);

                    int column = 0;
                    iTdStart = 0;
                    iTdEnd = 0;

                    PointD eLevel = new PointD();
                    List xref = new List();
                    string jpi = string.Empty;
                    List eGamma = new List();
                    List finalLevel = new List();
                    string gammaMult = string.Empty;

                    while((iTdStart = tr.IndexOf("<td", iTdStart)) >= 0) {
                        iTdStart = tr.IndexOf('>', iTdStart);
                        iTdEnd = tr.IndexOf("</td>", iTdStart);
                        if(iTdEnd < 0)
                            iTdEnd = tr.Length - 1;
                        string td = tr.Substring(iTdStart + 1, iTdEnd - iTdStart - 1);
                        td = td.Replace("&nbsp;", string.Empty);
                        td = td.Replace("&asymp;", string.Empty);
                        td = td.Replace("?", string.Empty);
                        td = td.Trim();

                        if(column < count) {
                            switch((ColumnType)columnTypes[column]) {
                                case ColumnType.ELevelKeV:
                                    this.ParseDouble(out eLevel, td);
                                    eLevel.X /= 1000.0; eLevel.Y /= 1000.0;
                                    break;

                                case ColumnType.XREF:
                                    int cref = references.Count;
                                    chRef = 'A';
                                    for(int j = 0; j < cref; j++) {
                                        if(td.IndexOf(chRef++) >= 0)
                                            xref.Add(j);
                                        if(chRef == 'Z')
                                            chRef = 'a';
                                    }
                                    break;

                                case ColumnType.JPi:
                                    jpi = td.Replace("<br>", string.Empty);
                                    break;

                                case ColumnType.EGamma:
                                    string[] seg = td.Replace("<br>", "@").Split('@');
                                    int leg = seg.Length - 1;
                                    for(int j = 0; j < leg; j++) {
                                        PointD eg;
                                        this.ParseDouble(out eg, seg[j]);
                                        eg.X /= 1000.0; eg.Y /= 1000.0;
                                        eGamma.Add(eg);
                                    }
                                    break;

                                case ColumnType.GammaMult:
                                    gammaMult = td.Replace("<br>", string.Empty);
                                    break;

                                case ColumnType.FinalLevel:
                                    string[] sfl = td.Replace("<br>", "@").Split('@');
                                    int lfl = sfl.Length - 1;
                                    for(int j = 0; j < lfl; j++) {
                                        PointD flp;
                                        this.ParseDouble(out flp, sfl[j]);
                                        double fl = flp.X / 1000.0;

                                        int cl = levels.Count;
                                        for(int k = 0; k < cl; k++)
                                            if(((levels[k] as List)[0] as PointD).X == fl)
                                                finalLevel.Add(k);
                                    }
                                    break;
                            }
                        }

                        column++;
                    }

                    List level = new List();
                    level.Add(eLevel);
                    level.Add(xref);
                    level.Add(jpi);
                    level.Add(eGamma);
                    level.Add(gammaMult);
                    level.Add(finalLevel);

                    levels.Add(level);
                }
            }


            result.Add(nucleus);
            result.Add(author);
            result.Add(qBeta);
            result.Add(sn);
            result.Add(sp);
            result.Add(references);
            result.Add(levels);

            guider.WriteLine(SpecialFormat.Format(DateTime.Now - startTime));

            return result;
        }

        private int ReadString(out string s, string text, string key, int start, int add) {
            int i = text.IndexOf(key, start);
            s = string.Empty;

            if(i > 0) {
                int end = text.IndexOf("<p>", i + add);
                s = text.Substring(i + add, end - i - add);
                s = s.Replace("&nbsp;", string.Empty).Trim();
            }

            return i;
        }

        private int ReadDouble(out PointD result, string text, string key, int start, int add) {
            int i = text.IndexOf(key, start);
            result = new PointD();

            if(i > 0) {
                int end = text.IndexOf('<', i + add);
                string v = text.Substring(i + add, end - i - add).ToLower();

                int kev = v.IndexOf("kev");
                int mev = v.IndexOf("mev");

                if(mev >= 0) {
                    v = v.Substring(0, mev).Trim();
                    result.X = double.Parse(v);
                }
                else if(kev >= 0) {
                    v = v.Substring(0, kev).Trim();
                    result.X = double.Parse(v);
                    result.X /= 1000.0;
                }
                else {      // Default - kev
                    v = v.Trim();
                    result.X = double.Parse(v);
                    result.X /= 1000.0;
                }

                int italic = text.IndexOf("<i>", end);
                if(italic == end) {
                    end = text.IndexOf('<', italic + 1);
                    string ve = text.Substring(italic + 3, end - italic - 3).Trim();

                    try {
                        result.Y = double.Parse(ve);
                    }
                    catch { }

                    if(mev < 0)
                        result.Y /= 1000.0;
                }
            }

            return i;
        }

        private bool ParseDouble(out PointD result, string text) {
            result = new PointD();
            bool successful = true;

            int italicStart = text.IndexOf("<i>");
            if(italicStart >= 0) {
                int italicEnd = text.IndexOf("</i>");

                string v = text.Substring(0, italicStart).Trim();
                string ve = text.Substring(italicStart + 3, italicEnd - italicStart - 3);

                try {
                    result.X = double.Parse(v);
                    result.Y = double.Parse(ve);
                }
                catch {
                    successful = false;
                }
            }
            else {
                try {
                    result.X = double.Parse(text.Trim());
                }
                catch {
                    successful = false;
                }
            }

            return successful;
        }
    }
}
