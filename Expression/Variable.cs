using System;

using PavelStransky.Core;

namespace PavelStransky.Expression {
	/// <summary>
	/// Prom�nn�
	/// </summary>
	public class Variable {
		// Jm�no objektu
		private string name;
		// Po�et instanc� objektu
		private static int count = 0;
		// Hodnota prom�nn�
		private object item;

		/// <summary>
		/// Jm�no objektu
		/// </summary>
		public string Name {get {return this.name;}}

		/// <summary>
		/// Hodnota prom�nn�
		/// </summary>
        public object Item { get { return this.item; } set { this.item = value; } }

        /// <summary>
        /// Zkontroluje, zda zvolen� n�zev prom�nn� vyhovuje
        /// </summary>
        /// <param name="name">N�zev prom�nn�</param>
        private void CheckName(string name) {
            // No space
            if(name.IndexOf(' ') >= 0)
                throw new VariableException(string.Format(Messages.EMBadVariableName, name), Messages.EMBadVariableNameNoSpace);

            int length = name.Length;
            for(int i = 0; i < length; i++) {
                char c = name[i];

                if(!((c >= 'a' && c <= 'z')
                    || (c >= 'A' && c <= 'Z') 
                    || c == '_' || c == '�'
                    || (i > 0 && ((c >= '1' && c <= '9')
                        || c == '0' || c == '-'))))
                    throw new VariableException(string.Format(Messages.EMBadVariableName, name),
                        string.Format(Messages.EMBadVariableNameInvalidCharacter, c, i));
            }
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name">Jm�no objektu</param>
        /// <param name="item">Hodnota</param>
        /// <param name="checkName">Kontrola platn�ho jm�na</param>
        public Variable(string name, object item) : this(name, item, true) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name">Jm�no objektu</param>
        /// <param name="item">Hodnota</param>
        /// <param name="checkName">Kontrola platn�ho jm�na</param>
        public Variable(string name, object item, bool checkName) {
            if(name == string.Empty)
                this.name = string.Format("{0}{1}", defaultName, count++);
            else {
                if(checkName)
                    this.CheckName(name);
                this.name = name;
            }

			this.item = item;
		}

		private string defaultName = "Object";
	}
}
