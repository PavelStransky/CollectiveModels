using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Forms {
	/// <summary>
	/// Prázdný formuláø. Grafy do nìj pøidáváme my
	/// </summary>
	public partial class GraphForm : ChildForm, IExportable {
        private GraphControl[] graphControl;

        /// <summary>
        /// Poèet sloucpù
        /// </summary>
        private int numColumns = 1;

		/// <summary>
		/// Konstruktor
		/// </summary>
        public GraphForm() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Nastaví data do grafu
        /// </summary>
        /// <param name="graphs">Objekt s daty</param>
        /// <param name="numColumns">Poèet sloupcù</param>
        public void SetGraph(Expression.Array graphs, int numColumns) {
            int count = graphs.Count;
            int numRows = (count - 1) / numColumns + 1;

            Rectangle r = this.ClientRectangle;
            int xStep = (r.Width - margin) / numColumns;
            int yStep = (r.Height - margin) / numRows;

            this.graphControl = new GraphControl[count];
            this.SuspendLayout();
            this.Controls.Clear();
            for(int i = 0; i < count; i++) {
                GraphControl gc = new GraphControl(graphs[i] as Graph);
                gc.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                gc.Location = new Point(r.X + margin + xStep * (i % numColumns), r.Y + margin + yStep * (i / numColumns));
                gc.Size = new Size(xStep - margin, yStep - margin);
                this.Controls.Add(gc);
                this.graphControl[i] = gc;
            }
            this.ResumeLayout();

            this.numColumns = numColumns;
        }

        protected override void OnSizeChanged(EventArgs e) {
            base.OnSizeChanged(e);

            if(this.graphControl == null)
                return;

            int length = this.graphControl.Length;
            int numRows = (length - 1) / numColumns + 1;

            Rectangle r = this.ClientRectangle;
            int xStep = (r.Width - margin) / numColumns;
            int yStep = (r.Height - margin) / numRows;

            this.SuspendLayout();
            for(int i = 0; i < length; i++) {
                GraphControl gc = this.graphControl[i];
                gc.Location = new Point(r.X + margin + xStep * (i % numColumns), r.Y + margin + yStep * (i / numColumns));
                gc.Size = new Size(xStep - margin, yStep - margin);
            }
            this.ResumeLayout();
        }

        #region Implementace IExportable
        /// <summary>
        /// Uloží obsah formuláøe do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            // Musíme ukládat binárnì
            if(!export.Binary)
                throw new Exception("");
            
            // Binárnì
            BinaryWriter b = export.B;
            b.Write(this.Location.X);
            b.Write(this.Location.Y);
            b.Write(this.Size.Width);
            b.Write(this.Size.Height);

            b.Write(this.Name);
            b.Write(this.numColumns);

            int length = this.graphControl.Length;
            b.Write(length);
            for(int i = 0; i < length; i++)
                export.Write(this.graphControl[i].GetGraph());
        }

        /// <summary>
        /// Naète obsah kontextu ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public void Import(PavelStransky.Math.Import import) {
            // Musíme èíst binárnì
            if(!import.Binary)
                throw new Exception("");

            // Binárnì
            BinaryReader b = import.B;
            this.Location = new Point(b.ReadInt32(), b.ReadInt32());
            this.Size = new Size(b.ReadInt32(), b.ReadInt32());

            this.Name = b.ReadString();
            this.Text = this.Name;
            int numColumns = b.ReadInt32();
            int length = b.ReadInt32();

            Expression.Array graphs = new Expression.Array();
            for(int i = 0; i < length; i++)
                graphs.Add(import.Read());
            this.SetGraph(graphs, numColumns);
        }
        #endregion

        // Okraj okolo GraphControlu v pixelech
        private int margin = 8;
    }
}
