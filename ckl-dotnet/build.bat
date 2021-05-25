@echo off
cd ckl-dotnet
dotnet publish -c Release --output ..\dist --self-contained --nologo
cd ..\ckl-run
dotnet publish --output ..\dist --self-contained --nologo --runtime win-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true -c Release
cd ..\ckl-repl
dotnet publish --output ..\dist --self-contained --nologo --runtime win-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true -c Release
cd ..

