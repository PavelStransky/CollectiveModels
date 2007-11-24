using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Forms {
    /// <summary>
    /// Rozhran� pro control grafu
    /// </summary>
    public interface IGraphControl {
        /// <summary>
        /// Nastav� graf
        /// </summary>
        /// <param name="graph">Objekt s grafem</param>
        void SetGraph(Graph graph);

        /// <summary>
        /// Na�te graf
        /// </summary>
        /// <param name="graph"></param>
        Graph GetGraph();

        /// <summary>
        /// Vr�t� text ToolTipu
        /// </summary>
        /// <param name="x">X-ov� sou�adnice</param>
        /// <param name="y">Y-ov� sou�adnice</param>
        string ToolTip(int x, int y);

        /// <summary>
        /// Vr�t� skute�n� sou�adnice vypo��tan� ze sou�adnic v okn�
        /// </summary>
        /// <param name="x">X-ov� sou�adnice</param>
        /// <param name="y">Y-ov� sou�adnice</param>
        /// <returns></returns>
        PointD CoordinatesFromPosition(int x, int y);
    }
}
