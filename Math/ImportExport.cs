using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Provede import zn�m�ch t��d podle typu
    /// </summary>
    public class Import {
        private FileStream f;
        private StreamReader t;
        private BinaryReader b;

        private bool binary;

        /// <summary>
        /// StreamReader
        /// </summary>
        public StreamReader T { get { return t; } }

        /// <summary>
        /// BinaryReader
        /// </summary>
        public BinaryReader B { get { return b; } }

        /// <summary>
        /// Je �ten� bin�rn�?
        /// </summary>
        public bool Binary { get { return this.binary; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Soubor k otev�en�</param>
        /// <param name="binary">True, pokud bude soubor bin�rn�</param>
        public Import(string fileName, bool binary) {
            this.f = new FileStream(fileName, FileMode.Open);

            this.binary = binary;

            if(binary)
                this.b = new BinaryReader(f);
            else
                this.t = new StreamReader(f);
        }

        /// <summary>
        /// P�e�te jeden z�znam a vr�t� jej jako objekt
        /// </summary>
        public object Read() {
            string typeName;

            try {
                if(binary)
                    typeName = b.ReadString();
                else
                    typeName = t.ReadLine();
            }
            catch(EndOfStreamException e) {
                return null;
            }

            return this.Read(typeName);
        }

        /// <summary>
        /// P�e�te objekt s dan�m typem
        /// </summary>
        /// <param name="typeName">Jm�no typu</param>
        public object Read(string typeName) {
            // Vytvo�en� objektu
            object result;
            if(typeName == typeof(int).FullName)
                result = binary ? b.ReadInt32() : Int32.Parse(t.ReadLine());
            else if(typeName == typeof(double).FullName)
                result = binary ? b.ReadDouble() : double.Parse(t.ReadLine());
            else if(typeName == typeof(string).FullName)
                result = binary ? b.ReadString() : t.ReadLine();
            else if(typeName == typeof(DateTime).FullName)
                result = DateTime.FromBinary(binary ? b.ReadInt64() : long.Parse(t.ReadLine()));
            else
                result = this.CreateObject(typeName);

            if(result == null && typeName != nullString)
                throw new ImportExportException(string.Format(errorMessageCannotCreateObject, typeName));

            // Import dat
            IExportable ie = result as IExportable;
            if(ie != null)
                ie.Import(this);

            return result;
        }

        /// <summary>
        /// Vytvo�� objekt
        /// </summary>
        /// <param name="typeName">N�zev typu objektu</param>
        public virtual object CreateObject(string typeName) {
            if(typeName == typeof(Vector).FullName)
                return new Vector();
            else if(typeName == typeof(Matrix).FullName)
                return new Matrix();
            else if(typeName == typeof(PointD).FullName)
                return new PointD();
            else if(typeName == typeof(PointVector).FullName)
                return new PointVector();
            else if(typeName == typeof(ComplexVector).FullName)
                return new ComplexVector();
            else if(typeName == typeof(Jacobi).FullName)
                return new Jacobi();
            else
                return null;
        }

        /// <summary>
        /// Uzav�e readery
        /// </summary>
        public void Close() {
            if(binary)
                this.b.Close();
            else
                this.t.Close();

            this.f.Close();
        }

        private const string errorMessageCannotCreateObject = "Nepoda�ilo se vytvo�it objekt typu {0}. Import se nezda�il.";
        public const string nullString = "null";
    }

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
        /// Zap�e objekt
        /// </summary>
        /// <param name="o">Objekt</param>
        public void Write(object o) {
            // Na prvn� ��dku zap�eme typ
            string typeName = (o == null) ? nullString : o.GetType().FullName;
            if(binary)
                b.Write(typeName);
            else
                t.WriteLine(typeName);

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
                if(binary) b.Write((string)o); else t.WriteLine(o);
            }
            else if(o is DateTime) {
                if(binary) b.Write(((DateTime)o).ToBinary()); else t.WriteLine(((DateTime)o).ToBinary());
            }
            else if(o as IExportable != null) {
                (o as IExportable).Export(this);
            }
            else if(o != null)
                throw new ImportExportException(string.Format(errorMessageBadType, typeName));
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

        public const string errorMessageBadType = "Typ {0} neum�m ulo�it. Ulo�en� se nezda�ilo.";
        public const string nullString = "null";
    }
}
