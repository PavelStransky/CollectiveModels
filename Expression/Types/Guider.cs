using System;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.Text;

using PavelStransky.Expression.Functions;
using PavelStransky.Core;

namespace PavelStransky.Expression {
    /// <summary>
    /// Pr�vodce v�po�tem
    /// </summary>
    public class Guider: IOutputWriter {
        public enum GuiderState {
            Normal, WaitingForPause, Paused
        }

        private IOutputWriter writer;
        private Context context;
        private Context localContext = null;
        private string execDir;
        private string tmpDir;
        private bool arrayEvaluation = false;
        private bool mute = false;

        private GuiderState state = GuiderState.Normal;
        private DateTime startTime;

        private DateTime timer;

        // P�i pozastaven� v�po�tu
        public event EventHandler CalcPaused;

        // Thread, ve kter�m se prov�d� v�po�et
        private ProcessThread thread;

        /// <summary>
        /// Thread, ve kter�m se po��t�
        /// </summary>
        public ProcessThread Thread { get { return this.thread; } set { this.thread = value; } }

        /// <summary>
        /// Vr�t� po�et tick�
        /// </summary>
        public long GetThreadTicks() {
            if(this.thread != null)
                return this.thread.TotalProcessorTime.Ticks;
            else
                return 0;
        }

        // P�i pozastaven� v�po�tu
        protected virtual void OnCalcPaused(EventArgs e) {
            this.state = GuiderState.Paused;

            if(this.CalcPaused != null)
                this.CalcPaused(this, e);
        }

        // Funkce, kter� se prov�d�j�
        private ArrayList functions;

        // Pro synchronizaci a pozastaven� threadu
        private ManualResetEvent resetEvent;

        private Mutex functionMutex = new Mutex(false);

        public string ExecDir { get { return this.execDir; } set { this.execDir = value; } }
        public string TmpDir { get { return this.tmpDir; } set { this.tmpDir = value; } }

        /// <summary>
        /// Kontext v�po�tu
        /// </summary>
        public Context Context { get { return this.context; } }

        /// <summary>
        /// Lok�ln� kontext
        /// </summary>
        public Context LocalContext { get { return this.localContext; } }

        /// <summary>
        /// Bude prov�d�n v�po�et �adou?
        /// </summary>
        public bool ArrayEvaluation { get { return this.arrayEvaluation; } set { this.arrayEvaluation = value; } }

        /// <summary>
        /// True pokud bude utlumen v�stup do writeru
        /// </summary>
        public bool Mute { get { return this.mute; } set { this.mute = value; } }

        /// <summary>
        /// Pro synchronizaci a pozastaven� v�po�tu
        /// </summary>
        public ManualResetEvent ResetEvent { get { return this.resetEvent; } }

        /// <summary>
        /// P�i pozastaven� v�po�tu
        /// </summary>
        public bool Paused { get { return this.state == GuiderState.Paused; } }

        /// <summary>
        /// Pokud v�po�et �ek� na pozastaven�
        /// </summary>
        public bool WaitingForPause { get { return this.state == GuiderState.WaitingForPause; } }

        /// <summary>
        /// Po��tek v�po�tu
        /// </summary>
        public DateTime StartTime { get { return this.startTime; } }

        /// <summary>
        /// Timer
        /// </summary>
        public DateTime Timer { get { return this.timer; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">Kontext, na kter�m se bude prov�d�t v�po�et</param>
        public Guider(Context context) {
            this.context = context;
            this.resetEvent = new ManualResetEvent(true);
            this.functions = new ArrayList();
            this.startTime = DateTime.Now;
            this.timer = DateTime.Now;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="context">Kontext, na kter�m se bude prov�d�t v�po�et</param>
        /// <param name="localContext">Lok�ln� kontext</param>
        /// <param name="writer">Vypisova� na obrazovku</param>
        public Guider(Context context, Context localContext, IOutputWriter writer)
            : this(context) {
            this.writer = writer;
            this.localContext = localContext;
        }

          /// <summary>
        /// Vytvo�� nov� Guider, do kter�ho p�ekop�ruje data ze
        /// st�vaj�c�ho Guideru a vym�n� kontext
        /// </summary>
        /// <param name="context">Nov� kontext</param>
        public Guider ChangeContext(Context context) {
            Guider result = new Guider(context);
            result.localContext = this.localContext;
            result.writer = this.writer;
            result.arrayEvaluation = this.arrayEvaluation;
            result.execDir = this.execDir;
            result.tmpDir = this.tmpDir;
            result.mute = this.mute;
            result.CalcPaused = this.CalcPaused;

            return result;
        }

        /// <summary>
        /// Za��n� se zpracov�vat funkce - ulo��me jej� n�zev
        /// </summary>
        /// <param name="fnName">N�zev funkce</param>
        public void StartFunction(Fnc function) {
            this.functionMutex.WaitOne();
            this.functions.Insert(0, function);
            this.functionMutex.ReleaseMutex();
        }

        /// <summary>
        /// Funkce skon�ila
        /// </summary>
        public void EndFunction() {
            this.functionMutex.WaitOne();
            this.functions.RemoveAt(0);
            this.functionMutex.ReleaseMutex();
        }

        /// <summary>
        /// Vr�t� aktu�ln� zpracov�vanou funkc�
        /// </summary>
        public string GetCurrentFunction() {
            this.functionMutex.WaitOne();
            lock(this.functions) {
                if(this.functions.Count > 0) {
                    this.functionMutex.ReleaseMutex();
                    return (this.functions[0] as Fnc).Name;
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
        /// Pozastaven� v�po�tu
        /// </summary>
        public void Pause() {
            if(this.state != GuiderState.Paused)
                this.state = GuiderState.WaitingForPause;
            this.resetEvent.Reset();            
        }

        /// <summary>
        /// Znovurozb�hnut� v�po�tu
        /// </summary>
        public void Resume() {
            this.state = GuiderState.Normal;
            this.resetEvent.Set();
        }

        /// <summary>
        /// Zresetuje �asova�
        /// </summary>
        internal void ResetTimer() {
            this.timer = DateTime.Now;
        }

        #region Implementace IOutputWriter
        /// <summary>
        /// Vyp�e objekt do writeru
        /// </summary>
        /// <param name="o">Object</param>
        public string Write(object o) {
            if(this.writer != null && !this.mute)
                return this.writer.Write(o);
            else
                return string.Empty;
        }

        /// <summary>
        /// Vyp�e objekt do writeru a zalom� ��dku
        /// </summary>
        /// <param name="o">Object</param>
        public string WriteLine(object o) {
            if(this.writer != null && !this.mute)
                return this.writer.WriteLine(o);
            else
                return string.Empty;
        }

        /// <summary>
        /// Zalom� ��dku na writeru
        /// </summary>
        /// <param name="o">Object</param>
        public string WriteLine() {
            if(this.writer != null && !this.mute)
                return this.writer.WriteLine();
            else
                return string.Empty;
        }

        /// <summary>
        /// Vyma�e v�e na writeru
        /// </summary>
        public void Clear() {
            if(this.writer != null && !this.mute)
                this.writer.Clear();
        }

        /// <summary>
        /// Odsazen� v�sledku
        /// </summary>
        public int Indent(int i) {
            if(this.writer != null && !this.mute)
                return this.writer.Indent(i);
            else
                return i;
        }
        #endregion

        /// <summary>
        /// Vr�t� seznam v�ech funkc�
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public string GetFunctions(bool numbers) {
            StringBuilder result = new StringBuilder();
            this.functionMutex.WaitOne();
            lock(this.functions) {
                int i = this.functions.Count;
                foreach(Fnc fnc in this.functions)
                    result.AppendFormat("{0}:{1} ({2}){3}", 
                        i--, 
                        fnc.Name,
                        fnc.Calls,
                        Environment.NewLine);
            }
            this.functionMutex.ReleaseMutex();
            return result.ToString();
        }
    }
}
