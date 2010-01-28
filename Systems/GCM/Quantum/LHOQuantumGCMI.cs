using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Kvantov� GCM, Hamiltonovu matici sestavujeme integrac� v x reprezentaci
    /// </summary>
    public abstract class LHOQuantumGCMI: LHOQuantumGCM {
        // Epsilon (pro numerickou integraci)
        protected const double epsilon = 1E-8;

        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        protected LHOQuantumGCMI() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        /// <param name="hbar">Planckova konstanta</param>
        public LHOQuantumGCMI(double a, double b, double c, double k, double a0, double hbar)
            : base(a, b, c, k, a0, hbar) {
        }

        /// <summary>
        /// Konstruktor pro IE
        /// </summary>
        /// <param name="import"></param>
        public LHOQuantumGCMI(Core.Import import) : base(import) { }

        /// <summary>
        /// Maxim�ln� mez vlnov� funkce
        /// </summary>
        protected abstract double MaximalRange { get;}

        /// <summary>
        /// Po�et uzl� vlnov� funkce
        /// </summary>
        protected abstract int MaximalNumNodes { get;}

        /// <summary>
        /// Vlnov� funkce pro odhad mez� integrace
        /// </summary>
        protected abstract double PsiRange(double range);

        /// <summary>
        /// Zkontroluje, zda je range �pln� a p��padn� dopln�
        /// </summary>
        /// <param name="range">Vstupn� rozm�ry</param>
        /// <returns>V�stupn� rozm�ry</returns>
        protected DiscreteInterval ParseRange(Vector range) {
            if((object)range == null || range.Length == 0)
                return new DiscreteInterval(this.GetRange(), 10 * this.MaximalNumNodes + 1);

            if(range.Length == 1)
                return new DiscreteInterval(this.GetRange(), (int)range[0]);

            if(range.Length == 2)
                return new DiscreteInterval(range[0], (int)range[1]);

            return new DiscreteInterval(range);
        }

        /// <summary>
        /// Vrac� parametr range podle dosahu nejvy��� pou�it� vlastn� funkce
        /// </summary>
        /// <param name="epsilon">Epsilon</param>
        /// <param name="maxn">Maxim�ln� rank vlastn� funkce</param>
        protected double GetRange() {
            // range je klasicky dosah oscilatoru, pridame urcitou rezervu
            double range = 5.0 * this.MaximalRange;

            // dx musi byt nekolikrat mensi, nez vzdalenost mezi sousednimi nody
            double dx = range / (50.0 * this.MaximalNumNodes);

            while(System.Math.Abs(this.PsiRange(range)) < epsilon)
                range -= dx;

            //jedno dx, abysme se dostali tam, co to bylo male a druhe jako rezerva
            return range + 2 * dx;
        }
    }
}
