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
	public class GraphicsBox: System.Windows.Forms.PictureBox {
        // Grafy
        private GraphItem[] graphs;

        // Stisknuté tlaèítko CTRL
        private bool ctrlPressed;

        // Stisknutý SHIFT
        private bool shiftPressed;

        /// <summary>
        /// Vrací prvky grafu
        /// </summary>
        public GraphItem[] GraphItems { get { return this.graphs; } }

		/// <summary>
		/// Základní konstruktor
		/// </summary>
        public GraphicsBox() : base() {
            this.BackColor = Color.White;
        }

        /// <param name="graphs">Objekty grafù</param>
        public GraphicsBox(TArray graphs)
            : this() {
            this.SetGraphs(graphs);
        }

		/// <summary>
		/// Nastaví øadu vektorù k zobrazení
		/// </summary>
		/// <param name="graphs">Objekty grafù</param>
		public void SetGraphs(TArray graphs) {
            int count = graphs.Length;
            this.graphs = new GraphItem[count];

            for(int i = 0; i < count; i++) 
                this.graphs[i] = new GraphItem(this, graphs[i] as Graph);            

			this.Invalidate();
		}

		/// <summary>
		/// Pøekreslení
		/// </summary>
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint (e);
            if(this.graphs != null) {
                int count = this.graphs.Length;

                for(int i = 0; i < count; i++)
                    this.graphs[i].Paint(e.Graphics, this.ClientRectangle);
            }
        }

        /// <summary>
        /// Urèí poøadové èíslo grafu, jež byl poslední vykreslen na daných souøadnicích
        /// </summary>
        /// <param name="x">X - ová souøadnice myši</param>
        /// <param name="y">Y - ová souøadnice myši</param>
        public int ActiveGraph(int x, int y) {
            for(int i = this.graphs.Length - 1; i >= 0; i--) {
                GraphItem graph = this.graphs[i];
                if(graph.IsActive(this.ClientRectangle, x, y))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Nastaví ToolTip
        /// </summary>
        /// <param name="x">X - ová souøadnice myši</param>
        /// <param name="y">Y - ová souøadnice myši</param>
        public string ToolTip(int x, int y) {
            int i = this.ActiveGraph(x, y);

            if(i < 0)
                return null;

            GraphItem gi = this.graphs[i];
            PointD p = gi.CoordinatesFromPosition(this.ClientRectangle, x, y);
            double bv = gi.BackgroundValue(p);

            if(!double.IsNaN(bv)) 
                return string.Format("({0,5:F}, {1,5:F}) = {2,5:F}", p.X, p.Y, bv);
            else
                return string.Format("({0,5:F}, {1,5:F})", p.X, p.Y);
        }

        /// <summary>
        /// Vrátí souøadnice podle zadané pozice myši
        /// </summary>
        /// <param name="x">X - ová souøadnice myši</param>
        /// <param name="y">Y - ová souøadnice myši</param>
        public PointD CoordintatesFromPosition(int x, int y) {
            int i = this.ActiveGraph(x, y);

            if(i < 0)
                return null;

            return this.graphs[i].CoordinatesFromPosition(this.ClientRectangle, x, y);
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

            // S tlaèítkem SHIFT vyvíjíme vše naráz
            if(shiftPressed) {
                int count = this.graphs.Length;

                // S tlaèítkem ctrl vyvíjíme køivku
                if(this.ctrlPressed) {
                    for(int i = 0; i < count; i++)
                        this.graphs[i].AnimCurve(e.Delta);
                }
                else {
                    for(int i = 0; i < count; i++)
                        this.graphs[i].AnimGroup(e.Delta);
                }
            }

            else {
                int i = this.ActiveGraph(e.X, e.Y);
                if(i < 0)
                    return;

                // S tlaèítkem ctrl vyvíjíme køivku
                if(this.ctrlPressed) {
                    this.graphs[i].AnimCurve(e.Delta);
                }
                else {
                    this.graphs[i].AnimGroup(e.Delta);
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);

            this.shiftPressed = e.Shift;
            this.ctrlPressed = e.Control;
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);

            this.ctrlPressed = e.Control;
            this.shiftPressed = e.Shift;
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