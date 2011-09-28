using System;

namespace comexbase
{
	
	/// <summary>
	/// Manage Pcsc readers
	/// </summary>
	public partial class PcscReader: IReader
	{
		
		
		// DllImport section
		
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
		
		
		
		
		// DllImport section end
		
		
		
		
	
		// SCARD Structures section
				
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
				
				
		// SCARD Structures section end
		
		
		
		
	
		// SCARD Constants section
		
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
		
		
		// SCARD Constants section end
		
		
		
		
		
		
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
		
		private cEncoding utilityObj = new cEncoding();	
			
		
		
		
		
		
		/// <summary>
		/// Initializes a new instance of PcscReader class.
		/// </summary>
		public PcscReader ()
		{
		}
		
		
		
		

		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
	}
}

