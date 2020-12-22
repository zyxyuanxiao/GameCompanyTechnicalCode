for /f "tokens=2 delims= " %%i in ('svn info ^| findstr "Revision"') do set rev=%%i
echo %rev%
