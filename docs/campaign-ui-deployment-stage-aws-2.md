# Campaign UI Deployment - Staging (AWS-2)

## Website URL

https://stage.idms.data-axle.com/


## Servers

| Server Name  | IP Address  |
|--|--|
|idms-web-2  |192.168.2.64  |
|idms-redis-1  |192.168.2.249  |

## User Accounts

 1. idms-dev(EC2AMAZ-MITCFIM\idms-dev)

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
11. Once the release is done the website would be deployed on web-1.
12. Add `default_aws_profile` to the folder if not present already.
13. Open [Campaign UI Stage (AWS)](https://stage.idms.data-axle.com/) on your desktop and test the most recent changes that were made.
