using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Forms {
    /// <summary>
    /// Rozhraní pro control grafu
    /// </summary>
    public interface IGraphControl {
        /// <summary>
        /// Nastaví graf
        /// </summary>
        /// <param name="graph">Objekt s grafem</param>
        void SetGraph(Graph graph);

        /// <summary>
        /// Naète graf
        /// </summary>
        /// <param name="graph"></param>
        Graph GetGraph();

        /// <summary>
        /// Vrátí text ToolTipu
        /// </summary>
        /// <param name="x">X-ová souøadnice</param>
        /// <param name="y">Y-ová souøadnice</param>
        string ToolTip(int x, int y);

        /// <summary>
        /// Vrátí skuteèné souøadnice vypoèítané ze souøadnic v oknì
        /// </summary>
        /// <param name="x">X-ová souøadnice</param>
        /// <param name="y">Y-ová souøadnice</param>
        /// <returns></returns>
        PointD CoordinatesFromPosition(int x, int y);

        /// <summary>
        /// Uloží graf jako GIF
        /// </summary>
        /// <param name="fName">Jméno souboru</param>
        void SaveGIF(string fName);

        /// <summary>
        /// Uloží graf jako obrázek v daném formátu.
        /// Pokud se jedná o animaci, uloží ji sekvenènì ve tvaru fName00.ext
        /// </summary>
        /// <param name="fName">Jméno souboru</param>
        void SavePicture(string fName);
    }
}
