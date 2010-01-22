using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Vytvoøí a zapouzdøí operátor porovnávání
    /// </summary>
    public class ComparisonOperator {
        private enum Operator {
            Equal,
            NotEqual,
            Less,
            LessEqual,
            Greater,
            GreaterEqual
        }

        private Operator op;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="soperator">Operátor ve tvaru øetìzce</param>
        public ComparisonOperator(string soperator) {
            soperator = soperator.Trim();

            if(soperator == "==")
                this.op = Operator.Equal;
            else if(soperator == "!=")
                this.op = Operator.NotEqual;
            else if(soperator == ">")
                this.op = Operator.Greater;
            else if(soperator == "<")
                this.op = Operator.Less;
            else if(soperator == ">=" || soperator == "=>")
                this.op = Operator.GreaterEqual;
            else if(soperator == "<=" || soperator == "=<")
                this.op = Operator.LessEqual;
            else
                throw new Exception(string.Format(Messages.EMBadComparisonOperator, soperator));
        }

        /// <summary>
        /// Vrací výsledek porovnání dvou hodnot
        /// </summary>
        /// <param name="x">První hodnota</param>
        /// <param name="y">Druhá hodnota</param>
        public bool Compare(double x, double y) {
            switch(this.op) {
                case Operator.Equal: return x == y;
                case Operator.NotEqual: return x != y;
                case Operator.Greater: return x > y;
                case Operator.GreaterEqual: return x >= y;
                case Operator.Less: return x < y;
                case Operator.LessEqual: return x <= y;
            }

            return false;
        }
    }
}
