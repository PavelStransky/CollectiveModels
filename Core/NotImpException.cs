using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Core {
    /// <summary>
    /// Výjimka pøi neimplementované metodì
    /// </summary>
    public class NotImpException: NotImplementedException {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="cl">Objekt, ve kterém došlo k chybì</param>
        /// <param name="method">Název metody</param>
        public NotImpException(object cl, string method) : 
            base(string.Format(Messages.EMNotImplemented, method, cl.GetType().FullName)) { }
    }
}
