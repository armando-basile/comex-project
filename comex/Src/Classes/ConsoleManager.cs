using System;
using System.Collections.Generic;

using log4net;
using comexbase;

namespace comex
{
	
	
	/// <summary>
	/// Perform exchange command using console
	/// </summary>
	public static class ConsoleManager
	{

		// Log4Net object
        private static readonly ILog log = LogManager.GetLogger(typeof(ConsoleManager));
		
		
		private static string ATR = "";
		private static string command = "";
		private static string response = "";
		private static string ret = "";
		#region Public Methods
		
		
		/// <summary>
		/// Console Application
		/// </summary>
		public static void StartApp()
		{
			
			// create readers list
			List<string> allreaders = new List<string>();
			allreaders.AddRange(GlobalObj.PCSC_Readers);
			allreaders.AddRange(GlobalObj.SerialPortsName);
			
			// display available readers
			Console.WriteLine("\r\n" + GlobalObj.LMan.GetString("readerslist") + ":");
			for (int n=0; n<allreaders.Count; n++)
			{
				Console.WriteLine(n.ToString().PadLeft(3) + ". " + allreaders[n]);
			}
			Console.WriteLine("\r\n" + GlobalObj.LMan.GetString("readersel"));
			Console.Write("> ");
			command = Console.ReadLine();
			
			if (command == "EXIT")
			{
				// exit from application
				return;
			}
			
			try
			{
				// set reader to use
				GlobalObj.SelectedReader = allreaders[int.Parse(command)];
			}
			catch (Exception Ex)
			{
				// error detected
				log.Error(Ex.Message);
				return;
			}
			
 			
			// Connect to smartcard
			ret = GlobalObj.AnswerToReset(ref ATR);
			
			if (ret != "")
			{
				// error on answer to reset
				log.Error(ret);
				Console.WriteLine(ret);
				return;
			}
			
			Console.WriteLine("\r\nAnswer To Reset: " + ATR + "\r\n");
			
			while (1==1)
			{			
				Console.WriteLine(GlobalObj.LMan.GetString("cmdtosend"));
				Console.Write("> ");
				command = Console.ReadLine();
				
				if (command == "EXIT")
				{
					// exit from loop and from application
					GlobalObj.CloseConnection();
					break;
				}
				
				ret = GlobalObj.SendReceive(command, ref response);
				if (ret != "")
				{
					// error on send command
					log.Error(ret);
					Console.WriteLine("< " + ret);
				}
				else
				{
					// response returned
					Console.WriteLine("< " + response + "\r\n");
				}
				
			}
			
		}
		
		
		
		
		
		
		
		
		
		
		#endregion Public Methods
		
		
		
		
		
		
		
		
	}
}

