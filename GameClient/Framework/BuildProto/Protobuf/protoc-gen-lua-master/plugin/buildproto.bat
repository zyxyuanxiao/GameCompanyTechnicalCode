rem 切换到.proto协议所在的目录

cd  E:\Python_Project\protoc-gen-lua-master\protobuf\luascript\
rem 将当前文件夹中的所有协议文件转换为lua文件

for %%i in (*.proto) do (  

echo %%i

"E:\Python_Project\protoc3.12.exe" --plugin=protoc-gen-lua="E:\Python_Project\protoc-gen-lua-master\plugin\protoc-gen-lua.bat" --lua_out=. %%i


)

echo end

pause
