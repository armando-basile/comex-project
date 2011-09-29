
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
		
		
		

		/*
		/// <summary>
		/// Close PCSC communication
		/// </summary>
		private static void ClosePCSC()
		{
			try
			{
				PCSC.Disconnect();
				PCSC.ReleaseContext();
			}
			catch (Exception Ex)
			{
				log.Warn(Ex.GetType().ToString() + ": " + Ex.Message);
			}
		}

		
		
		
		
		
		/// <summary>
		/// Create context to use pcsc
		/// </summary>
		private static string InitPCSC()
		{
			PCSC = new Pcsc();
			try
			{
				PCSC.EstablishContext();
			}
			catch (Exception Ex)
			{
			  	log.Warn(Ex.GetType().ToString() + ": " + Ex.Message);
				return Ex.GetType().ToString() + ": " + Ex.Message;
			}
			
			return "";
		}

		
		*/
		
		
		
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
		
		
		
		
		
		
/*
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
			PCSC_Readers = new List<string>();
			
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
		*/
		
		
		
		
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
		
		
/*		
		
		
		/// <summary>
		/// Perform ISOIN command using SmartMouse reader
		/// </summary>
		private static string ISOIN(string command, ref string response)
		{
					
			// Write header
			SMouse.FlushBuffers();
			ret = SMouse.WriteData(command.Substring(0,10));
	
			if (ret != "")
			{
				// error detected						
				return ret;
			}
			
			
			// Read knowledge
			ret = SMouse.ReadData(SerialSettings.ReadTimeout, out response);

			if (ret != "")
			{
				// error detected						
				return ret;
			}
	
			// check for right knowledge			
			if (response != command.Substring(0,10) + command.Substring(2,2))
			{
				// wrong command header			
				return response;
			}
			
			// write command data
			SMouse.FlushBuffers();
			ret = SMouse.WriteData(command.Substring(10));					
	
			if (ret != "")
			{
				// error detected						
				return ret;
			}
	
			ret = SMouse.ReadData(SerialSettings.ReadTimeout, out response);				
	
			if (ret != "")
			{
				// error detected						
				return ret;
			}
			
			// check for command part in response
			if (response.IndexOf(command.Substring(10)) == 0)
			{
				// remove command part from response
				response = response.Substring(command.Substring(10).Length);
			}
			
			return "";
		}
		
		
		
		
		
		
		/// <summary>
		/// Perform ISOOUT command using SmartMouse reader
		/// </summary>
		private static string ISOOUT(string command, ref string response)
		{
					
			// Write command
			SMouse.FlushBuffers();
			ret = SMouse.WriteData(command);
	
			if (ret != "")
			{
				// error detected						
				return ret;
			}
			
			
			// Read response
			ret = SMouse.ReadData(SerialSettings.ReadTimeout, out response);

			if (ret != "")
			{
				// error detected						
				return ret;
			}
			
			// check for command part in response
			if (response.IndexOf(command + command.Substring(2,2)) == 0)
			{
				// remove command part from response
				response = response.Substring(command.Length + 2);
			}
			
			return "";
		}
		
		
		
		*/
		
		
		
		#endregion Private Methods
		
		
		
		
		
	}
}

