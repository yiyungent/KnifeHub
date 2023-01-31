@echo off
:start
choice /t 5 /d y /n >nul     ###定时5s
tasklist|find /i "KnifeHub.Web"    ###寻找有无 KnifeHub.Web 进程
if %errorlevel%==0 (    ###如果存在该进程
echo "yes"
) else (    ###如果不存在，则自行启动 KnifeHub.Web.exe
echo "No"
start KnifeHub.Web.exe
)
goto start