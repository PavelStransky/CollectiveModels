using System;
using System.IO;
using System.Text;

namespace PavelStransky.Core {
    /// <summary>
    /// Provede export
    /// </summary>
    public class Export {
        private FileStream f;
        private StreamWriter t;
        private BinaryWriter b;

        private bool binary;

        /// <summary>
        /// StreamWriter
        /// </summary>
        public StreamWriter T { get { return t; } }

        /// <summary>
        /// BinaryWriter
        /// </summary>
        public BinaryWriter B { get { return b; } }

        /// <summary>
        /// Je �ten� bin�rn�?
        /// </summary>
        public bool Binary { get { return this.binary; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Soubor k otev�en�</param>
        public Export(string fileName, bool binary) {
            this.f = new FileStream(fileName, FileMode.Create);

            this.binary = binary;

            if(binary)
                this.b = new BinaryWriter(f);
            else
                this.t = new StreamWriter(f);
        }

        /// <summary>
        /// Zap�e objekt s koment��em
        /// </summary>
        /// <param name="o">Objekt</param>
        /// <param name="name">N�zev objektu</param>
        /// <param name="comment">Koment��</param>
        public void Write(object o, string name, string comment) {
            // Na prvn� ��dku zap�eme typ
            string typeName = (o == null) ? nullString : o.GetType().FullName;

            if(binary) {
                b.Write(typeName);
                b.Write(name);
                b.Write(comment);
            }
            else
                t.WriteLine("{0}\t{1}\t{2}", typeName, this.EncodeString(name), this.EncodeString(comment));
            this.Write(typeName, o);
        }

        /// <summary>
        /// Zap�e objekt
        /// </summary>
        /// <param name="o">Objekt</param>
        public void Write(object o) {
            // Na prvn� ��dku zap�eme typ
            string typeName = (o == null) ? nullString : o.GetType().FullName;

            if(binary) b.Write(typeName); else t.WriteLine(typeName);
            this.Write(typeName, o);
        }

        /// <summary>
        /// Zap�e objekt dan�ho typu
        /// </summary>
        /// <param name="typeName">N�zev typu objektu</param>
        /// <param name="o">Objekt</param>
        public void Write(string typeName, object o) {
            if(o is int) {
                if(binary) b.Write((int)o); else t.WriteLine(o);
            }
            else if(o is double) {
                if(binary) b.Write((double)o); else t.WriteLine(o);
            }
            else if(o is string) {
                if(binary) b.Write((string)o); else t.WriteLine(this.EncodeString((string)o));
            }
            else if(o is bool) {
                if(binary) b.Write((bool)o); else t.WriteLine(o);
            }
            else if(o is DateTime) {
                if(binary) b.Write(((DateTime)o).ToBinary()); else t.WriteLine(((DateTime)o).ToBinary());
            }
            else if(o is TimeSpan) {
                if(binary) b.Write(((TimeSpan)o).Ticks); else t.WriteLine(((TimeSpan)o).Ticks);
            }
            else if(o as IExportable != null) {
                (o as IExportable).Export(this);
            }
            else if(o != null)
                throw new IEException(string.Format(Messages.EMInvalidObjectTypeExport, typeName));
        }

        /// <summary>
        /// Uzav�e writery
        /// </summary>
        public void Close() {
            if(binary)
                this.b.Close();
            else
                this.t.Close();

            this.f.Close();
        }

        /// <summary>
        /// Z �et�zce odstran� znaky nov�ch ��dk� a tabul�tory
        /// </summary>
        /// <param name="s">�et�zec</param>
        public string EncodeString(string s) {
            return s.Replace("\n", "\\%n").Replace("\r", "\\%r").Replace("\t", "\\%t");
        }

        private const string nullString = "null";
    }
}
