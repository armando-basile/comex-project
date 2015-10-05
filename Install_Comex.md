# Introduction #

Before install Comex components you may want to try it, so you need to follow [Use binaries](#Use_binaries.md) section.
<br />


# Install from your package manager #
  * **Mageia Linux**
> <table><tr><td width='64' align='center' valign='middle'><img src='http://comex-project.googlecode.com/svn/wiki/mageia_mini.png' /></td>
<blockquote><td align='left' valign='middle'>from Mageia 1 can use <b>urpmi</b> to install packages </td></tr></table></blockquote>

  * **Mandriva/Rosa Linux**
> <table><tr><td width='64' align='center' valign='middle'><img src='http://monosim.integrazioneweb.com/images/mandriva_mini.png' /></td>
<blockquote><td align='left' valign='middle'>from Mandriva 2010.1 can use <b>urpmi</b> to install packages <i>(need to enable contrib/backports repository)</i></td></tr></table></blockquote>

  * **Debian/Ubuntu Linux**
> <table><tr><td width='64' align='center' valign='middle'><img src='http://monosim.integrazioneweb.com/images/ubuntu_mini.png' /></td>
<blockquote><td align='left' valign='middle'>can find latest packages on <a href='http://launchpad.net/~armando-basile/+archive/stable'>launchpad ppa</a></td></tr></table></blockquote>

  * **Opensuse Linux**
> <table><tr><td width='64' align='center' valign='middle'><img src='http://monosim.integrazioneweb.com/images/suse_mini.png' /></td>
<blockquote><td align='left' valign='middle'>can find packages on <a href='http://download.opensuse.org/repositories/home:/hmandevteam/'>OpenSUSE Build System</a></td></tr></table></blockquote>



<br />



# Install from tarballs #
Please see [Build and install from tarballs](http://code.google.com/p/comex-project/wiki/Build_Comex#Build_and_install_from_tarballs) section

<br />



# Use binaries #
Can find binaries package in [Downloads area](http://code.google.com/p/comex-project/downloads/list).
  * **On Linux**
> All Comex interfaces on Linux require **pcsc-lite** and **libpcsclite1**.<br />
> To launch **console interface** using:
> > <div><i>to obtain help message</i></div>
```
$ mono comex.exe --help 
```
> > <div><i>to launch without log</i></div>
```
$ mono comex.exe
```

> To launch **GTK user interface** using:
> > <div><i>to obtain help message</i></div>
```
$ mono comex-gtk.exe --help
```
> > <div><i>to launch without log</i></div>
```
$ mono comex-gtk.exe
```
> > NOTE: _you need to install **gtk-sharp2** and **glade-sharp2** to use it_

> To launch **QT user interface** using:
> > <div><i>to obtain help message</i></div>
```
$ mono comex-qt.exe --help
```
> > <div><i>to launch without log</i></div>
```
$ mono comex-qt.exe
```
> > NOTE: _you need to install **qyoto** to use it_
<br />
  * **On Windows**

> To launch **console interface** using:
> > <div><i>to obtain help message</i></div>
```
C:\...> comex.exe --help 
```
> > <div><i>to launch without log</i></div>
```
C:\...> comex.exe
```

> To launch **GTK user interface** using:<br />
> double click on monosim-gtk.exe <br />
> _**NOTE**_: _you need to install [Gtk# for .NET](http://www.go-mono.com/mono-downloads/download.html) to use it_