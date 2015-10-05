# Introduction #

This page lists methods to build Comex components on Linux and Windows os.

# Build on Linux #

## Build and install from tarballs ##

Can find tarballs in [Downloads area](http://code.google.com/p/comex-project/downloads/list).

### For comex-base ###

Build Dependencies:
  * mono devel package
  * log4net devel package

To build and install can use:
```
 $ ./configure --prefix=/usr --libdir=/usr/lib
 $ make
 # make linuxpkgconfigdir=/usr/share/pkgconfig install (as root)
```

NOTE: Before build other tarballs need also to register comex-base dll in Mono GAC_```
# gacutil -i /usr/lib/comex-base/comex-base.dll -package comex-base -gacdir /usr/lib/mono/gac -root /usr/lib
```

---_

### For comex ###

Build Dependencies:
  * mono devel package
  * log4net devel package
  * comex-base devel package (or tarball install)

To build and install can use:
```
 $ ./configure
 $ make
 # make install (as root)
```

---


### For comex-gtk ###

Build Dependencies:
  * mono devel package
  * log4net devel package
  * comex-base devel package (or tarball install)
  * gtk-sharp2 devel package
  * glade-sharp2

To build and install can use:
```
 $ ./configure
 $ make
 # make install (as root)
```

---


### For comex-qt ###

Build Dependencies:
  * mono devel package
  * log4net devel package
  * comex-base devel package (or tarball install)
  * qyoto devel package

To build and install can use:
```
 $ ./configure
 $ make
 # make install (as root)
```


<br />


## Build from sources ##
Need to obtain sources using checkout instructions contained in [Source Tabs](http://code.google.com/p/comex-project/source/checkout).

### For svn comex-base ###

Build Dependencies:
  * mono devel package
  * log4net devel package

To build can use build script:
```
 $ ./build-lnx-comex-base.sh
```
build output will be generated in comex-base/bin/Debug folder

After you need to register in GAC generated file using:
```
 # gacutil -i <path of svn checkout>/comex-base/bin/Debug/comex-base.dll -package comex-base -gacdir /usr/lib/mono/gac -root /usr/lib
```

You need also to create file /usr/share/pkgconfig/comex-base.pc as follow:
```
     Name: comex-base
     Description: base component for comex project
     Version: 0.1.8.4

     Requires: 
     Libs: -r:<path of svn checkout>/comex-base/bin/Debug/comex-base.dll
```

---


### For svn comex ###

Build Dependencies:
  * mono devel package
  * log4net devel package
  * comex-base devel (see [comex-base](#For_comex-base.md))

To build can use build script:
```
 $ ./build-lnx-comex.sh
```
build output will be generated in comex/bin/Debug folder. Can run using
```
 $ mono --debug comex.exe --log-console
```

---


### For svn comex-gtk ###

Build Dependencies:
  * mono devel package
  * log4net devel package
  * comex-base devel (see [comex-base](#For_comex-base.md))
  * gtk-sharp2 devel package
  * glade-sharp2

To build can use build script:
```
 $ ./build-lnx-comex-gtk.sh
```
build output will be generated in comex-gtk/bin/Debug folder. Can run using
```
 $ mono --debug comex-gtk.exe --log-console
```

---


### For svn comex-qt ###

Build Dependencies:
  * mono devel package
  * log4net devel package
  * comex-base devel (see [comex-base](#For_comex-base.md))
  * qyoto devel devel package

To build can use build script:
```
 $ ./build-lnx-comex-qt.sh
```
build output will be generated in comex-qt/bin/Debug folder. Can run using
```
 $ mono --debug comex-qt.exe --log-console
```


<br />


# Build on Windows #

## Build from sources on Win ##
Need to obtain sources using checkout instructions contained in [Source Tabs](http://code.google.com/p/comex-project/source/checkout).


After can use [SharpDevelop IDE](http://sharpdevelop.net/opensource/sd/) to build all solutions. Note that you need to change reference to comex-base and log4net in other projects (comex, comex-gtk) because default reference is in GAC.