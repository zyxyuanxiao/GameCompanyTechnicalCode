#!/bin/bash
# 64 Bit Version
mkdir -p window/x86_64

cd luajit-2.1
mingw32-make clean

mingw32-make BUILDMODE=static CC="gcc -m64 -O2" XCFLAGS=-DLUAJIT_ENABLE_GC64
cp src/libluajit.a ../window/x86_64/libluajit.a
mingw32-make clean

cd ..

CCFLAGS="-m64 -O2 -c -std=gnu99 -I. -I./luajit-2.1/src -I./luasocket"

gcc $CCFLAGS tolua.c 
gcc $CCFLAGS int64.c
gcc $CCFLAGS uint64.c
gcc $CCFLAGS pb.c
gcc $CCFLAGS lpeg.c
gcc $CCFLAGS struct.c
gcc $CCFLAGS cjson/strbuf.c 
gcc $CCFLAGS cjson/lua_cjson.c 
gcc $CCFLAGS cjson/fpconv.c 


CPPFLAGS="-std=c++11 -m64 -c -O2 -Igdata/ -Iluajit-2.1/src"

g++ $CPPFLAGS gdata/CSharpExport.cpp
g++ $CPPFLAGS gdata/GData.cpp
g++ $CPPFLAGS gdata/LuaExport.cpp

LINKFLAGS="-lws2_32 -Wl,--whole-archive ./window/x86_64/libluajit.a -Wl,--no-whole-archive -static-libgcc -static-libstdc++ -Wl,-Bstatic -lstdc++ -lpthread -Wl,-Bdynamic"

tolua_objs="tolua.o int64.o uint64.o pb.o lpeg.o struct.o strbuf.o lua_cjson.o fpconv.o"

gdata_objs='CSharpExport.o GData.o LuaExport.o'

g++ -shared -o Plugins/x86_64/tolua.dll $tolua_objs $gdata_objs $LINKFLAGS
rm *.o