using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public partial class Ising2D : IExportable {
        private class Polynomial: Binder {
            private double[,] power;    // Powers
            private int[] next;         // Highest power for each configuration

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="sizeX">Number of spins in a row</param>
            /// <param name="sizeY">Number of spin rows</param>
            public Polynomial(int sizeX, int sizeY) : base(sizeX, sizeY) {
                this.power = new double[this.num, this.maxk];
                this.next = new int[this.num];
            }

            /// <summary>
            /// Initiate the first row
            /// </summary>
            public void FirstRow() {
                for(int i = 0; i < num; i++) {
                    int n = this.row[i];
                    this.power[i, n]++;
                    this.next[i] = n;
                }
            }

            /// <summary>
            /// Fills the next row
            /// </summary>
            public void FillNext() {
                for(int k = 0; k < this.sizeX; k++) {
                    int d = 1 << k;

                    double[,] p = this.power.Clone() as double[,];
                    int[] n = this.next.Clone() as int[];

                    for(int i = 0; i < num; i++) {
                        int i1 = i ^ d;

                        for(int j = 0; j <= this.next[i1]; j++)
                            p[i, j + 1] += this.power[i1, j];
                        n[i] = System.Math.Max(this.next[i], this.next[i1] + 1);

                        if((k > 0) && (((i & d) >> k) != (i & (d >> 1)) >> (k - 1))) {
                            for(int j = n[i]; j >= 0; j--)
                                p[i, j + 1] = p[i, j];
                            p[i, 0] = 0;
                            n[i]++;
                        }

                    }

                    this.power = p;
                    this.next = n;
                }
            }

            /// <summary>
            /// Generates the coefficients of the polynomial
            /// </summary>
            public Vector Finalize() {
                Vector result = new Vector(this.maxk);

                for(int i = 0; i < num; i++)
                    for(int j = 0; j <= this.next[i]; j++)
                        result[j] += this.power[i, j];

                return result;
            }

            /// <summary>
            /// Calculates the coeffcients of the polynomial
            /// </summary>
            /// <param name="writer">Writer</param>
            /// <returns></returns>
            public Vector Compute(IOutputWriter writer) {
                if(writer != null)
                    writer.Write("Polynomial(" + this.maxk + ")");

                DateTime t = DateTime.Now;

                this.FirstRow();

                if(writer != null)
                    writer.Write(".");

                for(int i = 1; i < this.sizeY; i++) {
                    this.FillNext();
                    if(writer != null)
                        writer.Write(".");
                }

                Vector result = this.Finalize();

                if(writer != null) {
                    writer.Write("(" + result.MaxAbs() + ")");
                    writer.WriteLine(DateTime.Now - t);
                }
                return result;
            }
        }
    }
}