using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace PavelStransky.Expression {
    /// <summary>
    /// Øada barev (pro indexaci pomocí int)
    /// </summary>
    public class ColorArray {
        /// <summary>
        /// Barva
        /// </summary>
        /// <param name="color">Èíslo barvy</param>
        public static Color GetColor(int color) {
            switch(color) {
                case 0: return Color.Black;
                case 1: return Color.Red;
                case 2: return Color.Green;
                case 3: return Color.Blue;
                case 4: return Color.Magenta;
                case 5: return Color.Cyan;
                case 6: return Color.Yellow;
                case 7: return Color.White;
                case 8: return Color.DarkGray;
                case 9: return Color.LightCoral;
                case 10: return Color.LightGreen;
                case 11: return Color.LightBlue;
                case 12: return Color.LightPink;
                case 13: return Color.LightCyan;
                case 14: return Color.LightYellow;
                case 15: return Color.LightGray;
            }

            return Color.Black;
        }
    }
}
