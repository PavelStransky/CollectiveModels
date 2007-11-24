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
	/// Zobraz� graf i s pozad�m
	/// </summary>
	public class GraphicsBox: System.Windows.Forms.PictureBox, IGraphControl {
		// Data k vykreslen�
        private Graph graph;

        // �asova� pro animace
        private System.Timers.Timer timer = new System.Timers.Timer();

        // Index vykreslovan�ch dat (pro animaci)
        private int time, group, nGroups;

        // Animuj� se k�ivky a skupiny?
        private bool animGroup, animCurve;
        
        // Krok p�i rolov�n�
        private int scrollStep;

        // Stisknut� prost�edn� tla��tko my�i
        private bool rightMouseButton;

		/// <summary>
		/// Z�kladn� konstruktor
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
        /// Vr�t� objekt s daty
        /// </summary>
        public Graph GetGraph() {
            return this.graph;
        }

		/// <summary>
		/// Nastav� �adu vektor� k zobrazen�
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
        /// Vol�no p�i dopo��t�n� pozad� jednotliv� skupiny
        /// </summary>
        void graph_FinishedBackground(object sender, FinishedBackgroundEventArgs e) {
            if(e.Group == this.group)
                this.Invalidate();
        }

        /// <summary>
        /// Event �asova�e - postupn� vykreslov�n� k�ivky
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
		/// P�ekreslen�
		/// </summary>
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint (e);
            this.graph.PaintGraph(e.Graphics, this.ClientRectangle, this.group, this.time);
        }

        /// <summary>
        /// Nastav� ToolTip
        /// </summary>
        /// <param name="x">X - ov� sou�adnice my�i</param>
        /// <param name="y">Y - ov� sou�adnice my�i</param>
        public string ToolTip(int x, int y) {
            PointD p = this.CoordinatesFromPosition(x, y);

            double bv = this.graph.BackgroundValue(this.group, p);

            if(!double.IsNaN(bv)) 
                return string.Format("({0,5:F}, {1,5:F}) = {2,5:F}", p.X, p.Y, bv);
            else
                return string.Format("({0,5:F}, {1,5:F})", p.X, p.Y);
        }

        /// <summary>
        /// P�evede sou�adnice okna (my�i) na sou�adnice skute�n�
        /// </summary>
        /// <param name="x">X - ov� sou�adnice</param>
        /// <param name="y">Y - ov� sou�adnice</param>
        public PointD CoordinatesFromPosition(int x, int y) {
            return this.graph.CoordinatesFromPosition(this.group, this.ClientRectangle, x, y);
        }

		/// <summary>
		/// P�i zm�n� velikosti
		/// </summary>
        protected override void OnSizeChanged(EventArgs e) {
            base.OnSizeChanged(e);
            this.Invalidate();
        }

        /// <summary>
        /// Zm�na skupiny
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);

            // S tla��tkem vyv�j�me k�ivku
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
        /// Stisk tla��tka my�i
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            if(e.Button == MouseButtons.Right)
                this.rightMouseButton = true;
        }

        /// <summary>
        /// Uvoln�n� tla��tka my�i
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);

            if(e.Button == MouseButtons.Right)
                this.rightMouseButton = false;
        }

        /// <summary>
        /// Mus�me nastavit focus, jinak nefunguje OnMouseWheel
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if(!this.Focused && this.Parent.Focused)
                this.Focus();
        }
    }
}