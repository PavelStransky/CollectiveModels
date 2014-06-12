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
        public PointVector[] ComputeIMF(IOutputWriter writer, int s, double delta, bool allIterations) {
            return this.Sifting(this.data, s, delta, allIterations, writer);
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

            PointVector[] imf = null;
            if(writer != null)
                writer.Write(1);
            while((imf = this.Sifting(resid, s, delta, false, writer)).Length > 0) {
                result.Add(imf[0]);
                resid = new PointVector(resid.VectorX, resid.VectorY - imf[0].VectorY);
                if(writer != null)
                    writer.Write(result.Count + 1);
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
        private PointVector[] Sifting(PointVector source, int s, double delta, bool allIterations, IOutputWriter writer) {
            if(writer != null)
                writer.Write(string.Format("IMF"));

            int i = 0;
            int iterations = 0;
            SiftingStep sifting = null;
            ArrayList steps = new ArrayList();

            do {
                if(allIterations)
                    steps.Add(source.Clone());

                sifting = new SiftingStep(source, this.flat, delta);

                if(sifting.IsResiduum) {
                    if(writer != null)
                        writer.WriteLine(string.Format("residuum found (Max; Min; ErrorU; ErrorL) = ({0}; {1}; {2}; {3})",
                            sifting.MaxNum, sifting.MinNum,
                            sifting.ErrorU / source.Length, sifting.ErrorL / source.Length));

                    if(allIterations) {
                        PointVector[] resultR = new PointVector[1];
                        resultR[0] = steps[0] as PointVector;
                        return resultR;
                    }
                    else {
                        PointVector[] resultR = new PointVector[0];
                        return resultR;
                    }
                }

//                if(writer != null && iterations % 50 == 0)
//                    writer.Write(string.Format(" {0},{1},{2}", iterations, sifting.NumAssymetryExtreemes, sifting.SymmetryBreak));
                if(writer != null)
                    writer.Write(string.Format(".{0}", sifting.NumAssymetryExtreemes));

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

            steps.Add(source);
            
            int count = steps.Count;
            PointVector[] result = new PointVector[count];
            for(i = 0; i < count; i++)
                result[i] = steps[i] as PointVector;
            return result;
        }
    }
}