using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public partial class Ising2D : IExportable {
        private class Binder {
            protected int maxk;           // Maximal number of powers in the polynomial
            protected int sizeX, sizeY;   // Size of the lattice
            protected int num;            // Number of configurations in each row
            protected int[] row;          // Number of different neighbours in a row
            protected int maxRow;         // Maximum power in the row

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="sizeX">Lattice size</param>
            /// <param name="sizeY">Lattice size</param>
            public Binder(int sizeX, int sizeY) {
                this.sizeX = sizeX;
                this.sizeY = sizeY;
                this.num = 1 << this.sizeX;
                this.maxk = 2 * this.sizeX * this.sizeY - this.sizeX - this.sizeY + 1;
                this.Initialize();
            }

            /// <summary>
            /// Initiate the row array
            /// </summary>
            protected void Initialize() {
                this.row = new int[this.num];
                this.maxRow = 0;
                for(int i = 0; i < num; i++) {
                    this.row[i] = this.Neighbours(i);
                    this.maxRow = System.Math.Max(this.maxRow, this.row[i]);
                }
            }

            /// <summary>
            /// Number of different neigbours
            /// </summary>
            /// <param name="d">Number</param>
            protected int Neighbours(int d) {
                int x0 = d & 1;             // First bit (for periodic boundary conditions)
                int x = x0;                 // Actual bit

                int result = 0;

                for(int i = 1; i < this.sizeX; i++) {
                    d >>= 1;                // Move to the next bit
                    int x1 = d & 1;         
                    if(x != x1)
                        result++;
                    x = x1;
                }

                /* For periodic boundary conditions
                if(x != x0)
                    result++;
                */

                return result;
            }
        }
    }
}