using System;
using System.IO;
using System.Windows.Forms;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Forms {
    /// <summary>
    /// TextBox pro zad�v�n� p��kaz� - n�kter� speci�ln� funkce
    /// </summary>
    public class CommandTextBox: RichTextBox {
        // Ud�lost Execute
        public delegate void ExecuteCommandEventHandler(object sender, ExecuteCommandEventArgs e);
        public event ExecuteCommandEventHandler ExecuteCommand;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public CommandTextBox() : base() { }

        /// <summary>
        /// Spust� v�echny p��kazy
        /// </summary>
        public void ExecuteAll() {
            this.OnExecuteCommand(new ExecuteCommandEventArgs(this.Text, false));
        }

        /// <summary>
        /// Ulo�� obsah textboxu do souboru
        /// </summary>
        /// <param name="fName">Jm�no souboru</param>
        public void Export(string fName) {
            FileStream f = new FileStream(fName, FileMode.Create);
            StreamWriter t = new StreamWriter(f);

            this.Export(t);

            t.Close();
            f.Close();
        }

        /// <summary>
        /// Ulo�� obsah vektoru do souboru
        /// </summary>
        /// <param name="t">StreamWriter</param>
        public void Export(StreamWriter t) {
            t.Write(this.Text);
        }

        /// <summary>
        /// Na�te obsah vektoru ze souboru
        /// </summary>
        /// <param name="fName">Jm�no souboru</param>
        public void Import(string fName) {
            FileStream f = new FileStream(fName, FileMode.Open);
            StreamReader t = new StreamReader(f);

            this.Import(t);

            t.Close();
            f.Close();
        }

        /// <summary>
        /// Na�te obsah vektoru ze souboru
        /// </summary>
        /// <param name="t">StreamReader</param>
        public void Import(StreamReader t) {
            this.Text = t.ReadToEnd();
        }

        /// <summary>
        /// Vyvol�n� ud�losti o spu�t�n� p��kazu
        /// </summary>
        protected void OnExecuteCommand(ExecuteCommandEventArgs e) {
            if(this.ExecuteCommand != null)
                this.ExecuteCommand(this, e);
        }

        /// <summary>
        /// P�i stisknut�m tla��tku
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);

            if(!e.Alt && e.Control && e.KeyValue == 13) {
                // Pokud je d�lka v�b�ru 0, spust�me aktu�ln� p��kaz, jinak spou�t�me v�b�r
                if(this.SelectionLength == 0) {
                    int selectionStart = this.SelectionStart;

                    // Za��tek p��kazu
                    int commandStart = selectionStart + 1;
                    do {
                        commandStart = this.PreviousIndexOf(this.Text, semicolon, commandStart - 1);
                        if(commandStart < 0)
                            commandStart = 0;
                    } while(Atom.IsInBracket(this.Text, commandStart));

                    while(this.Text[commandStart] == semicolon)
                        commandStart++;

                    int commandEnd = selectionStart - 1;
                    do {
                        commandEnd = this.NextIndexOf(this.Text, semicolon, commandEnd + 1);
                        if(commandEnd < 0)
                            commandEnd = this.Text.Length;
                    } while(Atom.IsInBracket(this.Text.Substring(commandStart, commandEnd - commandStart)) && commandEnd != this.Text.Length);

                    while(commandEnd != this.Text.Length && this.Text[commandEnd] == semicolon)
                        commandEnd++;

                    if(this.Text[commandEnd - 1] != semicolon)
                        throw new Exception(errorMessageNotClosed);

                    this.SelectionStart = commandStart;
                    this.SelectionLength = commandEnd - commandStart;
                }

                this.OnExecuteCommand(new ExecuteCommandEventArgs(this.SelectedText, e.Shift));
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

            else if(!e.Alt && !e.Shift && e.Control && e.KeyValue == 32) {
                int selectionStart = this.SelectionStart;
                int selectionLength = this.SelectionLength;
                string text = this.Text;
                string selectedText = text.Substring(selectionStart, selectionLength).Replace(newLine, newLine + tab);

                this.Text = text.Substring(0, selectionStart);
                this.Text += tab;
                this.Text += selectedText;
                this.Text += text.Substring(selectionStart + selectionLength, text.Length - (selectionStart + selectionLength));

                this.SelectionStart = selectionStart + tab.Length;
                this.SelectionLength = selectedText.Length;

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// Najde p�edchoz� v�skyt znaku
        /// </summary>
        /// <param name="text">Text, ve kter�m se hled�</param>
        /// <param name="c">Znak</param>
        /// <param name="start">Po��te�n� pozice</param>
        private int PreviousIndexOf(string text, char c, int start) {
            int i = start;
            for(; i >= 0; i--)
                if(text[i] == c)
                    break;
            return i;
        }

        /// <summary>
        /// Najde n�sleduj�c� v�skyt znaku
        /// </summary>
        /// <param name="text">Text, ve kter�m se hled�</param>
        /// <param name="c">Znak</param>
        /// <param name="start">Po��te�n� pozice</param>
        private int NextIndexOf(string text, char c, int start) {
            return text.IndexOf(c, start);
        }

        private const string newLine = "\r\n";
        private const char semicolon = ';';
        private const string tab = "   ";
        private const string errorMessageNotClosed = "P��kaz nen� uzav�en� nebo nekon�� st�edn�kem, nelze jej vyhodnotit!";
    }

    /// <summary>
    /// P�ed�v�n� parametr� p�i ud�losti spu�t�n� p��kazu
    /// </summary>
    public class ExecuteCommandEventArgs: EventArgs {
        private string expression;
        private bool newWindow;

        /// <summary>
        /// V�raz k proveden�
        /// </summary>
        public string Expression { get { return this.expression; } }

        /// <summary>
        /// True, pokud p�i je po�adov�no otev�en� nov�ho okna
        /// </summary>
        public bool NewWindow { get { return this.newWindow; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="expression">V�raz k proveden�</param>
        /// <param name="newWindow">Nov� okno</param>
        public ExecuteCommandEventArgs(string expression, bool newWindow) {
            this.expression = expression;
            this.newWindow = newWindow;
        }
    }
}