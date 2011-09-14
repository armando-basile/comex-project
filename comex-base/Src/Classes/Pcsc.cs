
using System;
using System.Text;
using System.Collections.Generic;
using Utility;

// Neded for [DllImport] 
using System.Runtime.InteropServices;

namespace Pcsc_Sharp
{
	
	/// <summary>
	/// Manage Pcsc readers
	/// </summary>
	public class Pcsc
	{
		
	
		
#region SCARD Structures
		
		/// <summary>
		/// The SCARD_READERSTATE structure is used by functions
		/// for tracking smart cards within readers.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct	SCARD_READERSTATE
		{
		    public string szReader;
	        public IntPtr pvUserData;
	        public uint dwCurrentState;
	        public uint dwEventState;
	        public uint cbAtr;
	        
	        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
	        public byte[] rgbAtr;
		}
	
		
		
		/// <summary>
		/// The SCARD_IO_REQUEST structure begins a protocol control
		/// information structure. Any protocol-specific information
		/// then immediately follows this structure. The entire length
		/// of the structure must be aligned with the underlying hardware
		/// architecture word size. For example, in Win32 the length of
		/// any PCI information must be a multiple of four bytes so
		/// that it aligns on a 32-bit boundary.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct SCARD_IO_REQUEST
		{
			public uint dwProtocol;
			public uint cbPciLength;
		}
		
		
		
#endregion SCARD Structures		
		
		
		
		
		
		
		
		
		
		
#region DllImport
		
		[DllImport("winscard")]
		private static extern int SCardEstablishContext(uint dwScope, 
		                                                int nNotUsed1, 
		                                                int nNotUsed2, 
		                                            ref IntPtr phContext);
	
		
		[DllImport("winscard")]
		private static extern int SCardListReaders(IntPtr hContext, 
		                                           string cGroups,
			                                       byte[] cReaderLists, 
			                                   out IntPtr nReaderCount);
		
		
		[DllImport("winscard")]
		private static extern int SCardReleaseContext(IntPtr phContext);
	
		
		[DllImport("winscard")]
		private static extern int SCardListReaderGroups(IntPtr hContext, 
		                                           	    System.Text.StringBuilder cGroups, 
		                                            out IntPtr nStringSize);
	
		
		[DllImport("winscard")]
		private static extern int SCardConnect(IntPtr hContext, 
		                                      string cReaderName,
			                                  uint dwShareMode, 
			                                  uint dwPrefProtocol, 
			                              ref IntPtr phCard, 
			                              ref IntPtr ActiveProtocol);
	
		
		[DllImport("winscard")]
		private static extern int SCardDisconnect(IntPtr hCard, 
		                                         int Disposition);
	
		
	
		[DllImport("winscard")]
		private static extern int SCardStatus(IntPtr hCard, 
		                                      byte[] ReaderName, 
		                                  ref IntPtr RLen, 
		                                  out int State, 
		                                  out int Protocol, 
		                                      byte[] ATR, 
		                                  ref IntPtr ATRLen);
	
		
		[DllImport("winscard", SetLastError=true)]
		private static extern int SCardTransmit(IntPtr hCard, 
		                                    ref SCARD_IO_REQUEST pioSendPci,
		                                        byte[] pbSendBuffer, 
		                                        int cbSendLength, 
		                                        IntPtr pioRecvPci,
	                                            byte[] pbRecvBuffer, 
	                                        out IntPtr pcbRecvLength);
	
		
	    [DllImport("winscard")]             
	    private static extern int SCardGetStatusChange(IntPtr hContext, 
	                                                   uint dwTimeout, 
	                                               ref SCARD_READERSTATE rgReaderStates,
	                                                   int cReaders);
		
		
#endregion DllImport
	
		
		
		
#region Constants for PC/SC
				
		public enum SCARD_PROTOCOL: uint
		{
			SCARD_PROTOCOL_ANY = 3,
			SCARD_PROTOCOL_RAW = 4,
			SCARD_PROTOCOL_T0 = 1,
			SCARD_PROTOCOL_T1 = 2
		}
		

		public enum SCARD_SHARE: uint
		{
			SCARD_SHARE_DIRECT = 3,			
			SCARD_SHARE_EXCLUSIVE = 1,
			SCARD_SHARE_SHARED = 2
		}
		
		
		private const int SCARD_SCOPE_USER = 0;
		private const int SCARD_SCOPE_TERMINAL = 1;
		private const int SCARD_SCOPE_SYSTEM = 2;
		
		private const int SCARD_LEAVE_CARD = 0;
		private const int SCARD_UNPOWER_CARD = 2;
	
		public const uint SCARD_STATE_UNAWARE = 0x0;
	    public const uint SCARD_STATE_IGNORE = 0x1;
		public const uint SCARD_STATE_CHANGED = 0x2;
		public const uint SCARD_STATE_UNKNOWN = 0x4;
		public const uint SCARD_STATE_UNAVAILABLE = 0x8;
		public const uint SCARD_STATE_EMPTY = 0x10;
		public const uint SCARD_STATE_PRESENT = 0x20;
		public const uint SCARD_STATE_ATRMATCH = 0x40;
		public const uint SCARD_STATE_EXCLUSIVE = 0x80;
		public const uint SCARD_STATE_INUSE = 0x100;
		public const uint SCARD_STATE_MUTE = 0x200;
		public const uint SCARD_STATE_UNPOWERED = 0x400;
		
#endregion Constants for PC/SC
		
		

		
		
		
#region Local vars
		
		private IntPtr nContext = IntPtr.Zero;			// Card reader context handle - DWORD
		private IntPtr nCard = IntPtr.Zero;				// Connection handle - DWORD
		private IntPtr nActiveProtocol = new IntPtr(0);	// T0/T1
		private int nNotUsed1 = 0;
		private int nNotUsed2 = 0;
		private IntPtr readerNameLen = new IntPtr(0);   // Selected reader name len		
		private int cardProtocol = 0; 
		private byte[] atrValue;
		private IntPtr atrLen = new IntPtr(33);
		private cEncoding utilityObj = new cEncoding();	
	
		private string selectedReader = "";				// Selected reader name
		
#endregion Local vars
		
		
		
		
		
		
#region Properties
		
		/// <summary>
		/// Get selected reader name
		/// </summary>
		public string SelectedReader
		{
			get {return selectedReader;}
		}
		
		
#endregion Properties
		
		
		
		
		
		

		/// <summary>
		/// Constructor
		/// </summary>
		public Pcsc()
		{
		}
	
		
		
		
		
#region Public methods
		
		
		
		
		
		/// <summary>
		/// Establishes the resource manager context (the scope)
		/// within which database operations are performed
		/// </summary>
		/// <returns>
		/// Empty or error description
		/// </returns>
		public string EstablishContext()
		{
	
			// Delete previous context
			if (nContext.ToInt64() > 0)
			{
				ReleaseContext();
			}
	
			// Call external function
			int ret = SCardEstablishContext(SCARD_SCOPE_SYSTEM, nNotUsed1, nNotUsed2, ref nContext);
			
			if (ret != 0)
			{
				// Return error description
				return parseError(ret);
			}
				
			return "";
		}
	
	
		
		
		
		/// <summary>
		/// This function closes an established resource manager context, 
		/// freeing any resources allocated under that context, 
		/// including SCARDHANDLE objects and memory allocated 
		/// using the SCARD_AUTOALLOCATE length designator.
		/// </summary>
		/// <returns>
		/// Empty or error description
		/// </returns>
		public string ReleaseContext()
		{			
			if (nContext.ToInt64() == 0)
			{
				// Exit with context already deleted
				return "";
			}
			
			
			// Call external function
			int ret = SCardReleaseContext(nContext);
			
			if (ret != 0)
			{
				// Return error description
				return parseError(ret);
			}
			
			nContext = IntPtr.Zero;
			
			return "";
		}
	
		
		
		
		
		
		
		
		/// <summary>
		/// This function provides the list of readers within a 
		/// set of named reader groups, eliminating duplicates.
		/// </summary>
		/// <param name="readers">
		/// Array of pcsc readers
		/// </param>
		/// <returns>
		/// Empty or error description
		/// </returns>
		public string ListReaders(out string[] readers)
		{
	
			int ret;
			List<byte> lreaders = new List<byte>();		
			List<string> lstreaders = new List<string>();		
			byte[] readersBuf = new byte[0];
			IntPtr readersBufLen = IntPtr.Zero;
			
			readers = new string[0];
			
			//First time to retrieve the len of buffer for readers name
			ret = SCardListReaders( nContext, null, readersBuf, out readersBufLen);
	
			// Redim buffer
			readersBuf = new byte[readersBufLen.ToInt32()];
			
			// Second time to retrieve readers name
			ret = SCardListReaders(nContext, null, readersBuf, out readersBufLen);
			
			if (ret != 0)
			{
				// Return error description
				return parseError(ret);
			}
			
			
			if (readersBuf.Length < 5)
			{
				// If there aren't readers, return 
				return "";
			}

			// Loop to detect readers
			for (int j=0; j<readersBuf.Length; j++)
			{
				
				if (readersBuf[j] != 0x00)
				{
					// Add byte to byte list
					lreaders.Add(readersBuf[j]);					
				}
				else
				{
					// null byte
					if (readersBuf[j-1] == 0x00)
					{
						break;
					}
					
					// Update readers list
					lstreaders.Add(ASCIIEncoding.ASCII.GetString(lreaders.ToArray()));
					lreaders = new List<byte>();
					
				}
				
			}
			
			
			readers = lstreaders.ToArray();
			return "";			
	
		}
	    
		
	
		/// <summary>
		/// Establishes a connection (using a specific resource manager context) 
		/// between the calling application and a smart card contained 
		/// by a specific reader. If no card exists in the specified 
		/// reader, an error is returned.
		/// </summary>
		/// <param name="readerName">
		/// Reader name to use
		/// </param>
		/// <param name="response">
		/// ATR obtained after SCardConnect and SCardStatus
		/// </param>
		/// <param name="protocol">
		/// Accepted protocol
		/// </param>
		/// <param name="sharing">
		/// Sharing communication
		/// </param>		
		/// <returns>
		/// Empty or error description
		/// </returns>
		public string Connect(string readerName, 
		                  ref string response,
		                      SCARD_PROTOCOL protocol,
		                      SCARD_SHARE sharing)
		{
			// Disconnect before connect
			if (nCard.ToInt64() != 0)
			{
				SCardDisconnect(nCard, SCARD_UNPOWER_CARD);
			}

			
			int ret = SCardConnect(nContext, 
			                       readerName, 
			                       (uint)sharing, 
			                       (uint)protocol, 
			                   ref nCard, 
			                   ref nActiveProtocol);
			
			if (ret != 0)
			{
				// Return error description
				return parseError(ret);				
			}
			
			
			// Set reader to use
			selectedReader = readerName;
			
			// Get ATR
			string resp = "";
			string retStatus = ReaderStatus(ref resp);
			if (retStatus != "")
			{
				return retStatus;
			}

			response = resp;
			
			return "";
		}
		
	
		
		
		/// <summary>
		/// Terminates a connection previously opened between
		/// the calling application and a smart card in the target reader.
		/// </summary>
		public void Disconnect()
		{	
			SCardDisconnect(nCard, SCARD_UNPOWER_CARD);
			nCard = IntPtr.Zero;
			return;
		}
	
		
		
		
		/// <summary>
		/// Return reader status
		/// </summary>
		/// <param name="readerName">
		/// Reader name to get status
		/// </param>
		/// <param name="readerStatus">
		/// Reader status
		/// </param>
		/// <returns>
		/// Empty or error description
		/// </returns>
		public string ReaderGetStatusChange(string readerName, ref uint readerStatus)
		{
		    SCARD_READERSTATE rState = new SCARD_READERSTATE();
		    rState.szReader = readerName + Convert.ToChar(0);
		    
		    int ret = SCardGetStatusChange(nContext, 50, ref rState, 1);
			
		    if (ret != 0)
			{
				// Return error description
				return parseError(ret);
			}
		    
		    readerStatus = rState.dwEventState;
			return "";
		}
		
		
		
		
		/// <summary>
		/// Provides the current status of a smart card in a reader. 
		/// You can call it any time after a successful call to Connect and
		/// before a successful call to Disconnect. 
		/// It does not affect the state of the reader or reader driver.
		/// </summary>
		/// <param name="response">
		/// SmartCard status
		/// </param>
		/// <returns>
		/// Empty or error description
		/// </returns>
		public string ReaderStatus(ref string response)
		{
			// set empty byte array
			atrValue = new byte[33];

			byte[] retRName = new byte[64];
			readerNameLen = new IntPtr(64);			
			atrLen = new IntPtr(33);
			cardProtocol = 0;
			int readerState = 0; 
				
			int ret = SCardStatus(nCard, 
			                      retRName, 
			                  ref readerNameLen, 
			                  out readerState, 
			                  out cardProtocol, 
			                      atrValue, 
			                  ref atrLen);
			
			
			
			if (ret != 0)
			{
				// reset byte array to returned length and retry				
				atrValue = new byte[atrLen.ToInt32()];
				retRName = new byte[readerNameLen.ToInt32()];
				
				ret = SCardStatus(nCard, 
				                  retRName, 
				              ref readerNameLen, 
				              out readerState, 
				              out cardProtocol, 
				                  atrValue, 
				              ref atrLen);

			}
			
			
			if (ret != 0)
			{
				// Return error description
				return parseError(ret);
			}
			
			// Extract ATR value
			response = utilityObj.GetHexFromBytes(atrValue, 0, atrLen.ToInt32());
			
			return "";
		}
	
		
		
		
		
		/// <summary>
		/// Sends a service request to the smart card and
		/// expects to receive data back from the card.
		/// </summary>
		/// <param name="cardCommand">
		/// Command to send (in hexadecimal mode)
		/// </param>
		/// <param name="response">
		/// Response from Smart Card (in hexadecimal mode)
		/// </param>
		/// <returns>
		/// Empty or error description
		/// </returns>
		public string Transmit(string cardCommand, ref string response )
		{
			IntPtr retCommandLen = new IntPtr(261);
	
			cardCommand = cardCommand.Trim();
			
			// Set input command byte array
			byte[] inCommandByte = utilityObj.GetBytesFromHex(cardCommand);
			
			byte[] outCommandByte = new byte[261];
			
			// Prepare structures
			SCARD_IO_REQUEST pioSend = new SCARD_IO_REQUEST();
			SCARD_IO_REQUEST pioRecv = new SCARD_IO_REQUEST();
			pioSend.dwProtocol = (uint)cardProtocol;			
			pioSend.cbPciLength = (uint)8;
			pioRecv.dwProtocol = (uint)cardProtocol;
			pioRecv.cbPciLength = 0;
			
			int ret = SCardTransmit(nCard, 
			                    ref pioSend, 
			                        inCommandByte, 		                    
			                        inCommandByte.Length, 
			                        IntPtr.Zero,
			                        outCommandByte, 
			                    out retCommandLen);
			
			if (ret != 0)
			{	
				// Return error description
				return parseError(ret);
			}
			
			// Extract response
			response = utilityObj.GetHexFromBytes(outCommandByte, 0, retCommandLen.ToInt32()).Trim();
			
			return "";
		}
	
	
		
		
		
		/// <summary>
		/// Return error description from error code
		/// </summary>
		/// <param name="errorCode">
		/// Error code
		/// </param>
		/// <returns>
		/// Return error description
		/// </returns>
		private string parseError(int errorCode)
		{
			string hexError = string.Format("{0:x2}", errorCode);
			hexError = hexError.ToUpper();
			
			if (hexError == "80100001") {hexError += " - SCARD_F_INTERNAL_ERROR"; }
			if (hexError == "80100002") {hexError += " - SCARD_E_CANCELLED"; }
			if (hexError == "80100003") {hexError += " - SCARD_E_INVALID_HANDLE"; }
			if (hexError == "80100004") {hexError += " - SCARD_E_INVALID_PARAMETER"; }
			if (hexError == "80100005") {hexError += " - SCARD_E_INVALID_TARGET"; }
			if (hexError == "80100006") {hexError += " - SCARD_E_NO_MEMORY"; }
			if (hexError == "80100007") {hexError += " - SCARD_F_WAITED_TOO_LONG"; }
			if (hexError == "80100008") {hexError += " - SCARD_E_INSUFFICIENT_BUFFER"; }
			if (hexError == "80100009") {hexError += " - SCARD_E_UNKNOWN_READER"; }
			if (hexError == "8010000A") {hexError += " - SCARD_E_TIMEOUT"; }
			if (hexError == "8010000B") {hexError += " - SCARD_E_SHARING_VIOLATION"; }
			if (hexError == "8010000C") {hexError += " - SCARD_E_NO_SMARTCARD"; }
			if (hexError == "8010000D") {hexError += " - SCARD_E_UNKNOWN_CARD"; }
			if (hexError == "8010000E") {hexError += " - SCARD_E_CANT_DISPOSE"; }
			if (hexError == "8010000F") {hexError += " - SCARD_E_PROTO_MISMATCH"; }
			if (hexError == "80100010") {hexError += " - SCARD_E_NOT_READY"; }
			if (hexError == "80100011") {hexError += " - SCARD_E_INVALID_VALUE"; }
			if (hexError == "80100012") {hexError += " - SCARD_E_SYSTEM_CANCELLED"; }
			if (hexError == "80100013") {hexError += " - SCARD_F_COMM_ERROR"; }
			if (hexError == "80100014") {hexError += " - SCARD_F_UNKNOWN_ERROR"; }
			if (hexError == "80100015") {hexError += " - SCARD_E_INVALID_ATR"; }
			if (hexError == "80100016") {hexError += " - SCARD_E_NOT_TRANSACTED"; }
			if (hexError == "80100017") {hexError += " - SCARD_E_READER_UNAVAILABLE"; }
			if (hexError == "80100018") {hexError += " - SCARD_P_SHUTDOWN"; }
			if (hexError == "80100019") {hexError += " - SCARD_E_PCI_TOO_SMALL"; }
			if (hexError == "8010001A") {hexError += " - SCARD_E_READER_UNSUPPORTED"; }
			if (hexError == "8010001B") {hexError += " - SCARD_E_DUPLICATE_READER"; }
			if (hexError == "8010001C") {hexError += " - SCARD_E_CARD_UNSUPPORTED"; }
			if (hexError == "8010001D") {hexError += " - SCARD_E_NO_SERVICE"; }
			if (hexError == "8010001E") {hexError += " - SCARD_E_SERVICE_STOPPED"; }
			if (hexError == "8010001F") {hexError += " - SCARD_E_UNEXPECTED"; }
			if (hexError == "80100020") {hexError += " - SCARD_E_ICC_INSTALLATION"; }
			if (hexError == "80100021") {hexError += " - SCARD_E_ICC_CREATEORDER"; }
			if (hexError == "80100022") {hexError += " - SCARD_E_UNSUPPORTED_FEATURE"; }
			if (hexError == "80100023") {hexError += " - SCARD_E_DIR_NOT_FOUND"; }
			if (hexError == "80100024") {hexError += " - SCARD_E_FILE_NOT_FOUND"; }
			if (hexError == "80100025") {hexError += " - SCARD_E_NO_DIR"; }
			if (hexError == "80100026") {hexError += " - SCARD_E_NO_FILE"; }
			if (hexError == "80100027") {hexError += " - SCARD_E_NO_ACCESS"; }
			if (hexError == "80100028") {hexError += " - SCARD_E_WRITE_TOO_MANY"; }
			if (hexError == "80100029") {hexError += " - SCARD_E_BAD_SEEK"; }
			if (hexError == "8010002A") {hexError += " - SCARD_E_INVALID_CHV"; }
			if (hexError == "8010002B") {hexError += " - SCARD_E_UNKNOWN_RES_MNG"; }
			if (hexError == "8010002C") {hexError += " - SCARD_E_NO_SUCH_CERTIFICATE"; }
			if (hexError == "8010002D") {hexError += " - SCARD_E_CERTIFICATE_UNAVAILABLE"; }
			if (hexError == "8010002E") {hexError += " - SCARD_E_NO_READERS_AVAILABLE"; }
			if (hexError == "8010002F") {hexError += " - SCARD_E_COMM_DATA_LOST"; }
			
			if (hexError == "80100065") {hexError += " - SCARD_W_UNSUPPORTED_CARD"; }
			if (hexError == "80100066") {hexError += " - SCARD_W_UNRESPONSIVE_CARD"; }
			if (hexError == "80100067") {hexError += " - SCARD_W_UNPOWERED_CARD"; }
			if (hexError == "80100068") {hexError += " - SCARD_W_RESET_CARD"; }
			if (hexError == "80100069") {hexError += " - SCARD_W_REMOVED_CARD"; }
			if (hexError == "8010006A") {hexError += " - SCARD_W_SECURITY_VIOLATION"; }
			if (hexError == "8010006B") {hexError += " - SCARD_W_WRONG_CHV"; }
			if (hexError == "8010006C") {hexError += " - SCARD_W_CHV_BLOCKED"; }
			if (hexError == "8010006D") {hexError += " - SCARD_W_EOF"; }
			if (hexError == "8010006E") {hexError += " - SCARD_W_CANCELLED_BY_USER"; }
			
			
			return hexError;
		}
	
		
		
		
		
#endregion Public methods
		
		
		
		
		
		
		
		
		
		
		
		
	}

}