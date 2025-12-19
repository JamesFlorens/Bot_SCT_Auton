@echo off
chcp 65001 > nul
setlocal enabledelayedexpansion

:: ============================================
::           НАСТРОЙКИ
:: ============================================
set REPO_NAME=JamesFlorens/Bot_SCT_Auton
set EXE_NAME=Test.exe
set BACKUP_DIR=Backups
:: ============================================

:menu
cls
echo ============================================
echo       УПРАВЛЕНИЕ ПРОЕКТОМ (FULL + MANUAL)
echo ============================================
echo 1. ОЧИСТКА (.vs)
echo 2. БЭКАП (Полный ZIP)
echo 3. ПУШ КОДА (В выбранную ветку)
echo 4. РЕЛИЗ (Авто-версия X.Y.Z)
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
echo [!] ВНИМАНИЕ: Закройте Visual Studio перед очисткой!
rd /s /q .vs 2>nul
echo ✅ Очистка папки .vs завершена.
pause
goto menu

:backup
echo.
if not exist %BACKUP_DIR% mkdir %BACKUP_DIR%
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set ts=!datetime:~0,4!-!datetime:~4,2!-!datetime:~6,2!_!datetime:~8,2!-!datetime:~10,2!
set B_NAME=%BACKUP_DIR%/Backup_%ts%.zip
echo [!] Создание архива...
tar -a -c -f %B_NAME% --exclude=%BACKUP_DIR% --exclude=.vs .
echo ✅ Бэкап создан: %B_NAME%
pause
goto menu

:manual_push
echo.
echo [!] Список ваших веток:
git branch
set /p TARGET_BRANCH="В какую ветку пушим код? (например, master): "
if "!TARGET_BRANCH!"=="" set TARGET_BRANCH=master

set /p msg="Комментарий к пушу: "
echo [!] Синхронизация...
git pull origin !TARGET_BRANCH! --rebase
git add .
git commit -m "%msg%"
git push origin !TARGET_BRANCH!
echo ✅ Код успешно отправлен в ветку !TARGET_BRANCH!.
pause
goto menu

:auto_release
echo.
echo [!] Список ваших веток:
git branch
set /p TARGET_BRANCH="В какой ветке создаем релиз? (например, master): "
if "!TARGET_BRANCH!"=="" set TARGET_BRANCH=master

echo [!] Синхронизация с GitHub...
git pull origin !TARGET_BRANCH! --rebase

:: Поиск версии
set LATEST_TAG=
for /f "tokens=*" %%a in ('gh release list --repo %REPO_NAME% --limit 1 --json tagName --template "{{range .}}{{.tagName}}{{end}}"') do set LATEST_TAG=%%a

if "%LATEST_TAG%"=="" (
    echo [!] Релизов не найдено.
    set /p NEW_VER="Введите начальную версию (например, 1.0.0): "
) else (
    echo [!] Последняя версия на GitHub: %LATEST_TAG%
    set CLEAN_TAG=%LATEST_TAG:v=%
    for /f "tokens=1,2,3 delims=." %%a in ("!CLEAN_TAG!") do (
        set v1=%%a
        set v2=%%b
        set v3=%%c
        if "!v2!"=="" set v2=0
        if "!v3!"=="" set v3=0
        set /a next_num=!v3! + 1
        set NEW_VER=!v1!.!v2!.!next_num!
    )
)

echo [!] Новая версия: %NEW_VER%
set /p desc="Описание изменений: "

:: Обновление version.txt
echo %NEW_VER%> version.txt
git add version.txt
git commit -m "Bump version to %NEW_VER%"
git push origin !TARGET_BRANCH!

:: Создание релиза
echo [!] Публикация релиза v%NEW_VER%...
gh release create v%NEW_VER% --repo %REPO_NAME% --target !TARGET_BRANCH! --title "v%NEW_VER%" --notes "%desc%"

:: Загрузка EXE
set FOUND=0
for /r bin %%f in (%EXE_NAME%) do (
    if "%%~nxf"=="%EXE_NAME%" (
        echo [!] Загрузка актива: %%f
        gh release upload v%NEW_VER% "%%f" --repo %REPO_NAME% --clobber
        set FOUND=1
    )
)

if %FOUND%==1 (
    echo ✅ РЕЛИЗ %NEW_VER% ОПУБЛИКОВАН!
    start https://github.com/%REPO_NAME%/releases
) else (
    echo ❌ Файл %EXE_NAME% не найден. Соберите проект в VS!
)
pause
goto menu