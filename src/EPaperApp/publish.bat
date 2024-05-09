dotnet publish -r linux-arm64 /p:ShowLinkerSizeComparison=true -c Debug
pushd .\bin\Debug\net8.0\linux-arm64\publish
"c:\Program Files\PuTTY\pscp" -pw Odegaard1 -v -r .\* morten@192.168.1.122:/home/morten/epaper
popd