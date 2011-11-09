using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Math {
    /// <summary>
    /// Empirical Mode Decomposition
    /// </summary>
    /// <remarks>Irving y Emmanuel, marzo 2011</remarks>
    public partial class EMD {
        private PointVector data;
        private bool flat;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">Time series</param>
        /// <param name="flat">True if the flat parts of the level density is going to be considered 
        /// as a source of maxima / minima</param>
        public EMD(PointVector data, bool flat) {
            this.data = data;
            this.flat = flat;
        }

        /// <summary>
        /// Compute
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="s">Number of interations after the condition |#max-#min| leq 1 is satisfied</param>
        /// <param name="delta">A special parameter for the symmetry condition |U+L|/2|U| leq delta</param>
        public PointVector[] ComputeIMF(IOutputWriter writer, int s, double delta) {
            PointVector imf = this.Sifting(this.data, s, delta, writer);
            if(imf != null) {
                PointVector[] result = new PointVector[2];
                result[0] = imf;
                result[1] = new PointVector(this.data.VectorX, this.data.VectorY - imf.VectorY);
                return result;
            }
            else {
                PointVector[] result = new PointVector[1];
                result[0] = this.data.Clone() as PointVector;
                return result;
            }
        }
      
        /// <summary>
        /// Computes all IMF
        /// </summary>
        /// <param name="s">Number of iterations after the condition (MaxNum + MinNum - Cross0) is reached</param>
        /// <param name="delta">A special parameter for the symmetry condition |U+L|/|U,L| leq delta</param>
        /// <param name="boundary">Boundary condition</param>
        public PointVector[] ComputeAll(IOutputWriter writer, int s, double delta) {
            int length = this.data.Length;

            PointVector resid = this.data.Clone() as PointVector;
            ArrayList result = new ArrayList();

            PointVector imf = null;
            while((imf = this.Sifting(resid, s, delta, writer)) != null) {
                result.Add(imf);
                resid = new PointVector(resid.VectorX, resid.VectorY - imf.VectorY);
            }
            result.Add(resid);

            int count = result.Count;
            PointVector[] resultPV = new PointVector[count];
            for(int i = 0; i < count; i++)
                resultPV[i] = result[i] as PointVector;
            return resultPV;
        }

        /// <summary>
        /// Sifting iterative process for 1 IMF
        /// </summary>
        /// <param name="source">Source data</param>
        /// <param name="s">Number of iterations after the condition (MaxNum + MinNum - Cross0) is reached</param>
        /// <param name="delta">A special parameter for the symmetry condition |U+L|/|U,L| leq delta</param>
        private PointVector Sifting(PointVector source, int s, double delta, IOutputWriter writer) {
            if(writer != null)
                writer.Write(string.Format("IMF"));

            int i = 0;
            int iterations = 0;
            SiftingStep sifting = null;

            do {
                if(writer != null)
                    writer.Write(".");

                sifting = new SiftingStep(source, this.flat, delta);

                if(sifting.IsResiduum) {
                    if(writer != null)
                        writer.WriteLine(string.Format("residuum found (Max; Min; ErrorU; ErrorL) = ({0}; {1}; {2}; {3})",
                            sifting.MaxNum, sifting.MinNum,
                            sifting.ErrorU / source.Length, sifting.ErrorL / source.Length));
                    return null;
                }

                if(writer != null)
                    writer.Write(sifting.NumAssymetryExtreemes);

                if(sifting.NumAssymetryExtreemes == 0)
                    i++;
                else
                    i = 0;

                source = sifting.Result;
                iterations++;
            } while(i <= s || sifting.SymmetryBreak > 0);

            if(writer != null)
                writer.WriteLine(string.Format(":{0} (Max; Min; ErrorU; ErrorL) = ({1}; {2}; {3}; {4})",
                    iterations, sifting.MaxNum, sifting.MinNum, 
                    sifting.ErrorU / source.Length, sifting.ErrorL / source.Length));

            return sifting.Result;
        }
    }
}