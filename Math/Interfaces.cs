using System;
using System.IO;

namespace PavelStransky.Math {
    /// <summary>
    /// Delegát - reálná funkce reálné promìnné
    /// </summary>
    public delegate double RealFunction(double x);

    /// <summary>
    /// Delegát - Vektorová funkce vektorové promìnné
    /// </summary>
    /// <param name="rightSide">Vypoèítaná pravá strana</param>
    /// <param name="x">Hodnoty parametrù</param>
    public delegate Vector VectorFunction(Vector x);

    /// <summary>
    /// Delegát - Skalární funkce vektorové promìnné
    /// </summary>
    /// <param name="x">Hodnoty parametrù</param>
    public delegate double ScalarFunction(Vector x);

    /// <summary>
    /// Delegát - Jakobián transformace
    /// </summary>
    /// <param name="x">Souøadnice x</param>
    public delegate Matrix Jacobian(Vector x);

    /// <summary>
    /// Metody RK
    /// </summary>
    public enum RungeKuttaMethods { Normal, Energy, Adapted }

    /// <summary>
    /// Rozhraní pro export dat do souboru
    /// </summary>
    public interface IExportable {
        /// <summary>
        /// Uloží data do souboru
        /// </summary>
        void Export(Export export);

        /// <summary>
        /// Naète data
        /// </summary>
        void Import(Import import);
    }

    /// <summary>
    /// Interface pro kvantový systém
    /// </summary>
    public interface IQuantumSystem {
        /// <summary>
        /// Vlastní hodnota
        /// </summary>
        double[] EigenValue { get;}

        /// <summary>
        /// Vlastní vektor
        /// </summary>
        Vector[] EigenVector { get;}

        /// <summary>
        /// Matice s hustotou vlastní funkce
        /// </summary>
        /// <param name="n">Èíslo vlastní funkce</param>
        /// <param name="range">Oblast hodnot k zhobrazení (seøazená ve tvaru minx, maxx, numx, ...)</param>
        Matrix DensityMatrix(int n, Vector range);
    }

    /// <summary>
    /// Interface pro dynamický systém
    /// </summary>
    public interface IDynamicalSystem {
        /// <summary>
        /// Energie systému
        /// </summary>
        /// <param name="x">Souøadnice a rychlosti</param>
        double E(Vector x);

        /// <summary>
        /// Jakobián
        /// </summary>
        /// <param name="x">Souøadnice a rychlosti</param>
        /// <returns></returns>
        Matrix Jacobian(Vector x);

        /// <summary>
        /// Pravá strana pohybových rovnic
        /// </summary>
        /// <param name="x">Souøadnice a rychlosti</param>
        /// <returns></returns>
        Vector Equation(Vector x);

        /// <summary>
        /// Generuje poèáteèní podmínky s danou energií
        /// </summary>
        /// <param name="e">Energie</param>
        Vector IC(double e);

        /// <summary>
        /// Generuje poèáteèní podmínky s danou energií a s daným úhlovým momentem
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="l">Úhlový moment</param>
        Vector IC(double e, double l);

        /// <summary>
        /// Generuje hybnosti do poèáteèních podmínek
        /// </summary>
        /// <param name="ic">Souøadnice poèáteèních podmínek</param>
        /// <param name="e">Energie</param>
        /// <returns>True, pokud se generování podaøilo</returns>
        bool IC(Vector ic, double e);

        /// <summary>
        /// Hranice kinematicky dostupné oblasti na osach x, y, vx, vy
        /// </summary>
        /// <param name="e">Energie</param>
        Vector Bounds(double e);

        /// <summary>
        /// Poèet stupòù volnosti
        /// </summary>
        int DegreesOfFreedom { get;}
    }

    /// <summary>
    /// Rozhraní pro tøídìní objektù
    /// </summary>
    public interface ISortable {
        /// <summary>
        /// Jednoduché tøídìní
        /// </summary>
        object Sort();

        /// <summary>
        /// Jednoduché tøídìní sestupnì
        /// </summary>
        object SortDesc();

        /// <summary>
        /// Tøídìní s klíèi
        /// </summary>
        /// <param name="keys">Klíèe</param>
        object Sort(Vector keys);

        /// <summary>
        /// Tøídìní s klíèi sestupnì
        /// </summary>
        /// <param name="keys">Klíèe</param>
        object SortDesc(Vector keys);

        /// <summary>
        /// Délka - poèet objektù k setøídìní
        /// </summary>
        int Length { get;}
    }

    /// <summary>
    /// Interface pro všelijaký výstup
    /// </summary>
    public interface IOutputWriter {
        /// <summary>
        /// Vypíše daný objekt
        /// </summary>
        /// <param name="o">Objekt</param>
        void Write(object o);

        /// <summary>
        /// Vypíše daný objekt a zalomí øádku
        /// </summary>
        /// <param name="o">Objekt</param>
        void WriteLine(object o);

        /// <summary>
        /// Zalomí øádku
        /// </summary>
        void WriteLine();

        /// <summary>
        /// Vyèistí výstup
        /// </summary>
        void Clear();
    }
}
