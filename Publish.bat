@echo off

if not exist packages (
    md packages
)

for /R "packages" %%s in (*) do (
    del %%s
)

dotnet pack src/AspectCore.Container.Autofac --configuration Release --output packages
dotnet pack src/AspectCore.Container.DependencyInjection --configuration Release --output packages

for /R "packages" %%s in (*symbols.nupkg) do (
    del %%s
)

set /p key=input key:
set source=https://www.myget.org/F/aspectcore/api/v2/package

for /R "packages" %%s in (*.nupkg) do ( 
    call nuget push %%s -s %source% %key%
)

pause
