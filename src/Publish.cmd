@echo off
dotnet restore
dotnet publish -c Release -o ../prod
cd ..
cd prod
del /q *.pdb
del /q *.config
