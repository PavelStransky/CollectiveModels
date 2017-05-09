using System;
using System.Collections;
using System.Text;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Expression;
using PavelStransky.Math;

using MPIR;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// EP of a matrix in the form A + l B
    /// </summary>
    public class EP : Fnc {
        public override string Help { get { return Messages.HelpResultant; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);

            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));

            this.SetParam(2, true, true, false, Messages.PIntervalX, Messages.PIntervalXDescription, null, typeof(Vector));
            this.SetParam(3, true, true, false, Messages.PIntervalY, Messages.PIntervalYDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m1 = arguments[0] as Matrix;
            Matrix m2 = arguments[1] as Matrix;

            Vector intervalx = arguments[2] as Vector;
            Vector intervaly = arguments[3] as Vector;

            int length = m1.Length;

            double minx = intervalx[0];
            double maxx = intervalx[1];
            int numx = (int)intervalx[2];
            double coefx = (maxx - minx) / numx;

            double miny = intervaly[0];
            double maxy = intervaly[1];
            int numy = (int)intervaly[2];
            double coefy = (maxy - miny) / numy;

            PointVector[] result = new PointVector[length];
            for(int i = 0; i < length; i++)
                result[i] = new PointVector(2 * (numx + numy));
            int ir = 0;

            for(int i = 0; i < numx; i++) {
                double kappaRe = minx + i * coefx;
                double kappaIm = miny;
                CMatrix c = new CMatrix(length);
                for(int j = 0; j < length; j++)
                    for(int k = 0; k < length; k++) {
                        c[j, 2 * k] = m1[j, k] + kappaRe * m2[j, k];
                        c[j, 2 * k + 1] = kappaIm * m2[j, k];
                    }
                Vector[] v = c.EigenSystem(false, length, null);
                this.Connect(result, ir++, v);
            }

            for(int i = 0; i < numy; i++) {
                double kappaRe = maxx;
                double kappaIm = miny + i * coefy;
                CMatrix c = new CMatrix(length);
                for(int j = 0; j < length; j++)
                    for(int k = 0; k < length; k++) {
                        c[j, 2 * k] = m1[j, k] + kappaRe * m2[j, k];
                        c[j, 2 * k + 1] = kappaIm * m2[j, k];
                    }
                Vector[] v = c.EigenSystem(false, length, null);
                this.Connect(result, ir++, v);
            }

            for(int i = 0; i < numx; i++) {
                double kappaRe = maxx - i * coefx;
                double kappaIm = maxy;
                CMatrix c = new CMatrix(length);
                for(int j = 0; j < length; j++)
                    for(int k = 0; k < length; k++) {
                        c[j, 2 * k] = m1[j, k] + kappaRe * m2[j, k];
                        c[j, 2 * k + 1] = kappaIm * m2[j, k];
                    }
                Vector[] v = c.EigenSystem(false, length, null);
                this.Connect(result, ir++, v);
            }

            for(int i = 0; i < numy; i++) {
                double kappaRe = minx;
                double kappaIm = maxy - i * coefy;
                CMatrix c = new CMatrix(length);
                for(int j = 0; j < length; j++)
                    for(int k = 0; k < length; k++) {
                        c[j, 2 * k] = m1[j, k] + kappaRe * m2[j, k];
                        c[j, 2 * k + 1] = kappaIm * m2[j, k];
                    }
                Vector[] v = c.EigenSystem(false, length, null);
                this.Connect(result, ir++, v);
            }

            return new TArray(result);
        }


        private void Connect(PointVector[] result, int i, Vector[] v) {
            int length = v.Length;
            if(i >= 0) {
                for(int j = 0; j < length; j++)
                    result[j][i] = new PointD(v[0][j], v[1][j]);
                return;
            }

            bool []ks = new bool[length];

            for(int j = 0; j < length; j++) {
                double distance = double.MaxValue;
                PointD point = new PointD();
                int k0 = -1;

                for(int k = 0; k < length; k++) {                
                    PointD p = new PointD(v[0][k], v[1][k]);
                    double d = PointD.Distance(result[j][i - 1], p);
                    if(d < distance && !ks[k]) {
                        point = p;
                        distance = d;
                        k0 = k;
                    }
                }

                result[j][i] = point;
                ks[k0] = true;
            }
        }
    }
}
