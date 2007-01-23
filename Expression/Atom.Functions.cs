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
			binaryOperators.Add(new BinaryOperators.ArrayAdd());
			binaryOperators.Add(new BinaryOperators.Power());
			binaryOperators.Add(new BinaryOperators.ArrayGen());

			functions = new Functions.FunctionDefinitions();
			functions.Add(new Functions.AddItem());
			functions.Add(new Functions.FnArray());
			functions.Add(new Functions.Clear());
			functions.Add(new Functions.CM());
			functions.Add(new Functions.DropColumns());
			functions.Add(new Functions.DropRows());
			functions.Add(new Functions.EigenSystem());
			functions.Add(new Functions.Eval());
			functions.Add(new Functions.Exit());
            functions.Add(new Functions.Save());
			functions.Add(new Functions.FnExport());
			functions.Add(new Functions.GenArray());
			functions.Add(new Functions.GetColumns());
			functions.Add(new Functions.GetRows());
			functions.Add(new Functions.FnGraph());
			functions.Add(new Functions.FnImport());
			functions.Add(new Functions.Join());
			functions.Add(new Functions.Length());
			functions.Add(new Functions.SLength());
			functions.Add(new Functions.Norm());
			functions.Add(new Functions.Normalize());
			functions.Add(new Functions.SNormalize());
			functions.Add(new Functions.Objects());
			functions.Add(new Functions.Permute());
			functions.Add(new Functions.Split());
			functions.Add(new Functions.SplitColumns());
			functions.Add(new Functions.SplitRows());
			functions.Add(new Functions.String());
			functions.Add(new Functions.Transpose());
			functions.Add(new Functions.FnVector());
            functions.Add(new Functions.FnMatrix());
			functions.Add(new Functions.Windows());
			functions.Add(new Functions.SwapDim());
			functions.Add(new Functions.FnFor());
			functions.Add(new Functions.FnIf());
			functions.Add(new Functions.FnPoint());
			functions.Add(new Functions.Histogram());
			functions.Add(new Functions.CumulHistogram());
			functions.Add(new Functions.FnRegression());
			functions.Add(new Functions.FnPolynom());
			functions.Add(new Functions.FnPointVector());
			functions.Add(new Functions.Sort());
			functions.Add(new Functions.SortDesc());
			functions.Add(new Functions.Min());
			functions.Add(new Functions.Max());
			functions.Add(new Functions.MinAbs());
			functions.Add(new Functions.MaxAbs());
			functions.Add(new Functions.MinIndex());
			functions.Add(new Functions.MaxIndex());
			functions.Add(new Functions.MinAbsIndex());
			functions.Add(new Functions.MaxAbsIndex());
			functions.Add(new Functions.GetX());
			functions.Add(new Functions.GetY());
			functions.Add(new Functions.Spacing());
			functions.Add(new Functions.Mean());
			functions.Add(new Functions.Var());
			functions.Add(new Functions.Integrate());
			functions.Add(new Functions.Wigner());
			functions.Add(new Functions.FnType());
            functions.Add(new Functions.SwapXY());
            functions.Add(new Functions.Sum());
            functions.Add(new Functions.SumAbs());
            functions.Add(new Functions.SumSquare());
            functions.Add(new Functions.FnPrint());
            functions.Add(new Functions.PrintClear());
            functions.Add(new Functions.PrintLine());
            functions.Add(new Functions.TimeNow());
            functions.Add(new Functions.FnTime());
            functions.Add(new Functions.FnRand());
            functions.Add(new Functions.FnInt());
            functions.Add(new Functions.FnDouble());
            functions.Add(new Functions.Spectrum());
            functions.Add(new Functions.FRegMatrixEval());
            functions.Add(new Functions.Show());
            functions.Add(new Functions.EVNumDiff());
            functions.Add(new Functions.NumNonzeroItems());
            functions.Add(new Functions.BandWidth());

            // Kontext
            functions.Add(new Functions.FnContext());
            functions.Add(new Functions.GetContext());
            functions.Add(new Functions.UseContext());
            functions.Add(new Functions.SetContext());

			functions.Add(new Functions.PC());
			functions.Add(new Functions.Symmetry());

			// Standardní matematické funkce
			functions.Add(new Functions.ArcTan());
            functions.Add(new Functions.Abs());
            functions.Add(new Functions.Log());
            functions.Add(new Functions.Sqrt());
            functions.Add(new Functions.Laguerre());
            functions.Add(new Functions.Factorial());
            functions.Add(new Functions.BC());

			// Funkce vyžadující functions
			functions.Add(new Functions.FNames(functions));
			functions.Add(new Functions.Use(functions));
			functions.Add(new Functions.FnHelp(functions));
			functions.Add(new Functions.FullHelp(functions));

            // Funkce GCM
            functions.Add(new Functions.CGCM());
            functions.Add(new Functions.ExtendedCGCM1());
            functions.Add(new Functions.ExtendedCGCM2());
            functions.Add(new Functions.CGCMJ());
            functions.Add(new Functions.QGCM());
            functions.Add(new Functions.Equipotential());
            functions.Add(new Functions.TrajectoryM());
            functions.Add(new Functions.TrajectoryP());
            functions.Add(new Functions.InitialCondition());
            functions.Add(new Functions.Poincare());
            functions.Add(new Functions.FnSALI());
            functions.Add(new Functions.SALIR());
            functions.Add(new Functions.SALIG());
            functions.Add(new Functions.EnergyLevels());
            functions.Add(new Functions.Energy());
            functions.Add(new Functions.Bounds());
            functions.Add(new Functions.LHOQGCMC());
            functions.Add(new Functions.LHOQGCMR());
            functions.Add(new Functions.LHOQGCMRF());
            functions.Add(new Functions.LHOQGCMRL());
            functions.Add(new Functions.DensityMatrix());
            functions.Add(new Functions.ComputeSpectrum());
            functions.Add(new Functions.HamiltonianMatrix());
            functions.Add(new Functions.HamiltonianMatrixSize());
            functions.Add(new Functions.EValues());
            functions.Add(new Functions.EVectors());

            // Funkce IBM
            functions.Add(new Functions.CIBM());

            // Henon - Heiles
            functions.Add(new Functions.HH());

            // Aliasy
            functions.Add(new Functions.CreateGraph());
        }

        protected static readonly Operators.Operators unaryOperators;
        protected static readonly Operators.Operators binaryOperators;
        protected static readonly Functions.FunctionDefinitions functions;
    }
}
