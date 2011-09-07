using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

using System.Xml;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.Systems;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Test {
	/// <summary>
	/// Summary description for Class Test.
	/// </summary>
	class Test {
		private const string root = @"c:\gcm\";

		[STAThread]
		static void Main(string[] args) {
            Test.HOMatrixElements();

            Console.Write("Hotovo.");
			Console.ReadLine();
		}

        /// <summary>
        /// Maticové elementy Harmonického oscilátoru
        /// </summary>
        static void HOMatrixElements() {
            HOMatrixElement ho = new HOMatrixElement(1);
            int n1 = 1, n2 = 2;
            int m1 = 0, m2 = 0;
            int a = 1, b = 0;

            Console.WriteLine("(n1, n2, m1, m2, a, b) = ({0}, {1}, {2}, {3}, {4}, {5})", n1, n2, m1, m2, a, b);
            Console.WriteLine(ho.HO2D(n1, n2, m1, m2, a, b));
        }

        /// <summary>
        /// Data for Emmanuel 10.6.2011
        /// </summary>
        static void EmmanuelData() {
            FileStream f = new FileStream("c:\\data.txt", FileMode.Open);
            StreamReader s = new StreamReader(f);

            FileStream g = new FileStream("c:\\out.txt", FileMode.Create);
            StreamWriter t = new StreamWriter(g);

            string line = string.Empty;
            while((line = s.ReadLine()) != null) {
                line = line.ToLower().Replace("&#183;", ".");
                int fonti = -1;
                while((fonti = line.IndexOf("<font")) >= 0) {
                    line = line.Substring(0, fonti) + " " 
                        + line.Substring(line.IndexOf(">", line.IndexOf("</font")) + 1);
                }
                if(line != "")
                    t.WriteLine(line.Trim().Replace("  ", " "));
            }

            t.Close();
            g.Close();

            s.Close();
            f.Close();
        }

        static void PokusSturmCoulomb() {
            SturmCoulomb s = new SturmCoulomb(0);
//            Console.WriteLine(s.B(2, 2, 2, 2, 2));
//            Console.WriteLine(s.W(1, 0));
        }

        /// <summary>
        /// 29.3.2011
        /// </summary>
        static void PokusMaxima() {
            int length = 20;
            PointVector p = new PointVector(length);
            Random r = new Random();

            for(int i = 0; i < length; i++)
                p[i] = new PointD(i, r.Next(-1, 1));

            PointVector maxima = p.Maxima();
        }

        /// <summary>
        /// 10.2.2011
        /// </summary>
        static void PokusARPack() {
            SparseMatrix sp = SparseMatrix.ReadExample("c:\\matrix.txt");
            Matrix m = (Matrix)sp;
            SparseMatrix spc = new SparseMatrix(m.Length);
            for(int i = 0; i < sp.Length; i++)
                for(int j = 0; j < sp.Length; j++)
                    spc[i, j] = m[i, j];
            Vector v = ARPackDLL.dsaupd(sp, 10, false, false, null)[0];
            Vector vm = LAPackDLL.dsyev(m, false)[0];
            Vector vc = ARPackDLL.dsaupd(spc, 10, false, false, null)[0];
            Console.WriteLine(v.ToString());
            Console.WriteLine(vm.ToString());
            Console.WriteLine(vc.ToString());
        }

        /// <summary>
        /// 5.3.2009
        /// </summary>
        static void PokusXML() {
            XmlDocument doc = new XmlDocument();

            XmlElement lesson = doc.CreateElement("Lesson");
            XmlAttribute attless = doc.CreateAttribute("Number");
            attless.Value = "1";

            lesson.SetAttributeNode(attless);

            XmlElement e = doc.CreateElement("Expression");
            e.InnerText = "Hallo";
            lesson.AppendChild(e);

            e = doc.CreateElement("Meaning");
            e.InnerText = "Nazdar";
            lesson.AppendChild(e);

            doc.AppendChild(lesson);

            doc.Save("c:\\tmp.xml");
        }

        static void PokusMinimumBrody() {
            double result = 0.0;
            double num = 10;

            for(int j = 0; j < num; j++) {
                int length = 1000;
                Vector brodyv = new Vector(length);
                for(int i = 0; i < length; i++)
                    brodyv[i] = RMTDistribution.GetBrody(0.2);

                PointVector cb = brodyv.CumulativeHistogram();
                for(int i = 0; i < length; i++)
                    cb[i].Y /= length;

                BrodyFit brf = new BrodyFit(cb);
                double br = brf.Fit(0.0001);
                Console.WriteLine("Brody = {0}", br);
                result += br;
            }

            Console.WriteLine("Mean = {0}", result / num);
        }

        /// <summary>
        /// Pokus na minimum, 14.10.2008
        /// </summary>
        static void PokusMinimum() {
            Bisection b = new Bisection(PokusMinimumFnc);
            Console.WriteLine("Minimum = {0}", b.Minimum(-5, 5, 0.001));
        }

        static double PokusMinimumFnc(double x) {
            return (x - 2) * (x - 3);
        }

        /// <summary>
        /// Pokus na dlouhá èísla
        /// </summary>
        static void PokusLongNumber() {
            LongNumber result = new LongNumber(1);

            // Dlouhý faktoriál
            for(int i = 1; i < 100; i++) {
                result *= new LongNumber(i);
            }

            Console.WriteLine(result.ToString());
        }

        static void RescueMC() {
            RescueMemoryCard rc = new RescueMemoryCard();
            rc.FindFileDates();
        }

        /// <summary>
        /// Pokus jen na zpoždìný výstup na konzoli 20.4.2007
        /// </summary>
        static void PokusDelay() {
            double x = 0.0;
            for(int i = 0; i < 100; i++) {
                for(int j = 0; j < 10000000; j++)
                    x += j;
                Console.WriteLine(i);
            }

            for(int i = 0; i < 100; i++) {
                for(int j = 0; j < 10000000; j++)
                    x += j;
                Console.Write(i);
                Console.Write(' ');
            }
        }

        /// <summary>
        /// Pokus na novou implementaci øad (28.3.2007)
        /// </summary>
        static void PokusTArray() {
            TArray ta = new TArray(typeof(double), 20);

            ta[5] = 4;
            ta[3] = 11;

            object b = ta[4];
            b = 4;
        }

        /// <summary>
        /// Pokus na Laguerrovy polynomy vysokého øádu
        /// </summary>
        static void PokusLaguerre() {
            double l = 0.0;
            double e = 0.0;
            SpecialFunctions.Laguerre(out l, out e, 3, 5, 3.0);
            double l1 = l * System.Math.Exp(e);
            double l2 = SpecialFunctions.Laguerre(3.0, 3, 5);
        }

        /// <summary>
        /// Tøetí pokus
        /// </summary>
        static void PokusLAPack3() {
            int length = 10000;
            Matrix m1 = new Matrix(length);
            SymmetricBandMatrix m2 = new SymmetricBandMatrix(length, 3);

            Random r = new Random(1000);

            for(int i = 0; i < 3; i++)
                for(int j = 0; j < length - i; j++) {
                    double d = r.NextDouble();
                    m1[i + j, j] = d;
                    m1[j, i + j] = d;
                    m2[i + j, j] = d;
                }

//            Jacobi jacobi = new Jacobi(m1);
//            jacobi.SortAsc();
//            Vector v1 = new Vector(jacobi.EigenValue);
            Vector v2 = LAPackDLL.dsbevx(m2, false, 0, 100)[0];
//            Vector v3 = v2 - v1;
        }

        /// <summary>
        /// 19.1.2006
        /// </summary>
        static void PokusLAPack1() {
            double[,] d = { { 1, 3, 2, 1, 1 }, 
                            { 3, -1, 5, 0, 0 },
                            {2,5,0,4,3},
                            {1,0,4,-1,3},
                            {1,0,3,3,0}};
            Matrix m = new Matrix(d);

            int length = 100;
            Random r = new Random();
            m = new Matrix(length);
            for(int i = 0; i < length; i++)
                for(int j = 0; j < length; j++)
                    m[i, j] = r.NextDouble();

//            Console.WriteLine(m.ToString());

            Vector v1 = PavelStransky.DLLWrapper.LAPackDLL.dsyev(m, false)[0];
            Jacobi jacobi = new Jacobi(m);
            jacobi.SortAsc();
            Vector v2 = new Vector(jacobi.EigenValue);
        }

        /// <summary>
        /// 22.12.2006
        /// </summary>
        static void PokusFactorialI() {
            double f = 1;
            for(int i = 1; i < 1000; i++) {
                f *= i;
                Console.WriteLine("{0} {1}", i, f);
                if(double.IsInfinity(f))
                    break;
            }
        }

        /// <summary>
        /// 21.12.2006
        /// </summary>
        static void PokusNumberPrecision() {
            Console.WriteLine("Double");
            Console.WriteLine(sizeof(double));
            Console.WriteLine(double.Epsilon);
            Console.WriteLine(double.MinValue);
            Console.WriteLine(double.MaxValue);

            Console.WriteLine("Decimal");
            Console.WriteLine(sizeof(decimal));
            Console.WriteLine(decimal.MinValue);
            Console.WriteLine(decimal.MaxValue);

            Console.WriteLine("Long");
            Console.WriteLine(sizeof(long));
            Console.WriteLine(long.MinValue);
            Console.WriteLine(long.MaxValue);

            Console.WriteLine("Unsigned Long");
            Console.WriteLine(sizeof(ulong));
            Console.WriteLine(ulong.MinValue);
            Console.WriteLine(ulong.MaxValue);
        }

        /// <summary>
        /// 19.12.2006
        /// </summary>
        public static void PokusLHOQuantumGCMRHladiny() {
            for(int emax = 200; emax <= 200; emax++) {
                int count = 0;
                Console.Write("{0}\t", emax);
                for(int n = 0; n <= (emax - 1) / 2; n++) {
                    if(n != 0) {
                        Console.WriteLine();
                        Console.Write("  \t");
                    }
                    Console.Write("{0}\t", n);
                    for(int m = 0; m <= emax - 1 - 2 * n; m += 3) {
                        Console.Write("{0} ", m);
                        count++;
                    }
                }
                Console.WriteLine();
                Console.WriteLine(count);
                Console.WriteLine();
                Console.ReadLine();
            }
        }

        /// <summary>
        /// QuantumGCM, 10.10.2006
        /// </summary>
        public static void PokusQuantumGCM1() {
            QuantumGCM q = new QuantumGCM(-1, 1, 1, 1);
            Console.WriteLine(q.ToString());

            Vector v = q.EnergyLevels(0, 30);
            Console.WriteLine(v.ToString());
        }

        /// <summary>
        /// Pokus na SALIContourGraph, 9.10.2006
        /// </summary>
        public static void PokusSaliContourGraph() {
            ClassicalIBM ibm = new ClassicalIBM(0.7, -0.3);
            SALIContourGraph sali = new SALIContourGraph(ibm, 0, RungeKuttaMethods.Normal);
            Matrix m = sali.Compute(0, 100, 100)[0] as Matrix;
        }

        /// <summary>
        /// Pokus na rozšíøený klasický GCM
        /// </summary>
        public static void PokusClassicalGCMJ() {
            ClassicalGCMJ c = new ClassicalGCMJ(-1, 1, 1, 1);
            Vector x = new Vector(5);

//            Vector v = c.Roots(-0.1, 1, x);
            Vector ic = c.IC(-0.1, 0.1);

            double b = c.E(ic);
            double j = c.J(ic).EuklideanNorm();

            Trajectory t = new Trajectory(c, 1E-10, RungeKuttaMethods.Adapted);
            Matrix result = t.Compute(ic, 10, 1);
            Vector r = result.GetRowVector(5);
            Vector r1 = new Vector(r.Length - 1);
            for(int i = 0; i < r1.Length; i++)
                r1[i] = r[i + 1];

            b = c.E(r1);
            j = c.J(r1).EuklideanNorm();

            ClassicalGCM c1 = new ClassicalGCM(-1, 1, 1, 1);
            Vector v1 = c1.Roots(-0.1, System.Math.PI / 2.0);
        }

        /// <summary>
        /// Test na kvantové GCM, 13.6.2006
        /// </summary>
        public static void PokusQuantumGCM() {
            QuantumGCM gcm = new QuantumGCM(-1, 1, 1, 1);
            gcm.Test();
            gcm.Calculate();
        }

        /// <summary>
        /// Test na výpoèet radiálních maticových elementù, 2.5.2006
        /// </summary>
        public static void PokusRadialMatrixElement() {
            RadialMatrixElement r = new RadialMatrixElement(0, 5);
        }

        /// <summary>
        /// Test na výpoèet hranice dostupné oblasti (GCM kontura)
        /// </summary>
        private static void PokusGCMContour() {
            ClassicalGCM gcm = new ClassicalGCM(-1, 1, 1, 1);
            PointVector[] p = gcm.EquipotentialContours(-0.1, 0, 0);

            FileStream f = new FileStream("c:\\tmp\\gcm.txt", FileMode.Create);
            StreamWriter t = new StreamWriter(f);

            bool next = true;
            int i = 0;

            while(next) {
                next = false;

                for(int j = 0; j < p.Length; j++) {
                    if(p[j].Length <= i)
                        t.Write("\t\t");
                    else {
                        t.Write("{0}\t{1}\t", p[j][i].X, p[j][i].Y);
                        next = true;
                    }
                }

                t.WriteLine();
                i += 1;
            }

            t.Close();
            f.Close();
        }

        /// <summary>
        /// Test, zda je rychlejší pøistupovat do ArrayList na první nebo na poslední prvek
        /// Rozdíl je zadnedbatelný :-)
        /// </summary>
        private static void PokusArrayList() {
            ArrayList array=  new ArrayList();
            int length = 10000000;

            for(int i = 0; i < length; i++) {
                array.Add(i);
            }

            Console.WriteLine("Hotova generace");

            for(int i = 0; i < 100 * length; i++)
                array[0] = i;

            Console.WriteLine("Hotov výpoèet");
        }

    	/// <summary>
		/// Pokus na spline, 11.09.2005
		/// </summary>
		private static void PokusSpline() {
			const int length = 50;
			const int resultLength = 2514;

			Random r = new Random();
			PointVector pv = new PointVector(length);
			PointVector result;

			for(int i = 0; i < length; i++) {
				pv[i].X = 30 * r.NextDouble();
				pv[i].Y = 2 * r.NextDouble();
			}

			pv = pv.SortX();
            Export export = new Export(root + "spline1.txt", IETypes.Binary);
            export.Write(pv);
            export.Close();

			Spline s = new Spline(pv);
			result = s.GetPointVector(resultLength);
			export = new Export(root + "spline2.txt", IETypes.Binary);
            export.Write(result);
            export.Close();
		}

		/// <summary>
		/// Pokus na FFT, 11.09.2005
		/// </summary>
		private static void PokusFFT() {
			const int length = 1024;

			NormalDistribution normal = new NormalDistribution();
			Vector v = normal.GetVector(length);

			for(int i = 0; i < length; i++) {
				v[i] += System.Math.Sin(i / 2.2);
			}

			PointVector pv = FFT.PowerSpectrum(FFT.Compute(v), 0.005);

            Export export = new Export(root + "fft.txt", IETypes.Binary);
            export.Write(pv);
            export.Close();
		}

		/// <summary>
		/// Pokus na Context, 14.09.2005
		/// </summary>
		private static void PokusContext() {
			Context context = new Context();

			Variable v = context.SetVariable("Ahoj", new Vector(10));
			try {
				((context["Ahoj"] as Variable).Item as Vector)[2] = 4;
				Vector x = (context["Hallo"] as Variable).Item as Vector;
			}
			catch (ContextException e) {
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				Console.WriteLine();
			}

/*			TArray array = new TArray();
			try {
				array.Add(new Vector(10));
				array.Add(new Vector(2));
				array.Add(new Matrix(2, 3));
			}
			catch (ContextException e) {
				Console.WriteLine(e.Message);
				Console.WriteLine(e.DetailMessage);
				Console.WriteLine();
			}
*/		}

		/// <summary>
		/// Pokus na regulární výrazy kvùli parseru výrazù pro výpoèty, 14.09.2005
		/// </summary>
		private static void PokusRegularniVyraz() {
			string text = "a + (b * c - (d - 12 * a)) - 12 * (a + b - c) * (d * 2)";
			string pattern = @"\(.*\)";
			MatchCollection matches = Regex.Matches(text, pattern, RegexOptions.ExplicitCapture);
			int i = matches.Count;
		}

		/// <summary>
		/// Pokus na výrazy pro výpoèty, 14.09.2005
		/// </summary>
		private static void PokusExpression() {
			string text = "z = a + (b * c - (d - 12 * a) * a) * b - (12 * (a + b - c) * (d * 2)) * c;";
//			string text = "z = 2 * itabs(itabs(d)*(0-2))";
//			string text = "z = item(item(array(a; b; c; d);2);1) * c";
//			string text = "z = array(a;b;c;d)";
			Context context = new Context();
			Vector a = new Vector(3); a[0] = 1; a[1] = 0; a[2] = -1;
			Vector b = new Vector(3); b[0] = 0; b[1] = 0; b[2] = 2;
			Vector c = new Vector(3); c[0] = 2; c[1] = -1; c[2] = 3;
			Vector d = new Vector(3); d[0] = -2; d[1] = 4; d[2] = 0;
			Matrix m = new Matrix(5, 9);
			for(int i = 0; i < m.LengthX; i++)
				for(int j = 0; j < m.LengthY; j++)
					m[i, j] = i * 10 + j + 0.1;

			context.SetVariable("a", a);
			context.SetVariable("b", b);
			context.SetVariable("c", c);
			context.SetVariable("d", d);
			context.SetVariable("m", m);

			try {
				Expression.Expression exp = new Expression.Expression(text);
				exp.Evaluate(context);
				Vector v = (context["z"] as Variable).Item as Vector;
//				double x = (v == null) ? (double)(exp.Evaluate() as Variable).Item : 0.0;
				Console.WriteLine(v);
//				Console.WriteLine(x);
                Export export = new Export("c:\\eeg\\context.txt", IETypes.Binary);
                export.Write(context);
                export.Close();
				exp = new PavelStransky.Expression.Expression("save(\"c:\\eeg\\prdlacka.txt\")");
				exp.Evaluate(context);
			}
			catch(ExpressionException e) {
				Console.WriteLine(e.Message);
				Console.WriteLine(e.DetailMessage);
				Console.WriteLine(e.StackTrace);
				Console.WriteLine();
			}
			catch(Exception e) {
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				Console.WriteLine();
			}
		}


		/// <summary>
		/// Pokus na import a export kontextu, 15.09.2005
		/// </summary>
		private static void PokusImportExport() {
			Context context = new Context();
            Expression.Import import = new PavelStransky.Expression.Import("c:\\eeg\\context.txt");
            context = import.Read() as Context;
            import.Close();

            Export export = new Export("c:\\eeg\\context1.txt", IETypes.Binary);
            export.Write(context);
            export.Close();
		}

		/// <summary>
		/// Pokus na import a export dat, 15.09.2005
		/// </summary>
		private static void PokusImportEEG() {
			Context context = new Context();
            Expression.Import import = new PavelStransky.Expression.Import("c:\\eeg\\eeg.txt");
            context = import.Read() as Context;
            import.Close();

			string text = "z = splitcolumns(m)";
			Expression.Expression exp = new Expression.Expression(text);
			exp.Evaluate(context);

            Export export = new Export("c:\\eeg\\context1.txt", IETypes.Binary);
            export.Write(context);
            export.Close();
		}	

		/// <summary>
		/// Pøevede soubor s daty na správný formát
		/// </summary>
		/// <param name="fName1">Vstupní soubor</param>
		/// <param name="fName2">Výstupní soubor</param>
		private static void PrevodSoubor(string fName1, string fName2) {
			FileStream f = new FileStream(fName1, FileMode.Open);
			StreamReader t = new StreamReader(f);

			FileStream g = new FileStream(fName2, FileMode.Create);
			StreamWriter w = new StreamWriter(g);

			string line = string.Empty;
			int lines = 0;
			while((line = t.ReadLine()) != null) {
				line = line.Trim();
				while(line.IndexOf("  ") >= 0)
					line = line.Replace("  ", " ");
				line = line.Replace(' ', '\t');
				w.WriteLine(line);
				lines++;
			}

			w.Close();
			g.Close();

			t.Close();
			f.Close();

			Console.WriteLine(lines);
		}
	}
}
