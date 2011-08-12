using System;
using System.Text;

namespace Utility
{
	/// <summary>
	/// Class to manage bytes array and hexadecimal strings.
	/// </summary>
	public class cEncoding
	{

		/// <summary>
		/// Create an instance of object
		/// </summary>
		public cEncoding()
		{
		}
		
		
		
		
		/// <summary>
		///	Function to obtain an hexadecimal string (0x00 format)
		/// from a bytes array.
		/// </summary>
		/// <param name="inBytes">Bytes array to encoding</param>
		public string GetHexFromBytes(byte[] inBytes)
		{
			if (inBytes.Length == 0 )
			{
				return "";
			}
			
			return GetHexFromBytes(inBytes, 0, inBytes.Length);
		
		}
		
		
		
		
		/// <summary>
		///	Function to obtain an hexadecimal string (0x00 format)
		/// from a bytes array.
		/// </summary>
		/// <param name="inBytes">Bytes array to encoding</param>
		/// <param name="offSet">OffSet to start encoding</param>
		/// <param name="len">Encoding length</param>
		public string GetHexFromBytes(byte[] inBytes, int offSet, int len)
		{
			string tmpHexOut = "";
			
			if (inBytes.Length < (offSet + len) )
			{
				return "";
			}
			
			for (int j=offSet; j<(offSet + len); j++)
			{
				tmpHexOut = tmpHexOut + inBytes[j].ToString("X2");
			}
			
			return tmpHexOut;
		
		}

		
		
		
		
		/// <summary>
		///	Function to obtain an hexadecimal string (0x00 format)
		/// from an Hexadecimal string
		/// </summary>
		/// <param name="inHex">string hexadecimal to swap</param>
		public string SwapHex(string inHex)
		{
			string tmpHexOut = "";
			
			if (inHex.Length == 0 )
			{
				return "";
			}
			
			for (int j=(inHex.Length-2); j>=0; j=j-2)
			{
				tmpHexOut = tmpHexOut + inHex.Substring(j,2) ;
			}
			
			return tmpHexOut;
		
		}

		
		
		/// <summary>
		///	Function to obtain an hexadecimal string (0x00 format)
		/// from an Ascii string
		/// </summary>
		public string GetHexFromAscii(string inAscii)
		{
			return GetHexFromBytes(ASCIIEncoding.ASCII.GetBytes(inAscii));
		}

		

		/// <summary>
		///	Function to obtain an ASCII string 
		/// from an hexadecimal string.
		/// </summary>
		public string GetAsciiFromHex(string inHexString)
		{
			return ASCIIEncoding.ASCII.GetString(GetBytesFromHex(inHexString));
		
		}
		

		
		
		/// <summary>
		///	Function to obtain an Integer value from Bytes array
		/// </summary>
		public int GetIntFromBytes(byte[] inByteArray, int startOffSet, int length)
		{
			int j, n;
			n = 0;
			
			if (inByteArray.Length == 0 )
			{
				return 0;
			}
			
			for (j=startOffSet; j<(length + startOffSet); j++)
			{
				n = n + (inByteArray[j]<<( 8*(j-startOffSet) ) );
			}
				
			return n;
		
		}
		
		


		/// <summary>
		///	Function to obtain a Bytes array from an Hexadecimal string
		/// </summary>
		public byte[] GetBytesFromHex(string inData)
		{
			inData = inData.Trim();
			byte[] outByteArray = new byte[0];
			
			if (inData.Length == 0 )
			{
				return outByteArray;
			}
			
			outByteArray = new byte[inData.Length/2];
			
			for (int j=0; j<inData.Length-1; j+=2)
			{
				outByteArray[j/2] = Byte.Parse(inData.Substring(j,2) ,
				                               System.Globalization.NumberStyles.HexNumber);
			}
			
			return outByteArray;
		
		}

		
		

		/// <summary>
		///	Compare two array 
		/// </summary>
		public int CompareBytesArray(byte[] bigArray, byte[] smallArray, int offSet)
		{
			int len1 = bigArray.Length;
			int len2 = smallArray.Length;
			int k, j;
			bool okCompare=false;
			
			for (k=offSet; k<(len1-len2); k++)
			{
				okCompare=true;
				for (j=0; j<len2; j++)
				{
					if (bigArray[k+j] != smallArray[j])
					{
						okCompare=false;
						break;					
					}
					
				}
				
				if (okCompare==true)	
				{
					return k;
				}
			
			}
			
			return -1;
			
		
		}
		
		
	}
}
