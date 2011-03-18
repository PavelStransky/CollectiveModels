using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Math {
    public class Strutinsky:IExportable {
        private Vector energy;
        private Vector gamma;   // Range for each energy
        private int degree;     // Degree of the Laguerre polynomial

        private Vector cache;
        private Vector cachex;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="energy">Energy levels</param>
        /// <param name="gamma">Gamma smoothing parameter for each level</param>
        /// <param name="degree">Degree of the used Laguerre polynomial</param>
        public Strutinsky(Vector energy, Vector gamma, int degree) {
            this.energy = (Vector)energy.Sort();
            this.gamma = gamma;
            this.degree = degree;

            this.BuildCache();
        }

        /// <summary>
        /// Preparing cache
        /// </summary>
        private void BuildCache() {
            int length = (int)(max / precision) + 1;
            this.cache = new Vector(length);
            this.cachex = new Vector(length);

            double oldy = this.Function(0.0);
            double oldyx = 0.0;

            for(int i = 1; i < length; i++) {
                double x = precision * i;
                double y = this.Function(x);
                this.cache[i] = this.cache[i - 1] + precision * (y + oldy) / 2.0;
                oldy = y;

                y *= x;
                this.cachex[i] = this.cachex[i - 1] + precision * (y + oldyx) / 2.0;
                oldyx = y;
            }

            double maxCache = this.cachex.LastItem;
            for(int i = 0; i < length; i++)
                this.cachex[i] -= maxCache;
        }

        /// <summary>
        /// Smoothing function
        /// </summary>
        private double Function(double x) {
            x *= x;
            if(x > 100.0)
                return 0.0;
            return SpecialFunctions.Laguerre(x, this.degree, 0.5) * System.Math.Exp(-x) / System.Math.Sqrt(System.Math.PI);
        }

        /// <summary>
        /// Class wrapping a function to calculate the chemical potential lambda
        /// </summary>
        private class ChPBisectionFunction {
            private int A;
            private Vector energy;
            private Vector gamma;
            private Vector cache;
            private double maxCache;

            public ChPBisectionFunction(Vector energy, Vector gamma, Vector cache, int A) {
                this.energy = energy;
                this.gamma = gamma;
                this.cache = cache;
                this.A = A;

                this.maxCache = this.cache.LastItem;
            }

            public double BisectionFunction(double lambda) {
                int length = this.energy.Length;
                double result = 0.0;

                for(int i = 0; i < length; i++)
                    result += this.OccupationNumber(lambda, i);

                return result - this.A;
            }

            /// <summary>
            /// Occupation number for a level of a given index
            /// </summary>
            /// <param name="lambda">Chemical potential</param>
            /// <param name="i">Index a level</param>
            public double OccupationNumber(double lambda, int i) {
                double x = (lambda - this.energy[i]) / this.gamma[i];
                int xi = (int)(x / precision);
                if(xi <= -this.cache.Length)
                    return 0.0;
                else if(xi < 0)
                    return this.maxCache - this.cache[-xi];
                else if(xi < this.cache.Length)
                    return this.maxCache + this.cache[xi];
                else
                    return this.maxCache + this.maxCache;
            }
        }

        /// <summary>
        /// Calculates the chemical potential for a given mass number
        /// </summary>
        /// <param name="A">Mass number</param>
        /// <returns></returns>
        public double ChemicalPotential(int A) {
            ChPBisectionFunction bf = new ChPBisectionFunction(this.energy, this.gamma, this.cache, A);
            Bisection b = new Bisection(bf.BisectionFunction);
            return b.Solve(this.energy.Min(), this.energy.Max(), precision);
        }

        /// <summary>
        /// Calculates the occupation numbers
        /// </summary>
        /// <param name="A">Mass number</param>
        public Vector OccupationNumbers(int A) {
            ChPBisectionFunction bf = new ChPBisectionFunction(this.energy, this.gamma, this.cache, A);
            Bisection b = new Bisection(bf.BisectionFunction);

            double lambda = b.Solve(this.energy.Min(), this.energy.Max(), precision);
            int length = this.energy.Length;

            Vector result = new Vector(length);
            for(int i = 0; i < length; i++)
                result[i] = bf.OccupationNumber(lambda, i);

            return result;
        }

        /// <summary>
        /// Smooth part of the level density
        /// </summary>
        /// <param name="x">Point</param>
        public double SmoothLevelDensity(double x) {
            int length = this.energy.Length;
            
            double result = 0;
            for(int i = 0; i < length; i++) {                
                double y = (this.energy[i] - x) / this.gamma[i];
                result += this.Function(y) / this.gamma[i];
            }

            return result;
        }

        /// <summary>
        /// Calculates the plateau condition for a given mass number
        /// </summary>
        /// <param name="A">Mass number</param>
        public double PlateauCondition(int A) {
            double lambda = this.ChemicalPotential(A);
            int length = this.energy.Length;

            double result = 0.0;
            for(int i = 0; i < length; i++) {
                double x = (lambda - this.energy[i]) / this.gamma[i];
                int xi = (int)(x / precision);

                if(xi <= -this.cachex.Length)
                    continue;
                else if(xi < 0)
                    result += this.cachex[-xi];
                else if(xi < this.cachex.Length)
                    result += this.cachex[xi];
                else
                    continue;
            }

            return result;
        }

        /// <summary>
        /// Calculates shell corrections for a given mass number
        /// </summary>
        /// <param name="A">Mass number</param>
        public double ShellCorrection(int A) {
            Vector on = this.OccupationNumbers(A);
            int length = this.energy.Length;

            double result = 0.0;
            for(int i = 0; i < length; i++) {
                result -= on[i] * this.energy[i];

                if(i < A)
                    result += this.energy[i];
            }

            return result;
        }

        /// <summary>
        /// Export
        /// </summary>
        public void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.energy, "Energy");
            param.Add(this.gamma, "Gamma");
            param.Add(this.degree, "Degree");
            param.Export(export);
        }

        /// <summary>
        /// Constructor for import
        /// </summary>
        public Strutinsky(Core.Import import) {
            IEParam param = new IEParam(import);
            this.energy = param.Get() as Vector;
            this.gamma = param.Get() as Vector;
            this.degree = (int)param.Get();

            this.BuildCache();
        }

        private const double precision = 1E-5;
        private const double max = 8.0;
    }
}
