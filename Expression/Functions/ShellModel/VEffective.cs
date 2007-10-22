using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Expression {
    /// <summary>
    /// Effective potential calculation
    /// </summary>
    public class VEffective {
        private DataParams dataParams = new DataParams();
//        private static int counter = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public VEffective() {
            // Adding all the parameters
            ArrayList intType = new ArrayList();
            intType.Add("open-diagrams");
            intType.Add("coupled-cluster");
            intType.Add("core-diagrams");
            this.dataParams.Add("type_of_interaction", intType[0], intType, Messages.SMType_of_interaction);
            
            ArrayList orderType = new ArrayList();
            orderType.Add("second");
            orderType.Add("first");
            orderType.Add("third");
            this.dataParams.Add("order_of_interaction", orderType[0], orderType, Messages.SMOrder_of_interaction);

            this.dataParams.Add("hf_iterations", 0, typeof(int), Messages.SMHf_iterations);

            this.dataParams.Add("renorminteraction_file", "vrenorm_int.dat", typeof(string), Messages.SMRenorminteraction_fileI);
            this.dataParams.Add("spdata_file", "vrenorm_sporbits.dat", typeof(string), Messages.SMSpdata_fileI);

            this.dataParams.Add("output_run", "veffective_debug.dat", typeof(string), Messages.SMOutput_run);

            this.dataParams.Add("HFspdata_file", "veffective_HFsp.dat", typeof(string), Messages.SMHFspdata_file);
            this.dataParams.Add("HFrenorminteraction_file", "veffective_HFint.dat", typeof(string), Messages.SMHFrenorminteraction_file);

            this.dataParams.Add("veff_pp", "veffective_pp.dat", typeof(string), Messages.SMVeff_xx);
            this.dataParams.Add("veff_pn", "veffective_pn.dat", typeof(string), Messages.SMVeff_xx);
            this.dataParams.Add("veff_nn", "veffective_nn.dat", typeof(string), Messages.SMVeff_xx);

            this.dataParams.Add("CCkinetic_file", "veffective_CCkin.dat", typeof(string), Messages.SMCC_file);
            this.dataParams.Add("CCinteraction_file", "veffective_CCint.dat", typeof(string), Messages.SMCC_file);
            this.dataParams.Add("CCspdata_file", "veffective_CCsp.dat", typeof(string), Messages.SMCC_file);

            this.dataParams.Add("n_startenergy_veff", 11, typeof(int), Messages.SMStartingEnergy);
            this.dataParams.Add("starting_energy", -10.0, typeof(double), Messages.SMStartingEnergy);
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

        /// <summary>
        /// Runs the program VRenorm
        /// </summary>
        /// <param name="programPath">Path to the main program executable</param>
        /// <param name="workingPath">Path to the working directory</param>
        /// <returns></returns>
        public Process Run(string programPath, string workingPath) {
            this.SaveINIFile(workingPath);

            string exec = Path.Combine(programPath, "veffective");
            exec = Path.Combine(exec, "veffective.exe");

            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.UseShellExecute = false;
            procInfo.WorkingDirectory = workingPath;
            procInfo.FileName = exec;
            procInfo.CreateNoWindow = true;
            procInfo.RedirectStandardOutput = true;

            Process process = Process.Start(procInfo);
            process.PriorityClass = ProcessPriorityClass.BelowNormal;
            return process;
        }

        private const string iniFileName = "bhf.ini";
    }
}