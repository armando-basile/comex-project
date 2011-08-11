using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
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
		private static LanguageManager lMan = null;
		
		
		#region Properties
		
		
		/// <summary>
		/// Return language manager object
		/// </summary>
		public static LanguageManager LMan { get { return lMan; } }
		
		
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
		
		
		#endregion Public Methods
		
		
	}
}

