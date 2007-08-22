using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Parent for all mathematical functions with one parameter and only one function
    /// for double and integer values
    /// </summary>
    public class FncMathD: Fnc {
        /// <summary>
        /// Set the first parameter which contains value of x
        /// </summary>
        protected virtual void SetXParam() {
            this.SetParam(0, true, true, true, Messages.PX, Messages.PXDetail, null,
                typeof(double), typeof(PointD), typeof(Vector), typeof(PointVector), typeof(Matrix));
        }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetXParam();
        }

        protected virtual double FnDouble(double x, params object[] p) {
            return x;
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            int count = arguments.Count;
            object[] p = new object[count - 1];
            for(int i = 1; i < count; i++)
                p[i - 1] = arguments[i];

            if(item is int)
                return this.FnDouble((int)item, p);
            else if(item is double)
                return this.FnDouble((double)item, p);
            else if(item is PointD)
                return new PointD((item as PointD).X, this.FnDouble((item as PointD).Y, p));
            else if(item is Vector)
                return (item as Vector).Transform(this.FnDouble, p);
            else if(item is PointVector)
                return (item as PointVector).Transform(this.FnDouble, p);
            else if(item is Matrix)
                return (item as Matrix).Transform(this.FnDouble, p);

            return null;
        }
    }
}
