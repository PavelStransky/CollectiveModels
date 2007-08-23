using System;
using System.Collections;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Parametry pro import a export
    /// </summary>
    public class IEParam : IExportable {
        // Parametry objektu
        private ArrayList param = new ArrayList();
        private int paramIndex = 0;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public IEParam() { }

        /// <summary>
        /// Konstruktor, který rovnou naète data
        /// </summary>
        /// <param name="import"></param>
        public IEParam(Import import) {
            this.Import(import);
        }

        /// <summary>
        /// Poèet parametrù
        /// </summary>
        public int Count { get { return this.param.Count; } }

        /// <summary>
        /// Pøidá parametr do seznamu
        /// </summary>
        /// <param name="param">Hodnota parametru</param>
        /// <param name="name">Jméno parametru</param>
        /// <param name="comment">Komentáø</param>
        public void Add(object param, string name, string comment) {
            if(name == null)
                name = string.Empty;
            if(comment == null)
                comment = string.Empty;

            object[] o = new object[3];
            o[0] = param;
            o[1] = name;
            o[2] = comment;

            this.param.Add(o);
        }

        /// <summary>
        /// Pøidá parametr do seznamu k zapsání
        /// </summary>
        /// <param name="param">Hodnota parametru</param>
        /// <param name="comment">Komentáø</param>
        public void Add(object param, string comment) {
            this.Add(param, string.Empty, comment);
        }

        /// <summary>
        /// Pøidá parametr do seznamu k zapsání
        /// </summary>
        /// <param name="param">Hodnota parametru</param>
        public void Add(object param) {
            this.Add(param, string.Empty, string.Empty);
        }

        /// <summary>
        /// Vrátí parametr; pokud už není žádný k dispozici, vrátí defaultní hodnotu
        /// </summary>
        /// <param name="defaultValue">Default hodnota</param>
        /// <param name="name">Jméno parametru</param>
        /// <param name="comment">Komentáø parametru</param>
        public object Get(object defaultValue, out string name, out string comment) {
            object result = null;

            if(this.param.Count > this.paramIndex) {
                object[] o = this.param[this.paramIndex++] as object[];
                result = o[0];
                name = (string)o[1];
                comment = (string)o[2];
            }
            else {
                result = defaultValue;
                name = string.Empty;
                comment = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// Vrátí pøeètený parametr; pokud už není žádný k dispozici, vrátí defaultní hodnotu
        /// </summary>
        /// <param name="defaultValue">Default hodnota</param>
        public object Get(object defaultValue) {
            object result = null;

            if(this.param.Count > this.paramIndex)
                result = (this.param[this.paramIndex++] as object[])[0];
            else
                result = defaultValue;


            return result;
        }

        /// <summary>
        /// Vrátí pøeètený parametr; pokud už není žádný k dispozici, vrátí null
        /// </summary>
        public object Get() {
            return this.Get(null);
        }

        #region Implementace IExportable
        /// <summary>
        /// Vypíše parametry
        /// </summary>
        public void Export(Export export) {
            int count = this.param.Count;
            if(export.Binary) export.B.Write(count); else export.T.WriteLine(count);

            foreach(object[] o in this.param) {
                export.Write(o[0], o[1] as string, o[2] as string);
            }
        }

        /// <summary>
        /// Naète parametry
        /// </summary>
        public void Import(Import import) {
            int count = (int)import.Read(typeof(int).FullName);
            this.param.Clear();

            this.paramIndex = 0;

            for(int i = 0; i < count; i++) {
                string typeName = string.Empty;
                object[] o = new object[3];

                if(import.Binary) {
                    typeName = import.B.ReadString();
                    o[1] = import.B.ReadString();
                    o[2] = import.B.ReadString();
                }
                else {
                    string[] s = import.T.ReadLine().Split('\t');
                    typeName = s[0];
                    o[1] = s.Length > 1 ? import.DecodeString(s[1]) : string.Empty;
                    o[2] = s.Length > 2 ? import.DecodeString(s[2]) : string.Empty;
                }

                o[0] = import.Read(typeName);

                this.param.Add(o);
            }
        }
        #endregion
    }
}
