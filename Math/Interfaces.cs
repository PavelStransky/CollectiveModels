using System;
using System.IO;

namespace PavelStransky.Math {
    /// <summary>
    /// Deleg�t - re�ln� funkce re�ln� prom�nn�
    /// </summary>
    public delegate double RealFunction(double x);

    /// <summary>
    /// Deleg�t - Vektorov� funkce vektorov� prom�nn�
    /// </summary>
    /// <param name="rightSide">Vypo��tan� prav� strana</param>
    /// <param name="x">Hodnoty parametr�</param>
    public delegate Vector VectorFunction(Vector x);

    /// <summary>
    /// Deleg�t - Skal�rn� funkce vektorov� prom�nn�
    /// </summary>
    /// <param name="x">Hodnoty parametr�</param>
    public delegate double ScalarFunction(Vector x);

    /// <summary>
    /// Deleg�t - Jakobi�n transformace
    /// </summary>
    /// <param name="x">Sou�adnice x</param>
    public delegate Matrix Jacobian(Vector x);

    /// <summary>
    /// Metody RK
    /// </summary>
    public enum RungeKuttaMethods { Normal, Energy, Adapted }

    /// <summary>
    /// Rozhran� pro export dat do souboru
    /// </summary>
    public interface IExportable {
        /// <summary>
        /// Ulo�� data do souboru
        /// </summary>
        void Export(Export export);

        /// <summary>
        /// Na�te data
        /// </summary>
        void Import(Import import);
    }

    /// <summary>
    /// Interface pro kvantov� syst�m
    /// </summary>
    public interface IQuantumSystem {
        /// <summary>
        /// Vlastn� hodnota
        /// </summary>
        double[] EigenValue { get;}

        /// <summary>
        /// Vlastn� vektor
        /// </summary>
        Vector[] EigenVector { get;}

        /// <summary>
        /// Matice s hustotou vlastn� funkce
        /// </summary>
        /// <param name="n">��slo vlastn� funkce</param>
        /// <param name="range">Oblast hodnot k zhobrazen� (se�azen� ve tvaru minx, maxx, numx, ...)</param>
        Matrix DensityMatrix(int n, Vector range);
    }

    /// <summary>
    /// Interface pro dynamick� syst�m
    /// </summary>
    public interface IDynamicalSystem {
        /// <summary>
        /// Energie syst�mu
        /// </summary>
        /// <param name="x">Sou�adnice a rychlosti</param>
        double E(Vector x);

        /// <summary>
        /// Jakobi�n
        /// </summary>
        /// <param name="x">Sou�adnice a rychlosti</param>
        /// <returns></returns>
        Matrix Jacobian(Vector x);

        /// <summary>
        /// Prav� strana pohybov�ch rovnic
        /// </summary>
        /// <param name="x">Sou�adnice a rychlosti</param>
        /// <returns></returns>
        Vector Equation(Vector x);

        /// <summary>
        /// Generuje po��te�n� podm�nky s danou energi�
        /// </summary>
        /// <param name="e">Energie</param>
        Vector IC(double e);

        /// <summary>
        /// Generuje po��te�n� podm�nky s danou energi� a s dan�m �hlov�m momentem
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="l">�hlov� moment</param>
        Vector IC(double e, double l);

        /// <summary>
        /// Generuje hybnosti do po��te�n�ch podm�nek
        /// </summary>
        /// <param name="ic">Sou�adnice po��te�n�ch podm�nek</param>
        /// <param name="e">Energie</param>
        /// <returns>True, pokud se generov�n� poda�ilo</returns>
        bool IC(Vector ic, double e);

        /// <summary>
        /// Hranice kinematicky dostupn� oblasti na osach x, y, vx, vy
        /// </summary>
        /// <param name="e">Energie</param>
        Vector Bounds(double e);

        /// <summary>
        /// Po�et stup�� volnosti
        /// </summary>
        int DegreesOfFreedom { get;}
    }

    /// <summary>
    /// Rozhran� pro t��d�n� objekt�
    /// </summary>
    public interface ISortable {
        /// <summary>
        /// Jednoduch� t��d�n�
        /// </summary>
        object Sort();

        /// <summary>
        /// Jednoduch� t��d�n� sestupn�
        /// </summary>
        object SortDesc();

        /// <summary>
        /// T��d�n� s kl��i
        /// </summary>
        /// <param name="keys">Kl��e</param>
        object Sort(Vector keys);

        /// <summary>
        /// T��d�n� s kl��i sestupn�
        /// </summary>
        /// <param name="keys">Kl��e</param>
        object SortDesc(Vector keys);

        /// <summary>
        /// D�lka - po�et objekt� k set��d�n�
        /// </summary>
        int Length { get;}
    }

    /// <summary>
    /// Interface pro v�elijak� v�stup
    /// </summary>
    public interface IOutputWriter {
        /// <summary>
        /// Vyp�e dan� objekt
        /// </summary>
        /// <param name="o">Objekt</param>
        void Write(object o);

        /// <summary>
        /// Vyp�e dan� objekt a zalom� ��dku
        /// </summary>
        /// <param name="o">Objekt</param>
        void WriteLine(object o);

        /// <summary>
        /// Zalom� ��dku
        /// </summary>
        void WriteLine();

        /// <summary>
        /// Vy�ist� v�stup
        /// </summary>
        void Clear();
    }
}
