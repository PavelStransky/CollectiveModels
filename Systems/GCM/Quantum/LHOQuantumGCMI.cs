using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Kvantový GCM, Hamiltonovu matici sestavujeme integrací v x reprezentaci
    /// </summary>
    public abstract class LHOQuantumGCMI: LHOQuantumGCM {
        // Epsilon (pro numerickou integraci)
        protected const double epsilon = 1E-8;

        /// <summary>
        /// Prázdný konstruktor
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
        /// Maximální mez vlnové funkce
        /// </summary>
        protected abstract double MaximalRange { get;}

        /// <summary>
        /// Poèet uzlù vlnové funkce
        /// </summary>
        protected abstract int MaximalNumNodes { get;}

        /// <summary>
        /// Vlnová funkce pro odhad mezí integrace
        /// </summary>
        protected abstract double PsiRange(double range);

        /// <summary>
        /// Zkontroluje, zda je range úplný a pøípadnì doplní
        /// </summary>
        /// <param name="range">Vstupní rozmìry</param>
        /// <returns>Výstupní rozmìry</returns>
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
        /// Vrací parametr range podle dosahu nejvyšší použité vlastní funkce
        /// </summary>
        /// <param name="epsilon">Epsilon</param>
        /// <param name="maxn">Maximální rank vlastní funkce</param>
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
