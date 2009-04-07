using System;
using System.IO;
using System.Text;
using System.Collections;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Text file of an array reform is such a way that the items will be in one column separated by an empty line
    /// </summary>
    public class ArrayFileToColumn: FncIE {
        public override string Help { get { return Messages.HelpArrayFileToColumn; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PFileName, Messages.PFileNameDescription, null, typeof(string));
            this.SetParam(1, true, true, false, Messages.PFileName, Messages.PFileNameDescription, null, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            FileStream f = new FileStream(arguments[0] as string, FileMode.Open);
            StreamReader tr = new StreamReader(f);

            string type = tr.ReadLine().Trim();
            if(type != typeof(TArray).FullName)
                throw new FncException(this, Messages.EMArrayInFile, string.Format(Messages.EMArrayInFileDetail, type));

            int rank = int.Parse(tr.ReadLine());

            int[] index = new int[rank];
            string []sindex = tr.ReadLine().Trim().Split('\t');

            // Total number of elements in an array
            int count = 1;
            for(int i = 0; i < rank; i++) {
                index[i] = int.Parse(sindex[i]);
                count *= index[i];
            }

            string type1 = tr.ReadLine().Trim();

            if(type1 != typeof(Vector).FullName && type1 != typeof(PointVector).FullName)
                throw new FncException(this, string.Format(Messages.EMTypeInFile, type1));

            FileStream g = new FileStream(arguments[1] as string, FileMode.Create);
            StreamWriter tw = new StreamWriter(g);

            for(int i = 0; i < count; i++) {
                int nitems = int.Parse(tr.ReadLine());

                for(int j = 0; j < nitems; j++) 
                    tw.WriteLine(tr.ReadLine());

                tw.WriteLine();
            }

            tr.Close();
            f.Close();

            tw.Close();
            g.Close();

            return null;
        }
    }
}
