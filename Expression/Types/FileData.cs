using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Expression {
    /// <summary>
    /// Store informations and content of the file
    /// </summary>
    public class FileData: IExportable {
        // True for binary file
        private bool binary = true;

        // If the file is binary, this stores the data
        private byte[] data;

        // If the file is text, this contains all lines
        private TArray lines;

        /// <summary>
        /// True for binary file
        /// </summary>
        public bool Binary { get { return this.binary; } }

        /// <summary>
        /// Lines of the text file arranged in the array
        /// </summary>
        public TArray Lines { get { return this.lines; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="binary">True for binary file</param>
        public FileData(bool binary) {
            this.binary = binary;
        }

        /// <summary>
        /// Reads the whole file
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        public void Read(string fileName) {
            if(fileName == null || fileName == string.Empty)
                throw new ExpressionException(Messages.EMNoFile);

            FileInfo fInfo = new FileInfo(fileName);
            if(!fInfo.Exists)
                throw new ExpressionException(string.Format(Messages.EMFileNotExist, fileName));

            FileStream f = new FileStream(fileName, FileMode.Open);

            if(this.binary) {
                int length = (int)f.Length;

                BinaryReader b = new BinaryReader(f);
                this.data = b.ReadBytes(length);
                b.Close();

                this.lines = null;
            }
            else {
                List l = new List();

                StreamReader s = new StreamReader(f);
                string line = string.Empty;
                while((line = s.ReadLine()) != null)
                    l.Add(line);

                this.lines = l.ToTArray();
                this.data = null;
            }
        }

        /// <summary>
        /// Write data to a file
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        public void Write(string fileName) {
            if(fileName == null || fileName == string.Empty)
                throw new ExpressionException(Messages.EMNoFile);

            FileStream f = new FileStream(fileName, FileMode.Create);

            if(this.binary) {
                BinaryWriter b = new BinaryWriter(f);
                b.Write(this.binary);
                b.Close();
            }
            else {
                StreamWriter s = new StreamWriter(f);
                foreach(string line in this.lines)
                    s.WriteLine(line);
                s.Close();
            }
        }

        #region IExportable Members
        /// <summary>
        /// Export of the storage
        /// </summary>
        /// <param name="export">Export object</param>
        public void Export(Export export) {
            if(export.Binary) {
                BinaryWriter b = export.B;
                b.Write(this.binary);

                if(this.binary) {
                    if(this.data == null)
                        b.Write(-1);
                    else {
                        b.Write(this.data.Length);
                        b.Write(this.data);
                    }
                }
                else {
                    if(this.lines == null)
                        b.Write(-1);
                    else {
                        b.Write(this.lines.Length);
                        foreach(string line in this.lines)
                            b.Write(line);
                    }
                }
            }
            else {
                StreamWriter t = export.T;
                t.WriteLine(this.binary);

                if(this.binary) {
                    if(this.data == null)
                        t.WriteLine(-1);
                    else
                        t.WriteLine(this.data.Length);
                    // Must be finished !!!
                }
                else {
                    if(this.lines == null)
                        t.WriteLine(-1);
                    else {
                        t.WriteLine(this.lines.Length);
                        foreach(string line in this.lines)
                            t.WriteLine(line);
                    }
                }
            }
        }

        /// <summary>
        /// Import from the file
        /// </summary>
        /// <param name="import">Import object</param>
        public FileData(Core.Import import) {
            if(import.Binary) {
                BinaryReader b = import.B;
                this.binary = b.ReadBoolean();
                int length = b.ReadInt32();

                if(length < 0) {
                    this.data = null;
                    this.lines = null;
                }
                else {
                    if(this.binary) {
                        this.data = b.ReadBytes(length);
                        this.lines = null;
                    }
                    else {
                        this.data = null;
                        this.lines = new TArray(typeof(string), length);

                        for(int i = 0; i < length; i++)
                            this.lines[i] = b.ReadString();
                    }
                }
            }
            else {
                StreamReader t = import.T;
                this.binary = bool.Parse(t.ReadLine());
                int length = int.Parse(t.ReadLine());

                if(length < 0) {
                    this.data = null;
                    this.lines = null;
                }
                else {
                    if(!this.binary) {
                        this.data = null;
                        this.lines = new TArray(typeof(string), length);

                        for(int i = 0; i < length; i++)
                            this.lines[i] = t.ReadLine();
                    }
                }
            }
        }
        #endregion
    }
}