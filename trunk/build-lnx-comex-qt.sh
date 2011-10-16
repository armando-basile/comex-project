#! /bin/bash

TARGET="Debug"

# Clean and Build solution
mdtool build -t:Clean -c:$TARGET comex-qt.sln
mdtool build -t:Build -c:$TARGET comex-qt.sln
