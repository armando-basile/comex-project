
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
			
			// init Gtk Application
			Application.Init();
            
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
			
			
			
			
			
			// create new MainWindow and show it
			MainWindowClass mwc = new MainWindowClass();
			mwc.Show();
			
			

			
/*			
			MessageDialog mdl = new MessageDialog(null, 
			                                      DialogFlags.Modal, 
			                                      MessageType.Info, 
			                                      ButtonsType.Ok,
			                                      true,
			                                      "Prova <b>messaggio</b> di testo");
			mdl.Show();
			mdl.Title = "DEMO";
            mdl.Icon = Gdk.Pixbuf.LoadFromResource("comex.Resources.Images.comex_256.png");
            mdl.Run();
			mdl.Destroy();                  
*/
			
			
			
			
			
			Application.Run ();

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
			msg += "   --log-console     enable log into console\r\n";
			msg += "   --log-file        enable log into file comex.log into home folder\r\n";
			msg += "   --help            show this message\r\n";
			
			return msg;
		}
		
		
		
		
		
		#endregion Private Methods
		
		
		

	}
}

