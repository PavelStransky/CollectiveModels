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
        PointVector data;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">Time series</param>
        public EMD(PointVector data){
            int length = data.Length;
            this.data = data;
        }

        /// <summary>
        /// Compute
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public PointVector[] ComputeIMF(IOutputWriter writer, int s) {
            PointVector imf = this.Sifting(this.data, s, writer);
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
        public PointVector[] ComputeAll(IOutputWriter writer, int s) {
            int length = this.data.Length;

            PointVector resid = this.data.Clone() as PointVector;
            ArrayList result = new ArrayList();

            PointVector imf = null;
            while((imf = this.Sifting(resid, s, writer)) != null) {
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
        private PointVector Sifting(PointVector source, int s, IOutputWriter writer) {
            if(writer != null)
                writer.Write(string.Format("IMF"));

            int i = 0;
            int iterations = 0;
            SiftingStep sifting = null;

            while(i < s) {
                if(writer != null)
                    writer.Write(".");
                
                sifting = new SiftingStep(source);
                
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
            }

            if(writer != null)
                writer.WriteLine(string.Format(":{0} (Max; Min; ErrorU; ErrorL) = ({1}; {2}; {3}; {4})",
                    iterations, sifting.MaxNum, sifting.MinNum, 
                    sifting.ErrorU / source.Length, sifting.ErrorL / source.Length));

            return sifting.Result;
        }
    }
}