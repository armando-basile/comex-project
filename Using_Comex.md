# Introduction #

Here you can see simple steps to use Comex


## STEP 1 - SELECT READER TO USE ##
As first item you need to select reader to use. You can choose it from _**reader menu**_ where you find pc/sc readers and available serial port if you choose to use smartmouse reader

![http://comex-project.googlecode.com/svn/wiki/gtk_4.png](http://comex-project.googlecode.com/svn/wiki/gtk_4.png)

---


## STEP 2 - SETTING SERIAL PORT ##
**Only if you choose to use smartmouse**, you could need to setup serial port communication using special toolbar button as follow

![http://comex-project.googlecode.com/svn/wiki/gtk_8.png](http://comex-project.googlecode.com/svn/wiki/gtk_8.png)

---


## STEP 3 - POWER ON CARD ##
To power on card, after inserting card into selected reader, use special toolbar button as follow

![http://comex-project.googlecode.com/svn/wiki/gtk_5.png](http://comex-project.googlecode.com/svn/wiki/gtk_5.png)

---


## STEP 4 - SEND DATA TO CARD ##
To send data you need to add command in command field and use special _**send**_ button as follow. Card response will be displayed in response field

![http://comex-project.googlecode.com/svn/wiki/gtk_7.png](http://comex-project.googlecode.com/svn/wiki/gtk_7.png)

---


## STEP 5 - USE OF COMMAND FILES ##
Comex support use of .comex file format, a simple text file where on each row you can add a comment (# as first char) or a command (first 31 characters as command description, from 32° character start command).
Opening .comex file you can see content in command file area as follow

![http://comex-project.googlecode.com/svn/wiki/gtk_6.png](http://comex-project.googlecode.com/svn/wiki/gtk_6.png)

can find an example of comex file here: [gsm.comex](http://comex-project.googlecode.com/svn/trunk/resources/gsm.comex). To send immediately a command you need to select row and press _**F6**_ key function, instead if you want only copy command from row to command filed, you need to select row and press _**F5**_ key function or double click with mouse