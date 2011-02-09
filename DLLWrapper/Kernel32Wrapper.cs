using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.DLLWrapper {
    /// <summary>
    /// Wrapper pro Kernel32.dll
    /// </summary>
    unsafe public class Kernel32Wrapper {
        [DllImport("Kernel32", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
        public static extern Int32 GetCurrentWin32ThreadId();
    }
}
