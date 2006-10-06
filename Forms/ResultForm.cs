using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Forms {
    public partial class ResultForm: ChildForm, IOutputWriter, IExportable {
        /// Výraz
        private Expression.Expression expression;

        /// Thread, ve kterém pobìží výpoèet
        private Thread calcThread;

        // Pomocná promìnná, která pøenáší stav OnFormClosing na Close
        private bool cancelClosing = false;

        /// <summary>
        /// Delegát pro invoke metodu
        /// </summary>
        /// <param name="result">Výsledek výpoètu</param>
        private delegate void FinishedCalculationDelegate(object result);

        /// <summary>
        /// Delegát pro invoke metodu zpracování výjimky
        /// </summary>
        /// <param name="exc">Výjimka</param>
        private delegate void ExceptionDelegate(Exception exc);

        /// <summary>
        /// Delegát pro invoke metodu výstupu
        /// </summary>
        /// <param name="text">Text</param>
        private delegate void WriteDelegate(string text);

        // Poèátek výpoètu
        DateTime startTime = DateTime.Now;

        // Probíhající výpoèet
        private bool calculating;

        /// <summary>
        /// True pro probíhající výpoèet
        /// </summary>
        public bool Calulating { get { return this.calculating; } }

        /// <summary>
        /// Pro zadaný text v oknì txtCommand pøiøadí context a vytvoøí výraz
        /// </summary>
        /// <param name="expContext">Kontext</param>
        public void SetContext(Expression.Context expContext) {
            this.SetExpression(expContext, this.txtCommand.Text);
        }

        /// <summary>
        /// Výraz
        /// </summary>
        /// <param name="expContext">Kontext</param>
        /// <param name="command">Pøíkaz</param>
        public void SetExpression (Expression.Context expContext, string command) {
            try {
                this.calcThread = null;
                this.expression = new PavelStransky.Expression.Expression(expContext, command, this); 
                this.txtCommand.Text = command.Replace(newLine, "\n").Replace("\n", newLine);
                this.calcThread = new Thread(new ThreadStart(this.ThreadStart));

                this.btRecalculate.Visible = true;
                this.lblResult.Visible = true;
                this.lblComputing.Visible = false;
                this.btInterrupt.Visible = false;
                this.btPause.Visible = false;
                this.btContinue.Visible = false;
            }
            catch(Exception exc) {
                this.CatchException(exc);
                this.Close();
            }		
        }

        /// <summary>
        /// Zahájí výpoèet
        /// </summary>
        public void Start() {
            if(!this.calculating && this.calcThread != null) {
                this.calculating = true;
                
                this.txtResult.Clear();

                this.startTime = DateTime.Now;
                this.calcThread.Priority = ThreadPriority.BelowNormal;
                this.calcThread.Start();

                // Nastavení ovládacích prvkù
                this.SetCaption(captionCalculating);
                this.btRecalculate.Visible = false;
                this.lblResult.Visible = false;
                this.lblComputing.Visible = true;
                this.btInterrupt.Visible = true;
                this.btPause.Visible = true;
                this.btContinue.Visible = false;
            }
        }

        /// <summary>
        /// Pøeruší výpoèet
        /// </summary>
        /// <param name="closing">True, pokud se pøerušuje pøi zavírání oken</param>
        public void Abort(bool closing) {
            if(this.calcThread.ThreadState == ThreadState.Suspended)
                this.calcThread.Resume();

            while(this.calcThread.ThreadState == ThreadState.Suspended) ;

            this.calcThread.Abort();
            this.calculating = !closing;
        }

        /// <summary>
        /// Spouští thread
        /// </summary>
        private void ThreadStart() {
            try {
                object result = this.expression.Evaluate();

                // Po skonèení výpoètu
                this.Invoke(new FinishedCalculationDelegate(this.FinishedCalculation), result);
            }
            catch(Exception exc) {
                try {
                    if(exc.InnerException is ThreadAbortException && !this.calculating)
                        return;
                    this.Invoke(new ExceptionDelegate(this.CatchException), exc);
                }
                catch { }
            }
        }

        /// <summary>
        /// Zpracovává vyjímku
        /// </summary>
        /// <param name="exc">Vyjímka</param>
        private void CatchException(Exception exc) {
            DetailException dexc = exc as DetailException;

            if(dexc != null)
                MessageBox.Show(this, string.Format("{0}\n\n{1}", dexc.Message, dexc.DetailMessage));
            else
				MessageBox.Show(this, exc.Message);

            this.calculating = false;

            // Nastavení ovládacích prvkù
            this.btRecalculate.Visible = true;
            this.btInterrupt.Visible = false;
            this.btContinue.Visible = false;
            this.btPause.Visible = false;
            this.lblComputing.Visible = false;
            this.lblResult.Visible = true;
            this.SetCaption(captionInterrupted);
        }

        /// <summary>
        /// Voláno po ukonèení výpoètu
        /// </summary>
        /// <param name="result">Výsledky výpoètu</param>
        private void FinishedCalculation(object result) {
            TimeSpan duration = DateTime.Now - this.startTime;

            // Nastavení ovládacích prvkù
            this.btRecalculate.Visible = true;
            this.btInterrupt.Visible = false;
            this.btContinue.Visible = false;
            this.btPause.Visible = false;
            this.lblComputing.Visible = false;
            this.lblResult.Visible = true;
            this.SetCaption(captionFinished);

            this.txtResult.Text += newLine;
            this.txtResult.Text += string.Format(timeText, this.GetTimeLengthString(duration));

            if(result != null) {
                if(result is Variable)
                    result = (result as Variable).Item;

                string s = newLine + newLine + result.GetType().FullName;
                s += newLine + result.ToString().Replace("\r", string.Empty).Replace("\n", newLine);

                this.txtResult.Text += s;
            }

            this.calculating = false;
        }

        /// <summary>
        /// Nastaví titulek okna
        /// </summary>
        /// <param name="captionText">Text titulku</param>
        private void SetCaption(string captionText) {
            this.Text = string.Format(captionFormat, this.Name, captionText);
        }

        /// <summary>
        /// Vrátí dobu výpoètu jako øetìzec
        /// </summary>
        /// <param name="span">Èasový interval jako TimeSpan</param>
        private string GetTimeLengthString(TimeSpan span) {
            if(span.Hours > 0)
                return string.Format("{0}:{1,2:00}:{2,2:00}", span.Hours, span.Minutes, span.Seconds);
            else if(span.Minutes > 0)
                return string.Format("{0}:{1,2:00}", span.Minutes, span.Seconds);
            else
                return string.Format("{0}.{1,2:00}s", span.Seconds, span.Milliseconds / 10);
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public ResultForm() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Pøi uzavírání formuláøe
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e) {
            base.OnFormClosing(e);
            this.cancelClosing = this.InterruptCalculation();

            e.Cancel = this.cancelClosing;
        }

        /// <summary>
        /// Zeptá se, zda ukonèit výpoèet. Pokud ano, ukonèí jej
        /// </summary>
        /// <returns>True pro probíhající výpoèet, který nechce uživatel ukonèit</returns>
        private bool InterruptCalculation() {
            bool result = false;

            if(this.calculating) {
                DialogResult dialogResult = MessageBox.Show(this, string.Format(messageClose, this.Name), captionClose, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if(dialogResult == DialogResult.Yes)
                    this.calcThread.Abort();
                else
                    result = true;
            }

            return result;
        }

        /// <summary>
        /// Stiknutí tlaèítka Pøerušit
        /// </summary>
        private void btInterrupt_Click(object sender, EventArgs e) {
            this.Abort(false);
        }

        /// <summary>
        /// Stisknutí tlaèítka Pozastavit
        /// </summary>
        private void btPause_Click(object sender, EventArgs e) {
            this.calcThread.Suspend();

            // Nastavení ovládacích prvkù
            this.SetCaption(captionPaused);
            this.btPause.Visible = false;
            this.btContinue.Visible = true;
        }

        /// <summary>
        /// Stisknutí tlaèítka Pokraèovat
        /// </summary>
        private void btContinue_Click(object sender, EventArgs e) {
            this.calcThread.Resume();

            // Nastavení ovládacích prvkù
            this.SetCaption(captionCalculating);
            this.btContinue.Visible = false;
            this.btPause.Visible = true;

        }

        /// <summary>
        /// Znovu spustí výpoèet
        /// </summary>
        private void btRecalculate_Click(object sender, EventArgs e) {
            this.calcThread = new Thread(new ThreadStart(this.ThreadStart));
            this.Start();
        }

        #region IOutputWriter Members
        public void Clear() {
            this.Invoke(new WriteDelegate(this.WriteInvoke), string.Empty);
        }

        public void Write(object o) {
            string s = this.txtResult.Text + o.ToString();
            this.Invoke(new WriteDelegate(this.WriteInvoke), s);
        }

        public void WriteLine() {
            string s = this.txtResult.Text + newLine;
            this.Invoke(new WriteDelegate(this.WriteInvoke), s);
        }

        public void WriteLine(object o) {
            string s = this.txtResult.Text + o.ToString() + newLine;
            this.Invoke(new WriteDelegate(this.WriteInvoke), s);
        }

        private void WriteInvoke(string s) {
            this.txtResult.Text = s;
        }

        #endregion

        #region Implementace IExportable
        /// <summary>
        /// Uloží obsah formuáøe do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            // Musíme ukládat binárnì
            if(!export.Binary)
                throw new Exception("");

            // Binárnì
            BinaryWriter b = export.B;
            b.Write(this.Location.X);
            b.Write(this.Location.Y);
            b.Write(this.Size.Width);
            b.Write(this.Size.Height);

            b.Write(this.Name);
            b.Write(this.txtCommand.Text);
            b.Write(this.txtResult.Text);
        }

        /// <summary>
        /// Naète obsah formuláøe ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public void Import(PavelStransky.Math.Import import) {
            // Musíme èíst binárnì
            if(!import.Binary)
                throw new Exception("");

            // Binárnì
            BinaryReader b = import.B;
            this.Location = new Point(b.ReadInt32(), b.ReadInt32());
            this.Size = new Size(b.ReadInt32(), b.ReadInt32());

            this.Name = b.ReadString();
            this.txtCommand.Text = b.ReadString();
            this.txtResult.Text = b.ReadString();

            this.Text = this.Name;
        }
        #endregion

        private const string newLine = "\r\n";
        private const string timeText = "Doba výpoètu: {0}";
        private const string captionFormat = "{0} - {1}";

        private const string messageClose = "V oknì probíhá výpoèet. Opravdu chcete okno uzavøít a výpoèet ukonèit?";
        private const string captionClose = "Varování";

        private const string captionCalculating = "Probíhá výpoèet...";
        private const string captionFinished = "Hotovo";
        private const string captionInterrupted = "Pøerušeno";
        private const string captionPaused = "Pozastaveno";
    }
}