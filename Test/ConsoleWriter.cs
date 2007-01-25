using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.Test {
    public class ConsoleWriter : IOutputWriter {
        #region Implementace IOutputWriter
        int indent = 0;

        public void Write(object o) {
            Console.Write(o);
        }

        public void WriteLine(object o) {
            Console.WriteLine(o);
            StringBuilder s = new StringBuilder();
            s.Append(' ', indent);
            Console.Write(s.ToString());
        }

        public void WriteLine() {
            Console.WriteLine();
            StringBuilder s = new StringBuilder();
            s.Append(' ', indent);
            Console.Write(s.ToString());
        }

        public void Clear() {
            Console.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indent"></param>
        /// <returns></returns>
        public int Indent(int indent) {
            this.indent += indent;
            if(indent < 0)
                this.indent = 0;
            return this.indent;
        }

        #endregion
    }
}
