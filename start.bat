@echo off
chcp 65001 >nul
cd /d "%~dp0server"
echo.
echo  ROBO.SCHOOL — запуск сервера с базой данных
echo  -------------------------------------------
echo  Сайт:    http://localhost:5080
echo  Админ:   http://localhost:5080/admin.html
echo  Статус:  http://localhost:5080/status.html
echo  API:     http://localhost:5080/api/health
echo.
echo  Облако:  см. DEPLOY.md (Neon + Render)
echo.
dotnet run --urls http://localhost:5080
pause
