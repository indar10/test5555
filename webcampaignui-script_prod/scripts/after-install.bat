C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -Command Expand-Archive D:\vsts-filedrop\artifact_drop\artifact2.zip -DestinationPath D:\VSTS-FILEDROP\artifact_drop\ -Force
C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -Command Expand-Archive D:\vsts-filedrop\artifact_drop\appnetcoreartifact.zip -DestinationPath D:\VSTS-FILEDROP\campaign.infogroup.com -Force
C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -Command xcopy.exe D:\VSTS-FILEDROP\campaign.infogroup.com\* D:\Private\Release\campaign.infogroup.com\ /E/S/Y
C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -Command rm D:\vsts-filedrop\artifact_drop\dist\*
C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -Command tar.exe -xvzf D:\vsts-filedrop\artifact_drop\nodejsartifact.tar.gz -C D:\vsts-filedrop\artifact_drop\
C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -Command rm D:\Private\Release\campaign.infogroup.com\wwwroot\*
C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -Command xcopy.exe D:\vsts-filedrop\artifact_drop\dist\* D:\Private\Release\campaign.infogroup.com\wwwroot\ /E/S/Y
C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -Command copy D:\public\default_aws_profile.txt D:\Private\Release\campaign.infogroup.com\
