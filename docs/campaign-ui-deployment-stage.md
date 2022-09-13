# Campaign UI Deployment - Staging (On Premises)

## Website URL
https://vnext.idms.data-axle.com/

## Web Servers

 1. virdmswebd001w.dmz.infousa.com
 2. virdmswebd002w.dmz.infousa.com

## User Accounts

 1. IDMSDevSVC (idmsdevsvc@dmz.infousa.com)
 2. IDMSDevSVC (IUSA\IDMSDevSVC)
   
## Steps for Deployment

 1. Open Azure DevOps websites.
 2. Under it select [IDMS_NG](https://infogroupidms.visualstudio.com/IDMS_NG) project.
 3. Go to the pipeline section.
 4. Out of the four pipelines, select **IDMS_NG-WEBSITE-CI** pipeline.
 5. You will see the following actions in the action menu.
	  - Edit
	  - Run Pipeline
	  - Manage Security
	  - Rename/move
	  - Delete
6. Select Run Pipeline option.
7. This will trigger the CI pipeline.
8. Wait for 15-20 minutes for CI process to get completed.
9. CI on completion will again trigger the CD process.
10. To monitor it, move to Releases section in Azure DevOps.
11. Once the release gets completed move take the remote of virdmswebd001w server.
12. Go to the website folder 
    D:\Public\Websites
13. Create a back of `IDMSvNext.infogroup.com` folder by compressing the folder
14. Rename the zip file in the following format `IDMSvNext.infogroup.com_{DD}_{MM}_{YYYY}`
15. Move the above back up to the back up folder `D:\Backup\idmsvnext.infogroup.com`
16. Delete the oldest back-up out of the 11 back up as we only maintain the last 10 back ups in this folder.
17. After this stop the app poll and the website for idmsvnext.infogroup.com from the IIS console.
18. Go to Website folder and delete all the files from `idmsvnext.infogroup.com`folder except `appsettings.Staging.json`, `f5.htm`, `default_aws_profile` and `appconfig.json`.
19. Copy the latest artifacts from `WEB-PUBLISHED-CODE\idmsvnext.infogroup.com`  to `Websites\IDMSvNext.infogroup.com`
20. In case of any conflicts skip and use the existing files. It will be better to select `Skip these files`option.
21.  Once the artifacts are transferred successfully, repeat the steps `15` - `20` for the second server.
22.  Start the application pools and the website for both the servers.
23. Open [Campaign UI Stage](https://vnext.idms.data-axle.com/) on your desktop and test the most recent changes that were made.
