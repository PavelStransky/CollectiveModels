using System;
using System.IO;

namespace PavelStransky.Math {
    /// <summary>
    /// Deleg�t - re�ln� funkce re�ln� prom�nn�
    /// </summary>
    public delegate double RealFunction(double x);

    /// <summary>
    /// Delegate - a real function of a real parameter and other parameters
    /// </summary>
    public delegate double RealFunctionWithParams(double x, params object[] p);

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
}
