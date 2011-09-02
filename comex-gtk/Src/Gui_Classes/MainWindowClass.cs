using System;
using System.Collections.Generic;
using System.IO;
using Glade;
using Gtk;
using Gdk;
using Pango;

using log4net;

using comexbase;


namespace comexgtk
{
	
	
	public partial class MainWindowClass
	{

		// Attributes
		private string retStr = "";
		private string ATR = "";
		private string command = "";
		private string response = "";
		private string commandFileName = "";
		private string commandFilePath = "";
		
		
		
		
		
		
		
		#region Private Methods
		
		
		
		
		

		
		/// <summary>
		/// Update selected reader
		/// </summary>
		private void UpdateSelectedReader(string reader)
		{
			GlobalObj.CloseConnection();
			GlobalObj.SelectedReader = reader;
			StatusBar.Push(1, GlobalObj.LMan.GetString("selreader") + ": " + reader);
			TxtATR.Text = "";
			TxtResp.Text = "";
			TxtCmd.Text = "";
		}
		
		
		
		
		
		
		/// <summary>
		/// Perform Power On
		/// </summary>
		private void GetATR()
		{
			TxtATR.Text = "";
			TxtResp.Text = "";
			TxtCmd.Text = "";
			MainClass.GtkWait();
			
			// Connect to smartcard
			retStr = GlobalObj.AnswerToReset(ref ATR);
			
			if (retStr != "")
			{
				// error on answer to reset
				log.Error(retStr);
				MainClass.ShowMessage("ERROR", retStr, MessageType.Error);
				return;
			}
			
			TxtATR.Text = ATR;
		}
		
		
		
		
		
		
		/// <summary>
		/// Perform exchange data with card
		/// </summary>
		private void ExchangeData()
		{
			
			if (!GlobalObj.IsPowered)
			{
				MainClass.ShowMessage("WARNING", GlobalObj.LMan.GetString("noinitreader"), MessageType.Warning);
				return;
			}

			command = TxtCmd.Text;
			TxtResp.Text = "";
			TxtResp.Show();
			MainClass.GtkWait();
			
			retStr = GlobalObj.SendReceive(command, ref response);
			
			if (retStr != "")
			{
				log.Error(retStr);
				MainClass.ShowMessage("ERROR", retStr, MessageType.Error);
				return;
			}
			
			TxtResp.Text = response;
		}
		
		
		
		
		
		
		/// <summary>
		/// Open command file
		/// </summary>
		private void OpenCommandFile()
		{
                        
            // New dialog for select command file 
            Gtk.FileChooserDialog FileBox = 
				new Gtk.FileChooserDialog(GlobalObj.LMan.GetString("selectfile"), 
                                          MainWindow,
                                          FileChooserAction.Open,
				                          GlobalObj.LMan.GetString("cancellbl"),
                                          Gtk.ResponseType.Cancel,
				                          GlobalObj.LMan.GetString("openlbl"),
                                          Gtk.ResponseType.Accept);
            
            // Filter to use only comex files
            Gtk.FileFilter myFilter = new Gtk.FileFilter(); 
            myFilter.AddPattern("*.comex");
            myFilter.Name = "comex project files (*.comex)";
            FileBox.AddFilter(myFilter);
            
            // Manage result of dialog box
            FileBox.Icon = Gdk.Pixbuf.LoadFromResource("comex_256.png");
            int retFileBox = FileBox.Run();
            if ((ResponseType)retFileBox == Gtk.ResponseType.Accept)
            {       
                // path of a right file returned
                commandFilePath = FileBox.Filename;				
				commandFileName = System.IO.Path.GetFileNameWithoutExtension(commandFilePath);
				log.Debug("file selected: " + commandFilePath);
				
				UpdateGuiForCommandFile();
			
				
				
                FileBox.Destroy();
                FileBox.Dispose();                              
            }
            else
            {
                // nothing returned
                FileBox.Destroy();
                FileBox.Dispose();
                return;
            }
        
		}
		

		
		
		
		/// <summary>
		/// Close command file
		/// </summary>
		private void CloseCommandFile()
		{
			commandFileName = "";
			commandFilePath = "";
			
			UpdateGuiForCommandFile();
		}

		
		
		
		
		
		private void UpdateGuiForCommandFile()
		{
			if (commandFileName != "")
			{
				
				if (!UpdateFileAreaFromFile(commandFilePath))
				{
					// error detected
					lsCommands.Clear();
					return;
				}
				
				// Command file selected
				LblFile.Markup = "<b>" + GlobalObj.LMan.GetString("commandfilelbl") + " " + 
					"[" + commandFileName + "]</b>";
				
				TbOpen.Sensitive = false;
				TbClose.Sensitive = true;
				MenuFileOpen.Sensitive = false;
				MenuFileClose.Sensitive = true;
				
			}
			else
			{
				// No commanf dile selected
				LblFile.Markup = "<b>" + GlobalObj.LMan.GetString("commandfilelbl") + "</b>";

				TbOpen.Sensitive = true;
				TbClose.Sensitive = false;
				MenuFileOpen.Sensitive = true;
				MenuFileClose.Sensitive = false;
				
				lsCommands = new ListStore(typeof(string));
				LstCommands.Model=lsCommands;
			}
			
		}
		
		
		
		
		
		
		/// <summary>
		/// Fill command file area from command file
		/// </summary>
		private bool UpdateFileAreaFromFile(string filepath)
		{
			StreamReader sr = null;
			List<string> cmdList = new List<string>();
			
			try
			{
				sr = new StreamReader(filepath);
				
				string line = "";
				while (!sr.EndOfStream)
				{
					line = sr.ReadLine();
					cmdList.Add(line);
				}
				
				sr.Close();
				sr.Dispose();
				sr = null;
				
				UpdateTreeView(cmdList);
				
				return true;
			}
			catch(Exception Ex)
			{
				log.Error(Ex.Message);
				MainClass.ShowMessage("ERROR", Ex.Message, MessageType.Error);
				return false;
			}
			
		}
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		#endregion Private Methods
		
		
		
		
		
		
		
	}
}

