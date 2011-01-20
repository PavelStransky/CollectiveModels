using System;
using System.Collections;
using System.IO;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Saves a variable to a file
	/// </summary>
	public class FnExport: FncIE {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpExport; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PFileName, Messages.PFileNameDescription, null, typeof(string));
            this.SetParam(1, true, true, false, Messages.PExpression, Messages.PExpressionDescription, null);
            this.SetParam(2, false, true, false, Messages.PFileType, Messages.PFileTypeDescription, "binary", typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            string type = (string)arguments[2];
            if(type == "data") {
                string fileName = arguments[0] as string;

                if(arguments[1] is Vector) {
                    Vector v = arguments[1] as Vector;
                    FileStream f = new FileStream(fileName, FileMode.Create);
                    StreamWriter t = new StreamWriter(f);
                    for(int i = 0; i < v.Length; i++)
                        t.WriteLine(v[i]);
                    t.Close();
                    f.Close();
                }
                else if(arguments[1] is Matrix) {
                    Matrix m = arguments[1] as Matrix;
                    FileStream f = new FileStream(fileName, FileMode.Create);
                    StreamWriter t = new StreamWriter(f);

                    for(int i = 0; i < m.LengthX; i++) {
                        t.Write(m[i,0]);
                        for(int j = 1; j < m.LengthY; j++) {
                            t.Write('\t');
                            t.Write(m[i, j]);
                        }
                        t.WriteLine();
                    }
                    t.Close();
                    f.Close();
                }
            }
            else {
                Export export = new Export((string)arguments[0], this.Binary(arguments, 2));
                export.Write(arguments[1]);
                export.Close();
            }

			return null;
		}

        private const string name = "export";
	}
}
