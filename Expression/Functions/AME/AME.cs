using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Expression {
    public class AME: IExportable {
        public class AMEItem: IExportable {
            private int n, z;           // Neutron and proton number
            private string symbol;      // Text symbol

            private string orig;        // Origin

            // Mass excess with error;
            private double massExcess, eMassExcess;
            private bool massExcessExperiment;

            // Binding energy
            private double bindingEnergy, eBindingEnergy;
            private bool bindingEnergyExperiment;

            // Beta decay
            private bool betaDecayAvailable;
            private double betaDecay, eBetaDecay;
            private bool betaDecayExperiment;

            private double mass, eMass;
            private bool massExperiment;

            /// <summary>
            /// Import
            /// </summary>
            public AMEItem(Import import) {
                IEParam param = new IEParam(import);

                this.n = (int)param.Get();
                this.z = (int)param.Get();

                this.symbol = (string)param.Get();
                this.orig = (string)param.Get();

                this.massExcess = (double)param.Get();
                this.eMassExcess = (double)param.Get();
                this.massExcessExperiment = (bool)param.Get();

                this.bindingEnergy = (double)param.Get();
                this.eBindingEnergy = (double)param.Get();
                this.bindingEnergyExperiment = (bool)param.Get(); 
                
                this.betaDecay = (double)param.Get();
                this.eBetaDecay = (double)param.Get();
                this.betaDecayExperiment = (bool)param.Get();
                this.betaDecayAvailable = (bool)param.Get();

                this.mass = (double)param.Get();
                this.eMass = (double)param.Get();
                this.massExperiment = (bool)param.Get();
            }

            /// <summary>
            /// Export
            /// </summary>
            public void Export(Export export) {
                IEParam param = new IEParam();
                param.Add(this.n, "N");
                param.Add(this.z, "Z");

                param.Add(this.symbol, "Symbol");
                param.Add(this.orig, "Origin");
                
                param.Add(this.massExcess, "Mass Excess");
                param.Add(this.eMassExcess, "Mass Excess Error");
                param.Add(this.massExcessExperiment, "Mass Excess From Experiment");

                param.Add(this.bindingEnergy, "Binding Energy");
                param.Add(this.eBindingEnergy, "Binding Energy Error");
                param.Add(this.bindingEnergyExperiment, "Binding Energy From Experiment");

                param.Add(this.betaDecay, "Beta Decay Energy");
                param.Add(this.eBetaDecay, "Beta Decay Energy Error");
                param.Add(this.betaDecayExperiment, "Beta Decay Energy From Experiment");
                param.Add(this.betaDecayAvailable, "Beta Decay Energy Available");

                param.Add(this.mass, "Atomic Mass");
                param.Add(this.eMass, "Atomic Mass Error");
                param.Add(this.massExperiment, "Atomic Mass From Experiment");

                param.Export(export);
            }

            public AMEItem(string line) {
                // Remove the first two characters (Fortran character control)
                line = line.Substring(2);

                // Remove next two characters (N-Z)
                line = line.Substring(2);

                this.n = int.Parse(line.Substring(0, 5));
                line = line.Substring(5);

                this.z = int.Parse(line.Substring(0, 5));
                line = line.Substring(5);

                // Remove next 5 characters (A)
                line = line.Substring(5);

                this.symbol = line.Substring(0, 3).Trim();
                line = line.Substring(3);

                this.orig = line.Substring(0, 5).Trim();
                line = line.Substring(5);

                // Mass Excess
                this.massExcess = double.Parse(line.Substring(0, 8));
                line = line.Substring(8);
                this.massExcessExperiment = line.Substring(0, 1) == ".";
                line = line.Substring(1);
                this.massExcess += double.Parse(string.Format("0.{0}", line.Substring(0, 5)));
                line = line.Substring(5);

                this.eMassExcess = double.Parse(line.Substring(0, 5));
                line = line.Substring(6);
                this.eMassExcess += double.Parse(string.Format("0.{0}", line.Substring(0, 5)));
                line = line.Substring(5);

                // Binding energy
                this.bindingEnergy = double.Parse(line.Substring(0, 7));
                line = line.Substring(7);
                this.bindingEnergyExperiment = line.Substring(0, 1) == ".";
                line = line.Substring(1);
                this.bindingEnergy += double.Parse(string.Format("0.{0}", line.Substring(0, 3)));
                line = line.Substring(3);

                this.eBindingEnergy = double.Parse(line.Substring(0, 5));
                line = line.Substring(6);
                this.eBindingEnergy += double.Parse(string.Format("0.{0}", line.Substring(0, 3)));
                line = line.Substring(3);

                // Beta decay
                line = line.Substring(3);

                if(line.Substring(0, 7).Trim() == "*") {
                    this.betaDecayAvailable = false;
                    line = line.Substring(20);
                }
                else {
                    this.betaDecayAvailable = true;

                    this.betaDecay = double.Parse(line.Substring(0, 7));
                    line = line.Substring(7);
                    this.betaDecayExperiment = line.Substring(0, 1) == ".";
                    line = line.Substring(1);
                    this.betaDecay += double.Parse(string.Format("0.{0}", line.Substring(0, 3)));
                    line = line.Substring(3);

                    this.eBetaDecay = double.Parse(line.Substring(0, 5));
                    line = line.Substring(6);
                    this.eBetaDecay += double.Parse(string.Format("0.{0}", line.Substring(0, 3)));
                    line = line.Substring(3);
                }

                // Atomic mass
                this.mass = 1E6 * double.Parse(line.Substring(0, 4));
                line = line.Substring(4);

                this.mass += double.Parse(line.Substring(0, 7));
                line = line.Substring(7);
                this.massExperiment = line.Substring(0, 1) == ".";
                line = line.Substring(1);
                this.mass += double.Parse(string.Format("0.{0}", line.Substring(0, 5)));
                line = line.Substring(5);

                this.eMass = double.Parse(line.Substring(0, 5));
                line = line.Substring(6);
                this.eMass += double.Parse(string.Format("0.{0}", line));
            }

            /// <summary>
            /// Neutron number
            /// </summary>
            public int N { get { return this.n; } }

            /// <summary>
            /// Proton number
            /// </summary>
            public int Z { get { return this.z; } }

            /// <summary>
            /// Atomic number
            /// </summary>
            public int A { get { return this.n + this.z; } }

            /// <summary>
            /// Atomic mass (keV)
            /// </summary>
            public double AtomicMass { get { return masskeV * this.mass * 1E-6; } }

            /// <summary>
            /// Nuclear mass (keV)
            /// </summary>
            public double NuclearMass { get { return this.AtomicMass - massE * this.z + this.bindingEnergy; } }

            /// <summary>
            /// Nuclear mass from experiment
            /// </summary>
            public bool NuclearMassExperiment { get { return this.massExperiment && this.bindingEnergyExperiment; } }

            /// <summary>
            /// Binding energy (keV)
            /// </summary>
            public double BindingEnergy { get { return this.bindingEnergy; } }

            /// <summary>
            /// Mass excess (keV)
            /// </summary>
            public double MassExcess { get { return this.massExcess; } }
        }

        // Data
        private AMEItem[] ameItem;

        private static int[] magicNumber;
        static AME() {
            // Magic numbers
            magicNumber = new int[8];
            magicNumber[0] = 0;
            magicNumber[1] = 2;
            magicNumber[2] = 8;
            magicNumber[3] = 20;
            magicNumber[4] = 28;
            magicNumber[5] = 50;
            magicNumber[6] = 82;
            magicNumber[7] = 126;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fName">File name</param>
        public AME(string fName) {
            FileStream f = new FileStream(fName, FileMode.Open);
            StreamReader t = new StreamReader(f);

            ArrayList items = new ArrayList();

            // Header
            for(int i = 0; i < header; i++)
                t.ReadLine();

            string s = string.Empty;
            while((s = t.ReadLine()) != null) 
                items.Add(new AMEItem(s));

            this.ameItem = new AMEItem[items.Count];
            
            int j = 0;
            foreach(AMEItem it in items)
                this.ameItem[j++] = it;
        }

        /// <summary>
        /// Maximum value of Z in all the data
        /// </summary>
        private int MaximumZ() {
            int result = 0;
            foreach(AMEItem it in this.ameItem)
                result = System.Math.Max(result, it.Z);

            return result;
        }

        /// <summary>
        /// Maximum value of N in all the data
        /// </summary>
        private int MaximumN() {
            int result = 0;
            foreach(AMEItem it in this.ameItem)
                result = System.Math.Max(result, it.N);

            return result;
        }

        /// <summary>
        /// Two-Neutron Separation Energy
        /// </summary>
        /// <param name="onlyExp">Only experimental values</param>
        public TArray SeparationEnergy2N(bool onlyExp) {
            int maxZ = this.MaximumZ();
            int maxN = this.MaximumN();

            Matrix m = new Matrix(maxZ + 1, maxN + 1);
            m.Fill(-1.0);

            foreach(AMEItem it in this.ameItem)
                if(!onlyExp || (onlyExp && it.NuclearMassExperiment))
                    m[it.Z, it.N] = it.NuclearMass;

            PointVector[] result = new PointVector[maxZ];
            for(int i = 0; i < maxZ; i++) {
                ArrayList a = new ArrayList();
                for(int j = 2; j < maxN; j++)
                    if(m[i, j] != -1.0 && m[i, j - 2] != -1.0) {
                        a.Add(new PointD(j, -m[i, j] + m[i, j - 2] + 2.0 * massN));
                    }

                int length = a.Count;
                result[i] = new PointVector(length);
                for(int j = 0; j < length; j++)
                    result[i][j] = (PointD)a[j];
            }

            return new TArray(result);
        }

        public Matrix Mass(bool onlyExp) {
            int maxZ = this.MaximumZ();
            int maxN = this.MaximumN();

            Matrix result = new Matrix(maxN + 1, maxZ + 1);
            foreach(AMEItem it in this.ameItem)
                if(!onlyExp || (onlyExp && it.NuclearMassExperiment)) {
                    result[it.N, it.Z] = it.NuclearMass;
                }

            return result;
        }

        public Matrix BindingEnergy(bool onlyExp) {
            int maxZ = this.MaximumZ();
            int maxN = this.MaximumN();

            Matrix result = new Matrix(maxN + 1, maxZ + 1);
            foreach(AMEItem it in this.ameItem)
                if(!onlyExp || (onlyExp && it.NuclearMassExperiment)) {
                    result[it.N, it.Z] = it.BindingEnergy;
                }

            return result;
        }

        public Matrix MassExcess(bool onlyExp) {
            int maxZ = this.MaximumZ();
            int maxN = this.MaximumN();

            Matrix result = new Matrix(maxN + 1, maxZ + 1);
            foreach(AMEItem it in this.ameItem)
                if(!onlyExp || (onlyExp && it.NuclearMassExperiment)) {
                    result[it.N, it.Z] = it.MassExcess;
                }

            return result;
        }

        public Matrix Isotope(bool onlyExp) {
            int maxZ = this.MaximumZ();
            int maxN = this.MaximumN();

            Matrix result = new Matrix(maxN + 1, maxZ + 1);
            foreach(AMEItem it in this.ameItem)
                if(!onlyExp || (onlyExp && it.NuclearMassExperiment)) {
                    int shellN = this.Shell(it.N);
                    int shellZ = this.Shell(it.Z);

                    if(shellN < 0 || shellZ < 0)
                        result[it.N, it.Z] = 1.0;
                    else {

                        double dn1 = it.N - magicNumber[shellN];
                        double dz1 = it.Z - magicNumber[shellZ];

                        Vector v = new Vector(4);
                        v[0] = dn1 + dz1 == 0.0 ? 0.0 : dn1 * dz1 / (dn1 + dz1);

                        if(shellN < magicNumber.Length) {
                            double dn2 = magicNumber[shellN + 1] - it.N;
                            v[1] = dn2 + dz1 == 0.0 ? 0.0 : dn2 * dz1 / (dn2 + dz1);

                            if(shellZ < magicNumber.Length) {
                                double dz2 = magicNumber[shellZ + 1] - it.Z;
                                v[2] = dn1 + dz2 == 0.0 ? 0.0 : dn1 * dz2 / (dn1 + dz2);
                                v[3] = dn2 + dz2 == 0.0 ? 0.0 : dn2 * dz2 / (dn2 + dz2);
                            }
                            else {
                                v[2] = v[0];
                                v[3] = v[1];
                            }
                        }
                        else if(shellZ < magicNumber.Length) {
                            double dz2 = magicNumber[shellZ + 1] - it.Z;
                            v[1] = dn1 + dz2 == 0.0 ? 0.0 : dn1 * dz2 / (dn1 + dz2);
                            v[2] = v[0];
                            v[3] = v[1];
                        }
                        else {
                            v[1] = v[0];
                            v[2] = v[0];
                            v[3] = v[0];
                        }

                        result[it.N, it.Z] = v.Min() + 1.0;
                    }   
                }

            return result;
        }

        /// <summary>
        /// Number of shell
        /// </summary>
        private int Shell(int i) {
            for(int j = 0; j < magicNumber.Length; j++)
                if(i < magicNumber[j])
                    return j - 1;
            return -1;
        }


        public AME(Import import) {
            IEParam param = new IEParam(import);

            int length = (int)param.Get(0);
            this.ameItem = new AMEItem[length];
            for(int i = 0; i < length; i++)
                this.ameItem[i] = (AMEItem)param.Get();
        }

        public void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.ameItem.Length, "Length");
            foreach(AMEItem it in this.ameItem)
                param.Add(it);
            param.Export(export);
        }
                
        private const int header = 39;
        private const double masskeV = 931494.0954;
        private const double massE = 510.998910;
        private const double massN = 939565.4133;
    }
}
