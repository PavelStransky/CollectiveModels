using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.Test {
    public class ConsoleWriter : IOutputWriter {
        #region Implementace IOutputWriter
        public void Write(object o) {
            Console.Write(o);
        }

        public void WriteLine(object o) {
            Console.WriteLine(o);
        }

        public void WriteLine() {
            Console.WriteLine();
        }

        public void Clear() {
            Console.Clear();
        }
        #endregion
    }
}
