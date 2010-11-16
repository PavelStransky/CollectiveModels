using System;
using System.Text;
using System.Collections;

using PavelStransky.Core;
using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Computes spectrum the infinite-well box potential E=cx*nx^2 + cy*ny^2 + cz*nz^2
    /// for integer coeficients cx, cy, cz
    /// </summary>
    public class BoxSpectrum: Fnc {
        /// <summary>
        /// Class with all coeficients and solution
        /// (to make the recursion faster)
        /// </summary>
        private class Solution {
            private int[] coefs;        // Coeficients of the oscillator
            private int[] maxn;         // Maximum numbers in each dimension
            private int maxE;           // Total maximum energy
            private int dim;            // Number of dimensions

            private Vector result;      // Result

            /// <summary>
            /// Constructor
            /// </summary>
            public Solution(ArrayList arguments) {
                this.maxE = (int)arguments[0];
                this.dim = arguments.Count - 1;

                this.coefs = new int[this.dim];
                this.maxn = new int[this.dim];
                for(int i = 0; i < this.dim; i++) {
                    this.coefs[i] = (int)arguments[i + 1];
                    this.maxn[i] = (int)System.Math.Sqrt(this.maxE / this.coefs[i]);
                }

                this.result = new Vector(this.maxE);
            }

            public int[] MaxN { get { return this.maxn; } }
            public int MaxE { get { return this.maxE; } }
            public int Dim { get { return this.dim; } }
            public Vector Result { get { return this.result; } }

            /// <summary>
            /// Contribution of a given dimension
            /// </summary>
            /// <param name="pointer">Pointer to the dimension</param>
            /// <param name="n">Quantum number</param>
            public int GetValue(int pointer, int n) {
                return this.coefs[pointer] * n * n;
            }

            public override string ToString() {
                StringBuilder s = new StringBuilder();

                s.Append("Cube (");
                s.Append(this.maxE);
                s.Append(") ");

                s.Append(this.coefs[0]);
                for(int i = 1; i < this.dim; i++) {
                    s.Append(":");
                    s.Append(this.coefs[i]);
                }

                s.Append(" (");
                s.Append(this.maxn[0]);
                for(int i = 1; i < this.dim; i++) {
                    s.Append(", ");
                    s.Append(this.maxn[i]);
                }
                s.Append(")");

                return s.ToString();
            }

            /// <summary>
            /// Total number of levels
            /// </summary>
            public double TotalLevels() {
                decimal num = 0;

                for(int i = 0; i < this.MaxE; i++)
                    num += (decimal)this.result[i];

                return (double)num;
            }

            /// <summary>
            /// Calculates sorted degeneracies scaled by e^(1/2)
            /// </summary>
            /// <returns></returns>
            public Vector Degeneracy() {
                Vector degeneracy = new Vector(this.maxE - 1);
                for(int i = 1; i < this.maxE; i++)
                    degeneracy[i - 1] = this.result[i] / System.Math.Sqrt(i);
                return degeneracy.Sort() as Vector;
            }

            /// <summary>
            /// Result + degeneracies
            /// </summary>
            public List GetResult() {
                List list = new List();
                list.Add(this.result);
                list.Add(this.Degeneracy());
                list.Add(this.TotalLevels());
                return list;
            }
        }

        public override string Help { get { return Messages.HelpBoxSpectrum; } }

        protected override void CreateParameters() {
            this.SetNumParams(2, true);

            this.SetParam(0, true, true, false, Messages.PMaxEnergy, Messages.PMaxEnergyDescription, null, typeof(int));
            this.SetParam(1, true, true, false, Messages.P2BoxSpectrum, Messages.P2BoxSpectrumDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Solution s = new Solution(arguments);
            
            if(guider != null)
                guider.WriteLine(s.ToString());

            DateTime startTime = DateTime.Now;
            this.Solve(guider, s, 0, 0);

            if(guider != null) {
                guider.WriteLine();
                guider.WriteLine(string.Format(Messages.MsgLevelNumber, s.TotalLevels()));
                guider.WriteLine(SpecialFormat.Format(DateTime.Now - startTime, true));
            }

            

            return s.GetResult();
        }

        private void Solve(Guider guider, Solution s, int pointer, int result) {
            if(pointer == s.Dim)
                s.Result[result]++;
            else {
                int maxn = s.MaxN[pointer];

                for(int i = 0; i <= maxn; i++) {
                    int value = result + s.GetValue(pointer, i);
                    if(value >= s.MaxE)
                        break;

                    if(pointer == 0 && guider != null) {
                        if(maxn < 100)
                            guider.Write('.');
                        else if(i % (maxn / 100) == 0)
                            guider.Write('.');
                    }
                    
                    this.Solve(guider, s, pointer + 1, value);
                }
            }
        }
    }
}