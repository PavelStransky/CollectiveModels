using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Expression {
    /// <summary>
    /// Výjimka, kde máme pozici text a pozici, ve kterém je chyba
    /// </summary>
    public class PositionTextException: DetailException {
        private int position1 = -1;
        private int position2 = -1;
        private string text;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="message">Text chybového hlášení</param>
        /// <param name="position">Pozice chyby</param>
        public PositionTextException(string text, string message, params int[] position)
            : base(message) {
            this.text = text;
            this.SetPosition(position);
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="message">Text chybového hlášení</param>
        /// <param name="position">Pozice chyby</param>
        public PositionTextException(string text, string message, string detailMessage, params int[] position)
            : base(message, detailMessage) {
            this.text = text;
            this.SetPosition(position);
        }

        /// <summary>
        /// Nastaví pozice
        /// </summary>
        /// <param name="position">Pozice</param>
        private void SetPosition(int[] position) {
            if(position.Length > 0)
                this.position1 = position[0];
            if(position.Length > 1)
                this.position2 = position[1];
        }

        /// <summary>
        /// Pozice chyby (zaèátek)
        /// </summary>
        public int Position1 { get { return this.position1; } }

        /// <summary>
        /// Pozice chyby (konec)
        /// </summary>
        public int Position2 { get { return this.position2; } }

        /// <summary>
        /// Výraz
        /// </summary>
        public string Text{ get { return this.text; } }

        /// <summary>
        /// Vrátí text rozdìlený na èásti podle pozic
        /// </summary>
        public string[] TextParts() {
            if(this.position1 < 0) {
                string[] result = new string[1];
                result[0] = this.text;
                return result;
            }
            else if(this.position2 < 0) {
                string[] result = new string[2];
                result[0] = this.text.Substring(0, this.position1);
                result[1] = this.text.Substring(this.position1);
                return result;
            }
            else {
                string[] result = new string[3];
                result[0] = this.text.Substring(0, this.position1);
                result[1] = this.text.Substring(this.position1, this.position2 - this.position1);
                result[2] = this.text.Substring(this.position2);
                return result;
            }
        }
    }
}
