using System;
using System.Drawing;
using System.Text;


namespace PavelStransky.Expression {
    /// <summary>
    /// Interface objektu s textem ke zv�razn�n�
    /// </summary>
   public interface IHighlightText {
       int SelectionStart { set;}
       int SelectionLength { set;}
       Font SelectionFont { set;}
       Color SelectionColor { set;}
    }
}
