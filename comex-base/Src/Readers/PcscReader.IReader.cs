using System;

namespace comexbase
{
	
	public partial class PcscReader: IReader
	{

		
		
		

  		// IReader interface
		
		
		void IReader.SelectReader (string readerName)
		{
			throw new NotImplementedException ();
		}

		string IReader.AnswerToReset (ref string response)
		{
			throw new NotImplementedException ();
		}

		string IReader.SendReceive (string command, ref string response)
		{
			throw new NotImplementedException ();
		}

		void IReader.CloseConnection ()
		{
			throw new NotImplementedException ();
		}

		string IReader.TypeName {
			get {
				throw new NotImplementedException ();
			}
		}

		System.Collections.Generic.List<string> IReader.Readers {
			get {
				throw new NotImplementedException ();
			}
		}
		
		
  		// IReader interface end
		
		
		
		
		
		
		
		
		
	}
}

