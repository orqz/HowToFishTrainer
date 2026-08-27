@echo off
setlocal enabledelayedexpansion
title HowToFishTrainer installer

echo.
echo   HowToFishTrainer - installer
echo   ============================
echo.

set "GAME="

:: 1. already sitting in the game folder?
if exist "How to Fish.exe" set "GAME=%CD%"

:: 2. ask Steam where it lives
if not defined GAME (
  for /f "tokens=2,*" %%A in ('reg query "HKCU\Software\Valve\Steam" /v SteamPath 2^>nul ^| find "SteamPath"') do set "STEAM=%%B"
)

if defined STEAM (
  set "STEAM=!STEAM:/=\!"
  if exist "!STEAM!\steamapps\common\How to Fish\How to Fish\How to Fish.exe" (
    set "GAME=!STEAM!\steamapps\common\How to Fish\How to Fish"
  )
  :: other library drives listed in libraryfolders.vdf
  if not defined GAME (
    for /f "tokens=2 delims=^"" %%P in ('findstr /i /c:"\"path\"" "!STEAM!\steamapps\libraryfolders.vdf" 2^>nul') do (
      set "LIB=%%P"
      set "LIB=!LIB:\\=\!"
      if exist "!LIB!\steamapps\common\How to Fish\How to Fish\How to Fish.exe" (
        set "GAME=!LIB!\steamapps\common\How to Fish\How to Fish"
      )
    )
  )
)

if not defined GAME (
  echo   Could not find How to Fish automatically.
  echo.
  echo   Put this installer in your game folder - the one containing
  echo   "How to Fish.exe" - and run it again.
  echo.
  echo   In Steam: right-click How to Fish, Manage, Browse local files,
  echo   then open the "How to Fish" folder inside.
  echo.
  pause
  exit /b 1
)

echo   Found the game:
echo     !GAME!
echo.

xcopy /e /i /y /q "%~dp0BepInEx" "!GAME!\BepInEx" >nul
copy /y "%~dp0version.dll" "!GAME!\" >nul 2>nul
copy /y "%~dp0doorstop_config.ini" "!GAME!\" >nul 2>nul
copy /y "%~dp0.doorstop_version" "!GAME!\" >nul 2>nul
del /q "!GAME!\winhttp.dll" >nul 2>nul

echo   Installed.
echo.
echo   Launch the game and press DELETE to open the menu.
echo.
pause
