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
		
		
		
		private string retStr = "";
		
		
		
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
			GlobalObj.PCSC.ReleaseContext();
			
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
			
			// retrieve pcsc readers
			string[] pcsc_readers = new string[0];			
			retStr = GlobalObj.PCSC.ListReaders(out pcsc_readers);			
			GlobalObj.PCSC_Readers = pcsc_readers;

			// update gui menu
			Gtk.RadioMenuItem rmi;
			for (int n=0; n<pcsc_readers.Length; n++)
			{				
				if (n==0)
				{
					rmi = new Gtk.RadioMenuItem(pcsc_readers[n]);
					GlobalObj.SelectedReader = pcsc_readers[n];
					StatusBar.Push(1, GlobalObj.LMan.GetString("selreader") + ": " + pcsc_readers[n]);
				}
				else
				{
					rmi = new Gtk.RadioMenuItem((RadioMenuItem)MenuReader.Children[0],pcsc_readers[n]);
				}

				MenuReader.Add(rmi);
			}
			MenuReader.ShowAll();

			if (pcsc_readers.Length == 0)
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

