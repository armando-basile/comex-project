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
			string retContext = "";
			
			// check for selected reader
			if (selectedReader == "")
			{
				// no reader selected
				return GlobalObj.LMan.GetString("noselreader");
			}
			
			// close connection and context
			CloseConnection();
			System.Threading.Thread.Sleep(20);
			ReleaseContext();
				
			// delay before power on
			System.Threading.Thread.Sleep(200);
			
			// Try to create new context
			retContext = CreateContext();
			if (retContext != "")
			{
				// error detected
				return retContext;
			}
			
			System.Threading.Thread.Sleep(20);
			
			// Connect to smartcard
			int ret = SCardConnect(nContext, selectedReader, 
								   (uint)SCARD_SHARE.SCARD_SHARE_SHARED,
								   (uint)SCARD_PROTOCOL.SCARD_PROTOCOL_T0 |
								   (uint)SCARD_PROTOCOL.SCARD_PROTOCOL_T1,			                       
			                   ref nCard, ref nActiveProtocol);
			
			if (ret != 0)
			{
				// Error detected
				log.Error("PcScReader.IReader::AnswerToReset: SCardConnect " + parseError(ret));
				return "PcScReader.IReader::AnswerToReset: SCardConnect " + parseError(ret);
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
				log.Error("PcScReader.IReader::SendReceive: SCardTransmit " + parseError(ret));
				return "PcScReader.IReader::SendReceive: SCardTransmit " + parseError(ret);
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
			// check for card context
			if (nCard.ToInt64() != 0)
			{
				// disconnect
				ret = SCardDisconnect(nCard, (uint)SCARD_DISPOSITION.SCARD_UNPOWER_CARD);
				log.Debug("PcScReader.IReader::CloseConnection: SCardDisconnect " + ret.ToString());
				nCard = IntPtr.Zero;
			}
		}
		

		
  		#endregion IReader interface
		
		
		
		
		
		
		
		
		
	}
}

