using System;
using System.Xml;


namespace comex
{
	public class SettingsManager
	{
        private XmlDocument doc;
		private string fileName;
		private string rootName;

		
		
		/// <summary>
		/// Construct
		/// </summary>
		public SettingsManager(string fileName, string rootName)
		{
			this.fileName = fileName;
			this.rootName = rootName;

			doc = new XmlDocument();

			try
			{
				doc.Load(fileName);
			}
			catch(System.IO.FileNotFoundException)
			{
				CreateSettingsDocument();
			}
		}

		
		
		
		public SettingsManager(string fileName) : this(fileName, "Settings")
		{     
		}

		
		
		
		
		
		/// <summary>
		/// Deletes all entries of a section
		/// </summary>
		public void ClearSection(string section)
		{
			XmlNode root = doc.DocumentElement;
			XmlNode s = root.SelectSingleNode('/' + rootName + '/' + section);

			if(s == null)
				return;  //not found

			s.RemoveAll();
		}

		
		
		
		
		/// <summary>
		/// initializes a new settings file with the XML version and the root node
		/// </summary>
		protected void CreateSettingsDocument()
		{
			doc.AppendChild(doc.CreateXmlDeclaration("1.0", null, null));
			doc.AppendChild(doc.CreateElement(rootName));
			
			WriteString("SERIAL", "PortName", GlobalObj.SerialPortsName[0]);
			WriteInt("SERIAL", "PortSpeed", 9600);
			WriteInt("SERIAL", "PortSpeedReset", 9600);
			WriteInt("SERIAL", "DataBits", 8);
			WriteInt("SERIAL", "StopBits", 2);
			WriteString("SERIAL", "Parity", "Odd");
			WriteBool("SERIAL", "IsDirectConvention", true);
			
			Flush();			
		}

		
		
		
		
		public string Flush()
		{
			try
			{
				doc.Save(fileName);
			}
			catch(Exception ex)
			{
            	return ex.Message;
			}
			
			return"";
		}

		
		
		
		
		/// <summary>
		/// removes a section and all its entries
		/// </summary>
		public void RemoveSection(string section)
		{
			XmlNode root = doc.DocumentElement;
			XmlNode s = root.SelectSingleNode('/' + rootName + '/' + section);

			if(s != null)
				root.RemoveChild(s);
		}

		
		
		
		
		public void Save()
		{
			Flush();
		}


		
		
		
		
		
		
		
		
		
		
		#region Read methods

		
		
		public bool ReadBool(string section, string name, bool defaultValue)
		{
			string s = ReadString(section, name, "");

			if(s == Boolean.TrueString)
				return true;
			else if(s == Boolean.FalseString)
				return false;
			else
				return defaultValue;
		}

		
		
		public DateTime ReadDateTime(string section, string name, DateTime defaultValue)
		{
			string s = ReadString(section, name, "");

			if(s == "")
				return defaultValue;
			else
			{
				try
				{
					DateTime dt = Convert.ToDateTime(s);
					return dt;
				}
				catch(FormatException)
				{
					return defaultValue;
				}
			}
		}

		
		
		public double ReadDouble(string section, string name, double defaultValue)
		{
			string s = ReadString(section, name, "");

			if(s == "")
				return defaultValue;
			else
			{
				try
				{
					double d = Convert.ToDouble(s);
					return d;
				}
				catch(FormatException)
				{
					return defaultValue;
				}
			}
		}

		
		
		public float ReadFloat(string section, string name, float defaultValue)
		{
			string s = ReadString(section, name, "");

			if(s == "")
				return defaultValue;
			else
			{
				try
				{
					float f = Convert.ToSingle(s);
					return f;
				}
				catch(FormatException)
				{
					return defaultValue;
				}
			}
		}

		
		
		public int ReadInt(string section, string name, int defaultValue)
		{
			string s = ReadString(section, name, "");

			if(s == "")
				return defaultValue;
			else
			{
				try
				{
					int n = Convert.ToInt32(s);
					return n;
				}
				catch(FormatException)
				{
					return defaultValue;
				}
			}
		}

		
		
		public long ReadLong(string section, string name, long defaultValue)
		{
			string s = ReadString(section, name, "");

			if(s == "")
				return defaultValue;
			else
			{
				try
				{
					long l = Convert.ToInt64(s);
					return l;
				}
				catch(FormatException)
				{
					return defaultValue;
				}
			}
		}

		
		
		public short ReadShort(string section, string name, short defaultValue)
		{
			string s = ReadString(section, name, "");

			if(s == "")
				return defaultValue;
			else
			{
				try
				{
					short n = Convert.ToInt16(s);
					return n;
				}
				catch(FormatException)
				{
					return defaultValue;
				}
			}
		}

		
		
		public string ReadString(string section, string name, string defaultValue)
		{
			XmlNode root = doc.DocumentElement;
			XmlNode s = root.SelectSingleNode('/' + rootName + '/' + section);

			if(s == null)
				return defaultValue;  //not found

			XmlNode n = s.SelectSingleNode(name);

			if(n == null)
				return defaultValue;  //not found

			XmlAttributeCollection attrs = n.Attributes;

			foreach(XmlAttribute attr in attrs)
			{
				if(attr.Name == "value")
					return attr.Value;
			}

			return defaultValue;
		}

		
		
		public uint ReadUInt(string section, string name, uint defaultValue)
		{
			string s = ReadString(section, name, "");

			if(s == "")
				return defaultValue;
			else
			{
				try
				{
					uint n = Convert.ToUInt32(s);
					return n;
				}
				catch(FormatException)
				{
					return defaultValue;
				}
			}
		}

		
		
		public ulong ReadULong(string section, string name, ulong defaultValue)
		{
			string s = ReadString(section, name, "");

			if(s == "")
				return defaultValue;
			else
			{
				try
				{
					ulong l = Convert.ToUInt64(s);
					return l;
				}
				catch(FormatException)
				{
					return defaultValue;
				}
			}
		}

		
		
		public ushort ReadUShort(string section, string name, ushort defaultValue)
		{
			string s = ReadString(section, name, "");

			if(s == "")
				return defaultValue;
			else
			{
				try
				{
					ushort n = Convert.ToUInt16(s);
					return n;
				}
				catch(FormatException)
				{
					return defaultValue;
				}
			}
		}

		
		
		#endregion


		
		
		
		#region Write methods

		public void WriteBool(string section, string name, bool value)
		{
			WriteString(section, name, value.ToString());
		}

		public void WriteDateTime(string section, string name, DateTime value)
		{
			WriteString(section, name, value.ToString());
		}

		public void WriteDouble(string section, string name, double value)
		{
			WriteString(section, name, value.ToString());
		}

		public void WriteFloat(string section, string name, float value)
		{
			WriteString(section, name, value.ToString());
		}

		public void WriteInt(string section, string name, int value)
		{
			WriteString(section, name, value.ToString());
		}

		public void WriteLong(string section, string name, long value)
		{
			WriteString(section, name, value.ToString());
		}

		public void WriteShort(string section, string name, short value)
		{
			WriteString(section, name, value.ToString());
		}

		public void WriteString(string section, string name, string value)
		{
			XmlNode root = doc.DocumentElement;
			XmlNode s = root.SelectSingleNode('/' + rootName + '/' + section);

			if(s == null)
				s =	root.AppendChild(doc.CreateElement(section));

			XmlNode n = s.SelectSingleNode(name);

			if(n == null)
				n = s.AppendChild(doc.CreateElement(name));

			XmlAttribute attr = ((XmlElement)n).SetAttributeNode("value", "");
			attr.Value = value; 
		}

		public void WriteUInt(string section, string name, uint value)
		{
			WriteString(section, name, value.ToString());
		}

        public void WriteULong(string section, string name, ulong value)
		{
			WriteString(section, name, value.ToString());
		}

		public void WriteUShort(string section, string name, ushort value)
		{
			WriteString(section, name, value.ToString());
		}

		#endregion

		
		
		
		
		
	}
}

