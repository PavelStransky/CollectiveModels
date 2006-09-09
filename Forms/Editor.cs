using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

namespace PavelStransky.Forms {
	/// <summary>
	/// Editor pøíkazù pro Context
	/// </summary>
	public class Editor : System.Windows.Forms.Form {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem mnFile;
		private System.Windows.Forms.MenuItem mnFileOpen;
		private System.Windows.Forms.MenuItem mnFileExit;

		private System.Windows.Forms.OpenFileDialog openFileDialog;

		// Jednotlivé øádky dokumentu
		private ArrayList documentLines = new ArrayList();

		// Objekt graphics
		Graphics graphics;

		// Fonty
		private Font inputFont;
		private Font outputFont;

		// Brushes
		private Brush brush;

		public Editor() {
			this.graphics = this.CreateGraphics();
			this.brush = Brushes.Blue;

			this.InitializeComponent();
			this.CreateFonts();
			this.InitializeOpenFileDialog();
		}

		/// <summary>
		/// Vytvoøí fonty
		/// </summary>
		private void CreateFonts() {
			this.inputFont = new Font("Arial", 10, FontStyle.Bold);
			this.outputFont = new Font("Arial", 10, FontStyle.Regular);
			TextLineInformation.LineHeight = (int)(1.2*System.Math.Max(this.inputFont.Height, this.outputFont.Height));
		}

		/// <summary>
		/// Vytvoøí a nastaví dialog otevøení souboru
		/// </summary>
		private void InitializeOpenFileDialog() {
			this.openFileDialog = new OpenFileDialog();
			this.openFileDialog.Filter = "Textové soubory (*.txt)|*.txt|Všechny soubory (*.*)|*.*";
			this.openFileDialog.FileOk += new CancelEventHandler(openFileDialog_FileOk);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.mnFile = new System.Windows.Forms.MenuItem();
			this.mnFileOpen = new System.Windows.Forms.MenuItem();
			this.mnFileExit = new System.Windows.Forms.MenuItem();
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.mnFile});
			// 
			// mnFile
			// 
			this.mnFile.Index = 0;
			this.mnFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.mnFileOpen,
																				   this.mnFileExit});
			this.mnFile.Text = "&Soubor";
			// 
			// mnFileOpen
			// 
			this.mnFileOpen.Index = 0;
			this.mnFileOpen.Text = "&Otevøít";
			this.mnFileOpen.Click += new System.EventHandler(this.mnFileOpen_Click);
			// 
			// mnFileExit
			// 
			this.mnFileExit.Index = 1;
			this.mnFileExit.Text = "&Konec";
			// 
			// Editor
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Menu = this.mainMenu;
			this.Name = "Editor";
			this.Text = "Editor";

		}
		#endregion

		/// <summary>
		/// Zadán soubor k otevøení
		/// </summary>
		private void openFileDialog_FileOk(object sender, CancelEventArgs e) {
			StreamReader sr = new StreamReader(openFileDialog.FileName);

			string nextLine;
			this.documentLines.Clear();

			TextLineInformation.DocumentHeight = 0;
			TextLineInformation.DocumentWidth = 0;

			TextLineInformation nextLineInfo;
			SizeF sizeF;

			while((nextLine = sr.ReadLine()) != null) {
				nextLineInfo = new TextLineInformation();
				nextLineInfo.Text = nextLine;

				sizeF = graphics.MeasureString(nextLine, inputFont);
				nextLineInfo.Width = (int)sizeF.Width;

				int lineLength = nextLineInfo.Width + 2 * TextLineInformation.Margin;
				if(lineLength > TextLineInformation.DocumentWidth)
					TextLineInformation.DocumentWidth = lineLength;

				this.documentLines.Add(nextLineInfo);
			}

			sr.Close();

			TextLineInformation.DocumentHeight = this.documentLines.Count * TextLineInformation.LineHeight + 2 * TextLineInformation.Margin;
			this.AutoScrollMinSize = new Size(TextLineInformation.DocumentWidth, TextLineInformation.DocumentHeight);
			this.Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint (e);

			Graphics graphics = e.Graphics;
			int scrollPositionX = this.AutoScrollPosition.X;
			int scrollPositionY = this.AutoScrollPosition.Y;
			graphics.TranslateTransform(scrollPositionX, scrollPositionY);

			if(this.documentLines.Count != 0) {
				int minLineInClipRegion = this.WorldYCoordinateToLineIndex(e.ClipRectangle.Top - scrollPositionY);
				if(minLineInClipRegion < 0)
					minLineInClipRegion = 0;
				int maxLineInClipRegion = this.WorldYCoordinateToLineIndex(e.ClipRectangle.Bottom - scrollPositionY);
				if(maxLineInClipRegion >= this.documentLines.Count || maxLineInClipRegion < 0)
					maxLineInClipRegion = this.documentLines.Count - 1;

				for(int i = minLineInClipRegion; i <= maxLineInClipRegion; i++) {
					TextLineInformation line = (TextLineInformation)this.documentLines[i];
					graphics.DrawString(line.Text, inputFont, brush, this.LineIndexToWorldCoordinates(i));
				}
			}
		}

		private Point LineIndexToWorldCoordinates(int index) {
			return new Point(TextLineInformation.Margin, index * TextLineInformation.LineHeight + TextLineInformation.Margin);
		}

		private int WorldYCoordinateToLineIndex(int y) {
			if(y < TextLineInformation.Margin)
				return -1;
			else
				return (y - TextLineInformation.Margin) / TextLineInformation.LineHeight;
		}

		/// <summary>
		/// Otevøít soubor
		/// </summary>
		private void mnFileOpen_Click(object sender, System.EventArgs e) {
			this.openFileDialog.ShowDialog();
		}
	}

	/// <summary>
	/// Informace o každém øádku
	/// </summary>
	class TextLineInformation {
		public enum LineTypes {Input, Output}

		public string Text = string.Empty;
		public int Width = 0;
		public LineTypes LineType = LineTypes.Input;

		public static int DocumentHeight;
		public static int DocumentWidth;
		public static int LineHeight;

		public const int Margin = 10;
	}
}
