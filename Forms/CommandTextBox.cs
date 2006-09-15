using System;
using System.IO;
using System.Windows.Forms;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Forms {
    /// <summary>
    /// TextBox pro zadávání pøíkazù - nìkteré speciální funkce
    /// </summary>
    public class CommandTextBox: RichTextBox {
        // Událost Execute
        public delegate void ExecuteCommandEventHandler(object sender, ExecuteCommandEventArgs e);
        public event ExecuteCommandEventHandler ExecuteCommand;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public CommandTextBox() : base() { }

        /// <summary>
        /// Spustí všechny pøíkazy
        /// </summary>
        public void ExecuteAll() {
            this.OnExecuteCommand(new ExecuteCommandEventArgs(this.Text, false));
        }

        /// <summary>
        /// Uloží obsah textboxu do souboru
        /// </summary>
        /// <param name="fName">Jméno souboru</param>
        public void Export(string fName) {
            FileStream f = new FileStream(fName, FileMode.Create);
            StreamWriter t = new StreamWriter(f);

            this.Export(t);

            t.Close();
            f.Close();
        }

        /// <summary>
        /// Uloží obsah vektoru do souboru
        /// </summary>
        /// <param name="t">StreamWriter</param>
        public void Export(StreamWriter t) {
            t.Write(this.Text);
        }

        /// <summary>
        /// Naète obsah vektoru ze souboru
        /// </summary>
        /// <param name="fName">Jméno souboru</param>
        public void Import(string fName) {
            FileStream f = new FileStream(fName, FileMode.Open);
            StreamReader t = new StreamReader(f);

            this.Import(t);

            t.Close();
            f.Close();
        }

        /// <summary>
        /// Naète obsah vektoru ze souboru
        /// </summary>
        /// <param name="t">StreamReader</param>
        public void Import(StreamReader t) {
            this.Text = t.ReadToEnd();
        }

        /// <summary>
        /// Vyvolání události o spuštìní pøíkazu
        /// </summary>
        protected void OnExecuteCommand(ExecuteCommandEventArgs e) {
            if(this.ExecuteCommand != null)
                this.ExecuteCommand(this, e);
        }

        /// <summary>
        /// Pøi stisknutém tlaèítku
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);

            if(!e.Alt && e.Control && e.KeyValue == 13) {
                // Pokud je délka výbìru 0, spustíme aktuální pøíkaz, jinak spouštíme výbìr
                if(this.SelectionLength == 0) {
                    int selectionStart = this.SelectionStart;

                    // Zaèátek pøíkazu
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
        /// Najde pøedchozí výskyt znaku
        /// </summary>
        /// <param name="text">Text, ve kterém se hledá</param>
        /// <param name="c">Znak</param>
        /// <param name="start">Poèáteèní pozice</param>
        private int PreviousIndexOf(string text, char c, int start) {
            int i = start;
            for(; i >= 0; i--)
                if(text[i] == c)
                    break;
            return i;
        }

        /// <summary>
        /// Najde následující výskyt znaku
        /// </summary>
        /// <param name="text">Text, ve kterém se hledá</param>
        /// <param name="c">Znak</param>
        /// <param name="start">Poèáteèní pozice</param>
        private int NextIndexOf(string text, char c, int start) {
            return text.IndexOf(c, start);
        }

        private const string newLine = "\r\n";
        private const char semicolon = ';';
        private const string tab = "   ";
        private const string errorMessageNotClosed = "Pøíkaz není uzavøený nebo nekonèí støedníkem, nelze jej vyhodnotit!";
    }

    /// <summary>
    /// Pøedávání parametrù pøi události spuštìní pøíkazu
    /// </summary>
    public class ExecuteCommandEventArgs: EventArgs {
        private string expression;
        private bool newWindow;

        /// <summary>
        /// Výraz k provedení
        /// </summary>
        public string Expression { get { return this.expression; } }

        /// <summary>
        /// True, pokud pøi je požadováno otevøení nového okna
        /// </summary>
        public bool NewWindow { get { return this.newWindow; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="expression">Výraz k provedení</param>
        /// <param name="newWindow">Nové okno</param>
        public ExecuteCommandEventArgs(string expression, bool newWindow) {
            this.expression = expression;
            this.newWindow = newWindow;
        }
    }
}