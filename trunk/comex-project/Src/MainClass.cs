
using System;
using System.Collections.Generic;

using log4net;
using log4net.Config;

using Gtk;
using Gdk;



namespace comex
{
	
	/// <summary>
	/// Main class of comex application
	/// </summary>
	public class MainClass
	{
		
		
		
		// Log4Net object
        // private static readonly ILog log = LogManager.GetLogger(typeof(comex.MainClass));
		
		
		private static string retStr = "";
		
		
		
		
		
		#region Public Methods
		
		
		
		
		
		[STAThread]
        public static void Main(string[] args)
        {
			
			// check for help request
			if (new List<string>(args).Contains("--help"))
			{
				Console.WriteLine(GetHelpMsg());
				return;
			}
			
			retStr = GlobalObj.Initialize(args);
			
			if (retStr != "")
			{
				// error detected
				Console.WriteLine("ERROR: " + retStr);
				return;
			}

            
			// console application required
			ConsoleManager.StartApp();


		}
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		#endregion Public Methods
		
		
		
		
		
		
		
		
		#region Private Methods
		
		
	
		
		
		
		
		
		
		/// <summary>
		/// Get help message to send to console
		/// </summary>
		private static string GetHelpMsg()
		{
			string msg = GlobalObj.AppNameVer + " - card commands exchanger for PC/SC and serial readers\r\n\r\n";
			msg += "   usage:\r\n";
			msg += "   --log-console     enable log into console\r\n";
			msg += "   --log-file        enable log into file comex.log into home folder\r\n";
			msg += "   --help            show this message\r\n";
			
			return msg;
		}
		
		
		
		
		/// <summary>
		/// Show Message Window
		/// </summary>
		private static void ShowMessage(string title, string message)
		{
			// Console Message
			Console.WriteLine(title + " - " + message);
		}
		
		
		
		
		
		#endregion Private Methods
		
		
		

	}
}

