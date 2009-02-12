using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.GCM {
    /// <summary>
    /// Uchov�v� a zpacov�v� kontury (ekvipotenci�ln� k�ivky)
    /// </summary>
    public class Contour {
        // Aktu�ln� �ady bod� (pro jednotliv� ko�eny)
        ArrayList[] points;

        // Ukon�en� �ady bod�
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
        /// Za�ne p�id�v�n� ko�en� do �et�zc�
        /// </summary>
        public void Begin() {
            for(int i = 0; i < this.points.Length; i++)
                this.points[i] = null;

            this.chains.Clear();
        }

        /// <summary>
        /// P�id� v�echny ko�eny beta z vektoru do �et�zce
        /// </summary>
        /// <param name="beta">Ko�eny beta</param>
        /// <param name="gamma">Gamma</param>
        public void Add(Vector beta, double gamma) {
            for(int i = 0; i < beta.Length; i++)
                this.Add(i, beta[i], gamma);
        }

        /// <summary>
        /// P�id� ko�en do �et�zce
        /// </summary>
        /// <param name="i">Index ko�ene</param>
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

                // Zat�m m�me v �et�zci pouze jeden bod
                if(length < 2) {
                    this.points[i].Add(newPoint);
                }
                // V�ce bod�
                else {
                    PointD p1 = (PointDPolar)this.points[i][length - 1];
                    PointD p2 = (PointDPolar)this.points[i][length - 2];

                    // Mnohem v�t�� odchylka ne� p�edchoz� odchylky - za�neme nov� �et�z
                    if(diff * PointD.Distance(p1, p2) < PointD.Distance(newPoint, p1)) {
                        this.chains.Add(this.points[i]);

                        this.points[i] = new ArrayList();
                        this.points[i].Add(newPoint);
                    }
                    // Pouze dva body, odchylka prvn�ch dvou bod� v�t�� ne� u ostatn�ch - prvn� bod vyhod�me
                    else if(length == 2 && PointD.Distance(p1, p2) > diff * PointD.Distance(newPoint, p1)) {
                        this.points[i][0] = this.points[i][1];
                        this.points[i][1] = newPoint;
                    }
                    // Norm�ln� stav
                    else
                        this.points[i].Add(newPoint);
                }
            }
        }

        /// <summary>
        /// Ukon�� p�id�v�n� ko�en� do �et�zc�
        /// </summary>
        public void End() {
            for(int i = 0; i < this.points.Length; i++)
                if(this.points[i] != null) {
                    this.chains.Add(this.points[i]);
                    this.points[i] = null;
                }
        }

        /// <summary>
        /// Odstran� kr�tk� data (o m�n� ne� short bodech)
        /// </summary>
        /// <returns>Po�et odstran�n�ch dat</returns>
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
        /// Spojovac� procedura
        /// </summary>
        public void Join() {
            for(int i = 0; i < this.chains.Count; i++) {
                ArrayList a = this.chains[i] as ArrayList;

                PointD p1 = (PointDPolar)a[0];
                PointD p2 = (PointDPolar)a[a.Count - 1];

                // Hled�me �et�zec, kter� bychom napojili
                do {
                    // Index nejbli���ho �et�zce
                    int next = i;
                    // True, pokud je nejbl�e za��te�n� bod �et�zce
                    bool isBegin = true;
                    // Vzd�lenost nejbli���ho �et�zce
                    double d = PointD.Distance(p1, p2);

                    ArrayList b;

                    // Hled�n� nejmen�� vzd�lenosti
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
                    if(!isBegin)    // Data navazuj� obr�cen�m sm�rem - oto��me je
                        b.Reverse();

                    p2 = (PointDPolar)b[b.Count - 1];

                    a.AddRange(b);
                    this.chains.RemoveAt(next);
                } while(true);

                // Zacyklen� dat (posledn� bod = prvn� bod, aby nap�. Excel spojil)
//                a.Add(a[0]);
            }
        }

        /// <summary>
        /// Vr�t� v�sledky jako �adu PointVector
        /// </summary>
        /// <param name="pvItemNumber">Maxim�ln� po�et bod� v jednom vektoru</param>
        public PointVector[] GetPointVector(int pvItemNumber) {
            PointVector[] result = new PointVector[this.chains.Count];

            for(int i = 0; i < result.Length; i++) {
                ArrayList a = (ArrayList)this.chains[i];

                pvItemNumber = System.Math.Min(pvItemNumber, a.Count);
                result[i] = new PointVector(pvItemNumber + 1);

                for(int j = 0; j < pvItemNumber; j++)
                    result[i][j] = (PointDPolar)a[j * a.Count / pvItemNumber];

                // Zacyklen� dat
                result[i][pvItemNumber] = result[i][0];
            }

            return result;
        }

        /// <summary>
        /// Vr�t� v�sledky jako �adu PointVector
        /// </summary>
        public PointVector[] GetPointVector() {
            PointVector[] result = new PointVector[this.chains.Count];

            for(int i = 0; i < result.Length; i++) {
                ArrayList a = (ArrayList)this.chains[i];
                result[i] = new PointVector(a.Count + 1);

                for(int j = 0; j < a.Count; j++)
                    result[i][j] = (PointDPolar)a[j];

                // Zacyklen� dat
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
