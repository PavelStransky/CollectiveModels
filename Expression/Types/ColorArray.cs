using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace PavelStransky.Expression {
    /// <summary>
    /// �ada barev (pro indexaci pomoc� int)
    /// </summary>
    public class ColorArray {
        /// <summary>
        /// Barva
        /// </summary>
        /// <param name="color">��slo barvy</param>
        public static Color GetColor(int color) {
            Color result;

            switch(color) {
                case 0: result = Color.Black; break;
                case 1: result = Color.Red; break;
                case 2: result = Color.Green; break;
                case 3: result = Color.Blue; break;
                case 4: result = Color.Magenta; break;
                case 5: result = Color.Cyan; break;
                case 6: result = Color.Yellow; break;
                case 7: result = Color.DarkGray; break;
                case 8: result = Color.LightCoral; break;
                case 9: result = Color.LightGreen; break;
                case 10: result = Color.LightBlue; break;
                case 11: result = Color.LightPink; break;
                case 12: result = Color.LightCyan; break;
                case 13: result = Color.LightYellow; break;
                case 14: result = Color.LightGray; break;
                case 15: result = Color.Orange; break;
                case 16: result = Color.White; break;
                default: result = Color.FromKnownColor((KnownColor)color); break;
            }

            return result;
        }
    }
}
