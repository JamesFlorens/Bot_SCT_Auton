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
for /f "tokens=*" %%i in ('git rev-parse --abbrev-ref HEAD 2^>nul') do set CUR_B=%%i
echo ============================================
echo       УПРАВЛЕНИЕ ПРОЕКТОМ (TOTAL SYNC)
echo ============================================
echo Текущая ветка: %CUR_B%
echo --------------------------------------------
echo 1. ОЧИСТКА: Удалить .vs и obj (Build Clean)
echo 2. БЭКАП: Создать ZIP всего проекта (локально)
echo 3. ПУШ: Сохранить ВСЕ файлы на GitHub (main)
echo 4. РЕЛИЗ: Новая версия + ПАПКА СБОРОК (ZIP)
echo 5. ФИКС ВЕТОК: Master -> Main
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
tar -a -c -f "%BACKUP_DIR%/FullBackup_%ts%.zip" .
echo ✅ Локальный бэкап создан в папке %BACKUP_DIR%.
pause
goto menu

:manual_push
echo.
if exist "main" (rd /s /q "main" 2>nul & del /f /q "main" 2>nul)
set /p TARGET_BRANCH="Ветка (Enter для main): "
if "!TARGET_BRANCH!"=="" set TARGET_BRANCH=main
set /p msg="Комментарий: "

:: Копируем версию к EXE
if exist version.txt (
    for /r bin %%f in (%EXE_NAME%) do copy /y version.txt "%%~dpf" >nul
)

git pull origin !TARGET_BRANCH! --rebase >nul 2>&1
git add -A
git rm --cached main >nul 2>&1
git commit -m "%msg%"
git push origin !TARGET_BRANCH!
echo ✅ Файлы отправлены на GitHub.
pause
goto menu

:auto_release
echo.
if exist "main" (rd /s /q "main" 2>nul & del /f /q "main" 2>nul)
set /p TARGET_BRANCH="Ветка для релиза? (Enter для main): "
if "!TARGET_BRANCH!"=="" set TARGET_BRANCH=main

echo [!] Получение последней версии с GitHub...
git fetch --tags >nul 2>&1
set L_TAG=
for /f "tokens=*" %%a in ('gh release list --repo %REPO_NAME% --limit 1 --json tagName --template "{{range .}}{{.tagName}}{{end}}"') do set L_TAG=%%a

if "%L_TAG%"=="" (
    set /p NEW_VER="Начальная версия (например, 1.0.0): "
) else (
    set CLEAN=!L_TAG:v=!
    for /f "tokens=1,2,3 delims=." %%a in ("!CLEAN!") do (
        set /a n=%%c + 1
        set NEW_VER=%%a.%%b.!n!
    )
)
echo [!] Новая версия: !NEW_VER!
echo !NEW_VER!> version.txt

:: Копируем версию во все папки билда (для C# кода)
for /r bin %%f in (%EXE_NAME%) do copy /y version.txt "%%~dpf" >nul

git add -A
git rm --cached main >nul 2>&1
git commit -m "Release v!NEW_VER!"
git push origin !TARGET_BRANCH!

echo [!] Создание релиза v!NEW_VER!...
set /p desc="Описание изменений: "
gh release create v!NEW_VER! --repo %REPO_NAME% --target !TARGET_BRANCH! --title "v!NEW_VER!" --notes "!desc!"

:: Поиск папки с ботом и упаковка
set FOUND=0
for /r bin %%f in (%EXE_NAME%) do (
    if "%%~nxf"=="%EXE_NAME%" (
        set "FOLDER_PATH=%%~dpf"
        echo [!] Найдена папка сборки: !FOLDER_PATH!
        
        :: Создаем временный ZIP папки
        set ZIP_NAME=Bot_v!NEW_VER!_Full.zip
        pushd "!FOLDER_PATH!"
        tar -a -c -f "../../!ZIP_NAME!" *
        popd
        
        echo [!] Загрузка архива на GitHub...
        gh release upload v!NEW_VER! "!ZIP_NAME!" --repo %REPO_NAME% --clobber
        
        :: АВТОМАТИЧЕСКОЕ УДАЛЕНИЕ ВРЕМЕННОГО ZIP
        echo [!] Удаление временного архива !ZIP_NAME!...
        del /f /q "!ZIP_NAME!"
        
        set FOUND=1
        goto upload_done
    )
)

:upload_done
if !FOUND!==1 (
    echo ✅ УСПЕХ! В релиз загружена вся папка. Временный ZIP удален.
    start https://github.com/%REPO_NAME%/releases
) else (
    echo ❌ Ошибка: Папка с %EXE_NAME% не найдена.
)
pause
goto menu

:fix_branches
echo.
git branch -M main
git push -u origin main --force
echo ✅ Ветка исправлена на main.
pause
goto menu