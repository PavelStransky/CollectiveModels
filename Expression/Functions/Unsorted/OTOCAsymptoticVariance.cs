using System;
using System.Collections;
using System.Text;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Expression for the asymptotic value of the OTOC
    /// </summary>
    public class OTOCAsymptoticVariance : Fnc {
        public override string Help { get { return Messages.HelpRegression; } }

        protected override void CreateParameters() {
            this.SetNumParams(0);
        }

        // Item in one term of the expansion (x or p)
        private class Item : ICloneable {
            private int i1, i2;     // Sumation indices (depends on order)
            private bool isX;

            public Item(int i1, int i2, bool isX) {
                this.i1 = i1;
                this.i2 = i2;
                this.isX = isX;
            }

            /// <summary>
            /// Application of Delta
            /// </summary>
            /// <param name="p">Delta pair</param>
            public void Change(Pair p) {
                if(this.i1 == p.I2)
                    this.i1 = p.I1;
                if(this.i2 == p.I2)
                    this.i2 = p.I1;
            }

            /// <summary>
            /// True of both indices are the same (zero contribution due to parity)
            /// </summary>
            public bool IsZero() {
                return this.i1 == this.i2;
            }

            public object Clone() {
                return new Item(this.i1, this.i2, this.isX);
            }

            public override string ToString() {
                return string.Format("{0}{1}{2} ", this.isX ? "X" : "P", this.i1, this.i2);
            }
        }

        /// <summary>
        /// Ordered pair of numbers
        /// </summary>
        private class Pair {
            private int i1, i2;

            public Pair(int i1, int i2) {
                if(i1 < i2) {
                    this.i1 = i1;
                    this.i2 = i2;
                }
                else {
                    this.i1 = i2;
                    this.i2 = i1;
                }
            }

            /// <summary>
            /// Changes indices according to a pair of delta
            /// </summary>
            public void Normalize(Pair p) {
                if(this.i1 == p.I1 && this.i2 == p.I2)
                    return;

                if(this.i1 == p.I2)
                    this.i1 = p.I1;
                if(this.i2 == p.I2)
                    this.i2 = p.I1;

                if(this.i1 > this.i2) {
                    int s = this.i1;
                    this.i1 = this.i2;
                    this.i2 = s;
                }
            }

            public int I1 { get { return this.i1; } }
            public int I2 { get { return this.i2; } }
        }

        /// <summary>
        /// Product of four delta functions
        /// </summary>
        private class Quadruple {
            private Pair[] pairs;

            public Quadruple(params Pair[] pairs) {
                this.pairs = pairs;
                this.Normalize();
            }

            /// <summary>
            /// Change of indices (delta of delta)
            /// </summary>
            private void Normalize() {
                for(int i = 0; i < this.pairs.Length - 1; i++)
                    for(int j = i + 1; j < this.pairs.Length; j++)
                        this.pairs[j].Normalize(this.pairs[i]);
            }

            public Pair this[int i] { get { return this.pairs[i]; } }

            public override string ToString() {
                StringBuilder s = new StringBuilder();
                foreach(Pair pair in this.pairs)
                    s.Append(string.Format("{0}{1} ", pair.I1, pair.I2));
                return s.ToString();
            }
        }

        /// <summary>
        /// One term
        /// </summary>
        private class Term {
            private Item[] items;       // Terms x and p in the expansion
            private int i1, i2, j1, j2; // Indices in the exponential (E_i1i2 + E_j1j2)
            private bool sign;          // Sign of the term

            public Term(bool sign, int i1, int i2, int j1, int j2, params Item[] items) {
                this.sign = sign;
                this.i1 = i1;
                this.i2 = i2;
                this.j1 = j1;
                this.j2 = j2;
                this.items = items;
            }

            /// <summary>
            /// Sign of the term
            /// </summary>
            public bool Sign { get { return this.sign; } }

            /// <summary>
            /// Duplicating constructor
            /// </summary>
            private Term(Term t) {
                this.sign = t.sign;

                int length = t.items.Length;
                this.items = new Item[length];

                for(int i = 0; i < length; i++)
                    this.items[i] = (Item)t.items[i].Clone();
            }

            public bool IsZero() {
                foreach(Item item in this.items)
                    if(item.IsZero())
                        return true;
                return false;
            }

            private void Change(Pair p) {
                foreach(Item item in this.items)
                    item.Change(p);
            }

            public override string ToString() {
                StringBuilder s = new StringBuilder();
                foreach(Item item in this.items)
                    s.Append(item.ToString());
                return s.ToString();
            }

            /// <summary>
            /// Application of delta functions
            /// </summary>
            private Term Delta(Quadruple q) {
                Term result = new Term(this);

                for(int i = 0; i < 4; i++)
                    result.Change(q[i]);

                return result;
            }

            /// <summary>
            /// Multiplication of two terms, asymptotic limit
            /// </summary>
            public static string operator *(Term t1, Term t2) {
                Quadruple[] ps = GetPermutations(t1, t2);
                StringBuilder s = new StringBuilder();

                // Original term
                s.Append(string.Format("{0}{1} {2}{3} {4}{5} {6}{7}", t1.i1, t1.i2, t1.j1, t1.j2, t2.i1, t2.i2, t2.j1, t2.j2));
                s.Append(Environment.NewLine);
                s.Append(t1.sign ^ t2.sign ? "-" : "+");
                s.Append(t1.ToString());
                s.Append(t2.ToString());
                s.Append(Environment.NewLine);

                for(int i = 0; i < ps.Length; i++) {
                    Term t1x = t1.Delta(ps[i]);
                    Term t2x = t2.Delta(ps[i]);

                    if(!t1x.IsZero() && !t2x.IsZero()) {
                        s.Append(ps[i].ToString());
                        s.Append(": ");
                        s.Append(t1x.sign ^ t2x.sign ? "-" : "+");
                        s.Append(t1x.ToString());
                        s.Append(t2x.ToString());
                        s.Append(Environment.NewLine);
                    }
                    else if(true) {
                        s.Append("   ");
                        s.Append(ps[i].ToString());
                        s.Append(": ");
                        s.Append(t1x.sign ^ t2x.sign ? "-" : "+");
                        s.Append(t1x.ToString());
                        s.Append(t2x.ToString());
                        s.Append(Environment.NewLine);
                    }
                }

                return s.ToString();
            }

            private static Quadruple[] GetPermutations(Term t1, Term t2) {
                Quadruple[] ps = new Quadruple[24];       // Permutations

                int[] i1s = new int[4];
                i1s[0] = t1.i1;
                i1s[1] = t1.j1;
                i1s[2] = t2.i1;
                i1s[3] = t2.j1;

                int[] i2s = new int[4];
                i2s[0] = t1.i2;
                i2s[1] = t1.j2;
                i2s[2] = t2.i2;
                i2s[3] = t2.j2;

                // Steinhaus–Johnson–Trotter algorithm for permutations
                int k = 0; int s = 0;
                for(int i = 0; i < 3; i++) {
                    for(int j = 3; j > 0; j--) {
                        // Swap
                        s = i2s[j]; i2s[j] = i2s[j - 1]; i2s[j - 1] = s;
                        ps[k++] = GetQuadruple(i1s, i2s);
                    }

                    // Swap
                    s = i2s[2]; i2s[2] = i2s[3]; i2s[3] = s;
                    ps[k++] = GetQuadruple(i1s, i2s);

                    for(int j = 0; j < 3; j++) {
                        // Swap
                        s = i2s[j]; i2s[j] = i2s[j + 1]; i2s[j + 1] = s;
                        ps[k++] = GetQuadruple(i1s, i2s);
                    }

                    // Swap
                    s = i2s[0]; i2s[0] = i2s[1]; i2s[1] = s;
                    ps[k++] = GetQuadruple(i1s, i2s);
                }
                return ps;
            }

            private static Quadruple GetQuadruple(int[] i1s, int[] i2s) {
                Pair[] ps = new Pair[4];
                for(int i = 0; i < 4; i++)
                    ps[i] = new Pair(i1s[i], i2s[i]);
                return new Quadruple(ps);
            }
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            // Old wrong version (with sumation over r)
            /*
            Term[] ts1 = new Term[4];
            ts1[0] = new Term(true, 0, 1, 3, 2, new Item(0, 1, true), new Item(3, 2, true), new Item(1, 3, false), new Item(2, 4, false));
            ts1[1] = new Term(false, 1, 3, 3, 2, new Item(1, 3, true), new Item(3, 2, true), new Item(0, 1, false), new Item(2, 4, false));
            ts1[2] = new Term(false, 0, 1, 2, 4, new Item(0, 1, true), new Item(2, 4, true), new Item(1, 3, false), new Item(3, 2, false));
            ts1[3] = new Term(true, 1, 3, 2, 4, new Item(1, 3, true), new Item(2, 4, true), new Item(0, 1, false), new Item(3, 2, false));

            Term[] ts2 = new Term[4];
            ts2[0] = new Term(true, 4, 5, 7, 6, new Item(4, 5, true), new Item(7, 6, true), new Item(5, 7, false), new Item(6, 0, false));
            ts2[1] = new Term(false, 5, 7, 7, 6, new Item(5, 7, true), new Item(7, 6, true), new Item(4, 5, false), new Item(6, 0, false));
            ts2[2] = new Term(false, 4, 5, 6, 0, new Item(4, 5, true), new Item(6, 0, true), new Item(5, 7, false), new Item(7, 6, false));
            ts2[3] = new Term(true, 5, 7, 6, 0, new Item(5, 7, true), new Item(6, 0, true), new Item(4, 5, false), new Item(7, 6, false));
            */

            Term[] ts1 = new Term[4];
            ts1[0] = new Term(true, 0, 1, 3, 2, new Item(0, 1, true), new Item(3, 2, true), new Item(1, 3, false), new Item(2, 0, false));
            ts1[1] = new Term(false, 1, 3, 3, 2, new Item(1, 3, true), new Item(3, 2, true), new Item(0, 1, false), new Item(2, 0, false));
            ts1[2] = new Term(false, 0, 1, 2, 0, new Item(0, 1, true), new Item(2, 0, true), new Item(1, 3, false), new Item(3, 2, false));
            ts1[3] = new Term(true, 1, 3, 2, 0, new Item(1, 3, true), new Item(2, 0, true), new Item(0, 1, false), new Item(3, 2, false));

            Term[] ts2 = new Term[4];
            ts2[0] = new Term(true, 0, 5, 7, 6, new Item(0, 5, true), new Item(7, 6, true), new Item(5, 7, false), new Item(6, 0, false));
            ts2[1] = new Term(false, 5, 7, 7, 6, new Item(5, 7, true), new Item(7, 6, true), new Item(0, 5, false), new Item(6, 0, false));
            ts2[2] = new Term(false, 0, 5, 6, 0, new Item(0, 5, true), new Item(6, 0, true), new Item(5, 7, false), new Item(7, 6, false));
            ts2[3] = new Term(true, 5, 7, 6, 0, new Item(5, 7, true), new Item(6, 0, true), new Item(0, 5, false), new Item(7, 6, false));

            for(int i = 0; i < 4; i++)
                for(int j = 0; j < 4; j++) {
                    guider.WriteLine(string.Format("({0},{1})", i, j));
                    guider.WriteLine(ts1[i] * ts2[j]);
                }

            return null;
        }
    }
}