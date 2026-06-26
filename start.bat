@echo off
chcp 65001 >nul
cd /d "%~dp0"
set "PATH=C:\Program Files\dotnet;%PATH%"

where dotnet >nul 2>&1
if errorlevel 1 (
    echo.
    echo  ОШИБКА: .NET SDK не установлен
    echo  -------------------------------------------
    echo  Скачайте и установите .NET 10 SDK:
    echo  https://dotnet.microsoft.com/download/dotnet/10.0
    echo.
    echo  После установки закройте это окно и запустите start.bat снова.
    echo.
    pause
    exit /b 1
)

cd server
echo.
echo  ROBO.SCHOOL — запуск сервера с базой данных
echo  -------------------------------------------
echo  Главная:  http://localhost:5080/index.html
echo  О школе:  http://localhost:5080/about.html
echo  Тренеры:  http://localhost:5080/trainers.html
echo  Цены:     http://localhost:5080/pricing.html
echo  Заявка:   http://localhost:5080/apply.html
echo  Админ:    http://localhost:5080/admin.html
echo  API:      http://localhost:5080/api/health
echo.
echo  Облако:   см. DEPLOY.md (Neon + Render)
echo.
dotnet run --urls http://localhost:5080
pause
