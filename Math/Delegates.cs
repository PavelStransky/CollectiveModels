using System;
using System.IO;

namespace PavelStransky.Math {
    /// <summary>
    /// Delegát - reálná funkce reálné promìnné
    /// </summary>
    public delegate double RealFunction(double x);

    /// <summary>
    /// Delegate - a real function of a real parameter and other parameters
    /// </summary>
    public delegate double RealFunctionWithParams(double x, params object[] p);

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
}
