using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Forms {
    /// <summary>
    /// Formul�� pro editaci
    /// </summary>
    public partial class Editor : Form {
        //Kontext s v�razy
        private Context context;

        // Jm�no souboru
        private string fileName = string.Empty;

        // Do�lo k modifikaci?
        private bool modified = false;

        // ��slo v�sledkov�ho formul��e
        private static int resultNumber = 0;

        /// <summary>
        /// Nefunguje dob�e ud�lost txtCommand_OnModifiedChanged, proto mus�me vy�e�it po sv�m
        /// </summary>
        private bool Modified {
            get {
                return this.modified;
            }
            set {
                if(this.modified != value) {
                    this.modified = value;
                    this.SetCaption();
                }
            }
        }

        /// <summary>
        /// P�e�te / nastav� jm�no souboru
        /// </summary>
        public string FileName {
            get {
                return this.fileName;
            }
            set {
                this.Directory = this.DirectoryFromFile(value);
                this.fileName = value;
            }
        }

        /// <summary>
        /// Nastav� / vyzvedne aktu�ln� adres��
        /// </summary>
        public string Directory {
            get {
                if(this.context.Contains(directoryVariable) && this.context[directoryVariable].Item is string)
                    return this.context[directoryVariable].Item as string;
                else
                    return string.Empty;
            }
            set {
                this.context.SetVariable(directoryVariable, value);
            }
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Editor() {
            this.InitializeComponent();

            this.context = new Expression.Context();
            this.context.ExitRequest += new PavelStransky.Expression.Context.ExitEventHandler(context_ExitRequest);
            this.context.GraphRequest += new PavelStransky.Expression.Context.GraphRequestEventHandler(context_GraphRequest);

            this.SetCaption();
        }

        /// <summary>
        /// Ud�lost z kontextu - ��dost o uzav�en� okna
        /// </summary>
        private void context_ExitRequest(object sender, EventArgs e) {
            this.Close();
        }

        #region Obsluha grafu
        private delegate void GraphRequestDelegate(object sender, GraphRequestEventArgs e);

        /// <summary>
        /// Ud�lost z kontextu - ��dost o vytvo�en� grafu
        /// </summary>
        private void context_GraphRequest(object sender, GraphRequestEventArgs e) {
            // Spust�me ve vlastn�m threadu
            this.Invoke(new GraphRequestDelegate(this.GraphRequestInvoke), new object[] { sender, e });
        }

        /// <summary>
        /// Vytvo�en� nov�ho formul��e mus�me spustit ve vlastn�m threadu
        /// </summary>
        private void GraphRequestInvoke(object sender, GraphRequestEventArgs e) {
            GraphForm graphForm = (this.MdiParent as MainForm).NewParentForm(typeof(GraphForm), this, e.Variable.Name) as GraphForm;

            bool isGA = false;
            GraphArray ga = null;
            int count = 1;
            int lengthX = 1;
            int lengthY = 1;

            if(e.Variable.Item is GraphArray) {
                isGA = true;
                ga = e.Variable.Item as GraphArray;
                count = ga.Count;
                lengthX = ga.LengthX;
                lengthY = ga.LengthY;
            }

            if(graphForm.NumGraphControls() != count)
                graphForm.Controls.Clear();

            for(int i = 0; i < count; i++) {
                Expression.Graph g = null;
                int index = i;
                if(isGA)
                    g = ga[i];
                else {
                    index = -1;
                    g = e.Variable.Item as Expression.Graph;
                }

                RectangleF position = new RectangleF((float)(i % lengthX) / (float)lengthX, (int)(i / lengthX) / (float)lengthY, 1 / (float)lengthX, 1 / (float)lengthY);

                if(g is Expression.DensityGraph) {
                    DensityGraph densityGraph = graphForm.GraphControl(index) as DensityGraph;

                    if(densityGraph != null)
                        densityGraph.SetVariable(e.Variable, index);
                    else {
                        graphForm.SuspendLayout();
                        densityGraph = new DensityGraph(e.Variable, index, position);
                        this.SetGraphStyles(densityGraph, graphForm, string.Format("{0}{1}", e.Variable.Name, i));
                        graphForm.Controls.Add(densityGraph);
                        graphForm.ResumeLayout();
                    }
                }
                else if(g is Expression.LineGraph) {
                    LineGraph lineGraph = graphForm.GraphControl(index) as LineGraph;

                    if(lineGraph != null)
                        lineGraph.SetVariable(e.Variable, index);
                    else {
                        graphForm.SuspendLayout();
                        lineGraph = new LineGraph(e.Variable, index, position);
                        this.SetGraphStyles(lineGraph, graphForm, string.Format("{0}{1}", e.Variable.Name, i));
                        graphForm.Controls.Add(lineGraph);
                        graphForm.ResumeLayout();
                    }
                }
            }

            graphForm.Show();
            this.Activate();
        }

        /// <summary>
        /// Nastav� styly nov�ho grafu na formul��i
        /// </summary>
        /// <param name="graph">Graf</param>
        /// <param name="form">Formul��</param>
        /// <param name="name">Jm�no controlu</param>
        /// <param name="position">Um�st�n� controlu</param>
        private void SetGraphStyles(Graph graph, Form form, string name) {
            graph.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            graph.SetPosition(form.Width, form.Height);
            graph.Name = name;
            graph.KeyDown += new KeyEventHandler(Editor_KeyDown);
        }
        #endregion

        #region Otev�r�n� a ukl�d�n� p��kaz�
        /// <summary>
        /// Pokus� se zav��t v�echna pod��zen� okna
        /// </summary>
        /// <returns>True, pokud je v�e OK a uzav�en� se poda�ilo</returns>
        private bool CloseChildWindow() {
            MainForm form = this.MdiParent as MainForm;

            for(int i = 0; i < form.MdiChildren.Length; i++) {
                // V�echny pod��zen� formul��e
                ChildForm childForm = form.MdiChildren[i] as ChildForm;

                if(childForm != null && childForm.ParentEditor == this) {
                    ResultForm resultForm = childForm as ResultForm;
                    if(resultForm != null && resultForm.Calulating) {
                        DialogResult result = MessageBox.Show(this,
                            string.Format(messageClose, resultForm.Name),
                            captionClose, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                        switch(result) {
                            case DialogResult.Yes:
                                resultForm.Abort();
                                resultForm.Close();
                                break;
                            case DialogResult.No:
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Kontroluje, zda do�lo ke zm�n�m. Pokud ano, vy�le dotaz na ulo�en�
        /// </summary>
        /// <returns>False, pokud je ��dost o zru�en� akce</returns>
        private bool CheckForChanges() {
            if(this.Modified) {
                DialogResult result = MessageBox.Show(this,
                    this.fileName == string.Empty ? messageChanged : string.Format(messageFileChanged, this.fileName),
                    captionFileChanged, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                if(result == DialogResult.Cancel)
                    return false;
                else if(result == DialogResult.No)
                    return true;
                else if(result == DialogResult.Yes)
                    return this.Save();
            }

            return true;
        }

        /// <summary>
        /// Ulo�� soubor. Pokud nezn� jm�no, otev�e dialog
        /// </summary>
        /// <returns>False, pokud soubor nebyl ulo�en</returns>
        public bool Save() {
            if(this.fileName == string.Empty) {
                this.Activate();
                (this.MdiParent as MainForm).SaveFileDialog.FileName = this.fileName;
                if((this.MdiParent as MainForm).SaveFileDialog.ShowDialog() == DialogResult.OK)
                    return true;
                else
                    return false;
            }
            else
                return this.Save(this.fileName);
        }

        /// <summary>
        /// Ulo�� p��kazy do souboru
        /// </summary>
        /// <param name="fileName">Jm�no souboru</param>
        /// <returns>False, pokud se ulo�en� nezda�ilo</returns>
        public bool Save(string fileName) {
            try {
                this.txtCommand.Export(fileName);

                this.fileName = fileName;
                this.modified = false;
                this.SetCaption();

                return true;
            }
            catch(DetailException e) {
                MessageBox.Show(this, string.Format(messageFailedSaveDetail, fileName, e.Message, e.DetailMessage),
                    captionFailedSave, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            catch(Exception e) {
                MessageBox.Show(this, string.Format(messageFailedSave, fileName, e.Message),
                    captionFailedSave, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
        }

        /// <summary>
        /// Otev�e p��kazy ze souboru
        /// </summary>
        /// <param name="fileName">Jm�no souboru</param>
        /// <returns>False, pokud se otev�en� nezda�ilo</returns>
        public bool Open(string fileName) {
            try {
                this.txtCommand.Import(fileName);

                this.FileName = fileName;
                this.modified = false;
                this.txtCommand.Modified = false;

                this.SetCaption();

                return true;
            }
            catch(DetailException e) {
                MessageBox.Show(this, string.Format(messageFailedOpenDetail, fileName, e.Message, e.DetailMessage),
                    captionFailedOpen, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            catch(Exception e) {
                MessageBox.Show(this, string.Format(messageFailedOpen, fileName, e.Message),
                    captionFailedOpen, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
        }
        #endregion

        /// <summary>
        /// P�i ��dosti o spu�t�n� p��kazu
        /// </summary>
        private void txtCommand_ExecuteCommand(object sender, PavelStransky.Forms.ExecuteCommandEventArgs e) {
            // Chceme nov� okno
            if(e.NewWindow)
                resultNumber++;

            string windowName = string.Format(defaultResultWindowName, resultNumber);

            ResultForm f = (this.MdiParent as MainForm).NewParentForm(typeof(ResultForm), this, windowName) as ResultForm;

            if(f.Calulating) {
                f.Activate();
                MessageBox.Show(this, messageCalculationRunning);
                this.Activate();
            }
            else {
                f.SetExpression(this.context, e.Expression);
                f.Show();
                f.Start();

                this.Activate();
            }
        }

        /// <summary>
        /// P�i zm�n� textu zm�n�me atribut Modified
        /// </summary>
        private void txtCommand_TextChanged(object sender, System.EventArgs e) {
            this.Modified = txtCommand.Modified;
        }

        /// <summary>
        /// nastav� nadpis okna
        /// </summary>
        private void SetCaption() {
            this.Text = string.Format(this.fileName == string.Empty ? titleFormat : titleFormatFile,
                defaultName, this.fileName, this.Modified ? asterisk : string.Empty);
        }

        /// <summary>
        /// P�i aktivaci okna nastav�me focus
        /// </summary>
        private void Editor_Activated(object sender, System.EventArgs e) {
            this.txtCommand.Focus();
        }

        /// <summary>
        /// Pro KeyDown na kontextu
        /// </summary>
        private void Editor_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
            if(e.Alt || e.Shift)
                return;
            if(e.Control && e.KeyValue <= 'Z' && e.KeyValue >= 'A') {
                try {
                    e.Handled = this.context.HotKey((char)e.KeyValue);
                }
                catch(DetailException exc) {
                    MessageBox.Show(this, string.Format("{0}\n\n{1}", exc.Message, exc.DetailMessage));
                }
                catch(Exception exc) {
                    MessageBox.Show(this, string.Format("{0}", exc.Message));
                }
            }
        }

        /// <summary>
        /// P�i uzav�r�n� okna mus�me uzav��t v�echny p�idru�en� formul��e
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e) {
            e.Cancel = !(this.CheckForChanges() && this.CloseChildWindow());
        }

        /// <summary>
        /// Podle n�zvu souboru ur�� adres��
        /// </summary>
        /// <param name="fileName">N�zev souboru s cestou</param>
        private string DirectoryFromFile(string fileName) {
            FileInfo f = new FileInfo(fileName);
            return f.DirectoryName;
        }

        private const string directoryVariable = "_dir";

        private const string messageOpen = "Chcete, aby byly v�echny p��kazy historie po otev�en� automaticky spu�t�ny?";
        private const string captionOpen = "Otev�en� historie";

        private const string messageFileChanged = "Soubor '{0}' byl zm�n�n. Chcete zm�ny ulo�it?";
        private const string messageChanged = "Data nejsou ulo�ena. Chcete je ulo�it?";
        private const string captionFileChanged = "Ulo�en� souboru";

        private const string messageFailedSave = "Ulo�en� do souboru '{0}' se nezda�ilo.\n\nPodrobnosti: {1}";
        private const string messageFailedSaveDetail = "Ulo�en� do souboru '{0}' se nezda�ilo.\n\nPodrobnosti: {1}\n\n{2}";
        private const string captionFailedSave = "Chyba!";
        private const string messageFailedOpen = "Otev�en� souboru '{0}' se nezda�ilo.\n\nPodrobnosti: {1}";
        private const string messageFailedOpenDetail = "Otev�en� souboru '{0}' se nezda�ilo.\n\nPodrobnosti: {1}\n\n{2}";
        private const string captionFailedOpen = "Chyba!";

        private const string messageClose = "V okn� {0} prob�h� v�po�et. Opravdu chcete okno uzav��t a v�po�et ukon�it?";
        private const string captionClose = "Varov�n�";

        private const string messageCalculationRunning = "V aktu�ln�m okn� prob�h� v�po�et, nelze spustit nov� v�po�et!";
        private const string defaultResultWindowName = "Result{0}";

        private const string defaultName = "GCM Progr�mek";
        private const string titleFormatFile = "{0} - {1} {2}";
        private const string titleFormat = "{0} {2}";
        private const string asterisk = "*";
    }
}
