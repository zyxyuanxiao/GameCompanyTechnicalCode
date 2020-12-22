#!/bin/bash

project_path=$0

path=${project_path%/*}
cd ${path}
cd ..
file_path=$1

echo "`svn diff -rPREV ${file_path}`"
# echo "`svn blame -v ${file_path}`"


