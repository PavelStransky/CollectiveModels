using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Separates a set of points into lines
    /// </summary>
    public class Separate : Fnc {
        public override string Help { get { return Messages.HelpSeparate; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector));
        }

        private class Point {
            private double x;
            private ArrayList a;
            private Vector y;

            public Point(double x) {
                this.x = x;
                this.a = new ArrayList();
            }

            public void Add(double y) {
                this.a.Add(y);
            }

            private void Finish() {
                this.a.Sort();
                ArrayList b = new ArrayList();
                double last = double.MinValue;

                foreach(double d in this.a) {
                    if(System.Math.Abs(last - d) > error)
                        b.Add(d);
                    last = d;
                }

                this.a = b;
            }

            public Vector GetVector() {
                this.Finish();

                int length = this.a.Count;
                Vector result = new Vector(length);

                int i = 0;
                foreach(double d in this.a)
                    result[i++] = d;

                return result;
            }

            public double X { get { return this.x; } }
        }

        private class JoinPoints {
            ArrayList work = new ArrayList();
            List finished = new List();

            public JoinPoints() { }

            public void Add(Point p) {
                Vector v = p.GetVector();

                if(v.Length == work.Count) {
                    for(int i = 0; i < v.Length; i++)
                        (this.work[i] as ArrayList).Add(new PointD(p.X, v[i]));
                }
                else if(this.work.Count == 0) {
                    for(int i = 0; i < v.Length; i++) {
                        this.work.Add(new ArrayList());
                        (this.work[i] as ArrayList).Add(new PointD(p.X, v[i]));
                    }
                }
                else {      // This is the worst part - not an elegant solution, but what
                    foreach(ArrayList a in work) {
                        int length = a.Count;
                        PointVector pv = new PointVector(length);
                        int i = 0;
                        foreach(PointD pd in a)
                            pv[i++] = pd;
                        finished.Add(pv);
                    }

                    this.work = new ArrayList();

                    for(int i = 0; i < v.Length; i++) {
                        this.work.Add(new ArrayList());
                        (this.work[i] as ArrayList).Add(new PointD(p.X, v[i]));
                    }
                }
            }

            public List Finish() {
                foreach(ArrayList a in work) {
                    int length = a.Count;
                    PointVector pv = new PointVector(length);
                    int i = 0;
                    foreach(PointD pd in a)
                        pv[i++] = pd;
                    finished.Add(pv);
                }

                return this.finished;
            }
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PointVector source = arguments[0] as PointVector;
            int length = source.Length;

            source = source.SortX();

            ArrayList points = new ArrayList();
            Point point = new Point(0);
            double oldX = double.MinValue;

            for(int i = 0; i < length; i++) {
                if(System.Math.Abs(source[i].X - oldX) > error) {
                    points.Add(point);
                    oldX = source[i].X;
                    point = new Point(oldX);
                }
                point.Add(source[i].Y);
            }
            points.RemoveAt(0);

            JoinPoints j = new JoinPoints();
            foreach(Point p in points)
                j.Add(p);
            
            return j.Finish();
        }

        private const double error = 1e-6;
    }
}
