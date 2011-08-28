using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using Pcsc_Sharp;

using log4net;
using log4net.Config;


namespace comex
{
	
	/// <summary>
	/// Static global class
	/// </summary>
	public static class GlobalObj
	{

		private static string languageFolder = "";
		private static string languageTag = "";
		private static string selectedReader = "";
		private static string ret = "";
		private static LanguageManager lMan = null;
		
		// Log4Net object
        private static readonly ILog log = LogManager.GetLogger(typeof(GlobalObj));
		
		
		#region Properties
		
		
		/// <summary>
		/// Return language manager object
		/// </summary>
		public static LanguageManager LMan { get { return lMan; } }
		
		
		
		/// <summary>
		/// PCSC reader manager
		/// </summary>
		public static Pcsc PCSC { get; set ;}
		
		
		/// <summary>
		/// Set/Get if selected reader is a PCSC reader
		/// </summary>
		public static bool IsPCSC { get; set; }
		
		
		/// <summary>
		/// SmartMouse reader manager
		/// </summary>
		public static SmartMouse SMouse { get; set ;}
		
		
		
		
		/// <summary>
		/// PCSC readers name
		/// </summary>
		public static List<string> PCSC_Readers { get; set ;}
		
		
		
		
		/// <summary>
		/// PC serial port
		/// </summary>
		public static List<string> SerialPortsName { get; set;}
		
		
		/// <summary>
		/// Return selected reader name
		/// </summary>
		public static string SelectedReader 
		{ 
			get	{	return selectedReader; }
			set
			{
				selectedReader = value;
				if (PCSC_Readers.Contains(value))
				{
					// pcsc reader
					IsPCSC = true;
				}
				else
				{
					// serial reader
					IsPCSC = false;
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
					Assembly.GetExecutingAssembly().GetName().Version.ToString();
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
		
		
/*		
		/// <summary>
		/// Wait for gui processes
		/// </summary>
		public static void GtkWait()
		{
			while (Gtk.Application.EventsPending ())
			{
                Gtk.Application.RunIteration ();
			}
		}
*/
		
		

		
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
			
			
			FillSerialPorts();
			
			SMouse = new SmartMouse();
			
			ret = FillPcscReaders();
			
			if (ret != "")
			{
				return ret;
			}
			
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
		/// Power On card
		/// </summary>
		public static string AnswerToReset(ref string response)
		{
			// close other context
			ClosePCSC();
		
			// check for serial port opened
			if (SMouse.IsPortOpen)
			{
				// close serial port
				SMouse.Close();
			}
			
			if (IsPCSC)
			{
				// create new context
				ret = InitPCSC();
				
				if (ret != "")
				{
					return ret;
				}
				
				// Connect to smartcard
				ret = PCSC.Connect(selectedReader, ref response,
			                       Pcsc.SCARD_PROTOCOL.SCARD_PROTOCOL_ANY,
			                       Pcsc.SCARD_SHARE.SCARD_SHARE_EXCLUSIVE);
				
				string tmp = "";
				
				for (int b=0; b<response.Length; b+=2)
				{
					tmp += response.Substring(b,2) + " ";
				}
				
				tmp = tmp.Trim();
				response = tmp;
				
				return ret;
			}
			else
			{
				return lMan.GetString("notimplemented");
			}
		}
		
		
		
		
		
		/// <summary>
		/// Exchange data with card
		/// </summary>
		public static string SendReceive(string command, ref string response)
		{
			
			command = command.Replace("0x", "");
			command = command.Replace(" ", "");
			command = command.ToUpper();
			
			if (command.Length % 2 != 0)
			{
				// wrong command format
				return GlobalObj.LMan.GetString("wrongcmd") + "\r\n";
			}
			
			// parse all digits
			foreach(char digit in command)
			{
				if (!Uri.IsHexDigit(digit))
				{
					// wrong command format
					return GlobalObj.LMan.GetString("wrongcmd") + "\r\n";				
				}
			}
			
			
			if (IsPCSC)
			{
				// exchange data with smartcard using PCSC
				return GlobalObj.PCSC.Transmit(command, ref response);
			}
			else
			{
				return lMan.GetString("notimplemented");
			}
		}
		
		
		
		
		
		/// <summary>
		/// Close connection with reader
		/// </summary>
		public static void CloseConnection()
		{
			if (IsPCSC)
			{
				ClosePCSC();				
			}
			else
			{
				// check for serial port opened
				if (SMouse.IsPortOpen)
				{
					// close serial port
					SMouse.Close();
				}
			}
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
		
		
		
		
		
		/// <summary>
		/// Close PCSC communication
		/// </summary>
		private static void ClosePCSC()
		{
			PCSC.Disconnect();
			PCSC.ReleaseContext();
		}

		
		
		
		
		
		/// <summary>
		/// Create context to use pcsc
		/// </summary>
		private static string InitPCSC()
		{
			PCSC = new Pcsc();
			string retStr = PCSC.EstablishContext();
			
			if (retStr != "")
			{
				return retStr;
			}
			
			return "";
		}

		
		
		
		
		
		

		/// <summary>
		/// Detect available serial ports
		/// </summary>
		private static void FillSerialPorts()
		{
			SerialPortsName = new List<string>(System.IO.Ports.SerialPort.GetPortNames());
		}
		
		
		
		
		
		/// <summary>
		/// Detect available pcsc readers
		/// </summary>
		private static string FillPcscReaders()
		{
			// create new PCSC manager and create context
			string retStr = InitPCSC();
			
			if (retStr != "")
			{
				// error detected
				log.Error(retStr);
				return retStr;
			}
			else
			{
				// retrieve pcsc readers
				string[] pcsc_readers = new string[0];
				PCSC_Readers = new List<string>();
				retStr = PCSC.ListReaders(out pcsc_readers);			
				
				if (retStr != "")
				{
					// error detected
					log.Error(retStr);
					return retStr;
				}
				else
				{
					PCSC_Readers = new List<string>(pcsc_readers);
				}
			}
			
			ClosePCSC();
			return "";
		}
		
		
		
		
		
		#endregion Private Methods
		
		
		
		
		
		
		
		
		
		
		
	}
}

