#!/bin/bash

#UNITY程序的路径#
UNITY_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity

#游戏程序路径#
PROJECT_PATH=$2


#将unity导出成Apk#
echo "将unity进行windows构建"

killall Unity

$UNITY_PATH -projectPath $PROJECT_PATH -executeMethod ProjectBuild.BuildForWindows project-$1 channel-$3 releaseType-$4 productName-$5 bundleVersion-$6 bundleID-$7 buildOptions-$9 packServer-${10} UWA-${16} -quit -logFile $PROJECT_PATH/build.txt
echo $1 $2 $3 $4 $5 $6 $7 $8 $9
echo "windows构建完毕!"