using System;
using Glade;
using Gtk;
using Gdk;

using log4net;

namespace comex
{
	
	public class MainWindowClass
	{
		
		
		[Glade.Widget]  Gtk.Window             MainWindow;
		[Glade.Widget]  Gtk.ToolButton         TbOpen;
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
		
		
		
		
		// Log4Net object
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindowClass));
		
		
				
		
		
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
			MainWindow.Title = GlobalObj.AppNameVer;
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
		
		
		
		
		
		#endregion Reactors

		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		#region Private Methods
		
		
		
		
		

		/// <summary>
		/// Update Gtk objects properties
		/// </summary>
		private void UpdateGraphicObjects()
		{
			// Set dialog icon
            MainWindow.Icon = Gdk.Pixbuf.LoadFromResource("comex.Resources.Images.comex_256.png");
			
			// Set tool tip text for toolbutton
			TbOpen.TooltipText = GlobalObj.LMan.GetString("openact");
			TbOpen.Label = GlobalObj.LMan.GetString("openlbl");
			
			TbAbout.TooltipText = GlobalObj.LMan.GetString("infoact");
			TbAbout.Label = GlobalObj.LMan.GetString("infolbl");
			
			TbExit.TooltipText = GlobalObj.LMan.GetString("exitact");
			TbExit.Label = GlobalObj.LMan.GetString("exitlbl");

			// Set labels
			((Label)MenuFileItem.Child).TextWithMnemonic = GlobalObj.LMan.GetString("filemenulbl");
			((Label)MenuFileOpen.Child).TextWithMnemonic = GlobalObj.LMan.GetString("openmenulbl");
			((Label)MenuFileExit.Child).TextWithMnemonic = GlobalObj.LMan.GetString("exitmenulbl");
			((Label)MenuReaderItem.Child).TextWithMnemonic = GlobalObj.LMan.GetString("readermenulbl");
			((Label)MenuAboutItem.Child).TextWithMnemonic = GlobalObj.LMan.GetString("helpmenulbl");
			((Label)MenuAboutInfo.Child).TextWithMnemonic = GlobalObj.LMan.GetString("infomenulbl");
			


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
		}
		
		
		
		
		
		#endregion Private Methods
		
		
		
		
		
		
		
		
		
	}
}

