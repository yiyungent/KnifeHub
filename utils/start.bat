@echo off

echo "Running! Please don't quit. Quit: Ctrl+C"

:start
REM 定时 5 秒
choice /t 5 /d y /n >nul     

REM tasklist|find /i "KnifeHub.Web"    ###寻找有无 KnifeHub.Web 进程
REM if %errorlevel%==0 (    ###如果存在该进程
REM echo "yes"
REM ) else (    ###如果不存在，则自行启动 KnifeHub.Web.exe
REM echo "No"
REM start KnifeHub.Web.exe
REM )

REM 调用 ps
powershell.exe -executionpolicy remotesigned -File "corn.ps1"


goto start
