@echo off
echo ===========[爱你,爱世人] ==================
cd %~dp0
set absolutePath=%~dp0
echo "脚本绝对路径:" %absolutePath%
cd ../../
set  workdir=%cd%
echo "切换到当前工作目录:" %workdir%
echo "===========[爱你,爱世人] 开始执行 Python 打表=================="

start "检查全部Excel表格" cmd.exe /k call %workdir%\Windows\Windows-bat\CheckAllExcel.bat
REM start  /d   %workdir%\Windows\Windows-bat\   CheckAllExcel.bat

call ./Windows/py3-virtualenv/Scripts/activate.bat
REM ################ 将 xlsx 转成 CSV 格式 ################
python ./Windows/py3-Tools/Misc/XlsxToCsv.py
REM ################ 给 CSV 格式的文件添加 key ################
python ./Windows/py3-Tools/Misc/add_key_to_csv_tools/add_key_to_csv.py

REM 下面三行代码由操作者使用,第一行只输出 CSharp 文件,第二行只输出 Lua 文件,第三行输出 CSharp,Lua 文件
REM 打开其中一行,另外 2 行需要关闭
REM 修改之后请上传到 SVN

REM ################ 将 CSV 格式的文件转成 bytes/lua/cs 文件 ################
REM python ./Windows/py3-Tools/EntryPoint.py "China" "CSharp"
REM ################ 将 CSV 格式的文件转成 bytes/lua/cs 文件 ################
REM python ./Windows/py3-Tools/EntryPoint.py "China" "Lua"
REM ################ 将 CSV 格式的文件转成 bytes/lua/cs 文件 ################
python ./Windows/py3-Tools/EntryPoint.py "China" "CSharp,Lua"

call ./Windows/py3-virtualenv/Scripts/deactivate.bat

echo ===========[爱你,爱世人] 主人,Python 打表结束,请尽情玩耍我哦==================

pause