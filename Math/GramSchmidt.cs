using System;
using System.Collections;

using PavelStransky.Core;

namespace PavelStransky.Math {
    /// <summary>
    /// Tøída pro Gram-Schmidtovu ortogonalizaci
    /// </summary>
    public class GramSchmidt {
        private Vector[] vectors;
        private Vector[] orthogonal;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="vectors">Vstupní data</param>
        public GramSchmidt(Vector[] vectors) {
            int length = vectors.Length;
            for(int i = 0; i < length; i++)
                if(vectors[i].Length != length)
                    throw new MathException(Messages.EMGSLengths, 
                        string.Format(Messages.EMGSLengthsDetail, length, i, vectors[i].Length));

            this.vectors = vectors;
            this.Compute();
        }

        /// <summary>
        /// Provede výpoèet
        /// </summary>
        public void Compute() {
            int length = this.vectors.Length;

            this.orthogonal = new Vector[length];
            for(int i = 0; i < length; i++) {
                Vector u = this.vectors[i].Clone() as Vector;
                for(int j = 0; j < i; j++) {
                    Vector v = this.orthogonal[j];
                    u -= ((this.vectors[i] * v) / v.SquaredEuklideanNorm()) * v;
                }
                this.orthogonal[i] = u;
            }
        }

        /// <summary>
        /// Ortogonální vektory
        /// </summary>
        public Vector[] Orthogonal() {
            return this.orthogonal;
        }

        /// <summary>
        /// Ortonormální vektory
        /// </summary>
        public Vector[] Orthonormal() {
            int length = this.vectors.Length;

            Vector[]result = new Vector[length];
            for(int i = 0; i < length; i++) {
                result[i] = this.orthogonal[i].EuklideanNormalization();
            }

            return result;
        }

        /// <summary>
        /// Vrátí normy vektorù
        /// </summary>
        public Vector Norms() {
            int length = this.vectors.Length;

            Vector result = new Vector(length);
            for(int i = 0; i < length; i++)
                result[i] = this.orthogonal[i].EuklideanNorm();

            return result;
        }
    }
}

