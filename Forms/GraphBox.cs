using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Timers;
using System.IO;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Forms {
	/// <summary>
	/// Zobrazí graf i s pozadím
	/// </summary>
	public class GraphicsBox: System.Windows.Forms.PictureBox, IGraphControl {
		// Data k vykreslení
        private Graph graph;

        // Èasovaè pro animace
        private System.Timers.Timer timer = new System.Timers.Timer();

        // Index vykreslovaných dat (pro animaci)
        private int time, group, nGroups;

        // Animují se køivky a skupiny?
        private bool animGroup, animCurve;
        
        // Krok pøi rolování
        private int scrollStep;

        // Stisknuté prostøední tlaèítko myši
        private bool rightMouseButton;

		/// <summary>
		/// Základní konstruktor
		/// </summary>
		public GraphicsBox() : base() {
            this.SizeMode = PictureBoxSizeMode.StretchImage;

            this.timer.AutoReset = true;
            this.timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        }

        /// <param name="graph">Objekt grafu</param>
        public GraphicsBox(Graph graph)
            : this() {
            this.SetGraph(graph);
        }

        /// <summary>
        /// Vrátí objekt s daty
        /// </summary>
        public Graph GetGraph() {
            return this.graph;
        }

		/// <summary>
		/// Nastaví øadu vektorù k zobrazení
		/// </summary>
		/// <param name="graph">Objekt grafu</param>
		public void SetGraph(Graph graph) {
			this.graph = graph;

            this.group = 0;
            this.nGroups = this.graph.NumGroups;

            this.graph.FinishedBackground += new Graph.FinishedBackgroundEventHandler(graph_FinishedBackground);
            (this.Parent.Parent as GraphForm).NewProcess(string.Empty, this.graph.BackgroundWorkerCreate);            

            this.timer.Interval = this.graph.AnimInterval;
            this.scrollStep = this.graph.ScrollStep;

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

			this.Invalidate();
		}

        /// <summary>
        /// Voláno pøi dopoèítání pozadí jednotlivé skupiny
        /// </summary>
        void graph_FinishedBackground(object sender, FinishedBackgroundEventArgs e) {
            if(e.Group == this.group)
                this.Invalidate();
        }

        /// <summary>
        /// Event èasovaèe - postupné vykreslování køivky
        /// </summary>
        void timer_Elapsed(object sender, ElapsedEventArgs e) {
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
                this.Invalidate();
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
		/// Pøekreslení
		/// </summary>
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint (e);
            this.graph.PaintGraph(e.Graphics, this.ClientRectangle, this.group, this.time);
        }

        /// <summary>
        /// Nastaví ToolTip
        /// </summary>
        /// <param name="x">X - ová souøadnice myši</param>
        /// <param name="y">Y - ová souøadnice myši</param>
        public string ToolTip(int x, int y) {
            PointD p = this.CoordinatesFromPosition(x, y);

            double bv = this.graph.BackgroundValue(this.group, p);

            if(!double.IsNaN(bv)) 
                return string.Format("({0,5:F}, {1,5:F}) = {2,5:F}", p.X, p.Y, bv);
            else
                return string.Format("({0,5:F}, {1,5:F})", p.X, p.Y);
        }

        /// <summary>
        /// Pøevede souøadnice okna (myši) na souøadnice skuteèné
        /// </summary>
        /// <param name="x">X - ová souøadnice</param>
        /// <param name="y">Y - ová souøadnice</param>
        public PointD CoordinatesFromPosition(int x, int y) {
            return this.graph.CoordinatesFromPosition(this.group, this.ClientRectangle, x, y);
        }

		/// <summary>
		/// Pøi zmìnì velikosti
		/// </summary>
        protected override void OnSizeChanged(EventArgs e) {
            base.OnSizeChanged(e);
            this.Invalidate();
        }

        /// <summary>
        /// Zmìna skupiny
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);

            // S tlaèítkem vyvíjíme køivku
            if(this.rightMouseButton) {
                int newTime = this.time + scrollStep * System.Math.Sign(e.Delta);
                int maxTime = this.graph.MaxTime(this.group);
                if(newTime <= 0)
                    newTime = 1;
                else if(newTime > maxTime)
                    newTime = maxTime;
                this.time = newTime;
            }

            else {
                int newGroup = this.group + System.Math.Sign(e.Delta);
                if(newGroup < 0)
                    newGroup = 0;
                if(newGroup >= nGroups)
                    newGroup = this.nGroups - 1;
                this.group = newGroup;

                this.SetAnim();

                if(this.animCurve && e.Delta > 0)
                    this.time = 1;
                else
                    this.time = this.graph.MaxTime(this.group);
            }

            this.Invalidate();
        }

        /// <summary>
        /// Stisk tlaèítka myši
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            if(e.Button == MouseButtons.Right)
                this.rightMouseButton = true;
        }

        /// <summary>
        /// Uvolnìní tlaèítka myši
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);

            if(e.Button == MouseButtons.Right)
                this.rightMouseButton = false;
        }

        /// <summary>
        /// Musíme nastavit focus, jinak nefunguje OnMouseWheel
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if(!this.Focused && this.Parent.Focused)
                this.Focus();
        }
    }
}