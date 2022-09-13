-- ====================================================================      
-- Author: Sumit Shah
-- Description: Batch Task - Common S3 path where the batch reports will be uploaded 
-- and later downloaded when user clicks on the link in the report email
-- User Story - IDMS-953 NUA | App Refactor : Batch Task - Search and delete with file upload
-- =====================================================================

USE [DW_Admin];

INSERT INTO tblConfiguration 
	(
		DivisionID, 
		DatabaseID, 
		cDescription,  
		cItem, 
		cValue, 
		mValue, 
		iValue, 
		iIsActive, 
		iIsEncrypted, 
		dCreatedDate, 
		cCreatedBy
	)
VALUES (188, 0, 'BatchProcess', 'BatchReportCommonAWS', 's3://idms-7933-webfiles/Other/','',0,1,0,GETUTCDATE(),'jayesh.patel');
