using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
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

        public static List[] ReadStatistics() {
            string fName = Path.Combine(Context.GlobalContextDirectory, fNameStatistics);

            List names = new List();    // Jm�na funkc�
            List ticks = new List();    // Celkov� po�et str�ven�ch tick�
            List dates = new List();    // Data posledn�ho pou�it� funkc�
            List calls = new List();    // Celkov� po�et vol�n�

            // Na�teme, co m�me
            if(File.Exists(fName)) {
                Import import = new Import(fName);
                IEParam pi = new IEParam(import);
                names = (List)pi.Get(names);
                ticks = (List)pi.Get(ticks);
                dates = (List)pi.Get(dates);
                calls = (List)pi.Get(calls);
                import.Close();
            }

            int diff = names.Count - ticks.Count;
            for(int i = 0; i < diff; i++)
                ticks.Add((long)0);

            diff = names.Count - dates.Count;
            for(int i = 0; i < diff; i++)
                dates.Add((long)0);

            diff = names.Count - calls.Count;
            for(int i = 0; i < diff; i++)
                calls.Add((long)0);

            List[] result = new List[4];
            result[0] = names;
            result[1] = ticks;
            result[2] = dates;
            result[3] = calls;

            return result;
        }
        

        /// <summary>
        /// Ulo�� statistiku
        /// </summary>
        public static void SaveStatistics() {
            string fName = Path.Combine(Context.GlobalContextDirectory, fNameStatistics);

            // Na�teme, co m�me
            List[] read = ReadStatistics();

            List names = read[0];
            List ticks = read[1];
            List dates = read[2];
            List calls = read[3];

            // Uprav�me
            foreach(Fnc fnc in functions.Values) {
                int i = names.IndexOf(fnc.Name.ToLower());
                if(i >= 0) {
                    if(fnc.Calls > 0) {
                        ticks[i] = (long)ticks[i] + fnc.TotalTicks;
                        dates[i] = DateTime.Now.Ticks;
                        calls[i] = (long)calls[i] + fnc.Calls;
                    }
                }
                else {
                    names.Add(fnc.Name);
                    ticks.Add(fnc.TotalTicks);
                    dates.Add((long)0);
                    calls.Add((long)0);
                }
            }

            string fNameSave = fName + ".sav";

            // Ulo��me
            Export export = new Export(fNameSave, IETypes.Compressed, 1, "FncStatistics");
            IEParam pe = new IEParam();
            pe.Add(names, "Function names");
            pe.Add(ticks, "Total ticks");
            pe.Add(dates, "Last use");
            pe.Add(calls, "Number of callings");
            pe.Export(export);
            export.Close();

            // Do�asn� soubor, do kter�ho jsme ukl�dali, p�ejmenujeme na orig. verzi
            if(File.Exists(fName))
                File.Delete(fName);
            File.Move(fNameSave, fName);
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

        private static string fNameStatistics = "fnc.sta";

        private const string boolTrue = "true";
        private const string boolFalse = "false";

    }
}
