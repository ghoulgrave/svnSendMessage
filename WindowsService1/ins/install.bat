%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil.exe E:\0A项目资料\DailyRecord\WindowsService1\WindowsService1\bin\Debug\WindowsService1.exe
Net Start SVNMES
sc config SVNMES start= auto
pause