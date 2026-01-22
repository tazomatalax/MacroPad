@echo OFF
setlocal

if "%~1"=="" goto blank

SET FOLDER=MacroPad.%1.bin
SET RELEASE_DIR=..\Releases

rmdir /S /Q RSoft.MacroPad\bin\publish 2>nul

echo Building release %1...
dotnet publish RSoft.MacroPad\RSoft.MacroPad.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o RSoft.MacroPad\bin\publish

if errorlevel 1 (
    echo Build failed!
    goto end
)

mkdir %FOLDER% 2>nul
copy /Y RSoft.MacroPad\bin\publish\* %FOLDER%
del %FOLDER%\*.deps.json 2>nul
del %FOLDER%\*.pdb 2>nul

if not exist %RELEASE_DIR% mkdir %RELEASE_DIR%

cd %FOLDER%
7z a ..\..\%RELEASE_DIR%\RSoft.MacroPad.%1.7z *
cd ..

rmdir /S /Q %FOLDER%

echo ----
echo Release MacroPad.%1 done
echo ----
goto end

:blank
echo ----
echo Usage: release.bat ^<version^>
echo Example: release.bat 2.0.0
echo ----

:end
endlocal