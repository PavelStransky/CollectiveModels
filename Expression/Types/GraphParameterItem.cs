using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Expression {
    /// <summary>
    /// One parameter of a graph
    /// </summary>
    public class GraphParameterItem {
        private Graph.ParametersIndications indication;
        private string name;
        private string description;
        private object defaultValue;

        /// <summary>
        /// Indication of a parameter
        /// </summary>
        public Graph.ParametersIndications Indication { get { return this.indication; } }

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
        /// Constructor
        /// </summary>
        /// <param name="indication">Indication of a parameter</param>
        /// <param name="name">Name of the parameter</param>
        /// <param name="description">Description of the parameter</param>
        /// <param name="defaultValue">Default value of the parameter</param>
        public GraphParameterItem(Graph.ParametersIndications indication, string name, string description, object defaultValue) {
            this.indication = indication;
            this.name = name;
            this.description = description;
            this.defaultValue = defaultValue;
        }
    }
}
