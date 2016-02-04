using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Numerics;

using Ionic.Zlib;

namespace PavelStransky.Core {
    /// <summary>
    /// Provede export
    /// </summary>
    public class Export {
        private IETypes ieType;

        private FileStream f;
        private StreamWriter t;
        private BinaryWriter b;

        private int versionNumber = 0;
        private string versionName = string.Empty;

        private int objectCounter = 0;
        private DateTime startTime;

        // Událost
        public delegate void ExportEventHandler(object sender, ExportEventArgs e);
        public event ExportEventHandler ExportCommand;

        protected void OnExport(ExportEventArgs e) {
            if(this.ExportCommand != null)
                this.ExportCommand(this, e);
        }

        /// <summary>
        /// StreamWriter
        /// </summary>
        public StreamWriter T { get { return t; } }

        /// <summary>
        /// BinaryWriter
        /// </summary>
        public BinaryWriter B { get { return b; } }

        /// <summary>
        /// Je zápis binární?
        /// </summary>
        public bool Binary { get { return this.ieType != IETypes.Text; } }

        /// <summary>
        /// Verze ukládání
        /// </summary>
        public int VersionNumber { get { return this.versionNumber; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Soubor k otevøení</param>
        /// <param name="ieType">Typ ukládání</param>
        /// <param name="versionName">Jméno verze</param>
        /// <param name="versionNumber">Èíslo verze</param>
        public Export(string fileName, IETypes ieType, int versionNumber, string versionName) {
            this.ieType = ieType;
            this.versionName = versionName;
            this.versionNumber = versionNumber;
            this.CreateStream(fileName);
            this.startTime = DateTime.Now;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Soubor k otevøení</param>
        /// <param name="ieType">Typ ukládání</param>
        /// <param name="versionNumber">Èíslo verze</param>
        public Export(string fileName, IETypes ieType, int versionNumber) {
            this.ieType = ieType;
            this.versionNumber = versionNumber;
            this.CreateStream(fileName);
        }
        
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Soubor k otevøení</param>
        /// <param name="ieType">Typ ukládání</param>
        public Export(string fileName, IETypes ieType) {
            this.ieType = ieType;
            this.CreateStream(fileName);
        }

        /// <summary>
        /// Vytvoøí datové proudy
        /// </summary>
        /// <param name="fileName">Soubor k otevøení</param>
        private void CreateStream(string fileName) {
            this.f = new FileStream(fileName, FileMode.Create);

            switch(this.ieType) {
                case IETypes.Text:
                    this.t = new StreamWriter(this.f);
                    if(this.versionNumber > 0 || this.versionName != string.Empty)
                        this.t.WriteLine("{0}\t{1}", this.versionNumber, this.versionName);                    
                    break;
                case IETypes.Binary:
                    this.b = new BinaryWriter(this.f);
                    this.b.Write(binaryIdentifier);
                    this.b.Write(this.versionNumber);
                    this.b.Write(this.versionName);
                    break;
                case IETypes.Compressed:
                    MemoryStream ms = new MemoryStream();
                    BinaryWriter bw = new BinaryWriter(ms);
                    bw.Write(compressIdentifier);
                    bw.Write(this.versionNumber);
                    bw.Write(this.versionName);
                    ms.WriteTo(this.f);
                    bw.Close();
                    ms.Close();
                    this.b = new BinaryWriter(new DeflateStream(this.f, CompressionMode.Compress, CompressionLevel.Default));
                    break;
            }            
        }

        /// <summary>
        /// Zapíše objekt s komentáøem
        /// </summary>
        /// <param name="o">Objekt</param>
        /// <param name="name">Název objektu</param>
        /// <param name="comment">Komentáø</param>
        public void Write(object o, string name, string comment) {
            // Na první øádku zapíšeme typ
            string typeName = (o == null) ? nullString : o.GetType().FullName;

            if(o != null && o.GetType().Namespace.Contains("PavelStransky"))
                this.OnExport(new ExportEventArgs(string.Format("Saving... {0}:{1} {2}", this.objectCounter, typeName, name)));
            this.objectCounter++;

            if(this.Binary) {
                this.b.Write(typeName);
                this.b.Write(name);
                this.b.Write(comment);
            }
            else
                this.t.WriteLine("{0}\t{1}\t{2}", typeName, this.EncodeString(name), this.EncodeString(comment));
            this.Write(typeName, o);
        }

        /// <summary>
        /// Zapíše objekt
        /// </summary>
        /// <param name="o">Objekt</param>
        public void Write(object o) {
            // Na první øádku zapíšeme typ
            string typeName = (o == null) ? nullString : o.GetType().FullName;

            if(o != null && o.GetType().Namespace.Contains("PavelStransky"))
                this.OnExport(new ExportEventArgs(string.Format("Saving... {0}:{1}", this.objectCounter, typeName)));
            this.objectCounter++;

            if(this.Binary) this.b.Write(typeName); else this.t.WriteLine(typeName);
            this.Write(typeName, o);
        }

        /// <summary>
        /// Zapíše objekt daného typu
        /// </summary>
        /// <param name="typeName">Název typu objektu</param>
        /// <param name="o">Objekt</param>
        public void Write(string typeName, object o) {
            if(o is int) {
                if(this.Binary) this.b.Write((int)o); else this.t.WriteLine(o);
            }
            else if(o is long) {
                if(this.Binary) this.b.Write((long)o); else this.t.WriteLine(o);
            }
            else if(o is double) {
                if(this.Binary) this.b.Write((double)o); else this.t.WriteLine(o);
            }
            else if(o is string) {
                if(this.Binary) this.b.Write((string)o); else this.t.WriteLine(this.EncodeString((string)o));
            }
            else if(o is bool) {
                if(this.Binary) this.b.Write((bool)o); else this.t.WriteLine(o);
            }
            else if(o is DateTime) {
                if(this.Binary) this.b.Write(((DateTime)o).ToBinary()); else this.t.WriteLine(((DateTime)o).ToBinary());
            }
            else if(o is TimeSpan) {
                if(this.Binary) this.b.Write(((TimeSpan)o).Ticks); else this.t.WriteLine(((TimeSpan)o).Ticks);
            }
            else if(o is Color) {
                if(this.Binary) this.b.Write(((Color)o).ToArgb()); else this.t.WriteLine(((Color)o).ToArgb());
            }
            else if(o is BigInteger)
                if(this.Binary) this.b.Write(((BigInteger)o).ToString()); else this.t.WriteLine((BigInteger)o);
            else if(o as IExportable != null) {
                (o as IExportable).Export(this);
            }
            else if(o != null)
                throw new IEException(string.Format(Messages.EMInvalidObjectTypeExport, typeName));
        }

        /// <summary>
        /// Uzavøe writery
        /// </summary>
        public void Close() {
            if(this.Binary)
                this.b.Close();
            else
                this.t.Close();

            this.f.Close();

            this.OnExport(new ExportEventArgs(string.Format("{0} objects saved in {1}.",
                this.objectCounter, SpecialFormat.FormatInt(DateTime.Now - this.startTime))));
        }

        /// <summary>
        /// Z øetìzce odstraní znaky nových øádkù a tabulátory
        /// </summary>
        /// <param name="s">Øetìzec</param>
        public string EncodeString(string s) {
            return s.Replace("\n", "\\%n").Replace("\r", "\\%r").Replace("\t", "\\%t");
        }

        private const int binaryIdentifier = 10011980;
        private const int compressIdentifier = 19800110;

        private const string nullString = "null";
    }
}
