using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using log4net;
using Utility;



namespace comexbase
{
	
	/// <summary>
	/// Manage Pcsc readers
	/// </summary>
	public partial class PcscReader:IReader, IDisposable
	{
		
		
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
		                                         uint Disposition);
	
		
	
		[DllImport("winscard")]
		private static extern int SCardStatus(IntPtr hCard, 
		                                      byte[] ReaderName, 
		                                  ref IntPtr RLen, 
		                                  out uint State, 
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
		
		
		
		
	
		#region SCARD Constants
		
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
		
		private enum SCARD_DISPOSITION: uint
		{
			SCARD_LEAVE_CARD = 0,
			SCARD_RESET_CARD = 1,
			SCARD_UNPOWER_CARD = 2,
			SCARD_EJECT_CARD = 3,			
		}
		
		private const int SCARD_LEAVE_CARD = 0;
		private const int SCARD_UNPOWER_CARD = 2;
	
		private enum SCARD_STATE: uint
		{
			SCARD_STATE_UNAWARE = 0x0,
		    SCARD_STATE_IGNORE = 0x1,
			SCARD_STATE_CHANGED = 0x2,
			SCARD_STATE_UNKNOWN = 0x4,
			SCARD_STATE_UNAVAILABLE = 0x8,
			SCARD_STATE_EMPTY = 0x10,
			SCARD_STATE_PRESENT = 0x20,
			SCARD_STATE_ATRMATCH = 0x40,
			SCARD_STATE_EXCLUSIVE = 0x80,
			SCARD_STATE_INUSE = 0x100,
			SCARD_STATE_MUTE = 0x200,
			SCARD_STATE_UNPOWERED = 0x400,
		}
		
		
		#endregion SCARD Constants
		
		
		
		
		
		
  		// Attributes		
		
		private IntPtr nContext = IntPtr.Zero;			// Card reader context handle - DWORD
		private IntPtr nCard = IntPtr.Zero;				// Connection handle - DWORD
		private IntPtr nActiveProtocol = new IntPtr(0);	// T0/T1
		private int nNotUsed1 = 0;
		private int nNotUsed2 = 0;
		private string selectedReader = "";				// Selected reader name
		private IntPtr readerNameLen = new IntPtr(0);   // Selected reader name len		
		private int cardProtocol = 0; 
		private byte[] atrValue;						// ATR content
		private IntPtr atrLen = new IntPtr(33);			// ATR length
		private int ret = 0;
		
		
		private cEncoding utilityObj = new cEncoding();	
			
		// Log4Net object
        private static readonly ILog log = LogManager.GetLogger(typeof(PcscReader));
		
		
		
		
		
		/// <summary>
		/// Initializes a new instance of PcscReader class.
		/// </summary>
		public PcscReader ()
		{
			// Try to create PCSC context
			if (CreateContext() != "")
			{
				// Error detected
				return;
			}
			
			UpdateListReaders();
			
		}
		
		
		
		

		#region IDisposable interface
		
		public void Dispose ()
		{
			// Release PCSC context
			ReleaseContext();
		}		
		
		#endregion IDisposable interface

		
		
		
		
		#region Private methods
		
		
		
		
		/// <summary>
		/// Updates list of readers
		/// </summary>
		private void UpdateListReaders()
		{
			byte[] readersBuf = new byte[0];
			IntPtr readersBufLen = IntPtr.Zero;
			List<byte> lreaders = new List<byte>();	
			List<string> lstreaders = new List<string>();	
			
			//First time to retrieve the len of buffer for readers name
			ret = SCardListReaders( nContext, null, readersBuf, out readersBufLen);
	
			// Redim buffer
			readersBuf = new byte[readersBufLen.ToInt32()];
			
			// Second time to retrieve readers name
			ret = SCardListReaders(nContext, null, readersBuf, out readersBufLen);
			
			if (ret != 0)
			{
				// Error detected
				log.Error("PcscReader::UpdateListReaders: SCardListReaders " + parseError(ret));
				return;
			}
			
			if (readersBuf.Length < 5)
			{
				// No readers founded
				return;
			}
			
			// Loop to detect readers
			for (int j=0; j<readersBuf.Length; j++)
			{
				// check for first null byte
				if (readersBuf[j] != 0x00)
				{
					// Add byte to byte list
					lreaders.Add(readersBuf[j]);					
				}
				else
				{
					//check for second null byte (end of list)
					if (readersBuf[j-1] == 0x00)
					{
						break;
					}
					
					// Update readers list
					lstreaders.Add(ASCIIEncoding.ASCII.GetString(lreaders.ToArray()));
					lreaders = new List<byte>();					
				}
				
			}
			
			// update public list
			readers = lstreaders;
			
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
	
		
		
		
		
		/// <summary>
		/// Get ATR and smartcard status
		/// </summary>
		/// <returns>
		/// Empty or error message
		/// </returns>
		private string ReaderStatus(ref string response)
		{
			// set empty byte array
			atrValue = new byte[33];

			byte[] retRName = new byte[64];
			readerNameLen = new IntPtr(64);			
			atrLen = new IntPtr(33);
			cardProtocol = 0;
			uint readerState = 0; 
			
			// firts time to set sizes
			ret = SCardStatus(nCard, retRName, ref readerNameLen, out readerState, 
			                  out cardProtocol, atrValue, ref atrLen);
			
			if (ret != 0)
			{
				// reset byte array to returned length and retry			
				atrValue = new byte[atrLen.ToInt32()];
				retRName = new byte[readerNameLen.ToInt32()];
				
				ret = SCardStatus(nCard, retRName, ref readerNameLen, out readerState, 
				              out cardProtocol, atrValue, ref atrLen);
			}
			
			
			if (ret != 0)
			{
				// Error detected
				log.Error("PcscReader::ReaderStatus: SCardStatus " +  parseError(ret));
				return "PcscReader::ReaderStatus: SCardStatus " +  parseError(ret);
			}
			
			// Extract ATR value
			response = utilityObj.GetHexFromBytes(atrValue, 0, atrLen.ToInt32());
			
			return "";
		}
		
		
		
		
		
		/// <summary>
		/// Release PCSC context
		/// </summary>
		private void ReleaseContext()
		{
			if (nContext.ToInt64() != 0)
			{
				// Release PCSC context
				ret = SCardReleaseContext(nContext);
				log.Error("PcscReader::ReleaseContext: SCardReleaseContext " + parseError(ret));
				nContext = IntPtr.Zero;
			}
		}
		
		
		
		
		/// <summary>
		/// Create PCSC context
		/// </summary>
		private string CreateContext()
		{
			// Create PCSC context
			int ret = SCardEstablishContext(SCARD_SCOPE_SYSTEM, nNotUsed1, nNotUsed2, ref nContext);
			
			if (ret != 0)
			{
				// Error detected
				nContext = IntPtr.Zero;
				log.Error("PcscReader::CreateContext: SCardEstablishContext " + parseError(ret));
				return parseError(ret);
			}
			
			log.Debug("PcscReader::CreateContext: SCardEstablishContext " + parseError(ret));
			return "";
		}
		
		
		#endregion Private methods
		
		
	}
}

