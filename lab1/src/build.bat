set v=%1
set frontend=D:\study\RP\RP\lab1\src\Frontend
set backend=D:\study\RP\RP\lab1\src\Backend
set textListener=D:\study\RP\RP\lab1\src\TextListener
set textRankCalc=D:\study\RP\RP\lab1\src\TextRankCalc
set frontendPublish=%frontend%\bin\Debug\netcoreapp2.2\publish
set backendPublish=%backend%\bin\Debug\netcoreapp2.2\publish
set textListenerPublish=%textListener%\bin\Debug\netcoreapp2.2\publish
set textRankCalcPublish=%textRankCalc%\bin\Debug\netcoreapp2.2\publish
set redis=D:\redis\64bit\redis-server.exe

if defined v (goto checkExist) else (goto error)
:checkExist
if not exist %v% (
    cd %frontend%
    dotnet publish
    cd %backend% 
    dotnet publish
    cd %textListener% 
    dotnet publish
    cd %textRankCalc%
    dotnet publish
    cd ..
    mkdir %v%
    cd %v%
    mkdir config
    cd config
    echo backendPort:5000 > config.txt
    echo frontendPort:5001 >> config.txt
    cd ..
    mkdir Backend
    mkdir Frontend
    mkdir TextListener
    mkdir TextRankCalc
    cd Backend
    xcopy %backendPublish% /S
    cd ..
    cd Frontend
    xcopy %frontendPublish% /S
    cd .. 
    cd TextListener
    xcopy %textListenerPublish% /S
    cd ..
    cd TextRankCalc
    xcopy %textRankCalcPublish% /S
    cd ..
    echo start %redis% > run.bat
	echo start dotnet Frontend\Frontend.dll  >> run.bat
	echo start dotnet Backend\Backend.dll >> run.bat
	echo start dotnet TextListener\TextListener.dll >> run.bat
	echo start dotnet TextRankCalc\TextRankCalc.dll >> run.bat
	echo taskkill /IM dotnet.exe /F > stop.bat
    exit
) else (
    echo Version is alredy exist
    exit
)
:error
echo Version is not defined

