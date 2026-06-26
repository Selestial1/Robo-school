@echo off
chcp 65001 >nul
cd /d "%~dp0"

set "GIT="
if exist "C:\Program Files\Git\cmd\git.exe" set "GIT=C:\Program Files\Git\cmd\git.exe"
if exist "C:\Program Files (x86)\Git\cmd\git.exe" set "GIT=C:\Program Files (x86)\Git\cmd\git.exe"

if "%GIT%"=="" (
    echo Git не найден. Загрузите файлы вручную:
    echo 1. Откройте https://github.com/Selestial1/robo-school
    echo 2. Upload files: about.html trainers.html pricing.html apply.html
    echo    Dockerfile start.bat assets/js/cod.js assets/css/style.css index.html
    echo 3. На Render: Manual Deploy - Deploy latest commit
    pause
    exit /b 1
)

echo Отправка многостраничности на GitHub...
"%GIT%" add Dockerfile index.html about.html trainers.html pricing.html apply.html status.html assets/css/style.css assets/js/cod.js start.bat
"%GIT%" commit -m "Многостраничность: тренеры, курсы, заявка + исправлен Dockerfile"
"%GIT%" push origin main
echo.
echo Готово! На Render нажмите Manual Deploy - Deploy latest commit
pause
