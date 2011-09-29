using System;
using System.Text;
using System.IO.Ports;
using System.Collections.Generic;
using log4net;

using Utility;


namespace comexbase
{
	
	/// <summary>
	/// Manage SmartMouse Phoenix serial readers
	/// </summary>
	public partial class SmartMouseReader: IReader, IDisposable
	{
		
		// Attributes
		
		private SerialPort portObject;
		private string portName = "";
		private int portSpeed = 9600;
		private string portParity;
		private int portStopBit;
		private int portDataBit = 8;
		private bool isDirectConvention = true;
		private bool isPortOpen = false;		
		private string retStr = "";
		
		private cEncoding utils = new cEncoding();
		
		
		// Log4Net object
        private static readonly ILog log = LogManager.GetLogger(typeof(SmartMouseReader));
		
		
		
		
		
		
		#region Properties
		
		
		
		
		/// <summary>
		/// get Port status 
		/// </summary>
		public bool IsPortOpen
		{
			get	{	return isPortOpen;	}
		}
		
		

		/// <summary>
		/// set Direct or Inverse convention 
		/// </summary>
		public bool IsDirectConvention
		{
			get	{	return isDirectConvention;	}
			set	{	isDirectConvention = value;	}
		}

		
		
		/// <summary>
		/// Name of port to use
		/// </summary>
		public string PortName
		{
			get	{	return portName;	}
			set	{	portName = value;	}
		}
		
		
		
		/// <summary>
		/// Parity of serial communication ('Even', 'Odd', 'None')
		/// </summary>		
		public string PortParity
		{
			get	{	return portParity;	}
			set	{	portParity=value;	}
		}
		
		
		
		/// <summary>
		/// Stop Bits of serial communication (1, 1.5, 2)
		/// </summary>		
		public int PortStopBit
		{
			get	{	return portStopBit;	}
			set	{	portStopBit = value;	}
		}
		
		

		/// <summary>
		/// Data Bits of serial communication
		/// </summary>
		public int PortDataBit
		{
			get	{	return portDataBit;	}
			set	{	portDataBit = value;	}
		}
		
		
		
		/// <summary>
		/// BaudRate
		/// </summary>
		public int PortSpeed
		{
			get	{	return portSpeed;	}
			set	{	portSpeed = value;	}
		}
		
		
		
		
		#endregion Properties
		
		
		
		
		
		
		/// <summary>
		/// Initializes a new instance of SmartMouseReader class.
		/// </summary>
		public SmartMouseReader ()
		{
			portObject = new SerialPort();
			portObject.ReadTimeout = 50;
			
			// Get serial ports available
			readers = new List<string>(System.IO.Ports.SerialPort.GetPortNames());
		}
		
		
		
		
		
		

		/// <summary>
		/// Apply communication settings (speed, parity, databits, stopbits, portname)
		/// </summary>
		/// <returns>
		/// Empty or error message
		/// </returns>
		public string ApplySettings()
		{
			try
			{
				portObject.BaudRate = portSpeed;
				portObject.DataBits = portDataBit;
				
				// Parse Parity value ....
				if (portParity == "Even")
				{
					portObject.Parity = Parity.Even;
				}
				else if (portParity == "Odd")
				{
					portObject.Parity = Parity.Odd;
				}
				else
				{
					portObject.Parity = Parity.None;
				}
				
				
				
				// Parse StopBits value ....
				if (portStopBit == 1)
				{
					portObject.StopBits = StopBits.One;
				}
				else if (portStopBit == 2)
				{
					portObject.StopBits = StopBits.Two;
				}
				else if (portStopBit == 1.5)
				{
					portObject.StopBits = StopBits.OnePointFive;
				}
				else
				{
					portObject.StopBits = StopBits.None;
				}
				
				return "";
				
			}
			catch(Exception e)
			{
				log.Error("SmartMouseReader::ApplySettings: " + e.Message + "\r\n" + e.StackTrace);
				return e.Message;
			}
			
		}
		
		
		
		
		
		
		
		#region IDisposable interface
		
		public void Dispose ()
		{
			throw new NotImplementedException ();
		}
		
		#endregion IDisposable interface
		
		
		
		
		
		
		
		
		
		#region Private methods
		
		
		
		/// <summary>
		/// Clear buffers
		/// </summary>
		private void FlushBuffers()
		{
			portObject.DiscardInBuffer();
			portObject.DiscardOutBuffer();
		}
		
			
		

		
		/// <summary>
		/// Read data from serial port
		/// </summary>
		/// <returns>
		/// Empty or error message
		/// </returns>
		private string ReadData(int timeout, out string response)
		{
			byte[] theBytes = new System.Byte[300];
			string rxHexString = "";
			int BytesToRead = 0;
			response = "";
			
			
			DateTime startTime = DateTime.Now;
			
			// loop for polling
			while(DateTime.Now < (startTime.AddMilliseconds(timeout)))
			{
				try
				{
					// try to read bytes
					BytesToRead = 0;
					BytesToRead = portObject.Read(theBytes,0,theBytes.Length);						
				}
				catch(Exception Ex)
				{
					// check for non timeout exception
					if (Ex.GetType() != typeof(TimeoutException))
					{
						// error detected
						log.Error("SmartMouseReader::ReadData: " + Ex.GetType().ToString() + " " + Ex.Message + "\r\n" + Ex.StackTrace);
						return Ex.Message;
					}
				}
				
				if (BytesToRead > 0)
				{
					// incoming data presence
					break;
				}
			}
			
			
			
			// loop for read buffer
			while (BytesToRead > 0)
			{
				// update string from buffer
				rxHexString += utils.GetHexFromBytes(theBytes,0,BytesToRead);
				try
				{
					// try to read bytes
					BytesToRead = 0;
					BytesToRead = portObject.Read(theBytes,0,theBytes.Length);
				}
				catch(Exception Ex)
				{
					// check for non timeout exception
					if (Ex.GetType() != typeof(TimeoutException))
					{
						// error detected
						log.Error("SmartMouseReader::ReadData: " + Ex.GetType().ToString() + " " + Ex.Message + "\r\n" + Ex.StackTrace);
						return Ex.Message;
					}
				}
			}
			
			
			// check for inverse convention
			if (!isDirectConvention)
			{
				rxHexString = inverseToDirect(rxHexString);
			}

			response = rxHexString.ToUpper();
			return "";
		}
		
		
		
		
		
		/// <summary>
		/// Write Buffer
		/// </summary>
		/// <returns>
		/// Empty or error message
		/// </returns>
		private string WriteData(string dataIN)
		{
			byte[] tmpBytes;
			
			string tmpInString = dataIN;			
			tmpInString = tmpInString.Replace("0x","");
			tmpInString = tmpInString.Replace(" ","");
			
			if (tmpInString.Trim() == "" )
			{
				return "SmartMouseReader::WriteData: no data to write";
			}
			
			
			// check for inverse convention
			if (!isDirectConvention)
			{
				tmpInString = directToInverse(tmpInString);
			}
			
			// Prepare data buffer
			tmpBytes = utils.GetBytesFromHex(tmpInString);
			
			try
			{
				portObject.Write(tmpBytes,0,tmpBytes.Length);
			}
			catch(Exception e)
			{
				// Error detected
				log.Error("SmartMouseReader::WriteData: " + e.GetType().ToString() + " " + e.Message + "\r\n" + e.StackTrace);
				return e.Message;
			}
			
			return "";
		}
		
		
		
		
		

		/// <summary>
		/// Convert hexadecimal direct string to inverse
		/// </summary>
		private string directToInverse(string inStrValue)
		{
			string outString = "";
			string myByte;
			
			for (int j=0; j<inStrValue.Length; j+=2)
			{
        		myByte = inStrValue.Substring(j,2);
        		myByte = utils.HexToBin(myByte);
        		myByte = utils.swapBytes(myByte);
        		myByte = utils.turnBytes(myByte);
        		myByte = utils.BinToHex(myByte);
				outString += myByte;
        	}
    
			return outString;		
		}
		
		
		
		
		
		/// <summary>
		/// Convert hexadecimal inverse string to direct
		/// </summary>
		private string inverseToDirect(string myInputData)
		{
			string outString = "";
			string myByte;
			string inStrValue = myInputData;
			
			for (int j=0; j<inStrValue.Length; j+=2)
			{
        		myByte = inStrValue.Substring(j,2);
        		myByte = utils.HexToBin(myByte);
        		myByte = utils.turnBytes(myByte);
        		myByte = utils.swapBytes(myByte);
        		myByte = utils.BinToHex(myByte);
				outString += myByte;
        	}
    
			return outString;		
		}
		
		
		
		
		
		
		
		/// <summary>
		/// Perform ISOIN command using SmartMouse reader
		/// </summary>
		/// <returns>
		/// Empty or error message
		/// </returns>
		private string ISOIN(string command, ref string response)
		{
					
			// Write header
			FlushBuffers();
			retStr = WriteData(command.Substring(0,10));
	
			if (retStr != "")
			{
				// error detected
				log.Error("SmartMouseReader::ISOIN: " + retStr);
				return retStr;
			}
			
			
			// Read knowledge
			retStr = ReadData(SerialSettings.ReadTimeout, out response);

			if (retStr != "")
			{
				// error detected
				log.Error("SmartMouseReader::ISOIN: " + retStr);
				return retStr;
			}
	
			
			// check for right knowledge			
			if (response != command.Substring(0,10) + command.Substring(2,2))
			{
				// wrong command header
				log.Error("SmartMouseReader::ISOIN: wrong command header " + response);
				return response;
			}
			
			// write command data
			FlushBuffers();
			retStr = WriteData(command.Substring(10));					
	
			if (retStr != "")
			{
				// error detected
				log.Error("SmartMouseReader::ISOIN: " + retStr);			
				return retStr;
			}
	
			retStr = ReadData(SerialSettings.ReadTimeout, out response);				
	
			if (retStr != "")
			{
				// error detected	
				log.Error("SmartMouseReader::ISOIN: " + retStr);		
				return retStr;
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
		/// <returns>
		/// Empty or error message
		/// </returns>
		private string ISOOUT(string command, ref string response)
		{
					
			// Write command
			FlushBuffers();
			retStr = WriteData(command);
	
			if (retStr != "")
			{
				// error detected
				log.Error("SmartMouseReader::ISOOUT: " + retStr);		
				return retStr;
			}
			
			
			// Read response
			retStr = ReadData(SerialSettings.ReadTimeout, out response);

			if (retStr != "")
			{
				// error detected
				log.Error("SmartMouseReader::ISOOUT: " + retStr);		
				return retStr;
			}
			
			// check for command part in response
			if (response.IndexOf(command + command.Substring(2,2)) == 0)
			{
				// remove command part from response
				response = response.Substring(command.Length + 2);
			}
			
			return "";
		}
		
		
		
		
		#endregion Private methods
		
		
		
		
	}
}

