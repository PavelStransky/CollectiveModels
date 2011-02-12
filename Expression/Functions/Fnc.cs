using System;
using System.IO;
using System.Text;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Core;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Tøída implementující funkce
	/// </summary>
	public abstract class Fnc {
		/// <summary>
		/// Jméno funkce
		/// </summary>
		public virtual string Name {get {return this.GetType().Name.ToLower();}}

        /// <summary>
        /// True, pokud funkce bezprostøednì nemìní nic na contextu
        /// </summary>
        public virtual bool ContextThreadSafe { get { return true; } }

		/// <summary>
		/// Nápovìda k funkci
		/// </summary>
		public virtual string Help {get {return string.Empty;}}

		/// <summary>
		/// Plná nápovìda vèetnì užití
		/// </summary>
		public virtual string FullHelp {get {return string.Format(fullHelpFormat, this.Help, this.Use);}}

		/// <summary>
		/// Použití funkce
		/// </summary>
		public virtual string Use {get {return string.Format(useFormat, this.Name, this.ParametersHelp);}}

        /// <summary>
        /// Constructor
        /// </summary>
        public Fnc() {
            this.CreateParameters();
        }

        #region Parameters
        /// <summary>
        /// Parameters
        /// </summary>
        private ParameterItem[] parameters;
        private bool infiniteParams;

        /// <summary>
        /// Create the parameters' array
        /// </summary>
        protected virtual void CreateParameters() {
            this.parameters = new ParameterItem[0];
        }

        /// <summary>
        /// Creates a parameter array with specified number of parameters
        /// </summary>
        /// <param name="n">Number of parameters</param>
        protected void SetNumParams(int n) {
            this.SetNumParams(n, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="infiniteParams">Set the flag that says that the last parameter can repeat</param>
        protected void SetNumParams(int n, bool infiniteParams) {
            this.parameters = new ParameterItem[n];
            this.infiniteParams = infiniteParams;
        }

        /// <summary>
        /// Parameters
        /// </summary>
        protected ParameterItem[] Parameters { get { return this.parameters; } }

        /// <summary>
        /// Number of parameters
        /// </summary>
        public int NumParams { get { if(this.infiniteParams) return int.MaxValue; else return this.parameters.Length; } }

        /// <summary>
        /// Parameters of the function
        /// </summary>
        public virtual string ParametersHelp {
            get {
                StringBuilder s = new StringBuilder();
                int length = this.parameters.Length;

                for(int i = 0; i < length; i++) {
                    if(i != 0) 
                        s.Append(';');

                    s.Append(Environment.NewLine);
                    s.Append(' ');

                    ParameterItem p = this.parameters[i];

                    if(!p.Obligatory)
                        s.Append('[');

                    s.Append(p.Description);

                    if(p.Evaluate) {
                        if(p.Types.Length > 0) {
                            s.Append(" (");
                            s.Append(p.TypesNames);
                            s.Append(')');
                        }
                    }

                    if(p.DefaultValue != null) {
                        s.Append(" = ");
                        if(p.DefaultValue is string)
                            s.Append('"');
                        s.Append(p.DefaultValue);
                        if(p.DefaultValue is string)
                            s.Append('"');
                    }

                    if(!p.Obligatory)
                        s.Append(']');
                }

                if(this.infiniteParams) {
                    s.Append(Environment.NewLine);
                    s.Append(" ...");
                }

                s.Append(Environment.NewLine);

                return s.ToString();
            }
        }

        /// <summary>
        /// Set the parameter type
        /// </summary>
        /// <param name="i">Parameter position</param>
        /// <param name="obligatory">True if the parameter is obligatory (must not be null)</param>
        /// <param name="evaluate">True if the parameter should be evaluated</param>
        /// <param name="intToDouble">Integer should be implicitly converted to double</param>
        /// <param name="name">Name of the parameter</param>
        /// <param name="description">Description of the parameter</param>
        /// <param name="types">Possible types of the parameter</param>
        /// <param name="defaultValue">Default value of the parameter</param>
        protected void SetParam(int i, bool obligatory, bool evaluate, bool intToDouble, string name, string description, object defaultValue, params Type[] types) {
            this.parameters[i] = new ParameterItem(obligatory, evaluate, intToDouble, name, description, defaultValue, types);
        }

        /// <summary>
        /// Check the number and the types of the parameters
        /// </summary>
        protected virtual void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) { 
            int l = this.parameters.Length;
            int le = evaluatedArguments.Count;

            if(!this.infiniteParams)
                this.CheckArgumentsMaxNumber(evaluatedArguments, l);

            int i = 0;
            for(i = 0; i < le; i++) {
                object o = evaluatedArguments[i];
                ParameterItem p = i < l ? this.parameters[i] : this.parameters[l - 1];

                if(!p.Evaluate)
                    continue;

                if(o == null)
                    evaluatedArguments[i] = p.DefaultValue;
                if(o == null && p.Obligatory && i < l)
                    throw new FncException(this, string.Format(Messages.EMObligatoryParameter, p.Name, i, this.Name));

                if(p.IntToDouble)
                    this.ConvertInt2Double(evaluatedArguments, i);

                if(o != null)
                    this.CheckArgumentsType(evaluatedArguments, i, evaluateArray, p.Types);
            }

            for(int j = i; j < l; j++) {
                ParameterItem p = this.parameters[j];

                if(p.Obligatory)
                    throw new FncException(this, string.Format(Messages.EMObligatoryParameter, p.Name, i, this.Name));

                evaluatedArguments.Add(p.DefaultValue);
            }
        }

        /// <summary>
        /// Funkce, která zkontroluje, jestli je zadaný správný poèet argumentù, jinak vyhodí výjimku
        /// </summary>
        /// <param name="args">Argumenty funkce</param>
        /// <param name="number">Požadovaný minimální poèet argumentù</param>
        protected void CheckArgumentsMinNumber(ArrayList args, int number) {
            if(args.Count < number)
                throw new FncException(
                    this,
                    string.Format(Messages.EMFewArgs, this.Name),
                    string.Format(Messages.EMInvalidNumberArgsDetail, number, args.Count));
        }

        /// <summary>
        /// Funkce, která zkontroluje, jestli je zadaný správný poèet argumentù, jinak vyhodí výjimku
        /// </summary>
        /// <param name="args">Argumenty funkce</param>
        /// <param name="number">Požadovaný maximální poèet argumentù</param>
        protected void CheckArgumentsMaxNumber(ArrayList args, int number) {
            if(args.Count > number)
                throw new FncException(
                    this,
                    string.Format(Messages.EMManyArgs, this.Name),
                    string.Format(Messages.EMInvalidNumberArgsDetail, number, args.Count));
        }

        /// <summary>
        /// Funkce, která zkontroluje, jestli je zadaný správný poèet argumentù, jinak vyhodí výjimku
        /// </summary>
        /// <param name="args">Argumenty funkce</param>
        /// <param name="number">Požadovaný maximální poèet argumentù</param>
        protected void CheckArgumentsNumber(ArrayList args, int number) {
            if(args.Count != number)
                throw new FncException(
                    this,
                    string.Format(Messages.EMInvalidNumberArgs, this.Name),
                    string.Format(Messages.EMInvalidNumberArgsDetail, number, args.Count));
        }

        /// <summary>
        /// Kontroluje, jestli má zadaný argument správný typ
        /// </summary>
        /// <param name="args">Argumenty funkce</param>
        /// <param name="i">Index argumentu</param>
        /// <param name="type">Požadovaný typ argumentu</param>
        /// <param name="evaluateArray">True for evaluating array</param>
        protected void CheckArgumentsType(ArrayList args, int i, bool evaluateArray, Type type) {
            if(i >= args.Count)
                return;

            Type t = args[i].GetType();

            Type[] interfaces = type.IsInterface ? t.FindInterfaces(this.InterfaceFilter, type.FullName) : new Type[0];
            if(interfaces.Length > 0)
                return;

            do {
                if(t == type)
                    return;
                t = t.BaseType;
            } while(t != null);

            t = args[i].GetType();

            if(t == typeof(TArray) && evaluateArray) {
                Type at = (args[i] as TArray).GetItemType();

                interfaces = type.IsInterface ? at.FindInterfaces(this.InterfaceFilter, type.FullName) : new Type[0];
                if(interfaces.Length > 0)
                    return;

                do {
                    if(at == type)
                        return;
                    at = at.BaseType;
                } while(at != null);

                throw new FncException(
                    this,
                    string.Format(Messages.EMBadParamType, i + 1, this.Name),
                    string.Format(Messages.EMBadParamTypeDetail, type.FullName, at.FullName));
            }
            throw new FncException(
                this,
                string.Format(Messages.EMBadParamType, i + 1, this.Name),
                string.Format(Messages.EMBadParamTypeDetail, type.FullName, t.FullName));
        }

        /// <summary>
        /// Kontroluje, jestli má zadaný argument správný typ
        /// </summary>
        /// <param name="args">Argumenty funkce</param>
        /// <param name="i">Index argumentu</param>
        /// <param name="types">Požadované typy argumentu</param>
        /// <param name="evaluateArray">True for evaluating array</param>
        protected void CheckArgumentsType(ArrayList args, int i, bool evaluateArray, params Type[] types) {
            if(i >= args.Count)
                return;

            string ts = string.Empty;

            if(types == null || types.Length == 0)
                return;

            foreach(Type type in types) {
                Type t = args[i].GetType();
                Type at = (args[i] is TArray && evaluateArray) ? (args[i] as TArray).GetItemType() : null;

                Type[] interfaces = type.IsInterface ? t.FindInterfaces(this.InterfaceFilter, type.FullName) : new Type[0];
                if(interfaces.Length > 0)
                    return;

                interfaces = (type.IsInterface && at != null) ? at.FindInterfaces(this.InterfaceFilter, type.FullName) : new Type[0];
                if(interfaces.Length > 0)
                    return;

                if(ts != string.Empty)
                    ts += ", ";
                ts += type.FullName;

                do {
                    if(t == type)
                        return;
                    t = t.BaseType;
                } while(t != null);

                if(at == null)
                    continue;

                do {
                    if(at == type)
                        return;
                    at = at.BaseType;
                } while(at != null);
            }

            throw new FncException(
                this,
                string.Format(Messages.EMBadParamType, i + 1, this.Name),
                string.Format(Messages.EMBadParamTypeDetail, ts, (args[i] is TArray && evaluateArray) ? (args[i] as TArray).GetItemType() : args[i].GetType()));
        }

        /// <summary>
        /// Method providing finding interfacese
        /// </summary>
        /// <param name="type"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected bool InterfaceFilter(Type type, Object criteria) {
            if(type.FullName == criteria.ToString())
                return true;
            else
                return false;
        }

        /// <summary>
        /// Pokud je zadaný argument int, pøevede ho na double
        /// </summary>
        /// <param name="args">Argumenty funkce</param>
        /// <param name="i">Index argumentu</param>
        protected void ConvertInt2Double(ArrayList args, int i) {
            if(i < args.Count && args[i] is int)
                args[i] = (double)(int)args[i];
        }

        /// <summary>
        /// Evaluates the arguments
        /// </summary>
        /// <param name="arguments">Arguments before calculation (strings)</param>
        /// <param name="guider">Guide through calculation</param>
        protected ArrayList EvaluateArguments(Guider guider, ArrayList arguments) {
            ArrayList result = new ArrayList();
            int l = this.parameters.Length;
            bool evaluateLast = l > 0 ? this.parameters[l - 1].Evaluate : true;

            for(int i = 0; i < arguments.Count; i++) {
                bool evaluate = i < l ? this.parameters[i].Evaluate : evaluateLast;

                if(evaluate)
                    result.Add(Atom.EvaluateAtomObject(guider, arguments[i]));
                else
                    result.Add(arguments[i]);
            }
            return result;
        }
        #endregion

        /// <summary>
		/// Výpoèet funkce
		/// </summary>
		/// <param name="arguments">Argumenty funkce</param>
        /// <param name="guider">Prùvodce výpoètem</param>
        /// <param name="evaluateArray">True if the array shall be evaluated</param>
		public virtual object Evaluate(Guider guider, ArrayList arguments) {
            ArrayList evaluatedArguments = this.EvaluateArguments(guider, arguments);
            bool evaluateArray = guider.ArrayEvaluation;

			this.CheckArguments(evaluatedArguments, evaluateArray);

            if(evaluateArray)
                return this.EvaluateArray(guider, evaluatedArguments);
            else
                return this.EvaluateFn(guider, evaluatedArguments);
		}

		/// <summary>
		/// Provádí výpoèet po zkontrolování parametrù
		/// </summary>
		/// <param name="depth">Aktuální hloubka výpoètu</param>
		/// <param name="item">Prvek, který se poèítá</param>
		/// <param name="arguments">Parametry</param>
        protected virtual object EvaluateFn(Guider guider, ArrayList arguments) {
            return null;
        }

        /// <summary>
        /// Zkontroluje, zda mají øady stejné dimenze. Pokud ne, vyhodí výjimku
        /// </summary>
        /// <param name="a1">Øada 1</param>
        /// <param name="a2">Øada 2</param>
        protected void CheckArrayLengths(TArray a1, TArray a2) {
            if(!TArray.IsEqualDimension(a1, a2)) 
                throw new FncException(
                    this,
                    string.Format(Messages.EMBadArrayDimensions, this.Name),
                    string.Format(Messages.EMBadArrayDimensionsDetail, a1.LengthsString(), a2.LengthsString()));
        }

		/// <summary>
		/// Vypoèítá zadanou funkci pro každý prvek øady
		/// </summary>
		/// <param name="arguments">Parametry funkce</param>
        /// <param name="guider">Prùvodce výpoètem</param>
		protected object EvaluateArray(Guider guider, ArrayList arguments) {
            // Hledáme, zda je ve výsledcích nìjaká øada a kontrolujeme, zda ostatní
            // øady mají stejnou délku
            TArray array = null;
            foreach(object arg in arguments)
                if(arg is TArray) {
                    if(array == null)
                        array = arg as TArray;
                    else
                        this.CheckArrayLengths(array, arg as TArray);
                }

            // Nenašli jsme øadu - poèítáme jeden prvek
            if(array == null)
                return this.EvaluateFn(guider, arguments);

            array.ResetEnumerator();
            int[] index = (int[])array.StartEnumIndex.Clone();
            int[] lengths = array.Lengths;
            int rank = array.Rank;
            TArray result = null;

            do {
                ArrayList args = new ArrayList();
                foreach(object arg in arguments) {
                    if(arg is TArray)
                        args.Add((arg as TArray)[index]);
                    else
                        args.Add(arg);
                }

                object o = this.EvaluateFn(guider, args);
                if(result == null)
                    result = new TArray(o.GetType(), lengths);

                result[index] = o;
            }
            while(TArray.MoveNext(rank, index, array.StartEnumIndex, array.EndEnumIndex));

			return result;
		}

        /// <summary>
        /// Pøevede zadaný argument (øadu) na øadu vektorù
        /// </summary>
        /// <param name="args">Argumenty funkce</param>
        /// <param name="i">Index argumentu</param>
        protected void ConvertArray2Vectors(ArrayList args, int i) {
            if(args[i] is int) {
                Vector v = new Vector(1);
                v[0] = (int)args[i];
                args[i] = v;
            }
            else if(args[i] is double) {
                Vector v = new Vector(1);
                v[0] = (double)args[i];
                args[i] = v;
            }
            else
                args[i] = (args[i] as TArray).CovertToVectors();
        }

		/// <summary>
		/// Vyvolá výjimku pøi nalezení chybného typu objektu
		/// </summary>
		/// <param name="item">Prvek</param>
		protected object BadTypeError(object item, int i) {
			throw new FncException(
                this,
                string.Format(Messages.EMBadParamType, i + 1, this.Name),
				string.Format(Messages.EMBadParamTypeDetail, this.parameters[i].TypesNames, item.GetType().FullName));
		}

        /// <summary>
        /// Z contextu vytáhne název aktuálního adresáøe a doplní cestu
        /// </summary>
        /// <param name="context">Kontext</param>
        /// <param name="fileName">Zadané jméno souboru</param>
        protected string AddPath(Context context, string fileName) {
            if(!Path.IsPathRooted(fileName))
                fileName = Path.Combine(context.Directory, fileName);
           
            return fileName;
        }

        private const string useFormat = "{0}({1});";
		private const string fullHelpFormat = "{0}\r\n\r\n{1}";
	}
}
