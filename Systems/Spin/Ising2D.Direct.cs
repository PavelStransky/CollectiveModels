using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public partial class Ising2D : IExportable {
        private class Direct : Binder {
            private PointVector z;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="sizeX">Number of spins in a row</param>
            /// <param name="sizeY">Number of spin rows</param>
            public Direct(int sizeX, int sizeY)
                : base(sizeX, sizeY) {
                this.z = new PointVector(this.num);
            }

            /// <summary>
            /// Multiplies two complex numbers
            /// </summary>
            /// <param name="x">X</param>
            /// <param name="y">Y</param>
            private PointD Multiply(PointD x, PointD y) {
                return new PointD(x.X * y.X - x.Y * y.Y, x.Y * y.X + x.X * y.Y);
            }


            /// <summary>
            /// Initiate the first row
            /// </summary>
            public void FirstRow(PointD x) {
                PointVector povp = new PointVector(this.maxRow + 1);

                povp[0] = new PointD(1.0, 0);
                for(int i = 1; i <= this.maxRow; i++) 
                    povp[i] = this.Multiply(povp[i - 1], x);
                
                for(int i = 0; i < this.num; i++) 
                    this.z[i] = povp[this.row[i]];                
            }

            /// <summary>
            /// Fills the next row
            /// </summary>
            public void FillNext(PointD x) {
                for(int k = 0; k < this.sizeX; k++) {
                    int d = 1 << k;

                    PointVector zn = new PointVector(this.num);
                    for(int i = 0; i < num; i++) {
                        int i1 = i ^ d;

                        zn[i] = this.z[i] + this.Multiply(this.z[i1], x);

                        if(k > 0) {
                            if(((i & d) >> k) != (i & (d >> 1)) >> (k - 1))
                                zn[i] = this.Multiply(zn[i], x);
                        }

                    }
                    this.z = zn;
                }
            }
            
            public PointD Finalize(PointD xn) {
                PointD result = new PointD(0.0, 0.0);

                for(int i = 0; i < this.num; i++)
                    result += this.z[i];

                for(int i = 0; i < this.maxk / 2; i++)
                    result = this.Multiply(result, xn);

                return result;
            }

            public PointD Compute(PointD xp, PointD xn) {
                this.FirstRow(xp);
                for(int i = 1; i < this.sizeY; i++)
                    this.FillNext(xp);
                return this.Finalize(xn);
            }
        }
    }
}