using System;
using System.Runtime.InteropServices;

namespace PavelStransky.DLLWrapper {
    /// <summary>
    /// Alokov�n� pam�ti na nespravovan� hald�
    /// </summary>
    /// <remarks>http://www.codeproject.com/dotnet/pointers.asp</remarks>
    public unsafe class Memory {
        //Handle pro haldu
        private static int processHeap = GetProcessHeap();

        // Heap API functions
        [DllImport("kernel32")]
        private static extern int GetProcessHeap();

        [DllImport("kernel32")]
        private static extern void* HeapAlloc(int hHeap, int flags, int size);

        [DllImport("kernel32")]
        private static extern bool HeapFree(int hHeap, int flags, void* block);

        [DllImport("kernel32")]
        private static extern void* HeapReAlloc(int hHeap, int flags, void* block, int size);

        [DllImport("kernel32")]
        private static extern int HeapSize(int hHeap, int flags, void* block);

        /// <summary>
        /// Soukrom� konstruktor (abychom nemohli vytvo�it instanci t��dy)
        /// </summary>
        private Memory() { }

        /// <summary>
        /// Alokuje dan� po�et byt� pam�ti
        /// </summary>
        /// <param name="size">Po�et byt� pam�ti</param>
        private static void* Alloc(int size) {
            void* result = HeapAlloc(processHeap, HEAP_ZERO_MEMORY, size);
            if(result == null)
                throw new OutOfMemoryException();
            return result;
        }

        /// <summary>
        /// Alokuje pole int o zadan� d�lce
        /// </summary>
        /// <param name="length">D�lka pole</param>
        public static int* NewInt(int length) {
            return (int*)Alloc(length * sizeof(int));
        }

        /// <summary>
        /// Alokuje pole double o zadan� d�lce
        /// </summary>
        /// <param name="length">D�lka pole</param>
        public static double* NewDouble(int length) {
            return (double*)Alloc(length * sizeof(double));
        }

        /// <summary>
        /// Uvoln� alokovanou pam�
        /// </summary>
        /// <param name="block">Ukazatel na pam�</param>
        public static void Delete(void* block) {
            if(block != null)
                if(!HeapFree(processHeap, 0, block)) 
                    throw new InvalidOperationException();
        }

        // Indikuje, �e alokovan� pam� bude vynulov�na
        private const int HEAP_ZERO_MEMORY = 0x00000008;
    }
}