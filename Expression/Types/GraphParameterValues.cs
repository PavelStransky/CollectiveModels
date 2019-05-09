using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;

namespace PavelStransky.Expression {
    /// <summary>
    /// Hodnoty parametrù grafu
    /// </summary>
    public class GraphParameterValues: IExportable, ICloneable {
        private Hashtable values = new Hashtable();
        private Hashtable isDefault = new Hashtable();

        private GraphParameterDefinitions definitions;

        /// <summary>
        /// Nastaví parametry
        /// </summary>
        /// <param name="first">True, pokud voláme poprvé (nastavujeme i defaultní hodnoty)</param>
        /// <param name="definitions">Definice parametrù</param>
        /// <param name="c1">1. kontext</param>
        /// <param name="c2">2. kontext</param>
        /// <param name="c3">3. kontext</param>
        private void SetParams(bool first, Context c1, Context c2, Context c3) {
            foreach(GraphParameterItem i in this.definitions.Definitions) {
                object dvalue = i.DefaultValue;
                string name = i.Name;
                bool isdef = false;

                object value = null;
                if(c1 != null && c1.Contains(name))
                    value = c1[name];
                else if(c2 != null && c2.Contains(name))
                    value = c2[name];
                else if(c3 != null && c3.Contains(name))
                    value = c3[name];

                if(value == null) {
                    if(first) {
                        value = dvalue;
                        isdef = true;
                    }
                    else
                        continue;
                }

                else {
                    if(value is Variable)
                        value = (value as Variable).Item;

                    if(dvalue is Graph.LineStyles) {
                        if(value is string)
                            value = Enum.Parse(typeof(Graph.LineStyles), value as string, true);
                        else if(value is int)
                            value = (Graph.LineStyles)(int)value;
                    }

                    else if(dvalue is Graph.PointStyles) {
                        if(value is string)
                            value = Enum.Parse(typeof(Graph.PointStyles), value as string, true);
                        else if(value is int)
                            value = (Graph.PointStyles)(int)value;
                    }

                    else if(dvalue is Color) {
                        if(value is string)
                            value = Color.FromName(value as string);
                        else if(value is int)
                            value = ColorArray.GetColor((int)value);
                        else if(value is Vector)                         {
                            Vector v = value as Vector;
                            if(v.Length == 3) 
                                value = Color.FromArgb((int)(255.0 * v[0]), (int)(255.0 * v[1]), (int)(255.0 * v[2]));
                            else if(v.Length == 4)  // With alpha channel
                                value = Color.FromArgb((int)(255.0 * v[0]), (int)(255.0 * v[1]), (int)(255.0 * v[2]), (int)(255.0 * v[3]));
                        }
                    }

                    else if(dvalue is double) {
                        if(value is int)
                            value = (double)(int)value;
                    }

                    else if(dvalue is System.Drawing.Drawing2D.DashStyle) {
                        if(value is string)
                            value = Enum.Parse(typeof(System.Drawing.Drawing2D.DashStyle), value as string, true);
                        else if(value is Vector)
                            dvalue = value;
                    }

                    if(value.GetType() != dvalue.GetType())
                        throw new ParamsException(string.Format(Messages.EMBadGraphParamType, name),
                            string.Format(Messages.EMBadGraphParamTypeDetail, dvalue.GetType().FullName, value.GetType().FullName));
                }

                if(first) {
                    this.values.Add(i.Indication, value);
                    this.isDefault.Add(i.Indication, isdef);
                }

                else {
                    this.values[i.Indication] = value;
                    this.isDefault[i.Indication] = false;
                }
            }
        }

        /// <summary>
        /// Nastaví parametry
        /// </summary>
        /// <param name="definitions">Definice parametrù</param>
        /// <param name="c1">1. kontext</param>
        /// <param name="c2">2. kontext</param>
        /// <param name="c3">3. kontext</param>
        public void SetParams(Context c1, Context c2, Context c3) {
            this.SetParams(false, c1, c2, c3);
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="definitions">Definice parametrù</param>
        /// <param name="c1">1. kontext</param>
        /// <param name="c2">2. kontext</param>
        /// <param name="c3">3. kontext</param>
        public GraphParameterValues(GraphParameterDefinitions definitions, Context c1, Context c2, Context c3) {
            this.definitions = definitions;
            this.SetParams(true, c1, c2, c3);
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="i">Oznaèení parametru</param>
        public object this[Graph.ParametersIndications i] {
            get {
                return this.values[i];
            }
            set {
                this.values[i] = value;
            }
        }

        /// <summary>
        /// True, pokud je hodnota brána z defaultních
        /// </summary>
        public bool IsDefault(Graph.ParametersIndications i) {
            return (bool)this.isDefault[i];
        }

        #region Implementace IExportable
        public void Export(Export export) {
            IEParam param = new IEParam();

            foreach(Graph.ParametersIndications key in this.values.Keys) {
                if(!this.isDefault.ContainsKey(key) || !(bool)this.isDefault[key]) {
                    param.Add((int)key);
                    object o = this.values[key];
                    if(o is Graph.LineStyles)
                        param.Add((int)(Graph.LineStyles)o);
                    else if(o is Graph.PointStyles)
                        param.Add((int)(Graph.PointStyles)o);
                    else
                        param.Add(o);
                }
            }

            param.Export(export);
        }

        /// <summary>
        /// Konstruktor pro import
        /// </summary>
        public GraphParameterValues(Core.Import import) {
            IEParam param = new IEParam(import);

            int count = param.Count;
            for(int i = 0; i < count; i += 2) {
                Graph.ParametersIndications key = (Graph.ParametersIndications)(int)param.Get();
                this.values.Add(key, param.Get());
            }
        }

        /// <summary>
        /// Pøidá defaultní parametry, které nebyly naimportovány
        /// </summary>
        /// <param name="definitions">Definice parametrù</param>
        public void AddDefaultParams(GraphParameterDefinitions definitions) {
            this.definitions = definitions;

            foreach(GraphParameterItem i in definitions.Definitions) {
                Graph.ParametersIndications key = i.Indication;

                if(this.values.Contains(key)) {
                    this.isDefault.Add(key, false);

                    if(i.DefaultValue is Graph.PointStyles)
                        this.values[key] = (Graph.PointStyles)(int)this.values[key];
                    else if(i.DefaultValue is Graph.LineStyles)
                        this.values[key] = (Graph.LineStyles)(int)this.values[key];
                }
                else {
                    this.isDefault.Add(key, true);
                    this.values.Add(key, i.DefaultValue);
                }
            }
        }
        #endregion

        #region Implementace klonování
        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        private GraphParameterValues() { }

        public object Clone() {
            GraphParameterValues result = new GraphParameterValues();

            result.definitions = this.definitions;
            result.isDefault = (Hashtable)this.isDefault.Clone();
            result.values = (Hashtable)this.values.Clone();

            return result;
        }
        #endregion
    }
}
