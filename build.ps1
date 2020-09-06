msbuild OpenVRDeviceBattery.sln /t:Build /p:Configuration=Release /p:Platform="Any CPU"
Compress-Archive -Force "OpenVRDeviceBattery\bin\Release\*" "OpenVRDeviceBattery.zip"