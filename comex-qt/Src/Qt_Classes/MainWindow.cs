/********************************************************************************
** Form generated from reading ui file 'MainWindow.ui'
**
** Created: lun ago 29 17:50:43 2011
**      by: Qt User Interface Compiler for C# version 4.6.3
**
** WARNING! All changes made in this file will be lost when recompiling ui file!
********************************************************************************/


using Qyoto;

public class Ui_MainWindow
{
    public QAction action_Open;
    public QAction action_Close;
    public QAction action_Exit;
    public QAction action_Info;
    public QAction action_ATR;
    public QAction action_Exec_Command;
    public QWidget centralwidget;
    public QGridLayout gridLayout;
    public QGroupBox FrameATR;
    public QGridLayout gridLayout1;
    public QLineEdit TxtATR;
    public QGroupBox FrameFile;
    public QGridLayout gridLayout2;
    public QListWidget LstCommands;
    public QGroupBox FrameExchange;
    public QGridLayout gridLayout3;
    public QLabel LblCommand;
    public QLineEdit TxtCmd;
    public QPushButton BtnSend;
    public QLabel LblResponse;
    public QLineEdit TxtResp;
    public QMenuBar menubar;
    public QMenu menu_File;
    public QMenu menu_Reader;
    public QMenu menu_About;
    public QStatusBar statusbar;
    public QToolBar toolBar;

    public void SetupUi(QMainWindow MainWindow)
    {
    if (MainWindow.ObjectName == "")
        MainWindow.ObjectName = "MainWindow";
    QSize Size = new QSize(631, 570);
    Size = Size.ExpandedTo(MainWindow.MinimumSizeHint());
    MainWindow.Size = Size;
    MainWindow.MinimumSize = new QSize(600, 550);
    MainWindow.WindowIcon = new QIcon(":/main/resources/Images/comex_256.png");
    action_Open = new QAction(MainWindow);
    action_Open.ObjectName = "action_Open";
    action_Open.icon = new QIcon(":/main/resources/Images/document-open.png");
    action_Close = new QAction(MainWindow);
    action_Close.ObjectName = "action_Close";
    action_Close.Enabled = false;
    action_Close.icon = new QIcon(":/main/resources/Images/document-close.png");
    action_Exit = new QAction(MainWindow);
    action_Exit.ObjectName = "action_Exit";
    action_Exit.icon = new QIcon(":/main/resources/Images/application-exit.png");
    action_Info = new QAction(MainWindow);
    action_Info.ObjectName = "action_Info";
    action_Info.icon = new QIcon(":/main/resources/Images/dialog-information.png");
    action_ATR = new QAction(MainWindow);
    action_ATR.ObjectName = "action_ATR";
    action_ATR.icon = new QIcon(":/main/resources/Images/quickopen.png");
    action_Exec_Command = new QAction(MainWindow);
    action_Exec_Command.ObjectName = "action_Exec_Command";
    centralwidget = new QWidget(MainWindow);
    centralwidget.ObjectName = "centralwidget";
    gridLayout = new QGridLayout(centralwidget);
    gridLayout.ObjectName = "gridLayout";
    FrameATR = new QGroupBox(centralwidget);
    FrameATR.ObjectName = "FrameATR";
    QSizePolicy sizePolicy = new QSizePolicy(QSizePolicy.Policy.Preferred, QSizePolicy.Policy.Fixed);
    sizePolicy.SetHorizontalStretch(0);
    sizePolicy.SetVerticalStretch(0);
    sizePolicy.SetHeightForWidth(FrameATR.SizePolicy.HasHeightForWidth());
    FrameATR.SizePolicy = sizePolicy;
    gridLayout1 = new QGridLayout(FrameATR);
    gridLayout1.ObjectName = "gridLayout1";
    TxtATR = new QLineEdit(FrameATR);
    TxtATR.ObjectName = "TxtATR";
    TxtATR.StyleSheet = "color: rgb(30, 109, 30);";
    TxtATR.ReadOnly = true;

    gridLayout1.AddWidget(TxtATR, 0, 0, 1, 1);


    gridLayout.AddWidget(FrameATR, 0, 0, 1, 1);

    FrameFile = new QGroupBox(centralwidget);
    FrameFile.ObjectName = "FrameFile";
    gridLayout2 = new QGridLayout(FrameFile);
    gridLayout2.ObjectName = "gridLayout2";
    LstCommands = new QListWidget(FrameFile);
    LstCommands.ObjectName = "LstCommands";
    LstCommands.EditTriggers = Qyoto.Qyoto.GetCPPEnumValue("QAbstractItemView", "NoEditTriggers");

    gridLayout2.AddWidget(LstCommands, 0, 0, 1, 1);


    gridLayout.AddWidget(FrameFile, 1, 0, 1, 1);

    FrameExchange = new QGroupBox(centralwidget);
    FrameExchange.ObjectName = "FrameExchange";
    sizePolicy.SetHeightForWidth(FrameExchange.SizePolicy.HasHeightForWidth());
    FrameExchange.SizePolicy = sizePolicy;
    gridLayout3 = new QGridLayout(FrameExchange);
    gridLayout3.ObjectName = "gridLayout3";
    LblCommand = new QLabel(FrameExchange);
    LblCommand.ObjectName = "LblCommand";

    gridLayout3.AddWidget(LblCommand, 0, 0, 1, 1);

    TxtCmd = new QLineEdit(FrameExchange);
    TxtCmd.ObjectName = "TxtCmd";
    TxtCmd.StyleSheet = "color: rgb(30, 109, 30);";

    gridLayout3.AddWidget(TxtCmd, 0, 1, 1, 1);

    BtnSend = new QPushButton(FrameExchange);
    BtnSend.ObjectName = "BtnSend";
    BtnSend.icon = new QIcon(":/main/resources/Images/arrow-right.png");

    gridLayout3.AddWidget(BtnSend, 0, 2, 1, 1);

    LblResponse = new QLabel(FrameExchange);
    LblResponse.ObjectName = "LblResponse";

    gridLayout3.AddWidget(LblResponse, 1, 0, 1, 1);

    TxtResp = new QLineEdit(FrameExchange);
    TxtResp.ObjectName = "TxtResp";
    TxtResp.StyleSheet = "color: rgb(0, 0, 255);";
    TxtResp.ReadOnly = true;

    gridLayout3.AddWidget(TxtResp, 1, 1, 1, 1);


    gridLayout.AddWidget(FrameExchange, 2, 0, 1, 1);

    MainWindow.SetCentralWidget(centralwidget);
    menubar = new QMenuBar(MainWindow);
    menubar.ObjectName = "menubar";
    menubar.Geometry = new QRect(0, 0, 631, 22);
    menu_File = new QMenu(menubar);
    menu_File.ObjectName = "menu_File";
    menu_Reader = new QMenu(menubar);
    menu_Reader.ObjectName = "menu_Reader";
    menu_About = new QMenu(menubar);
    menu_About.ObjectName = "menu_About";
    MainWindow.SetMenuBar(menubar);
    statusbar = new QStatusBar(MainWindow);
    statusbar.ObjectName = "statusbar";
    MainWindow.SetStatusBar(statusbar);
    toolBar = new QToolBar(MainWindow);
    toolBar.ObjectName = "toolBar";
    toolBar.Movable = false;
    toolBar.ToolButtonStyle = Qt.ToolButtonStyle.ToolButtonIconOnly;
    toolBar.Floatable = false;
    MainWindow.AddToolBar(Qt.ToolBarArea.TopToolBarArea, toolBar);

    menubar.AddAction(menu_File.MenuAction());
    menubar.AddAction(menu_Reader.MenuAction());
    menubar.AddAction(menu_About.MenuAction());
    menu_File.AddAction(action_Open);
    menu_File.AddAction(action_Close);
    menu_File.AddSeparator();
    menu_File.AddAction(action_Exit);
    menu_About.AddAction(action_Info);
    toolBar.AddAction(action_Open);
    toolBar.AddAction(action_Close);
    toolBar.AddAction(action_ATR);
    toolBar.AddAction(action_Info);
    toolBar.AddSeparator();
    toolBar.AddAction(action_Exit);

    RetranslateUi(MainWindow);

    QMetaObject.ConnectSlotsByName(MainWindow);
    } // SetupUi

    public void RetranslateUi(QMainWindow MainWindow)
    {
    MainWindow.WindowTitle = QApplication.Translate("MainWindow", "MainWindow", null, QApplication.Encoding.UnicodeUTF8);
    action_Open.Text = QApplication.Translate("MainWindow", "&Apri file comandi", null, QApplication.Encoding.UnicodeUTF8);
    action_Open.Shortcut = QApplication.Translate("MainWindow", "Ctrl+O", null, QApplication.Encoding.UnicodeUTF8);
    action_Close.Text = QApplication.Translate("MainWindow", "&Chiudi file comandi", null, QApplication.Encoding.UnicodeUTF8);
    action_Close.Shortcut = QApplication.Translate("MainWindow", "Ctrl+C", null, QApplication.Encoding.UnicodeUTF8);
    action_Exit.Text = QApplication.Translate("MainWindow", "&Esci", null, QApplication.Encoding.UnicodeUTF8);
    action_Exit.Shortcut = QApplication.Translate("MainWindow", "Ctrl+Q", null, QApplication.Encoding.UnicodeUTF8);
    action_Info.Text = QApplication.Translate("MainWindow", "&Informazioni", null, QApplication.Encoding.UnicodeUTF8);
    action_Info.Shortcut = QApplication.Translate("MainWindow", "Ctrl+I", null, QApplication.Encoding.UnicodeUTF8);
    action_ATR.Text = QApplication.Translate("MainWindow", "Answer To Reset", null, QApplication.Encoding.UnicodeUTF8);
    action_ATR.ToolTip = QApplication.Translate("MainWindow", "Answer To Reset", null, QApplication.Encoding.UnicodeUTF8);
    action_Exec_Command.Text = QApplication.Translate("MainWindow", "Exec Command", null, QApplication.Encoding.UnicodeUTF8);
    FrameATR.Title = QApplication.Translate("MainWindow", "GroupBox", null, QApplication.Encoding.UnicodeUTF8);
    FrameFile.Title = QApplication.Translate("MainWindow", "GroupBox", null, QApplication.Encoding.UnicodeUTF8);
    FrameExchange.Title = QApplication.Translate("MainWindow", "GroupBox", null, QApplication.Encoding.UnicodeUTF8);
    LblCommand.Text = QApplication.Translate("MainWindow", "TextLabel", null, QApplication.Encoding.UnicodeUTF8);
    BtnSend.Text = QApplication.Translate("MainWindow", "Send", null, QApplication.Encoding.UnicodeUTF8);
    LblResponse.Text = QApplication.Translate("MainWindow", "TextLabel", null, QApplication.Encoding.UnicodeUTF8);
    menu_File.Title = QApplication.Translate("MainWindow", "&File", null, QApplication.Encoding.UnicodeUTF8);
    menu_Reader.Title = QApplication.Translate("MainWindow", "&Lettore", null, QApplication.Encoding.UnicodeUTF8);
    menu_About.Title = QApplication.Translate("MainWindow", "&Aiuto", null, QApplication.Encoding.UnicodeUTF8);
    toolBar.WindowTitle = QApplication.Translate("MainWindow", "toolBar", null, QApplication.Encoding.UnicodeUTF8);
    } // RetranslateUi

}

namespace Ui {
    public class MainWindow : Ui_MainWindow {}
} // namespace Ui

