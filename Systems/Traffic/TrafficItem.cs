using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Systems {
    /// <summary>
    /// Z�kladn� p�edek definuj�c� prvek dopravn�ho syst�mu
    /// </summary>
    public abstract class TrafficItem {
        protected static int Rule(int rule, int x1, int x2, int x3) {
            int x = (x1 == 1 ? 4 : 0) | (x2 == 1 ? 2 : 0) | (x3 == 1 ? 1 : 0);
            return (rule >> x) & 1;
        }

        /// <summary>
        /// Krok v�po�tu
        /// </summary>
        public abstract void Step();

        /// <summary>
        /// Dokon�en� kroku
        /// </summary>
        public abstract void FinalizeStep();
    }
}