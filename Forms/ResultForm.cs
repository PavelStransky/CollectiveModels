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
        // V�raz
        private Expression.Expression expression;

        // Thread, ve kter�m pob�� v�po�et
        private Thread calcThread;

        // Pomocn� prom�nn�, kter� p�en�� stav OnFormClosing na Close
        private bool cancelClosing = false;

        /// <summary>
        /// Deleg�t pro invoke metodu
        /// </summary>
        /// <param name="result">V�sledek v�po�tu</param>
        private delegate void FinishedCalculationDelegate(object result);

        /// <summary>
        /// Deleg�t pro invoke metodu zpracov�n� v�jimky
        /// </summary>
        /// <param name="exc">V�jimka</param>
        private delegate void ExceptionDelegate(Exception exc);

        /// <summary>
        /// Deleg�t pro invoke metodu v�stupu
        /// </summary>
        /// <param name="text">Text</param>
        private delegate void WriteDelegate(string text);

        // Po��tek v�po�tu
        DateTime startTime = DateTime.Now;

        // Prob�haj�c� v�po�et a pausa
        private bool calculating = false;
        private bool paused = false;

        /// <summary>
        /// True pro prob�haj�c� v�po�et
        /// </summary>
        public bool Calulating { get { return this.calculating; } }

        /// <summary>
        /// True, pokud byl v�po�et pozastaven
        /// </summary>
        public bool Paused { get { return this.paused; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public ResultForm() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Nastav� tla��tka do dan�ho stavu
        /// </summary>
        public void SetButtons() {
            if(this.calculating) {
                this.btRecalculate.Visible = false;
                this.lblResult.Visible = false;
                this.chkAsync.Visible = false;
                this.lblComputing.Visible = true;

                if(this.chkAsync.Checked) {
                    if(this.paused) {
                        this.btContinue.Visible = true;
                        this.btInterrupt.Visible = false;
                        this.btPause.Visible = false;
                    }
                    else {
                        this.btContinue.Visible = false;
                        this.btInterrupt.Visible = true;
                        this.btPause.Visible = true;
                    }
                }
                else {
                    this.btContinue.Visible = false;
                    this.btInterrupt.Visible = false;
                    this.btPause.Visible = false;
                }
            }

            else {
                if(this.expression != null)
                    this.btRecalculate.Visible = true;
                else
                    this.btRecalculate.Visible = false;

                this.lblResult.Visible = true;
                this.chkAsync.Visible = true;
                this.lblComputing.Visible = false;

                this.btContinue.Visible = false;
                this.btInterrupt.Visible = false;
                this.btPause.Visible = false;
            }
        }

        /// <summary>
        /// V�raz
        /// </summary>
        /// <param name="command">P��kaz</param>
        public void SetExpression (string command) {
            try {
                this.expression = new PavelStransky.Expression.Expression(command, this); 
                this.txtCommand.Text = command.Replace(newLine, "\n").Replace("\n", newLine);
                this.calcThread = new Thread(new ThreadStart(this.ThreadStart));
                this.calcThread.Priority = ThreadPriority.BelowNormal;
                this.SetButtons();
            }
            catch(Exception exc) {
                this.expression = null;
                this.calcThread = null;
                this.CatchException(exc);
                this.SetButtons();
            }		
        }

        #region Obsluha vlastn�ch ud�lost�
        public event EventHandler CalcStarted;
        public event EventHandler CalcFinished;
        public event EventHandler CalcPaused;

        protected virtual void OnCalcStarted(EventArgs e) {
            if(this.CalcStarted != null)
                this.CalcStarted(this, e);
        }

        protected virtual void OnCalcFinished(EventArgs e) {
            if(this.CalcFinished != null)
                this.CalcFinished(this, e);
        }

        protected virtual void OnCalcPaused(EventArgs e) {
            if(this.CalcPaused != null)
                this.CalcPaused(this, e);
        }
        #endregion

        /// <summary>
        /// Zah�j� v�po�et
        /// </summary>
        public void Start() {
            if(!this.calculating && this.calcThread != null) {
                this.calculating = true;
                
                // Nastaven� ovl�dac�ch prvk�
                this.txtResult.Clear();
                this.SetCaption(captionCalculating);
                this.SetButtons();

                this.startTime = DateTime.Now;

                this.OnCalcStarted(new EventArgs());

                if(this.chkAsync.Checked) 
                    this.calcThread.Start();
                else
                    this.ThreadStart();
            }
        }

        /// <summary>
        /// P�eru�� v�po�et
        /// </summary>
        /// <param name="closing">True, pokud se p�eru�uje p�i zav�r�n� oken</param>
        public void Abort(bool closing) {
            if(this.calcThread.ThreadState == ThreadState.Suspended)
                this.calcThread.Resume();

            while(this.calcThread.ThreadState == ThreadState.Suspended) ;

            this.calcThread.Abort();
            this.calculating = !closing;
        }

        /// <summary>
        /// Spou�t� thread
        /// </summary>
        private void ThreadStart() {
            Context context = this.ParentEditor.Context;

            try {
                object result = this.expression.Evaluate(context);

                // Po skon�en� v�po�tu
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
        /// Zpracov�v� vyj�mku
        /// </summary>
        /// <param name="exc">Vyj�mka</param>
        private void CatchException(Exception exc) {
            DetailException dexc = exc as DetailException;

            if(dexc != null)
                MessageBox.Show(this, string.Format("{0}\n\n{1}", dexc.Message, dexc.DetailMessage));
            else
				MessageBox.Show(this, exc.Message);

            this.calculating = false;
            this.paused = false;

            this.SetCaption(captionInterrupted);
            this.SetButtons();

            this.OnCalcFinished(new EventArgs());
        }

        /// <summary>
        /// Vol�no po ukon�en� v�po�tu
        /// </summary>
        /// <param name="result">V�sledky v�po�tu</param>
        private void FinishedCalculation(object result) {
            TimeSpan duration = DateTime.Now - this.startTime;

            this.calculating = false;
            this.paused = false;

            this.SetCaption(captionFinished);
            this.SetButtons();

            if(this.txtResult.Text != string.Empty)
                this.txtResult.Text += newLine;
            this.txtResult.Text += string.Format(timeText, this.GetTimeLengthString(duration));

            if(result != null) {
                if(result is Variable)
                    result = (result as Variable).Item;

                string s = newLine + newLine + result.GetType().FullName;
                s += newLine + result.ToString().Replace("\r", string.Empty).Replace("\n", newLine);

                this.txtResult.Text += s;
            }

            this.OnCalcFinished(new EventArgs());
        }

        /// <summary>
        /// Nastav� titulek okna
        /// </summary>
        /// <param name="captionText">Text titulku</param>
        private void SetCaption(string captionText) {
            this.Text = string.Format(captionFormat, this.Name, captionText);
        }

        /// <summary>
        /// Vr�t� dobu v�po�tu jako �et�zec
        /// </summary>
        /// <param name="span">�asov� interval jako TimeSpan</param>
        private string GetTimeLengthString(TimeSpan span) {
            if(span.Hours > 0)
                return string.Format("{0}:{1,2:00}:{2,2:00}", span.Hours, span.Minutes, span.Seconds);
            else if(span.Minutes > 0)
                return string.Format("{0}:{1,2:00}", span.Minutes, span.Seconds);
            else
                return string.Format("{0}.{1,2:00}s", span.Seconds, span.Milliseconds / 10);
        }

        /// <summary>
        /// P�i uzav�r�n� formul��e
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e) {
            base.OnFormClosing(e);
            this.cancelClosing = this.InterruptCalculation();

            e.Cancel = this.cancelClosing;
        }

        /// <summary>
        /// Zept� se, zda ukon�it v�po�et. Pokud ano, ukon�� jej
        /// </summary>
        /// <returns>True pro prob�haj�c� v�po�et, kter� nechce u�ivatel ukon�it</returns>
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
        /// Stiknut� tla��tka P�eru�it
        /// </summary>
        private void btInterrupt_Click(object sender, EventArgs e) {
            this.Abort(false);
        }

        /// <summary>
        /// Stisknut� tla��tka Pozastavit
        /// </summary>
        private void btPause_Click(object sender, EventArgs e) {
            this.calcThread.Suspend();

            // Nastaven� ovl�dac�ch prvk�
            this.SetCaption(captionPaused);
            this.paused = true;
            this.SetButtons();

            this.OnCalcPaused(new EventArgs());
        }

        /// <summary>
        /// Stisknut� tla��tka Pokra�ovat
        /// </summary>
        private void btContinue_Click(object sender, EventArgs e) {
            this.calcThread.Resume();

            // Nastaven� ovl�dac�ch prvk�
            this.SetCaption(captionCalculating);
            this.paused = false;
            this.SetButtons();

            this.OnCalcStarted(new EventArgs());
        }

        /// <summary>
        /// Znovu spust� v�po�et
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
            this.txtResult.SuspendLayout();
            this.txtResult.Text = s;
            this.txtResult.SelectionStart = System.Math.Max(0, this.txtResult.Text.Length - 1);
            this.txtResult.ScrollToCaret();
            this.txtResult.ResumeLayout();
        }

        #endregion

        #region Implementace IExportable
        /// <summary>
        /// Ulo�� obsah formu��e do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            // Mus�me ukl�dat bin�rn�
            if(!export.Binary)
                throw new Exception("");

            // Bin�rn�
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
        /// Na�te obsah formul��e ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public void Import(PavelStransky.Math.Import import) {
            // Mus�me ��st bin�rn�
            if(!import.Binary)
                throw new Exception("");

            // Bin�rn�
            BinaryReader b = import.B;
            this.Location = new Point(b.ReadInt32(), b.ReadInt32());
            this.Size = new Size(b.ReadInt32(), b.ReadInt32());

            this.Name = b.ReadString();
            this.txtCommand.Text = b.ReadString();
            this.txtResult.Text = b.ReadString();

            this.SetExpression(this.txtCommand.Text);
            this.Text = this.Name;
        }
        #endregion

        private const string newLine = "\r\n";
        private const string timeText = "Doba v�po�tu: {0}";
        private const string captionFormat = "{0} - {1}";

        private const string messageClose = "V okn� prob�h� v�po�et. Opravdu chcete okno uzav��t a v�po�et ukon�it?";
        private const string captionClose = "Varov�n�";

        private const string captionCalculating = "Prob�h� v�po�et...";
        private const string captionFinished = "Hotovo";
        private const string captionInterrupted = "P�eru�eno";
        private const string captionPaused = "Pozastaveno";
    }
}