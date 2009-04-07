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
    /// TextBox pro zad�v�n� p��kaz� - n�kter� speci�ln� funkce
    /// </summary>
    public class CommandTextBox: RichTextBox, IHighlightText {
        // Ud�lost Execute
        public delegate void ExecuteCommandEventHandler(object sender, ExecuteCommandEventArgs e);
        public event ExecuteCommandEventHandler ExecuteCommand;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public CommandTextBox() : base() {
            this.AllowDrop = true;
            this.SetHighlight();
            this.SetTimers();
        }

        /// <summary>
        /// Spust� v�echny p��kazy
        /// </summary>
        public void ExecuteAll() {
            this.OnExecuteCommand(new ExecuteCommandEventArgs(this.Text, false));
        }

        /// <summary>
        /// Vyvol�n� ud�losti o spu�t�n� p��kazu
        /// </summary>
        protected void OnExecuteCommand(ExecuteCommandEventArgs e) {
            if(this.ExecuteCommand != null)
                this.ExecuteCommand(this, e);
        }

        /// <summary>
        /// Kontroluje text, kter� m� b�t spu�t�n, na syntaktickou spr�vnost
        /// </summary>
        private string CheckTextForExecution() {
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
                } while(Atom.IsInBracket(this.Text.Substring(commandStart, commandEnd - commandStart), 0) && commandEnd != this.Text.Length);

                while(commandEnd != this.Text.Length && this.Text[commandEnd] == semicolon)
                    commandEnd++;

                if(this.Text[commandEnd - 1] != semicolon)
                    throw new Exception(errorMessageNotClosed);

                this.SelectionStart = commandStart;
                this.SelectionLength = commandEnd - commandStart;
            }

            return this.SelectedText;
        }

        /// <summary>
        /// Spust� ozna�en� text
        /// </summary>
        public void RunSelectedText() {
            this.OnExecuteCommand(new ExecuteCommandEventArgs(this.CheckTextForExecution(), false));
        }

        /// <summary>
        /// P�i stisknut�m tla��tku
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e) {
            this.tRedraw.Stop();

            base.OnKeyDown(e);

            if(!e.Alt && !e.Shift && e.KeyValue == 116) {
                this.OnExecuteCommand(new ExecuteCommandEventArgs(this.CheckTextForExecution(), e.Control));
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

        #region Zastaven� p�ekreslov�n�
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

        // �asova� (p�ekreslujeme v�dy po ur�it� dob� po zm�n�, proto�e p�ekreslov�n� je
        // bohu�el hrozn� �asov� n�ro�n�)
        private System.Timers.Timer tRedraw;

        private Highlight highlight;

        private bool recalculating = false;
        private bool redrawing = false;

        /// <summary>
        /// True, pokud chceme zobrazovat zv�raz�ov�n� syntaxe
        /// </summary>
        public bool Highlighting {
            get {
                return this.highlighting;
            }
            set {
                if(value == true & this.highlighting == false) 
                    this.ForceHighlightSyntax();
                else if(value == false && this.highlighting == true) 
                    this.HighlightSyntax(false);

                this.highlighting = value;
            }
        }

        /// <summary>
        /// Nastav� t��du pro zv�raz�ov�n�
        /// </summary>
        private void SetHighlight() {
            this.highlight = new Highlight();

            this.highlight.BaseFont = new System.Drawing.Font("Courier New", 9.75F,
                System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Point, ((byte)(238)));
        }

        /// <summary>
        /// Nastav� �asova�e pro p�ekreslov�n� syntaxe
        /// </summary>
        private void SetTimers() {
            this.tRedraw = new System.Timers.Timer();
            this.tRedraw.Interval = 2000;
            this.tRedraw.AutoReset = false;
            this.tRedraw.Elapsed += new System.Timers.ElapsedEventHandler(tRedraw_Elapsed);
        }

        /// <summary>
        /// Zv�razn� syntaxi v cel�m textu
        /// </summary>
        private void HighlightSyntax() {
            this.HighlightSyntax(true);
        }

        /// <summary>
        /// Zru�� nebo zv�razn� syntaxi v cel�m textu
        /// </summary>
        /// <param name="h">True, chceme-li syntaxi zv�raznit</param>
        private void HighlightSyntax(bool h) {
            this.redrawing = true;

//            PavelStransky.Core.Export export =
//                new PavelStransky.Core.Export("c:\\Documents and Settings\\Pavel\\Plocha\\prd.txt", false);
//            export.Write(this.Text);
//            export.Close();

            int selectionStart = this.SelectionStart;
            int selectionLength = this.SelectionLength;
            int firstShowedChar = this.GetCharIndexFromPosition(new Point(this.Margin.Left, this.Margin.Top));

            this.StopRedrawing();

            // Tenhle debiln� p��kaz je na vymaz�n� RTF form�tov�n� uvnit� textu
            // a na vymaz�n� obr�zk�
            string s = this.Text;

            // Reset p�sma
            this.ResetText();
            this.Font = this.highlight.BaseFont;
            this.ForeColor = this.highlight.DefaultColor;

            this.Text = s;

            if(h)
                this.highlight.HighlightAll(this);

            this.SelectionStart = firstShowedChar;
            this.ScrollToCaret();

            this.SelectionStart = selectionStart;
            this.SelectionLength = selectionLength;

            this.ResumeRedrawing();

            this.Invalidate();

 //           export =
 //               new PavelStransky.Core.Export("c:\\Documents and Settings\\Pavel\\Plocha\\prd1.txt", false);
 //           export.Write(this.Rtf);
 //           export.Close();

            this.redrawing = false;
        }

        /// <summary>
        /// Provede okam�it� ozna�en� syntaxe
        /// </summary>
        public void ForceHighlightSyntax() {
            this.highlight.CheckSyntax(this.Text); 
            this.HighlightSyntax(true);
        }

        protected override void OnTextChanged(EventArgs e) {
            base.OnTextChanged(e);
            if(!this.redrawing)
                this.Recalculate();
        }

        // Deleg�t kv�li Invoke
        private delegate void InvokeDelegate();

        /// <summary>
        /// Provede p�epo��t�n�
        /// </summary>
        private void Recalculate() {
            this.recalculating = true;
            this.highlight.CheckSyntax(this.Text);
            this.recalculating = false;
            this.tRedraw.Start();
        }

        /// <summary>
        /// P�ekreslen�
        /// </summary>
        void tRedraw_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            if(!this.IsDisposed) {
                if(this.recalculating)
                    this.tRedraw.Start();
                else {
                    this.Invoke(new InvokeDelegate(this.HighlightSyntax));
                }
            }
        }

        // Ud�lost HighlighItemPointed
        public delegate void HighlightItemPointedEventHandler(object sender, HighlightItemEventArgs e);
        public event HighlightItemPointedEventHandler HighlightItemPointed;

        Highlight.HighlightItem lastItem = null;

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if(this.highlighting && !this.recalculating) {
                int index = this.GetCharIndexFromPosition(e.Location);
                Highlight.HighlightItem item = this.highlight.FindItem(index);

                if(item != this.lastItem) 
                    this.OnHighlightItemPointed(new HighlightItemEventArgs(item));

                this.lastItem = item;
            }        
        }

        protected virtual void OnHighlightItemPointed(HighlightItemEventArgs e) {
            if(this.HighlightItemPointed != null)
                this.HighlightItemPointed(this, e);
        }

        /// <summary>
        /// �asova� - nyn� po�leme event o vykreslen� 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tHelp_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            this.OnHighlightItemPointed(new HighlightItemEventArgs(this.lastItem));
        }
        #endregion

        #region Zv�razn�n� dvojice z�vorek
        /// <summary>
        /// Stisk kl�vesy - z�vorky zv�raz�ujeme p�i stisku CTRL
        /// </summary>
        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e) {
            base.OnPreviewKeyDown(e);
            
            if(e.Control && !this.showedBracketPair) {
                this.bracketPair = this.highlight.FindSecondBracket(this.SelectionStart);
                this.ShowBracketPair(true);
            }
        }

        /// <summary>
        /// Uvoln�n� kl�vesy
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
        /// Zobraz� p�r z�vorek
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
            this.Invalidate();
        }
        #endregion

        #region Obsluha Drag & Drop
        protected override void OnDragEnter(DragEventArgs drgevent) {
            base.OnDragEnter(drgevent);

            if(drgevent.Data.GetDataPresent(typeof(PointD))) {
                drgevent.Effect = DragDropEffects.Copy;
            }
        }

        protected override void OnDragDrop(DragEventArgs drgevent) {
            base.OnDragDrop(drgevent);

            Point ps = this.PointToClient(new Point(drgevent.X, drgevent.Y));
            int index = this.GetCharIndexFromPosition(ps);

            PointD p = (PointD)drgevent.Data.GetData(typeof(PointD));
            string text = string.Format("point({0}; {1})", p.X, p.Y);

            int firstShowedChar = this.GetCharIndexFromPosition(new Point(this.Margin.Left, this.Margin.Top));
            this.StopRedrawing();
            this.SelectionStart = index;
            this.SelectionLength = 0;
            this.SelectedText = text;

            this.SelectionStart = firstShowedChar;
            this.ScrollToCaret();

            this.ResumeRedrawing();
            this.Invalidate();

            this.tRedraw.Start();
        }
        #endregion

        private const string newLine = "\r\n";
        private const char semicolon = ';';
        private const string tab = "   ";
        private const string errorMessageNotClosed = "P��kaz nen� uzav�en� nebo nekon�� st�edn�kem, nelze jej vyhodnotit!";
    }
}