using System;
using System.Collections.Generic;
using System.IO;
using Glade;
using Gtk;
using Gdk;
using Pango;

using log4net;

using comex;

namespace comexgtk
{
	
	public class MainWindowClass
	{
		
		
		[Glade.Widget]  Gtk.Window             MainWindow;
		[Glade.Widget]  Gtk.ToolButton         TbOpen;
		[Glade.Widget]  Gtk.ToolButton         TbClose;
		[Glade.Widget]  Gtk.ToolButton         TbATR;
		[Glade.Widget]  Gtk.ToolButton         TbAbout;
		[Glade.Widget]  Gtk.ToolButton         TbExit;		
		[Glade.Widget]  Gtk.Statusbar          StatusBar;
		[Glade.Widget]  Gtk.MenuItem       	   MenuFileItem;
		[Glade.Widget]  Gtk.ImageMenuItem  	   MenuFileOpen;
		[Glade.Widget]  Gtk.ImageMenuItem  	   MenuFileClose;
		[Glade.Widget]  Gtk.ImageMenuItem  	   MenuFileExit;
		[Glade.Widget]  Gtk.Menu       	       MenuReader;
		[Glade.Widget]  Gtk.MenuItem       	   MenuReaderItem;
		[Glade.Widget]  Gtk.MenuItem       	   MenuAboutItem;
		[Glade.Widget]  Gtk.ImageMenuItem  	   MenuAboutInfo;
		[Glade.Widget]  Gtk.Label              LblATR;
		[Glade.Widget]  Gtk.Label              LblFile;
		[Glade.Widget]  Gtk.Entry              TxtATR;
		[Glade.Widget]  Gtk.Label              LblCommand;
		[Glade.Widget]  Gtk.Label              LblResponse;
		[Glade.Widget]  Gtk.Label              LblSend;
		[Glade.Widget]  Gtk.Label              LblExchange;
		[Glade.Widget]  Gtk.Entry              TxtCmd;
		[Glade.Widget]  Gtk.Entry              TxtResp;
		[Glade.Widget]  Gtk.Button             BtnSend;
		[Glade.Widget]  Gtk.TreeView           LstCommands;
		
		
		
		// Log4Net object
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindowClass));
		
		
		// Attributes
		private string retStr = "";
		private string ATR = "";
		private string command = "";
		private string response = "";
		private string commandFileName = "";
		private string commandFilePath = "";
		
		private ListStore lsCommands;
		
		
		
		
		
		#region Public Methods
		
		
		
		
		/// <summary>
        /// Constructor
        /// </summary>
        public MainWindowClass()
        {                
            // Instance glade xml object using glade file
            Glade.XML gxml =  new Glade.XML("MainWindow.glade", "MainWindow");
            
            // Aonnect glade xml object to this Gtk.Windows
            gxml.Autoconnect(this);
            
            // Update Gtk graphic objects
            UpdateGraphicObjects();
            
            // Update Event Handlers
            UpdateReactors();                       
                       
        }
		
		
		
		
		
		/// <summary>
		/// Show MainWindow
		/// </summary>
		public void Show()
		{
			MainWindow.Show();
			MainWindow.Title = MainClass.AppNameVer + " [" + GlobalObj.AppNameVer + "]";
		}
		
		
		
		
		
		
		
		
		
		#endregion Public Methods
		
		
		
		
		
		
		
		
		
		
		

		
		#region Reactors
		
		
		/// <summary>
		/// Close GtkWindows
		/// </summary>
		[GLib.ConnectBefore]
		public void ActionCancel(object sender, EventArgs args)
		{
			GlobalObj.CloseConnection();
			
			MainWindow.Destroy();
            MainWindow.Dispose();
			Application.Quit();
		}
		
		
		
		/// <summary>
		/// Open Info about window
		/// </summary>
		[GLib.ConnectBefore]
		public void ActionAbout(object sender, EventArgs args)
		{
			AboutDialogClass adc = new AboutDialogClass();
			adc.SetParent(ref MainWindow);			
			adc.About = GlobalObj.LMan.GetString("infodesc").Replace("\t", "");
			adc.Thanks = GlobalObj.LMan.GetString("thanksdesc").Replace("\t", "") + GlobalConst.ThanksTo;
			adc.Description = "<b>COMEX Project</b> - " + GlobalObj.LMan.GetString("description") ;
			adc.Title = GlobalObj.LMan.GetString("frmabout");
			adc.Show();
		}
		
		
		
		
		
		
		/// <summary>
		/// Close command file
		/// </summary>
		[GLib.ConnectBefore]
		public void ActionCloseFile(object sender, EventArgs args)
		{
			CloseCommandFile();
		}
		

		
		
		
		/// <summary>
		/// Open command file
		/// </summary>
		[GLib.ConnectBefore]
		public void ActionOpenFile(object sender, EventArgs args)
		{
			OpenCommandFile();
		}

		
		
		
		
		/// <summary>
		/// ChangeReader
		/// </summary>
		[GLib.ConnectBefore]
		public void ActionChangeReader(object sender, ButtonReleaseEventArgs args)
		{
			string newReader = ((AccelLabel)(((RadioMenuItem)(sender)).Children[0])).Text;
			log.Info("Changing reader to " + newReader);
			UpdateSelectedReader(newReader);
		}
		
		
		
		
		/// <summary>
		/// Perform Power On card
		/// </summary>
		[GLib.ConnectBefore]
		public void ActionATR(object sender, EventArgs args)
		{
			GetATR();
		}
		
		
		
		
		/// <summary>
		/// Perform exchange data with card
		/// </summary>
		[GLib.ConnectBefore]
		public void ActionSendCommand(object sender, EventArgs args)
		{
			// Send command and receive response
			ExchangeData();
		}
		
		
		
		/// <summary>
		/// Perform update of command text from selected command file row
		/// </summary>
		[GLib.ConnectBefore]
		private void ActionAddCommand(object sender, ButtonPressEventArgs e)
		{
			if (e.Event.Type == Gdk.EventType.TwoButtonPress)
		    {
				// Update text of command
				GetCommandFromList();
		    }	
		}
		
		
		
		
		/// <summary>
		/// Perform exec of command from selected command file row
		/// </summary>
		[GLib.ConnectBefore]
		private void ActionExecCommand(object sender, KeyPressEventArgs e)
		{
			if (e.Event.Key == Gdk.Key.F5)
			{
				// Update text of command
				GetCommandFromList();
			}
			else if (e.Event.Key == Gdk.Key.F6)
			{
				// Update text of command,  send it and receive response
				GetCommandFromList();
				ExchangeData();
			}
		}
		
		
		
		
		#endregion Reactors

		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		#region Private Methods
		
		
		
		
		

		/// <summary>
		/// Update Gtk objects properties
		/// </summary>
		private void UpdateGraphicObjects()
		{
			// Set dialog icon
            MainWindow.Icon = Gdk.Pixbuf.LoadFromResource("comex_256.png");
			
			// Set tool tip text for toolbutton
			TbOpen.TooltipText = GlobalObj.LMan.GetString("openact");
			TbOpen.Label = GlobalObj.LMan.GetString("openlbl");
			
			TbClose.Label = GlobalObj.LMan.GetString("closelbl");
			
			TbAbout.TooltipText = GlobalObj.LMan.GetString("infoact");
			TbAbout.Label = GlobalObj.LMan.GetString("infolbl");
			
			TbExit.TooltipText = GlobalObj.LMan.GetString("exitact");
			TbExit.Label = GlobalObj.LMan.GetString("exitlbl");

			TbATR.TooltipText = GlobalObj.LMan.GetString("atract");
			TbATR.Label = GlobalObj.LMan.GetString("atrlbl");
			
			// Set labels
			((Label)MenuFileItem.Child).TextWithMnemonic = GlobalObj.LMan.GetString("filemenulbl");
			((Label)MenuFileOpen.Child).TextWithMnemonic = GlobalObj.LMan.GetString("openmenulbl");
			((Label)MenuFileClose.Child).TextWithMnemonic = GlobalObj.LMan.GetString("closemenulbl");
			((Label)MenuFileExit.Child).TextWithMnemonic = GlobalObj.LMan.GetString("exitmenulbl");
			((Label)MenuReaderItem.Child).TextWithMnemonic = GlobalObj.LMan.GetString("readermenulbl");
			((Label)MenuAboutItem.Child).TextWithMnemonic = GlobalObj.LMan.GetString("helpmenulbl");
			((Label)MenuAboutInfo.Child).TextWithMnemonic = GlobalObj.LMan.GetString("infomenulbl");
			
			LblATR.Markup = "<b>" + GlobalObj.LMan.GetString("atrframelbl") + "</b>"; 
			LblCommand.Text = GlobalObj.LMan.GetString("cmdlbl");
			LblResponse.Text = GlobalObj.LMan.GetString("resplbl");
			LblSend.Text = GlobalObj.LMan.GetString("sendlbl");
			LblExchange.Markup = "<b>" + GlobalObj.LMan.GetString("cardframelbl") + "</b>";
			LblFile.Markup =  "<b>" + GlobalObj.LMan.GetString("commandfilelbl") + "</b>";
			
			Gdk.Color color = new Gdk.Color();
			Gdk.Color.Parse("#0000FF", ref color);
			TxtATR.ModifyText(StateType.Normal, color);
			Gdk.Color.Parse("#0000FF", ref color);
			TxtResp.ModifyText(StateType.Normal, color);
			
			Gdk.Color.Parse("#1F6D20", ref color);
			TxtCmd.ModifyText(StateType.Normal, color);
			
			// TreeView (List) 
			LstCommands.Selection.Mode = SelectionMode.Single;
			CellRendererText rendererText = new CellRendererText();
        	
			TreeViewColumn column = new TreeViewColumn();
			column.Title = "Commands";
			column.Resizable = true;
			column.PackStart(rendererText, true);
			column.AddAttribute(rendererText, "text", 0);
			
			LstCommands.RulesHint = true;
        	LstCommands.AppendColumn(column);
			
			if (GlobalObj.IsWindows())
			{
				LstCommands.ModifyFont(FontDescription.FromString("Courier New Normal 10"));
			}
			else
			{
				LstCommands.ModifyFont(FontDescription.FromString("Fixed Normal 10"));
			}
			
			
			// update gui menu
			Gtk.RadioMenuItem rmi;
			// loop for all pcsc readers
			for (int n=0; n<GlobalObj.PCSC_Readers.Count; n++)
			{	
				// set first as selected
				if (n==0)
				{
					rmi = new Gtk.RadioMenuItem(GlobalObj.PCSC_Readers[n]);
					GlobalObj.SelectedReader = GlobalObj.PCSC_Readers[n];
					StatusBar.Push(1, GlobalObj.LMan.GetString("selreader") + ": " + GlobalObj.PCSC_Readers[n]);
				}
				else
				{
					// added others
					rmi = new Gtk.RadioMenuItem((RadioMenuItem)MenuReader.Children[0], GlobalObj.PCSC_Readers[n]);
				}
				
				rmi.ButtonReleaseEvent += ActionChangeReader;
				MenuReader.Add(rmi);
			}
			
			// loop for all serial port available
			for (int n=0; n<GlobalObj.SerialPortsName.Count; n++)
			{
				// detect if there are pcsc reader
				if (MenuReader.Children.Length > 0)
				{
					// added all serial port after pcsc readers
					rmi = new Gtk.RadioMenuItem((RadioMenuItem)MenuReader.Children[0], GlobalObj.SerialPortsName[n]);
				}
				else
				{
					// if there aren't pcsc reader, add and select first serial port reader
					if (n==0)
					{
						rmi = new Gtk.RadioMenuItem(GlobalObj.SerialPortsName[n]);
						GlobalObj.SelectedReader = GlobalObj.SerialPortsName[n];
						StatusBar.Push(1, GlobalObj.LMan.GetString("selreader") + ": " + GlobalObj.SerialPortsName[n]);
					}
					else
					{
						// added other serial port reader
						rmi = new Gtk.RadioMenuItem((RadioMenuItem)MenuReader.Children[0], GlobalObj.SerialPortsName[n]);
					}					
				}
				
				rmi.ButtonReleaseEvent += ActionChangeReader;
				MenuReader.Add(rmi);
			}
			
			MenuReader.ShowAll();

			if (GlobalObj.PCSC_Readers.Count == 0)
			{
				StatusBar.Push(1, GlobalObj.LMan.GetString("nopcscreader"));
			}
			
		}
		
		
		
		
		
		
		/// <summary>
		/// Update Gtk objects reactors
		/// </summary>
		private void UpdateReactors()
		{
			MainWindow.DeleteEvent += ActionCancel;
			BtnSend.Clicked += ActionSendCommand;
			LstCommands.ButtonPressEvent += ActionAddCommand;
			LstCommands.KeyPressEvent += ActionExecCommand;
		}
		
		
		
		
		
		
		
		
		/// <summary>
		/// Update selected reader
		/// </summary>
		private void UpdateSelectedReader(string reader)
		{
			GlobalObj.SelectedReader = reader;
			StatusBar.Push(1, GlobalObj.LMan.GetString("selreader") + ": " + reader);
			TxtATR.Text = "";
			TxtResp.Text = "";
			TxtCmd.Text = "";
		}
		
		
		
		
		
		
		/// <summary>
		/// Perform Power On
		/// </summary>
		private void GetATR()
		{
			TxtATR.Text = "";
			TxtResp.Text = "";
			TxtCmd.Text = "";
			MainClass.GtkWait();
			
			// Connect to smartcard
			retStr = GlobalObj.AnswerToReset(ref ATR);
			
			if (retStr != "")
			{
				// error on answer to reset
				log.Error(retStr);
				MainClass.ShowMessage("ERROR", retStr, MessageType.Error);
				return;
			}
			
			TxtATR.Text = ATR;
		}
		
		
		
		
		
		
		/// <summary>
		/// Perform exchange data with card
		/// </summary>
		private void ExchangeData()
		{
			command = TxtCmd.Text;
			TxtResp.Text = "";
			TxtResp.Show();
			MainClass.GtkWait();
			
			retStr = GlobalObj.SendReceive(command, ref response);
			
			if (retStr != "")
			{
				log.Error(retStr);
				MainClass.ShowMessage("ERROR", retStr, MessageType.Error);
				return;
			}
			
			TxtResp.Text = response;
		}
		
		
		
		
		
		
		/// <summary>
		/// Open command file
		/// </summary>
		private void OpenCommandFile()
		{
                        
            // New dialog for select contacts file 
            Gtk.FileChooserDialog FileBox = 
				new Gtk.FileChooserDialog(GlobalObj.LMan.GetString("selectfile"), 
                                          MainWindow,
                                          FileChooserAction.Open,
				                          GlobalObj.LMan.GetString("cancellbl"),
                                          Gtk.ResponseType.Cancel,
				                          GlobalObj.LMan.GetString("openlbl"),
                                          Gtk.ResponseType.Accept);
            
            // Filter to use only comex files
            Gtk.FileFilter myFilter = new Gtk.FileFilter(); 
            myFilter.AddPattern("*.comex");
            myFilter.Name = "comex project files (*.comex)";
            FileBox.AddFilter(myFilter);
            
            // Manage result of dialog box
            FileBox.Icon = Gdk.Pixbuf.LoadFromResource("comex_256.png");
            int retFileBox = FileBox.Run();
            if ((ResponseType)retFileBox == Gtk.ResponseType.Accept)
            {       
                // path of a right file returned
                commandFilePath = FileBox.Filename;				
				commandFileName = System.IO.Path.GetFileNameWithoutExtension(commandFilePath);
				log.Debug("file selected: " + commandFilePath);
				
				UpdateGuiForCommandFile();
			
				
				
                FileBox.Destroy();
                FileBox.Dispose();                              
            }
            else
            {
                // nothing returned
                FileBox.Destroy();
                FileBox.Dispose();
                return;
            }
        
		}
		

		
		
		
		/// <summary>
		/// Close command file
		/// </summary>
		private void CloseCommandFile()
		{
			commandFileName = "";
			commandFilePath = "";
			
			UpdateGuiForCommandFile();
		}

		
		
		
		
		
		private void UpdateGuiForCommandFile()
		{
			if (commandFileName != "")
			{
				
				if (!UpdateFileAreaFromFile(commandFilePath))
				{
					// error detected
					lsCommands.Clear();
					return;
				}
				
				// Command file selected
				LblFile.Markup = "<b>" + GlobalObj.LMan.GetString("commandfilelbl") + " " + 
					"[" + commandFileName + "]</b>";
				
				TbOpen.Sensitive = false;
				TbClose.Sensitive = true;
				MenuFileOpen.Sensitive = false;
				MenuFileClose.Sensitive = true;
				
			}
			else
			{
				// No commanf dile selected
				LblFile.Markup = "<b>" + GlobalObj.LMan.GetString("commandfilelbl") + "</b>";

				TbOpen.Sensitive = true;
				TbClose.Sensitive = false;
				MenuFileOpen.Sensitive = true;
				MenuFileClose.Sensitive = false;
				
				lsCommands = new ListStore(typeof(string));
				LstCommands.Model=lsCommands;
			}
			
		}
		
		
		
		
		
		
		/// <summary>
		/// Fill command file area from command file
		/// </summary>
		private bool UpdateFileAreaFromFile(string filepath)
		{
			StreamReader sr = null;
			List<string> cmdList = new List<string>();
			
			try
			{
				sr = new StreamReader(filepath);
				
				string line = "";
				while (!sr.EndOfStream)
				{
					line = sr.ReadLine();
					cmdList.Add(line);
				}
				
				sr.Close();
				sr.Dispose();
				sr = null;
				
				UpdateTreeView(cmdList);
				
				return true;
			}
			catch(Exception Ex)
			{
				log.Error(Ex.Message);
				MainClass.ShowMessage("ERROR", Ex.Message, MessageType.Error);
				return false;
			}
			
		}
		
		
		
		
		
		
		private void UpdateTreeView(List<string> rows)
		{
			lsCommands = new ListStore(typeof(string));
			
			foreach(string row in rows)
			{
				
				lsCommands.AppendValues(row);
			}
			
  			LstCommands.Model = lsCommands;
			LstCommands.ShowAll();
		}
		
		
		
		
		
		/// <summary>
		/// update command from selected row in command file area
		/// </summary>
		private void GetCommandFromList()
		{
			// clear text
			TxtCmd.Text = "";
			MainClass.GtkWait();
			
			// obtain row content
			TreeSelection ts = LstCommands.Selection;
			TreeIter ti;
			bool issel = ts.GetSelected(out ti);
			
			// if no row selected
			if (!issel)
			{
				return;
			}
			
			string command = (string)lsCommands.GetValue(ti, 0);
			
			// check for empty row
			if (command.Trim() == "")
			{
				return;
			}
			
			
			// check for comments
			if (command.Trim().IndexOf("#") == 0)
			{
				return;
			}

			
			TxtCmd.Text = command.Substring(31);
		}
		
		
		
		#endregion Private Methods
		
		
		
		
		
		
		
		
		
	}
}

