using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Expression.Functions;
using PavelStransky.Expression.Functions.Def;

namespace PavelStransky.Expression {
    // Definice funkcí pro tøídu Atom
    public abstract partial class Atom {
        /// <summary>
		/// Statický konstruktor (vytváøí všechny tøídy a operátory)
		/// </summary>
		static Atom() {
			functions = new FncList();

            // Vlastní Assembly
            Assembly assembly = Assembly.GetCallingAssembly();
            Type[] types = assembly.GetTypes();
            int length = types.Length;

            for(int i = 0; i < length; i++) {
                // Konstruujeme všechny tøídy ze jmenného prostoru PavelStransky.Expression.Functions.Def
                if(types[i].Namespace != "PavelStransky.Expression.Functions.Def")
                    continue;

                // Najdeme konstruktory
                ConstructorInfo[] c = types[i].GetConstructors();

                if(c.Length == 1) {
                    ParameterInfo[] p = c[0].GetParameters();

                    Fnc function = null;

                    // Žádný parametr konstruktoru
                    if(p.Length == 0)
                        function = Activator.CreateInstance(types[i]) as Fnc;

                    // Parametr vyžadující zpìtnou referenci na functions
                    else if(p[0].ParameterType == functions.GetType())
                        function = Activator.CreateInstance(types[i], functions) as Fnc;

                    else
                        continue;

                    functions.Add(function);
                }
            }
        }

        protected static readonly FncList functions;

        // Znaky otevírání a uzavírání
        protected static char[] openBracketChars = { '(', '[' };
        protected static char[] closeBracketChars = { ')', ']' };

        private static char stringChar = '"';
        private static char specialChar = '\\';
        private static char separatorChar = ';';
        private static char endVariableChar = '$';

        private static string commentChars = "%%";
        private static string noMeanChars = " \t\r\n";
        private static string variableChars = "_";          // Znaky, které se smìjí vyskytovat v promìnné

        private const string boolTrue = "true";
        private const string boolFalse = "false";

    }
}
