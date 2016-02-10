using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates an Ising2D class
    /// </summary>
    public class Ising : Fnc {
        public override string Help { get { return Messages.HelpIsing2D; } }
        
        protected override void CreateParameters() {
            this.SetNumParams(4);
            this.SetParam(0, true, true, false, Messages.PSizeX, Messages.PSizeXDescription, null, typeof(int));
            this.SetParam(1, false, true, false, Messages.PSizeY, Messages.PSizeYDescription, -1, typeof(int));
            this.SetParam(2, false, true, false, Messages.PCyclicBoundary, Messages.PCyclicBoundaryDescription, false, typeof(bool));
            this.SetParam(3, false, true, false, Messages.PIsing2D, Messages.PIsing2DDescription, true, typeof(bool));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int sizeX = (int)arguments[0];
            int sizeY = (int)arguments[1];
            if(sizeY <= 0)
                sizeY = sizeX;

            bool cb = (bool)arguments[2];

            bool isPolynomial = (bool)arguments[3];
            return new Ising2D(sizeX, sizeY, cb, isPolynomial, guider);
        }
    }
}