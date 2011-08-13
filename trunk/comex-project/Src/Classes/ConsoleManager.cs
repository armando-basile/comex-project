using System;
using log4net;


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
			
			// check for pcsc reader
			if (GlobalObj.PCSC_Readers.Length == 0)
			{
				// no pcsc reader found
				Console.WriteLine(GlobalObj.LMan.GetString("nopcscreader"));
				return;
			}
			
			GlobalObj.SelectedReader = GlobalObj.PCSC_Readers[0];
			Console.WriteLine(GlobalObj.LMan.GetString("selreader") + ": " + GlobalObj.SelectedReader);
			
			// Connect to smartcard
			ret = GlobalObj.PCSC.Connect(GlobalObj.SelectedReader,
			                             ref ATR,
			                             Pcsc_Sharp.Pcsc.SCARD_PROTOCOL.SCARD_PROTOCOL_ANY,
			                             Pcsc_Sharp.Pcsc.SCARD_SHARE.SCARD_SHARE_EXCLUSIVE);
			
			if (ret != "")
			{
				// error on answer to reset
				log.Error(ret);
				Console.WriteLine(ret);
				return;
			}
			
			Console.WriteLine("\r\nAnswer To Reset: " + ATR + "\r\n");
			
			Console.WriteLine(GlobalObj.LMan.GetString("cmdtosend"));
			Console.Write("> ");
			command = Console.ReadLine();
			
			if (command == "EXIT")
			{
				GlobalObj.PCSC.Disconnect();
				return;
			}
			
			ret = GlobalObj.PCSC.Transmit(command, ref response);
			if (ret != "")
			{
				// error on send command
				log.Error(ret);
				Console.WriteLine("< " + ret);
				return;
			}
			
			Console.WriteLine("< " + response + "\r\n");
			
			
		}
		
		
		
		
		
		
		
		
		
		
		#endregion Public Methods
		
		
		
		
		
		
		
		
	}
}

