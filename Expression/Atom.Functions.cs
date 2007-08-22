using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Expression.Functions;
using PavelStransky.Expression.Functions.Def;

namespace PavelStransky.Expression {
    // Definice funkc� pro t��du Atom
    public abstract partial class Atom {
        /// <summary>
		/// Statick� konstruktor (vytv��� v�echny t��dy a oper�tory)
		/// </summary>
		static Atom() {
			functions = new FncList();

            // Vlastn� Assembly
            Assembly assembly = Assembly.GetCallingAssembly();
            Type[] types = assembly.GetTypes();
            int length = types.Length;

            for(int i = 0; i < length; i++) {
                // Konstruujeme v�echny t��dy ze jmenn�ho prostoru PavelStransky.Expression.Functions.Def
                if(types[i].Namespace != "PavelStransky.Expression.Functions.Def")
                    continue;

                // Najdeme konstruktory
                ConstructorInfo[] c = types[i].GetConstructors();

                if(c.Length == 1) {
                    ParameterInfo[] p = c[0].GetParameters();

                    Fnc function = null;

                    // ��dn� parametr konstruktoru
                    if(p.Length == 0)
                        function = Activator.CreateInstance(types[i]) as Fnc;

                    // Parametr vy�aduj�c� zp�tnou referenci na functions
                    else if(p[0].ParameterType == functions.GetType())
                        function = Activator.CreateInstance(types[i], functions) as Fnc;

                    else
                        continue;

                    functions.Add(function);
                }
            }
        }

        protected static readonly FncList functions;

        // Znaky otev�r�n� a uzav�r�n�
        protected static char[] openBracketChars = { '(', '[' };
        protected static char[] closeBracketChars = { ')', ']' };

        private static char stringChar = '"';
        private static char specialChar = '\\';
        private static char separatorChar = ';';
        private static char endVariableChar = '$';

        private static string commentChars = "%%";
        private static string noMeanChars = " \t\r\n";
        private static string variableChars = "_";          // Znaky, kter� se sm�j� vyskytovat v prom�nn�

        private const string boolTrue = "true";
        private const string boolFalse = "false";

    }
}
