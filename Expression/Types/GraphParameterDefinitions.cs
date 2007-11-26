using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Expression {
    /// <summary>
    /// Parameters of a graph (definition)
    /// </summary>
    public class GraphParameterDefinitions {
        private Hashtable items = new Hashtable();

        /// <summary>
        /// Definitions of functions
        /// </summary>
        public ICollection Definitions { get { return this.items.Values; } }

        /// <summary>
        /// Adds a value into a dictionary of parameters
        /// </summary>
        /// <param name="indication">Indication of a parameter</param>
        /// <param name="name">Name of the parameter</param>
        /// <param name="description">Description of the parameter</param>
        /// <param name="defaultValue">Default value of the parameter</param>
        public void Add(Graph.ParametersIndications indication, string name, string description, object defaultValue) {
            GraphParameterItem item = new GraphParameterItem(indication, name, description, defaultValue);
            this.items.Add(indication, item);
        }

        /// <summary>
        /// Number of elements
        /// </summary>
        public int Count { get { return this.items.Count; } }
    }
}
