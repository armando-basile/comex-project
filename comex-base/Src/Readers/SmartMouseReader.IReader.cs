using System;
using System.Text;
using System.IO.Ports;
using System.Collections.Generic;
using log4net;
using Utility;

namespace comexbase
{
	public partial class SmartMouseReader: IReader, IDisposable
	{
		
		
		
		// Constants
		private static readonly string typeName = "SmartMouse Serial";
		
		
		// Attributes
		List<string> readers = new List<string>();
		
		
		
		
		#region IReader interface
		
		
		/// <summary>
		/// Get/Set serial port to use
		/// </summary>
		public string SelectedReader 
		{
			get {	return portName;	}
			set {	portName = value;	}
		}

		
		
		/// <summary>
		/// Get managed reader type name
		/// </summary>
		public string TypeName 
		{
			get {	return typeName;	}
		}
		
		
		
		/// <summary>
		/// Get serial ports available
		/// </summary>
		public List<string> Readers 
		{
			get {	return readers; }
		}

		
		
		
		/// <summary>
		/// Power On smartcard
		/// </summary>
		/// <returns>
		/// Empty or error message
		/// </returns>
		public string AnswerToReset (ref string response)
		{
			
			// check for selected reader
			if (portName == "")
			{
				// no reader selected
				return GlobalObj.LMan.GetString("noselreader");
			}
			
			
			try
			{
				// check for object
				if (portObject != null)
				{
					// check for connection status
					if (portObject.IsOpen)
					{
						// close is opened
						portObject.Close();
					}
				}
				
				// Open serial communication
				portObject.DtrEnable=true;
				portObject.RtsEnable=false;
				portObject.Handshake= Handshake.None;
				portObject.PortName = portName;				
				portObject.Open();
				isPortOpen = true;
				
				// Clear all buffers
				FlushBuffers();
								
				// Get ATR: Set and Re-set Reset Line
				portObject.RtsEnable = true;
				System.Threading.Thread.Sleep(50);
				portObject.RtsEnable = false;
				
				// read buffer
				return ReadData(2000, out response);
				
			}
			catch(Exception e)
			{
				log.Error("SmartMouseReader::AnswerToReset: " + e.Message + "\r\n" + e.StackTrace);
				return e.Message;
			}
			
			
			
			
			
			
		}
		
		
		
		
		/// <summary>
		/// Exchange data with smartcard
		/// </summary>
		/// <returns>
		/// Empty or error message
		/// </returns>
		public string SendReceive (string command, ref string response)
		{
			if (command.Length > 10)
			{
				// ISO IN for SmartMouse
				return ISOIN(command, ref response);
				
			}
			else
			{
				// ISO OUT
				return ISOOUT(command, ref response);
			}
		}
		
		
		
		
		/// <summary>
		/// Close serial communication
		/// </summary>
		public void CloseConnection ()
		{
			// check for port opened
			if (isPortOpen)
			{
				try
				{
					portObject.Close();
					isPortOpen = false;
				}
				catch(Exception e)
				{
					// Error detected
					log.Error("SmartMouseReader::CloseConnection: " + e.Message + "\r\n" + e.StackTrace);
				}
			}
		}
		
		
		
		
		
		
		#endregion IReader interface
		
		
		
		
		
		
		
		
		
	}
}
