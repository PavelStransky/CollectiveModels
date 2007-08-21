using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

using System.Runtime.InteropServices;

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
        public CommandTextBox() : base() {
            this.timer.Interval = 1500;
            this.timer.AutoReset = false;
            this.timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
        }

        /// <summary>
        /// Spustí všechny pøíkazy
        /// </summary>
        public void ExecuteAll() {
            this.OnExecuteCommand(new ExecuteCommandEventArgs(this.Text, false));
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

            if(!e.Alt && !e.Shift && e.KeyValue == 116) {
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
                    } while(Atom.IsInBracket(this.Text.Substring(commandStart, commandEnd - commandStart), 0) && commandEnd != this.Text.Length);

                    while(commandEnd != this.Text.Length && this.Text[commandEnd] == semicolon)
                        commandEnd++;

                    if(this.Text[commandEnd - 1] != semicolon)
                        throw new Exception(errorMessageNotClosed);

                    this.SelectionStart = commandStart;
                    this.SelectionLength = commandEnd - commandStart;
                }

                this.OnExecuteCommand(new ExecuteCommandEventArgs(this.SelectedText, e.Control));
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

        #region Zastavení pøekreslování
        private const int WM_SETREDRAW      = 0x000B;
        private const int WM_USER           = 0x400;
        private const int EM_GETEVENTMASK   = (WM_USER + 59);
        private const int EM_SETEVENTMASK   = (WM_USER + 69);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private extern static IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        private IntPtr eventMask = IntPtr.Zero;

        public void StopRedrawing() {
            // Stop redrawing:
            SendMessage(this.Handle, WM_SETREDRAW, 0, IntPtr.Zero);

            // Stop sending of events:
            this.eventMask = SendMessage(this.Handle, EM_GETEVENTMASK, 0, IntPtr.Zero);
        }

        public void ResumeRedrawing() {
            // turn on events
            SendMessage(this.Handle, EM_SETEVENTMASK, 0, eventMask);

            // turn on redrawing
            SendMessage(this.Handle, WM_SETREDRAW, 1, IntPtr.Zero);
        }
        #endregion

        #region Highlight designer
        private System.Timers.Timer timer = new System.Timers.Timer();
        private RichTextBox doubleBuffer = new RichTextBox();

        private Highlight highlight;

        /// <summary>
        /// Zvýrazní syntaxi v celém textu
        /// </summary>
        public void HighlightSyntax() {
            this.highlight = Atom.CheckSyntax(this.Text);

            Font font = new System.Drawing.Font("Courier New", 9.75F,
                System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            Font fontBold = new Font(font, FontStyle.Bold);
            Font fontItalic = new Font(font, FontStyle.Italic);

            int selectionStart = this.SelectionStart;
            int selectionLength = this.SelectionLength;
            int firstShowedChar = this.GetCharIndexFromPosition(new Point(this.Margin.Left, this.Margin.Top));

            this.StopRedrawing();

            // Tenhle debilní pøíkaz je na vymazání RTF formátování uvnitø textu
            // a na vymazání obrázkù
            string s = this.Text;

            // Reset písma
            this.Font = font;
            this.ForeColor = Color.Blue;

            this.Text = s;

            HighlightTypes lastType = HighlightTypes.Separator;

            foreach(Highlight.HighlightItem item in this.highlight) {
                if(item.HighlightType == HighlightTypes.IndexBracket
                    || item.HighlightType == HighlightTypes.Number
                    || item.HighlightType == HighlightTypes.Operator
                    || item.HighlightType == HighlightTypes.Variable)
                    continue;

                if(item.HighlightType == HighlightTypes.NormalBracket) {
                    if(lastType == HighlightTypes.Function) {
                        this.SelectionStart = item.Start;
                        this.SelectionLength = 1;
                        this.SelectionFont = fontBold;
                        this.SelectionColor = Color.Black;

                        this.SelectionStart = item.End;
                        this.SelectionLength = 1;
                        this.SelectionFont = fontBold;
                        this.SelectionColor = Color.Black;
                    }

                    lastType = item.HighlightType;
                    continue;
                }

                this.SelectionStart = item.Start;
                this.SelectionLength = item.Length;

                if(item.HighlightType == HighlightTypes.Comment)
                    this.SelectionColor = Color.Gray;

                else if(item.HighlightType == HighlightTypes.EndVariable)
                    this.SelectionColor = Color.Black;

                else if(item.HighlightType == HighlightTypes.Error) {
                    this.SelectionFont = fontBold;
                    this.SelectionColor = Color.Red;
                }

                else if(item.HighlightType == HighlightTypes.Function) {
                    this.SelectionFont = fontBold;
                    this.SelectionColor = Color.Black;
                }

                else if(item.HighlightType == HighlightTypes.Separator) {
                    this.SelectionFont = fontBold;
                    this.SelectionColor = Color.Black;
                }

                else if(item.HighlightType == HighlightTypes.String)
                    this.SelectionColor = Color.Green;

                lastType = item.HighlightType;
            }

            this.SelectionStart = firstShowedChar;
            this.ScrollToCaret();

            this.SelectionStart = selectionStart;
            this.SelectionLength = selectionLength;

            this.ResumeRedrawing();
            this.ResumeLayout();

            this.Invalidate();
        }

        protected override void OnKeyPress(KeyPressEventArgs e) {
            this.timer.Stop();
            this.timer.Start();
        }

        private delegate void InvokeDelegate();

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            if(!this.IsDisposed)
                this.Invoke(new InvokeDelegate(this.HighlightSyntax));
        }

        #endregion

        private const string newLine = "\r\n";
        private const char semicolon = ';';
        private const string tab = "   ";
        private const string errorMessageNotClosed = "Pøíkaz není uzavøený nebo nekonèí støedníkem, nelze jej vyhodnotit!";
    }
}