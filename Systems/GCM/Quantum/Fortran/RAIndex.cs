using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace PavelStransky.GCM {
    /// <summary>
    /// Urèí, jak mohou být kombinovány indexy radiálních a úhlových maticových elementù
    /// pro urèité hodnoty kvantových èísel.
    /// Stavy jsou uspoøádány do urèitých množin kvantových èísel
    /// </summary>
    public class RAIndex {
        // Úhlový index (prvního stavu) závisející na na lambda a mue
        int[] ia;

        // Radiální index
        int[] ir;

        /// <summary>
        /// Úhlový index
        /// </summary>
        public int[] IA { get { return this.ia; } }

        /// <summary>
        /// Radiální index
        /// </summary>
        public int[] IR { get { return this.ir; } }

        /// <summary>
        /// Délka (poèet) indexù (celkový poèet stavù pro dané nph a l
        /// </summary>
        public int Length { get { return this.ia.Length; } }

        ///<summary>
        /// Konstruktor
        /// </summary>
        /// <param name="nph">Poèet fononù</param>
        public RAIndex(int l, int nph) {
            Debug.Assert(l == 0);
            // Speciální promìnná, která je rovna 3 pro sudý spin
            int modl23 = (l % 2) * 3;
            int lmod = l - modl23;

            int mue = l / 6;
            int muep1 = mue + 1;

            ArrayList ia = new ArrayList();
            ArrayList ir = new ArrayList();

            int iaMax = 0;
            int irMax = 0;

            for(int irad = 0; irad < iradMax; irad++) {
                //                for(int iang = 0; iang < iangMax; iang++) {
                // Aktuální fononové èíslo, které roste, dokud nedosáhne nph
                int nue = (int)(System.Math.Sqrt(irad) + System.Math.Sqrt(irad + 1)) - 1;
                //                    if(nue > nph) continue;
                if(nue > nph) break;

                int nuep1 = nue + 1;
                int ihelp = irad + 1 - (nue % 2 == 0 ? nuep1 * nuep1 - 1 : nuep1 * nuep1) / 4;
                // ihelp zavisi jen na irad
                //                    Debug.Print("ihlp {0}\tnue {1}", ihelp, nue);
                // Poèet fononù nesvázaných do páru na celkový moment hybnosti 0
                int lambda = 2 * (ihelp - 1) + nue % 2;//zavisi jen na irad
                /*phru              int lacont = iang / muep1 + modl23;

                                    if(lambda != lacont) continue;
                                    // pro l = 0 plati: lambda = iang
                                    int ihelp0 = iang % muep1 + 1;
                                    int ihelp1 = (lambda - lmod / 2) / 3 - modl23 / 3 + 1;

                                    int ibound = lambda - modl23 - 3 * (System.Math.Max(ihelp1 - mue, 1) + ihelp0 - 2);

                                    // Výbìrové pravidlo
                                    if(ibound < lmod / 2 || ibound > lmod) continue;
                phru*/
                //                    if (lambda != iang) continue;
                //                    Debug.Print("{0}\t{1}", irad, lambda);
                if(lambda % 3 != 0) continue;

                if(irMax < irad) irMax = irad;
                if(iaMax < lambda) iaMax = lambda;

                ir.Add(irad);
                ia.Add(/*iang*/lambda);
                //                }
            }

            Debug.Print("irMax = {0}, iaMax = {1}, count={2}", irMax, iaMax, ia.Count);

            this.ia = new int[ia.Count];
            this.ir = new int[ir.Count];

            int index = 0;
            foreach(int i in ia)
                this.ia[index++] = i;

            index = 0;
            foreach(int i in ir)
                this.ir[index++] = i;


        }

        private const int iradMax = 100000;
        //        private const int iangMax = 650;
    }
}
