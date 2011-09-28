using System;
using System.Collections.Generic;

namespace comexbase
{
	/// <summary>
	/// Interface for smartcard readers
	/// </summary>
	public interface IReader
	{
		
		/// <summary>
		/// Get or Set reader to use
		/// </summary>
		string SelectedReader {get; set;}
		
		
		/// <summary>
		/// Gets name of readers typology
		/// </summary>
		string TypeName {get;}
		
		
		/// <summary>
		/// Gets readers name list for specific typology
		/// </summary>
		List<string> Readers {get;}

		
		/// <summary>
		/// Answers to reset get bytes that smartcard send after power on.
		/// </summary>
		/// <returns>
		/// Empty or error
		/// </returns>
		/// <param name='response'>
		/// Bytes sended from smartcard
		/// </param>
		string AnswerToReset(ref string response);
		
		
		/// <summary>
		/// Exchange data with smartcard
		/// </summary>
		/// <returns>
		/// Empty or error
		/// </returns>
		/// <param name='command'>
		/// Command bytes
		/// </param>
		/// <param name='response'>
		/// Response bytes
		/// </param>
		string SendReceive(string command, ref string response);
		
		
		/// <summary>
		/// Closes connection with reader
		/// </summary>
		void CloseConnection();
		
		
	}
}

