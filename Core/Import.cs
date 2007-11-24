using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace PavelStransky.Core {
    /// <summary>
    /// Provede import známých tøíd podle typu
    /// </summary>
    public class Import {
        private FileStream f;
        private StreamReader t;
        private BinaryReader b;

        private bool binary;

        // Informace o verzi
        private string versionName;
        private int versionNumber;

        public string VersionName { get { return this.versionName; } set { this.versionName = value; } }
        public int VersionNumber { get { return this.versionNumber; } set { this.versionNumber = value; } }

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
        public bool Binary { get { return this.binary; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Soubor k otevøení</param>
        /// <param name="binary">True, pokud bude soubor binární</param>
        public Import(string fileName, bool binary) {
            this.f = new FileStream(fileName, FileMode.Open);

            this.binary = binary;

            if(binary)
                this.b = new BinaryReader(f);
            else
                this.t = new StreamReader(f);
        }

        /// <summary>
        /// Pøeète jeden záznam a vrátí jej jako objekt
        /// </summary>
        public object Read() {
            string typeName;

            try {
                if(binary)
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
                result = binary ? b.ReadInt32() : Int32.Parse(t.ReadLine());
            else if(typeName == typeof(double).FullName)
                result = binary ? b.ReadDouble() : double.Parse(t.ReadLine());
            else if(typeName == typeof(string).FullName)
                result = binary ? b.ReadString() : this.DecodeString(t.ReadLine());
            else if(typeName == typeof(bool).FullName)
                result = binary ? b.ReadBoolean() : bool.Parse(t.ReadLine());
            else if(typeName == typeof(DateTime).FullName)
                result = DateTime.FromBinary(binary ? b.ReadInt64() : long.Parse(t.ReadLine()));
            else if(typeName == typeof(TimeSpan).FullName)
                result = TimeSpan.FromTicks(binary ? b.ReadInt64() : long.Parse(t.ReadLine()));
            else if(typeName == typeof(Color).FullName)
                result = Color.FromArgb(binary ? b.ReadInt32() : int.Parse(t.ReadLine()));
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
            if(binary)
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

        public const string nullString = "null";
    }
}
