using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.GCM {
    /// <summary>
    /// Kvantov� GCM, Hamiltonovu matici sestavujeme integrac� v x reprezentaci
    /// </summary>
    public abstract class LHOQuantumGCMI: LHOQuantumGCM {
        // Epsilon (pro numerickou integraci)
        protected const double epsilon = 1E-8;
    }
}
