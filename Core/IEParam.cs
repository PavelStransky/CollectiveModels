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
        /// Konstruktor, kter� rovnou na�te data
        /// </summary>
        /// <param name="import"></param>
        public IEParam(Import import) {
            this.Import(import);
        }

        /// <summary>
        /// Po�et parametr�
        /// </summary>
        public int Count { get { return this.param.Count; } }

        /// <summary>
        /// P�id� parametr do seznamu
        /// </summary>
        /// <param name="param">Hodnota parametru</param>
        /// <param name="name">Jm�no parametru</param>
        /// <param name="comment">Koment��</param>
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
        /// P�id� parametr do seznamu k zaps�n�
        /// </summary>
        /// <param name="param">Hodnota parametru</param>
        /// <param name="comment">Koment��</param>
        public void Add(object param, string comment) {
            this.Add(param, string.Empty, comment);
        }

        /// <summary>
        /// P�id� parametr do seznamu k zaps�n�
        /// </summary>
        /// <param name="param">Hodnota parametru</param>
        public void Add(object param) {
            this.Add(param, string.Empty, string.Empty);
        }

        /// <summary>
        /// Vr�t� parametr; pokud u� nen� ��dn� k dispozici, vr�t� defaultn� hodnotu
        /// </summary>
        /// <param name="defaultValue">Default hodnota</param>
        /// <param name="name">Jm�no parametru</param>
        /// <param name="comment">Koment�� parametru</param>
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
        /// Vr�t� p�e�ten� parametr; pokud u� nen� ��dn� k dispozici, vr�t� defaultn� hodnotu
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
        /// Vr�t� p�e�ten� parametr; pokud u� nen� ��dn� k dispozici, vr�t� null
        /// </summary>
        public object Get() {
            return this.Get(null);
        }

        #region Implementace IExportable
        /// <summary>
        /// Vyp�e parametry
        /// </summary>
        public void Export(Export export) {
            int count = this.param.Count;
            if(export.Binary) export.B.Write(count); else export.T.WriteLine(count);

            foreach(object[] o in this.param) {
                export.Write(o[0], o[1] as string, o[2] as string);
            }
        }

        /// <summary>
        /// Na�te parametry
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
