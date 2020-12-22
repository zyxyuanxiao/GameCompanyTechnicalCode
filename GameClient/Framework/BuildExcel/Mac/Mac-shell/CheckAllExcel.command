#!/usr/bin/env bash
echo "===========[爱你,爱世人] =================="
cd "$(dirname "$0")"
absolutePath=$(cd "$(dirname "$0")";pwd)
echo "脚本绝对路径:" ${absolutePath}

cd ../../
workdir=$(pwd)
echo "切换到当前工作目录:" ${workdir}
echo "===========[爱你,爱世人] 开始执行 Python 打表=================="

python3 "${workdir}/Mac/py3-Tools/Misc/CheckExcelConfig.py"


