﻿using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates the microcanonical OTOC
    /// </summary>
    public class OTOC: Fnc {
        public override string Help { get { return Messages.HelpOTOC; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);
            this.SetParam(0, true, true, false, Messages.PQuantumSystem, Messages.PQuantumSystemDescription, null, typeof(QuantumDicke), typeof(JaynesCummings));
            this.SetParam(1, true, true, false, Messages.PInitialState, Messages.PInitialStateDescription, null, typeof(int));
            this.SetParam(2, true, true, false, Messages.PTime, Messages.PTimeDescription, null, typeof(Vector));
            this.SetParam(3, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 1E-4, typeof(double));
        }
                                 
        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int s = (int)arguments[1];
            Vector time = (Vector)arguments[2];
            double precision = (double)arguments[3];

            if(arguments[0] is QuantumDicke)
                return (arguments[0] as QuantumDicke).OTOC(s, time, precision, guider);
            else if(arguments[0] is JaynesCummings)
                return (arguments[0] as JaynesCummings).OTOC(s, time, precision, guider);

            return null;
        }
    }
}