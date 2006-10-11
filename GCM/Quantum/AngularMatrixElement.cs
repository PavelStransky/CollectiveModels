using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.GCM {
    /// <summary>
    /// Naète úhlové maticové elementy ze souboru (èasem bude možná i poèítat :-)
    /// </summary>
    using M = System.Math;
    public class AngularMatrixElement {
        // Potenciální maticové elementy èásti závislé na gamma a Eulerových úhlech
        // (úhlové maticové elementy mezi stavy i1, i2 pro spin i4 a operátor ve tvaru cos(3 * gamma) ^ i3)
        private double[, , ,] rws = new double[301, 301, 3, 11];

        /// <summary>
        /// Konstruktor
        /// </summary>
        public AngularMatrixElement() {
            ClebshGordon cg = new ClebshGordon();
            for(int l1 = 0; l1 < 301; l1 += 3)
                for(int l2 = l1; l2 < 301; l2 += 3) {

                    int j1 = l1 / 3;
                    int j2 = l2 / 3;


                    double A = M.Sqrt((2 * j1 + 1) * (2 * j2 + 1));

                    rws[l1, l2, 0, 0] = l1 == l2 ? 1 : 0;
                    rws[l1, l2, 1, 0] = A * M.Pow(cg.Six_j(j1, j2, 1, 0, 0, 0), 2);
                    rws[l1, l2, 2, 0] = (A * M.Pow(cg.Six_j(j1, j2, 2, 0, 0, 0), 2) * 2 + 1) / 3;
                    rws[l2, l1, 1, 0] = rws[l1, l2, 1, 0];
                    rws[l2, l1, 2, 0] = rws[l1, l2, 2, 0];

                }

            /*                FileStream f = new FileStream(defaultFileName, FileMode.Open);
                            StreamReader s = new StreamReader(f);

                            // i - index - angular momentum
                            for (int i = 0; i <= 10; i++)
                            {

                                // Úhlový moment nemùže být 1
                                if (i == 1)
                                    continue;

                                // j - index exponentu èlenu cos(3 * gamma)
                                for (int j = 0; j < 3; j++)
                                {
                                    int i1 = 1;
                                    int i2;
                                    double val;

                                    // Když skonèí hodnoty pro dané i, j, jsou všechny hodnoty 0
                                    while (i1 > 0)
                                    {
                                        string[] items = s.ReadLine().Split(' ');
                                        ArrayList numbers = new ArrayList();

                                        // Odstranìní pouhých mezer
                                        for (int k = 0; k < items.Length; k++)
                                            if (items[k] != null && items[k] != string.Empty)
                                                numbers.Add(items[k]);

                                        // Data jsou v souboru ve 3 sloupcích
                                        for (int k = 0; k < 9; k += 3)
                                        {
                                            i1 = int.Parse(numbers[k] as string);
                                            i2 = int.Parse(numbers[k + 1] as string);
                                            val = double.Parse(numbers[k + 2] as string);

                                            if (j < 3 && i1 > 0 && i2 > 0)
                                            {
                                                this.rws[i1 - 1, i2 - 1, j, i] = val;
                                                this.rws[i2 - 1, i1 - 1, j, i] = val;
                                            }
                                        }
                                    }
                                }
                            }
                            s.Close();
                            f.Close();*/
        }

        /// <summary>
        /// Indexer
        /// </summary>
        public double this[int i1, int i2, int mue, int l] { get { return this.rws[i1, i2, mue, l]; } }

        //        private string defaultFileName = @"c:\diplomka\fortran\gcm\angp.dat";
        //        private int maxL = 8;
    }
}
