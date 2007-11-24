using System;
using System.Collections;
using System.Text;

namespace PavelStransky.Expression {
    /// <summary>
    /// T��da k p�ed�v�n� informac� p�i ukon�en� v�po�tu jednotliv�ho pozad�
    /// </summary>
    public class FinishedBackgroundEventArgs: EventArgs {
        private int group;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="group">��slo skupiny, pro kterou bylo pozad� dopo��t�no</param>
        public FinishedBackgroundEventArgs(int group) {
            this.group = group;
        }

        /// <summary>
        /// ��slo skupiny, pro kterou bylo pozad� dopo��t�no
        /// </summary>
        public int Group { get { return this.group; } }
    }
}
