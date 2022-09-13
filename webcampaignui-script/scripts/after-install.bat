C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -Command Expand-Archive D:\vsts-filedrop\artifact_drop\artifact2.zip -DestinationPath D:\VSTS-FILEDROP\artifact_drop\ -Force
C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -Command Expand-Archive D:\vsts-filedrop\artifact_drop\appnetcoreartifact.zip -DestinationPath D:\VSTS-FILEDROP\idmsvnext.infogroup.com -Force
C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -Command xcopy.exe D:\VSTS-FILEDROP\idmsvnext.infogroup.com\* D:\VSTS-FILEDROP\web-published-code\idmsvnext.infogroup.com\ /E/S/Y
rmdir /s /q d:\VSTS-FILEDROP\artifact_drop\dist\
del d:\VSTS-FILEDROP\artifact_drop\nodejsartifact.tar
rmdir /s /q d:\VSTS-FILEDROP\WEB-PUBLISHED-CODE\idmsvnext.infogroup.com\wwwroot
C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -Command C:\'Program files'\7-Zip\7z x D:\VSTS-FILEDROP\artifact_drop\nodejsartifact.tar.gz -oD:\VSTS-FILEDROP\artifact_drop\ -y 
C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -Command C:\'Program files'\7-Zip\7z  x D:\VSTS-FILEDROP\artifact_drop\nodejsartifact.tar -oD:\VSTS-FILEDROP\artifact_drop -y
C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -Command md d:\VSTS-FILEDROP\WEB-PUBLISHED-CODE\idmsvnext.infogroup.com\wwwroot
C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -Command xcopy.exe D:\vsts-filedrop\artifact_drop\dist\* D:\VSTS-FILEDROP\web-published-code\idmsvnext.infogroup.com\wwwroot\ /E/S/Y
C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -Command xcopy D:\public\default_aws_profile.txt D:\VSTS-FILEDROP\web-published-code\idmsvnext.infogroup.com\ /Y
