using System;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Expression {
    /// <summary>
    /// Prùvodce výpoètem
    /// </summary>
    public class Guider: IOutputWriter {
        public enum GuiderState {
            Normal, WaitingForPause, Paused
        }

        private IOutputWriter writer;
        private Context context;
        private string execDir;
        private string tmpDir;
        private bool arrayEvaluation = false;
        private bool mute = false;

        private GuiderState state = GuiderState.Normal;
        private DateTime startTime;

        // Pøi pozastavení výpoètu
        public event EventHandler CalcPaused;

        // Thread, ve kterém se provádí výpoèet
        private ProcessThread thread;

        /// <summary>
        /// Thread, ve kterém se poèítá
        /// </summary>
        public ProcessThread Thread { get { return this.thread; } set { this.thread = value; } }

        /// <summary>
        /// Vrátí poèet tickù
        /// </summary>
        public long GetThreadTicks() {
            if(this.thread != null)
                return this.thread.TotalProcessorTime.Ticks;
            else
                return 0;
        }

        // Pøi pozastavení výpoètu
        protected virtual void OnCalcPaused(EventArgs e) {
            this.state = GuiderState.Paused;

            if(this.CalcPaused != null)
                this.CalcPaused(this, e);
        }

        // Jména funkcí, která se provádìjí
        private ArrayList functions;

        // Pro synchronizaci a pozastavení threadu
        private ManualResetEvent resetEvent;

        private Mutex functionMutex = new Mutex(false, "GuiderLock");

        public string ExecDir { get { return this.execDir; } set { this.execDir = value; } }
        public string TmpDir { get { return this.tmpDir; } set { this.tmpDir = value; } }

        /// <summary>
        /// Kontext výpoètu
        /// </summary>
        public Context Context { get { return this.context; } }

        /// <summary>
        /// Bude provádìn výpoèet øadou?
        /// </summary>
        public bool ArrayEvaluation { get { return this.arrayEvaluation; } set { this.arrayEvaluation = value; } }

        /// <summary>
        /// True pokud bude utlumen výstup do writeru
        /// </summary>
        public bool Mute { get { return this.mute; } set { this.mute = value; } }

        /// <summary>
        /// Pro synchronizaci a pozastavení výpoètu
        /// </summary>
        public ManualResetEvent ResetEvent { get { return this.resetEvent; } }

        /// <summary>
        /// Pøi pozastavení výpoètu
        /// </summary>
        public bool Paused { get { return this.state == GuiderState.Paused; } }

        /// <summary>
        /// Pokud výpoèet èeká na pozastavení
        /// </summary>
        public bool WaitingForPause { get { return this.state == GuiderState.WaitingForPause; } }

        /// <summary>
        /// Poèátek výpoètu
        /// </summary>
        public DateTime StartTime { get { return this.startTime; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">Kontext, na kterém se bude provádìt výpoèet</param>
        public Guider(Context context) {
            this.context = context;
            this.resetEvent = new ManualResetEvent(true);
            this.functions = new ArrayList();
            this.startTime = DateTime.Now;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">Kontext, na kterém se bude provádìt výpoèet</param>
        /// <param name="writer">Vypisovaè na obrazovku</param>
        public Guider(Context context, IOutputWriter writer)
            : this(context) {
            this.writer = writer;
        }

        /// <summary>
        /// Vytvoøí nový Guider, do kterého pøekopíruje data ze
        /// stávajícího Guideru a vymìní kontext
        /// </summary>
        /// <param name="context">Nový kontext</param>
        public Guider ChangeContext(Context context) {
            Guider result = new Guider(context);
            result.writer = this.writer;
            result.arrayEvaluation = this.arrayEvaluation;
            result.execDir = this.execDir;
            result.tmpDir = this.tmpDir;
            result.mute = this.mute;
            result.CalcPaused = this.CalcPaused;

            return result;
        }

        /// <summary>
        /// Zaèíná se zpracovávat funkce - uložíme její název
        /// </summary>
        /// <param name="fnName">Název funkce</param>
        public void StartFunction(string fnName) {
            this.functionMutex.WaitOne();
            this.functions.Insert(0, fnName);
            this.functionMutex.ReleaseMutex();
        }

        /// <summary>
        /// Funkce skonèila
        /// </summary>
        public void EndFunction() {
            this.functionMutex.WaitOne();
            this.functions.RemoveAt(0);
            this.functionMutex.ReleaseMutex();
        }

        /// <summary>
        /// Vrátí aktuálnì zpracovávanou funkcí
        /// </summary>
        public string GetCurrentFunction() {
            this.functionMutex.WaitOne();
            lock(this.functions) {
                if(this.functions.Count > 0) {
                    this.functionMutex.ReleaseMutex();
                    return (string)this.functions[0];
                }
            }
            this.functionMutex.ReleaseMutex();
            return string.Empty;
        }

        public void WaitOne() {
            if(this.state == GuiderState.WaitingForPause)
                this.OnCalcPaused(new EventArgs());
            this.ResetEvent.WaitOne();
        }

        /// <summary>
        /// Pozastavení výpoètu
        /// </summary>
        public void Pause() {
            if(this.state != GuiderState.Paused)
                this.state = GuiderState.WaitingForPause;
            this.resetEvent.Reset();            
        }

        /// <summary>
        /// Znovurozbìhnutí výpoètu
        /// </summary>
        public void Resume() {
            this.state = GuiderState.Normal;
            this.resetEvent.Set();
        }

        #region Implementace IOutputWriter
        /// <summary>
        /// Vypíše objekt do writeru
        /// </summary>
        /// <param name="o">Object</param>
        public string Write(object o) {
            if(this.writer != null && !this.mute)
                return this.writer.Write(o);
            else
                return string.Empty;
        }

        /// <summary>
        /// Vypíše objekt do writeru a zalomí øádku
        /// </summary>
        /// <param name="o">Object</param>
        public string WriteLine(object o) {
            if(this.writer != null && !this.mute)
                return this.writer.WriteLine(o);
            else
                return string.Empty;
        }

        /// <summary>
        /// Zalomí øádku na writeru
        /// </summary>
        /// <param name="o">Object</param>
        public string WriteLine() {
            if(this.writer != null && !this.mute)
                return this.writer.WriteLine();
            else
                return string.Empty;
        }

        /// <summary>
        /// Vymaže vše na writeru
        /// </summary>
        public void Clear() {
            if(this.writer != null && !this.mute)
                this.writer.Clear();
        }

        /// <summary>
        /// Odsazení výsledku
        /// </summary>
        public int Indent(int i) {
            if(this.writer != null && !this.mute)
                return this.writer.Indent(i);
            else
                return i;
        }
        #endregion

        /// <summary>
        /// Vrátí seznam všech funkcí
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public string GetFunctions(bool numbers) {
            StringBuilder result = new StringBuilder();
            this.functionMutex.WaitOne();
            lock(this.functions) {
                int i = this.functions.Count;
                foreach(string s in this.functions)
                    result.AppendFormat("{0}:{1}{2}", i--, s, Environment.NewLine);
            }
            this.functionMutex.ReleaseMutex();
            return result.ToString();
        }
    }
}
