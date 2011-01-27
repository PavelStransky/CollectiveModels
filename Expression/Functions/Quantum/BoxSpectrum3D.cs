using System;
using System.Text;
using System.Collections;

using PavelStransky.Core;
using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Computes spectrum the 3D infinite-well box potential 
    /// E = (hbar^2 pi^2)/(2ML^2)[(nx^2+ny^2)e^2a + nz^2 e^-4a]
    /// with sizes 
    /// x = y = L e^-a
    /// z = L e^2a
    /// </summary>
    public class BoxSpectrum3D: Fnc {
        public override string Help { get { return Messages.HelpBoxSpectrum3D; } }

        protected override void CreateParameters() {
            this.SetNumParams(2, true);

            this.SetParam(0, true, true, true, Messages.PMaxEnergy, Messages.PMaxEnergyDescription, null, typeof(double));
            this.SetParam(1, true, true, true, Messages.PDeformation, Messages.PDeformationDescription, null, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double maxE = (double)arguments[0];
            double deformation = (double)arguments[1];

            int maxxy = (int)(maxE / System.Math.Exp(-2.0 * System.Math.Abs(deformation))) + 1;
            int maxz = (int)(maxE / System.Math.Exp(-4.0 * System.Math.Abs(deformation))) + 1;

            ArrayList result = new ArrayList();

            for(int nx = 0; nx < maxxy; nx++)
                for(int ny = 0; ny < maxxy; ny++)
                    for(int nz = 0; nz < maxz; nz++) {
                        double l = (nx * nx + ny * ny) * System.Math.Exp(2 * deformation) + nz * nz * System.Math.Exp(-4 * deformation);
                        if(l > maxE)
                            break;
                        result.Add(l);
                    }

            int c = result.Count;
            Vector v = new Vector(c);

            c = 0;
            foreach(double l in result)
                v[c++] = l;

            return v.Sort() as Vector;
        }
    }
}