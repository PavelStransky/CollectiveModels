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
            this.SetNumParams(4);

            this.SetParam(0, true, true, false, Messages.PFileName, Messages.PFileNameDescription, null, typeof(string));
            this.SetParam(1, true, true, false, Messages.PExpression, Messages.PExpressionDescription, null);
            this.SetParam(2, false, true, false, Messages.PFileType, Messages.PFileTypeDescription, "binary", typeof(string));
            this.SetParam(3, false, true, false, Messages.PInfo, Messages.PInfoDescription, new Vector(0), typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            string fileName = (string)arguments[0];
            string type = (string)arguments[2];

            if(type == "data") {
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
            else if(type == "wav") {
                if(arguments[1] is Vector) {
                    FileStream f = new FileStream(fileName, FileMode.Create);
                    BinaryWriter b = new BinaryWriter(f);
                    TArray a = new TArray(typeof(Vector), 1);
                    a[0] = arguments[1] as Vector;
                    ExportWave(b, a, arguments[3] as Vector);
                    b.Close();
                    f.Close();
                }
                else if(arguments[1] is TArray && (arguments[1] as TArray).GetItemType() == typeof(Vector)) {
                    FileStream f = new FileStream(fileName, FileMode.Create);
                    BinaryWriter b = new BinaryWriter(f);
                    ExportWave(b, arguments[1] as TArray, arguments[3] as Vector);
                    b.Close();
                    f.Close();
                }
                else
                    this.BadTypeError(arguments[1], 1);
            }
            else {
                Export export = new Export(fileName, this.IEType(arguments, 2));
                export.Write(arguments[1]);
                export.Close();
            }

			return null;
		}

        /// <summary>
        /// Zap�e data do form�tu WAV
        /// </summary>
        /// <remarks>
        /// https://ccrma.stanford.edu/courses/422/projects/WaveFormat/
        /// http://www.sonicspot.com/guide/wavefiles.html
        /// </remarks>
        /// <param name="data">Jednotliv� kan�ly</param>
        /// <param name="parameters">Parametry (SampleRate = 44100; BitsPerSample = 16)</param>
        public static void ExportWave(BinaryWriter b, TArray data, Vector parameters) {
            uint sampleRate = parameters.Length > 0 ? (uint)parameters[0] : 44100;
            uint bitsPerSample = parameters.Length > 1 ? (uint)parameters[1] : 16;

            uint numChannels = (uint)data.Length;
            uint numSamples = (uint)(data[0] as Vector).Length;
            uint subchunk2Size = numChannels * numSamples * (bitsPerSample / 8);

            b.Write((UInt32)0x46464952);    // "RIFF"
            b.Write((UInt32)(subchunk2Size + 36));
            b.Write((UInt32)0x45564157);    // "WAVE"

            b.Write((UInt32)0x20746d66);    // "fmt "
            b.Write((UInt32)16);
            b.Write((UInt16)1);
            b.Write((UInt16)numChannels);
            b.Write((UInt32)sampleRate);
            b.Write((UInt32)sampleRate * numChannels * (bitsPerSample / 8));
            b.Write((UInt16)(numChannels * (bitsPerSample / 8)));
            b.Write((UInt16)bitsPerSample);

            b.Write((UInt32)0x61746164);    // "data"
            b.Write((UInt32)subchunk2Size);

            for(int i = 0; i < numSamples; i++)
                for(int j = 0; j < numChannels; j++) {
                    if(bitsPerSample == 16)
                    b.Write((Int16)((data[j] as Vector)[i]));
                    else if(bitsPerSample == 8)
                        b.Write((byte)((data[j] as Vector)[i]));
                    else
                        b.Write((Int32)((data[j] as Vector)[i]));
                }
        }

        private const string name = "export";
	}
}
