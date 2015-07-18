set path=C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319;%path%

if "%1" == "" goto noversion
msbuild build.proj /p:SchemaVersion="%1"  /t:Migrate
goto end

:noversion
msbuild build.proj /t:Migrate
