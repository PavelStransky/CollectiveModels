using System;
using System.Runtime.InteropServices;

namespace PavelStransky.DLLWrapper {
    /// <summary>
    /// Priorita procesu
    /// </summary>
    public enum ProcessPriority {
        Normal = 0x20,
        BelowNormal = 0x4000,
        AboveNormal = 0x8000,
        Idle = 0x40,
        High = 0x80,
        RealTime = 0x100
    }

    /// <summary>
    /// Operace s procesem (zmìna priority)
    /// </summary>
    public class Process {
        private int processID;

        [DllImport("kernel32")]
        private static extern int OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32")]
        private static extern int CloseHandle(int hObject);

        [DllImport("kernel32")]
        private static extern int GetCurrentProcessId();

        [DllImport("kernel32")]
        private static extern bool SetPriorityClass(int hProcess, int dwPriorityClass);

        [DllImport("kernel32")]
        private static extern int GetPriorityClass(int hProcess);

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Process() {
            this.processID = GetCurrentProcessId();
        }

        /// <summary>
        /// Nastaví prioritu procesu
        /// </summary>
        /// <returns>True, pokud se nastavení podaøilo</returns>
        public bool SetPriority(ProcessPriority priority) {
            int hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_SET_INFORMATION, false, this.processID);
            bool result = false;

            if(hProcess != 0) {
                result = SetPriorityClass(hProcess, (int)priority);
                CloseHandle(hProcess);
            }

            return result;
        }

        /// <summary>
        /// Vrátí prioritu procesu
        /// </summary>
        public ProcessPriority GetPriority() {
            int hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_SET_INFORMATION, false, this.processID);
            ProcessPriority result = ProcessPriority.Normal;

            if(hProcess != 0) {
                result = (ProcessPriority)GetPriorityClass(hProcess);
                CloseHandle(hProcess);
            }

            return result;
        }

        private const int PROCESS_QUERY_INFORMATION = 0x400;
        private const int PROCESS_SET_INFORMATION = 0x200;
    }
}
