#! /bin/bash

TARGET="Debug"

# Clean and Build solution
mdtool build -t:Clean -c:$TARGET comex-gtk.sln
mdtool build -t:Build -c:$TARGET comex-gtk.sln
