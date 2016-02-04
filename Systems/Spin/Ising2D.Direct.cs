using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public partial class Ising2D : IExportable {
        private class Direct : Binder {
            private ComplexVector z;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="sizeX">Number of spins in a row</param>
            /// <param name="sizeY">Number of spin rows</param>
            public Direct(int sizeX, int sizeY)
                : base(sizeX, sizeY) {
                this.z = new ComplexVector(this.num);
            }

            /// <summary>
            /// Initiate the first row
            /// </summary>
            public void FirstRow(Complex x) {
                ComplexVector povp = new ComplexVector(this.maxRow + 1);

                povp[0] = 1.0;
                for(int i = 1; i <= this.maxRow; i++) 
                    povp[i] = povp[i - 1] * x;
                
                for(int i = 0; i < this.num; i++) 
                    this.z[i] = povp[this.row[i]];                
            }

            /// <summary>
            /// Fills the next row
            /// </summary>
            public void FillNext(Complex x) {
                for(int k = 0; k < this.sizeX; k++) {
                    int d = 1 << k;

                    ComplexVector zn = new ComplexVector(this.num);
                    for(int i = 0; i < num; i++) {
                        int i1 = i ^ d;

                        zn[i] = this.z[i] + this.z[i1] * x;

                        if(k > 0) {
                            if(((i & d) >> k) != (i & (d >> 1)) >> (k - 1))
                                zn[i] *= x;
                        }

                    }
                    this.z = zn;
                }
            }
            
            public Complex Finalize(Complex x) {
                Complex result = new Complex();

                for(int i = 0; i < this.num; i++)
                    result += this.z[i];

                for(int i = 0; i < this.maxk / 2; i++)
                    result /= x;

                return result;
            }

            public Complex Compute(Complex x) {
                this.FirstRow(x);
                for(int i = 1; i < this.sizeY; i++)
                    this.FillNext(x);
                return this.Finalize(x);
            }
        }
    }
}