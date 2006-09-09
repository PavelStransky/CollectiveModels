using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.GCM {
    /// <summary>
    /// Naète úhlové maticové elementy ze souboru (èasem bude možná i poèítat :-)
    /// </summary>
    public class AngularMatrixElement {
        // Potenciální maticové elementy èásti závislé na gamma a Eulerových úhlech
        // (úhlové maticové elementy mezi stavy i1, i2 pro spin i4 a operátor ve tvaru cos(3 * gamma) ^ i3)
        private double[, , ,] rws = new double[90, 90, 2, 10];

        /// <summary>
        /// Konstruktor
        /// </summary>
        public AngularMatrixElement() {
            FileStream f = new FileStream(defaultFileName, FileMode.Open);
            StreamReader s = new StreamReader(f);

            // i - index - angular momentum
            for(int i = 0; i < 6; i++) {

                // Úhlový moment nemùže být 1
                if(i == 1)
                    continue;

                // j - index exponentu èlenu cos(3 * gamma)
                for(int j = 0; j < 3; j++) {
                    int i1 = 0;
                    int i2 = 0;
                    double val = 0;

                    // Když skonèí hodnoty pro dané i, j, jsou všechny hodnoty 0
                    while(i1 >= 0) {
                        string[] items = s.ReadLine().Split(' ');
                        ArrayList numbers = new ArrayList();

                        // Odstranìní pouhých mezer
                        for(int k = 0; k < items.Length; k++)
                            if(items[k] != null && items[k] != string.Empty)
                                numbers.Add(items[k]);

                        // Data jsou v souboru ve 3 sloupcích
                        for(int k = 0; k < 9; k += 3) {
                            i1 = int.Parse(numbers[k] as string) - 1;
                            i2 = int.Parse(numbers[k + 1] as string) - 1;
                            val = double.Parse(numbers[k + 2] as string);

                            if(j < 2 && i1 >= 0 && i2 >= 0) {
                                this.rws[i1, i2, j, i] = val;
                                this.rws[i2, i1, j, i] = val;
                            }
                        }
                    }
                }
            }
            s.Close();
            f.Close();
        }

        /// <summary>
        /// Indexer
        /// </summary>
        public double this[int i1, int i2, int mue, int l] { get { return this.rws[i1, i2, mue, l]; } }

        private string defaultFileName = @"c:\gcm\comp\angp.dat";
        private int maxL = 8;
    }
}
