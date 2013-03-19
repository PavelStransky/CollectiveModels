using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Smooths a vector
    /// </summary>
    public class Smooth : Fnc {
        public override string Help { get { return Messages.HelpSmooth; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null,
                typeof(Vector), typeof(PointVector), typeof(Matrix));
        } 

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is Vector)
                return (item as Vector).Smooth();
            else if(item is PointVector) 
                return new PointVector((item as PointVector).VectorX, (item as PointVector).VectorY.Smooth());
            else if(item is Matrix)
                return (item as Matrix).Smooth();

            return null;
        }
    }
}
