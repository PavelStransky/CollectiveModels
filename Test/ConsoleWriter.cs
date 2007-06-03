using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.Test {
    public class ConsoleWriter : IOutputWriter {
        #region Implementace IOutputWriter
        int indent = 0;

        public string Write(object o) {
            Console.Write(o);

            return o.ToString();
        }

        public string WriteLine(object o) {
            Console.WriteLine(o);
            StringBuilder s = new StringBuilder();
            s.Append(' ', indent);
            Console.Write(s.ToString());

            return string.Format("{0}{1}", o.ToString(), Environment.NewLine);
        }

        public string WriteLine() {
            Console.WriteLine();
            StringBuilder s = new StringBuilder();
            s.Append(' ', indent);
            Console.Write(s.ToString());

            return Environment.NewLine;
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
