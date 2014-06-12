using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PavelStransky.Core;

namespace PavelStransky.Math {
    /// <summary>
    /// AM-FM decomposition by the Direct quadrature procedure
    /// </summary>
    /// <remarks>Huang et al., On Instantaneous Frequency (Advances in Adaptive Data Analysis, 2009)</remarks>
    public class AMFMDecomposition {
        private PointVector data;
        private ArrayList envelopes;

        private PointVector fm;

        public AMFMDecomposition(PointVector data, bool flat) {
            this.data = data;
            this.envelopes = new ArrayList();

            Vector x = data.VectorX;

            do {
                ExtremesEnvelope e = new ExtremesEnvelope(data.AbsY(), flat);
                Vector am = e.GetValue(x);
                data = data / am;
                this.envelopes.Add(e);
            } while(System.Math.Abs(data.VectorY.MaxAbs()) > 1.0);

            this.fm = data;
        }

        /// <summary>
        /// Envelope of the signal
        /// </summary>
        public PointVector GetAM() {
            Vector result = new Vector(this.data.Length);
            Vector x = this.data.VectorX;

            for(int i = 0; i < result.Length; i++)
                result[i] = 1.0;

            foreach(ExtremesEnvelope e in this.envelopes) 
                result = Vector.ItemMul(result, e.GetValue(x));

            return new PointVector(x, result);
        }

        /// <summary>
        /// Carrier of the signal
        /// </summary>
        public PointVector GetFM() {
            return this.fm;
        }

        /// <summary>
        /// Phase of the signal
        /// </summary>
        public PointVector Phase() {
            PointVector result = new PointVector(this.fm.Length);
            double shift = 0.0;
            int sign = 1;

            double oldy = System.Math.Acos(this.fm[0].Y);
            if(System.Math.Acos(this.fm[1].Y) < oldy) {
                sign = -1;
                shift = System.Math.PI;
                oldy = shift + sign * oldy;
            }

            result[0].X = this.fm[0].X;
            result[0].Y = oldy;
                
            for(int i = 1; i < result.Length; i++) {
                result[i].X = this.fm[i].X;
                double y = shift + sign * System.Math.Acos(this.fm[i].Y);
                if(y < oldy) {
                    if(sign > 0) 
                        shift += 2.0 * System.Math.PI;
                    
                    sign = -sign;
                    y = shift + sign * System.Math.Acos(this.fm[i].Y);
                }
                result[i].Y = y;
                oldy = y;
            }

            return result;
        }

        /// <summary>
        /// Phase of the signal (pure ArcCos of the carrier)
        /// </summary>
        public PointVector PhaseT() {
            PointVector result = new PointVector(this.fm.Length);

            for(int i = 0; i < result.Length; i++) {
                result[i].X = this.fm[i].X;
                result[i].Y = System.Math.Acos(this.fm[i].Y);
            }

            return result;
        }
    }
}
