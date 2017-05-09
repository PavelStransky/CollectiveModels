﻿using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns matrices for the calculation of exceptional points
    /// </summary>
    public class ParameterMatrix : Fnc {
        public override string Help { get { return Messages.HelpParameterMatrix; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PLipkin, Messages.PLipkinDescription, null, typeof(LipkinFull));
            this.SetParam(1, true, true, false, Messages.PMaxEnergy, Messages.PMaxEnergyDescription, null, typeof(Vector), typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            LipkinFull lipkin = arguments[0] as LipkinFull;

            Vector basisParams;
            if(arguments[1] is int) {
                basisParams = new Vector(1);
                basisParams[0] = (int)arguments[1];
            }
            else
                basisParams = arguments[1] as Vector;

            

            return new TArray(lipkin.ParameterMatrix(basisParams));
        }
    }
}