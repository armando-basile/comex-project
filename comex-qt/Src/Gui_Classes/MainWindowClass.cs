
using System;
using Qyoto;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using comexbase;

using log4net;

namespace comexqt
{
	
	
	public partial class MainWindowClass: QMainWindow
	{


		// Attributes
		private string retStr = "";
		private string ATR = "";
		private string command = "";
		private string response = "";
		private string commandFileName = "";
		private string commandFilePath = "";
		private List<string> allReaders = new List<string>();
		
		
		
		
		
		#region Private Methods

		
		
		
		
		/// <summary>
		/// Information about
		/// </summary>
		private void OpenInfo()
		{
			AboutDialogClass adc = new AboutDialogClass();
			adc.Show();
			
		}
		
		
		
		
		/// <summary>
		/// Show settings dialog
		/// </summary>
		private void OpenSettings()
		{
			SettingsDialogClass sdc = new SettingsDialogClass();
			sdc.Show();
		}
		
		
		
		
		/// <summary>
		/// Update selected reader
		/// </summary>
		private void UpdateSelectedReader(string reader)
		{
			GlobalObj.CloseConnection();			
			GlobalObj.SelectedReader = reader;
			mainwindow_Ui.statusbar.ShowMessage(GlobalObj.LMan.GetString("selreader") + ": " + reader);
			mainwindow_Ui.TxtATR.Text = "";
			mainwindow_Ui.TxtResp.Text = "";
			mainwindow_Ui.TxtCmd.Text = "";
		}
		
		
		
		
		
		

		/// <summary>
		/// Perform Power On
		/// </summary>
		private void GetATR()
		{
			mainwindow_Ui.TxtATR.Text = "";
			mainwindow_Ui.TxtResp.Text = "";
			mainwindow_Ui.TxtCmd.Text = "";
			MainClass.QtWait();
			
			// Connect to smartcard
			retStr = GlobalObj.AnswerToReset(ref ATR);
			
			if (retStr != "")
			{
				// error on answer to reset
				log.Error(retStr);
				MainClass.ShowMessage(this, "ERROR", retStr, MainClass.MessageType.Error);
				return;
			}
			
			mainwindow_Ui.TxtATR.Text = ATR;
		}
		
		
		
		
		
		
		/// <summary>
		/// Perform exchange data with card
		/// </summary>
		private void ExchangeData()
		{
			if (!GlobalObj.IsPowered)
			{
				MainClass.ShowMessage(this, "WARNING", GlobalObj.LMan.GetString("noinitreader"), MainClass.MessageType.Warning);
				return;
			}
			
			command = mainwindow_Ui.TxtCmd.Text;
			mainwindow_Ui.TxtResp.Text = "";
			mainwindow_Ui.TxtResp.Show();
			MainClass.QtWait();
			
			retStr = GlobalObj.SendReceive(command, ref response);
			
			if (retStr != "")
			{
				log.Error(retStr);
				MainClass.ShowMessage(this, "ERROR", retStr, MainClass.MessageType.Error);
				return;
			}
			
			mainwindow_Ui.TxtResp.Text = response;
		}
		
		
		
		
		
		
		/// <summary>
		/// Open command file
		/// </summary>
		private void OpenCommandFile()
		{
            
            // New dialog for select command file 
			string selectedFile = QFileDialog.GetOpenFileName(this, 
                                                        GlobalObj.LMan.GetString("selectfile"),
                                                        null,
                                                        "*.comex");
                                
            if (string.IsNullOrEmpty(selectedFile))
            {
            	return;
            }

			
            // path of a right file returned
            commandFilePath = selectedFile;				
			commandFileName = System.IO.Path.GetFileNameWithoutExtension(commandFilePath);
			log.Debug("file selected: " + commandFilePath);
				
			UpdateGuiForCommandFile();
        
		}
		

		
		
		
		/// <summary>
		/// Close command file
		/// </summary>
		private void CloseCommandFile()
		{
			log.Debug("CloseCommandFile");
			commandFileName = "";
			commandFilePath = "";
			
			UpdateGuiForCommandFile();
		}

		
		
		
		
		
		private void UpdateGuiForCommandFile()
		{
			if (commandFileName != "")
			{
				log.Debug("UpdateFileAreaFromFile " + commandFileName);
				if (!UpdateFileAreaFromFile(commandFilePath))
				{
					// error detected
					log.Debug("UpdateFileAreaFromFile error");
					mainwindow_Ui.LstCommands.SetModel(new QStringListModel());
					return;
				}
				
				// Command file selected
				mainwindow_Ui.FrameFile.Title = GlobalObj.LMan.GetString("commandfilelbl") + " " + 
					"[" + commandFileName + "]";
				
				mainwindow_Ui.action_Open.SetEnabled(false);
				mainwindow_Ui.action_Close.SetEnabled(true);
				
			}
			else
			{
				log.Debug("UpdateFileAreaFromFile <empty>");
				// No command file selected
				mainwindow_Ui.FrameFile.Title = GlobalObj.LMan.GetString("commandfilelbl");

				mainwindow_Ui.action_Open.SetEnabled(true);
				mainwindow_Ui.action_Close.SetEnabled(false);
	
				mainwindow_Ui.LstCommands.Clear();
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
				
				UpdateLstCommands(cmdList);
				
				return true;
			}
			catch(Exception Ex)
			{
				log.Error(Ex.Message);
				MainClass.ShowMessage(this, "ERROR", Ex.Message, MainClass.MessageType.Error);
				return false;
			}
			
		}
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		#endregion Private Methods
		
		
		
		
		

	}
}
