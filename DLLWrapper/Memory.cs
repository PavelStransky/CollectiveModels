using System;
using System.Runtime.InteropServices;
using System.IO;

namespace PavelStransky.DLLWrapper {
    /// <summary>
    /// Alokov·nÌ pamÏti na nespravovanÈ haldÏ
    /// </summary>
    /// <remarks>http://www.codeproject.com/dotnet/pointers.asp</remarks>
    public unsafe class Memory {
        //Handle pro haldu
        private static IntPtr processHeap = GetProcessHeap();

        // Heap API functions
        [DllImport("kernel32")]
        private static extern IntPtr GetProcessHeap();

        [DllImport("kernel32")]
        private static extern IntPtr HeapCreate(uint flOptions, uint initialSize, uint maximumSize);

        [DllImport("kernel32")]
        private static extern void* HeapAlloc(IntPtr hHeap, uint flags, uint size);

        [DllImport("kernel32")]
        private static extern bool HeapFree(IntPtr hHeap, int flags, void* block);

        [DllImport("kernel32")]
        private static extern void* HeapReAlloc(IntPtr hHeap, int flags, void* block, int size);

        [DllImport("kernel32")]
        private static extern int HeapSize(IntPtr hHeap, int flags, void* block);

        [DllImport("kernel32")]
        static extern void FillMemory(IntPtr destination, uint length, byte fill);
    
        /// <summary>
        /// Soukrom˝ konstruktor (abychom nemohli vytvo¯it instanci t¯Ìdy)
        /// </summary>
        private Memory() { }

        /// <summary>
        /// Alokuje dan˝ poËet byt˘ pamÏti
        /// </summary>
        /// <param name="size">PoËet byt˘ pamÏti</param>
        private static IntPtr Alloc(IntPtr size) {
            IntPtr result = Marshal.AllocHGlobal(size);
            //            void* result = HeapAlloc(processHeap, HEAP_ZERO_MEMORY, size);
            if(result == null)
                throw new OutOfMemoryException();
            return result;
        }

        /// <summary>
        /// Alokuje pole int o zadanÈ dÈlce
        /// </summary>
        /// <param name="length">DÈlka pole</param>
        public static int* NewInt(long length) {
            int* result = (int*)Alloc((IntPtr)(length * sizeof(int)));

            for(long l = 0; l < length; l++)
                result[l] = 0;

            return result;
        }

        /// <summary>
        /// Alokuje pole double o zadanÈ dÈlce
        /// </summary>
        /// <param name="length">DÈlka pole</param>
        public static double* NewDouble(long length) {
            double* result = (double*)Alloc((IntPtr)(length * sizeof(double)));

            for(long l = 0; l < length; l++)
                result[l] = 0.0;

            return result;
        }

        /// <summary>
        /// Alokuje pole char o zadanÈ dÈlce
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte* NewByte(long length) {
            byte* result = (byte*)Alloc((IntPtr)(length * sizeof(byte)));
            for(long l = 0; l < length; l++)
                result[l] = 0;

            return result;
        }

        /// <summary>
        /// UvolnÌ alokovanou pamÏù
        /// </summary>
        /// <param name="block">Ukazatel na pamÏù</param>
        public static void Delete(void* block) {
            if(block != null)
                Marshal.FreeHGlobal((IntPtr)block);
//                if(!HeapFree(processHeap, 0, block)) 
//                    throw new InvalidOperationException();
        }

        // Indikuje, ûe alokovan· pamÏù bude vynulov·na
        private const int HEAP_ZERO_MEMORY = 0x00000008;
        private const int HEAP_GENERATE_EXCEPTIONS = 0x4;
    }
}
