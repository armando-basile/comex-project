#! /bin/bash

TARGET="Debug"

# Clean and Build solution
mdtool build -t:Clean -c:$TARGET comex.sln
mdtool build -t:Build -c:$TARGET comex.sln
