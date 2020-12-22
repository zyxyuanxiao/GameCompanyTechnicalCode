@echo off
echo ===========[����,������] ==================
cd %~dp0
set absolutePath=%~dp0
echo "�ű�����·��:" %absolutePath%
cd ../../../
set  workdir=%cd%
echo "�л�����ǰ����Ŀ¼:" %workdir%
echo "===========[����,������] ��ʼִ�� Python ���=================="

start "���ȫ��Excel���" cmd.exe /k call %workdir%\Windows\Windows-bat\CheckAllExcel.bat
REM start  /d   %workdir%\Windows\Windows-bat\   CheckAllExcel.bat

call ./Windows/py3-virtualenv/Scripts/activate.bat
echo "==== start convert Error Code Excel ====="
python ./Windows/py3-Tools/error_code.py

REM ################ �� xlsx ת�� CSV ��ʽ ################
python ./Windows/py3-Tools/Misc/XlsxToCsv.py %absolutePath%
REM ################ �� CSV ��ʽ���ļ���� key ################
python ./Windows/py3-Tools/Misc/add_key_to_csv_tools/add_key_to_csv.py %absolutePath%

REM �������д����ɲ�����ʹ��,��һ��ֻ��� CSharp �ļ�,�ڶ���ֻ��� Lua �ļ�,��������� CSharp,Lua �ļ�
REM ������һ��,���� 2 ����Ҫ�ر�
REM �޸�֮�����ϴ��� SVN

REM ################ �� CSV ��ʽ���ļ�ת�� bytes/lua/cs �ļ� ################
REM python ./Windows/py3-Tools/EntryPoint.py "China" "CSharp" %absolutePath%
REM ################ �� CSV ��ʽ���ļ�ת�� bytes/lua/cs �ļ� ################
REM python ./Windows/py3-Tools/EntryPoint.py "China" "Lua" %absolutePath%
REM ################ �� CSV ��ʽ���ļ�ת�� bytes/lua/cs �ļ� ################
python ./Windows/py3-Tools/EntryPoint.py "China" "CSharp,Lua" %absolutePath%

call ./Windows/py3-virtualenv/Scripts/deactivate.bat

echo ===========[����,������] ����,Python ������,�뾡����ˣ��Ŷ==================

pause