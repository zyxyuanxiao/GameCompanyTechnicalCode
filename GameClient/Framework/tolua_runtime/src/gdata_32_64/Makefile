CC=g++
CFLAGS=-std=c++11 -m64 -c -O2 -I./ -I../luajit-2.1/src
DEPS=GData.h MemoryUsage.h

gdata: 
	g++ -lm -o myProgram main.o -L. -lmylib

%.o: %.cpp $(DEPS)
	$(CC) $(CFLAGS) $^

lib: GData.o CSharpExport.o LuaExport.o
	ar rcs gdata.a GData.o CSharpExport.o LuaExport.o

cleanup:
	rm -f *.o *.gch