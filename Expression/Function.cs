using System;
using System.IO;
using System.Text;
using System.Collections;

using PavelStransky.Math;

namespace PavelStransky.Expression
{
	/// <summary>
	/// T��da pro vyhodnocen� funkce
	/// </summary>
	public class Function: Atom, IOutputWriter {
		// Typ oper�toru
		private Functions.FunctionDefinition function;

        private bool logging = defaultLogging;
        private string logFile = defaultLogFile;

        private static int depth = 0;

		// Argumenty funkce
		private ArrayList arguments = new ArrayList();

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="context">Kontext</param>
		/// <param name="expression">V�raz funkce</param>
        /// <param name="parent">Rodi�</param>
        /// <param name="writer">Writer pro textov� v�stupy</param>
        public Function(Context context, string expression, Atom parent, IOutputWriter writer)
            : base(context, expression, parent, writer) { 
			int pos = FindOpenBracketPosition(this.expression);
			this.function = functions[this.expression.Substring(0, pos).Trim().ToLower()];
			string args = this.expression.Substring(pos + 1, this.expression.Length - pos - 2).Trim();

			if(args.Length == 0)
				return;

			string [] a = SplitArguments(args);
			for(int i = 0; i < a.Length; i++) {
				string arg = RemoveOutsideBracket(a[i]).Trim();
				this.arguments.Add(this.CreateAtomObject(arg));
			}
		}

        /// <summary>
        /// Vyp�e do streamu po�et mezer podle aktu�ln� hloubky v�po�tu
        /// </summary>
        /// <param name="t">StreamWriter</param>
        private void WriteWhiteSpace(StreamWriter t) {
            for(int i = 0; i < depth; i++)
                t.Write(whiteSpace);
        }

        /// <summary>
        /// Vyp�e comment mark + aktu�ln� hloubku
        /// </summary>
        /// <param name="t">StreamWriter</param>
        private void WriteCommentMark(StreamWriter t) {
            t.Write(" {0}{1} ", commentMark, depth);
        }

		/// <summary>
		/// Provede v�po�et funkce
		/// </summary>
		/// <returns>V�sledek v�po�tu</returns>
		public override object Evaluate() {
			object result = base.Evaluate();

            DateTime startTime = DateTime.Now;

            // Logov�n� �innosti do souboru
            if(this.logging) {
                FileStream f = new FileStream(this.logFile, FileMode.Append);
                StreamWriter t = new StreamWriter(f);

                if(depth == 0)
                    t.WriteLine();

                this.WriteWhiteSpace(t);
                t.WriteLine(this.expression.Trim());

                this.WriteWhiteSpace(t);
                this.WriteCommentMark(t);
                t.WriteLine("start = {0}", startTime.ToString());

                t.Close();
                f.Close();
            }
 			
			try {
                depth++;
				result = this.function.Evaluate(context, this.arguments, this);
                depth--;
			}
			catch(DetailException e) {
                depth--;
				throw e;
			}
			catch(Exception e) {
                depth--;
				throw new ExpressionException(e.Message, string.Format(errorMessageDetail, this.expression), e);
			}

            TimeSpan duration = DateTime.Now - startTime;

            // Logov�n� �innosti do souboru
            if(this.logging) {
                FileStream f = new FileStream(this.logFile, FileMode.Append);
                StreamWriter t = new StreamWriter(f);

                this.WriteWhiteSpace(t);
                this.WriteCommentMark(t);
                t.Write(resultSign);
                if(result == null)
                    t.WriteLine("null");
                else {
                    string s = result.ToString();
                    if(s.Length > maxLogResultSize) {
                        t.Write(s.Substring(0, maxLogResultSize));
                        t.Write("...");
                    }
                    else
                        t.Write(s);

                    t.WriteLine(" ({0})", result.GetType().FullName);
                }

                this.WriteWhiteSpace(t);
                this.WriteCommentMark(t);
                t.WriteLine(" time = {0}", System.Math.Round(duration.TotalMilliseconds / 1000.0, 2));
                
                t.Close();
                f.Close();
            }

			return result;
		}

        private const string newLine = "\r\n";
        private const bool defaultLogging = false;
        private const string whiteSpace = " ";
        private const string resultSign = " -> ";

        private const int maxLogResultSize = 64;

        private const string defaultLogFile = "c:\\gcm\\default.log";
        private const string errorMessageDetail = "V�raz: {0}";

        #region IOutputWriter Members
        public void Clear() {
            if(this.writer == null)
                return;

            Atom p = this.parent;
            StringBuilder s = new StringBuilder();

            while(p != null) {
                s.Insert(0, p.InfoText);
                p = p.Parent;
            }

            this.infoText = string.Empty;

            this.writer.Clear();
            this.writer.Write(s.ToString());
        }

        public void Write(object o) {
            if(this.writer == null)
                return;

            string s = o.ToString();
            this.infoText += s;
            this.writer.Write(s);
        }

        public void WriteLine() {
            if(this.writer == null)
                return;

            this.infoText += newLine;
            this.writer.WriteLine();
        }

        public void WriteLine(object o) {
            if(this.writer == null)
                return;

            string s = o.ToString();
            this.infoText += s + newLine;
            this.writer.WriteLine(s);
        }
        #endregion
    }
}
