SET USERNAME=username
SET HOSTNAME=hostname
SET PASSWORD=password
dotnet publish EPaperApp\EPaperApp.csproj -r linux-arm64 /p:ShowLinkerSizeComparison=true -c Debug
pushd .\EPaperApp\bin\Debug\net8.0\linux-arm64\publish
"c:\Program Files\PuTTY\pscp" -pw %PASSWORD% -v -r .\* %USERNAME%@%HOSTNAME%:/home/%USERNAME%/epaper
popd