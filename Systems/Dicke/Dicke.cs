﻿using System;
using System.IO;
using System.Collections;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
    /// <summary>
    /// Dicke - Tavis-Cummings system (Bastarrachea, Lerma, Hirsch, 2014)
    /// </summary>
    public class Dicke : IExportable {
        // Parameters of the model
        private double omega0, omega, gamma, j, delta;

        /// <summary>
        /// Excitation energy of the atomic part
        /// </summary>
        public double Omega0 { get { return this.omega0; } }

        /// <summary>
        /// Frequency of the radiation mode omega
        /// </summary>
        public double Omega { get { return this.omega; } }

        /// <summary>
        /// Coupling constant (interaction parameter)
        /// </summary>
        public double Gamma { get { return this.gamma; } }

        /// <summary>
        /// Critical value of the coupling constant
        /// </summary>
        public double GammaC {
            get {
                // This probably works ONLY for delta=0,1
                return System.Math.Sqrt(this.omega * this.omega0) / (1.0 + this.delta);
            }
        }

        /// <summary>
        /// Minimum energy
        /// </summary>
        public double EMin {
            get{
                if(this.gamma <= this.GammaC)
                    return -this.omega0 * this.j;
                else {
                    double gammaC2 = this.GammaC; gammaC2 *= gammaC2;
                    double gamma2 = this.gamma * this.gamma;
                    return -0.5 * this.omega0 * this.j * (gammaC2 / gamma2 + gamma2 / gammaC2);
                }
            }
        }

        /// <summary>
        /// Total angular momentum
        /// </summary>
        public double J { get{ return this.j; } }

        /// <summary>
        /// Constant distinguishing between Tavis-Cummings (Delta=0) and Dicke (Delta=1) models
        /// </summary>
        public double Delta { get { return this.delta; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Dicke(double omega0, double omega, double gamma, double j, double delta) {
            this.omega0 = omega0;
            this.omega = omega;
            this.gamma = gamma;
            this.j = j;
            this.delta = delta;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        protected Dicke() { }

        #region Implementace IExportable
        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public virtual void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.omega0, "Omega0");
            param.Add(this.omega, "Omega");
            param.Add(this.gamma, "Gamma");
            param.Add(this.j, "J");
            param.Add(this.delta, "Delta");

            param.Export(export);
        }

        /// <summary>
        /// Načte výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public Dicke(Core.Import import) {
            IEParam param = new IEParam(import);

            this.omega0 = (double)param.Get(1.0);
            this.omega = (double)param.Get(1.0);
            this.gamma = (double)param.Get(0.0);
            this.j = (double)param.Get(1.0);
            this.delta = (double)param.Get(0.0);
        }
        #endregion

        /// <summary>
        /// Potential (momenta = 0)
        /// </summary>
        /// <param name="q">Coordinate of the radiation field approximated by the Harmonic Oscillaror</param>
        /// <param name="phi">Azimutal angle of the vector J</param>
        public double V(double q, double phi) {
            return this.omega / 2.0 * q * q + gamma * System.Math.Sqrt(this.j) * (1.0 + this.delta) * q * System.Math.Cos(phi);
        }

        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("Omega0 = {0,10:#####0.000}\nOmega  = {1,10:#####0.000}\nGamma  = {2,10:#####0.000}\nJ     = {3,10:#####0.000}", this.omega0, this.omega, this.gamma, this.j));
            s.Append(string.Format("\nDelta  = {0,10:#####0.000}\n", this.delta));
            s.Append(string.Format("\nGammaC = {0,10:#####0.000}", this.GammaC));
            s.Append(string.Format("\nEMin   = {0,10:#####0.000}", this.EMin));
            return s.ToString();
        }
    }
}