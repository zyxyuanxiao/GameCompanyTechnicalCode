#!/bin/bash

#UNITY程序的路径#
UNITY_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity

#游戏程序路径#
PROJECT_PATH=$2

#IOS打包脚本路径#
BUILD_IOS_PATH=${PROJECT_PATH}/shell/buildxcode_proj.sh

#生成的Xcode工程路径#
XCODE_PATH=${PROJECT_PATH}/$1

echo "00=>"$0
echo "01=>"$1
echo "02=>"$2
echo "03=>"$3
echo "04=>"$4
echo "05=>"$5
echo "06=>"$6
echo "07=>"$7
echo "08=>"$8
echo "09=>"$9

#将unity导出成xcode工程#

killall Unity

$UNITY_PATH -projectPath $PROJECT_PATH -executeMethod ProjectBuild.BuildForIPhone project-$1 channel-$3 releaseType-$4 productName-$5 bundleVersion-$6 bundleID-$7 packProfile-$8 buildOptions-$9 packServer-${10} UWA-${16} -quit -logFile $PROJECT_PATH/build.txt

echo "XCODE工程生成完毕"

# #开始生成ipa#
$BUILD_IOS_PATH $PROJECT_PATH/$1 $1

echo "ipa生成完毕"