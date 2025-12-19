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

:menu
cls
:: Определение текущей ветки для заголовка
for /f "tokens=*" %%i in ('git rev-parse --abbrev-ref HEAD 2^>nul') do set CUR_B=%%i
if "%CUR_B%"=="" set CUR_B=not_init

echo ============================================
echo       УПРАВЛЕНИЕ ПРОЕКТОМ (TOTAL SYNC)
echo ============================================
echo Локальная ветка сейчас: %CUR_B%
echo Дефолтная ветка на GitHub: main
echo --------------------------------------------
echo 1. ОЧИСТКА: Удалить .vs и obj (Build Clean)
echo 2. БЭКАП: Создать ZIP всего проекта (включая всё)
echo 3. ПУШ: Сохранить ВСЕ файлы на GitHub (по дефолту main)
echo 4. РЕЛИЗ: Новая версия + EXE (X.Y.Z)
echo 5. ФИКС ВЕТОК: Переименовать master -> main
echo 6. ВЫХОД
echo ============================================
set /p choice="Выберите действие (1-6): "

if "%choice%"=="1" goto clean
if "%choice%"=="2" goto backup
if "%choice%"=="3" goto manual_push
if "%choice%"=="4" goto auto_release
if "%choice%"=="5" goto fix_branches
if "%choice%"=="6" exit
goto menu

:clean
echo.
echo [!] ВНИМАНИЕ: Закройте Visual Studio перед очисткой!
rd /s /q .vs 2>nul
rd /s /q obj 2>nul
echo ✅ Временные файлы удалены.
pause
goto menu

:backup
echo.
if not exist %BACKUP_DIR% mkdir %BACKUP_DIR%
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set ts=!datetime:~0,4!-!datetime:~4,2!-!datetime:~6,2!_!datetime:~8,2!-!datetime:~10,2!
set B_NAME=%BACKUP_DIR%/FullBackup_%ts%.zip
:: Архивируем всё без исключений
tar -a -c -f "%B_NAME%" .
echo ✅ Бэкап создан: %B_NAME%
pause
goto menu

:manual_push
echo.
set /p TARGET_BRANCH="В какую ветку пушим? (Enter для main): "
if "!TARGET_BRANCH!"=="" set TARGET_BRANCH=main
set /p msg="Комментарий к пушу: "
echo [!] Синхронизация...
git pull origin !TARGET_BRANCH! --rebase >nul 2>&1
git add -A
git commit -m "%msg%"
git push origin !TARGET_BRANCH!
echo ✅ ВСЕ файлы отправлены в ветку !TARGET_BRANCH!.
pause
goto menu

:auto_release
echo.
set /p TARGET_BRANCH="Ветка для релиза? (Enter для main): "
if "!TARGET_BRANCH!"=="" set TARGET_BRANCH=main

echo [!] Получение версии из GitHub...
git fetch --tags >nul 2>&1
set LATEST_TAG=
for /f "tokens=*" %%a in ('gh release list --repo %REPO_NAME% --limit 1 --json tagName --template "{{range .}}{{.tagName}}{{end}}"') do set LATEST_TAG=%%a

if "%LATEST_TAG%"=="" (
    set /p NEW_VER="Введите начальную версию (например, 1.0.0): "
) else (
    echo [!] Последняя версия: %LATEST_TAG%
    set CLEAN_TAG=%LATEST_TAG:v=%
    for /f "tokens=1,2,3 delims=." %%a in ("!CLEAN_TAG!") do (
        set /a next_num=%%c + 1
        set NEW_VER=%%a.%%b.!next_num!
    )
)

echo [!] Новая версия: %NEW_VER%
set /p desc="Описание изменений: "

:: Обновляем файл и пушим ВСЁ перед релизом
echo %NEW_VER%> version.txt
git add -A
git commit -m "Release v%NEW_VER%"
git push origin !TARGET_BRANCH!

:: Создаем релиз на GitHub
gh release create v%NEW_VER% --repo %REPO_NAME% --target !TARGET_BRANCH! --title "v%NEW_VER%" --notes "%desc%"

:: Поиск и загрузка EXE
set FOUND=0
for /r bin %%f in (%EXE_NAME%) do (
    if "%%~nxf"=="%EXE_NAME%" (
        echo [!] Загрузка актива: %%f
        gh release upload v%NEW_VER% "%%f" --repo %REPO_NAME% --clobber
        set FOUND=1
    )
)

if %FOUND%==1 (
    echo ✅ УСПЕХ! Релиз %NEW_VER% создан.
    start https://github.com/%REPO_NAME%/releases
) else (
    echo ❌ EXE не найден в bin. Соберите проект!
)
pause
goto menu

:fix_branches
echo.
for /f "tokens=*" %%i in ('git rev-parse --abbrev-ref HEAD 2^>nul') do set LOCAL_B=%%i
if "!LOCAL_B!"=="master" (
    echo [!] Обнаружена ветка master. Исправляем на main...
    git branch -M main
    git push -u origin main --force
    echo ✅ Теперь ваша локальная ветка - main и она связана с GitHub.
) else (
    echo ✅ У вас уже активна ветка !LOCAL_B!. Исправление не требуется.
)
pause
goto menu
