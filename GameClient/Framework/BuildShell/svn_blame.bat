@echo off
REM @set "n=&echo;"
REM @setlocal enableextensions enabledelayedexpansion
REM for /f "usebackq delims=" %%i in (`"svn blame --xml %1"`) do (
REM set FileInfo=!FileInfo!%n%%%i
REM )
REM @echo !FileInfo!

@set "n=&echo;"
@setlocal enableextensions enabledelayedexpansion
for /f "usebackq delims=" %%i in (`"svn diff -rPREV %1"`) do (
set FileInfo=!FileInfo!%n%%%i
)
@echo !FileInfo!