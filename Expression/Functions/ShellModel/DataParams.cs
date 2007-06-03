using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Expression {
    /// <summary>
    /// Stores all parameters needed for specific calculation
    /// </summary>
    public class DataParams {
        private ArrayList names = new ArrayList();
        private ArrayList values = new ArrayList();
        private ArrayList help = new ArrayList();
        private ArrayList types = new ArrayList();

        /// <summary>
        /// Add parameter with its default value
        /// </summary>
        /// <param name="paramName">Name of the parameter</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="type">Type of the parameter</param>
        /// <param name="help">Help for the parameter</param>
        public void Add(string paramName, object defaultValue, object type, string help) {
            this.names.Add(paramName);
            this.values.Add(defaultValue);
            this.help.Add(help);
            this.types.Add(type);
        }

        /// <summary>
        /// Add parameter which will not be synchronized (stored type will be null)
        /// </summary>
        /// <param name="paramName">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <param name="help">Help for the parameter</param>
        public void Add(string paramName, object value, string help) {
            this.Add(paramName, value, null, help);
        }

        /// <summary>
        /// Save all the values into the file
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        public void Save(string fileName) {
            FileStream f = new FileStream(fileName, FileMode.Create);
            StreamWriter s = new StreamWriter(f);

            int count = this.names.Count;
            for(int i = 0; i < count; i++)
                s.WriteLine(string.Format("{0} = {1} ", this.names[i], this.values[i]));

            s.Close();
            f.Close();
        }

        /// <summary>
        /// Synchronizes all the parameters with variables in the context
        /// </summary>
        /// <param name="context">Context</param>
        public void Synchronize(Context context) {
            int count = this.names.Count;

            for(int i = 0; i < count; i++) {
                // If type is null, we do not synchronize anything
                if(this.types[i] == null)
                    continue;

                string name = this.names[i] as string;
                if(context.Contains(name)) {
                    object value = context[name];
                    if(value.GetType() != this.types[i] as Type)
                        throw new ExpressionException(string.Format(Messages.EMBadParamType, name),
                            string.Format(Messages.EMBadParamTypeDetail, (this.types[i] as Type).FullName, (value.GetType()).FullName));
                    this.values[i] = value;
                }
                else {
                    context.SetVariable(name, this.values[i]);
                }
            }
        }
    }
}