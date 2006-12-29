using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Z argument� funkce vytvo�� vektor
	/// </summary>
	public class FnVector: FunctionDefinition {
		public override string Name {get {return name;}}
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}
		
		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			ArrayList array = new ArrayList();

			for(int i = 0; i < arguments.Count; i++) {
                this.ConvertInt2Double(arguments, i);
                if(!this.Add(array, arguments[i]))
                    this.BadTypeError(arguments[i], i);
			}

            // P�eveden� na vektor
            Vector result = new Vector(array.Count);
            int j = 0;

            foreach(double d in array)
                result[j++] = d;

			return result;
		}

        /// <summary>
        /// P�id� prvek do seznamu jako double
        /// </summary>
        /// <param name="array">Seznam</param>
        /// <param name="item">P�id�van� prvek</param>
        /// <returns>True, pokud se operace poda�ila</returns>
        private bool Add(ArrayList array, object item) {
            if(item is double)
                array.Add((double)item);

            else if(item is Vector) {
                Vector v = item as Vector;
                for(int j = 0; j < v.Length; j++)
                    array.Add(v[j]);
            }

            else if(item is Matrix) {
                Matrix m = item as Matrix;
                for(int j = 0; j < m.LengthX; j++)
                    for(int k = 0; k < m.LengthY; k++)
                        array.Add(m[j, k]);
            }

            else if(item is TArray) {
                foreach(object o in item as TArray)
                    if(!this.Add(array, o))
                        return false;
            }

            else
                return false;

            return true;
        } 

		private const string name = "vector";
		private const string help = "Z argument� funkce vytvo�� vektor (Vector)";
		private const string parameters = "[prvek1; [prvek2; [; ...]]]";
	}
}
