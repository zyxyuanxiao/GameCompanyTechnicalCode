#!/bin/bash

project_path=$0
path=${project_path%/*}
cd ${path}
cd ..
# unity 调用过来需要输入用户名密码
up=$(svn update -r HEAD --username yishen --password xlyishen)

cd shell
sh svn_info.command