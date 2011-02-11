using System;
using System.IO;
using System.Net;
using System.Collections;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.DLLWrapper;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Generates randomly a symmetric sparse matrix and diagonalizes it
    /// </summary>
    public class TestARPack: Fnc {
        public override string Help { get { return Messages.HelpTestARPack; } }

        private Random random = new Random();

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, true, true, false, Messages.PLength, Messages.PLengthDescription, null, typeof(int));
            this.SetParam(1, true, true, false, Messages.PNonZeros, Messages.PNonZerosDescription, null, typeof(int));
            this.SetParam(2, true, true, false, Messages.PNumEV, Messages.PNumEVDescription, null, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int length = (int)arguments[0];
            int nz = (int)arguments[1];
            int numev = (int)arguments[2];

            if(guider != null)
                guider.Write(string.Format("Filling a symmetric sparse matrix {0} x {0} with {1} elements...", length, nz));

            DateTime startTime = DateTime.Now;

            SparseMatrix sm = new SparseMatrix(length);
            for(int i = 0; i < nz; i++) {
                int x = this.random.Next(length);
                int y = this.random.Next(length);
                double v = 10.0 * this.random.NextDouble() - 5.0;
                sm[x, y] = v;
                sm[y, x] = v;
            }

            if(guider != null) {
                guider.WriteLine(SpecialFormat.Format(DateTime.Now - startTime));
                guider.Write(string.Format("Diagonalizing matrix ({0} eigenvalues)...", numev));
            }

            startTime = DateTime.Now;
            Vector result = ARPackDLL.dsaupd(sm, numev, false, false)[0];

            if(guider != null) {
                guider.WriteLine(SpecialFormat.Format(DateTime.Now - startTime));
            }

            return result;
        }
    }
}
