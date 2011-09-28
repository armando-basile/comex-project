using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using log4net;
using Utility;


namespace comexbase
{
	
	public partial class PcscReader: IReader, IDisposable
	{

		// Constants
		private static readonly string typeName = "PCSC";
		
		
		// Attributes
		private List<string> readers = new List<string>();
		
		
		
		
		
		

		#region IReader interface
		
		
		/// <summary>
		/// Get/Set reader to use
		/// </summary>
		public string SelectedReader 
		{
			get { return selectedReader;}
			set { selectedReader = value;}
		}
		
		
		
		
		/// <summary>
		/// Type of managed readers
		/// </summary>
		public string TypeName 
		{
			get { return typeName;	}
		}
		
		
		
		
		/// <summary>
		/// PCSC readers list
		/// </summary>
		public List<string> Readers 
		{
			get { return readers;	}
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
			if (selectedReader == "")
			{
				// no reader selected
				return GlobalObj.LMan.GetString("noselreader");
			}
			
			// check for prev. connection
			if (nCard.ToInt64() != 0)
			{
				CloseConnection();
			}
			
			// Connect to smartcard
			int ret = SCardConnect(nContext, selectedReader, 
								   (uint)SCARD_PROTOCOL.SCARD_PROTOCOL_ANY,
			                       (uint)SCARD_SHARE.SCARD_SHARE_EXCLUSIVE,
			                   ref nCard, ref nActiveProtocol);
			
			if (ret != 0)
			{
				// Error detected
				log.Error("SCardConnect: " + parseError(ret));
				return "SCardConnect: " + parseError(ret);
			}
			
			
			// Get ATR
			string resp = "";
			string retStatus = ReaderStatus(ref resp);
			if (retStatus != "")
			{
				// Error detected
				return retStatus;
			}

			response = resp;			
			return "";
			
		}
		
		
		
		
		
		/// <summary>
		/// Exchange data with smartcard
		/// </summary>
		/// <returns>
		/// Empty or error message
		/// </returns>
		public string SendReceive (string command, ref string response)
		{
			// check for selected reader
			if (selectedReader == "")
			{
				// no reader selected
				return GlobalObj.LMan.GetString("noselreader");
			}
			
			
			IntPtr retCommandLen = new IntPtr(261);
			
			// remove unused digits
			command = command.Trim().Replace("0x", "").Replace(" ", "");
			
			// Set input command byte array
			byte[] inCommandByte = utilityObj.GetBytesFromHex(command);			
			byte[] outCommandByte = new byte[261];
			
			// Prepare structures
			SCARD_IO_REQUEST pioSend = new SCARD_IO_REQUEST();
			SCARD_IO_REQUEST pioRecv = new SCARD_IO_REQUEST();
			pioSend.dwProtocol = (uint)cardProtocol;			
			pioSend.cbPciLength = (uint)8;
			pioRecv.dwProtocol = (uint)cardProtocol;
			pioRecv.cbPciLength = 0;
			
			// exchange data with smartcard
			int ret = SCardTransmit(nCard, 
			                    ref pioSend, 
			                        inCommandByte, 		                    
			                        inCommandByte.Length, 
			                        IntPtr.Zero,
			                        outCommandByte, 
			                    out retCommandLen);
			
			if (ret != 0)
			{	
				// Error detected
				log.Error("SCardTransmit: " + parseError(ret));
				return "SCardTransmit: " + parseError(ret);
			}
			
			// Extract response
			response = utilityObj.GetHexFromBytes(outCommandByte, 0, retCommandLen.ToInt32()).Trim();
			
			return "";
			
			
		}
		
		
		
		
		
		/// <summary>
		/// Disconnect smartcard from reader
		/// </summary>
		public void CloseConnection ()
		{
			// Disconnect from smartcard
			SCardDisconnect(nCard, SCARD_UNPOWER_CARD);
			nCard = IntPtr.Zero;
		}
		

		
  		#endregion IReader interface
		
		
		
		
		
		
		
		
		
	}
}

