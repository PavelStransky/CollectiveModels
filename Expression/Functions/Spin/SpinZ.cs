using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates the Partition Function of a 2D Spin system of length L
    /// </summary>
    public class SpinZ : Fnc {
        public override string Help { get { return Messages.HelpSpinZ; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, true, true, false, Messages.PIsing2D, Messages.PIsing2DDescription, null, typeof(Ising2D));
            this.SetParam(1, true, true, false, Messages.PIntervalX, Messages.PIntervalXDescription, null, typeof(Vector));
            this.SetParam(2, false, true, false, Messages.PIntervalY, Messages.PIntervalYDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Ising2D ising = arguments[0] as Ising2D;

            Vector intervalX = arguments[1] as Vector;            
            Vector intervalY = arguments[2] == null ? intervalX : arguments[2] as Vector;

            int lengthX = (int)intervalX[2];
            int lengthY = (int)intervalY[2];

            Matrix resultX = new Matrix(lengthX, lengthY);
            Matrix resultY = new Matrix(lengthX, lengthY);

            double x0 = intervalX[0];
            double y0 = intervalY[0];
            double cx = (intervalX[1] - x0) / (lengthX - 1);
            double cy = (intervalY[1] - x0) / (lengthY - 1);

            if(guider != null) {
                guider.Write("Matrix(" + lengthX + "," + lengthY + ")");
            }

            DateTime t = DateTime.Now;

            int l10 = lengthX / 10;
            if(l10 == 0)
                l10 = 1;

            for(int i = 0; i < lengthX; i++) {
                if(guider != null && i % l10 == 0)
                    guider.Write(".");

                for(int j = 0; j < lengthY; j++) {
                    double x = x0 + i * cx;
                    double y = y0 + j * cy;

                    PointD p = ising.GetValue(new PointD(x, y), false);
                    resultX[i, j] = p.X;
                    resultY[i, j] = p.Y;
                }
            }

            if(guider != null)
                guider.WriteLine(DateTime.Now - t);

            TArray result = new TArray(typeof(Matrix), 2);
            result[0] = resultX;
            result[1] = resultY;

            return result;
        }
    }
}