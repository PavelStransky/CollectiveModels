using System;
using System.Timers;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Forms {
    /// <summary>
    /// Jeden prvek grafu se v��m v�udy 
    /// (pro t��du GraphicsBox a pro ukl�d�n�)
    /// </summary>
    public class GraphItem {
        private GraphicsBox parent;

        // Data k vykreslen�
        private Graph graph;

        // �asova� pro animace
        private System.Timers.Timer timer;

        // Index vykreslovan�ch dat (pro animaci)
        private int time, group, nGroups;

        // Animuj� se k�ivky a skupiny?
        private bool animGroup, animCurve;

        // Krok p�i rolov�n�
        private int scrollStep;

        /// <summary>
        /// Objekt s daty
        /// </summary>
        public Graph Graph { get { return this.graph; } }

        /// <summary>
        /// Po�et skupin grafu
        /// </summary>
        public int NGroups { get { return this.nGroups; } }

        /// <summary>
        /// Vrac� true, pokud je dan� graf pod dan�mi sou�adnicemi
        /// </summary>
        /// <param name="x">X - ov� sou�adnice</param>
        /// <param name="y">Y - ov� sou�adnice</param>
        /// <param name="rectangle">Obd�ln�k, ve kter�m je zobrazen�</param>
        public bool IsActive(Rectangle rectangle, int x, int y) {
            return this.graph.IsActive(this.group, rectangle, x, y);
        }

        /// <summary>
        /// P�evede sou�adnice okna (my�i) na sou�adnice skute�n�
        /// </summary>
        /// <param name="x">X - ov� sou�adnice</param>
        /// <param name="y">Y - ov� sou�adnice</param>
        /// <param name="rectangle">Obd�ln�k, ve kter�m je zobrazen�</param>
        public PointD CoordinatesFromPosition(Rectangle rectangle, int x, int y) {
            return this.graph.CoordinatesFromPosition(this.group, rectangle, x, y);
        }

        /// <summary>
        /// Vr�t� hodnotu matice z pozad�; pokud na dan� pozici matice pozad� nen�, vr�t� NaN
        /// </summary>
        /// <param name="p">Bod, z kter�ho chceme hodnotu pozad�</param>
        public double BackgroundValue(PointD p) {
            return this.graph.BackgroundValue(this.group, p);
        }

        /// <summary>
        /// Vr�t� ��slo aktu�ln� skupiny
        /// </summary>
        public int ActualGroup { get { return this.group; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="g">Objekt grafu</param>
        public GraphItem(GraphicsBox parent, Graph graph) {
            this.parent = parent;
            this.graph = graph;

            this.timer = new System.Timers.Timer();
            this.timer.AutoReset = true;
            this.timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            this.timer.Interval = this.graph.AnimInterval;

            this.group = 0;
            this.nGroups = this.graph.NumGroups;
            this.scrollStep = this.graph.ScrollStep;

            this.graph.FinishedBackground += new Graph.FinishedBackgroundEventHandler(graph_FinishedBackground);
            (this.parent.Parent as SingleGraphForm).NewProcess(string.Empty, this.graph.BWCreate);

            if(this.graph.AnimGroup && this.nGroups > 1)
                this.animGroup = true;
            else
                this.animGroup = false;

            if(this.graph.AnimCurves(this.group)) {
                this.time = 1;
                this.animCurve = true;
            }
            else {
                this.time = this.graph.MaxTime(this.group);
                this.animCurve = false;
            }

            if(this.animCurve || this.animGroup)
                this.timer.Start();
        }

        /// <summary>
        /// Vol�no p�i dopo��t�n� pozad� jednotliv� skupiny
        /// </summary>
        private void graph_FinishedBackground(object sender, FinishedBackgroundEventArgs e) {
            if(e.Group == this.group)
                this.parent.Invalidate();
        }

        /// <summary>
        /// Event �asova�e - postupn� vykreslov�n� k�ivky
        /// </summary>
        private void timer_Elapsed(object sender, ElapsedEventArgs e) {
            bool moveGroup = false;
            bool changed = this.animGroup;

            if(this.animCurve) {
                this.time++;
                if(this.time > this.graph.MaxTime(this.group)) {
                    this.time = 1;
                    moveGroup = true;
                }
                changed = true;
            }
            else if(this.animGroup)
                moveGroup = true;

            if(moveGroup) {
                this.group = (this.group + 1) % this.nGroups;

                this.SetAnim();

                if(this.animCurve)
                    this.time = 1;
                else
                    this.time = this.graph.MaxTime(this.group);
            }

            if(changed)
                this.parent.Invalidate();
        }

        /// <summary>
        /// Nastav� parametry animace podle aktu�ln� skupiny
        /// </summary>
        private void SetAnim() {
            this.animCurve = this.graph.AnimCurves(this.group);
            if(this.animCurve || this.animGroup)
                this.timer.Start();
            else
                this.timer.Stop();
        }

        /// <summary>
        /// Provede animaci k�ivky o zadan� delta
        /// </summary>
        /// <param name="delta">Posun k�ivky</param>
        public void AnimCurve(int delta) {
            int newTime = this.time + scrollStep * System.Math.Sign(delta);
            int maxTime = this.graph.MaxTime(this.group);

            if(newTime <= 0)
                newTime = 1;
            else if(newTime > maxTime)
                newTime = maxTime;
            this.time = newTime;

            this.parent.Invalidate();
        }

        /// <summary>
        /// Provede animaci skupiny o zadan� delta
        /// </summary>
        /// <param name="delta">Posun skupiny</param>
        public void AnimGroup(int delta) {
            int newGroup = this.group + System.Math.Sign(delta);

            if(newGroup < 0)
                newGroup = 0;
            if(newGroup >= nGroups)
                newGroup = this.nGroups - 1;
            this.group = newGroup;

            this.SetAnim();

            if(this.animCurve && delta > 0)
                this.time = 1;
            else
                this.time = this.graph.MaxTime(this.group);

            this.parent.Invalidate();
        }

        /// <summary>
        /// Provede vykreslen� grafu
        /// </summary>
        /// <param name="g">Grafick� objekt</param>
        /// <param name="rectangle">Obd�ln�k</param>
        public void Paint(Graphics g, Rectangle rectangle) {
            this.graph.PaintGraph(g, rectangle, this.group, this.time);
        }

        /// <summary>
        /// Vykresl� zadanou skupinu a �as grafu; 
        /// pokud graf neanimujeme, pak vykresl� aktu�ln� skupinu, resp. �as
        /// </summary>
        /// <param name="g">Grafick� objekt</param>
        /// <param name="rectangle">Obd�ln�k</param>
        /// <param name="group">��slo skupiny</param>
        /// <param name="time">�as grafu</param>
        public void PaintActual(Graphics g, Rectangle rectangle, int group, int time) {
            if(!this.animGroup)
                group = this.group;
            if(!this.graph.AnimCurves(group))
                time = this.graph.MaxTime(group);

            this.graph.PaintGraph(g, rectangle, group, time);
        }
    }
}
