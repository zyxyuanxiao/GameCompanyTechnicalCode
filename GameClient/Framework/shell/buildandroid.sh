#!/bin/bash
UNITY_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity
PROJECT_PATH=$2
killall Unity

method=ProjectBuild.BuildForAndroid

if [ ! -n "${16}" ]; then
    echo "UWA参数为空"
else
    if [ ${16}==true ]; then
        method=ProjectBuild.BuildForAndroid_DEV
    fi
fi
${UNITY_PATH} -quit -projectPath ${PROJECT_PATH} -executeMethod ${method} project-${1} channel-${3} releaseType-${4} productName-${5} bundleVersion-${6} bundleID-${7} buildOptions-${9} packServer-${10} UWA-${16}  -logFile ${PROJECT_PATH}/build.txt