using System;
using System.Timers;

namespace PavelStransky.Expression {
	/// <summary>
	/// Zapouzdøuje èasovaè
	/// </summary>
	public class Timer: System.Timers.Timer {
		private Atom atom;

		/// <summary>
		/// Konstruktor
		/// </summary>
		public Timer(Atom atom, int period) : base(period) {
			this.atom = atom;

			this.AutoReset = true;
			this.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
			this.Enabled = true;
		}

		/// <summary>
		/// Spustí výpoèet
		/// </summary>
		private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
			this.atom.Evaluate();
		}
	}
}
