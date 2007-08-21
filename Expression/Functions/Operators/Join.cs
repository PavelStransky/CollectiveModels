using System.Collections;
using System.Text;
using System;

using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Operator join
    /// </summary>
    public class OpJoin: Operator {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpOpJoin; } }
        public override OperatorPriority Priority { get { return OperatorPriority.JoinPriority; } }

        protected override void CreateParameters() {
            this.SetNumParams(1, true);
            this.SetParam(0, true, true, true, Messages.PValue, Messages.PValueDescription, null,
                typeof(double), typeof(Vector), typeof(PointD), typeof(PointVector), typeof(List), typeof(string), typeof(TimeSpan));

            this.AddCompatibility(typeof(double), typeof(Vector));
            this.AddCompatibility(typeof(double), typeof(List));
            this.AddCompatibility(typeof(double), typeof(string));
            this.AddCompatibility(typeof(Vector), typeof(List));
            this.AddCompatibility(typeof(PointD), typeof(PointVector));
            this.AddCompatibility(typeof(PointVector), typeof(List));
            this.AddCompatibility(typeof(List), typeof(string));
            this.AddCompatibility(typeof(string), typeof(TimeSpan));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int count = arguments.Count;
            int[] lengths = this.GetTypesLength(arguments, false);

            if(lengths[8] >= 0) {                           // List
                List result = new List();

                foreach(object o in arguments)
                    if(o is List)
                        result.AddRange(o as List);
                    else
                        result.Add(o);

                return result;
            }

            else if(lengths[10] >= 0) {                     // string
                StringBuilder s = new StringBuilder();

                foreach(object o in arguments)
                    s.Append(o.ToString());

                return s.ToString();
            }

            else if(lengths[0] >= 0 || lengths[2] >= 0) {   // Vektor
                Vector[] v = new Vector[count];
                int i = 0;

                foreach(object o in arguments) {
                    if(o is double) {
                        v[i] = new Vector(1);
                        v[i][0] = (double)o;
                    }
                    else
                        v[i] = (Vector)o;
                    i++;
                }

                return Vector.Join(v);
            }

            else if(lengths[4] >= 0 || lengths[6] >= 0) {   // PointVektor
                PointVector[] pv = new PointVector[count];
                int i = 0;

                foreach(object o in arguments) {
                    if(o is PointD) {
                        pv[i] = new PointVector(1);
                        pv[i][0] = (PointD)o;
                    }
                    else
                        pv[i] = (PointVector)o;
                    i++;
                }

                return PointVector.Join(pv);
            }

            return null;
        }

        private const string name = "~";
    }
}
