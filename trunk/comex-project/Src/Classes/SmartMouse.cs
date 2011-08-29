using System;
using System.IO.Ports;
using Utility;
using log4net;

namespace comex
{
	public class SmartMouse
	{
		
		
		// Log4Net object
        private static readonly ILog log = LogManager.GetLogger(typeof(SmartMouse));
		
		private cEncoding utils = new cEncoding();
		
		private SerialPort portObject;
		private string portName = "";
		private int portSpeed = 9600;
		private string portParity;
		private int portStopBit;
		private int portDataBit = 8;
		private bool isDirectConvention = true;
		private bool isPortOpen = false;
		
		
		
		
		
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
		/// Parity of serial communication
		/// </summary>		
		public string PortParity
		{
			get	{	return portParity;	}
			set	{	portParity=value;	}
		}

		
		/// <summary>
		/// Stop Bits of serial communication
		/// </summary>		
		public int PortStopBit
		{
			get	{	return portStopBit;	}
			set	{	portStopBit = value;	}
		}

		
		/// <summary>
		/// BaudRate
		/// </summary>
		public int PortSpeed
		{
			get	{	return portSpeed;	}
			set	{	portSpeed = value;	}
		}
		
		
		/// <summary>
		/// DataBit of serial communication
		/// </summary>
		public int PortDataBit
		{
			get	{	return portDataBit;	}
			set	{	portDataBit = value;	}
		}
		
		
		#endregion Properties
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Constructor
		/// </summary>
		public SmartMouse ()
		{
			portObject = new SerialPort();
		}
		
		
		
		
		#region Public Methods
		
		
		/// <summary>
		/// Open Serial Port Communication
		/// </summary>
		public string Open()
		{
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
				
				portObject.DtrEnable=true;
				portObject.Handshake= Handshake.None;
				portObject.PortName = portName;				
				portObject.Open();
				isPortOpen = true;
				return "";
				
			}
			catch(Exception e)
			{
				log.Error(e.Message);
				return e.Message;
			}
		}
		
		
		
		/// <summary>
		/// Clear buffers
		/// </summary>
		public void FlushBuffers()
		{
			portObject.DiscardInBuffer();
			portObject.DiscardOutBuffer();
		}
		
		
		
		
		/// <summary>
		/// Close Serial Port Communication
		/// </summary>
		public string Close()
		{
			try
			{
				portObject.Close();
				isPortOpen = false;
				return "";
			}
			catch(Exception e)
			{
				log.Error(e.Message);
				return e.Message;
			}
		}

		
		
		
		
		/// <summary>
		/// Apply communication settings (speed, parity, databits, stopbits, portname)
		/// </summary>
		public string ApplySettings()
		{
			try
			{
				portObject.BaudRate = portSpeed;
				portObject.DataBits = portDataBit;
				
				// Parse Parity value ....
				if (portParity == "E")
				{
					portObject.Parity = Parity.Even;
				}
				else if (portParity == "O")
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
				log.Error(e.Message);
				return e.Message;
			}
			
		}
		
		
		
		
		
		
		
		
		/// <summary>
		/// Power On card
		/// </summary>
		public string Connect(out string response)
		{
			string outData = "";
			response = "";
			
			try
			{
				// Clear all buffers
				portObject.DiscardInBuffer();
				portObject.DiscardOutBuffer();
				
				// set and reset Reset Line
				portObject.RtsEnable = true;
				System.Threading.Thread.Sleep(50);
				portObject.RtsEnable = false;
				
				// read buffer
				outData = ReadData(800, out response);
				
				
			}
			catch(Exception e)
			{
				log.Error(e.Message);
				return e.Message;
			}
			
			return outData;
		}
		
		
		
		
		/// <summary>
		/// Read data from serial port
		/// </summary>
		public string ReadData(int timeout, out string response)
		{
			byte[] theBytes = new System.Byte[300];
			string rxHexString = "";
			int BytesToRead = 0;
			response = "";
			
			
			DateTime startTime = DateTime.Now;
			
			try
			{
				// loop for polling
				while(DateTime.Now < (startTime.AddMilliseconds(timeout)))
				{				
					BytesToRead = portObject.Read(theBytes,0,theBytes.Length);
					if (BytesToRead > 0)
					{
						// incoming data presence
						break;
					}
				}
				
				// loop for read buffer
				while (BytesToRead > 0)
				{
					rxHexString += utils.GetHexFromBytes(theBytes,0,BytesToRead);
					BytesToRead = portObject.Read(theBytes,0,theBytes.Length);
				}
			}
			catch(Exception e)
			{
				// error detected
				log.Error(e.Message);
				return e.Message;
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
		public string WriteData(string dataIN)
		{
			byte[] tmpBytes;
			
			string tmpInString = dataIN;			
			tmpInString = tmpInString.Replace("0x","");
			tmpInString = tmpInString.Replace(" ","");
			
			if (tmpInString.Trim() == "" )
			{
				return "no data to write";
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
				log.Error(e.Message);
				return e.Message;
			}
			
			return "";
		}
		
		
		
		
		
		
		
		
		
		
		
		
		#endregion Public Methods
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		#region Private Methods
		
		
		

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
		
		
		
		#endregion Private Methods
		
		
		
		
		
		
		
		

	}
}

