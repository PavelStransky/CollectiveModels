using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Bak-Tang-Wiesenfeld sandpile 2D celular automaton
    /// </summary>
    /// <remarks>
    /// PRL 59, 381 (1987)
    /// </remarks>
    public class BTW: Fnc {
        private Random r = new Random();

        public override string Help { get { return Messages.HelpAvoidedCrossings; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, false, true, false, Messages.PCriticalValue, Messages.PCriticalValueDescription, 4, typeof(int));
            this.SetParam(2, false, true, false, Messages.PStep, Messages.PStepDescription, 1, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = (Matrix)arguments[0];
            int critical = (int)arguments[1];
            int step = (int)arguments[2];

            int lengthX = m.LengthX;
            int lengthY = m.LengthY;

            m = m.Clone() as Matrix;

            for(int i = 0; i < step; i++) {
                m[r.Next(lengthX), r.Next(lengthY)]++;

                bool avalanche = false;
                do {
                    avalanche = false;

                    for(int j = 0; j < lengthX; j++)
                        for(int k = 0; k < lengthY; k++)
                            if(m[j, k] > critical) {
                                avalanche = true;

                                m[j, k] -= 4;

                                if(j > 0) m[j - 1, k]++;
                                if(j < lengthX - 1) m[j + 1, k]++;
                                if(k > 0) m[j, k - 1]++;
                                if(k < lengthY - 1) m[j, k + 1]++;
                            }
                } while(avalanche);
            }

            return m;
        }
    }
}