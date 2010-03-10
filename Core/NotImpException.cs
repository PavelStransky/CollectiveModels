using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Core {
    /// <summary>
    /// V�jimka p�i neimplementovan� metod�
    /// </summary>
    public class NotImpException: NotImplementedException {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="cl">Objekt, ve kter�m do�lo k chyb�</param>
        /// <param name="method">N�zev metody</param>
        public NotImpException(object cl, string method) : 
            base(string.Format(Messages.EMNotImplemented, method, cl.GetType().FullName)) { }
    }
}
