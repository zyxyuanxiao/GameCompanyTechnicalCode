@echo.
echo ========Add evnPath=========
xcopy "%~dp0protoc.exe" C:\Windows\System32\
cd %~dp0protobuf-3.14.0\python\
python setup.py build 
python setup.py install 
python setup.py test
pause