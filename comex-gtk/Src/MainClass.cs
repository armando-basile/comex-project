using System;
using System.Reflection;
using System.Collections.Generic;

using log4net;
using log4net.Config;

using Gtk;
using Gdk;

using comexbase;



namespace comexgtk
{
	
	public class MainClass
	{
		
		
		// Log4Net object
        //private static readonly ILog log = LogManager.GetLogger(typeof(comexgtk.MainClass));
		
		
		private static string retStr = "";
		
		
		public static string AppNameVer = "";
		
		
		
		
		#region Public Methods
		
		
		
		[STAThread]
		public static void Main (string[] args)
		{
			AppNameVer = Assembly.GetExecutingAssembly().GetName().Name + " " +
					     Assembly.GetExecutingAssembly().GetName().Version.ToString();
			
			// check for help request
			if (new List<string>(args).Contains("--help"))
			{
				Console.WriteLine(GetHelpMsg());
				return;
			}
			
			Application.Init();
			
			retStr = GlobalObj.Initialize(args);
			
			// check for problems detected
			if (retStr != "")
			{
				// check for problem type
				if (!retStr.Contains("SCARD_"))
				{
					// error detected (not scard problem)
					ShowMessage("ERROR", retStr, MessageType.Error);
					return;
				}
				else
				{
					// warning (scard problem, can use serial reader)
					ShowMessage("WARNING", retStr, MessageType.Warning);
				}
				
			}

			// create new Gtk Gui for application and show it
			MainWindowClass mwc = new MainWindowClass();
			mwc.Show();
			Application.Run ();
		}
		
		
		
		
		
		/// <summary>
		/// Show Message Window
		/// </summary>
		public static void ShowMessage(string title, string message, MessageType mt)
		{
			
			// Gui Message
			MessageDialog mdl = new MessageDialog(null, 
			                                      DialogFlags.DestroyWithParent, 
			                                      mt, 
			                                      ButtonsType.Ok,
			                                      true,
			                                      message);
			mdl.Show();
			mdl.Title = title;
	        mdl.Icon = Gdk.Pixbuf.LoadFromResource("comex_256.png");
	        mdl.Run();
			mdl.Destroy();                  
	
		}
		
		
		
		
		
		/// <summary>
		/// Wait for pending GTK processes
		/// </summary>
		public static void GtkWait()
		{
			// Update GUI...
			while (Gtk.Application.EventsPending ())
			{
            	Gtk.Application.RunIteration ();
			}
		}
		
		
		
		
		
		
		#endregion Public Methods
		
		
		
		
		
		
		#region Private Methods
		
		
		
		/// <summary>
		/// Get help message to send to console
		/// </summary>
		private static string GetHelpMsg()
		{
			string msg = AppNameVer + " - GTK ui for comex core component\r\n" + 
				         GlobalObj.AppNameVer + " - base component\r\n\r\n";
			msg += "   usage:\r\n";
			msg += "   --log-console     enable log into console\r\n";
			msg += "   --log-file        enable log into file comex.log into home folder\r\n";
			msg += "   --help            show this message\r\n";
			
			return msg;
		}
		
		
		
		
		
		
		
		#endregion Private Methods
		
		
		
	}
	
	
	
	
}

