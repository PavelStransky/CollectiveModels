using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.GCM {
    /// <summary>
    /// Ur��, jak mohou b�t kombinov�ny indexy radi�ln�ch a �hlov�ch maticov�ch element�
    /// pro ur�it� hodnoty kvantov�ch ��sel.
    /// Stavy jsou uspo��d�ny do ur�it�ch mno�in kvantov�ch ��sel
    /// </summary>
    public class RAIndex {
        // �hlov� index (prvn�ho stavu) z�visej�c� na na lambda a mue
        int[] ia;

        // Radi�ln� index
        int[] ir;

        /// <summary>
        /// �hlov� index
        /// </summary>
        public int[] IA { get { return this.ia; } }

        /// <summary>
        /// Radi�ln� index
        /// </summary>
        public int[] IR { get { return this.ir; } }

        /// <summary>
        /// D�lka (po�et) index� (celkov� po�et stav� pro dan� nph a l
        /// </summary>
        public int Length { get { return this.ia.Length; } }

        ///<summary>
        /// Konstruktor
        /// </summary>
        /// <param name="l">�hlov� moment</param>
        /// <param name="nph">Po�et fonon�</param>
        public RAIndex(int l, int nph) {
            // Speci�ln� prom�nn�, kter� je rovna 3 pro sud� spin
            int modl23 = (l % 2) * 3;
            int lmod = l - modl23;

            int mue = l / 6;
            int muep1 = mue + 1;

            ArrayList ia = new ArrayList();
            ArrayList ir = new ArrayList();

            for(int irad = 0; irad < iradMax; irad++) {
                for(int iang = 0; iang < iangMax; iang++) {
                    // Aktu�ln� fononov� ��slo, kter� roste, dokud nedos�hne nph
                    int nue = (int)(System.Math.Sqrt(irad) + System.Math.Sqrt(irad + 1)) - 1;
                    if(nue > nph) continue;

                    int nuep1 = nue + 1;
                    int ihelp = irad + 1 - (nue % 2 == 0 ? nuep1 * nuep1 - 1 : nuep1 * nuep1) / 4;

                    // Po�et fonon� nesv�zan�ch do p�ru na celkov� moment hybnosti 0
                    int lambda = 2 * (ihelp - 1) + nue % 2;
                    int lacont = iang / muep1 + modl23;

                    if(lambda != lacont) continue;

                    int ihelp0 = iang % muep1 + 1;
                    int ihelp1 = (lambda - lmod / 2) / 3 - modl23 / 3 + 1;

                    int ibound = lambda - modl23 - 3 * (System.Math.Max(ihelp1 - mue, 1) + ihelp0 - 2);

                    // V�b�rov� pravidlo
                    if(ibound < lmod / 2 || ibound > lmod) continue;

                    ir.Add(irad);
                    ia.Add(iang);
                }
            }

            this.ia = new int[ia.Count];
            this.ir = new int[ir.Count];

            int index = 0;
            foreach(int i in ia)
                this.ia[index++] = i;

            index = 0;
            foreach(int i in ir)
                this.ir[index++] = i;
        }

        private const int iradMax = 256;
        private const int iangMax = 65;
    }
}
