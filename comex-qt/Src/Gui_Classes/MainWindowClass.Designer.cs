
using System;
using Qyoto;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using comex;

using log4net;

namespace comexqt
{
	
	
	public partial class MainWindowClass: QMainWindow
	{
		
		// Attributes		
		Ui.MainWindow mainwindow_Ui;
		
		// Log4Net object
        private static readonly ILog log = LogManager.GetLogger(typeof(comexqt.MainWindowClass));

		
		
		/// <summary>
		/// Constructor
		/// </summary>
		public MainWindowClass()
		{
			// Create new mainwindow_Ui object
			mainwindow_Ui = new Ui.MainWindow();
			
			// Configure layout of this new QMainWindow with 
			// mainwindow_Ui objects and data
			mainwindow_Ui.SetupUi(this);
			
			// Update Graphic Objects
			UpdateGraphicObjects();
			
			// Add eventhandlers
			UpdateReactors();
			
		}
		
		
		
		
		
		
		
		
		
		#region Q_SLOTS
		
		
		[Q_SLOT]
		public void ActionExit()
		{
			QApplication.Quit();
		}

		
		
		
		[Q_SLOT]		
		public void ActionOpen()
		{
			OpenCommandFile();			
		}
		
		
		
		[Q_SLOT]
		public void ActionClose()
		{
			CloseCommandFile();
		}
		
		
		
		
		[Q_SLOT]		
		public void ActionInfo()
		{
			OpenInfo();
		}
		
		
		
		
		
		[Q_SLOT]
		public void ActionATR()
		{
			GetATR();
		}
		
		
		
		[Q_SLOT]
		public void ActionChangeReader()
		{
			QAction sender = (QAction)Sender();			
			string newReader = sender.Text;
			
			log.Info("ActionChangeReader " + newReader);
			UpdateSelectedReader(newReader);
		}
		
		
		
		
		
		
		[Q_SLOT]
		public void ActionSendCommand()
		{
			ExchangeData();
		}
		

		
		
		
		[Q_SLOT]
		public void ActionAddCommand(QListWidgetItem qlwi)
		{
			// check for double click
			GetCommandFromList();

		}
		
		
		
		
		[Q_SLOT]
		public void ActionExecCommand()
		{
			QShortcut sender = (QShortcut)Sender();
			
			if (sender.Key.ToString() == "F5")
			{
				// Update text of command
				GetCommandFromList();
			}
			else if (sender.Key.ToString() == "F6")
			{
				// Update text of command,  send it and receive response
				GetCommandFromList();
				ExchangeData();
			}

		}
		
		
		#endregion Q_SLOTS
		
		
		
		
		
		
		
		
		
	

		#region Private Methods
		
		
		
		/// <summary>
		/// Update graphic objects with language file values
		/// </summary>
		private void UpdateGraphicObjects()
		{
			// Main Window Title
			this.WindowTitle = MainClass.AppNameVer + " [" + GlobalObj.AppNameVer + "]";
			
			// Update menu and toolbar 
			mainwindow_Ui.menu_File.Title = GlobalObj.LMan.GetString("filemenulbl").Replace("_", "&");
			mainwindow_Ui.action_Open.Text = GlobalObj.LMan.GetString("openmenulbl").Replace("_", "&");
			mainwindow_Ui.action_Close.Text = GlobalObj.LMan.GetString("closemenulbl").Replace("_", "&");
			mainwindow_Ui.action_Exit.Text = GlobalObj.LMan.GetString("exitmenulbl").Replace("_", "&");
			mainwindow_Ui.menu_Reader.Title = GlobalObj.LMan.GetString("readermenulbl").Replace("_", "&");
			mainwindow_Ui.menu_About.Title = GlobalObj.LMan.GetString("helpmenulbl").Replace("_", "&");
			mainwindow_Ui.action_Info.Text = GlobalObj.LMan.GetString("infomenulbl").Replace("_", "&");
			
			mainwindow_Ui.FrameATR.Title = GlobalObj.LMan.GetString("atrframelbl");
			mainwindow_Ui.FrameFile.Title = GlobalObj.LMan.GetString("commandfilelbl");
			mainwindow_Ui.FrameExchange.Title = GlobalObj.LMan.GetString("cardframelbl");
			
			mainwindow_Ui.LblCommand.Text = GlobalObj.LMan.GetString("cmdlbl");
			mainwindow_Ui.LblResponse.Text = GlobalObj.LMan.GetString("resplbl");
			mainwindow_Ui.BtnSend.Text = GlobalObj.LMan.GetString("sendlbl");
			
			
			// Update font for command list
			if (GlobalObj.IsWindows())
			{
				mainwindow_Ui.LstCommands.Font = new QFont("Courier New", 10);
			}
			else
			{
				mainwindow_Ui.LstCommands.Font = new QFont("Fixed", 10);				
			}
			
			
			// Update readers list on gui
			allReaders.AddRange(GlobalObj.PCSC_Readers);
			allReaders.AddRange(GlobalObj.SerialPortsName);
			
			QAction action_Reader;
			QActionGroup readersGrp = new QActionGroup(this);
			
			for (int r=0; r<allReaders.Count; r++)
			{
				action_Reader = new QAction(allReaders[r], mainwindow_Ui.menu_Reader);
				action_Reader.ObjectName = "action_Reader_" + r.ToString();
				action_Reader.SetVisible(true);
				action_Reader.IconVisibleInMenu = false;
				action_Reader.Checkable = true;
				action_Reader.SetActionGroup(readersGrp);
				
				if (r==0)
				{
					action_Reader.SetChecked(true);
				}
				else
				{
					action_Reader.SetChecked(false);
				}
				mainwindow_Ui.menu_Reader.AddAction(action_Reader);
				
				Connect( action_Reader, SIGNAL("activated()"), this, SLOT("ActionChangeReader()"));
				
			}
			
			// check for available readers
			if (allReaders.Count > 0)
			{
				// select first reader
				GlobalObj.SelectedReader = allReaders[0];
			}

			
			
			if (GlobalObj.PCSC_Readers.Count == 0)
			{
				mainwindow_Ui.statusbar.ShowMessage(GlobalObj.LMan.GetString("nopcscreader"));
			}
			
			// LANGUAGE STATUS TIP
			// mainwindow_Ui.action_Open.StatusTip = "";  // status bar desc
				
			// LANGUAGE TOOL TIP
			// mainwindow_Ui.action_Open.ToolTip = ""; // tool tip text
			
		}
			
		
		
		
		
		
		private void UpdateReactors()
		{
			// Configure events reactors
			Connect( mainwindow_Ui.action_Exit, SIGNAL("activated()"), this, SLOT("ActionExit()"));
			Connect( mainwindow_Ui.action_Open, SIGNAL("activated()"), this, SLOT("ActionOpen()"));			
			Connect( mainwindow_Ui.action_Close, SIGNAL("activated()"), this, SLOT("ActionClose()"));
			Connect( mainwindow_Ui.action_Info, SIGNAL("activated()"), this, SLOT("ActionInfo()"));
			Connect( mainwindow_Ui.action_ATR, SIGNAL("activated()"), this, SLOT("ActionATR()"));	
			Connect( mainwindow_Ui.BtnSend, SIGNAL("clicked()"), this, SLOT("ActionSendCommand()"));	
			Connect( mainwindow_Ui.LstCommands, SIGNAL("itemDoubleClicked(QListWidgetItem*)"),this,SLOT("ActionAddCommand(QListWidgetItem*)"));
			
			QShortcut qsc = new QShortcut(new QKeySequence("F5"), mainwindow_Ui.LstCommands);
			Connect( qsc, SIGNAL("activated()"),this,SLOT("ActionExecCommand()"));
			
			qsc = new QShortcut(new QKeySequence("F6"), mainwindow_Ui.LstCommands);
			Connect( qsc, SIGNAL("activated()"),this,SLOT("ActionExecCommand()"));
		}
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// update command from selected row in command file area
		/// </summary>
		private void GetCommandFromList()
		{
			// clear text
			mainwindow_Ui.TxtCmd.Text = "";
			MainClass.QtWait();
			
			// obtain row content
			List<QListWidgetItem> selection = mainwindow_Ui.LstCommands.SelectedItems();
			
			// check for selection
			if (selection.Count == 0)
			{
				// if no selection
				return;
			}
			
			// get command
			string command = selection[0].Text();
			
			// check for empty row
			if (command.Trim() == "")
			{
				return;
			}
			
			
			// check for comments
			if (command.Trim().IndexOf("#") == 0)
			{
				return;
			}

			
			mainwindow_Ui.TxtCmd.Text = command.Substring(31);
		}
		
		
		
		
		
		/// <summary>
		/// Update graphic list object from collection
		/// </summary>
		private void UpdateLstCommands(List<string> rows)
		{
			mainwindow_Ui.LstCommands.Clear();
			QListWidgetItem qlwi;
			
			foreach (string row in rows)
			{
				qlwi = new QListWidgetItem(row, mainwindow_Ui.LstCommands);
				mainwindow_Ui.LstCommands.AddItem(qlwi);
			}

		}
		
		
		
		
		
		
		
		#endregion Private Methods
		
		
		
		
		
		
		
		
		
		
		
		
		
		

		
		
	}
}
