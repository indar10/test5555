@ECHO OFF
if not "%1" == "max" start /MAX cmd /c %0 max & exit/b
:choice 
ECHO Choices:
ECHO - If you want to create a new feature branch, please type "a" OR
ECHO - If you want to commit the feature branch locally, please type "b" OR
ECHO - If you want to commit the feature branch to origin, please type "c" OR
ECHO - If you want to merge the feature branch with develop(to merge your code with other commits in develop branch), please type "d" OR
ECHO - If you want to switch branches, please type "e" OR
ECHO - If you want to undo/ revert commits, please type "f" ELSE
ECHO - Type "g" to exit
set /P z=
if /I "%z%" EQU "a" goto :CreateFeatureBranch
if /I "%z%" EQU "b" goto :CommitFeatureBranchLocally
if /I "%z%" EQU "c" goto :CommitFeatureBranchToOrigin
if /I "%z%" EQU "d" goto :MergeFeatureBranchWithDevelop
if /I "%z%" EQU "e" goto :SwitchBranch
if /I "%z%" EQU "f" goto :UndoCommit
if /I "%z%" EQU "g" goto :ExitBranch
goto :choice
	:CreateFeatureBranch
		set /p TicketID=Please mention the ticket ID:
		set "branch=VSTSID-%TicketID%"
		git pull
		git checkout -b %branch% develop
		if errorlevel 1 ( 
			echo **********************************
			echo Please fix the error and try again 
			echo ********************************** && goto choice)
		ECHO
		ECHO ****************************
		ECHO New branch %branch% created
		ECHO ****************************
		ECHO
		goto choice
	:CommitFeatureBranchLocally
		set /p TicketID=Please mention the ticket ID:
		if %TicketID%==develop (git checkout develop) else (git checkout VSTSID-%TicketID%)
		if errorlevel 1 ( 
			echo **********************************
			echo Please fix the error and try again 
			echo ********************************** && goto choice)
		git pull
		set /p CommitMessage=Please mention the commit message:
		git commit -am "%CommitMessage%"
		if errorlevel 1 ( 
			echo **********************************
			echo Please fix the error and try again 
			echo ********************************** && goto choice)
		ECHO
		ECHO *******************************************
		ECHO Branch VSTSID-%TicketID% committed locally
		ECHO *******************************************
		ECHO
		goto choice
	:CommitFeatureBranchToOrigin
		set /p TicketID=Please mention the ticket ID:
		if %TicketID%==develop (git checkout develop) else (git checkout VSTSID-%TicketID%)
		if errorlevel 1 ( 
			echo **********************************
			echo Please fix the error and try again 
			echo ********************************** && goto choice)
		
		git pull origin
		set /p Files=Please mention the file names with spaces(if multiple files) and in case of all files type dot(.):
		git add %Files%
		if errorlevel 1 ( 
			echo **********************************
			echo Please fix the error and try again 
			echo ********************************** && goto choice)
			
		set /p CommitMessage=Please mention the commit message:
		git commit -m "%CommitMessage%"
		if errorlevel 1 ( 
			echo **********************************
			echo Please fix the error and try again 
			echo ********************************** && goto choice)
		git push --set-upstream origin VSTSID-%TicketID%
		if errorlevel 1 ( 
			echo **********************************
			echo Please fix the error and try again 
			echo ********************************** && goto choice)
		ECHO
		ECHO **********************************************
		ECHO Branch VSTSID-%TicketID% committed and pushed
		ECHO **********************************************
		ECHO
		goto choice
	:MergeFeatureBranchWithDevelop		
		set /p TicketID=Please mention the ticket ID:
		if %TicketID%==develop (git checkout develop) else (git checkout VSTSID-%TicketID%)
		if errorlevel 1 ( 
			echo **********************************
			echo Please fix the error and try again 
			echo ********************************** && goto choice)
		git pull
		git rebase develop
		if errorlevel 1 ( 
			echo **********************************
			echo Please fix the error and try again 
			echo ********************************** && goto choice)
		ECHO
		ECHO ***************************
		ECHO Branch merged with All Develop branch commits.
		ECHO Please raise a Pull-request(PR) for code review.
		ECHO ***************************
		ECHO
		goto choice
	:SwitchBranch
		set /p TicketID=Please mention the ticket ID you wish to, switch to:
		if %TicketID%==develop (git checkout develop) else (git checkout VSTSID-%TicketID%)
		if errorlevel 1 ( 
			echo **********************************
			echo Please fix the error and try again 
			echo ********************************** && goto choice)
		ECHO
		ECHO ***************************
		ECHO Branch switched sucessfully
		ECHO ***************************
		ECHO		
		goto choice
	:UndoCommit
		:choice1
			ECHO Do you want to undo/ revert a particualar commit OR
			ECHO all the commits after a particular commit[P/A]?
			set /P y=
			if /I "%y%" EQU "p" goto :RevertCommit
			if /I "%y%" EQU "a" goto :ResetCommit
			goto :choice1
			:RevertCommit
				set /p CommitID=Please mention the commit ID you wish to revert:
				git revert %CommitID%
				if errorlevel 1 ( 
					echo **********************************
					echo Please fix the error and try again 
					echo ********************************** && goto choice)
				ECHO
				ECHO ***************************
				ECHO Commit reverted sucessfully
				ECHO ***************************
				ECHO		
				goto choice
			:ResetCommit
				set /p TicketID=Please mention the ticket ID you wish to revert commit from:
				if %TicketID%==develop (git checkout develop) else (git checkout VSTSID-%TicketID%)
				set /p CommitID=Please mention the commit ID you wish to revert(you will get the commit ID from Azure devops portal):
				set /p x=All commits that came after this version will be effectively undone; your project will be exactly as it was at that point in time.Are you sure[Y/N]?
				if /I "%x%" EQU "y" (git reset --hard %CommitID%) else (goto choice)				
				if errorlevel 1 ( 
					echo **********************************
					echo Please fix the error and try again 
					echo ********************************** && goto choice)
				ECHO
				ECHO ***************************
				ECHO Commit reverted sucessfully
				ECHO ***************************
				ECHO		
				goto choice
	:ExitBranch
		exit