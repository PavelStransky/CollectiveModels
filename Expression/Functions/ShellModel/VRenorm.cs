using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// The renormalization of the potential
    /// </summary>
    public class VRenorm: Fnc {
        private DataParams dataParams = new DataParams();

        /// <summary>
        /// Constructor
        /// </summary>
        public VRenorm() {
            // Adding all the parameters
            ArrayList renormType = new ArrayList();
            renormType.Add("vlowk");
            renormType.Add("g-matrix");
            renormType.Add("no-core");
            this.dataParams.Add("type_of_renormv", renormType[0], renormType, Messages.SMType_of_renormv);

            ArrayList coulombType = new ArrayList();
            coulombType.Add("coulomb");
            coulombType.Add("no-coulomb");
            this.dataParams.Add("coulomb_included", coulombType[0], coulombType, Messages.SMCoulomb_included);

            ArrayList csbType = new ArrayList();
            csbType.Add("csb");
            csbType.Add("no-csb");
            this.dataParams.Add("csb_choice", csbType[0], csbType, Messages.SMCsb_choice);

            ArrayList cibType = new ArrayList();
            cibType.Add("cib");
            cibType.Add("no-cib");
            this.dataParams.Add("cib_choice", cibType[0], cibType, Messages.SMCib_choice);

            this.dataParams.Add("mass_nucleus", 16.0, typeof(double), Messages.SMMass_nucleus);
            this.dataParams.Add("hbar_omega", 14.0, typeof(double), Messages.SMHbar_omega);

            this.dataParams.Add("jmin", 0, typeof(int), Messages.SMJminmax);
            this.dataParams.Add("jmax", 3, typeof(int), Messages.SMJminmax);

            ArrayList intType = new ArrayList();
            intType.Add("n3lo");
            intType.Add("argonnev18");
            intType.Add("argonnev8");
            intType.Add("CD-bonn");
            intType.Add("Idaho-A");
            intType.Add("Idaho-B");
            this.dataParams.Add("type_of_pot", intType[0], intType, Messages.SMType_of_pot);

            ArrayList pauliOperatorType = new ArrayList();
            pauliOperatorType.Add("square");
            pauliOperatorType.Add("triangular");
            pauliOperatorType.Add("wings");
            this.dataParams.Add("pauli_operator", pauliOperatorType[0], pauliOperatorType, Messages.SMPauli_operator);

            this.dataParams.Add("lab_lmax", 1, typeof(int), Messages.SMLab_lmax);
            this.dataParams.Add("lab_nmax", 1, typeof(int), Messages.SMLab_nmax);

            this.dataParams.Add("max_space", 20, typeof(int), Messages.SMMax_space);

            this.dataParams.Add("n_startenergy_g", 5, typeof(int), Messages.SMStartenergy_g);
            this.dataParams.Add("first_startingenergy", -5.0, typeof(double), Messages.SMStartenergy_g);
            this.dataParams.Add("last_startingenergy", -150.0, typeof(double), Messages.SMStartenergy_g);

            this.dataParams.Add("n_k1", 50, typeof(int), Messages.SMVlowk);
            this.dataParams.Add("n_k2", 50, typeof(int), Messages.SMVlowk);
            this.dataParams.Add("k_cutoff", 2.1, typeof(double), Messages.SMVlowk);
            this.dataParams.Add("k_max", 20.0, typeof(double), Messages.SMVlowk);

            this.dataParams.Add("output_run", "vrenorm_debug.dat", Messages.SMOutput_run);
            this.dataParams.Add("renorminteraction_file", "vrenorm_int.dat", Messages.SMRenorminteraction_fileO);
            this.dataParams.Add("spdata_file", "vrenorm_sporbits.dat", Messages.SMSpdata_fileO);
        }

        /// <summary>
        /// Save the file into the specified path
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>Full name of the saved file (including the path)</returns>
        public string SaveINIFile(string path) {
            string fileName = Path.Combine(path, iniFileName);
            this.dataParams.Save(fileName);
            return fileName;
        }

        private Guider guider;

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            this.dataParams.Synchronize(guider.Context);
            this.SaveINIFile(Context.WorkingDirectory);

            string exec = Path.Combine(Context.ExecDirectory, "Functions");
            exec = Path.Combine(exec, "ShellModel");
            exec = Path.Combine(exec, "vrenorm.exe");

            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.UseShellExecute = false;
            procInfo.WorkingDirectory = Context.WorkingDirectory;
            procInfo.FileName = exec;
            procInfo.CreateNoWindow = true;
            procInfo.RedirectStandardOutput = true;

            this.guider = guider;

//            Process process = Process.Start(procInfo);
//            process.PriorityClass = ProcessPriorityClass.BelowNormal;
//            process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
//            process.BeginOutputReadLine();
//            process.WaitForExit();

            FileData f = new FileData(false);
            f.Read(Path.Combine(Context.WorkingDirectory, "vrenorm_debug.dat"));
            guider.Context.SetVariable("output_run", f);

            f = new FileData(false);
            f.Read(Path.Combine(Context.WorkingDirectory, "vrenorm_int.dat"));
            guider.Context.SetVariable("renorminteraction_file", f);

            f = new FileData(false);
            f.Read(Path.Combine(Context.WorkingDirectory, "vrenorm_sporbits.dat"));
            guider.Context.SetVariable("spdata_file", f);

            return null;
        }

        /// <summary>
        /// Resends the output writen to the console
        /// </summary>
        void process_OutputDataReceived(object sender, DataReceivedEventArgs e) {
            if(e.Data != null && this.guider != null)
                this.guider.WriteLine(e.Data);
        }

        private const string iniFileName = "renorm.ini";
    }
}