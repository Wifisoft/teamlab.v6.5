set Configuration=Release
set DeployTo=INSTALL

%SystemRoot%\Microsoft.NET\Framework\v3.5\MSBuild.exe ..\..\_ci\projects\build.proj /p:BuildTargets=ReBuild /fl1 /flp1:LogFile=Build.log;Verbosity=Normal
if %errorlevel% == 0 %SystemRoot%\Microsoft.NET\Framework\v3.5\MSBuild.exe ..\..\_ci\projects\deploy.proj /fl1 /flp1:LogFile=Deploy.log;Verbosity=Normal
pause
