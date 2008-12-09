using System;
using System.IO;
using System.Collections;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Export three vectors in three columns format: (v, x, y)
    /// </summary>
    public class ExportVector3D: FncIE {
        public override string Help { get { return Messages.HelpExportVector3D; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);

            this.SetParam(0, true, true, false, Messages.PFileName, Messages.PFileNameDescription, null, typeof(string));
            this.SetParam(1, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
            this.SetParam(2, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
            this.SetParam(3, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            FileStream f = null;
            StreamWriter t = null;

            try {
                f = new FileStream(arguments[0] as string, FileMode.Create);
                t = new StreamWriter(f);

                Vector x = arguments[1] as Vector;
                Vector y = arguments[2] as Vector;
                Vector z = arguments[3] as Vector;

                int lengthX = x.Length;
                int lengthY = y.Length;
                int lengthZ = z.Length;

                if(lengthX != lengthY && lengthX != lengthZ)
                    throw new FncException(Messages.EMNotEqualLength,
                        string.Format(Messages.EMNotEqualLengthDetail3, lengthX, lengthY, lengthZ));

                for(int i = 0; i < lengthX; i++)
                    t.WriteLine("{0}\t{1}\t{2}", x[i], y[i], z[i]);
            }
            finally {
                t.Close();
                f.Close();
            }

            return null;
        }
    }
}
