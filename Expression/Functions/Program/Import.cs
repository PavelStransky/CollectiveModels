using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Provede import dat
	/// </summary>
	public class FnImport: FncIE {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpImport; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PFileName, Messages.PFileNameDescription, null, typeof(string));
            this.SetParam(1, false, true, false, Messages.PFileTypeImport, Messages.PFileTypeImportDescription, "binary", typeof(string));
            this.SetParam(2, false, true, false, Messages.PLinesOmit, Messages.PLinesOmitDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object result = null;

			string fileName = arguments[0] as string;

            if((string)arguments[1] == paramData) {
                FileStream f = new FileStream(fileName, FileMode.Open);
                StreamReader t = new StreamReader(f);
                try {
                    result = this.ImportData(t);
                }
                finally {
                    t.Close();
                    f.Close();
                }
            }
            else if((string)arguments[1] == paramMatlab) {
                int linesOmit = (int)arguments[2];

                // Import dat z matlabu - matice, nevíme, jaké má rozmìry
                FileStream f = new FileStream(fileName, FileMode.Open);
                StreamReader t = new StreamReader(f);
                try {
                    result = this.ImportMatlab(t, linesOmit);
                }
                finally {
                    t.Close();
                    f.Close();
                }
            }
            else if((string)arguments[1] == paramDigits) {
                int digits = (int)arguments[2];

                // Import èíslic - èíslice jsou v textové formì za sebou;
                // nevíme, kolik jich je, sdružujeme je do skupin podle poètu digits
                FileStream f = new FileStream(fileName, FileMode.Open);
                try {
                    result = this.ImportDigits(f, digits);
                }
                finally {
                    f.Close();
                }
            }
            else if((string)arguments[1] == paramMatica) {
                // Import dat Mathematica - matice
                FileStream f = new FileStream(fileName, FileMode.Open);
                StreamReader t = new StreamReader(f);
                try {
                    result = this.ImportMatica(t);
                }
                finally {
                    t.Close();
                    f.Close();
                }
            }
            else if((string)arguments[1] == paramKML) {
                // Import dat KML - formát a,b,c a,b,c a,b,c...
                FileStream f = new FileStream(fileName, FileMode.Open);
                StreamReader t = new StreamReader(f);
                try {
                    result = this.ImportKML(t);
                }
                finally {
                    t.Close();
                    f.Close();
                }
            }
            else if((string)arguments[1] == paramWave) {
                // Import dat WAV
                FileStream f = new FileStream(fileName, FileMode.Open);
                BinaryReader b = new BinaryReader(f);
                try {
                    result = this.ImportWave(b);
                }
                finally {
                    b.Close();
                    f.Close();
                }
            }
            else {
                Import import = new Import(fileName);
                result = import.Read();
                import.Close();
            }

			return result;
		}

        /// <summary>
        /// Provede import geografických KML dat
        /// </summary>
        /// <param name="t">StreamReader</param>
        private object ImportKML(StreamReader t) {
            string s = t.ReadToEnd().Trim(' ', '\n', '\r', '\t');
            int i1 = s.IndexOf("<coordinates>") + "<coordinates>".Length;
            int i2 = s.IndexOf("</coordinates>");

            string date = string.Empty;
            TimeSpan ts = new TimeSpan();
            Vector[] data;

            // Verze standard
            if(i1 > 0 && i2 > 0) {
                string begin = s.Substring(0, i1);
                string end = s.Substring(i2);
                s = s.Substring(i1, i2 - i1).Trim(' ', '\n', '\r', '\t');

                string[] items = s.Split(' ');

                int length = items.Length;

                data = new Vector[3];
                data[0] = new Vector(length);
                data[1] = new Vector(length);
                data[2] = new Vector(length);

                for(int i = 0; i < length; i++) {
                    string[] its = items[i].Split(',');

                    for(int j = 0; j < its.Length; j++)
                        data[j][i] = double.Parse(its[j]);
                }
            }
            else {
                i1 = s.IndexOf("<when>") + "<when>".Length;
                i2 = s.IndexOf("</when>");

                DateTime start = DateTime.Parse(s.Substring(i1, i2 - i1));

                i1 = s.LastIndexOf("<when>") + "<when>".Length;
                i2 = s.LastIndexOf("</when>");

                DateTime end = DateTime.Parse(s.Substring(i1, i2 - i1));

                ts = end - start;
                date = start.ToString("yyyy-MM-dd");

                int length = 0;
                i1 = 0;
                while((i1 = s.IndexOf("<gx:coord>", i1)) >= 0) {
                    i1 += "<gx:coord>".Length;
                    length++;
                }

                data = new Vector[3];
                data[0] = new Vector(length);
                data[1] = new Vector(length);
                data[2] = new Vector(length);

                i1 = 0;
                int i = 0;
                while((i1 = s.IndexOf("<gx:coord>", i1)) >= 0) {
                    i1 += "<gx:coord>".Length;
                    i2 = s.IndexOf("</gx:coord>", i1);

                    string[] its = s.Substring(i1, i2 - i1).Split(' ');
                    for(int j = 0; j < its.Length; j++)
                        data[j][i] = double.Parse(its[j]);

                    i++;
                }
            }

            List result = new List();
            result.Add(new TArray(data));
            result.Add(date);
            result.Add(ts.TotalMinutes);
            return result;
        }

        private string[] DivideLine(string s) {
            return s.Trim(' ', '\n', '\r', '\t', ',').Replace("  ", " ").Replace(' ', ',').Replace('\t', ',').Split(',');
        }

        /// <summary>
        /// Obecný import dat: data ve sloupcích a øádkách, øádky oddìleny \n, sloupce mezerami, tabulátorem nebo èárkou
        /// </summary>
        private object ImportData(StreamReader t) {
            t.BaseStream.Position = 0;
            
            int numLines = 0;
            int lineLength = 0;

            string s = null;
            while((s = t.ReadLine()) != null) {
                lineLength = System.Math.Max(lineLength, this.DivideLine(s).Length);
                numLines++;
            }

            Matrix result = new Matrix(numLines, lineLength);
            t.BaseStream.Position = 0;

            for(int i = 0; i < numLines; i++) {
                string[] line = this.DivideLine(t.ReadLine());

                for(int j = 0; j < line.Length; j++)
                    result[i, j] = double.Parse(line[j]);
            }
            
            return result;
        }

        /// <summary>
        /// Provede import dat z matiky
        /// </summary>
        /// <param name="t">StreamReader</param>
        private object ImportMatica(StreamReader t) {
            string s = t.ReadToEnd().Trim(' ', '\n', '\r', '\t');
            string[] items = s.Split('\n');

            Matrix result = null;            

            for(int i = 0; i < items.Length; i++) {
                string[] its = items[i].Replace(" ", "").Split('\t');

                if(result == null)
                    result = new Matrix(items.Length, its.Length);

                for(int j = 0; j < its.Length; j++)
                    result[i, j] = double.Parse(its[j]);
            }

            return result;
        }

        /// <summary>
        /// Provede import dat z matlabu
        /// </summary>
        /// <param name="t">StreamReader</param>
        /// <param name="linesOmit">Poèet vynechaných øádek</param>
        private object ImportMatlab(StreamReader t, int linesOmit) {
			ArrayList rows = new ArrayList();

			int columns = -1;

            for(int i = 0; i < linesOmit; i++)
                t.ReadLine();

			string s;
			while((s = t.ReadLine()) != null) {
				string [] items = s.Replace('\t', ' ').Split(' ');
				
				if(columns < 0) {
					columns = 0;
					foreach(string item in items)
						if(item.Trim() != string.Empty)
							columns++;
				}
				
				Vector v = new Vector(columns);
				int index = 0;
				foreach(string item in items) {
					string trim = item.Trim();
					if(trim != string.Empty)
						v[index++] = double.Parse(trim);
				}

				rows.Add(v);
			}

			if(rows.Count == 0)
				return null;
			else if(columns == 1) {
				// Jeden sloupec - vracíme vektor
				Vector result = new Vector(rows.Count);
				for(int i = 0; i < rows.Count; i++)
					result[i] = (rows[i] as Vector)[0];
				return result;
			}
			else if(rows.Count == 1)
				// Jeden øádek - vracíme vektor
				return rows[0];
			else {
				// Jinak matice
				Matrix result = new Matrix(rows.Count, columns);

				for(int i = 0; i < rows.Count; i++) {
					Vector v = rows[i] as Vector;

					for(int j = 0; j < columns; j++)
						result[i, j] = v[j];
				}

				return result;
			}
		}

		/// <summary>
		/// Provede import ze souboru èíslic a èíslice nastrká do vektoru a seskupí podle poètu digits
		/// </summary>
		/// <param name="f">FileStream</param>
        /// <param name="digits">Poèet èíslic ve skupinì</param>        
        private Vector ImportDigits(FileStream f, int digits) {
            ArrayList a = new ArrayList();

            bool finish = false;

            while(!finish) {
                int n = 0;
                for(int i = 0; i < digits; i++) {
                    int d = f.ReadByte() - (int)'0';
                    if(d < 0) {
                        finish = true;
                        break;
                    }
                    if(n == 0)
                        n = d;
                    else {
                        n *= 10;
                        n += d;
                    }
                }
                a.Add(n);
            }

            int count = a.Count;
            Vector result = new Vector(count);
            int j = 0;
            foreach(int i in a)
                result[j++] = i;
            return result;
        }

        /// <summary>
        /// Naète data formátu WAV
        /// </summary>
        /// <remarks>
        /// https://ccrma.stanford.edu/courses/422/projects/WaveFormat/
        /// http://www.sonicspot.com/guide/wavefiles.html
        /// </remarks>
        /// <returns>Øada se všemi kanály (L, P, ...)</returns>
        private TArray ImportWave(BinaryReader b) {
            uint chunkID = b.ReadUInt32();
            if(chunkID != 0x46464952)       // "RIFF"
                throw new FncException(this,
                    string.Format(Messages.EMBadWavFormat, "ChunkID"));

            uint chunkSize = b.ReadUInt32();

            uint format = b.ReadUInt32();
            if(format != 0x45564157)        // "WAVE"
                throw new FncException(this,
                    string.Format(Messages.EMBadWavFormat, "Format"));

            uint subchunk1ID = b.ReadUInt32();
            if(subchunk1ID != 0x20746d66)        // "fmt "
                throw new FncException(this,
                    string.Format(Messages.EMBadWavFormat, "ID1"));

            uint subchunk1Size = b.ReadUInt32();

            uint audioFormat = b.ReadUInt16();
            if(audioFormat != 1)
                throw new FncException(this,
                    string.Format(Messages.EMBadWavAudioFormat, audioFormat));

            uint numChannels = b.ReadUInt16();

            uint sampleRate = b.ReadUInt32();
            uint byteRate = b.ReadUInt32();
            uint blockAlign = b.ReadUInt16();

            uint bitsPerSample = b.ReadUInt16();
            if(bitsPerSample != 8 && bitsPerSample != 16 && bitsPerSample != 32)
                throw new FncException(this,
                    string.Format(Messages.EMBadWavBitRate, bitsPerSample));

            uint subchunk2ID = b.ReadUInt32();
            if(subchunk2ID != 0x61746164)        // "data"
                throw new FncException(this,
                    string.Format(Messages.EMBadWavFormat, "ID2"));

            uint subchunk2Size = b.ReadUInt32();

            double sr = 1.0 / (double)sampleRate;

            PointVector[] data = new PointVector[numChannels];
            uint numSamples = subchunk2Size / numChannels / (bitsPerSample / 8);
            for(int i = 0; i < numChannels; i++)
                data[i] = new PointVector((int)numSamples);

            for(int i = 0; i < numSamples; i++) {
                double x = i * sr;
                for(int j = 0; j < numChannels; j++)
                    data[j][i] = new PointD(x, bitsPerSample == 16 ? b.ReadInt16() :
                        (bitsPerSample == 8 ? b.ReadByte() : b.ReadInt32()));
            }

            return new TArray(data);
        }

        private const string paramData = "data";
        private const string paramDigits = "digits";
		private const string paramMatlab = "matlab";
        private const string paramMatica = "mathmat";
        private const string paramWave = "wav";
        private const string paramKML = "kml";

        private const string name = "import";
	}
}
