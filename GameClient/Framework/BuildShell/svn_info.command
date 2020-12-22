#!/bin/bash

project_path=$0
path=${project_path%/*}
cd ${path}
cd ..

new_svn_rev=$(svn info |grep "Revision") 


new_rev=${new_svn_rev:10}

# 将new_rev写入内容
echo ${new_rev}

# # 将svn版本信息写入文件
# for line in `cat Assets/StreamingAssets/packVersionInfo`
# do
# 	echo ${line}
# 	# 暂时确定这个文件夹只有一行
# 	left=${line%%,*}
# 	echo ${left}
# 	right=$(echo ${line:(${#left}+1)})
# 	right=${right#*,}
#  	echo ${right}
#  	echo ${left},${new_rev},${right} > Assets/StreamingAssets/packVersionInfo
#  	break
# done


# cd shell
# sh svn_info.sh