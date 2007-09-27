using System;
using System.IO;
using System.Collections;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Expression {
    public class List: ArrayList, IExportable {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public List() { }

        #region Implementace IExportable
        /// <summary>
        /// Uloží obsah seznamu do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            int count = this.Count;

            IEParam param = new IEParam();
            param.Add(count, "Number of elements");

            foreach(object o in this)
                param.Add(o);

            param.Export(export);
        }

        /// <summary>
        /// Naète obsah øady ze souboru textovì
        /// </summary>
        /// <param name="import">Import</param>
        public List(Core.Import import)
            : base() {
            IEParam param = new IEParam(import);
            int count = (int)param.Get(0);

            for(int i = 0; i < count; i++)
                this.Add(param.Get());
        }
        #endregion

        /// <summary>
        /// Výpis øady jako øetìzec
        /// </summary>
        public override string ToString() {
            StringBuilder sb = new StringBuilder();

            foreach(object o in this) {
                sb.Append(o);
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Return the items of the list as an array
        /// </summary>
        public TArray ToTArray() {
            int count = this.Count;

            if(this.Count == 0)
                throw new ExpressionException(Messages.EMZeroLengthList);

            bool isDouble = false;
            bool isInt = false;
            bool isOther = false;

            foreach(object o in this) {
                if(o is int)
                    isInt = true;
                else if(o is double)
                    isDouble = true;
                else
                    isOther = true;
            }

            if(isOther && (isDouble || isInt))
                throw new ExpressionException(Messages.EMListToArrayConvert);

            TArray result;
            if(isDouble)
                result = new TArray(typeof(double), count);
            else if(isInt)
                result = new TArray(typeof(int), count);
            else
                result = new TArray(this[0].GetType(), count);

            int i = 0;
            foreach(object o in this) 
                result[i++] = o;

            return result;
        }

        public override object Clone() {
            List result = new List();
            result.AddRange(this);
            return result;
        }

        /// <summary>
        /// Checks whether all elements of the list has the same type; if not, throw an exception
        /// </summary>
        /// <returns>Type of the elements</returns>
        public Type CheckOneType() {
            Type t = null;
            foreach(object o in this) {
                if(o == null)
                    continue;
                if(t == null)
                    t = o.GetType();
                else if(t != o.GetType())
                    throw new ExpressionException(Messages.EMNotEqualTypes);
            }

            return t;
        }
    }
}
