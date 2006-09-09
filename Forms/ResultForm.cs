using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Forms {
    public partial class ResultForm: ContextForm, IOutputWriter {
        /// V�raz
        private Expression.Expression expression;

        /// Thread, ve kter�m pob�� v�po�et
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

        // Prob�haj�c� v�po�et
        private bool calculating;

        /// <summary>
        /// True pro prob�haj�c� v�po�et
        /// </summary>
        public bool Calulating { get { return this.calculating; } }

        /// <summary>
        /// V�raz
        /// </summary>
        public void SetExpression (Expression.Context expContext, string command) {
            try {
                this.calcThread = null;
                this.expression = new PavelStransky.Expression.Expression(expContext, command, this); 
                this.txtCommand.Text = command;
                this.calcThread = new Thread(new ThreadStart(this.ThreadStart));
            }
            catch(Exception exc) {
                this.CatchException(exc);
            }		
        }

        /// <summary>
        /// Zah�j� v�po�et
        /// </summary>
        public void Start() {
            if(!this.calculating && this.calcThread != null) {
                this.calculating = true;
                
                this.txtResult.Clear();

                this.startTime = DateTime.Now;
                this.calcThread.Priority = ThreadPriority.BelowNormal;
                this.calcThread.Start();

                // Nastaven� ovl�dac�ch prvk�
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
        /// Spou�t� thread
        /// </summary>
        private void ThreadStart() {
            try {
                object result = this.expression.Evaluate();

                // Po skon�en� v�po�tu
                this.Invoke(new FinishedCalculationDelegate(this.FinishedCalculation), result);
            }
            catch(Exception exc) {
                try {
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
            this.txtResult.Clear();

            // Nastaven� ovl�dac�ch prvk�
            this.btRecalculate.Visible = false;
            this.btInterrupt.Visible = false;
            this.btContinue.Visible = false;
            this.btPause.Visible = false;
            this.lblComputing.Visible = false;
            this.lblResult.Visible = true;
            this.SetCaption(captionInterrupted);
        }

        /// <summary>
        /// Vol�no po ukon�en� v�po�tu
        /// </summary>
        /// <param name="result">V�sledky v�po�tu</param>
        private void FinishedCalculation(object result) {
            TimeSpan duration = DateTime.Now - this.startTime;

            // Nastaven� ovl�dac�ch prvk�
            this.btRecalculate.Visible = true;
            this.btInterrupt.Visible = false;
            this.btContinue.Visible = false;
            this.btPause.Visible = false;
            this.lblComputing.Visible = false;
            this.lblResult.Visible = true;
            this.SetCaption(captionFinished);

            this.txtResult.Clear();
            this.txtResult.Text = string.Format(timeText, this.GetTimeLengthString(duration));

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
        /// Konstruktor
        /// </summary>
        public ResultForm()
            : base() {
        }

        /// <summary>
        /// Konstruktor s kontextem
        /// </summary>
        /// <param name="context">Kontext</param>
        public ResultForm(Context context)
            : base(context) {
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
        /// Uzav�en� formul��e
        /// </summary>
        public override bool Close() {
            base.Close();

            return this.cancelClosing;
        }

        /// <summary>
        /// Stiknut� tla��tka P�eru�it
        /// </summary>
        private void btInterrupt_Click(object sender, EventArgs e) {
            if(this.calcThread.ThreadState == ThreadState.Suspended)
                this.calcThread.Resume();

            while(this.calcThread.ThreadState == ThreadState.Suspended) ;

            this.calcThread.Abort();
        }

        /// <summary>
        /// Stisknut� tla��tka Pozastavit
        /// </summary>
        private void btPause_Click(object sender, EventArgs e) {
            this.calcThread.Suspend();

            // Nastaven� ovl�dac�ch prvk�
            this.SetCaption(captionPaused);
            this.btPause.Visible = false;
            this.btContinue.Visible = true;
        }

        /// <summary>
        /// Stisknut� tla��tka Pokra�ovat
        /// </summary>
        private void btContinue_Click(object sender, EventArgs e) {
            this.calcThread.Resume();

            // Nastaven� ovl�dac�ch prvk�
            this.SetCaption(captionCalculating);
            this.btContinue.Visible = false;
            this.btPause.Visible = true;

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
            this.txtResult.Text = s;
        }
        #endregion

        private const string newLine = "\r\n";
        private const string timeText = "Doba v�po�tu: {0}";
        private const string captionFormat = "{0} - {1}";

        private const string messageClose = "V okn� {0} prob�h� v�po�et. Opravdu chcete okno uzav��t a v�po�et ukon�it?";
        private const string captionClose = "Varov�n�";

        private const string captionCalculating = "Prob�h� v�po�et...";
        private const string captionFinished = "Hotovo";
        private const string captionInterrupted = "P�eru�eno";
        private const string captionPaused = "Pozastaveno";
    }
}