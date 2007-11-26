using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Forms {
    /// <summary>
    /// Z·sobnÌk s animovan˝mi GIFy
    /// </summary>
    public class AnimGIFBuffer {
        private byte[] buf2, buf3;

        private MemoryStream m;
        private FileStream f;
        private BinaryWriter b;

        private bool first;

        /// <summary>
        /// Statick˝ konstruktor
        /// </summary>
        private void CreateBuffer() {
            buf2 = new Byte[19];
            buf3 = new Byte[8];
            buf2[0] = 33;                   // extension introducer
            buf2[1] = 255;                  // application extension
            buf2[2] = 11;                   // size of block
            buf2[3] = 78;                   // N
            buf2[4] = 69;                   // E
            buf2[5] = 84;                   // T
            buf2[6] = 83;                   // S
            buf2[7] = 67;                   // C
            buf2[8] = 65;                   // A
            buf2[9] = 80;                   // P
            buf2[10] = 69;                  // E
            buf2[11] = 50;                  // 2
            buf2[12] = 46;                  // .
            buf2[13] = 48;                  // 0
            buf2[14] = 3;                   // Size of block
            buf2[15] = 1;                   //
            buf2[16] = 0;                   //
            buf2[17] = 0;                   //
            buf2[18] = 0;                   // Block terminator

            buf3[0] = 33;                   // Extension introducer
            buf3[1] = 249;                  // Graphic control extension
            buf3[2] = 4;                    // Size of block
            buf3[3] = 9;                    // Flags: reserved, disposal method, user input, transparent color
            buf3[6] = 255;                  // Transparent color index
            buf3[7] = 0;                    // Block terminator
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="interval">Interval animovanÈho GIFu</param>
        /// <param name="fileName">JmÈno souboru</param>
        public AnimGIFBuffer(int interval, string fileName) {
            this.CreateBuffer();

            buf3[4] = (byte)(interval % 256);// Delay time low byte
            buf3[5] = (byte)(interval / 256);// Delay time high byte

            this.m = new MemoryStream();
            this.f = new FileStream(fileName, FileMode.Create);
            this.b = new BinaryWriter(f);

            this.first = true;
        }

        /// <summary>
        /// P¯id· GIF do pamÏùovÈho streamu
        /// </summary>
        public void Add(Image image) {
            image.Save(this.m, ImageFormat.Gif);
            byte[] buf1 = m.ToArray();

            if(this.first) {
                //only write these the first time....
                this.b.Write(buf1, 0, 781); //Header & global color table
                this.b.Write(buf2, 0, 19); //Application extension
                this.first = false;
            }

            this.b.Write(buf3, 0, 8); //Graphic extension
            this.b.Write(buf1, 789, buf1.Length - 790); //Image data

            this.m.SetLength(0);
        }

        /// <summary>
        /// Uzav¯e soubor
        /// </summary>
        public void Close() {
            this.b.Write((byte)0x3B); //Image terminator
            this.b.Close();
            this.f.Close();
            this.m.Close();
        }
    }
}
