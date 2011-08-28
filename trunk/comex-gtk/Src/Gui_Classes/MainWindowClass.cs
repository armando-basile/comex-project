using System;
using Glade;
using Gtk;
using Gdk;

using log4net;

using comex;

namespace comexgtk
{
	
	public class MainWindowClass
	{
		
		
		[Glade.Widget]  Gtk.Window             MainWindow;
		[Glade.Widget]  Gtk.ToolButton         TbOpen;
		[Glade.Widget]  Gtk.ToolButton         TbATR;
		[Glade.Widget]  Gtk.ToolButton         TbAbout;
		[Glade.Widget]  Gtk.ToolButton         TbExit;		
		[Glade.Widget]  Gtk.Statusbar          StatusBar;
		[Glade.Widget]  Gtk.MenuItem       	   MenuFileItem;
		[Glade.Widget]  Gtk.ImageMenuItem  	   MenuFileOpen;
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
		
		
		
		
		// Log4Net object
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindowClass));
		
		
		// Attributes
		private string retStr = "";
		private string ATR = "";
		private string command = "";
		private string response = "";
		
		
		
		
		
		
		
		
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
		/// ChangeReader
		/// </summary>
		public void ActionChangeReader(object sender, ButtonReleaseEventArgs args)
		{
			string newReader = ((AccelLabel)(((RadioMenuItem)(sender)).Children[0])).Text;
			log.Info("Changing reader to " + newReader);
			UpdateSelectedReader(newReader);
		}
		
		
		
		
		/// <summary>
		/// Perform Power On card
		/// </summary>
		public void ActionATR(object sender, EventArgs args)
		{
			GetATR();
		}
		
		
		
		
		/// <summary>
		/// Perform exchange data with card
		/// </summary>
		public void ActionSendCommand(object sender, EventArgs args)
		{
			ExchangeData();
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
			
			TbAbout.TooltipText = GlobalObj.LMan.GetString("infoact");
			TbAbout.Label = GlobalObj.LMan.GetString("infolbl");
			
			TbExit.TooltipText = GlobalObj.LMan.GetString("exitact");
			TbExit.Label = GlobalObj.LMan.GetString("exitlbl");

			TbATR.TooltipText = GlobalObj.LMan.GetString("atract");
			TbATR.Label = GlobalObj.LMan.GetString("atrlbl");
			
			// Set labels
			((Label)MenuFileItem.Child).TextWithMnemonic = GlobalObj.LMan.GetString("filemenulbl");
			((Label)MenuFileOpen.Child).TextWithMnemonic = GlobalObj.LMan.GetString("openmenulbl");
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
			BtnSend.Activated += ActionSendCommand;
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
		
		
		
		
		
		
		#endregion Private Methods
		
		
		
		
		
		
		
		
		
	}
}

