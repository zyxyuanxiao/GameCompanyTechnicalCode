rem �л���.protoЭ�����ڵ�Ŀ¼

cd  E:\Python_Project\protoc-gen-lua-master\protobuf\luascript\
rem ����ǰ�ļ����е�����Э���ļ�ת��Ϊlua�ļ�

for %%i in (*.proto) do (  

echo %%i

"E:\Python_Project\protoc3.12.exe" --plugin=protoc-gen-lua="E:\Python_Project\protoc-gen-lua-master\plugin\protoc-gen-lua.bat" --lua_out=. %%i


)

echo end

pause
