using System;
namespace comexbase
{
	public static class SerialSettings
	{

		public static int PortSpeed {get; set;}
		public static int PortSpeedReset {get; set;}
		public static int DataBits {get; set;}
		public static int StopBits {get; set;}
		public static string Parity {get; set;}		
		public static bool IsDirectConvention {get; set;}
		public static int ReadTimeout {get; set;}
		
	}
}

