# Campaign UI Deployment - Production

## Website URL
https://campaign.idms.data-axle.com/

## Web Servers

 1. virdmswebp001w.dmz.infousa.com
 2. virdmswebp002w.dmz.infousa.com

## User Accounts

 1. IDMSDevSVC (idmsdevsvc@dmz.infousa.com)
 2. IDMSDevSVC (IUSA\IDMSDevSVC)
   
## Steps for Deployment
 
 1. Open Azure DevOps websites.
 2. Under it select [IDMS_NG](https://infogroupidms.visualstudio.com/IDMS_NG) project.
 3. Go to the pipeline section.
 4. Out of the four pipelines, select **IDMS_NG-WEBSITE-CI-PROD** pipeline.
 5. You will see the following actions in the action menu.
	  - Edit
	  - Run Pipeline
	  - Manage Security
	  - Rename/move
	  - Delete
6. Select Run Pipeline option.
7. This will trigger the CI pipeline.
8. Wait for 15-20 minutes for CI process to get completed.
9. CI on completion, trigger the CD process.
10. Move to Releases section in Azure DevOps.
11. Select `IDMS_NG-Website-CD-Prod` release.
12. Click on `Create Release`  blue colored button on the top right.
13. On the create release pop up, again click on `Create` button without changing any option.
14. This will trigger the CD process.
15. Upon completion the site will be deployed on virdmswebp001w server.
16. Now go to virdmswebp002w server.
17. Open the website folder and create a back-up of Campaign UI website.
18. Stop the app pool and the website.
19. Delete all the files from campaign.idms.data-axle.com folder expect `wwwroot\f5.htm`
20. Tranfer the website contents from the virdmswebp001w public website.
21. Start the app pool and the website.
22. Add `default_aws_profile` to the folder if not present already.
23. Go to [Prod Websdite](https://campaign.idms.data-axle.com/) to verify the changes.