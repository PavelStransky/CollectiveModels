using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Systems {
    /// <summary>
    /// Základní pøedek definující prvek dopravního systému
    /// </summary>
    public abstract class TrafficItem {
        protected static int Rule(int rule, int x1, int x2, int x3) {
            int x = (x1 == 1 ? 4 : 0) | (x2 == 1 ? 2 : 0) | (x3 == 1 ? 1 : 0);
            return (rule >> x) & 1;
        }

        /// <summary>
        /// Krok výpoètu
        /// </summary>
        public abstract void Step();

        /// <summary>
        /// Dokonèení kroku
        /// </summary>
        public abstract void FinalizeStep();

        /// <summary>
        /// Poèet aut
        /// </summary>
        public abstract int CarNumber();

        /// <summary>
        /// Vymaže auta
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Poèet zmìn v pøedchozím kroku
        /// </summary>
        public abstract int Changes { get;}
    }
}