using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Expression;
using PavelStransky.Systems;

namespace PavelStransky.Expression {
    /// <summary>
    /// Nová implementace Import (nové typy)
    /// </summary>
    public class Import : PavelStransky.Systems.Import {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Soubor k otevøení</param>
        public Import(string fileName) : base(fileName) { }

        /// <summary>
        /// Nové objekty
        /// </summary>
        /// <param name="typeName">Jméno typu objektu</param>
        /// <returns></returns>
        public override object CreateObject(string typeName) {
            if(typeName == typeof(TArray).FullName || typeName == "PavelStransky.Expression.Array")
                return new TArray(this);
            else if(typeName == typeof(List).FullName)
                return new List(this);
            else if(typeName == typeof(Context).FullName)
                return new Context(this);
            else if(typeName == typeof(GraphParameterValues).FullName)
                return new GraphParameterValues(this);
            else if(typeName == typeof(Graph).FullName)
                return new Graph(this);
            else if(typeName == typeof(UserFunction).FullName)
                return new UserFunction(this);
            else if(typeName == typeof(AME.AMEItem).FullName)
                return new AME.AMEItem(this);
            else if(typeName == typeof(AME).FullName)
                return new AME(this);

                return base.CreateObject(typeName);
        }
    }
}
