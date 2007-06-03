using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Parent for all mathematical functions with one parameter and different functions
    /// for double and integer values
    /// </summary>
    public class MathFnID: MathFnD {
        protected override void SetXParam() {
            this.SetParam(0, true, true, false, Messages.PX, Messages.PXDetail, null,
                typeof(double), typeof(PointD), typeof(Vector), typeof(PointVector), typeof(Matrix));
        }

        protected virtual int FnInt(int i, params object[] p) {
            return i;
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            int count = arguments.Count;
            object[] p = new object[count - 1];
            for(int i = 1; i < count; i++)
                p[i - 1] = arguments[i];

            if(item is int)
                return this.FnInt((int)item, p);
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
