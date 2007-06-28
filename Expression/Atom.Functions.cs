using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Expression {
    // Definice funkcí pro tøídu Atom
    public abstract partial class Atom {
        /// <summary>
		/// Statický konstruktor
		/// </summary>
		static Atom() {
			unaryOperators = new Operators.Operators();
			unaryOperators.Add(new UnaryOperators.Plus());
			unaryOperators.Add(new UnaryOperators.Minus());

			binaryOperators = new Operators.Operators();
			binaryOperators.Add(new BinaryOperators.Plus());
			binaryOperators.Add(new BinaryOperators.Minus());
			binaryOperators.Add(new BinaryOperators.Times());
			binaryOperators.Add(new BinaryOperators.Divide());
			binaryOperators.Add(new BinaryOperators.Equals());
			binaryOperators.Add(new BinaryOperators.NotEquals());
			binaryOperators.Add(new BinaryOperators.GEquals());
			binaryOperators.Add(new BinaryOperators.LEquals());
			binaryOperators.Add(new BinaryOperators.Greater());
			binaryOperators.Add(new BinaryOperators.Lesser());
			binaryOperators.Add(new BinaryOperators.Interval());
			binaryOperators.Add(new BinaryOperators.Power());
            binaryOperators.Add(new BinaryOperators.BoolOr());
            binaryOperators.Add(new BinaryOperators.BoolAnd());
            binaryOperators.Add(new BinaryOperators.Join());

			functions = new Functions.FunctionDefinitions();
            functions.Add(new Functions.Exit());
			functions.Add(new Functions.FnExport());
			functions.Add(new Functions.GetColumns());
			functions.Add(new Functions.GetRows());
			functions.Add(new Functions.FnImport());
			functions.Add(new Functions.Join());
			functions.Add(new Functions.Length());
			functions.Add(new Functions.FnFor());
			functions.Add(new Functions.FnIf());
			functions.Add(new Functions.Histogram());
			functions.Add(new Functions.CumulHistogram());
			functions.Add(new Functions.GetX());
			functions.Add(new Functions.GetY());
			functions.Add(new Functions.Integrate());
            functions.Add(new Functions.VRenorm());
            functions.Add(new Functions.Norm());
            functions.Add(new Functions.Spacing());

            // Matrix
            functions.Add(new Functions.BandWidth());
            functions.Add(new Functions.CM());
            functions.Add(new Functions.DropColumns());
            functions.Add(new Functions.DropRows());
            functions.Add(new Functions.FnJacobi());
            functions.Add(new Functions.SetDiagonal());
            functions.Add(new Functions.SetNonDiagonal());

            // Unsorted
            functions.Add(new Functions.IntervalA());
            functions.Add(new Functions.IntervalV());
            functions.Add(new Functions.IntervalPV());
            functions.Add(new Functions.FnTime());
            functions.Add(new Functions.TimeNow());
            functions.Add(new Functions.FnRegression());
            functions.Add(new Functions.RemoveBadPoints());
            functions.Add(new Functions.FFTSpectrum());

            // List
            functions.Add(new Functions.FnAdd());
            functions.Add(new Functions.AddGlobal());

            // Program
            functions.Add(new Functions.FnPrint());
            functions.Add(new Functions.PrintClear());
            functions.Add(new Functions.PrintLine());
            functions.Add(new Functions.Save());
            functions.Add(new Functions.SafeValue());

            // Test
            functions.Add(new Functions.TestArray());
            functions.Add(new Functions.RWF5DLHO());

            // Graphics
            functions.Add(new Functions.Show());

            // Types
            functions.Add(new Functions.FnNew());
            functions.Add(new Functions.FnArray());
            functions.Add(new Functions.FnVector());
            functions.Add(new Functions.FnGraph());
            functions.Add(new Functions.FnDouble());
            functions.Add(new Functions.FnList());
            functions.Add(new Functions.FnInt());
            functions.Add(new Functions.FnPoint());
            functions.Add(new Functions.FnPointVector());
            functions.Add(new Functions.FnString());
            functions.Add(new Functions.ToArray());
            functions.Add(new Functions.FnType());
            functions.Add(new Functions.Deflate());

            functions.Add(new Functions.MatrixRow());
            functions.Add(new Functions.MatrixColumn());

            // Kontext
            functions.Add(new Functions.Clear());
            functions.Add(new Functions.ClearGlobal());
            functions.Add(new Functions.FnContext());
            functions.Add(new Functions.GetContext());
            functions.Add(new Functions.UseContext());
            functions.Add(new Functions.SetContext());
            functions.Add(new Functions.GetVar());
            functions.Add(new Functions.SetGlobalVar());
            functions.Add(new Functions.GetGlobalVar());
            functions.Add(new Functions.GetGlobalContext());
            functions.Add(new Functions.SetGlobalContext());

            // Quantum
            functions.Add(new Functions.LHOQGCMC());
            functions.Add(new Functions.LHOQGCMR());
            functions.Add(new Functions.LHOQGCMRF());
            functions.Add(new Functions.LHOQGCMRL());
            functions.Add(new Functions.LHOQGCMRLO());
            functions.Add(new Functions.LHOQGCMRLE());
            functions.Add(new Functions.LHOQGCM5D());
            functions.Add(new Functions.DensityMatrix());
            functions.Add(new Functions.ComputeSpectrum());
            functions.Add(new Functions.HamiltonianMatrix());
            functions.Add(new Functions.HamiltonianMatrixSize());
            functions.Add(new Functions.EValues());
            functions.Add(new Functions.EVectors());
            functions.Add(new Functions.HamiltonianMatrixTrace());
            functions.Add(new Functions.SecondInvariant());
            functions.Add(new Functions.Parity());

            // Sorting
            functions.Add(new Functions.Sort());
            functions.Add(new Functions.SortDesc());

            // Summing
            functions.Add(new Functions.Sum());
            functions.Add(new Functions.SumAbs());
            functions.Add(new Functions.SumSquare());

            // MinMax
            functions.Add(new Functions.Max());
            functions.Add(new Functions.Min());
            functions.Add(new Functions.MinAbs());
            functions.Add(new Functions.MaxAbs());
            functions.Add(new Functions.MinIndex());
            functions.Add(new Functions.MaxIndex());
            functions.Add(new Functions.MinAbsIndex());
            functions.Add(new Functions.MaxAbsIndex());

            // Standardní matematické funkce
			functions.Add(new Functions.ArcTan());
            functions.Add(new Functions.Abs());
            functions.Add(new Functions.Log());
            functions.Add(new Functions.Sqrt());
            functions.Add(new Functions.Laguerre());
            functions.Add(new Functions.Legendre());
            functions.Add(new Functions.Factorial());
            functions.Add(new Functions.BC());
            functions.Add(new Functions.Sin());
            functions.Add(new Functions.Pi());
            functions.Add(new Functions.FnPolynom());
            functions.Add(new Functions.Wigner());
            functions.Add(new Functions.Poisson());

			// Funkce vyžadující functions
			functions.Add(new Functions.FNames(functions));
			functions.Add(new Functions.Use(functions));
			functions.Add(new Functions.FnHelp(functions));
			functions.Add(new Functions.FullHelp(functions));

            // Dynamics funkce 
            functions.Add(new Functions.Bounds());
            functions.Add(new Functions.CGCM());
            functions.Add(new Functions.CGCMJ());
            functions.Add(new Functions.CIBM());
            functions.Add(new Functions.Equipotential());
            functions.Add(new Functions.Energy());
            functions.Add(new Functions.ExtendedCGCM1());
            functions.Add(new Functions.ExtendedCGCM2());
            functions.Add(new Functions.HH());
            functions.Add(new Functions.InitialCondition());
            functions.Add(new Functions.Poincare());
            functions.Add(new Functions.PotentialRoots());
            functions.Add(new Functions.FnSALI());
            functions.Add(new Functions.SALIR());
            functions.Add(new Functions.SALIG());
            functions.Add(new Functions.TrajectoryM());
            functions.Add(new Functions.TrajectoryP());
        }

        protected static readonly Operators.Operators unaryOperators;
        protected static readonly Operators.Operators binaryOperators;
        protected static readonly Functions.FunctionDefinitions functions;
    }
}
