#!/usr/bin/env bash
echo "===========[爱你,爱世人] =================="
cd "$(dirname "$0")"
absolutePath=$(cd "$(dirname "$0")";pwd)
echo "脚本绝对路径:" ${absolutePath}
cd ../../../
workdir=$(pwd)
echo "切换到当前工作目录:" ${workdir}
echo "===========[爱你,爱世人] 开始执行 Python 打表=================="

# 开启另外一个终端,进行检查
open -a Terminal.app "${workdir}/Mac/Mac-shell/CheckAllExcel.command"

source  ./Mac/py3-virtualenv/bin/activate

################ 将 xlsx 转成 CSV 格式 ################
python3 ./Mac/py3-Tools/Misc/XlsxToCsv.py ${absolutePath}
################ 给 CSV 格式的文件添加 key ################
python3 ./Mac/py3-Tools/Misc/add_key_to_csv_tools/add_key_to_csv.py ${absolutePath}

# 下面三行代码由操作者使用,第一行只输出 CSharp 文件,第二行只输出 Lua 文件,第三行输出 CSharp,Lua 文件
# 打开其中一行,另外 2 行需要关闭
# 修改之后请上传到 SVN

################ 将 CSV 格式的文件转成 cs 文件 ################
#python3 ./Mac/py3-Tools/EntryPoint.py "China" "CSharp" ${absolutePath}
################ 将 CSV 格式的文件转成 Lua 文件 ################
# python3 ./Mac/py3-Tools/EntryPoint.py "China" "Lua" ${absolutePath}
################ 将 CSV 格式的文件转成 CSharp和Lua 文件 ################
python3 ./Mac/py3-Tools/EntryPoint.py "China" "CSharp,Lua" ${absolutePath}

deactivate

echo "===========[爱你,爱世人] 主人,Python 打表结束,请尽情玩耍我哦=================="


sleep 30s