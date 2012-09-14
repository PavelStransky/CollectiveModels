using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates distances (in meters) from GPS coordinates
    /// </summary>
    public class GPSDistance: Fnc {
        public override string Help { get { return Messages.HelpGPS; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, true, true, false, Messages.PX, Messages.PXDetail, null, typeof(Vector));
            this.SetParam(1, true, true, false, Messages.PY, Messages.PYDescription, null, typeof(Vector));
            this.SetParam(2, false, true, false, Messages.PZ, Messages.PZDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector x = arguments[0] as Vector;
            Vector y = arguments[1] as Vector;
            Vector z = arguments[2] as Vector;

            int length = x.Length - 1;
            Vector result = new Vector(length);

            double rad = System.Math.PI / 180.0;

            for(int i = 0; i < length; i++) {
                double dlon = (x[i + 1] - x[i]) * rad;
                double dlat = (y[i + 1] - y[i]) * rad;

                double sdlat = System.Math.Sin(dlat / 2.0); sdlat *= sdlat;
                double sdlon = System.Math.Sin(dlon / 2.0); sdlon *= sdlon;

                double a = sdlat + sdlon * System.Math.Cos(y[i + 1] * rad) * System.Math.Cos(y[i] * rad);
                double c = 2.0 * System.Math.Atan2(System.Math.Sqrt(a), System.Math.Sqrt(1.0 - a));
                double d = 6371000.0 * c;

                if(z != null) {
                    double dz = z[i + 1] - z[i];
                    d = System.Math.Sqrt(dz * dz + d * d);
                }

                result[i] = d;
            }

            return result;
        }
    }
}