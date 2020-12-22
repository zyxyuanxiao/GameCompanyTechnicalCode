@echo off

echo "===========[love you,love people] =================="

cd    %~dp0

set   absolutePath=%~dp0

cd   ../../

set    workdir=%cd%

echo "change work dir : %workdir%" 

echo "===========[love you,love people] start Python check=================="

echo %workdir%\Windows\py3-Tools\Misc\CheckExcelConfig.py

python   "%workdir%\Windows\py3-Tools\Misc\CheckExcelConfig.py


echo "===========[love you,love people] Python check over=================="

pause


