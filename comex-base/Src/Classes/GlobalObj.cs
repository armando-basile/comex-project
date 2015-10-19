using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;


using log4net;
using log4net.Config;


namespace comexbase
{
	
	/// <summary>
	/// Static global class
	/// </summary>
	public static partial class GlobalObj
	{

		private static string languageFolder = "";
		private static string languageTag = "";
		private static string selectedReader = "";
		private static string selectedReaderType = "";
		private static bool isPowered = false;
		private static string ret = "";
		private static string settingsFilePath = "";
		private static LanguageManager lMan = null;
		private static SettingsManager sMan = null;
		
		// Log4Net object
        private static readonly ILog log = LogManager.GetLogger(typeof(GlobalObj));
		
		
		#region Properties
		
		
		/// <summary>
		/// Return language manager object
		/// </summary>
		public static LanguageManager LMan { get { return lMan; } }
		
		
		/// <summary>
		/// Readers manager dictionary
		/// </summary>
		public static Dictionary<string, IReader> ReaderManager = new Dictionary<string, IReader>();
		
		
		
		
		
		
		/// <summary>
		/// Return true if selected reader was powered on
		/// </summary>
		public static bool IsPowered { get { return isPowered; } }
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Return selected reader name
		/// </summary>
		public static string SelectedReader 
		{ 
			get	{	return selectedReader; }
			set
			{
				// check for actual selected reader
				if (selectedReader != "")
				{
					// close prev. connection
					ReaderManager[selectedReaderType].CloseConnection();
				}
				
				// set new selected reader
				selectedReader = value;
				selectedReaderType = "";
				
				// set selected reader unpowered
				isPowered = false;
				
				foreach(IReader rm in ReaderManager.Values)
				{
					if (rm.Readers.Contains(value))
					{
						// set new reader name and type to use
						selectedReaderType = rm.TypeName;
						rm.SelectedReader = value;
					}
				}				
			}
		}
		
		
		/// <summary>
		/// Return language tag to use
		/// </summary>
		public static string LanguageTag { get { return languageTag; }	}
		
		
		
		
		/// <summary>
		/// Application folder path
		/// </summary>
		public static string AppPath { get; set; }
		
		
		
		
		/// <summary>
		/// Application command line arguments
		/// </summary>
		public static List<string> AppArgs { get; set; }
		
		
		
		
		/// <summary>
		/// Get Application name and release
		/// </summary>
		public static string AppNameVer
		{
			get 
			{
				return Assembly.GetExecutingAssembly().GetName().Name + " " +
					Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
			}
		}
		
		
		/// <summary>
		/// Path of log file used
		/// </summary>
		public static string LogFilePath { get; set; }
		
		
		
		public static bool LogToConsole = false;
		public static bool LogToFile = false;
		
		
		
		
		#endregion Properties
		
		
		
		
		
		
		
		
		#region Public Methods
		
		
		
		

		
		/// <summary>
		/// Init all components
		/// </summary>
		public static string Initialize(string[] args)
		{
			// set command line application arguments
			AppArgs = new List<string>(args);
			
			// set application folder path
			string dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;                        
			AppPath = new System.IO.FileInfo(dllPath).DirectoryName;
			
			ParseCommandArgs();
			
			ConfigLog4Net();
			
			log.Info("Application Started");
			
			if (LogToFile)
			{
				// enabled log to file
				log.Info("Enabled also log in file: " + LogFilePath);
			}
			
			
			// set language file
			try
			{
				log.Info("System      Language Tag: " + System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag);
				SetLanguage();
				log.Info("Application Language Tag: " + LanguageTag);
			}
			catch(Exception Ex)
			{
				// error detected
				log.Error(Ex.Message);
				return Ex.Message;
			}
			
			
			InitReadersManagers();
			

			// init settings file
			settingsFilePath = Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			settingsFilePath += System.IO.Path.DirectorySeparatorChar;
			settingsFilePath += "." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".xml";
			sMan = new SettingsManager(settingsFilePath);
			ReadSettings();
			
			return "";
			
		}
		
		
		
		
		
		
		/// <summary>
		/// Detect if OS is an ms windows 
		/// </summary>
		public static bool IsWindows()
        {
            PlatformID platform = Environment.OSVersion.Platform;           
            return (platform == PlatformID.Win32NT | platform == PlatformID.Win32Windows |
                    platform == PlatformID.Win32S | platform == PlatformID.WinCE);    
        }

		
		
		
		

		/// <summary>
		/// Read serial port settings from config xml file or create it
		/// </summary>
		public static void ReadSettings()
		{
			SerialSettings.PortSpeed = sMan.ReadInt("SERIAL", "PortSpeed", 9600);
			SerialSettings.PortSpeedReset = sMan.ReadInt("SERIAL", "PortSpeedReset", 9600);
			SerialSettings.DataBits = sMan.ReadInt("SERIAL", "DataBits", 8);
			SerialSettings.StopBits = sMan.ReadInt("SERIAL", "StopBits", 1);
			SerialSettings.Parity = sMan.ReadString("SERIAL", "Parity", "Odd");
			SerialSettings.IsDirectConvention = sMan.ReadBool("SERIAL", "IsDirectConvention", true);
			SerialSettings.ReadTimeout = sMan.ReadInt("SERIAL", "ReadTimeout", 2000);
		}
		
		
		
		
		
		
		/// <summary>
		/// Read serial port settings from config xml file or create it
		/// </summary>
		public static void WriteSettings()
		{
			sMan.WriteInt("SERIAL", "PortSpeed", SerialSettings.PortSpeed);
			sMan.WriteInt("SERIAL", "PortSpeedReset", SerialSettings.PortSpeedReset);
			sMan.WriteInt("SERIAL", "DataBits", SerialSettings.DataBits);
			sMan.WriteInt("SERIAL", "StopBits", SerialSettings.StopBits);
			sMan.WriteString("SERIAL", "Parity", SerialSettings.Parity);			
			sMan.WriteBool("SERIAL", "IsDirectConvention", SerialSettings.IsDirectConvention);
			sMan.WriteInt("SERIAL", "ReadTimeout", SerialSettings.ReadTimeout);
			sMan.Flush();
		}		
		
		
		

		
		
		#endregion Public Methods
		
		
		
		
		
		
		
		
		
		
		
		
		#region Private Methods
		
		
		
		
		
		/// <summary>
		/// Set language to use
		/// </summary>
		private static void SetLanguage()
		{
			string envLang = System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag;
			languageFolder = AppPath + Path.DirectorySeparatorChar + "Languages";
			
			// check for language folder
			if (!Directory.Exists(languageFolder))
			{
				// use share folder to search languages
				languageFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
					             Path.DirectorySeparatorChar + Assembly.GetExecutingAssembly().GetName().Name +
						         Path.DirectorySeparatorChar + "Languages";
				
				if (!Directory.Exists(languageFolder))
				{
					// no languages founded
					throw new Exception("no language folder founded... ");
				}
			}
			
			// check for language file
			DirectoryInfo di = new DirectoryInfo(languageFolder);
			if (di.GetFiles(envLang + ".xml").Length == 1)
			{
				// language file exists, use it
				languageTag = envLang;
			}
			else
			{
				// language file don't exists, use en-US as default
				languageTag = "en-US";
			}
			
			lMan = new LanguageManager(languageFolder + Path.DirectorySeparatorChar + languageTag + ".xml");
			
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

			LogFilePath = logfilename;
			
			// attach to repository hierarchy
			log4net.Repository.Hierarchy.Hierarchy repository = LogManager.GetRepository() as log4net.Repository.Hierarchy.Hierarchy; 
			
			// check for logging into file
			if (LogToFile)
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
			if (LogToConsole)
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
		/// Parse command line arguments
		/// </summary>
		private static void ParseCommandArgs()
		{
			
			// check for console log request
			if (AppArgs.Contains("--log-console"))
			{
				LogToConsole = true;
			}

			// check for file log request
			if (AppArgs.Contains("--log-file"))
			{
				LogToFile = true;
			}
			
		}
		
		
		
		
		
		
		#endregion Private Methods
		
		
		
		
		
		
		
		
		
		
		
	}
}

