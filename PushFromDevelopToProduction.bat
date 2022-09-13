@echo off
git pull
set /p CurrentBranch=Please mention your current active branch in VS?
git stash
set /p SourceBranch=What is the source branch from where you need to merge?
git checkout %SourceBranch%
git pull
set /p DestinationBranch=What is the destination branch?
git checkout %DestinationBranch%
git pull
git merge %SourceBranch%
git push origin %DestinationBranch%
git checkout %CurrentBranch%
git stash pop



pause
	

