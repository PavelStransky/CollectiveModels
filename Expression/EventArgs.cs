using System;

namespace PavelStransky.Expression
{
	/// <summary>
	/// T��da k p�ed�v�n� ��dosti o vytvo�en� nov�ho grafu
	/// </summary>
	public class GraphRequestEventArgs: EventArgs {
		private Variable variable;
		private Context context;

		/// <summary>
		/// Konstruktor
		/// </summary>
		public GraphRequestEventArgs(Context context, Variable variable) {
			this.context = context;
			this.variable = variable;
		}

		public Context Context {get {return this.context;}}
		public Variable Variable {get {return this.variable;}}
	}

	/// <summary>
	/// T��da k p�ed�v�n� informac� p�i zm�n� prom�nn�
	/// </summary>
	public class ChangeEventArgs: EventArgs {
		private Context context;
		// P�vodn� objekt
		private object oldItem;
		// Nov� objekt
		private object newItem;

		/// <summary>
		/// Konstruktor
		/// </summary>
		public ChangeEventArgs(Context context, object oldItem, object newItem) {
			this.context = context;
			this.oldItem = oldItem;
			this.newItem = newItem;
		}

		public Context Context {get {return this.context;}}
		public object OldItem {get {return this.oldItem;}}
		public object NewItem {get {return this.newItem;}}
	}
}
