# Script to Backup DMI application and Deploy fresh build

#Define servers in an array
#$servers = @("virdmswebd001w.dmz.infousa.com","virdmswebd002w.dmz.infousa.com")
$servers = $env:servers -split (",")
#$servers = $webservers -split (",")

#Site name is the input parameter
$sitename = $env:sitename

#Backup server is the input parameter
$backupserver = $env:backupserver

#Fresh build source is the input parameter
$source = $env:source

#Application pool name is the input parameter
$appPoolName =$env:apppoolname

#script blocks to start/ stop website
$stopsiteblock = {Stop-WebSite $args[0]};
$startsiteblock = {Start-WebSite $args[0]};
$getapppoolstatus =  {Import-Module WebAdministration; Get-WebAppPoolState $args[0]}
$stopapppool = {Stop-WebAppPool -Name $args[0]};
$startapppool = {Start-WebAppPool -Name $args[0]};

foreach ($server in $servers) 
{
	# Stop website
	Invoke-Command -ComputerName $server -ScriptBlock $stopsiteblock -ArgumentList $siteName
	# Wait time
	Start-Sleep -s 5
	# Stop Application Pool
	$AppPoolState = Invoke-Command -ComputerName $server -ScriptBlock $getapppoolstatus -ArgumentList $appPoolName
	If($AppPoolState.Value -eq "Started")
	{
		Invoke-Command -ComputerName $server -ScriptBlock $stopapppool -ArgumentList $appPoolName
	}
	# Wait time
	Start-Sleep -s 5

	# Backup the current files from the server
	# Keep only 10 latest backups delete the rest
	gci $backupserver -Recurse| where{-not $_.PsIsContainer}| sort  LastWriteTime -desc| select -Skip 10| Remove-Item -Recurse -Force
	
	# Get the current date
	$date = Get-Date -Format M-dd-yyyy
	
	# Get the source from where we need to take back up
	$bkupSource = "\\$server\Public\Websites\$sitename"
	
	# Create the destination folder by appending current date
	$bkupDest = "$backupserver\Web_"+$date +"_"+$server+".zip"
	
	# If backup already exists for current date, remove the backup
	If(Test-path $bkupDest) {Remove-item $bkupDest}
	Add-Type -assembly "system.io.compression.filesystem"
	[io.compression.zipfile]::CreateFromDirectory($bkupSource, $bkupDest)

	# wait time
	Start-Sleep -s 10

	# Define the destination folder
	$destination = "\\$server\Public\Websites\$sitename"

	try
	{
		# Remove all the files 
		$Exclusions = "web.config","f5.htm","appconfig.production.json","appsettings.Production.json","appsettings.Staging.json","@fortawesome\*"
		Get-ChildItem -Path "$destination\" -Exclude $Exclusions -Include *.* -File -Recurse -Force | foreach { $_.Delete()}

		# Wait time
		Start-Sleep -s 10

		# Copy files
		Copy-Item  -Path $source -destination $destination -Recurse -ErrorAction SilentlyContinue -exclude web.config,appconfig.production.json,appsettings.Production.json		
	}
	catch
	{
		$errorDestination = "\\$server\Public\Websites\$sitename\errors.txt"
		$_ | Out-File $errorDestination -Append
	}

	# Start the website
	Invoke-Command -ComputerName $server -ScriptBlock $startsiteblock -ArgumentList $siteName
	# Start Application Pool
	$AppPoolState = Invoke-Command -ComputerName $server -ScriptBlock $getapppoolstatus -ArgumentList $appPoolName
	If($AppPoolState.Value -eq "Stopped")
	{
		Invoke-Command -ComputerName $server -ScriptBlock $startapppool -ArgumentList $appPoolName
	}
}