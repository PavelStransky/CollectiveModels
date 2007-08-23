using System;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

using System.Runtime.InteropServices;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Forms {
    /// <summary>
    /// TextBox pro zadávání pøíkazù - nìkteré speciální funkce
    /// </summary>
    public class CommandTextBox: RichTextBox, IHighlightText {
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
            this.tRedraw.Stop();

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

            this.tRedraw.Start();
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

        #region Highlighting of syntax
        private bool highlighting = false;

        // Èasovaè (pøekreslujeme vždy po urèité dobì po zmìnì, protože pøekreslování je
        // bohužel hroznì èasovì nároèné)
        private System.Timers.Timer tRecalculate, tRedraw;
        private bool timersInicialized = false;

        private BackgroundWorker workerRecalculate;
        private Highlight highlight;

        private bool recalculating = false;

        /// <summary>
        /// True, pokud chceme zobrazovat zvýrazòování syntaxe
        /// </summary>
        public bool Highlighting {
            get {
                return this.highlighting;
            }
            set {
                if(value == true & this.highlighting == false) {
                    this.SetHighlight();
                    this.SetTimers();
                }

                this.highlighting = value;
            }
        }

        /// <summary>
        /// Nastaví tøídu pro zvýrazòování
        /// </summary>
        private void SetHighlight() {
            this.highlight = new Highlight();

            this.highlight.BaseFont = new System.Drawing.Font("Courier New", 9.75F,
                System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Point, ((byte)(238)));
        }

        /// <summary>
        /// Nastaví èasovaèe pro pøekreslování syntaxe
        /// </summary>
        private void SetTimers() {
            if(!this.timersInicialized) {
                this.tRecalculate = new System.Timers.Timer();
                this.tRecalculate.Interval = 1500;
                this.tRecalculate.AutoReset = false;
                this.tRecalculate.Elapsed += new System.Timers.ElapsedEventHandler(tRecalculate_Elapsed);

                this.tRedraw = new System.Timers.Timer();
                this.tRedraw.Interval = 2000;
                this.tRedraw.AutoReset = false;
                this.tRedraw.Elapsed += new System.Timers.ElapsedEventHandler(tRedraw_Elapsed);

                this.workerRecalculate = new BackgroundWorker();
                this.workerRecalculate.WorkerReportsProgress = false;
                this.workerRecalculate.WorkerSupportsCancellation = false;
                this.workerRecalculate.DoWork += new DoWorkEventHandler(workerRecalculate_DoWork);

                this.timersInicialized = true;

            }
        }

        /// <summary>
        /// Zvýrazní syntaxi v celém textu
        /// </summary>
        private void HighlightSyntax() {
            int selectionStart = this.SelectionStart;
            int selectionLength = this.SelectionLength;
            int firstShowedChar = this.GetCharIndexFromPosition(new Point(this.Margin.Left, this.Margin.Top));

            this.StopRedrawing();

            // Tenhle debilní pøíkaz je na vymazání RTF formátování uvnitø textu
            // a na vymazání obrázkù
            string s = this.Text;

            // Reset písma
            this.ResetText();
            this.Font = this.highlight.BaseFont;
            this.ForeColor = this.highlight.DefaultColor;

            this.Text = s;

            this.highlight.HighlightAll(this);

            this.SelectionStart = firstShowedChar;
            this.ScrollToCaret();

            this.SelectionStart = selectionStart;
            this.SelectionLength = selectionLength;

            this.ResumeRedrawing();

            this.Invalidate();
        }

        /// <summary>
        /// Provede okamžitì oznaèení syntaxe
        /// </summary>
        public void ForceHighlightSyntax() {
            this.SetHighlight();
            this.highlight.CheckSyntax(this.Text); 
            this.HighlightSyntax();
        }

        /// <summary>
        /// Stisk klávesy - indikuje zmìnu textu, pøepoèítáváme
        /// </summary>
        protected override void OnKeyPress(KeyPressEventArgs e) {
            base.OnKeyPress(e);
            this.tRecalculate.Start();
        }

        /// <summary>
        /// Stisk myšky - znovu rozjedeme vykreslování
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e) {
            this.tRedraw.Stop();
            base.OnMouseClick(e);
            this.tRedraw.Start();
        }

        // Delegát kvùli Invoke
        private delegate void InvokeDelegate();

        /// <summary>
        /// Pøepoèítání formátování
        /// </summary>
        private void tRecalculate_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            if(!this.IsDisposed)
                this.Invoke(new InvokeDelegate(this.RecalculateSyntax));
        }

        /// <summary>
        /// This.Text mùžeme naèíst jen ve vlastním threadu, ale poèítat chci na pozadí.
        /// Proto tahle šaráda.
        /// </summary>
        private void RecalculateSyntax() {
            this.workerRecalculate.RunWorkerAsync(this.Text);
        }

        void workerRecalculate_DoWork(object sender, DoWorkEventArgs e) {
            this.recalculating = true;
            this.highlight.CheckSyntax(e.Argument as string);
            this.recalculating = false;
            this.tRedraw.Start();
        }

        /// <summary>
        /// Pøekreslení
        /// </summary>
        void tRedraw_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            if(!this.IsDisposed) {
                if(this.recalculating)
                    this.tRedraw.Start();
                else
                    this.Invoke(new InvokeDelegate(this.HighlightSyntax));
            }
        }

        // Událost HighlighItemPointed
        public delegate void HighlightItemPointedEventHandler(object sender, HighlightItemEventArgs e);
        public event HighlightItemPointedEventHandler HighlightItemPointed;

        Highlight.HighlightItem lastItem = null;

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if(this.highlighting) {
                int index = this.GetCharIndexFromPosition(e.Location);
                Highlight.HighlightItem item = this.highlight.FindItem(index);

                if(item != this.lastItem) {
                    this.OnHighlightItemPointed(new HighlightItemEventArgs(item));
                }

                this.lastItem = item;
            }        
        }

        protected virtual void OnHighlightItemPointed(HighlightItemEventArgs e) {
            if(this.HighlightItemPointed != null)
                this.HighlightItemPointed(this, e);
        }
        #endregion

        #region Zvýraznìní dvojice závorek
        /// <summary>
        /// Stisk klávesy - závorky zvýrazòujeme pøi stisku CTRL
        /// </summary>
        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e) {
            base.OnPreviewKeyDown(e);
            
            if(e.Control && !this.tRecalculate.Enabled && !this.showedBracketPair) {
                this.bracketPair = this.highlight.FindSecondBracket(this.SelectionStart);
                this.ShowBracketPair(true);
            }
        }

        /// <summary>
        /// Uvolnìní klávesy
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);

            if(!e.Control && this.showedBracketPair) 
                this.ShowBracketPair(false);
        }

        private bool showedBracketPair = false;
        private Highlight.HighlightItem bracketPair;
        private Font oldFont;
        private Color oldColor;

        /// <summary>
        /// Zobrazí pár závorek
        /// </summary>
        private void ShowBracketPair(bool show) {
            if(this.bracketPair == null)
                return;

            this.showedBracketPair = !this.showedBracketPair;

            int selectionStart = this.SelectionStart;
            int selectionLength = this.SelectionLength;
            int firstShowedChar = this.GetCharIndexFromPosition(new Point(this.Margin.Left, this.Margin.Top));

            this.StopRedrawing();

            if(show) {
                Font font = new System.Drawing.Font("Courier New", 12F,
                    System.Drawing.FontStyle.Regular,
                    System.Drawing.GraphicsUnit.Point, ((byte)(238)));
                
                Font fontBold = new Font(font, FontStyle.Bold);
                this.SelectionStart = this.bracketPair.Start;
                this.SelectionLength = 1;

                this.oldFont = this.SelectionFont;
                this.oldColor = this.SelectionColor;

                this.SelectionFont = fontBold;
                this.SelectionColor = Color.Magenta;

                this.SelectionStart = this.bracketPair.End;
                this.SelectionLength = 1;

                this.SelectionFont = fontBold;
                this.SelectionColor = Color.Magenta;

                this.showedBracketPair = true;
            }

            else {
                this.SelectionStart = this.bracketPair.Start;
                this.SelectionLength = 1;

                this.SelectionFont = this.oldFont;
                this.SelectionColor = this.oldColor;

                this.SelectionStart = this.bracketPair.End;
                this.SelectionLength = 1;

                this.SelectionFont = this.oldFont;
                this.SelectionColor = this.oldColor;

                this.showedBracketPair = false;
            }

            this.SelectionStart = firstShowedChar;
            this.ScrollToCaret();

            this.SelectionStart = selectionStart;
            this.SelectionLength = selectionLength;

            this.ResumeRedrawing();
            this.ResumeLayout();

            this.Invalidate();
        }
        #endregion

        private const string newLine = "\r\n";
        private const char semicolon = ';';
        private const string tab = "   ";
        private const string errorMessageNotClosed = "Pøíkaz není uzavøený nebo nekonèí støedníkem, nelze jej vyhodnotit!";
    }
}