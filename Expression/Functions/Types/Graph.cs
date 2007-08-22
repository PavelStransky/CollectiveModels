using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Vytvoøí graf
	/// </summary>
	/// <param name="args">Argumenty funkce
	/// 0 ... data k vykreslení (vektor, øada, øada øad)
    /// 1 ... pozadí
	/// 2 ... parametry (string)
	/// 3 ... parametry jednotlivých køivek (array of string)
    /// 4 ... parametry pozadí
	/// 5 ... chyby ke køivkám
    /// Možnosti zadání (data, pozadí):
    /// (1, 1)
    /// (Array(N), 1) - k jednomu pozadí více køivek najednou
    /// (Array(N, 1), 1) - k jednomu pozadí více køivek, ale postupnì (animace)
    /// (Array(N), Array(N)) - jedna køivka k jednomu pozadí
    /// (Array(N, M), Array(N)) - M køivek ke každému pozadí
	/// </param>
	public class FnGraph: Fnc {
        public override string Name { get { return name; } }
		public override string Help {get {return Messages.HelpGraph;}}

        protected override void CreateParameters() {
            this.SetNumParams(6);

            this.SetParam(0, false, true, false, Messages.PGraph1, Messages.P1GraphDescription, null,
                typeof(TArray), typeof(Vector), typeof(PointVector));
            this.SetParam(1, false, true, false, Messages.P2Graph, Messages.P2GraphDescription, null,
                typeof(TArray), typeof(Matrix));
            this.SetParam(2, false, true, false, Messages.P3Graph, Messages.P3GraphDescription, null,
                typeof(TArray), typeof(Vector));
            this.SetParam(3, false, true, false, Messages.P4Graph, Messages.P4GraphDescription, null,
                typeof(string), typeof(Context));
            this.SetParam(4, false, true, false, Messages.P5Graph, Messages.P5GraphDescription, null,
                typeof(TArray), typeof(string), typeof(Context));
            this.SetParam(5, false, true, false, Messages.P6Graph, Messages.P6GraphDescription, null,
                typeof(TArray), typeof(string), typeof(Context));
        }

		protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            // Druhý parametr - pozadí
            object item = arguments.Count > 1 ? arguments[1] : null;
            int groupsB = 0;

            TArray background = null;
            
            bool oneBackground = true; // True, pokud máme pouze jedno pozadí (rozkopírovává se)

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

            // První parametr - data
            item = arguments[0];
            int groupsI = 0;

            TArray data = null;

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

            // Doplnìní správného poètu pozadí
            Matrix b = oneBackground ? background[0] as Matrix : new Matrix(0);
            TArray back = background;
            background = new TArray(typeof(Matrix), groupsI);
            for(int g = 0; g < groupsB; g++)
                background[g] = back[g];
            for(int g = groupsB; g < groupsI; g++)
                background[g] = b;

            groupsB = groupsI;

            // Tøetí parametr - chybové úseèky
            item = arguments.Count > 2 ? arguments[2] : null;
            TArray errors = null;

            if(item != null) {
                // 1
                if(item is Vector) {
                    errors = new TArray(typeof(TArray), groupsI);
                    Vector v = item as Vector;
        
                    for(int g = 0; g < groupsI; g++)
                    {
                        int length = (data[g] as TArray).Length;
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
                        errors = new TArray(typeof(TArray), groupsI);
            
                        for(int i = 0; i < groupsI; i++)
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

            // 4. parametr - vlastnosti grafu (string nebo Context)
            item = arguments.Count > 3 ? arguments[3] : null;
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

			// 5. parametr - vlastnosti jednotlivých skupin (pozadí) grafu
            // (string nebo Context nebo Array of Context)
            item = arguments.Count > 4 ? arguments[4] : null;
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
                    groupParams = new TArray(typeof(Context), groupsI);
                    for(int g = 0; g < groupsI; g++)
                        groupParams[g] = c;
                }

                else {
                    TArray itemt = (item as TArray).Deflate();
                    Type t = itemt.GetItemType();

                    if(t == typeof(string)) {
                        int groups = itemt.Length;
                        groupParams = new TArray(typeof(Context), groups);

                        for(int g = 0; g < groups; g++) {
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

            // 6. parametr - vlastnosti jednotlivých køivek grafu
            item = arguments.Count > 5 ? arguments[5] : null;
            TArray itemParams = null;

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

                    for(int g = 0; g < groupsI; g++) {
                        int length = (data[g] as TArray).Length;
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

                        itemParams = new TArray(typeof(TArray), groupsI);
                        for(int g = 0; g < groupsI; g++)
                            itemParams[g] = ct;
                    }

                    // (N; M)
                    else if(t == typeof(TArray)) {
                        t = (itemt[0] as TArray).GetItemType();
                        int groups = itemt.Length;

                        if(t != typeof(string) && t != typeof(Context))
                            this.BadTypeError(item, 5);

                        if(t == typeof(string)) {
                            itemParams = new TArray(typeof(TArray), groups);

                            for(int g = 0; g < groups; g++) {
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

            return new Graph(data, background, errors, graphParams, groupParams, itemParams);
		}

        private const string name = "graph";
	}
}
