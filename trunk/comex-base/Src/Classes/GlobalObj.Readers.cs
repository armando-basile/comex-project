
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


		
		
		
		#region Public Methods
		
		
		

		
		/// <summary>
		/// Power On card
		/// </summary>
		public static string AnswerToReset(ref string response)
		{	
			response = "";
			
			if (selectedReaderType == "")
			{
				// No reader manager selected
				return lMan.GetString("noselreader");
			}
			
			// create alias
			IReader rSelected = (IReader)ReaderManager[selectedReaderType];
			
			// check for smartmouse serial reader
			if (selectedReaderType == "SmartMouse Serial")
			{
				// Apply serial port settings
				((SmartMouseReader)rSelected).PortDataBit = SerialSettings.DataBits;
				((SmartMouseReader)rSelected).PortName = selectedReader;
				((SmartMouseReader)rSelected).PortStopBit = SerialSettings.StopBits;
				((SmartMouseReader)rSelected).PortParity = SerialSettings.Parity;
				((SmartMouseReader)rSelected).PortSpeed = SerialSettings.PortSpeedReset;
				((SmartMouseReader)rSelected).IsDirectConvention = SerialSettings.IsDirectConvention;
				
				ret = ((SmartMouseReader)rSelected).ApplySettings();
				
				if (ret != "")
				{
					// Error detected
					return ret;
				}
			}
			
			
			// Get ATR
			ret = rSelected.AnswerToReset(ref response);
			
			if (ret != "")
			{
				// Error detected
				return ret;
			}
			
			response = AddSpace(response);
			
			// set selected reader powered
			isPowered = true;
			
			// check for smartmouse serial reader
			if (selectedReaderType == "SmartMouse Serial")
			{
				// change port speed after reset
				((SmartMouseReader)rSelected).PortSpeed = SerialSettings.PortSpeed;
				ret = ((SmartMouseReader)rSelected).ApplySettings();
				
				if (ret != "")
				{
					// error detected
					return ret;
				}
			}
				
			return "";
				
			
		}
		
		
		
		
		
		/// <summary>
		/// Exchange data with card
		/// </summary>
		public static string SendReceive(string command, ref string response)
		{
			
			if (selectedReaderType == "")
			{
				// No reader manager selected
				return lMan.GetString("noselreader");
			}
			
			// create alias
			IReader rSelected = (IReader)ReaderManager[selectedReaderType];
			
			command = command.Replace("0x", "");
			command = command.Replace(" ", "");
			command = command.ToUpper();
			
			if (command.Length == 0)
			{
				// wrong command format
				return LMan.GetString("wrongcmd") + "\r\n";
			}
			
			
			if (command.Length % 2 != 0)
			{
				// wrong command format
				return LMan.GetString("wrongcmd") + "\r\n";
			}
			
			// parse all digits
			foreach(char digit in command)
			{
				if (!Uri.IsHexDigit(digit))
				{
					// wrong command format
					return LMan.GetString("wrongcmd") + "\r\n";				
				}
			}
			
			// Exchange data with smartcard in selected reader
			return rSelected.SendReceive(command, ref response);
			
			
		}
		
		
		
		
		
		
		
		
		/// <summary>
		/// Close connection with reader
		/// </summary>
		public static void CloseConnection()
		{
			if (selectedReaderType == "")
			{
				// No reader manager selected
				return;
			}
			
			ReaderManager[selectedReaderType].CloseConnection();
			selectedReader = "";
		}
		
		
		
		
		
		
		
		#endregion Public Methods
		
		
		
		
		
		
		
		
		
		#region Private Methods
		
		
		
		
		
		/// <summary>
		/// Init readers managers
		/// </summary>
		private static void InitReadersManagers()
		{
			// Add PCSC reader manager
			IReader pcsc = new PcscReader();
			ReaderManager.Add(pcsc.TypeName, pcsc);
			
			// Add SmartMouse reader manager
			IReader smouse = new SmartMouseReader();
			ReaderManager.Add(smouse.TypeName, smouse);
			
			
		}
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Add space between each bytes
		/// </summary>
		private static string AddSpace(string data)
		{
			string outStr = "";
			
			for (int b=0; b<data.Length; b+=2)
			{
				outStr += data.Substring(b,2) + " ";
			}
			
			return outStr.Trim();
		}
		
		
		
		
		#endregion Private Methods
		
		
		
		
		
	}
}

