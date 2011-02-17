using System;
using System.Drawing;
using System.IO;
using System.Text;

using Ionic.Zlib;

namespace PavelStransky.Core {
    /// <summary>
    /// Provede import známých tøíd podle typu
    /// </summary>
    public class Import {
        private FileStream f;
        private StreamReader t;
        private BinaryReader b;

        private IETypes ieType;

        // Informace o verzi
        private string versionName = string.Empty;
        private int versionNumber = 0;

        public string VersionName { get { return this.versionName; } }
        public int VersionNumber { get { return this.versionNumber; } }

        public void SetVersionNumber(int versionNumber) {
            this.versionNumber = versionNumber;
        }

        /// <summary>
        /// StreamReader
        /// </summary>
        public StreamReader T { get { return t; } }

        /// <summary>
        /// BinaryReader
        /// </summary>
        public BinaryReader B { get { return b; } }

        /// <summary>
        /// Je ètení binární?
        /// </summary>
        public bool Binary { get { return this.ieType != IETypes.Text; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Soubor k otevøení</param>
        public Import(string fileName) {
            // Ètení hlavièky
            this.f = new FileStream(fileName, FileMode.Open);
            BinaryReader br = new BinaryReader(this.f);
            long position = 0;

            int identifier = br.ReadInt32();
            if(identifier == binaryIdentifier || identifier == compressIdentifier) {
                if(identifier == binaryIdentifier)
                    this.ieType = IETypes.Binary;
                else
                    this.ieType = IETypes.Compressed;

                this.versionNumber = br.ReadInt32();
                this.versionName = br.ReadString();
                position = this.f.Position;
            }
            else {
                this.ieType = IETypes.Text;
                for(int i = 0; i < 512 && br.PeekChar() >= 0; i++)
                    if(br.ReadByte() <= 8) {
                        this.ieType = IETypes.Binary;
                        break;
                    }                    
            }
            br.Close();
            this.f.Close();

            this.f = new FileStream(fileName, FileMode.Open);
            this.f.Seek(position, SeekOrigin.Begin);
            
            switch(this.ieType) {
                case IETypes.Text:
                    this.t = new StreamReader(this.f);
                    break;
                case IETypes.Binary:
                    this.b = new BinaryReader(this.f);
                    break;
                case IETypes.Compressed:
                    this.b = new BinaryReader(new DeflateStream(this.f, CompressionMode.Decompress));
                    break;
            }
        }

        /// <summary>
        /// Pøeète jeden záznam a vrátí jej jako objekt
        /// </summary>
        public object Read() {
            string typeName;

            try {
                if(this.Binary)
                    typeName = b.ReadString();
                else
                    typeName = t.ReadLine();
            }
            catch(EndOfStreamException) {
                return null;
            }

            return this.Read(typeName);
        }

        /// <summary>
        /// Pøeète objekt s daným typem
        /// </summary>
        /// <param name="typeName">Jméno typu</param>
        public object Read(string typeName) {
            // Vytvoøení objektu
            object result;
            if(typeName == typeof(int).FullName)
                result = this.Binary ? b.ReadInt32() : Int32.Parse(t.ReadLine());
            else if(typeName == typeof(long).FullName)
                result = this.Binary ? b.ReadInt64() : Int64.Parse(t.ReadLine());
            else if(typeName == typeof(double).FullName)
                result = this.Binary ? b.ReadDouble() : double.Parse(t.ReadLine());
            else if(typeName == typeof(string).FullName)
                result = this.Binary ? b.ReadString() : this.DecodeString(t.ReadLine());
            else if(typeName == typeof(bool).FullName)
                result = this.Binary ? b.ReadBoolean() : bool.Parse(t.ReadLine());
            else if(typeName == typeof(DateTime).FullName)
                result = DateTime.FromBinary(this.Binary ? b.ReadInt64() : long.Parse(t.ReadLine()));
            else if(typeName == typeof(TimeSpan).FullName)
                result = TimeSpan.FromTicks(this.Binary ? b.ReadInt64() : long.Parse(t.ReadLine()));
            else if(typeName == typeof(Color).FullName)
                result = Color.FromArgb(this.Binary ? b.ReadInt32() : int.Parse(t.ReadLine()));
            else
                result = this.CreateObject(typeName);

            if(result == null && typeName != nullString)
                throw new IEException(string.Format(Messages.EMInvalidObjectTypeImport, typeName));

            return result;
        }

        /// <summary>
        /// Vytvoøí objekt
        /// </summary>
        /// <param name="typeName">Název typu objektu</param>
        public virtual object CreateObject(string typeName) {
            return null;
        }

        /// <summary>
        /// Uzavøe readery
        /// </summary>
        public void Close() {
            if(this.Binary)
                this.b.Close();
            else
                this.t.Close();

            this.f.Close();
        }

        /// <summary>
        /// Zakódovaný uložený øetìzec navrátí do pùvodního tvaru
        /// </summary>
        /// <param name="s">Øetìzec</param>
        public string DecodeString(string s) {
            return s.Replace("\\%n", "\n").Replace("\\%r", "\r").Replace("\\%t", "\t");
        }

        private const int binaryIdentifier = 10011980;
        private const int compressIdentifier = 19800110;

        public const string nullString = "null";
    }
}
