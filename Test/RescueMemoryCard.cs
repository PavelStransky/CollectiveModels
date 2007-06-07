using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Test {
    /// <summary>
    /// Zachraòuje data z porouchané pamìové karty
    /// </summary>
    public class RescueMemoryCard {
        private byte []data;

        public RescueMemoryCard() {
            int length = 100000000;

            FileStream f = new FileStream("c:\\nebe.img", FileMode.Open);
            BinaryReader b = new BinaryReader(f);
            this.data = b.ReadBytes(length);
            b.Close();
            f.Close();

//            f = new FileStream("c:\\prd.img", FileMode.Create);
//            BinaryWriter bw = new BinaryWriter(f);
//            bw.Write(this.data);
//            bw.Close();
//            f.Close();
        }

        public void FindFileDates() {
            int exif = 0;
            int oldexif = 0;
            int n = 0;

            do {
                exif = this.LookFor(exif, "Exif") - 6;
                if(exif < 0)
                    break;

                int date = this.LookFor(exif, "2007");
                if(date < 0)
                    break;

                StringBuilder s = new StringBuilder();
                for(int i = 0; i < 19; i++)
                    s.Append((char)this.data[date + i]);

                Console.Write(s.ToString());
                Console.Write('\t');
                Console.Write(exif / 1000);
                Console.Write('\t');

                int length = exif - oldexif;
                byte[] buffer = new byte[length];

                Console.WriteLine(length);

                for(int i = exif, j = 0; j < length; i++, j++)
                    buffer[j] = this.data[i];

                FileStream f = new FileStream(string.Format("c:\\pic\\{0}.jpg", n++), FileMode.Create);
                BinaryWriter b = new BinaryWriter(f);
                b.Write(buffer);
                b.Close();
                f.Close();

                oldexif = exif;
                exif += 10;
            } while(true);
        }

        /// <summary>
        /// Hledá danı øetìzec
        /// </summary>
        /// <param name="start">Poèáteèní pozice</param>
        private int LookFor(int start, string s) {
            int z = 0;

            int length = this.data.Length;
            for(int i = start; i < length; i++) {
                if(this.data[i] == (byte)s[z]) {
                    z++;
                    if(z == s.Length)
                        return i - z + 1;
                }
                else {
                    i -= z;
                    z = 0;
                }
            }

            return -1;
        }
    }
}
