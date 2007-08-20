using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// One parameter
    /// </summary>
    public class ParameterItem {
        private bool obligatory;
        private bool intToDouble;
        private bool evaluate;

        private string name;
        private string description;

        private object defaultValue;

        private Type[] types;

        /// <summary>
        /// True if the parameter is obligatory (must not be null)
        /// </summary>
        public bool Obligatory { get { return this.obligatory; } }

        /// <summary>
        /// Integer should be implicitly converted to double
        /// </summary>
        public bool IntToDouble { get { return this.intToDouble; } }

        /// <summary>
        /// Name of the parameter
        /// </summary>
        public string Name { get { return this.name; } }

        /// <summary>
        /// Description of the parameter
        /// </summary>
        public string Description { get { return this.description; } }

        /// <summary>
        /// Default value of the parameter
        /// </summary>
        public object DefaultValue { get { return this.defaultValue; } }

        /// <summary>
        /// Possible types of the parameter
        /// </summary>
        public Type[] Types { get { return this.types; } }

        /// <summary>
        /// True if the parameter should be evaluated
        /// </summary>
        public bool Evaluate { get { return this.evaluate; } }

        /// <summary>
        /// Returns all possible type names in a string separated by |
        /// </summary>
        public string TypesNames {
            get {
                StringBuilder result = new StringBuilder();

                foreach(Type t in this.types) {
                    if(result.Length > 0)
                        result.Append(" | ");
                    result.Append(t.Name);
                }

                return result.ToString();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="obligatory">True if the parameter is obligatory (must not be null)</param>
        /// <param name="intToDouble">Integer should be implicitly converted to double</param>
        /// <param name="name">Name of the parameter</param>
        /// <param name="description">Description of the parameter</param>
        /// <param name="defaultValue">Default value of the parameter</param>
        /// <param name="types">Possible types of the parameter</param>
        /// <param name="evaluate">True if the parameter should be evaluated</param>
        public ParameterItem(bool obligatory, bool evaluate, bool intToDouble, string name, string description, object defaultValue, params Type[] types) {
            this.obligatory = obligatory;
            this.intToDouble = intToDouble;
            this.name = name;
            this.description = description;
            this.defaultValue = defaultValue;
            this.types = types;
            this.evaluate = evaluate;
        }
    }
}
