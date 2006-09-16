using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using PavelStransky.Math;
using PavelStransky.Expression;

public partial class _Default: System.Web.UI.Page, IOutputWriter {
    private string txtResult = string.Empty;

    public string TxtResult { get { return this.txtResult; } }

    /// <summary>
    /// Vyzvedne Context ze Session
    /// </summary>
    public Context ExpContext {
        get {
            Context context = this.Session[nameContext] as Context;

            if(context == null) {
                context = new Context();
                this.Session.Add(nameContext, context);
            }

            return context;
        }
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
    /// Pøi stisku tlaèítka Button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void btExecute_Click(object sender, EventArgs e) {
        Expression expression = new Expression(this.ExpContext, this.txtCommand.Text, this);

        DateTime startTime = DateTime.Now;
        object result = expression.Evaluate();
        TimeSpan duration = DateTime.Now - startTime;

        this.txtResult = string.Format(timeText, this.GetTimeLengthString(duration));

        if(result != null) {
            if(result is Variable)
                result = (result as Variable).Item;

            string s = newLine + newLine + result.GetType().FullName;
            s += newLine + result.ToString().Replace("\r", string.Empty).Replace("\n", newLine).Replace("\t", tab);

            this.txtResult += s;
        }
    }

    protected void Page_Load(object sender, EventArgs e) {
        this.btExecute.Click += new EventHandler(btExecute_Click);

    }

    private const string newLine = "&nbsp;</td></tr><tr><td>";
    private const string tab = "</td><td>";
    private const string timeText = "Doba výpoètu: {0}";

    private const string nameContext = "context";

    #region IOutputWriter Members

    public void Clear() {
    }

    public void Write(object o) {
    }

    public void WriteLine() {
    }

    public void WriteLine(object o) {
    }

    #endregion
}
