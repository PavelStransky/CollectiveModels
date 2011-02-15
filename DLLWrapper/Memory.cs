using System;
using System.Runtime.InteropServices;

namespace PavelStransky.DLLWrapper {
    /// <summary>
    /// Alokování pamìti na nespravované haldì
    /// </summary>
    /// <remarks>http://www.codeproject.com/dotnet/pointers.asp</remarks>
    public unsafe class Memory {
        //Handle pro haldu
        private static int processHeap = GetProcessHeap();

        // Heap API functions
        [DllImport("kernel32")]
        private static extern int GetProcessHeap();

        [DllImport("kernel32")]
        private static extern void* HeapAlloc(int hHeap, int flags, uint size);

        [DllImport("kernel32")]
        private static extern bool HeapFree(int hHeap, int flags, void* block);

        [DllImport("kernel32")]
        private static extern void* HeapReAlloc(int hHeap, int flags, void* block, int size);

        [DllImport("kernel32")]
        private static extern int HeapSize(int hHeap, int flags, void* block);

        /// <summary>
        /// Soukromı konstruktor (abychom nemohli vytvoøit instanci tøídy)
        /// </summary>
        private Memory() { }

        /// <summary>
        /// Alokuje danı poèet bytù pamìti
        /// </summary>
        /// <param name="size">Poèet bytù pamìti</param>
        private static void* Alloc(uint size) {
            void* result = HeapAlloc(processHeap, HEAP_ZERO_MEMORY, size);
            if(result == null)
                throw new OutOfMemoryException();
            return result;
        }

        /// <summary>
        /// Alokuje pole int o zadané délce
        /// </summary>
        /// <param name="length">Délka pole</param>
        public static int* NewInt(int length) {
            return (int*)Alloc((uint)length * (uint)sizeof(int));
        }

        /// <summary>
        /// Alokuje pole double o zadané délce
        /// </summary>
        /// <param name="length">Délka pole</param>
        public static double* NewDouble(int length) {
            return (double*)Alloc((uint)length * (uint)sizeof(double));
        }

        /// <summary>
        /// Alokuje pole char o zadané délce
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte* NewByte(int length) {
            return (byte*)Alloc((uint)length * (uint)sizeof(byte));
        }

        /// <summary>
        /// Uvolní alokovanou pamì
        /// </summary>
        /// <param name="block">Ukazatel na pamì</param>
        public static void Delete(void* block) {
            if(block != null)
                if(!HeapFree(processHeap, 0, block)) 
                    throw new InvalidOperationException();
        }

        // Indikuje, e alokovaná pamì bude vynulována
        private const int HEAP_ZERO_MEMORY = 0x00000008;
    }
}
