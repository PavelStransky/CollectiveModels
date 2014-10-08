using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates a contour of a given value
    /// </summary>
    public class Contour : Fnc {
        public override string Help { get { return Messages.HelpContour; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, false, true, true, Messages.PValue, Messages.PValueDescription, true, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = (Matrix)arguments[0];
            double v = (double)arguments[1];

            ArrayList points = new ArrayList();

            for(int i = 0; i < m.LengthX; i++) {
                double x = m[i, 0];
                for(int j = 1; j < m.LengthY; j++) {
                    double y = m[i, j];
                    if((x <= v && y > v) || (y <= v && x > v)) 
                        points.Add(new PointD(i, (y - v) / (x - y) + j));
                    x = y;
                }
            }

            for(int i = 0; i < m.LengthY; i++) {
                double x = m[0, i];
                for(int j = 1; j < m.LengthX; j++) {
                    double y = m[j, i];
                    if((x <= v && y > v) || (y <= v && x > v))
                        points.Add(new PointD((y - v) / (x - y) + j, i));
                    x = y;
                }
            }

            // Joining
            ArrayList curves = new ArrayList();
            int numPoints = points.Count;

            while(points.Count > 0) {
                int k = 0;
                PointD p = (PointD)points[0];
                ArrayList curve = new ArrayList();
                double mindist = 0.0;
                do {
                    curve.Add(p);
                    points.RemoveAt(k);

                    mindist = m.LengthX * m.LengthY;
                    k = -1;
                    for(int j = 0; j < points.Count; j++) {
                        double d = PointD.Distance(p, (PointD)points[j]);
                        if(k < 0 || d < mindist) {
                            k = j;
                            mindist = d;
                        }
                    }

                    if(k >= 0) 
                        p = (PointD)points[k];
                    
                } while(mindist <= 2.0);

                k = -1;
                p = (PointD)curve[0];
                mindist = 0.0;
                do {
                    if(k >= 0) {
                        curve.Insert(0, p);
                        points.RemoveAt(k);
                    }

                    mindist = m.LengthX * m.LengthY;
                    k = -1;
                    for(int j = 0; j < points.Count; j++) {
                        double d = PointD.Distance(p, (PointD)points[j]);
                        if(k < 0 || d < mindist) {
                            k = j;
                            mindist = d;
                        }
                    }

                    if(k >= 0)
                        p = (PointD)points[k];

                } while(mindist <= 2.0);

                curves.Add(new PointVector(curve));
            }

            TArray result = new TArray(typeof(PointVector), curves.Count);
            for(int i = 0; i < curves.Count; i++)
                result[i] = curves[i];

            return result;
        }
    }
}
