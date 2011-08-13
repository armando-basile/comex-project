using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using Pcsc_Sharp;
using Gtk;


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
		
		
		
		
		/// <summary>
		/// Set language to use
		/// </summary>
		public static void SetLanguage()
		{
			string envLang = System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag;
			languageFolder = AppPath + Path.DirectorySeparatorChar + "Languages";
			
			// check for language folder
			if (!Directory.Exists(languageFolder))
			{
				throw new Exception("no language folder founded... ");
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
		
		
		
		
		
		public static string InitPCSC()
		{
			PCSC = new Pcsc();
			string retStr = PCSC.EstablishContext();
			
			if (retStr != "")
			{
				return retStr;
			}
			
			return "";
		}
		
		
		
		
		public static void ClosePCSC()
		{
			PCSC.Disconnect();
			PCSC.ReleaseContext();
		}
		
		
		
		
		public static void FillSerialPorts()
		{
			SerialPortsName = new List<string>(System.IO.Ports.SerialPort.GetPortNames());
		}
		
		
		
		
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
				return PCSC.Connect(selectedReader, ref response,
			                        Pcsc.SCARD_PROTOCOL.SCARD_PROTOCOL_ANY,
			                        Pcsc.SCARD_SHARE.SCARD_SHARE_EXCLUSIVE);
			}
			else
			{
				return "Not yet implemented";
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
				return "Not yet implemented";
			}
		}
		
		
		
		
		
		
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
		
		
	}
}

