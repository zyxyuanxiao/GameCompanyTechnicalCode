svn update -r HEAD
for /f "tokens=2 delims= " %%i in ('svn info ^| findstr "Revision"') do set rev=%%i
cd Assets&cd StreamingAssets
@echo off
for /f "tokens=2 delims=," %%j in (packVersionInfo) do set lastrev=%%j
setlocal enabledelayedexpansion
for /f "tokens=*" %%k in (packVersionInfo) do (
    echo before %%k
    set "str=%%k"
    set "str=!str:%lastrev%=%rev%!"
    echo after !str!
    goto out
)
:out
echo !str! > packVersionInfo
