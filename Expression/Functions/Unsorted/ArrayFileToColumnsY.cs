using System;
using System.IO;
using System.Text;
using System.Collections;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Text file of an array reform is such a way that the items will be in the columns
    /// </summary>
    public class ArrayFileToColumnsY: FncIE {
        public override string Help { get { return Messages.HelpArrayFileToColumnsY; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, true, true, false, Messages.PFileName, Messages.PFileNameDescription, null, typeof(string));
            this.SetParam(1, true, true, false, Messages.PFileName, Messages.PFileNameDescription, null, typeof(string));
            this.SetParam(2, false, true, false, Messages.PNumberXItems, Messages.PNumberXItemsDescription, 1, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int numx = (int)arguments[2];

            FileStream f = new FileStream(arguments[0] as string, FileMode.Open);
            StreamReader tr = new StreamReader(f);

            string type = tr.ReadLine().Trim();
            if(type != typeof(TArray).FullName)
                throw new ExpressionException(Messages.EMArrayInFile, string.Format(Messages.EMArrayInFileDetail, type));

            int rank = int.Parse(tr.ReadLine());

            int[] index = new int[rank];
            string[] sindex = tr.ReadLine().Trim().Split('\t');

            int count = 1;
            for(int i = 0; i < rank; i++) {
                index[i] = int.Parse(sindex[i]);
                count *= index[i];
            }

            string type1 = tr.ReadLine().Trim();

            if(type1 != typeof(Vector).FullName && type1 != typeof(PointVector).FullName)
                throw new ExpressionException(string.Format(Messages.EMTypeInFile, type1));

            ArrayList lines = new ArrayList();
            lines.Add(new StringBuilder());
            int cl = 1;

            for(int i = 0; i < count; i++) {
                int nitems = int.Parse(tr.ReadLine());

                for(int j = 0; j < nitems; j++) {
                    StringBuilder line;
                    string rline = tr.ReadLine();

                    string[] s = rline.Trim().Split('\t'); 

                    int tabs0 = (lines[0] as StringBuilder).ToString().Split('\t').Length;

                    if(i < numx)
                        tabs0 -= s.Length;
                    else
                        tabs0--;

                    if(cl <= j) {
                        line = new StringBuilder();
                        line.Append('\t', tabs0);
                        lines.Add(line);
                        cl++;
                    }
                    else {
                        line = lines[j] as StringBuilder;

                        if(j != 0) {
                            int tabs1 = (lines[j] as StringBuilder).ToString().Split('\t').Length;
                            line.Append('\t', tabs0 - tabs1 + 1);
                        }
                        else if(line.Length != 0)
                            line.Append('\t');
                    }

                    if(i < numx)
                        line.Append(rline);
                    else 
                        line.Append(s[s.Length - 1]);                   
                }
            }

            tr.Close();
            f.Close();

            f = new FileStream(arguments[1] as string, FileMode.Create);
            StreamWriter tw = new StreamWriter(f);

            foreach(StringBuilder l in lines)
                tw.WriteLine(l);

            tw.Close();
            f.Close();

            return null;
        }
    }
}
