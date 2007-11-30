using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Tøída pøidávající funkce pro práci s grafem
    /// </summary>
    public class FncGraph: Fnc {
        /// <summary>
        /// Pøipraví pozadí - poèet skupin, atd.
        /// </summary>
        /// <param name="groupsB">Poèet pozadí</param>
        /// <param name="oneBackground">True, pokud máme pouze jedno pozadí</param>
        /// <param name="item">Vstup - pozadí</param>
        protected TArray PrepareBackground(out int groupsB, out bool oneBackground, object item) {
            TArray background = null;

            oneBackground = true; // True, pokud máme pouze jedno pozadí (rozkopírovává se)

            if(item != null) {
                // 1
                if(item is Matrix) {
                    background = new TArray(typeof(Matrix), 1);
                    background[0] = item;
                }
                else {
                    background = (item as TArray).Deflate();
                    oneBackground = false;
                }
            }
            else {
                background = new TArray(typeof(Matrix), 1);
                background[0] = new Matrix(0);
            }

            groupsB = background.Length;

            return background;
        }

        /// <summary>
        /// Zpracuje data (køivky)
        /// </summary>
        /// <param name="groupsI">Poèet skupin</param>
        /// <param name="groupsB">Poèet skupin pozadí</param>
        /// <param name="item">Vstup - køivky</param>
        protected TArray ProceedData(out int groupsI, int groupsB, object item) {
            TArray data = null;
            groupsI = 0;

            if(item != null) {
                // 1
                if(item is Vector || item is PointVector) {
                    TArray d = new TArray(typeof(PointVector), 1);
                    if(item is Vector)
                        d[0] = new PointVector(item as Vector);
                    else
                        d[0] = item;

                    groupsI = groupsB;
                    data = new TArray(typeof(TArray), groupsI);
                    for(int i = 0; i < groupsI; i++)
                        data[i] = d;
                }

                // N
                else if(item is TArray) {
                    TArray itemt = (item as TArray).Deflate();
                    Type t = itemt.GetItemType();

                    // (N; 1)
                    if(t == typeof(PointVector) || t == typeof(Vector)) {
                        TArray d = itemt;
                        if(t == typeof(Vector)) {
                            int length = itemt.Length;
                            d = new TArray(typeof(PointVector), length);
                            for(int i = 0; i < length; i++)
                                d[i] = new PointVector(itemt[i] as Vector);
                        }

                        groupsI = groupsB;
                        data = new TArray(typeof(TArray), groupsI);
                        for(int i = 0; i < groupsI; i++)
                            data[i] = d;
                    }

                    // (N; M)
                    else if(t == typeof(TArray)) {
                        t = (itemt[0] as TArray).GetItemType();
                        int groups = itemt.Length;
                        groupsI = System.Math.Max(groupsB, groups);

                        if(t != typeof(Vector) && t != typeof(PointVector))
                            this.BadTypeError(item, 0);

                        data = new TArray(typeof(TArray), groupsI);
                        for(int g = 0; g < groups; g++) {
                            TArray itemt1 = (itemt[g] as TArray).Deflate();
                            if(t == typeof(Vector)) {
                                int length = itemt1.Length;
                                TArray d = new TArray(typeof(PointVector), length);
                                for(int i = 0; i < length; i++)
                                    d[i] = new PointVector(itemt1[i] as Vector);
                                itemt1 = d;
                            }
                            data[g] = itemt1;
                        }

                        // Doplnìní správného poètu dat
                        itemt = new TArray(typeof(PointVector), 1);
                        itemt[0] = new PointVector(0);
                        for(int g = groups; g < groupsI; g++)
                            data[g] = itemt;
                    }
                    else
                        this.BadTypeError(item, 0);
                }
            }
            else {
                groupsI = groupsB;
                data = new TArray(typeof(TArray), groupsI);
                TArray d = new TArray(typeof(PointVector), 1);
                d[0] = new PointVector(0);
                for(int g = 0; g < groupsI; g++)
                    data[g] = d;
            }

            return data;
        }

        /// <summary>
        /// Doplní správný poèet pozadí (podle poètu skupin køivek)
        /// </summary>
        protected TArray FillBackground(bool oneBackground, int groupsB, int groupsI, TArray background) {
            Matrix b = oneBackground ? background[0] as Matrix : new Matrix(0);

            TArray result = new TArray(typeof(Matrix), groupsI);
            for(int g = 0; g < groupsB; g++)
                result[g] = background[g];
            for(int g = groupsB; g < groupsI; g++)
                result[g] = b;

            return result;
        }

        /// <summary>
        /// Vytvoøí pole s poèty køivek v jednotlivých skupinách
        /// </summary>
        /// <param name="data">Zpracovaná data - køivky</param>
        protected int[] NumCurvesFromData(TArray data) {
            int groups = data.Length;
            int[] result = new int[groups];

            for(int g = 0; g < groups; g++)
                result[g] = (data[g] as TArray).Length;

            return result;
        }

        /// <summary>
        /// Zpracuje chyby
        /// </summary>
        /// <param name="groups">Pole s poèty køivek ve skupinách</param>
        /// <param name="item">Vstup - chyby</param>
        protected TArray ProceedErrors(int[] groups, object item) {
            TArray errors = null;

            int nGroups = groups.Length;

            if(item != null) {
                // 1
                if(item is Vector) {
                    errors = new TArray(typeof(TArray), groups);
                    Vector v = item as Vector;

                    for(int g = 0; g < nGroups; g++) {
                        int length = groups[g];
                        TArray e = new TArray(typeof(Vector), length);
                        for(int i = 0; i < length; i++)
                            e[i] = v;
                        errors[g] = e;
                    }
                }

                // N
                else if(item is TArray) {
                    TArray itemt = (item as TArray).Deflate();
                    Type t = itemt.GetItemType();

                    // (N; 1)
                    if(t == typeof(Vector)) {
                        errors = new TArray(typeof(TArray), groups);

                        for(int i = 0; i < nGroups; i++)
                            errors[i] = itemt;
                    }

                    // (N; M)
                    else if(t == typeof(TArray)) {
                        if(t != typeof(Vector))
                            this.BadTypeError(item, 2);

                        errors = item as TArray;
                    }
                    else
                        this.BadTypeError(item, 2);
                }
            }
            else {
                errors = new TArray(typeof(TArray), 1);
                TArray e = new TArray(typeof(Vector), 1);
                e[0] = new Vector(0);
                errors[0] = e;
            }

            return errors;
        }

        /// <summary>
        /// Zpracuje parametry celého grafu
        /// </summary>
        /// <param name="item">Vstup - parametry grafu</param>
        protected Context ProceedGlobalParams(Guider guider, object item) {
            Context graphParams = null;

            if(item != null) {
                if(item is Context)
                    graphParams = item as Context;
                else {
                    graphParams = new Context();
                    string s = item as string;
                    s = s.Trim();
                    if(s != string.Empty)
                        new Expression(s).Evaluate(guider.ChangeContext(graphParams));
                }
            }
            else
                graphParams = new Context();

            return graphParams;
        }

        /// <summary>
        /// Zpracuje parametry skupin
        /// </summary>
        /// <param name="groups">Poèet skupin</param>
        /// <param name="item">Vstup - parametry skupin</param>
        protected TArray ProceedGroupParams(Guider guider, int groups, object item) {
            TArray groupParams = null;

            if(item != null) {
                if(item is string || item is Context) {
                    Context c = null;
                    if(item is Context)
                        c = item as Context;
                    else {
                        c = new Context();
                        string s = item as string;
                        s = s.Trim();
                        if(s != string.Empty)
                            new Expression(s).Evaluate(guider.ChangeContext(c));
                    }
                    groupParams = new TArray(typeof(Context), groups);
                    for(int g = 0; g < groups; g++)
                        groupParams[g] = c;
                }

                else {
                    TArray itemt = (item as TArray).Deflate();
                    Type t = itemt.GetItemType();

                    if(t == typeof(string)) {
                        int gr = itemt.Length;
                        groupParams = new TArray(typeof(Context), gr);

                        for(int g = 0; g < gr; g++) {
                            Context c = new Context();
                            string s = itemt[g] as string;
                            s = s.Trim();
                            if(s != string.Empty)
                                new Expression(s).Evaluate(guider.ChangeContext(c));
                            groupParams[g] = c;
                        }
                    }
                    else
                        groupParams = itemt;
                }
            }
            else {
                groupParams = new TArray(typeof(Context), 1);
                groupParams[0] = new Context();
            }

            return groupParams;
        }

        /// <summary>
        /// Zpracuje parametry jednotlivých køivek grafu
        /// </summary>
        /// <param name="groups">Pole s poèty køivek ve skupinách</param>
        /// <param name="item">Vstup - parametry køivek</param>
        protected TArray ProceedItemParams(Guider guider, int[] groups, object item) {
            TArray itemParams = null;

            int nGroups = groups.Length;

            if(item != null) {
                // 1
                if(item is string || item is Context) {
                    Context c = null;
                    if(item is Context)
                        c = item as Context;
                    else {
                        c = new Context();
                        string s = item as string;
                        s = s.Trim();
                        if(s != string.Empty)
                            new Expression(s).Evaluate(guider.ChangeContext(c));
                    }

                    for(int g = 0; g < nGroups; g++) {
                        int length = groups[g];
                        TArray ct = new TArray(typeof(Context), length);
                        for(int i = 0; i < length; i++)
                            ct[i] = c;
                        itemParams[g] = c;
                    }
                }

                // N
                else if(item is TArray) {
                    TArray itemt = (item as TArray).Deflate();
                    Type t = itemt.GetItemType();

                    // (N; 1)
                    if(t == typeof(string) || t == typeof(Context)) {
                        TArray ct = itemt;
                        if(t == typeof(string)) {
                            int length = itemt.Length;
                            ct = new TArray(typeof(Context), length);
                            for(int i = 0; i < length; i++) {
                                Context c = new Context();
                                string s = itemt[i] as string;
                                s = s.Trim();
                                if(s != string.Empty)
                                    new Expression(s).Evaluate(guider.ChangeContext(c));
                                ct[i] = c;
                            }
                        }

                        itemParams = new TArray(typeof(TArray), groups);
                        for(int g = 0; g < nGroups; g++)
                            itemParams[g] = ct;
                    }

                    // (N; M)
                    else if(t == typeof(TArray)) {
                        t = (itemt[0] as TArray).GetItemType();
                        int gr = itemt.Length;

                        if(t != typeof(string) && t != typeof(Context))
                            this.BadTypeError(item, 5);

                        if(t == typeof(string)) {
                            itemParams = new TArray(typeof(TArray), gr);

                            for(int g = 0; g < gr; g++) {
                                TArray itemt1 = (itemt[g] as TArray).Deflate();
                                int length = itemt1.Length;
                                TArray ct = new TArray(typeof(Context), length);
                                for(int i = 0; i < length; i++) {
                                    Context c = new Context();
                                    string s = itemt1[i] as string;
                                    s = s.Trim();
                                    if(s != string.Empty)
                                        new Expression(s).Evaluate(guider.ChangeContext(c));
                                    ct[i] = c;
                                }

                                itemParams[g] = ct;
                            }
                        }
                        else
                            itemParams = itemt;
                    }
                    else
                        this.BadTypeError(item, 6);
                }
            }
            else {
                itemParams = new TArray(typeof(TArray), 1);
                TArray ct = new TArray(typeof(Context), 1);
                ct[0] = new Context();
                itemParams[0] = ct;
            }

            return itemParams;
        }
    }
}
