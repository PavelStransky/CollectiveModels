using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.GCM {
    /// <summary>
    /// Uchovává a zpacovává kontury (ekvipotenciální køivky)
    /// </summary>
    public class Contour {
        // Aktuální øady bodù (pro jednotlivé koøeny)
        ArrayList[] points;

        // Ukonèené øady bodù
        ArrayList chains;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Contour() {
            this.points = new ArrayList[rootNumber];
            this.chains = new ArrayList();
        }

        /// <summary>
        /// Konstruktor s poctem korenu
        /// </summary>
        /// <param name="rootNumber">Pocet korenu</param>
        public Contour(int rootNumber) {
            this.points = new ArrayList[rootNumber];
            this.chains = new ArrayList();
        }

        /// <summary>
        /// Zaène pøidávání koøenù do øetìzcù
        /// </summary>
        public void Begin() {
            for(int i = 0; i < this.points.Length; i++)
                this.points[i] = null;

            this.chains.Clear();
        }

        /// <summary>
        /// Pøidá všechny koøeny beta z vektoru do øetìzce
        /// </summary>
        /// <param name="beta">Koøeny beta</param>
        /// <param name="gamma">Gamma</param>
        public void Add(Vector beta, double gamma) {
            for(int i = 0; i < beta.Length; i++)
                this.Add(i, beta[i], gamma);
        }

        /// <summary>
        /// Pøidá koøen do øetìzce
        /// </summary>
        /// <param name="i">Index koøene</param>
        /// <param name="beta">Beta</param>
        /// <param name="gamma">Gamma</param>
        public void Add(int i, double beta, double gamma) {
            PointDPolar newPoint = new PointDPolar(beta, gamma);

            if(this.points[i] == null) {
                this.points[i] = new ArrayList();
                this.points[i].Add(newPoint);
            }
            else {
                int length = this.points[i].Count;

                // Zatím máme v øetìzci pouze jeden bod
                if(length < 2) {
                    this.points[i].Add(newPoint);
                }
                // Více bodù
                else {
                    PointD p1 = (PointDPolar)this.points[i][length - 1];
                    PointD p2 = (PointDPolar)this.points[i][length - 2];

                    // Mnohem vìtší odchylka než pøedchozí odchylky - zaèneme nový øetìz
                    if(diff * PointD.Distance(p1, p2) < PointD.Distance(newPoint, p1)) {
                        this.chains.Add(this.points[i]);

                        this.points[i] = new ArrayList();
                        this.points[i].Add(newPoint);
                    }
                    // Pouze dva body, odchylka prvních dvou bodù vìtší než u ostatních - první bod vyhodíme
                    else if(length == 2 && PointD.Distance(p1, p2) > diff * PointD.Distance(newPoint, p1)) {
                        this.points[i][0] = this.points[i][1];
                        this.points[i][1] = newPoint;
                    }
                    // Normální stav
                    else
                        this.points[i].Add(newPoint);
                }
            }
        }

        /// <summary>
        /// Ukonèí pøidávání koøenù do øetìzcù
        /// </summary>
        public void End() {
            for(int i = 0; i < this.points.Length; i++)
                if(this.points[i] != null) {
                    this.chains.Add(this.points[i]);
                    this.points[i] = null;
                }
        }

        /// <summary>
        /// Odstraní krátká data (o ménì než short bodech)
        /// </summary>
        /// <returns>Poèet odstranìných dat</returns>
        public int RemoveShort() {
            int length = this.chains.Count;

            for(int i = 0; i < this.chains.Count; i++) {
                if(((ArrayList)this.chains[i]).Count < shortLength) {
                    this.chains.RemoveAt(i);
                    i--;
                }
            }

            return length - this.chains.Count;
        }

        /// <summary>
        /// Spojovací procedura
        /// </summary>
        public void Join() {
            for(int i = 0; i < this.chains.Count; i++) {
                ArrayList a = this.chains[i] as ArrayList;

                PointD p1 = (PointDPolar)a[0];
                PointD p2 = (PointDPolar)a[a.Count - 1];

                // Hledáme øetìzec, který bychom napojili
                do {
                    // Index nejbližšího øetìzce
                    int next = i;
                    // True, pokud je nejblíže zaèáteèní bod øetìzce
                    bool isBegin = true;
                    // Vzdálenost nejbližšího øetìzce
                    double d = PointD.Distance(p1, p2);

                    ArrayList b;

                    // Hledání nejmenší vzdálenosti
                    for(int j = i + 1; j < this.chains.Count; j++) {
                        b = this.chains[j] as ArrayList;

                        PointD q1 = (PointDPolar)b[0];
                        PointD q2 = (PointDPolar)b[b.Count - 1];

                        double d1 = PointD.Distance(p2, q1);
                        double d2 = PointD.Distance(p2, q2);

                        if(d1 < d) {
                            next = j;
                            d = d1;
                            isBegin = true;
                        }
                        if(d2 < d) {
                            next = j;
                            d = d2;
                            isBegin = false;
                        }
                    }

                    if(next == i)
                        break;

                    b = this.chains[next] as ArrayList;
                    if(!isBegin)    // Data navazují obráceným smìrem - otoèíme je
                        b.Reverse();

                    p2 = (PointDPolar)b[b.Count - 1];

                    a.AddRange(b);
                    this.chains.RemoveAt(next);
                } while(true);

                // Zacyklení dat (poslední bod = první bod, aby napø. Excel spojil)
//                a.Add(a[0]);
            }
        }

        /// <summary>
        /// Vrátí výsledky jako øadu PointVector
        /// </summary>
        /// <param name="pvItemNumber">Maximální poèet bodù v jednom vektoru</param>
        public PointVector[] GetPointVector(int pvItemNumber) {
            PointVector[] result = new PointVector[this.chains.Count];

            for(int i = 0; i < result.Length; i++) {
                ArrayList a = (ArrayList)this.chains[i];

                pvItemNumber = System.Math.Min(pvItemNumber, a.Count);
                result[i] = new PointVector(pvItemNumber + 1);

                for(int j = 0; j < pvItemNumber; j++)
                    result[i][j] = (PointDPolar)a[j * a.Count / pvItemNumber];

                // Zacyklení dat
                result[i][pvItemNumber] = result[i][0];
            }

            return result;
        }

        /// <summary>
        /// Vrátí výsledky jako øadu PointVector
        /// </summary>
        public PointVector[] GetPointVector() {
            PointVector[] result = new PointVector[this.chains.Count];

            for(int i = 0; i < result.Length; i++) {
                ArrayList a = (ArrayList)this.chains[i];
                result[i] = new PointVector(a.Count + 1);

                for(int j = 0; j < a.Count; j++)
                    result[i][j] = (PointDPolar)a[j];

                // Zacyklení dat
                result[i][a.Count] = result[i][0];
            }

            return result;
        }

        private const int rootNumber = 4;
        private const int shortLength = 4;
        private const int diff = 20;
        private const int defaultPointVectorItemNumber = 100;
    }
}
