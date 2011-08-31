
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
	public static partial class GlobalObj
	{


		
		
		
		#region Public Methods
		
		
		

		
		/// <summary>
		/// Power On card
		/// </summary>
		public static string AnswerToReset(ref string response)
		{			
			response = "";
			
			if (IsPCSC)
			{
				// close other context
				ClosePCSC();
			}
			else
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
				
				if (ret != "")
				{
					// error detected
					log.Error(ret);
					return ret;
				}
				
				response = AddSpace(response);
				
				isPowered = true;				
				return ret;
			}
			else
			{
				// SmartMouse serial reader				
				SMouse.PortDataBit = SerialSettings.DataBits;
				SMouse.PortName = selectedReader;
				SMouse.PortStopBit = SerialSettings.StopBits;
				SMouse.PortParity = SerialSettings.Parity;
				SMouse.PortSpeed = SerialSettings.PortSpeedReset;
				SMouse.IsDirectConvention = SerialSettings.IsDirectConvention;
				
				ret = SMouse.ApplySettings();
				ret = SMouse.Open();
				ret = SMouse.Connect(out response);				
				
				log.Debug("response: " + response);
				
				if (ret != "")
				{
					// error detected
					return ret;
				}
				
				response = AddSpace(response);
				
				// change port speed after reset
				SMouse.PortSpeed = SerialSettings.PortSpeed;
				ret = SMouse.ApplySettings();
				
				if (ret != "")
				{
					// error detected
					return ret;
				}
				isPowered = true;		
				return ret;
				
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
			
			
			if (IsPCSC)
			{
				// exchange data with smartcard using PCSC
				return PCSC.Transmit(command, ref response);
			}
			else
			{
				// Not Yet Stable...
				return LMan.GetString("notimplemented");
				
/*
				// SmartMouse serial
				if (command.Length > 10)
				{
					// ISO IN
					
					// Write header
					SMouse.FlushBuffers();
					ret = SMouse.WriteData(command.Substring(0,10));
					if (ret != "")
					{
						// error detected
						return ret;
					}
					
					// Read knowledge
					ret = SMouse.ReadData(2000, out response);				
					if (ret != "")
					{
						// error detected
						return ret;
					}

					// check for right knowledge
					if (response != command.Substring(0,10))
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
									
					ret = SMouse.ReadData(2000, out response);				
					if (ret != "")
					{
						// error detected
						return ret;
					}
					
				}
				else
				{
					// ISO OUT
					
					// write command
					SMouse.FlushBuffers();
					ret = SMouse.WriteData(command);
					
					if (ret != "")
					{
						// error detected
						return ret;
					}
									
					ret = SMouse.ReadData(2000, out response);				
					if (ret != "")
					{
						// error detected
						return ret;
					}
				}
				
				return ret;
*/

			}
		}
		
		
		
		
		
		/// <summary>
		/// Close connection with reader
		/// </summary>
		public static void CloseConnection()
		{
			if (selectedReader == "")
			{
				return;
			}
			
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
			
			selectedReader = "";
			isPowered = false;
		}
		
		
		
		
		
		
		
		#endregion Public Methods
		
		
		
		
		
		
		
		
		
		#region Private Methods
		
		
		

		
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

