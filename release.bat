@echo off
chcp 65001 > nul
setlocal enabledelayedexpansion

:: ============================================
::           НАСТРОЙКИ
:: ============================================
set REPO_URL=https://github.com/JamesFlorens/Bot_SCT_Auton.git
set REPO_NAME=JamesFlorens/Bot_SCT_Auton
set EXE_NAME=Test.exe
set BACKUP_DIR=Backups
:: ============================================

:: ПРОВЕРКА СВЯЗИ С GITHUB
git remote get-url origin >nul 2>&1
if %errorlevel% neq 0 (
    echo [!] Связь с GitHub не найдена. Восстановление...
    git init >nul 2>&1
    git remote add origin %REPO_URL% >nul 2>&1
    echo ✅ Связь восстановлена.
)

:menu
cls
echo ============================================
echo       УПРАВЛЕНИЕ ПРОЕКТОМ (ULTIMATE FIX)
echo ============================================
echo 1. ОЧИСТКА: Удалить .vs и мусор (Build Clean)
echo 2. БЭКАП: Создать полный ZIP проекта
echo 3. ПУШ КОДА: Сохранить исходники на GitHub
echo 4. РЕЛИЗ: Новая версия + EXE (X.Y.Z)
echo 5. ВЫХОД
echo ============================================
set /p choice="Выберите действие (1-5): "

if "%choice%"=="1" goto clean
if "%choice%"=="2" goto backup
if "%choice%"=="3" goto manual_push
if "%choice%"=="4" goto auto_release
if "%choice%"=="5" exit
goto menu

:clean
echo.
echo [!] Очистка... Закройте Visual Studio!
rd /s /q .vs 2>nul
rd /s /q obj 2>nul
:: bin не удаляем, так как там лежит EXE для релиза, но очистим лишнее
echo ✅ Временные файлы удалены.
pause
goto menu

:backup
echo.
if not exist %BACKUP_DIR% mkdir %BACKUP_DIR%
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set ts=!datetime:~0,4!-!datetime:~4,2!-!datetime:~6,2!_!datetime:~8,2!-!datetime:~10,2!
set B_NAME=%BACKUP_DIR%/Backup_%ts%.zip
tar -a -c -f %B_NAME% --exclude=%BACKUP_DIR% --exclude=.vs --exclude=.git .
echo ✅ Бэкап создан: %B_NAME%
pause
goto menu

:manual_push
echo.
git branch
set /p TARGET_BRANCH="В какую ветку пушим? (master/main): "
if "!TARGET_BRANCH!"=="" set TARGET_BRANCH=master
set /p msg="Комментарий: "
:: Игнорируем bin и obj при пуше кода
git add .
git commit -m "%msg%"
git push origin !TARGET_BRANCH! --force
pause
goto menu

:auto_release
echo.
git branch
set /p TARGET_BRANCH="Ветка для релиза? (master/main): "
if "!TARGET_BRANCH!"=="" set TARGET_BRANCH=master

echo [!] Синхронизация версий...
git fetch --tags >nul 2>&1
set LATEST_TAG=
for /f "tokens=*" %%a in ('gh release list --repo %REPO_NAME% --limit 1 --json tagName --template "{{range .}}{{.tagName}}{{end}}"') do set LATEST_TAG=%%a

if "%LATEST_TAG%"=="" (
    set /p NEW_VER="Начальная версия (например, 1.0.0): "
) else (
    echo [!] Последняя версия: %LATEST_TAG%
    set CLEAN_TAG=%LATEST_TAG:v=%
    for /f "tokens=1,2,3 delims=." %%a in ("!CLEAN_TAG!") do (
        set /a next_num=%%c + 1
        set NEW_VER=%%a.%%b.!next_num!
    )
)

echo [!] Новая версия: %NEW_VER%
set /p desc="Описание: "

:: Обновляем файл и пушим только его
echo %NEW_VER%> version.txt
git add version.txt
git commit -m "Bump version to %NEW_VER%"
git push origin !TARGET_BRANCH!

:: Создаем релиз
gh release create v%NEW_VER% --repo %REPO_NAME% --target !TARGET_BRANCH! --title "v%NEW_VER%" --notes "%desc%"

:: Загрузка EXE
set FOUND=0
for /r bin %%f in (%EXE_NAME%) do (
    if "%%~nxf"=="%EXE_NAME%" (
        echo [!] Загрузка: %%f
        gh release upload v%NEW_VER% "%%f" --repo %REPO_NAME% --clobber
        set FOUND=1
    )
)

if %FOUND%==1 (
    echo ✅ УСПЕХ! Версия %NEW_VER% в сети.
    start https://github.com/%REPO_NAME%/releases
) else (
    echo ❌ EXE не найден в bin! Сделайте Build.
)
pause
goto menu