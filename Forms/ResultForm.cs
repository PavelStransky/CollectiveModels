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

using PavelStransky.Core;
using PavelStransky.Expression;

namespace PavelStransky.Forms {
    public partial class ResultForm: ChildForm, IOutputWriter, IExportable {
        // Výraz
        private Expression.Expression expression;

        // Thread, ve kterém pobìží výpoèet
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

        private static SoundPlayer soundSuccess = new SoundPlayer(Path.Combine(Application.StartupPath, soundSuccessFile));
        private static SoundPlayer soundFailure = new SoundPlayer(Path.Combine(Application.StartupPath, soundFailureFile));
        // Poèátek výpoètu
        DateTime startTime = DateTime.Now;

        // Probíhající výpoèet a pausa
        private bool calculating = false;
        private bool paused = false;

        /// <summary>
        /// True pro probíhající výpoèet
        /// </summary>
        public bool Calculating { get { return this.calculating; } }

        /// <summary>
        /// True, pokud byl výpoèet pozastaven
        /// </summary>
        public bool Paused { get { return this.paused; } }

        /// <summary>
        /// Text pøíkazu, který se poèítá
        /// </summary>
        public TextBox TxtCommand { get { return this.txtCommand; } }

        /// <summary>
        /// Text výsledku
        /// </summary>
        public TextBox TxtResult { get { return this.txtResult; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public ResultForm() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Nastaví tlaèítka do daného stavu
        /// </summary>
        public void SetButtons() {
            if(this.calculating) {
                this.btRecalculate.Visible = false;
                this.lblResult.Visible = false;
                this.chkAsync.Visible = false;
                this.lblComputing.Visible = true;
                this.lblStartTime.Visible = true;
                this.lblLblStartTime.Visible = true;
                this.lblDuration.Visible = true;
                this.lblLblDuration.Visible = true;

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

                this.lblStartTime.Visible = false;
                this.lblLblStartTime.Visible = false;
                this.lblDuration.Visible = false;
                this.lblLblDuration.Visible = false;

                this.btContinue.Visible = false;
                this.btInterrupt.Visible = false;
                this.btPause.Visible = false;
            }
        }

        /// <summary>
        /// Výraz
        /// </summary>
        /// <param name="command">Pøíkaz</param>
        public void SetExpression(string command) {
            try {
                this.expression = new PavelStransky.Expression.Expression(command);
                this.txtCommand.Text = command.Replace(Environment.NewLine, "\n").Replace("\n", Environment.NewLine);
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

        /// <summary>
        /// Nastaví vše pro zobrazení èasu spuštìní a doby výpoètu (vèetnì timeru)
        /// </summary>
        public void SetStartTime() {
            this.startTime = DateTime.Now;
            this.lblStartTime.Text = this.startTime.ToString("g");
            this.lblDuration.Text = string.Empty;

            this.timerInfo.Interval = 1000;
            this.timerInfo.Start();
        }

        /// <summary>
        /// Tiknutí pro informaci o dobì trvání výpoètu
        /// </summary>
        private void timerInfo_Tick(object sender, EventArgs e) {
            TimeSpan duration = DateTime.Now - this.startTime;
            if(this.timerInfo.Interval != 60000 && duration.Hours != 0)
                this.timerInfo.Interval = 60000;
            duration.Subtract(new TimeSpan(0, 0, 0, 0, duration.Milliseconds));
            this.lblDuration.Text = SpecialFormat.Format(duration);
        }

        #region Obsluha vlastních událostí
        public delegate void FinishedEventHandler(object sender, FinishedEventArgs e);

        public event EventHandler CalcStarted;
        public event FinishedEventHandler CalcFinished;
        public event EventHandler CalcPaused;

        protected virtual void OnCalcStarted(EventArgs e) {
            if(this.CalcStarted != null)
                this.CalcStarted(this, e);
        }

        protected virtual void OnCalcFinished(FinishedEventArgs e) {
            if(this.CalcFinished != null)
                this.CalcFinished(this, e);
        }

        protected virtual void OnCalcPaused(EventArgs e) {
            if(this.CalcPaused != null)
                this.CalcPaused(this, e);
        }
        #endregion

        /// <summary>
        /// Zahájí výpoèet
        /// </summary>
        public void Start() {
            if(!this.calculating && this.calcThread != null) {
                this.calculating = true;

                // Nastavení ovládacích prvkù
                this.txtResult.Clear();
                this.SetCaption(Messages.MCalculating);
                this.SetButtons();

                this.SetStartTime();
                this.OnCalcStarted(new EventArgs());

                this.indent = 0;
                if(this.chkAsync.Checked)
                    this.calcThread.Start();
                else
                    this.ThreadStart();
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
            Context context = this.ParentEditor.Context;

            try {
                Guider guider = new Guider(context, this);
                guider.ExecDir = Path.GetDirectoryName(Application.ExecutablePath);
                guider.TmpDir = Application.UserAppDataPath;

                object result = this.expression.Evaluate(guider);
                this.timerInfo.Stop();
                // Po skonèení výpoètu
                this.Invoke(new FinishedCalculationDelegate(this.FinishedCalculation), result);
            }
            catch(Exception exc) {
                this.timerInfo.Stop();
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
            PositionTextException pexc = exc as PositionTextException;

            if(WinMain.PlaySounds)
                soundFailure.Play();

            if(pexc != null) {
                string[] s = pexc.TextParts();
                    StringBuilder e = new StringBuilder();
                e.Append(s[0]);
                if(s.Length > 1)
                {
                    e.Append("\n->");
                    e.Append(s[1]);
                }
                if(s.Length > 2)
                {
                    e.Append("<-\n");
                    e.Append(s[2]);
                }
                MessageBox.Show(this, string.Format("{0}\n\n{1}", pexc.GetText("\n\n"), e.ToString()), Messages.EMCalculationError);
            }
            else if(dexc != null)
                MessageBox.Show(this, dexc.GetText("\n\n"));
            else
                MessageBox.Show(this, exc.Message);

            this.calculating = false;
            this.paused = false;

            this.SetCaption(Messages.MInterrupted);
            this.SetButtons();

            this.OnCalcFinished(new FinishedEventArgs(false));
        }

        /// <summary>
        /// Voláno po ukonèení výpoètu
        /// </summary>
        /// <param name="result">Výsledky výpoètu</param>
        private void FinishedCalculation(object result) {
            TimeSpan duration = DateTime.Now - this.startTime;

            this.calculating = false;
            this.paused = false;

            this.SetCaption(Messages.MFinished);
            this.SetButtons();

            if(WinMain.PlaySounds)
                soundSuccess.Play();

            if(this.txtResult.Text != string.Empty)
                this.txtResult.Text += Environment.NewLine;
            this.txtResult.Text += string.Format(Messages.MCalculationTime, SpecialFormat.Format(duration));

            if(result != null) {
                if(result is Variable)
                    result = (result as Variable).Item;

                string s = Environment.NewLine + Environment.NewLine + result.GetType().FullName;
                s += Environment.NewLine + result.ToString().Replace("\r", string.Empty).Replace("\n", Environment.NewLine);

                this.txtResult.Text += s;
            }

            this.OnCalcFinished(new FinishedEventArgs(true));
        }

        /// <summary>
        /// Nastaví titulek okna
        /// </summary>
        /// <param name="captionText">Text titulku</param>
        private void SetCaption(string captionText) {
            this.Text = string.Format(captionFormat, this.Name, captionText);
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
                DialogResult dialogResult = MessageBox.Show(this, string.Format(Messages.MClose, this.Name), Messages.MCloseCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

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
            this.SetCaption(Messages.MPaused);
            this.paused = true;
            this.SetButtons();

            this.OnCalcPaused(new EventArgs());
        }

        /// <summary>
        /// Stisknutí tlaèítka Pokraèovat
        /// </summary>
        private void btContinue_Click(object sender, EventArgs e) {
            this.calcThread.Resume();

            // Nastavení ovládacích prvkù
            this.SetCaption(Messages.MCalculating);
            this.paused = false;
            this.SetButtons();

            this.OnCalcStarted(new EventArgs());
        }

        /// <summary>
        /// Znovu spustí výpoèet
        /// </summary>
        private void btRecalculate_Click(object sender, EventArgs e) {
            this.calcThread = new Thread(new ThreadStart(this.ThreadStart));
            this.Start();
        }

        #region IOutputWriter Members
        private int indent = 0;
        private bool lineStart = true;

        public int Indent(int indent) {
            this.indent += indent;
            if(this.indent < 0)
                this.indent = 0;
            return indent;
        }

        public void Clear() {
            this.Invoke(new WriteDelegate(this.WriteInvoke), string.Empty);
        }

        public string Write(object o) {
            StringBuilder sb = new StringBuilder(this.txtResult.Text);

            if(this.lineStart) {
                sb.Append(' ', this.indent);
            }
            sb.Append(o);

            this.Invoke(new WriteDelegate(this.WriteInvoke), sb.ToString());

            this.lineStart = false;

            return o.ToString();
        }

        public string WriteLine() {
            StringBuilder sb = new StringBuilder(this.txtResult.Text);
            sb.Append(Environment.NewLine);
            this.Invoke(new WriteDelegate(this.WriteInvoke), sb.ToString());
            this.lineStart = true;

            return Environment.NewLine;
        }

        public string WriteLine(object o) {
            StringBuilder sb = new StringBuilder(this.txtResult.Text);
            if(this.lineStart)
                sb.Append(' ', this.indent);
            sb.Append(o);
            sb.Append(Environment.NewLine);
            this.Invoke(new WriteDelegate(this.WriteInvoke), sb.ToString());
            this.lineStart = true;

            return string.Format("{0}{1}", o.ToString(), Environment.NewLine);
        }

        private void WriteInvoke(string s) {
            this.txtResult.Text = s;
            this.txtResult.SelectionStart = System.Math.Max(0, this.txtResult.Text.Length - 1);
            this.txtResult.ScrollToCaret();
        }

        #endregion

        /// <summary>
        /// Zmìna zalamování
        /// </summary>
        private void chkWrap_CheckedChanged(object sender, EventArgs e) {
            this.txtResult.WordWrap = this.chkWrap.Checked;
            this.txtCommand.WordWrap = this.chkWrap.Checked;
        }

        #region Drag and Drop obsluha
        private void ResultForm_DragEnter(object sender, DragEventArgs e) {
            if(!this.calculating && e.Data.GetDataPresent(DataFormats.Text)) {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void ResultForm_DragDrop(object sender, DragEventArgs e) {
            this.SetExpression(e.Data.GetData(DataFormats.Text).ToString());
            this.Start();
        }
        #endregion

        #region Implementace IExportable
        /// <summary>
        /// Uloží obsah formuáøe do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.Location.X, "X");
            param.Add(this.Location.Y, "Y");
            param.Add(this.Size.Width, "Width");
            param.Add(this.Size.Height, "Height");

            param.Add(this.Name, "ResultName");
            param.Add(this.txtCommand.Text, "Command");
            param.Add(this.txtResult.Text, "Result");

            param.Add(this.WindowState.ToString(), "WindowState");

            param.Export(export);
        }

        /// <summary>
        /// Naète obsah formuláøe ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public ResultForm(Core.Import import)
            : this() {
            IEParam param = new IEParam(import);

            this.Location = new Point((int)param.Get(0), (int)param.Get(0));
            this.Size = new Size((int)param.Get(this.Size.Width), (int)param.Get(this.Size.Height));

            this.Name = (string)param.Get(string.Empty);

            this.txtCommand.Text = (string)param.Get(string.Empty);
            this.txtResult.Text = (string)param.Get(string.Empty);

            this.WindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), (string)param.Get(FormWindowState.Normal.ToString()), true);

            this.SetExpression(this.txtCommand.Text);
            this.Text = this.Name;
        }
        #endregion

        private const string captionFormat = "{0} - {1}";

        private const string soundSuccessFile = "success.wav";
        private const string soundFailureFile = "failure.wav";
    }
}