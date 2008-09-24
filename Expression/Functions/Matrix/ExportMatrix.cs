using System;
using System.IO;
using System.Collections;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Export a matrix in three columns format: (x, y, value)
    /// </summary>
    public class ExportMatrix: FncIE {
        public override string Help { get { return Messages.HelpExportMatrix; } }

        protected override void CreateParameters() {
            this.SetNumParams(6);

            this.SetParam(0, true, true, false, Messages.PFileName, Messages.PFileNameDescription, null, typeof(string));
            this.SetParam(1, true, true, true, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(2, false, true, true, Messages.PMinX, Messages.PMinXDescription, double.NaN, typeof(double));
            this.SetParam(3, false, true, true, Messages.PMaxX, Messages.PMaxXDescription, double.NaN, typeof(double));
            this.SetParam(4, false, true, true, Messages.PMinY, Messages.PMinYDescription, double.NaN, typeof(double));
            this.SetParam(5, false, true, true, Messages.PMaxY, Messages.PMaxYDescription, double.NaN, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            FileStream f = new FileStream(arguments[0] as string, FileMode.Create);
            StreamWriter t = new StreamWriter(f);

            Matrix m = arguments[1] as Matrix;
            int lengthX = m.LengthX;
            int lengthY = m.LengthY;

            double minX = (double)arguments[2];
            double maxX = (double)arguments[3];
            double minY = (double)arguments[4];
            double maxY = (double)arguments[5];

            if(double.IsNaN(minX))
                minX = 0;
            if(double.IsNaN(maxX))
                maxX = lengthX - 1;
            if(double.IsNaN(minY))
                minY = 0;
            if(double.IsNaN(maxY))
                maxY = lengthY - 1;

            double koefX = lengthX <= 1 ? 0 : (maxX - minX) / (lengthX - 1);
            double koefY = lengthY <= 1 ? 0 : (maxY - minY) / (lengthY - 1);

            for(int i = 0; i < lengthX; i++)
                for(int j = 0; j < lengthY; j++)
                    t.WriteLine("{0}\t{1}\t{2}", i * koefX + minX, j * koefY + minY, m[i, j]);

            t.Close();
            f.Close();

            return null;
        }
    }
}
