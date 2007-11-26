using System;
using System.Timers;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Forms {
    /// <summary>
    /// Jeden prvek grafu se vším všudy 
    /// (pro tøídu GraphicsBox a pro ukládání)
    /// </summary>
    public class GraphItem {
        private GraphicsBox parent;

        // Data k vykreslení
        private Graph graph;

        // Èasovaè pro animace
        private System.Timers.Timer timer;

        // Index vykreslovaných dat (pro animaci)
        private int time, group, nGroups;

        // Animují se køivky a skupiny?
        private bool animGroup, animCurve;

        // Krok pøi rolování
        private int scrollStep;

        /// <summary>
        /// Objekt s daty
        /// </summary>
        public Graph Graph { get { return this.graph; } }

        /// <summary>
        /// Poèet skupin grafu
        /// </summary>
        public int NGroups { get { return this.nGroups; } }

        /// <summary>
        /// Vrací true, pokud je daný graf pod danými souøadnicemi
        /// </summary>
        /// <param name="x">X - ová souøadnice</param>
        /// <param name="y">Y - ová souøadnice</param>
        /// <param name="rectangle">Obdélník, ve kterém je zobrazení</param>
        public bool IsActive(Rectangle rectangle, int x, int y) {
            return this.graph.IsActive(this.group, rectangle, x, y);
        }

        /// <summary>
        /// Pøevede souøadnice okna (myši) na souøadnice skuteèné
        /// </summary>
        /// <param name="x">X - ová souøadnice</param>
        /// <param name="y">Y - ová souøadnice</param>
        /// <param name="rectangle">Obdélník, ve kterém je zobrazení</param>
        public PointD CoordinatesFromPosition(Rectangle rectangle, int x, int y) {
            return this.graph.CoordinatesFromPosition(this.group, rectangle, x, y);
        }

        /// <summary>
        /// Vrátí hodnotu matice z pozadí; pokud na dané pozici matice pozadí není, vrátí NaN
        /// </summary>
        /// <param name="p">Bod, z kterého chceme hodnotu pozadí</param>
        public double BackgroundValue(PointD p) {
            return this.graph.BackgroundValue(this.group, p);
        }

        /// <summary>
        /// Vrátí èíslo aktuální skupiny
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
        /// Voláno pøi dopoèítání pozadí jednotlivé skupiny
        /// </summary>
        private void graph_FinishedBackground(object sender, FinishedBackgroundEventArgs e) {
            if(e.Group == this.group)
                this.parent.Invalidate();
        }

        /// <summary>
        /// Event èasovaèe - postupné vykreslování køivky
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
        /// Nastaví parametry animace podle aktuální skupiny
        /// </summary>
        private void SetAnim() {
            this.animCurve = this.graph.AnimCurves(this.group);
            if(this.animCurve || this.animGroup)
                this.timer.Start();
            else
                this.timer.Stop();
        }

        /// <summary>
        /// Provede animaci køivky o zadané delta
        /// </summary>
        /// <param name="delta">Posun køivky</param>
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
        /// Provede animaci skupiny o zadané delta
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
        /// Provede vykreslení grafu
        /// </summary>
        /// <param name="g">Grafický objekt</param>
        /// <param name="rectangle">Obdélník</param>
        public void Paint(Graphics g, Rectangle rectangle) {
            this.graph.PaintGraph(g, rectangle, this.group, this.time);
        }

        /// <summary>
        /// Vykreslí zadanou skupinu a èas grafu; 
        /// pokud graf neanimujeme, pak vykreslí aktuální skupinu, resp. èas
        /// </summary>
        /// <param name="g">Grafický objekt</param>
        /// <param name="rectangle">Obdélník</param>
        /// <param name="group">Èíslo skupiny</param>
        /// <param name="time">Èas grafu</param>
        public void PaintActual(Graphics g, Rectangle rectangle, int group, int time) {
            if(!this.animGroup)
                group = this.group;
            if(!this.graph.AnimCurves(group))
                time = this.graph.MaxTime(group);

            this.graph.PaintGraph(g, rectangle, group, time);
        }
    }
}
