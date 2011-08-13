
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
        private static readonly ILog log = LogManager.GetLogger(typeof(MainClass));
		
		
		private static string retStr = "";
		private static bool isConsoleAppReq = false;
		
		
		[STAThread]
        public static void Main(string[] args)
        {
			// set application folder path
			string dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;                        
			GlobalObj.AppPath = new System.IO.FileInfo(dllPath).DirectoryName;
			
			// set command line application arguments
			GlobalObj.AppArgs = new List<string>(args);
			
			// check for help request
			if (GlobalObj.AppArgs.Contains("--help"))
			{
				Console.WriteLine(GetHelpMsg());
				return;
			}
						
			// parge command line arguments
			ParseCommandArgs();
			
			// config Log4Net
			ConfigLog4Net();
            
			log.Info("Application Started");
			
			if (GlobalObj.LogToFile)
			{
				log.Info("Enabled also log in file: " + GlobalObj.LogFilePath);
			}
			
			// set language file
			try
			{
				log.Info("System      Language Tag: " + System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag);
				GlobalObj.SetLanguage();
				log.Info("Application Language Tag: " + GlobalObj.LanguageTag);
			}
			catch(Exception Ex)
			{
				// error detected
				log.Error(Ex.Message);
				return;
			}
			
			// init Gtk Application
			Application.Init();
			
			
			// create new PCSC manager and create context
			retStr = GlobalObj.InitPCSC();
			
			if (retStr != "")
			{
				log.Error(retStr);
				ShowMessage(MessageType.Error, "Error", retStr);
				return;
			}
			
			
			
			if (isConsoleAppReq)
			{
				// console application required
				ConsoleManager.StartApp();
				
			}
			else
			{			
				// create new Gtk Gui for application and show it
				MainWindowClass mwc = new MainWindowClass();
				mwc.Show();
				Application.Run ();
			}
			


		}
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		#region Private Methods
		
		
		
		/// <summary>
		/// Parse command line arguments
		/// </summary>
		private static void ParseCommandArgs()
		{
			// check for console log request
			if (GlobalObj.AppArgs.Contains("--log-console"))
			{
				GlobalObj.LogToConsole = true;
			}

			// check for file log request
			if (GlobalObj.AppArgs.Contains("--log-file"))
			{
				GlobalObj.LogToFile = true;
			}
			
			// check for console application required
			if (GlobalObj.AppArgs.Contains("--app-console"))
			{
				isConsoleAppReq = true;
			}			
			
			
		}
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Configure log4net object
		/// </summary>
		private static void ConfigLog4Net()
		{
			// set log filename
			string logfilename = Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			logfilename += System.IO.Path.DirectorySeparatorChar;
			logfilename += System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".log";

			GlobalObj.LogFilePath = logfilename;
			
			// attach to repository hierarchy
			log4net.Repository.Hierarchy.Hierarchy repository = LogManager.GetRepository() as log4net.Repository.Hierarchy.Hierarchy; 
			
			// check for logging into file
			if (GlobalObj.LogToFile)
			{
				// add log file appender
				log4net.Appender.RollingFileAppender fileappender = new log4net.Appender.RollingFileAppender();
		        fileappender.Layout = new log4net.Layout.PatternLayout("%-5level %date{yyyy-MM-dd HH:mm:ss,fff} [%thread] %message (%file:%line)%newline");
		        fileappender.File = logfilename;
		        fileappender.MaxSizeRollBackups = 10;
		        fileappender.MaximumFileSize = "10MB";
		        fileappender.AppendToFile = true;		        
		        fileappender.ActivateOptions();
				repository.Root.AddAppender(fileappender);
			}
	        
			// check for logging into console
			if (GlobalObj.LogToConsole)
			{
				// add console appender
				log4net.Appender.ConsoleAppender consoleappender = new log4net.Appender.ConsoleAppender();
		        consoleappender.Layout = new log4net.Layout.PatternLayout("%-5level %date{HH:mm:ss} %message%newline");		        
		        consoleappender.ActivateOptions();				
				repository.Root.AddAppender(consoleappender);
			}
			
			// set to log all events
			repository.Root.Level = log4net.Core.Level.All;
			repository.Configured = true;  
			repository.RaiseConfigurationChanged(EventArgs.Empty);
			
		}
		
		
		
		
		
		
		
		/// <summary>
		/// Get help message to send to console
		/// </summary>
		private static string GetHelpMsg()
		{
			string msg = GlobalObj.AppNameVer + " - card commands exchanger for PC/SC and serial readers\r\n\r\n";
			msg += "   usage:\r\n";
			msg += "   --app-console     disable gtk gui and use console interface\r\n";
			msg += "   --log-console     enable log into console\r\n";
			msg += "   --log-file        enable log into file comex.log into home folder\r\n";
			msg += "   --help            show this message\r\n";
			
			return msg;
		}
		
		
		
		
		/// <summary>
		/// Show Message Window
		/// </summary>
		private static void ShowMessage(MessageType mt, string title, string message)
		{
			
			if (isConsoleAppReq)
			{
				// Console Message
				Console.WriteLine(title + " - " + message);
			}
			else
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
	            mdl.Icon = Gdk.Pixbuf.LoadFromResource("comex.Resources.Images.comex_256.png");
	            mdl.Run();
				mdl.Destroy();                  
			}

		}
		
		
		
		
		
		#endregion Private Methods
		
		
		

	}
}

